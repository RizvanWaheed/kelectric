CRMApp.controller('GroupAndTeamController', ['$rootScope', '$scope', '$window', '$stateParams', '$location', '$uibModal', 'GroupService', 'ProfileService', 'AuthService', '$compile', 'TeamService', '$log', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $stateParams, $location, $uibModal, GroupService, ProfileService, AuthService, $compile, TeamService, $log, GlobalService, notify) {
    $scope.userGroupDel = false;
    $scope.myEventListeners = {
        onItemSelect: onItemSelect
    };

    // MultiSelect Drop down select - Event
    function onItemSelect(property) {
        arr = jQuery.grep($scope.groupdata, function (a) {
            return a.id == property.id;
        });
        $scope.selectedGroupId = arr[0].id;
        $scope.selectedGroupName = arr[0].label;

        GetUserProfile($scope.selectedGroupId);
        console.log('select > ' + JSON.stringify(arr));
    }

    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';
    $scope.instaIcon = '/Content/images/insta_ico16.png';
    $scope.outlookIcon = '/Content/images/outlook_ico16.png';

    $scope.isGroup = true;

    $scope.groupmodel = {};
    $scope.groupdata = [];
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

    $scope.reportsmodel = [];

    $scope.reportsdata = [];

    $scope.rightsmodel = [];

    $scope.rightsdata = [];

    $scope.init = function () {
        $scope.CompanyName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.CompanyId = $rootScope.selectedCompanyId;
        $scope.LoggedInTeamMemberId = jQuery.parseJSON($rootScope.currentUser).TeamMemberId;
        
        //Get Team Members Data
        $scope.getTeamMembers($scope.CompanyId);
        //Populate Groups section
        $scope.GetGroups();

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
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching dropdown Data, please try again.");
            console.log(err);
        });

        $scope.example5customTexts = { buttonDefaultText: 'Please Select' };
        $scope.groupProfiles = [];
    }

    $scope.UpdateSelectedMember = function (IsSelectedTeamMemberActive) {
        var memberUpdated = $scope.groupMembers.filter(function (el) {
            return el.TeamMemberId == $scope.SelectedTeamMemberId;
        });
        memberUpdated[0].IsActive = IsSelectedTeamMemberActive;
    }

    $scope.RemoveMemberFromCompany = function () {
        if ($scope.IsSelectedTeamMemberOwner == true) {
            GlobalService.ErrorPopup("Error", "Owner of Company cannot be deactivated!");
        }
        else{
            TeamService.RemoveMemberFromCompany($scope.SelectedTeamMemberId, $scope.CompanyId).then(function (l) {
                notify({
                    message: "Team member deactivated successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $scope.IsSelectedTeamMemberActive = false;
                $scope.UpdateSelectedMember($scope.IsSelectedTeamMemberActive);
            }, function (err) {
                //alert('Error in fetching pages data');

                GlobalService.ErrorPopup("Error", "Error in deactivating team member, please try again.");
                console.log(err);
            });
        }
    }

    $scope.AddMemberToCompany = function () {
        TeamService.AddMemberToCompany($scope.SelectedTeamMemberId, $scope.CompanyId).then(function (l) {
            notify({
                message: "Team member activated successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $scope.IsSelectedTeamMemberActive = true;
            $scope.UpdateSelectedMember($scope.IsSelectedTeamMemberActive);
        }, function (err) {
            //alert('Error in fetching pages data');

            GlobalService.ErrorPopup("Error", "Error in activating team member, please try again.");
            console.log(err);
        });
    }

    $scope.getTeamMembers = function (companyId) {
        TeamService.GetByCompanyId(companyId).then(function (l) {
            var response = l.data;
            $scope.groupMembers = response.Team;
            // alert(JSON.stringify($scope.groupMembers));
            $scope.TeamMembers = response.TotalMember;
            $scope.TotalGroups = response.TotalGroup;
            $scope.TotalProfiles = response.TotalProfile;
            $scope.CompanyName = response.CompanyName;
            var firstMem = $scope.groupMembers[0];
            $scope.loadUserData(companyId, firstMem.TeamMemberId);


        }, function (err) {
            //alert('Error in fetching pages data');

            GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            console.log(err);
        });
    }

    $scope.loadUserData = function (companyId, teamMemberId) {
        // companyId = 1;//get company id at the time of login

        if (teamMemberId != $scope.LoggedInTeamMemberId) {
            ProfileService.GetAllProfilesByCompanyAndUserId(companyId, teamMemberId).then(function (l) {
                var response = l.data[0];
                //alert(JSON.stringify(response.Team))
                if (response.Team != null) {
                    // alert(JSON.stringify(response.Team))
                    $scope.SelectedUserName = response.Team.Profile.Name;
                    $scope.SelectedUserPic = response.Team.Profile.ProfileUrl == null && "/Content/img/user128.jpg" || $scope.replaceTilda(response.Team.Profile.ProfileUrl);
                    $scope.SelectedTeamMemberId = response.Team.Profile.TeamMemberId;
                    $scope.SelectedTeamMemberFirstName = response.Team.Profile.FirstName;
                    $scope.SelectedTeamMemberLastName = response.Team.Profile.LastName;
                    $scope.ProfilesAssignedToUser = response.Team.Profile.AssignedProfile;
                    $scope.IsSelectedTeamMemberActive = response.Team.Profile.IsActive;
                    $scope.IsSelectedTeamMemberOwner = response.Team.Profile.IsOwner;
                    $scope.IsSelectedTeamMemberAdmin = response.Team.Profile.IsAdmin;
                    $scope.UserGroups = [];
                    $scope.UserVisibleReports = [];
                    $scope.UserRights = [];

                    for (i = 0; i < response.Team.VisibleReports.length; i++) {
                        //if (response.Team.VisibleReports[i].Id != 6) {
                            var obj = { "label": response.Team.VisibleReports[i].Name, "id": "" + response.Team.VisibleReports[i].Id }
                            $scope.UserVisibleReports.push(obj);
                        //}
                    }
                    for (i = 0; i < response.Team.Rights.length; i++) {
                        var obj = { "label": response.Team.Rights[i].Name, "id": "" + response.Team.Rights[i].Id }
                        $scope.UserRights.push(obj);
                    }
                    for (i = 0; i < response.Team.Groups.length; i++) {
                        var obj = { "label": response.Team.Groups[i].GroupName, "id": "" + response.Team.Groups[i].GroupId }
                        $scope.UserGroups.push(obj)
                    }
                    if ($scope.UserGroups.length == 0) {
                        $scope.userGroupDel = false
                    }
                    else {
                        $scope.userGroupDel = true;
                    }
                    //response.Rights
                    if ($scope.IsSelectedTeamMemberAdmin)
                        angular.copy($scope.rolesdata[1], $scope.rolesmodel);
                    else
                        angular.copy($scope.rolesdata[0], $scope.rolesmodel);

                    $scope.reportsmodel = $scope.UserVisibleReports;
                    $scope.rightsmodel = $scope.UserRights;
                    $scope.groupdata = $scope.UserGroups;
                    $scope.Connected_Profiles2 = [];
                    $scope.profileCount = 0;
                    if ($scope.groupdata.length > 0) {
                        $scope.groupmodel = { "id": "" + $scope.groupdata[0].id, "label": $scope.groupdata[0].label };// $scope.groupdata[0];
                        $scope.selectedGroupId = $scope.groupdata[0].id;
                        $scope.selectedGroupName = $scope.groupdata[0].label;



                        if (response.ProfileCompanyGroup != null) {
                            $scope.Connected_Group_Profiles = response.ProfileCompanyGroup;
                            GetUserProfile($scope.selectedGroupId);
                        }

                    }
                }
                else {
                    $scope.SelectedUserName = "";
                    $scope.SelectedUserPic = "/Content/img/user128.jpg";
                    $scope.SelectedTeamMemberId = "";
                    $scope.ProfilesAssignedToUser = 0;
                    $scope.UserGroups = [];
                    $scope.UserVisibleReports = [];
                    $scope.UserRights = [];
                    $scope.rolesmodel = {};
                    $scope.reportsmodel = $scope.UserVisibleReports;
                    $scope.rightsmodel = $scope.UserRights;
                    $scope.groupdata = $scope.UserGroups;
                    $scope.profileCount = 0;
                    //alert("No Data Found")

                    GlobalService.ErrorPopup("Error", "No profiles data found, please try again.");
                }
                // alert(groupmodel.label)

            }, function (err) {
                // alert('Error in fetching data');

                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                console.log(err);
            });
        }
    }

    function GetUserProfile(groupId) {
        for (i = 0; i < $scope.Connected_Group_Profiles.length; i++) {
            if ($scope.Connected_Group_Profiles[i].GroupId == groupId) {
                $scope.Connected_Profiles2 = $scope.Connected_Group_Profiles[i].ProfileId;
                break;
            }
        }
        var count = 0;
        for (i = 0; i < $scope.Connected_Profiles2.length; i++) {
            $scope.Connected_Profiles2[i].CompositeId = $scope.Connected_Profiles2[i].ProfileTypeId + '' + $scope.Connected_Profiles2[i].ProfileId;
            if ($scope.Connected_Profiles2[i].IsConnected == true)
                count++;

        }
        $scope.profileCount = count;
        //$scope.Connected_Profiles = l.data


    }
    $scope.RemoveTeamFromGroup = function (groupid, teamMemberId) {
        var isConfirmed = confirm('Are You Sure to Remove this Member from Group.');
        if (isConfirmed) {
            TeamService.RemoveProfileToGroup(teamMemberId, groupid).then(function (l) {
                if (l.data == false)
                    $scope.getTeamMembers($scope.CompanyId);
                else
                    $scope.loadUserData($scope.CompanyId, teamMemberId);
              //  alert("user removed from profile")
                notify({
                    message: "User removed from group",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

            }, function (err) {
                //alert('Error in remove profile, please try again.');

                GlobalService.ErrorPopup("Error", "Error in removing user from group, please try again.");
                console.log(err);
            });
        }
    }
    $scope.Cancel = function () {
        $scope.init();
    };

    $scope.Save = function () {

        var ProfileArr = [];
        //var tempArr = [];
        if ($scope.Connected_Group_Profiles != undefined) {
            for (i = 0; i < $scope.Connected_Group_Profiles.length; i++) {
                for (j = 0; j < $scope.Connected_Group_Profiles[i].ProfileId.length; j++) {

                    var data = {
                        GroupId: $scope.Connected_Group_Profiles[i].GroupId,
                        ProfileId: $scope.Connected_Group_Profiles[i].ProfileId[j].ProfileId,
                        IsConnected: $scope.Connected_Group_Profiles[i].ProfileId[j].IsConnected,
                        ProfileCompanyGroupId: $scope.Connected_Group_Profiles[i].ProfileId[j].ProfileCompnayGroupId,
                        ProfileTypeId: $scope.Connected_Group_Profiles[i].ProfileId[j].ProfileTypeId
                    }
                    ProfileArr.push(data);

                }
            }
        }

        var Rights = [];
        var Reports = [];
        for (i = 0; i < $scope.rightsmodel.length; i++) {
            Rights.push($scope.rightsmodel[i].id);
        }
        for (i = 0; i < $scope.reportsmodel.length; i++) {
            Reports.push($scope.reportsmodel[i].id);
        }

        //profileService.UpdateUserGroupProfle = function (UserId, CompanyId, AssignedRights, AssignedReports,Profiles)
        ProfileService.UpdateUserGroupProfle($scope.SelectedTeamMemberId, $scope.CompanyId, Rights, Reports, ProfileArr, $scope.rolesmodel.id).then(function (l) {
            // $location.path("/Signup_Code").search({ param: $scope.Email });;
            //alert("Successfully saved");
            notify({
                message: "Sucessfully Saved Data",
                position: GlobalService.position,
                duration: GlobalService.duration
            });

        }, function (err) {
           // alert('Error in saving, please try again.');
            GlobalService.ErrorPopup("Error", "Error in saving Data, please try again.");
            console.log(err);
        });
    }
    $rootScope.$on('Team_Reload', function (evt, msg) {
        $scope.getTeamMembers(msg.CompanyId);

    });

    //Set Active Group Class
    $scope.groupActiveClass = function (groupId) {
        if ($scope.GroupId == groupId)
            return 'groupTag_Ative';
        else
            return 'groupTag';
    }

    //Set Active Team Class
    $scope.teamActiveClass = function (TeamMemberId) {
        if ($scope.SelectedTeamMemberId == TeamMemberId)
            return 'team_active';
        else
            return '';

    }
    $scope.editgroupName = function (EditWhat) {
    if(EditWhat == "Group")
        $scope.items = { GroupName: $scope.selectedGroupName, GroupId: $scope.selectedGroupId, EditWhat: EditWhat };
    else
        $scope.items = { GroupName: $scope.CompanyName, GroupId: $scope.CompanyId, EditWhat: EditWhat };

    var modalInstance = $uibModal.open({
        animation: true,
        templateUrl: '/Scripts/Views/Shared/_EditGroupName.html',
        controller: '_EditGroupNameController',

        resolve: {
            items: function () {
                return $scope.items;
            }
        }
    });

    modalInstance.result.then(function (selectedItem) {
        //$scope.selected = selectedItem;
        if(EditWhat == "Group"){
            $('#' + selectedItem.GroupId + 'GroupBlock').html('<i class="fa fa-folder-open"></i>' + selectedItem.GroupName);
            $('.LeftGroupPanel').html(selectedItem.GroupName);
            $rootScope.$broadcast('Group_Dropdown_Text', { GroupId: selectedItem.GroupId, GroupName: selectedItem.GroupName });
        }
        else {
            $scope.CompanyId = selectedItem.GroupId;
            $scope.CompanyName = selectedItem.GroupName;
            $rootScope.$broadcast('Business_Name_Change', { CompanyId: $scope.CompanyId, CompanyName: $scope.CompanyName });
        }
    }, function () {
        $log.info('Modal dismissed at: ' + new Date());
    });

    }

    $scope.editTeamMemberName = function () {
        $scope.items = { FirstName: $scope.SelectedTeamMemberFirstName, LastName: $scope.SelectedTeamMemberLastName, MemberId: $scope.SelectedTeamMemberId };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EditTeamMemberName.html',
            controller: '_EditTeamMemberNameController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            $('#' + selectedItem.MemberId + 'MemberBlock').html(selectedItem.FirstName + " " + selectedItem.LastName);
            $('#' + selectedItem.MemberId + 'MemberGroupBlock').html(selectedItem.FirstName + " " + selectedItem.LastName);
            $scope.SelectedUserName = selectedItem.FirstName + " " + selectedItem.LastName;
            $scope.SelectedTeamMemberFirstName = selectedItem.FirstName;
            $scope.SelectedTeamMemberLastName = selectedItem.LastName;
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });

    }
    $scope.deleteGroup = function () {
        $scope.items = { GroupName: $scope.selectedGroupName, GroupId: $scope.selectedGroupId };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_DeleteGroupName.html',
            controller: '_DeleteGroupNameController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            // $scope.selected = selectedItem;
            // $('#' + selectedItem.GroupId + 'GroupBlock').html('<i class="fa fa-folder-open"></i>');
            // $('.LeftGroupPanel').html('');
            // $rootScope.$broadcast('Group_Reload2', '');
            // $scope.toggleVis('team');
            $('#' + selectedItem.GroupId + 'GroupBlock').parent().remove();
            if (selectedItem.GroupId != $scope.groups[0].GroupId)
            {
                $scope.GetGroupHead($scope.groups[0].GroupId);
                $rootScope.selectedGroupId = $scope.groups[0].GroupId;
            }
            
            $rootScope.$broadcast('Group_Deleted', { GroupId: selectedItem.GroupId});

            $scope.GetGroups();
            //$rootScope.selectedGroupId

            //alert($scope.selectedItem)
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.GetGroups = function () {

        GroupService.GetByCompanyId($scope.CompanyId).then(function (l) {
            if (l.data != null) {
                if ($rootScope.selectedGroupId != undefined) {
                    $scope.GroupId = $rootScope.selectedGroupId;
                }
                else {
                    $scope.GroupId = l.data[0].GroupId;
                }
                $scope.groups = l.data;
                $scope.GroupCount = l.data.length;
                //$compile($("#grplists ul"))($scope);
                if ($scope.broadcastedGroupId != undefined) {
                    $scope.GroupId = $scope.broadcastedGroupId;
                }
                $scope.GetGroupHead($scope.GroupId);
                $scope.GetGroupDetails($scope.GroupId);
                $scope.GetTeamDetails($scope.GroupId);
            }
            else {
               // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching groups , please try again.");
            }
        }, function (err) {
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching groups, please try again.");
            console.log(err);
        });
    }
    $scope.init();

    $scope.GetGroupHead = function (data) {
        GroupService.GetById(data).then(function (l) {
            if (l.data != null) {
                $scope.TotalProfiles = l.data.TotalProfiles;
                $scope.TotalMembers = l.data.TotalMembers;
                $scope.GroupName = l.data.GroupName;
                $scope.GroupId = l.data.GroupId;
                $scope.CompanyId = l.data.CompanyId;
                $scope.selectedGroupId = l.data.GroupId;
                $scope.selectedGroupName = l.data.GroupName;
            }
            else {
                //  alert('No Data');
            }
        }, function (err) {
          //  alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });



    }
    $scope.toggleVis = function (condition, teamMemberId) {
        if (teamMemberId != $scope.LoggedInTeamMemberId) {
            if (condition == 'group') {
                $scope.isGroup = true;
            }
            else {
                $scope.isGroup = false;
            }
        }
    }
    $scope.GetGroupDetailsClick = function (data) {
        $scope.GetGroupHead(data);
        $scope.GetGroupDetails(data);
        $scope.GetTeamDetails(data);
    }
    $scope.GetGroupDetails = function (data) {

        ProfileService.GetByGroupId(data).then(function (l) {
            if (l.data != null) {
                $.each(l.data, function (index, item) {
                    l.data[index].CompositeId = l.data[index].ProfileTypeId + '' + l.data[index].ProfileId;
                });
                $scope.Connected_Profiles = l.data
            }
            else {
                //alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching profiles data, please try again.");
            }
        }, function (err) {
           // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching profiles data, please try again.");
            console.log(err);
        });

    }
    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    }; //for replacing ~ from images path
    $scope.GetTeamDetails = function (data) {
        GroupService.GetTeamByGroupId(data).then(function (l) {
            if (l.data != null) {
                $scope.SelectedGroupTeam = l.data.Team;

            }
            else {
               // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            }
        }, function (err) {
           // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            console.log(err);
        });
    }
    $scope.ConnectTwitter = function () {

        $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.RemoveProfileFromGroup = function (ProfileId, CompanyId, ProfileType, IsPage, CompositeId) {
        //alert(ProfileId);
        //alert(CompanyId);
        //alert($scope.GroupId);
        if (IsPage)
            ProfileType = 5;
        ProfileService.RemoveProfileToGroup(ProfileId, CompanyId, $scope.GroupId, ProfileType).then(function (l) {
            $("#ConnectedProfile" + CompositeId).remove();
            if (l.data == "NGR") {
                $rootScope.$broadcast('Group_Deleted', { GroupId: $scope.GroupId });
                $scope.GetGroups();
            }
            //alert("profile removed")
            notify({
                message: "Profile has been removed",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
        }, function (err) {
            if (err.data.Message == "EBUN") {
                GlobalService.ErrorPopup("Error", "This is the last group containing Base Twitter Account. It cannot be deleted.");
            }
            else {
                GlobalService.ErrorPopup("Error", "Error in removing profile, please try again.");
            }

            console.log(err);
        });
    }

    $rootScope.$on('Group_Reload', function (evt, msg) {
        //alert("in broadcast")
        $scope.broadcastedGroupId = msg.GroupId;
        $scope.init();
    });
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.broadcastedGroupId = msg.GroupId;
        $scope.init();
    });
    $rootScope.$on('Team_Reload', function (evt, msg) {
        $scope.GetGroupHead(msg.GroupId);
        $scope.GetTeamDetails(msg.GroupId);
    });
}]);


