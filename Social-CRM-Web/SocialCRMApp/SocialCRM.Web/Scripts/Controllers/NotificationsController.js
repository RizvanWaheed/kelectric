CRMApp.controller('NotificationsController', ['$rootScope', '$scope', '$location', 'AuthService', '$window', '$stateParams', '$cookies', 'NotificationService', function ($rootScope, $scope, $location, AuthService, $window, $stateParams, $cookies, NotificationService) {
    $scope.firstTime = true;
    $scope.init = function () {
        $scope.NextCall = null;
        $scope.notificationList = [];
        $scope.NotifTypeSelected = $scope.NotificationTypeModel[0].Id;
        $scope.fetchNotification();
    }
   
    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };
    $scope.popup1 = {
        opened: false
    };
    
    $scope.NotificationTypeModel = [{
        Id: 1,
        Name: "All Notifications"
    }, {
        Id: 2,
        Name: "Read Notifications"
    },{
        Id: 3,
        Name: "Unread Notifications"
    }];

    $scope.loadMoreNotifications = function () {
        if ($scope.NextCall != "End") {
            if ($scope.NotifTypeSelected == 1)
                $scope.fetchNotification();
            else if ($scope.NotifTypeSelected == 2)
                $scope.fetchReadUnReadNotification(true);
            else if ($scope.NotifTypeSelected == 3)
                $scope.fetchReadUnReadNotification(false);
        }
    }

    $scope.selectedNotification = function (notifType) {
        $scope.NextCall = null;
        $scope.notificationList = [];
        if (notifType == 1)
            $scope.fetchNotification();
        else if (notifType == 2)
            $scope.fetchReadUnReadNotification(true);
        else if (notifType == 3)
            $scope.fetchReadUnReadNotification(false);
    }

    $scope.fetchReadUnReadNotification = function (isRead) {
        NotificationService.GetReadUnReadUserNotifications(isRead, 10, $scope.NextCall).then(function (l) {
            if (l.data != null) {
                $scope.notificationList.push.apply($scope.notificationList, l.data.UserNotifications);
                $scope.NotificationCount = l.data.UserNotifications.length;
                $scope.NextCall = l.data.NextCall;
                //alert(l.data);
            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.fetchNotification = function () {
        NotificationService.GetUserNotifications($rootScope.selectedCompanyId, 10, $scope.NextCall).then(function (l) {
            if (l.data != null) {
                $scope.notificationList.push.apply($scope.notificationList, l.data.UserNotifications);
                $scope.NotificationCount = l.data.UserNotifications.length;
                $scope.NextCall = l.data.NextCall;
                //alert(l.data);
            }

        }, function (err) {
             GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.init();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        if ($scope.firstTime) {
            $scope.init();
            $scope.firstTime = false;
        }
    });
}]);