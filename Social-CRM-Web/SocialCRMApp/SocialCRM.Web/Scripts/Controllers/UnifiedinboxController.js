CRMApp.controller('UnifiedinboxController', ['$rootScope', '$scope', '$sce', '$window', '$location', 'UnifiedService', 'UnifiedDeckService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'GroupService', 'notify', function ($rootScope, $scope, $sce, $window, $location, UnifiedService, UnifiedDeckService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, GroupService, notify) {
    $scope.getTagStyle = function (tag) {
        var BGColor = '';
        var FontColor = '';
        for (i = 0; i < $scope.TagsTypes.length; i++) {
            if ($scope.TagsTypes[i].BrandType == tag) {
                BGColor = $scope.TagsTypes[i].BGColor;
                FontColor = $scope.TagsTypes[i].FontColor;
                break;
            }
        }

        return {
            "background-color": BGColor,
            "color": FontColor

        }
        // $scope.TagsTypes.push({ BrandType: response[i].KeywordText, BrandTypeId: response[i].Id, FontColor: response[i].FontColor, BGColor: response[i].BGColor });

    }
    $scope.getTagName = function (tag) {
        var name = '';
        for (i = 0; i < $scope.TagsTypes.length; i++) {
            if ($scope.TagsTypes[i].BrandType == tag) {
                if ($scope.TagsTypes[i].BrandName == "")
                    name = $scope.TagsTypes[i].BrandType;
                else
                    name = $scope.TagsTypes[i].BrandName;
                break;
            }
        }
        return name;
    }
    $scope.getSentimentStyle = function (sentiment) {
        var FontColor = '';
        if (sentiment == "Positive")
            FontColor = '#67c23f';
        else if (sentiment == "Neutral")
            FontColor = '#575757';
        else if (sentiment == "Negative")
            FontColor = '#e11920';
        else if (sentiment == null || sentiment == "Unknown")
            FontColor = '#3c8dbc';

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
    $scope.changeSentimentOfPost = function (feedId, MessageType, sentiment, index) {
        UnifiedDeckService.InsertSentimentForFeed(feedId, MessageType, sentiment, $rootScope.selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                $scope.inboxitemDetails[index].Sentiment = sentiment;
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in updating sentiment, please try again.");
            console.log(err);
        });
    }
    $scope.removeTagFromPost = function (feedId, MessageType, TagId, TagName, tagIndex, feedIndex) {
        console.log("Remove");
        UnifiedDeckService.RemoveTagFromFeed(feedId, MessageType, TagId).then(function (result) {
            if (result.statusText == "OK") {
                $scope.inboxitemDetails[feedIndex].Tags.splice(tagIndex, 1);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }
    $scope.quickRemoveTagFromPost = function (feedId, MessageType, TagName, tagIndex, feedIndex) {
        //find tag id
        var TagId = -1;
        for (var i = 0; i < $scope.TagsTypes.length; i++) {
            if ($scope.TagsTypes[i].BrandType == TagName) {
                TagId = $scope.TagsTypes[i].BrandTypeId;
                break;
            }
        }

        if (TagId != -1) {
            $scope.removeTagFromPost(feedId, MessageType, TagId, TagName, tagIndex, feedIndex);
        }
    }
    $scope.clickFont = function () {
        $('#colorpalette2').colorPalette()
            .on('selectColor', function (e) {
                $('#selected-color2 i').css('color', e.color);
                $scope.FontColor = e.color;
                //alert(e.color)
            });
        // alert("click")
    }
    $scope.clickBG = function () {
        $('#colorpalette3').colorPalette()
            .on('selectColor', function (e) {
                $('#selected-color2 i').css('background-color', e.color);
                $scope.BGColor = e.color;
            });

    }

    $scope.clickFontEdit = function () {
        $('#colorpalette2edit').colorPalette()
            .on('selectColor', function (e) {
                $('#selected-color2-edit i').css('color', e.color);
                $scope.FontColorEdit = e.color;
                //alert(e.color)
            });
        // alert("click")
    }
    $scope.clickBGEdit = function () {
        $('#colorpalette3edit').colorPalette()
            .on('selectColor', function (e) {
                $('#selected-color2-edit i').css('background-color', e.color);
                $scope.BGColorEdit = e.color;
            });

    }

    $scope.IsPersonal = $rootScope.IsPersonal.toString();
    $scope.init = function () {
        if ($stateParams.pageType == 'Unified') {
            $scope.hideTCFilter = false;

        }
        else {
            $scope.hideTCFilter = true;
        }

        $scope.IsActive = true;
        $scope.PageEmpty = false;
        $scope.result1 = '';
        $scope.options1 = { types: '(cities)' };
        $scope.details1 = '';
        $scope.TagsTypes = [];
        $scope.TagText = "";
        $scope.TagName = "";
        $scope.KeywordText = "";
        $scope.KeywordName = "";
        $scope.TagTextEdit = "";
        $scope.TagIdEdit = 0;
        $scope.TagNameEdit = "";
        $scope.HCItems = false;
        $scope.SComplaints = false;
        $scope.PageNumber = 0;
        $scope.PageSize = 30; //100;
        $scope.FontColorEdit = "";
        $scope.BGColorEdit = "";
        $scope.ActivePattern = "";
        $scope.AuthorName = "";
        $scope.AuthorUserName = "";
        /*$('#grplists').slimscroll({
            height: '485px',
            disableFadeOut: true
        }).bind('slimscroll', function (e, pos) {
            if (pos == "bottom") {
                $scope.PageNumber++;
                $scope.fetchUnifiedInboxData();
            }
        });*/
        $scope.FbCount = 0;
        $scope.InCount = 0;
        $scope.TwitCount = 0;
        $scope.AnaCount = 0;
        $scope.ConsumerNumber = "0";
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.instaIcon = '/Content/images/insta_ico16.png';
        $scope.selected = {};
        $scope.selectedSent = {};
        $scope.selectedMsg = {};
        $scope.selectedTeamMembers = {};
        $scope.selectedbrand = [];
        $scope.selectedtag = [];
        $scope.selectedConsumerNum = [];
        $scope.select = true;
        $scope.selectMsg = true;
        $scope.selectTeamMember = false;
        $scope.selectSent = false;
        $scope.selectBrand = false;
        $scope.selectTag = false;
        $scope.selectConsumer = false;
        $scope.sentTypes = [
            { sentId: "1", sentName: "Positive" },
            { sentId: "2", sentName: "Neutral" },
            { sentId: "3", sentName: "Negative" }
        ];
        $scope.fbMsgTypes = [];
        $scope.TwitterMsgTypes = [];
        $scope.InstagramMsgTypes = [];
        $scope.BrandsTypes = [];

        $scope.dateRange = {
            startDate: moment(), //moment().subtract(7, "days"),
            endDate: moment()
        };
        $scope.opts = {
            locale: {
                format: 'DD/MM/YYYY'
            },
            "maxDate": $scope.dateRange.endDate,
            "dateLimit": { "days": 180 },
            "eventHandlers": {
                'apply.daterangepicker': function (ev, picker) {
                    $scope.SearchByUserName();
                }
            }
        };

        $scope.getProfileDetails($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString());
        $scope.GetTeamDetails($rootScope.selectedGroupId);
    };

    function preg_quote(str) {

        return (str + '').replace(/([\\\.\+\*\?\[\^\]\$\(\)\{\}\=\!\<\>\|\:])/g, "\\$1");
    }
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


        for (i = 0; i < tags.length; i++) {
            var BGColor = '';
            var FontColor = '';
            for (j = 0; j < $scope.TagsTypes.length; j++) {
                if ($scope.TagsTypes[j].BrandType == tags[i]) {
                    BGColor = $scope.TagsTypes[j].BGColor;
                    FontColor = $scope.TagsTypes[j].FontColor;
                    break;
                }
            }
            var style = {
                "background-color": BGColor,
                "color": FontColor

            };
            //text1 = text1.replace(new RegExp("(" + preg_quote(tags[i]) + ")", 'gi'), "<b style='color:" + FontColor + ";background-color:" + BGColor + ";' >$1</b>"); //ng-style={'background-color':" + BGColor + ",'color':" + FontColor + "}
            text1 = text1.replace(new RegExp("(" + preg_quote(tags[i]) + ")", 'gi'), "<b>$1</b>");
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
                if (MessageType == "TM" || MessageType == "TP")
                    str = str.replace(users[i], "<a target='_blank' href='//twitter.com/" + users[i].split('@')[1] + "'>" + users[i] + "</a>");
                else if (MessageType == "IM" || MessageType == "IC")
                    str = str.replace(users[i], "<a target='_blank' href='//instagram.com/" + users[i].split('@')[1] + "'>" + users[i] + "</a>");
            }
        }

        $("#" + Id + "-" + MessageType + "-converted_url").html(str);
        // return text;



    }
    /*$scope.Profiles = [
        {
            ProfileIco: "fb_ico16",
            HandleName: " @AyeshaShamim7",
            Id:1
        },
        {
            ProfileIco: "in_ico16",
            HandleName: " @AyeshaShamim7",
            Id:2
        },
        {
            ProfileIco: "t_ico16",
            HandleName: " @AyeshaShamim7",
            Id:3
        }
    ];*/

    $scope.PageType = $stateParams.pageType;

    $scope.fetchConsumerProfiles = function () {
        var selectedProfiles = $.grep($scope.Profiles, function (record) {
            return $scope.selected[record.UserId];
        });

        var profileData = [];
        var profilePageData = [];
        $.each(selectedProfiles, function (key, value) {
            if (value.IsPage == true) {
                profilePageData.push(value.ProfileId);
            }
            else {
                profileData.push(value.ProfileId);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";

        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";

        var groupId = $rootScope.selectedGroupId;

        UnifiedService.getCustomerSocialProfiles(commaSeperatedProfiles, commaSeperatedPages, groupId).then(function (l) {

            var response = l.data;
            if (response)
                $scope.customerProfiles = response;

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching customer profiles");
        });
    }

    $scope.checkAllProfilesAndMsgTypes = function () {
        $scope.HCItems = false;
        $scope.select = true;
        $scope.selectMsg = true;
        $scope.selectTeamMember = false;
        $scope.selectSent = false;
        $scope.selectTag = false;
        angular.forEach($scope.Profiles, function (obj) {
            $scope.selected[obj.UserId] = $scope.select;
        });

        angular.forEach($scope.msgTypes, function (obj) {
            $scope.selectedMsg[obj.msgTypeId] = $scope.selectMsg;
        });
        angular.forEach($scope.TeamMembers, function (obj) {
            $scope.selectedTeamMembers[obj.TeamMemberId] = $scope.selectTeamMember;
        });
        angular.forEach($scope.sentTypes, function (obj) {
            $scope.selectedSent[obj.sentId] = $scope.selectSent;
        });
        angular.forEach($scope.BrandsTypes, function (obj) {
            $scope.selectedbrand[obj.BrandTypeId] = $scope.selectBrand;
        });
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedtag[obj.BrandTypeId] = $scope.selectTag;
        });
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        //$scope.fetchConsumerProfiles();
        $scope.fetchUnifiedInboxData();
    }

    $scope.SearchByUserName = function () {
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };
    $scope.checkAll = function () {
        angular.forEach($scope.Profiles, function (obj) {
            $scope.selected[obj.UserId] = $scope.select;
        });

        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };
    $scope.checkAllSents = function () {
        angular.forEach($scope.sentTypes, function (obj) {
            $scope.selectedSent[obj.sentId] = $scope.selectSent;
        });

        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };
    $scope.checkAllMsgs = function () {
        angular.forEach($scope.msgTypes, function (obj) {
            $scope.selectedMsg[obj.msgTypeId] = $scope.selectMsg;
        });

        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };
    $scope.checkAllTeamMembers = function () {
        angular.forEach($scope.TeamMembers, function (obj) {
            $scope.selectedTeamMembers[obj.TeamMemberId] = $scope.selectTeamMember;
        });

        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };
    $scope.checkAllBrands = function () {
        angular.forEach($scope.BrandsTypes, function (obj) {
            $scope.selectedbrand[obj.BrandTypeId] = $scope.selectBrand;
        });

        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    $scope.checkAllTags = function () {
        angular.forEach($scope.TagsTypes, function (obj) {
            $scope.selectedtag[obj.BrandTypeId] = $scope.selectTag;
        });

        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    $scope.checkAllConsumerNumbers = function () {
        angular.forEach($scope.consumerNumbers, function (obj) {
            $scope.selectedConsumerNum[obj.Value] = $scope.selectConsumer;
        });

        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    $scope.GetTeamDetails = function (groupId) {
        $scope.TeamMembers = [];
        //  $scope.taskAssigneemodel = [];
        GroupService.GetTeamByGroupIdForTaskList(groupId).then(function (l) {

            if (l != null) {
                var teamData = l.data.Team;

                for (i = 0; i < teamData.length; i++) {
                    var obj = { TeamMemberId: teamData[i].TeamMemberId, Name: teamData[i].Name }
                    $scope.TeamMembers.push(obj);
                    //  $scope.taskAssigneemodel.push({ id: teamData[i].TeamMemberId });
                }
                // $scope.taskAssigneemodel = $scope.taskAssignee;
            }
            else {
                //alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            }

            // $scope.GetGroupDetails($scope.GroupId);

        }, function (err) {
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    };

    $scope.getProfileDetails = function (groupId, companyId, isPersonal) {
        $scope.IsPersonal = $rootScope.IsPersonal.toString();

        UnifiedService.GetAllProfilesForUserGroup(groupId, companyId, isPersonal).then(function (l) {
            //debugger;
            var response = l.data;
            $scope.fbMsgTypes = [];
            $scope.TwitterMsgTypes = [];
            $scope.InstagramMsgTypes = [];
            if ($rootScope.FBPosts) {
                $scope.fbMsgTypes.push({
                    msgTypelogo: "fb_ico16",
                    MsgType: " Wall Posts",
                    msgTypeId: "1"
                });
            }
            if ($rootScope.FBComs) {
                $scope.fbMsgTypes.push({
                    msgTypelogo: "fb_ico16",
                    MsgType: " Comments",
                    msgTypeId: "2"
                });
            }
            if ($rootScope.FBChats) {
                $scope.fbMsgTypes.push({
                    msgTypelogo: "fb_ico16",
                    MsgType: " Page Chats",
                    msgTypeId: "7"
                });
            }
            if ($rootScope.TwitTweets) {
                $scope.TwitterMsgTypes = [{
                    msgTypelogo: "t_ico16",
                    MsgType: " Sent Tweets",
                    msgTypeId: "3"
                },
                {
                    msgTypelogo: "t_ico16",
                    MsgType: " Mentions",
                    msgTypeId: "4"
                },
                {
                    msgTypelogo: "t_ico16",
                    MsgType: " Retweets",
                    msgTypeId: "6"
                }];
            }
            if ($rootScope.TwitDMs) {
                $scope.TwitterMsgTypes.push({
                    msgTypelogo: "t_ico16",
                    MsgType: " Direct Messages",
                    msgTypeId: "5"
                });
            }
            if ($rootScope.IMPosts) {
                $scope.InstagramMsgTypes.push({
                    msgTypelogo: "insta_ico16",
                    MsgType: " Wall Posts",
                    msgTypeId: "8"
                });
            }
            if ($rootScope.IMComs) {
                $scope.InstagramMsgTypes.push({
                    msgTypelogo: "insta_ico16",
                    MsgType: " Comments",
                    msgTypeId: "9"
                });
            }

            $scope.FbCount = 0;
            $scope.InCount = 0;
            $scope.TwitCount = 0;
            for (var i = 0; i < response.length; i++) {
                if (response[i].ProfileTypeId == 1) {
                    if (!$rootScope.FBPosts && !$rootScope.FBComs && !$rootScope.FBChats) {
                        response.splice(i, 1);
                        continue;
                    }
                    $scope.FbCount++;
                }
                else if (response[i].ProfileTypeId == 6) {
                    if (!$rootScope.IMPosts && !$rootScope.IMComs) {
                        response.splice(i, 1);
                        continue;
                    }
                    $scope.InCount++;
                }
                else if (response[i].ProfileTypeId == 4) {
                    if (!$rootScope.TwitTweets && !$rootScope.TwitDMs) {
                        response.splice(i, 1);
                        continue;
                    }
                    $scope.TwitCount++;
                }
            }

            $scope.Profiles = response;
            $scope.msgTypes = [];

            //if ($scope.FbCount > 0 || $scope.TwitCount > 0) {
            //    var complaintMsgType = [{
            //        msgTypelogo: "fb_ico16",
            //        MsgType: " Complaints",
            //        msgTypeId: "9"
            //    }];
            //    $scope.msgTypes.push.apply($scope.msgTypes, complaintMsgType);
            //}

            if ($scope.FbCount > 0) {
                $scope.msgTypes.push.apply($scope.msgTypes, $scope.fbMsgTypes);
            }

            if ($scope.TwitCount > 0) {
                $scope.msgTypes.push.apply($scope.msgTypes, $scope.TwitterMsgTypes);
            }

            if ($scope.InCount > 0) {
                $scope.msgTypes.push.apply($scope.msgTypes, $scope.InstagramMsgTypes);
            }

            if (isPersonal == 'false') {
                TaskService.GetBrandKeywordByCompanyId(companyId).then(function (l) {
                    var response = l.data;
                    $scope.BrandsTypes = [];
                    $scope.TagsTypes = [];
                    for (var i = 0; i < response.length; i++) {
                        if (response[i].IsTag)
                            $scope.TagsTypes.push({ BrandType: response[i].KeywordText, BrandName: response[i].KeywordName, BrandTypeId: response[i].Id, FontColor: (response[i].FontColor == null ? '#ffffff' : response[i].FontColor), BGColor: (response[i].BGColor == null ? '#6e889c' : response[i].BGColor) });
                        else
                            $scope.BrandsTypes.push({ BrandType: response[i].KeywordText, BrandName: response[i].KeywordName, BrandTypeId: response[i].Id, LocationName: response[i].LocationName });
                    }

                    $scope.checkAllProfilesAndMsgTypes();
                    $scope.setPattern();
                    //Show Empty Page Screen if no profiles are attached and no keywords are defined
                    if ($scope.BrandsTypes.length == 0 && $scope.Profiles.length == 0)
                        $scope.PageEmpty = true;
                    else
                        $scope.PageEmpty = false;
                }, function (err) {
                    alert('Error in fetching brands');
                });
            }
            else {
                $scope.checkAllProfilesAndMsgTypes();
            }

        }, function (err) {
            // alert('Error in fetching User Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching User Profiles.");
            //console.log(err);
        });
    };

    $scope.addToArchiveList = function (obj) {
        UnifiedService.AddMsgToArchiveList(obj.MessageType, obj.Id, $rootScope.selectedGroupId, $rootScope.selectedCompanyId).then(function (l) {
            obj.IsArchived = true;
            /*$scope.inboxitemDetails = $scope.inboxitemDetails.filter(function (el) {
                return el.Id !== obj.Id;
            });*/
            console.log("Msg Archived Successfully");
        }, function (err) {
            // alert('Error in Archiving, please try again.');

            GlobalService.ErrorPopup("Error", "Error in Archiving, please try again.");
            console.log(err);
        });
    };

    $scope.removeFromArchiveList = function (obj) {
        UnifiedService.RemoveMsgFromArchiveList(obj.MessageType, obj.Id, $rootScope.selectedGroupId, $rootScope.selectedCompanyId).then(function (l) {
            obj.IsArchived = false;

            if ($scope.PageType == "Archive") {
                $scope.inboxitemDetails = $scope.inboxitemDetails.filter(function (el) {
                    return el.Id !== obj.Id;
                });
            }
            console.log("Msg UnArchived Successfully");
        }, function (err) {
            // alert('Error in UnArchiving, please try again.');
            GlobalService.ErrorPopup("Error", "Error in UnArchiving, please try again.");
            console.log(err);
        });
    };

    $scope.profileSelectedChanged = function () {
        $scope.select = false;
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
        //$scope.fetchConsumerProfiles();
    };

    $scope.msgSelectedChanged = function () {
        $scope.selectMsg = false;
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    $scope.teamMemberSelectedChanged = function () {
        $scope.selectTeamMember = false;
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    $scope.sentSelectedChanged = function () {
        $scope.selectSent = false;
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    $scope.brandSelectedChanged = function () {
        $scope.selectBrand = false;
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    $scope.tagSelectedChanged = function () {
        $scope.selectTag = false;
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };


    $scope.loadConsumerNumbers = function () {
        var selectedProfile = $scope.ConnectedProfile;
        var selectedProfiles = $.grep($scope.Profiles, function (record) {
            return $scope.selected[record.UserId];
        });

        var profileData = [];
        var profilePageData = [];
        $.each(selectedProfiles, function (key, value) {
            if (value.IsPage == true) {
                profilePageData.push(value.ProfileId);
            }
            else {
                profileData.push(value.ProfileId);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";

        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";

        var groupId = $rootScope.selectedGroupId;

        if (selectedProfile) {
            UnifiedService.getConsumerNumbersByProfile(selectedProfile, commaSeperatedProfiles, commaSeperatedPages, groupId).then(function (l) {
                var response = l.data;
                // alert(response)
                if (response)
                    $scope.consumerNumbers = response;

            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching consumer numbers");
                // alert('Error in fetching Inbox data');
                //console.log(err);
            });
        }
    };

    $scope.onSelectedProfile = function ($item) {
        var selectedProfile = $item.Value;
        var selectedProfiles = $.grep($scope.Profiles, function (record) {
            return $scope.selected[record.UserId];
        });

        var profileData = [];
        var profilePageData = [];
        $.each(selectedProfiles, function (key, value) {
            if (value.IsPage == true) {
                profilePageData.push(value.ProfileId);
            }
            else {
                profileData.push(value.ProfileId);
            }
        });

        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";

        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";

        var groupId = $rootScope.selectedGroupId;

        if (selectedProfile) {
            UnifiedService.getConsumerNumbersByProfile(selectedProfile, commaSeperatedProfiles, commaSeperatedPages, groupId).then(function (l) {
                var response = l.data;
                // alert(response)
                if (response)
                    $scope.consumerNumbers = response;

            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching consumer numbers");
                // alert('Error in fetching Inbox data');
                //console.log(err);
            });
        }
        console.log("SelectedValue: " + $item);
    }

    $scope.consumerNumSelectedChanged = function () {
        $scope.selectConsumer = false;
        $scope.PageNumber = 0;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize);
        $scope.fetchUnifiedInboxData();
    };

    //$scope.consumerNumberSelectedChanged = function () {
    //    $scope.PageNumber = 0;
    //    $scope.fetchUnifiedInboxData();
    //};

    $scope.profileLoadMoreFeeds = function () {
        $scope.PageNumber++;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize) + $scope.newInboxItemsCount;
        $scope.fetchUnifiedInboxData();
    };

    $scope.recentMessagesTWId = 0;
    $scope.recentPageChatsFBId = 0;
    $scope.recentPostsFBId = 0;
    $scope.recentPostsTWId = 0;
    $scope.recentCommentsFBId = 0;
    $scope.recentInstagramId = 0;
    $scope.recentSearchPostTWId = 0;
    $scope.recentInstagramCommentId = 0;
    $scope.newInboxItemsCount = 0;
    $scope.consumerNumbers = new Array();

    $scope.tempURL = $sce.trustAsResourceUrl('https://www.facebook.com/bollywoodnow/videos/10153982156706925/');
    $scope.tempURLIFrame = $sce.trustAsResourceUrl('http://www.dailymotion.com/embed/video/x4q8d7d');


    $scope.fetchUnifiedInboxData = function () {
        var selectedProfiles = $.grep($scope.Profiles, function (record) {
            return $scope.selected[record.UserId];
        });

        var selectedTeam = $.grep($scope.TeamMembers, function (record) {
            return $scope.selectedTeamMembers[record.TeamMemberId];
        });

        var selectedMsgTypes = $.grep($scope.msgTypes, function (record) {
            return $scope.selectedMsg[record.msgTypeId];
        });

        var selectedSentTypes = $.grep($scope.sentTypes, function (record) {
            return $scope.selectedSent[record.sentId];
        });

        var selectedBrandTypes = $.grep($scope.BrandsTypes, function (record) {
            return $scope.selectedbrand[record.BrandTypeId];
        });

        var selectedTagTypes = $.grep($scope.TagsTypes, function (record) {
            return $scope.selectedtag[record.BrandTypeId];
        });

        var profileData = [];
        var profilePageData = [];
        var msgTypeData = [];
        var sentTypeData = [];
        var brandTypeData = [];
        var tagTypeData = [];
        var teamData = [];

        $.each(selectedProfiles, function (key, value) {
            if (value.IsPage == true) {
                profilePageData.push(value.ProfileId);
            }
            else {
                profileData.push(value.ProfileId);
            }
        });

        $.each(selectedTeam, function (key, value) {
            teamData.push(value.TeamMemberId);
        });

        $.each(selectedMsgTypes, function (key, value) {
            msgTypeData.push(value.msgTypeId);
        });

        $.each(selectedSentTypes, function (key, value) {
            sentTypeData.push(value.sentId);
        });

        $.each(selectedBrandTypes, function (key, value) {
            brandTypeData.push(value.BrandTypeId);
        });

        $.each(selectedTagTypes, function (key, value) {
            tagTypeData.push(value.BrandTypeId);
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

        var commaSeperatedTeam = teamData.join(",");
        if (commaSeperatedTeam == "")
            commaSeperatedTeam = "0";

        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";

        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        var commaSeperatedSentTypes = sentTypeData.join(",");
        if (commaSeperatedSentTypes == "")
            commaSeperatedSentTypes = "0";

        var commaSeperatedConsumerNumbers = "0";

        if (true) {
            var selectedConsumerNumbers = $.grep($scope.consumerNumbers, function (record) {
                return $scope.selectedConsumerNum[record.Value];
            });

            var consumerNumberData = [];

            $.each(selectedConsumerNumbers, function (key, value) {
                consumerNumberData.push(value.Value);
            });

            commaSeperatedConsumerNumbers = consumerNumberData.join(",");
            if (commaSeperatedConsumerNumbers == "")
                commaSeperatedConsumerNumbers = "0";
        }
        //jin        
        var authorName = '0';
        var authorHandle = '0';
        if ($scope.AuthorName != "")
            authorName = $scope.AuthorName;

        if ($scope.AuthorUserName != "") {
            if ($scope.AuthorUserName.startsWith("@"))
                authorHandle = $scope.AuthorUserName.substring(1);
            else
                authorHandle = $scope.AuthorUserName;
        }
        var dateFrom = moment($scope.dateRange.startDate).add(5, "hours").toDate();
        var dateTo = moment($scope.dateRange.endDate).add(5, "hours").toDate();

        UnifiedService.GetSelectedProfilesActivity(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedBrandTypes, commaSeperatedSentTypes, $scope.Counter,
            $scope.PageSize, $scope.PageType, commaSeperatedTeam, $rootScope.selectedGroupId, $rootScope.selectedCompanyId, !$scope.HCItems, commaSeperatedConsumerNumbers, $scope.SComplaints, authorName, authorHandle, dateFrom, dateTo).then(function (l) {
                //
                var response = l.data;

                response.forEach(function (i) {
                    i.Attachments.forEach(function (i) {
                        if (i.Type == 'video_fb' || i.Type == 'video_share' || i.Type == 'twitVid_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });

                //console.log(l.data);
                if ($scope.PageNumber == 0) {
                    $scope.inboxitemDetails = response;
                    if (response.length == 0) {
                        $scope.inboxitemDetails = new Array();
                    }
                }
                else {
                    if (response.length == 0) {
                        $scope.PageNumber--;
                    }
                    else {
                        $scope.inboxitemDetails.push.apply($scope.inboxitemDetails, response);
                    }
                }

                /*$scope.inboxitemDetails.forEach(function(i){
                    i.Attachments.forEach(function(i){
                        if (i.Type == 'video_fb' || i.Type == 'video_share' || i.Type == 'twitVid_share')
                            i.Url = $sce.trustAsResourceUrl(i.Url);
                    });
                });*/

                var data = $.grep(response, function (record) {
                    return record.MessageType == 'TM'
                });

                if (data.length > 0)
                    $scope.recentMessagesTWId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentMessagesTWId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentMessagesTWId;

                data = $.grep(response, function (record) {
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
                    return record.MessageType == 'TP'
                });

                if (data.length > 0)
                    $scope.recentPostsTWId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentPostsTWId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentPostsTWId;

                data = $.grep(response, function (record) {
                    return record.MessageType == 'FC'
                });

                if (data.length > 0)
                    $scope.recentCommentsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentCommentsFBId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentCommentsFBId;

                data = $.grep(response, function (record) {
                    return record.MessageType == 'IM'
                });

                if (data.length > 0)
                    $scope.recentInstagramId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentInstagramId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentInstagramId;

                data = $.grep(response, function (record) {
                    return record.MessageType == 'IC'
                });

                if (data.length > 0)
                    $scope.recentInstagramCommentId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentInstagramCommentId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentInstagramCommentId;

                data = $.grep(response, function (record) {
                    return record.MessageType == 'STP'
                });

                if (data.length > 0)
                    $scope.recentSearchPostTWId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentSearchPostTWId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentSearchPostTWId;
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching Inbox data");
                // alert('Error in fetching Inbox data');
                //console.log(err);
            });
    }


    /*$scope.msgTypes = [
        {
            msgTypelogo: "t_ico16",
            MsgType: " Mentions",
            msgTypeId:"1"
        },
          {
              msgTypelogo: "t_ico16",
              MsgType: " Direct Messages",
              msgTypeId: "2"
          },
            {
                msgTypelogo: "t_ico16",
                MsgType: " Retweets",
                msgTypeId: "3"
            },
              {
                  msgTypelogo: "t_ico16",
                  MsgType: " New Followers",
                  msgTypeId: "4"
              },
                {
                    msgTypelogo: "fb_ico16",
                    MsgType: " Wall Posts",
                    msgTypeId: "5"
                } ,
                {
                    msgTypelogo: "fb_ico16",
                    MsgType: " Comments",
                    msgTypeId: "6"
                }, {
                    msgTypelogo: "fb_ico16",
                    MsgType: " Private Messages",
                    msgTypeId: "7"
                }
    ];
    */
    $scope.items = [{
        id: 0,
        label: 'Select Location'
    }, {
        id: 1,
        label: 'Lahore'
    }, {
        id: 2,
        label: 'Karachi'
    }, {
        id: 3,
        label: 'Islamabad'
    }];


    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.getProfileDetails(msg.GroupId, msg.CompanyId, msg.IsPersonal);
        $scope.GetTeamDetails(msg.GroupId);
    });

    $scope.init();
    $scope.CreateTask = function (msg) {
        UnifiedService.ProfileDataObj = msg;
        $window.location.href = "/#/CreateCase?ActivityId=" + msg.Id + "&ActivityTable=" + msg.MessageType + "&Status=" + msg.IsTaskCreated;
    }
    $scope.openReplyAdd = function (obj) {
        $scope.items = { status: obj, FormType: "Reply" };

        if (obj.MessageType == "IM" || obj.MessageType == "IC") {
            //Instagram Reply View
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
        else {
            //Twitter & Facebook Reply View
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
                //  $scope.selected = selectedItem;
                //  alert($scope.selected)
            }, function () {
                // $log.info('Modal dismissed at: ' + new Date());
            });
        }


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
            // $scope.selected = selectedItem;
            // alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.openRetweet = function (obj) {
        $scope.items = { status: obj, FormType: "Retweet" };
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
        $scope.items = { status: obj, FormType: "RetweetComment" };
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
            //  $scope.selected = selectedItem;
            //  alert($scope.selected)
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
            //  $scope.selected = selectedItem;
            //  alert($scope.selected)
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
            //  $scope.selected = selectedItem;
            //  alert($scope.selected)
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.openUpdateTags = function (obj) {
        $scope.items = { status: obj, AllTags: $scope.TagsTypes };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_UpdateTags.html',
            controller: '_UpdateTagsController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //  $scope.selected = selectedItem;
            //  alert($scope.selected)
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
            //  $scope.selected = selectedItem;
            // alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });

    }

    $scope.deleteObj = function (obj) {
        var isConfirmed = confirm('Are you sure you want to delete?');
        if (isConfirmed) {
            if (obj.MessageType == "FP" || obj.MessageType == "FC") {
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
                        $scope.inboxitemDetails = $scope.inboxitemDetails.filter(function (el) {
                            return el.StatusId.indexOf(sendingPostId) == -1;
                        });
                    }
                    else {
                        GlobalService.ErrorPopup("Error", l.data);
                    }
                }, function (err) {
                    // alert('Error in Archiving, please try again.');

                    GlobalService.ErrorPopup("Error", "Error in Deleting Facebook Object, please try again.");
                    console.log(err);
                });
            }
            else if (obj.MessageType == "TP") {
                UnifiedDeckService.DeleteTwitterTweet(obj.User, obj.StatusId).then(function (l) {
                    if (l.data == "success") {
                        notify({
                            message: "Tweet deleted Successfully",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.inboxitemDetails = $scope.inboxitemDetails.filter(function (el) {
                            return el.Id !== obj.Id;
                        });
                    }
                    else {
                        GlobalService.ErrorPopup("Error", l.data);
                    }
                }, function (err) {
                    // alert('Error in Archiving, please try again.');

                    GlobalService.ErrorPopup("Error", "Error in Deleting Tweet, please try again.");
                    console.log(err);
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
                        $scope.inboxitemDetails = $scope.inboxitemDetails.filter(function (el) {
                            return el.Id !== obj.Id;
                        });
                    }
                    else {
                        GlobalService.ErrorPopup("Error", l.data);
                    }
                }, function (err) {
                    // alert('Error in Archiving, please try again.');

                    GlobalService.ErrorPopup("Error", "Error in Deleting Twitter DM, please try again.");
                    console.log(err);
                });
            }
            else if (obj.MessageType == "IC") {
                UnifiedDeckService.DeleteInstagramComment(obj.UserId, obj.StatusId).then(function (l) {
                    if (l.data == "success") {
                        notify({
                            message: "Instagram Comment deleted Successfully",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.inboxitemDetails = $scope.inboxitemDetails.filter(function (el) {
                            return el.Id !== obj.Id;
                        });
                    }
                    else {
                        GlobalService.ErrorPopup("Error", l.data);
                    }
                }, function (err) {
                    // alert('Error in Archiving, please try again.');

                    GlobalService.ErrorPopup("Error", "Error in Deleting Instagram Comment, please try again.");
                    console.log(err);
                });
            }
        }
    }

    $scope.hideObj = function (obj) {
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
                str = "This Post/Comment is already hidden, are you sure you want to unhide?";
                msg = "Facebook Object Unhidden Successfully";
                hide = false;
            }
            else {
                str = "Are you sure you want to hide?";
                msg = "Facebook Object Hidden Successfully";
                hide = true;
            }

            var isConfirmed = confirm(str);
            if (isConfirmed) {
                FacebookService.FacebookHideUnhidePost(obj.UserId, obj.IsPage, sendingPostId, hide).then(function (l) {
                    notify({
                        message: msg,
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }, function (err) {
                    GlobalService.ErrorPopup("Error", err.data.Message);
                    // alert('Error in post tweet, please try again.');
                    console.log(err);
                });
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            // alert('Error in post tweet, please try again.');
            console.log(err);
        });
    }

    $scope.AddBrandKeyword = function (obj) {
        var locationName = ($scope.result1 == "") ? 0 : $scope.result1;
        var locationLat = ($scope.details1 == "") ? 0 : $scope.details1.geometry.location.lat();
        var locationLng = ($scope.details1 == "") ? 0 : $scope.details1.geometry.location.lng();
        TaskService.AddBrandKeyword($scope.KeywordText, $scope.KeywordName, locationName, locationLat, locationLng, $rootScope.selectedGroupId, false).then(function (l) {
            //obj.IsArchived = false;
            $scope.BrandsTypes.push({ BrandType: $scope.KeywordText, BrandName: $scope.KeywordName, BrandTypeId: l.data, LocationName: locationName });
            $scope.selectedbrand[l.data] = false;
            $scope.KeywordText = "";
            $scope.KeywordName = "";
            $scope.result1 = "";
            $scope.details1 = "";
            console.log("Brand Keyword added successfully");
            notify({
                message: "Brand Keyword added successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);

            //alert('Error in Adding Brand Keyword, please try again.');
            console.log(err);
        });
    }

    $scope.AddBrandTag = function (obj) {
        TaskService.AddBrandKeyword($scope.TagText, $scope.TagName, 0, 0, 0, $rootScope.selectedGroupId, true, $scope.FontColor, $scope.BGColor).then(function (l) {
            //obj.IsArchived = false;
            $scope.TagsTypes.push({ BrandType: $scope.TagText, BrandName: $scope.TagName, BrandTypeId: l.data, FontColor: $scope.FontColor, BGColor: $scope.BGColor });
            $scope.selectedtag[l.data] = false;
            $scope.TagText = "";
            $scope.TagName = "";
            console.log("Tag added successfully");
            notify({
                message: "Tag added successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);
            //  alert('Error in Adding Brand Tag, please try again.');
            console.log(err);
        });
    }

    $scope.HideCompletedItems = function () {
        // $scope.HCItems;
        $scope.fetchUnifiedInboxData();
    }
    $scope.ShowComplaints = function () {

        $scope.fetchUnifiedInboxData();
    }

    $scope.ShowOutlook = function () {

        window.location.href = "/#/Outlook";
    }

    $scope.MarkAsCompleted = function (msg, index) {
        // alert(JSON.stringify(msg));
        TaskService.InsertTaskCompleted(msg.Id, msg.MessageType, true, 0, $rootScope.selectedGroupId, msg.UserTwit, $rootScope.selectedCompanyId, msg.StatusId, msg.UserId, msg.IsPage).then(function (l) {
            //obj.IsArchived = false;

            msg.IsTaskCompleted = true;
            notify({
                message: "Successfully Mark as Completed",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            //alert();
            if ($scope.HCItems == true && $stateParams.pageType == 'Unified')
                $("#inbox-" + msg.Id).slideUp("slow")
            console.log("Successfully Mark as Completed");
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error, please try again. " + err);

            // alert('Error, please try again.' + JSON.stringify(err));
            console.log(err);
        });
        //UnifiedService.ProfileDataObj = msg;
        // $window.location.href = "/#/CreateTask?ActivityId=" + msg.Id + "&ActivityTable=" + msg.MessageType + "&Status=" + (msg.IsTaskCreated == true ? 1 : 0);
    }

    $scope.MarkAsInComplete = function (msg, index) {



        $scope.items = { msg: msg, ConfirmationTitle: "Mark as Incomplete", ConfirmationMessage: "Are you sure, you want to mark this as incomplete? " };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ConfirmationPopUp.html',
            controller: '_MarkAsInCompleteController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            if (selectedItem == 'OK') {
                msg.IsTaskCompleted = false;
                if ($scope.PageType == "Completed") {
                    $scope.inboxitemDetails = $scope.inboxitemDetails.filter(function (el) {
                        return el.Id !== msg.Id;
                    });
                }
            }
            //  $scope.selected = selectedItem;
            // alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });


        //  alert(JSON.stringify(msg));
        //TaskService.InsertTaskCompleted(msg.Id, msg.MessageType, true, 0, $window.localStorage.getItem('selectedGroupId')).then(function (l) {
        //    //obj.IsArchived = false;
        //    alert("Successfully Mark as Completed");
        //    console.log("Successfully Mark as Completed");
        //}, function (err) {
        //    alert('Error, please try again.' + JSON.stringify(err));
        //    console.log(err);
        //});
        //UnifiedService.ProfileDataObj = msg;
        // $window.location.href = "/#/CreateTask?ActivityId=" + msg.Id + "&ActivityTable=" + msg.MessageType + "&Status=" + (msg.IsTaskCreated == true ? 1 : 0);
    }
    $scope.openProfileConnect = function () {
        //var path = window.location.href.split('?')[1]
        //if (path == null)
        //    {
        //    $window.location.href = $window.location.href + '?query=value';
        //}
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
            // alert($scope.selected)
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.modelOptions = {
        debounce: {
            default: 500,
            blur: 250
        },
        getterSetter: true
    };

    $scope.customerProfiles;
    $scope.latestInboxitemDetails = new Array();

    $scope.loadLatestInboxItems = function () {
        Array.prototype.push.apply($scope.latestInboxitemDetails, $scope.inboxitemDetails);
        $scope.inboxitemDetails = $scope.latestInboxitemDetails;
        $scope.latestInboxitemDetails = new Array();
    };

    $scope.fetchLatestUnifiedInboxData = function () {
        var selectedProfiles = $.grep($scope.Profiles, function (record) {
            return $scope.selected[record.UserId];
        });

        var selectedMsgTypes = $.grep($scope.msgTypes, function (record) {
            return $scope.selectedMsg[record.msgTypeId];
        });

        var selectedTeam = $.grep($scope.TeamMembers, function (record) {
            return $scope.selectedTeamMembers[record.TeamMemberId];
        });

        var selectedSentTypes = $.grep($scope.sentTypes, function (record) {
            return $scope.selectedSent[record.sentId];
        });

        var selectedBrandTypes = $.grep($scope.BrandsTypes, function (record) {
            return $scope.selectedbrand[record.BrandTypeId];
        });

        var selectedTagTypes = $.grep($scope.TagsTypes, function (record) {
            return $scope.selectedtag[record.BrandTypeId];
        });

        var selectedConsumerNumbers = $.grep($scope.consumerNumbers, function (record) {
            return $scope.selectedConsumerNum[record.Value];
        });

        var profileData = [];
        var profilePageData = [];
        var msgTypeData = [];
        var sentTypeData = [];
        var brandTypeData = [];
        var tagTypeData = [];
        var consumerNumberData = [];
        var teamData = [];
        $.each(selectedProfiles, function (key, value) {
            if (value.IsPage == true) {
                profilePageData.push(value.ProfileId);
            }
            else {
                profileData.push(value.ProfileId);
            }
        });

        $.each(selectedTeam, function (key, value) {
            teamData.push(value.TeamMemberId);
        });

        $.each(selectedMsgTypes, function (key, value) {
            msgTypeData.push(value.msgTypeId);
        });

        $.each(selectedSentTypes, function (key, value) {
            sentTypeData.push(value.sentId);
        });

        $.each(selectedBrandTypes, function (key, value) {
            brandTypeData.push(value.BrandTypeId);
        });

        $.each(selectedTagTypes, function (key, value) {
            tagTypeData.push(value.BrandTypeId);
        });

        $.each(selectedConsumerNumbers, function (key, value) {
            consumerNumberData.push(value.Value);
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

        var commaSeperatedTeam = teamData.join(",");
        if (commaSeperatedTeam == "")
            commaSeperatedTeam = "0";

        var commaSeperatedBrandTypes = brandTypeData.join(",");
        if (commaSeperatedBrandTypes == "")
            commaSeperatedBrandTypes = "0";

        var commaSeperatedTagTypes = tagTypeData.join(",");
        if (commaSeperatedTagTypes == "")
            commaSeperatedTagTypes = "0";

        var commaSeperatedSentTypes = sentTypeData.join(",");
        if (commaSeperatedSentTypes == "")
            commaSeperatedSentTypes = "0";

        var commaSeperatedConsumerNumbers = consumerNumberData.join(",");
        if (commaSeperatedConsumerNumbers == "")
            commaSeperatedConsumerNumbers = "0";

        var ids = $scope.recentMessagesTWId + '|' + $scope.recentPageChatsFBId + '|' + $scope.recentPostsFBId + '|' + $scope.recentPostsTWId + '|' + $scope.recentCommentsFBId + '|' + $scope.recentInstagramId + '|' + $scope.recentSearchPostTWId + '|' + $scope.recentInstagramCommentId;

        var authorName = '0';
        var authorHandle = '0';
        if ($scope.AuthorName != "")
            authorName = $scope.AuthorName;

        if ($scope.AuthorUserName != "") {
            if ($scope.AuthorUserName.startsWith("@"))
                authorHandle = $scope.AuthorUserName.substring(1);
            else
                authorHandle = $scope.AuthorUserName;
        }
        var dateFrom = moment($scope.dateRange.startDate).add(5, "hours").toDate();
        var dateTo = moment($scope.dateRange.endDate).add(5, "hours").toDate();

        UnifiedService.GetLatestUnifiedInboxData(commaSeperatedProfiles, commaSeperatedPages, commaSeperatedMsgTypes, commaSeperatedTagTypes, commaSeperatedBrandTypes, commaSeperatedSentTypes, 0,
            $scope.PageSize, $scope.PageType, commaSeperatedTeam, $rootScope.selectedGroupId, $rootScope.selectedCompanyId, !$scope.HCItems, $scope.SComplaints, commaSeperatedConsumerNumbers, authorName, authorHandle, dateFrom, dateTo, ids
        ).then(function (l) {
            if (l.data != null) {
                //alert(l.data);
                var response = l.data
                //$scope.newInboxItemsCount += response.length;
                //Array.prototype.push.apply(response, $scope.latestInboxitemDetails)
                //$scope.latestInboxitemDetails = response;
                var tempArray = new Array();

                if (response != null && response !== '') {
                    response.forEach(function (i) {
                        var data = $.grep($scope.latestInboxitemDetails, function (record) {
                            return record.MessageType == i.MessageType && record.Id == i.Id
                        });
                        if (data.length == 0) {
                            i.ClassName = "new_msg";
                            i.Attachments.forEach(function (j) {
                                if (j.Type == 'video_fb' || j.Type == 'video_share' || j.Type == 'twitVid_share')
                                    j.Url = $sce.trustAsResourceUrl(j.Url);
                            });
                            tempArray.push(i);
                        }
                    });
                }

                $scope.newInboxItemsCount += tempArray.length;
                Array.prototype.push.apply(tempArray, $scope.latestInboxitemDetails)
                $scope.latestInboxitemDetails = tempArray;

                $scope.loadLatestInboxItems();


                if (response.length > 0) {
                    var data = $.grep(response, function (record) {
                        return record.MessageType == 'TM'
                    });

                    if (data.length > 0)
                        $scope.recentMessagesTWId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));

                    data = $.grep(response, function (record) {
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
                        return record.MessageType == 'TP'
                    });

                    if (data.length > 0)
                        $scope.recentPostsTWId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));

                    data = $.grep(response, function (record) {
                        return record.MessageType == 'FC'
                    });

                    if (data.length > 0)
                        $scope.recentCommentsFBId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));

                    data = $.grep(response, function (record) {
                        return record.MessageType == 'IM'
                    });

                    if (data.length > 0)
                        $scope.recentInstagramId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));

                    data = $.grep(response, function (record) {
                        return record.MessageType == 'IC'
                    });

                    if (data.length > 0)
                        $scope.recentInstagramCommentId = Math.max.apply(Math, data.map(function (o) { return o.Id; })) > $scope.recentInstagramCommentId ? Math.max.apply(Math, data.map(function (o) { return o.Id; })) : $scope.recentInstagramCommentId;

                    data = $.grep(response, function (record) {
                        return record.MessageType == 'STP'
                    });

                    if (data.length > 0)
                        $scope.recentSearchPostTWId = Math.max.apply(Math, data.map(function (o) { return o.Id; }));
                }
            }
        }, function (err) {
            // GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });

    }

    $scope.deleteTag = function (id) {
        GlobalService.ConfirmPopup("Confirm", "Are you sure you want to delete?", function (selectedItem) {
            if (selectedItem) {
                UnifiedService.DeleteTag(id).then(function (l) {
                    console.log("Tag Deleted Successfully");
                    notify({
                        message: "Tag deleted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.init();

                }, function (err) {
                    GlobalService.ErrorPopup("Error", "Error in Deleting Tag, please try again.");
                    console.log(err);
                });
            }
        });
    }

    $scope.populateTagInEditMode = function (id) {
        var tag = $.grep($scope.TagsTypes, function (record) {
            return record.BrandTypeId == id;
        });

        if (tag.length > 0) {
            $scope.TagTextEdit = tag[0].BrandType;
            $scope.TagNameEdit = tag[0].BrandName;
            $scope.TagIdEdit = tag[0].BrandTypeId;
        }
        angular.element(document.getElementById('editTag')).show();

    };

    $scope.editBrandTag = function () {
        //tagId, text,name, fontColor, bgColor, groupId
        UnifiedService.EditTag($scope.TagIdEdit, $scope.TagTextEdit, $scope.TagNameEdit, $scope.FontColorEdit, $scope.BGColorEdit, $rootScope.selectedGroupId).then(function (l) {
            //obj.IsArchived = false;
            //$scope.TagsTypes.push({ BrandType: $scope.TagText, BrandName: $scope.TagName, BrandTypeId: l.data, FontColor: $scope.FontColor, BGColor: $scope.BGColor });
            //$scope.selectedtag[l.data] = false;
            //console.log("Brand Tag Added Successfully");
            //notify({
            //    message: "Brand Tag Added Successfully",
            //    position: GlobalService.position,
            //    duration: GlobalService.duration
            //});
            $scope.init();
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in Updating Tag, please try again.");
            console.log(err);
        });
    };

    $scope.cancelEditTag = function () {
        angular.element(document.getElementById('editTag')).hide();
        $scope.TagIdEdit = 0;
        $scope.TagTextEdit = "";
        $scope.TagNameEdit = "";
    };

    $scope.deleteKeyword = function (id) {
        GlobalService.ConfirmPopup("Confirm", "Are you sure you want to delete?", function (selectedItem) {
            if (selectedItem) {
                var groupId = $rootScope.selectedGroupId;
                UnifiedService.DeleteKeyword(id, groupId).then(function (l) {
                    console.log("Brand Keyword Deleted Successfully");
                    notify({
                        message: "Brand Keyword deleted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.init();

                }, function (err) {
                    GlobalService.ErrorPopup("Error", "Error in Deleting Keyword, please try again.");
                    console.log(err);
                });
            }
        });
    }

    $scope.openReplyFbPageChat = function (obj) {
        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "Chat", Id: obj.Id };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_FbPageChat.html',
            controller: '_FbPageChatController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {

        }, function () {

        });
    };

    $scope.openDirectMessage = function (obj) {

        $scope.items = { UserId: obj.UserId, IsPage: obj.IsPage, replyedBy: obj.ReplyedBy, StatusId: obj.StatusId, MessageType: obj.MessageType, FormType: "DM", ToUser: obj.ToUser, Id: obj.Id };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterDMReply.html',
            controller: '_TwitterDMReplyController',

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

    $scope.setPattern = function () {
        UnifiedService.GetActivePattern().then(function (l) {
            var response = l.data;
            $scope.ActivePattern = response;
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Inbox data");
        });
    }

    $scope.OpenConsumerPopup = function (item) {
        var UserObj = [];
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
    };
}]);
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

CRMApp.filter("FBPageChat",
    function () {
        return function (value) {
            return ("?threadid=" + value.split('_').pop());
        }
    });