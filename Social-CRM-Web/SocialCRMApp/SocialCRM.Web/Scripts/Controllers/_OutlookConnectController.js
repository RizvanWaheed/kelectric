CRMApp.controller('_OutlookConnectController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'items', 'ProfileService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, items, ProfileService, GlobalService, notify) {
    $scope.GroupId = items.groupid; //$location.search().groupid;
    $scope.CompanyId = items.companyid; //$location.search().companyid;
    $scope.IsPersonal = $rootScope.IsPersonal.toString();

    $scope.Save = function () {
       // $scope.RegexFor = $scope.RegexFor.trim();
        //var isValid = true;
        //if ($scope.RegexFor.indexOf(' ') >= 0) {
        //    var inputs = $scope.RegexFor.split(' ')
        //    for (var i = 1; i < inputs.length; i++) {
        //        if (inputs[i].length != inputs[0].length)
        //            isValid = false;
        //    }
        //}
       // if (isValid) {
        ProfileService.AddOutlookAccount($scope.GroupId, $scope.CompanyId, $scope.IsPersonal, $scope.UserID, $scope.Passwrd, $scope.Email, $scope.Domain).then(function (l) {
        // $window.location.href = '/Profile/ConnectOutlook/' + $scope.CompanyId + '/' + $scope.GroupId;
                if (l.data != null && l.data != -1) {
                    notify({
                        message: "Outlook Account saved successfully.",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                } else {
                    GlobalService.ErrorPopup("Error", "Error in adding Outlook Profile. Check your configuration data and network connection and try again.");
                }
                $scope.cancel();
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in adding Outlook Profile. Check your configuration data and network connection and try again.");

                console.log(err);
            });
        //}
        //else {
        //    GlobalService.ErrorPopup("Error", "Not a valid pattern.");
        //}
    };
    
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
   
}]);