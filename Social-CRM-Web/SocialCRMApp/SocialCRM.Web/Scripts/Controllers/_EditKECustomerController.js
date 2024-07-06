CRMApp.controller('_EditKECustomerController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'CustomerProfileService', '$uibModalInstance', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $stateParams, CustomerProfileService, $uibModalInstance, items, GlobalService, notify) {

    $scope.initModel = function () {
        $scope.KERegions = [];
        $scope.KEAreas = [];
        $scope.KEAreas.unshift({ Id: 0, Name: "Select Areas" });
        $scope.AreaSelected = $scope.KEAreas[0];

        $scope.GetCustomerRegions(true);
    };

    $scope.GetCustomerRegions = function (isFirstTime) {
        CustomerProfileService.GetCustomerRegions().then(function (l) {
            l.data.unshift({ Id: 0, Name: "Select Region" });
            $scope.KERegions = l.data;
            $scope.RegionSelected = $scope.KERegions[0];

            if (isFirstTime && items.Cust.Region !== null) {
                $scope.RegionSelected = $scope.KERegions.filter(function (el) {
                    return el.Name === items.Cust.Region;
                })[0];
                $scope.GetCustomerAreas(isFirstTime);
            }
        }, function (err) {
            //alert('Error in fetching User Facebook Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching Regions.");
        });
    }

    $scope.GetCustomerAreas = function (isFirstTime) {
        if ($scope.RegionSelected.Id == 0) {
            $scope.KEAreas = [];
            $scope.KEAreas.unshift({ Id: 0, Name: "Select Areas" });
            $scope.AreaSelected = $scope.KEAreas[0];
        }
        else {
            CustomerProfileService.GetCustomerAreas($scope.RegionSelected.Id).then(function (l) {
                l.data.unshift({ Id: 0, Name: "Select Areas" });
                $scope.KEAreas = l.data;
                $scope.AreaSelected = $scope.KEAreas[0];

                if (isFirstTime && items.Cust.IBC !== null) {
                    $scope.AreaSelected = $scope.KEAreas.filter(function (el) {
                        return el.Name === items.Cust.IBC;
                    })[0];
                }
            }, function (err) {
                //alert('Error in fetching User Facebook Profiles');
                GlobalService.ErrorPopup("Error", "Error in fetching Regions.");
            });
        }
    }

    $scope.SubmitUser = function () {
        items.Cust.Region = $scope.RegionSelected.Name;
        items.Cust.IBC = $scope.AreaSelected.Name;
        items.Cust.IBC_Code = $scope.AreaSelected.Code;

        CustomerProfileService.UpdateCustomer(items.Cust).then(function (l) {
            if (l.data == true) {
                notify({
                    message: "Customer Information updated successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $uibModalInstance.close("OK");
            }
        }, function (err) {
            //alert('Error in fetching User Facebook Profiles');
            GlobalService.ErrorPopup("Error", "Error in updating Customer.");
        });
    };

    $scope.initModel();

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

}]);