CRMApp.controller('TaskPerformanceReportController', ['$rootScope', '$scope', '$location', '$stateParams', 'ReportService', '$window', '$cookies', 'NotificationService', 'GlobalService', 'notify', function ($rootScope, $scope, $location, $stateParams, ReportService, $window, $cookies, NotificationService, GlobalService, notify) {
    $scope.init = function () {
        $scope.BusinessName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.GetTaskPerformanceData();
    }
    $scope.dateFrom;
    $scope.dateTo;
    $scope.data = new Array();
    $scope.chartSeries = new Array();    
    $scope.TotalAssignedTasks = 0;
    $scope.TotalCompletedTasks = 0;
    $scope.IncreaseInTasks = false;
    $scope.IncreaseInTasksVal = "";
    $scope.TasksByMember = new Array();
    $scope.ShowReport = false;

    //date items
    $scope.startDate = '';
    $scope.PrevStartDate = '';
    $scope.endDate = '';
    $scope.PrevEndDate = '';

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
                    $scope.GetTaskPerformanceData();
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
                    $scope.GetTaskPerformanceData();
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
                    $scope.GetTaskPerformanceData();
                    //end get data
                }
            }
        }
    };

    $scope.date = {
        startDate: moment().subtract(15, "days"),
        endDate: moment()
    };

    $scope.startDate = $scope.date.startDate.format('DD/MM/YYYY');
    $scope.PrevStartDate = $scope.startDate;
    $scope.endDate = $scope.date.endDate.format('DD/MM/YYYY');
    $scope.PrevEndDate = $scope.endDate;

    $scope.dateFilterEnum = 1;

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
        $scope.GetTaskPerformanceData();
        //end get data
    }

    $scope.reflow = function () {
        $scope.$broadcast('highchartsng.reflow');
    };

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.GetTaskPerformanceData = function () {
        var grpId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;
        var dateFrom = $scope.startDate;
        var dateTo = $scope.endDate;
        ReportService.GetTaskPerformance(grpId, companyId, dateFrom, dateTo).then(function (l) {
            $scope.TotalAssignedTasks = l.data.TotalAssignedTasks;
            $scope.TotalCompletedTasks = l.data.TotalCompletedTasks;
            var increasedVal = l.data.AssignedTasksIncreased;
            if (increasedVal == "Infinity" || isNaN(increasedVal)) {
                $scope.IncreaseInTasks = true;
                $scope.IncreaseInTasksVal = "Infinity";
            }
            else {
                var val = parseInt(increasedVal);
                $scope.IncreaseInTasks = val > 0;
                $scope.IncreaseInTasksVal = increasedVal + " %";
            }
            $scope.data = l.data.TasksByDate;
            $scope.TasksByMember = l.data.TaskPerformance;
            $scope.ShowReport = Math.max.apply(Math, $scope.data.map(function (o) { return o.NumOfTasks; })) > 0;

            if ($scope.ShowReport) {
                var days = [];
                for (var i = 0; i < $scope.data.length; i++) {
                    days.push($scope.data[i].Date);
                }

                var tasks = [];
                for (var i = 0; i < $scope.data.length; i++) {
                    tasks.push($scope.data[i].NumOfTasks);
                }

                $scope.chartSeries = [
                  { "name": "Days", "data": tasks }
                ];

                $scope.chartConfig = {
                    options: {
                        chart: {
                            type: 'line'
                        },
                        yAxis: {
                            title: {
                                text: 'Tasks'
                            }
                        },
                        plotOptions: {
                            series: {
                                stacking: ''
                            }
                        }
                    },
                    series: $scope.chartSeries,
                    xAxis: {
                        categories: days
                    },
                    title: {
                        text: 'Task Performance Data'
                    },
                    credits: {
                        enabled: false
                    },
                    loading: false,
                    size: {}
                }
            }
            
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    $scope.init();
}]);