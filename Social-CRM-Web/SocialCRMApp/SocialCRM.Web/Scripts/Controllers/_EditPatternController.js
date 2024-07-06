CRMApp.controller('_EditPatternController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'PatternCustomizatioService', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, PatternCustomizatioService, items, GlobalService, notify) {
    
    //$scope.commentsLength = {
    //    height: '380px',
    //    disableFadeOut: true
    //}

    $scope.RegexFor = items.RegexFor;
    $scope.Pattern = items.Pattern;
    $scope.Id = items.Id;
    $scope.Type = items.Type;

    $scope.SavePattern = function () {
        $scope.RegexFor = $scope.RegexFor.trim();
        var isValid = true;
        if ($scope.RegexFor.indexOf(' ') >= 0)
        {
            var inputs = $scope.RegexFor.split(' ')
            for(var i = 1; i < inputs.length; i++)
            {
                if(inputs[i].length != inputs[0].length)
                    isValid = false;
            }
        }
        if (isValid) {
            PatternCustomizatioService.UpdatePattern($scope.Id, $scope.RegexFor, $scope.Pattern).then(function (l) {
                if (l.data) {
                    notify({
                        message: "Pattern saved successfully.",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                } else {
                    notify({
                        message: "Pattern saved failed.",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
                $scope.cancel();
                $scope.fetchPattern();
            }, function (err) {
                // alert('Error in update, please try again.');
                GlobalService.ErrorPopup("Error", "Error in update, please try again.");

                console.log(err);
            });
        }
        else {
            GlobalService.ErrorPopup("Error", "Not a valid pattern.");
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


}]);