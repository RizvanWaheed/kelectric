CRMApp.controller('_RetweetController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'GlobalService','notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, GlobalService, notify) {


    $scope.ofUser = items.replyedBy;
    $scope.replyedMsgModel = '@' + items.replyedBy;
    $scope.StatusId = items.StatusId;
    $scope.replyedTo = "";
    $scope.FormType = items.FormType
    if ($scope.FormType == "Retweet") {
        $scope.FormText = "RETWEET";
        $scope.FormDescription = "Are you sure you want to Retweet this Message?";
        $scope.IsEnable = false;
    }
    else {
        $scope.FormText = "RETWEET WITH COMMENT";
        $scope.FormDescription = "Are you sure you want to Retweet this Message with a Comment?";
        $scope.IsEnable = true;
    }
    $scope.PostTweet = function () {

        TwitterService.TwitterReplyTweet($scope.replyedTo, $scope.StatusId, $scope.replyedMsgModel, null,$rootScope.selectedCompanyId).then(function (l) {

          //  alert("Tweet posted successfully ")
            notify({
                message: "Tweet posted successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            //alert('Error in post tweet, please try again.');
           // console.log(err);
        });
    };
   // $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    
}]);