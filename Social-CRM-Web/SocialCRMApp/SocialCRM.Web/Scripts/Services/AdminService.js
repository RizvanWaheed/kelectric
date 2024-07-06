CRMApp.factory('AdminService', function ($http) {

    var adminService = {};

    adminService.ChangeCompanyStatus = function (companyId, status) {
        var requestUrl = base_url + '/api/Admin/ChangeCompanyStatus/' + companyId + '/' + status;
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

    
    adminService.GetCompanyDetails = function () {
        var requestUrl = base_url + '/api/Admin/GetCompanyDetails/';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            
            return response;
        }, function errorCallback(response) {
            
        });
        return request;
    }

    adminService.GetMembershipPackage = function (subscriberId) {
        var requestUrl = base_url + '/api/Admin/GetMembershipPackage/' + subscriberId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            
            return response;
        }, function errorCallback(response) {
            
        });
        return request;
    }

    adminService.UpdateMembershipPackage = function (packageValue) {

        var _data = { SubscriberId: packageValue.SubscriberId, Permissions: packageValue.Permissions, AccessLevels: packageValue.AccessLevels, Restrictions: packageValue.Restrictions };
        var requestUrl = base_url + '/api/Admin/UpdateMembershipPackage';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: angular.toJson(_data)
        }).then(function successCallback(response) {
            
            return response;
        }, function errorCallback(response) {
            
        });
        return request;
    }
    //Return this service
    return adminService;
});