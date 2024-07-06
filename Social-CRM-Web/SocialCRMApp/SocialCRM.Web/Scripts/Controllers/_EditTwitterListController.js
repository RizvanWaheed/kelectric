CRMApp.controller('_EditTwitterListController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'items', 'UnifiedService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, items, UnifiedService, GlobalService, notify) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.init = function () {
        $scope.ListName = items.selectedList.Name;
        $scope.Mode = items.selectedList.Mode;
        $scope.ListDescrip = items.selectedList.Description;
    };

    $scope.EditList = function () {
        items.selectedList.Name = $scope.ListName;
        items.selectedList.Mode = $scope.Mode;
        items.selectedList.Description = $scope.ListDescrip;
        UnifiedService.EditTwitterList(items.selectedList).then(function (l) {
            $uibModalInstance.close(items.selectedList);
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in updating twitter list info, please try again.");

        });
    };

    $scope.init();
}]);