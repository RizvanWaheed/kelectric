CRMApp.factory('TaskService', function ($http) {
    var taskService = {};
  
    taskService.Create = function (ClassificaitonOfTask, TaskPriority, Status, Assignee, ActivityId, ActivityTable, GroupId, TaskMessage) {

        var _data = { ClassificaitonOfTask: ClassificaitonOfTask, TaskPriority: TaskPriority, Status: Status, Assignee: Assignee, ActivityId: ActivityId, ActivityTable: ActivityTable, GroupId: GroupId, TaskMessage: TaskMessage };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/Create',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    taskService.Delete = function (ActivityId) {


        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/Delete/' + ActivityId
        }).then(function (res) {

            return res;
        });
        return request;

    }
    taskService.GetFilteredTask = function (SelectedClassifications, SelectedPriorities, SelectedTeamAssigneeMemberId, ProfileId, SelectedStatus, GroupId) {

        var _data = { SelectedClassifications: SelectedClassifications, SelectedPriorities: SelectedPriorities, SelectedTeamAssigneeMemberId: SelectedTeamAssigneeMemberId, ProfileId: ProfileId, SelectedStatus: SelectedStatus, GroupId: GroupId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/GetFilteredTask',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    taskService.GetFilteredDraftTask = function (GroupId) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/GetFilteredDraftTask/' + GroupId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    taskService.GetLog = function (taskId) {
        var requestUrl = base_url + '/api/Task/GetLog/' + taskId;
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

    taskService.AddComment = function (ActivityId, CommentTitle, CommentDescription,  GroupId) {

        var _data = { ActivityId: ActivityId, CommentTitle: CommentTitle, CommentDescription: CommentDescription,GroupId: GroupId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/AddComment',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }
    taskService.RemoveComment = function (commentId, groupId) {

       
        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/RemoveComment/' + commentId + '/' + groupId
        }).then(function (res) {

            return res;
        });
        return request;

    }
    taskService.GetComment = function (activityId) {
        var requestUrl = base_url + '/api/Task/GetComment/' + activityId;
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
    taskService.GetTaskClassification = function (activityId, activityTable, groupId) {
        var requestUrl = base_url + '/api/Task/GetTaskClassification/' + activityId + '/' + activityTable + '/' + groupId;
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
    taskService.AddBrandKeyword = function (KeywordText, KeywordName, LocationName, LocationLat, LocationLng, GroupId, IsTag, FontColor, BGColor, BrandKeywordCategoryId) {

        var _data = { KeywordText: KeywordText, KeywordName: KeywordName, LocationName: LocationName, LocationLat: LocationLat, LocationLng: LocationLng, GroupId: GroupId, IsTag: IsTag, FontColor: FontColor, BGColor: BGColor, BrandKeywordCategoryId: BrandKeywordCategoryId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/AddBrandKeyword',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }

    taskService.EditBrandKeyword = function (Id, KeywordText, KeywordName, LocationName, LocationLat, LocationLng, GroupId, IsTag, FontColor, BGColor, BrandKeywordCategoryId) {
        var _data = { Id: Id, KeywordText: KeywordText, KeywordName: KeywordName, LocationName: LocationName, LocationLat: LocationLat, LocationLng: LocationLng, GroupId: GroupId, IsTag: IsTag, FontColor: FontColor, BGColor: BGColor, BrandKeywordCategoryId: BrandKeywordCategoryId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/EditBrandKeyword',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }
    
    taskService.RemoveBrandKeyword = function (brandKeywordId) {

        var _data = { brandKeywordId: brandKeywordId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/RemoveBrandKeyword',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }
    taskService.GetBrandKeyword = function (groupId) {
        var requestUrl = base_url + '/api/Task/GetBrandKeyword/' + groupId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
        });
        return request;
    }
    taskService.GetBrandKeywordByCompanyId = function (companyId) {
        var requestUrl = base_url + '/api/Task/GetBrandKeywordByCompanyId/' + companyId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }
    //*****************************************************************************************
    taskService.AddBrandKeywordCategory = function (BrandKeywordCategoryText, GroupId, FontColor, BGColor) {

        var _data = { BrandKeywordCategoryText: BrandKeywordCategoryText, GroupId: GroupId, FontColor: FontColor, BGColor: BGColor };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/AddBrandKeywordCategory',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }

    taskService.EditBrandKeywordCategory = function (Id, BrandKeywordCategoryText, GroupId, FontColor, BGColor) {
        var _data = { Id: Id, BrandKeywordCategoryText: BrandKeywordCategoryText, GroupId: GroupId, FontColor: FontColor, BGColor: BGColor };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/EditBrandKeywordCategory',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }

    taskService.RemoveBrandKeywordCategory = function (brandKeywordCategoryId) {

        var _data = { Id: brandKeywordCategoryId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/RemoveBrandKeywordCategory',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }
    taskService.GetBrandKeywordCategory = function (groupId) {
        var requestUrl = base_url + '/api/Task/GetBrandKeywordCategory/' + groupId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
        });
        return request;
    }
    taskService.GetBrandKeywordCategoryByCompanyId = function (companyId) {
        var requestUrl = base_url + '/api/Task/GetBrandKeywordCategoryByCompanyId/' + companyId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });
        return request;
    }

    //************************************************************************************************
    taskService.InsertTaskCompleted = function (ActivityId, ActivityTable, IsCompleted, CompletedBy, GroupId, FeedData, CompanyId, StatusId, ProfileId, IsPage) {

        var _data = { ActivityId: ActivityId, ActivityTable: ActivityTable, IsCompleted: IsCompleted, CompletedBy: CompletedBy, GroupId: GroupId, FeedData: FeedData, CompanyId: CompanyId, StatusId: StatusId, ProfileId: ProfileId, IsPage: IsPage };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/InsertTaskCompleted',
            data: JSON.stringify(_data)
        }).then(function (res) {
            return res;
        });
        return request;
    }

    taskService.DeleteTaskCompleted = function (ActivityId, ActivityTable, GroupId) {


        var request = $http({
            method: 'POST',
            url: base_url + '/api/Task/DeleteTaskCompleted/' + ActivityId + '/' + ActivityTable + '/' + GroupId
        }).then(function (res) {

            return res;
        });
        return request;

    }

    taskService.GetProfiles = function (UserIds, CommaSeperatedPages, GroupId) {
        var requestUrl = base_url + '/api/Task/GetProfiles/' + UserIds + '/' + CommaSeperatedPages + '/' + GroupId;
        var request = $http({
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    taskService.GetCustomerTasks = function (CustomerProfileId, GroupId) {
        var requestUrl = base_url + '/api/Task/GetCustomerTasks/' + CustomerProfileId + '/' + GroupId;
        var request = $http({
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    return taskService;
});