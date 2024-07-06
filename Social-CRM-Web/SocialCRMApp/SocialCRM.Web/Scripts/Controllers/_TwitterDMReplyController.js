CRMApp.controller('_TwitterDMReplyController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'FacebookService', 'InstagramService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, FacebookService, InstagramService, GlobalService, notify) {

    $scope.commentsLength = {
        height: '380px',//350
        disableFadeOut: true
    }

    $scope.IsPage = false;
    $scope.IsReceivedPage = items.IsPage;
    $scope.UserId = items.UserId;
    $scope.MessageType = items.MessageType;
    $scope.ofUser = items.replyedBy;
    $scope.PostAuthor = items.replyedBy;
    $scope.replyedMsgModel = "";
    $scope.StatusId = items.StatusId;
    $scope.ToUser = items.ToUser;
    $scope.FeedId = items.Id;

    $scope.attachedProfiles = [];

    $scope.init = function () {
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.HeadColorCount = 0;
        $scope.DetailColorCount = 0;
        $scope.ErrorTaskClassification = "";
        $scope.ErrorTaskPriority = "";
        $scope.ErrorTaskAssignee = "";
        $scope.ErrorTaskStatus = "";

        $scope.totalConversation = 280;//140;
        $scope.conversationHistory = [

        ];
        $scope.inboxHistoryItemDetails = [];

        $scope.IsEnable = true;
        $scope.FormText = "Direct Message"

        $scope.getHistory();

        $scope.GetTwitterProfilesByGroupId();

        $scope.MessageItself = "";
        $scope.MessagePostDateTime = items.PostDateTime;
        $scope.MessagePostUserImageUrl = "";
    };

    //$scope.CheckProfile = function (profileId, UserName, IsPage, ProfileExternalId, ProfilePicURL) {
    //    // alert(profileId);
    //    $scope.replyedTo = UserName;
    //    //$scope.IsPage = IsPage;
    //    $scope.ProfileExternalId = ProfileExternalId;
    //    $scope.MessagePostUserImageUrl = ProfilePicURL;
    //    $scope.SelectedProfileId = profileId;

    //    $.each($scope.attachedProfiles, function (key, value) {
    //        if (value.Id != profileId) {
    //            $("#profile-" + value.Id).prop('checked', false);
    //        }
    //        //profiledata.push({ profileId: value.ProfileId, ProfileType: value.ProfileTypeId, GroupId: $scope.GroupId });
    //    });
    //}

    $scope.getHistory = function () {
        TwitterService.GetTwitterDirectMessages($scope.ToUser, $scope.ofUser).then(function (l) {
            $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, l.data);
            console.log("Conversations fetched Successfully");
        }, function (err) {
            //alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");
            console.log(err);
        });
    };

    $scope.GetTwitterProfilesByGroupId = function () {
        //Follower details        
       
        ProfileService.GetTwitterProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

            $.each(l.data, function (key, value) {
                $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
            });
            $scope.MessagePostUserImageUrl = $scope.attachedProfiles.filter(function (i) { return i.ProfileExternalId == $scope.UserId })[0].ProfilePicURL;
            //$scope.ofUser = $scope.UserId;
            //if ($scope.FSObj.length > 0)
            //    $scope.replyedTo = $scope.FSObj[0].UserName;
        }, function (err) {
            // alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

            console.log(err);
        });
    };

    $scope.PostMessage = function () {
        $scope.ProfileExternalId = $scope.UserId;
        //$scope.ToUser, $scope.ofUser
        TwitterService.TwitterSendDM($scope.ProfileExternalId, $scope.ToUser, $scope.ofUser, $scope.replyedMsgModel,$rootScope.selectedCompanyId).then(function (l) {
            notify({
                message: "Message sent successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);

            console.log(err);
        });
    };
    $scope.init();

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);