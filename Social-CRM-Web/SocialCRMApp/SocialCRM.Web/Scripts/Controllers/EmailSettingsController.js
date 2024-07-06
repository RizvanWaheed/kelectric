CRMApp.controller('EmailSettingsController', ['$rootScope', '$scope', '$sce', '$window', '$location', 'UnifiedService', 'UnifiedDeckService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', 'GeneralSettingsService', function ($rootScope, $scope, $sce, $window, $location, UnifiedService, UnifiedDeckService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify, GeneralSettingsService) {

    $scope.UserTypesdata = [];
    $scope.UserTypesmodel = [];
    $scope.userId = 0;
    $scope.userEmail = "";
    $scope.userName = "";    
    $scope.rightsToReceiveReports = {
        value: false
    };
    $scope.socialMediaUsersUpdateReceivers = [];    

    $scope.AddEmail = function () {        
        var selectedUserTypeIds = [];
        for (i = 0; i < $scope.UserTypesmodel.length; i++) {
            selectedUserTypeIds.push($scope.UserTypesmodel[i].id); //only selected ids
        }
        
        //Commented As per client requirement
        //if (selectedUserTypeIds.length == 0) {
        //    GlobalService.ErrorPopup("Alert", "Please select atleast 1 user type");
        //    return;
        //}

        console.log(selectedUserTypeIds);
        console.log("Rights:" + $scope.rightsToReceiveReports.value);

        var over10Kfollowers = false;
        var politicalFigures = false;
        var verifiedAccount = false;
        var topInfluencer = false;
        for (i = 0; i < selectedUserTypeIds.length; i++) {
            if (selectedUserTypeIds[i] == 1) {
                over10Kfollowers = true;
            } else if (selectedUserTypeIds[i] == 2) {
                politicalFigures = true;
            } else if (selectedUserTypeIds[i] == 3) {
                verifiedAccount = true;
            } else if (selectedUserTypeIds[i] == 4) {
                topInfluencer = true;
            }
        }

        GeneralSettingsService.AddUpdateEmailSettings($scope.userId, $scope.userName, $scope.userEmail, over10Kfollowers, politicalFigures, verifiedAccount, topInfluencer, $scope.rightsToReceiveReports.value, $rootScope.selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                notify({
                    message: "Email settings user added successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                //Reset
                $scope.userId = 0;
                $scope.getSocialMediaUsersUpdateReceivers();
                ClearAllFields();
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });

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

    $scope.getSocialMediaUsersUpdateReceivers = function () {
        GeneralSettingsService.GetSocialMediaUsersUpdateReceiversList().then(function (result) {
            if (result.statusText == "OK") {
                $scope.socialMediaUsersUpdateReceivers = result.data;
                console.log($scope.socialMediaUsersUpdateReceivers);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    function ClearAllFields() {
        $scope.UserTypesmodel = [];
        $scope.userEmail = "";
        $scope.userName = "";
        $scope.rightsToReceiveReports = {
            value: false
        };
    }

    $scope.actionBtnClick = function (btnName, updateReceiverObj) {
        debugger
        if (btnName == "Edit") {
            $('#AddUpdateEmail').html("Update Email");
            $scope.userEmail = updateReceiverObj.Email;
            $scope.userName = updateReceiverObj.Name;
            $scope.userId = updateReceiverObj.ID;
            $scope.rightsToReceiveReports.value = updateReceiverObj.ReceiveReport;

            for (i = 0; i < $scope.UserTypesdata.length; i++) {
                if ($scope.UserTypesdata[i].id == 1 && updateReceiverObj.Over10KFollowers == true) {
                    $scope.UserTypesmodel.push($scope.UserTypesdata[i]);
                } else if ($scope.UserTypesdata[i].id == 2 && updateReceiverObj.PoliticalFigures == true) {
                    $scope.UserTypesmodel.push($scope.UserTypesdata[i]);
                } else if ($scope.UserTypesdata[i].id == 3 && updateReceiverObj.VerifiedAccounts == true) {
                    $scope.UserTypesmodel.push($scope.UserTypesdata[i]);
                } else if ($scope.UserTypesdata[i].id == 4 && updateReceiverObj.TopInfluencers == true) {
                    $scope.UserTypesmodel.push($scope.UserTypesdata[i]);
                }

            }

        } else if (btnName == "Delete") {

            GeneralSettingsService.DeleteEmailSettings(updateReceiverObj.ID).then(function (result) {
                if (result.statusText == "OK") {
                    notify({
                        message: "Email settings user deleted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.getSocialMediaUsersUpdateReceivers();
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                console.log(err);
            });
        }
        else if (btnName == "Cancel") {
            $('#AddUpdateEmail').html("Add Email");
            $scope.userId = 0;
            ClearAllFields();
        }
    }

    $scope.init = function () {
        $scope.getSocialMediaUserTypes();
        $scope.getSocialMediaUsersUpdateReceivers();
    }

    $scope.init();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
}]);