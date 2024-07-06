
CRMApp.factory('UserService', function ($http) {
    var userService = {};


    userService.GetPersonalProfile = function (companyId) {
        var requestUrl = base_url + '/api/User/GetPersonalProfile/' + companyId;
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

    userService.GetUserLogs = function () {
        var requestUrl = base_url + '/api/User/GetUserLogs';
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

    userService.UpdatePersonalProfile = function (AssignedRights, AssignedReports, ConnectedProfiles, Name, Email, ProfileUrl) {

        var _data = { AssignedRights: AssignedRights, AssignedReports: AssignedReports, ConnectedProfiles: ConnectedProfiles, Name: Name, Email: Email, ProfileUrl: ProfileUrl };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/user/UpdatePersonalProfile',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

   
    return userService;
});