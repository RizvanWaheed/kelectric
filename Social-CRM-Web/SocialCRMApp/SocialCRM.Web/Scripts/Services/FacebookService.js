
CRMApp.factory('FacebookService', function ($http) {
    var facebookService = {};

    facebookService.GetPages = function (analyticInfo)
    {
        var requestUrl = base_url + '/Facebook/GetFacebookPages?parm1=' + analyticInfo;
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

    facebookService.AddPages = function (PagesList,CompanyId,GroupId) {

        var _pages = PagesList;//{ FirstName: FirstName, LastName: LastName, Email: Email, Password: Password, ImageUrl: ImageUrl, ConfirmationCode: "", AccountType: "Silver" };

        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/AddFacebookPages',
            data: { parm1: JSON.stringify(_pages), companyId: CompanyId, groupId: GroupId }
        }).then(function (res) {

            return res;
        });
        return request;
    };

    facebookService.FacebookLikePost = function (userId, isPage, postId, isPost) {

        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookLikePost?userId=' + userId + '&isPage=' + isPage + '&postId=' + postId + '&isPost=' + isPost
        }).then(function (res) {

            return res;
        });
        return request;

    }

    facebookService.FacebookUpdateMessage = function (userId, isPage, postId, message, msgType) {

        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookUpdateMessage?userId=' + userId + '&isPage=' + isPage + '&objectId=' + postId + '&message=' + message + '&msgType=' + msgType
        }).then(function (res) {

            return res;
        });
        return request;

    }

    facebookService.FacebookHideUnhidePost = function (userId, isPage, postId, hide) {

        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookHideUnhidePost?userId=' + userId + '&isPage=' + isPage + '&objectId=' + postId + '&hide=' + hide
        }).then(function (res) {

            return res;
        });
        return request;

    }

    facebookService.FacebookIsPostHidden = function (userId, isPage, postId) {

        var request = $http({
            method: 'GET',
            url: base_url + '/Facebook/FacebookIsPostHidden?userId=' + userId + '&isPage=' + isPage + '&postId=' + postId
        }).then(function (res) {

            return res;
        });
        return request;
    }

    facebookService.FacebookIsPostLiked = function (userId, isPage, postId, isPost) {

        var request = $http({
            method: 'GET',
            url: base_url + '/Facebook/FacebookIsPostLiked?userId=' + userId + '&isPage=' + isPage + '&postId=' + postId + '&isPost=' + isPost
        }).then(function (res) {

            return res;
        });
        return request;

    }

    facebookService.FacebookUnLikePost = function (userId, isPage, postId, isPost) {

        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookUnLikePost?userId=' + userId + '&isPage=' + isPage + '&postId=' + postId + '&isPost=' + isPost
        }).then(function (res) {

            return res;
        });
        return request;

    }

    facebookService.FacebookCommentPost = function (userId, isPage, postId, comment, filePath, contentType, fileName, companyId, ReplyType) {
        var _data = { userId: userId, isPage: isPage, postId: postId, status: comment, filePath: filePath, contentType: contentType, fileName: fileName, companyId: companyId, ReplyType: ReplyType };
        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookCommentPost',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }
    facebookService.FacebookNewPost = function (userId, isPage, comment, filePath, contentType, fileName, placeId, companyId) {
    
        var _data = { userId: userId, isPage: isPage, status: comment, filePath: filePath, contentType: contentType, fileName: fileName, placeId: placeId, companyId: companyId };
        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookNewPost',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }
    
    facebookService.FacebookGetAllPostInfo = function (userId, isPage, postId) {

        var request = $http({
            method: 'GET',
            url: base_url + '/Facebook/FacebookGetAllPostInfo?userId=' + userId + '&isPage=' + isPage + '&postId=' + postId
        }).then(function (res) {

            return res;
        });
        return request;
    }

    facebookService.FacebookGetPageReportStats = function (pageId, from, to) {
        //var request = $http({
        //    method: 'GET',
        //    url: base_url + '/Facebook/FacebookGetAllPostInfo?pageId=' + pageId + '&from=' + from + '&to=' + to
        //}).then(function (res) {

        //    return res;
        //});
        var _data = { PageId: pageId, From: from, To: to};
        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookGetPageReportStats',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });        
        return request;
    }

    facebookService.FacebookGetPageChatConversation = function (conId) {

        var request = $http({
            async: true,
            method: 'GET',
            url: base_url + '/Facebook/FacebookGetUserConversation?conId=' + conId
        }).then(function (res) {
            return res;
        });
        return request;
    }

    facebookService.FacebookPostPageChatMessage = function (userId, conId, isPage, status, companyId) {
        var _data = { userId: userId, conId: conId, isPage: isPage, status: status, companyId: companyId };
        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookNewPageChat',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;

    }

    facebookService.FacebookGetPostInfo = function (postId, commentId) {

        var request = $http({
            method: 'GET',
            url: base_url + '/Facebook/FacebookGetPostInfo?postId=' + postId + '&commentId=' + commentId
        }).then(function (res) {
            return res;
        });
        return request;
    }

    facebookService.FacebookGetMoreRecentPosts = function (pageid, lastPostId, startDate, endDate) {
        var _data = { PageId: pageid, lastPostId: lastPostId, From: startDate, To: endDate };
        var request = $http({
            method: 'POST',
            url: base_url + '/Facebook/FacebookGetMoreRecentPosts',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
        return request;
    }

    return facebookService;
});