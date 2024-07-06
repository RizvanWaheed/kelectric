CRMApp.controller('_EditGroupNameController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'GroupService', 'items', 'GlobalService', 'notify', 'BusinessProfileService', function ($rootScope, $scope, $window, $location, $uibModalInstance, GroupService, items, GlobalService, notify, BusinessProfileService) {
    //alert($location.search().param);
   
    $scope.init = function () {
        $scope.GroupName = items.GroupName;
        $scope.GroupId = items.GroupId;
        $scope.EditWhat = items.EditWhat;
    }
    $scope.init();
   
    $scope.save=function()
    {
        if ($scope.EditWhat == "Group") {
            GroupService.UpdateGroup($scope.GroupName, $scope.GroupId).then(function (l) {
                // alert("group name has been updated")
                notify({
                    message: "group name has been updated",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                $uibModalInstance.close({ GroupName: $scope.GroupName, GroupId: $scope.GroupId });
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);


            });
        }
        else if ($scope.EditWhat == "Business") {
            var submitObj = {};
            var Notificationlist = [];
            // alert(JSON.stringify(arr))
            submitObj = {
                CompanyId: $scope.GroupId,
                CompanyName: $scope.GroupName,
                Notificaitons: Notificationlist
            }

            BusinessProfileService.UpdateBusinessProfileName(submitObj).then(function (l) {
                //  alert("Data has been updated Succesfully")
                notify({
                    message: "Data has been updated Succesfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $uibModalInstance.close({ GroupName: $scope.GroupName, GroupId: $scope.GroupId });
                //  $scope.init();

            }, function (err) {
                //alert('Error in Saving, please try again.');
                GlobalService.ErrorPopup("Error", "Error in saving Data, please try again.");

                console.log(err);
            });
        }
       // $uibModalInstance.close();
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

   }]);