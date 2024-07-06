CRMApp.controller('WarnBanProfileController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'CustomerProfileService', '$uibModal', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $stateParams, CustomerProfileService, $uibModal, GlobalService, notify) {
    
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
                    //$scope.GetTeamData();
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
                    //$scope.GetTeamData();
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
                    //$scope.GetTeamData();
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
    $scope.FromTime;
    $scope.ToTime;

    //from time
    var that = this;
    this.picker1 = {
        date: new Date(),
        timepickerOptions: {
            readonlyInput: false,
            showMeridian: false
        }
    };

    this.openCalendar = function (e, picker) {
        that[picker].open = true;
    };    
    //

    //To Time
    var that = this;
    this.picker2 = {
        date: new Date(),
        timepickerOptions: {
            readonlyInput: false,
            showMeridian: false
        }
    };
    //

    var tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    var afterTomorrow = new Date();
    afterTomorrow.setDate(tomorrow.getDate() + 1);
    $scope.events = [
      {
          date: tomorrow,
          status: 'full'
      },
      {
          date: afterTomorrow,
          status: 'partially'
      }
    ];
    function getDayClass(data) {
        var date = data.date,
          mode = data.mode;
        if (mode === 'day') {
            var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

            for (var i = 0; i < $scope.events.length; i++) {
                var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                if (dayToCheck === currentDay) {
                    return $scope.events[i].status;
                }
            }
        }

        return '';
    }
    
    $scope.init = function (){
        $scope.initWarnModel();
        $scope.initBanModel();

        $scope.ReasonList = [];
        $scope.PlatformList = [];
        $scope.ProfileList = [];
        $scope.StatusList = [];
        $scope.ReasonWiseRecords = [];
        $scope.TotalReasonWiseRecords = 0;

        $scope.example5customTexts = { buttonDefaultText: 'Please Select' };
        $scope.getUsersList("0", "0", "", "", $rootScope.selectedGroupId, true, 0, 1);
        $scope.getWarnBanUserReport();
    };

    $scope.initWarnModel = function (){
        $scope.ReasonModelWarn = [];
        $scope.PlatformModelWarn = [];
        $scope.nameWarn = "";
        $scope.usernameWarn = "";
        $scope.pageNumberWarn = 0;
        $scope.TotalWarnRecords = new Array();
        $scope.TotalWarnedRecords = 0;
        $scope.WarnUserList = new Array();
    };

    $scope.initBanModel = function () {
        $scope.ReasonModelBan = [];
        $scope.PlatformModelBan = [];
        $scope.nameBan = "";
        $scope.usernameBan = "";
        $scope.pageNumberBan = 0;
        $scope.TotalBanRecords = new Array();
        $scope.TotalBannedRecords = 0;
        $scope.BanUserList = new Array();
    };

    $scope.example5customTexts = { buttonDefaultText: 'Please Select' };    

    $scope.getUsersList = function (reasonIds, platformIds, name, userHandle, groupId, getOptions, pageNumber, type) {
        CustomerProfileService.GetUsersList(reasonIds, platformIds, name, userHandle, groupId, getOptions, pageNumber, type).then(function (l) {

              //for (var i = 0; i < l.data.WarnedProfilesList.length; i++) {
              //    var link = getPostLink(l.data.WarnedProfilesList[i].Id);
              //    l.data.WarnedProfilesList[i].PostLink = link;
              //}
              //for (var i = 0; i < l.data.BannedProfilesList.length; i++) {
              //    var link2 = getPostLink(l.data.BannedProfilesList[i].Id);
              //    l.data.BannedProfilesList[i].PostLink = link2;
              //}

            if (l.data.BannedProfilesList.length == 0 && type == 2 && $scope.pageNumberBan > 0)
                    $scope.pageNumberBan--;
            else if (l.data.WarnedProfilesList.length == 0 && type == 3 && $scope.pageNumberWarn > 0)
                    $scope.pageNumberWarn--;
            else {
                var response = l.data;
                if (type == 1) {
                    $scope.BanUserList = response.BannedProfilesList;
                    $scope.WarnUserList = response.WarnedProfilesList;

                    $scope.TotalWarnedRecords = l.data.TotalWarnedRecords;
                    var records = l.data.TotalWarnedRecords / 5;

                    if (records > Math.floor(records)) {
                        var length = Math.floor(records) + 1
                        for (var i = 0; i < length; i++)
                            $scope.TotalWarnRecords.push(i);
                    }
                    else {
                        var length = Math.floor(records);
                        for (var i = 0; i < length; i++)
                            $scope.TotalWarnRecords.push(i);
                    }

                    if ($scope.TotalWarnRecords.length == 1) {
                        $scope.TotalWarnRecords = new Array();
                    }

                    $scope.TotalBannedRecords = l.data.TotalBannedRecords;
                    records = l.data.TotalBannedRecords / 5;

                    if (records > Math.floor(records)) {
                        var length = Math.floor(records) + 1
                        for (var i = 0; i < length; i++)
                            $scope.TotalBanRecords.push(i);
                    }
                    else {
                        var length = Math.floor(records);
                        for (var i = 0; i < length; i++)
                            $scope.TotalBanRecords.push(i);
                    }

                    if ($scope.TotalBanRecords.length == 1) {
                        $scope.TotalBanRecords = new Array();
                    }
                }
                else if (type == 2) {
                    $scope.BanUserList = response.BannedProfilesList;

                    $scope.TotalBannedRecords = l.data.TotalBannedRecords;
                    var records = l.data.TotalBannedRecords / 5;

                    if (records > Math.floor(records)) {
                        var length = Math.floor(records) + 1
                        for (var i = 0; i < length; i++)
                            $scope.TotalBanRecords.push(i);
                    }
                    else {
                        var length = Math.floor(records);
                        for (var i = 0; i < length; i++)
                            $scope.TotalBanRecords.push(i);
                    }

                    if ($scope.TotalBanRecords.length == 1) {
                        $scope.TotalBanRecords = new Array();
                    }
                }
                else if (type == 3) {
                    $scope.WarnUserList = response.WarnedProfilesList;

                    $scope.TotalWarnedRecords = l.data.TotalWarnedRecords;
                    var records = l.data.TotalWarnedRecords / 5;

                    if (records > Math.floor(records)) {
                        var length = Math.floor(records) + 1
                        for (var i = 0; i < length; i++)
                            $scope.TotalWarnRecords.push(i);
                    }
                    else {
                        var length = Math.floor(records);
                        for (var i = 0; i < length; i++)
                            $scope.TotalWarnRecords.push(i);
                    }

                    if ($scope.TotalWarnRecords.length == 1) {
                        $scope.TotalWarnRecords = new Array();
                    }
                }

                $scope.userStatusChartConfig = {
                    chart: {
                        type: 'pie',
                        options3d: {
                            enabled: true,
                            alpha: 45,
                            beta: 0
                        }
                    },
                    title: {
                        text: 'Status Wise User Report'
                    },
                    tooltip: {
                        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            depth: 35,
                            dataLabels: {
                                enabled: true,
                                format: '{point.name}'
                            }
                        }
                    },
                    series: [{
                        type: 'pie',
                        name: 'Record(s)',
                        data: [
                            ['Ban', $scope.TotalBannedRecords],
                            ['Warn', $scope.TotalWarnedRecords]
                        ]
                    }]
                };

                $scope.ReasonWiseRecords = response.ReasonWiseData;

                var reasonData = [];
                $scope.ReasonWiseRecords.forEach(function (i) {
                    $scope.TotalReasonWiseRecords += i.NumOfRecords;
                    var reasonArray = new Array();
                    reasonArray.push(i.XAxisColumn);
                    reasonArray.push(i.NumOfRecords);
                    reasonData.push(reasonArray);
                });

                $scope.reasonChartConfig = {
                    chart: {
                        type: 'pie',
                        options3d: {
                            enabled: true,
                            alpha: 45,
                            beta: 0
                        }
                    },
                    title: {
                        text: 'Reason wise User report'
                    },
                    tooltip: {
                        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            depth: 35,
                            dataLabels: {
                                enabled: true,
                                format: '{point.name}'
                            }
                        }
                    },
                    series: [{
                        type: 'pie',
                        name: 'Record(s)',
                        data: reasonData
                    }]
                };

                if (getOptions) {
                    var reasons = response.ReasonList;
                    var platform = response.PlatformList;
                    var status = response.StatusList;

                    $scope.ProfileList = response.ProfilesList;

                    $.each(reasons, function (key, value) {
                        $scope.ReasonList.push({ label: value.Text, id: value.Value });
                    });

                    $.each(platform, function (key, value) {
                        $scope.PlatformList.push({ label: value.Text, id: value.Value });
                    });

                    $.each(status, function (key, value) {
                        $scope.StatusList.push({ label: value.Text, id: value.Value });
                    });
                }
            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    $scope.getWarnBanUserReport = function () {
        var dateFrom = $scope.startDate;
        var dateTo = $scope.endDate;
        var fromTime = $scope.FromTime != null ? $scope.FromTime.getHours() + ":" + $scope.FromTime.getMinutes() : "";
        var toTime = $scope.ToTime != null ? $scope.ToTime.getHours() + ":" + $scope.ToTime.getMinutes() : "";
        //
        CustomerProfileService.GetWarnBanUserReport($rootScope.selectedGroupId, dateFrom, dateTo, fromTime, toTime).then(function (l) {
            var response = l.data;                //
            var chartTypes = new Array();
            var type = 'User Banned';
            var userData = [];
            var rnd = [];
            response.forEach(function (i) {
                rnd.push({
                    y: (i.NumOfRecords)
                });
                chartTypes.push(i.XAxisColumn);
            });
            var data = {
                "name": 'User Banned',
                "data": rnd
            }
            userData.push(data);

            $scope.bannedUserChartConfig = {
                chart: {
                    type: 'areaspline'
                },
                title: {
                    text: 'User Banned'
                },
                legend: {
                    layout: 'vertical',
                    align: 'left',
                    verticalAlign: 'top',
                    x: 150,
                    y: 100,
                    floating: true,
                    borderWidth: 1,
                    backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'
                },
                xAxis: {
                    categories: chartTypes,
                    plotBands: [{ // visualize the weekend
                        from: 4.5,
                        to: 6.5,
                        color: 'rgba(68, 170, 213, .2)'
                    }]
                },
                yAxis: {
                    title: {
                        text: type
                    },
                    allowDecimals: false
                },
                tooltip: {
                    shared: true,
                    valueSuffix: ''
                },
                credits: {
                    enabled: false
                },
                plotOptions: {
                    areaspline: {
                        fillOpacity: 0.5
                    }
                },
                series: userData
            };
            //

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
        //
    };
    
    $scope.addUser = function (){
        $scope.items = { Type: "Add", Username: "", UserObj: null };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_AddWarnBanUser.html',
            controller: '_AddWarnBanUserController',
            scope: $scope,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
            if (selectedItem != "" || selectedItem !== undefined) {
                $scope.init();
            }
        }, function () {
            //  $log.info('Modal dismissed at: ' + new Date());
        });
    };

    $scope.editUser = function (id, type) {
        var UserObj = null;
        if (type == 1) {
            UserObj = $.grep($scope.WarnUserList, function (record) {
                return record.Id == id
            })[0];
        }
        else {
            UserObj = $.grep($scope.BanUserList, function (record) {
                return record.Id == id
            })[0];
        }
        $scope.items = { Type: "Edit", Username: "", UserObj: UserObj };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_AddWarnBanUser.html',
            controller: '_AddWarnBanUserController',
            scope: $scope,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
            if (selectedItem != "" || selectedItem !== undefined) {
                $scope.init();
            }
        }, function () {
            //  $log.info('Modal dismissed at: ' + new Date());
        });
    };

    $scope.deleteUser = function (id) {
        GlobalService.ConfirmPopup("Confirm", "Are you sure you want to delete?", function (selectedItem){
	        if (selectedItem) {
		        CustomerProfileService.DeleteBannedProfile(id).then(function (l) {
                var response = l.data;
                if (response == true) {
                    notify({
                        message: "User deleted Successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.init();
                }
                else {
                    GlobalService.ErrorPopup("Error", "Delete Failed");
                }

            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
            });
	        }
        });       
    }

    $scope.filterWarnUsers = function (calledFrom) {
        if(calledFrom == "Outside")
            $scope.pageNumberWarn = 0;
        var reasonIds = "";
        $scope.ReasonModelWarn.forEach(function (i) {
            reasonIds += i.id + ","
        });
        if (reasonIds.length > 0)
            reasonIds = reasonIds.substring(0, reasonIds.length - 1);

        var platformIds = "";
        $scope.PlatformModelWarn.forEach(function (i) {
            platformIds += i.id + ","
        });
        if (platformIds.length > 0)
            platformIds = platformIds.substring(0, platformIds.length - 1);

        $scope.getUsersList(reasonIds, platformIds, $scope.nameWarn, $scope.usernameWarn, $rootScope.selectedGroupId, false, $scope.pageNumberWarn, 3);
    };

    $scope.resetWarnUsers = function () {
        $scope.initWarnModel();
        $scope.getUsersList("0", "0", "", "", $rootScope.selectedGroupId, false, 0, 3);
    };

    $scope.filterBanUsers = function (calledFrom) {
        if(calledFrom == "Outside")
            $scope.pageNumberBan = 0;
        var reasonIds = "";
        $scope.ReasonModelBan.forEach(function (i) {
            reasonIds += i.id + ","
        });
        if (reasonIds.length > 0)
            reasonIds = reasonIds.substring(0, reasonIds.length - 1);

        var platformIds = "";
        $scope.PlatformModelBan.forEach(function (i) {
            platformIds += i.id + ","
        });
        if (platformIds.length > 0)
            platformIds = platformIds.substring(0, platformIds.length - 1);

        $scope.getUsersList(reasonIds, platformIds, $scope.nameBan, $scope.usernameBan, $rootScope.selectedGroupId, false, $scope.pageNumberBan, 2);
    };

    $scope.resetBanUsers = function () {
        $scope.initBanModel();
        $scope.getUsersList("0", "0", "", "", $rootScope.selectedGroupId, false, 0, 2);
    };

    $scope.loadMoreWarnUsers = function (pageNumber) {
        $scope.pageNumberWarn = pageNumber;
        $scope.filterWarnUsers("Inside");
    };

    $scope.loadMoreBanUsers = function (pageNumber) {
        $scope.pageNumberBan = pageNumber;
        $scope.filterBanUsers("Inside");
    };

    $scope.banUnbanUser = function (id, action) {
        CustomerProfileService.BanUnbanUser(id, action).then(function (l) {
            var response = l.data;
            if (response == "success") {
                notify({
                    message: "User " + action + " Successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $scope.init();
            }
            else {
                GlobalService.ErrorPopup(action + " Failed", response);
            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    
    $scope.viewAttachment = function (id) {
        $scope.items = { Id: id };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ViewAttachment.html',
            controller: '_ViewAttachmentController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });
    }

    getPostLink = function (id) {

        var PostLink = "";
        CustomerProfileService.GetPostLink(id).then(function (l) {
            if (l.data != undefined)
                PostLink = l.data;
            else
                PostLink = "";

            return PostLink;
        });
    }

    $scope.banUser = function (id, type) {
        var UserObj = null;
        if (type == 1) {
            //UserObj = $.grep($scope.WarnUserList, function (record) {
            //    return record.Id == id
            //})[0];
            UserObj = angular.copy($.grep($scope.WarnUserList, function (record) {
                return record.Id == id
            })[0]);
        }
        else {
            //UserObj = $.grep($scope.BanUserList, function (record) {
            //    return record.Id == id
            //})[0];
            UserObj = angular.copy($.grep($scope.BanUserList, function (record) {
                return record.Id == id
            })[0]);
        }
        if(UserObj)
            UserObj.StatusId = 2;
        $scope.items = { Type: "Ban", Username: "", UserObj: UserObj };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_AddWarnBanUser.html',
            controller: '_AddWarnBanUserController',
            scope: $scope,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
            if (selectedItem != "" || selectedItem !== undefined) {
                $scope.init();
            }
        }, function () {
            //  $log.info('Modal dismissed at: ' + new Date());
        });
    };

    $scope.goToNextPageWarn = function () {
        $scope.pageNumberWarn++;
        $scope.filterWarnUsers("Inside");
    }

    $scope.goToPreviousPageWarn = function () {
        if ($scope.pageNumberWarn > 0) {
            $scope.pageNumberWarn--;
            $scope.filterWarnUsers("Inside");
        }
    }

    $scope.goToNextPageBan = function () {
        $scope.pageNumberBan++;
        $scope.filterBanUsers("Inside");
    }

    $scope.goToPreviousPageBan = function () {
        if ($scope.pageNumberBan > 0) {
            $scope.pageNumberBan--;
            $scope.filterBanUsers("Inside");
        }
    }

    $scope.init();
    

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

}]);