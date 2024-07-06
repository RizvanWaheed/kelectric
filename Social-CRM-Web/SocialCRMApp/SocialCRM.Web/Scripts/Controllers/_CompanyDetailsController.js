CRMApp.controller('_CompanyDetailsController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'GlobalService', 'notify', '$stateParams', 'AdminService', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, GlobalService, notify, $stateParams, AdminService) {
    $scope.CompanyData = items.CompanyData;
    
    $scope.GroupsCount = items.CompanyData.GroupCount;
    $scope.TeamMembersCount = items.CompanyData.TeamMemberCount;
    $scope.ProfilesCount = items.CompanyData.ProfileList.length;
    $scope.selected = {};    
    $scope.select = true;
    $scope.selectedProfileList = items.CompanyData.ProfileList;
    $scope.selectedTeamList = items.CompanyData.TeamList;

    $scope.selectAllProfile = true;
    $scope.SelectFbProfiles = true;
    $scope.SelectTwProfiles = true;
    $scope.SelectInstaProfiles = true;

    $scope.checkAll = function () {
        angular.forEach($scope.CompanyData.GroupList , function (obj) {
            $scope.selected[obj.GroupId] = $scope.select;
        });
        $scope.UpdateTeamMembersAndProfiles();        
    };

    $scope.commentsLength = {
        height: '800px',//350
        disableFadeOut: true        
    }

    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';

    $scope.changeCompanyStatus = function (CompanyId, Status) {
        AdminService.ChangeCompanyStatus(CompanyId, Status).then(function (l) {            
            console.log(l);
            if (Status == 0) {
                notify({
                    message: "Company deactivated successfully.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }
            else {
                notify({
                    message: "Company activated successfully.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }
            $scope.cancel();
            $scope.GetCompaniesDetail();
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            console.log(err);
        });
    }    

    $scope.changeProfileTypes = function () {
        debugger
        $scope.CompanyData = items.CompanyData;
        $scope.UpdateTeamMembersAndProfiles();

        var fbProfiles = [];
        var twProfiles = [];
        var instaProfiles = [];

        angular.forEach($scope.selectedProfileList, function (Profile) {
            if (Profile.ProfileType == "Twitter") {
                twProfiles.push(Profile);
            } else if (Profile.ProfileType == "Facebook") {
                fbProfiles.push(Profile);
            } else if (Profile.ProfileType == "Instagram") {
                instaProfiles.push(Profile);
            }
        });

        $scope.selectedProfileList = [];
        if ($scope.SelectFbProfiles == true) {
            angular.forEach(fbProfiles, function (Profile) {
                $scope.selectedProfileList.push(Profile);
            });            
        }
        if ($scope.SelectTwProfiles == true) {
            angular.forEach(twProfiles, function (Profile) {
                $scope.selectedProfileList.push(Profile);
            });            
        }
        if ($scope.SelectInstaProfiles == true) {
            angular.forEach(instaProfiles, function (Profile) {
                $scope.selectedProfileList.push(Profile);
            });            
        }

        $scope.ProfilesCount = $scope.selectedProfileList.length;

    }

    $scope.init = function () {
        $scope.FormText = $scope.CompanyData.CompannyName + " Details";             
        $scope.checkAll();
    };    

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.UpdateTeamMembersAndProfiles = function () {
        var selectedGroups = $.grep($scope.CompanyData.GroupList, function (record) {
            return $scope.selected[record.GroupId];
        });

        $scope.selectedProfileList = [];
        $scope.selectedTeamList = [];

        //console.log(selectedGroups);
        var found = false;
        angular.forEach($scope.CompanyData.ProfileList, function (Profile) {
            found = false;

            angular.forEach(selectedGroups, function (Group) {
                if (Group.GroupId == Profile.GroupId) {
                    found = true;
                }
            });

            angular.forEach($scope.selectedProfileList, function (selProfile) {
                if (selProfile.ProfileId == Profile.ProfileId) {
                    found = false;
                }
            });

            if (found == true) {               
                $scope.selectedProfileList.push(Profile);
            }
        });

        angular.forEach($scope.CompanyData.TeamList, function (TeamMember) {
            found = false;

            angular.forEach(selectedGroups, function (Group) {
                if (Group.GroupId == TeamMember.GroupId) {
                    found = true;
                }
            });

            angular.forEach($scope.selectedTeamList, function (selTeamMem) {
                if (selTeamMem.TeamId == TeamMember.TeamId) {
                    found = false;
                }
            });

            if (found == true) {
                $scope.selectedTeamList.push(TeamMember);
            }
        });

        $scope.ProfilesCount = $scope.selectedProfileList.length;
        $scope.TeamMembersCount = $scope.selectedTeamList.length;
    }

    $scope.groupSelectedChanged = function () {
        $scope.select = false;
        $scope.UpdateTeamMembersAndProfiles();
    }

    $scope.checkAllProfileTypes = function () {
        $scope.SelectFbProfiles = $scope.selectAllProfile;
        $scope.SelectTwProfiles = $scope.selectAllProfile;
        $scope.SelectInstaProfiles = $scope.selectAllProfile;
        $scope.changeProfileTypes();
    }

    function containsObject(obj, list) {
        var i;
        for (i = 0; i < list.length; i++) {
            if (list[i].ProfileId === obj) {
                return true;
            }
        }

        return false;
    }

    $rootScope.$on('Session_Fetched_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.init();
}]);