CRMApp.controller('CustomerProfileController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'CustomerProfileService', 'notify', '$uibModal', 'GlobalService', 'UnifiedService', function ($rootScope, $scope, $window, $location, $stateParams, CustomerProfileService, notify, $uibModal, GlobalService, UnifiedService) {
    
    $scope.reportInit = function () {
        $scope.selectedDateReport = {
            startDate: moment().subtract(1, "months"),
            endDate: moment()
        };
        $scope.dateOptsRegion = {
            locale: {
                format: 'DD/MM/YYYY'
            },
            "dateLimit": { "months": 3 },
            "maxDate": $scope.selectedDateReport.endDate,
            "eventHandlers": {
                'apply.daterangepicker': function (ev, picker) {
                    $scope.fetchRegionReport();
                }
            }
        };
        $scope.dateOptsArea = {
            locale: {
                format: 'DD/MM/YYYY'
            },
            "dateLimit": { "months": 3 },
            "maxDate": $scope.selectedDateReport.endDate,
            "eventHandlers": {
                'apply.daterangepicker': function (ev, picker) {
                    $scope.fetchAreaReport();
                }
            }
        };
        $scope.selectedRegionDate = $scope.selectedDateReport;
        $scope.selectedAreaDate = $scope.selectedDateReport;
        $scope.AreaReportKERegions = [];
        $scope.RegionReportData = [];
        $scope.AreaReportData = [];
        $scope.isRegionEvolved = false;
        $scope.isAreaEvolved = false;
        $scope.RegionInOutEvolved = "Inbound";
        $scope.AreaInOutEvolved = "Inbound";
        $scope.MediaPlatforms = [
            { Id: 0, Name: "All" },
            { Id: 1, Name: "Facebook" },
            { Id: 2, Name: "Twitter" }
        ];

        $scope.ReportFor = [
            { Id: 0, Name: "Inbound/Outbound" },
            { Id: 1, Name: "Sentiments" },
            { Id: 2, Name: "Tags" }
        ];

        $scope.OtherOptions = [
            { Id: 0, Name: "Total" },
            { Id: 1, Name: "Unique Contracts" }
        ];
        $scope.selectedRegionPlatform = $scope.MediaPlatforms[0];
        $scope.selectedAreaPlatform = $scope.MediaPlatforms[0];
        $scope.selectedRegionReport = $scope.ReportFor[0];
        $scope.selectedAreaReport = $scope.ReportFor[0];
        $scope.selectedRegionOtherOptions = $scope.OtherOptions[0];
        $scope.selectedAreaOtherOptions = $scope.OtherOptions[0];

        $scope.fetchRegionReport();
    };

    $scope.init = function () {
        $scope.PageNumber = 0;
        $scope.PageSize = 5;
        $scope.customerDetailsList = [];
        $scope.FBProfiles = [];
        $scope.TwitProfiles = [];
        $scope.KERegions = [];
        $scope.KEAreas = [];
        $scope.KEAreas.unshift({ Id: 0, Name: "Select Areas" });
        $scope.AreaSelected = $scope.KEAreas[0];
        $scope.TempTwitSelected = {};
        $scope.TempFBSelected = {};
        $scope.TempRegionSelected = {};
        $scope.TempAreaSelected = $scope.AreaSelected;
        $scope.startDate = moment().subtract(3, "months");
        $scope.endDate = moment();
        $scope.selectedDate = {
            startDate: $scope.startDate,
            endDate: $scope.endDate
        };

        $scope.dateOpts = {
            locale: {
                format: 'DD/MM/YYYY'
            },
            "dateLimit": { "years": 5 },
            "maxDate": $scope.endDate,
            "eventHandlers": {
                'apply.daterangepicker': function (ev, picker) {
                    $scope.filterRecords();
                }
            }
        };

        $scope.SearchByOptions = [
            { Id: 0, Name: "Search by" },
            { Id: 1, Name: "Contract Account" },
            { Id: 2, Name: "Name" },
            { Id: 3, Name: "Mobile No." },
            { Id: 4, Name: "Landline No." },
            { Id: 5, Name: "Email ID" },
            { Id: 6, Name: "Region/IBC" },
            { Id: 7, Name: "Customer Facebook Name" },
            { Id: 8, Name: "Customer Twitter Handle" }
        ];
        $scope.selectedSearchOption = $scope.SearchByOptions[0];
        $scope.TempSearchOption = $scope.selectedSearchOption;
        $scope.searchValue = "";
        $scope.TempSearchValue = $scope.searchValue;
        $scope.GetCustomerRegions();

        UnifiedService.GetGroupFacebookProfiles($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal).then(function (l) {
            l.data.unshift({ Id: 0, Name: "Select Facebook Profile", IsPage: false });
            $scope.FBProfiles = l.data;
            UnifiedService.GetGroupTwitterProfiles($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal).then(function (l) {
                $scope.TwitProfiles = l.data;
                l.data.unshift({ Id: 0, Name: "Select Twitter Profile", IsPage: false });
                if ($scope.FBProfiles.length > 1 && $scope.TwitProfiles.length > 1) {
                    $scope.TwitProfileSelected = $scope.TwitProfiles[1];
                    $scope.TwitProfileMappingSelected = $scope.TwitProfiles[1];
                    $scope.FBProfileSelected = $scope.FBProfiles[1];
                }
                else {
                    $scope.TwitProfileSelected = $scope.TwitProfiles[0];
                    $scope.TwitProfileMappingSelected = $scope.TwitProfiles[0];
                    $scope.FBProfileSelected = $scope.FBProfiles[0];
                }
                $scope.TempTwitSelected = $scope.TwitProfileSelected;
                $scope.TempFBSelected = $scope.FBProfileSelected;
                $scope.fetchCustomerAllInfo();
            }, function (err) {
                //alert('Error in fetching User Facebook Profiles');
                GlobalService.ErrorPopup("Error", "Error in fetching User Twitter Profiles.");

                //console.log(err);
            });
        }, function (err) {
            //alert('Error in fetching User Facebook Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching User Facebook Profiles.");

            //console.log(err);
        });
    };

    $scope.GetCustomerRegions = function () {
        CustomerProfileService.GetCustomerRegions().then(function (l) {
            l.data.unshift({ Id: 0, Name: "Select Region" });
            $scope.KERegions = l.data;
            $scope.RegionSelected = $scope.KERegions[0];
            $scope.TempRegionSelected = $scope.RegionSelected;
            $scope.AreaReportKERegions = l.data;
            $scope.selectedAreaRegion = $scope.AreaReportKERegions[0];

            $scope.fetchAreaReport();
        }, function (err) {
            //alert('Error in fetching User Facebook Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching Regions.");
        });
    }

    $scope.SearchOptionChanged = function () {
        if ($scope.selectedSearchOption.Id == 0 || $scope.selectedSearchOption.Id == 6) {
            $scope.searchValue = "";
        }
        else {
            $scope.RegionSelected = $scope.KERegions[0];
            $scope.AreaSelected = $scope.KEAreas[0];
        }
    }

    $scope.GetCustomerAreas = function () {
        if ($scope.RegionSelected.Id == 0) {
            $scope.KEAreas = [];
            $scope.KEAreas.unshift({ Id: 0, Name: "Select Areas" });
            $scope.AreaSelected = $scope.KEAreas[0];

            $scope.filterRecords();
        }
        else {
            CustomerProfileService.GetCustomerAreas($scope.RegionSelected.Id).then(function (l) {
                l.data.unshift({ Id: 0, Name: "Select Areas" });
                $scope.KEAreas = l.data;
                $scope.AreaSelected = $scope.KEAreas[0];

                $scope.filterRecords();
            }, function (err) {
                //alert('Error in fetching User Facebook Profiles');
                GlobalService.ErrorPopup("Error", "Error in fetching Regions.");
            });
        }
    }

    $scope.goToNextPage = function () {
        $scope.PageNumber++;
        $scope.fetchCustomerAllInfo();
    }

    $scope.goToPreviousPage = function () {
        if ($scope.PageNumber > 0) {
            $scope.PageNumber--;
            $scope.fetchCustomerAllInfo();
        }
    }

    $scope.WarnBanCustomer = function (parent, child) {
        var obj = $scope.customerDetailsList[parent].SocialProfiles[child];

        if (obj.WarnStatus == 0 || obj.WarnStatus == 1) {
            var PlatformId = 0;
            var ProfileUrl = "";
            var Username = "";
            var ConnectedProfileId = 0;
            if(obj.SocialType == "Twitter"){
                PlatformId = 4;
                ProfileUrl = "http://twitter.com/" + obj.SocialUserName;
                Username = obj.SocialUserName;
                ConnectedProfileId = $scope.TwitProfileSelected.Id;
            }
            else if(obj.SocialType == "Facebook"){
                PlatformId = 1;
                ProfileUrl = "http://facebook.com/" + obj.SocialExternalId;
                Username = obj.SocialExternalId;
                ConnectedProfileId = $scope.FBProfileSelected.Id;
            }

            var UserObj = { 
                ProfileUrl: ProfileUrl,
                Name: Username,
                BannedBy: "",
                Details: "",
                ReasonId: 0,
                PlatformId: PlatformId,
                StatusId: obj.WarnStatus + 1,
                ProfileId: ConnectedProfileId,
                Id: obj.WarnId
            };

            $scope.items = { Type: obj.WarnStatus == 0?"Warn":"Ban", Username: "", UserObj: UserObj };

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
                if (selectedItem != "cancel") {
                    obj.WarnStatus++;
                    obj.WarnId = selectedItem;
                }
                //alert($scope.selected)
            }, function () {
                //  $log.info('Modal dismissed at: ' + new Date());
            });
        }
        else if (obj.WarnStatus == 2) {
            $scope.banUnbanUser(obj, "unban");
        }
    }

    $scope.banUnbanUser = function (obj, action) {
        CustomerProfileService.BanUnbanUser(obj.WarnId, action).then(function (l) {
            var response = l.data;
            if (response == "success") {
                notify({
                    message: "User " + action + " Successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                obj.WarnStatus = 0;
                obj.WarnId = 0;
            }
            else {
                GlobalService.ErrorPopup(action + " Failed", response);
            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    $scope.filterRecords = function () {
        $scope.PageNumber = 0;
        $scope.TempTwitSelected = $scope.TwitProfileSelected;
        $scope.TempFBSelected = $scope.FBProfileSelected;
        $scope.TempRegionSelected = $scope.RegionSelected;
        $scope.TempAreaSelected = $scope.AreaSelected;
        $scope.startDate = $scope.selectedDate.startDate;
        $scope.endDate = $scope.selectedDate.endDate;
        $scope.TempSearchValue = $scope.searchValue;
        $scope.TempSearchOption = $scope.selectedSearchOption;

        $scope.fetchCustomerAllInfo();
    }

    $scope.fetchCustomerAllInfo = function () {
        var profileId = 0;
        var profilePageId = 0;
        if ($scope.TempFBSelected.IsPage == true) {
            profilePageId = $scope.TempFBSelected.Id;
        }
        else {
            profileId = $scope.TempFBSelected.Id;
        }
        var searchText = $scope.TempSearchValue;
        if (searchText == "") {
            searchText = "0";
        }

        if ($scope.TempSearchOption.Id == 8 && searchText.startsWith("@"))
            searchText = searchText.substring(1);

        CustomerProfileService.GetCustomerAllInfo($scope.PageNumber, $scope.PageSize, $rootScope.selectedGroupId, profileId, profilePageId,
            $scope.TempTwitSelected.Id, $scope.TempRegionSelected.Id, $scope.TempAreaSelected.Id, moment($scope.startDate).add(5, "hours").toISOString(),
            moment($scope.endDate).add(5, "hours").toISOString(), $scope.TempSearchOption.Id, searchText).then(function (l) {
            if (l != null) {
                if (l.data.length == 0 && $scope.PageNumber > 0) {
                    $scope.PageNumber--;
                }
                else {
                    $scope.customerDetailsList = l.data;
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup('Error in fetching, please try again.');
            console.log(err);
        });
    };

    $scope.updateSocialFilePath = function () {
        var file = $("input[type='file']").get(1).files[0];
        if (file) {
            $scope.$apply(function () {
                // every changes goes here
                $scope.SocialFilePathUpload = file.name;
            });
        }
    }

    $scope.uploadSocialExcelFile = function () {
        $scope.socialErrorMsg = "";
        var allowedExtentions = ["xls", "xlsx"];
        var file = $("input[type='file']").get(1).files[0];
        if (file) {
            var fileExtension = file.name.split('.')[1].toLowerCase();
            if (!allowedExtentions.includes(fileExtension)) {
                $scope.socialErrorMsg = "Invalid file";
                return;
            }
            if ($scope.TwitProfileMappingSelected.Id == 0) {
                $scope.socialErrorMsg = "Please select a connected twitter profile first";
                return;
            }

            $scope.socialFileName = file.name;
            $scope.socialContentType = file.type;
            $scope.convertToSocialBase64StringAndSend(file);
        }
        else {
            $scope.socialErrorMsg = "Please select an excel file";
            return;
        }
    };

    $scope.convertToSocialBase64StringAndSend = function (file) {
        var reader = new FileReader();
        reader.addEventListener("loadend", function () {
            console.log(reader.result.length);
            $scope.socialbase64String = reader.result;
            $scope.uploadSocialCustomerFile();

        }, false);

        if (file) {
            reader.readAsDataURL(file);
        }
    };

    $scope.uploadSocialCustomerFile = function () {

        CustomerProfileService.UploadSocialCustomerFile($scope.socialFileName, $scope.socialContentType, $scope.socialbase64String, $scope.TwitProfileMappingSelected.Id).then(function (l) {
            if (l != null) {
                var response = l.data;
                if (response != -1) {
                    notify({
                        message: response + " Customers mapped successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.init();
                }
                else {
                    notify({
                        message: "Customers mapping information save failed. Please ensure that the column names are right!",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup('Error in saving, please try again.');
            console.log(err);
        });
    };

    $scope.updateFilePath = function () {
        var file = $("input[type='file']").get(0).files[0];
        if (file) {
            $scope.$apply(function () {
                // every changes goes here
                $scope.FilePathUpload = file.name;
            });
        }
    }

    $scope.uploadExcelFile = function () {
        $scope.errorMsg = "";
        var allowedExtentions = ["xls", "xlsx"];
        var file = $("input[type='file']").get(0).files[0];
        if (file) {
            var fileExtension = file.name.split('.')[1].toLowerCase();
            if (!allowedExtentions.includes(fileExtension)) {
                $scope.errorMsg = "Invalid file";
                return;
            }
            $scope.fileName = file.name;
            $scope.contentType = file.type;
            $scope.convertToBase64StringAndSend(file);
        }
        else {
            $scope.errorMsg = "Please select an excel file";
            return;
        }
    };

    $scope.convertToBase64StringAndSend = function (file) {
        var reader = new FileReader();
        reader.addEventListener("loadend", function () {
            console.log(reader.result.length);
            $scope.base64String = reader.result;
            $scope.uploadCustomerFile();

        }, false);

        if (file) {
            reader.readAsDataURL(file);
        }
    };

    $scope.uploadCustomerFile = function () {

        CustomerProfileService.UploadCustomerFile($scope.fileName, $scope.contentType, $scope.base64String).then(function (l) {
            if (l != null) {
                var response = l.data;
                if (response != -1) {
                    notify({
                        message: response + " Customers imported successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.init();
                }
                else {
                    notify({
                        message: "Customers information save failed. Please ensure that the column names are right!",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }

            }
        }, function (err) {
            GlobalService.ErrorPopup('Error in saving, please try again.');
            console.log(err);
        });
    };

    $scope.deleteCustomer = function (contractAccount) {
        var isConfirmed = confirm('Are you sure you want to delete?');
        if (isConfirmed) {
            CustomerProfileService.DeleteCustomer(contractAccount).then(function (l) {
                if (l.data == true) {
                    notify({
                        message: "Customer Information deleted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.fetchCustomerAllInfo();
                }
            }, function (err) {
                //alert('Error in fetching User Facebook Profiles');
                GlobalService.ErrorPopup("Error", "Error in deleting Customer.");
            });
        }
    }

    $scope.editCustomer = function (cust) {
        $scope.items = { Cust: cust };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EditKECustomer.html',
            controller: '_EditKECustomerController',
            scope: $scope,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            if (selectedItem == "OK") {
                $scope.fetchCustomerAllInfo();
            }
            //alert($scope.selected)
        }, function () {
            //  $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.fetchRegionReport = function () {
        CustomerProfileService.GetRegionReport($rootScope.selectedGroupId, $scope.selectedRegionPlatform.Name, moment($scope.selectedRegionDate.startDate).add(5, "hours").toISOString(),
            moment($scope.selectedRegionDate.endDate).add(5, "hours").toISOString(), 0, $scope.selectedRegionReport.Id, 0, $scope.isRegionEvolved, false, $scope.selectedRegionOtherOptions.Id).then(function (l) {
                if (l != null) {
                    $scope.RegionReportData = l.data;

                    if ($scope.selectedRegionReport.Id == 0 && $scope.isRegionEvolved == false) {
                        var regions = new Array();
                        var InbRnd = [];
                        var OutbRnd = [];
                        $scope.RegionReportData.forEach(function (i) {
                            regions.push(i.Region);

                            InbRnd.push(i.Total_Inbound);
                            OutbRnd.push(i.Total_Outbound);
                        });
                        var bigrnd = [{
                            "name": 'Inbound',
                            "data": InbRnd,
                            "type": "column",
                            "color": "#3A3B7A"
                        },
                        {
                            "name": 'Outbound',
                            "data": OutbRnd,
                            "type": "column",
                            "color": "#F57C4C"
                        }];

                        $scope.RegionChartConfig = $scope.getHighChartConfig(regions, bigrnd, false);
                    }
                    else if ($scope.selectedRegionReport.Id == 0 && $scope.isRegionEvolved) {
                        var regions = new Array();
                        var names = new Array();
                        $scope.RegionReportData.forEach(function (i) {
                            if (i.InsertionDate != undefined) {
                                var InsertionDate = i.InsertionDate.split('T')[0];
                                if (regions.indexOf(InsertionDate) == -1) {
                                    regions.push(InsertionDate);
                                }
                            }
                            else {
                                if (regions.indexOf(i.InsertionHour) == -1) {
                                    regions.push(i.InsertionHour);
                                }
                            }


                            if (names.indexOf(i.Region) == -1) {
                                names.push(i.Region);
                            }
                        });

                        var rnd = []
                        var bigrnd = [];
                        names.forEach(function (name) {
                            rnd = [];
                            regions.forEach(function (region) {
                                var dataSet = $.grep($scope.RegionReportData, function (record) {
                                    if (record.InsertionDate != undefined)
                                        return record.Region == name && record.InsertionDate.split('T')[0] == region;
                                    else
                                        return record.Region == name && record.InsertionHour == region;
                                });
                                if (dataSet.length == 0)
                                    rnd.push({
                                        y: (0)
                                    });
                                else
                                    rnd.push({
                                        y: (dataSet[0].Total_Inbound)
                                    });
                            });
                            var data = {
                                "name": name + "-Inbound",
                                "data": rnd,
                                "type": "line"
                            }
                            bigrnd.push(data);

                            rnd = [];
                            regions.forEach(function (region) {
                                var dataSet = $.grep($scope.RegionReportData, function (record) {
                                    if (record.InsertionDate != undefined)
                                        return record.Region == name && record.InsertionDate.split('T')[0] == region;
                                    else
                                        return record.Region == name && record.InsertionHour == region;
                                });
                                if (dataSet.length == 0)
                                    rnd.push({
                                        y: (0)
                                    });
                                else
                                    rnd.push({
                                        y: (dataSet[0].Total_Outbound)
                                    });
                            });

                            data = {
                                "name": name + "-Outbound",
                                "data": rnd,
                                "type": "line"
                            }
                            bigrnd.push(data);
                        });

                        $scope.RegionChartConfig = $scope.getHighChartConfig(regions, bigrnd, false);
                    }
                    else{
                        var regions = new Array();
                        var names = new Array();

                        $scope.RegionReportData.forEach(function (i) {
                            if (i.Name != "Unknown") {
                                if (regions.indexOf(i.Region) == -1) {
                                    regions.push(i.Region);
                                }

                                if (names.indexOf(i.Name) == -1) {
                                    names.push(i.Name);
                                }
                            }
                        });

                        var rnd = []
                        var bigrnd = [];
                        var colorCoding = [{ Name: "Positive", Value: "#007AFF" }, { Name: "Negative", Value: "#C00000" }, { Name: "Neutral", Value: "#FFC000" }];
                        names.forEach(function (name) {
                            rnd = [];
                            regions.forEach(function (region) {
                                var dataSet = $.grep($scope.RegionReportData, function (record) {
                                    return record.Name == name && record.Region == region;
                                });
                                if (dataSet.length == 0)
                                    rnd.push({
                                        y: (0)
                                    });
                                else
                                    rnd.push({
                                        y: (dataSet[0].Total_Inbound)
                                    });
                            });
                            
                            var data;
                            if ($scope.selectedRegionReport.Id == 1) {
                                var color = colorCoding.filter(function (el) {
                                    return el.Name == name
                                })[0].Value;
                                data = {
                                    "name": name,
                                    "data": rnd,
                                    "type": "column",
                                    "color": color
                                }
                            }
                            else {
                                data = {
                                    "name": name,
                                    "data": rnd,
                                    "type": "column"
                                }
                            }
                            bigrnd.push(data);
                        });

                        $scope.RegionChartConfig = $scope.getHighChartConfig(regions, bigrnd, false);
                    }
                }
            }, function (err) {
                GlobalService.ErrorPopup('Error in fetching, please try again.');
                console.log(err);
            });
    };

    $scope.fetchAreaReport = function () {
       // debugger;
        CustomerProfileService.GetRegionReport($rootScope.selectedGroupId, $scope.selectedAreaPlatform.Name, moment($scope.selectedAreaDate.startDate).add(5, "hours").toISOString(),
            moment($scope.selectedAreaDate.endDate).add(5, "hours").toISOString(), 1, $scope.selectedAreaReport.Id, $scope.selectedAreaRegion.Id, $scope.isAreaEvolved, false, $scope.selectedAreaOtherOptions.Id)
            .then(function (l) {
                if (l != null) {
                    $scope.AreaReportData = l.data;
                   // debugger;
                    if ($scope.selectedAreaReport.Id == 0 && $scope.isAreaEvolved == false) {
                        var regions = new Array();
                        var InbRnd = [];
                        var OutbRnd = [];
                        $scope.AreaReportData.forEach(function (i) {
                            regions.push(i.Region);

                            InbRnd.push(i.Total_Inbound);
                            OutbRnd.push(i.Total_Outbound);
                        });
                        var bigrnd = [{
                            "name": 'Inbound',
                            "data": InbRnd,
                            "type": "column",
                            "color": "#3A3B7A"
                        },
                        {
                            "name": 'Outbound',
                            "data": OutbRnd,
                            "type": "column",
                            "color": "#F57C4C"
                        }];

                        $scope.AreaChartConfig = $scope.getHighChartConfig(regions, bigrnd, true);
                    }
                    else if ($scope.selectedAreaReport.Id == 0 && $scope.isAreaEvolved) {
                        var regions = new Array();
                        var names = new Array();
                        $scope.AreaReportData.forEach(function (i) {
                            if (i.InsertionDate != undefined) {
                                var InsertionDate = i.InsertionDate.split('T')[0];
                                if (regions.indexOf(InsertionDate) == -1) {
                                    regions.push(InsertionDate);
                                }
                            }
                            else {
                                if (regions.indexOf(i.InsertionHour) == -1) {
                                    regions.push(i.InsertionHour);
                                }
                            }

                            if (names.indexOf(i.Region) == -1) {
                                names.push(i.Region);
                            }
                        });

                        var rnd = []
                        var bigrnd = [];
                        names.forEach(function (name) {
                            rnd = [];
                            regions.forEach(function (region) {
                                var dataSet = $.grep($scope.AreaReportData, function (record) {
                                    if (record.InsertionDate != undefined)
                                        return record.Region == name && record.InsertionDate.split('T')[0] == region;
                                    else
                                     return record.Region == name && record.InsertionHour == region;
                                });
                                if (dataSet.length == 0)
                                    rnd.push({
                                        y: (0)
                                    });
                                else if ($scope.AreaInOutEvolved == "Inbound")
                                    rnd.push({
                                        y: (dataSet[0].Total_Inbound)
                                    });
                                else
                                    rnd.push({
                                        y: (dataSet[0].Total_Outbound)
                                    });
                            });

                            var data = {
                                "name": name,
                                "data": rnd,
                                "type": "line"
                            }

                            bigrnd.push(data);
                        });

                        $scope.AreaChartConfig = $scope.getHighChartConfig(regions, bigrnd, true);
                    }
                    else {
                        var regions = new Array();
                        var names = new Array();
                        $scope.AreaReportData.forEach(function (i) {
                            if (i.Name != "Unknown") {
                                if (regions.indexOf(i.Region) == -1) {
                                    regions.push(i.Region);
                                }

                                if (names.indexOf(i.Name) == -1) {
                                    names.push(i.Name);
                                }
                            }
                        });

                        var rnd = []
                        var bigrnd = [];
                        var colorCoding = [{ Name: "Positive", Value: "#007AFF" }, { Name: "Negative", Value: "#C00000" }, { Name: "Neutral", Value: "#FFC000" }];
                        names.forEach(function (name) {
                            rnd = [];
                            regions.forEach(function (region) {
                                var dataSet = $.grep($scope.AreaReportData, function (record) {
                                    return record.Name == name && record.Region == region;
                                });
                                
                                if (dataSet.length == 0)
                                    rnd.push({
                                        y: (0)
                                    });
                                else
                                    rnd.push({
                                        y: (dataSet[0].Total_Inbound)
                                    });
                            });
                            var data;
                            if ($scope.selectedAreaReport.Id == 1) {
                                var color = colorCoding.filter(function(el){
                                    return el.Name == name
                                })[0].Value;
                                data = {
                                    "name": name,
                                    "data": rnd,
                                    "type": "column",
                                    "color": color
                                }
                            }
                            else {
                                data = {
                                    "name": name,
                                    "data": rnd,
                                    "type": "column"
                                }
                            }
                            bigrnd.push(data);
                        });

                        $scope.AreaChartConfig = $scope.getHighChartConfig(regions, bigrnd, true);
                    }
                }
            }, function (err) {
                GlobalService.ErrorPopup('Error in fetching, please try again.');
                console.log(err);
            });
    };

    $scope.getHighChartConfig = function (categories, series, isArea) {
        var title = "";
        var stacking = true;
        var stackLabelEnable = false;
        if (isArea) {
            if ($scope.selectedAreaReport.Id == 2){
                stackLabelEnable = false;
                stacking = true;
            }
            else {
                stacking = false;
                stackLabelEnable = true;
            }
            title = $scope.selectedAreaReport.Name;
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
       // debugger;
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

    $scope.init();
    $scope.reportInit();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
        $scope.reportInit();
    });

}]);