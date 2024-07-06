CRMApp.controller('UserNSocialProfileController', ['$rootScope', '$scope', '$window', '$location', 'TeamService', 'ProfileService', 'GlobalService', 'notify', 'GroupService', function ($rootScope, $scope, $window, $location, TeamService, ProfileService, GlobalService, notify, GroupService) {

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        //alert("in broadcast")
        // $scope.init();
        // $scope.grouplist
        // GroupId  { GroupId: obj.GroupId, CompanyId: obj.CompanyId, IsPersonal: 'false' }

        if(msg.IsPersonal==true)
        {
            //Do processing on code submission
            // $location.path("/UnifiedInbox");
        }
        else
        {
            $scope.GetTeamDetails(msg.GroupId);
        }
    });

    
    $scope.GetTeamDetails = function (data) {
        GroupService.GetTeamByGroupId(data).then(function (l) {
            if (l.data != null) {
                
                $scope.CompanyName = l.data.CompanyName;
                $scope.groupMembers = l.data.Team;
                $scope.TeamMemberInGroup = $scope.groupMembers.length;
                var firstMem = $scope.groupMembers[0];
                $scope.loadUserData($window.localStorage.getItem('selectedCompanyId'), firstMem.TeamMemberId);

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
   
    $scope.init = function () {
        //$scope.GetGroupDetails($window.localStorage.getItem('selectedGroupId')); //for fetching profiles of group
        //$scope.GetGroupHead($window.localStorage.getItem('selectedGroupId'));
        $scope.GetTeamDetails($window.localStorage.getItem('selectedGroupId')); //for fetching group team and company
       // $scope.CompanyName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.groupmodel = {};
        $scope.groupdata = [];
        $scope.groupsettings = {
            selectionLimit: 1,
            smartButtonMaxItems: 2,
            smartButtonTextConverter: function (itemText, originalItem) {

                return itemText;
            }
        }
        $scope.reportsmodel = [];

        $scope.reportsdata = [{
            "label": "Twitter Report",
            "id": "1"
        }, {
            "label": "Facebook Report",
            "id": "2"
        }, {
            "label": "Task Management Report",
            "id": "3"
        }, {
            "label": "Group Management Report",
            "id": "4"
        }, {
            "label": "Trend Report",
            "id": "5"
        }, {
            "label": "Competitor Analysis Report",
            "id": "6"
        }];

        $scope.rightsmodel = [];

        $scope.rightsdata = [{
            "label": "Add Team Members",
            "id": "1"
        }, {
            "label": "Grant Rights to others",
            "id": "2"
        }, {
            "label": "Manage Message Status",
            "id": "3"
        }, {
            "label": "Publish Without Approval",
            "id": "4"
        },
        {
            "label": "Group Manager",
            "id": "5"
        }];

        $scope.example5customTexts = { buttonDefaultText: 'Please Select' };
        $scope.groupProfiles = [];

        $scope.companyId = $window.localStorage.getItem('selectedCompanyId');//get company id at the time of login



        $scope.slim = {
            height: '200px',
            disableFadeOut: true
        }
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.Connected_Group_Profiles = [];
        $scope.Connected_Profiles = [];

       // $scope.getTeamMembers($scope.companyId);

    };
    $scope.replaceTilda = function (label) {
       // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };
    //$scope.getTeamMembers = function (companyId) {
    //    TeamService.GetByCompanyId(companyId).then(function (l) {
    //        var response = l.data;
    //        $scope.groupMembers = response.Team;
    //       // alert(JSON.stringify($scope.groupMembers));
    //        $scope.TeamMembers = response.TotalMember;
    //        $scope.TotalGroups = response.TotalGroup;
    //        $scope.TotalProfiles = response.TotalProfile;
    //        $scope.CompanyName = response.CompanyName;
    //        var firstMem = $scope.groupMembers[0];
    //        $scope.loadUserData(companyId,firstMem.TeamMemberId);


    //    }, function (err) {
    //        GlobalService.ErrorPopup("Error", 'Error in fetching pages data');
    //        //alert('Error in fetching pages data');
    //        console.log(err);
    //    });
    //}
    // runs once per controller instantiation
    $scope.init();

    $scope.loadUserData = function (companyId,teamMemberId) {
       // companyId = 1;//get company id at the time of login

        ProfileService.GetAllProfilesByCompanyAndUserId(companyId, teamMemberId).then(function (l) {
            var response = l.data[0];
            //alert(JSON.stringify(response.Team))
            if (response.Team != null) {
               // alert(JSON.stringify(response.Team))
                $scope.SelectedUserName = response.Team.Profile.Name;
                $scope.SelectedUserPic = response.Team.Profile.ProfileUrl == null && "/Content/img/user128.jpg" || $scope.replaceTilda(response.Team.Profile.ProfileUrl);
            $scope.SelectedTeamMemberId = response.Team.Profile.TeamMemberId;
            $scope.ProfilesAssignedToUser = response.Team.Profile.AssignedProfile;
            $scope.UserGroups = [];
            $scope.UserVisibleReports = [];
            $scope.UserRights = [];

            for (i = 0; i < response.Team.VisibleReports.length; i++) {
                var obj = { "label": response.Team.VisibleReports[i].Name, "id": "" + response.Team.VisibleReports[i].Id }
                $scope.UserVisibleReports.push(obj)
            }
            for (i = 0; i < response.Team.Rights.length; i++) {
                var obj = { "label": response.Team.Rights[i].Name, "id": "" + response.Team.Rights[i].Id }
                $scope.UserRights.push(obj)
            }
            for (i = 0; i < response.Team.Groups.length; i++) {
                var obj = { "label": response.Team.Groups[i].GroupName, "id": "" + response.Team.Groups[i].GroupId }
                $scope.UserGroups.push(obj)
            }
            //response.Rights
            $scope.reportsmodel = $scope.UserVisibleReports;
            $scope.rightsmodel = $scope.UserRights;
            for (i = 0; i < $scope.UserGroups.length; i++)
            {
                $scope.groupdata = [];
                if ($window.localStorage.getItem('selectedGroupId') == $scope.UserGroups[i].id) {
                    $scope.groupdata.push({ "id": "" + $scope.UserGroups[i].id, "label": $scope.UserGroups[i].label })
                    //$scope.selectedGroupName = $scope.groupdata[0].label;
                    break;
                }
            }
          //  $scope.groupdata = $scope.UserGroups;
            $scope.Connected_Profiles = [];
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
                $scope.reportsmodel = $scope.UserVisibleReports;
                $scope.rightsmodel = $scope.UserRights;
                $scope.groupdata = $scope.UserGroups;
                $scope.profileCount = 0;
                GlobalService.ErrorPopup("Error", 'No Data Found');
                //alert("No Data Found")
            }
            // alert(groupmodel.label)

        }, function (err) {
            GlobalService.ErrorPopup("Error", 'Error in fetching data');
            //alert('Error in fetching data');
            console.log(err);
        });

    }
    function GetUserProfile(groupId)
    {
        for (i = 0; i < $scope.Connected_Group_Profiles.length; i++)
        {
            if($scope.Connected_Group_Profiles[i].GroupId==groupId)
            {
                $scope.Connected_Profiles = $scope.Connected_Group_Profiles[i].ProfileId;
                break;
            }
        }
        var count = 0;
        for(i=0;i<$scope.Connected_Profiles.length;i++)
        {
            if ($scope.Connected_Profiles[i].IsConnected==true)
            count++;

        }
        $scope.profileCount = count;
                //$scope.Connected_Profiles = l.data
              
    
    }
    $scope.RemoveTeamFromGroup = function (groupid, teamMemberId) {
        var isConfirmed = confirm('Are You Sure to Remove this Member from Group.');
        if (isConfirmed) {
            TeamService.RemoveProfileToGroup(teamMemberId, groupid).then(function (l) {
                $scope.loadUserData($scope.companyId, teamMemberId);
               
                notify({
                    message: "Sucessfully Removed ",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in removing, please try again.");
               
                console.log(err);
            });
        }
    }

    $scope.Cancel = function () {
        $scope.init();
    };
    
    $scope.Save =function()
    {
        //{
        //    GroupId:1,
        //    ProfileId:11,
        //    IsConnected:true,
        //    ProfileCompnayGroupId:1,
        //    ProfileTypeId:1
        //}

        var ProfileArr = [];
        //var tempArr = [];
        for (i = 0; i < $scope.Connected_Group_Profiles.length; i++) {
            for (j = 0; j < $scope.Connected_Group_Profiles[i].ProfileId.length; j++)
            {

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
        var Rights = [];
        var Reports = [];
        for (i = 0; i < $scope.rightsmodel.length;i++)
        {
            Rights.push($scope.rightsmodel[i].id);
        }
        for (i = 0; i < $scope.reportsmodel.length; i++) {
            Reports.push($scope.reportsmodel[i].id);
        }
        //profileService.UpdateUserGroupProfle = function (UserId, CompanyId, AssignedRights, AssignedReports,Profiles)
        ProfileService.UpdateUserGroupProfle($scope.SelectedTeamMemberId, $scope.companyId, Rights, Reports, ProfileArr).then(function (l) {
            // $location.path("/Signup_Code").search({ param: $scope.Email });;
            notify({
                message: "Sucessfully Saved Data",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
           // alert("Successfully saved");
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in saving Data, please try again.");
            
            console.log(err);
        });
    }
    $rootScope.$on('Team_Reload', function (evt, msg) {
        $scope.getTeamMembers(msg);

    });

    
}]);