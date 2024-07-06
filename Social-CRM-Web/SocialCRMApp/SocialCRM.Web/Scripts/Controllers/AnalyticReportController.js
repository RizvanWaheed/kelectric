CRMApp.controller('AnalyticReportController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'GoogleService', function ($rootScope, $scope, $window, $location, $stateParams, GoogleService) {
    // alert(('#ctrlanalyticdatepicker').val())
    $scope.init = function () {

        $scope.BusinessName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.GroupId = $rootScope.selectedGroupId;

        $scope.SelectedAnalyticAccounts = [];
        $scope.analyticAccountList = [];
        $scope.AnalyticAccountName = "";
        $scope.SelectedAnalyticAccountId = "";
        $scope.websiteTraffic = "";
        $scope.websiteBounces = "";
        $scope.websiteBounceRate = "";
        $scope.chartConfig.series = [{ "name": "Website Traffic ", "data": [], type: "column" }];
        $scope.chartConfig.loading = true;

        GoogleService.GetAnalyticsChartAndAccount($scope.GroupId).then(function (l) {
            if (l.data.Analytics != undefined) {
                var result = [];
                var graphvalues = [];
                for (var index = 0; index < l.data.Analytics.AnalyticGraph.length; index++) {
                    graphvalues = [];
                    graphvalues.push(l.data.Analytics.AnalyticGraph[index].TrafficDate);
                    graphvalues.push(l.data.Analytics.AnalyticGraph[index].WebsiteTraffic);
                    result.push(graphvalues);
                }
                //console.clear();
                //console.log(l.data.Accounts[0]);
                $scope.SetDateRanges(15);
                $scope.SelectedAnalyticAccounts = l.data.Accounts[0];
                $scope.analyticAccountList = l.data.Accounts;
                $scope.AnalyticAccountName = l.data.Accounts[0].AccountName + ' ' + l.data.Accounts[0].AccountUrl;
                $scope.SelectedAnalyticAccountId = l.data.Accounts[0].Id;
                $scope.websiteTraffic = l.data.Analytics.TotalWebsiteTraffic;
                $scope.websiteBounces = l.data.Analytics.TotalBounces;
                $scope.websiteBounceRate = l.data.Analytics.BounceRate;
                $scope.chartConfig.series = [{ "name": "Website Traffic ", "data": result, type: "column" }];
                $scope.chartConfig.loading = false;
            }
        }, function (err) {
            console.log(err);
        });
    };
    //Charting Start
    $scope.chartConfig = {
        options: {
            chart: {
                type: 'column'
            },
            plotOptions: {
                series: {
                    stacking: ''
                }
            }
        },
        series: [],
        title: {
            text: 'Google Analytics'
        },
        credits: {
            enabled: false
        },
        loading: true,
        size: {}
    };
    // runs once per controller instantiation
    $scope.init();
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
    $scope.SelectedAnalyticAccountId;
    $scope.analyticAccountList;
    $scope.SelectedDateFilter=1;
    $scope.exportPdf=function()
    {

    }

    $scope.exportCsv=function()
    {

    }
    
    $scope.SetDateRanges=function(days)
    {
        $today = new Date();
        $last15day = new Date($today);
        $last15day.setDate($today.getDate() - days);
        var todayMonth = ($today.getMonth() + 1);
        var todayDay = $today.getDate();
        todayMonth = parseInt(todayMonth) > 9 ? todayMonth : '0' + todayMonth;
        todayDay = parseInt(todayDay) > 9 ? todayDay : '0' + todayDay;

        var last15Month = ($last15day.getMonth() + 1);
        var last15Day = $last15day.getDate();
        last15Month = parseInt(last15Month) > 9 ? last15Month : '0' + last15Month;
        last15Day = parseInt(last15Day) > 9 ? last15Day : '0' + last15Day;

        $scope.SearchToDate = todayMonth + '/' + todayDay + '/' + $today.getFullYear();
        $scope.SearchFromDate = last15Month + '/' + last15Day + '/' + $last15day.getFullYear();
    }
    //Charting End
    //Loading Charts on Dropdown change.
    $scope.selectedAnalyticAccount = function (analyticAccountId) {
        $scope.SelectedAnalyticAccountId = analyticAccountId;
        var accountInfo;
        $.each( $scope.analyticAccountList, function (i, obj) {
            if (obj.Id === analyticAccountId) accountInfo = obj;
        });
        console.clear();
        console.log(accountInfo);
        if (accountInfo != null)
            $scope.AnalyticAccountName = accountInfo.AccountName + ' ' + accountInfo.AccountUrl;
        else
            $scope.AnalyticAccountName = '';

        GoogleService.GetAnalyticsChart(analyticAccountId, $scope.SelectedDateFilter).then(function (l) {
            var result = [];
            var graphvalues = [];
            for (var index = 0; index < l.data.AnalyticGraph.length; index++) {
                graphvalues = [];
                graphvalues.push(l.data.AnalyticGraph[index].TrafficDate);
                graphvalues.push(l.data.AnalyticGraph[index].WebsiteTraffic);
                result.push(graphvalues);
            }
            
            $scope.websiteTraffic = l.data.TotalWebsiteTraffic;
            $scope.websiteBounces = l.data.TotalBounces;
            $scope.websiteBounceRate = l.data.BounceRate;
            $scope.chartConfig.series = [{ "name": "Website Traffic ", "data": result, type: "column" }];
            $scope.chartConfig.loading = false;
        }, function (err) {
            console.clear();
            console.log(err);
        });
    };
    //Loading Charts on Calender control.
    $scope.GetAnalytics = function (dateId)
    {
        var customDateFilter = '';
        if (dateId == 1)
            $scope.SetDateRanges(15);
        else if (dateId == 2)
            $scope.SetDateRanges(30);
        else if (dateId == 3)
            $scope.SetDateRanges(60);
        else if (dateId == 4)
            $scope.SetDateRanges(90);
        else if (dateId == 5) {
            //yyyy-MM-dd"
            customDateFilter = $scope.SearchFromDate + '-' + $scope.SearchToDate;
            customDateFilter = customDateFilter.split('/').join('|');
        }
        $scope.SelectedDateFilter = dateId;
        GoogleService.GetAnalyticsChart($scope.SelectedAnalyticAccountId, dateId, customDateFilter).then(function (l) {
            var result = [];
            var graphvalues = [];
            for (var index = 0; index < l.data.AnalyticGraph.length; index++) {
                graphvalues = [];
                graphvalues.push(l.data.AnalyticGraph[index].TrafficDate);
                graphvalues.push(l.data.AnalyticGraph[index].WebsiteTraffic);
                result.push(graphvalues);
            }

            $scope.websiteTraffic = l.data.TotalWebsiteTraffic;
            $scope.websiteBounces = l.data.TotalBounces;
            $scope.websiteBounceRate = l.data.BounceRate;
            $scope.chartConfig.series = [{ "name": "Website Traffic ", "data": result, type: "column" }];
            $scope.chartConfig.loading = false;
        }, function (err) {
            console.clear();
            console.log(err);
        });
    };
}]);