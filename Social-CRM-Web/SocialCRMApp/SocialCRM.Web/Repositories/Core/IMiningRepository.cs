using CRM.Entities;
using SocialCRM.Web.Models.Group;
using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IMiningRepository
    {
        Task<List<PostsFB>> GetFBPosts();
        Task<List<CommentsFB>> GetFBComments();
        Task<List<PostsTwitter>> GetTwitterTweets();
        Task<List<MessagesTwitter>> GetTwitterMsgs();
        Task<List<InboxItemModel>> GetUnifiedInboxForUserSelectedProfiles(int pageType, int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string brandTypeData, string sentTypeData, int pageNumber, int pageSize, string teamData, int groupId, int companyId, bool isAllTask, string consumerIds, bool showComplaints, string authorName, string authorHandle, DateTime startDate, DateTime endDate);
        Task<bool> AddMsgToArchiveList(MsgArchiveModel model);
        Task<bool> RemoveMsgFromArchiveList(MsgArchiveModel model);
        Task<int> AddSocialActivity(string profileId, string profilePageId, string feedId, string feedType, int activityTypeId, string retweetedStatusId, string comment, int UserId, int companyId, string fromUser, string toUser, string commentId);
        Task<List<InboxItemModel>> GetFeedsById(int ActivityId, string ActivityType);
        Task<List<SelectListItem>> GetCustomerSocialProfiles(string userIds, string commaSeperatedPages, int GroupId);
        Task<List<SelectListItem>> GetConsumerNumbers(string profileId, string commaSeperatedProfiles, string commaSeperatedPages, int groupId);
        Task<List<InboxItemModel>> GetData(int userId, string profileData, string profilePageData, string pageType, string msgTypeData, string tagTypeData, string brandTypeData, string sentTypeData, int pageNumber, int pageSize, string teamData, int groupId, int companyId, bool isAllTask, bool showComplaints, string consumerIds, string authorName, string authorHandle, DateTime startDate, DateTime endDate, string postIds);
        Task<SocialActivityModel> SocialActivityExists(string userId, bool isPage, string postId, string feedType, int activityTypeId);
        Task<bool> DeleteTag(int tagId);
        Task<bool> EditTag(TagModel model);
        Task<bool> DeleteKeyword(int keywordId, int GroupId);
        Task<string> GetActivePattern();
    }
}