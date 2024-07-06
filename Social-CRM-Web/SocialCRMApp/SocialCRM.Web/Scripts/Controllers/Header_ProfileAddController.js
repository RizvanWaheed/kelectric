CRMApp.controller('Header_ProfileAddController', ['$rootScope', '$scope', '$window', '$location', '$uibModal','$uibModalInstance','items', function ($rootScope, $scope, $window, $location, $uibModal, $uibModalInstance, items) {
    $scope.CompanyId = $rootScope.selectedCompanyId; //loggedin selected company (BusinessName ID)
    $scope.GroupId = $rootScope.selectedGroupId;

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
        $uibModalInstance.dismiss('cancel');
    };
    $scope.ConnectTwitter = function () { 

        $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '') ;
    }

    $scope.ConnectFacebookPage = function () {

        $window.location.href = '/Profile/Connect_Facebook_Page/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '');
    }

    $scope.ConnectFacebookProfile = function () {

        $window.location.href = '/Profile/Connect_Facebook/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '');
    }
    
    $scope.ConnectLinkedinProfile = function () {

        $window.location.href = '/Profile/Connect_LinkedIn/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '');
    }

    $scope.ConnectGoogle = function () {
        $window.location.href = '/Profile/ConnectGoogleAnalytics/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '');///Profile/ConnectGoogleAnalytics/{companyId}/{groupId}
    }
    $scope.ConnectInstagram = function () {
        $window.location.href = '/Profile/ConnectInstagram/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '');
    };

    $scope.ConnectOutlook = function () {
        //$window.location.href = '#/GroupAndTeam/OutlookConnect?groupid=' + $scope.GroupId + '&companyid=' + $scope.CompanyId;
        //$uibModalInstance.close('cancel');

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
    }

}]);