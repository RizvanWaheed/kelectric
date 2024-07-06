CRMApp.controller('_ProfileAddController', ['$rootScope','$scope', '$window', '$location', '$uibModal','$uibModalInstance', function ($rootScope,$scope, $window, $location, $uibModal, $uibModalInstance) {
    $scope.GroupId = $location.search().groupid;
    $scope.CompanyId = $location.search().companyid;
    $scope.IsPersonal = $rootScope.IsPersonal.toString();
    $('#lstFruits').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '100%',
    });

    $('#2stFruits').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '100%',
    });

    $('#3stFruits').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '100%',
    });
    
    $scope.cancel = function () {
        $rootScope.$broadcast('Group_Profile_Reload', { GroupId: $scope.GroupId,CompanyId: $scope.CompanyId, IsPersonal: $scope.IsPersonal } );
        $uibModalInstance.dismiss('cancel');
    }

    $scope.ConnectTwitter = function () {

        $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.ConnectFacebookPage = function () {

        $window.location.href = '/Profile/Connect_Facebook_Page/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.ConnectFacebookProfile = function () {

        $window.location.href = '/Profile/Connect_Facebook/' + $scope.CompanyId + '/' + $scope.GroupId;
    }
    
    $scope.ConnectLinkedinProfile = function () {

        $window.location.href = '/Profile/Connect_LinkedIn/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.ConnectGoogle = function () {
        $window.location.href = '/Profile/ConnectGoogleAnalytics/' + $scope.CompanyId + '/' + $scope.GroupId;///Profile/ConnectGoogleAnalytics/{companyId}/{groupId}
    }
    $scope.ConnectInstagram = function () {
        $window.location.href = '/Profile/ConnectInstagram/' + $scope.CompanyId + '/' + $scope.GroupId;
    };
    $scope.ConnectOutlook = function () {
        $scope.items = { groupid: $scope.GroupId, companyid: $scope.CompanyId };
        var modalInstance = $uibModal.open({
            // animation: true,
            templateUrl: '/Scripts/Views/Shared/_OutlookConnect.html',
            controller: '_OutlookConnectController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function () {
        }, function () {
        });
    };

}]);