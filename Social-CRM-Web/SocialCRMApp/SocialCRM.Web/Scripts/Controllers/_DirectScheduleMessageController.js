CRMApp.controller('_DirectScheduleMessageController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'PublisherService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, PublisherService, GlobalService, notify) {
    
    $scope.ConfirmationTitle = items.ConfirmationTitle;
    $scope.ConfirmationMessage = items.ConfirmationMessage;
   // $scope.msg = items.msg;
    $scope.CompanyId = items.CompanyId;
    $scope.hideButton = true;
    $scope.Save=function()
    {
        PublisherService.AddNewScheduledMessage(items.Message, items.SelectedProfileIds, items.PostTimes, items.IsDraft, items.IsScheduledMessage, items.IsPreConfiguredQueue, items.IsQueueAtBeginning, items.CreatorUserId, items.CompanyId, items.MessageStatus, items.ScheduledMessageId, items.MediaInfo).then(function (resultSet) {
            if (resultSet != null) {
                console.log(resultSet);
                if (resultSet["statusText"] == "OK") {
                    //$scope.messageBodyArea = "";
                    //alert("Message Scheduled");
                    if (items.ScheduleType == "ScheduleOnly" || items.ScheduleType == "Approve") {
                        notify({
                            message: "Message Scheduled.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                    } else if (items.ScheduleType == "Save") {
                        notify({
                            message: "Message saved to Drafts.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                    }
                } else {
                    GlobalService.ErrorPopup("Error", "Error in scheduling, please try again.");
                }
            }
            $uibModalInstance.close('Activated');
            //alert("defaultfully fetched");
        }, function (err) {
            //alert('Error in fetching, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            console.log(err);
        });
        
    }
    $scope.DeActivate = function () {             
    };
    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);