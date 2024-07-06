CRMApp.factory('PublisherService', function ($http) {
    var publisherService = {};

    //Implementation goes here
    publisherService.AddNewScheduledMessage = function (MessageText, SelectedProfiles, PostTimes, IsDraft, IsScheduledMessage, IsPreConfiguredQueue, IsQueueAtBeginning, CreatorUserId, CompanyId, MessageStatus, ScheduledMessageId, mediaInfo) {

        var _data = { MessageText: MessageText, SelectedProfiles: SelectedProfiles, PostTimes: PostTimes, IsDraft: IsDraft, IsScheduledMessage: IsScheduledMessage, IsPreConfiguredQueue: IsPreConfiguredQueue, CreatorUserId: CreatorUserId, CompanyId: CompanyId, MessageStatus: MessageStatus, ScheduledMessageId: ScheduledMessageId, mediaInfo: mediaInfo };
        var requestUrl = base_url + '/api/Publisher/ScheduleMessage/';
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

    publisherService.UpdateScheduledMessage = function (MessageText, SelectedProfiles, PostTimes, IsDraft, IsScheduledMessage, IsPreConfiguredQueue, IsQueueAtBeginning, CreatorUserId, CompanyId, MessageStatus, ScheduledMessageId, mediaInfo) {
        debugger
        var _data = { MessageText: MessageText, SelectedProfiles: SelectedProfiles, PostTimes: PostTimes, IsDraft: IsDraft, IsScheduledMessage: IsScheduledMessage, IsPreConfiguredQueue: IsPreConfiguredQueue, CreatorUserId: CreatorUserId, CompanyId: CompanyId, MessageStatus: MessageStatus, ScheduledMessageId: ScheduledMessageId, mediaInfo: mediaInfo };
        var requestUrl = base_url + '/api/Publisher/UpdateScheduledMessage/';
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

    publisherService.ScheduledMessageDateRange = function (MessageText, SelectedProfiles, PostTimes, IsDraft, IsScheduledMessage, IsPreConfiguredQueue, IsQueueAtBeginning, CreatorUserId, CompanyId, MessageStatus, ScheduledMessageId, mediaInfo) {

        var _data = { MessageText: MessageText, SelectedProfiles: SelectedProfiles, PostTimes: PostTimes, IsDraft: IsDraft, IsScheduledMessage: IsScheduledMessage, IsPreConfiguredQueue: IsPreConfiguredQueue, CreatorUserId: CreatorUserId, CompanyId: CompanyId, MessageStatus: MessageStatus, ScheduledMessageId: ScheduledMessageId, mediaInfo: mediaInfo };
        var requestUrl = base_url + '/api/Publisher/ScheduleMessageDateRange/';
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

    publisherService.GetAllScheduledMessages = function (SelectedProfiles, MessageTypes, CompanyId, PageNumber, getLatest) {
        debugger
        var _data = { MessageText: null, SelectedProfiles: SelectedProfiles, PostTimes: null, IsDraft: null, IsScheduledMessage: null, IsPreConfiguredQueue: null, CreatorUserId: null, CompanyId: CompanyId, MessageTypes: MessageTypes, PageNumber: PageNumber, getLatest: getLatest };
        var requestUrl = base_url + '/api/Publisher/GetAllScheduledMessages/';
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

    publisherService.LoadPreviouslySavedOrScheduledMessage = function (ScheduledMessageId) {
        debugger
        //var postData = JSON.stringify(ResultList);
        var postData = JSON.stringify(ScheduledMessageId);
        var _data = { PostData: postData };

        //var _data = { ScheduledMessageId: ScheduledMessageId };
        var requestUrl = base_url + '/api/Publisher/LoadPreviouslySavedOrScheduledMessage/';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    /************************** QUEUE *******************************/

    publisherService.AddTimeToQueue = function (ResultList) {
        debugger
        var postData = JSON.stringify(ResultList);
        var _data = { PostData: postData };
        var requestUrl = base_url + '/api/Publisher/AddTimeToQueue/';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    publisherService.DeleteAllSlotsOfQueue = function (ResultList) {
        debugger
        var postData = JSON.stringify(ResultList);
        var _data = { PostData: postData };
        var requestUrl = base_url + '/api/Publisher/DeleteAllSlotsOfQueue/';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    publisherService.GetAllQueues = function (CompanyId) {
        debugger
        var _data = { Time: null, ProfileCompanyGroupId: null, Day: null, CreatorUserId: null, CompanyId: CompanyId };
        var requestUrl = base_url + '/api/Publisher/GetAllQueues/';
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

    publisherService.ScheduleQueueMessage = function (messageText, selectedProfiles, queueOption, CompanyId, MediaInfo) {
        debugger
        var _data = { MessageText: messageText, SelectedProfiles: selectedProfiles, QueueOption: queueOption, CreatorUserId: null, CompanyId: CompanyId, mediaInfo: MediaInfo };
        var requestUrl = base_url + '/api/Publisher/ScheduleQueueMessage/';
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

    publisherService.GetAllQueuedMessages = function (SelectedProfiles, MessageTypes, PageNumber, getLatest) {
        debugger
        var _data = { MessageText: null, SelectedProfiles: SelectedProfiles, QueueOption: null, CreatorUserId: null, CompanyId: null, MessageTypes: MessageTypes, PageNumber: PageNumber, getLatest: getLatest};
        var requestUrl = base_url + '/api/Publisher/GetAllQueuedMessages/';
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

    return publisherService;
});