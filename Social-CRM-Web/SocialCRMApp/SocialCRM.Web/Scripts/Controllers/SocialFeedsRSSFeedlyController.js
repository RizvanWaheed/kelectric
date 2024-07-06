CRMApp.controller('SocialFeedsRSSFeedlyController', ['$rootScope', '$scope', '$window', '$location', '$uibModal', 'GroupService', 'ProfileService', '$compile', function ($rootScope, $scope, $window, $location, $uibModal, GroupService, ProfileService, $compile) {
    $scope.slim = {
        height: '90px',
        disableFadeOut: true
    }
    $scope.AddKeywordShow = false;
    $scope.CollapseAddKeyword=function()
    {
        $scope.AddKeywordShow = !$scope.AddKeywordShow;
    }
}]);


