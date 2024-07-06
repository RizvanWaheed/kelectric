CRMApp.controller('InboundReportController',
['$scope', '$location', 'AuthService', '$window', '$cookies', 'NotificationService', 'ReportService', 'TaskService', '$rootScope', '$stateParams', 'UnifiedService', 'GlobalService', '$filter', 'notify', 'CustomerProfileService',
function ($scope, $location, AuthService, $window, $cookies, NotificationService, ReportService, TaskService, $rootScope, $stateParams, UnifiedService, GlobalService, $filter, notify, CustomerProfileService) {
    
    $scope.colorsSentiment = [{ Name: "Positive", Value: "#007AFF" }, { Name: "Neutral", Value: "#FFC000" }, { Name: "Negative", Value: "#C00000" }];
    $scope.colorsInOut = [{ Name: "Inbound", Value: "#3A3B7A" }, { Name: "Outbound", Value: "#F57C4C" }];

    $scope.dateRangePicker = "";

    $scope.CompanyProfiles = [];

    $scope.SentimentsList = [];
    $scope.SentimentsSelected = [];
    $scope.TagsList = [];
    $scope.TagsSelected = [];
    $scope.ChannelsList = [];
    $scope.ChannelsSelected = [];
    $scope.MsgTypesSelected = [];
    $scope.MsgTypes = [{
        icon: "/Content/images/fb_ico16.png",
        label: "Posts",
        id: "1", 
        img: "//:0"
    },
    {
        icon: "/Content/images/fb_ico16.png",
        label: "Comments",
        id: "2",
        img: "//:0"
    },
    {
        icon: "/Content/images/fb_ico16.png",
        label: "Chats",
        id: "3",
        img: "//:0"
    },
    {
        icon: "/Content/images/t_ico16.png",
        label: "Tweets",
        id: "4",
        img: "//:0"
    },
    {
        icon: "/Content/images/t_ico16.png",
        label: "DMs",
        id: "5",
        img: "//:0"
    },
    {
        icon: "/Content/images/insta_ico16.png",
        label: "Posts",
        id: "6",
        img: "//:0"
    },
    {
        icon: "/Content/images/insta_ico16.png",
        label: "Comments",
        id: "7",
        img: "//:0"
    }];

    $scope.customMultiselectBtnText = {
        buttonDefaultText: 'Please Select'
    };
    $scope.customMultiselectExtraSettingsForProfile = {
        scrollableHeight: '300px',
        scrollable: true,
        enableSearch: true,
        template: '<img src="{{option.icon}}" width="15"/> <img src="{{option.img}}" alt="" width="15"/> {{option.label}}'
    };
    $scope.customMultiselectExtraSettings = {
        scrollableHeight: '300px',
        scrollable: true,
        enableSearch: true
    };

    $scope.report = "InOutBound";
    $scope.reportType = "date";
    $scope.isCrisis = false;

    $scope.InboundSum = 'N/A';
    $scope.OutboundSum = 'N/A';

    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';
    $scope.instaIcon = '/Content/images/insta_ico16.png';
    $scope.pintIcon = '/Content/images/pin_ico16.png';

    //set dates and calendar
    $scope.dateRange = {
        startDate: moment().subtract(7, "days"),
        endDate: moment()
    };
    $scope.startDate = '';
    $scope.PrevStartDate = '';
    $scope.endDate = '';
    $scope.PrevEndDate = '';
    $scope.CrisisReportOnTime = $scope.CrisisReportOffTime =  moment();

    $scope.opts = {
        locale: {
            format: 'DD/MM/YYYY'
        },
        "maxDate": $scope.dateRange.endDate,
        "dateLimit": { "years": 2 },
        "eventHandlers": {
            'apply.daterangepicker': function (ev, picker) {
                $scope.filterChanged(null);
            }
        }
    };

    $scope.checkAll = function () {
        angular.forEach($scope.attachedProfiles, function (obj) {
            $scope.selected[obj.CompositeId] = $scope.select;
        });        
    };

    $scope.profileSelectedChanged = function () {
        $scope.select = false;      
    };

    $scope.init = function () {
        $scope.CompanyProfiles = [];
        $scope.SentimentsList = [];
        $scope.SentimentsSelected = [];
        $scope.TagsList = [];
        $scope.TagsSelected = [];
        $scope.ChannelsList = [];
        $scope.MsgTypesSelected = [];
        $scope.ChannelsSelected = [];
        $scope.report = "InOutBound";
        $scope.reportType = "date";
        $scope.InboundSum = 'N/A';
        $scope.OutboundSum = 'N/A';
        $scope.isCrisis = false;
        $scope.totalRow = [];

        $scope.SentimentsList.push({ label: "Positive", id: 1 });
        $scope.SentimentsList.push({ label: "Neutral", id: 2 });
        $scope.SentimentsList.push({ label: "Negative", id: 3 });

        TaskService.GetBrandKeywordByCompanyId($rootScope.selectedCompanyId).then(
            function (res) {
                for(var i = 0; i < res.data.length; i++)
                    $scope.TagsList.push({ label: res.data[i].KeywordText, id: res.data[i].Id });
            },
            function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching tags.");
            }
        );

        UnifiedService.GetAllProfilesForUserGroup($rootScope.selectedGroupId, $rootScope.selectedCompanyId, false).then(
            function (res) {
                for (var i = 0; i < res.data.length; i++) {
                    res.data[i].myId = i;
                    if (res.data[i].ProfileTypeId == 1 || res.data[i].ProfileTypeId == 5){
                        res.data[i].icon = $scope.fbIcon;
                        res.data[i].ProfileName = res.data[i].ProfileName + "(FB)";
                    }
                    if (res.data[i].ProfileTypeId == 3)
                        res.data[i].icon = $scope.linkedinIcon;
                    if (res.data[i].ProfileTypeId == 2)
                        res.data[i].icon = $scope.googleIcon;
                    if (res.data[i].ProfileTypeId == 4) {
                        res.data[i].icon = $scope.twitterIcon;
                        res.data[i].ProfileName = res.data[i].ProfileName + "(TW)";
                    }
                    if (res.data[i].ProfileTypeId == 6) {
                        res.data[i].icon = $scope.instaIcon;
                        res.data[i].ProfileName = res.data[i].ProfileName + "(IN)";
                    }
                    if (res.data[i].ProfileTypeId == 7)
                        res.data[i].icon = $scope.pintIcon;

                    $scope.ChannelsList.push({ label: res.data[i].ProfileName, id: i, icon: res.data[i].icon, img: res.data[i].ProfilePicURL });
                    $scope.CompanyProfiles = res.data;
                }
            },
            function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching User Profiles.");
            }
        );

        //UnifiedService.GetAllProfilesByCompanyId($rootScope.selectedCompanyId).then( //$rootScope.selectedGroupId
        //    function (res) {
        //        for (var i = 0; i < res.data.length; i++) {
        //            res.data[i].myId = i;
        //            if (res.data[i].ProfileTypeId == 1 || res.data[i].ProfileTypeId == 5)
        //                res.data[i].icon = $scope.fbIcon;
        //            if (res.data[i].ProfileTypeId == 3)
        //                res.data[i].icon = $scope.linkedinIcon;
        //            if (res.data[i].ProfileTypeId == 2)
        //                res.data[i].icon = $scope.googleIcon;
        //            if (res.data[i].ProfileTypeId == 4)
        //                res.data[i].icon = $scope.twitterIcon;
        //            if (res.data[i].ProfileTypeId == 6)
        //                res.data[i].icon = $scope.instaIcon;
        //            if (res.data[i].ProfileTypeId == 7)
        //                res.data[i].icon = $scope.pintIcon;

        //            $scope.ChannelsList.push({ label: res.data[i].ProfileName, id: i, icon: res.data[i].icon, img: res.data[i].ProfilePicURL });
        //            $scope.CompanyProfiles = res.data;
        //        }
        //    },
        //    function (err) {
        //        GlobalService.ErrorPopup("Error", "Error in fetching User Profiles.");
        //    }
        //);

        $scope.getInOutBoundReport();
    }

    //$scope.$watch("dateRange", function (newValue, oldValue) {
    //    $scope.filterChanged(null);
    //});

    $scope.events = {
        onSelectionChanged: function (item) {
            $scope.filterChanged(item);
        }
    }

    $scope.filterChanged = function (item) {
        $scope.getInOutBoundReport();
    }

    $scope.getInputs = function () {
        var dateFrom = moment($scope.dateRange.startDate);
        var dateTo = moment($scope.dateRange.endDate);
        var msgs = "";
        if ($scope.MsgTypesSelected.length == 0)
            msgs = "0";
        else
            $scope.MsgTypesSelected.forEach(function (i, idx, array) {
                if (idx === array.length - 1)
                    msgs += i.id + "";
                else
                    msgs += i.id + ",";
            });

        var tags = "";
        if ($scope.TagsSelected.length == 0)
            tags = "0";
        else
            $scope.TagsSelected.forEach(function (i, idx, array) {
                if (idx === array.length - 1)
                    tags += i.id + "";
                else
                    tags += i.id + ",";
            });

        var sentiments = "";
        if ($scope.SentimentsSelected.length == 0)
            sentiments = "0";
        else
            $scope.SentimentsSelected.forEach(function (i, idx, array) {
                if (idx === array.length - 1)
                    sentiments += i.id + "";
                else
                    sentiments += i.id + ",";
            });

        var channels = "";
        var channelsPage = "";
        if ($scope.ChannelsSelected.length == 0) {
            channels = "0";
            channelsPage = "0";
        } else {
            var ctemp = new Array(); //for profiles
            var ptemp = new Array(); //for pages

            $scope.ChannelsSelected.forEach(function (i, idx, array) {
                var obj = $filter('filter')($scope.CompanyProfiles, { myId: i.id }, true);
                if (obj.length>0){
                    if (obj[0].IsPage) //ProfileTypeId == 5
                        ptemp.push(obj[0].ProfileId);
                    else
                        ctemp.push(obj[0].ProfileId);
                }
            });

            if (ptemp.length == 0)
                channelsPage = "0";
            else
                ptemp.forEach(function (i, idx, array) {
                    if (idx === array.length - 1)
                        channelsPage += i + "";
                    else
                        channelsPage += i + ",";
                });

            if (ctemp.length == 0)
                channels = "0";
            else
                ctemp.forEach(function (i, idx, array) {
                    if (idx === array.length - 1)
                        channels += i + "";
                    else
                        channels += i + ",";
                });
        }
        return {
            companyId: $rootScope.selectedCompanyId,
            groupId: $rootScope.selectedGroupId,
            tagIds: tags,
            sentimentIds: sentiments,
            profileIds: channels,
            profilePageIds: channelsPage,
            msgIds: msgs,
            dateFrom: dateFrom.add(5, "hours").toDate(),
            dateTo: dateTo.add(5, "hours").toDate(),
            reportType: $scope.reportType,
            report: $scope.report
        }
    }

    $scope.getInOutBoundReport = function () {
        var data = $scope.getInputs();
        ReportService.GetInOutBoundReport(data).then(
            function (res) {
                $scope.InboundSum = res.data.InOutStats.InboundSum;
                $scope.OutboundSum = res.data.InOutStats.OutboundSum;
                $scope.ReportData = res.data.InOutBoundReport;
                $scope.CrisisReportData = res.data.CrisisReport;
                $scope.SentimentsPerChannel = res.data.SentimentsPerChannel;
                $scope.TagsPerChannel = res.data.TagsPerChannel;
                $scope.channelsReportData = res.data.ChannelReport;

                if($scope.CrisisReportData.length >0)
                {
                    $scope.isCrisis = true;
                    $scope.CrisisReportOnTime = res.data.CrisisReport_Ontime;
                    $scope.CrisisReportOffTime = res.data.CrisisReport_Offtime;
                    $scope.getAreasData();
                    $scope.getTotalRow();
                }

                var temp = new Array();
                temp.push({ name: "Answered", y: res.data.InOutStats.Answered, type: "pie" });
                temp.push({ name: "UnAnswered", y: res.data.InOutStats.UnAnswered, type: "pie" });
                $scope.PieChartConfig = $scope.getPieChartConfig([{ name: "Average Response Per Post", data: temp, type: "pie" }]);

                $scope.getEvolutionGraph();
                $scope.getBarGraph();
            },
            function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching Stats.");
            }
        );
    }

    $scope.getEvolutionGraph = function () {
        var regions = new Array();
        var names = new Array();
        $scope.Crisis_regions = new Array();
        var Crisis_names = new Array();

        if ($scope.reportType == "hour")
            for (var i = 1; i <= 24; i++) {
                regions.push(i);
            }

        if ($scope.reportType == "day")
            regions = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

        if ($scope.reportType == "month")
            regions = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

        if ($scope.isCrisis) {
            $scope.CrisisReportData.forEach(function (i) {
                if ($scope.Crisis_regions.indexOf(i.Hour) == -1) {
                    $scope.Crisis_regions.push(i.Hour);
                }

                if (Crisis_names.indexOf(i.Name) == -1) {
                    Crisis_names.push(i.Name);
                }

                if (i.Count == null)
                    i.Count = 0;
            });
        }

        if ($scope.report == "Channel") {
            $scope.channelsReportData.forEach(function (i) {
                if ($scope.reportType == "date") {
                    var InsertionDate = i.Date.split('T')[0];
                    if (regions.indexOf(InsertionDate) == -1) {
                        regions.push(InsertionDate);
                    }
                }

                if (names.indexOf(i.Name) == -1) {
                    names.push(i.Name);
                }

                if (i.Count == null)
                    i.Count = 0;
            });
        }else{
            $scope.ReportData.forEach(function (i) {
                if ($scope.reportType == "date") {
                    var InsertionDate = i.Date.split('T')[0];
                    if (regions.indexOf(InsertionDate) == -1) {
                        regions.push(InsertionDate);
                    }
                }

                if (names.indexOf(i.Name) == -1) {
                    names.push(i.Name);
                }

                if (i.Count == null)
                    i.Count = 0;
            });
        }

        final_Crisis = [];
        if ($scope.isCrisis) {
           // Crisis_names.forEach(function (name) {
                $scope.Crisis_TW = [];
                $scope.Crisis_FB = [];

                $scope.Crisis_regions.forEach(function (region) {
                    dataSetTW = [];
                    dataSetFB = [];
                
                    dataSetTW = $.grep($scope.CrisisReportData, function (record) {
                        return record.Hour == region && record.Name == "Twitter";  
                    });
                    dataSetFB = $.grep($scope.CrisisReportData, function (record) {
                        return record.Hour == region && record.Name == "Facebook";   
                    });
                   
                    //Twitter
                    if (dataSetTW.length == 0) {
                        $scope.Crisis_TW.push({
                            y: (0)
                        });
                    }
                    else
                    {
                        $scope.Crisis_TW.push({
                            y: (dataSetTW[0].Count)
                        });
                    }

                    //Facebook
                    if (dataSetFB.length == 0)
                    {
                        $scope.Crisis_FB.push({
                            y: (0)
                        });
                    }
                    else
                    {
                        $scope.Crisis_FB.push({
                            y: (dataSetFB[0].Count)
                        });
                    }
                });
                
                var data = {
                    "name": 'Twitter',
                    "data": $scope.Crisis_TW,
                    "type": "line"
                }
                var data2 = {
                    "name": 'Facebook',
                    "data": $scope.Crisis_FB,
                    "type": "line"
                }

                final_Crisis.push(data);
                final_Crisis.push(data2);
         //   });
        }
 
        var c = 0;
        var rnd = [];
        var rnd2 = [];
        var bigrnd = [];
        var regionsMonth = [];
        names.forEach(function (name) {
            rnd = [];
            rnd2 = [];
          
            regions.forEach(function (region) {
                var dataSet = [];
                var dataSetIn = [];
                var dataSetOut = [];

                if ($scope.report == "Channel"){
                    dataSetIn = $.grep($scope.channelsReportData, function (record) {
                        if ($scope.reportType == "day")
                            return record.Name == name && record.Day == region && record.Volume == "Inbound";
                        if ($scope.reportType == "hour")
                            return record.Name == name && record.Hour == (region - 1) && record.Volume == "Inbound";
                        if ($scope.reportType == "date")
                            return record.Name == name && record.Date.split('T')[0] == region && record.Volume == "Inbound";
                    });
                    dataSetOut = $.grep($scope.channelsReportData, function (record) {
                        if ($scope.reportType == "day")
                            return record.Name == name && record.Day == region && record.Volume == "Outbound";
                        if ($scope.reportType == "hour")
                            return record.Name == name && record.Hour == (region - 1) && record.Volume == "Outbound";
                        if ($scope.reportType == "date")
                            return record.Name == name && record.Date.split('T')[0] == region && record.Volume == "Outbound";
                    });
                   
                    //inbound
                    if (dataSetIn.length == 0) {
                        if ($scope.reportType != "hour")
                            rnd.push({
                                y: (0)
                            });
                    }
                    else
                        rnd.push({
                            y: (dataSetIn[0].Count)
                        });

                    //outbound
                    if (dataSetOut.length == 0)
                    {
                        if ($scope.reportType != "hour")
                            rnd2.push({
                                y: (0)
                            });
                    }   
                    else
                        rnd2.push({
                            y: (dataSetOut[0].Count)
                        });
                } else {
                    dataSet = $.grep($scope.ReportData, function (record) {
                        if ($scope.reportType == "day")
                            return record.Name == name && record.Day == region;
                        if ($scope.reportType == "hour")
                            return record.Name == name && record.Hour == (region - 1);
                        if ($scope.reportType == "date")
                            return record.Name == name && record.Date.split('T')[0] == region;
                        if ($scope.reportType == "month")
                            return record.Name == name && regions[record.Month - 1] == region;
                    });

                    if (dataSet.length == 0) {
                        if ($scope.reportType != "hour" && $scope.reportType != "month") // this IF condition is added on July 10, 2017
                            rnd.push({
                                y: (0)
                            });
                    }
                    else {
                        if ($scope.reportType == "month")
                            regionsMonth.push(region);
                        rnd.push({
                            y: (dataSet[0].Count)
                        });
                    }
                }
            });

            if ($scope.report == "Channel") {
                var nameArr = name.split('|');
                if (nameArr[0] == "P")
                    name = $filter('filter')($scope.CompanyProfiles, { ProfileId: nameArr[1], IsPage: false })[0].ProfileName;
                else if (nameArr[0] == "PP")
                    name = $filter('filter')($scope.CompanyProfiles, { ProfileId: nameArr[1], IsPage: true })[0].ProfileName;
            }

            if ($scope.report == "Tag")
                name = $filter('filter')($scope.TagsList, { label: name })[0].label;

            if ($scope.report == "Channel") {
                var data = {
                    "name": name + ' - Inbound',
                    "data": rnd,
                    "type": "line"
                }
                var data2 = {
                    "name": name + ' - Outbound',
                    "data": rnd2,
                    "type": "line"
                }
                bigrnd.push(data);
                bigrnd.push(data2);
            } else if ($scope.report == "Sentiment") {
                var color = $scope.colorsSentiment.filter(function (el) {
                    return el.Name == name
                })[0].Value;

                var data = {
                    "name": name,
                    "data": rnd,
                    "type": "line",
                    "color": color
                }
                bigrnd.push(data);
            }
            else if ($scope.report == "Tag") {
                var data = {
                    "name": name,
                    "data": rnd,
                    "type": "line"
                }
                bigrnd.push(data);
            }
            else {
                var color = $scope.colorsInOut.filter(function (el) {
                    return el.Name == name
                })[0].Value;

                var data = {
                    "name": name,
                    "data": rnd,
                    "type": $scope.reportType == "month" ? "bar" : "line",
                    "color": color
                }
                bigrnd.push(data);
            }
        });

        //if($scope.report=="Channel")
        //    names.forEach(function (name, i) {
        //        names[i] = $filter('filter')($scope.CompanyProfiles, { ProfileId: name })[0].ProfileName;
        //    });

        //if ($scope.report == "Tag")
        //    names.forEach(function (name, i) {
        //        names[i] = $filter('filter')($scope.TagsList, { id: name })[0].label;
        //    });
        if ($scope.reportType == "month")
            regions = regionsMonth;

        $scope.ChartConfig = $scope.getHighChartConfig(regions, bigrnd, true, false);
        $scope.CrisisChartConfig = $scope.getHighChartConfig($scope.Crisis_regions, final_Crisis, true, true);
    }

    $scope.getBarGraph = function () {
        var channelArr = new Array();
        var negatives = new Array();
        var neutrals = new Array();
        var positives = new Array();

        for (var i = 0; i < $scope.SentimentsPerChannel.length; i++) {
            channelArr.push($scope.SentimentsPerChannel[i].Profile.ProfileName);
            negatives.push($scope.SentimentsPerChannel[i].Negative);
            neutrals.push($scope.SentimentsPerChannel[i].Neutral);
            positives.push($scope.SentimentsPerChannel[i].Positive);
        }

        var outboundSeries = [];

        if ($scope.SentimentsSelected.length > 0) {
            if ($filter('filter')($scope.SentimentsSelected, { id: 1 }, true).length > 0) //po
                outboundSeries.push({
                    name: 'Positive',
                    data: positives,
                    type: "column",
                    color: $scope.colorsSentiment[0].Value
                });
            if ($filter('filter')($scope.SentimentsSelected, { id: 2 }, true).length > 0) //neu
                outboundSeries.push({
                    name: 'Neutral',
                    data: neutrals,
                    type: "column",
                    color: $scope.colorsSentiment[1].Value
                });
            if ($filter('filter')($scope.SentimentsSelected, { id: 3 }, true).length > 0) //neg
                outboundSeries.push({
                    name: 'Negative',
                    data: negatives,
                    type: "column",
                    color: $scope.colorsSentiment[2].Value
                });
        } else {
            outboundSeries.push({
                name: 'Positive',
                data: positives,
                type: "column",
                color: $scope.colorsSentiment[0].Value
            });
            outboundSeries.push({
                name: 'Neutral',
                data: neutrals,
                type: "column",
                color: $scope.colorsSentiment[1].Value
            });
            outboundSeries.push({
                name: 'Negative',
                data: negatives,
                type: "column",
                color: $scope.colorsSentiment[2].Value
            });
        }

        

        $scope.SentimentsPerChannelChartConfig = $scope.getBarChartConfig(channelArr, outboundSeries, "Sentiments Per Channels", "Sentiment Volume", false);

        channelArr = new Array();
        var tagSeries = new Array();
        var tags = new Array();

        for (var i = 0; i < $scope.TagsPerChannel.length; i++) {
            channelArr.push($scope.TagsPerChannel[i].Profile.ProfileName);
        }

        for (var k = 0; k < $scope.TagsPerChannel[0].Tags.length; k++) {
            tagSeries.push({ name: $scope.TagsPerChannel[0].Tags[k], data: [], type: "column", index: k });
        }


        for (var j = 0; j < tagSeries.length; j++) {
            var data = new Array();
            for (var k = 0; k < channelArr.length; k++) {
                data.push($scope.TagsPerChannel[k].TagCounts[j]);
            }
            tagSeries[j].data = data;
        }

        $scope.TagsPerChannelChartConfig = $scope.getBarChartConfig(channelArr, tagSeries, "Tags Per Channels", "Tags Volume", true);
    }

    $scope.getHighChartConfig = function (categories, series, isArea, isCrisis) {
        var title = "";
        var stacking = true;
        var stackLabelEnable = false;
        if (isArea) {
            stacking = false;
            stackLabelEnable = true;
            title = isCrisis ? "Crisis Report" : $scope.report == "InOutBound" ? "Inbound / Outbound Report" : $scope.report;
        }
        else {
            if ($scope.selectedRegionReport.Id == 2) {
                stackLabelEnable = false;
                stacking = true;
            }
            else {
                stacking = false;
                stackLabelEnable = true;
            }
            title = $scope.selectedRegionReport.Name;
        }

        return {
            options: {
                xAxis: {
                    categories: categories
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Volume of ' + title
                    },
                    allowDecimals: false,
                    stackLabels: {
                        enabled: true,
                        style: {
                            fontWeight: 'bold',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                        }
                    }
                },
                plotOptions: {
                    column: {
                        stacking: stacking,
                        dataLabels: {
                            enabled: stackLabelEnable,
                            style: {
                                fontWeight: 'bold',
                                color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                            }
                        }
                    }
                }
            },
            legend: {
                align: 'right',
                x: -70,
                verticalAlign: 'top',
                y: 20,
                floating: true,
                backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                shadow: false
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
            },
            series: series,
            title: {
                text: title

            },
            loading: false,
            "credits": {
                "enabled": false
            }
        };
    }

    $scope.getPieChartConfig = function (series) {
        var title = "Average Response Per Post";

        return {
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 0
                }
            },
            options: {
                plotOptions: {
                    pie: {
                        innerSize: 50,
                        depth: 100,
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: "{percentage:.1f}%"
                        },
                        showInLegend: true
                    }
                }
            },
            series: series,
            title: {
                text: title
            },
            loading: false,
            "credits": {
                "enabled": false
            }
        };
    }

    $scope.getBarChartConfig = function (categories, series, title, ytitle, enableStack) {
        return {
            options: {
                xAxis: {
                    categories: categories
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: ytitle
                    },
                    allowDecimals: false,
                    stackLabels: {
                        enabled: true,
                        style: {
                            fontWeight: 'bold',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                        }
                    }
                },
                plotOptions: {
                    column: {
                        stacking: enableStack,
                        dataLabels: {
                            enabled: true,
                            color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                            format: '{point.y}',
                            style: {
                                textShadow: '0 0 3px black, 0 0 3px black'
                            }
                        }
                    }
                }
            },
            title: {
                text: title
            },
            legend: {
                align: 'right',
                x: -70,
                verticalAlign: 'top',
                y: 20,
                floating: true,
                backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                shadow: false
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
            },
            series: series,
            "credits": {
                "enabled": false
            }
        };
    }

    $scope.updateCrisisData = function () {
    if($scope.isCrisis)
        message = "Are you sure you want to turn off Crisis Reporting?"
    else
        message = "Are you sure you want to turn on Crisis Reporting?"

    GlobalService.ConfirmPopup("Confirm", message, function (selectedItem) {
        if (selectedItem) {
            ReportService.UpdateCrisisData($scope.isCrisis).then(
                function (res) {
                    if (res != undefined) {
                        if (res.data.success) {
                            notify({
                                message: res.data.msg,
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });
                            $scope.init();
                        }
                        else
                            GlobalService.ErrorPopup("Error", res.data.msg);
                    }
                },
                function (err) {
                    GlobalService.ErrorPopup("Error", "Error in saving state.");
                }
            );
        }
    });
    }

    $scope.getAreasData = function () {
        CustomerProfileService.GetRegionReport($rootScope.selectedGroupId, "All", $scope.CrisisReportOnTime, $scope.CrisisReportOffTime, 1, 0, 0, false, true)
        .then(function (res) {
            var areas = $filter('orderBy')(res.data, "Total_Inbound", true);
            if (areas.length > 0) {
                $scope.top3Areas = "";
                var length = areas.length > 3 ? 3 : areas.length;
                for (var i = 0; i < length; i++) {
                    if (i == length - 1 && i > 0) {
                        $scope.top3Areas += "& " + areas[i].Region;
                    } else if (i == length - 1) {
                        $scope.top3Areas += areas[i].Region;
                    } else if (i == length - 2) {
                        $scope.top3Areas += areas[i].Region + " ";
                    } else {
                        $scope.top3Areas += areas[i].Region + ", ";
                    }
                }
            }
            else {
                $scope.top3Areas = "None";
            }
        }, function (err) {
            GlobalService.ErrorPopup('Error in fetching, please try again.');
        });
    }

    $scope.emailCrisisReport = function () {
        var svg = $scope.CrisisChartConfig.getHighcharts().container.innerHTML;
        svg = "data:image/svg+xml," + svg;
        var myElement = document.createElement('img');

        myElement.src = svg;
        myElement.width = 657;
        myElement.height = 340;

        myElement.onload = function () {
            var canvas = document.createElement('canvas');
            var ctx = canvas.getContext('2d');
            canvas.width = myElement.width;
            canvas.height = myElement.height;

            ctx.drawImage(myElement, 0, 0, canvas.width, canvas.height);
            var crisisReportImage = canvas.toDataURL('image/png');

            var data = {
                reportOnTime: $scope.CrisisReportOnTime,
                top3Areas: $scope.top3Areas,
                companyId: $rootScope.selectedCompanyId,
                crisisReportImage: crisisReportImage,
                crisisTWInboundData: $scope.Crisis_TW,
                crisisFBInboundData: $scope.Crisis_FB,
                crisisTotalInboundData: $scope.totalRow
            };

            ReportService.EmailCrisisReport(data).then(
                function (res) {
                    if (res != undefined) {
                        if (res.data.success)
                            GlobalService.ErrorPopup("Success", res.data.msg);
                        else
                            GlobalService.ErrorPopup("Error", res.data.msg);
                    }
                },
                function (err) {
                    GlobalService.ErrorPopup("Error", "Error in fetching Data.");
                }
            );
        };
    }

    $scope.totalRow = []
    $scope.getTotalRow = function () {
        var cnt = 0;
        var totalHours = -1;

        $scope.CrisisReportData.forEach(function (record) {
            if (record.Hour == totalHours) {
                cnt = cnt + record.Count;
            }
            else {
                if (totalHours != -1) {
                    $scope.totalRow.push(
                        {
                            y: cnt,
                            hour: totalHours
                        });
                }
                cnt = record.Count;
            }

            totalHours = record.Hour;
        });

        //adding the last record
        $scope.totalRow.push(
                      {
                          y: cnt,
                          hour: totalHours
                      });
    };

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.init();
}]);
