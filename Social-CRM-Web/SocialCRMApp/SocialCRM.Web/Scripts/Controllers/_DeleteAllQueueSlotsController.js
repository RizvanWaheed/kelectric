CRMApp.controller('_DeleteAllQueueSlotsController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'PublisherService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, PublisherService, GlobalService, notify) {
    
    $scope.ConfirmationTitle = items.ConfirmationTitle;
    $scope.ConfirmationMessage = items.ConfirmationMessage;   
    $scope.CompanyId = items.CompanyId;
    $scope.hideButton = true;
    $scope.Save=function()
    {
        PublisherService.DeleteAllSlotsOfQueue({ "ProfileCompanyGroupId": items.ProfileCompanyGroupId, "CompanyId": items.CompanyId, "CreatorId": 0 }).then(function (resultSet) {
            if (resultSet != null) {
                console.log(resultSet);
                if (resultSet["statusText"] == "OK") {
                    //alert("Done");                    
                    $uibModalInstance.close('Activated');
                } else {
                    GlobalService.ErrorPopup("Error", "Error in deleting all slots, please try again.");
                }
            }            
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            alert('Error in fetching, please try again.');
            console.log(err);
        });
    }
    $scope.DeActivate = function () {             
    };
    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);