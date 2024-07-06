CRMApp.factory('GoogleService', function ($http) {
    var googleService = {};

    googleService.GetPages=function(analyticInfo)
    {
        var requestUrl = base_url + '/Analytics/GetUserAnalyticAccounts/' + analyticInfo + '/1';
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

    googleService.AddPages = function (PagesList) {

        var _pages = PagesList;//{ FirstName: FirstName, LastName: LastName, Email: Email, Password: Password, ImageUrl: ImageUrl, ConfirmationCode: "", AccountType: "Silver" };


        var request = $http({
            method: 'POST',
            url: base_url + '/Analytics/AddAnalyticsAccount',
            data: { parm1: JSON.stringify(_pages) }
        }).then(function (res) {

            return res;
        });
        return request;
    };

    googleService.GetUserAnalyticAccounts
        = function (analyticInfo) {
            var requestUrl = base_url + '/Analytics/GetUserAnalyticAccounts/' + analyticInfo + '/1';
            var request = $.ajax({
                crossdomain: true,
                url: requestUrl,
                type: 'Get',
                asyc:false,
                contentType: 'application/x-www-form-urlencoded',
                dataType: 'json',
                success: function (analyticAccounts) {
                    console.log(JSON.stringify(analyticAccounts));
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(JSON.stringify(jqXHR));
                    console.log(JSON.stringify(textStatus));
                    console.log(JSON.stringify(errorThrown));
                }
            });
            return request;
        };
    googleService.GetAnalyticsChart
        = function (analyticAccountId, dateFilter, customDateFilter) {
            var requestUrl = base_url + '/Analytics/GetAnalyticsChart/' + analyticAccountId + '/' + dateFilter + '/' + customDateFilter;
            var request = $http({
                method: 'GET',
                url: requestUrl,
                crossdomain: true
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
            return request;
        };
    googleService.GetAnalyticsChartAndAccount
       = function (groupId) {
           var requestUrl = base_url + '/Analytics/GetAnalyticsChartAndAccount?groupId=' + groupId;
           var request = $http({
               method: 'GET',
               url: requestUrl,
               crossdomain: true
           }).then(function successCallback(response) {
               return response;
           }, function errorCallback(response) {
               // called asynchronously if an error occurs
               // or server returns response with an error status.
           });
           return request;
       };
    return googleService;
});