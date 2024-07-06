CRMApp.controller('SocialFeedsFacebookController', ['$rootScope', '$scope', '$sce', '$window', '$location', 'UnifiedService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', function ($rootScope, $scope,$sce, $window, $location, UnifiedService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify)
{
    $scope.recentMessagesTWId = 0;
    $scope.recentPageChatsFBId = 0;
    $scope.recentPostsFBId = 0;
    $scope.recentPostsTWId = 0;
    $scope.recentCommentsFBId = 0;
    $scope.recentInstagramId = 0;
    $scope.recentSearchPostTWId = 0;
    $scope.recentInstagramCommentId = 0;
    $scope.newInboxItemsCount = 0;

    $scope.init = function ()
    {
        $scope.PageNumber = 0;
        $scope.PageSize = 30;
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.Profiles = [];
        $scope.profileSelect = "0";
        
        $scope.getProfileDetails($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString());
    };

    $scope.getProfileDetails = function (groupId, companyId, isPersonal) {
        $scope.inboxitemDetails = new Array();
        UnifiedService.GetGroupFacebookProfiles(groupId, companyId, isPersonal).then(function (l) {
            $scope.Profiles = l.data;
            if ($scope.Profiles.length > 0) {
                $scope.fetchUnifiedInboxData();
            }
        }, function (err) {
            //alert('Error in fetching User Facebook Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching User Facebook Profiles.");

            //console.log(err);
        });
    };

    $scope.selectedProfileChanged = function () {
        $scope.PageNumber = 0;
        $scope.fetchUnifiedInboxData();
    };

    $scope.profileLoadMoreFeeds = function () {
        $scope.PageNumber++;
        $scope.fetchUnifiedInboxData();
    };

    $scope.fetchUnifiedInboxData = function () {
        var selectedProfile = $scope.Profiles[$scope.profileSelect];

        var commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes = "1,2,7", commaSeperatedTagTypes = "0", commaSeperatedBrandTypes = "0";
        if (selectedProfile.IsPage == true) {
            commaSeperatedPages = selectedProfile.Id;
            commaSeperatedProfiles = "0";
            }
            else {
            commaSeperatedPages = "0";
            commaSeperatedProfiles = selectedProfile.Id;
        }

        $scope.Counter = ($scope.PageNumber * $scope.PageSize) + $scope.newInboxItemsCount;

        UnifiedService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedBrandTypes, $scope.Counter,
            $scope.PageSize, "Unified", "0", $rootScope.selectedGroupId, $rootScope.selectedCompanyId, true, "0", false).then(function (l) {
            var response = l.data;
            // alert(response)
            if($scope.PageNumber == 0)
                $scope.inboxitemDetails = response;
            else {
                if (response.length == 0) {
                    $scope.PageNumber--;
                }
                else {
                    $scope.inboxitemDetails.push.apply($scope.inboxitemDetails, response);
                }
            }

            $scope.inboxitemDetails.forEach(function (i) {
                i.Attachments.forEach(function (i) {
                    if (i.Type == 'video_fb' || i.Type == 'video_share')
                        i.Url = $sce.trustAsResourceUrl(i.Url);
                });
            });

            var data = $.grep(response, function (record) {
                return record.MessageType == 'FCP'
            });

            if (data.length > 0)
                $scope.recentPageChatsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentPageChatsFBId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentPageChatsFBId;

            data = $.grep(response, function (record) {
                return record.MessageType == 'FP'
            });

            if (data.length > 0)
                $scope.recentPostsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentPostsFBId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentPostsFBId;

            data = $.grep(response, function (record) {
                return record.MessageType == 'FC'
            });

            if (data.length > 0)
                $scope.recentCommentsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentCommentsFBId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentCommentsFBId;
        }, function (err) {
           // alert('Error in fetching Inbox data');
            GlobalService.ErrorPopup("Error", "Error in fetching feeds.");

            //console.log(err);
        });
    }

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.getProfileDetails(msg.GroupId, msg.CompanyId, msg.IsPersonal);
    });

    $scope.init();
    $scope.CreateTask = function (msg) {
        UnifiedService.ProfileDataObj = msg;
        $window.location.href = "/#/CreateCase?ActivityId=" + msg.Id + "&ActivityTable=" + msg.MessageType + "&Status=" + (msg.IsTaskCreated == true ? 1 : 0);
    }
    $scope.openReplyAdd = function (obj) {
        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "Reply" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
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

     $scope.latestInboxitemDetails = new Array();

     $scope.loadLatestInboxItems = function () {
         Array.prototype.push.apply($scope.latestInboxitemDetails, $scope.inboxitemDetails);
         $scope.inboxitemDetails = $scope.latestInboxitemDetails;
         $scope.latestInboxitemDetails = new Array();
     };

     $scope.fetchLatestUnifiedInboxData = function () {
         var selectedProfile = $scope.Profiles[$scope.profileSelect];

         var commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes = "1,2,7", commaSeperatedTagTypes = "0", commaSeperatedBrandTypes = "0",commaSeperatedConsumerNumbers = "0";
         if (selectedProfile.IsPage == true) {
             commaSeperatedPages = selectedProfile.Id;
             commaSeperatedProfiles = "0";
         }
         else {
             commaSeperatedPages = "0";
             commaSeperatedProfiles = selectedProfile.Id;
         }

         var ids = $scope.recentMessagesTWId + '|' + $scope.recentPageChatsFBId + '|' + $scope.recentPostsFBId + '|' + $scope.recentPostsTWId + '|' + $scope.recentCommentsFBId + '|' + $scope.recentInstagramId + '|' + $scope.recentSearchPostTWId + '|' + $scope.recentInstagramCommentId;


         UnifiedService.GetLatestUnifiedInboxData(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedBrandTypes, 0,
            $scope.PageSize, "Unified", $rootScope.selectedGroupId, $rootScope.selectedCompanyId, false, false, commaSeperatedConsumerNumbers, ids).then(function (l) {
                 if (l.data != null) {
                     //alert(l.data);
                     var response = l.data;
                     response = $.grep(response, function (record) {
                         return record.MessageType == 'FCP' || record.MessageType == 'FP' || record.MessageType == 'FC'
                     });

                     var tempArray = new Array();

                     response.forEach(function (i) {
                         var data = $.grep($scope.latestInboxitemDetails, function (record) {
                             return record.MessageType == i.MessageType && record.Id == i.Id
                         });
                         if (data.length == 0) {
                             i.ClassName = "new_msg";
                             tempArray.push(i);
                         }
                     });

                     $scope.newInboxItemsCount += tempArray.length;
                     Array.prototype.push.apply(tempArray, $scope.latestInboxitemDetails)
                     $scope.latestInboxitemDetails = tempArray;

                     $scope.loadLatestInboxItems();

                     //$scope.newInboxItemsCount += response.length;
                     //Array.prototype.push.apply(response, $scope.latestInboxitemDetails)
                     //$scope.latestInboxitemDetails = response;

                     if (response.length > 0) {
                         var data = $.grep(response, function (record) {
                             return record.MessageType == 'FCP'
                         });

                         if (data.length > 0)
                             $scope.recentPageChatsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));

                         data = $.grep(response, function (record) {
                             return record.MessageType == 'FP'
                         });

                         if (data.length > 0)
                             $scope.recentPostsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));


                         data = $.grep(response, function (record) {
                             return record.MessageType == 'FC'
                         });

                         if (data.length > 0)
                             $scope.recentCommentsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));
                     }
                 }
             }, function (err) {
                 // GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                 console.log(err);
             });

     }

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

}]);