CRMApp.controller('_AddWarnBanUserController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'CustomerProfileService', '$uibModalInstance', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $stateParams, CustomerProfileService, $uibModalInstance, items, GlobalService, notify) {
       

    $scope.initModel = function () {
        $scope.UserObj = items.UserObj;
        $scope.Type = items.Type;
        $scope.ProfileUrl = "";
        $scope.Username = "";
        $scope.BannedBy = "";
        $scope.Details = "";

        $scope.ReasonModel = {};
        $scope.PlatformModel = {};
        $scope.ProfileModel = {};
        $scope.StatusModel = {};

        $scope.ReasonList = [];//items.ReasonList;
        $scope.PlatformList = [];// items.PlatformList;
        $scope.ProfileListAll = [];// items.ProfileList;
        $scope.ProfileList = [];
        $scope.StatusList = [];// items.StatusList;

        $scope.ProfileUrl = "";
        $scope.Username = "";
        $scope.BannedBy = "";
        $scope.Details = "";
        $scope.errorMsg = "";
        $scope.Id = $scope.UserObj == null ? 0 : $scope.UserObj.Id;
        $scope.base64String = "";
        $scope.fileName = "";
        $scope.contentType = "";

        

        CustomerProfileService.GetBannedProfilesOptions($rootScope.selectedGroupId).then(function (l) {
            var response = l.data;

            var reasons = response.ReasonList;
            var platform = response.PlatformList;
            var status = response.StatusList;

            $scope.ProfileListAll = response.ProfilesList;

            $.each(reasons, function (key, value) {
                $scope.ReasonList.push({ label: value.Text, id: value.Value });
            });

            $.each(platform, function (key, value) {
                $scope.PlatformList.push({ label: value.Text, id: value.Value });
            });

            $.each(status, function (key, value) {
                $scope.StatusList.push({ label: value.Text, id: value.Value });
            });

            if ($scope.Type == "Add") {
                $scope.PlatformList = $.grep($scope.PlatformList, function (a) {
                    return a.label == "Twitter";
                });
            }

            if ($scope.UserObj != null) {
                $scope.ProfileUrl = $scope.UserObj.ProfileUrl;
                $scope.Username = $scope.UserObj.Name;
                $scope.BannedBy = $scope.UserObj.BannedBy;
                $scope.Details = $scope.UserObj.Details;

                var selectedItem = [];

                selectedItem = $.grep($scope.ReasonList, function (a) {
                    return a.id == $scope.UserObj.ReasonId;
                })[0];
                if (selectedItem)
                    $scope.ReasonModel = { "label": selectedItem.label, "id":  selectedItem.id }

                selectedItem = $.grep($scope.PlatformList, function (a) {
                    return a.id == $scope.UserObj.PlatformId;
                })[0];
                if (selectedItem)
                    $scope.PlatformModel = { "label": selectedItem.label, "id":  selectedItem.id }

                selectedItem = $.grep($scope.StatusList, function (a) {
                    return a.id == $scope.UserObj.StatusId;
                })[0];

                if (selectedItem)
                    $scope.StatusModel = { "label": selectedItem.label, "id":  selectedItem.id }

                var property = new Object();
                property.id = $scope.UserObj.PlatformId;
                onItemSelectPlatform(property);

                selectedItem = $.grep($scope.ProfileListAll, function (a) {
                    return a.Value == $scope.UserObj.ProfileId;
                })[0];
                if (selectedItem)
                    $scope.ProfileModel = { "label": selectedItem.Text, "id": selectedItem.Value }

            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    

    $scope.platformEventListeners = {
        onItemSelect: onItemSelectPlatform
    };

    function onItemSelectPlatform(property) {
        var platformId = property.id;

        var profileList = $.grep($scope.ProfileListAll, function (a) {
            return a.ProfileType == platformId;
        });

        $scope.ProfileList = new Array();

        $.each(profileList, function (key, value) {
            $scope.ProfileList.push({ "label": value.Text, "id": value.Value });
        });

        if ($scope.Username != "")
            $scope.ProfileUrl = (platformId == 1 ? "https://www.facebook.com/" : "https://www.twitter.com/") + $scope.Username;
    }

    $scope.example5customTexts = { buttonDefaultText: 'Please Select' };

    $scope.selectSettings = {
        selectionLimit: 1,
        smartButtonMaxItems: 2,
        smartButtonTextConverter: function (itemText, originalItem) {
            return itemText;
        }
    }

    $scope.SubmitUser = function () {
        var allowedExtentions = ["png", "jpg", "jpeg"];
        if ($scope.ProfileUrl == "") {
            $scope.errorMsg = "Please enter Profile Url";
            return;
        }

        if ($scope.Username == "") {
            $scope.errorMsg = "Please enter Username";
            return;
        }
        
        if ($scope.ReasonModel.id === undefined) {
            $scope.errorMsg = "Please select a Reason";
            return;
        }

        if ($scope.PlatformModel.id === undefined) {
            $scope.errorMsg = "Please select a Platform";
            return;
        }

        if ($scope.ProfileModel.id === undefined) {
            $scope.errorMsg = "Please select a Profile";
            return;
        }

        if ($scope.StatusModel.id === undefined) {
            $scope.errorMsg = "Please select a status";
            return;
        }

        if ($scope.Details == "") {
            $scope.errorMsg = "Please enter Details";
            return;
        }

        if ($scope.Type == "Ban") {
            CustomerProfileService.BanUnbanUser($scope.Id, "ban").then(function (l) {
                var response = l.data;
                if (response == "success") {
                    var file = $("input[type='file']").get(0).files[0];
                    if (file) {
                        var uploadedFilesExtentions = file.type.split('/')[1];
                        if (!allowedExtentions.includes(uploadedFilesExtentions)) {
                            $scope.errorMsg = "Only images are allowed";
                            return;
                        }
                        $scope.fileName = file.name;
                        $scope.contentType = file.type;
                        $scope.convertToBase64StringAndSend(file);
                    } else {
                        $scope.Submit();
                    }
                    //$scope.init();
                }
                else {
                    GlobalService.ErrorPopup(action + " Failed", response);
                }

            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.Message);
            });
        }
        else {
            if ($scope.Id == 0) {
            var file = $("input[type='file']").get(0).files[0];

            if (file) {
                var uploadedFilesExtentions = file.type.split('/')[1];
                if (!allowedExtentions.includes(uploadedFilesExtentions)) {
                    $scope.errorMsg = "Only images are allowed";
                    return;
                }
            } else {
                $scope.errorMsg = "Please upload an attachment";
                return;
            }

            $scope.fileName = file.name;
            $scope.contentType = file.type;
            $scope.convertToBase64StringAndSend(file);
        }
        else {
            var file = $("input[type='file']").get(0).files[0];
            if (file) {
                var uploadedFilesExtentions = file.type.split('/')[1];
                if (!allowedExtentions.includes(uploadedFilesExtentions)) {
                    $scope.errorMsg = "Only images are allowed";
                    return;
                }
                $scope.fileName = file.name;
                $scope.contentType = file.type;
                $scope.convertToBase64StringAndSend(file);
            } else {
                $scope.Submit();
            }
        }
        }

        
    };

    $scope.convertToBase64StringAndSend = function (file) {
        var reader = new FileReader();
        reader.addEventListener("loadend", function () {
            console.log(reader.result.length);
            $scope.base64String = reader.result;
            $scope.Submit();
           
        }, false);

        if (file) {
            reader.readAsDataURL(file);
        }
    };

    $scope.Submit = function () {
        var reason = parseInt($scope.ReasonModel.id);
        var platform = parseInt($scope.PlatformModel.id);
        var profile = platform == 4 ? parseInt($scope.ProfileModel.id) : 0;
        var page = platform == 1 ? parseInt($scope.ProfileModel.id) : 0;
        var status = $scope.StatusModel.id;

        CustomerProfileService.SaveBannedProfiles($scope.Id, $scope.Username, $scope.ProfileUrl, reason, platform, $scope.Details, profile, page, $scope.base64String, $scope.contentType, $scope.fileName, status, $rootScope.selectedCompanyId).then(function (l) {
            var response = l.data;
            if (response.indexOf("success") > -1) {
                notify({
                    message: "User saved Successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                var id = response.split('|')[1];
                $uibModalInstance.close(id);
                //$scope.init();
            }
            else {
                GlobalService.ErrorPopup("Save Failed", response);
            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    $scope.initModel();

    $scope.PopulateUrl = function () { 
        var platform = parseInt($scope.PlatformModel.id);
        $scope.ProfileUrl = (platform == 1 ? "https://www.facebook.com/" : "https://www.twitter.com/") + $scope.Username;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

}]);