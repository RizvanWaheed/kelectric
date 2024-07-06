CRMApp.factory('GroupService', function ($http) {
    var groupService = {};

    groupService.GetTeamByGroupId = function (id) {
        var requestUrl = base_url + '/api/Team/GetByGroupId/' + id;
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

    groupService.GetTeamByGroupIdForTaskList = function (id) {
        var requestUrl = base_url + '/api/Team/GetByGroupIdForTaskList/' + id;
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

    groupService.GetById = function (id) {
        var requestUrl = base_url + '/api/Group/GetById/' + id;
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

    groupService.GetByUserId = function (id) {
        var requestUrl = base_url + '/api/Group/GetByUserId/' + id;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
    groupService.GetUserCompanyGroups = function () {
        var requestUrl = base_url + '/api/Group/GetUserCompanyGroups';
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
    groupService.GetByCompanyId = function (id) {
        var requestUrl = base_url + '/api/Group/GetByCompanyId/' + id;
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

    groupService.GetGroupByUserId = function (id) {
        var requestUrl = base_url + '/api/Group/GetGroupByUserId/' + id;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
    groupService.GetGroupByCompanyId = function (id) {
        var requestUrl = base_url + '/api/Group/GetGroupByCompanyId/' + id;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    groupService.RegisterGroup = function (GroupName, CompanyId, IsActive, profiledata) {

        //var dataP = "[{ profileId: 1, ProfileType: 1 }, { profileId: 2, ProfileType: 2 }, { profileId: 4, ProfileType: 3 }]"
        var _data = { GroupName: GroupName, CompanyId: CompanyId, Profiles: profiledata };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Group/RegisterGroup',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }
    groupService.UpdateGroup = function (GroupName,   GroupId) {
        var _data = { GroupName: GroupName,  GroupId: GroupId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Group/UpdateGroup',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;
    }
    groupService.DeleteGroup = function (id) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Group/DeleteGroup/' + id
        }).then(function (res) {

            return res;
        });
        return request;
    }

    groupService.GetTeamGroupProfileCount = function (groupid) {
        var requestUrl = base_url + '/api/Company/GetTeamGroupProfileCount/' + groupid;
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
    return groupService;
});