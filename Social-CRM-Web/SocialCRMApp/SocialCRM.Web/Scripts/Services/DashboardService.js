CRMApp.factory('DashboardService', function ($http) {

    var dashboardService = {};

    dashboardService.GetGroupPerformance = function (CompanyId,GroupId, DateFrom, DateTo) {

        var _data = { CompanyId: CompanyId, ClassificationId: GroupId, DateFrom: DateFrom, DateTo: DateTo};
        var requestUrl = base_url + '/api/Dashboard/GetGroupPerformance/';
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

    dashboardService.GetTopInfluencers = function (CompanyId, DateFrom, DateTo) {

        var _data = { CompanyId: CompanyId, DateFrom: DateFrom, DateTo: DateTo };
        var requestUrl = base_url + '/api/Dashboard/GetTopInfluencers/';
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

    dashboardService.GetByCompanyId = function (companyid) {
        var requestUrl = base_url + '/api/Team/GetByCompanyId/' + companyid;
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

    dashboardService.GetDashboardInfo = function (companyId, groupId) {
        var requestUrl = base_url + '/api/Dashboard/GetDashboardInfo/' + companyId + '/' + groupId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
        });
        return request;
    }

    dashboardService.FilterGroupKeyStatistics = function (CompanyId, GroupId, DateFrom, DateTo) {
        DateFrom = DateFrom === undefined ? "" : DateFrom;
        DateTo = DateTo === undefined ? "" : DateTo;
        var _data = { companyId: CompanyId, groupId: GroupId, dateFrom: DateFrom, dateTo: DateTo };
        //var requestUrl = base_url + '/api/Dashboard/FilterGroupKeyStatistics/' + companyId + '/' + groupId + '/' + dateFrom + '/' + dateTo;
        var requestUrl = base_url + '/api/Dashboard/FilterGroupKeyStatistics/';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
        });
        return request;
    }

    dashboardService.FilterGroupPerformance = function (companyId, groupId, dateFrom, dateTo) {
        var requestUrl = base_url + '/api/Dashboard/FilterGroupPerformance/' + companyId + '/' + groupId + '/' + dateFrom + '/' + dateTo;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
        });
        return request;
    }
    //Return this service
    return dashboardService;
});