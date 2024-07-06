CRMApp.controller('TopInfluencersSettingController', ['$rootScope', '$scope', '$sce', '$window', '$location', 'UnifiedService', 'UnifiedDeckService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', 'GeneralSettingsService', function ($rootScope, $scope, $sce, $window, $location, UnifiedService, UnifiedDeckService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify, GeneralSettingsService) {
    
    $scope.socialMediaURL = '';
    $scope.UserTypesdata = [];
    $scope.UserTypesmodel = [];
    $scope.connectedProfiles = [];
    $scope.connectedTwitterProfiles = [];
    $scope.selectedTwitterProfile = [];
    $scope.pageNumber = 0;
    $scope.InfluencersList = [];
    $scope.errorMsg = "";
    $scope.Id = 0;
    $scope.ProceedToNext = false;
    $scope.selectedPlatform = "Choose Platform";
    $scope.disablePlatform = false;

    $scope.selectsettings = {
        selectionLimit: 1,
        smartButtonMaxItems: 2,
        smartButtonTextConverter: function (itemText, originalItem) {            
            return itemText;
        },
        template: '<b>{{option.name}}</b>'
    }

    //Group Change
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.select = true;
        $scope.init();
    });

    $scope.platformChanged = function (platform) {
        if (platform == 4)
        {
            $scope.selectedPlatform = 'Twitter';
            $scope.connectedTwitterProfiles = [];
            angular.forEach($scope.connectedProfiles, function (obj) {
                if (obj.type == 4) {
                    $scope.connectedTwitterProfiles.push({
                        id: obj.id,
                        label: obj.label
                    });
                }
            });
        }
        else if (platform == 1)
        {
            $scope.selectedPlatform = 'Facebook';
            $scope.connectedTwitterProfiles = [];
            angular.forEach($scope.connectedProfiles, function (obj) {
                if (obj.type == 1) {
                    $scope.connectedTwitterProfiles.push({
                        id: obj.id,
                        label: obj.label
                    });
                }
            });
        }        
        
    }

    $scope.getSocialMediaUserTypes = function () {
        GeneralSettingsService.GetSocialMediaUserTypes().then(function (result) {
            if (result.statusText == "OK") {
                $scope.UserTypesdata = [];
                angular.forEach(result.data, function (obj) {
                    $scope.UserTypesdata.push({ label: obj.Name, id: obj.Id });
                });                
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.getInfluencerList = function (getOptions) {
        $scope.InfluencersList = [];
        GeneralSettingsService.GetInfluencersList($rootScope.selectedGroupId, $scope.pageNumber, getOptions).then(function (result) {
            if (result.statusText == "OK") {
                $scope.InfluencersList = result.data.UserList;
                $scope.ProceedToNext = result.data.UserList.length == 10;
                if (getOptions) {
                    $scope.UserTypesdata = [];
                    $scope.connectedProfiles = [];

                    angular.forEach(result.data.AccountTypeList, function (obj) {
                        $scope.UserTypesdata.push({ label: obj.Text, id: obj.Value });
                    });
                    //Removing the options for the other two scenarios cause we don't them
                    $scope.UserTypesdata = $scope.UserTypesdata.filter(function (el) {
                        return el.id !== 1 && el.id !== 3;
                    });
                    
                    angular.forEach(result.data.ProfilesList, function (obj) {
                        $scope.connectedProfiles.push({
                            id: obj.ProfileType + "_" + obj.Value,
                            label: obj.Text,
                            type: obj.ProfileType
                        });
                    });

                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.saveInfluencer = function () {
        if ($scope.socialMediaURL == "") {
            $scope.errorMsg = "Please Enter Customer Handle";
            return;
        }

        if ($scope.UserTypesmodel.id === undefined) {
            $scope.errorMsg = "Please select an Account Type";
            return;
        }

        if ($scope.selectedTwitterProfile.id === undefined) {
            $scope.errorMsg = "Please select a Profile";
            return;
        }

        var platformAndId = $scope.selectedTwitterProfile.id.split("_");
        var platform = platformAndId[0];
        var profileId = platformAndId[1];

        GeneralSettingsService.SaveInfluencer($scope.Id, $scope.socialMediaURL, profileId, $scope.UserTypesmodel.id, platform).then(function (result) {
            if (result.statusText == "OK" && result.data.split('|')[0] == "success") {
                notify({
                    message: "User saved Successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $scope.getInfluencerList(false);
            } else {
                GlobalService.ErrorPopup("Error", result.data);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    };

    $scope.cancel = function () {
        $scope.Id = 0;
        $scope.socialMediaURL = "";
        $scope.UserTypesmodel = [];
        $scope.selectedTwitterProfile = [];
        $scope.disablePlatform = false;
    };

    // Fetching Group Members profile
    //$scope.GetProfilesByGroupId = function () {
    //    debugger;
    //    UnifiedService.GetAllProfilesForUserGroup($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {            
    //        var compositeId = "";
    //        angular.forEach(l.data, function (obj) {                
    //            if (obj.ProfileTypeId == '4') //Twitter
    //            {
    //                $scope.connectedTwitterProfiles.push({
    //                    id: obj.ProfileCompnayGroupId,
    //                    label: obj.AccountName
    //                });
    //            }                
    //        });
    //    }, function (err) {
    //        GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
    //        console.log(err);
    //    });
    //};

    $scope.editInfluencer = function (id) {
        var UserObj = $.grep($scope.InfluencersList, function (record) {
            return record.Id == id
        })[0];

        if (UserObj) {
            $scope.Id = UserObj.Id
            $scope.socialMediaURL = UserObj.CustomerName;

            var selectedItem = [];

            selectedItem = $.grep($scope.UserTypesdata, function (a) {
                return a.id == UserObj.AccountTypeId;
            })[0];
            if (selectedItem)
                $scope.UserTypesmodel = { "label": selectedItem.label, "id": selectedItem.id }

            debugger;
            $scope.platformChanged(UserObj.PlatformId);
            $scope.disablePlatform = true;

            selectedItem = $.grep($scope.connectedProfiles, function (a) {
                return a.id == UserObj.PlatformId + "_" + UserObj.ProfileId;
            })[0];
            if (selectedItem)
                $scope.selectedTwitterProfile = { "label": selectedItem.label, "id": selectedItem.id }
        }
        
    };

    $scope.deleteInfluencer = function (id) {
        GlobalService.ConfirmPopup("Confirm", "Are you sure you want to delete?", function (selectedItem) {
            if (selectedItem) {
                GeneralSettingsService.DeleteInfluencer(id).then(function (l) {
                var response = l.data;
                if (response == "success") {
                    notify({
                        message: "User deleted Successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.getInfluencerList(true);
                }
                else {
                    GlobalService.ErrorPopup("Delete failed", response);
                }

            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
            }); 
            }
        });        
    }

    $scope.goToNextPage = function () {
        if ($scope.ProceedToNext) {
            $scope.pageNumber++;
            $scope.getInfluencerList(false);
        }
    }

    $scope.goToPreviousPage = function () {
        if ($scope.pageNumber > 0) {
            $scope.pageNumber--;
            $scope.getInfluencerList(false);
        }
    }

    $scope.init = function () {
        $scope.selectedPlatform = "Choose Platform";
        $scope.connectedProfiles = [];
        $scope.connectedTwitterProfiles = [];

        $scope.getInfluencerList(true);
    }

    $scope.init();
}])    