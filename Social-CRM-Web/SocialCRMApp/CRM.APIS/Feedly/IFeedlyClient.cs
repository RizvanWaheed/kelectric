using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.APIS.Feedly.FeedlySharp
{
    public interface IFeedlyClient
    {
        void Activate(string accessToken, string userId);
        System.Threading.Tasks.Task<bool> AddOrUpdateSubscription(string id, System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyCategory> categories = null, string optionalTitle = null, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> AddOrUpdateTopic(string topicId, CRM.APIS.Feedly.Models.Interest interest = CRM.APIS.Feedly.Models.Interest.Low, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task DeleteCategory(string id, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        void Dispose();
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlySearchResponse> FindEntries(string contentId, string searchQuery, DateTime? newerThan = null, string continuation = null, string fields = null, string embedded = null, string engagement = null, int? count = null, string locale = null, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlySearchFeed>> FindFeeds(string searchQuery, int? count = null, string locale = null, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        string GetAuthenticationUri(string scope = "subscriptions", string state = default(String));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyCategory>> GetCategories(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyEntry>> GetEntries(string[] ids, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlyEntry> GetEntry(string id, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlyFeed> GetFeed(string id, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyFeed>> GetFeeds(string[] ids, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlyReadOperations> GetMarkersReadOperations(DateTime newerThan, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyCategoryUnreadCount>> GetMarkersUnreadCount(bool isAutoRefresh = false, DateTime? newerThan = null, string categoryId = null, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyEntry>> GetMixes(string contentId, int? count = null, bool unreadOnly = false, int? limitHours = null, DateTime? newerThan = null, bool backfill = true, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<string> GetOPML(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string>> GetPreferences(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlyUser> GetProfile(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlyStreamEntriesResponse> GetStreamEntries(string id, ContentType type, int? count = null, FeedSorting sorting = FeedSorting.Newest, bool? unreadOnly = null, DateTime? newerThan = null, string continuation = null, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlyStreamEntryIdsResponse> GetStreamEntryIds(string id, ContentType type, int? count = null, FeedSorting sorting = FeedSorting.Newest, bool? unreadOnly = null, DateTime? newerThan = null, string continuation = null, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlySubscription>> GetSubscriptions(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyTag>> GetTags(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.List<CRM.APIS.Feedly.Models.FeedlyTopic>> GetTopics(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> ImportOPML(string opml, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> KeepEntriesAsUnread(string[] entryIds, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> KeepEntryAsUnread(string entryId, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkCategoriesAsRead(string[] categoryIds, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkCategoryAsRead(string categoryId, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkEntriesAsRead(string[] entryIds, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkEntriesAsSaved(string[] entryIds, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkEntriesAsUnsaved(string[] entryIds, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkEntryAsRead(string entryId, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkEntryAsSaved(string entryId, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkEntryAsUnsaved(string entryId, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkFeedAsRead(string feedId, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> MarkFeedsAsRead(string[] feedIds, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        CRM.APIS.Feedly.Models.AuthenticationResponse ParseAuthenticationResponseUri(string uri);
        CRM.APIS.Feedly.Models.AuthenticationResponse ParseAuthenticationResponseUri(Uri uri);
        System.Threading.Tasks.Task<bool> RemoveSubscription(string id, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> RemoveTags(string[] entryIds, string[] tags, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> RemoveTags(string[] tags, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> RemoveTopic(string topicId, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task RenameCategory(string id, string label, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> RenameTag(string oldTag, string newTag, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.AccessTokenResponse> RequestAccessToken(string authenticationCode, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.AccessTokenResponse> RequestRefreshToken(string refreshToken, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task RevokeRefreshToken(string refreshToken, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string>> UpdatePreferences(System.Collections.Generic.Dictionary<string, string> preferences, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<CRM.APIS.Feedly.Models.FeedlyUser> UpdateProfile(System.Collections.Generic.Dictionary<string, string> parameters, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> UpdateTags(string entryId, string[] tags, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
        System.Threading.Tasks.Task<bool> UpdateTags(string[] entryIds, string[] tags, System.Threading.CancellationToken cancellationToken = default(CancellationToken));
    }
}
