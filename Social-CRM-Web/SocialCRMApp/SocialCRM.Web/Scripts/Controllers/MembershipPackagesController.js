CRMApp.controller('MembershipPackagesController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', '$compile', 'AuthService', 'GroupService', '$uibModal', '$cookies', 'AuthService', 'AdminService', 'GlobalService', 'notify', 'NotificationService', function ($rootScope, $scope, $window, $location, $stateParams, $compile, AuthService, GroupService, $uibModal, $cookies, AuthService, AdminService, GlobalService, notify, NotificationService) {
    $scope.init = function () {
        $scope.packageData = {};
        $scope.GroupsAllowed = 0;
        $scope.TeamsAllowed = 0;
        $scope.KeywordsAllowed = 0;
        $scope.TagsAllowed = 0;
        $scope.FbProfilesAllowed = 0;
        $scope.TwProfilesAllowed = 0;
        $scope.InProfilesAllowed = 0;
        $scope.packageName = $rootScope.PackageName;

        $scope.GetMembershipPackage();
    }

    $scope.openMembershipDetails = function () {
        $scope.items = { "packageData": $scope.packageData };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EditMembershipPackage.html',
            controller: '_EditMembershipPackageController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.GetMembershipPackage();
            //  $scope.selected = selectedItem;
            //  alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.GetMembershipPackage = function () {
        AdminService.GetMembershipPackage(5).then(function (l) {
            $scope.packageData = l.data;
            for (var i = 0; i < $scope.packageData.Restrictions.length; i++) {
                var restrict = $scope.packageData.Restrictions[i];
                if (restrict.RestrictionTypeId == 1)
                    $scope.GroupsAllowed = restrict.Count;
                else if (restrict.RestrictionTypeId == 2)
                    $scope.TeamsAllowed = restrict.Count;
                else if (restrict.RestrictionTypeId == 3)
                    $scope.KeywordsAllowed = restrict.Count;
                else if (restrict.RestrictionTypeId == 4)
                    $scope.TagsAllowed = restrict.Count;
                else if (restrict.RestrictionTypeId == 5)
                    $scope.FbProfilesAllowed = restrict.Count;
                else if (restrict.RestrictionTypeId == 6)
                    $scope.TwProfilesAllowed = restrict.Count;
                else if (restrict.RestrictionTypeId == 7)
                    $scope.InProfilesAllowed = restrict.Count;
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            console.log(err);
        });
    }

    $rootScope.$on('Session_Fetched_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.init();

}]);