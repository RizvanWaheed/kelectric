CRMApp.factory('FeedlyService', function ($http) {
    var feedlyService = {};

    feedlyService.AddSearchTerm = function (SearchKeyword, GroupId, IsSearchTerm) {

        var _data = { SearchKeyword: SearchKeyword, GroupId: GroupId, IsSearchTerm: IsSearchTerm};

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Feedly/AddSearchTerm',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    feedlyService.DeleteSearchTerm = function (id) {

       // var _data = { SearchKeyword: SearchKeyword, GroupId: GroupId, IsSearchTerm: IsSearchTerm };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Feedly/DeleteSearchTerm/' + id
        }).then(function (res) {

            return res;
        });
        return request;

    }
    
    feedlyService.GetSearchTerm = function (groupId, isSearchTerm) {
        var requestUrl = base_url + '/api/Feedly/GetSearchTerm/' + groupId + '/' + isSearchTerm;
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

    feedlyService.GetFeed = function (GroupId, IsSearchTerm, PageIndex, PageSize, SearchTerm, MaxFeedId) {

        var _data = { GroupId: GroupId, IsSearchTerm: IsSearchTerm, PageIndex: PageIndex, PageSize: PageSize, SearchTerm: SearchTerm, MaxFeedId:MaxFeedId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Feedly/GetMinnedFeed',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    return feedlyService;
});