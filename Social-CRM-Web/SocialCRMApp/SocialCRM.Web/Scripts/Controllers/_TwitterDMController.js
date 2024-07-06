CRMApp.controller('_TwitterDMController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'FacebookService', 'GlobalService','notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, FacebookService, GlobalService, notify) {

    $scope.UserId = items.UserId;
    $scope.FSObj = items.FSObj;
    $scope.attachedProfiles = [];

    $scope.init = function () {
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.HeadColorCount = 0;
        $scope.DetailColorCount = 0;
        $scope.ErrorTaskClassification = "";
        $scope.ErrorTaskPriority = "";
        $scope.ErrorTaskAssignee = "";
        $scope.ErrorTaskStatus = "";


        $scope.totalConversation = 127;
        $scope.conversationHistory = [

        ];
        $scope.inboxHistoryItemDetails = [];


        $scope.IsEnable = true;
        $scope.FormText = "Direct Message"
        $scope.getHistory();

        $scope.GetTwitterProfilesByGroupId();

    };

    $scope.CheckProfile = function (profileId, UserName) {
        // alert(profileId);
        $scope.replyedTo = UserName;
        $.each($scope.attachedProfiles, function (key, value) {
            if (value.Id != profileId) {
                $("#profile-" + value.Id).prop('checked', false);
            }
            //profiledata.push({ profileId: value.ProfileId, ProfileType: value.ProfileTypeId, GroupId: $scope.GroupId });
        });
    }

    $scope.getHistory = function () {
        UnifiedService.getTwitterHistory("1", $scope.UserId, 500, 0, $rootScope.selectedGroupId, $rootScope.selectedCompanyId).then(function (l) {
            $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, l.data.tweets);

            console.log("Contact History Tweets fetched Successfully");
        }, function (err) {
            //alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

            console.log(err);
        });
    };

    $scope.GetTwitterProfilesByGroupId = function () {

        ProfileService.GetTwitterProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

            $.each(l.data, function (key, value) {
                $.each($scope.FSObj, function (key, valueFS) {
                    if (valueFS.Id == value.Id)
                        $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ischecked: key == 0 ? true : false });
                });
            });

            $scope.ofUser = $scope.UserId;
            if ($scope.FSObj.length > 0)
                $scope.replyedTo = $scope.FSObj[0].UserName;
        }, function (err) {
           // alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

            console.log(err);
        });

    };

    $scope.PostTweet = function () {

        TwitterService.TwitterSendDirectMessage($scope.replyedTo, $scope.ofUser, $scope.replyedMsgModel).then(function (l) {

          //  alert("Direct Message posted successfully ")
            notify({
                message: "Direct Message posted successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close('cancel');
        }, function (err) {
           // alert('Error in post tweet, please try again.');
            GlobalService.ErrorPopup("Error", err.data.Message);

            console.log(err);
        });


    };
    $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);