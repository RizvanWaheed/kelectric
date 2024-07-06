CRMApp.controller('_FeedsSchedulerController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'TaskService', 'items', 'UnifiedService', function ($rootScope, $scope, $window, $location, $uibModalInstance, TaskService, items, UnifiedService) {

    $('#timepicker4').timepicker({
        minuteStep: 1,
        secondStep: 5,
        showInputs: false,
        template: 'modal',
        modalBackdrop: true,
        showSeconds: true,
        showMeridian: false
    });
    $('#ctrlanalyticdatepicker').daterangepicker();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
   
    $scope.pop_schprof = {
        height: '100px',
        disableFadeOut: true
    }
  
}]);