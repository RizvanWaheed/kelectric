CRMApp.controller('TeamReportController', ['$rootScope', '$scope', '$location', '$stateParams', '$uibModal', 'ReportService', '$window', '$cookies', 'NotificationService', 'GlobalService', 'notify', 'TeamService', function ($rootScope, $scope, $location, $stateParams, $uibModal, ReportService, $window, $cookies, NotificationService, GlobalService, notify, TeamService) {
    $scope.init = function () {
        $scope.TeamMembers = [];
        $scope.TeamMemberModel = [];
        $scope.example5customTexts = { buttonDefaultText: 'Select Agents' };
        $scope.BusinessName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.GetTeamData();
        $scope.GetTeamMembers();
      //  $scope.IsAdmin = false;
      //  $scope.TeamMemberId = 0;
        $scope.ActivityParams = [{ name: "TotalFBPosts", value: "FB Posts" },
                                { name: "TotalFBComments", value: "FB Comments" },
                                { name: "TotalFBNestComments", value: "FB Nested Comments" },
                                { name: "TotalFBPageChats", value: "FB Chats" },
                                { name: "TotalTwitTweets", value: "Tweets" },
                                { name: "TotalTwitReplies", value: "Replies" },
                                { name: "TotalTwitDMs", value: "DMs" },
                                { name: "TotalInstaComments", value: "Insta Comments" },
                                { name: "TotalDelFBPosts", value: "Deleted FB Posts" },
                                { name: "TotalDelFBComments", value: "Deleted FB Comments" },
                                { name: "TotalDelFBNestComments", value: "Deleted FB Nested Comments" },
                                { name: "TotalDelTwitTweets", value: "Deleted Tweets" },
                                { name: "TotalDelTwitDMs", value: "Deleted DMs" },
                                { name: "TotalDelInstaComments", value: "Deleted Insta Comments" },
                                { name: "TotalMarkCompleted", value: "Marked Completed Without Reply" },
                                { name: "TotalBannedUsers", value: "IDs Banned" },
                                { name: "TotalWarnedUsers", value: "IDs Warned" },
                                { name: "TotalTagsAdded", value: "Tags Assigned" },
                                { name: "TotalSentimentsAdded", value: "Sentiments Assigned" }];
    }

    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        var boom = label.replace('~', '');

        return boom;
    };

    $scope.teamEventListeners = {
        onSelectionChanged: onItemSelectMember
    };

    function onItemSelectMember(property) {
        $scope.GetTeamData();
    }

    $scope.multiSelectSettings = {
        keyboardControls: true,
        enableSearch: true,
        smartButtonMaxItems: 3,
        smartButtonTextConverter: function (itemText, originalItem) {
            return itemText;
        }
    };

    $scope.dateFrom = "";
    $scope.dateTo = "";
    $scope.dateDifference = "";

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
    $scope.TeamReport = new Array();
    $scope.TeamAndProfileReport = new Array();

    //date items
    $scope.date = {
        startDate: moment().subtract(15, "days").add(5, "hours"),
        endDate: moment().add(5, "hours")
    };

    $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY HH:mm:ss');
    $scope.PrevStartDate = $scope.startDate;
    $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY HH:mm:ss');
    $scope.PrevEndDate = $scope.endDate;

    $scope.opts = {
        locale: {
            applyClass: 'btn-green',
            applyLabel: "Apply",
            fromLabel: "From",
            format: "MMMM D, YYYY",
            toLabel: "To",
            cancelLabel: 'Cancel',
            customRangeLabel: 'Custom range'
        },
        "dateLimit": { "years": 5 },
        "maxDate": $scope.date.endDate,
        eventHandlers: {
            'apply.daterangepicker': function (ev, picker) {
                $scope.startDate = moment($scope.date.startDate).add(5, "hours").format('DD/MM/YYYY HH:mm:ss');
                $scope.endDate = moment($scope.date.endDate).add(5, "hours").format('DD/MM/YYYY HH:mm:ss');
                if ($scope.startDate == $scope.PrevStartDate && $scope.endDate == $scope.PrevEndDate) {
                    //alert('not changed')
                }
                else {
                    $scope.PrevStartDate = $scope.startDate;
                    $scope.PrevEndDate = $scope.endDate;
                    $scope.dateFilterEnum = 5;
                    // call function which will fetch data
                    //get report data for default
                    $scope.GetTeamData();
                    //end get data
                }
            }
        ,
            'cancel.daterangepicker': function (ev, picker) {
                $scope.startDate = moment($scope.date.startDate).add(5, "hours").format('DD/MM/YYYY HH:mm:ss');
                $scope.endDate = moment($scope.date.endDate).add(5, "hours").format('DD/MM/YYYY HH:mm:ss');
                if ($scope.startDate == $scope.PrevStartDate && $scope.endDate == $scope.PrevEndDate) {
                    //alert('not changed')
                }
                else {
                    $scope.PrevStartDate = $scope.startDate;
                    $scope.PrevEndDate = $scope.endDate;
                    $scope.dateFilterEnum = 5;
                    // call function which will fetch data
                    //get report data for default
                    $scope.GetTeamData();
                    //end get data
                }
            }
        ,
            'hide.daterangepicker': function (ev, picker) {
                $scope.startDate = moment($scope.date.startDate).add(5, "hours").format('DD/MM/YYYY HH:mm:ss');
                $scope.endDate = moment($scope.date.endDate).add(5, "hours").format('DD/MM/YYYY HH:mm:ss');
                if ($scope.startDate == $scope.PrevStartDate && $scope.endDate == $scope.PrevEndDate) {
                    //alert('not changed')
                }
                else {
                    $scope.PrevStartDate = $scope.startDate;
                    $scope.PrevEndDate = $scope.endDate;
                    $scope.dateFilterEnum = 5;
                    // call function which will fetch data
                    //get report data for default
                    $scope.GetTeamData();
                    //end get data
                }
            }
        }
    };

    $scope.dateFilterEnum = 5;

    $scope.filterActivityName = function (name) {
        return  $scope.ActivityParams.filter(function (el) {
                    return el.name == name;
                })[0].value;
    }

    $scope.dateFilter = function (enumId) {
        var days = 0;
        if (enumId == 1)
            days = 15;
        else if (enumId == 2)
            days = 30;
        else if (enumId == 3)
            days = 60;
        else if (enumId == 4)
            days = 90;

        $scope.date = {
            startDate: moment().subtract(days, "days").add(5, "hours"),
            endDate: moment().add(5, "hours")
        };

        $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY HH:mm:ss');
        $scope.PrevStartDate = $scope.startDate;
        $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY HH:mm:ss');
        $scope.PrevEndDate = $scope.endDate;


        $scope.dateFilterEnum = enumId;
        //get report data for default
        $scope.GetTeamData();
        //end get data
    }

    $scope.GetTeamMembers = function () {
        TeamService.GetByCompanyId($rootScope.selectedCompanyId).then(function (l) {
            var response = l.data;
            response.Team.forEach(function (i) {
               // if ($scope.IsAdmin == true || i.TeamMemberId == $scope.TeamMemberId)
                    $scope.TeamMembers.push({ label: i.Name, id: i.TeamMemberId });
            });
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    }

    $scope.GetTeamData = function () {
        var grpId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var dateFrom = $scope.startDate;
        var dateTo = $scope.endDate;
        var teamIds = "";

        $scope.IsAdmin = $rootScope.IsAdmin;
        $scope.TeamMemberId = jQuery.parseJSON($rootScope.currentUser).TeamMemberId;

        $scope.TeamMemberModel.forEach(function (i) {
            teamIds += i.id + ","
        });
        if (teamIds.length > 0)
            teamIds = teamIds.substring(0, teamIds.length - 1);
        else
            teamIds = "0";

        var initial = dateFrom.split(/\//);
        dateFrom = [initial[1], initial[0], initial[2]].join('/');

        initial = dateTo.split(/\//);
        dateTo = [initial[1], initial[0], initial[2]].join('/');
        var date1 = new Date(dateFrom);
        var date2 = new Date(dateTo);
        var timeDiff = Math.abs(date2.getTime() - date1.getTime());
        $scope.dateDifference = Math.ceil(timeDiff / (1000 * 3600 * 24));

        ReportService.GetTeamReport(grpId, companyId, date1, date2, $scope.dateFilterEnum, teamIds, $scope.IsAdmin, $scope.TeamMemberId).then(function (l) {
            $scope.TeamReport = l.data.ReplyByTeamMember;
            //$scope.PublishedAndTeamReport = l.data.PublishByTeamMember;

            $scope.TeamAndProfileReport = l.data.ReplyByProfileAndTeamMemberNew;
            $scope.TotalTeamStats = l.data.TeamStats;
            $scope.TeamStatsEvolution = l.data.TeamStatsEvolution;

            // Comment & chat Chart
            var profiles = new Array();
            var comment = new Array();
            var chat = new Array();
            var posts = new Array();
            var firstShiftAgents = new Array();
            var secondShiftAgents = new Array();
            var thirdShiftAgents = new Array();
            console.clear();
            $scope.TeamAndProfileReport.forEach(function (i) {
                if ($scope.IsAdmin == true || i.ProfileId == $scope.TeamMemberId) {
                    profiles.push(i.ProfileName);
                    comment.push(i.CommentCount);
                    chat.push(i.ChatCount);
                    posts.push(i.TotalPosts);

                    if (i.FirstShiftOutbound != 0)
                        firstShiftAgents.push({ name: i.ProfileName, y: i.FirstShiftOutbound });
                    if (i.SecondShiftOutbound != 0)
                        secondShiftAgents.push({ name: i.ProfileName, y: i.SecondShiftOutbound });
                    if (i.ThirdShiftOutbound != 0)
                        thirdShiftAgents.push({ name: i.ProfileName, y: i.ThirdShiftOutbound });
                }
                console.log(i.ThirdShiftOutbound);
            });
            var outboundSeries = [
            //{
            //    name: 'Posts',
            //    data: posts,
            //    type: "column"
            //},
            {
                name: 'Replies / Comments',
                data: comment,
                type: "column"
            }, {
                name: 'Chats / Direct Msgs',
                data: chat,
                type: "column"
            }];
            $scope.TeamOutboundChartConfig = $scope.CreateAxisChartConfig(profiles, outboundSeries, '', 'Outbound Volume');

            var agentData = [];
            $.each($scope.TotalTeamStats, function (i, n) {
                if (n != 0 && i != "ActivityDate")
                    agentData.push({ name: $scope.filterActivityName(i), y: n });
            });
            var agentSeries = [{
                name: "Activity Volume",
                colorByPoint: true,
                data: agentData
            }];
            // Build the chart
            $('#TeamActivitieBreakdownChartConfig').highcharts($scope.CreatePieChartConfig(agentSeries, 'AGENT ACTIVITIES BREAKDOWN'));

            var agent1ShiftSeries = [{
                name: "Activity Volume",
                colorByPoint: true,
                data: firstShiftAgents
            }];
            // Build the chart
            $('#TeamOutboundFirstShiftConfig').highcharts($scope.CreatePieChartConfig(agent1ShiftSeries, 'SHIFT 1 FROM [8 AM to 4 PM]'));

            var agent2ShiftSeries = [{
                name: "Activity Volume",
                colorByPoint: true,
                data: secondShiftAgents
            }];
            // Build the chart
            $('#TeamOutboundSecondShiftConfig').highcharts($scope.CreatePieChartConfig(agent2ShiftSeries, 'SHIFT 2 FROM [4 PM to 12 AM]'));

            var agent3ShiftSeries = [{
                name: "Activity Volume",
                colorByPoint: true,
                data: thirdShiftAgents
            }];
            // Build the chart
            $('#TeamOutboundThirdShiftConfig').highcharts($scope.CreatePieChartConfig(agent3ShiftSeries, 'SHIFT 3 FROM [12 AM to 8 AM]'));

            var regions = new Array();
            var names = new Array();
            var iter = 0;
            $scope.TeamStatsEvolution.forEach(function (obj) {
                var ActivityDate = obj.ActivityDate.split('T')[0];
                if (regions.indexOf(ActivityDate) == -1) {
                    regions.push(ActivityDate);
                }

                if (iter == 0) {
                    $.each(obj, function (i, n) {
                        if (i != "ActivityDate")
                            names.push(new Array());
                    });
                }
                var iter2 = 0;
                $.each(obj, function (i, n) {
                    if (i != "ActivityDate") {
                        names[iter2].push({ y: n });
                        iter2++;
                    }
                });
                iter++;
            });

            var bigrnd = [];
            iter = 0;
            $.each($scope.TotalTeamStats, function (i, n) {
                if (i != "ActivityDate") {
                    bigrnd.push({ "name": $scope.filterActivityName(i), "data": names[iter], "type": "line" });
                    iter++;
                }
            });
            $scope.TeamActivitiesEvolutionChartConfig = $scope.CreateAxisChartConfig(regions, bigrnd, 'AGENT ACTIVITIES EVOLUTION', 'Activity Volume');
            // Media wise Chart
            /*var rnd = []
            var bigrnd = [];
            for (var i = 0; i < 3; i++) {
                rnd = [];
                if (i == 0) {
                    $scope.TeamReport.forEach(function (i) {
                        var data = $.grep($scope.TeamAndProfileReportChart, function (record) {
                            return (record.ProfileType == 'FB_PROFILE' || record.ProfileType == 'FB_PAGE') && record.TeamId == i.ProfileId;
                        });
                        var replies = 0;
                        data.forEach(function (i) {
                            replies += parseInt(i.CommentCount);
                            replies += parseInt(i.ChatCount);
                        });
                        rnd.push({
                            y: (replies)
                        })
                    });

                    var data = {
                        "name": 'Facebook ',
                        "data": rnd,
                        "type": "column"
                    }
                    bigrnd.push(data);
                }
                if (i == 1) {
                    $scope.TeamReport.forEach(function (i) {
                        var data = $.grep($scope.TeamAndProfileReportChart, function (record) {
                            return (record.ProfileType == 'TW') && record.TeamId == i.ProfileId;
                        });
                        var replies = 0;
                        data.forEach(function (i) {
                            replies += parseInt(i.CommentCount);
                            replies += parseInt(i.ChatCount);
                        });
                        rnd.push({
                            y: (replies)
                        })
                    });

                    var data = {
                        "name": 'Twitter',
                        "data": rnd,
                        "type": "column"
                    }
                    bigrnd.push(data);
                }
                if (i == 2) {
                    $scope.TeamReport.forEach(function (i) {
                        var data = $.grep($scope.TeamAndProfileReportChart, function (record) {
                            return (record.ProfileType == 'IC') && record.TeamId == i.ProfileId;
                        });
                        var replies = 0;
                        data.forEach(function (i) {
                            replies += parseInt(i.CommentCount);
                            replies += parseInt(i.ChatCount);
                        });
                        rnd.push({
                            y: (replies)
                        })
                    });

                    var data = {
                        "name": 'Instagram',
                        "data": rnd,
                        "type": "column"
                    }
                    bigrnd.push(data);
                }
            }            
            $scope.TeamAndProfileMediaWiseChartConfig = {
                options: {
                    chart: {
                        type: 'areaspline'
                    },
                    xAxis: {
                        categories: profiles
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Number of Replies'
                        },
                        stackLabels: {
                            enabled: true,
                            style: {
                                fontWeight: 'bold',
                                color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                            }
                        }
                    },
                    plotOptions: {
                        "series": {
                            "stacking": ""
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
                    formatter: function () {
                        return '<b>' + this.x + '</b><br/>' +
                            this.series.name + ': ' + this.y + '<br/>' +
                            'Total: ' + this.point.stackTotal;
                    }
                },
                plotOptions: {
                    column: {
                        stacking: 'normal',
                        dataLabels: {
                            enabled: true,
                            format: '{point.y}',
                            color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                            style: {
                                textShadow: '0 0 3px black, 0 0 3px black'
                            }
                        }
                    }
                },
                series: bigrnd,
                title: {
                    text: 'Social Profile Wise Replies'

                },
                loading: false,
                "credits": {
                    "enabled": false
                }
            };
            //

            // Publisher Chart

            var publisherData = [];
            $scope.PublishedAndTeamReport.forEach(function (i) {
                var data = {
                    name: i.ProfileName,
                    y: i.TotalPosts
                }
                publisherData.push(data);
            });
            $scope.PublisherChartConfig = {
                options: {
                    chart: {
                        type: 'column'
                    },
                    xAxis: {
                        categories: profiles,
                        type: 'category'
                    },
                    yAxis: {
                        title: {
                            text: 'No. of posts by team'
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
                            stacking: "",
                            dataLabels: {
                                enabled: true,
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
                    formatter: function () {
                        return '<b>' + this.x + '</b><br/>' +
                            this.series.name + ': ' + this.y + '<br/>' +
                            'Total: ' + this.point.stackTotal;
                    }
                },
                series: [{
                    name: 'Posts',
                    colorByPoint: true,
                    data: publisherData
                }],
                title: {
                    text: ''
                },
                loading: false,
                "credits": {
                    "enabled": false
                }
            };*/
            //


        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };
    $scope.init();

    $scope.CreatePieChartConfig = function (series, title) {
        return {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: title
            },
            tooltip: {
                pointFormat: '<b>{point.percentage:.1f}%</b><br/>{series.name}: <b>{point.y}</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true
                    },
                    showInLegend: true
                }
            },
            series: series,
            "credits": {
                "enabled": false
            }
        };
    }

    $scope.CreateAxisChartConfig = function (categories, series, title, ytitle) {
        return {
            options:{
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
                        stacking: true,
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

    $scope.PerformanceLevel = function (value) {
        if (value >= 80)
            return "progress-bar-green";
        else if (value < 80 && value >= 60)
            return "progress-bar-aqua";
        else if (value < 60 && value >= 40)
            return "progress-bar-yellow";
        else if (value < 40 && value >= 6)
            return "progress-bar-light-blue";
        else if (value < 6 && value >= 0)
            return "progress-bar-red";
    }

    $scope.OpenReplies = function (TeamId, ProfileId, ProfileType, ActivityType) {
        var dateFrom = $scope.startDate;
        var dateTo = $scope.endDate;
        ReportService.GetCommentsOrChat(TeamId, ProfileId, ProfileType, ActivityType, dateFrom, dateTo, $scope.dateFilterEnum).then(function (l) {
            var response = l.data;
            $scope.items = { ThreadList: response };
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/Scripts/Views/Shared/_TeamReportThread.html',
                controller: '_TeamReportThreadController',
                resolve: {
                    items: function () {
                        return $scope.items;
                    }
                }
            });

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    $scope.OpenQuickOrSlowReply = function (TeamId, ProfileId, ProfileType, ReplyType) {
        var dateFrom = $scope.startDate;
        var dateTo = $scope.endDate;
        ReportService.GetQuickOrSlowReply(TeamId, ProfileId, ProfileType, ReplyType, dateFrom, dateTo, $scope.dateFilterEnum).then(function (l) {
            var response = new Array();
            response.push(l.data);
            $scope.items = { ThreadList: response };
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/Scripts/Views/Shared/_TeamReportThread.html',
                controller: '_TeamReportThreadController',
                resolve: {
                    items: function () {
                        return $scope.items;
                    }
                }
            });

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

}]);

CRMApp.filter('positive', function () {
    return function (input) {
        if (!input) {
            return 0;
        }
        return Math.abs(input);
    };
})