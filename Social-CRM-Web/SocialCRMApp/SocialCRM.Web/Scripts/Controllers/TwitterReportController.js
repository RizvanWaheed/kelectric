CRMApp.controller('TwitterReportController', ['$scope', '$location', 'AuthService', '$window', '$cookies', 'NotificationService', 'ReportService', 'UnifiedService', '$rootScope', '$stateParams', 'TwitterService', 'GlobalService', function ($scope, $location, AuthService, $window, $cookies, NotificationService, ReportService, UnifiedService, $rootScope, $stateParams, TwitterService, GlobalService) {
    
    $scope.dateRangePicker = "";    
    $scope.selectedProfiles = [];
    $scope.showProfiles = false;

    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png'

    $scope.selected = {};
    $scope.select = true;

    $scope.filter_1 = true;
    $scope.filter_2 = false;
    $scope.filter_3 = false;
    $scope.filter_4 = false;

    $scope.ReferenceTimePeriod = '';

    //set dates and calendar
    $scope.date = {
        startDate: moment().subtract(15, "days"),
        endDate: moment()
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

                $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY HH:mm:ss');
                $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY HH:mm:ss');
                if ($scope.startDate == $scope.PrevStartDate && $scope.endDate == $scope.PrevEndDate) {
                    //alert('not changed')
                }
                else {
                    $scope.PrevStartDate = $scope.startDate;
                    $scope.PrevEndDate = $scope.endDate;
                    $scope.dateFilterEnum = 5;
                    // call function which will fetch data
                    //get report data for default
                    $scope.generateReport();
                    //end get data                
                }
            }
        }
    };

    $scope.findPerIncDec = function (val1, val2) {
        if (val2 == 0)
            return Math.abs(val1 / 100);
        else
            return Math.abs((val1 - val2) / val2);
    };

    $scope.checkAll = function () {
        angular.forEach($scope.attachedProfiles, function (obj) {
            $scope.selected[obj.CompositeId] = $scope.select;
        });
        $scope.generateReport();
    };

    $scope.profileSelectedChanged = function () {
        $scope.select = false;
        $scope.generateReport();
    };

    $scope.GetProfilesByGroupId = function () {
        UnifiedService.GetAllProfilesForUserGroup($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {
            $scope.attachedProfiles = [];
            var compositeId = "";
            angular.forEach(l.data, function (obj) {
                if(obj.ProfileTypeId == 4){
                compositeId = obj.IsPage + "_" + obj.ProfileCompnayGroupId;
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
                }                
            });
            console.log($scope.attachedProfiles);
            $scope.checkAll();
            $scope.generateReport();
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            console.log(err);
        });
    };

    $scope.downloadPDF = function () {
        //Call for download pdf (Server Call)
        var reportData = {"Text":"Hello World!"};
        ReportService.CreateReportPDF('TwitterReport', reportData).then(function (l) {
            
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in creating pdf, please try again.");
            console.log(err);
        });
    }

    $scope.init = function () {
        $scope.GetProfilesByGroupId();
    }    

    $scope.loadTwitterProfilePostsAndConversation = function (groupId, companyId, dateFrom, dateTo) {
        var sendDateFrom = new Date(dateFrom);
        var sendDateTo = new Date(dateTo);

        ReportService.GetTwitterProfilePostsAndConversation(groupId, companyId, sendDateFrom, sendDateTo).then(function (resultSet) {
            if (resultSet != null) {
                console.log(resultSet);
                if (resultSet["statusText"] == "OK") {                   
                }
            }
            //alert("defaultfully fetched");
        }, function (err) {
            //alert('Error in fetching, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching please try again.");
            console.log(err);
        });

    }    

    //Group Change
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.select = true;
        $scope.init();
    });

    $scope.init();

    $scope.dateFilter = function (enumId) {
        var days = 0;
        if (enumId == 1) {
            days = 15;
            $scope.filter_1 = true;
            $scope.filter_2 = false;
            $scope.filter_3 = false;
            $scope.filter_4 = false;
        }
        else if (enumId == 2) {
            days = 30;
            $scope.filter_1 = false;
            $scope.filter_2 = true;
            $scope.filter_3 = false;
            $scope.filter_4 = false;
        }
        else if (enumId == 3) {
            days = 60;
            $scope.filter_1 = false;
            $scope.filter_2 = false;
            $scope.filter_3 = true;
            $scope.filter_4 = false;
        }
        else if (enumId == 4) {
            days = 90;
            $scope.filter_1 = false;
            $scope.filter_2 = false;
            $scope.filter_3 = false;
            $scope.filter_4 = true;
        }

        $scope.date = {
            startDate: moment().subtract(days, "days"),
            endDate: moment()
        };

        $scope.startDate = $scope.date.startDate;
        $scope.PrevStartDate = $scope.startDate;
        $scope.endDate = $scope.date.endDate;
        $scope.PrevEndDate = $scope.endDate;

        $scope.generateReport();
        $scope.dateFilterEnum = enumId;

        ////get report data for default
        //$scope.getReportData();
        ////end get data
    }

    /* audienceGrowthGraph */
    $(function () {
        $('#audienceGrowthGraph').highcharts({
        title: {
            text: null
            },
        chart: {
            type: 'area'
        },       
        xAxis: {            
        //categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']           
            type: 'datetime',
            dateTimeLabelFormats: {
                day: '%e %b'
            }   
        },
        yAxis: {
            title: {
                text: 'Number of Followers'
            },
            labels: {
                formatter: function () {
                    return this.value / 1000 + 'k';
                }
            }
        },
        tooltip: {
            pointFormat: '{series.name} Number of Followers: <b>{point.y:,.0f}</b><br/> in {point.x}'
        },
        plotOptions: {
            area: {                
                marker: {
                    enabled: false,
                    symbol: 'circle',
                    radius: 2,
                    states: {
                        hover: {
                            enabled: true
                        }
                    }
                }
            }
        },
        series: [{
            name: 'New Followers',
            data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6],
            pointStart: Date.UTC(2016, 8, 1),
            pointInterval: 24 * 3600 * 1000 * 7 // one week
        }, {
            name: 'Actual Followers',
            data: [21.5, 25.2, 26.5, 14.5, 5.4, 1.2, 18.3, 13.9, 7.0, 6.9, 9.5, 9.6],
            pointStart: Date.UTC(2016, 8, 1),
            pointInterval: 24 * 3600 * 1000 * 7 // one week
        }],
        credits: {
            enabled: false
        }
    });
    });        

    setDifferenceText = function (diffInDays) {
        if (diffInDays > 0 && diffInDays < 7) {
            $scope.ReferenceTimePeriod = "Since last " + diffInDays + "  day(s)";
        } else if (diffInDays >= 7 && diffInDays < 30) {
            $scope.ReferenceTimePeriod = "Since last " + Math.floor(diffInDays / 7) + "  week(s)";
        } else if (diffInDays >= 30) {
            $scope.ReferenceTimePeriod = "Since last " + Math.floor(diffInDays / 30) + "  month(s)";
        }
    }

    //Generate Report
    $scope.generateReport = function () {
        var _startDate = moment($scope.date.startDate).add(5, "hours").format('YYYY-MM-DD HH:mm:ss');
        var _endDate = moment($scope.date.endDate).add(5, "hours").format('YYYY-MM-DD HH:mm:ss');
        //Call Facebook service
        var startDate = new Date(_startDate);
        var endDate = new Date(_endDate);
        
        //Diff between 2 dates to find time span
        var diffInDays = Math.ceil((endDate - startDate) / (1000 * 60 * 60 * 24));
        //alert(diffInDays);
        setDifferenceText(diffInDays);

        //Data variables
        var TweetsSentSpan = [];
        var DMsSentSpan = [];
        var MentionsReceivedSpan = [];
        var DMsReceivedSpan = [];
        var EngRepliesSpan = [];
        var EngRetweetsSpan = [];
        var EngTweetsSpan = [];
        
        //Selected Profiles
        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        var selectedProfilesArray = [];

        angular.forEach(selectedProfiles, function (profile) {
            selectedProfilesArray.push(profile.UserId);
        });

        TwitterService.TwitterGetPageReportStats(selectedProfilesArray, startDate.toISOString(), endDate.toISOString()).then(function (l) {
            if (l.statusText == "OK") {
                $scope.reportData = l.data;

                //Graph Plotting
                /* PostsAndConversation */
                angular.forEach($scope.reportData.TweetsSentSpan, function (item) {
                    TweetsSentSpan.push(item.ActivityCount);
                });
                angular.forEach($scope.reportData.DMsSentSpan, function (item) {
                    DMsSentSpan.push(item.ActivityCount);
                });
                angular.forEach($scope.reportData.MentionsReceivedSpan, function (item) {
                    MentionsReceivedSpan.push(item.ActivityCount);
                });
                angular.forEach($scope.reportData.DMsReceivedSpan, function (item) {
                    DMsReceivedSpan.push(item.ActivityCount);
                });
                
                $(function () {
                    $('#PostsAndConversation').highcharts({
                        chart: {
                            type: 'line'
                        },
                        title: {
                            text: null
                        },
                        subtitle: {
                            text: null
                        },
                        xAxis: {
                            //categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                day: '%e. %b',
                                week: '%e. %b',
                                month: '%b \'%y'
                            }
                        },
                        yAxis: {
                            title: {
                                text: 'Count'
                            },
                            labels: {
                                formatter: function () {
                                    return this.value / 1000 + 'k';
                                }
                            }
                        },
                        plotOptions: {
                            line: {
                                dataLabels: {
                                    enabled: false
                                },
                                enableMouseTracking: true
                            }
                        },
                        series: [{
                            name: 'Tweets Sent',
                            data: TweetsSentSpan,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }, {
                            name: 'DMs Sent',
                            data: DMsSentSpan,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }, {
                            name: 'Tweets Received',
                            data: MentionsReceivedSpan,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }, {
                            name: 'DMs Received',
                            data: DMsReceivedSpan,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }],
                        credits: {
                            enabled: false
                        }
                    });
                });

                /* Content Engagemen & Habits */
                var VideoTweetsCount = $scope.reportData.VideoTweetsCount;
                var BasicTweetsCount = $scope.reportData.BasicTweetsCount;
                var PhotoTweetsCount = $scope.reportData.PhotoTweetsCount;
                var ShareTweetsCount = $scope.reportData.ShareTweetsCount;

                var tweetsSentTotal = VideoTweetsCount + BasicTweetsCount + PhotoTweetsCount;

                $(function () {
                    $(document).ready(function () {                        
                        // Build the chart
                        $('#contentEngagementAndHabbits1').highcharts({
                            chart: {
                                type: 'pie',
                                options3d: {
                                    enabled: true,
                                    alpha: 45
                                }
                            },
                            title: {
                                text: 'Tweets Sent'
                            },
                            subtitle: {
                                text: null
                            },
                            tooltip: {
                                pointFormat: '<b>{point.percentage:.1f}%</b><br/>{series.name}: <b>{point.y}</b>'
                            },
                            plotOptions: {
                                pie: {
                                    innerSize: 100,
                                    depth: 45,
                                    allowPointSelect: true,
                                    cursor: 'pointer',
                                    dataLabels: {
                                        enabled: true,
                                        format: "{percentage:.1f}%"
                                    },
                                    showInLegend: true
                                }
                            },
                            series: [{
                                name: 'Content Type',
                                colorByPoint: true,
                                data: [{
                                    name: 'Plain Text',
                                    y: BasicTweetsCount,
                                    sliced: true,
                                    selected: true
                                }, {
                                    name: 'Videos',
                                    y: VideoTweetsCount
                                }, {
                                    name: 'Photos',
                                    y: PhotoTweetsCount
                                }, {
                                    name: 'Links',
                                    y: ShareTweetsCount
                                }]
                            }],
                            credits: {
                                enabled: false
                            }
                        });
                    });
                });

                /* Content Engagemen & Habits 2 */
                var TotalProfileTweets =$scope.reportData.TotalProfileTweets;
                var TotalProfileDMs = $scope.reportData.TotalProfileDMs;
                var profileTotal = TotalProfileTweets + TotalProfileDMs;
                $(function () {
                    $(document).ready(function () {

                        // Build the chart
                        $('#contentEngagementAndHabbits2').highcharts({
                            chart: {
                                type: 'pie',
                                options3d: {
                                    enabled: true,
                                    alpha: 45
                                }
                            },
                            title: {
                                text: 'Your Tweeting Behaviour'
                            },
                            subtitle: {
                                text: null
                            },
                            tooltip: {
                                pointFormat: '<b>{point.percentage:.1f}%</b><br/>{series.name}: <b>{point.y}</b>'
                            },
                            plotOptions: {
                                pie: {
                                    innerSize: 100,
                                    depth: 45,
                                    allowPointSelect: true,
                                    cursor: 'pointer',
                                    dataLabels: {
                                        enabled: true,
                                        format: "{percentage:.1f}%"
                                    },
                                    showInLegend: true
                                }
                            },
                            series: [{
                                name: 'Behaviour Type',
                                colorByPoint: true,
                                data: [{
                                    name: 'Tweets',
                                    y: TotalProfileDMs
                                }, {
                                    name: 'DMs',
                                    y: TotalProfileTweets,
                                    sliced: true,
                                    selected: true
                                }]
                            }],
                            colors: ['#1da1f2', '#7b868a'],
                            credits: {
                                enabled: false
                            }
                        });
                    });
                });

                /* Audience Engagement */
                angular.forEach($scope.reportData.EngRepliesSpan, function (item) {
                    EngRepliesSpan.push(item.ActivityCount);
                });
                angular.forEach($scope.reportData.EngRetweetsSpan, function (item) {
                    EngRetweetsSpan.push(item.ActivityCount);
                });
                angular.forEach($scope.reportData.EngTweetsSpan, function (item) {
                    EngTweetsSpan.push(item.ActivityCount);
                });
                $(function () {
                    $('#AudienceEngagement').highcharts({
                        chart: {
                            type: 'line'
                        },
                        title: {
                            text: null
                        },
                        subtitle: {
                            text: null
                        },
                        xAxis: {
                            //categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                day: '%e. %b',
                                week: '%e. %b',
                                month: '%b \'%y'
                            }
                        },
                        yAxis: {
                            title: {
                                text: null
                            }
                        },
                        plotOptions: {
                            line: {
                                dataLabels: {
                                    enabled: false
                                },
                                enableMouseTracking: true
                            }
                        },
                        series: [{
                            name: 'Replies',
                            data: EngRepliesSpan,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }, {
                            name: 'Retweets',
                            data: EngRetweetsSpan,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        },  {
                            name: 'Tweets Sent',
                            data: EngTweetsSpan,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }],
                        credits: {
                            enabled: false
                        }
                    });
                });

            }
            console.log(l);

        }, function (err) {
            //alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            console.log(err);
        });        
    }

}]);