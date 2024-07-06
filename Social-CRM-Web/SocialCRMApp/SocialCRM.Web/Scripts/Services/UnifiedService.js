CRMApp.factory('UnifiedService', function ($http) {
    var inboxService = {};
   
    inboxService.GetSelectedProfilesActivity = function (profileData, profilePageData, msgTypeData, tagTypeData, brandTypeData, sentTypeData,
        pageNumber, pageSize, pageType, teamMemberId, groupId, companyId, isAllTask, consumerId, showComplaints, authorName, authorHandle, startDate, endDate) {
        var _data = {
            profileData: profileData, profilePageData: profilePageData, msgTypeData: msgTypeData, tagTypeData: tagTypeData, brandTypeData: brandTypeData,
            sentTypeData: sentTypeData, pageNumber: pageNumber, pageSize: pageSize, pageType: pageType, teamData: teamMemberId, groupId: groupId, companyId: companyId,
            isAllTask: isAllTask, consumerId: consumerId, showComplaints: showComplaints, authorName: authorName, authorHandle: authorHandle,
            startDate: startDate, endDate: endDate
        };

        var requestUrl = base_url + '/api/Mining/GetUnifiedInboxForSelectedProfiles';
        var request = $http({
            method: 'POST',
            data: JSON.stringify(_data),
            url: requestUrl
        }).then(function successCallback(response) {
           // 
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    };

    inboxService.GetAllProfilesForUserGroup = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Mining/GetProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
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
    };

    inboxService.GetAllProfilesByCompanyId = function (companyId) {
        var requestUrl = base_url + '/api/Mining/GetProfilesByCompanyId/' + companyId;
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
    };

    inboxService.GetProfilesWithOutGroupByCompanyId = function (companyId) {
        var requestUrl = base_url + '/api/Mining/GetProfilesWithOutGroupByCompanyId/' + companyId;
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
    };

    inboxService.SocialActivityExists = function (userId, isPage, postId, feedType, activityTypeId) {
        var requestUrl = base_url + '/api/Mining/SocialActivityExists/' + userId + '/' + isPage + '/' + postId + '/' + feedType + '/' + activityTypeId;
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
    };

    inboxService.AddMsgToArchiveList = function (msgType, msgId, groupId, companyId) {
        var _data = { MsgType: msgType, MsgId: msgId, GroupId: groupId, CompanyId: companyId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Mining/AddMsgToArchiveList',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;
    };

    inboxService.RemoveMsgFromArchiveList = function (msgType, msgId, groupId, companyId) {
        var _data = { MsgType: msgType, MsgId: msgId, GroupId: groupId, CompanyId: companyId };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Mining/RemoveMsgFromArchiveList',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;
    };

    inboxService.getTwitterProfile = function (fromUser, ofUser) {
        var requestUrl = base_url + '/api/Twitter/GetTwitterProfile/' + fromUser + '/' + ofUser;
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

    inboxService.getTwitterFollowers = function (fromUser, ofUser, cursor) {
        var requestUrl = base_url + '/api/Twitter/getTwitterFollowers/' + fromUser + '/' + ofUser + '/' + cursor;
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

    inboxService.GetTwitterListMembers = function (fromUser, listId, cursor) {
        var requestUrl = base_url + '/api/Twitter/GetTwitterListMembers/' + fromUser + '/' + listId + '/' + cursor;
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

    inboxService.getTwitterFriends = function (fromUser, ofUser, cursor) {
        var requestUrl = base_url + '/api/Twitter/getTwitterFriends/' + fromUser + '/' + ofUser + '/' + cursor;
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

    inboxService.getTwitterTweets = function (fromUser, ofUser, count, maxId) {
        var requestUrl = base_url + '/api/Twitter/getTwitterTweets/' + fromUser + '/' + ofUser + '/' + count + '/' + maxId;
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

    inboxService.getTwitterHomeTweets = function (fromUser, ofUser, count, maxId) {
        var requestUrl = base_url + '/api/Twitter/GetTwitterHomeTweets/' + fromUser + '/' + ofUser + '/' + count + '/' + maxId;
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

    inboxService.getTwitterListTweets = function (fromUser, listId, count, maxId) {
        var requestUrl = base_url + '/api/Twitter/getTwitterListTweets/' + fromUser + '/' + listId + '/' + count + '/' + maxId;
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

    inboxService.getTwitterMentions = function (fromUser, ofUser, count, maxId) {
        var requestUrl = base_url + '/api/Twitter/getTwitterMentions/' + fromUser + '/' + ofUser + '/' + count + '/' + maxId;
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

    inboxService.getTwitterHistory = function (fromUser, ofUser, count, maxId, groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Twitter/GetTwitterHistory/' + fromUser + '/' + ofUser + '/' + count + '/' + maxId + '/' + groupId + '/' + companyId + '/' + isPersonal;
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
    inboxService.GetFeedsById = function (ActivityId, ActivityType) {
        var requestUrl = base_url + '/api/mining/GetFeedsById/' + ActivityId + '/' + ActivityType;
        var request = $http({
            method: 'GET',
            url: requestUrl,
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
        });

        return request;
    }
    
    inboxService.GetFriendshipInfo = function (fromUser, ofUser, groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Twitter/GetFriendshipInfo/' + fromUser + '/' + ofUser + '/' + groupId + '/' + companyId + '/' + isPersonal;
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

    inboxService.FollowUser = function (fromUser, ofUser) {
        var requestUrl = base_url + '/api/Twitter/TwitterFollowUser/' + fromUser + '/' + ofUser;
        var request = $http({
            method: 'POST',
            url: requestUrl
        }).then(function successCallback(response) {
           // 
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.UnfollowUser = function (fromUser, ofUser) {
        var requestUrl = base_url + '/api/Twitter/TwitterUnfollowUser/' + fromUser + '/' + ofUser;
        var request = $http({
            method: 'POST',
            url: requestUrl
        }).then(function successCallback(response) {
          //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.GetUserContactInfo = function (ofUser, groupId, companyId) {
        var requestUrl = base_url + '/api/Twitter/GetUserContactInfo/' + ofUser + '/' + groupId + '/' + companyId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
           // 
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.UpdateUserEmailInfo = function (profileExternalId, userName, companyId, groupId, email) {
        var _data = { ProfileExternalId: profileExternalId, UserName: userName, Email: email, CompanyId: companyId, GroupId: groupId };
        var requestUrl = base_url + '/api/Twitter/UpdateUserContactInfo';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
         //   
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.UpdateUserCompanyInfo = function (profileExternalId, userName, companyId, groupId, company) {
        var _data = { ProfileExternalId: profileExternalId, UserName: userName, Company: company, CompanyId: companyId, GroupId: groupId };
        var requestUrl = base_url + '/api/Twitter/UpdateUserContactInfo';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
          //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.UpdateUserTitleInfo = function (profileExternalId, userName, companyId, groupId, title) {
        var _data = { ProfileExternalId: profileExternalId, UserName: userName, Title: title, CompanyId: companyId, GroupId: groupId };
        var requestUrl = base_url + '/api/Twitter/UpdateUserContactInfo';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
          //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.UpdateUserPhoneInfo = function (profileExternalId, userName, companyId, groupId, phone) {
        var _data = { ProfileExternalId: profileExternalId, UserName: userName, Phone: phone, CompanyId: companyId, GroupId: groupId };
        var requestUrl = base_url + '/api/Twitter/UpdateUserContactInfo';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
          //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }
    
    inboxService.InsertUserComment = function (profileExternalId, userName, companyId, groupId, commentTitle, commentDescription) {
        var _data = {
            ProfileExternalId: profileExternalId, UserName: userName, CompanyId: companyId, GroupId: groupId,
            Comments: [{
                Title: commentTitle,
                Description: commentDescription
            }]
        };
        var requestUrl = base_url + '/api/Twitter/InsertUserContactComment';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
         //   
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.DeleteUserContactComment = function (commentId) {
        var requestUrl = base_url + '/api/Twitter/DeleteUserContactComment/' + commentId;
        var request = $http({
            method: 'POST',
            url: requestUrl
        }).then(function successCallback(response) {
         //   
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.GetUsersOwnedTwitterLists = function (ofUser, groupId, companyId, isPersonal, checkMembership) {
        var requestUrl = base_url + '/api/Twitter/GetUsersOwnedTwitterLists/' + ofUser + '/' + groupId + '/' + companyId + '/' + isPersonal + '/' + checkMembership;
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

    inboxService.GetGroupTwitterProfiles = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Profile/GetTwitterProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
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

    inboxService.GetGroupFacebookProfiles = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Profile/GetFacebookProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
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

    inboxService.GetGroupInstagramProfiles = function (groupId, companyId, isPersonal) {
        var requestUrl = base_url + '/api/Profile/GetInstagramProfilesByGroupId/' + groupId + '/' + companyId + '/' + isPersonal;
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

    inboxService.AddContactToUserLists = function (ofUser, listIds) {
        var _data = { OfUser: ofUser, ListIds: listIds };
        var requestUrl = base_url + '/api/Twitter/AddContactToUserLists';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
         //   
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.RemoveContactFromUserList = function (ofUser, list) {
        var listIds = [];
        listIds.push(list);
        var _data = { OfUser: ofUser, ListIds: listIds };
        var requestUrl = base_url + '/api/Twitter/RemoveContactFromUserList';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
          //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.CreateNewList = function (fromUser, ofUser, listName, mode, description) {
        var _data = { FromUser: fromUser, OfUser: ofUser, ListName: listName, Mode: mode, Description: description };
        var requestUrl = base_url + '/api/Twitter/CreateNewList';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
          //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.EditTwitterList = function (list) {
        var _data = {
            Id: list.Id, Name: list.Name, Description: list.Description, Slug: list.Slug, Mode: list.Mode, OwnerName: list.OwnerName,
            OwnerUserName: list.OwnerUserName, OwnerId: list.OwnerId, IsMember: list.IsMember
        };
        var requestUrl = base_url + '/api/Twitter/EditTwitterList';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
            //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.DeleteTwitterList = function (fromUser, listId) {
        var requestUrl = base_url + '/api/Twitter/DeleteTwitterList/' + fromUser + '/' + listId;
        var request = $http({
            method: 'POST',
            url: requestUrl
        }).then(function successCallback(response) {
            //  
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.DeleteTwitterTweet = function (fromUser, tweetId) {
        var requestUrl = base_url + '/api/Twitter/TwitterDeleteTweet/' + fromUser + '/' + tweetId;
        var request = $http({
            method: 'POST',
            url: requestUrl
        }).then(function successCallback(response) {
            //  
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.GetContactOwnedSocialProfiles = function (ofUser) {
        var requestUrl = base_url + '/api/Twitter/GetContactOwnedSocialProfiles/' + ofUser;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
         //   
            return response;
        }, function errorCallback(response) {
            
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    }

    inboxService.getCustomerSocialProfiles = function (UserIds, CommaSeperatedPages,GroupId) {
        var requestUrl = base_url + '/api/Mining/GetSocialProfiles/' + UserIds + '/' + CommaSeperatedPages + '/' + GroupId;
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

    inboxService.getConsumerNumbersByProfile = function (ProfileId, CommaSeperatedProfiles, CommaSeperatedPages, GroupId) {
        var requestUrl = base_url + '/api/Mining/GetConsumerNumbersByProfile/' + ProfileId + '/' + CommaSeperatedProfiles + '/' + CommaSeperatedPages + '/' + GroupId;
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

    inboxService.GetLatestUnifiedInboxData = function (profileData, profilePageData, msgTypeData, tagTypeData, brandTypeData, sentTypeData,
        pageNumber, pageSize, pageType, teamMemberId, groupId, companyId, isAllTask, consumerId, showComplaints, authorName, authorHandle, startDate, endDate, maxIds) {
        var _data = {
            profileData: profileData, profilePageData: profilePageData, msgTypeData: msgTypeData, tagTypeData: tagTypeData, brandTypeData: brandTypeData,
            sentTypeData: sentTypeData, pageNumber: pageNumber, pageSize: pageSize, pageType: pageType, teamData: teamMemberId, groupId: groupId, companyId: companyId,
            isAllTask: isAllTask, consumerId: consumerId, showComplaints: showComplaints, authorName: authorName, authorHandle: authorHandle,
            startDate: startDate, endDate: endDate, maxIds: maxIds
        };

        var requestUrl = base_url + '/api/Mining/GetLatestUnifiedInboxDataForSelectedProfiles';
        var request = $http({
            method: 'POST',
            data: JSON.stringify(_data),
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    };

    inboxService.DeleteTag = function (tagId) {
        var requestUrl = base_url + '/api/Mining/DeleteTag/' + tagId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    };

    inboxService.EditTag = function (tagId, text,name, fontColor, bgColor, groupId) {
        var _data = {
            Id: tagId, KeywordText: text, KeywordName: name, FontColor: fontColor, BgColor: bgColor, GroupId: groupId
        };
        var requestUrl = base_url + '/api/Mining/EditTag';
        var request = $http({
            method: 'POST',
            url: requestUrl,
            data: _data
        }).then(function successCallback(response) {
            //  
            return response;
        }, function errorCallback(response) {
            
        });
        return request;
    };

    inboxService.DeleteKeyword = function (keywordId, groupId) {
        var requestUrl = base_url + '/api/Mining/DeleteKeyword/' + keywordId + '/' + groupId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            // 
            return response;
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        return request;
    };

    inboxService.GetActivePattern = function () {
        var requestUrl = base_url + '/api/Mining/GetActivePattern/';
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
    };
    
    var ProfileDataObj; // get JSOBJ from anywhere: localStorage, AJAX, window, etc.
    inboxService.getProfileDataObj = function () {
        return ProfileDataObj;
    }
    return inboxService;
});