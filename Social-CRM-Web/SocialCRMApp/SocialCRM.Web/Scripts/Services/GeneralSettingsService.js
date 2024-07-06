CRMApp.factory('GeneralSettingsService', function ($http) {
    var generalSettingsService = {};

    generalSettingsService.UpdateSampleReply = function (Id, ReplyText) {
        var _data = { Id: Id, ReplyText: ReplyText };
        var requestUrl = base_url + '/api/GeneralSettings/UpdateSampleReply/';
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
    };

    generalSettingsService.DeleteSampleReply = function (Id) {
        var requestUrl = base_url + '/api/GeneralSettings/DeleteSampleReply/' + Id;
        var request = $http({
            method: 'POST',
            url: requestUrl,
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    };

    generalSettingsService.GetSocialMediaUserTypes = function () {
        var requestUrl = base_url + '/api/GeneralSettings/GetSocialMediaUserTypes/';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {           
            return response;
        }, function errorCallback(response) {            
        });
        return request;
    };

    generalSettingsService.AddUpdateEmailSettings = function (ID, Name, Email, Over10KFollowers, PoliticalFigures, VerifiedAccounts, TopInfluencers, ReceiveReport, CompanyId) {        
        var _data = { ID: ID, Name: Name, Email: Email, Over10KFollowers: Over10KFollowers, PoliticalFigures: PoliticalFigures, VerifiedAccounts: VerifiedAccounts, TopInfluencers: TopInfluencers, ReceiveReport: ReceiveReport, CompanyId: CompanyId };
        var requestUrl = base_url + '/api/GeneralSettings/AddUpdateEmailSettings/';
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

    generalSettingsService.GetSocialMediaUsersUpdateReceiversList = function () {
        var requestUrl = base_url + '/api/GeneralSettings/GetSocialMediaUsersUpdateReceiversList/';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    };

    generalSettingsService.DeleteEmailSettings = function (ID) {
        var _data = { ID: ID };
        var requestUrl = base_url + '/api/GeneralSettings/DeleteEmailSettings/';
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

    generalSettingsService.GetInfluencersList = function (groupId, pageNumber, getOptions) {
        var requestUrl = base_url + '/api/GeneralSettings/GetInfluencersList/' + groupId + '/' + pageNumber + '/' + getOptions;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    };

    generalSettingsService.SaveInfluencer = function (Id, CustomerProfileHandle, ProfileId, AccountTypeId, PlatformId) {
        var _data = { Id: Id, CustomerProfileHandle: CustomerProfileHandle, ProfileId: ProfileId, AccountTypeId: AccountTypeId, PlatformId: PlatformId };
        var requestUrl = base_url + '/api/GeneralSettings/SaveInfluencer/';
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

    generalSettingsService.DeleteInfluencer = function (Id) {
        var requestUrl = base_url + '/api/GeneralSettings/DeleteInfluencer/' + Id;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    };

    return generalSettingsService;
});
