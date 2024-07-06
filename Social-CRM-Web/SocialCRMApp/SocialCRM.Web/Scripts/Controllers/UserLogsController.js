CRMApp.controller('UserLogsController', ['$rootScope', '$scope', '$sce', '$window', '$location', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', 'UserService', function ($rootScope, $scope, $sce, $window, $location, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify, UserService) {
   
    $scope.rightsToReceiveReports = {
        value: false
    };
    $scope.userLogs = [];    

    $scope.getUserLogs = function () {
        UserService.GetUserLogs().then(function (result) {
            if (result.statusText == "OK") {
                $scope.userLogs = result.data;
               // console.log($scope.socialMediaUsersUpdateReceivers);
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.init = function () {
        $scope.getUserLogs();
    }

    $scope.init();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
}]);