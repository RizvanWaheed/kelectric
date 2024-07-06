CRMApp.controller('_EditMembershipPackageController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'AdminService', 'items', 'UnifiedService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, AdminService, items, UnifiedService, GlobalService, notify) {
    $scope.packageData = items.packageData;
    $scope.selectedRight = {};
    $scope.selectRight = true;
    $scope.packageName = $rootScope.PackageName;

    $scope.selectedReport = {};
    $scope.selectReport = true;

    angular.forEach($scope.packageData.Permissions, function (obj) {
        $scope.selectedRight[obj.PermissionId] = obj.Enabled;
    });

    angular.forEach($scope.packageData.AccessLevels, function (obj) {
        $scope.selectedReport[obj.AccessLevelId] = obj.Enabled;
    });

    $scope.rightSelectedChanged = function () {
        $scope.selectRight = false;
    };

    $scope.checkAllRights = function () {
        angular.forEach($scope.packageData.Permissions, function (obj) {
            $scope.selectedRight[obj.PermissionId] = $scope.selectRight;
        });
    };

    $scope.reportSelectedChanged = function () {
        $scope.selectReport = false;
    };

    $scope.checkAllReports = function () {
        angular.forEach($scope.packageData.AccessLevels, function (obj) {
            $scope.selectedReport[obj.AccessLevelId] = $scope.selectReport;
        });
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.updatePackage = function () {
        AdminService.UpdateMembershipPackage($scope.packageData).then(function (l) {
            notify({
                message: "Package updated successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            //alert("Successfully saved ");
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            console.log(err);
        });
    }

    $scope.validateCount = function (el, id) {
        console.log(id);
        if (el.msg.Count === undefined) {
            angular.element('#warning-' + id).css("display", "inline-block")
        }
        else {
            angular.element('#warning-' + id).css("display", "none")
        }
    };

}]);