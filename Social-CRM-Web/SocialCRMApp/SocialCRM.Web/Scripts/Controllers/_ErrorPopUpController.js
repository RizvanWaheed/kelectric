CRMApp.controller('_ErrorPopUpController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items',  function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items) {

    $scope.ErrorTitle = items.ErrorTitle;
    $scope.ErrorMessage = items.ErrorMessage;
    // $scope.msg = items.msg;
   
    
    $scope.Cancel = $scope.OK = function () {
        
        $uibModalInstance.close('cancel');
    };

}]);