CRMApp.controller('_ViewAttachmentController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'CustomerProfileService', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, CustomerProfileService) {

    $scope.commentsLength = {
        height: '380px',//350
        disableFadeOut: true
    }

    $scope.Id = items.Id;

    $scope.init = function () {
        CustomerProfileService.GetAttachmentAsBase64String($scope.Id).then(function (l) {
            var response = l;
            $scope.ImgSrc = response.data;
            console.log(response);
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    },
    
    $scope.cancel = $scope.OK = function () {        
        $uibModalInstance.close('cancel');
    };

    $scope.init();

}]);