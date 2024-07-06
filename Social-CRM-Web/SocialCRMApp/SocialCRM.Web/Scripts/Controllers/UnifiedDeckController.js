CRMApp.controller('UnifiedDeckController', ['$rootScope', '$scope', '$sce', '$window', '$location', 'UnifiedService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', 'UnifiedDeckService', '$sce', 'FileUploader', 'hotkeys', '$timeout', function ($rootScope, $scope, $sce, $window, $location, UnifiedService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify, UnifiedDeckService, $sce, FileUploader, hotkeys, $timeout) {

    $scope.ColumnsInitializer = [
      {
          name: 'TwitterHome',
          order: 0
      },
      {
          name: 'TwitterDM',
          order: 1
      },
      {
          name: 'FacebookPost',
          order: 2
      },
      {
          name: 'FacebookComment',
          order: 3
      },
      {
          name: 'FacebookChat',
          order: 4
      }
    ];

    $scope.LayoutLength = {
        height: '600px'
    }

    // Common Variables

    $scope.list1 = [
    { 'title': 'N', 'drag': true },
    { 'title': 'L', 'drag': true },
    { 'title': 'I', 'drag': true },
    { 'title': 'I', 'drag': true },
    { 'title': 'E', 'drag': true },
    { 'title': 'N', 'drag': true }
    ];

    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';

    $scope.PageSize = 30;
    $scope.firstTime = true;
    $scope.MinutesofCollector = 7;

    $scope.connectedFacebookProfiles = [];
    $scope.connectedFacebookPages = [];
    $scope.connectedTwitterProfiles = [];
    $scope.connectedAllFacebookProfiles = [];

    $scope.recentMessagesTWId = 0;
    $scope.recentPageChatsFBId = 0;
    $scope.recentPostsFBId = 0;
    $scope.recentPostsTWId = 0;
    $scope.recentCommentsFBId = 0;
    $scope.recentUnrespondedId = 0;
    $scope.newInboxItemsCount = 0;

    $scope.oldestMessagesTWId = 2147483647; //int.maxvalue
    $scope.oldestPageChatsFBId = 2147483647; //int.maxvalue
    $scope.oldestPostsFBId = 2147483647; //int.maxvalue
    $scope.oldestPostsTWId = 2147483647; //int.maxvalue
    $scope.oldestCommentsFBId = 2147483647; //int.maxvalue
    $scope.oldestUnrespondedId = 2147483647; //int.maxvalue

    var leftorderids = rightorderids = 1;
    var maxOrderId = 0;
    //var isHidden = false;

    //Tab focus disabling
    //$(window).keydown(function (e){ // $(':input').keydown(function (e) {
    //    if (e.keyCode == 9)   
    //        e.preventDefault();
    //});

    //Enter key disabling
    //$(window).keydown(function (e) {
    //    if (e.keyCode == 13)
    //        e.preventDefault();
    //});

    $scope.columnArrangementData = {};

    //Top movement
    var el = document.getElementById('deckContainer');
    var sortable = Sortable.create(el, {
        animation: 150,  // ms, animation speed moving items when sorting, `0` — without animation
        handle: ".box-header",  // Drag handle selector within list items
        forceFallback: false,  // ignore the HTML5 DnD behaviour and force the fallback to kick in
        // Element dragging ended
        onEnd: function (/**Event*/evt) {
            var startIndex = evt.oldIndex;  // element's old index within parent
            var endIndex = evt.newIndex;  // element's new index within parent

            if (startIndex == endIndex) //Same place
                return;

            if (startIndex == 5 || endIndex == 5) return; // Unresponded No

            ////console.log("Column " + startIndex + " moved to Column " + endIndex);

            //Update columns location
            //If column moved right                        
            if (endIndex > startIndex) {
                var tempValueHolder = $scope.columnArrangementData[startIndex].Value;
                for (var loop = startIndex + 1; loop <= endIndex; loop++) {
                    $scope.columnArrangementData[loop - 1].Value = $scope.columnArrangementData[loop].Value;
                }
                $scope.columnArrangementData[endIndex].Value = tempValueHolder;
            } else if (startIndex > endIndex) {
                var tempValueHolder = $scope.columnArrangementData[startIndex].Value;
                for (var loop = startIndex; loop > endIndex; loop--) {
                    $scope.columnArrangementData[loop].Value = $scope.columnArrangementData[loop - 1].Value;
                }
                $scope.columnArrangementData[endIndex].Value = tempValueHolder;
            }
            ////console.log($scope.columnArrangementData);

            for (var loop = 0; loop < $scope.columnArrangementData.length; loop++) {
                var obj = $scope.columnArrangementData[loop];
                switch (obj.Value) {
                    case 1:
                        $scope.OrderIds.TP = obj.Key;
                        $scope.ColumnsInitializer[0].order = obj.Key;
                        break;
                    case 2:
                        $scope.OrderIds.TM = obj.Key;
                        $scope.ColumnsInitializer[1].order = obj.Key;
                        break;
                    case 3:
                        $scope.OrderIds.FP = obj.Key;
                        $scope.ColumnsInitializer[2].order = obj.Key;
                        break;
                    case 4:
                        $scope.OrderIds.FC = obj.Key;
                        $scope.ColumnsInitializer[3].order = obj.Key;
                        break;
                    case 5:
                        $scope.OrderIds.FCP = obj.Key;
                        $scope.ColumnsInitializer[4].order = obj.Key;
                        break;
                }
            }

            //Difficulty
            //$scope.$digest();

            //DB Update
            UnifiedDeckService.UpdateColumnsOrderForResponder($rootScope.selectedCompanyId, $scope.columnArrangementData).then(function (result) {
                if (result.statusText == "OK") {
                    //Db updated successfully
                    notify({
                        message: "New setting saved successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                ////console.log(err);
            });
        }
    });

    $scope.OrderIds = {
        TP: '0',
        TM: '0',
        FP: '0',
        FC: '0',
        FCP: '0',
        UR: '0'
    }

    $scope.UnrespondedRecentIds = {
        recentMessagesTWId: 0,
        recentPageChatsFBId: 0,
        recentPostsFBId: 0,
        recentPostsTWId: 0,
        recentCommentsFBId: 0
    }

    $scope.ActivePattern = '';

    $scope.WarningColor = '#ccf1bb';
    $scope.AlertColor = '#f5fcbb';
    $scope.DangerColor = '#fdbfbf';
    $scope.WarningTime = 0;
    $scope.AlertTime = 0;
    $scope.DangerTime = 0;
    $scope.EnableUnrespondedColumnTiming = true;

    $scope.BrandsTypes = [];
    $scope.TagsTypes = [];
    $scope.BrandCategoryTypes = [];

    $scope.itemWarning = {
        "background-color": "#ccf1bb" /*#F9BF3B*/
    }

    $scope.itemAlert = {
        "background-color": "#f5fcbb" /*#3FC380*/
    }

    $scope.itemDanger = {
        "background-color": "#fdbfbf" /*#EF4836*/
    }

    $scope.sampleReplies = [];
    $scope.ReplyAreaUnresponded = '';
    $scope.ReplyArea = '';
    $scope.ReplyAreaTwitterDM = '';
    $scope.ReplyAreaFacebookWallpost = '';
    $scope.ReplyAreaFacebookComment = '';
    $scope.ReplyAreaFacebookChatThread = '';

    //Unresponded Inner Threads
    $scope.UR_ReplyArea = '';
    $scope.UR_ReplyAreaTwitterDM = '';
    $scope.UR_ReplyAreaFacebookWallpost = '';
    $scope.UR_ReplyAreaFacebookComment = '';
    $scope.UR_ReplyAreaFacebookChatThread = '';

    //Unresponded Area
    $scope.UnrespondedExpansionThread = {};

    //Media
    $scope.mediaData = new Array();
    $scope.UploadedFiles = new Array();
    $scope.UploadedFilesCount = 0;

    //Sentiments
    $scope.sentimentFilter = {
        selectTwitterHomeSentimentTypes: false,
        selectedTwitterHomeSentimentTypes: {},
        sentimentTypesTwitterHome: [
        {
            id: "1",
            name: "Positive",
            select: false
        },
        {
            id: "2",
            name: "Neutral",
            select: false
        },
        {
            id: "3",
            name: "Negative",
            select: false
        }
        ],
        selectTwitterDMSentimentTypes: false,
        selectedTwitterDMSentimentTypes: {},
        sentimentTypesTwitterDM: [
        {
            id: "1",
            name: "Positive",
            select: false
        },
        {
            id: "2",
            name: "Neutral",
            select: false
        },
        {
            id: "3",
            name: "Negative",
            select: false
        }
        ],
        selectFacebookHomeSentimentTypes: false,
        selectedFacebookHomeSentimentTypes: {},
        sentimentTypesFacebookHome: [
        {
            id: "1",
            name: "Positive",
            select: false
        },
        {
            id: "2",
            name: "Neutral",
            select: false
        },
        {
            id: "3",
            name: "Negative",
            select: false
        }
        ],
        selectFacebookCommentSentimentTypes: false,
        selectedFacebookCommentSentimentTypes: {},
        sentimentTypesFacebookComment: [
        {
            id: "1",
            name: "Positive",
            select: false
        },
        {
            id: "2",
            name: "Neutral",
            select: false
        },
        {
            id: "3",
            name: "Negative",
            select: false
        }
        ],
        selectFacebookChatSentimentTypes: false,
        selectedFacebookChatSentimentTypes: {},
        sentimentTypesFacebookChat: [
        {
            id: "1",
            name: "Positive",
            select: false
        },
        {
            id: "2",
            name: "Neutral",
            select: false
        },
        {
            id: "3",
            name: "Negative",
            select: false
        }
        ],
        selectUnrespondedQueriesSentimentTypes: false,
        selectedUnrespondedQueriesSentimentTypes: {
        },
        sentimentTypesUnrespondedQueries: [
        {
            id: "1",
            name: "Positive",
            select: false
        },
        {
            id: "2",
            name: "Neutral",
            select: false
        },
        {
            id: "3",
            name: "Negative",
            select: false
        }
        ]
    };

    $scope.RetweetComment = '';

    //Opening Thread Objects
    $scope.OpeningThreadObjects = {
        TwitterHome: {},
        TwitterDM: {},
        FacebookHome: {},
        FacebookComment: {},
        FacebookChat: {},
        Unresponded: {}
    }

    /*-----------------------------------------------*/
    /*-------------REGION TWITTER HOME---------------*/
    /*-----------------------------------------------*/
    $scope.selectedTwitterHome = {};
    //$scope.selectTwitterHome = true;
    $scope.selectTwitterHome = {
        value: true
    };
    $scope.dataTwitterHome = [];
    $scope.PageNumberTwitterHome = 0;
    $scope.newInboxItemsCountTwitterHome = 0;
    $scope.CounterTwitterHome = 0;
    $scope.selectedTwitterTypes = {};
    $scope.selectTwittertypes = true;
    $scope.totalConversationTwitterHome = 280;
    $scope.twitterHomeReplyProfile = {
        'AccountName': '',
        'ProfilePicURL': ''
    };
    $scope.ReplyingProfileTwitterHome = '';
    $scope.threadTwitterHome = {};
    $scope.selectTwittertags = { value: false };
    $scope.selectedTwitterTags = {};
    $scope.previewSizeTwitterHome = {
        width: '162px',
        height: '162px'
    }

    // Fetch Twitter Home Data
    $scope.GetUnifiedDeckDataForTwitterHome = function () {
        //Emptying for new data
        if ($scope.PageNumberTwitterHome == 0)
            $scope.dataTwitterHome = [];

        var selectedProfiles = $.grep($scope.connectedTwitterProfiles, function (record) {
            return $scope.selectedTwitterHome[record.CompositeId];
        });

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        angular.forEach($scope.selectedTwitterTypes, function (obj, key) {
            if (obj == true) {
                msgTypeData.push(key);
            }
        });

        var sentimentTypeData = [];
        angular.forEach($scope.sentimentFilter.selectedTwitterHomeSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var tagTypeData = [];
        var brandTypeData = [];

        var pageNumber = $scope.CounterTwitterHome;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;
        var mediaType = 5; //twitterPosts

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        //for (var i=0; i<scope.selectedTwitterHome.length; i++) {
        //    tagTypeData.push()
        //}

        angular.forEach($scope.selectedTwitterTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        UnifiedDeckService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedSentimentTypes, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType).then(function (result) {
            if (result.statusText == "OK" && result.data !== undefined) {
                ////console.log(result);
                if (result.data.TwitterPostItems.length == 0) {
                    $scope.PageNumberTwitterHome--;
                }
                else {
                    if ($scope.PageNumberTwitterHome == 0) {
                        $scope.dataTwitterHome = result.data.TwitterPostItems;
                        angular.forEach($scope.dataTwitterHome, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        });

                    } else {
                        angular.forEach(result.data.TwitterPostItems, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                            $scope.dataTwitterHome.push(obj);
                        });
                    }
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            ////console.log(err);
        });
    }

    $scope.checkAllProfilesTwitterHome = function (value) {
        angular.forEach($scope.connectedTwitterProfiles, function (obj) {
            //if (obj.ProfileTypeId == 4)
            //if (typeof value !== "undefined") {
            //    $scope.selectTwitterHome.value = value;
            //}
            $scope.selectedTwitterHome[obj.CompositeId] = $scope.selectTwitterHome.value;
        });

        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.profileSelectedChangedTwitterHome = function () {
        $scope.selectTwitterHome.value = false;
        //$('#checkAllProfilesTwitterHome').attr('checked', false);

        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.TwitterHomeMessageTypes = [
        {
            id: "4",
            name: "Mentions",
            select: false
        },
        {
            id: "3",
            name: "Tweets",
            select: false
        },
        {
            id: "6",
            name: "Retweets",
            select: false
        }
    ]

    $scope.checkAllTypesTwitterHome = function () {
        angular.forEach($scope.TwitterHomeMessageTypes, function (obj) {
            $scope.selectedTwitterTypes[obj.id] = $scope.selectTwittertypes;
        });
        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.typeSelectedChangedTwitterHome = function () {
        $scope.selectTwittertypes = false;
        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.twitterLoadMoreFeeds = function () {
        $scope.PageNumberTwitterHome++;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize) + $scope.newInboxItemsCountTwitterHome;
        $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.loadProfileTwitterHome = function (AccountName, ProfilePicURL) {
        $scope.ReplyingProfileTwitterHome = 'ReplyingProfileTwitterHome-' + AccountName;
        $scope.twitterHomeReplyProfile.AccountName = AccountName;
        $scope.twitterHomeReplyProfile.ProfilePicURL = ProfilePicURL;
    }

    $scope.quickReplyTwitterHome = function (replyText, itemModel, isUnresponded) {
        var replyedTo = $scope.twitterHomeReplyProfile.AccountName;  ////commented on 09 Jan ,2018 for Telenor lead working
        var StatusId = itemModel.StatusId;
        var selectedCompanyId = $rootScope.selectedCompanyId;
        var feedId = itemModel.Id;
        var groupId = $rootScope.selectedGroupId;
        var mediaInfo = null;

        if (itemModel.fileUploadObject != undefined) {
            mediaInfo = itemModel.fileUploadObject.mediaData;
            if (mediaInfo.length == 0) {
                mediaInfo = null;
            }
        }

        TwitterService.TwitterReplyTweet(replyedTo, StatusId, replyText, mediaInfo, selectedCompanyId).then(function (result) {

            if (result.statusText == "OK") {
                notify({
                    message: "Tweet Replied successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                if (isUnresponded !== undefined && isUnresponded == true) {
                    if (angular.equals($scope.OpeningThreadObjects.Unresponded, {
                    })) {
                        $scope.ReplyAreaUnresponded = "";
                        $scope.UnrespondedMarkAsCompleted(itemModel);
                    } else {
                        $scope.UR_ReplyArea = "";
                        $scope.UnrespondedMarkAsCompleted($scope.OpeningThreadObjects.Unresponded, "UnrespondedQueries");
                    }
                } else {
                    $scope.ReplyArea = "";
                    if (angular.equals($scope.OpeningThreadObjects.TwitterHome, {
                    })) {
                        $scope.MarkAsCompleted(itemModel);
                    } else {
                        $scope.MarkAsCompleted($scope.OpeningThreadObjects.TwitterHome, "TwitterPost");
                    }
                }
                $scope.showTextBox(feedId);
                //$scope.UpdateTaskCompletedItems(feedId, 'TP');
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            ////console.log(err);
        });
    }

    $scope.quickRetweetTwitterHome = function (StatusId, displayId) {

        var displayParameter = StatusId;
        if (displayId !== undefined) {
            displayParameter = displayId;
        }

        var replyedTo = $scope.twitterHomeReplyProfile.AccountName;
        if ($('#retweet-' + displayParameter).attr("value") == 'Retweet') {
            TwitterService.TwitterRetweet(replyedTo, StatusId).then(function (result) {
                if (result.statusText == "OK") {
                    notify({
                        message: "Tweet retweeted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
                $("#retweetWithCommentSet-" + StatusId).collapse('hide');
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
                ////console.log(err);
            });
        } else {
            var retweetStatusId = '';
            TwitterService.TwitterIsRetweeted($scope.twitterHomeReplyProfile.AccountName, StatusId).then(function (l) {
                if (l.data != "") {
                    retweetStatusId = l.data;
                    TwitterService.TwitterUnRetweet(replyedTo, retweetStatusId).then(function (result) {
                        if (result.statusText == "OK") {
                            notify({
                                message: "Tweet Unretweeted successfully",
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });
                        }
                    }, function (err) {
                        GlobalService.ErrorPopup("Error", err.data.Message);
                        ////console.log(err);
                    });

                }
            }, function (err) {
            });

        }
    }

    $scope.quickFavouriteTwitterHome = function (StatusId, Id, isUnresponded) {
        //$("#replySet-" + StatusId).collapse('hide');
        var replyedTo = $scope.twitterHomeReplyProfile.AccountName;

        var displayId = '#favourite-'
        if (Id !== undefined) {
            if (isUnresponded) {
                displayId = displayId + 'UR-' + Id;
            }
            else {
                displayId = displayId + Id;
            }
        } else {
            if (isUnresponded) {
                displayId = displayId + 'UR-' + StatusId;
            }
            else {
                displayId = displayId + StatusId;
            }
        }


        if ($(displayId).attr("value") == 'Favourite') {
            TwitterService.TwitterMarkFavorite(replyedTo, StatusId).then(function (result) {
                if (result.statusText == "OK") {
                    notify({
                        message: "Tweet liked successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
            });
        } else {
            TwitterService.TwitterUnmarkFavorite(replyedTo, StatusId).then(function (l) {
                notify({
                    message: "Tweet unliked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
            });
        }

    }

    $scope.quickRetweetWithCommentTwitterHome = function (MsgObj, comment) {
        var StatusId = MsgObj.StatusId;
        var ofUser = MsgObj.ReplyedBy;
        var RetweetComment = comment;
        var feedId = MsgObj.Id;
        var groupId = $rootScope.selectedGroupId;
        var replyedTo = $scope.twitterHomeReplyProfile.AccountName;
        TwitterService.TwitterRetweetWithComment(replyedTo, ofUser, StatusId, RetweetComment, null, $rootScope.selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                notify({
                    message: "Retweet with comment successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            ////console.log(err);
        });
    }

    $scope.openTwitterPostThread = function (tweetId, ofuser, feedId, self) {
        //var ofuser = MsgObj.ReplyedBy; //User
        //var tweetId = MsgObj.StatusId;
        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        TwitterService.TwitterGetTweetThreadByTweetId(ofuser, tweetId).then(function (result) {
            if (result.statusText == "OK") {
                //threadTwitterHome
                if (self == true) {
                    $scope.threadTwitterHome = result.data;
                    $scope.threadTwitterHome.Tweet.fileUploadObject = {};
                    angular.copy(fileUploadObject, $scope.threadTwitterHome.Tweet.fileUploadObject);

                    $scope.ReplyArea = '@' + $scope.threadTwitterHome.Tweet.ReplyedBy + ' ';
                    $scope.RetweetComment = '@' + $scope.threadTwitterHome.Tweet.ReplyedBy + ' ';
                }
                else {
                    $scope.UnrespondedExpansionThread = result.data;
                    $scope.UnrespondedExpansionThread.Tweet.fileUploadObject = {
                    };

                    $scope.UR_ReplyArea = '@' + $scope.UnrespondedExpansionThread.Tweet.ReplyedBy + ' ';
                    $scope.RetweetComment = '@' + $scope.UnrespondedExpansionThread.Tweet.ReplyedBy + ' ';

                    angular.copy(fileUploadObject, $scope.UnrespondedExpansionThread.Tweet.fileUploadObject);
                }
                //console.log(result);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);

            //console.log(err);
        });
    }

    $scope.checkAllTagsTwitterHome = function () {
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedTwitterTags[obj.BrandTypeId] = $scope.selectTwittertags.value;
        });
        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.tagSelectedChangedTwitterHome = function (value, index) {
        if (value == true) {
            $scope.selectTwittertags = false;
        }

        $scope.selectedTwitterTags[index] = !value;

        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.checkAllSentimentTypesTwitterHome = function () {
        angular.forEach($scope.sentimentFilter.sentimentTypesTwitterHome, function (obj) {
            $scope.sentimentFilter.selectedTwitterHomeSentimentTypes[obj.id] = $scope.sentimentFilter.selectTwitterHomeSentimentTypes;
        });
        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    $scope.sentimentTypeSelectedChangedTwitterHome = function () {
        $scope.sentimentFilter.selectTwitterHomeSentimentTypes = false;
        $scope.PageNumberTwitterHome = 0;
        $scope.CounterTwitterHome = ($scope.PageNumberTwitterHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterHome();
    };

    /*-----------------------------------------------*/
    /*-------------REGION TWITTER DM-----------------*/
    /*-----------------------------------------------*/

    $scope.selectedTwitterDM = {};
    $scope.selectTwitterDM = {
        value: true
    };
    $scope.dataTwitterDM = [];
    $scope.PageNumberTwitterDM = 0;
    $scope.newInboxItemsCountTwitterDM = 0;
    $scope.CounterTwitterDM = 0;
    $scope.selectedTwitterTypes = {};
    $scope.selectTwittertypes = true;
    $scope.totalConversationTwitterDM = 2000;//140;
    $scope.twitterDMHistory = {};
    $scope.twitterDMReplyProfile = {
        'AccountName': '',
        'ProfilePicURL': '',
        'MsgObj': {}
    };
    $scope.selectTwitterDMtags = { value: false };
    $scope.selectedTwitterDMTags = {};
    $scope.previewSizeTwitterDM = {
        width: '162px',
        height: '162px'
    }

    // Fetch Twitter DM Data
    $scope.GetUnifiedDeckDataForTwitterDM = function () {
        //Emptying for new data
        if ($scope.PageNumberTwitterDM == 0)
            $scope.dataTwitterDM = [];

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedTwitterDM[record.CompositeId];
        });

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        msgTypeData.push("5"); //5 is for DM msgType

        var sentimentTypeData = [];
        angular.forEach($scope.sentimentFilter.selectedTwitterDMSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var tagTypeData = [];
        var brandTypeData = [];

        var pageNumber = $scope.CounterTwitterDM;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;
        var mediaType = 6; //twitterDMs

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        angular.forEach($scope.selectedTwitterDMTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        UnifiedDeckService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedSentimentTypes, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType).then(function (result) {
            if (result.statusText == "OK" && result.data !== undefined) {
                ////console.log(result);
                if (result.data.TwitterMessageItems.length == 0) {
                    $scope.PageNumberTwitterDM--;
                }
                else {
                    if ($scope.PageNumberTwitterDM == 0) {
                        $scope.dataTwitterDM = result.data.TwitterMessageItems;

                        angular.forEach($scope.dataTwitterDM, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        });

                    } else {
                        angular.forEach(result.data.TwitterMessageItems, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                            $scope.dataTwitterDM.push(obj);
                        });
                    }
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.checkAllProfilesTwitterDM = function () {
        angular.forEach($scope.attachedProfiles, function (obj) {
            if (obj.ProfileTypeId == 4)
                $scope.selectedTwitterDM[obj.CompositeId] = $scope.selectTwitterDM.value;
        });

        $scope.PageNumberTwitterDM = 0;
        $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterDM();
    };

    $scope.profileSelectedChangedTwitterDM = function () {
        $scope.selectTwitterDM.value = false;

        $scope.PageNumberTwitterDM = 0;
        $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterDM();
    };

    //$scope.TwitterDMMessageTypes = [
    //    {
    //        id: "4",
    //        name: "Mentions",
    //        select: false
    //    },
    //    {
    //        id: "3",
    //        name: "Tweets",
    //        select: false
    //    },
    //    {
    //        id: "6",
    //        name: "Retweets",
    //        select: false
    //    }
    //]

    //$scope.checkAllTypesTwitterDM = function () {
    //    angular.forEach($scope.TwitterDMMessageTypes, function (obj) {
    //        $scope.selectedTwitterTypes[obj.id] = $scope.selectTwittertypes;
    //    });
    //    $scope.PageNumberTwitterDM = 0;
    //    $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForTwitterDM();
    //};

    //$scope.typeSelectedChangedTwitterDM = function () {
    //    $scope.selectTwittertypes = false;
    //    $scope.PageNumberTwitterDM = 0;
    //    $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForTwitterDM();
    //};

    $scope.twitterLoadMoreDMs = function () {
        $scope.PageNumberTwitterDM++;
        $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize) + $scope.newInboxItemsCountTwitterDM;
        $scope.GetUnifiedDeckDataForTwitterDM();
    };

    $scope.loadProfileTwitterDM = function (MsgObj, self) {
        var AccountName = MsgObj.ToUser;
        $scope.TMId = MsgObj.Id;
        var replyProfileTwitterDM = $scope.connectedTwitterProfiles.filter(function (profile) {
            if (profile.AccountName == AccountName) {
                return profile.ProfilePicURL;
            }
        });
        var ProfilePicURL = replyProfileTwitterDM[0].ProfilePicURL;

        $scope.twitterDMReplyProfile.AccountName = AccountName;
        $scope.twitterDMReplyProfile.ProfilePicURL = ProfilePicURL;
        $scope.twitterDMReplyProfile.MsgObj = MsgObj;
        //console.log(MsgObj);

        //Emptying New Messages object
        MsgObj.NewCount = 0;
        MsgObj.ClassName = "";

        //if (self == true) {
        //    //document.getElementById("ReplyBoxTwitterDM").focus();
        //    document.getElementById("replyBox-" + $scope.twitterDMReplyProfile.MsgObj.Id).focus();
        //} else if (self == false) {
        //    window.setTimeout(function ()
        //    {
        //        document.getElementById("ReplyBoxTwitterDMUnresponded").focus();
        //    }, 0);
        //}

    }

    $scope.quickReplyTwitterDM = function (ReplyText, MsgObj, self) {
        var ProfileExternalId = MsgObj.UserId;
        var ToUser = MsgObj.ToUser;
        var ofUser = MsgObj.ReplyedBy;
        var selectedCompanyId = $rootScope.selectedCompanyId;
        var feedId = MsgObj.Id;
        var groupId = $rootScope.selectedGroupId;

        TwitterService.TwitterSendDM(ProfileExternalId, ToUser, ofUser, ReplyText, selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                notify({
                    message: "Message sent successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                if (self == true) {
                    $scope.MarkAsCompleted(MsgObj);
                    $scope.ReplyAreaTwitterDMthread = "";
                    $scope.closeNav("TwitterMessage", MsgObj.Id);
                } else if (self == false) {
                    $scope.UnrespondedMarkAsCompleted(MsgObj);
                    $scope.UR_ReplyAreaTwitterDMthread = "";
                    $scope.closeNav("UnrespondedQueries", MsgObj.Id);
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);

            //console.log(err);
        });
    };

    $scope.getTwitterDMHistory = function (ToUser, ofUser, self) {
        //var ToUser = MsgObj.ToUser;
        //var ofUser = MsgObj.replyedBy;
        TwitterService.GetTwitterDirectMessages(ToUser, ofUser).then(function (result) {
            if (result.statusText == "OK") {

                if (self == true) {
                    $scope.twitterDMHistory = result.data;
                    //console.log("Twitter Conversation fetched Successfully");
                    //console.log($scope.twitterDMHistory);
                }
                else {
                    $scope.UnrespondedExpansionThread = result.data;
                    //console.log("Twitter Conversation fetched Successfully");
                    //console.log($scope.UnrespondedExpansionThread);
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching history tweets DM, please try again.");
            //console.log(err);
        });
    }

    $scope.checkAllTagsTwitterDM = function () {
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedTwitterDMTags[obj.BrandTypeId] = $scope.selectTwitterDMtags.value;
        });
        $scope.PageNumberTwitterDM = 0;
        $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterDM();
    };

    $scope.tagSelectedChangedTwitterDM = function (value, index) {
        if (value == true) {
            $scope.selectTwitterDMtags = false;
        }

        $scope.selectedTwitterDMTags[index] = !value;

        $scope.PageNumberTwitterDM = 0;
        $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterDM();
    };

    $scope.checkAllSentimentTypesTwitterDM = function () {
        angular.forEach($scope.sentimentFilter.sentimentTypesTwitterDM, function (obj) {
            $scope.sentimentFilter.selectedTwitterDMSentimentTypes[obj.id] = $scope.sentimentFilter.selectTwitterDMSentimentTypes;
        });
        $scope.PageNumberTwitterDM = 0;
        $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterDM();
    };

    $scope.sentimentTypeSelectedChangedTwitterDM = function () {
        $scope.sentimentFilter.selectTwitterDMSentimentTypes = false;
        $scope.PageNumberTwitterDM = 0;
        $scope.CounterTwitterDM = ($scope.PageNumberTwitterDM * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForTwitterDM();
    };

    /*-----------------------------------------------*/
    /*-------------REGION FACEBOOK WALLPOST----------*/
    /*-----------------------------------------------*/

    $scope.selectedFacebookHome = {};
    $scope.selectFacebookHome = {
        value: true
    };
    $scope.dataFacebookHome = [];
    $scope.PageNumberFacebookHome = 0;
    $scope.newInboxItemsCountFacebookHome = 0;
    $scope.CounterFacebookHome = 0;
    $scope.selectedFacebookTypes = {};
    $scope.selectFacebooktypes = true;
    $scope.totalConversationFacebookWallpost = 2000;
    $scope.facebookWallpostReplyProfile = {
        'AccountName': '',
        'UserId': '',
        'ProfilePicURL': '',
        'IsPage': false
    };
    $scope.ReplyingProfileFacebookWallpost = '';
    $scope.threadFacebookHome = {};
    $scope.selectFacebooktags = { value: false };
    $scope.selectedFacebookTags = {};
    $scope.previewSizeFacebookHome = {
        width: '162px',
        height: '162px'
    }

    // Fetch Facebook Home Data
    $scope.GetUnifiedDeckDataForFacebookHome = function () {
        //Emptying for new data
        if ($scope.PageNumberFacebookHome == 0)
            $scope.dataFacebookHome = [];

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedFacebookHome[record.CompositeId];
        });

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        msgTypeData.push('1');

        var sentimentTypeData = [];
        angular.forEach($scope.sentimentFilter.selectedFacebookHomeSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var tagTypeData = [];
        var brandTypeData = [];

        var pageNumber = $scope.CounterFacebookHome;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;
        var mediaType = 2; //FacebookWallPosts

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        angular.forEach($scope.selectedFacebookTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        UnifiedDeckService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedSentimentTypes, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType).then(function (result) {
            if (result.statusText == "OK" && result.data !== undefined) {
                ////console.log(result);
                if (result.data.FacebookPostItems.length == 0) {
                    $scope.PageNumberFacebookHome--;
                }
                else {
                    if ($scope.PageNumberFacebookHome == 0) {
                        $scope.dataFacebookHome = result.data.FacebookPostItems;
                        angular.forEach($scope.dataFacebookHome, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        });

                    } else {
                        angular.forEach(result.data.FacebookPostItems, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                            $scope.dataFacebookHome.push(obj);
                        });
                    }
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.checkAllProfilesFacebookHome = function () {
        angular.forEach($scope.attachedProfiles, function (obj) {
            if (obj.ProfileTypeId == 1)
                $scope.selectedFacebookHome[obj.CompositeId] = $scope.selectFacebookHome.value;
        });

        $scope.PageNumberFacebookHome = 0;
        $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookHome();
    };

    $scope.profileSelectedChangedFacebookHome = function () {
        $scope.selectFacebookHome.value = false;

        $scope.PageNumberFacebookHome = 0;
        $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookHome();
    };

    //$scope.FacebookHomeMessageTypes = [
    //    {
    //        id: "4",
    //        name: "Mentions",
    //        select: false
    //    },
    //    {
    //        id: "3",
    //        name: "Tweets",
    //        select: false
    //    },
    //    {
    //        id: "6",
    //        name: "Retweets",
    //        select: false
    //    }
    //]

    //$scope.checkAllTypesFacebookHome = function () {
    //    angular.forEach($scope.FacebookHomeMessageTypes, function (obj) {
    //        $scope.selectedFacebookTypes[obj.id] = $scope.selectFacebooktypes;
    //    });
    //    $scope.PageNumberFacebookHome = 0;
    //    $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForFacebookHome();
    //};

    //$scope.typeSelectedChangedFacebookHome = function () {
    //    $scope.selectFacebooktypes = false;
    //    $scope.PageNumberFacebookHome = 0;
    //    $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForFacebookHome();
    //};

    $scope.FacebookLoadMoreFeeds = function () {
        $scope.PageNumberFacebookHome++;
        $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize) + $scope.newInboxItemsCountFacebookHome;
        $scope.GetUnifiedDeckDataForFacebookHome();
    };

    $scope.loadProfileFacebookWallpost = function (AccountName, UserId, ProfilePicURL, IsPage) {
        $scope.ReplyingProfileFacebookWallpost = 'ReplyingProfileFacebookWallpost-' + UserId;
        $scope.facebookWallpostReplyProfile.AccountName = AccountName;
        $scope.facebookWallpostReplyProfile.UserId = UserId;
        $scope.facebookWallpostReplyProfile.ProfilePicURL = ProfilePicURL;
        $scope.facebookWallpostReplyProfile.IsPage = IsPage;
    }

    $scope.quickReplyFacebookWallpost = function (ReplyText, MsgObj, isUnresponded) {
        var ProfileExternalId = $scope.facebookWallpostReplyProfile.UserId; //commented on 09 Jan ,2018 for Telenor lead working
        var IsPage = $scope.facebookWallpostReplyProfile.IsPage;
        var StatusId = MsgObj.StatusId;
        var selectedCompanyId = $rootScope.selectedCompanyId;
        var feedId = MsgObj.Id;
        var groupId = $rootScope.selectedGroupId;
        var filePath = '';
        var contentType = '';
        var fileName = '';

        if (MsgObj.fileUploadObject != undefined) {
            if (MsgObj.fileUploadObject.mediaData.length > 0) {
                filePath = MsgObj.fileUploadObject.mediaData[0].Base64String;
                contentType = MsgObj.fileUploadObject.mediaData[0].ContentType;
                fileName = MsgObj.fileUploadObject.mediaData[0].FileName;
            }
        }
        FacebookService.FacebookCommentPost(ProfileExternalId, IsPage, StatusId, ReplyText, filePath, contentType, fileName, selectedCompanyId, 2).then(function (result) {
            if (result.statusText == "OK") {
                notify({
                    message: "Posted successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                if (isUnresponded !== undefined && isUnresponded == true) {
                    if (angular.equals($scope.OpeningThreadObjects.Unresponded, {})) {
                        $scope.ReplyAreaUnresponded = "";
                        $scope.UnrespondedMarkAsCompleted(MsgObj);//$scope.MarkAsCompletedUnresponded(MsgObj);
                    } else {
                        $scope.UR_ReplyAreaFacebookWallpost = "";
                        $scope.UnrespondedMarkAsCompleted($scope.OpeningThreadObjects.Unresponded, "UnrespondedQueries");
                    }
                } else {
                    $scope.ReplyAreaFacebookWallpost = "";
                    if (angular.equals($scope.OpeningThreadObjects.FacebookHome, {})) {
                        $scope.MarkAsCompleted(MsgObj);
                    } else {
                        $scope.MarkAsCompleted($scope.OpeningThreadObjects.FacebookHome, "FacebookPost");
                    }
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            //alert('Error in post tweet, please try again.');
            //console.log(err);
        });

    }

    $scope.quickLikeFacebookWallpost = function (MsgObj) {
        var ProfileExternalId = $scope.facebookWallpostReplyProfile.UserId; //commented on 09 Jan ,2018 for Telenor lead working
        var IsPage = $scope.facebookWallpostReplyProfile.IsPage;
        var statusId = MsgObj.StatusId;

        if ($('#like-' + statusId).attr("value") == 'Like') {
            FacebookService.FacebookLikePost(ProfileExternalId, IsPage, statusId, false).then(function (result) {
                if (result.statusText == "OK") {
                    notify({
                        message: "Comment Liked successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
            });
        } else {
            FacebookService.FacebookUnLikePost(ProfileExternalId, IsPage, statusId).then(function (l) {
                $scope.isLiked = false;
                notify({
                    message: "FB unliked successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
            });
        }


    }

    $scope.openFacebookPostThread = function (PostId, self) {
        //var ofuser = MsgObj.ReplyedBy; //User
        //var tweetId = MsgObj.StatusId;
        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        FacebookService.FacebookGetPostInfo(PostId, '0').then(function (result) {
            if (result.statusText == "OK") {

                if (self == true) {
                    $scope.threadFacebookHome = result.data;
                    $scope.threadFacebookHome.fbPost.fileUploadObject = {};
                    angular.copy(fileUploadObject, $scope.threadFacebookHome.fbPost.fileUploadObject);
                }
                else {
                    $scope.UnrespondedExpansionThread = result.data;
                    $scope.UnrespondedExpansionThread.fbPost.fileUploadObject = {};
                    angular.copy(fileUploadObject, $scope.UnrespondedExpansionThread.fbPost.fileUploadObject);
                }
                //console.log(result);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);

            //console.log(err);
        });
    }

    $scope.checkAllTagsFacebookHome = function () {
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedFacebookTags[obj.BrandTypeId] = $scope.selectFacebooktags.value;
        });
        $scope.PageNumberFacebookHome = 0;
        $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookHome();
    };

    $scope.tagSelectedChangedFacebookHome = function (value, index) {
        if (value == true) {
            $scope.selectFacebooktags = false;
        }

        $scope.selectedFacebookTags[index] = !value;

        $scope.PageNumberFacebookHome = 0;
        $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookHome();
    };

    $scope.checkAllSentimentTypesFacebookHome = function () {
        angular.forEach($scope.sentimentFilter.sentimentTypesFacebookHome, function (obj) {
            $scope.sentimentFilter.selectedFacebookHomeSentimentTypes[obj.id] = $scope.sentimentFilter.selectFacebookHomeSentimentTypes;
        });
        $scope.PageNumberFacebookHome = 0;
        $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookHome();
    };

    $scope.sentimentTypeSelectedChangedFacebookHome = function () {
        $scope.sentimentFilter.selectFacebookHomeSentimentTypes = false;
        $scope.PageNumberFacebookHome = 0;
        $scope.CounterFacebookHome = ($scope.PageNumberFacebookHome * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookHome();
    };

    /*-----------------------------------------------*/
    /*-------------REGION FACEBOOK COMMENTS----------*/
    /*-----------------------------------------------*/

    $scope.selectedFacebookComment = {};
    $scope.selectFacebookComment = {
        value: true
    };
    $scope.dataFacebookComment = [];
    $scope.PageNumberFacebookComment = 0;
    $scope.newInboxItemsCountFacebookComment = 0;
    $scope.CounterFacebookComment = 0;
    $scope.selectedFacebookTypes = {};
    $scope.selectFacebooktypesComments = true;
    $scope.totalConversationFacebookComment = 2000;
    $scope.facebookCommentReplyProfile = {
        'AccountName': '',
        'UserId': '',
        'ProfilePicURL': '',
        'IsPage': false
    };
    $scope.ReplyingProfileFacebookComment = '';
    $scope.threadFacebookComment = {};
    $scope.selectFacebookCommenttags = { value: false };
    $scope.selectedFacebookCommentTags = {};
    $scope.previewSizeFacebookComment = {
        width: '162px',
        height: '162px'
    }

    // Fetch Facebook Comment Data
    $scope.GetUnifiedDeckDataForFacebookComment = function () {
        //Emptying for new data
        if ($scope.PageNumberFacebookComment == 0)
            $scope.dataFacebookComment = [];

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedFacebookComment[record.CompositeId];
        });

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        //angular.forEach($scope.selectedFacebookTypes, function (obj, key) {
        //    if (obj == true) {
        //        msgTypeData.push(key);
        //    }
        //});
        msgTypeData.push('2');

        var sentimentTypeData = [];
        angular.forEach($scope.sentimentFilter.selectedFacebookCommentSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var tagTypeData = [];
        var brandTypeData = [];

        var pageNumber = $scope.CounterFacebookComment;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;
        var mediaType = 3; //FacebookWallPosts

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        angular.forEach($scope.selectedFacebookCommentTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        UnifiedDeckService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedSentimentTypes, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType).then(function (result) {
            if (result.statusText == "OK" && result.data !== undefined) {
                // //console.log(result);
                if (result.data.FacebookCommentItems.length == 0) {
                    $scope.PageNumberFacebookComment--;
                }
                else {
                    if ($scope.PageNumberFacebookComment == 0) {
                        $scope.dataFacebookComment = result.data.FacebookCommentItems;

                        angular.forEach($scope.dataFacebookComment, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        });

                    } else {
                        angular.forEach(result.data.FacebookCommentItems, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                            $scope.dataFacebookComment.push(obj);
                        });
                    }
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.checkAllProfilesFacebookComment = function () {
        angular.forEach($scope.attachedProfiles, function (obj) {
            if (obj.ProfileTypeId == 1)
                $scope.selectedFacebookComment[obj.CompositeId] = $scope.selectFacebookComment.value;
        });

        $scope.PageNumberFacebookComment = 0;
        $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookComment();
    };

    $scope.profileSelectedChangedFacebookComment = function () {
        $scope.selectFacebookComment.value = false;

        $scope.PageNumberFacebookComment = 0;
        $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookComment();
    };

    //$scope.FacebookCommentMessageTypes = [
    //    {
    //        id: "4",
    //        name: "Mentions",
    //        select: false
    //    },
    //    {
    //        id: "3",
    //        name: "Tweets",
    //        select: false
    //    },
    //    {
    //        id: "6",
    //        name: "Retweets",
    //        select: false
    //    }
    //]

    //$scope.checkAllTypesFacebookComment = function () {
    //    angular.forEach($scope.FacebookCommentMessageTypes, function (obj) {
    //        $scope.selectedFacebookTypes[obj.id] = $scope.selectFacebooktypes;
    //    });
    //    $scope.PageNumberFacebookComment = 0;
    //    $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForFacebookComment();
    //};

    //$scope.typeSelectedChangedFacebookComment = function () {
    //    $scope.selectFacebooktypes = false;
    //    $scope.PageNumberFacebookComment = 0;
    //    $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForFacebookComment();
    //};

    $scope.FacebookLoadMoreComments = function () {
        $scope.PageNumberFacebookComment++;
        $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize) + $scope.newInboxItemsCountFacebookComment;
        $scope.GetUnifiedDeckDataForFacebookComment();
    };

    $scope.loadProfileFacebookComment = function (AccountName, UserId, ProfilePicURL, IsPage) {
        $scope.ReplyingProfileFacebookWallpost = 'ReplyingProfileFacebookComment-' + UserId;
        $scope.facebookCommentReplyProfile.AccountName = AccountName;
        $scope.facebookCommentReplyProfile.UserId = UserId;
        $scope.facebookCommentReplyProfile.ProfilePicURL = ProfilePicURL;
        $scope.facebookCommentReplyProfile.IsPage = IsPage;
    }

    $scope.quickReplyFacebookComment = function (ReplyText, MsgObj, isUnresponded) {
        var userId=$scope.facebookCommentReplyProfile.UserId;
        var ProfileExternalId = $scope.facebookCommentReplyProfile.UserId; //commented on 09 Jan ,2018 for Telenor lead working
        var IsPage = $scope.facebookCommentReplyProfile.IsPage;

        var useableStatusId = '';
        var StatusId = '';
        var feedId = '';
        if (MsgObj.StatusId === undefined) {
            useableStatusId = MsgObj.Id;
            StatusId = MsgObj.Id.split('|')[0];
            feedId = MsgObj.FeedId;
        } else {
            useableStatusId = MsgObj.StatusId;
            StatusId = MsgObj.StatusId.split('|')[0];
            feedId = MsgObj.Id;
        }

        var groupId = $rootScope.selectedGroupId;
        var replyType = 2;
        if (useableStatusId.split('|').length == 3) {
            StatusId = useableStatusId.split('|')[1];
            replyType = 3;
        } 582
        var selectedCompanyId = $rootScope.selectedCompanyId;
        var filePath = '';
        var contentType = '';
        var fileName = '';

        if (MsgObj.fileUploadObject != undefined)
            if (MsgObj.fileUploadObject.mediaData.length > 0) {
                filePath = MsgObj.fileUploadObject.mediaData[0].Base64String;
                contentType = MsgObj.fileUploadObject.mediaData[0].ContentType;
                fileName = MsgObj.fileUploadObject.mediaData[0].FileName;
            }
        FacebookService.FacebookCommentPost(ProfileExternalId, IsPage, StatusId, ReplyText, filePath, contentType, fileName, selectedCompanyId, replyType).then(function (result) {
            if (result.statusText == "OK") {
                notify({
                    message: "Posted successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                if (isUnresponded !== undefined && isUnresponded == true) {
                    if (angular.equals($scope.OpeningThreadObjects.Unresponded, {})) {
                        $scope.UnrespondedMarkAsCompleted(MsgObj);
                        $scope.ReplyAreaUnresponded = "";
                    } else {
                        $scope.UnrespondedMarkAsCompleted($scope.OpeningThreadObjects.Unresponded, "UnrespondedQueries");
                        $scope.UR_ReplyAreaFacebookComment = "";
                    }
                } else {
                    $scope.ReplyAreaFacebookComment = "";
                    if (angular.equals($scope.OpeningThreadObjects.FacebookComment, {})) {
                        $scope.MarkAsCompleted(MsgObj);
                    } else {
                        $scope.MarkAsCompleted($scope.OpeningThreadObjects.FacebookComment, "FacebookComment");
                    }
                }

                //if(MsgObj.MessageType === undefined){
                //    $scope.MarkAsCompleted({"Id": MsgObj.feedId, "MessageType": MsgObj.MsgType, "UserTwit": MsgObj.Msg, "StatusId": MsgObj.Id, "UserId": MsgObj.AuthorId});
                //} else{
                //    $scope.MarkAsCompleted(MsgObj);                    
                //}                
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            //alert('Error in post tweet, please try again.');
            //console.log(err);
        });

    }

    $scope.quickLikeFacebookComment = function (MsgObj, isNested) {
        var ProfileExternalId = $scope.facebookCommentReplyProfile.UserId; 
        var IsPage = $scope.facebookCommentReplyProfile.IsPage;
        var statusId = '';
        var feedId = '';

        if (isNested) {
            statusId = MsgObj.Id.split('|')[0];
            feedid = MsgObj.FeedId;
        } else {
            statusId = MsgObj.StatusId.split('|')[0];
        }

        if (!isNested) {
            if ($('#like-' + statusId).attr("value") == 'Like') {
                FacebookService.FacebookLikePost(ProfileExternalId, IsPage, statusId, false).then(function (result) {
                    if (result.statusText == "OK") {
                        notify({
                            message: "Comment Liked successfully",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                    }
                }, function (err) {
                    GlobalService.ErrorPopup("Error", err.data.Message);
                });
            }
            else {
                FacebookService.FacebookUnLikePost(ProfileExternalId, IsPage, statusId, false).then(function (l) {
                    notify({
                        message: "FB unliked successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }, function (err) {
                    GlobalService.ErrorPopup("Error", err.data.Message);

                });
            }
        }
        else {
            if ($('#like-' + feedid).attr("value") == 'Like') {
                FacebookService.FacebookLikePost(ProfileExternalId, IsPage, statusId, false).then(function (result) {
                    if (result.statusText == "OK") {
                        notify({
                            message: "Comment Liked successfully",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                    }
                }, function (err) {
                    GlobalService.ErrorPopup("Error", err.data.Message);
                });
            }
            else {
                FacebookService.FacebookUnLikePost(ProfileExternalId, IsPage, statusId, false).then(function (l) {
                    notify({
                        message: "FB unliked successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }, function (err) {
                    GlobalService.ErrorPopup("Error", err.data.Message);

                });
            }
        }
    }

    $scope.openFacebookCommentThread = function (PostId, self) {
        //var ofuser = MsgObj.ReplyedBy; //User
        //var tweetId = MsgObj.StatusId;
        var sendingPostId = PostId;
        var sendingCommentId = '0';
        var isNestedComment = false;
        if (PostId.split('|').length == 2) {
            sendingPostId = PostId.split('|')[1];
            sendingCommentId = PostId.split('|')[0];
        }
        else if (PostId.split('|').length == 3) {
            sendingPostId = PostId.split('|')[2];
            sendingCommentId = PostId.split('|')[1];
            isNestedComment = true;
        }
        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        FacebookService.FacebookGetPostInfo(sendingPostId, sendingCommentId).then(function (result) {
            if (result.statusText == "OK") {

                var fetchedData = result.data.fbPostComments;
                $scope.FCId = fetchedData[0].FeedId;

                if (self == true) {
                    $scope.threadFacebookComment.Post = result.data.fbPost;
                    if (!isNestedComment) {
                        for (var i = 0; i < fetchedData.length; i++) {
                            if (fetchedData[i].Id == PostId.split('|')[0]) {
                                $scope.threadFacebookComment.Comments = fetchedData[i];
                                break;
                            }
                        }
                    } else {
                        for (var i = 0; i < fetchedData.length; i++) {
                            if (fetchedData[i].Id == PostId.split('|')[1]) {
                                $scope.threadFacebookComment.Comments = fetchedData[i];
                                break;
                            }
                        }
                    }

                    $scope.threadFacebookComment.Comments.fileUploadObject = {};
                    angular.copy(fileUploadObject, $scope.threadFacebookComment.Comments.fileUploadObject);
                }
                else {
                    $scope.UnrespondedExpansionThread.Post = result.data.fbPost;
                    if (!isNestedComment) {
                        for (var i = 0; i < fetchedData.length; i++) {
                            if (fetchedData[i].Id == PostId.split('|')[0]) {
                                $scope.UnrespondedExpansionThread.Comments = fetchedData[i];
                                break;
                            }
                        }
                    } else {
                        for (var i = 0; i < fetchedData.length; i++) {
                            if (fetchedData[i].Id == PostId.split('|')[1]) {
                                $scope.UnrespondedExpansionThread.Comments = fetchedData[i];
                                break;
                            }
                        }
                    }

                    $scope.UnrespondedExpansionThread.Comments.fileUploadObject = {};
                    angular.copy(fileUploadObject, $scope.UnrespondedExpansionThread.Comments.fileUploadObject);
                }
                ////console.log($scope.threadFacebookComment);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);

            //console.log(err);
        });
    }

    $scope.checkAllTagsFacebookComment = function () {
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedFacebookCommentTags[obj.BrandTypeId] = $scope.selectFacebookCommenttags.value;
        });
        $scope.PageNumberFacebookComment = 0;
        $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookComment();
    };

    $scope.tagSelectedChangedFacebookComment = function (value, index) {
        if (value == true) {
            $scope.selectFacebookCommenttags = false;
        }

        $scope.selectedFacebookCommentTags[index] = !value;

        $scope.PageNumberFacebookComment = 0;
        $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookComment();
    };

    $scope.checkAllSentimentTypesFacebookComment = function () {
        angular.forEach($scope.sentimentFilter.sentimentTypesFacebookComment, function (obj) {
            $scope.sentimentFilter.selectedFacebookCommentSentimentTypes[obj.id] = $scope.sentimentFilter.selectFacebookCommentSentimentTypes;
        });
        $scope.PageNumberFacebookComment = 0;
        $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookComment();
    };

    $scope.sentimentTypeSelectedChangedFacebookComment = function () {
        $scope.sentimentFilter.selectFacebookCommentSentimentTypes = false;
        $scope.PageNumberFacebookComment = 0;
        $scope.CounterFacebookComment = ($scope.PageNumberFacebookComment * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookComment();
    };

    /*-----------------------------------------------*/
    /*-------------REGION FACEBOOK DM----------------*/
    /*-----------------------------------------------*/

    $scope.selectedFacebookChat = {};
    $scope.selectFacebookChat = {
        value: true
    };
    $scope.dataFacebookChat = [];
    $scope.PageNumberFacebookChat = 0;
    $scope.newInboxItemsCountFacebookChat = 0;
    $scope.CounterFacebookChat = 0;
    $scope.selectedFacebookTypes = {};
    $scope.selectFacebooktypes = true;
    $scope.totalConversationFacebookChat = 2000;//140;
    $scope.facebookChatReplyProfile = {
        'AccountName': '',
        'ProfilePicURL': '',
        'MsgObj': {}
    };
    $scope.facebookChatHistory = {};
    $scope.selectFacebookDMtags = { value: false };
    $scope.selectedFacebookDMTags = {};
    $scope.previewSizeFacebookDM = {
        width: '162px',
        height: '162px'
    }

    // Fetch Facebook Chat Data
    $scope.GetUnifiedDeckDataForFacebookChat = function () {
        //Emptying for new data
        if ($scope.PageNumberFacebookChat == 0)
            $scope.dataFacebookChat = [];

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedFacebookChat[record.CompositeId];
        });

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        //angular.forEach($scope.selectedFacebookTypes, function (obj, key) {
        //    if (obj == true) {
        //        msgTypeData.push(key);
        //    }
        //});
        msgTypeData.push('7');

        var sentimentTypeData = [];
        angular.forEach($scope.sentimentFilter.selectedFacebookChatSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var tagTypeData = [];
        var brandTypeData = [];

        var pageNumber = $scope.CounterFacebookChat;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;
        var mediaType = 4; //FacebookChats

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        angular.forEach($scope.selectedFacebookDMTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        UnifiedDeckService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedSentimentTypes, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType).then(function (result) {
            if (result.statusText == "OK" && result.data !== undefined) {
                // //console.log(result);
                if (result.data.FacebookPageChatItems.length == 0) {
                    $scope.PageNumberFacebookChat--;
                }
                else {
                    if ($scope.PageNumberFacebookChat == 0) {
                        $scope.dataFacebookChat = result.data.FacebookPageChatItems;

                        angular.forEach($scope.dataFacebookChat, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        });

                    } else {
                        angular.forEach(result.data.FacebookPageChatItems, function (obj) {
                            //obj.fileUploadObject = fileUploadObject;
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                            $scope.dataFacebookChat.push(obj);
                        });
                    }
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.checkAllProfilesFacebookChat = function () {
        angular.forEach($scope.attachedProfiles, function (obj) {
            if (obj.ProfileTypeId == 1)
                $scope.selectedFacebookChat[obj.CompositeId] = $scope.selectFacebookChat.value;
        });

        $scope.PageNumberFacebookChat = 0;
        $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookChat();
    };

    $scope.profileSelectedChangedFacebookChat = function () {
        $scope.selectFacebookChat.value = false;

        $scope.PageNumberFacebookChat = 0;
        $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookChat();
    };

    $scope.loadProfileFacebookChat = function (MsgObj, self) {
        var AccountName = MsgObj.ToUser;
        $scope.FCPId = MsgObj.Id;
        var replyProfileFacebookChat = $scope.connectedFacebookPages.filter(function (profile) {
            if (profile.AccountName == AccountName) {
                return profile.ProfilePicURL;
            }
        });
        var ProfilePicURL = replyProfileFacebookChat[0].ProfilePicURL;

        $scope.facebookChatReplyProfile.AccountName = AccountName;
        $scope.facebookChatReplyProfile.ProfilePicURL = ProfilePicURL;
        $scope.facebookChatReplyProfile.MsgObj = MsgObj;

        //Emptying New Messages object
        MsgObj.NewCount = 0;
        MsgObj.ClassName = "";

        //if (self == true) {            
        //    //document.getElementById("ReplyBoxFacebookChat").focus();
        //    document.getElementById("replyBox-" + $scope.facebookChatReplyProfile.MsgObj.Id).focus(); // document.getElementById("replyBox-" + $scope.facebookChatReplyProfile.MsgObj.StatusId).focus();
        //} else if (self == false) {            
        //    window.setTimeout(function () {
        //        document.getElementById("ReplyBoxFacebookChatUnresponded").focus();
        //         // document.getElementById("ReplyBoxFacebookChatUnresponded"+ $scope.facebookChatReplyProfile.MsgObj.StatusId).focus();
        //    }, 0);
        //}
    }

    //$scope.FacebookChatMessageTypes = [
    //    {
    //        id: "4",
    //        name: "Mentions",
    //        select: false
    //    },
    //    {
    //        id: "3",
    //        name: "Tweets",
    //        select: false
    //    },
    //    {
    //        id: "6",
    //        name: "Retweets",
    //        select: false
    //    }
    //]

    //$scope.checkAllTypesFacebookChat = function () {
    //    angular.forEach($scope.FacebookChatMessageTypes, function (obj) {
    //        $scope.selectedFacebookTypes[obj.id] = $scope.selectFacebooktypes;
    //    });
    //    $scope.PageNumberFacebookChat = 0;
    //    $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForFacebookChat();
    //};

    //$scope.typeSelectedChangedFacebookChat = function () {
    //    $scope.selectFacebooktypes = false;
    //    $scope.PageNumberFacebookChat = 0;
    //    $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
    //    if ($scope.firstTime == false)
    //        $scope.GetUnifiedDeckDataForFacebookChat();
    //};

    $scope.FacebookLoadMoreChats = function () {
        $scope.PageNumberFacebookChat++;
        $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize) + $scope.newInboxItemsCountFacebookChat;
        $scope.GetUnifiedDeckDataForFacebookChat();
    };

    $scope.quickReplyFacebookChat = function (MsgObj, ReplyText, self) {
        var ProfileExternalId = MsgObj.UserId;
        var ToUser = MsgObj.ToUser;
        var IsPage = MsgObj.IsPage;
        var selectedCompanyId = $rootScope.selectedCompanyId;
        var StatusId = MsgObj.StatusId;
        var feedId = MsgObj.Id;
        var groupId = $rootScope.selectedGroupId;

        FacebookService.FacebookPostPageChatMessage(ProfileExternalId, StatusId, IsPage, ReplyText, selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                notify({
                    message: "Message sent successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $scope.showTextBox(feedId);

                if (self == true) {
                    $scope.MarkAsCompleted(MsgObj);
                    $scope.ReplyAreaFacebookChatthread = "";
                    $scope.closeNav("FacebookChat", MsgObj.Id);
                } else if (self == false) {
                    $scope.UnrespondedMarkAsCompleted(MsgObj);
                    $scope.UR_ReplyAreaFacebookChatthread = "";
                    $scope.closeNav("UnrespondedQueries", MsgObj.Id);
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error occurred while sending message", err.data.Message);
            //console.log(err);
        });
    };

    $scope.getFacebookChatHistory = function (StatusId, self) {
        FacebookService.FacebookGetPageChatConversation(StatusId).then(function (result) {
            if (result.statusText == "OK") {

                if (self == true) {
                    $scope.facebookChatHistory = result.data;
                    //console.log($scope.facebookChatHistory);
                }
                else {
                    $scope.UnrespondedExpansionThread = result.data;
                    //console.log($scope.UnrespondedExpansionThread);
                }
                //console.log("Facebook Conversations fetched Successfully");
            }
        }, function (err) {
            //alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");
            //console.log(err);
        });
    }

    $scope.checkAllTagsFacebookDM = function () {
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedFacebookDMTags[obj.BrandTypeId] = $scope.selectFacebookDMtags.value;
        });
        $scope.PageNumberFacebookChat = 0;
        $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookChat();
    };

    $scope.tagSelectedChangedFacebookDM = function (value, index) {
        if (value == true) {
            $scope.selectFacebookDMtags = false;
        }

        $scope.selectedFacebookDMTags[index] = !value;

        $scope.PageNumberFacebookChat = 0;
        $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookChat();
    };

    $scope.checkAllSentimentTypesFacebookChat = function () {
        angular.forEach($scope.sentimentFilter.sentimentTypesFacebookChat, function (obj) {
            $scope.sentimentFilter.selectedFacebookChatSentimentTypes[obj.id] = $scope.sentimentFilter.selectFacebookChatSentimentTypes;
        });
        $scope.PageNumberFacebookChat = 0;
        $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookChat();
    };

    $scope.sentimentTypeSelectedChangedFacebookChat = function () {
        $scope.sentimentFilter.selectFacebookChatSentimentTypes = false;
        $scope.PageNumberFacebookChat = 0;
        $scope.CounterFacebookChat = ($scope.PageNumberFacebookChat * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForFacebookChat();
    };

    /*-----------------------------------------------*/
    /*-------------REGION UNRESPONDED QUERIES--------*/
    /*-----------------------------------------------*/

    $scope.selectedUnrespondedQueries = {};
    $scope.selectUnrespondedQueries = {
        value: true
    };

    $scope.UnrespondedQueriesMessageTypes = [];

    $scope.selectedUnrespondedTypes = {};
    $scope.selectUnrespondedtypes = true;

    $scope.dataUnrespondedQueries = [];
    $scope.PageNumberUnresponded = 0;
    $scope.selectUnrespondedQueriestags = { value: false };
    $scope.selectedUnrespondedQueriesTags = {};
    $scope.previewSizeUnrespondedQueries = {
        width: '162px',
        height: '162px'
    }
    $scope.CounterUnrespondedQueries = 0;
    $scope.newInboxItemsCountUnresponded = 0;

    $scope.GetUnifiedDeckDataForUnrespondedQueries = function () {

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedUnrespondedQueries[record.CompositeId];
        });

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        angular.forEach($scope.selectedUnrespondedTypes, function (obj, key) {
            if (obj == true) {
                msgTypeData.push(key);
            }
        });

        var sentimentTypeData = [];
        angular.forEach($scope.sentimentFilter.selectedUnrespondedQueriesSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var tagTypeData = [];
        angular.forEach($scope.selectedUnrespondedQueriesTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        var brandTypeData = [];

        var pageNumber = $scope.CounterUnrespondedQueries;//0;//$scope.CounterTwitterHome;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        UnifiedDeckService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedSentimentTypes, pageNumber, pageSize, groupId, companyId, isAllTask, 1).then(function (result) {
            if (result.statusText == "OK" && result.data != null) {
                // //console.log(result);
                //Emptying
                $scope.dataUnrespondedQueries = [];

                //Unresponded Queries
                angular.forEach(result.data.UnrespondedItems, function (obj) {
                    //obj.fileUploadObject = fileUploadObject;
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                    $scope.dataUnrespondedQueries.push(obj);
                });

                if ($scope.dataUnrespondedQueries.length > 0) {
                    $scope.recentUnrespondedId = Math.max.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) > $scope.recentUnrespondedId ? Math.max.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) : $scope.recentUnrespondedId;
                    $scope.oldestUnrespondedId = Math.min.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) > $scope.oldestUnrespondedId ? Math.min.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) : $scope.oldestUnrespondedId;

                    //For colours initially
                    RecheckColours();
                }
                //setTimeout(function () { loadCursor(); }, 5000);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });

    }

    $scope.checkAllProfilesUnrespondedQueries = function (value) {
        angular.forEach($scope.attachedProfiles, function (obj) {
            $scope.selectedUnrespondedQueries[obj.CompositeId] = $scope.selectUnrespondedQueries.value;
        });

        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForUnrespondedQueries();
    };

    $scope.profileSelectedChangedUnrespondedQueries = function () {
        $scope.selectUnrespondedQueries.value = false;

        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForUnrespondedQueries();
    };

    $scope.checkAllTypesUnrespondedQueries = function () {
        angular.forEach($scope.UnrespondedQueriesMessageTypes, function (obj) {
            $scope.selectedUnrespondedTypes[obj.id] = $scope.selectUnrespondedtypes;
        });
        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForUnrespondedQueries();
    };

    $scope.typeSelectedChangedUnrespondedQueries = function () {
        $scope.selectUnrespondedtypes = false;
        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForUnrespondedQueries();
    };

    function getMinimumIds() {

        if ($scope.dataTwitterHome.length > 0) {
            $scope.oldestPostsTWId = Math.min.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) < $scope.oldestPostsTWId ? Math.min.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) : $scope.oldestPostsTWId;
        } else $scope.oldestPostsTWId = 0;

        if ($scope.dataTwitterDM.length > 0) {
            $scope.oldestMessagesTWId = Math.min.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) < $scope.oldestMessagesTWId ? Math.min.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) : $scope.oldestMessagesTWId;
        } else $scope.oldestMessagesTWId = 0;

        if ($scope.dataFacebookHome.length > 0) {
            $scope.oldestPostsFBId = Math.min.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) < $scope.oldestPostsFBId ? Math.min.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) : $scope.oldestPostsFBId;
        } else $scope.oldestPostsFBId = 0;

        if ($scope.dataFacebookComment.length > 0) {
            $scope.oldestCommentsFBId = Math.min.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) < $scope.oldestCommentsFBId ? Math.min.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) : $scope.oldestCommentsFBId;
        } else $scope.oldestCommentsFBId = 0;

        if ($scope.dataFacebookChat.length > 0) {
            $scope.oldestPageChatsFBId = Math.min.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) < $scope.oldestPageChatsFBId ? Math.min.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) : $scope.oldestPageChatsFBId;
        } else $scope.oldestPageChatsFBId = 0;

    }

    function getMaximumIdsFromUnresponded() {

        var unrespondedTwitterData = [];
        var unrespondedTwitterDMData = [];
        var unrespondedFacebookData = [];
        var unrespondedFacebookCommentData = [];
        var unrespondedFacebookChatData = [];


        for (var i = 0; i < $scope.dataUnrespondedQueries.length; i++) {
            if ($scope.dataUnrespondedQueries[i].MessageType == "TP") {
                unrespondedTwitterData.push($scope.dataUnrespondedQueries[i]);
            }
            else if ($scope.dataUnrespondedQueries[i].MessageType == "TM") {
                unrespondedTwitterDMData.push($scope.dataUnrespondedQueries[i]);
            }
            else if ($scope.dataUnrespondedQueries[i].MessageType == "FP") {
                unrespondedFacebookData.push($scope.dataUnrespondedQueries[i]);
            }
            else if ($scope.dataUnrespondedQueries[i].MessageType == "FC") {
                unrespondedFacebookCommentData.push($scope.dataUnrespondedQueries[i]);
            }
            else if ($scope.dataUnrespondedQueries[i].MessageType == "FCP") {
                unrespondedFacebookChatData.push($scope.dataUnrespondedQueries[i]);
            }
        }
        $scope.UnrespondedRecentIds.recentPostsTWId = Math.max.apply(Math, unrespondedTwitterData.map(function (o) { return o.Id; })) > $scope.UnrespondedRecentIds.recentPostsTWId ? Math.max.apply(Math, unrespondedTwitterData.map(function (o) { return o.Id; })) : $scope.UnrespondedRecentIds.recentPostsTWId;
        $scope.UnrespondedRecentIds.recentMessagesTWId = Math.max.apply(Math, unrespondedTwitterDMData.map(function (o) { return o.Id; })) > $scope.UnrespondedRecentIds.recentMessagesTWId ? Math.max.apply(Math, unrespondedTwitterDMData.map(function (o) { return o.Id; })) : $scope.UnrespondedRecentIds.recentMessagesTWId;
        $scope.UnrespondedRecentIds.recentPostsFBId = Math.max.apply(Math, unrespondedFacebookData.map(function (o) { return o.Id; })) > $scope.UnrespondedRecentIds.recentPostsFBId ? Math.max.apply(Math, unrespondedFacebookData.map(function (o) { return o.Id; })) : $scope.UnrespondedRecentIds.recentPostsFBId;
        $scope.UnrespondedRecentIds.recentCommentsFBId = Math.max.apply(Math, unrespondedFacebookCommentData.map(function (o) { return o.Id; })) > $scope.UnrespondedRecentIds.recentCommentsFBId ? Math.max.apply(Math, unrespondedFacebookCommentData.map(function (o) { return o.Id; })) : $scope.UnrespondedRecentIds.recentCommentsFBId;
        $scope.UnrespondedRecentIds.recentPageChatsFBId = Math.max.apply(Math, unrespondedFacebookChatData.map(function (o) { return o.Id; })) > $scope.UnrespondedRecentIds.recentPageChatsFBId ? Math.max.apply(Math, unrespondedFacebookChatData.map(function (o) { return o.Id; })) : $scope.UnrespondedRecentIds.recentPageChatsFBId;
    }

    $scope.DataProcessorForLatestUnrespondedPosts = function (isMarkAllasCompleted) {
        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedUnrespondedQueries[record.CompositeId];
        });

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        angular.forEach($scope.selectedUnrespondedTypes, function (obj, key) {
            if (obj == true) {
                msgTypeData.push(key);
            }
        });

        var sentimentTypeData = [];
        angular.forEach($scope.sentimentFilter.selectedUnrespondedQueriesSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var tagTypeData = [];
        angular.forEach($scope.selectedUnrespondedQueriesTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        var brandTypeData = [];
        var pageNumber = $scope.PageNumberUnresponded;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        //getMinimumIds();
        getMaximumIdsFromUnresponded();

        //New Implementation
        var ids = $scope.UnrespondedRecentIds.recentMessagesTWId + '|' + $scope.UnrespondedRecentIds.recentPageChatsFBId + '|' + $scope.UnrespondedRecentIds.recentPostsFBId + '|' + $scope.UnrespondedRecentIds.recentPostsTWId + '|' + $scope.UnrespondedRecentIds.recentCommentsFBId;
        //var old_ids = $scope.oldestMessagesTWId + '|' + $scope.oldestPageChatsFBId + '|' + $scope.oldestPostsFBId + '|' + $scope.oldestPostsTWId + '|' + $scope.oldestCommentsFBId;

        var MarkAllAsCompleted = false;
        if (isMarkAllasCompleted)
            MarkAllAsCompleted = true;

        resultObject = {
            'commaSeperatedProfiles': commaSeperatedProfiles,
            'commaSeperatedPages': commaSeperatedPages,
            'commaSeperatedMsgTypes': commaSeperatedMsgTypes,
            'commaSeperatedTagTypes': commaSeperatedTagTypes,
            'commaSeperatedSentimentTypes': commaSeperatedSentimentTypes,
            'pageNumber': pageNumber,
            'pageSize': pageSize,
            'groupId': groupId,
            'companyId': companyId,
            'isAllTask': isAllTask,
            'ids': ids,
            'MarkAllAsCompleted': MarkAllAsCompleted
        };

        return resultObject;
    }

    // Fetch all un-responded posts
    $scope.GetLatestUnrespondedPosts = function () {

        var resultObject = $scope.DataProcessorForLatestUnrespondedPosts(false);

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        };

        UnifiedDeckService.GetNextUnrespondedUnifiedDeckData(resultObject.commaSeperatedProfiles, resultObject.commaSeperatedPages, resultObject.commaSeperatedMsgTypes, resultObject.commaSeperatedTagTypes, resultObject.commaSeperatedSentimentTypes, resultObject.pageNumber, resultObject.pageSize, resultObject.groupId, resultObject.companyId, resultObject.isAllTask, resultObject.ids, resultObject.MarkAllAsCompleted).then(function (result) {
            if (result.statusText == "OK") {
                //console.log(result);

                if (result.data.length == 0) {
                    $scope.PageNumberUnresponded--;
                } else {
                    //angular.forEach(result.data, function (obj, key) {
                    //    $scope.dataUnrespondedQueries.push(obj);                        
                    //});

                    for (var i = 0; i < result.data.length; i++) {
                        result.data[i].fileUploadObject = fileUploadObject;
                        $scope.dataUnrespondedQueries.push(result.data[i]);
                    }
                    //console.log(result.data);
                    RecheckColours();
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.MarkAllAsComplete = function () {
        GlobalService.ConfirmPopup("Confirm", "Do you want to mark all unresponded items as completed?", function (selectedItem) {
            if (selectedItem) {
                var resultObject = $scope.DataProcessorForLatestUnrespondedPosts(true);

                UnifiedDeckService.GetNextUnrespondedUnifiedDeckData(resultObject.commaSeperatedProfiles, resultObject.commaSeperatedPages, resultObject.commaSeperatedMsgTypes, resultObject.commaSeperatedTagTypes, resultObject.commaSeperatedSentimentTypes, resultObject.pageNumber, resultObject.pageSize, resultObject.groupId, resultObject.companyId, resultObject.isAllTask, resultObject.ids, resultObject.MarkAllAsCompleted).then(function (result) {
                    if (result.statusText == "OK") {
                        //console.log(result);
                        $scope.dataUnrespondedQueries = [];
                        $scope.PageNumberUnresponded = 0;
                        //$scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
                        //if ($scope.firstTime == false)
                        //    $scope.GetUnifiedDeckDataForUnrespondedQueries();
                    }
                }, function (err) {
                    GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                    //console.log(err);
                });
            }
        });
    }

    $scope.UnrespondedLoadMoreFeeds = function () {
        //Implemenation to get the next feed for unresponded
        $scope.PageNumberUnresponded++;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize) + $scope.newInboxItemsCountUnresponded;
        $scope.GetLatestUnrespondedPosts();
    }

    function formatAMPM(date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }

    $scope.formatMsgDatetimeForDeck = function (receivedDate) {
        var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        var today = new Date();
        var difference = (today - receivedDate) / 1000;
        var replyedDate = "";
        var replyedDateDetail = "";

        var days = difference / 86400;
        var hours = (difference / 3600) % 24;
        var mins = (difference / 60) % 60;
        var secs = (difference % 60);

        replyedDateDetail = monthNames[receivedDate.getMonth()] + " " + receivedDate.getDate() + " at " + formatAMPM(receivedDate);

        if (days >= 1) {
            replyedDate = monthNames[receivedDate.getMonth()] + " " + receivedDate.getDate();
        }
        else if (hours >= 1) {
            replyedDate = Math.round(hours) + "h";
            //replyedDateDetail = hours + " hour(s) ago";
        }
        else if (mins >= 1) {
            replyedDate = Math.round(mins) + "m";
            //replyedDateDetail = mins + " minutes(s) ago";
        }
        else if (secs >= 1) {
            replyedDate = Math.round(secs) + "s";
            //replyedDateDetail = secs + " second(s) ago";
        }
        else {
            replyedDate = "Now";
            //replyedDateDetail = "Now";
        }

        var result = {
            'replyedDate': replyedDate,
            'replyedDateDetail': replyedDateDetail
        };

        return result;
    }

    $scope.CollectNewUnrespondedItemsForUnrespondedColumn = function () {
        //Hide items which are displayed for more than 5 minutes
        //so technically if minning time is 2 minutes and 5 minutes is to be displayed keeping
        //check of 7 minutes to hide the feed
        var currentDate = new Date();
        var compareDate = new Date();
        var reload = false;

        //Asjad did this : Danger time was usesless so using it as unresponded garbage collector time
        $scope.MinutesofCollector = $scope.DangerTime;
        var minutes = $scope.MinutesofCollector;

        var dateFormatterResult = {};

        //Lookup on twitter                
        if ($scope.firstTime == false) {
            //Remove Twitter old post
            angular.forEach($scope.dataTwitterHome, function (obj, key) {
                compareDate = new Date(Date.parse(obj.CreatedDate));
                //compareDate = new Date(compareDate.valueOf() + compareDate.getTimezoneOffset() * 60000);
                compareDate = new Date(compareDate.getTime() + (minutes * 60 * 1000));
                if (compareDate < currentDate) {
                    $scope.dataTwitterHome.splice(key, 1);
                    reload = true;
                }
                else {
                    dateFormatterResult = $scope.formatMsgDatetimeForDeck(new Date(Date.parse(obj.CreatedDate)));
                    obj.ReplyedDateForDeck = dateFormatterResult.replyedDate;
                    obj.ReplyedDate = dateFormatterResult.replyedDateDetail;
                }
            });

            //Remove Twitter DM old post
            angular.forEach($scope.dataTwitterDM, function (obj, key) {
                compareDate = new Date(Date.parse(obj.CreatedDate));
                //compareDate = new Date(compareDate.valueOf() + compareDate.getTimezoneOffset() * 60000);
                compareDate = new Date(compareDate.getTime() + (minutes * 60 * 1000));
                if (compareDate < currentDate) {
                    $scope.dataTwitterDM.splice(key, 1);
                    reload = true;
                }
                else {
                    dateFormatterResult = $scope.formatMsgDatetimeForDeck(new Date(Date.parse(obj.CreatedDate)));
                    obj.ReplyedDateForDeck = dateFormatterResult.replyedDate;
                    obj.ReplyedDate = dateFormatterResult.replyedDateDetail;
                }
            });

            //Remove Facebook old post
            angular.forEach($scope.dataFacebookHome, function (obj, key) {
                compareDate = new Date(Date.parse(obj.CreatedDate));
                //compareDate = new Date(compareDate.valueOf() + compareDate.getTimezoneOffset() * 60000);
                compareDate = new Date(compareDate.getTime() + (minutes * 60 * 1000));
                if (compareDate < currentDate) {
                    $scope.dataFacebookHome.splice(key, 1);
                    reload = true;
                }
                else {
                    dateFormatterResult = $scope.formatMsgDatetimeForDeck(new Date(Date.parse(obj.CreatedDate)));
                    obj.ReplyedDateForDeck = dateFormatterResult.replyedDate;
                    obj.ReplyedDate = dateFormatterResult.replyedDateDetail;
                }
            });

            //Remove Facebook comment old post
            angular.forEach($scope.dataFacebookComment, function (obj, key) {
                compareDate = new Date(Date.parse(obj.CreatedDate));
                //compareDate = new Date(compareDate.valueOf() + compareDate.getTimezoneOffset() * 60000);
                compareDate = new Date(compareDate.getTime() + (minutes * 60 * 1000));
                if (compareDate < currentDate) {
                    $scope.dataFacebookComment.splice(key, 1);
                    reload = true;
                }
                else {
                    dateFormatterResult = $scope.formatMsgDatetimeForDeck(new Date(Date.parse(obj.CreatedDate)));
                    obj.ReplyedDateForDeck = dateFormatterResult.replyedDate;
                    obj.ReplyedDate = dateFormatterResult.replyedDateDetail;
                }
            });

            //Remove Facebook chat old post
            angular.forEach($scope.dataFacebookChat, function (obj, key) {
                compareDate = new Date(Date.parse(obj.CreatedDate));
                compareDate = new Date(compareDate.getTime() + (minutes * 60 * 1000));
                if (compareDate < currentDate) {
                    $scope.dataFacebookChat.splice(key, 1);
                    reload = true;
                } else {
                    dateFormatterResult = $scope.formatMsgDatetimeForDeck(new Date(Date.parse(obj.CreatedDate)));
                    obj.ReplyedDateForDeck = dateFormatterResult.replyedDate;
                    obj.ReplyedDate = dateFormatterResult.replyedDateDetail;
                }
            });

            //Unresponded Time update
            angular.forEach($scope.dataUnrespondedQueries, function (obj, key) {
                dateFormatterResult = $scope.formatMsgDatetimeForDeck(new Date(Date.parse(obj.CreatedDate)));
                obj.ReplyedDateForDeck = dateFormatterResult.replyedDate;
                obj.ReplyedDate = dateFormatterResult.replyedDateDetail;
            });
        }

        //For the time being
        //if (reload == true) {
        //    $scope.PageNumberUnresponded++;
        //    $scope.GetLatestUnrespondedPosts();
        //}

        //ColourChanger
        RecheckColours();

        //Remove mark as completed items from array
        //Remove Twitter mark as completed post
        angular.forEach($scope.dataTwitterHome, function (obj, key) {
            if (obj.IsTaskCompleted == true) {
                $scope.dataTwitterHome.splice(key, 1);
            }
        });

        //Remove Twitter message mark as completed post
        angular.forEach($scope.dataTwitterDM, function (obj, key) {
            if (obj.IsTaskCompleted == true) {
                $scope.dataTwitterDM.splice(key, 1);
            }
        });

        //Remove facebook home mark as completed post
        angular.forEach($scope.dataFacebookHome, function (obj, key) {
            if (obj.IsTaskCompleted == true) {
                $scope.dataFacebookHome.splice(key, 1);
            }
        });

        //Remove facebook comment mark as completed post
        angular.forEach($scope.dataFacebookComment, function (obj, key) {
            if (obj.IsTaskCompleted == true) {
                $scope.dataFacebookComment.splice(key, 1);
            }
        });

        //Remove facebook chat mark as completed post
        angular.forEach($scope.dataFacebookChat, function (obj, key) {
            if (obj.IsTaskCompleted == true) {
                $scope.dataFacebookChat.splice(key, 1);
            }
        });

        //Remove unresponded mark as completed post
        angular.forEach($scope.dataUnrespondedQueries, function (obj, key) {
            if (obj.IsTaskCompleted == true) {
                $scope.dataUnrespondedQueries.splice(key, 1);
            }
        });

    }

    function RecheckColours() {
        var currentDate = new Date();
        var compareDate = new Date();
        var collectionTime = $scope.DangerTime;
        var firstThreshold = collectionTime + $scope.WarningTime; //Collection time added to first limit
        var secondThreshold = firstThreshold + $scope.AlertTime;
        var thirdThreshold = secondThreshold + 1;
        angular.forEach($scope.dataUnrespondedQueries, function (obj) {

            compareDate = new Date(Date.parse(obj.CreatedDate));
            //Time zone issue resolved
            //compareDate = new Date(compareDate.valueOf() + compareDate.getTimezoneOffset() * 60000);            
            var diffMs = (currentDate - compareDate); // milliseconds between now & compare date of item
            var diffMins = Math.ceil((diffMs) / 60000); // minutes

            if (diffMins < firstThreshold) {
                obj.ItemStatus = "Warning";
            } else if ((diffMins < secondThreshold) && (diffMins >= firstThreshold)) {
                obj.ItemStatus = "Alert";
            } else {
                obj.ItemStatus = "Danger";
            }

        });
    }

    $scope.checkAllTagsUnrespondedQueries = function () {
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedUnrespondedQueriesTags[obj.BrandTypeId] = $scope.selectUnrespondedQueriestags.value;
        });
        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        $scope.dataUnrespondedQueries = [];
        if ($scope.firstTime == false)
            $scope.GetLatestUnrespondedPosts();
    };

    $scope.tagSelectedChangedUnrespondedQueries = function (value, index) {
        if (value == true) {
            $scope.selectUnrespondedQueriestags = false;
        }

        $scope.selectedUnrespondedQueriesTags[index] = !value;

        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        $scope.dataUnrespondedQueries = [];
        if ($scope.firstTime == false)
            $scope.GetLatestUnrespondedPosts();
    };

    $scope.setUnrespondedColumnSettings = function () {
        UnifiedDeckService.GetUnrespondedQueriesColumnSettings().then(function (l) {
            var response = l.data;
            //console.log(response);

            angular.forEach(response, function (obj) {
                if (obj.Id == 1) { //Warning                    
                    $scope.WarningColor = obj.ColorCode;
                    $scope.WarningTime = obj.Time;
                } else if (obj.Id == 2) { //Alert
                    $scope.AlertColor = obj.ColorCode;
                    $scope.AlertTime = obj.Time;
                } else if (obj.Id == 3) { //Danger
                    $scope.DangerColor = obj.ColorCode;
                    $scope.DangerTime = obj.Time;
                }
            });

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Inbox data");
        });
    }

    $scope.updateZonesInformation = function () {
        var errorFlag = false;
        if ($scope.WarningTime === undefined) {
            errorFlag = true;
        }

        if ($scope.AlertTime === undefined) {
            errorFlag = true;
        }

        if ($scope.DangerTime === undefined) {
            errorFlag = true;
        }

        if (errorFlag == true) {
            GlobalService.ErrorPopup("Error", "Please provide timings within specified range. (5 to 3600 minutes)");
            return;
        }

        var lstZones = [];
        lstZones.push({ 'Id': 1, 'ZoneName': 'Zone 1', 'Time': $scope.WarningTime, 'ColorCode': $scope.WarningColor });
        lstZones.push({ 'Id': 2, 'ZoneName': 'Zone 2', 'Time': $scope.AlertTime, 'ColorCode': $scope.AlertColor });
        lstZones.push({ 'Id': 3, 'ZoneName': 'Zone 3', 'Time': $scope.DangerTime, 'ColorCode': $scope.DangerColor });

        UnifiedDeckService.updateUnrespondedColumnSettings(lstZones).then(function (l) {
            if (l.statusText == "OK") {
                var response = l.data;
                //console.log(response);
                notify({
                    message: "Unresponded column settings updated successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                //To reload msgs according to new time zones
                $scope.dataTwitterHome = [];
                $scope.dataTwitterDM = [];
                $scope.dataFacebookHome = [];
                $scope.dataFacebookComment = [];
                $scope.dataFacebookChat = [];
                $scope.dataUnrespondedQueries = [];

                $scope.init();
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Inbox data");
        });
    }

    $scope.checkAllSentimentTypesUnrespondedQueries = function () {
        angular.forEach($scope.sentimentFilter.sentimentTypesUnrespondedQueries, function (obj) {
            $scope.sentimentFilter.selectedUnrespondedQueriesSentimentTypes[obj.id] = $scope.sentimentFilter.selectUnrespondedQueriesSentimentTypes;
        });
        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForUnrespondedQueries();
    };

    $scope.sentimentTypeSelectedChangedUnrespondedQueries = function () {
        $scope.sentimentFilter.selectUnrespondedQueriesSentimentTypes = false;
        $scope.PageNumberUnresponded = 0;
        $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
        if ($scope.firstTime == false)
            $scope.GetUnifiedDeckDataForUnrespondedQueries();
    };

    //  *********************************************
    //  C O M M O N   F U N C T I O N A L I T I E S
    //  *********************************************

    // Fetching Group Members profile
    $scope.GetProfilesByGroupId = function () {
        UnifiedService.GetAllProfilesForUserGroup($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {
            if ($scope.firstTime == true) {
                $scope.attachedProfiles = [];
                $scope.connectedFacebookProfiles = [];
                $scope.connectedFacebookPages = [];
                $scope.connectedTwitterProfiles = [];
                $scope.connectedAllFacebookProfiles = [];
                var compositeId = "";
                angular.forEach(l.data, function (obj) {
                    compositeId = obj.IsPage + "_" + obj.ProfileCompnayGroupId;
                    if (obj.ProfileTypeId == '4') {
                        $scope.connectedTwitterProfiles.push({
                            ProfileCompnayGroupId: obj.ProfileCompnayGroupId,
                            CompositeId: compositeId,
                            ProfileId: obj.ProfileId,
                            AccountName: obj.AccountName,
                            ProfilePicURL: obj.ProfilePicURL,
                            ProfileName: obj.ProfileName,
                            ProfileTypeId: obj.ProfileTypeId,
                            ProfileType: obj.ProfileType,
                            CompanyId: obj.CompanyId,
                            IsConnected: obj.IsConnected,
                            IsPage: obj.IsPage,
                            PageId: obj.PageId,
                            PageName: obj.PageName,
                            PageUrl: obj.PageUrl,
                            UserId: obj.UserId,
                            select: false
                        });
                    }
                    else if (obj.ProfileTypeId == '1' && obj.IsPage == true) {
                        $scope.connectedFacebookPages.push({
                            ProfileCompnayGroupId: obj.ProfileCompnayGroupId,
                            CompositeId: compositeId,
                            ProfileId: obj.ProfileId,
                            AccountName: obj.AccountName,
                            ProfilePicURL: obj.ProfilePicURL,
                            ProfileName: obj.ProfileName,
                            ProfileTypeId: obj.ProfileTypeId,
                            ProfileType: obj.ProfileType,
                            CompanyId: obj.CompanyId,
                            IsConnected: obj.IsConnected,
                            IsPage: obj.IsPage,
                            PageId: obj.PageId,
                            PageName: obj.PageName,
                            PageUrl: obj.PageUrl,
                            UserId: obj.UserId,
                            select: false
                        });
                        $scope.connectedAllFacebookProfiles.push({
                            ProfileCompnayGroupId: obj.ProfileCompnayGroupId,
                            CompositeId: compositeId,
                            ProfileId: obj.ProfileId,
                            AccountName: obj.AccountName,
                            ProfilePicURL: obj.ProfilePicURL,
                            ProfileName: obj.ProfileName,
                            ProfileTypeId: obj.ProfileTypeId,
                            ProfileType: obj.ProfileType,
                            CompanyId: obj.CompanyId,
                            IsConnected: obj.IsConnected,
                            IsPage: obj.IsPage,
                            PageId: obj.PageId,
                            PageName: obj.PageName,
                            PageUrl: obj.PageUrl,
                            UserId: obj.UserId,
                            select: false
                        });
                    }
                    else if (obj.ProfileTypeId == '1' && obj.IsPage == false) {
                        $scope.connectedFacebookProfiles.push({
                            ProfileCompnayGroupId: obj.ProfileCompnayGroupId,
                            CompositeId: compositeId,
                            ProfileId: obj.ProfileId,
                            AccountName: obj.AccountName,
                            ProfilePicURL: obj.ProfilePicURL,
                            ProfileName: obj.ProfileName,
                            ProfileTypeId: obj.ProfileTypeId,
                            ProfileType: obj.ProfileType,
                            CompanyId: obj.CompanyId,
                            IsConnected: obj.IsConnected,
                            IsPage: obj.IsPage,
                            PageId: obj.PageId,
                            PageName: obj.PageName,
                            PageUrl: obj.PageUrl,
                            UserId: obj.UserId,
                            select: false
                        });
                        $scope.connectedAllFacebookProfiles.push({
                            ProfileCompnayGroupId: obj.ProfileCompnayGroupId,
                            CompositeId: compositeId,
                            ProfileId: obj.ProfileId,
                            AccountName: obj.AccountName,
                            ProfilePicURL: obj.ProfilePicURL,
                            ProfileName: obj.ProfileName,
                            ProfileTypeId: obj.ProfileTypeId,
                            ProfileType: obj.ProfileType,
                            CompanyId: obj.CompanyId,
                            IsConnected: obj.IsConnected,
                            IsPage: obj.IsPage,
                            PageId: obj.PageId,
                            PageName: obj.PageName,
                            PageUrl: obj.PageUrl,
                            UserId: obj.UserId,
                            select: false
                        });
                    }
                    if (obj.ProfileTypeId == '4' || obj.ProfileTypeId == '1')
                        $scope.attachedProfiles.push({
                            ProfileCompnayGroupId: obj.ProfileCompnayGroupId,
                            CompositeId: compositeId,
                            ProfileId: obj.ProfileId,
                            AccountName: obj.AccountName,
                            ProfilePicURL: obj.ProfilePicURL,
                            ProfileName: obj.ProfileName,
                            ProfileTypeId: obj.ProfileTypeId,
                            ProfileType: obj.ProfileType,
                            CompanyId: obj.CompanyId,
                            IsConnected: obj.IsConnected,
                            IsPage: obj.IsPage,
                            PageId: obj.PageId,
                            PageName: obj.PageName,
                            PageUrl: obj.PageUrl,
                            UserId: obj.UserId,
                            select: false
                        });
                });


                //Twitter
                $scope.checkAllProfilesTwitterHome();
                $scope.checkAllTypesTwitterHome();
                $scope.checkAllProfilesTwitterDM();

                //Facebook
                $scope.checkAllProfilesFacebookHome();
                $scope.checkAllProfilesFacebookComment();
                $scope.checkAllProfilesFacebookChat();

                //Unresponded Queries
                $scope.checkAllProfilesUnrespondedQueries();
                $scope.checkAllTypesUnrespondedQueries();
                $scope.setUnrespondedColumnSettings();

                //Sentiments
                //$scope.checkAllSentimentTypesTwitterHome();
                //$scope.checkAllSentimentTypesTwitterDM();
                //$scope.checkAllSentimentTypesFacebookHome();
                //$scope.checkAllSentimentTypesFacebookComment();
                //$scope.checkAllSentimentTypesFacebookChat();
                //$scope.checkAllSentimentTypesUnrespondedQueries();

                //Get tags
                $scope.getTagsForParticularGroup();
                $scope.getTagsCategoryByCompanyId();
                //Check                   
                $scope.GetUnifiedDeckDataForAllColumns();
                $scope.GetSampleReplies();
                $scope.GetColumnArrangementOrder();

                //Old
                $scope.setPattern();
            }

            ////console.log($scope.attachedProfiles);           
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            //console.log(err);
        });
    };

    $scope.latestInboxitemTPDetails = new Array();
    $scope.latestInboxitemTMDetails = new Array();
    $scope.latestInboxitemFPDetails = new Array();
    $scope.latestInboxitemFCDetails = new Array();
    $scope.latestInboxitemFPCDetails = new Array();

    //$scope.tempLink = $sce.trustAsResourceUrl("https://t.co/tULyAwX67c");


    // Fetching Unified Deck Data for the first time
    $scope.GetUnifiedDeckDataForAllColumns = function () {
        //var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
        //    return $scope.selectedTwitterHome[record.CompositeId];
        //});
        var selectedProfiles = $scope.attachedProfiles;

        var profileData = [];
        var profilePageData = [];
        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        var msgTypeData = [];
        angular.forEach($scope.UnrespondedQueriesMessageTypes, function (obj) {
            msgTypeData.push(obj.id);
        });
        //angular.forEach($scope.selectedTwitterTypes, function (obj, key) {            
        //    if (obj == true) {
        //        msgTypeData.push(key);
        //    }
        //});

        var tagTypeData = [];
        var brandTypeData = [];

        var pageNumber = $scope.CounterTwitterHome;
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        UnifiedDeckService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, "0", pageNumber, pageSize, groupId, companyId, isAllTask, 1).then(function (result) {
            if (result.statusText == "OK" && result.data != null) {
                // //console.log(result);

                /*result.data.TwitterPostItems.forEach(function (i) {
                    i.Attachments.forEach(function (i) {
                        if (i.Type == 'video_fb' || i.Type == 'video_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });

                result.data.TwitterMessageItems.forEach(function (i) {
                    i.Attachments.forEach(function (i) {
                        if (i.Type == 'video_fb' || i.Type == 'video_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });

                result.data.FacebookPostItems.forEach(function (i) {
                    i.Attachments.forEach(function (i) {
                        if (i.Type == 'video_fb' || i.Type == 'video_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });

                result.data.FacebookCommentItems.forEach(function (i) {
                    i.Attachments.forEach(function (i) {
                        if (i.Type == 'video_fb' || i.Type == 'video_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });

                result.data.FacebookPageChatItems.forEach(function (i) {
                    i.Attachments.forEach(function (i) {
                        if (i.Type == 'video_fb' || i.Type == 'video_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });

                result.data.UnrespondedItems.forEach(function (i) {
                    i.Attachments.forEach(function (i) {
                        if (i.Type == 'video_fb' || i.Type == 'video_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });*/

                //Emptying
                $scope.dataTwitterHome = [];
                $scope.dataTwitterDM = [];
                $scope.dataFacebookHome = [];
                $scope.dataFacebookComment = [];
                $scope.dataFacebookChat = [];
                $scope.dataUnrespondedQueries = [];

                //Twitter
                angular.forEach(result.data.TwitterPostItems, function (obj) {
                    $scope.dataTwitterHome.push(obj);
                });

                //Twitter Message
                angular.forEach(result.data.TwitterMessageItems, function (obj) {
                    obj.NewCount = 0;
                    var replyProfileTwitterDM = $scope.connectedTwitterProfiles.filter(function (profile) {
                        if (profile.AccountName == obj.ToUser) {
                            return profile.ProfilePicURL;
                        }
                    });
                    obj.ToUserProfilePicUrl = replyProfileTwitterDM[0].ProfilePicURL;
                    $scope.dataTwitterDM.push(obj);
                });

                //Facebook
                angular.forEach(result.data.FacebookPostItems, function (obj) {
                    $scope.dataFacebookHome.push(obj);
                });

                angular.forEach(result.data.FacebookCommentItems, function (obj) {
                    $scope.dataFacebookComment.push(obj);
                });

                angular.forEach(result.data.FacebookPageChatItems, function (obj) {
                    obj.NewCount = 0;
                    //Commenting because I do not remember its use
                    //var replyProfileFacebookChat = $scope.connectedFacebookPages.filter(function (profile) {
                    //    if (profile.AccountName == obj.ToUser) {
                    //        return profile.ProfilePicURL;
                    //    }
                    //});
                    //obj.ToUserProfilePicUrl = replyProfileFacebookChat[0].ProfilePicURL;
                    $scope.dataFacebookChat.push(obj);
                });

                //Unresponded Queries
                angular.forEach(result.data.UnrespondedItems, function (obj) {
                    $scope.dataUnrespondedQueries.push(obj);
                });


                //Adding Media Objects to all items                    
                angular.forEach($scope.dataTwitterHome, function (obj) {
                    //obj.fileUploadObject = fileUploadObject;
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                });
                angular.forEach($scope.dataTwitterDM, function (obj) {
                    //obj.fileUploadObject = fileUploadObject;
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                });
                angular.forEach($scope.dataFacebookHome, function (obj) {
                    //obj.fileUploadObject = fileUploadObject;
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                });
                angular.forEach($scope.dataFacebookComment, function (obj) {
                    //obj.fileUploadObject = fileUploadObject;
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                });
                angular.forEach($scope.dataFacebookChat, function (obj) {
                    //obj.fileUploadObject = fileUploadObject;
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                });
                angular.forEach($scope.dataUnrespondedQueries, function (obj) {
                    //obj.fileUploadObject = fileUploadObject;
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                });

                if ($scope.dataTwitterHome.length > 0) {
                    $scope.recentPostsTWId = Math.max.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) > $scope.recentPostsTWId ? Math.max.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) : $scope.recentPostsTWId;
                    $scope.oldestPostsTWId = Math.min.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) < $scope.oldestPostsTWId ? Math.min.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) : $scope.oldestPostsTWId;
                } else $scope.oldestPostsTWId = 0;

                if ($scope.dataTwitterDM.length > 0) {
                    $scope.recentMessagesTWId = Math.max.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) > $scope.recentMessagesTWId ? Math.max.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) : $scope.recentMessagesTWId;
                    $scope.oldestMessagesTWId = Math.min.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) < $scope.oldestMessagesTWId ? Math.min.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) : $scope.oldestMessagesTWId;
                } else $scope.oldestMessagesTWId = 0;
                if ($scope.dataFacebookHome.length > 0) {
                    $scope.recentPostsFBId = Math.max.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) > $scope.recentPostsFBId ? Math.max.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) : $scope.recentPostsFBId;
                    $scope.oldestPostsFBId = Math.min.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) < $scope.oldestPostsFBId ? Math.min.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) : $scope.oldestPostsFBId;
                } else $scope.oldestPostsFBId = 0;

                if ($scope.dataFacebookComment.length > 0) {
                    $scope.recentCommentsFBId = Math.max.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) > $scope.recentCommentsFBId ? Math.max.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) : $scope.recentCommentsFBId;
                    $scope.oldestCommentsFBId = Math.min.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) < $scope.oldestCommentsFBId ? Math.min.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) : $scope.oldestCommentsFBId;
                } else $scope.oldestCommentsFBId = 0;

                if ($scope.dataFacebookChat.length > 0) {
                    $scope.recentPageChatsFBId = Math.max.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) > $scope.recentPageChatsFBId ? Math.max.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) : $scope.recentPageChatsFBId;
                    $scope.oldestPageChatsFBId = Math.min.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) < $scope.oldestPageChatsFBId ? Math.min.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) : $scope.oldestPageChatsFBId;
                } else $scope.oldestPageChatsFBId = 0;

                if ($scope.dataUnrespondedQueries.length > 0) {
                    $scope.recentUnrespondedId = Math.max.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) > $scope.recentUnrespondedId ? Math.max.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) : $scope.recentUnrespondedId;
                    $scope.oldestUnrespondedId = Math.min.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) < $scope.oldestUnrespondedId ? Math.min.apply(Math, $scope.dataUnrespondedQueries.map(function (o) { return o.Id; })) : $scope.oldestUnrespondedId;

                    //For colours initially
                    RecheckColours();
                }

                //setTimeout(function () { loadCursor(); }, 5000);               

                $scope.firstTime = false;
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    //Signal-R.js Implementation
    $scope.UpdateTaskCompletedItems = function (FeedId, FeedType) {
        //console.log(FeedType + "->" + FeedId);
        var flag = false;
        //Before going to unresponded
        switch (FeedType) {
            case 'TP':
                //Remove Twitter post that are completed by somebody
                if (angular.equals($scope.OpeningThreadObjects.TwitterHome, {})) {
                    //Thread is closed
                } else {
                    //1- Check in main thread and tell user it has been replyed
                    var TweetId = $scope.threadTwitterHome.Tweet.Id;
                    if (FeedId == TweetId) {
                        notify({
                            message: "This thread is replied successfully.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.closeNav("TwitterPost");
                    }
                }
                // 2- Remove from main thread
                angular.forEach($scope.dataTwitterHome, function (obj, key) {
                    if (obj.Id == FeedId) {
                        $scope.dataTwitterHome.splice(key, 1);
                        flag = true;
                        //Reducing the count for next fetch
                        $scope.CounterTwitterHome--;
                    }
                });
                break;
            case 'TM':
                //Remove Twitter DM post that are completed by somebody
                if (angular.equals($scope.twitterDMHistory, {})) {
                    //Thread is closed                    
                } else {
                    //1- Check in main thread and tell user it has been replyed
                    var TMId = $scope.TMId;
                    if (FeedId == TMId) {
                        notify({
                            message: "This thread is replied successfully.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.closeNav("TwitterMessage");
                    }
                }
                //2- Remove from main thread
                angular.forEach($scope.dataTwitterDM, function (obj, key) {
                    if (obj.Id == FeedId) {
                        $scope.dataTwitterDM.splice(key, 1);
                        flag = true;
                        //Reducing the count for next fetch
                        $scope.CounterTwitterDM--;
                    }
                });
                break;
            case 'FP':
                //Remove facebook home mark as completed post that are completed by somebody
                if (angular.equals($scope.OpeningThreadObjects.FacebookHome, {})) {
                    //Thread is closed                    
                } else {
                    //1- Check in main thread and tell user it has been replyed
                    var FPId = $scope.threadFacebookHome.fbPost.Id;

                    if (FeedId == FPId) {
                        notify({
                            message: "This thread is replied successfully.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.closeNav("FacebookPost");
                    }
                }
                //2- Remove from main thread
                angular.forEach($scope.dataFacebookHome, function (obj, key) {
                    if (obj.Id == FeedId) {
                        $scope.dataFacebookHome.splice(key, 1);
                        flag = true;
                        //Reducing the count for next fetch
                        $scope.CounterFacebookHome--;
                    }
                });
                break;
            case 'FC':
                //Remove facebook comment mark as completed post that are completed by somebody
                if (angular.equals($scope.OpeningThreadObjects.FacebookComment, {})) {
                    //Thread is closed                    
                } else {
                    //1- Check in main thread and tell user it has been replyed
                    var FCId = $scope.FCId; //$scope.threadFacebookComment.Post.Id;

                    if (FeedId == FCId) {
                        notify({
                            message: "This thread is replied successfully.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.closeNav("FacebookComment");
                    }
                }
                //2- Remove from main thread
                angular.forEach($scope.dataFacebookComment, function (obj, key) {
                    if (obj.Id == FeedId) {
                        $scope.dataFacebookComment.splice(key, 1);
                        flag = true;
                        //Reducing the count for next fetch
                        $scope.CounterFacebookComment--;
                    }
                });
                break;
            case 'FCP':
                //Remove facebook chat mark as completed post that are completed by somebody
                if (angular.equals($scope.facebookChatHistory, {})) {
                    //Thread is closed                    
                } else {
                    //1- Check in main thread and tell user it has been replyed
                    var FCPId = $scope.FCPId;
                    if (FeedId == FCPId) {
                        notify({
                            message: "This thread is replied successfully.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.closeNav("FacebookChat");
                    }
                }
                //2- Remove from main thread
                angular.forEach($scope.dataFacebookChat, function (obj, key) {
                    if (obj.Id == FeedId) {
                        $scope.dataFacebookChat.splice(key, 1);
                        flag = true;
                        //Reducing the count for next fetch
                        $scope.CounterFacebookChat--;
                        // $scope.$apply();// $scope.$digest();
                    }
                });
                break;
            default:
                //Do something default;
                //console.log("No type selected");

        }

        //After coming to unresponded; Remove from unresponded
        if (!flag) {
            if (angular.equals($scope.OpeningThreadObjects.Unresponded, {})) {
                //Thread is closed                
            } else {
                //1- Check in main thread and tell user it has been replyed
                notify({
                    message: "This thread is replied successfully.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $scope.closeNav("UnrespondedQueries");
            }
            //2- Remove from main thread
            angular.forEach($scope.dataUnrespondedQueries, function (obj, key) {
                if (obj.Id == FeedId) {
                    $scope.dataUnrespondedQueries.splice(key, 1);
                    $scope.CounterUnrespondedQueries--;
                }
            });
        }
    }

    $scope.markAllUnrespondedItemsCompleted = function (GroupId) {
        if (GroupId == $rootScope.selectedGroupId) {
            $scope.PageNumberUnresponded = 0;
            $scope.dataUnrespondedQueries = [];
        }
    }

    // Fetching Unified Deck Data periodically
    $scope.GetLatestUnifiedDeckDataForAllColumns = function () {

        if ($scope.firstTime == true) {
            return;
        }

        //Twitter
        var profileData = [];
        var profilePageData = [];
        var msgTypeData = [];
        angular.forEach($scope.UnrespondedQueriesMessageTypes, function (obj) {
            msgTypeData.push(obj.id);
        });
        var tagTypeData = [];
        var brandTypeData = [];
        var sentimentTypeData = [];
        var pageSize = $scope.PageSize;
        var pageType = 0;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var isAllTask = true;
        var final_commaSeperatedProfiles = '';
        var final_commaSeperatedPages = '';
        var final_commaSeperatedSentimentTypes = '';
        var final_commaSeperatedBrandTypes = '';
        var final_commaSeperatedTagTypes = '';

        var selectedProfiles = $.grep($scope.connectedTwitterProfiles, function (record) {
            return $scope.selectedTwitterHome[record.CompositeId];
        });

        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        angular.forEach($scope.selectedTwitterTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        angular.forEach($scope.sentimentFilter.selectedTwitterHomeSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        var pageNumber = $scope.PageNumberTwitterHome + '|';

        var fileUploadObject = {
            'mediaData': new Array(),
            'UploadedFiles': new Array(),
            'UploadedFilesCount': 0
        }

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        var commaSeperatedMsgTypes = msgTypeData.join(",");
        if (commaSeperatedMsgTypes == "")
            commaSeperatedMsgTypes = "0";
        var commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        final_commaSeperatedProfiles += commaSeperatedProfiles + "|";
        final_commaSeperatedPages += commaSeperatedPages + "|";
        final_commaSeperatedSentimentTypes += commaSeperatedSentimentTypes + '|';
        final_commaSeperatedBrandTypes += commaSeperatedBrandTypes + "|";
        final_commaSeperatedTagTypes += commaSeperatedTagTypes + "|";

        //Twitter DM
        profileData = [];
        profilePageData = [];
        tagTypeData = [];
        brandTypeData = [];
        sentimentTypeData = [];

        selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedTwitterDM[record.CompositeId];
        });

        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        angular.forEach($scope.sentimentFilter.selectedTwitterDMSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        pageNumber = pageNumber + $scope.PageNumberTwitterDM + '|';

        commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        final_commaSeperatedProfiles += commaSeperatedProfiles + "|";
        final_commaSeperatedPages += commaSeperatedPages + "|";
        final_commaSeperatedSentimentTypes += commaSeperatedSentimentTypes + '|';
        final_commaSeperatedBrandTypes += commaSeperatedBrandTypes + "|";
        final_commaSeperatedTagTypes += commaSeperatedTagTypes + "|";

        //Facebook Home
        profileData = [];
        profilePageData = [];
        tagTypeData = [];
        brandTypeData = [];
        sentimentTypeData = [];

        selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedFacebookHome[record.CompositeId];
        });

        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        angular.forEach($scope.sentimentFilter.selectedFacebookHomeSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        pageNumber = pageNumber + $scope.PageNumberFacebookHome + '|';

        angular.forEach($scope.selectedFacebookTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        final_commaSeperatedProfiles += commaSeperatedProfiles + "|";
        final_commaSeperatedPages += commaSeperatedPages + "|";
        final_commaSeperatedSentimentTypes += commaSeperatedSentimentTypes + '|';
        final_commaSeperatedBrandTypes += commaSeperatedBrandTypes + "|";
        final_commaSeperatedTagTypes += commaSeperatedTagTypes + "|";

        //Facebook Comment
        profileData = [];
        profilePageData = [];
        tagTypeData = [];
        brandTypeData = [];
        sentimentTypeData = [];

        selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedFacebookComment[record.CompositeId];
        });

        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        angular.forEach($scope.sentimentFilter.selectedFacebookCommentSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        pageNumber = pageNumber + $scope.PageNumberFacebookComment + '|';

        angular.forEach($scope.selectedFacebookCommentTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        final_commaSeperatedProfiles += commaSeperatedProfiles + "|";
        final_commaSeperatedPages += commaSeperatedPages + "|";
        final_commaSeperatedSentimentTypes += commaSeperatedSentimentTypes + '|';
        final_commaSeperatedBrandTypes += commaSeperatedBrandTypes + "|";
        final_commaSeperatedTagTypes += commaSeperatedTagTypes + "|";

        //Facebook Chat
        profileData = [];
        profilePageData = [];
        tagTypeData = [];
        brandTypeData = [];
        sentimentTypeData = [];

        selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selectedFacebookChat[record.CompositeId];
        });

        angular.forEach(selectedProfiles, function (obj) {
            if (obj.IsPage == true)
                profilePageData.push(obj.ProfileId);
            else if (obj.IsPage == false)
                profileData.push(obj.ProfileId);
        });

        angular.forEach($scope.sentimentFilter.selectedFacebookChatSentimentTypes, function (obj, key) {
            if (obj == true) {
                sentimentTypeData.push(key);
            }
        });

        pageNumber = pageNumber + $scope.PageNumberFacebookChat;

        angular.forEach($scope.selectedFacebookDMTags, function (obj, key) {
            if (obj == true) {
                tagTypeData.push(key);
            }
        });

        commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";
        commaSeperatedSentimentTypes = sentimentTypeData.join(",");
        if (commaSeperatedSentimentTypes == "")
            commaSeperatedSentimentTypes = "0";
        commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";
        commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        final_commaSeperatedProfiles += commaSeperatedProfiles;
        final_commaSeperatedPages += commaSeperatedPages;
        final_commaSeperatedSentimentTypes += commaSeperatedSentimentTypes;
        final_commaSeperatedBrandTypes += commaSeperatedBrandTypes;
        final_commaSeperatedTagTypes += commaSeperatedTagTypes;

        var ids = $scope.recentMessagesTWId + '|' + $scope.recentPageChatsFBId + '|' + $scope.recentPostsFBId + '|' + $scope.recentPostsTWId + '|' + $scope.recentCommentsFBId;

        UnifiedDeckService.GetLatestUnifiedDeckData(final_commaSeperatedProfiles, final_commaSeperatedPages, commaSeperatedMsgTypes, final_commaSeperatedTagTypes, final_commaSeperatedSentimentTypes, pageNumber, pageSize, groupId, companyId, isAllTask, 7, ids).then(function (result) {
            if (result.statusText == "OK" && result.data != null) {
                //console.log(result.data);
                /*angular.forEach(result.data.TwitterPostItems, function (obj) {
                    obj.fileUploadObject = {};
                    angular.copy(fileUploadObject, obj.fileUploadObject);
                    obj.ClassName = "read_msg";
                });
                $scope.dataTwitterHome = result.data.TwitterPostItems.concat($scope.dataTwitterHome);
                $timeout(function () {
                    angular.forEach(result.data.TwitterPostItems, function (obj) {
                        obj.ClassName = "unread_msg";
                    });
                }, 10000);*/
                // inserting Twitter Posts
                var tempArray = new Array();

                //Twitter Post: updating sentiment and attachments
                $scope.dataTwitterHome.forEach(function (obj, key) {
                    var delIndex = 0;
                    var TP_item = $.grep(result.data.TwitterPostItems, function (record, index) {
                        if (record.Id == obj.Id) {
                            delIndex = index;
                            return record;
                        }
                    });

                    if (TP_item.length > 0) {
                        $scope.dataTwitterHome[key].Sentiment = TP_item[0].Sentiment;
                        $scope.dataTwitterHome[key].Tags = TP_item[0].Tags;
                        $scope.dataTwitterHome[key].TagIds = TP_item[0].TagIds;
                        result.data.TwitterPostItems.splice(delIndex, 1);
                    }
                });

                result.data.TwitterPostItems.forEach(function (i) {
                    var data = $.grep($scope.latestInboxitemTPDetails, function (record) {
                        return record.Id == i.Id
                    });
                    if (data.length == 0) {
                        i.ClassName = "read_msg";
                        tempArray.push(i);
                    }
                });

                $scope.newInboxItemsCountTwitterHome += tempArray.length;
                Array.prototype.push.apply(tempArray, $scope.latestInboxitemTPDetails)
                $scope.latestInboxitemTPDetails = tempArray;

                angular.forEach($scope.latestInboxitemTPDetails, function (obj, index) {
                    UnifiedDeckService.IsPostMarkedAsCompleted('TP', obj.Id).then(function (l) {
                        var iscompleted = l.data;
                        if (iscompleted) {
                            $scope.dataTwitterHome = $scope.dataTwitterHome.filter(function (e) { return e.Id != obj.Id; });
                        }
                        else {
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        }
                    }, function (err) {
                        //GlobalService.ErrorPopup("Error", err.data.Message);
                    });
                });

                Array.prototype.push.apply($scope.latestInboxitemTPDetails, $scope.dataTwitterHome);
                $scope.dataTwitterHome = $scope.latestInboxitemTPDetails;

                $scope.latestInboxitemTPDetails = new Array();

                // inserting Twitter Messages
                tempArray = new Array();

                //Twitter Message: updating sentiment and attachments
                $scope.dataTwitterDM.forEach(function (obj, key) {
                    var delIndex = 0;
                    var TM_item = $.grep(result.data.TwitterMessageItems, function (record, index) {
                        if (record.Id == obj.Id) {
                            delIndex = index;
                            return record;
                        }
                    });

                    if (TM_item.length > 0) {
                        $scope.dataTwitterDM[key].Sentiment = TM_item[0].Sentiment;
                        $scope.dataTwitterDM[key].Tags = TM_item[0].Tags;
                        $scope.dataTwitterDM[key].TagIds = TM_item[0].TagIds;
                        result.data.TwitterMessageItems.splice(delIndex, 1);
                    }
                });

                result.data.TwitterMessageItems.forEach(function (i) {
                    var data = $.grep($scope.latestInboxitemTMDetails, function (record) {
                        return record.Id == i.Id
                    });
                    if (data.length == 0) {
                        i.ClassName = "read_msg";
                        tempArray.push(i);
                    }
                });

                $scope.newInboxItemsCountTwitterDM += tempArray.length;
                Array.prototype.push.apply(tempArray, $scope.latestInboxitemTMDetails)
                $scope.latestInboxitemTMDetails = tempArray;

                //angular.forEach($scope.latestInboxitemTMDetails, function (obj) {
                //    //obj.fileUploadObject = fileUploadObject;
                //    obj.fileUploadObject = {};
                //    angular.copy(fileUploadObject, obj.fileUploadObject);

                //});

                angular.forEach($scope.latestInboxitemTMDetails, function (obj, index) {
                    UnifiedDeckService.IsPostMarkedAsCompleted('TM', obj.Id).then(function (l) {
                        var iscompleted = l.data;
                        if (iscompleted) {
                            $scope.dataTwitterDM = $scope.dataTwitterDM.filter(function (e) { return e.Id != obj.Id; });
                        }
                        else {
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        }
                    }, function (err) {
                        //GlobalService.ErrorPopup("Error", err.data.Message);
                    });
                });

                //Trial start
                //var goodTogo = true;
                var deadpool = [];
                for (var i = 0; i < $scope.latestInboxitemTMDetails.length; i++) {
                    //goodTogo = true;
                    if ($scope.twitterDMHistory.length > 0) {
                        for (var key = 0; key < $scope.dataTwitterDM.length; key++) {
                            var obj = $scope.dataTwitterDM[key];
                            if ($scope.latestInboxitemTMDetails[i].ReplyedBy == obj.ReplyedBy
                                && $scope.latestInboxitemTMDetails[i].ToUser == obj.ToUser) {
                                //Latest message arrives of same user                                
                                //some thread is opened, now we need to check is that the same thread of user or any other
                                if ($scope.twitterDMHistory[0].ReplyedBy == $scope.latestInboxitemTMDetails[i].ReplyedBy
                                    && $scope.twitterDMHistory[0].ToUser == $scope.latestInboxitemTMDetails[i].ToUser) {
                                    //its the same thread 
                                    //so 1) Update message at the end of thread
                                    //and 2) set this message as previous one and dont splice

                                    //goodTogo = false;
                                    //1)
                                    //$scope.latestInboxitemTMDetails[i].ClassName = "read_msg";
                                    //$scope.twitterDMHistory.unshift($scope.latestInboxitemTMDetails[i]);
                                    $scope.getTwitterDMHistory($scope.dataTwitterDM[key].ToUser, $scope.dataTwitterDM[key].ReplyedBy, true);
                                    $scope.twitterDMHistory[0].ClassName = "read_msg";

                                    //2)
                                    $scope.dataTwitterDM.splice(key, 1);
                                    $scope.twitterDMReplyProfile.MsgObj = $scope.latestInboxitemTMDetails[i];
                                }
                            }
                        }
                    }

                    for (var key = 0; key < $scope.dataTwitterDM.length; key++) {
                        var obj = $scope.dataTwitterDM[key];
                        if ($scope.latestInboxitemTMDetails[i].ReplyedBy == obj.ReplyedBy
                            && $scope.latestInboxitemTMDetails[i].ToUser == obj.ToUser) {
                            if (isNaN(obj.NewCount))
                                obj.NewCount = 0;
                            $scope.latestInboxitemTMDetails[i].NewCount = obj.NewCount + 1;
                            $scope.latestInboxitemTMDetails[i].ToUserProfilePicUrl = obj.ToUserProfilePicUrl;
                            $scope.dataTwitterDM.splice(key, 1);
                        }
                    }
                }
                //Trial end

                Array.prototype.push.apply($scope.latestInboxitemTMDetails, $scope.dataTwitterDM);
                $scope.dataTwitterDM = $scope.latestInboxitemTMDetails;
                $scope.latestInboxitemTMDetails = new Array();

                // inserting Facebook Posts
                tempArray = new Array();

                //Facebook Post: updating sentiment and attachments
                $scope.dataFacebookHome.forEach(function (obj, key) {
                    var delIndex = 0;
                    var FP_item = $.grep(result.data.FacebookPostItems, function (record, index) {
                        if (record.Id == obj.Id) {
                            delIndex = index;
                            return record;
                        }
                    });

                    if (FP_item.length > 0) {
                        $scope.dataFacebookHome[key].Sentiment = FP_item[0].Sentiment;
                        $scope.dataFacebookHome[key].Tags = FP_item[0].Tags;
                        $scope.dataFacebookHome[key].TagIds = FP_item[0].TagIds;
                        result.data.FacebookPostItems.splice(delIndex, 1);
                    }
                });

                result.data.FacebookPostItems.forEach(function (i) {
                    var data = $.grep($scope.latestInboxitemFPDetails, function (record) {
                        return record.Id == i.Id
                    });
                    if (data.length == 0) {
                        i.ClassName = "read_msg";
                        tempArray.push(i);
                    }
                });

                $scope.newInboxItemsCountFacebookHome += tempArray.length;
                Array.prototype.push.apply(tempArray, $scope.latestInboxitemFPDetails)
                $scope.latestInboxitemFPDetails = tempArray;

                angular.forEach($scope.latestInboxitemFPDetails, function (obj, index) {
                    UnifiedDeckService.IsPostMarkedAsCompleted('FP', obj.Id).then(function (l) {
                        var iscompleted = l.data;
                        if (iscompleted) {
                            $scope.dataFacebookHome = $scope.dataFacebookHome.filter(function (e) { return e.Id != obj.Id; });
                        }
                        else {
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        }
                    }, function (err) {
                        //GlobalService.ErrorPopup("Error", err.data.Message);
                    });
                });

                Array.prototype.push.apply($scope.latestInboxitemFPDetails, $scope.dataFacebookHome);
                $scope.dataFacebookHome = $scope.latestInboxitemFPDetails;
                $scope.latestInboxitemFPDetails = new Array();


                // inserting Facebook Comments
                tempArray = new Array();

                //Facebook Comment: updating sentiment and attachments
                $scope.dataFacebookComment.forEach(function (obj, key) {
                    var delIndex = 0;
                    var FC_item = $.grep(result.data.FacebookCommentItems, function (record, index) {
                        if (record.Id == obj.Id) {
                            delIndex = index;
                            return record;
                        }
                    });

                    if (FC_item.length > 0) {
                        $scope.dataFacebookComment[key].Sentiment = FC_item[0].Sentiment;
                        $scope.dataFacebookComment[key].Tags = FC_item[0].Tags;
                        $scope.dataFacebookComment[key].TagIds = FC_item[0].TagIds;
                        result.data.FacebookCommentItems.splice(delIndex, 1);
                    }
                });

                result.data.FacebookCommentItems.forEach(function (i) {
                    var data = $.grep($scope.latestInboxitemFCDetails, function (record) {
                        return record.Id == i.Id
                    });
                    if (data.length == 0) {
                        i.ClassName = "read_msg";
                        tempArray.push(i);
                    }
                });

                $scope.newInboxItemsCountFacebookComment += tempArray.length;
                Array.prototype.push.apply(tempArray, $scope.latestInboxitemFCDetails)
                $scope.latestInboxitemFCDetails = tempArray;

                angular.forEach($scope.latestInboxitemFCDetails, function (obj, index) {
                    UnifiedDeckService.IsPostMarkedAsCompleted('FC', obj.Id).then(function (l) {
                        var iscompleted = l.data;
                        if (iscompleted) {
                            $scope.dataFacebookComment = $scope.dataFacebookComment.filter(function (e) { return e.Id != obj.Id; });
                        }
                        else {
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        }
                    }, function (err) {
                        //GlobalService.ErrorPopup("Error", err.data.Message);
                    });
                });

                Array.prototype.push.apply($scope.latestInboxitemFCDetails, $scope.dataFacebookComment);
                $scope.dataFacebookComment = $scope.latestInboxitemFCDetails;
                $scope.latestInboxitemFCDetails = new Array();

                // inserting Facebook Chats
                tempArray = new Array();

                //Facebook Chat: updating sentiment and attachments
                $scope.dataFacebookChat.forEach(function (obj, key) {
                    var delIndex = 0;
                    var FCP_item = $.grep(result.data.FacebookPageChatItems, function (record, index) {
                        if (record.Id == obj.Id) {
                            delIndex = index;
                            return record;
                        }
                    });

                    if (FCP_item.length > 0) {
                        $scope.dataFacebookChat[key].Sentiment = FCP_item[0].Sentiment;
                        $scope.dataFacebookChat[key].Tags = FCP_item[0].Tags;
                        $scope.dataFacebookChat[key].TagIds = FCP_item[0].TagIds;
                        result.data.FacebookPageChatItems.splice(delIndex, 1);
                    }
                });

                result.data.FacebookPageChatItems.forEach(function (i) {
                    var data = $.grep($scope.latestInboxitemFPCDetails, function (record) {
                        return record.Id == i.Id
                    });
                    if (data.length == 0) {
                        i.ClassName = "read_msg";
                        tempArray.push(i);
                    }
                });

                $scope.newInboxItemsCountFacebookChat += tempArray.length;
                Array.prototype.push.apply(tempArray, $scope.latestInboxitemFPCDetails)
                $scope.latestInboxitemFPCDetails = tempArray;

                //angular.forEach($scope.latestInboxitemFPCDetails, function (obj) {
                //    //obj.fileUploadObject = fileUploadObject;
                //    obj.fileUploadObject = {};
                //    angular.copy(fileUploadObject, obj.fileUploadObject);
                //});

                angular.forEach($scope.latestInboxitemFPCDetails, function (obj, index) {
                    UnifiedDeckService.IsPostMarkedAsCompleted('FCP', obj.Id).then(function (l) {
                        var iscompleted = l.data;
                        if (iscompleted) {
                            $scope.dataFacebookChat = $scope.dataFacebookChat.filter(function (e) { return e.Id != obj.Id; });
                        }
                        else {
                            obj.fileUploadObject = {};
                            angular.copy(fileUploadObject, obj.fileUploadObject);
                        }
                    }, function (err) {
                        //GlobalService.ErrorPopup("Error", err.data.Message);
                    });
                });

                //Updating Message            
                //for (var i = 0; i < $scope.latestInboxitemFPCDetails.length; i++) {
                //    for (var key = 0; key < $scope.dataFacebookChat.length; key++) {
                //        var obj = $scope.dataFacebookChat[key];
                //        if ($scope.latestInboxitemFPCDetails[i].ReplyedByUserId == obj.ReplyedByUserId
                //            && $scope.latestInboxitemFPCDetails[i].ToUser == obj.ToUser) {
                //            if (isNaN(obj.NewCount))
                //                obj.NewCount = 0;
                //            $scope.latestInboxitemFPCDetails[i].NewCount = obj.NewCount + 1;
                //            $scope.latestInboxitemFPCDetails[i].ToUserProfilePicUrl = obj.ToUserProfilePicUrl;
                //            $scope.dataFacebookChat.splice(key, 1);
                //        }
                //    }
                //}

                //Trial start
                for (var i = 0; i < $scope.latestInboxitemFPCDetails.length; i++) {
                    //goodTogo = true;
                    if ($scope.facebookChatHistory.length > 0) {
                        for (var key = 0; key < $scope.dataFacebookChat.length; key++) {
                            var obj = $scope.dataFacebookChat[key];
                            if ($scope.latestInboxitemFPCDetails[i].ReplyedByUserId == obj.ReplyedByUserId
                                && $scope.latestInboxitemFPCDetails[i].ToUser == obj.ToUser
                                && $scope.latestInboxitemFPCDetails[i].Id != obj.Id) {
                                //Latest message arrives of same user                                
                                //some thread is opened, now we need to check is that the same thread of user or any other
                                if ($scope.facebookChatHistory[0].ReplyedByUserId == $scope.latestInboxitemFPCDetails[i].ReplyedByUserId
                                    && $scope.facebookChatHistory[0].ToUserId == $scope.latestInboxitemFPCDetails[i].ToUserId) {
                                    //its the same thread 
                                    //so 1) Update message at the end of thread
                                    //and 2) set this message as previous one and dont splice

                                    //goodTogo = false;
                                    //1)

                                    //$scope.latestInboxitemFPCDetails[i].ClassName = "read_msg";
                                    //$scope.latestInboxitemFPCDetails[i].AuthorName = $scope.latestInboxitemFPCDetails[i].ReplyedByFullName;
                                    //$scope.latestInboxitemFPCDetails[i].ConversationDate = $scope.latestInboxitemFPCDetails[i].ReplyedDate;
                                    //$scope.latestInboxitemFPCDetails[i].ConversationDateForDeck = $scope.latestInboxitemFPCDetails[i].ReplyedDateForDeck;
                                    //$scope.latestInboxitemFPCDetails[i].Msg = $scope.latestInboxitemFPCDetails[i].UserTwit;
                                    //$scope.latestInboxitemFPCDetails[i].MsgType = "FCP";
                                    //$scope.facebookChatHistory.unshift($scope.latestInboxitemFPCDetails[i]);
                                    $scope.getFacebookChatHistory($scope.dataFacebookChat[key].StatusId, true);
                                    $scope.facebookChatHistory[0].ClassName = "read_msg";

                                    //2)
                                    $scope.dataFacebookChat.splice(key, 1);
                                    $scope.facebookChatReplyProfile.MsgObj = $scope.latestInboxitemFPCDetails[i];
                                }
                            }
                        }
                    }

                    for (var key = 0; key < $scope.dataFacebookChat.length; key++) {
                        var obj = $scope.dataFacebookChat[key];
                        if ($scope.latestInboxitemFPCDetails[i].ReplyedByUserId == obj.ReplyedByUserId
                            && $scope.latestInboxitemFPCDetails[i].ToUser == obj.ToUser
                            && $scope.latestInboxitemFPCDetails[i].Id != obj.Id) {
                            if (isNaN(obj.NewCount))
                                obj.NewCount = 0;
                            $scope.latestInboxitemFPCDetails[i].NewCount = obj.NewCount + 1;
                            $scope.latestInboxitemFPCDetails[i].ToUserProfilePicUrl = obj.ToUserProfilePicUrl;
                            $scope.dataFacebookChat.splice(key, 1);
                        }
                    }
                }
                //Trial end

                Array.prototype.push.apply($scope.latestInboxitemFPCDetails, $scope.dataFacebookChat);
                $scope.dataFacebookChat = $scope.latestInboxitemFPCDetails;
                $scope.latestInboxitemFPCDetails = new Array();


                if ($scope.dataTwitterHome.length > 0)
                    $scope.recentPostsTWId = Math.max.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) > $scope.recentPostsTWId ? Math.max.apply(Math, $scope.dataTwitterHome.map(function (o) { return o.Id; })) : $scope.recentPostsTWId;

                if ($scope.dataTwitterDM.length > 0)
                    $scope.recentMessagesTWId = Math.max.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) > $scope.recentMessagesTWId ? Math.max.apply(Math, $scope.dataTwitterDM.map(function (o) { return o.Id; })) : $scope.recentMessagesTWId;

                if ($scope.dataFacebookHome.length > 0)
                    $scope.recentPostsFBId = Math.max.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) > $scope.recentPostsFBId ? Math.max.apply(Math, $scope.dataFacebookHome.map(function (o) { return o.Id; })) : $scope.recentPostsFBId;

                if ($scope.dataFacebookComment.length > 0)
                    $scope.recentCommentsFBId = Math.max.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) > $scope.recentCommentsFBId ? Math.max.apply(Math, $scope.dataFacebookComment.map(function (o) { return o.Id; })) : $scope.recentCommentsFBId;

                if ($scope.dataFacebookChat.length > 0)
                    $scope.recentPageChatsFBId = Math.max.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) > $scope.recentPageChatsFBId ? Math.max.apply(Math, $scope.dataFacebookChat.map(function (o) { return o.Id; })) : $scope.recentPageChatsFBId;

                //Timeout for removing background color of all columns latest posts
                $timeout(function () {
                    result.data.TwitterPostItems.forEach(function (i) {
                        i.ClassName = "unread_msg";
                    });

                    result.data.TwitterMessageItems.forEach(function (i) {
                        i.ClassName = "unread_msg";
                    });


                    if (!angular.equals($scope.twitterDMHistory, {})) {
                        $scope.twitterDMHistory.forEach(function (i) {
                            i.ClassName = "unread_msg";
                        });
                    }

                    result.data.FacebookPostItems.forEach(function (i) {
                        i.ClassName = "unread_msg";
                    });

                    result.data.FacebookCommentItems.forEach(function (i) {
                        i.ClassName = "unread_msg";
                    });

                    result.data.FacebookPageChatItems.forEach(function (i) {
                        i.ClassName = "unread_msg";
                    });

                    if (!angular.equals($scope.facebookChatHistory, {})) {
                        $scope.facebookChatHistory.forEach(function (i) {
                            i.ClassName = "unread_msg";
                        });
                    }
                }, 10000);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });

        // See if we want to fetch the unresponded feed latest or not
        if ($scope.dataUnrespondedQueries.length <= 10) {
            $scope.PageNumberUnresponded = 0;
            $scope.CounterUnrespondedQueries = ($scope.PageNumberUnresponded * $scope.PageSize);
            $scope.GetLatestUnrespondedPosts();
            //$scope.GetUnifiedDeckDataForUnrespondedQueries();
        }
    }

    $scope.TextToURLConvert = function (text, Id, MessageType, tagIds, attachments) {   
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


        for (i = 0; i < tagIds.length; i++) {
            var BGColor = '';
            var FontColor = '';
            for (j = 0; j < $scope.TagsTypes.length; j++) {
                if ($scope.TagsTypes[j].BrandTypeId == tagIds[i]) {
                    BGColor = $scope.TagsTypes[j].BGColor;
                    FontColor = $scope.TagsTypes[j].FontColor;
                    break;
                }
            }
            var style = {
                "background-color": BGColor,
                "color": FontColor

            };
            var Tags = $scope.TagsTypes.filter(function (item) {
                return item.BrandTypeId == tagIds[i];
            });
            var tagText = "";
            if (Tags.length > 0)
                tagText = Tags[0].BrandType;
            //text1 = text1.replace(new RegExp("(" + preg_quote(tags[i]) + ")", 'gi'), "<b style='color:" + FontColor + ";background-color:" + BGColor + ";' >$1</b>"); //ng-style={'background-color':" + BGColor + ",'color':" + FontColor + "}
            text1 = text1.replace(new RegExp("(" + preg_quote(tagText) + ")", 'gi'), "<b>$1</b>");
        }

        if ((MessageType == "TM" || MessageType == "FCP" || MessageType == "FP" || MessageType == "TP" || MessageType == "FC")) {
            var pattern = "\\b" + $scope.ActivePattern + "\\b";
            var re = new RegExp(pattern, 'g');
            matches = text1.match(re);
            var word = matches != null ? matches[0] : '';
            if (word)
                text1 = text1.replace(new RegExp("(" + preg_quote(word) + ")", 'gi'), "<b>$1</b>");
        }

        var exp2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
        // document.getElementById(MessageType + "-converted_url").innerHTML = text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>');

        var str = text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>');
        var pattern = /\B@[a-z0-9_-]+/gi; //@users
        var users = str.match(pattern);
        if (users != null) {
            for (i = 0; i < users.length; i++) {
                str = str.replace(users[i], "<a target='_blank' href='//twitter.com/" + users[i].split('@')[1] + "'>" + users[i] + "</a>");
            }
        }

        $("#" + Id + "-" + MessageType + "-converted_url").html(str);
        // return text;



    }

    $scope.TextToURLConvert_Custom = function (text, Id, MessageType, tags, attachments, displayId) {
        // var t = tags.length;
        if (text === undefined) {
            return;
        }

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
            var pattern = "\\b" + $scope.ActivePattern + "\\b";
            var re = new RegExp(pattern, 'g');
            matches = text1.match(re);
            var word = matches != null ? matches[0] : '';
            if (word)
                text1 = text1.replace(new RegExp("(" + preg_quote(word) + ")", 'gi'), "<b>$1</b>");
        }

        var exp2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
        // document.getElementById(MessageType + "-converted_url").innerHTML = text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>');

        var str = text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>');
        var pattern = /\B@[a-z0-9_-]+/gi; //@users
        var users = str.match(pattern);
        if (users != null) {
            for (i = 0; i < users.length; i++) {
                str = str.replace(users[i], "<a target='_blank' href='//twitter.com/" + users[i].split('@')[1] + "'>" + users[i] + "</a>");
            }
        }

        //$("#" + Id + "-" + MessageType + "-converted_url").html(str);
        $("#ThreadedMain-" + displayId).html(str);
        //return text;



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

    $scope.trustSrc = function (src) {
        return $sce.trustAsResourceUrl(src);
    }

    $scope.quickEmail = function (MsgObj, isToBeEdited) {

        if (isToBeEdited) {
            MsgObj.MessageType = MsgObj.MsgType;
            MsgObj.ReplyedBy = MsgObj.AuthorName;
            MsgObj.UserTwit = MsgObj.Msg;
            MsgObj.ReplyedDate = new Date(MsgObj.CreatedDate.match(/\d+/)[0] * 1);
            MsgObj.UserId = MsgObj.ToUserId;
            if (MsgObj.MsgType == 'FC') {
                MsgObj.StatusId = "0|0_" + MsgObj.Id;
                MsgObj.UserId = MsgObj.postAuthorId;
            }
        }

        $scope.items = { obj: MsgObj };
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
        }, function () {
        });
    }

    $scope.UnrespondedMarkAsCompleted = function (msg, CollapsibleId) {
        $scope.MarkAsCompleted(msg, CollapsibleId, false);
    }

    $scope.IsPostLiked = function (postType, StatusId, self) {
        if (postType == 'FacebookPost') {
               // $("#replySet-" + displayId).collapse('hide');
                FacebookService.FacebookIsPostLiked($scope.facebookWallpostReplyProfile.UserId, $scope.facebookWallpostReplyProfile.IsPage, StatusId, true).then(function (l) {
                    if (l.data.Message == false) {
                        FacebookService.FacebookLikePost($scope.facebookWallpostReplyProfile.UserId, $scope.facebookWallpostReplyProfile.IsPage, StatusId, true).then(function (result) {
                            if (result.statusText == "OK") {
                                notify({
                                    message: "Facebook Post liked successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });

                                // $('#AdminPostLike-' + StatusId).attr('style', $scope.getLikeStyle(true));  //not working for some awkward reason
                                $('#AdminPostLike-' + StatusId).attr('style', 'color: #e11920');
                                $('#AdminPostLike-' + StatusId).attr('title', 'Unlike');
                            }
                        }, function (err) {
                            GlobalService.ErrorPopup("Error", err.data.Message);
                        });
                    }
                    else {
                        FacebookService.FacebookUnLikePost($scope.facebookWallpostReplyProfile.UserId, $scope.facebookWallpostReplyProfile.IsPage, StatusId, true).then(function (l) {
                            notify({
                                message: "Facebook Post unliked successfully",
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });

                            $('#AdminPostLike-' + StatusId).attr('style', $scope.getLikeStyle(false));
                            $('#AdminPostLike-' + StatusId).attr('title', 'Like');
                        }, function (err) {
                            GlobalService.ErrorPopup("Error", err.data.Message);

                        });
                    }
                }, function (err) {
                    //GlobalService.ErrorPopup("Error", err.data.Message);
                });
        }
        else if (postType == 'FacebookComment') {
                var splitted = StatusId.split('|')[0];
                FacebookService.FacebookIsPostLiked($scope.facebookCommentReplyProfile.UserId, $scope.facebookCommentReplyProfile.IsPage, splitted, false).then(function (l) {
                    if(l.data.Message == false)
                    {
                        FacebookService.FacebookLikePost($scope.facebookCommentReplyProfile.UserId, $scope.facebookCommentReplyProfile.IsPage, splitted, false).then(function (result) {
                            if (result.statusText == "OK") {
                                notify({
                                    message: "Facebook Comment liked successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });

                                // $('#AdminCommentLike-' + StatusId).attr('style', $scope.getLikeStyle(true));  //not working for some awkward reason
                                $('#AdminCommentLike-' + splitted).attr('style', 'color: #e11920');
                                $('#AdminCommentLike-' + splitted).attr('title', 'Unlike');
                            }
                        }, function (err) {
                            GlobalService.ErrorPopup("Error", err.data.Message);
                        });
                    }
                    else {
                        FacebookService.FacebookUnLikePost($scope.facebookCommentReplyProfile.UserId, $scope.facebookCommentReplyProfile.IsPage, splitted, false).then(function (l) {
                            notify({
                                message: "Facebook Comment unliked successfully",
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });

                            $('#AdminCommentLike-' + splitted).attr('style', $scope.getLikeStyle(false));
                            $('#AdminCommentLike-' + splitted).attr('title', 'Like');
                        }, function (err) {
                            GlobalService.ErrorPopup("Error", err.data.Message);

                        });
                    }
                }, function (err) {
                });
        }
        else if (postType == 'TwitterPost') {
            TwitterService.TwitterIsFavorited($scope.twitterHomeReplyProfile.AccountName, StatusId).then(function (l) {
                if (l.data == false) {
                    TwitterService.TwitterMarkFavorite($scope.twitterHomeReplyProfile.AccountName, StatusId).then(function (result) {
                        if (result.statusText == "OK") {
                                    notify({
                                        message: "Tweet liked successfully",
                                        position: GlobalService.position,
                                        duration: GlobalService.duration
                                    });

                                    // $('#AdminTweetLike-' + StatusId).attr('style', $scope.getLikeStyle(true));  //not working for some awkward reason
                                    $('#AdminTweetLike-' + StatusId).attr('style', 'color: #e11920');
                                    $('#AdminTweetLike-' + StatusId).attr('title', 'Unfavorite');
                        }
                    });
                }
                else {
                    TwitterService.TwitterUnmarkFavorite($scope.twitterHomeReplyProfile.AccountName, StatusId).then(function (result) {
                        if (result.statusText == "OK") {
                            notify({
                                message: "Tweet unliked successfully",
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });
                            $('#AdminTweetLike-' + StatusId).attr('style', $scope.getLikeStyle(false));
                            $('#AdminTweetLike-' + StatusId).attr('title', 'Favorite');
                        }
                    });
                }
            }, function (err) {
                        $scope.isLiked = false;
                //GlobalService.ErrorPopup("Error", err.data.Message);
            });
        }
    }

    $scope.getLikeStyle = function (isliked) {
        var FontColor = '';
        if (isliked == true) 
            FontColor = '#e11920';
        else if (isliked == false || isliked == null || isliked == "Unknown") 
            FontColor = '#afb4bd';

        return {
            "color": FontColor
        }
    }

    $scope.MarkAsCompleted = function (msg, CollapsibleId, self) {
        //if (msg.Id === undefined) msg.Id = msg.FeedId;
        //if (msg.UserTwit === undefined) msg.UserTwit = msg.MsgText;
        //if (msg.StatusId === undefined) msg.StatusId = msg.Id;

        if (msg.MessageType === undefined) msg.MessageType = msg.MsgType;

        TaskService.InsertTaskCompleted(msg.Id, msg.MessageType, true, 0, $rootScope.selectedGroupId, msg.UserTwit, $rootScope.selectedCompanyId, msg.StatusId, msg.UserId, msg.IsPage).then(function (result) {
            if (result.statusText == "OK") {
                msg.IsTaskCompleted = true;
                //Change Request - Client
                //notify({
                //    message: "Successfully Mark as Completed",
                //    position: GlobalService.position,
                //    duration: GlobalService.duration
                //});                

                //Close Navigation
                if (CollapsibleId !== undefined) {
                    $scope.closeNav(CollapsibleId);
                }

                //Decrement in counter value based on column
                if (self == false) {
                    //Unresponded column - only unresponded counter will be changed
                    // alert('Unresponded');
                    $scope.CounterUnrespondedQueries--;
                } else {
                    //Main columns - 5 columns counter will be changed
                    //alert('Main columns');
                    if (msg.MessageType == 'TP') {
                        $scope.CounterTwitterHome--;
                    } else if (msg.MessageType == 'TM') {
                        $scope.CounterTwitterDM--;
                    } else if (msg.MessageType == 'FP') {
                        $scope.CounterFacebookHome--;
                    } else if (msg.MessageType == 'FC') {
                        $scope.CounterFacebookComment--;
                    } else if (msg.MessageType == 'FCP') {
                        $scope.CounterFacebookChat--;
                    }
                }

                //console.log("Successfully Mark as Completed");
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error, please try again. " + err);
            //console.log(err);
        });
    }

    $scope.openProfileConnect = function () {
        $scope.items = "lalalal";
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ProfileAdd.html',
            controller: 'Header_ProfileAddController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.openMenuCollapse = function (btnPressed, StatusId, postType, displayId, twitterId) {

        if (displayId.toString().indexOf('|') > -1 && displayId.toString().split('|').length > 1) {
            displayId = displayId.split('|')[0];
        }
        //commented on 11th Oct, 2017. Functionality is changed on client's request
        //if (btnPressed == 'Favourite') {
        //    $("#replySet-" + displayId).collapse('hide');
        //    $("#retweetSet-" + displayId).collapse('hide');
        //    $("#retweetWithCommentSet-" + displayId).collapse('hide');
        //    $("#tagSet-" + displayId).collapse('hide');

        //    //Twitter
        //    if (btnPressed == 'Favourite' && postType == 'TwitterPost') {
        //        TwitterService.TwitterIsFavorited($scope.twitterHomeReplyProfile.AccountName, StatusId).then(function (l) {
        //            if (l.data == true) {
        //                $('#favourite-' + displayId).html('Unfavourite');
        //                $('#favourite-' + displayId).attr('value', 'Unfavourite');
        //            } else {
        //                $('#favourite-' + displayId).html('Favourite');
        //                $('#favourite-' + displayId).attr('value', 'Favourite');
        //            }
        //            $scope.isLiked = l.data;
        //        }, function (err) {
        //            $scope.isLiked = false;
        //            //GlobalService.ErrorPopup("Error", err.data.Message);
        //        });
        //    }
        //}
        if (btnPressed == 'Retweet') {
            $("#replySet-" + displayId).collapse('hide');
            $("#favouriteSet-" + displayId).collapse('hide');
            $("#retweetWithCommentSet-" + displayId).collapse('hide');

            TwitterService.TwitterIsRetweeted($scope.twitterHomeReplyProfile.AccountName, StatusId).then(function (l) {
                if (l.data == "") {
                    $('#retweet-' + displayId).html('Retweet');
                    $('#retweet-' + displayId).attr('value', 'Retweet');
                }
                else {
                    $('#retweet-' + displayId).html('Unretweet');
                    $('#retweet-' + displayId).attr('value', 'Unretweet');
                }
            }, function (err) {
                $scope.isRetweeted = false;
            });
        }
        else if (btnPressed == 'Reply') {
            $('#textboxreplyplace-' + displayId).css('display', 'none');
            $("#tagSet-" + displayId).collapse('hide');

            if (postType == 'TwitterPost') {
                $("#replySet-" + displayId + " textarea").val("@" + twitterId + " ");
                $("#retweetSet-" + displayId).collapse('hide');
                $("#favouriteSet-" + displayId).collapse('hide');
            } else if (postType == 'FacebookPost') {
                $("#likeSet-" + displayId).collapse('hide');
            } else if (postType == 'FacebookComment') {
                var splitted = displayId.split('|')[0];
                $("#likeSet-" + splitted).collapse('hide');
                $("#likeSet-" + displayId).collapse('hide');
            }
        }
        //commented on 11th Oct, 2017. Functionality is changed on client's request
        //else if (btnPressed == 'Like') {
        //    if (postType == 'FacebookPost') {
        //        $("#replySet-" + displayId).collapse('hide');
        //        FacebookService.FacebookIsPostLiked($scope.facebookWallpostReplyProfile.UserId, $scope.facebookWallpostReplyProfile.IsPage, StatusId).then(function (l) {

        //            if (l.data.Message == true) {
        //                $('#like-' + displayId).html('Unlike');
        //                $('#like-' + displayId).attr('value', 'Unlike');
        //            } else {
        //                $('#like-' + displayId).html('Like');
        //                $('#like-' + displayId).attr('value', 'Like');
        //            }
        //        }, function (err) {
        //            //GlobalService.ErrorPopup("Error", err.data.Message);
        //        });
        //    } else if (postType == 'FacebookComment') {

        //        var splitted = StatusId.split('|')[0];
        //        $("#replySet-" + displayId).collapse('hide');
        //        FacebookService.FacebookIsPostLiked($scope.facebookCommentReplyProfile.UserId, $scope.facebookCommentReplyProfile.IsPage, splitted).then(function (l) {
        //            if (l.data.Message == true) {
        //                $('#like-' + displayId).html('Unlike');
        //                $('#like-' + displayId).attr('value', 'Unlike');
        //            } else {
        //                $('#like-' + displayId).html('Like');
        //                $('#like-' + displayId).attr('value', 'Like');
        //            }
        //        }, function (err) {
        //            //GlobalService.ErrorPopup("Error", err.data.Message);
        //        });
        //    }
        //}

        else if (btnPressed == 'RetweetWithComment' && postType == 'TwitterPost') {
            $("#retweetSet-" + displayId).collapse('hide');
            $("#favouriteSet-" + displayId).collapse('hide');
            $("#replySet-" + displayId).collapse('hide');
        }
        else if (btnPressed == 'Tags') {
            $("#retweetSet-" + displayId).collapse('hide');
            $("#favouriteSet-" + displayId).collapse('hide');
            $("#replySet-" + displayId).collapse('hide');
        }
    }

    $scope.fillReplyBox = function (ReplyedBy, ReplyText, ElementId, item, self) {

        if (item.MessageType == 'TP') {
            if (self == true)
                $scope.ReplyArea = '@' + ReplyedBy + ' ' + ReplyText;
            else
                $scope.ReplyAreaUnresponded = '@' + ReplyedBy + ' ' + ReplyText;
            document.getElementById(ElementId).value = '@' + ReplyedBy + ' ' + ReplyText;
        }
        else if (item.MessageType == 'FP' || item.MsgType == 'FP') {
            if (self == true)
                $scope.ReplyAreaFacebookWallpost = ReplyText;
            else
                $scope.ReplyAreaUnresponded = ReplyText;
            document.getElementById(ElementId).value = ReplyText;
        }
        else if (item.MessageType == 'FC' || item.MsgType == 'FC') {
            if (self == true)
                $scope.ReplyAreaFacebookComment = ReplyText;
            else
                $scope.ReplyAreaUnresponded = ReplyText;

            document.getElementById(ElementId).value = ReplyText;
        }
    }

    $scope.fillChatBox = function (ReplyText, ElementId, Type, self) {
        if (Type == 'TM') {
            if (self)
                $scope.ReplyAreaTwitterDMthread = ReplyText;
            else
                $scope.UR_ReplyAreaTwitterDMthread = ReplyText;
            document.getElementById(ElementId).value = ReplyText;
        } else if (Type == 'FCP') {
            if (self)
                $scope.ReplyAreaFacebookChatthread = ReplyText;
            else
                $scope.UR_ReplyAreaFacebookChatthread = ReplyText;
            document.getElementById(ElementId).value = ReplyText;
        }
    }

    //Common visual changes
    $scope.openNav = function (StatusId, ofUser, ReplyedBy, columnName, feedId, self) {
        /* Inner threads */
        $('.sidenavInnerThread').slimscroll({
            disableFadeOut: false,
            height: '100%'
        });

        $scope.togglerOfTextBox = false;

        //$scope.threadTwitterHome = {};
        //$scope.threadFacebookHome = {};
        //$scope.threadFacebookComment = {};

        if (self == true) {
            if (columnName == 'TP') {
                document.getElementById("mySidenav-TwitterPost").style.width = "100%";
                //$('#replySet-' + feedId).collapse('show');  [commented on June 30, 2017]
                $scope.openTwitterPostThread(StatusId, ReplyedBy, feedId, self);
            }
            else if (columnName == 'TM') {
                document.getElementById("mySidenav-TwitterMessage").style.width = "100%";
                $scope.getTwitterDMHistory(ofUser, ReplyedBy, self);
            }
            else if (columnName == 'FP') {
                document.getElementById("mySidenav-FacebookPost").style.width = "100%";
                $scope.openFacebookPostThread(StatusId, self);
            }
            else if (columnName == 'FC') {
                document.getElementById("mySidenav-FacebookComment").style.width = "100%";
                $scope.openFacebookCommentThread(StatusId, self);
            }
            else if (columnName == 'FCP') {
                document.getElementById("mySidenav-FacebookChat").style.width = "100%";
                $scope.getFacebookChatHistory(StatusId, self);
            }
        } else {
            //The other case for Unresponded queries
            document.getElementById("mySidenav-UnrespondedQueries").style.width = "100%";

            if (columnName == "TP") {
                $scope.openTwitterPostThread(StatusId, ReplyedBy, feedId, self);
            } else if (columnName == "TM") {
                $scope.getTwitterDMHistory(ofUser, ReplyedBy, self);
            } else if (columnName == "FP") {
                $scope.openFacebookPostThread(StatusId, self);
            } else if (columnName == "FC") {
                $scope.openFacebookCommentThread(StatusId, self);
            } else if (columnName == "FCP") {
                $scope.getFacebookChatHistory(StatusId, self);
            }
        }

    }

    $scope.closeNav = function (Id, objId) {
        document.getElementById("mySidenav-" + Id).style.width = "0%";

        if (Id == "TwitterPost") {
            //below lines added on June 30, 2017 to solve unintentional/automatic/blank twet replies issue
            var TweetId = $scope.threadTwitterHome.Tweet.Id;
            $("#replySet-" + TweetId).collapse('hide');
            ////////////////////////////
            $scope.threadTwitterHome = {};
            $scope.OpeningThreadObjects.TwitterHome = {};
        } else if (Id == "TwitterMessage") {
            $scope.twitterDMHistory = {};

            if (objId != undefined)
                $('#div-TM-' + objId).removeClass("hotkeysFocused");   //to avoid sending blank replies
        } else if (Id == "FacebookPost") {
            $scope.threadFacebookHome = {};
            $scope.OpeningThreadObjects.FacebookHome = {};
        } else if (Id == "FacebookComment") {
            $scope.threadFacebookComment = {};
            $scope.OpeningThreadObjects.FacebookComment = {};
        } else if (Id == "FacebookChat") {
            $scope.facebookChatHistory = {};

            if (objId != undefined)
                $('#div-FCP-' + objId).removeClass("hotkeysFocused");   //to avoid sending blank replies
        } else if (Id == "UnrespondedQueries") {
            $scope.UnrespondedExpansionThread = {};
            $scope.OpeningThreadObjects.Unresponded = {};
            if (objId != undefined) {
                //to avoid sending blank replies
                $('#div-FCP-UR-' + objId).removeClass("hotkeysFocused");
                $('#div-TM-UR-' + objId).removeClass("hotkeysFocused");
            }
        }
    }

    $scope.outerHandleToOpenNavMethod = function (item) {
        var self = false;
        if (item.MessageType == 'FP' || item.MessageType == 'FC' || item.MessageType == 'TP') {
            $scope.openNav(item.StatusId, item.ToUser, item.ReplyedBy, item.MessageType, item.Id, self);
        }
        else if (item.MessageType == 'TM') {
            $scope.openNav(item.StatusId, item.ToUser, item.ReplyedBy, item.MessageType, item.Id, self);
            $scope.loadProfileTwitterDM(item, false);
        }
        else if (item.MessageType == 'FCP') {
            $scope.openNav(item.StatusId, item.ToUser, item.ReplyedBy, item.MessageType, item.Id, self);
            $scope.loadProfileFacebookChat(item, false);
        }
    }

    $scope.setPattern = function () {
        UnifiedService.GetActivePattern().then(function (l) {
            var response = l.data;
            $scope.ActivePattern = response;
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Inbox data");
        });
    };

    function preg_quote(str) {

        return (str + '').replace(/([\\\.\+\*\?\[\^\]\$\(\)\{\}\=\!\<\>\|\:])/g, "\\$1");
    }

    $scope.toggleTextBox = function (feedId) {
        $scope.togglerOfTextBox = $scope.togglerOfTextBox === false ? true : false;

        if ($scope.togglerOfTextBox) {
            $scope.hideTextBox(feedId);
        } else {
            $scope.showTextBox(feedId);
        }
    }

    $scope.hideTextBox = function (feedId) {
        $('#textboxreplyplace-' + feedId).css('display', 'none');
        $('#replySet-' + feedId).collapse('show');
        $('#replySet-UR-' + feedId).collapse('show');
        $scope.togglerOfTextBox = true;
    }

    $scope.showTextBox = function (feedId) {
        $('#textboxreplyplace-' + feedId).css('display', 'block');
        $('#replySet-' + feedId).collapse('hide');
        $('#replySet-UR-' + feedId).collapse('hide');

    }

    $scope.GetSampleReplies = function () {
        UnifiedDeckService.GetSampleRepliesText().then(function (result) {
            if (result.statusText == "OK") {
                ////console.log(result);
                $scope.sampleReplies = [];
                angular.forEach(result.data, function (obj) {
                    $scope.sampleReplies.push(obj.ReplyText);
                });
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.GetColumnArrangementOrder = function () {

        UnifiedDeckService.GetColumnsOrderForResponder($rootScope.selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                /* Reference
                 1  Twitter Tweets
                 2	Twitter Direct Messages
                 3	Facebook Posts
                 4	Facebook Comments
                 5	Facebook Page Chats
                 6	Instagram Posts
                 7	Instagram Comments
                 */
                //console.log(result.data);
                $scope.columnArrangementData = result.data;
                angular.forEach(result.data, function (obj) {
                    switch (obj.Value) {
                        case 1:
                            $scope.OrderIds.TP = obj.Key;
                            $scope.ColumnsInitializer[0].order = obj.Key;
                            break;
                        case 2:
                            $scope.OrderIds.TM = obj.Key;
                            $scope.ColumnsInitializer[1].order = obj.Key;
                            break;
                        case 3:
                            $scope.OrderIds.FP = obj.Key;
                            $scope.ColumnsInitializer[2].order = obj.Key;
                            break;
                        case 4:
                            $scope.OrderIds.FC = obj.Key;
                            $scope.ColumnsInitializer[3].order = obj.Key;
                            break;
                        case 5:
                            $scope.OrderIds.FCP = obj.Key;
                            $scope.ColumnsInitializer[4].order = obj.Key;
                            break;
                    }

                    if (obj.Value < 6) {
                        maxOrderId = maxOrderId + 1;
                        $scope.OrderIds.UR = maxOrderId + 1;
                    }
                });
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.ResetColumnLayout = function () {
        $scope.columnArrangementData = [
            { 'Key': 1, 'Value': 1 },
            { 'Key': 2, 'Value': 2 },
            { 'Key': 3, 'Value': 3 },
            { 'Key': 4, 'Value': 4 },
            { 'Key': 5, 'Value': 5 },
            { 'Key': 6, 'Value': 6 },
            { 'Key': 7, 'Value': 7 }
        ];

        for (var loop = 0; loop < $scope.columnArrangementData.length; loop++) {
            var obj = $scope.columnArrangementData[loop];
            switch (obj.Value) {
                case 1:
                    $scope.OrderIds.TP = obj.Key;
                    $scope.ColumnsInitializer[0].order = obj.Key;
                    break;
                case 2:
                    $scope.OrderIds.TM = obj.Key;
                    $scope.ColumnsInitializer[1].order = obj.Key;
                    break;
                case 3:
                    $scope.OrderIds.FP = obj.Key;
                    $scope.ColumnsInitializer[2].order = obj.Key;
                    break;
                case 4:
                    $scope.OrderIds.FC = obj.Key;
                    $scope.ColumnsInitializer[3].order = obj.Key;
                    break;
                case 5:
                    $scope.OrderIds.FCP = obj.Key;
                    $scope.ColumnsInitializer[4].order = obj.Key;
                    break;
            }
        }

        //DB Update
        UnifiedDeckService.UpdateColumnsOrderForResponder($rootScope.selectedCompanyId, $scope.columnArrangementData).then(function (result) {
            if (result.statusText == "OK") {
                //Db updated successfully
                notify({
                    message: "New setting saved successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    //Initializer for Unified Deck
    $scope.init = function () {
        //Single call
        $scope.firstTime = true;
        $scope.select = true;
        $scope.UnrespondedQueriesMessageTypes = [];
        if ($rootScope.TwitTweets) {
            $scope.UnrespondedQueriesMessageTypes.push({
                id: "3",
                name: "Tweets",
                select: false
            });
            $scope.UnrespondedQueriesMessageTypes.push({
                id: "4",
                name: "Mentions",
                select: false
            });
            $scope.UnrespondedQueriesMessageTypes.push({
                id: "6",
                name: "Retweets",
                select: false
            });
        }
        if ($rootScope.TwitDMs) {
            $scope.UnrespondedQueriesMessageTypes.push({
                id: "5",
                name: "Direct Messages",
                select: false
            });
        }
        if ($rootScope.FBPosts) {
            $scope.UnrespondedQueriesMessageTypes.push({
                id: "1",
                name: "Wall Posts",
                select: false
            });
        }
        if ($rootScope.FBComs) {
            $scope.UnrespondedQueriesMessageTypes.push({
                id: "2",
                name: "Comments",
                select: false
            });
        }
        if ($rootScope.FBChats) {
            $scope.UnrespondedQueriesMessageTypes.push({
                id: "7",
                name: "Page Chats",
                select: false
            });
        }

        $scope.GetProfilesByGroupId();
        //CR - Client
        notify.config({
            maximumOpen: 2
        });
    }

    //Group Change
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.init();

    function highlightCell() {
        $('.inboxObject').removeClass('selected');
        $scope.calendarMap[$scope.position.y][$scope.position.x].addClass('selected');
    }

    $scope.position = { x: 0, y: 0 };
    $scope.calendarMap = [];
    function loadCursor() {
        $scope.calendarMap = [];
        $('.columnSeparator').each(function () {
            $scope.calendarMap.push([]);
            $('.inboxObject', this).each(function () {
                $scope.calendarMap[$scope.calendarMap.length - 1].push($(this));
            });
        });
        highlightCell();
    }

    $scope.moveUp = function () {
        $scope.position.x--;
        if ($scope.position.x < 0)
            $scope.position.x = 0;
        highlightCell();
        if ($scope.position.x >= 3) {
            if ($scope.position.y == 0) {
                $('#TwitterLayoutLength').slimscroll({
                    scrollBy: (($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10) * -1)
                }, 'slow');
            } else if ($scope.position.y == 1) {
                $('#TwitterDMLength').slimscroll({
                    scrollBy: (($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10) * -1)
                }, 'slow');
            } else if ($scope.position.y == 2) {
                $('#FacebookWallpostLayoutLength').slimscroll({
                    scrollBy: (($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10) * -1)
                }, 'slow');
            } else if ($scope.position.y == 3) {
                $('#FacebookCommentLayoutLength').slimscroll({
                    scrollBy: (($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10) * -1)
                }, 'slow');
            } else if ($scope.position.y == 4) {
                $('#FacebookChatLayoutLength').slimscroll({
                    scrollBy: (($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10) * -1)
                }, 'slow');
            } else if ($scope.position.y == 5) {
                $('#UnrespondedLayoutLength').slimscroll({
                    scrollBy: (($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10) * -1)
                }, 'slow');
            }
        }
    }

    $scope.moveLeft = function () {
        $scope.position.y--;
        if ($scope.position.y < 0)
            $scope.position.y = 0;
        highlightCell();
    }

    $scope.moveDown = function () {
        $scope.position.x++;
        if ($scope.position.x >= $scope.calendarMap[0].length)
            $scope.position.x = $scope.calendarMap[0].length - 1;
        highlightCell();
        if ($scope.position.x >= 3) {
            if ($scope.position.y == 0) {
                $('#TwitterLayoutLength').slimscroll({
                    scrollBy: ($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10)
                }, 'slow');
            } else if ($scope.position.y == 1) {
                $('#TwitterDMLength').slimscroll({
                    scrollBy: ($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10)
                }, 'slow');
            } else if ($scope.position.y == 2) {
                $('#FacebookWallpostLayoutLength').slimscroll({
                    scrollBy: ($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10)
                }, 'slow');
            } else if ($scope.position.y == 3) {
                $('#FacebookCommentLayoutLength').slimscroll({
                    scrollBy: ($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10)
                }, 'slow');
            } else if ($scope.position.y == 4) {
                $('#FacebookChatLayoutLength').slimscroll({
                    scrollBy: ($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10)
                }, 'slow');
            } else if ($scope.position.y == 5) {
                $('#UnrespondedLayoutLength').slimscroll({
                    scrollBy: ($scope.calendarMap[$scope.position.y][$scope.position.x][0].clientHeight + 10)
                }, 'slow');
            }
        }
    }

    $scope.moveRight = function () {
        $scope.position.y++;
        if ($scope.position.y >= $scope.calendarMap.length)
            $scope.position.y = $scope.calendarMap.length - 1;
        highlightCell();
    }

    $scope.openThreadEnter = function () {
        //Open the navigation panel according to column
        //console.log($scope.calendarMap[$scope.position.y][$scope.position.x]);
    }

    $scope.getTagsForParticularGroup = function () {
        var companyId = $rootScope.selectedCompanyId;
        TaskService.GetBrandKeywordByCompanyId(companyId).then(function (result) {
            if (result.statusText == 'OK') {
                var response = result.data;
                $scope.BrandsTypes = [];
                $scope.TagsTypes = [];
                for (var i = 0; i < response.length; i++) {
                    if (response[i].IsTag)
                        $scope.TagsTypes.push({ BrandType: response[i].KeywordText, BrandName: response[i].KeywordName, BrandTypeId: response[i].Id, FontColor: response[i].FontColor, BGColor: response[i].BGColor, BrandKeywordCategoryText: response[i].BrandKeywordCategoryText });
                    else
                        $scope.BrandsTypes.push({ BrandType: response[i].KeywordText, BrandName: response[i].KeywordName, BrandTypeId: response[i].Id, LocationName: response[i].LocationName, BrandKeywordCategoryText: response[i].BrandKeywordCategoryText });
                }

                angular.forEach($scope.TagsTypes, function (obj) {
                    $scope.selectedTwitterTags[obj.BrandTypeId] = false;
                });

            }
        }, function (err) {
            alert('Error in fetching brands');
        });
    }
    $scope.getTagsCategoryByCompanyId = function () {
        var companyId = $rootScope.selectedCompanyId;
        TaskService.GetBrandKeywordCategoryByCompanyId(companyId).then(function (l) {
            var response = l.data;
            $scope.BrandCategoryTypes = [];
            for (var i = 0; i < response.length; i++) {
                $scope.BrandCategoryTypes.push({ BrandKeywordCategoryText: response[i].BrandKeywordCategoryText, BrandKeywordCategoryId: response[i].Id, FontColor: response[i].FontColor, BGColor: response[i].BGColor });
            }
        }, function (err) {
            alert('Error in fetching brands');
        });
    }
    $scope.getTagStyle = function (tag) {
        var BGColor = '';
        var FontColor = '';
        for (i = 0; i < $scope.BrandCategoryTypes.length; i++) {
            if ($scope.BrandCategoryTypes[i].BrandKeywordCategoryText == tag) {
                BGColor = $scope.BrandCategoryTypes[i].BGColor;
                FontColor = $scope.BrandCategoryTypes[i].FontColor;
                flag = true;
                break;
            }
        }

        if (BGColor == null || FontColor == null) {
            BGColor = '#6e889c'; //defaults
            FontColor = '#ffffff';
        }

        return {
            "background-color": BGColor,
            "color": FontColor

        }
        // $scope.TagsTypes.push({ BrandType: response[i].KeywordText, BrandTypeId: response[i].Id, FontColor: response[i].FontColor, BGColor: response[i].BGColor });

    }

    $scope.getTagName = function (tag) {
        //if ($scope.TagsTypes === undefined) {
        //    return;
        //} else {
        var name = '';
        for (i = 0; i < $scope.BrandCategoryTypes.length; i++) {
            if ($scope.BrandCategoryTypes[i].BrandKeywordCategoryText == tag) {
                name = $scope.BrandCategoryTypes[i].BrandKeywordCategoryText;
                break;
            }
        }
        return name;
        //}
    }

    $scope.changeImagePreview = function (size, column) {
        if (column == 'TP') {
            if (size == 'L') {
                $scope.previewSizeTwitterHome = {
                    width: '162px',
                    height: '162px'
                }
            } else if (size == 'M') {
                $scope.previewSizeTwitterHome = {
                    width: '122px',
                    height: '122px'
                }
            } else if (size == 'S') {
                $scope.previewSizeTwitterHome = {
                    width: '82px',
                    height: '82px'
                }
            } else if (size == 'Off') {
                $scope.previewSizeTwitterHome = {
                    width: '0px',
                    height: '0px'
                }
            }
        }
        else if (column == 'TM') {
            if (size == 'L') {
                $scope.previewSizeTwitterDM = {
                    width: '162px',
                    height: '162px'
                }
            } else if (size == 'M') {
                $scope.previewSizeTwitterDM = {
                    width: '122px',
                    height: '122px'
                }
            } else if (size == 'S') {
                $scope.previewSizeTwitterDM = {
                    width: '82px',
                    height: '82px'
                }
            } else if (size == 'Off') {
                $scope.previewSizeTwitterDM = {
                    width: '0px',
                    height: '0px'
                }
            }
        }
        else if (column == 'FP') {
            if (size == 'L') {
                $scope.previewSizeFacebookHome = {
                    width: '162px',
                    height: '162px'
                }
            } else if (size == 'M') {
                $scope.previewSizeFacebookHome = {
                    width: '122px',
                    height: '122px'
                }
            } else if (size == 'S') {
                $scope.previewSizeFacebookHome = {
                    width: '82px',
                    height: '82px'
                }
            } else if (size == 'Off') {
                $scope.previewSizeFacebookHome = {
                    width: '0px',
                    height: '0px'
                }
            }
        }
        else if (column == 'FC') {
            if (size == 'L') {
                $scope.previewSizeFacebookComment = {
                    width: '162px',
                    height: '162px'
                }
            } else if (size == 'M') {
                $scope.previewSizeFacebookComment = {
                    width: '122px',
                    height: '122px'
                }
            } else if (size == 'S') {
                $scope.previewSizeFacebookComment = {
                    width: '82px',
                    height: '82px'
                }
            } else if (size == 'Off') {
                $scope.previewSizeFacebookComment = {
                    width: '0px',
                    height: '0px'
                }
            }
        }
        else if (column == 'FCP') {
            if (size == 'L') {
                $scope.previewSizeFacebookDM = {
                    width: '162px',
                    height: '162px'
                }
            } else if (size == 'M') {
                $scope.previewSizeFacebookDM = {
                    width: '122px',
                    height: '122px'
                }
            } else if (size == 'S') {
                $scope.previewSizeFacebookDM = {
                    width: '82px',
                    height: '82px'
                }
            } else if (size == 'Off') {
                $scope.previewSizeFacebookDM = {
                    width: '0px',
                    height: '0px'
                }
            }
        }
        else if (column == 'UR') {
            if (size == 'L') {
                $scope.previewSizeUnrespondedQueries = {
                    width: '162px',
                    height: '162px'
                }
            } else if (size == 'M') {
                $scope.previewSizeUnrespondedQueries = {
                    width: '122px',
                    height: '122px'
                }
            } else if (size == 'S') {
                $scope.previewSizeUnrespondedQueries = {
                    width: '82px',
                    height: '82px'
                }
            } else if (size == 'Off') {
                $scope.previewSizeUnrespondedQueries = {
                    width: '0px',
                    height: '0px'
                }
            }
        }
    }

    $scope.addTagtoPost = function (feedId, MessageType, TagId, tagItem, index, self) {
        //console.log("Add Tag");
        //console.log(index);
        UnifiedDeckService.InsertTagForFeed(feedId, MessageType, TagId, $rootScope.selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                //console.log(result);
                var idname = feedId + '-' + MessageType + '-' + TagId;

                if (self == true) {
                    if (MessageType == 'TP') {
                        $scope.dataTwitterHome[index].TagIds.push(TagId + "");
                        var obj = $.grep($scope.dataTwitterHome[index].Tags, function (record) {
                            return record.indexOf(tagItem.BrandKeywordCategoryText) !== -1
                        });
                        if (obj.length == 0)
                            $scope.dataTwitterHome[index].Tags.push(tagItem.BrandKeywordCategoryText);

                    } else if (MessageType == 'TM') {
                        $scope.dataTwitterDM[index].TagIds.push(TagId + "");
                        var obj = $.grep($scope.dataTwitterDM[index].Tags, function (record) {
                            return record.indexOf(tagItem.BrandKeywordCategoryText) !== -1
                        });
                        if (obj.length == 0)
                            $scope.dataTwitterDM[index].Tags.push(tagItem.BrandKeywordCategoryText);
                    } else if (MessageType == 'FP') {
                        $scope.dataFacebookHome[index].TagIds.push(TagId + "");
                        var obj = $.grep($scope.dataFacebookHome[index].Tags, function (record) {
                            return record.indexOf(tagItem.BrandKeywordCategoryText) !== -1
                        });
                        if (obj.length == 0)
                            $scope.dataFacebookHome[index].Tags.push(tagItem.BrandKeywordCategoryText);
                    } else if (MessageType == 'FC') {
                        $scope.dataFacebookComment[index].TagIds.push(TagId + "");
                        var obj = $.grep($scope.dataFacebookComment[index].Tags, function (record) {
                            return record.indexOf(tagItem.BrandKeywordCategoryText) !== -1
                        });
                        if (obj.length == 0)
                            $scope.dataFacebookComment[index].Tags.push(tagItem.BrandKeywordCategoryText);
                    } else if (MessageType == 'FCP') {
                        $scope.dataFacebookChat[index].TagIds.push(TagId + "");
                        var obj = $.grep($scope.dataFacebookChat[index].Tags, function (record) {
                            return record.indexOf(tagItem.BrandKeywordCategoryText) !== -1
                        });
                        if (obj.length == 0)
                            $scope.dataFacebookChat[index].Tags.push(tagItem.BrandKeywordCategoryText);
                    }
                    //$('#tagsList-' + MessageType + '-' + feedId).append('<a id="' + feedId + '-' + MessageType + '-' + tagItem.BrandType + '" href="javascript:;" class="inboxTag" style="background-color: ' + tagItem.BGColor + '; color: ' + tagItem.FontColor + '">' + tagItem.BrandName + ' <div class="showCross"><i class="fa fa-times"></i></div></a>');
                    $('#tagBox-tag-' + idname).removeClass('halfOpacity');
                    $('#tagBox-tag-' + idname).addClass('active');
                    $('#tagBox-tag-' + idname).addClass('fullOpacity');
                } else if (self == false) {
                    $scope.dataUnrespondedQueries[index].TagIds.push(TagId + "");
                    var obj = $.grep($scope.dataUnrespondedQueries[index].Tags, function (record) {
                        return record.indexOf(tagItem.BrandKeywordCategoryText) !== -1
                    });
                    if (obj.length == 0)
                        $scope.dataUnrespondedQueries[index].Tags.push(tagItem.BrandKeywordCategoryText);
                    $('#tagBox-tag-UR-' + idname).removeClass('halfOpacity');
                    $('#tagBox-tag-UR-' + idname).addClass('active');
                    $('#tagBox-tag-UR-' + idname).addClass('fullOpacity');
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.removeTagFromPost = function (feedId, MessageType, TagId, TagName, index, self) {
        //console.log("Remove");
        UnifiedDeckService.RemoveTagFromFeed(feedId, MessageType, TagId).then(function (result) {
            if (result.statusText == "OK") {
                //console.log(result);
                if (self == true) {
                    if (MessageType == 'TP') {

                        $scope.dataTwitterHome[index].TagIds = $scope.dataTwitterHome[index].TagIds.filter(function (item) {
                            return item != TagId;
                        });
                        $scope.dataTwitterHome[index].Tags = [];
                        angular.forEach($scope.dataTwitterHome[index].TagIds, function (obj, key) {
                            var Tags = $scope.TagsTypes.filter(function (item) {
                                return item.BrandTypeId == obj;
                            });
                            if (Tags.length > 0) {
                                var record = $scope.dataTwitterHome[index].Tags.filter(function (el) {
                                    return el.indexOf(Tags[0].BrandKeywordCategoryText) != -1;
                                });
                                if (record == 0)
                                    $scope.dataTwitterHome[index].Tags.push(Tags[0].BrandKeywordCategoryText);
                            }

                        });
                        
                    } else if (MessageType == 'TM') {
                        $scope.dataTwitterDM[index].TagIds = $scope.dataTwitterDM[index].TagIds.filter(function (item) {
                            return item != TagId;
                        });
                        $scope.dataTwitterDM[index].Tags = [];
                        angular.forEach($scope.dataTwitterDM[index].TagIds, function (obj, key) {
                            var Tags = $scope.TagsTypes.filter(function (item) {
                                return item.BrandTypeId == obj;
                            });
                            if (Tags.length > 0) {
                                var record = $scope.dataTwitterDM[index].Tags.filter(function (el) {
                                    return el.indexOf(Tags[0].BrandKeywordCategoryText) != -1;
                                });
                                if (record == 0)
                                    $scope.dataTwitterDM[index].Tags.push(Tags[0].BrandKeywordCategoryText);
                            }

                        });
                    } else if (MessageType == 'FP') {
                        $scope.dataFacebookHome[index].TagIds = $scope.dataFacebookHome[index].TagIds.filter(function (item) {
                            return item != TagId;
                        });
                        $scope.dataFacebookHome[index].Tags = [];
                        angular.forEach($scope.dataFacebookHome[index].TagIds, function (obj, key) {
                            var Tags = $scope.TagsTypes.filter(function (item) {
                                return item.BrandTypeId == obj;
                            });
                            if (Tags.length > 0) {
                                var record = $scope.dataFacebookHome[index].Tags.filter(function (el) {
                                    return el.indexOf(Tags[0].BrandKeywordCategoryText) != -1;
                                });
                                if (record == 0)
                                    $scope.dataFacebookHome[index].Tags.push(Tags[0].BrandKeywordCategoryText);
                            }

                        });                       
                    } else if (MessageType == 'FC') {
                        $scope.dataFacebookComment[index].TagIds = $scope.dataFacebookComment[index].TagIds.filter(function (item) {
                            return item != TagId;
                        });
                        $scope.dataFacebookComment[index].Tags = [];
                        angular.forEach($scope.dataFacebookComment[index].TagIds, function (obj, key) {
                            var Tags = $scope.TagsTypes.filter(function (item) {
                                return item.BrandTypeId == obj;
                            });
                            if (Tags.length > 0) {
                                var record = $scope.dataFacebookComment[index].Tags.filter(function (el) {
                                    return el.indexOf(Tags[0].BrandKeywordCategoryText) != -1;
                                });
                                if (record == 0)
                                    $scope.dataFacebookComment[index].Tags.push(Tags[0].BrandKeywordCategoryText);
                            }

                        });
                       
                    } else if (MessageType == 'FCP') {
                        $scope.dataFacebookChat[index].TagIds = $scope.dataFacebookChat[index].TagIds.filter(function (item) {
                            return item != TagId;
                        });
                        $scope.dataFacebookChat[index].Tags = [];
                        angular.forEach($scope.dataFacebookChat[index].TagIds, function (obj, key) {
                            var Tags = $scope.TagsTypes.filter(function (item) {
                                return item.BrandTypeId == obj;
                            });
                            if (Tags.length > 0) {
                                var record = $scope.dataFacebookChat[index].Tags.filter(function (el) {
                                    return el.indexOf(Tags[0].BrandKeywordCategoryText) != -1;
                                });
                                if (record == 0)
                                    $scope.dataFacebookChat[index].Tags.push(Tags[0].BrandKeywordCategoryText);
                            }

                        });
                       
                    }

                    $('#tagBox-tag-' + feedId + '-' + MessageType + '-' + TagId).removeClass("active fullOpacity")
                    $('#tagBox-tag-' + feedId + '-' + MessageType + '-' + TagId).addClass("halfOpacity");

                } else if (self == false) {                 

                    $scope.dataUnrespondedQueries[index].TagIds = $scope.dataUnrespondedQueries[index].TagIds.filter(function (item) {
                        return item != TagId;
                    });
                    $scope.dataUnrespondedQueries[index].Tags = [];
                    angular.forEach($scope.dataUnrespondedQueries[index].TagIds, function (obj, key) {
                        var Tags = $scope.TagsTypes.filter(function (item) {
                            return item.BrandTypeId == obj;
                        });
                        if (Tags.length > 0) {
                            var record = $scope.dataUnrespondedQueries[index].Tags.filter(function (el) {
                                return el.indexOf(Tags[0].BrandKeywordCategoryText) != -1;
                            });
                            if (record == 0)
                                $scope.dataUnrespondedQueries[index].Tags.push(Tags[0].BrandKeywordCategoryText);
                        }

                    });

                    $('#tagBox-tag-UR-' + feedId + '-' + MessageType + '-' + TagId).removeClass("active fullOpacity")
                    $('#tagBox-tag-UR-' + feedId + '-' + MessageType + '-' + TagId).addClass("halfOpacity");
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    $scope.quickRemoveTagFromPost = function (feedId, MessageType, TagName, index) {
        //find tag id
        var TagId = -1;
        var Tag = {};
        for (var i = 0; i < $scope.TagsTypes.length; i++) {
            if ($scope.TagsTypes[i].BrandKeywordCategoryText == TagName) {
                TagId = $scope.TagsTypes[i].BrandTypeId;
                Tag = $scope.TagsTypes[i];
                $scope.removeTagFromPost(feedId, MessageType, TagId, Tag, index, true);
                //break;
            }
        }

        if (TagId != -1) {
            //  $scope.removeTagFromPost(feedId, MessageType, TagId, Tag, index, true);
        }
    }

    $scope.quickRemoveTagFromPost_UR = function (feedId, MessageType, TagName, index) {
        //find tag id
        var TagId = -1;
        for (var i = 0; i < $scope.TagsTypes.length; i++) {
            if ($scope.TagsTypes[i].BrandKeywordCategoryText == TagName) {
                TagId = $scope.TagsTypes[i].BrandTypeId;
                Tag = $scope.TagsTypes[i];
                $scope.removeTagFromPost(feedId, MessageType, TagId, TagName, index, false);
               // break;
            }
        }

        if (TagId != -1) {
          //  $scope.removeTagFromPost(feedId, MessageType, TagId, TagName, index, false);
        }
    }

    $scope.selectWarningZoneColor = function () {
        $('#colorpaletteWarning').colorPalette()
        .on('selectColor', function (e) {
            $scope.WarningColor = e.color;
            $('#colorWarningZone').collapse('hide');
        });
    }

    $scope.selectAlertZoneColor = function () {
        $('#colorpaletteAlert').colorPalette()
        .on('selectColor', function (e) {
            $scope.AlertColor = e.color;
            $('#colorAlertZone').collapse('hide');
        });
    }

    $scope.selectDangerZoneColor = function () {
        $('#colorpaletteDanger').colorPalette()
        .on('selectColor', function (e) {
            $scope.DangerColor = e.color;
            $('#colorDangerZone').collapse('hide');
        });
    }

    //$scope.setReplyValue = function (type, text) {
    //    if (type == 'UR') {
    //        $scope.ReplyAreaUnresponded = text;
    //    } else if (type == 'TP') {
    //        $scope.ReplyArea = text;
    //    } else if (type == 'TM') {
    //        $scope.ReplyAreaTwitterDM = text;
    //    } else if (type == 'FP') {
    //        $scope.ReplyAreaFacebookWallpost = text;
    //    } else if (type == 'FC') {
    //        $scope.ReplyAreaFacebookComment = text;
    //    } else if (type == 'FCP') {
    //        $scope.ReplyAreaFacebookChat = text;
    //    }
    //}

    $scope.InsertSentimentForFeed = function (feedId, feedType, sentiment, index, self) {
        UnifiedDeckService.InsertSentimentForFeed(feedId, feedType, sentiment, $rootScope.selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                notify({
                    message: "Sentiment added successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                if (self == true) {
                    if (feedType == 'TP') {
                        $scope.dataTwitterHome[index].Sentiment = sentiment;
                    } else if (feedType == 'TM') {
                        $scope.dataTwitterDM[index].Sentiment = sentiment;
                    } else if (feedType == 'FP') {
                        $scope.dataFacebookHome[index].Sentiment = sentiment;
                    } else if (feedType == 'FC') {
                        $scope.dataFacebookComment[index].Sentiment = sentiment;
                    } else if (feedType == 'FCP') {
                        $scope.dataFacebookChat[index].Sentiment = sentiment;
                    }
                } else if (self == false) {
                    $scope.dataUnrespondedQueries[index].Sentiment = sentiment;
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            //console.log(err);
        });
    }

    /*  Media Uploads */
    $scope.OpenFileUploadModal = function (ItemObject) {
        var selected = [];

        if (ItemObject.MessageType == 'TP' || ItemObject.MsgType == 'TP') {
            angular.forEach($scope.attachedProfiles, function (record) {
                if (record.AccountName == $scope.twitterHomeReplyProfile.AccountName) {
                    selected[record.IsPage + "_" + record.ProfileCompnayGroupId] = true;
                }
            });
        }
        else if (ItemObject.MessageType == 'TM' || ItemObject.MsgType == 'TM') {
            angular.forEach($scope.attachedProfiles, function (record) {
                if (record.AccountName == $scope.twitterDMReplyProfile.AccountName) {
                    selected[record.IsPage + "_" + record.ProfileCompnayGroupId] = true;
                }
            });
        }
        else if (ItemObject.MessageType == 'FP' || ItemObject.MsgType == 'FP') {
            angular.forEach($scope.attachedProfiles, function (record) {
                if (record.AccountName == $scope.facebookWallpostReplyProfile.AccountName) {
                    selected[record.IsPage + "_" + record.ProfileCompnayGroupId] = true;
                }
            });
        }
        else if (ItemObject.MessageType == 'FC' || ItemObject.MsgType == 'FC') {
            angular.forEach($scope.attachedProfiles, function (record) {
                if (record.AccountName == $scope.facebookCommentReplyProfile.AccountName) {
                    selected[record.IsPage + "_" + record.ProfileCompnayGroupId] = true;
                }
            });
        }
        else if (ItemObject.MessageType == 'FCP' || ItemObject.MsgType == 'FCP') {
            angular.forEach($scope.attachedProfiles, function (record) {
                if (record.AccountName == $scope.facebookChatReplyProfile.AccountName) {
                    selected[record.IsPage + "_" + record.ProfileCompnayGroupId] = true;
                }
            });
        }

        $scope.items = { attachedProfiles: $scope.attachedProfiles, selected: selected, files: ItemObject.fileUploadObject.UploadedFiles, result: ItemObject.fileUploadObject.mediaData };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_UploadFile.html',
            controller: '_UploadFileController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //console.log("selectedItem: "+selectedItem);            
            if ($scope.items.result != null) {
                ItemObject.fileUploadObject.UploadedFiles = $scope.items.files;
                ItemObject.fileUploadObject.mediaData = $scope.items.result;
                ItemObject.fileUploadObject.UploadedFilesCount = $scope.items.result.length;
                //console.log(ItemObject.fileUploadObject.mediaData);
            }
        }, function () { });
    }

    //Sentiments
    $scope.getSentimentStyle = function (sentiment) {
        var FontColor = '';
        if (sentiment == "Positive")
            FontColor = '#67c23f';
        else if (sentiment == "Neutral")
            FontColor = '#575757';
        else if (sentiment == "Negative")
            FontColor = '#e11920';
        else if (sentiment == null || sentiment == "Unknown")
            FontColor = '#afb4bd';

        return {
            "color": FontColor
        }
    }

    $scope.getSentimentClass = function (sentiment) {
        var sentClass = '';
        if (sentiment == "Positive")
            sentClass = 'fa fa-thumbs-up';
        else if (sentiment == "Neutral")
            sentClass = 'fa fa-thumbs-up';
        else if (sentiment == "Negative")
            sentClass = 'fa fa-thumbs-down';
        else if (sentiment == null || sentiment == "Unknown")
            sentClass = 'fa fa-smile-o';

        return sentClass;
    }

    $scope.deleteObj = function (obj, self) {
        GlobalService.ConfirmPopup("Confirm", "Are you sure you want to delete?", function (selectedItem) {
            if (selectedItem) {
                if (self == true) {
                    if (obj.MessageType == "FP") {
                        var sendingPostId = obj.StatusId;
                        var PostType = 1;
                        if (obj.StatusId.split('|').length == 2) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 2;
                        }
                        else if (obj.StatusId.split('|').length == 3) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 3;
                        }
                        UnifiedDeckService.DeleteFacebookItem(obj.UserId, obj.IsPage, sendingPostId, PostType).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Facebook Object Deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataFacebookHome = $scope.dataFacebookHome.filter(function (el) {
                                    return el.StatusId.indexOf(sendingPostId) == -1;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Facebook Object, please try again.");
                            //console.log(err);
                        });
                    } else if (obj.MessageType == "FC") {
                        var sendingPostId = obj.StatusId;
                        var PostType = 1;
                        if (obj.StatusId.split('|').length == 2) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 2;
                        }
                        else if (obj.StatusId.split('|').length == 3) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 3;
                        }
                        UnifiedDeckService.DeleteFacebookItem(obj.UserId, obj.IsPage, sendingPostId, PostType).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Facebook Object Deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataFacebookComment = $scope.dataFacebookComment.filter(function (el) {
                                    return el.StatusId.indexOf(sendingPostId) == -1;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Facebook Object, please try again.");
                            //console.log(err);
                        });
                    } else if (obj.MessageType == "TP") {
                        UnifiedDeckService.DeleteTwitterTweet(obj.User, obj.StatusId).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Tweet deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataTwitterHome = $scope.dataTwitterHome.filter(function (el) {
                                    return el.Id !== obj.Id;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Tweet, please try again.");
                            //console.log(err);
                        });
                    }
                    else if (obj.MessageType == "TM") {
                        UnifiedDeckService.DeleteTwitterDM(obj.User, obj.StatusId).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Twitter DM deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataTwitterDM = $scope.dataTwitterDM.filter(function (el) {
                                    return el.Id !== obj.Id;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Twitter DM, please try again.");
                            //console.log(err);
                        });
                    }
                } else if (self == false) {
                    if (obj.MessageType == "FP") {
                        var sendingPostId = obj.StatusId;
                        var PostType = 1;
                        if (obj.StatusId.split('|').length == 2) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 2;
                        }
                        else if (obj.StatusId.split('|').length == 3) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 3;
                        }
                        UnifiedDeckService.DeleteFacebookItem(obj.UserId, obj.IsPage, sendingPostId, PostType).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Facebook Object Deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataUnrespondedQueries = $scope.dataUnrespondedQueries.filter(function (el) {
                                    return el.StatusId.indexOf(sendingPostId) == -1;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Facebook Object, please try again.");
                            //console.log(err);
                        });
                    } else if (obj.MessageType == "FC") {
                        var sendingPostId = obj.StatusId;
                        var PostType = 1;
                        if (obj.StatusId.split('|').length == 2) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 2;
                        }
                        else if (obj.StatusId.split('|').length == 3) {
                            sendingPostId = obj.StatusId.split('|')[0];
                            PostType = 3;
                        }
                        UnifiedDeckService.DeleteFacebookItem(obj.UserId, obj.IsPage, sendingPostId, PostType).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Facebook Object Deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataUnrespondedQueries = $scope.dataUnrespondedQueries.filter(function (el) {
                                    return el.StatusId.indexOf(sendingPostId) == -1;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Facebook Object, please try again.");
                            //console.log(err);
                        });
                    } else if (obj.MessageType == "TP") {
                        UnifiedDeckService.DeleteTwitterTweet(obj.User, obj.StatusId).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Tweet deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataUnrespondedQueries = $scope.dataUnrespondedQueries.filter(function (el) {
                                    return el.Id !== obj.Id;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Tweet, please try again.");
                            //console.log(err);
                        });
                    }
                    else if (obj.MessageType == "TM") {
                        UnifiedDeckService.DeleteTwitterDM(obj.User, obj.StatusId).then(function (l) {
                            if (l.data == "success") {
                                notify({
                                    message: "Twitter DM deleted Successfully",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                                $scope.dataUnrespondedQueries = $scope.dataUnrespondedQueries.filter(function (el) {
                                    return el.Id !== obj.Id;
                                });
                            }
                            else {
                                GlobalService.ErrorPopup("Error", l.data);
                            }
                        }, function (err) {
                            // alert('Error in Archiving, please try again.');

                            GlobalService.ErrorPopup("Error", "Error in Deleting Twitter DM, please try again.");
                            //console.log(err);
                        });
                    }
                }
            }
        });
    }

    $scope.hideObj = function (obj, self) {
        var sendingPostId = obj.StatusId;
        if (obj.MessageType == "FC") {
            sendingPostId = obj.StatusId.split('|')[0];
        }

        FacebookService.FacebookIsPostHidden(obj.UserId, obj.IsPage, sendingPostId, true).then(function (l) {
            var hidden = l.data.Message;
            var hide = true;
            var msg = "";
            var str = "";
            if (hidden) {
                str = "Are you sure you want to unhide?";
                msg = "Facebook Object UnHidden Successfully";
                hide = false;
            }
            else {
                str = "Are you sure you want to hide?";
                msg = "Facebook Object Hidden Successfully";
                hide = true;
            }

            GlobalService.ConfirmPopup("Confirm", str, function (selectedItem) {
                if (selectedItem) {
                    FacebookService.FacebookHideUnhidePost(obj.UserId, obj.IsPage, sendingPostId, hide).then(function (l) {
                        notify({
                            message: msg,
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });

                        if (self) {
                            //On client requirement on 13/04/2017 by Asjad Azeem
                            $scope.MarkAsCompleted(obj);

                            //Remove facebook home mark as completed post
                            if (obj.MessageType == "FP") {
                                angular.forEach($scope.dataFacebookHome, function (obj, key) {
                                    if (obj.IsTaskCompleted == true) {
                                        $scope.dataFacebookHome.splice(key, 1);
                                    }
                                });
                            }
                            else if (obj.MessageType == "FC") {
                                //Remove facebook comment mark as completed post
                                angular.forEach($scope.dataFacebookComment, function (obj, key) {
                                    if (obj.IsTaskCompleted == true) {
                                        $scope.dataFacebookComment.splice(key, 1);
                                    }
                                });
                            }
                        } else {
                            //On client requirement on 13/04/2017 by Asjad Azeem
                            $scope.UnrespondedMarkAsCompleted(obj);

                            //Remove facebook post and comments mark as completed post from unresponded
                            angular.forEach($scope.dataUnrespondedQueries, function (obj, key) {
                                if (obj.IsTaskCompleted == true) {
                                    $scope.dataUnrespondedQueries.splice(key, 1);
                                }
                            });
                        }

                    }, function (err) {
                        GlobalService.ErrorPopup("Error", err.data.Message);
                        // alert('Error in post tweet, please try again.');
                        //console.log(err);
                    });
                }
            });

            //var isConfirmed = confirm(str);
            //if (isConfirmed) {
            //    FacebookService.FacebookHideUnhidePost(obj.UserId, obj.IsPage, sendingPostId, hide).then(function (l) {
            //        notify({
            //            message: msg,
            //            position: GlobalService.position,
            //            duration: GlobalService.duration
            //        });
            //        //On client requirement on 13/04/2017 by Asjad Azeem
            //        $scope.MarkAsCompleted(obj);

            //        if(self){
            //            //Remove facebook home mark as completed post
            //            if(obj.MessageType == "FP"){
            //                angular.forEach($scope.dataFacebookHome, function (obj, key) {
            //                    if (obj.IsTaskCompleted == true) {
            //                        $scope.dataFacebookHome.splice(key, 1);
            //                    }
            //                });
            //                }
            //            else if (obj.MessageType == "FC") {
            //                //Remove facebook comment mark as completed post
            //                angular.forEach($scope.dataFacebookComment, function (obj, key) {
            //                    if (obj.IsTaskCompleted == true) {
            //                        $scope.dataFacebookComment.splice(key, 1);
            //                    }
            //                });
            //            }
            //        } else {
            //            //Remove facebook post and comments mark as completed post from unresponded
            //            angular.forEach($scope.dataUnrespondedQueries, function (obj, key) {
            //                if (obj.IsTaskCompleted == true) {
            //                    $scope.dataUnrespondedQueries.splice(key, 1);
            //                }
            //            });
            //        }

            //    }, function (err) {
            //        GlobalService.ErrorPopup("Error", err.data.Message);
            //        // alert('Error in post tweet, please try again.');
            //        //console.log(err);
            //    });
            //}
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            // alert('Error in post tweet, please try again.');
            //console.log(err);
        });
    }

    $scope.OpenConsumerPopup = function (item) {
        $scope.items = { UserObj: item };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ConsumerDetails.html',
            controller: '_ConsumerDetailsController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            if (item.IsWarned && selectedItem == "response") {
                item.IsWarned = false;
            }
            if (!item.IsWarned && selectedItem == "response") {
                item.IsWarned = true;
            }
        }, function () {

        });
    }

    // hotkey="{'ctrl+down': hotkeyTest}"
    //$scope.hotkeyTest = function () {
    //    angular.element('.hotkeysDeckFocused .hotkeysFocused .text-left').trigger("click");
    //}

    //enter
    hotkeys.add({
        combo: 'shift',
        description: 'select post',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused").length > 0) {
                    if ($('.hotkeysDeckFocused .sidenav .hotkeysFocused [id*="likeSet-"]').is(":visible")) {
                        $timeout(function () {
                            angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused button[data-target*="#likeSet-"]').trigger("click");
                        }, 50);
                    } else
                        if ($('.hotkeysDeckFocused .sidenav .hotkeysFocused [id*="retweetWithCommentSet-"] textarea').is(":visible")) {
                            $timeout(function () {
                                angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused button[data-target*="#retweetWithCommentSet-"]').trigger("click");
                            }, 50);
                        } else
                            if ($('.hotkeysDeckFocused .sidenav .hotkeysFocused [id*="replySet-"] textarea').is(":visible")) {
                                $timeout(function () {
                                    angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused button[data-target*="#replySet-"]').trigger("click");
                                }, 50);
                            } else
                                if ($(".hotkeysDeckFocused .sidenav.hotkeysFocused .hotkeySentiment[aria-expanded='true']").is(":visible")) {
                                    $timeout(function () {
                                        angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused a').trigger("click");
                                    }, 50);
                                } else
                                    if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeyAutoReply[aria-expanded='true']").is(":visible")) {
                                        $timeout(function () {
                                            angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused a').trigger("click");
                                        }, 50);
                                    } else
                                        if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused div[id*='retweetSet-']").is(":visible")) {
                                            $timeout(function () {
                                                angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused div[id*="retweetSet-"] button').trigger("click");
                                            }, 50);
                                        } else
                                            if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused div[id*='favouriteSet-']").is(":visible")) {
                                                $timeout(function () {
                                                    angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused div[id*="favouriteSet-"] button').trigger("click");
                                                }, 50);
                                            } else
                                                if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused [id*=tagSet-]").is(":visible")) {
                                                    $timeout(function () {
                                                        angular.element('.hotkeysDeckFocused .sidenav .hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused').trigger("click");
                                                    }, 50);
                                                } else {
                                                    $timeout(function () {
                                                        //angular.element('.hotkeysDeckFocused .hotkeysFocused .text-left').trigger("click");
                                                        angular.element('.hotkeysDeckFocused .hotkeysFocused .text-left').scope().$$childHead.updateFn();
                                                    }, 50);
                                                }
                }
            }
            else
                if ($(".hotkeysDeckFocused .hotkeysFocused").length > 0) {

                    if ($('.hotkeysDeckFocused .hotkeysFocused [id*="likeSet-"]').is(":visible")) {
                        $timeout(function () {
                            angular.element('.hotkeysDeckFocused .hotkeysFocused button[data-target*="#likeSet-"]').trigger("click");
                        }, 50);
                    } else
                        if ($('.hotkeysDeckFocused .hotkeysFocused [id*="retweetWithCommentSet-"] textarea').is(":visible")) {
                            $timeout(function () {
                                angular.element('.hotkeysDeckFocused .hotkeysFocused button[data-target*="#retweetWithCommentSet-"]').trigger("click");
                            }, 50);
                        } else
                            if ($('.hotkeysDeckFocused .hotkeysFocused [id*="replySet-"] textarea').is(":visible")) {
                                $timeout(function () {
                                    var prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").next(".hotkeysFocusable");
                                    if (prevElement.length == 0)
                                        prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").prev(".hotkeysFocusable");

                                    angular.element('.hotkeysDeckFocused .hotkeysFocused button[data-target*="#replySet-"]').trigger("click");
                                    prevElement.addClass("hotkeysFocused");
                                    $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                                        scrollBy: prevElement.position().top
                                    }, 'slow');
                                }, 50);
                            } else
                                if ($(".hotkeysFocused .hotkeySentiment[aria-expanded='true']").is(":visible")) {
                                    $timeout(function () {
                                        angular.element('.hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused a').trigger("click");
                                    }, 50);
                                } else
                                    if ($(".hotkeysFocused .hotkeyAutoReply[aria-expanded='true']").is(":visible")) {
                                        $timeout(function () {
                                            var prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").next(".hotkeysFocusable");
                                            if (prevElement.length == 0)
                                                prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").prev(".hotkeysFocusable");

                                            angular.element('.hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused a').trigger("click");
                                            prevElement.addClass("hotkeysFocused");
                                            $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                                                scrollBy: prevElement.position().top
                                            }, 'slow');
                                        }, 50);
                                    } else
                                        if ($(".hotkeysFocused div[id*='retweetSet-']").is(":visible")) {
                                            $timeout(function () {
                                                angular.element('.hotkeysFocused div[id*="retweetSet-"] button').trigger("click");
                                            }, 50);
                                        } else
                                            if ($(".hotkeysFocused div[id*='favouriteSet-']").is(":visible")) {
                                                $timeout(function () {
                                                    angular.element('.hotkeysFocused div[id*="favouriteSet-"] button').trigger("click");
                                                }, 50);
                                            } else
                                                if ($(".hotkeysFocused [id*=tagSet-]").is(":visible")) {
                                                    $timeout(function () {
                                                        angular.element('.hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused').trigger("click");
                                                    }, 50);
                                                } else {
                                                    $timeout(function () {
                                                        //angular.element('.hotkeysDeckFocused .hotkeysFocused .text-left').trigger("click");
                                                        angular.element('.hotkeysDeckFocused .hotkeysFocused .text-left').scope().$$childHead.updateFn();
                                                    }, 50);
                                                }
                }
        }
    });

    hotkeys.add({
        combo: 'backspace',
        description: 'back from selected post',
        callback: function () {
            if ($(".hotkeysDeckFocused").length > 0) {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .closebtn').trigger("click");
                }, 50);
            }
        }
    });

    hotkeys.add({
        combo: 'e',
        description: 'email selected post',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .quickEmail').trigger("click");
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .quickEmail').trigger("click");
                    }, 50);
                }
        }
    });

    hotkeys.add({
        combo: 'f',
        description: 'fav selected post',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyFavourite').trigger("click");
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyFavourite').trigger("click");
                    }, 50);
                }
        }
    });

    //commented on June 13, 2017 on client's wish
    //hotkeys.add({
    //    combo: 'w',
    //    description: 'retweet selected post',
    //    callback: function () {
    //        if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
    //            $timeout(function () {
    //                angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyRetweet').trigger("click");
    //            }, 50);
    //        } else
    //        if ($(".hotkeysFocused").length > 0) {
    //            $timeout(function () {
    //                angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyRetweet').trigger("click");
    //            }, 50);
    //        }
    //    }
    //});

    hotkeys.add({
        combo: 'h',
        description: 'hide selected post',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyHidePost').trigger("click");
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        var prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").next(".hotkeysFocusable");
                        if (prevElement.length == 0)
                            prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").prev(".hotkeysFocusable");

                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyHidePost').trigger("click");

                        prevElement.addClass("hotkeysFocused");
                        $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                            scrollBy: prevElement.position().top
                        }, 'slow');
                    }, 50);
                }

            // isHidden = true;
        }
    });

    hotkeys.add({
        combo: 'r',
        description: 'reply selected post',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") { //nav opened
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyReply').trigger("click");
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused  .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*="replySet-"] textarea').focus()
                    }, 200);
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyReply').trigger("click");
                        $timeout(function () {
                            angular.element('.hotkeysDeckFocused .hotkeysFocused [id*="replySet-"] textarea').focus()
                        }, 200);
                    }, 50);
                }
        }
    });

    //commented on June 13, 2017 on client's will
    //hotkeys.add({
    //    combo: 'q',
    //    description: 'reply with comment to selected post',
    //    callback: function () {
    //        if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
    //            $timeout(function () {
    //                angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyReplyWithComment').trigger("click");
    //                $timeout(function () {
    //                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*="retweetWithCommentSet-"] textarea').focus()
    //                }, 200);
    //            }, 50);
    //        } else
    //        if ($(".hotkeysFocused").length > 0) {
    //            $timeout(function () {
    //                angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyReplyWithComment').trigger("click");
    //                $timeout(function () {
    //                    angular.element('.hotkeysDeckFocused .hotkeysFocused [id*="retweetWithCommentSet-"] textarea').focus()
    //                }, 200);
    //            }, 50);
    //        }
    //    }
    //});

    hotkeys.add({
        combo: 'a',
        description: 'auto reply',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReply').trigger("click");
                    $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReply[data-toggle='dropdown']").blur();
                    $('.hotkeysDeckFocused .hotkeysFocused .searchAutoReply').focus();
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyAutoReply').trigger("click");
                        $(".hotkeysFocusable.hotkeysFocused .hotkeyAutoReply[data-toggle='dropdown']").blur();
                        $('.hotkeysDeckFocused .hotkeysFocused .searchAutoReply').focus();
                    }, 50);
                }
        }
    });

    hotkeys.add({
        combo: 's',
        description: 'sentiment',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeySentiment').trigger("click");
                    $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeySentiment[data-toggle='dropdown']").blur();
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeySentiment').trigger("click");
                        $(".hotkeysFocusable.hotkeysFocused .hotkeySentiment[data-toggle='dropdown']").blur();
                    }, 50);
                }
        }
    });

    hotkeys.add({
        combo: 'c',
        description: 'mark as complete',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyMarkAsComplete').trigger("click");
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        var prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").next(".hotkeysFocusable");
                        if (prevElement.length == 0)
                            prevElement = $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").prev(".hotkeysFocusable");

                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyMarkAsComplete').trigger("click");
                        prevElement.addClass("hotkeysFocused");
                        $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                            scrollBy: prevElement.position().top
                        }, 'slow');
                    }, 50);
                }
        }
    });

    hotkeys.add({
        combo: 't',
        description: 'open tags',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*=tagSet-] .halfOpacity").removeClass("hotkeyTagFocused");

                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyTags').trigger("click");
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $(".hotkeysFocused [id*=tagSet-] .halfOpacity").removeClass("hotkeyTagFocused");

                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyTags').trigger("click");
                    }, 50);
                }
        }
    });

    //commented on June 13, 2017 on client's will
    //hotkeys.add({
    //    combo: 'del',
    //    description: 'delete',
    //    callback: function () {
    //        if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
    //            $timeout(function () {
    //                angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyDelete').trigger("click");
    //            }, 50);
    //        } else 
    //        if ($(".hotkeysFocused").length > 0) {
    //            $timeout(function () {
    //                angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyDelete').trigger("click");
    //            }, 50);
    //        }
    //    }
    //});

    hotkeys.add({
        combo: 'right', //ctrl+
        description: 'select next deck',
        callback: function () {
            //if ($(".hotkeysDeckFocusable.hotkeysDeckFocused").length > 0 && $(".hotkeysDeckFocusable.hotkeysDeckFocused").next(".hotkeysDeckFocusable").length > 0) {
            //    $(".hotkeysDeckFocusable.hotkeysDeckFocused .hotkeysFocused").removeClass("hotkeysFocused");
            //    $(".hotkeysDeckFocusable.hotkeysDeckFocused").removeClass("hotkeysDeckFocused").next(".hotkeysDeckFocusable").addClass("hotkeysDeckFocused");
            //    $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable.hotkeysDeckFocused").first(), 20);
            //} else if ($(".hotkeysDeckFocusable.hotkeysDeckFocused").length == 0) {
            //    $(".hotkeysDeckFocusable").removeClass("hotkeysDeckFocused").first().addClass("hotkeysDeckFocused");
            //    $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable.hotkeysDeckFocused").first(), 20);
            //} //else {
            //   // $(".hotkeysDeckFocusable").removeClass("hotkeysDeckFocused").first().addClass("hotkeysDeckFocused");
            //   // $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable.hotkeysDeckFocused").first(), 20);
            ////}

            $(".hotkeysDeckFocusable" + $scope.OrderIds.UR).removeClass("hotkeysDeckFocused");

            if ($(".hotkeysDeckFocusable" + (rightorderids - 1) + ".hotkeysDeckFocused").length > 0) {
                $(".hotkeysDeckFocusable" + (rightorderids - 1) + ".hotkeysDeckFocused .hotkeysFocused").removeClass("hotkeysFocused");
                $(".hotkeysDeckFocusable" + (rightorderids - 1)).removeClass("hotkeysDeckFocused");
                $(".hotkeysDeckFocusable" + rightorderids).addClass("hotkeysDeckFocused");
                $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable" + rightorderids + ".hotkeysDeckFocused").first(), 20);
            } else if ($(".hotkeysDeckFocusable1.hotkeysDeckFocused").length == 0) {
                $(".hotkeysDeckFocusable1").removeClass("hotkeysDeckFocused").first().addClass("hotkeysDeckFocused");
                $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable1.hotkeysDeckFocused").first(), 20);
            }

            rightorderids = rightorderids == $scope.OrderIds.UR ? 1 : rightorderids + 1;
            leftorderids = rightorderids == 1 ? $scope.OrderIds.UR : rightorderids - 1;
        }
    });

    hotkeys.add({
        combo: 'left', //ctrl+
        description: 'select previous deck',
        callback: function () {
            //if ($(".hotkeysDeckFocusable.hotkeysDeckFocused").length > 0 && $(".hotkeysDeckFocusable.hotkeysDeckFocused").prev(".hotkeysDeckFocusable").length > 0) {
            //    $(".hotkeysDeckFocusable.hotkeysDeckFocused .hotkeysFocused").removeClass("hotkeysFocused");
            //    $(".hotkeysDeckFocusable.hotkeysDeckFocused").removeClass("hotkeysDeckFocused").prev(".hotkeysDeckFocusable").addClass("hotkeysDeckFocused");
            //    $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable.hotkeysDeckFocused").first(), 20);
            //} else if ($(".hotkeysDeckFocusable.hotkeysDeckFocused").length == 0) {
            //    $(".hotkeysDeckFocusable").removeClass("hotkeysDeckFocused").first().addClass("hotkeysDeckFocused");
            //    $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable.hotkeysDeckFocused").first(), 20);
            //}// else {
            ////    $(".hotkeysDeckFocusable").removeClass("hotkeysDeckFocused").last().addClass("hotkeysDeckFocused");
            // //   $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable.hotkeysDeckFocused").first(), 20);
            ////}

            if ($(".hotkeysDeckFocusable" + leftorderids + ".hotkeysDeckFocused").length > 0 && leftorderids > 1) {
                $(".hotkeysDeckFocusable" + leftorderids + ".hotkeysDeckFocused .hotkeysFocused").removeClass("hotkeysFocused");
                $(".hotkeysDeckFocusable" + leftorderids).removeClass("hotkeysDeckFocused");
                $(".hotkeysDeckFocusable" + (leftorderids - 1)).addClass("hotkeysDeckFocused");
                $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable" + (leftorderids - 1) + ".hotkeysDeckFocused").first(), 20);
            } else if ($(".hotkeysDeckFocusable1.hotkeysDeckFocused").length == 0) {
                $(".hotkeysDeckFocusable1").removeClass("hotkeysDeckFocused").first().addClass("hotkeysDeckFocused");
                $("#deck .container .col-md-12").first().scrollTo($(".hotkeysDeckFocusable1.hotkeysDeckFocused").first(), 20);
            }

            leftorderids = leftorderids > 1 ? leftorderids - 1 : leftorderids;
            rightorderids = leftorderids + 1;
        }
    });

    hotkeys.add({
        combo: 'down', //ctrl+
        description: 'select next item in seleted deck',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") { // sidenav opened
                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeySentiment[aria-expanded='true']").length > 0) {
                    if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").length == 0) {
                        $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeySentimentOption").first().addClass("hotkeySentimentOptionFocused");
                    } else {
                        $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").removeClass("hotkeySentimentOptionFocused").next(".hotkeySentimentOption").addClass("hotkeySentimentOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReply[aria-expanded='true']").length > 0) {
                    if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").length == 0) {
                        $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReplyOption").first().addClass("hotkeyAutoReplyOptionFocused");
                    } else {
                        $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").removeClass("hotkeyAutoReplyOptionFocused").next(".hotkeyAutoReplyOption").addClass("hotkeyAutoReplyOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*=tagSet-]").is(":visible")) {
                    if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").length == 0) {
                        $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*=tagSet-] .inboxTag").first().addClass("hotkeyTagFocused");
                    } else {
                        $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").removeClass("hotkeyTagFocused").next(".inboxTag").addClass("hotkeyTagFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").length > 0 && $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").next(".hotkeyInnerFocusable").length > 0) {
                    $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").removeClass("hotkeysFocused").next(".hotkeyInnerFocusable").addClass("hotkeysFocused");
                    $('.hotkeysDeckFocused .sidenav').animate({
                        scrollTop: $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").first().offset().top
                    }, 20);
                } else {
                    $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable").removeClass("hotkeysFocused").first().addClass("hotkeysFocused");
                    $('.hotkeysDeckFocused .sidenav').animate({
                        scrollTop: $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").first().offset().top
                    }, 20);
                }
            } else { // sidenav not opened
                if ($(".hotkeysFocusable.hotkeysFocused .hotkeySentiment[aria-expanded='true']").length > 0) {
                    if ($(".hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").length == 0) {
                        $(".hotkeysFocused .hotkeySentimentOption").first().addClass("hotkeySentimentOptionFocused");
                    } else {
                        $(".hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").removeClass("hotkeySentimentOptionFocused").next(".hotkeySentimentOption").addClass("hotkeySentimentOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysFocusable.hotkeysFocused .hotkeyAutoReply[aria-expanded='true']").length > 0) {
                    if ($(".hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").length == 0) {
                        $(".hotkeysFocused .hotkeyAutoReplyOption").first().addClass("hotkeyAutoReplyOptionFocused");
                    } else {
                        $(".hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").removeClass("hotkeyAutoReplyOptionFocused").next(".hotkeyAutoReplyOption").addClass("hotkeyAutoReplyOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysFocusable.hotkeysFocused [id*=tagSet-]").is(":visible")) {
                    if ($(".hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").length == 0) {
                        $(".hotkeysFocused [id*=tagSet-] .inboxTag").first().addClass("hotkeyTagFocused");
                    } else {
                        $(".hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").removeClass("hotkeyTagFocused").next(".inboxTag").addClass("hotkeyTagFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").length > 0 && $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").next(".hotkeysFocusable").length > 0) {
                    $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").removeClass("hotkeysFocused").next(".hotkeysFocusable").addClass("hotkeysFocused");

                    $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                        scrollBy: $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").first().position().top
                    }, 'slow');
                } else if ($(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").length == 0) {
                    $(".hotkeysDeckFocused .hotkeysFocusable").removeClass("hotkeysFocused").first().addClass("hotkeysFocused");

                    $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                        scrollBy: $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").first().position().top
                    }, 'slow');
                }
            }
        }
    });

    hotkeys.add({
        combo: 'up', //ctrl+
        description: 'select previous item in seleted deck',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") { // sidenav opened
                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeySentiment[aria-expanded='true']").next(".dropdown-menu").is(":visible")) {
                    if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").length == 0) {
                        $(".hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeySentimentOption").last().addClass("hotkeySentimentOptionFocused");
                    } else {
                        $(".hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").removeClass("hotkeySentimentOptionFocused").prev(".hotkeySentimentOption").addClass("hotkeySentimentOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReply[aria-expanded='true']").next(".dropdown-menu").is(":visible")) {
                    if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").length == 0) {
                        $(".hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeyAutoReplyOption").last().addClass("hotkeyAutoReplyOptionFocused");
                    } else {
                        $(".hotkeysDeckFocused .sidenav .hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").removeClass("hotkeyAutoReplyOptionFocused").prev(".hotkeyAutoReplyOption").addClass("hotkeyAutoReplyOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused [id*=tagSet-]").is(":visible")) {
                    if ($(".hotkeysDeckFocused .sidenav .hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").length == 0) {
                        $(".hotkeysDeckFocused .sidenav .hotkeysFocused [id*=tagSet-] .inboxTag").last().addClass("hotkeyTagFocused");
                    } else {
                        $(".hotkeysDeckFocused .sidenav .hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").removeClass("hotkeyTagFocused").prev(".inboxTag").addClass("hotkeyTagFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").length > 0 && $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").prev(".hotkeyInnerFocusable").length > 0) {
                    $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").removeClass("hotkeysFocused").prev(".hotkeyInnerFocusable").addClass("hotkeysFocused");

                    $('.hotkeysDeckFocused .sidenav').animate({
                        scrollTop: $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").first().position().top
                    }, 20);
                } else {
                    $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable").removeClass("hotkeysFocused").last().addClass("hotkeysFocused");

                    $('.hotkeysDeckFocused .sidenav').animate({
                        scrollTop: $(".hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused").first().position().top
                    }, 20);
                }
            } else { // sidenav not opened
                if ($(".hotkeysFocusable.hotkeysFocused .hotkeySentiment[aria-expanded='true']").next(".dropdown-menu").is(":visible")) {
                    if ($(".hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").length == 0) {
                        $(".hotkeysFocused .hotkeySentimentOption").last().addClass("hotkeySentimentOptionFocused");
                    } else {
                        $(".hotkeysFocused .hotkeySentimentOption.hotkeySentimentOptionFocused").removeClass("hotkeySentimentOptionFocused").prev(".hotkeySentimentOption").addClass("hotkeySentimentOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysFocusable.hotkeysFocused .hotkeyAutoReply[aria-expanded='true']").next(".dropdown-menu").is(":visible")) {
                    if ($(".hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").length == 0) {
                        $(".hotkeysFocused .hotkeyAutoReplyOption").last().addClass("hotkeyAutoReplyOptionFocused");
                    } else {
                        $(".hotkeysFocused .hotkeyAutoReplyOption.hotkeyAutoReplyOptionFocused").removeClass("hotkeyAutoReplyOptionFocused").prev(".hotkeyAutoReplyOption").addClass("hotkeyAutoReplyOptionFocused");
                    }
                    return;
                }

                if ($(".hotkeysFocusable.hotkeysFocused [id*=tagSet-]").is(":visible")) {
                    if ($(".hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").length == 0) {
                        $(".hotkeysFocused [id*=tagSet-] .inboxTag").last().addClass("hotkeyTagFocused");
                    } else {
                        $(".hotkeysFocused [id*=tagSet-] .inboxTag.hotkeyTagFocused").removeClass("hotkeyTagFocused").prev(".inboxTag").addClass("hotkeyTagFocused");
                    }
                    return;
                }

                if ($(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").length > 0 && $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").prev(".hotkeysFocusable").length > 0) {
                    $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").removeClass("hotkeysFocused").prev(".hotkeysFocusable").addClass("hotkeysFocused");

                    $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                        scrollBy: $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").first().position().top
                    }, 'slow');
                } else if ($(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").length == 0) {
                    $(".hotkeysDeckFocused .hotkeysFocusable").removeClass("hotkeysFocused").last().addClass("hotkeysFocused");
                    $('.hotkeysDeckFocused').find(".columnSeparator").slimscroll({
                        scrollBy: $(".hotkeysDeckFocused .hotkeysFocusable.hotkeysFocused").first().position().top
                    }, 'slow');
                }
            }

        }
    });

    hotkeys.add({
        combo: 'esc',
        description: 'loose focus',
        callback: function () {
            if ($(".hotkeysDeckFocused").length > 0) {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .hotkeyDirectMessage').blur();
                }, 50);
            }
        }
    });

    hotkeys.add({
        combo: 'd',
        description: 'details of consumer of selected post',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyConsumerDetails').trigger("click");
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyConsumerDetails').trigger("click");
                    }, 50);
                }
        }
    });


    /*
        To select Auto reply option from dropdown
        To be Completed
    */
    hotkeys.add({
        combo: 'space',
        description: 'Select Auto Reply from dropdown',
        callback: function () {
            if ($('.hotkeysDeckFocused .sidenav').css("width") != "0px") {
                $timeout(function () {
                    angular.element('.hotkeysDeckFocused .sidenav .hotkeyInnerFocusable.hotkeysFocused .hotkeyAutoReplyOption .hotkeyAutoReplyOptionFocused .autoOption').trigger("click");
                }, 50);
            } else
                if ($(".hotkeysFocused").length > 0) {
                    $timeout(function () {
                        angular.element('.hotkeysDeckFocused .hotkeysFocused .hotkeyAutoReplyOption .hotkeyAutoReplyOptionFocused .autoOption').trigger("click");
                    }, 50);
                }
        }
    });

    $(window).resize(function () {
        ///$("#deckContainer").css("height", ($("#UnifiedDeckController").height()-40)+"px");
        //$('#appBody').css("overflow", "auto");
        var minusHeight = $("nav").height() + $("#FooterController").height();
        var height = $(window).height() - minusHeight;
        var diff = 50;//140;

        $("#TwitterLayoutLength").css("height", height - diff + "px");
        $("#TwitterLayoutLength").parent().css("height", height - diff + "px");

        $("#TwitterDMLength").css("height", height - diff + "px");
        $("#TwitterDMLength").parent().css("height", height - diff + "px");

        $("#FacebookWallpostLayoutLength").css("height", height - diff + "px");
        $("#FacebookWallpostLayoutLength").parent().css("height", height - diff + "px");

        $("#FacebookCommentLayoutLength").css("height", height - diff + "px");
        $("#FacebookCommentLayoutLength").parent().css("height", height - diff + "px");

        $("#FacebookChatLayoutLength").css("height", height - diff + "px");
        $("#FacebookChatLayoutLength").parent().css("height", height - diff + "px");

        $("#UnrespondedLayoutLength").css("height", height - diff + "px");
        $("#UnrespondedLayoutLength").parent().css("height", height - diff + "px");

        //$("#UnifiedDeckController").css("height", height - 20 + "px");
    });

    $scope.onItemsChange = function (fromIdx, toIdx) {
        // Do whatever you want here...
        //console.log('onItemsChange');
    };

    //$scope.initValue = function (ElementId) {
    //    //document.getElementById(ElementId).value = '@' + $scope.threadTwitterHome.Tweet.ReplyedBy + ' ';
    //    $scope.ReplyArea = '@' + $scope.threadTwitterHome.Tweet.ReplyedBy + ' ';
    //}

}]);

//CRMApp.filter("pipeBreak",
//    function () {
//        return function (value) {
//            if (!value.length) return;

//            if (value.indexOf("|") >= 0) {
//                if (value.split('|')[0].indexOf("_" > 0)) {
//                    return value.split('|')[0].split("_")[1];
//                } else {
//                    return value.split('|')[0];
//                }                
//            } else {                
//                if (value.indexOf("_" > 0)) {
//                    return value.split("_")[1];
//                } else {
//                    return value
//                }
//            }            
//        }
//    });

CRMApp.filter("pipeBreak",
    function () {
        return function (value) {
            if (!value.length) return;
            return value.split('|')[0];
        }
    });

CRMApp.filter("removeSpaces",
    function () {
        return function (value) {
            if (!value.length) return;
            return value.replace(' ', '');
        }
    });

CRMApp.filter("underscoreLast",
    function () {
        return function (value) {
            if (value.indexOf("_" > 0)) {
                return value.split("_")[1];
            } else {
                return value
            }
        }
    });

CRMApp.filter("underscoreFirst",
    function () {
        return function (value) {
            if (value.indexOf("_" > 0)) {
                return value.split("_")[0];
            } else {
                return value
            }
        }
    });

CRMApp.filter("FBPost",
function () {
    return function (value) {
        return value.split("_")[1];
    }
});

CRMApp.filter("FBComment",
function () {
    return function (value) {
        if (value.split('|').length == 2) {
            return value.split('|')[1].split('_')[1];
        } else {
            return value.split('|')[2].split('_')[1];
        }
    }
});

