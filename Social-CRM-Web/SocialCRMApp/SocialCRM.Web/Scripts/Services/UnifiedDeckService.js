CRMApp.factory('UnifiedDeckService', function ($http) {
    var deckService = {};

    deckService.GetSelectedProfilesActivity = function (profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType) {
        var requestUrl = base_url + '/api/UnifiedDeck/GetUnifiedDeckForUserSelectedProfiles/' + profileData + '/' + profilePageData + '/' + msgTypeData + '/' + tagTypeData + '/' + sentTypeData + '/' +
                            pageNumber + '/' + pageSize + '/' + groupId + '/' + companyId + '/' + isAllTask + '/' + mediaType;
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

    deckService.GetLatestUnifiedDeckData = function (profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType, ids) {
        //var requestUrl = base_url + '/api/UnifiedDeck/GetLatestUnifiedDeckDataForSelectedProfiles/' + profileData + '/' + profilePageData + '/' + msgTypeData + '/' + tagTypeData + '/' +
        //                    pageNumber + '/' + pageSize + '/' + groupId + '/' + companyId + '/' + isAllTask + '/' + mediaType + '/' + ids;
        var requestUrl = base_url + '/api/UnifiedDeck/GetLatestUnifiedDeckForUserSelectedProfiles/' + profileData + '/' + profilePageData + '/' + msgTypeData + '/' + tagTypeData + '/' + sentTypeData + '/' +
                            pageNumber + '/' + pageSize + '/' + groupId + '/' + companyId + '/' + isAllTask + '/' + mediaType + '/' + ids;
        var request = $http({
            async: true,
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
           // 
            return response;
        }, function errorCallback(response) {
            
        });

        return request;
    };

    deckService.IsPostMarkedAsCompleted = function (messageType, ActivityId) {
        var requestUrl = base_url + '/api/UnifiedDeck/IsPostMarkedAsCompleted/' + messageType + '/' + ActivityId;
        var request = $http({
            async: true,
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };
    deckService.GetNextUnrespondedUnifiedDeckData = function (profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, maxUnrespondedPostIds, markCompleted) {
        //debugger;
        var requestUrl = base_url + '/api/UnifiedDeck/GetNextUnifiedDeckForUserSelectedProfilesUnresponded/' + profileData + '/' + profilePageData + '/' + msgTypeData + '/' + tagTypeData + '/' + sentTypeData + '/' +
                            pageNumber + '/' + pageSize + '/' + groupId + '/' + companyId + '/' + isAllTask + '/' + maxUnrespondedPostIds + '/' + markCompleted;
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

    deckService.UpdateTagsForFeed = function (feedId, feedType, tagIds, companyId) {
        var _data = { feedId: feedId, feedType: feedType, tagIds: tagIds, companyId: companyId };
        var requestUrl = base_url + '/api/UnifiedDeck/UpdateTagsForFeed/'; // + feedId + '/' + feedType + '/' + tagId;
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    deckService.InsertTagForFeed = function (feedId, feedType, tagId, companyId) {
        var _data = { feedId: feedId, feedType: feedType, tagId: tagId, companyId: companyId };
        var requestUrl = base_url + '/api/UnifiedDeck/InsertTagsForFeed/'; // + feedId + '/' + feedType + '/' + tagId;
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    deckService.RemoveTagFromFeed = function (feedId, feedType, tagId) {
        //debugger;
        var _data = { feedId: feedId, feedType: feedType, tagId: tagId };
        var requestUrl = base_url + '/api/UnifiedDeck/RemoveTagFromFeed/'; // + feedId + '/' + feedType + '/' + tagId;
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    deckService.InsertSentimentForFeed = function (feedId, feedType, sentiment, companyId) {
        //debugger;
        var _data = { feedId: feedId, feedType: feedType, sentiment: sentiment, companyId: companyId };
        var requestUrl = base_url + '/api/UnifiedDeck/InsertSentimentForFeed/'; // + feedId + '/' + feedType + '/' + tagId;
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };
    
    deckService.GetUnrespondedQueriesColumnSettings = function () {
        //debugger;
        var requestUrl = base_url + '/api/UnifiedDeck/GetUnrespondedQueriesColumnSettings/';
        var request = $http({
            method: 'GET',
            url: requestUrl            
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    deckService.updateUnrespondedColumnSettings = function (ResultList) {
        //debugger;
        var postData = JSON.stringify(ResultList);
        var _data = { PostData: postData };
        var requestUrl = base_url + '/api/UnifiedDeck/updateUnrespondedColumnSettings/'; // + feedId + '/' + feedType + '/' + tagId;
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    };

    deckService.GetSampleRepliesText = function () {
        //debugger;
        var requestUrl = base_url + '/api/UnifiedDeck/GetSampleRepliesText/';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    deckService.DeleteFacebookItem = function (userId, isPage, id, type) {
        var requestUrl = base_url + '/api/UnifiedDeck/DeleteFacebookItem/' + userId + '/' + isPage + '/' + id + '/' + type;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    };

    deckService.DeleteTwitterTweet = function (userName, tweetId) {
        var requestUrl = base_url + '/api/UnifiedDeck/DeleteTwitterTweet/' + userName + '/' + tweetId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    };

    deckService.DeleteTwitterDM = function (userName, msgId) {
        var requestUrl = base_url + '/api/UnifiedDeck/DeleteTwitterDM/' + userName + '/' + msgId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    };

    deckService.DeleteInstagramComment = function (userId, commentId) {
        var requestUrl = base_url + '/api/UnifiedDeck/DeleteInstagramComment/' + userId + '/' + commentId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    };

    deckService.GetConsumerData = function (profile, onProfile, isPage, feedType) {
        var requestUrl = base_url + '/api/UnifiedDeck/GetConsumerData/' + profile + '/' + onProfile + '/' + isPage + '/' + feedType;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });

        return request;
    };

    deckService.GetColumnsOrderForResponder = function (companyId) {
        var requestUrl = base_url + '/api/UnifiedDeck/GetColumnsOrderForResponder/' + companyId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {           
            return response;
        }, function errorCallback(response) {
            
        });
        return request;
    };

    deckService.UpdateColumnsOrderForResponder = function (companyId, arrangementData) {
        var _data = { 'CompanyId': companyId, 'PostData': JSON.stringify(arrangementData) };
        var requestUrl = base_url + '/api/UnifiedDeck/UpdateColumnsOrderForResponder/';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    };

    return deckService;
});
