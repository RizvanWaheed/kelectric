CRMApp.factory('CustomerProfileService', function ($http) {

    var customerProfileService = {};

    customerProfileService.GetUsersList = function (ReasonIds, PlatformId, Name, UserHandle, GroupId, GetOptions, PageNumber, Type) {

        var _data = { ReasonIds: ReasonIds, PlatformId: PlatformId, Name: Name, UserHandle: UserHandle, GroupId: GroupId, GetOptions: GetOptions, PageNumber: PageNumber, Type: Type };
        var requestUrl = base_url + '/api/CustomerProfile/GetUsersList/';
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

    customerProfileService.SaveBannedProfiles = function (Id, Name, ProfileUrl, ReasonId, PlatformId, Details, Profile, ProfilePage, Base64String, ContentType, FileName, StatusId, CompanyId) {

        var Attachment = new Object();
        Attachment.Base64String = Base64String;
        Attachment.ContentType = ContentType;
        Attachment.FileName = FileName;

        var _data = { Id: Id, Name: Name, ProfileUrl: ProfileUrl, ReasonId: ReasonId, PlatformId: PlatformId, Details: Details, Profile: Profile, ProfilePage: ProfilePage, Attachment: Attachment, StatusId: StatusId, CompanyId: CompanyId };
        var requestUrl = base_url + '/api/CustomerProfile/SaveBannedProfiles/';
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

    customerProfileService.UploadCustomerFile = function (FileName, ContentType, Base64String) {
        var _data = { FileName: FileName, ContentType: ContentType, Base64String: Base64String };
        var request = $http({
            method: 'POST',
            url: base_url + '/api/CustomerProfile/UploadKECustomers',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    customerProfileService.UploadSocialCustomerFile = function (FileName, ContentType, Base64String, TwitProfileId) {
        var _data = { FileName: FileName, ContentType: ContentType, Base64String: Base64String, TwitProfileId: TwitProfileId };
        var request = $http({
            method: 'POST',
            url: base_url + '/api/CustomerProfile/UploadKESocialCustomers',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    customerProfileService.DeleteBannedProfile = function (id) {
        var requestUrl = base_url + '/api/CustomerProfile/DeleteBannedProfile/' + id;
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

    customerProfileService.BanUnbanUser = function (id, actionType) {
        var requestUrl = base_url + '/api/CustomerProfile/BanUnbanUser/' + id + '/' + actionType;
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

    customerProfileService.GetBannedProfilesOptions = function (groupId) {
        var requestUrl = base_url + '/api/CustomerProfile/GetBannedProfilesOptions/' + groupId;
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

    customerProfileService.GetAttachmentAsBase64String = function (id) {
        var requestUrl = base_url + '/api/CustomerProfile/GetAttachmentAsBase64String/' + id;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    }

    customerProfileService.GetWarnBanUserReport = function (groupId, fromDate, toDate, fromTime, toTime) {

        var _data = { groupId: groupId, fromDate: fromDate, toDate: toDate, fromTime: fromTime, toTime: toTime };
        var requestUrl = base_url + '/api/CustomerProfile/GetWarnBanUserReport/';
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

    customerProfileService.WarnBanUser = function (Id, Name, ProfileUrl, ReasonId, PlatformId, Details, Profile, ProfilePage, Base64String, ContentType, FileName, StatusId, CompanyId, FeedId, FeedType) {

        var Attachment = new Object();
        Attachment.Base64String = Base64String;
        Attachment.ContentType = ContentType;
        Attachment.FileName = FileName;

        var _data = { Id: Id, Name: Name, ProfileUrl: ProfileUrl, ReasonId: ReasonId, PlatformId: PlatformId, Details: Details, Profile: Profile, ProfilePage: ProfilePage, Attachment: Attachment, StatusId: StatusId, CompanyId: CompanyId, FeedId: FeedId, FeedType: FeedType };
        var requestUrl = base_url + '/api/CustomerProfile/WarnBanUser/';
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

    customerProfileService.GetCustomerAllInfo = function (pageNumber, pageSize, groupId, fbProfileId, fbProfilePageId, twitProfileId,
        regionId, areaId, startDate, endDate, searchOptionId, searchValue) {
        var _data = {
            PageNumber: pageNumber, PageSize: pageSize, GroupId: groupId, FBProfileId: fbProfileId, FBProfilePageId: fbProfilePageId,
            TwitProfileId: twitProfileId, RegionId: regionId, AreaId: areaId, StartDate: startDate, EndDate: endDate,
            SearchOptionId: searchOptionId, SearchValue: searchValue
        };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/CustomerProfile/GetCustomerAllInfo',
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    }

    customerProfileService.GetCustomerRegions = function () {
        var requestUrl = base_url + '/api/CustomerProfile/GetCustomerRegions';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    }

    customerProfileService.GetCustomerAreas = function (regionId) {
        var requestUrl = base_url + '/api/CustomerProfile/GetCustomerAreas/' + regionId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    }

    customerProfileService.DeleteCustomer = function (contractAccount) {
        var _data = { ContractAccount: contractAccount };
        var requestUrl = base_url + '/api/CustomerProfile/DeleteCustomer';
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

    customerProfileService.UpdateCustomer = function (customerInfo) {
        var _data = customerInfo;
        var requestUrl = base_url + '/api/CustomerProfile/UpdateCustomer';
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

    customerProfileService.GetRegionReport = function (groupId, socialPlatform, startDate, endDate, reportId, reportTypeId, regionId, isEvolution, isShiftReport, isUnique) {
        var _data = {
            GroupId: groupId, SocialPlatform: socialPlatform, StartDate: startDate, EndDate: endDate, ReportId: reportId, ReportTypeId: reportTypeId,
            RegionId: regionId, IsEvolution: isEvolution, IsShiftReport: isShiftReport, IsUnique: isUnique
        };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/CustomerProfile/GetRegionReport',
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {

        });
        return request;
    }

    //Return this service
    return customerProfileService;
});