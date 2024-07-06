CRMApp.factory('NotificationService', function ($http) {
    var notificationService = {};

    notificationService.GetAllMessages = function () {
        var requestUrl = base_url + '/api/Notification/GetAllMessages/5';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
    notificationService.GetNotificationById = function (companyId,id) {
        var requestUrl = base_url + '/api/Notification/GetNotificationById/' + companyId + "/" + id;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
    notificationService.GetUserNotifications = function (companyId, pageSize, nextCall) {
        var requestUrl = base_url + '/api/Notification/GetUserNotifications';
        var _data = { CompanyId: companyId, PageSize: pageSize, NextCall: nextCall };
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    notificationService.SendNotificationEmailUpdates = function () {
        var requestUrl = base_url + '/api/Notification/SendNotificationEmailUpdates';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    notificationService.GetReadUnReadUserNotifications = function (isRead, pageSize, nextCall) {
        var _data = { IsRead: isRead, PageSize: pageSize, NextCall: nextCall };
        var requestUrl = base_url + '/api/Notification/GetReadUnReadUserNotifications';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    notificationService.MarkReadNotification = function (companyId) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Notification/MarkReadNotification/' + companyId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    notificationService.MarkReadNotificationById = function (notifId) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Notification/MarkReadNotificationById/' + notifId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    return notificationService;
});