CRMApp.controller('_ApproveQueuedMessageController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'PublisherService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, PublisherService, GlobalService, notify) {
    
    $scope.ConfirmationTitle = items.ConfirmationTitle;
    $scope.ConfirmationMessage = items.ConfirmationMessage;
   // $scope.msg = items.msg;
    $scope.CompanyId = items.CompanyId;
    $scope.hideButton = true;
    $scope.Save=function()
    {
        PublisherService.ScheduleQueueMessage(items.Message, items.SelectedProfileIds, items.SelectedOption, items.CompanyId, items.MediaInfo).then(function (resultSet) {
            if (resultSet != null) {
                console.log(resultSet);
                debugger
                if (resultSet.data.Result == false) {
                    GlobalService.ErrorPopup("Error", "One or more profiles do not have any queue, kindly create queue slots and try again.");
                    $uibModalInstance.dismiss('cancel');
                } else {
                    if (resultSet["statusText"] == "OK") {
                        //alert("Done");                    
                        $uibModalInstance.close('Activated');
                    } else {
                        GlobalService.ErrorPopup("Error", "Error in queuing, please try again.");
                    }
                }
            }
            //alert("defaultfully fetched");
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            //alert('Error in fetching, please try again.');
            console.log(err);
        });
    }
    $scope.DeActivate = function () {             
    };
    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);

//comment