CRMApp.controller('Personal_ProfileAddController', ['$rootScope', '$scope', '$window', '$location', '$uibModal', '$uibModalInstance', 'items', function ($rootScope, $scope, $window, $location, $uibModal, $uibModalInstance, items) {
    $scope.CompanyId = $rootScope.selectedCompanyId; //loggedin selected company (BusinessName ID)
    $scope.GroupId = $rootScope.selectedGroupId;
    $scope.IsPersonal = $rootScope.IsPersonal;

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
        $rootScope.$broadcast('Group_Profile_Reload', { GroupId: $scope.GroupId, CompanyId: $scope.CompanyId, IsPersonal: $scope.IsPersonal });
        $uibModalInstance.dismiss('cancel');
    };

    //$rootScope.$on('Group_Profile_Reload', function (evt, msg) {
    //    $scope.getProfileDetails($scope.CompanyId, $scope.GroupId, true);
    //});

    $scope.ConnectTwitter = function () {

        $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '') + '&type=personal';
    }

    $scope.ConnectFacebookPage = function () {

        $window.location.href = '/Profile/Connect_Facebook_Page/' + $scope.CompanyId + '/' + 1 + '?url=' + location.hash.split('?')[0].replace('#/', '') + '&type=personal';
    }

    $scope.ConnectFacebookProfile = function () {

        $window.location.href = '/Profile/Connect_Facebook/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '') + '&type=personal';
    }

    $scope.ConnectLinkedinProfile = function () {

        $window.location.href = '/Profile/Connect_LinkedIn/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '') + '&type=personal';
    }

    $scope.ConnectGoogle = function () {
        $window.location.href = '/Profile/ConnectGoogleAnalytics/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '') + '&type=personal';///Profile/ConnectGoogleAnalytics/{companyId}/{groupId}
    }
    $scope.ConnectInstagram = function () {
        $window.location.href = '/Profile/ConnectInstagram/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=' + location.hash.split('?')[0].replace('#/', '') + '&type=personal';
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
    };


}]);