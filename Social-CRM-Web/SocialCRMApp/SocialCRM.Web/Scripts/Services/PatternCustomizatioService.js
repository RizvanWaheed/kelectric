CRMApp.factory('PatternCustomizatioService', function ($http) {
    var patternCustomizatioService = {};
  
    patternCustomizatioService.FetchConsumerPattern = function () {

        var requestUrl = base_url + '/api/Profile/GetConsumerPattern/';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    patternCustomizatioService.UpdateActivePattern = function (id) {

        var requestUrl = base_url + '/api/Profile/UpdateActivePattern/' + id;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    }

    patternCustomizatioService.UpdatePattern = function (Id, Regex, regexPattern) {
        var _data = { Id : Id, RegexFor: Regex, Pattern: regexPattern };

        var requestUrl = base_url + '/api/Profile/UpdatePattern';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    patternCustomizatioService.DisablePattern = function (id) {

        var requestUrl = base_url + '/api/Profile/DisablePattern';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    }

    return patternCustomizatioService;
});