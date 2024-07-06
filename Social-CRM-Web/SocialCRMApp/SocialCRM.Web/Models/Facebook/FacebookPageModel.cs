using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Facebook
{
    public class FacebookPageModel
    {
        public int Id { get; set; }
        public long AccountId { get; set; }
        public string ProfileId { get; set; }
        public string AccountName { get; set; }
        public string AccountUrl { get; set; }
        public bool? IsConnected { get; set; }
    }

    public class FacebookPageChatMessageModel
    {
        public string userId { get; set; }
        public string conId { get; set; }
        public string status { get; set; }
        public bool isPage { get; set; }
        public int companyId { get; set; }
        public int feedId { get; set; }
        public int groupId { get; set; }
        public string replyFrom { get; set; }
    }

    public class FacebookPageReportInputModel
    {
        public string PageId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int lastPostId { get; set; }
    }

    public class DemographicModel
    {
        public string AgeRange { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
    }

    public class FacebookPageReportModel
    {
        public int TotalPageLikes { get; set; }
        public int TotalPageUnLikes { get; set; }
        public int TotalPostLikes { get; set; }
        public int TotalPostComments { get; set; }
        public int TotalPostShares { get; set; }
        public int TotalPublishedPagePosts { get; set; }
        public int TotalPublishedUserPosts { get; set; }
        public int TotalLinkClicks { get; set; }
        public int LastTotalPageLikes { get; set; }
        public int LastTotalPagePosts { get; set; }
        public int LastTotalPublishedPagePosts { get; set; }
        public int LastTotalPublishedUserPosts { get; set; }
        public int LastTotalPostLikes { get; set; }
        public int LastTotalPostComments { get; set; }
        public int LastTotalPostShares { get; set; }
        public List<DateWiseActivityCount> PageLikeSpan { get; set; }
        public List<DateWiseActivityCount> PageUnlikeSpan { get; set; }
        public List<DateWiseActivityCount> PostLikeSpan { get; set; }
        public List<DateWiseActivityCount> PostShareSpan { get; set; }
        public List<DateWiseActivityCount> PostCommentSpan { get; set; }
        public List<DateWiseActivityCount> PublishedPagePostSpan { get; set; }
        public List<DateWiseActivityCount> PublishedUserPostSpan { get; set; }
        public List<DateWiseActivityCount> LinkClicksSpan { get; set; }
        public List<DemographicModel> PageFansGenderAge { get; set; }
        public List<DateWiseActivityCount> TotalPeopleWhoViewedSpan { get; set; }
        public List<pageReactionsTotal> pageReactionsSpan { get; set; }
        public List<DateWiseActivityCount> pageReachSpan { get; set; }
        public List<recentPost> lstRecentPosts { get; set; }
        public string loadMoreRecentPostsLink { get; set; }
        public int lastPostId { get; set; }
        public List<externalLink> lstExternalReferrals { get; set; }
        public List<FacebookTopCustomers> TopFiveCustomers { get; set; }
    }

    public class DateWiseActivityCount
    {
        public DateTime DateValue { get; set; }
        public int ActivityCount { get; set; }
    }

    public class FacebookTopCustomers
    {
        public string CustomerId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public string Gender { get; set; }
        public int Count { get; set; }
        public int PostCount { get; set; }
        public int CommentCount { get; set; }
        public int MsgCount { get; set; }
    }

    public class pageReactionsTotal
    {
        public int like { get; set; }
        public int love { get; set; }
        public int wow { get; set; }
        public int haha { get; set; }
        public int sorry { get; set; }
        public int anger { get; set; }
        public DateTime totalReactionsDateValue { get; set; }
    }

    public class recentPost
    {
        public int Id { get; set; }
        public string postId { get; set; }
        public DateTime createdDate { get; set; }
        public string strCreatedDate { get; set; }
        public string strCreatedTime { get; set; }
        public string messageText { get; set; }
        public string pictureURL { get; set; }
        public string type { get; set; }
        public int postReach { get; set; }
        public int postClicks { get; set; }
        public int postReactions { get; set; }
        public int postComments { get; set; }
        public int postShares { get; set; }
        public double engagementRate { get; set; }        
        public string postGrade { get; set; }
    }

    public class externalLink
    {
        public string link { get; set; }
        public int count { get; set; }
    }

    public class recentPosts
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string PageId { get; set; }
        public string Description { get; set; }
        public string EngagmentRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PostId { get; set; }
    }

    public class FacebookUserAccount
    {
        public string UserId { get; set; }
        public string PageId { get; set; }
        public string AccessToken { get; set; }
        public string ProfilePic { get; set; }        
    }
}