CRMApp.controller('_ExistingMemberController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'TeamService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, TeamService, GlobalService, notify) {
    $scope.ErrorShow = false;
    $scope.GroupId = $location.search().groupid;
    $scope.CompanyId = $location.search().companyid;
    $scope.selected = {};
    $scope.init = function () {
        $scope.slim = {
            height: '400px',
            disableFadeOut: true
        }
        $scope.Connected_Profiles = [
             //{
             //    ProfilePicURL: '/Content/img/user2.jpg',
             //    ProfileName: 'ZohaibAkhtarKh1',
             //    ProfileEmail: 'Zoaib@trg.com',
             //    ProfileId: 12
             //}
        ]
        TeamService.GetTeamMembersNotInGroup($scope.CompanyId, $scope.GroupId).then(function (l) {
            if (l != null) {
                $scope.Connected_Profiles = l.data.Team;
            }
            else {
               // alert('No Profiles Attached');

                GlobalService.ErrorPopup("Error", 'No Profiles Attached');
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");

           
        });
        
       

    };
    // runs once per controller instantiation
    $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.AddProfile=function()
    {
        $scope.ErrorShow = false;
       // $scope.SelectedTeamMemId=jQuery.parseJSON($rootScope.currentUser).;
       // [{1,2},{1,3},{teamMemberId,GroupId}]
        var teamdata = [];
        var data = $.grep($scope.Connected_Profiles, function (record) {
            return $scope.selected[record.TeamMemberId];
        });
        $.each(data, function (key, value) {
            
            teamdata.push({ TeammemberId: value.TeamMemberId, GroupId: parseInt($scope.GroupId) });
        });
        if (teamdata.length>0){
        TeamService.AssignGroup(teamdata).then(function (l) {
          //  alert("Team Member Added Successfully");
            notify({
                message: "Team Member Added Successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $rootScope.$broadcast('Team_Reload', { CompanyId: $scope.CompanyId, GroupId: $scope.GroupId });
           // if()
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in adding team member, please try again.");
            
        });
        }
        else {
            $scope.ErrorShow = true;
            $scope.ErrorMessage = 'There should be atleast one team member selected.';
        }
    }
}]);