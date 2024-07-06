CRMApp.controller('InstagramController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'InstagramService', 'ProfileService', 'GlobalService', function ($rootScope, $scope, $window, $location, $stateParams, InstagramService, ProfileService, GlobalService) {
    // alert(('#ctrlanalyticdatepicker').val())

    //get instagram report data
    $scope.getReportData = function () {
        var grpId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        InstagramService.GetInstagramReports($scope.dateFilterEnum, $scope.startDate, $scope.endDate, $scope.selectedProfile.Id, grpId, companyId).then(function (l) {
            if (l.data != null) {
                $scope.reportShow = true;
                $scope.TopPostGraph = l.data.TopPostGraph;
                $scope.GeneralStatics = l.data.GeneralStatics;

                //Start Audience Growth graph

                //$scope.audienceGrowthChartConfig = { //dummy data
                //    options: {
                //        chart: {
                //            type: 'area'
                //        }
                //    },
                //    series: [{
                //        data: [10, 15, 12, 8, 7]
                //    }],
                //    title: {
                //        text: 'Audience Growth'
                //    },
                //    loading: false
                //}

                $scope.data = l.data.AudienceGrowthData;
                var days = [];
                for (var i = 0; i < $scope.data.length; i++) {
                    days.push($scope.data[i].Date);
                }

                var followers = [];
                for (var i = 0; i < $scope.data.length; i++) {
                    followers.push($scope.data[i].NumOfFollowers);
                }

                $scope.chartSeries = [
                  { "name": "Days", "data": followers }
                ];

                $scope.audienceGrowthChartConfig = {
                    options: {
                        chart: {
                            type: 'area'
                        },
                        yAxis: {
                            title: {
                                text: 'Followers'
                            }
                        }
                    },
                    series: $scope.chartSeries,
                    xAxis: {
                        categories: days
                    },
                    title: {
                        text: 'Audience Growth'
                    },
                    loading: false
                };



                $scope.TotalFollowers = $scope.GeneralStatics.TotalFollowers;
                $scope.TotalFollows = $scope.GeneralStatics.TotalFollows;


                $scope.FollowersGained = $scope.GeneralStatics.FollowersGained;
                var followerIncreasedVal = $scope.GeneralStatics.FollowersIncreased;
                var val = parseInt(followerIncreasedVal);
                $scope.IncreaseInFollowers = val > 0;
                $scope.IncreaseInFollowersVal = Math.abs(followerIncreasedVal) + " %";

                //End Audience Growth graph

                //publish behaviour graph
                var rnd = []
                $scope.PublisingBehaviourGraph = l.data.PublisingBehaviourGraph;
                for (var i = 0; i < $scope.PublisingBehaviourGraph.length; i++) {
                    rnd.push({
                        name: 'PublishedDate: ' + ($scope.PublisingBehaviourGraph[i].PublishedDate),
                        y: ($scope.PublisingBehaviourGraph[i].MediaSent)
                    })
                }
                $scope.publishingBehaviourChartConfig = {
                    options: {
                        chart: {
                            type: 'pie'
                        }
                    },
                    series: [{
                        "name": "Media Sent",
                        "data": rnd
                    }
                    ],
                    title: {
                        text: 'Total Media Published: ' + $scope.PublisingBehaviourGraph.length > 0 ? $scope.PublisingBehaviourGraph[0].TotalMedia : 0

                    },

                    loading: false,
                    "credits": {
                        "enabled": false
                    }
                }

                $scope.TotalPosts = $scope.GeneralStatics.TotalPosts;
                $scope.TotalPhotos = $scope.GeneralStatics.TotalPhotos;
                $scope.TotalVideos = $scope.GeneralStatics.TotalVideos;

                var mediaIncreasedVal = $scope.GeneralStatics.MediaIncreased;
                var val = parseInt(mediaIncreasedVal);
                $scope.IncreaseInMedia = val > 0;
                $scope.IncreaseInMediaVal = Math.abs(mediaIncreasedVal) + " %";

                //publish behaviour graph End

                //OUTBOUND HASHTAG PERFORMANCE
                $scope.MostUsedHashTag = new Array();
                $scope.MostUsedHashTag = l.data.MostUsedHashTags;
                $scope.MostEngagedHashTags = l.data.MostEngagedHashTags;
                // End OUTBOUND HASHTAG PERFORMANCE


                //Aurdience Engagement graph
                rnd = []
                bigrnd = []
                $scope.AudienceEngagementGraph = l.data.AudienceEngagementGraph;
                var days = [];
                for (var i = 0; i < $scope.AudienceEngagementGraph.length; i++) {
                    days.push($scope.AudienceEngagementGraph[i].PublishedDate);
                }
                for (var i = 0; i < 3; i++) {
                    rnd = [];
                    if (i == 0) {
                        for (var j = 0; j < $scope.AudienceEngagementGraph.length; j++) {
                            rnd.push({
                                name: 'PublishedDate: ' + $scope.AudienceEngagementGraph[j].PublishedDate,
                                y: ($scope.AudienceEngagementGraph[j].LikesReceived)
                            })
                        }
                        var data = {
                            "name": 'Likes Received ',
                            "data": rnd,
                            "type": "column"
                        }
                        bigrnd.push(data);
                    }
                    if (i == 1) {
                        for (var j = 0; j < $scope.AudienceEngagementGraph.length; j++) {
                            rnd.push({
                                name: 'PublishedDate: ' + $scope.AudienceEngagementGraph[j].PublishedDate,
                                y: ($scope.AudienceEngagementGraph[j].CommentReceived)
                            })
                        }
                        var data = {
                            "name": 'Comments Received ',
                            "data": rnd,
                            "type": "column"
                        }
                        bigrnd.push(data);
                    }
                    if (i == 2) {
                        for (var j = 0; j < $scope.AudienceEngagementGraph.length; j++) {
                            rnd.push({
                                name: 'PublishedDate: ' + $scope.AudienceEngagementGraph[j].PublishedDate,
                                y: ($scope.AudienceEngagementGraph[j].MediaSent)
                            })
                        }
                        var data = {
                            "name": 'Media Sent ',
                            "data": rnd,
                            "type": "column"
                        }
                        bigrnd.push(data);
                    }
                }
                $scope.audienceEngagementChartConfig = {
                    options: {
                        chart: {
                            type: 'areaspline'
                        },
                        yAxis: {
                            title: {
                                text: 'Volume'
                            }
                        },
                        plotOptions: {
                            "series": {
                                "stacking": ""
                            }
                        }
                    },
                    series: bigrnd,
                    xAxis: {
                        categories: days
                    },
                    title: {
                        text: 'Likes, Comments and Media Sent '

                    },

                    loading: false,
                    "credits": {
                        "enabled": false
                    }
                }
                $scope.TotalLikesReceived = $scope.GeneralStatics.TotalLikesReceived;
                $scope.TotalCommentReceived = $scope.GeneralStatics.TotalCommentReceived;
                $scope.TotalEngagements = $scope.TotalLikesReceived + $scope.TotalCommentReceived;
                $scope.EngagementsPerFollower = $scope.GeneralStatics.EngagementsPerFollower;
                $scope.EngagementsPerMedia = $scope.GeneralStatics.EngagementsPerMedia;

                var engagementIncreasedVal = $scope.GeneralStatics.EngagementIncreased;
                var val = parseInt(engagementIncreasedVal);
                $scope.IncreaseInEngagement = val > 0;
                $scope.IncreaseInEngagementVal = Math.abs(engagementIncreasedVal) + " %";

                var engagementPerMediaIncreasedVal = $scope.GeneralStatics.EngagementPerMediaIncreased;
                var val = parseInt(engagementPerMediaIncreasedVal);
                $scope.IncreaseInEngagementPerMedia = val > 0;
                $scope.IncreaseInEngagementPerMediaVal = Math.abs(engagementPerMediaIncreasedVal) + " %";

                //End Audience engagement graph


                //Profile Stats
                $scope.InstagramProfilesStats = l.data.InstagramProfilesStats;
                //end Profile Stats
            }
            else {
                // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            }
        }, function (err) {
            $scope.reportShow = false;
            // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            console.log(err);
        });

    }

    //set dates and calendar
    $scope.date = {
        startDate: moment().subtract(15, "days"),
        endDate: moment()
    };

    $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY');
    $scope.PrevStartDate = $scope.startDate;
    $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY');
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
                $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY');
                $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY');
                if ($scope.startDate == $scope.PrevStartDate && $scope.endDate == $scope.PrevEndDate) {
                    //alert('not changed')
                }
                else {
                    $scope.PrevStartDate = $scope.startDate;
                    $scope.PrevEndDate = $scope.endDate;
                    $scope.dateFilterEnum = 5;
                    // call function which will fetch data
                    //get report data for default
                    $scope.getReportData();
                    //end get data
                }
            }
        ,
            'cancel.daterangepicker': function (ev, picker) {
                $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY');
                $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY');
                if ($scope.startDate == $scope.PrevStartDate && $scope.endDate == $scope.PrevEndDate) {
                    //alert('not changed')
                }
                else {
                    $scope.PrevStartDate = $scope.startDate;
                    $scope.PrevEndDate = $scope.endDate;
                    $scope.dateFilterEnum = 5;
                    // call function which will fetch data
                    //get report data for default
                    $scope.getReportData();
                    //end get data
                }
            }
        ,
            'hide.daterangepicker': function (ev, picker) {
                $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY');
                $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY');
                if ($scope.startDate == $scope.PrevStartDate && $scope.endDate == $scope.PrevEndDate) {
                    //alert('not changed')
                }
                else {
                    $scope.PrevStartDate = $scope.startDate;
                    $scope.PrevEndDate = $scope.endDate;
                    $scope.dateFilterEnum = 5;
                    // call function which will fetch data
                    //get report data for default
                    $scope.getReportData();
                    //end get data
                }
            }
        }
    };

    $scope.dateFilterEnum = 1;
    // end set dates

    //
    //Last15Days = 1,
    //Last30Days = 2,
    //Last60Days = 3,
    //Last90Days = 4,
    //Custom = 5

    $scope.selectedItemChanged=function()
    {//"{"Id":33,"UserName":"ibex360insight","UserId":"3137705600","Name":"Insight360","ProfileExternalId":"3137705600","ProfilePicURL":"https://igcdn-photos-c-a.akamaihd.net/hphotos-ak-xpl1/t51.2885-19/12519410_1597704290544162_1977335063_a.jpg","IsFollowingContact":false,"IsFollowedByContact":false,"IsPage":false,"$$hashKey":"object:298"}"
        var a = $scope.selectedProfile;
        //get report data for default
        $scope.getReportData();
        //end get data
    }
    $scope.init = function () {
        // set business name and fetch group id
        $scope.BusinessName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.CompanyId = $rootScope.selectedCompanyId;

        // get dropdown data
        $scope.GetGroupsProfile();
    };

    $scope.dateFilter=function(enumId)
    {
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
            startDate: moment().subtract(days, "days"),
            endDate: moment()
        };

        $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY');
        $scope.PrevStartDate = $scope.startDate;
        $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY');
        $scope.PrevEndDate = $scope.endDate;


        $scope.dateFilterEnum = enumId;
        //get report data for default
        $scope.getReportData();
        //end get data
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

    //get list of profiles for dropdown
    $scope.GetGroupsProfile=function()
    {
        ProfileService.GetInstagramProfilesByGroupId($scope.GroupId, $scope.CompanyId, false).then(function (l) {
            if (l.data != null) {
                $scope.ddlProfileList = l.data;
                $scope.selectedProfile = $scope.ddlProfileList[0];//setting default value for dropdown
                
                //get report data for default
                $scope.getReportData();
                //end get data
            }
            else {
                // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            }
        }, function (err) {
            // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            console.log(err);
        });
    }
    // runs once per controller instantiation
    $scope.init();
  
    $scope.exportPdf=function()
    {

    }

    $scope.exportCsv=function()
    {

    }
  
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
  
      
}]);