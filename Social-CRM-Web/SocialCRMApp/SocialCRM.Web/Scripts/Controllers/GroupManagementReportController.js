CRMApp.controller('GroupManagementReportController', ['$scope', '$location', 'AuthService', '$window', '$cookies', 'NotificationService', 'ReportService', 'UnifiedService', function ($scope, $location, AuthService, $window, $cookies, NotificationService, ReportService, UnifiedService) {
    
    $scope.dateRangePicker = "";
    $scope.attachedProfiles = [];
    $scope.selected = {};
    $scope.selectProfiles = false;
    $scope.isCustomizedReport = false;

    $scope.GetProfilesByGroupId = function () {        
        UnifiedService.GetAllProfilesForUserGroup($window.localStorage.getItem('selectedGroupId'), $window.localStorage.getItem('selectedCompanyId'), $window.localStorage.getItem('Ispersonal')).then(function (l) {
            $scope.attachedProfiles = [];
            var compositeId = "";
            angular.forEach(l.data, function (obj) {                
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
                    IsPage : obj.IsPage,
                    PageId : obj.PageId,
                    PageName : obj.PageName,
                    PageUrl : obj.PageUrl,
                    UserId : obj.UserId,
                    select: false
                })
            });
            console.log($scope.attachedProfiles);           
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            console.log(err);
        });
    };

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

    $scope.GenerateReport = function () {
        console.log($scope.dateRangePicker);
        var dates = $scope.dateRangePicker.split('-');
        var dateFrom = dates[0].replace(/\s+/, "");
        var dateTo = dates[1].replace(/\s+/, "");

        $scope.loadTwitterProfilePostsAndConversation($window.localStorage.getItem('selectedGroupId'), $window.localStorage.getItem('selectedCompanyId'), dateFrom, dateTo);
    }

    $scope.init();

    /* Audience Engagement */
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
                    day: '%e %b'
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
                data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3,
                       13.9, 9.6, 1.0, 2.4, 4.5, 6.7, 5.2, 4.8, 2.3, 11.2],
                pointStart: Date.UTC(2016, 6, 1),
                pointInterval: 24 * 3600 * 1000 * 7 // one week
            }, {
                name: 'Retweets',
                data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3,
                       6.6, 4.8, 2.4, 4.5, 6.7, 5.2, 1.8, 11.2, 2.4, 4.3],
                pointStart: Date.UTC(2016, 6, 1),
                pointInterval: 24 * 3600 * 1000 * 7 // one week
            }, {
                name: 'Likes',
                data: [0, 0, 0, 0, 3.9, 4.2, 17.0, 16.6, 14.2, 10.3,
                       5.7, 8.5, 11.9, 15.2, 6.6, 4.8, 2.1, 0, 0, 0],
                pointStart: Date.UTC(2016, 6, 1),
                pointInterval: 24 * 3600 * 1000 * 7 // one week
            }, {
                name: 'Tweets Sent',
                data: [0, 10.3, 6.6, 4.8, 3.9, 4.2, 5.7, 8.5, 11.9, 15.2,
                       17.0, 2.1, 16.6, 14.2, 8.4, 16.1, 17.2, 14.5, 11.1, 2.0],
                pointStart: Date.UTC(2016, 6, 1),
                pointInterval: 24 * 3600 * 1000 * 7 // one week
            }
            ]
        });
    });

    /* audienceGrowthGraph */
    $(function () {
        $('#audienceGrowthGraphFacebook').highcharts({
        title: {
            text: 'Fan Growth'
        },
        subtitle:{
            text: 'Likes breakdown by month'
        },
        chart: {
            type: 'area'
        },       
        xAxis: {            
            categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']               
        },
        yAxis: {
            title: {
                text: 'Number of Likes'
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
            name: 'Likes',
            data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6],            
        }, {
            name: 'Unlikes',
            data: [21.5, 25.2, 26.5, 14.5, 5.4, 1.2, 18.3, 13.9, 7.0, 6.9, 9.5, 9.6],            
        }]
    });
    });

    /* Audience Engagement */
    $(function () {
        $('#AudienceEngagementFacebook').highcharts({
            chart: {
                type: 'line'
            },
            title: {
                text: 'Audience Engagement'
            },
            subtitle: {
                text: null
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
                name: 'Reaction',
                data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3,
                       13.9, 9.6, 1.0, 2.4, 4.5, 6.7, 5.2, 4.8, 2.3, 11.2],
                pointStart: Date.UTC(2016, 9, 28),
                pointInterval: 24 * 3600 * 1000 * 1 // day
            }, {
                name: 'Comments',
                data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3,
                       6.6, 4.8, 2.4, 4.5, 6.7, 5.2, 1.8, 11.2, 2.4, 4.3],
                pointStart: Date.UTC(2016, 9, 28),
                pointInterval: 24 * 3600 * 1000 * 1 // one day
            }, {
                name: 'Shares',
                data: [0, 0, 0, 0, 3.9, 4.2, 17.0, 16.6, 14.2, 10.3,
                       5.7, 8.5, 11.9, 15.2, 6.6, 4.8, 2.1, 0, 0, 0],
                pointStart: Date.UTC(2016, 9, 28),
                pointInterval: 24 * 3600 * 1000 * 1 // one day
            }
            ]
        });
    });

}]);