CRMApp.controller('_ActivateCompanyAccountController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'CompanyService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, CompanyService, GlobalService, notify) {
    
    $scope.ConfirmationTitle = items.ConfirmationTitle;
    $scope.ConfirmationMessage = items.ConfirmationMessage;
   // $scope.msg = items.msg;
    $scope.CompanyId = items.CompanyId;
    $scope.hideButton = false;
    $scope.Save=function()
    {
        CompanyService.ActivateCompany($scope.CompanyId).then(function (l) {
            notify({
                message: "Successfully Activated",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            //alert("Successfully Activated ");
            $uibModalInstance.close('Activated');
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in Activation, please try again.");

        });     
    }
    $scope.DeActivate = function () {
        CompanyService.DeActivateCompany($scope.CompanyId,false).then(function (l) {
            notify({
                message: "Successfully Deactivated",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            //alert("Successfully Deactivated ");
            $uibModalInstance.close('Deactivated');
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in Deactivation, please try again.");

        });
      
    };
    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);