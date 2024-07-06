CRMApp.factory('TwitterService', function ($http) {
    var twitterService = {};


    twitterService.TwitterReplyTweet = function (fromUser, tweetId, status, mediaInfo, companyId) {

        var _data = { fromUser: fromUser, tweetId: tweetId, status: status, mediaInfo: mediaInfo, companyId: companyId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterReplyTweet',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterReportSpam = function (fromUser, userId) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterReportSpam/' + fromUser + '/' + userId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterMarkFavorite = function (fromUser, tweetId) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterMarkFavorite/' + fromUser + '/' + tweetId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterIsFavorited = function (fromUser, tweetId) {

        var request = $http({
            method: 'GET',
            url: base_url + '/api/Twitter/TwitterIsFavorited/' + fromUser + '/' + tweetId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterIsRetweeted = function (fromUser, tweetId) {

        var request = $http({
            method: 'GET',
            url: base_url + '/api/Twitter/TwitterIsRetweeted/' + fromUser + '/' + tweetId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterUnmarkFavorite = function (fromUser, tweetId) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterUnmarkFavorite/' + fromUser + '/' + tweetId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterRetweet = function (fromUser, tweetId) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterRetweet/' + fromUser + '/' + tweetId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterUnRetweet = function (fromUser, tweetId) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterUnRetweet/' + fromUser + '/' + tweetId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterNewTweet = function (fromUser, status, mediaInfo, lat, lng, companyId) {
        var _data = { fromUser: fromUser, status: status, mediaInfo: mediaInfo, lat: lat, lng: lng, companyId: companyId };
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterNewTweet',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }
    
    twitterService.TwitterSendDirectMessage = function (fromUser,toUser, status) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/TwitterSendDirectMessage/' + fromUser + '/' + toUser + '/' + status
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterSendDM = function (UserId, FromUser, ToUser, Status, CompanyId) {
        var _data = { userId: UserId, fromUser: FromUser, toUser: ToUser, status: Status, CompanyId: CompanyId };
        var request = $http({
            method: 'POST',
            data: JSON.stringify(_data),
            url: base_url + '/api/Twitter/TwitterSendDM'
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.TwitterRetweetWithComment = function (fromUser, authorUserName, tweetId, status, mediaInfo, companyId) {
        var _data = { fromUser: fromUser, authorUserName: authorUserName, tweetId: tweetId, status: status, mediaInfo: mediaInfo, companyId: companyId };

        var request = $http({
            method: 'POST',
            data: JSON.stringify(_data),
            url: base_url + '/api/Twitter/TwitterRetweetWithComment'
        }).then(function (res) {

            return res;
        });
        return request;

    }

    twitterService.GetTwitterConversation = function (fromUser, ofUser, count, maxId, groupId, companyId, statusId, MType) {
        var requestUrl = base_url + '/api/Twitter/GetTwitterConversation/' + fromUser + '/' + ofUser + '/' + count + '/' + maxId + '/' + groupId + '/' + companyId + '/' + statusId + '/' + MType;
        var request = $http({
            async: true,
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {
        
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    twitterService.GetTweetMinnedReplies = function (ofUser, tweetId) {
        var requestUrl = base_url + '/api/Twitter/GetTweetMinnedReplies/' + ofUser + '/' + tweetId;
        var request = $http({
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    twitterService.GetTweetDirectReplies = function (fromUser, ofUser, tweetId) {
        var requestUrl = base_url + '/api/Twitter/GetTweetDirectReplies/' + fromUser + '/' + ofUser + '/' + tweetId;
        var request = $http({
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    twitterService.GetTwitterDirectMessages = function (fromUser, ofUser) {
        var requestUrl = base_url + '/api/Twitter/GetTwitterDirectMessages/' + fromUser + '/' + ofUser;
        var request = $http({
            async: true,
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    twitterService.TwitterGetPageReportStats = function (profiles, from, to) {
        var _data = { Profiles: profiles, From: from, To: to };
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Twitter/GetProfilesReportStats',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });        
        return request;
    }

    twitterService.TwitterGetTweetThreadByTweetId = function (ofuser, tweetId) {
        var _data = { ofuser: ofuser, tweetId: tweetId};
        var request = $http({
            method: 'GET',
            url: base_url + '/api/Twitter/GetTweetThreadByTweetId/' + ofuser + '/' + tweetId,
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }

    return twitterService;
});