using CRM.Common.Enum;
using CRM.Entities;
using SocialCRM.Web.Models;
using SocialCRM.Web.Models.UnifiedDeck;
using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IUnifiedDeckRepository
    {
        Task<UnifiedDeckModel> GetUnifiedDeckForUserSelectedProfiles(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, string pageNumber, int pageSize, int groupId, int companyId, bool isAllTask, int mediaType);
        Task<UnifiedDeckModel> GetRecentUnifiedDeckForUserSelectedProfiles(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, string pageNumber, int pageSize, int groupId, int companyId, bool isAllTask, int mediaType, string postIds);
        Task<Boolean> IsPostMarkedAsCompleted(string msgTypeData, string ActivityId);
        Task<List<InboxItemModel>> GetNextUnifiedDeckForUserSelectedProfilesUnresponded(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask, string maxUnrespondedPostIds, bool markCompleted);
        Task<bool> InsertTagsForFeed(int feedId, string feedType, string tagIds, int userId, int companyId);
        Task<bool> InsertTagsForFeed(int feedId, string feedType, int tagId, int userId, int companyId);
        Task<bool> RemoveTagFromFeed(int feedId, string feedType, int tagId);
        Task<bool> InsertSentimentForFeed(int feedId, string feedType, int sentimentId, int userId, int companyId);
        Task<List<UnifiedDeckColumnSetting>> GetColumnSettings();
        Task<bool> UpdateColumnSettings(List<UnifiedDeckColumnSetting> model);
        Task<string> DeleteFacebookItem(string userId, bool isPage, string id, FacbookReplyType type, int teamId);
        Task<bool> DeleteFacebookPageChatItem(string userId, string id);
        Task<string> DeleteTwitterTweet(string userName, string tweetId, int teamId);
        Task<string> DeleteTwitterDM(string userName, string msgId, int teamId);
        Task<string> DeleteInstagramComment(string userId, string commentId, int teamId);
        Task<List<SampleReply>> GetSampleReplies();
        Task<ConsumerProfileModel> GetConsumerData(string profile, string onProfile, bool isPage, string feedType);
        Task<List<KeyValuePair<int, int>>> GetColumnsOrderForResponder(string userId, int companyId);
        Task<List<KeyValuePair<int, int>>> UpdateColumnsOrderForResponder(int userId, int companyId, List<KeyValuePair<int, int>> receivedOrder);
    }
}
