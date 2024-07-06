CRMApp.controller('ConnectSocialController', ['$scope', '$window', '$location', '$modal',function ($scope, $window, $location,$modal) {
    $scope.CompanyId = $window.localStorage.getItem('selectedCompanyId'); //loggedin selected company (BusinessName ID)
    $scope.GroupId = $window.localStorage.getItem('selectedGroupId');

    $scope.ConnectTwitter = function () {

        $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.ConnectFacebookPage = function () {
        var modalInstance = $modal.open({
            templateUrl: '/Scripts/Views/Profile/Connect_FB_Pages.html',
        });
    }
    //    $window.location.href = '/Profile/Connect_Facebook';
    //}

    $scope.ConnectFacebookProfile = function () {

        $window.location.href = '/Profile/Connect_Facebook/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.ConnectLinkedinPage = function () {

        $window.location.href = '/Profile/Connect_LinkedIn/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.ConnectLinkedinProfile = function () {

        $window.location.href = '/Profile/Connect_LinkedIn/' + $scope.CompanyId + '/' + $scope.GroupId;
    }


    $scope.ConnectGoogle = function () {
       $window.location.href = '/Profile/ConnectGoogleAnalytics/' + $scope.CompanyId + '/' + $scope.GroupId;
    }
    $scope.ConnectInstagram = function () {
        $window.location.href = '/Profile/ConnectInstagram/' + $scope.CompanyId + '/' + $scope.GroupId;
    };
}]);