CRMApp.controller('_GroupsUserAddController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'GroupService', 'GlobalService','notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, GroupService, GlobalService, notify) {
    //alert($location.search().param);
    $scope.ErrorShow = false;
    $scope.Connected_Groups = [];
    $scope.TempData = [];
    $scope.visi = false;
    $scope.selected = {};
    $scope.GroupId = $location.search().groupid;
    $scope.CompanyId = $location.search().companyid;
    $scope.init = function () {
        $scope.slim = {
            height: '180px',
            disableFadeOut: true
        }
        $scope.example5customTexts = { buttonDefaultText: 'Please Select' };
        $scope.example5settings = {

        };
        $scope.rightsmodel = [];
        $scope.rightsdata = [];
        $scope.reportsmodel = [];
        $scope.reportsdata = [];
        $scope.groupsettings = {
            selectionLimit: 1,
            smartButtonMaxItems: 2,
            smartButtonTextConverter: function (itemText, originalItem) {
                return itemText;
            }
        }
        $scope.rolesdata = [{
                "id": 0,
                "label": "Normal User"
            },
            {
                "id": 1,
                "label": "Admin"
            }];
        $scope.rolesmodel = {};
        angular.copy($scope.rolesdata[0],$scope.rolesmodel);
        TeamService.GetDropDown().then(function (l) {
            if (l != null) {

                $.each(l.data.VisibleReports, function (key, value) {
                    $scope.reportsdata.push({ label: value, id: key });
                });
                $.each(l.data.Rights, function (key, value) {
                    $scope.rightsdata.push({ label: value, id: key });
                });
            }
            else {
                //alert('No Dropdown data found');
                GlobalService.ErrorPopup("Error", "No Dropdown data found, please try again.");

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");

        });
        ProfileService.GetAllProfilesByCompanyId($scope.CompanyId).then(function (l) {
            if (l.data != null) {
                $scope.Connected_Groups_Profiles = l.data;

                for (i = 0; i < $scope.Connected_Groups_Profiles.length; i++) {
                    var obj = {};
                    var GroupId = $scope.Connected_Groups_Profiles[i].GroupId;
                    var GroupName = $scope.Connected_Groups_Profiles[i].GroupName;
                    var isSelected = false;
                    var profileObj = [];
                    for (y = 0; y < $scope.Connected_Groups_Profiles[i].ProfileId.length; y++) {
                        var obj1 = {
                            'ProfileId': $scope.Connected_Groups_Profiles[i].ProfileId[y].ProfileId,
                            'ProfileType': $scope.Connected_Groups_Profiles[i].ProfileId[y].ProfileType,
                            'ProfileTypeId': $scope.Connected_Groups_Profiles[i].ProfileId[y].ProfileTypeId,
                            'ProfileName': $scope.Connected_Groups_Profiles[i].ProfileId[y].ProfileName,
                            'IsConnected': false,
                            'IsPage' :  $scope.Connected_Groups_Profiles[i].ProfileId[y].IsPage,
                            'PageName': $scope.Connected_Groups_Profiles[i].ProfileId[y].PageName,
                            'PageUrl': $scope.Connected_Groups_Profiles[i].ProfileId[y].PageUrl,
                            'ProfilePicURL': $scope.Connected_Groups_Profiles[i].ProfileId[y].ProfilePicURL,
                            'CompositeId': $scope.Connected_Groups_Profiles[i].ProfileId[y].ProfileTypeId + '' + $scope.Connected_Groups_Profiles[i].ProfileId[y].ProfileId,
                            'AccountName': $scope.Connected_Groups_Profiles[i].ProfileId[y].AccountName
                        }
                        profileObj.push(obj1);
                    }
                    obj = {
                        'GroupId': GroupId,
                        'GroupName': GroupName,
                        'isSelected': isSelected,
                        'profileObj': profileObj
                    }
                    $scope.TempData.push(obj);
                }
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");

           
        });
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.instaIcon = '/Content/images/insta_ico16.png';

    }
    $scope.init();

    $scope.getGroupIndex = function (GroupId)//group id is passed here and profile are fetched of the selected groups
    {

        for (i = 0; i < $scope.TempData.length; i++) {
            if (GroupId === $scope.TempData[i].GroupId) {

                $scope.Connected_Profiles = $scope.TempData[i].ProfileId;
                $scope.index = i;

                break;
            }
            else {
                $scope.visi = false;
            }
        }
        if ($('#' + GroupId + 'group').is(':checked')) {
            $scope.visi = true;
            $scope.TempData[$scope.index].isSelected = true;
        }
        else {
            $scope.visi = false;
            $scope.TempData[$scope.index].isSelected = false;
        }
        //if ($('#' + GroupId + 'group').is(':checked'))
        //    {
        //ProfileService.GetByGroupId(GroupId).then(function (l) {
        //    if (l != null) {
        //        $scope.Connected_Profiles = l.data
        //    }
        //    else {
        //        alert('Error in fetching data, please try again.');
        //    }
        //}, function (err) {
        //    alert('Error in fetching data, please try again.');
        //    console.log(err);
        //});
        //}
        //else {
        //    $scope.Connected_Profiles = {};
        //}
        ////alert(GroupId);
    }



    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.CreateGroup = function () {

        var profiledata = [];
        var AssignedRights = [];
        var AssignedReports = [];
        var AssignedGroups = [];
        var countGroupWithProfile = 0;
        var countGroupWithoutProfile = 0;
        var atleastOneGroup = false;
        var groupPresent = false;
        var profilePresent = false;

        $.each($scope.TempData, function (key, value) {
            if (value.isSelected) {
                AssignedGroups.push(value.GroupId);
                groupPresent = true;
                $.each(value.profileObj, function (key, valueInner) {
                    if (valueInner.IsConnected) {
                        profilePresent = true;
                        var ProfileTypeId = valueInner.IsPage ? 5 : valueInner.ProfileTypeId;
                        profiledata.push({
                            profileId: valueInner.ProfileId, ProfileType: ProfileTypeId, GroupId: value.GroupId
                        });
                    }
                });
                if(groupPresent==true && profilePresent==true)
                {
                    countGroupWithProfile++;
                    groupPresent = false;
                    profilePresent = false;

                }
                else {
                    countGroupWithoutProfile++;
                    groupPresent = false;
                    profilePresent = false;
                }
            }
        });
        
        $.each($scope.rightsmodel, function (key, value) {
            AssignedRights.push(value.id);
        });
        $.each($scope.reportsmodel, function (key, value) {
            AssignedReports.push(value.id);
        });
        var teamdata = {
            FirstName: $scope.FirstName,
            LastName: $scope.LastName,
            Email: $scope.Email,
            CompanyId: $scope.CompanyId,
            AssignedGroups: AssignedGroups,
            AssignedRights: AssignedRights,
            AssignedReports: AssignedReports,
            AssignedProfiles: profiledata,
            IsAdmin: $scope.rolesmodel.id
        };

        var isError = false;
        $scope.ErrorMessage = '';
        if (countGroupWithoutProfile > 0)
        {
            isError = true;
            $scope.ErrorMessage = $scope.ErrorMessage + 'o All selected groups should have atleast one profile selected with them ';
        }
        if (countGroupWithoutProfile == 0 && countGroupWithProfile == 0)
        {
            isError = true;
            $scope.ErrorMessage = $scope.ErrorMessage + '\n\r o There should be atleast one group selected, with a selection of minimum of one profile ';
        }
        if (AssignedRights.length == 0)
        {
            isError = true;
            $scope.ErrorMessage = $scope.ErrorMessage + '\n\r o There should be atleast one right assigned to user ';
        }
        if (AssignedReports.length == 0)
        {
            isError = true;
            $scope.ErrorMessage = $scope.ErrorMessage + '\n\r o There should be atleast one report assigned to user ';
        }

        if (isError == true && $scope.rolesmodel.id == 0)
        {
            $scope.ErrorShow = true;
        }
        else
        {
            $scope.ErrorShow = false;
            TeamService.Register(teamdata).then(function (l) {
           // alert("Team Member Added Successfully");
            notify({
                message: "Team Member Added Successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $rootScope.$broadcast('Team_Reload', {CompanyId: $scope.CompanyId, GroupId: $scope.GroupId});
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);
        });
        }

    }
}]);