CRMApp.controller('_UpdateTagsController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'UnifiedDeckService', 'UnifiedService', 'items', 'TwitterService', 'InstagramService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, UnifiedDeckService, UnifiedService, items, TwitterService, InstagramService, GlobalService, notify) {

    $scope.Id = items.status.Id;
    $scope.MessageType = items.status.MessageType;
    $scope.AllTags = items.AllTags;
    $scope.ExistingTags = items.status.Tags.slice();
    
    $scope.UpdateTags = function () {
        var selectedTagIds = "";
        angular.forEach($scope.AllTags, function (obj) {
            if ($scope.ExistingTags.indexOf(obj.BrandType) !== -1) {
                if (selectedTagIds == "") {
                    selectedTagIds = obj.BrandTypeId;
                }
                else {
                    selectedTagIds += "," + obj.BrandTypeId;
                }
            } 
        });

        UnifiedDeckService.UpdateTagsForFeed($scope.Id, $scope.MessageType, selectedTagIds, $rootScope.selectedCompanyId).then(function (result) {
            if (result.statusText == "OK") {
                items.status.Tags = $scope.ExistingTags;
                $uibModalInstance.dismiss('cancel');
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in updating tags, please try again.");
            console.log(err);
        });
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);