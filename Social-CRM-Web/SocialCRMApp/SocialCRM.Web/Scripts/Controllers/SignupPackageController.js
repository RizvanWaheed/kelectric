CRMApp.controller('SignupPackageController', ['$scope', '$location', function ($scope, $location) {

    $scope.Trial = function () {
        $location.path("/Signup").search({ param: 'Trial' });
    }
    $scope.Basic = function () {
        $location.path("/Signup").search({param: 'Basic'});
    }
    $scope.Silver = function () {
        $location.path("/Signup").search({ param: 'Silver' });
    }
    $scope.Gold = function () {
        $location.path("/Signup").search({ param: 'Gold' });
    }
    $scope.Platinum = function () {
        $location.path("/Signup").search({ param: 'Platinum' });
    }
}]);


