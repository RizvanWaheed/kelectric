CRMApp.controller('_InstagramReplyController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'FacebookService', 'InstagramService', 'GlobalService', 'notify', '$sce', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, FacebookService, InstagramService, GlobalService, notify, $sce) {
    $scope.VideoURL = "";
    $scope.commentsLength = {
        height: '300px',//350
        disableFadeOut: true,
        width: '100%'
    }
  
    $scope.IsPage = items.status.IsPage;
    $scope.ProfileExternalId = items.status.UserId;
    $scope.MessageType = items.status.MessageType;
    $scope.ofUser = items.status.replyedBy;
    $scope.PostAuthor = "";
    $scope.StatusId = items.status.StatusId;
    $scope.replyedTo = "";
    $scope.isRetweeted = false;
    //$scope.retweetId = 0;
    $scope.retweetedStatusId = 0;

    $scope.FormType = items.FormType

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

        $scope.GetTwitterProfilesByGroupId();

        if ($scope.FormType == "Reply") {
            $scope.IsEnable = true;
            $scope.FormText = "Reply"
        }

        $scope.MessageItself = "";
        $scope.MessagePostDateTime = items.status.PostDateTime;
        $scope.MessagePostUserImageUrl = "";
    };

    $scope.CheckProfile = function (profileId, UserName, IsPage, ProfileExternalId, ProfilePicURL) {
        // alert(profileId);
        $scope.replyedTo = UserName;
        $scope.IsPage = IsPage;
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

    $scope.SocialActivityExists = function () {
        /*UnifiedService.SocialActivityExists($scope.ProfileExternalId, $scope.IsPage, $scope.StatusId, $scope.MessageType, 2).then(function (l) {
            if (l.data != null) {
                $scope.isRetweeted = true;
            }
            else {
                $scope.isRetweeted = false;
            }

            if (l.data != null) {
                $scope.retweetId = l.data.Id;
                $scope.retweetedStatusId = l.data.RetweetedStatusId;
            }
        }, function (err) {
            // alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching Retweet Status Data, please try again.");

            console.log(err);
        });*/
        TwitterService.TwitterIsRetweeted($scope.replyedTo, $scope.StatusId).then(function (l) {
            if (l.data == "") {
                $scope.isRetweeted = false;
            }
            else {
                $scope.isRetweeted = true;
                $scope.retweetedStatusId = l.data;
            }
        }, function (err) {
            $scope.isRetweeted = false;
            //GlobalService.ErrorPopup("Error", err.data.Message);
        });
    }

    $scope.getHistory = function () {
        $scope.conversationHistory = [];
        $scope.inboxHistoryItemDetails = [];
        //Follower details
        if ($scope.MessageType == "IM") {
            //Instagram Posts
            InstagramService.GetPostDetails(items.status.StatusId).then(function (l) {
                var data = l.data;
                console.log(data);
                $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, data.CommentsList);
                $scope.totalConversation = 140;
                $scope.MessageItself = data.PostUrl;

                var splittedURL = data.PostUrl.split("?");
                var fileExtension = splittedURL[0].substr(splittedURL[0].length - 3);

                if (fileExtension == "jpg" || fileExtension == "png" || fileExtension == "eps") {
                    $scope.IsImage = true;
                } else {
                    $scope.IsImage = false;
                    $scope.VideoURL = splittedURL[0];
                }

                $scope.SelectedProfileId = $scope.attachedProfiles[0].Id;
                $scope.Caption = data.PostCaption;
                $scope.PostAuthorProfilePic = data.AuthorProfilePicture;
                $scope.PostAuthor = data.AuthorName;
                $scope.replyedMsgModel = "";

            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");
                console.log(err);
            });
        }
        else if ($scope.MessageType == "IC") {
            //Instagram Comments
            var tempDistribution = items.status.StatusId.split("|");
            InstagramService.GetPostDetails(tempDistribution[1]).then(function (l) {
                var data = l.data;
                console.log(data);
                $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, data.CommentsList);
                $scope.totalConversation = 140;
                $scope.MessageItself = data.PostUrl;

                var splittedURL = data.PostUrl.split("?");
                var fileExtension = splittedURL[0].substr(splittedURL[0].length - 3);

                if (fileExtension == "jpg" || fileExtension == "png" || fileExtension == "eps") {
                    $scope.IsImage = true;
                } else {
                    $scope.IsImage = false;
                    $scope.VideoURL = splittedURL[0];
                }

                $scope.SelectedProfileId = $scope.attachedProfiles[0].Id;
                $scope.Caption = data.PostCaption;
                $scope.PostAuthorProfilePic = data.AuthorProfilePicture;
                $scope.PostAuthor = data.AuthorName;
                $scope.replyedMsgModel = "";

            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");
                console.log(err);
            });

        }                
    };

    $scope.GetTwitterProfilesByGroupId = function () {
        //Follower details
        if ($scope.MessageType == "IM" || $scope.MessageType == "IC") {
            ProfileService.GetInstagramProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

                $.each(l.data, function (key, value) {
                    $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
                });

                if ($scope.attachedProfiles.length > 0) {
                    $scope.replyedTo = $scope.attachedProfiles[0].UserName;
                    $scope.IsPage = $scope.attachedProfiles[0].IsPage;
                    $scope.ProfileExternalId = $scope.attachedProfiles[0].ProfileExternalId;
                    $scope.MessagePostUserImageUrl = $scope.attachedProfiles[0].ProfilePicURL;                    
                }

                $scope.getHistory();
            }, function (err) {
                // alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

                console.log(err);
            });
        }        
    };    
    
    $scope.AddComment = function (commentText, isNested, commentId) {        
        console.log(commentId);

        if (isNested) {
            $scope.commentReply = commentText;
            //$scope.StatusId = $scope.StatusId.split('|')[0];
            $scope.StatusId = commentId;
            $scope.MessageType = "FC";
        } else {
            $scope.replyedMsgModel = commentText;
            $scope.StatusId = $scope.StatusId.split('|')[1];
            $scope.MessageType = "FP";
        }
        
        $scope.PostTweet();
    }    

    $scope.PostTweet = function () {

        if ($scope.MessageType == "IM") {
            //PostId,UserId,Comment
            InstagramService.PostComment($scope.StatusId, $scope.SelectedProfileId, $scope.replyedMsgModel,$rootScope.selectedCompanyId).then(function (l) {
                // alert("Posted successfully ")
                if(l.statusText == "OK"){
                    notify({
                        message: "Posted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                } else {
                    GlobalService.ErrorPopup("Error", "Error in posting comment.");
                }

                $uibModalInstance.close('cancel');
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
                //alert('Error in post tweet, please try again.');
                console.log(err);
            });
        } else if ($scope.MessageType == "IC") {
            var tempDistribution = items.status.StatusId.split("|");
            InstagramService.PostComment(tempDistribution[1], $scope.SelectedProfileId, encodeURIComponent($scope.replyedMsgModel),$rootScope.selectedCompanyId).then(function (l) {
                // alert("Posted successfully ")
                if (l.statusText == "OK") {
                    notify({
                        message: "Posted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                } else {
                    GlobalService.ErrorPopup("Error", "Error in posting comment.");
                }

                $uibModalInstance.close('cancel');
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
                //alert('Error in post tweet, please try again.');
                console.log(err);
            });
        }

    };
    $scope.init();

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


    $scope.IsHidden = true;    

    $scope.AddInstagramComment = function (commentText) {
        console.log(commentText);
        $scope.replyedMsgModel = commentText;
        $scope.StatusId = items.status.StatusId;
        $scope.PostTweet();
    }   
    
    $scope.getTimeDifference = function (PostTime) {
        var current = new Date();
        var previous = new Date(PostTime);

        var msPerMinute = 60 * 1000;
        var msPerHour = msPerMinute * 60;
        var msPerDay = msPerHour * 24;
        var msPerMonth = msPerDay * 30;
        var msPerYear = msPerDay * 365;

        var elapsed = current - previous;

        if (elapsed < msPerMinute) {
             return 'few seconds ago';   
        }

        else if (elapsed < msPerHour) {
             return Math.round(elapsed/msPerMinute) + ' minutes ago';   
        }

        else if (elapsed < msPerDay ) {
             return Math.round(elapsed/msPerHour ) + ' hours ago';   
        }

        else if (elapsed < msPerMonth) {
            return 'around ' + Math.round(elapsed/msPerDay) + ' days ago';   
        }

        else if (elapsed < msPerYear) {
            return 'around ' + Math.round(elapsed/msPerMonth) + ' months ago';   
        }

        else {
            return 'approximately ' + Math.round(elapsed/msPerYear ) + ' years ago';   
        }
    }

    $scope.getVideoURL = function () {
        return $sce.trustAsResourceUrl($scope.VideoURL);
    }

}]);