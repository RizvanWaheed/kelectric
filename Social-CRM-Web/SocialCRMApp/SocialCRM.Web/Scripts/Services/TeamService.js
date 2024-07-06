
CRMApp.factory('TeamService', function ($http) {
    var teamService = {};


    teamService.GetMemberByUserId = function (userid, companyid) {
        var requestUrl = base_url + '/api/Team/GetMemberByUserId/' + userid + '/' + companyid;
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

    teamService.GetByCompanyId = function (companyid) {
        var requestUrl = base_url + '/api/Team/GetByCompanyId/' + companyid;
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

    teamService.GetByCompanyIdAndGroupId = function (companyid, groupid) {
        var requestUrl = base_url + '/api/Team/GetByCompanyIdAndGroupId/' + companyid + '/' + groupid;
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

    teamService.UpdateTeamMember = function (FirstName, LastName, MemberId) {
        var _data = { FirstName: FirstName, LastName: LastName, MemberId: MemberId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Team/UpdateTeamMember',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;
    }

    teamService.GetDropDown = function () {
        var requestUrl = base_url + '/api/Company/GetDropDown';
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
    teamService.Register = function (_data) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Team/Register',
            data: _data
        }).then(function (res) {

            return res;
        });
        return request;

    }
    teamService.AssignGroup = function (_data) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Team/AssignGroup',
            data:_data 
        }).then(function (res) {

            return res;
        });
        return request;

    }
    teamService.RemoveProfileToGroup = function (teamMemberId, groupId) {
        var _data = {};

        var request = $http({
            method: 'GET',
            url: base_url + '/api/Group/RemoveMember/' + teamMemberId + '/' + groupId,
            data: _data
        }).then(function (res) {

            return res;
        });
        return request;
    }

    teamService.GetTeamMembersNotInGroup = function (companyid, groupId) {
        var requestUrl = base_url + '/api/Team/GetTeamMembersNotInGroup/' + companyid + '/' + groupId;
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

    teamService.RemoveMemberFromCompany = function (teamMemberId, companyId) {
        var requestUrl = base_url + '/api/Team/RemoveMemberFromCompany/' + teamMemberId + '/' + companyId;
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

    teamService.AddMemberToCompany = function (teamMemberId, companyId) {
        var requestUrl = base_url + '/api/Team/AddMemberToCompany/' + teamMemberId + '/' + companyId;
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

    return teamService;
});