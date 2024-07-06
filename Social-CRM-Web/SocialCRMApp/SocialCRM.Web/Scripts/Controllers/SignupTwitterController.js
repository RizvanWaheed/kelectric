CRMApp.controller('SignupTwitterController', ['$window', '$scope', '$rootScope', '$location', '$http', 'AuthService', 'GlobalService', 'notify', function ($window, $scope, $rootScope, $location, $http, AuthService, GlobalService, notify) {
   
    $scope.Email = $location.search().param;
    if ($scope.Email == undefined)
        $window.location.href = "/#/Login";

    $scope.ConnectTwitter = function () {
        AuthService.CreateNewUser().then(function (l) {
            AuthService.UserProfile($scope.Email, 0).then(function (l) {
                if (l != null) {
                    $rootScope.selectedGroupId = l.data.GroupId;
                    $rootScope.selectedCompanyId = l.data.CompanyId;
                    $rootScope.selectedCompanyName = l.data.CompanyName;
                    $rootScope.currentUser = JSON.stringify(l.data);
                    $scope.CompanyId = $rootScope.selectedCompanyId;
                    $scope.GroupId = $rootScope.selectedGroupId;
                
                    //Adding 1 is type value in order to point that it is base account for the signing up user
                    $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId + '?url=GroupAndTeam' + '&type=personal,baseaccount';
                }
                  
            }, function (err) {
                // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching user data, please try again.");
                console.log(err);
            });

        }, function (err) {
            // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in Registering user, please try again.");
            console.log(err);
        });
    }
}]);