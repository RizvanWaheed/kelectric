CRMApp.controller('_FbPageChatController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'FacebookService', 'InstagramService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, FacebookService, InstagramService, GlobalService, notify) {
    
    $scope.commentsLength = {
        height: '380px',//350
        disableFadeOut: true
    }

    $scope.IsPage = true;
    $scope.IsReceivedPage = items.IsPage;
    $scope.UserId = items.UserId;
    $scope.MessageType = items.MessageType;
    $scope.ofUser = items.replyedBy;
    $scope.PostAuthor = items.replyedBy;
    $scope.replyedMsgModel = "";
    $scope.StatusId = items.StatusId.split('|')[1];
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

        $scope.totalConversation = 140;
        $scope.conversationHistory = [

        ];
        $scope.inboxHistoryItemDetails = [];

        $scope.IsEnable = true;
        $scope.FormText = "Reply"

        $scope.getHistory();

        $scope.GetTwitterProfilesByGroupId();

        $scope.MessageItself = "";
        $scope.MessagePostDateTime = items.PostDateTime;
        $scope.MessagePostUserImageUrl = "";
    };

    $scope.CheckProfile = function (profileId, UserName, IsPage, ProfileExternalId, ProfilePicURL) {
        // alert(profileId);
        $scope.replyedTo = UserName;
        //$scope.IsPage = IsPage;
        $scope.ProfileExternalId = ProfileExternalId;
        $scope.MessagePostUserImageUrl = ProfilePicURL;
        $scope.SelectedProfileId = profileId;

        $.each($scope.attachedProfiles, function (key, value) {
            if (value.Id != profileId) {
                $("#profile-" + value.Id).prop('checked', false);
            }
            //profiledata.push({ profileId: value.ProfileId, ProfileType: value.ProfileTypeId, GroupId: $scope.GroupId });
        });
    }

    $scope.getHistory = function () {
        //Follower details
        
        FacebookService.FacebookGetPageChatConversation($scope.StatusId).then(function (l) {
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
        ProfileService.GetFacebookProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

            $.each(l.data, function (key, value) {
                $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
            });

            $scope.MessagePostUserImageUrl = $scope.attachedProfiles.filter(function (i) { return i.ProfileExternalId == $scope.UserId })[0].ProfilePicURL;
            $scope.replyedTo = $scope.attachedProfiles.filter(function (i) { return i.ProfileExternalId == $scope.UserId })[0].UserName;
            $scope.ProfileExternalId = $scope.attachedProfiles.filter(function (i) { return i.ProfileExternalId == $scope.UserId })[0].ProfileExternalId;
            //if ($scope.attachedProfiles.length > 0) {
            //    $scope.replyedTo = $scope.attachedProfiles[0].UserName;
            //    //$scope.IsPage = $scope.attachedProfiles[0].IsPage;
            //    $scope.ProfileExternalId = $scope.attachedProfiles[0].ProfileExternalId;
            //    $scope.MessagePostUserImageUrl = $scope.attachedProfiles[0].ProfilePicURL;
            //}
        }, function (err) {
            
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

            console.log(err);
        });
    };    

    $scope.PostMessage = function () {
        $scope.ProfileExternalId = $scope.UserId;
        FacebookService.FacebookPostPageChatMessage($scope.ProfileExternalId, $scope.StatusId, $scope.IsPage, $scope.replyedMsgModel, $rootScope.selectedCompanyId).then(function (l) {
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