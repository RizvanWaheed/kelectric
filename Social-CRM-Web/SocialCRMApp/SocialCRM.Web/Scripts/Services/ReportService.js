CRMApp.factory('ReportService', function ($http) {
    var reportService = {};
    reportService.GetTaskPerformance = function (groupId, companyId, dateFrom, dateTo, dateRange) {
        var _data = { companyId: companyId, groupId: groupId, dateFrom: dateFrom, dateTo: dateTo, dateRange: dateRange };
        var requestUrl = base_url + '/api/Report/GetTaskPerformanceReport';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
        });
        return request;
    },

    reportService.GetTeamReport = function (groupId, companyId, dateFrom, dateTo, dateRange, selectedTeamMembers, isAdmin, teamMemberId) {
        var _data = { companyId: companyId, groupId: groupId, dateFrom: dateFrom, dateTo: dateTo, dateRange: dateRange, selectedTeamMembers: selectedTeamMembers, isCurrUserAdmin: isAdmin, currUserTeamId: teamMemberId};
        var requestUrl = base_url + '/api/Report/GetTeamReport';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.GetTwitterProfilePostsAndConversation = function (groupId, companyId, dateFrom, dateTo) {
        debugger
        var _data = { companyId: companyId, groupId: groupId, dateFrom: dateFrom, dateTo: dateTo, classificationId: null };
        var requestUrl = base_url + '/api/Report/GetTwitterProfilePostsAndConversation';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.CreateReportPDF = function (reportType, reportData) {
        debugger
        var _data = { reportType: reportType, reportData: reportData };
        var requestUrl = base_url + '/api/Report/CreateReportPDF';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.GetCommentsOrChat = function (teamId, profileId, profileType, activityType, dateFrom, dateTo, dateRange) {
        var _data = { teamId: teamId, profileId: profileId, profileType: profileType, activityType: activityType, dateFrom: dateFrom, dateTo: dateTo, dateRange: dateRange };
        var requestUrl = base_url + '/api/Report/GetCommentAndChatByTeam';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.GetQuickOrSlowReply = function (teamId, profileId, profileType, replyType, dateFrom, dateTo, dateRange) {
        var _data = { teamId: teamId, profileId: profileId, profileType: profileType, replyType: replyType, dateFrom: dateFrom, dateTo: dateTo, dateRange: dateRange };
        var requestUrl = base_url + '/api/Report/GetQuickOrSlowReplyByTeam';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.GetInOutBoundReport = function (data) {
        var requestUrl = base_url + '/api/Report/GetInOutBoundReport';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.GetShiftReportData = function (data) {
        var requestUrl = base_url + '/api/Report/GetShiftReport';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.EmailShiftReport = function (data) {
        var requestUrl = base_url + '/api/Report/EmailShiftReport';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.EmailCrisisReport = function (data) {
        var requestUrl = base_url + '/api/Report/EmailCrisisReport';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.GetSentimentsPerChannelsReport = function (data) {
        var requestUrl = base_url + '/api/Report/GetSentimentsPerChannels';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    reportService.UpdateCrisisData = function (isCrisis) {
        //var _data = {
        //    isCrisis: isCrisis
        //};
        var requestUrl = base_url + '/api/Report/UpdateCrisisData/' + isCrisis;
        var request = $http({
            method: 'GET',
            url: requestUrl,
          //  data: JSON.stringify(_data)
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    return reportService;
});