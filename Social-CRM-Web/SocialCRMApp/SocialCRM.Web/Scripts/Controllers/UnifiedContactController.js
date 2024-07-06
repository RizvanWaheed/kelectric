CRMApp.controller('UnifiedContactController', ['$rootScope', '$scope', '$window', '$stateParams', '$location', '$uibModal', 'UnifiedService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $stateParams, $location, $uibModal, UnifiedService, GlobalService, notify) {
    $scope.fromUser = $location.search().fromUser;
    $scope.ofUser = $location.search().ofUser;
    if ($scope.fromUser == undefined || $scope.ofUser == undefined)
        $window.location.href = "/#/UnifiedInbox"

    $scope.isShowDM = false;

    $scope.init = function () {
        $scope.CompanyId = $rootScope.selectedCompanyId;
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.IsPersonal = $rootScope.IsPersonal.toString();
        $scope.friendshipItemDetails = [];
        $scope.inboxHistoryItemDetails = [];
        $scope.mentionItemDetails = [];
        $scope.tweetItemDetails = [];
        $scope.followerItemDetails = [];
        $scope.followingItemDetails = [];
        $scope.TwitterAccounts = [];
        $scope.TwitterProfiles = [];
        $scope.Comments = [];
        $scope.OwnedSocialAccounts = [];
        $scope.currentCursor = -1;
        $scope.currentTab = "history";
        $scope.currentEndReached = false;
        $scope.pageSize = 30;
        $scope.CompanyName = "";
        $scope.UserTitle = "";
        $scope.UserEmail = "";
        $scope.UserPhone = "";
        $scope.profileSelect = "0";
        $scope.Mode = "private";
        $scope.lists = {};

        UnifiedService.getTwitterProfile($scope.fromUser, $scope.ofUser).then(function (l) {
            $scope.contactProfilePic = l.data.contactProfilePic;
            $scope.contactExternalId = l.data.contactExternalId;
            $scope.contactName = l.data.contactName;
            $scope.contactTwitName = l.data.contactTwitName;
            $scope.contactMsg = l.data.contactMsg;
            $scope.contactCountry = l.data.contactCountry;
            $scope.contactURL = l.data.contactURL;
            $scope.totalFollower = l.data.totalFollower;
            $scope.totalFollowing = l.data.totalFollowing;
            $scope.isContactVerified = l.data.isContactVerified;
            console.log("Contact Profile fetched Successfully");
        }, function (err) {
          //  alert('Error in fetching contact profile, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact profile, please try again.");
            console.log(err);
        });

        UnifiedService.GetFriendshipInfo($scope.fromUser, $scope.ofUser, $scope.GroupId, $scope.CompanyId, $scope.IsPersonal).then(function (l) {
            $scope.friendshipItemDetails = l.data;
            for (x = 0; x < l.data.length; x++) {
                if (!$scope.isShowDM) {
                    if (l.data[x].IsFollowingContact == true && l.data[x].IsFollowedByContact == true) {
                        $scope.isShowDM = true;
                    }
                }
            }

            console.log("Contact Friendship Status fetched Successfully");
        }, function (err) {
           // alert('Error in fetching contact Friendship Status, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact Friendship Status, please try again.");
            console.log(err);
        });

        UnifiedService.GetContactOwnedSocialProfiles($scope.ofUser).then(function (l) {
            $scope.OwnedSocialAccounts = l.data;
            console.log("Contact Owned Social Accounts fetched Successfully");
        }, function (err) {
            //alert('Error in fetching Contact Owned Social Accounts, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching Contact Owned Social Accounts, please try again.");
            console.log(err);
        });

        UnifiedService.GetUsersOwnedTwitterLists($scope.ofUser, $scope.GroupId, $scope.CompanyId, $scope.IsPersonal, true).then(function (l) {
            $scope.TwitterAccounts = l.data;
            console.log("Contact Twitter Lists fetched Successfully");
        }, function (err) {
           // alert('Error in fetching contact Twitter Lists, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact Twitter Lists, please try again.");
            console.log(err);
        });

        UnifiedService.GetGroupTwitterProfiles($scope.GroupId, $scope.CompanyId, $scope.IsPersonal).then(function (l) {
            $scope.TwitterProfiles = l.data;
            console.log("Group Twitter Profiles fetched Successfully");
        }, function (err) {
           // alert('Error in fetching Group Twitter Profiles, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching Group Twitter Profiles, please try again.");
            console.log(err);
        });

        UnifiedService.GetUserContactInfo($scope.ofUser, $scope.GroupId, $scope.CompanyId).then(function (l) {
            if (l.data != undefined) {
                $scope.CompanyName = (l.data.Company == undefined) ? '' : l.data.Company;
                $scope.UserTitle = (l.data.Title == undefined) ? '' : l.data.Title;
                $scope.UserEmail = (l.data.Email == undefined) ? '' : l.data.Email;
                $scope.UserPhone = (l.data.Phone == undefined) ? '' : l.data.Phone;
                $scope.Comments = l.data.Comments;
            }

            console.log("Contact Extra Info fetched Successfully");
        }, function (err) {
            //alert('Error in fetching contact extra info, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact extra info, please try again.");
            console.log(err);
        });

        if($('.panel-collapse.in').attr('id') == "collapseOne")
            $scope.loadHistory();
        else if ($('.panel-collapse.in').attr('id') == "collapseTwo")
            $scope.loadTweets();
        else if ($('.panel-collapse.in').attr('id') == "collapseThree")
            $scope.loadMentions();
        else if ($('.panel-collapse.in').attr('id') == "collapseFour")
            $scope.loadFollowers();
        else if ($('.panel-collapse.in').attr('id') == "collapseFive")
            $scope.loadFriends();
    };

    $scope.EditField = function (actualRow, editableRow) {
        $(actualRow).css('display', 'none');
        $(editableRow).css('display', 'block');
    };

    $scope.SaveCompanyName = function (company) {
        UnifiedService.UpdateUserCompanyInfo($scope.contactExternalId, $scope.ofUser, $scope.CompanyId, $scope.GroupId, company).then(function (l) {
            $("#trActualRowCompany").css('display', 'block');
            $("#trEditableRowCompany").css('display', 'none');
            console.log("Contact Company updated Successfully");
        }, function (err) {
           // alert('Error in updating contact company info, please try again.');
            GlobalService.ErrorPopup("Error", "Error in updating contact company info, please try again.");
            console.log(err);
        });
    }

    $scope.SaveUserTitle = function (title) {
        UnifiedService.UpdateUserTitleInfo($scope.contactExternalId, $scope.ofUser, $scope.CompanyId, $scope.GroupId, title).then(function (l) {
            $("#trActualRowTitle").css('display', 'block');
            $("#trEditableRowTitle").css('display', 'none');
            console.log("Contact Title updated Successfully");
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in updating contact Title info, please try again.");
           // alert('Error in updating contact Title info, please try again.');
            console.log(err);
        });
    }

    $scope.SaveUserEmail = function (email) {
        UnifiedService.UpdateUserEmailInfo($scope.contactExternalId, $scope.ofUser, $scope.CompanyId, $scope.GroupId, email).then(function (l) {
            $("#trActualRowEmail").css('display', 'block');
            $("#trEditableRowEmail").css('display', 'none');
            console.log("Contact Email updated Successfully");
        }, function (err) {
           // alert('Error in updating contact Email info, please try again.');
            GlobalService.ErrorPopup("Error", "Error in updating contact Email info, please try again.");
            console.log(err);
        });
    }

    $scope.SaveUserPhone = function (phone) {
        UnifiedService.UpdateUserPhoneInfo($scope.contactExternalId, $scope.ofUser, $scope.CompanyId, $scope.GroupId, phone).then(function (l) {
            $("#trActualRowPhone").css('display', 'block');
            $("#trEditableRowPhone").css('display', 'none');
            console.log("Contact Phone updated Successfully");
        }, function (err) {
            //alert('Error in updating contact Phone info, please try again.');
            GlobalService.ErrorPopup("Error", "Error in updating contact Phone info, please try again.");
            console.log(err);
        });
    }

    $scope.updateVar = function (event) {
        $scope.myVar = angular.element(event.target).text();
    };

    $scope.redirectTo = function (fromUser, ofUser) {
        $window.location.href = "/#/UnifiedContact?fromUser=" + fromUser + "&ofUser=" + ofUser;
        $window.location.reload();
    };

    $scope.getFollowers = function () {
        //Follower details
        UnifiedService.getTwitterFollowers($scope.fromUser, $scope.ofUser, $scope.currentCursor).then(function (l) {
            $scope.followerItemDetails.push.apply($scope.followerItemDetails, l.data.followers);
            $scope.currentCursor = l.data.cursor;
            if (l.data.cursor == 0) {
                $scope.currentEndReached = true;
            }
            console.log("Contact Followers fetched Successfully");
        }, function (err) {
           // alert('Error in fetching contact followers, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact followers, please try again.");
            console.log(err);
        });
    };

    $scope.getFriends = function () {
        //Following Details
        UnifiedService.getTwitterFriends($scope.fromUser, $scope.ofUser, $scope.currentCursor).then(function (l) {
            $scope.followingItemDetails.push.apply($scope.followingItemDetails, l.data.followers);
            $scope.currentCursor = l.data.cursor;
            if (l.data.cursor == 0) {
                $scope.currentEndReached = true;
            }
            console.log("Contact Friends fetched Successfully");
        }, function (err) {
           // alert('Error in fetching contact friends, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact friends, please try again.");
            console.log(err);
        });
    };

    $scope.getTweets = function () {
        //Follower details
        UnifiedService.getTwitterTweets($scope.fromUser, $scope.ofUser, $scope.pageSize, $scope.currentCursor).then(function (l) {
            $scope.tweetItemDetails.push.apply($scope.tweetItemDetails, l.data.tweets);
            $scope.currentCursor = l.data.maxId;
            if (l.data.tweets.length == 0) {
                $scope.currentEndReached = true;
            }
            console.log("Contact Tweets fetched Successfully");
        }, function (err) {
           // alert('Error in fetching contact tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact tweets, please try again.");
            console.log(err);
        });
    };

    $scope.getMentions = function () {
        //Follower details
        UnifiedService.getTwitterMentions($scope.fromUser, $scope.ofUser, $scope.pageSize, $scope.currentCursor).then(function (l) {
            $scope.mentionItemDetails.push.apply($scope.mentionItemDetails, l.data.tweets);
            $scope.currentCursor = l.data.maxId;
            if (l.data.tweets.length == 0) {
                $scope.currentEndReached = true;
            }
            console.log("Contact Tweets fetched Successfully");
        }, function (err) {
           // alert('Error in fetching contact tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact tweets, please try again.");
            console.log(err);
        });
    };

    $scope.getHistory = function () {
        //Follower details
        UnifiedService.getTwitterHistory($scope.fromUser, $scope.ofUser, $scope.pageSize, $scope.currentCursor, $scope.GroupId, $scope.CompanyId, $scope.IsPersonal).then(function (l) {
            $scope.inboxHistoryItemDetails.push.apply($scope.inboxHistoryItemDetails, l.data.tweets);
            $scope.currentCursor = l.data.maxId;
            if (l.data.tweets.length == 0) {
                $scope.currentEndReached = true;
            }
            console.log("Contact History Tweets fetched Successfully");
        }, function (err) {
          //  alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");
            console.log(err);
        });
    };

    $scope.loadMoreDataOfCurrentTab = function () {
        if ($scope.currentEndReached == false) {
            if ($scope.currentTab == "history") {
                $scope.getHistory();
            }
            else if ($scope.currentTab == "tweets") {
                $scope.getTweets();
            }
            else if ($scope.currentTab == "mentions") {
                $scope.getMentions();
            }
            else if ($scope.currentTab == "followers") {
                $scope.getFollowers();
            }
            else if ($scope.currentTab == "friends") {
                $scope.getFriends();
            }
        }
    };

    $scope.loadHistory = function () {
        $scope.currentCursor = 0;
        $scope.currentEndReached = false;
        $scope.currentTab = "history";
        $scope.inboxHistoryItemDetails = [];
        $scope.getHistory();
    };
    $scope.loadTweets = function () {
        $scope.currentCursor = 0;
        $scope.currentEndReached = false;
        $scope.currentTab = "tweets";
        $scope.tweetItemDetails = [];
        $scope.getTweets();
    };
    $scope.loadMentions = function () {
        $scope.currentCursor = 0;
        $scope.currentEndReached = false;
        $scope.currentTab = "mentions";
        $scope.mentionItemDetails = [];
        $scope.getMentions();
    };
    $scope.loadFollowers = function () {
        $scope.currentCursor = -1;
        $scope.currentEndReached = false;
        $scope.currentTab = "followers";
        $scope.followerItemDetails = [];
        $scope.getFollowers();
    };
    $scope.loadFriends = function () {
        $scope.currentCursor = -1;
        $scope.currentEndReached = false;
        $scope.currentTab = "friends";
        $scope.followingItemDetails = [];
        $scope.getFriends();
    };

    $scope.OpenComment = function () {
        $scope.items = {};
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_AddComments.html',
            controller: '_AddCommentsController',

            resolve: {
                items: function () {
                    return {
                        contactExternalId: $scope.contactExternalId,
                        ofUser: $scope.ofUser,
                        fromPage: "CC"
                    };//$scope.items;
                }
            }
        });
        modalInstance.result.then(function (selectedItem) {
            $scope.Comments.push(selectedItem);
        }, function () {
        });
    };

    $scope.DeleteComment = function (commentId) {
        UnifiedService.DeleteUserContactComment(commentId).then(function (l) {
            $scope.Comments = $scope.Comments.filter(function (el) {
                return el.Id !== commentId;
            });
            console.log("Contact Comment deleted Successfully");
        }, function (err) {
          //  alert('Error in deleting contact comment, please try again.');
            GlobalService.ErrorPopup("Error", "Error in deleting contact comment, please try again.");
            console.log(err);
        });
    };

    $scope.unfollowUser = function (data) {
        UnifiedService.UnfollowUser(data.UserName, $scope.ofUser).then(function (l) {
            data.IsFollowingContact = false;
            console.log("Contact Unfollowed Successfully");
        }, function (err) {
           // alert('Error in unfollowing contact, please try again.');
            GlobalService.ErrorPopup("Error", "Error in unfollowing contact, please try again.");
            console.log(err);
        });
    };

    $scope.followUser = function (data) {
        UnifiedService.FollowUser(data.UserName, $scope.ofUser).then(function (l) {
            data.IsFollowingContact = true;
            console.log("Contact Followed Successfully");
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in following contact, please try again.");
            //alert('Error in following contact, please try again.');
            console.log(err);
        });
    };

    $scope.AddContactToUserLists = function () {
        var selectedLists = $.grep($scope.TwitterAccounts, function (record) {
            return $scope.lists[record.Id];
        });

        UnifiedService.AddContactToUserLists($scope.ofUser, selectedLists).then(function (l) {
            selectedLists.forEach(function (entry) {
                entry.IsMember = true;
                $scope.lists[entry.Id] = false;
            });
            console.log("Contact Added To Lists Successfully");
        }, function (err) {
            //alert('Error in adding contacts to Lists, please try again.');
            GlobalService.ErrorPopup("Error", "Error in adding contacts to Lists, please try again.");
            console.log(err);
        });
    }

    $scope.RemoveContactFromUserList = function (list) {
        UnifiedService.RemoveContactFromUserList($scope.ofUser, list).then(function (l) {
            list.IsMember = false;
            console.log("Contact Deleted from List Successfully");
        }, function (err) {
            //alert('Error in deleting contact from List, please try again.');
            GlobalService.ErrorPopup("Error", "Error in deleting contact from List, please try again.");
            console.log(err);
        });
    }

    $scope.AddNewList = function () {
        var selectedProfile = $scope.TwitterProfiles[$scope.profileSelect];

        UnifiedService.CreateNewList(selectedProfile.UserName, $scope.ofUser, $scope.ListName, $scope.Mode, $scope.ListDescrip).then(function (l) {
            $scope.TwitterAccounts.push(l.data);
            console.log("New List Created Successfully");
        }, function (err) {
           // alert('Error in creating new List, please try again.');
            GlobalService.ErrorPopup("Error", "Error in creating new List, please try again.");
            console.log(err);
        });
    }

    $scope.init();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.openDirectMessage = function () {

        $scope.friendshipobj = [];
        for (x = 0; x < $scope.friendshipItemDetails.length; x++) {
            if ($("#frnship-" + $scope.friendshipItemDetails[x].Id).html() == "true")
            {
                $scope.friendshipobj.push($scope.friendshipItemDetails[x]);
            }
          // alert($("#frnship-" + $scope.friendshipItemDetails[x].Id).html());
        }

        $scope.items = { UserId: $scope.ofUser, FSObj: $scope.friendshipobj };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterDM.html',
            controller: '_TwitterDMController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }
    /*$scope.Comments = function () {
        $scope.CompanyName = "IbexGlobal";
        $scope.Designation = "Software";
        $scope.UserEmail = "ayesha.shamim@ibexglobal.com";
        $scope.UserContactNumber = " +92-345-589-7856";
        $scope.UserPhoneNumber = "+92-51-456-9827";
    };
    $scope.Comments ();*/
}]);