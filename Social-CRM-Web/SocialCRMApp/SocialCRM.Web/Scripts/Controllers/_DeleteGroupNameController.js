CRMApp.controller('_DeleteGroupNameController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'GroupService', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, GroupService, items, GlobalService, notify) {
    
    $scope.init = function () {
        $scope.GroupName = items.GroupName;
        $scope.GroupId = items.GroupId;
    }
    $scope.init();
   
    $scope.Delete=function()
    {
        GroupService.DeleteGroup($scope.GroupId).then(function (l) {
          //  alert("group has been deleted")
            notify({
                message: "group has been deleted",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $uibModalInstance.close({ GroupName: $scope.GroupName, GroupId: $scope.GroupId });
        }, function (err) {
            if (err.data.Message == "EBUN") {
                GlobalService.ErrorPopup("Error", "This is the last group containing Base Twitter Account. It cannot be deleted.");
            }
            else {
                GlobalService.ErrorPopup("Error", "Error in deleting group, please try again.");
            }

            $uibModalInstance.dismiss('cancel');
        });
        
    }
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

   }]);