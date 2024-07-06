
CRMApp.factory('BusinessProfileService', function ($http) {
    var BusinessProfileService = {};



    BusinessProfileService.GetBusinessProfile = function () { //companyId
        var requestUrl = base_url + '/api/User/GetBusinessProfile';
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


    BusinessProfileService.UpdateNotificationSettings = function (BusniessProfileData) {

        //var dataP = "[{ profileId: 1, ProfileType: 1 }, { profileId: 2, ProfileType: 2 }, { profileId: 4, ProfileType: 3 }]"
        var _data = BusniessProfileData;

        var request = $http({
            method: 'POST',
            url: base_url + '/api/User/UpdateNotificationSettings',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    BusinessProfileService.UpdateBusinessProfileName = function (BusniessProfileData) {

        //var dataP = "[{ profileId: 1, ProfileType: 1 }, { profileId: 2, ProfileType: 2 }, { profileId: 4, ProfileType: 3 }]"
        var _data = BusniessProfileData;

        var request = $http({
            method: 'POST',
            url: base_url + '/api/User/UpdateBusinessProfileName',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    return BusinessProfileService;
});