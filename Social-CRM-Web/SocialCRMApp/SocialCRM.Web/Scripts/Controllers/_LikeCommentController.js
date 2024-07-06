CRMApp.controller('_LikeCommentController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'FacebookService', 'UnifiedService', 'items', 'TwitterService', 'InstagramService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, FacebookService, UnifiedService, items, TwitterService, InstagramService, GlobalService, notify) {
    
    /*$scope.IsPage = items.IsPage;
    $scope.UserId = items.UserId;
    $scope.ofUser = items.replyedBy;
    $scope.replyedMsgModel = '@' + items.replyedBy;
    $scope.replyedTo = "";*/
    $scope.StatusId = items.StatusId;
    $scope.MessageType = items.MessageType;
    if ($scope.MessageType == "FC") {
        $scope.StatusId = $scope.StatusId.split('|')[0];
    }
    $scope.attachedProfiles = [];
    $scope.isLiked = false;
    //$scope.likeId = 0;

    $scope.CheckProfile = function (profileId, UserName , UserId, IsPage) {
        // alert(profileId);
        $scope.replyedTo = UserName;
        $scope.UserId = UserId;
        $scope.IsPage = IsPage;

        $scope.SocialActivityExists();
        $.each($scope.attachedProfiles, function (key, value) {
            if (value.Id != profileId) {
                $("#profile-" + value.Id).prop('checked', false);
            }
        });
    }

    $scope.SocialActivityExists = function () {
        /*UnifiedService.SocialActivityExists($scope.UserId, $scope.IsPage, $scope.StatusId, $scope.MessageType, 1).then(function (l) {
            if (l.data != null) {
                $scope.isLiked = true;
            }
            else {
                $scope.isLiked = false;
            }

            if (l.data != null) {
                $scope.likeId = l.data.Id;
            }
        }, function (err) {
            // alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching Like Status Data, please try again.");

            console.log(err);
        });*/
        if ($scope.MessageType == "FC" || $scope.MessageType == "FP") {
            FacebookService.FacebookIsPostLiked($scope.UserId, $scope.IsPage, $scope.StatusId).then(function (l) {
                $scope.isLiked = l.data.Message;
            }, function (err) {
                $scope.isLiked = false;
                //GlobalService.ErrorPopup("Error", err.data.Message);
            });
        }
        else if ($scope.MessageType == "IM") {
            InstagramService.IsPostLiked($scope.StatusId, $scope.UserId).then(function (l) {
                $scope.isLiked = l.data;
            }, function (err) {
                $scope.isLiked = false;
                //GlobalService.ErrorPopup("Error", err.data.Message);
            });
        }
        else {
            TwitterService.TwitterIsFavorited($scope.replyedTo, $scope.StatusId).then(function (l) {
                $scope.isLiked = l.data;
            }, function (err) {
                $scope.isLiked = false;
                //GlobalService.ErrorPopup("Error", err.data.Message);
            });
        }
    }

    $scope.GetTwitterProfilesByGroupId = function () {
        //Follower details
        if ($scope.MessageType == "IM") {
            ProfileService.GetInstagramProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

                $.each(l.data, function (key, value) {
                    $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
                });

                if ($scope.attachedProfiles.length > 0) {
                    $scope.IsPage = $scope.attachedProfiles[0].IsPage;
                    $scope.UserId = $scope.attachedProfiles[0].ProfileExternalId;
                    $scope.replyedTo = $scope.attachedProfiles[0].UserName;

                    $scope.SocialActivityExists();
                }
            }, function (err) {
               // alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching instagram profile Data, please try again.");

                console.log(err);
            });
        }
        else if ($scope.MessageType == "FC" || $scope.MessageType == "FP") {
            ProfileService.GetFacebookProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

                $.each(l.data, function (key, value) {
                    $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
                });

                if ($scope.attachedProfiles.length > 0) {
                    $scope.IsPage = $scope.attachedProfiles[0].IsPage;
                    $scope.UserId = $scope.attachedProfiles[0].ProfileExternalId;
                    $scope.replyedTo = $scope.attachedProfiles[0].UserName;

                    $scope.SocialActivityExists();
                }
            }, function (err) {
               // alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching facebook profile Data, please try again.");

                console.log(err);
            });
        }
        else {
            ProfileService.GetTwitterProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

                $.each(l.data, function (key, value) {
                    $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
                });

                if ($scope.attachedProfiles.length > 0) {
                    $scope.IsPage = $scope.attachedProfiles[0].IsPage;
                    $scope.UserId = $scope.attachedProfiles[0].ProfileExternalId;
                    $scope.replyedTo = $scope.attachedProfiles[0].UserName;

                    $scope.SocialActivityExists();
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

            });
        }
    };
    $scope.LikeObject = function () {
        if ($scope.MessageType == "FC" || $scope.MessageType == "FP") {
            FacebookService.FacebookLikePost($scope.UserId, $scope.IsPage, $scope.StatusId).then(function (l) {
                //$scope.likeId = l.data.Message;
                $scope.isLiked = true;

                //  alert("FB Post liked successfully ")
                notify({
                    message: "FB liked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);

            });
        }
        else if ($scope.MessageType == "IM") {
            InstagramService.LikePost($scope.StatusId, $scope.UserId).then(function (l) {
                //$scope.likeId = l.data;
                $scope.isLiked = true;

              //  alert("FB Post liked successfully ")
                notify({
                    message: "Instagram liked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);

            });
        }
        else {
            TwitterService.TwitterMarkFavorite($scope.replyedTo, $scope.StatusId).then(function (l) {
                //$scope.likeId = l.data;
                $scope.isLiked = true;

              //  alert("Tweet liked successfully ")
                notify({
                    message: "Tweet liked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);

                //alert('Error in post tweet, please try again.');
                //console.log(err);
            });
        }
    };

    $scope.UnLikeObject = function () {
        if ($scope.MessageType == "FC" || $scope.MessageType == "FP") {
            FacebookService.FacebookUnLikePost($scope.UserId, $scope.IsPage, $scope.StatusId).then(function (l) {
                //$scope.likeId = 0;
                $scope.isLiked = false;

                //  alert("FB Post liked successfully ")
                notify({
                    message: "FB unliked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);

            });
        }
        else if ($scope.MessageType == "IM"){
            InstagramService.UnlikePost($scope.StatusId, $scope.UserId).then(function (l) {
                //$scope.likeId = 0;
                $scope.isLiked = false;

                //  alert("Tweet liked successfully ")
                notify({
                    message: "Instagram unliked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);

                //alert('Error in post tweet, please try again.');
                //console.log(err);
            });
        }
        else {
            TwitterService.TwitterUnmarkFavorite($scope.replyedTo, $scope.StatusId).then(function (l) {
                //$scope.likeId = 0;
                $scope.isLiked = false;

                //  alert("Tweet liked successfully ")
                notify({
                    message: "Tweet unliked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);

                //alert('Error in post tweet, please try again.');
                //console.log(err);
            });
        }
    };
    $scope.GetTwitterProfilesByGroupId();
   // $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);