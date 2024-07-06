CRMApp.controller('_TwitterReplyController', ['$rootScope', '$scope', '$sce', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'FacebookService', 'InstagramService', 'GlobalService', 'notify', 'FileUploader', function ($rootScope, $scope, $sce, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, FacebookService, InstagramService, GlobalService, notify, FileUploader) {
    $scope.commentsLength = {
        height: '150px',//350
        disableFadeOut: true
    }

    $scope.InstaComments = {
        height: '250px',
        disableFadeOut: true
    }

    $scope.UploadedFiles = new Array();
    var uploader = $scope.uploader = new FileUploader({
        url: ''
    });


    uploader.filters.push({
        name: 'customFilter',
        fn: function (item, options) {
            return this.queue.length < 10;
        }
    });

    uploader.removeFromQueue = function (e) {
        var t = this.getIndexOfItem(e), o = this.queue[t];
        o.isUploading && o.cancel(), this.queue.splice(t, 1), o._destroy(), this.progress = this._getTotalProgress();
        $scope.UploadedFiles = this.queue;
        $scope.ValidateFiles();
    };

    uploader.onAfterAddingAll = function (addedFileItems) {
        $scope.UploadedFiles = this.queue;
        $scope.ValidateFiles();
    };

    $scope.notValidated = true;
    $scope.errorMsg = "";
    $scope.PageName = items.status.PageName;
    $scope.IsPage = items.status.IsPage;
    $scope.ProfileExternalId = items.status.UserId;
    $scope.AuthorUserId = items.status.UserId;
    $scope.AuthorIsPage = items.status.IsPage;
    $scope.MessageType = items.status.MessageType;
    $scope.ofUser = items.status.ReplyedBy;
    $scope.FeedId = items.status.Id;
    $scope.PostAuthor = "";
    if ($scope.MessageType == "FP" || $scope.MessageType == "FC") {
        $scope.replyedMsgModel = "";
        $scope.commentReply = "";
    }
    else
    $scope.replyedMsgModel = '@' + items.status.ReplyedBy;

    $scope.StatusId = items.status.StatusId;

    //Cause it takes time to set the profiles first and replies are called instantly otherwise it would be empty
    $scope.replyedTo = items.status.ReplyedBy;
    $scope.isRetweeted = false;
    //$scope.retweetId = 0;
    $scope.retweetedStatusId = 0;

    $scope.FormType = items.FormType

    $scope.attachedProfiles = [];

    $scope.openSlider = function (obj) {

        $scope.items = { Images: obj };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ImageSlider.html',
            controller: '_ImageSliderController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        }, function () {

        });
    }

    $scope.TextToURLConvert = function (text) {
        var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
        var text1 = text.replace(exp, "<a target='_blank' href='$1'>$1</a>");
        var exp2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;

        var str = text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>');
        var pattern = /\B@[a-z0-9_-]+/gi; //@users
        var users = str.match(pattern);
        if (users != null) {
            for (i = 0; i < users.length; i++) {
                str = str.replace(users[i], "<a target='_blank' href='//twitter.com/" + users[i].split('@')[1] + "'>" + users[i] + "</a>");
            }
        }

        return str;
    }

    $scope.trustSrc = function (src) {
        return $sce.trustAsResourceUrl(src);
    }

    $scope.init = function () {
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.HeadColorCount = 0;
        $scope.DetailColorCount = 0;
        $scope.ErrorTaskClassification = "";
        $scope.ErrorTaskPriority = "";
        $scope.ErrorTaskAssignee = "";
        $scope.ErrorTaskStatus = "";

        $scope.totalConversation = 280;
        $scope.conversationHistory = [

        ];
        $scope.inboxHistoryItemDetails = [];

        $scope.GetTwitterProfilesByGroupId();

        if ($scope.FormType == "Reply") {
            $scope.IsEnable = true;
            $scope.FormText = "Reply"
        }
        else if ($scope.FormType == "Retweet") {
            $scope.IsEnable = false;
            $scope.FormText = "Retweet"
        }
        else {
            $scope.IsEnable = true;
            $scope.getHistory();
            $scope.FormText = "Retweet With Comment"
        }

        //Facebook History
        if ($scope.MessageType == "FC" || $scope.MessageType == "FP")
            $scope.getHistory();

        if ($scope.MessageType == "IM" || $scope.MessageType == "IC") {
            $scope.getHistory();
        }

        if ($scope.MessageType == "TP" || $scope.MessageType == "STP") {
            $scope.getHistory();
        }

        $scope.MessageItself = "";
        $scope.MessagePostDateTime = items.status.ReplyedDate;
        $scope.MessagePostUserImageUrl = "";

        //$scope.commentReply = "A";
    };

    $scope.CheckProfile = function (profileId, UserName, IsPage, ProfileExternalId, ProfilePicURL) {
        // alert(profileId);
        $scope.replyedTo = UserName;
        $scope.IsPage = IsPage;
        $scope.ProfileExternalId = ProfileExternalId;
        $scope.MessagePostUserImageUrl = ProfilePicURL;
        $scope.SelectedProfileId = profileId;

        if ($scope.FormType == "Retweet")
            $scope.SocialActivityExists();

        /*if ($scope.MessageType == "FC" || $scope.MessageType == "FP")
            $scope.getHistory();*/

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
        var sendingPostId = $scope.StatusId;
        var sendingCommentId = '0';
        if ($scope.StatusId.split('|').length == 2) {
            sendingPostId = $scope.StatusId.split('|')[1];
            sendingCommentId = $scope.StatusId.split('|')[0];
        }
        else if ($scope.StatusId.split('|').length == 3) {
            sendingPostId = $scope.StatusId.split('|')[2];
            sendingCommentId = $scope.StatusId.split('|')[1];
        }

        if ($scope.MessageType == "FP" || $scope.MessageType == "FC") {
            FacebookService.FacebookGetPostInfo(sendingPostId, sendingCommentId).then(function (l) {
                if (l.statusText == "OK") {
                    $scope.inboxHistoryItemDetails = l.data;
                    $scope.totalConversation = 2000;
                    $scope.PostAuthor = l.data.fbPost.AuthorName;
                    $scope.MessageItself = $scope.TextToURLConvert(l.data.fbPost.Msg);
                    console.log("Contact History Tweets fetched Successfully");
                }
            }, function (err) {
                //alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching post history, please try again.");
                console.log(err);
            });
        }
        else if ($scope.MessageType == "IM" || $scope.MessageType == "IC") {
            //Instagram Posts
            InstagramService.GetPostDetails(sendingPostId).then(function (l) {
                $scope.inboxHistoryItemDetails = l.data;
                $scope.totalConversation = 140;
                $scope.SelectedProfileId = $scope.attachedProfiles[0].Id;
                $scope.replyedMsgModel = "";
                $scope.MessageItself = $scope.TextToURLConvert(l.data.PostUrl);

            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching post history, please try again.");
                console.log(err);
            });
        }
        else if ($scope.MessageType == "TP" && $scope.PageName != "TwitterFeed") {
            //Twitter Post            
            TwitterService.TwitterGetTweetThreadByTweetId($scope.ofUser, sendingPostId).then(function (l) {
                if (l.statusText == "OK") {
                    //threadTwitterHome
                    $scope.inboxHistoryItemDetails = l.data;
                    $scope.MessageItself = $scope.TextToURLConvert(l.data.Tweet.UserTwit);
                    $scope.PostAuthor = null;
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);

                console.log(err);
            });


            /*TwitterService.GetTweetMinnedReplies($scope.ofUser, $scope.StatusId).then(function (l) {
                $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, l.data);                
                $scope.MessageItself = items.status.UserTwit;
                if(l.data.length > 0){
                    $scope.PostAuthor = l.data[0].ToUser;
                } else{
                    $scope.PostAuthor = null;
                }
                console.log("Contact History Tweets fetched Successfully");
            }, function (err) {
                //alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

                console.log(err);
            });*/
        }
        else if ($scope.MessageType == "STP" || $scope.PageName == "TwitterFeed") {
            //Twitter Post            
            TwitterService.GetTweetDirectReplies($scope.replyedTo, $scope.ofUser, $scope.StatusId).then(function (l) {
                $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, l.data);
                $scope.MessageItself = items.status.UserTwit;
                //$scope.TextToURLConvert($scope.MessageItself, $scope.Id, $scope.MessageType, $scope.Attachments);
                if (l.data.length > 0) {
                    $scope.PostAuthor = l.data[0].ToUser;
                } else {
                    $scope.PostAuthor = null;
                }
                console.log("Contact History Tweets fetched Successfully");
            }, function (err) {
                //alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching tweet history, please try again.");

                console.log(err);
            });
        }
    };

    $scope.GetTwitterProfilesByGroupId = function () {
        //Follower details
        if ($scope.MessageType == "FC" || $scope.MessageType == "FP") {
            ProfileService.GetFacebookProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

                $.each(l.data, function (key, value) {
                    $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
                });
                
                if ($scope.attachedProfiles.length > 0) {
                    $scope.replyedTo = $scope.attachedProfiles[0].UserName;
                    $scope.IsPage = $scope.attachedProfiles[0].IsPage;
                  //  $scope.ProfileExternalId = $scope.attachedProfiles[0].ProfileExternalId;  //commented on Jan 18, 2018 for Telenor
                    //  $scope.MessagePostUserImageUrl = $scope.attachedProfiles[0].ProfilePicURL; //commented on Jan 18, 2018 for Telenor

                    for (i = 0; i < $scope.attachedProfiles.length; i++) {
                        if ($scope.attachedProfiles[i].ProfileExternalId == $scope.ProfileExternalId) {
                            $scope.MessagePostUserImageUrl = $scope.attachedProfiles[i].ProfilePicURL;
                        }
                    }

                    if ($scope.FormType == "Retweet")
                        $scope.SocialActivityExists();
                }
            }, function (err) {
               // alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching post history, please try again.");

                console.log(err);
            });
        }
        else if ($scope.MessageType == "IM" || $scope.MessageType == "IC") {
            ProfileService.GetInstagramProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {

                $.each(l.data, function (key, value) {
                    $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
                });

                if ($scope.attachedProfiles.length > 0) {
                    $scope.replyedTo = $scope.attachedProfiles[0].UserName;
                    $scope.IsPage = $scope.attachedProfiles[0].IsPage;
                    // $scope.ProfileExternalId = $scope.attachedProfiles[0].ProfileExternalId; //commented on Jan 18, 2018 for Telenor
                    // $scope.MessagePostUserImageUrl = $scope.attachedProfiles[0].ProfilePicURL;  //commented on Jan 18, 2018 for Telenor
                   
                      for (i = 0; i < $scope.attachedProfiles.length; i++) {
                        if ($scope.attachedProfiles[i].ProfileExternalId == $scope.ProfileExternalId) {
                            $scope.MessagePostUserImageUrl = $scope.attachedProfiles[i].ProfilePicURL;
                        }
                    }
                }
            }, function (err) {
                // alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching post history, please try again.");

                console.log(err);
            });
        }
        else {
            ProfileService.GetTwitterProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {
                var i = 0;
                $.each(l.data, function (key, value) {
                        $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ProfileExternalId: value.ProfileExternalId, IsPage: value.IsPage, ischecked: key == 0 ? true : false });
                });

                if ($scope.attachedProfiles.length > 0) {
                   // $scope.replyedTo = $scope.attachedProfiles[0].UserName;//commented on Jan 18, 2018 for Telenor
                    $scope.IsPage = $scope.attachedProfiles[0].IsPage;
                  //  $scope.ProfileExternalId = $scope.attachedProfiles[0].ProfileExternalId; ////commented on Jan 18, 2018 for Telenor
                  //  $scope.MessagePostUserImageUrl = $scope.attachedProfiles[0].ProfilePicURL;

                    for (i = 0; i < $scope.attachedProfiles.length; i++) {
                        if ($scope.attachedProfiles[i].ProfileExternalId == $scope.ProfileExternalId) {
                            $scope.MessagePostUserImageUrl = $scope.attachedProfiles[i].ProfilePicURL;
                            $scope.replyedTo = $scope.attachedProfiles[i].UserName;
                        }
                    }

                    if ($scope.FormType == "Retweet")
                        $scope.SocialActivityExists();
                }
            }, function (err) {
               // alert('Error in fetching contact history tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching tweet history, please try again.");

                console.log(err);
            });
        }
    };

    $scope.LikeComment = function (commentId, nestedCommentId) {
        var statusId = commentId;
        if (nestedCommentId != "")
            statusId = nestedCommentId;
        FacebookService.FacebookLikePost($scope.ProfileExternalId, $scope.IsPage, statusId).then(function (l) {
            var comment = $scope.inboxHistoryItemDetails.filter(function (el) {
                return el.Id == commentId;
            });
            if (nestedCommentId != "") {
                var nestedComment = comment[0].fbCommentComments.filter(function (el) {
                    return el.Id == nestedCommentId;
                });
                nestedComment[0].IsLiked = true;
            }
            else {
                comment[0].IsLiked = true;
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    }

    $scope.UnlikeComment = function (commentId, nestedCommentId) {
        var statusId = commentId;
        if (nestedCommentId != "")
            statusId = nestedCommentId;
        FacebookService.FacebookUnLikePost($scope.ProfileExternalId, $scope.IsPage, statusId).then(function (l) {
            var comment = $scope.inboxHistoryItemDetails.filter(function (el) {
                return el.Id == commentId;
            });
            if (nestedCommentId != "") {
                var nestedComment = comment[0].fbCommentComments.filter(function (el) {
                    return el.Id == nestedCommentId;
                });
                nestedComment[0].IsLiked = false;
            }
            else {
                comment[0].IsLiked = false;
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    }

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

    $scope.UnRetweet = function () {
        TwitterService.TwitterUnRetweet($scope.replyedTo, $scope.retweetedStatusId).then(function (l) {

            // alert("Tweet posted successfully ")
            notify({
                message: "Tweet Unretweeted successfully",
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

    $scope.mediaData = new Array();

    $scope.PostTweet = function () {
        if ($scope.MessageType == "FC") {
            console.log($scope.commentReply);
            if ($scope.UploadedFiles.length > 0) {
                $scope.UploadedFiles.forEach(function (i) {
                    var file = i._file;
                    $scope.mediaData.push({ Base64String: "", ContentType: file.type, FileName: file.name });
                });

                $scope.UploadedFiles.forEach(function (i) {
                    var file = i._file;
                    $scope.convertToBase64StringAndSend(file, $scope.MessageType);
                });
            }
            else {
                $scope.postFBNestedComment();
            }
        }
        else if ($scope.MessageType == "FP") {
            if ($scope.UploadedFiles.length > 0) {
                $scope.UploadedFiles.forEach(function (i) {
                    var file = i._file;
                    $scope.mediaData.push({ Base64String: "", ContentType: file.type, FileName: file.name });
                });

                $scope.UploadedFiles.forEach(function (i) {
                    var file = i._file;
                    $scope.convertToBase64StringAndSend(file, $scope.MessageType);
                });
            }
            else {
                $scope.postFBComment();
            }
        }
        else if ($scope.MessageType == "IM") {
            //PostId,UserId,Comment
            InstagramService.PostComment($scope.StatusId, $scope.SelectedProfileId, encodeURIComponent($scope.replyedMsgModel),$rootScope.selectedCompanyId).then(function (l) {
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
        }
        else if ($scope.MessageType == "TP" || $scope.MessageType == "STP") {
            if ($scope.FormType == "Reply") {
                if ($scope.UploadedFiles.length > 0) {
                    $scope.UploadedFiles.forEach(function (i) {
                        var file = i._file;
                        $scope.mediaData.push({ Base64String: "", ContentType: file.type, FileName: file.name });
                    });

                    $scope.UploadedFiles.forEach(function (i) {
                        var file = i._file;
                        $scope.convertToBase64StringAndSend(file, $scope.FormType);
                    });
                }
                else {
                    $scope.postReplyTweet();
                }                
            }
            else if ($scope.FormType == "Retweet") {
                $scope.postReplyRetweet();
            }
            else {
                if ($scope.UploadedFiles.length > 0) {
                    $scope.UploadedFiles.forEach(function (i) {
                        var file = i._file;
                        $scope.mediaData.push({ Base64String: "", ContentType: file.type, FileName: file.name });
                    });

                    $scope.UploadedFiles.forEach(function (i) {
                        var file = i._file;
                        $scope.convertToBase64StringAndSend(file, $scope.FormType);
                    });
                }
                else {
                    $scope.postTwitterRetweetWithComment();
                }
            }
        }

    };
    $scope.init();

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


    $scope.IsHidden = true;
    $scope.ShowHideReplyBox = function () {
        //If DIV is hidden it will be visible and vice versa.
        $scope.IsHidden = $scope.IsHidden ? false : true;
    }
    $scope.MakeCommentEditable = function (id) {
        //If DIV is hidden it will be visible and vice versa.
        $('#' + id + '-text').attr('contentEditable', true);
        $('#' + id + '-edit').hide();
        $('#' + id + '-save').show();
        $('#' + id + '-cancel').show();
    }
    $scope.CancelUpdateComment = function (id) {
        $('#' + id + '-text').attr('contentEditable', false);
        $('#' + id + '-edit').show();
        $('#' + id + '-save').hide();
        $('#' + id + '-cancel').hide();
    }
    $scope.UpdateComment = function (id) {
        FacebookService.FacebookUpdateMessage($scope.AuthorUserId, $scope.AuthorIsPage, $scope.StatusId.split('|')[0], $('#' + id + '-text').html(), "FC").then(function (l) {
            notify({
                message: "Comment updated successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });

            $scope.CancelUpdateComment(id);
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            // alert('Error in post tweet, please try again.');
            console.log(err);

            $scope.CancelUpdateComment(id);
        });
    }
    $scope.AddInstagramComment = function (commentText) {
        console.log(commentText);
        $scope.replyedMsgModel = commentText;
        $scope.StatusId = items.status.StatusId;
        $scope.PostTweet();
    }

    $scope.AddTwitterComment = function (commentText) {
        console.log(commentText);
        $scope.replyedMsgModel = commentText;
        $scope.StatusId = items.status.StatusId;
        $scope.PostTweet();
    }

    $scope.ValidateFiles = function () {
        $scope.errorMsg = "";
        var allowedExtentions = ["png", "jpg", "jpeg", "gif"]
        var allowedFbExtentions = ["png", "jpg", "jpeg"];
        var allowedTWImgExtentions = ["png", "jpg", "jpeg", "webp"];
        $scope.notValidated = false;
        $scope.count = 0;

        //$scope.selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
        //    return $scope.selected[record.ProfileId];
        //});

        var profileType = "";
        if ($scope.MessageType == "FP" || $scope.MessageType == "FC") {
            profileType = "FB";
        }
        else if ($scope.MessageType == "TP" || $scope.MessageType == "STP")
        {
            profileType = "TW";
        }
        
        var uploadedFilesExtentions = new Array();
        $scope.UploadedFiles.forEach(function (i) {
            uploadedFilesExtentions.push(i.file.type.split('/')[1]);
        });

        uploadedFilesExtentions.forEach(function (i) {
            if (!allowedExtentions.includes(i)) {
                $scope.notValidated = true;
            }
        });

        if ($scope.notValidated) {
            $scope.errorMsg = "Invalid Extention uploaded";
            return;
        }

        var images = $scope.UploadedFiles.filter(function (record) {
            return record._file.type.split('/')[1] != "mp4" && record._file.type.split('/')[1] == "mov";
        });

        if (images.length > 0) {
            images.forEach(function (i) {
                if (i._file.size / 1024 / 1024 > 5) {
                    errorMsg = "Max file limit for image is 5 MB.";
                    $scope.notValidated = true;
                }
            });
            if ($scope.notValidated) {
                return;
            }
        }

        if (profileType == "TW") {
            var filesCountExceeded = $scope.UploadedFiles.length > 4;
            if (filesCountExceeded) {
                $scope.errorMsg = "File upload count limit exceeded for Twitter.";
                $scope.notValidated = true;
                return;
            }
            
            var isGifUploadedError = (uploadedFilesExtentions.includes("gif") && $scope.UploadedFiles.length > 1);
            if (isGifUploadedError) {
                $scope.errorMsg = "Max Gif upload limit is 1 for Twitter";
                $scope.notValidated = true;
                return;
            }
            var areAllImages = true;
            uploadedFilesExtentions.forEach(function (i) {
                if (!allowedTWImgExtentions.includes(i)) {
                    areAllImages = false;
                }
            });

            var isImgUploadedError = (areAllImages && $scope.UploadedFiles.length > 4);
            if (isImgUploadedError) {
                $scope.errorMsg = "Max Image upload limit is 4 for Twitter";
                $scope.notValidated = true;
                return;
            }
        }

        if (profileType == "FB") {
            var filesCountExceeded = $scope.UploadedFiles.length > 1;
            var invalidExtentions = !uploadedFilesExtentions.every(function (val) { return allowedFbExtentions.indexOf(val) >= 0; });

            if (filesCountExceeded)
                $scope.errorMsg += $scope.errorMsg.length > 0 ? (", Max upload limit is 1 for facebook profile") : "Max upload limit is 1 for facebook profile";

            if (invalidExtentions)
                $scope.errorMsg += $scope.errorMsg.length > 0 ? (", Only jpg jpeg or png extention allowed for facebook profile") : "Only jpg jpeg or png extention allowed for facebook profile";

            $scope.notValidated = filesCountExceeded || invalidExtentions;
            return;
        }
        $scope.notValidated = false;
        return;
    };

    $scope.convertToBase64StringAndSend = function (file, replyType) {
        var reader = new FileReader();
        reader.addEventListener("loadend", function () {
            console.log(reader.result.length);
            $scope.mediaData[$scope.count].Base64String = reader.result;

            if ($scope.mediaData.length == ($scope.count + 1)) {
                console.log($scope.mediaData);
                if (replyType == "Reply") {
                    $scope.postReplyTweet();
                }
                else if (replyType == "Retweet") {
                    $scope.postReplyRetweet();
                }
                else if (replyType == "FC") {
                    $scope.postFBNestedComment();
                }
                else if (replyType == "FP") {
                    $scope.postFBComment();
                }
                else {
                    $scope.postTwitterRetweetWithComment();
                }
                
            }
            $scope.count++;
        }, false);

        if (file) {
            reader.readAsDataURL(file);
        }
    };

    $scope.postReplyTweet = function () {
        TwitterService.TwitterReplyTweet($scope.replyedTo, $scope.StatusId, $scope.replyedMsgModel, $scope.mediaData, $rootScope.selectedCompanyId).then(function (l) {
            notify({
                message: "Tweet Replied successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close('cancel');
        }, function (err) {
            //alert('Error in post tweet, please try again.');
            GlobalService.ErrorPopup("Error", err.data.Message);
            console.log(err);
        });
    };

    $scope.postReplyRetweet = function () {
        TwitterService.TwitterRetweet($scope.replyedTo, $scope.StatusId).then(function (l) {
            // alert("Tweet posted successfully ")
            notify({
                message: "Tweet retweeted successfully",
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

    $scope.postTwitterRetweetWithComment = function () {
        TwitterService.TwitterRetweetWithComment($scope.replyedTo, $scope.ofUser, $scope.StatusId, $scope.replyedMsgModel, $scope.mediaData, $rootScope.selectedCompanyId).then(function (l) {
            notify({
                message: "Retweet with comment successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            // alert('Error in post tweet, please try again.');
            console.log(err);
        });
    };

    $scope.postFBNestedComment = function () {
        var fileContent = "";
        var contentType = "";
        var fileName = "";
        if ($scope.mediaData.length > 0) {
            fileContent = $scope.mediaData[0].Base64String;
            contentType = $scope.mediaData[0].ContentType;
            fileName = $scope.mediaData[0].FileName;
        }

        FacebookService.FacebookCommentPost($scope.ProfileExternalId, $scope.IsPage, $scope.StatusId, $scope.commentReply, fileContent, contentType, fileName, $rootScope.selectedCompanyId, 3).then(function (l) {
            notify({
                message: "Comment posted successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            // alert('Error in post tweet, please try again.');
            console.log(err);
        });
    };

    $scope.postFBComment = function () {
        var fileContent = "";
        var contentType = "";
        var fileName = "";
        if ($scope.mediaData.length > 0) {
            fileContent = $scope.mediaData[0].Base64String;
            contentType = $scope.mediaData[0].ContentType;
            fileName = $scope.mediaData[0].FileName;
        }

        FacebookService.FacebookCommentPost($scope.ProfileExternalId, $scope.IsPage, $scope.StatusId, $scope.replyedMsgModel, fileContent, contentType, fileName, $rootScope.selectedCompanyId, 2).then(function (l) {
            notify({
                message: "Posted successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            //alert('Error in post tweet, please try again.');
            console.log(err);
        });
    };

}]);