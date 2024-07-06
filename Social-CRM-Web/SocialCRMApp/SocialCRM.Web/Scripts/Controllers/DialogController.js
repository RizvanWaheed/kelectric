CRMApp.controller('DialogController', ['$scope','$mdDialog', '$window', '$location', function ($scope, $mdDialog, $window, $location) {


    $scope.hide = function () {
        $mdDialog.hide();
    };
    $scope.cancel = function () {
        $mdDialog.cancel();
    };
    $scope.answer = function (answer) {
        $mdDialog.hide(answer);
    };

}]);