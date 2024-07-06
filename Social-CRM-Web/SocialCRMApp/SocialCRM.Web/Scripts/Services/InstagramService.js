CRMApp.factory('InstagramService', function ($http) {
    var instagramService = {};
    instagramService.GetAnalyticsChart
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
    instagramService.GetAnalyticsChartAndAccount
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
    instagramService.GetInstagramReports = function (DateRange, StartDate, EndDate, InstagramAccountId, GroupId, CompanyId)
    {
        _data = { DateRange: DateRange, StartDate: StartDate, EndDate: EndDate, InstagramAccountId: InstagramAccountId, GroupId : GroupId, CompanyId:CompanyId };
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Instagram/GetInstagramReports',
            data: _data
        }).then(function (res) {

            return res;
        });
        return request;
    }

    instagramService.GetPostDetails
        = function (PostId) {
            var requestUrl = base_url + '/api/Instagram/GetInstagramPostDetails/' + PostId;
            var request = $http({
                method: 'GET',
                url: requestUrl,
                crossdomain: true
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
            });
            return request;
        };

    instagramService.PostComment = function (PostId, UserId, Comment,CompanyId) {
            var _data = { PostId: PostId, UserId: UserId, Comment: Comment, CompanyId: CompanyId};

            var requestUrl = base_url + '/api/Instagram/PostComment/';
            var request = $http({
                method: 'POST',
                url: requestUrl,
                data: JSON.stringify(_data),
                crossdomain: true
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
            });
            return request;
        };

    instagramService.DeleteComment
        = function (PostId, UserId, CommentId) {
            var requestUrl = base_url + '/api/Instagram/DeleteComment/' + PostId + '/' + UserId + '/' + CommentId;
            var request = $http({
                method: 'GET',
                url: requestUrl,
                crossdomain: true
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
            });
            return request;
        };

    instagramService.LikePost
        = function (PostId, UserId) {
            var requestUrl = base_url + '/api/Instagram/LikePost/' + PostId + '/' + UserId;
            var request = $http({
                method: 'GET',
                url: requestUrl,
                crossdomain: true
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
            });
            return request;
        };

    instagramService.UnlikePost
        = function (PostId, UserId) {
            var requestUrl = base_url + '/api/Instagram/UnlikePost/' + PostId + '/' + UserId;
            var request = $http({
                method: 'GET',
                url: requestUrl,
                crossdomain: true
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
            });
            return request;
        };

    instagramService.IsPostLiked
        = function (PostId, UserId) {
            var requestUrl = base_url + '/api/Instagram/IsPostLiked/' + PostId + '/' + UserId;
            var request = $http({
                method: 'GET',
                url: requestUrl,
                crossdomain: true
            }).then(function successCallback(response) {
                return response;
            }, function errorCallback(response) {
            });
            return request;
        };

    return instagramService;
});