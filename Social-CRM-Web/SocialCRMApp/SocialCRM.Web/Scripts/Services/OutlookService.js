CRMApp.factory('OutlookService', function ($http) {
    var outlookService = {};

    outlookService.GetOutlookProfiles = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Profile/GetOutlookProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
        var request = $http({
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {
            //  
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    outlookService.GetOutlookInboxEmails = function (email, lastDt, index, maxDt, status) {
        var requestUrl = base_url + '/api/Outlook/GetOutlookInboxEmails?email=' + email + '&lastdt=' + lastDt + '&index=' + index + '&maxdt=' + maxDt + '&st=' + status;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    //this service call is created just to block loader on real time data fetch
    outlookService.GetOutlookNewInboxEmails = function (email, lastDt, index, maxDt, status) {
        var requestUrl = base_url + '/api/Outlook/GetOutlookNewInboxEmails?email=' + email + '&lastdt=' + lastDt + '&index=' + index + '&maxdt=' + maxDt + '&st=' + status;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    outlookService.SetReadStatus = function (email, date) {
        var _data = { EmailRecDate: date, EmailAddress: email };

        var requestUrl = base_url + '/api/Outlook/SetReadStatus';//  ?email=' + email + '&EmailDate=' + date;
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    }

    outlookService.GetOutlookSentEmails = function (email, lastDt, index, maxDt, status) {
        var requestUrl = base_url + '/api/Outlook/GetOutlookSentEmails?email=' + email + '&lastdt=' + lastDt + '&index=' + index + '&maxdt=' + maxDt + '&st=' + status;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    //this service call is created just to block loader on real time data fetch
    outlookService.GetOutlookNewSentEmails = function (email, lastDt, index, maxDt, status) {
        var requestUrl = base_url + '/api/Outlook/GetOutlookNewSentEmails?email=' + email + '&lastdt=' + lastDt + '&index=' + index + '&maxdt=' + maxDt + '&st=' + status;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    return outlookService;
});