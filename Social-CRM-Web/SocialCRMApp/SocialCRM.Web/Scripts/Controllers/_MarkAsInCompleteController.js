CRMApp.controller('_MarkAsInCompleteController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'TaskService', 'GlobalService','notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, TaskService, GlobalService, notify) {
    
    $scope.ConfirmationTitle = items.ConfirmationTitle;
    $scope.ConfirmationMessage = items.ConfirmationMessage;
    $scope.msg = items.msg;
    $scope.hideButton = true;
    $scope.Save=function()
    {
        TaskService.DeleteTaskCompleted($scope.msg.Id, $scope.msg.MessageType, $rootScope.selectedGroupId).then(function (l) {
            //obj.IsArchived = false;
           // $scope.inboxitemDetails[index].IsTaskCompleted = true;
            //alert("Successfully Mark as InCompleted");
           // console.log("Successfully Mark as Completed");
            notify({
                message: "Successfully Marked as InCompleted",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close('OK');
           
        }, function (err) {
           // alert('Error, please try again.' + JSON.stringify(err));
            GlobalService.ErrorPopup("Error", "Error in marking as incomplete, please try again.");

            $uibModalInstance.dismiss('cancel');
            console.log(err);
        });
    }
    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);