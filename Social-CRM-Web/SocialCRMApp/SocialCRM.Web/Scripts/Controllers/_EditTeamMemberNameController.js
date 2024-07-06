CRMApp.controller('_EditTeamMemberNameController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'TeamService', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, TeamService, items, GlobalService, notify) {
    //alert($location.search().param);
   
    $scope.init = function () {
        $scope.FirstName = items.FirstName;
        $scope.LastName = items.LastName;
        $scope.MemberId = items.MemberId;
    }
    $scope.init();
   
    $scope.save=function()
    {
        TeamService.UpdateTeamMember($scope.FirstName, $scope.LastName, $scope.MemberId).then(function (l) {
           // alert("group name has been updated")
            notify({
                message: "team member name has been updated",
                position: GlobalService.position,
                duration: GlobalService.duration
            });

            $uibModalInstance.close({ FirstName: $scope.FirstName, LastName: $scope.LastName, MemberId: $scope.MemberId });
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in updating team member name, please try again.");

           
        });
       // $uibModalInstance.close();
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

   }]);