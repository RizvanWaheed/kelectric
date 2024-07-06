
CRMApp.factory('GlobalService', function ($http,$uibModal,$window) {
    var globalService = {
        position: 'center',
        duration: 8000,
        error_css: 'alert-danger',
        warning_css: '',
        success_css: 'alert-success'
    };

    globalService.ErrorPopup = function (ErrorTitle, ErrorMessage) {
        var items = { ErrorTitle: ErrorTitle, ErrorMessage: ErrorMessage };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ErrorPopUp.html',
            controller: '_ErrorPopUpController',

            resolve: {
                items: function () {
                    return items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //alert($window.location.hash.split('?')[0])
            $window.location.href = $window.location.hash.split('?')[0];
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    globalService.ConfirmPopup = function (ErrorTitle, ErrorMessage, callback) {
        var items = { ErrorTitle: ErrorTitle, ErrorMessage: ErrorMessage };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ConfirmPopUp.html',
            controller: '_ConfirmPopUpController',

            resolve: {
                items: function () {
                    return items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //alert($window.location.hash.split('?')[0])
            callback(selectedItem);
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
            callback(false);
        });
    }

    return globalService;
})