
CRMApp.factory('ProfileService', function ($http) {
    var profileService = {};


    profileService.GetByUserId = function (id) {
        var requestUrl = base_url + '/api/Profile/GetByUserId/' + id;
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
    profileService.GetProfilesByCompanyId = function (id) {
        var requestUrl = base_url + '/api/Profile/GetProfilesByCompanyId/' + id;
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
    
    profileService.GetAllProfilesByCompanyId = function (id) {
        var requestUrl = base_url + '/api/Profile/GetAllProfilesByCompanyId/' + id;
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

    profileService.GetAllProfilesByCompanyAndUserId = function (companyId,teamMemberId) {
        var requestUrl = base_url + '/api/Profile/GetAllProfilesByMemberId/' + companyId + '/' + teamMemberId;
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

    profileService.GetByCompanyId = function (id) {
        var requestUrl = base_url + '/api/Profile/GetByCompanyId/' + id;
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
    profileService.GetByGroupId = function (id) {
        var requestUrl = base_url + '/api/Profile/GetByGroupId/' + id;
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
   
    
    profileService.GetByUserNGroupId = function (userId,groupId) {
        var requestUrl = base_url + '/api/Profile/GetByGroupId/' + userId + '/' + groupId;
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
    profileService.GetProfilesNotInGroup = function (companyId, groupId) {
        var requestUrl = base_url + '/api/Profile/GetProfilesNotInGroup/' + companyId + '/' + groupId;
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
    
  
    profileService.AssignProfileToGroup = function (GroupName,CompanyId,IsActive,GroupId) {

        var _data = { GroupName: GroupName, CompanyId: companyId, GroupId: groupId, IsActive: IsActive };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Profile/AssignProfileToGroup',
            data: JSON.stringify(_data) 
        }).then(function (res) {

            return res;
        });
        return request;

    }

    profileService.UpdateUserGroupProfle = function (TeamMemberId, CompanyId, AssignedRights, AssignedReports,Profiles,IsAdmin) {

        var _data = { TeamMemberId : TeamMemberId, CompanyId: CompanyId, AssignedRights: AssignedRights, AssignedReports: AssignedReports, Profiles: Profiles, IsAdmin: IsAdmin };
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Profile/UpdateUserGroupProfle',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }
    profileService.RemoveProfileToGroup = function (ProfileId, CompanyId, GroupId , ProfileType) {
        var _data = { ProfileId: ProfileId, CompanyId: CompanyId, GroupId: GroupId , ProfileType : ProfileType};

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Profile/RemoveProfileToGroup',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;
    }
    profileService.RemoveProfile = function (id) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Profile/RemoveProfile/' + id
           // ,data: { profileId: id }
        }).then(function (res) {

            return res;
        });
        return request;
    }
    profileService.UpdatePerssonalProfile = function (id) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Profile/UpdatePerssonalProfile/' + id
            // ,data: { profileId: id }
        }).then(function (res) {

            return res;
        });
        return request;
    }
    profileService.AssignGroup = function (_data) {
        //var _data = { ProfileId: ProfileId, CompanyId: CompanyId, ProfilesType: ProfilesType };
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Profile/AssignGroup',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;
    }
    profileService.GetTwitterProfilesByGroupId = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Profile/GetTwitterProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
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
    profileService.GetFacebookProfilesByGroupId = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Profile/GetFacebookProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
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

    profileService.GetInstagramProfilesByGroupId = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Profile/GetInstagramProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
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

    profileService.AddOutlookAccount = function (GroupId, CompanyId, IsPersonal, UserID, Passwrd, Email, Domain) {
        var _data = { GroupID: GroupId, CompanyID: CompanyId, UserId: UserID, Passwrd: Passwrd, Email: Email, Domain: Domain, Type: IsPersonal };

        var requestUrl = base_url + '/api/Profile/AddOutlookAccount';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: JSON.stringify(_data)
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }
    return profileService;
});