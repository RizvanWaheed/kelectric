CRMApp.controller('SignupBusinessNameController', ['$scope', '$window', '$location', 'AuthService', 'CompanyService', 'GlobalService', 'notify', function ($scope, $window, $location, AuthService, CompanyService, GlobalService, notify) {
    $scope.Email = $location.search().param;
    if ($scope.Email == undefined)
        $window.location.href = "/#/Login";

    AuthService.UserProfile($scope.Email, 0).then(function (l) {
        if (l != null) {
            $scope.CompanyId = l.data.CompanyId;
        }
        else {
            // alert('No Data');y
        }
    }, function (err) {
      //  alert('Error in fetching data, please try again.');
        GlobalService.ErrorPopup("Error", "Error in fetching user data, please try again.");
        console.log(err);
    });
    
    $scope.Submit = function () {
        var CompanyId = $scope.CompanyId;
        var CompanyName = $scope.BName;
        var UserEmail=$scope.Email;
        var CompanyGroups=[];
        CompanyService.update(CompanyId, CompanyName, UserEmail, CompanyGroups).then(function (l) {
            if (l != null) {
                var usr = l;
                //  $scope.$parent.setCurrentUser(usr);
               // alert("Business Name has been set");
                notify({
                    message: "Business Name has been set.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                //Do processing on code submission
                $location.path("/Signup_Twitter").search({ param: $scope.Email });
            }
            else {
               // alert('Error Creating Business Name');
                GlobalService.ErrorPopup("Error", "Error Creating Business Name, please try again.");

            }
        }, function (err) {
//alert('Error Creating Business Name');
            GlobalService.ErrorPopup("Error", "Error Creating Business Name, please try again.");
            console.log(err);
        });
        
    }

    
}]);


