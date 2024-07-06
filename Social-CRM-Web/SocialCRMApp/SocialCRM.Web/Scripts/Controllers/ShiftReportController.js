CRMApp.controller('ShiftReportController',
['$scope', '$location', 'AuthService', '$window','$timeout', '$cookies', 'NotificationService', 'ReportService', 'TaskService', '$rootScope', '$stateParams', 'UnifiedService', 'GlobalService', '$filter', 'CustomerProfileService',
function ($scope, $location, AuthService, $window,$timeout, $cookies, NotificationService, ReportService, TaskService, $rootScope, $stateParams, UnifiedService, GlobalService, $filter, CustomerProfileService) {
    
    $scope.colors = ['#31ce56', '#3187ce', '#a5abaf', '#a5abaf', '#02505F', '#7B9500', '#7C0046', '#007575', '#C2C200', '#FF7400',
                    '#8605AF', '#00A0A0', '#00B26F', '#9FEF00', '#FFE100', '#FF0000', '#1139B2', '#323459', '#421D0F', '#240A11', '#201D05', 
                    '#09201A', '#212120', '#719B53', '#794A5F', '#1D1F72', '#8A6411', '#24221C', '#4F4500', '#003827', '#6B7225'];

    $scope.selectedShift = "1";
    $scope.selectedDate = new Date();
    $scope.selectedShiftStartTime = "";
    $scope.selectedShiftEndTime = "";

    $scope.dateDisplay = "";
    $scope.monthDisplay = "";
    $scope.shiftDisplay = "";

    $scope.TagsListShift = [];
    $scope.TagsSelected = [];

    $scope.customMultiselectBtnText = {
        buttonDefaultText: 'Please Select'
    };
    $scope.customMultiselectExtraSettings = {
        scrollableHeight: '300px',
        scrollable: true,
        enableSearch: true
    };

    $scope.top3Areas = "None";
    $scope.shiftReport = {};
    $scope.shiftReport.TotalInboundTraffic = 0;
    $scope.shiftReport.TagOccourances = [];
    
    $scope.currentDateopts = new Date();

    $scope.ShiftChartConfig = {};

    $scope.opts = {
        locale: {
            format: 'DD/MM/YYYY'
        },
        singleDatePicker: true,
        "endDate": $scope.currentDateopts.getDate(),
    };

    $scope.getTags = function () {
        TaskService.GetBrandKeywordByCompanyId($rootScope.selectedCompanyId).then(
            function (res) {
                if (res != undefined) {
                    for (var i = 0; i < res.data.length; i++)
                        $scope.TagsListShift.push({ label: res.data[i].KeywordText, id: res.data[i].Id });
                }
            },
            function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching tags.");
            }
        );
    }

    $scope.filteredTags = function () {
        
        return $scope.shiftReport.TagOccourances.filter(function (tag) {
            var t = $scope.TagsSelected.map(function (item) {
                return item.id;
            })
            return t.indexOf(tag.Id) !== -1;
        });
    };

    $scope.getShiftReportData = function () {
        var data = $scope.getInputs();

        ReportService.GetShiftReportData(data).then(
            function (res) {
                if (res != undefined) {
                    $scope.shiftReport = res.data;
                    
                    $scope.getEvolutionGraph();
                }
            },
            function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching Data.");
            }
        );
    }

    $scope.$watch("selectedDate", function (newValue, oldValue) {
        $scope.filterChanged();
    });

    $scope.filterChanged = function () {
        $scope.ShiftChartConfig = {};

        $scope.dateDisplay = moment($scope.selectedDate).format("MMMM Do YYYY");
        $scope.monthDisplay = moment($scope.selectedDate).format("MMMM");
        if ($scope.selectedShift == "1") {
            $scope.shiftText = "Morning Shift";
            $scope.shiftDisplay = "8AM - 4PM";
            $scope.selectedShiftStartTime = "T08:01:00.000Z";
            $scope.selectedShiftEndTime = "T16:00:00.000Z";
        }
        if ($scope.selectedShift == "2") {
            $scope.shiftText = "Evening Shift";
            $scope.shiftDisplay = "4PM - 12AM";
            $scope.selectedShiftStartTime = "T16:01:00.000Z";
            $scope.selectedShiftEndTime = "T23:59:59.000Z";
        }
        if ($scope.selectedShift == "3") {
            $scope.shiftText = "Night Shift";
            $scope.shiftDisplay = "12AM - 8AM";
            $scope.selectedShiftStartTime = "T00:00:00.000Z";
            $scope.selectedShiftEndTime = "T08:00:00.000Z";
        }

        if ($scope.TagsListShift == null || $scope.TagsListShift.length == 0) {
            $scope.getTags();
        }
        $scope.getShiftReportData();
        $scope.getAreasData();
    }

    $scope.getInputs = function () {
        var tags = [];

        for (var i = 0; i < $scope.TagsListShift.length; i++) {
            tags.push($scope.TagsListShift[i].id);
        }

        return {
            tagIds: tags,
            shiftId: $scope.selectedShift,
            companyId: $rootScope.selectedCompanyId,
            groupId: $rootScope.selectedGroupId,
            date: moment($scope.selectedDate).format("YYYY-MM-DD") + "T19:00:00.000Z"
        }
    }
$scope.myFunction=function()
{
    alert('asas')
}
$(document).ready(function(){
    $("#asadasad").load(function(){
        alert('asasas')
    })
})
    $scope.emailReport = function (shiftReportImage) {
        // debugger;
        // var svg = $scope.ShiftChartConfig.getHighcharts().container.innerHTML;
        // svg = "data:image/svg+xml," + svg;
        // var myElement = document.createElement('img');

        // myElement.src = svg;
        // myElement.width = 657;
        // myElement.height = 340;
        // myElement.id="asadasad";
      
       // var svgSize;
       // svgSize.width=657;
       // svgSize.height=340;
        // var canvas = document.createElement('canvas');
        // canvas.width = 657;
        // canvas.height = 340;
        // var ctx = canvas.getContext('2d');
        
        // var img = document.createElement('img');
        // img.setAttribute('src', 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent($scope.ShiftChartConfig.getHighcharts().container.innerHTML))));
        // img.onload = function() {
        //     ctx.drawImage(img, 0, 0);
        //     window.open(canvas.toDataURL('image/png'));
        //     debugger
        // }


    //    myElement.onload = function () {
            // var canvas = document.createElement('canvas');
            // var ctx = canvas.getContext('2d');
            // canvas.width = myElement.width;
            // canvas.height = myElement.height;

            // ctx.drawImage(myElement, 0, 0, canvas.width, canvas.height);
            // var shiftReportImage = canvas.toDataURL('image/png');

            var data = {
                shift: $scope.selectedShift,
                date: moment($scope.selectedDate).format("YYYY-MM-DD") + "T19:00:00.000Z",
                totalInboundTraffic: $scope.shiftReport.TotalInboundTraffic,
                top3Areas: $scope.top3Areas,
                onDutyAgents: $scope.shiftReport.OnDutyAgents,
                tagOccurances: $scope.filteredTags(),
                companyId: $rootScope.selectedCompanyId,
                shiftReportImage: shiftReportImage,
                weekData: $scope.shiftReport.WeekData,
                shiftTiming: $scope.shiftDisplay
            };

            ReportService.EmailShiftReport(data).then(
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
       // };
    }


    
    
    $scope.save_chart=function(chart, filename) {
        //debugger;
        var EXPORT_WIDTH = 1000;
        var render_width = EXPORT_WIDTH;
        var render_height = render_width * chart.chartHeight / chart.chartWidth
    
        var svg = chart.getSVG({
            exporting: {
                sourceWidth: chart.chartWidth,
                sourceHeight: chart.chartHeight
            }
        });
    
        var canvas = document.createElement('canvas');
        canvas.height = render_height;
        canvas.width = render_width;
    
        var image = new Image;
        image.onload = function() {
            canvas.getContext('2d').drawImage(this, 0, 0, render_width, render_height);
            var data = canvas.toDataURL("image/png")
           // download(data, filename + '.png');
            $scope.emailReport(data)
        };
        image.src = 'data:image/svg+xml;base64,' + window.btoa(svg);
    }
    $scope.saveImageEmail=function(){
        $scope.save_chart($('#graphDiv').highcharts(), 'chart');
    }
   
    $scope.getAreasData = function () {
        CustomerProfileService.GetRegionReport($rootScope.selectedGroupId, "All", moment($scope.selectedDate).format("YYYY-MM-DD") + $scope.selectedShiftStartTime, moment($scope.selectedDate).format("YYYY-MM-DD") + $scope.selectedShiftEndTime, 1, 0, 0, false, true)
        .then(function (res) {
            var areas = $filter('orderBy')(res.data, "Total_Inbound", true);
            if (areas.length > 0) {
                $scope.top3Areas = "";
                var length = areas.length > 3 ? 3 : areas.length;
                for (var i = 0; i < length; i++) {
                    if (i == length - 1 && i > 0) {
                        $scope.top3Areas += "& "+areas[i].Region;
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

    $scope.getEvolutionGraph = function () {
        var regions = new Array();
        var tw = new Array();
        var fb = new Array();
        var twAverage = new Array();
        var fbAverage = new Array();

        $scope.shiftReport.WeekData.forEach(function (i) {
            var InsertionDate = i.Date;
            if (regions.indexOf(InsertionDate) == -1) {
                regions.push(InsertionDate);
            }

            tw.push(i.TwData);
            fb.push(i.FbData);

            twAverage.push($scope.shiftReport.TwAverage);
            fbAverage.push($scope.shiftReport.FbAverage);
        });

        var data = [{
            "name": "Twitter",
            "data": tw,
            "type": "line",
            "color": $scope.colors[1]
        },
        {
            "name": "Facebook",
            "data": fb,
            "type": "line",
            "color": $scope.colors[0]
        }];

        if ($scope.shiftReport.FbAverage != 0) {
            data.push({
                "name": "Facebook Average",
                "data": fbAverage,
                "type": "line",
                "dashStyle": "shortdot",
                "color": $scope.colors[2]
            });
        }
        if ($scope.shiftReport.TwAverage != 0) {
            data.push({
                "name": "Twitter Average",
                "data": twAverage,
                "type": "line",
                "dashStyle": "shortdot",
                "color": $scope.colors[2]
            });
        }

        $scope.ShiftChartConfig = $scope.getHighChartConfig(regions, data, true);
    }

    $scope.getHighChartConfig = function (categories, series, isArea) {
        var title = "";
        var stacking = true;
        var stackLabelEnable = false;

        stacking = false;
        stackLabelEnable = true;
        title = "Inbound Traffic - " + $scope.shiftText;

        return {
            options: {
                xAxis: {
                    categories: categories,
                    labels: {
                        style: { fontWeight: 'bold', "font-family": "Calibri", "fontSize": "15px" }
                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: ""
                    },
                    allowDecimals: false,
                    stackLabels: {
                        enabled: true,
                        style: {
                            fontWeight: 'bold',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                        }
                    },
                    labels: {
                        style: { fontWeight: 'bold', "font-family": "Calibri", "fontSize": "15px" }
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
                text: title,
                style: { fontWeight: 'bold', "font-family": "Calibri", "fontSize": "24px" }
            },
            loading: false,
            "credits": {
                "enabled": false
            }
        };
    }

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.filterChanged();
    });

}]);
