using CRM.Entities;
using LinqToTwitter;
using SocialAPIsWrapper.Models;
using SocialCRM.Web.Models.Twitter;
using SocialCRM.Web.Models.UnifiedContact;
using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories.Core
{
    public interface ITwitterRepository
    {
        Task<ContactProfileModel> GetTwitterProfile(string fromUser, string ofUser);
        Task<ContactFollowersModel> GetTwitterFriends(string fromUser, string ofUser, long cursor);
        Task<ContactFollowersModel> GetTwitterFollowers(string fromUser, string ofUser, long cursor);
        Task<ContactTweetsModel> GetTwitterTweets(string fromUser, string ofUser, int count, ulong maxId);
        Task<ContactTweetsModel> GetTwitterHomeTweets(string fromUser, string ofUser, int count, ulong maxId);
        Task<ContactTweetsModel> GetTwitterMentions(string fromUser, string ofUser, int count, ulong maxId);
        Task<ContactTweetsModel> GetTwitterHistory(int userId, string ofUser, int pageNumber, int pageSize, int groupId, int companyId, bool isPersonal);
        Task<ContactTweetsModel> GetTwitterConversation(int userId, string ofUser, int pageNumber, int pageSize, int groupId, int companyId, string statusId, string Mtype);
        Task<List<ContactFollowStatusModel>> GetTwitterFriendshipStatus(string fromUser, string ofUser, int userId, int groupId, int companyId, bool isPersonal);
        Task<User> TwitterFollowUser(string fromUser, string ofUser);
        Task<User> TwitterUnfollowUser(string fromUser, string ofUser);
        Task<Status> TwitterNewTweet(string fromUser, string status, List<MediaModelTwitter> mediaInfo, string lat, string lng, int teamId, int companyId);
        Task<Status> TwitterDeleteTweet(string fromUser, ulong tweetId);
        Task<DirectMessage> TwitterSendDirectMessage(string fromUser, string toUser, string status);
        Task<DMEvent> TwitterSendDirectMessageEvent(string fromUser, string toUser, string status);
        Task<DirectMessage> TwitterDeleteDirectMessage(string fromUser, ulong messageId);
        Task<bool> TwitterDeleteDirectMessageEvent(string fromUser, ulong messageId);
        Task<Status> TwitterReplyTweet(string fromUser, ulong tweetId, string status, List<MediaModelTwitter> mediaInfo, int userId,int companyId);
        Task<Status> TwitterRetweet(string fromUser, ulong tweetId);
        Task<Status> TwitterUnRetweet(string fromUser, ulong retweetedStatusId);
        Task<Status> TwitterRetweetWithComment(string fromUser, string authorUserName, ulong tweetId, string status, List<MediaModelTwitter> mediaInfo, int userId, int companyId);
        Task<Status> TwitterMarkFavorite(string fromUser, string tweetId);
        Task<bool> TwitterIsFavorited(string fromUser, string tweetId);
        Task<string> TwitterIsRetweeted(string fromUser, ulong tweetId);
        Task<Status> TwitterUnmarkFavorite(string fromUser, string tweetId);
        Task<User> TwitterReportSpam(string fromUser, ulong userId);
        Task<ContactExtraInfoModel> GetUserContactInfo(string username, int groupId, int companyId);
        Task<bool> UpdateUserContactInfo(ContactExtraInfoModel contactObj);
        Task<ContactCommentModel> InsertUserContactComment(ContactExtraInfoModel contactObj);
        Task<bool> DeleteUserContactComment(int commentId);
        Task<List<ContactTwitListModel>> GetUsersOwnedTwitterLists(string ofUser, int userId, int groupId, int companyId, bool isPersonal, bool checkMembership);
        void AddContactToUserLists(string ofUser, List<ContactTwitListModel> listIds);
        void RemoveContactFromUserList(string ofUser, ContactTwitListModel ulist);
        Task<ContactTwitListModel> CreateNewList(string fromUser, string ofUser, string listName, string mode, string description);
        Task<List<UserOwnedSocialProfiles>> GetContactOwnedSocialProfiles(string ofUser);
        Task<ContactFollowersModel> GetTwitterListMembers(string fromUser, ulong listId, long cursor);
        Task<ContactTweetsModel> GetTwitterListTweets(string fromUser, ulong listId, int count, ulong maxId);
        void EditTwitterList(ContactTwitListModel listModel);
        void DeleteTwitterList(string fromUser, ulong listId);
        Task<List<InboxItemModel>> GetTweetDirectReplies(string fromUser, string ofUser, ulong tweetId);
        Task<List<InboxItemModel>> GetTweetMinnedReplies(string ofUser, ulong tweetId);
        Task<List<InboxItemModel>> GetTwitterDirectMessages(string fromUser, string toUser);
        Task<DirectMessage> TwitterSendDM(string userId, string fromUser, string toUser, string status, int teamId, int companyId);
        Task<DMEvent> TwitterSendDMEvent(string userId, string fromUser, string toUser, string status, int teamId, int companyId);
        Task<TwitterProfileReportModel> GetProfilesReportStats(string[] profiles, DateTime from, DateTime to);
        Task<TwitterProfileReportModel> GetProfilesReportStatsED(string[] profiles, DateTime from, DateTime to);
        Task<TwitterTweetThread> GetTweetThread(string ofUser, ulong tweetId);
        Task<List<TwitterProfileDrillDownReportModel>> GetProfilesReportStatsEDDrillDown(string[] profiles, DateTime fromDate, DateTime toDate, string ReportType , int PageNumber , int PageSize);
    }
}