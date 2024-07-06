CRMApp.controller('CompanyAccountActivation', ['$rootScope', '$scope', '$window', '$location', 'CompanyService', 'GlobalService', 'AuthService', 'notify', function ($rootScope, $scope, $window, $location, CompanyService, GlobalService, AuthService, notify) {

    $scope.CompanyName = 'ibex global';
    CompanyService.GetCompanyName().then(function (l) {
        $scope.CompanyName = l.data.CompanyName;
        $scope.CompanyId = l.data.CompanyId;

    }, function (err) {
        //alert('Error in fetching contact history tweets, please try again.');
        GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");
        console.log(err);
    });

    $scope.Activate=function()
    {
        CompanyService.ActivateCompany($scope.CompanyId).then(function (l) {
            //alert("Successfully Activated ");
            notify({
                message: "Sucessfully Activated",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $window.location.href = "#/Login";
        }, function (err) {
            //alert('Error in Activation, please try again.');
            GlobalService.ErrorPopup("Error", "Error in Activation, please try again.");

            console.log(err);
        });
    }
    $scope.DeActivate = function () {
        CompanyService.DeActivateCompany($scope.CompanyId,true).then(function (l) {
           // alert("Successfully Deactivated ");
            notify({
                message: "Successfully Deactivated",
                position: GlobalService.position,
                duration: GlobalService.duration
            });

            AuthService.LogOut().then(function (l) {
                //alert("group has been added")
                $rootScope.selectedGroupId = null;
                $rootScope.selectedCompanyId = null;
                $rootScope.selectedCompanyName = null;
                $rootScope.currentUser = null;
                $rootScope.IsAdmin = null;
                $rootScope.IsPersonal = null;
                $window.location.href = "#/Login";
            }, function (err) {
             //   alert('Error in Logging out, please try again.');
                GlobalService.ErrorPopup("Error", "Error in Logging out, please try again.");
                console.log(err);
            });
        }, function (err) {
           // alert('Error in DeActivation, please try again.');
            GlobalService.ErrorPopup("Error", "Error in Deactivation, please try again.");

            console.log(err);
        });
    }
}]);