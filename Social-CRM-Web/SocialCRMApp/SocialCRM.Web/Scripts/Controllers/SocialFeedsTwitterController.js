CRMApp.controller('SocialFeedsTwitterController', ['$rootScope', '$scope', '$window', '$location', 'UnifiedService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, UnifiedService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify) {
    $scope.init = function () {
        $scope.PageNumber = 0;
        $scope.PageSize = 30;
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.Profiles = [];
        $scope.ProfileLists = [];
        $scope.profileSelect = "0";
        $scope.isProfileSelected = true;
        $scope.listOption = "timeline";
        $scope.followerItemDetails = [];
        $scope.inboxitemDetails = [];

        $scope.getProfileDetails($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString());
    };

    $scope.getProfileDetails = function (groupId, companyId, isPersonal) {
        $scope.inboxitemDetails = new Array();
        UnifiedService.GetGroupTwitterProfiles(groupId, companyId, isPersonal).then(function (l) {
            $scope.Profiles = l.data;
            if ($scope.Profiles.length > 0) {
                $scope.fetchUnifiedInboxData();
            }
        }, function (err) {
           // alert('Error in fetching User Twitter Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching User Twitter Profiles.");
            //console.log(err);
        });

        UnifiedService.GetUsersOwnedTwitterLists("bla", groupId, companyId, isPersonal, false).then(function (l) {
            $scope.ProfileLists = l.data;
        }, function (err) {
            //alert('Error in fetching User Twitter Profile Lists');
            GlobalService.ErrorPopup("Error", "Error in fetching User Twitter Profile Lists.");
            //console.log(err);
        });
    };

    $scope.filteredProfileLists = function (profile) {
        return $scope.ProfileLists.filter(function (list) {
            return profile.UserName == list.OwnerUserName;
        });
    };

    $scope.selectedProfileListChanged = function () {
        $scope.inboxitemDetails = [];
        $scope.listOption = "timeline";
        $scope.isProfileSelected = false;
        $scope.selectedList = $scope.ProfileLists.filter(function (list) {
            return $scope.profileSelect == list.Id;
        })[0];
        $scope.PageNumber = 0;
        $scope.getTweets();
    };

    $scope.selectedListOptionChanged = function () {
        if ($scope.listOption == "timeline") {
            $scope.inboxitemDetails = [];
            $scope.PageNumber = 0;
        }
        else if ($scope.listOption == "member") {
            $scope.followerItemDetails = [];
            $scope.PageNumber = -1;
        }
        $scope.getTweets();
    };

    $scope.getTweets = function () {
        if ($scope.listOption == "timeline") {
            UnifiedService.getTwitterListTweets($scope.selectedList.OwnerUserName, $scope.selectedList.Id, $scope.PageSize, $scope.PageNumber).then(function (l) {
                if ($scope.PageNumber == 0)
                    $scope.inboxitemDetails = l.data.tweets;
                else {
                    $scope.inboxitemDetails.push.apply($scope.inboxitemDetails, l.data.tweets);
                }

                if (l.data.tweets.length > 0)
                    $scope.PageNumber = l.data.maxId;

                console.log("List Tweets fetched Successfully");
            }, function (err) {
              //  alert('Error in fetching list tweets, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching list tweets, please try again.");
                console.log(err);
            });
        }
        else if ($scope.listOption == "member") {
            UnifiedService.GetTwitterListMembers($scope.selectedList.OwnerUserName, $scope.selectedList.Id, $scope.PageNumber).then(function (l) {
                if ($scope.PageNumber == -1)
                    $scope.followerItemDetails = l.data.followers;
                else {
                    $scope.followerItemDetails.push.apply($scope.followerItemDetails, l.data.followers);
                }

                if (l.data.followers.length > 0)
                    $scope.PageNumber = l.data.cursor;

                console.log("List Members fetched Successfully");
            }, function (err) {
                //alert('Error in fetching list members, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching list members, please try again.");
                console.log(err);
            });
        }
    };

    $scope.selectedProfileChanged = function () {
        $scope.inboxitemDetails = [];
        $scope.isProfileSelected = true;
        $scope.PageNumber = 0;
        $scope.fetchUnifiedInboxData();
    };

    $scope.profileLoadMoreFeeds = function () {
        if ($scope.isProfileSelected) {
            $scope.PageNumber++;
            $scope.fetchUnifiedInboxData();
        }
        else {
            $scope.getTweets();
        }
    };

    $scope.fetchUnifiedInboxData = function () {
        var selectedProfile = $scope.Profiles[$scope.profileSelect];

        /*var commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes = "3,4,6", commaSeperatedTagTypes = "0", commaSeperatedBrandTypes = "0";
        if (selectedProfile.IsPage == true) {
            commaSeperatedPages = selectedProfile.Id;
            commaSeperatedProfiles = "0";
        }
        else {
            commaSeperatedPages = "0";
            commaSeperatedProfiles = selectedProfile.Id;
        }

        UnifiedService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedBrandTypes, $scope.PageNumber,
            $scope.PageSize, "Unified", "0", $window.localStorage.getItem('selectedGroupId'), $window.localStorage.getItem('selectedCompanyId'), true, "0").then(function (l) {
                var response = l.data;
                // alert(response)
                if ($scope.PageNumber == 0)
                    $scope.inboxitemDetails = response;
                else {
                    if (response.length == 0) {
                        $scope.PageNumber--;
                    }
                    else {
                        $scope.inboxitemDetails.push.apply($scope.inboxitemDetails, response);
                    }
                }
            }, function (err) {
              //  alert('Error in fetching Inbox data');
                GlobalService.ErrorPopup("Error", "Error in fetching feeds");
                //console.log(err);
            });*/

        
        UnifiedService.getTwitterHomeTweets(selectedProfile.UserName, selectedProfile.UserName, $scope.PageSize, $scope.PageNumber).then(function (l) {
            if ($scope.PageNumber == 0)
                $scope.inboxitemDetails = l.data.tweets;
            else {
                $scope.inboxitemDetails.push.apply($scope.inboxitemDetails, l.data.tweets);
            }

            if (l.data.tweets.length > 0)
                $scope.PageNumber = l.data.maxId;

            console.log("User Home Tweets fetched Successfully");
        }, function (err) {
            //  alert('Error in fetching list tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching user home tweets, please try again.");
            console.log(err);
        });
    }

    $scope.RemoveContactFromUserList = function (index) {
        UnifiedService.RemoveContactFromUserList($scope.followerItemDetails[index].contactTwitName, $scope.selectedList).then(function (l) {
            $scope.followerItemDetails.splice(index, 1);
            console.log("User Deleted from List Successfully");
        }, function (err) {
            //alert('Error in deleting user from List, please try again.');
            GlobalService.ErrorPopup("Error", "Error in deleting user from List, please try again.");
            console.log(err);
        });
    }

    $scope.OpenEditList = function () {
        $scope.items = {};
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EditTwitterList.html',
            controller: '_EditTwitterListController',

            resolve: {
                items: function () {
                    return {
                        selectedList: $scope.selectedList
                    };//$scope.items;
                }
            }
        });
        modalInstance.result.then(function (selectedList) {
            $scope.selectedList = selectedList
        }, function () {
        });
    };

    $scope.DeleteTwitterList = function () {
        UnifiedService.DeleteTwitterList($scope.selectedList.OwnerUserName, $scope.selectedList.Id).then(function (l) {
            $scope.ProfileLists = $scope.ProfileLists.filter(function (list) {
                return list.Id != $scope.profileSelect;
            });
            $scope.profileSelect = "0";
            $scope.selectedProfileChanged();
            console.log("Twitter List Deleted Successfully");
        }, function (err) {
          //  alert('Error in deleting Twitter List, please try again.');
            GlobalService.ErrorPopup("Error", "Error in deleting Twitter List, please try again.");
            console.log(err);
        });
    };

    $scope.redirectTo = function (ofUser) {
        $window.location.href = "/#/UnifiedContact?fromUser=" + $scope.selectedList.OwnerUserName + "&ofUser=" + ofUser;
        $window.location.reload();
    };

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.getProfileDetails(msg.GroupId, msg.CompanyId, msg.IsPersonal);
    });

    $scope.init();
    $scope.CreateTask = function (msg) {
        UnifiedService.ProfileDataObj = msg;
        $window.location.href = "/#/CreateCase?ActivityId=" + msg.Id + "&ActivityTable=" + msg.MessageType + "&Status=" + (msg.IsTaskCreated == true ? 1 : 0);
    }
    $scope.openReplyAdd = function (obj) {
        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "Reply", PageName: "TwitterFeed" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterReply.html',
            controller: '_TwitterReplyController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.openConversation = function (StatusId, replyedBy, MessageType) {
        $scope.items = { replyedBy: replyedBy, StatusId: StatusId, MessageType: MessageType };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ProfileConversation.html',
            controller: '_ProfileConversationController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.openRetweet = function (obj) {
        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "Retweet" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterReply.html',
            controller: '_TwitterReplyController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            //  $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.openRetweetWithComment = function (obj) {
        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "RetweetComment" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterReply.html',
            controller: '_TwitterReplyController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.openReportAsSpam = function (StatusId, replyedBy, MessageType) {
        $scope.items = { replyedBy: replyedBy, StatusId: StatusId, MessageType: MessageType, FormType: "RetweetComment" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ReportAsSpam.html',
            controller: '_ReportAsSpamController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.openEmailTweets = function (obj) {
        $scope.items = { obj: obj };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EmailTweets.html',
            controller: '_EmailTweetsController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.likeTweet = function (obj) {

        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "RetweetComment" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_LikeComment.html',
            controller: '_LikeCommentController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });

    }

}]);