CRMApp.controller('SocialFeedsInstagramController', ['$rootScope', '$scope', '$window', '$location', 'UnifiedService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, UnifiedService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify)
{
    

    $scope.init = function ()
    {
        $scope.PageNumber = 0;
        $scope.PageSize = 30;
        $scope.recentInstagramId = 0;
        $scope.recentInstagramCommentId = 0;
        $scope.newInboxItemsCount = 0;
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
        UnifiedService.GetGroupInstagramProfiles(groupId, companyId, isPersonal).then(function (l) {            
            $scope.Profiles = l.data;
            if ($scope.Profiles.length > 0) {
                $scope.fetchUnifiedInboxData();
            }
        }, function (err) {
            //alert('Error in fetching User Facebook Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching User Instagram Profiles.");

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

        var commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes = "8,9", commaSeperatedTagTypes = "0", commaSeperatedBrandTypes = "0";
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

            var data = $.grep(response, function (record) {
                return record.MessageType == 'IM'
            });

            if (data.length > 0)
                $scope.recentInstagramId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentInstagramId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentInstagramId;

            data = $.grep(response, function (record) {
                        return record.MessageType == 'IC'
                    });

            if (data.length > 0)
                $scope.recentInstagramCommentId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentInstagramCommentId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentInstagramCommentId;

        }, function (err) {
           // alert('Error in fetching Inbox data');
            GlobalService.ErrorPopup("Error", "Error in fetching feeds.");

            //console.log(err);
        });
    }

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.getProfileDetails(msg.GroupId, msg.CompanyId, msg.IsPersonal);
    });

    $scope.TextToURLConvert = function (text, Id, MessageType, tags, attachments) {
        // var t = tags.length;
        var text = text;
        var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
        //For Instagram
        if (MessageType == "IM") {
            //var compImagePath = text.split("?");
            //var regexForMediaType = /(?:\.([^.]+))?$/;
            //var regexResponse = regexForMediaType.exec(compImagePath[0]);
            //var fileExtension = regexResponse[1];
            var compImagePath = attachments[0].Url.split("?");
            var regexForMediaType = /(?:\.([^.]+))?$/;
            var regexResponse = regexForMediaType.exec(compImagePath[0]);
            var fileExtension = regexResponse[1];

            if (fileExtension == "jpg" || fileExtension == "png") {
                var text1 = attachments[0].Url.replace(exp, '<img src="$1" height="294" width="294" class="img-thumbnail img_margin" style="border-radius:0%; object-fit: cover;">');
            } else if (fileExtension == "mp4") {
                var text1 = attachments[0].Url.replace(exp, '<video controls height="294" width="294" class="img-thumbnail img_margin"><source ng-src="$1" src="$1"></video>');
            }
            text1 = '<div>' + text + '</div><br/>' + text1;

        } else {
            var text1 = text.replace(exp, "<a target='_blank' href='$1'>$1</a>");
        }


        //for (i = 0; i < tags.length; i++) {
        //    var BGColor = '';
        //    var FontColor = '';
        //    for (j = 0; j < $scope.TagsTypes.length; j++) {
        //        if ($scope.TagsTypes[j].BrandType == tags[i]) {
        //            BGColor = $scope.TagsTypes[j].BGColor;
        //            FontColor = $scope.TagsTypes[j].FontColor;
        //            break;
        //        }
        //    }
        //    var style = {
        //        "background-color": BGColor,
        //        "color": FontColor

        //    };
        //    //text1 = text1.replace(new RegExp("(" + preg_quote(tags[i]) + ")", 'gi'), "<b style='color:" + FontColor + ";background-color:" + BGColor + ";' >$1</b>"); //ng-style={'background-color':" + BGColor + ",'color':" + FontColor + "}
        //    text1 = text1.replace(new RegExp("(" + preg_quote(tags[i]) + ")", 'gi'), "<b>$1</b>");
        //}

        if ((MessageType == "TM" || MessageType == "FCP" || MessageType == "FP" || MessageType == "TP" || MessageType == "FC")) {
            matches = text1.match(/\b\d{9}\b/g);
            var word = matches != null ? matches[0] : '';
            if (word)
                text1 = text1.replace(new RegExp("(" + preg_quote(word) + ")", 'gi'), "<b>$1</b>");
        }

        var exp2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
        // document.getElementById(MessageType + "-converted_url").innerHTML = text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>');


        $("#" + Id + "-" + MessageType + "-converted_url").html(text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>'))
        // return text;



    }

    $scope.init();
    $scope.CreateTask = function (msg) {
        UnifiedService.ProfileDataObj = msg;
        $window.location.href = "/#/CreateCase?ActivityId=" + msg.Id + "&ActivityTable=" + msg.MessageType + "&Status=" + (msg.IsTaskCreated == true ? 1 : 0);
    }
    $scope.openReplyAdd = function (obj) {
        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "Reply", MessageItself: obj.UserTwit, PostDateTime: obj.ReplyedDate };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_InstagramReply.html',
            controller: '_InstagramReplyController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) { }, function () { });
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

         var commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes = "8,9", commaSeperatedTagTypes = "0", commaSeperatedBrandTypes = "0",commaSeperatedConsumerNumbers = "0";
         if (selectedProfile.IsPage == true) {
             commaSeperatedPages = selectedProfile.Id;
             commaSeperatedProfiles = "0";
         }
         else {
             commaSeperatedPages = "0";
             commaSeperatedProfiles = selectedProfile.Id;
         }

         var ids = '0|0|0|0|0|' + $scope.recentInstagramId + '|0|' + $scope.recentInstagramCommentId;


         UnifiedService.GetLatestUnifiedInboxData(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedBrandTypes, 0,
            $scope.PageSize, "Unified", $rootScope.selectedGroupId, $rootScope.selectedCompanyId, false, false, commaSeperatedConsumerNumbers, ids).then(function (l) {
                if (l.data != null) {
                    //alert(l.data);
                    //var response = l.data;
                    //response = $.grep(response, function (record) {
                    //    return record.MessageType == 'IM'
                    //});
                    //$scope.newInboxItemsCount += response.length;
                    //Array.prototype.push.apply(response, $scope.latestInboxitemDetails)
                    //$scope.latestInboxitemDetails = response;

                    var response = l.data
                    response = $.grep(response, function (record) {
                        return record.MessageType == 'IM' || record.MessageType == 'IC'
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


                    if (response.length > 0) {
                        var data = $.grep(response, function (record) {
                            return record.MessageType == 'IM'
                        });

                        if (data.length > 0)
                            $scope.recentInstagramId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));

                        data = $.grep(response, function (record) {
                            return record.MessageType == 'IC'
                        });

                        if (data.length > 0)
                            $scope.recentInstagramCommentId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentInstagramCommentId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentInstagramCommentId;
                    }
                }
            }, function (err) {
                // GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                console.log(err);
            });

     }

}]);