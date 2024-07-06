CRMApp.controller('_ProfileConversationController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, GlobalService, notify) {


    $scope.ofUser = items.replyedBy;
    $scope.replyedMsgModel = '@' + items.replyedBy;
    $scope.StatusId = items.StatusId;
    $scope.replyedTo = "";
    $scope.MessageType = items.MessageType;
    $scope.init = function () {
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.slim = {
            height: '450px',
            disableFadeOut: true
        }
        $scope.conversationHistory = [];
        $scope.inboxHistoryItemDetails = [];
        $scope.getConversation();
    };

    $scope.getConversation = function () {
        //Follower details
        TwitterService.GetTwitterConversation("1", $scope.ofUser, 250, 0, $rootScope.selectedGroupId, $rootScope.selectedCompanyId, $scope.StatusId, $scope.MessageType).then(function (l) {
            $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, l.data.tweets);
            // $scope.currentCursor = l.data.maxId;
            //if (l.data.tweets.length == 0) {
            //    $scope.currentEndReached = true;
            //}
            //console.log("Contact History Tweets fetched Successfully");
        }, function (err) {
          //  alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

            //console.log(err);
        });
    };

    //$scope.CheckProfile = function (profileId, UserName) {
    //    // alert(profileId);
    //    $scope.replyedTo = UserName;
    //    $.each($scope.attachedProfiles, function (key, value) {
    //        if (value.Id != profileId) {
    //            $("#profile-" + value.Id).prop('checked', false);
    //        }
    //        //profiledata.push({ profileId: value.ProfileId, ProfileType: value.ProfileTypeId, GroupId: $scope.GroupId });
    //    });
    //}

    //$scope.getHistory = function () {
    //    //Follower details
    //    UnifiedService.getTwitterHistory("1", $scope.ofUser, 250, 0, $window.sessionStorage.selectedGroupId, $window.sessionStorage.selectedCompanyId).then(function (l) {
    //        $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, l.data.tweets);
    //        // $scope.currentCursor = l.data.maxId;
    //        //if (l.data.tweets.length == 0) {
    //        //    $scope.currentEndReached = true;
    //        //}
    //        console.log("Contact History Tweets fetched Successfully");
    //    }, function (err) {
    //        alert('Error in fetching contact history tweets, please try again.');
    //        console.log(err);
    //    });
    //};

    //$scope.GetTwitterProfilesByGroupId = function () {
    //    //Follower details
    //    ProfileService.GetTwitterProfilesByGroupId($window.sessionStorage.selectedGroupId, $window.sessionStorage.selectedCompanyId).then(function (l) {
    //      //  $scope.attachedProfiles = l.data;
    //        // $("#profile-" + l.data[0].Id).prop('checked', true);
    //        $.each(l.data, function (key, value) {
    //            $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ischecked: key == 0 ? true : false });
    //        });
    //        $scope.replyedTo = l.data[0].UserName;
    //        // console.log("Contact History Tweets fetched Successfully");
    //    }, function (err) {
    //        alert('Error in fetching contact history tweets, please try again.');
    //        console.log(err);
    //    });
    //};

    //$scope.PostTweet = function () {

    //    TwitterService.TwitterReplyTweet($scope.replyedTo, $scope.StatusId, $scope.replyedMsgModel).then(function (l) {

    //        alert("profile has been removed ")
    //    }, function (err) {
    //        alert('Error in remove profile, please try again.');
    //        console.log(err);
    //    });
    //};
    $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);