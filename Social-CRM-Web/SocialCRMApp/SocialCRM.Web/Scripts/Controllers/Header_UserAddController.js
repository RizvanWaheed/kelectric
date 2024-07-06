CRMApp.controller('Header_UserAddController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, items, GlobalService, notify) {
    //alert($location.search().param);
    $scope.selected = {};
    $scope.GroupId = items.groupId;
    $scope.CompanyId = items.companyId;
    $scope.init = function () {
        $scope.slim = {
            height: '200px',
            disableFadeOut: true
        }
        $scope.example5customTexts = { buttonDefaultText: 'Please Select' };
        $scope.example5settings = {

        };
        $scope.rightsmodel = [];
        $scope.rightsdata = []; 
        $scope.reportsmodel = [];
        $scope.reportsdata = [];
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
               // alert('No Dropdown data found');
                GlobalService.ErrorPopup("Error", "No Dropdown data found, please try again.");
            }
        }, function (err) {
           // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");

            console.log(err);
        });
    }
    $scope.init();
   
    $scope.clickclick = function ()//group id is passed here and profile are fetched of the selected groups
    {
       
        ProfileService.GetByGroupId($scope.GroupId).then(function (l) {
            if (l != null) {
                $scope.Connected_Profiles = l.data
                
            }
            else {
               // alert('No Profiles Attached');
                GlobalService.ErrorPopup("Error", "No Profiles Attached, please try again.");

            }
        }, function (err) {
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");

            console.log(err);
        });
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.instaIcon = '/Content/images/insta_ico16.png';
        //alert(id);
    }

    $scope.clickclick();

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.CreateGroup = function () {

        var profiledata = [];
        var AssignedRights = [];
        var AssignedReports = [];
        var data = $.grep($scope.Connected_Profiles, function (record) {
            return $scope.selected[record.ProfileId];
        });
        $.each(data, function (key, value) {
            profiledata.push({ profileId: value.ProfileId, ProfileType: value.ProfileTypeId, GroupId: $scope.GroupId });
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
            AssignedGroups: [$scope.GroupId],
            AssignedRights: AssignedRights,
            AssignedReports: AssignedReports,
            AssignedProfiles: profiledata
        };
        
        TeamService.Register(teamdata).then(function (l) {
            //alert("Team Member Added Successfully");
            notify({
                message: "Team Member Added Successfully.",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $rootScope.$broadcast('Team_Reload', {CompanyId: $scope.CompanyId, GroupId: $scope.GroupId});
            $uibModalInstance.close('cancel');
        }, function (err) {
            //alert('Error in Saving, please try again.');
            GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);
            console.log(err);
        });

     
    }
}]);