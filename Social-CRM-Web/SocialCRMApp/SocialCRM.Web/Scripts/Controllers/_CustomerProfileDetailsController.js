CRMApp.controller('_CustomerProfileDetailsController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'TaskService', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, TaskService, items, GlobalService, notify) {

    $scope.commentsLength = {
        height: '480px',
        disableFadeOut: true
    }

   $scope.init = function () {
        $scope.GroupId = items.GroupId;
        $scope.CustomerProfileId = items.CustomerProfileId
        $scope.fetchCustomerTasks();
        //$scope.commentReply = "A";
    };

    $scope.fetchCustomerTasks = function (){
        TaskService.GetCustomerTasks($scope.CustomerProfileId,$scope.GroupId).then(function (l) {            
            $scope.taskList = l.data;            
        }, function (err) {
            // alert('Error in saving, please try again.');
            GlobalService.ErrorPopup("Error", "Error in saving, please try again.");
            console.log(err);
        });
    };
    
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };


    $scope.init();
}]);