CRMApp.controller('FacebookReportController', ['$rootScope', '$scope', '$stateParams', '$location', 'AuthService', '$window', '$cookies', 'NotificationService', 'ReportService', 'UnifiedService', 'FacebookService', 'GlobalService', function ($rootScope, $scope, $stateParams, $location, AuthService, $window, $cookies, NotificationService, ReportService, UnifiedService, FacebookService, GlobalService) {
    
    $scope.Math = window.Math;
    $scope.dateRangePicker = "";
    $scope.showProfiles = false;
    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';
    $scope.selectDeselectAll = false;
    $scope.currentProfile = {};
    $scope.MaleCount = 0;
    $scope.FemaleCount = 0;

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
    
    var minDateAllowed = moment().subtract(2, "years");
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
        "dateLimit": { "days": 92 }, //facebook api limitation
        "minDate": minDateAllowed,
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

    setDifferenceText = function (diffInDays) {
        if (diffInDays > 0 && diffInDays < 7) {
            $scope.ReferenceTimePeriod = "Since last " + diffInDays + "  day(s)";
        } else if (diffInDays >= 7 && diffInDays < 30) {
            $scope.ReferenceTimePeriod = "Since last " + Math.floor(diffInDays / 7) + "  week(s)";
        } else if (diffInDays >= 30) {
            $scope.ReferenceTimePeriod = "Since last " + Math.floor(diffInDays / 30) + "  month(s)";
        }
    }

    $scope.findPerIncDec = function (val1, val2) {
        if (val2 == 0)
            return Math.abs(val1 / 100);
        else
            return Math.abs((val1 - val2) / val2);
    };

    $scope.GetProfilesByGroupId = function () {
        UnifiedService.GetAllProfilesForUserGroup($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {
            $scope.attachedProfiles = [];
            var compositeId = "";
            angular.forEach(l.data, function (obj) {
                if (obj.ProfileTypeId == 1 && obj.IsPage == true) {
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
                    })
                }                
            });
            console.log($scope.attachedProfiles);
            $scope.currentProfile = $scope.attachedProfiles[0];
            $scope.generateReport();
            //$scope.checkAllPages();
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            console.log(err);
        });
    };

    $scope.init = function () {
        $scope.GetProfilesByGroupId();
        //$scope.generateReport();
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

    $scope.generateReport = function () {
        var _startDate = $scope.date.startDate.format('YYYY-MM-DD HH:mm:ss');
        var _endDate = $scope.date.endDate.format('YYYY-MM-DD HH:mm:ss');

        //Call Facebook service
        var startDate = new Date(_startDate);
        var endDate = new Date(_endDate);
        var profileId = $scope.currentProfile.UserId;
        //d2.setDate(d1.getDate() - 5);

        //Diff between 2 dates to find time span
        var diffInDays = Math.ceil((endDate - startDate) / (1000 * 60 * 60 * 24));
        //alert(diffInDays);
        setDifferenceText(diffInDays);

        //Graph variables
        var PageLikeDatewise = [];
        var PageUnlikeDatewise = [];
        var PageLikeTotal = 0;
        var PageUnlikeTotal = 0;

        var months = [];
        var strMonths = [];
        var PublishPagePostsSentCount = [];
        var PublishPageBehaviour = {};

        var PublishUserPostsSentCount = [];
        var PublishUserBehaviour = {};
        var PagePostsTotal = 0;
        var UsersPostsTotal = 0;

        var PostCommentDatewise = [];
        var PostLikeDatewise = [];
        var PostShareDatewise = [];
        var PostCommentTotal = 0;
        var PostLikeTotal = 0;
        var PostShareTotal = 0;

        var TotalPeopleWhoViewedDatewise = [];
        var pageReactionLike = [];
        var pageReactionLove = [];
        var pageReactionWow = [];
        var pageReactionHaha = [];
        var pageReactionSorry = [];
        var pageReactionAnger = [];
        var TotalPageReachDatewise = [];

        var externalLinksData = [];
        
        FacebookService.FacebookGetPageReportStats(profileId, startDate.toISOString(), endDate.toISOString()).then(function (l) {
            if(l.statusText == "OK"){
                $scope.reportData = l.data;
                console.log($scope.reportData);

                //Graph Population
                /* audienceGrowthGraph */
                angular.forEach($scope.reportData.PageLikeSpan, function (item) {
                    PageLikeDatewise.push(item.ActivityCount);
                    PageLikeTotal = PageLikeTotal + item.ActivityCount;
                });
                angular.forEach($scope.reportData.PageUnlikeSpan, function (item) {
                    PageUnlikeDatewise.push(item.ActivityCount);
                    PageUnlikeTotal = PageUnlikeTotal + item.ActivityCount;
                });
                $scope.PageLikeTotal = PageLikeTotal;
                $scope.PageUnlikeTotal = PageUnlikeTotal;

                $(function () {
                    $('#audienceGrowthGraph').highcharts({
                        title: {
                            text: null
                        },
                        subtitle: {
                            text: 'Likes breakdown by day'
                        },
                        chart: {
                            type: 'area'
                        },
                        xAxis: {
                            //categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                day: '%e. %b',
                                week: '%e. %b',
                                month: '%b \'%y',
                            }
                        },
                        yAxis: {
                            title: {
                                text: 'Number of Likes/ Unlikes'
                            },
                            minRange: 1,
                            allowDecimals: false,
                            labels: {
                                formatter: function () {
                                    return this.value;
                                    //return this.value / 1000 + 'k';
                                }
                            }
                        },
                        tooltip: {
                            pointFormat: '{series.name}: <b>{point.y:,.0f}</b>'
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
                            name: 'Likes',
                            data: PageLikeDatewise,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }, {
                            name: 'Unlikes',
                            data: PageUnlikeDatewise,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }],
                        credits: {
                            enabled: false
                        }
                    });
                });

                /* Publishing Behavior */
                var PublishedPageBehaviour = [];
                var PublishedUserBehaviour = [];
                var newMonthItem = {"Month": "", "Count":0 }
                var tempDate = new Date();
                angular.forEach($scope.reportData.PublishedPagePostSpan, function (item) {
                    var strDate = item.DateValue.match(/\d/g);
                    strDate = strDate.join("");
                    tempDate = new Date(parseInt(strDate));

                    if (months.indexOf(tempDate.getMonth()) === -1) {
                        months.push(tempDate.getMonth());
                    }
                });
                console.log(months);
                angular.forEach(months, function (month) {
                    newMonthItem.Month = month;
                    newMonthItem.Count = 0;
                    strMonths.push(GetMonthString(month));
                    angular.forEach($scope.reportData.PublishedPagePostSpan, function (item) {
                        var strDate2 = item.DateValue.match(/\d/g);
                        strDate2 = strDate2.join("");
                        tempDate = new Date(parseInt(strDate2));
                        if (tempDate.getMonth() == month && item.ActivityCount > 0) {
                            newMonthItem.Count++;
                            PagePostsTotal = PagePostsTotal + newMonthItem.Count;
                        }
                    });
                    PublishPagePostsSentCount.push(newMonthItem.Count);
                    PublishedPageBehaviour.push(newMonthItem);

                    newMonthItem.Count = 0;
                    angular.forEach($scope.reportData.PublishedUserPostSpan, function (item) {
                        var strDate2 = item.DateValue.match(/\d/g);
                        strDate2 = strDate2.join("");
                        tempDate = new Date(parseInt(strDate2));
                        if (tempDate.getMonth() == month && item.ActivityCount>0) {
                            newMonthItem.Count++;
                            UsersPostsTotal = UsersPostsTotal + newMonthItem.Count++;
                        }
                    });
                    PublishUserPostsSentCount.push(newMonthItem.Count);
                    PublishedUserBehaviour.push(newMonthItem);
                });
                $scope.PagePostsTotal = PagePostsTotal;
                $scope.UsersPostsTotal = UsersPostsTotal;
                console.log(PublishedPageBehaviour);

                $(function () {
                    $('#PublishingBehavior').highcharts({
                        chart: {
                            type: 'column',
                            options3d: {
                                enabled: true,
                                alpha: 10,
                                beta: 25,
                                depth: 70
                            }
                        },
                        title: {
                            text: null
                        },
                        subtitle: {
                            text: null
                        },
                        plotOptions: {
                            column: {
                                stacking: 'normal',
                                depth: 25
                            }
                        },
                        xAxis: {
                            categories: strMonths
                        },
                        yAxis: {
                            title: {
                                text: null
                            },
                            labels: {
                                formatter: function () {
                                    return this.value;
                                    //return this.value / 1000 + 'k';
                                }
                            }
                        },
                        series: [{
                            name: 'Page Posts',
                            data: PublishPagePostsSentCount,
                            stack: 'Page'
                        },{
                            name: 'Users Posts',
                            data: PublishUserPostsSentCount,
                            stack: 'User'
                        }],
                        colors: ['#3b5998', '#7b868a'],
                        credits: {
                            enabled: false
                        }
                    });
                });

                /* Audience Engagement */
                //Comment
                angular.forEach($scope.reportData.PostCommentSpan, function (item) {
                    PostCommentDatewise.push(item.ActivityCount);
                    PostCommentTotal = PostCommentTotal + item.ActivityCount;
                });

                //Like
                angular.forEach($scope.reportData.PostLikeSpan, function (item) {
                    PostLikeDatewise.push(item.ActivityCount);
                    PostLikeTotal = PostLikeTotal + item.ActivityCount;
                });

                //Shares
                angular.forEach($scope.reportData.PostShareSpan, function (item) {
                    PostShareDatewise.push(item.ActivityCount);
                    PostShareTotal = PostShareTotal + item.ActivityCount;
                });
                
                $scope.PostCommentTotal = PostCommentTotal;
                $scope.PostLikeTotal = PostLikeTotal;
                $scope.PostShareTotal = PostShareTotal;

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
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                day: '%e. %b',
                                week: '%e. %b',
                                month: '%b \'%y',
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
                            name: 'Likes',
                            data: PostLikeDatewise,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }, {
                            name: 'Comments',
                            data: PostCommentDatewise,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }, {
                            name: 'Shares',
                            data: PostShareDatewise,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }
                        ],
                        credits: {
                            enabled: false
                        }
                    });
                });

                //Total People who viewed
                angular.forEach($scope.reportData.TotalPeopleWhoViewedSpan, function (item) {
                    TotalPeopleWhoViewedDatewise.push(item.ActivityCount);                    
                });

                $(function () {
                    $('#TotalPeopleWhoViewed').highcharts({
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
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                day: '%e. %b',
                                week: '%e. %b',
                                month: '%b \'%y',
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
                            name: 'Total People Who Viewed',
                            data: TotalPeopleWhoViewedDatewise,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }],
                        credits: {
                            enabled: false
                        }
                    });
                });

                //Reactions
                angular.forEach($scope.reportData.pageReactionsSpan, function (item) {
                    pageReactionLike.push(item.like);
                    pageReactionLove.push(item.love);
                    pageReactionWow.push(item.wow);
                    pageReactionHaha.push(item.haha);
                    pageReactionSorry.push(item.sorry);
                    pageReactionAnger.push(item.anger);
                });

                //Reactions Graph
                $(function () {
                    $('#ReactionsGraph').highcharts({
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
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                day: '%e. %b',
                                week: '%e. %b',
                                month: '%b \'%y',
                            }
                        },
                        yAxis: {
                            title: {
                                text: null
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
                            column: {
                                stacking: 'normal'                                
                            }
                        },
                        series: [{
                            name: 'Like',
                            data: pageReactionLike,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        },
                        {
                            name: 'Love',
                            data: pageReactionLove,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        },
                        {
                            name: 'Wow',
                            data: pageReactionWow,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        },
                        {
                            name: 'Haha',
                            data: pageReactionHaha,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        },
                        {
                            name: 'Sorry',
                            data: pageReactionSorry,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        },
                        {
                            name: 'Anger',
                            data: pageReactionAnger,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }
                        ],
                        credits: {
                            enabled: false
                        }
                    });
                });

                //Page Reachability                
                angular.forEach($scope.reportData.pageReachSpan, function (item) {
                    TotalPageReachDatewise.push(item.ActivityCount);
                });

                //Page Reachability Graph
                $(function () {
                    $('#PageReachability').highcharts({
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
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                day: '%e. %b',
                                week: '%e. %b',
                                month: '%b \'%y',
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
                            name: 'Page Reach',
                            data: TotalPageReachDatewise,
                            pointStart: Date.UTC(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()),
                            pointInterval: 24 * 3600 * 1000 * 1 // one day
                        }],
                        credits: {
                            enabled: false
                        }
                    });
                });               

                //Gender
                angular.forEach($scope.reportData.PageFansGenderAge, function (item) {
                    $scope.MaleCount = $scope.MaleCount + item.MaleCount;
                    $scope.FemaleCount = $scope.FemaleCount + item.FemaleCount;
                });

                //External Links
                angular.forEach($scope.reportData.lstExternalReferrals, function (item) {
                    externalLinksData.push({'name':item.link, 'y':item.count});
                });

                //External Links Graph                
                $('#ExternalLinks').highcharts({
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        type: 'pie'
                    },
                    title: {
                        text: null
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
                    series: [{
                        name: 'Views',
                        colorByPoint: true,
                        data: externalLinksData
                        //data: [{
                        //    name: 'Microsoft Internet Explorer',
                        //    y: 56.33
                        //}, {
                        //    name: 'Chrome',
                        //    y: 24.03,
                        //    sliced: true,
                        //    selected: true
                        //}, {
                        //    name: 'Firefox',
                        //    y: 10.38
                        //}, {
                        //    name: 'Safari',
                        //    y: 4.77
                        //}, {
                        //    name: 'Opera',
                        //    y: 0.91
                        //}, {
                        //    name: 'Proprietary or Undetectable',
                        //    y: 0.2
                        //}]
                    }],
                    credits: {
                            enabled: false
                    }
                });

            }
            console.log(l);

        }, function (err) {
            //alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            console.log(err);
        });        
    }

    $scope.assignProfile = function (Profile) {
        $scope.currentProfile = Profile;
        $scope.generateReport();
    }

    //$scope.checkAllPages = function () {
    //    $scope.selectDeselectAll = !$scope.selectDeselectAll
    //    angular.forEach($scope.attachedProfiles, function (obj) {
    //        obj.select = !$scope.selectDeselectAll;
    //    });
    //}

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

    GetMonthString = function (month) {
        var strMonth = "";
        switch (month) {
            case 0:
                strMonth = "Jan";
                break;
            case 1:
                strMonth = "Feb";
                break;
            case 2:
                strMonth = "Mar";
                break;
            case 3:
                strMonth = "Apr";
                break;
            case 4:
                strMonth = "May";
                break;
            case 5:
                strMonth = "Jun";
                break;
            case 6:
                strMonth = "Jul";
                break;
            case 7:
                strMonth = "Aug";
                break;
            case 8:
                strMonth = "Sep";
                break;
            case 9:
                strMonth = "Oct";
                break;
            case 10:
                strMonth = "Nov";
                break;
            case 11:
                strMonth = "Dec";
        }
        return strMonth;
    }
    
    $scope.loadMoreRecentPosts = function () {
        var recentPostId = $scope.reportData.lastPostId;
        var _startDate = $scope.date.startDate.format('YYYY-MM-DD HH:mm:ss');
        var _endDate = $scope.date.endDate.format('YYYY-MM-DD HH:mm:ss');

        //Call Facebook service
        var startDate = new Date(_startDate);
        var endDate = new Date(_endDate);

        FacebookService.FacebookGetMoreRecentPosts($scope.currentProfile.UserId, recentPostId, startDate.toISOString(), endDate.toISOString()).then(function (l) {
            if (l.statusText == "OK" && l.data != null) {
                //Adding more posts
                angular.forEach(l.data.lstRecentPosts, function (item) {
                    $scope.reportData.lstRecentPosts.push(item);
                });

                //Changing load more value
                $scope.reportData.lastPostId = l.data.lastPostId;

                //Test code
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            console.log(err);
        });
    }
    
}]);