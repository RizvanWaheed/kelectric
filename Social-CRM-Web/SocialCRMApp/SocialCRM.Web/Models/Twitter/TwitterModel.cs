using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Twitter
{
    public class TwitterDirectMessageModel
    {
        public string userId { get; set; }
        public string fromUser { get; set; }
        public string toUser { get; set; }
        public string status { get; set; }
        public int companyId { get; set; }
        public int feedId { get; set; }
        public int groupId { get; set; }
        public string replyFrom { get; set; }
    }

    public class TwitterProfileReportInputModel
    {
        public string[] Profiles { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
    public class TwitterProfileDrillDownReportInputModel
    {
        public string[] Profiles { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string ReportType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class TwitterProfileReportModel
    {
        public int TotalTweets { get; set; }
        public int TotalReplies { get; set; }
        public int TotalTweetsSent { get; set; }
        public int TotalDMsSent { get; set; }
        public int TotalMentionsReceived { get; set; }
        public int TotalDMsReceived { get; set; }
        public int BasicTweetsCount { get; set; }
        public int PhotoTweetsCount { get; set; }
        public int VideoTweetsCount { get; set; }
        public int ShareTweetsCount { get; set; }
        public int TotalProfileTweets { get; set; }
        public int TotalProfileDMs { get; set; }
        public int TotalEngTweets { get; set; }
        public int TotalEngReplies { get; set; }
        public int TotalEngRetweets { get; set; }
        public int LastTotalTweetsSent { get; set; }
        public int LastTotalDMsSent { get; set; }
        public int LastTotalMentionsReceived { get; set; }
        public int LastTotalDMsReceived { get; set; }
        public int LastTotalEngTweets { get; set; }
        public int LastTotalEngReplies { get; set; }
        public int LastTotalEngRetweets { get; set; }
        public List<DateWiseActivityCount> TweetsSentSpan { get; set; }
        public List<DateWiseActivityCount> DMsSentSpan { get; set; }
        public List<DateWiseActivityCount> MentionsReceivedSpan { get; set; }
        public List<DateWiseActivityCount> DMsReceivedSpan { get; set; }
        public List<DateWiseActivityCount> EngTweetsSpan { get; set; }
        public List<DateWiseActivityCount> EngRepliesSpan { get; set; }
        public List<DateWiseActivityCount> EngRetweetsSpan { get; set; }
        public List<TwitterTopCustomers> TopFiveCustomers { get; set; }
    }
    public class TwitterProfileDrillDownReportModel
    {
        public DateTime? InsertionDate { get; set; }
        public string StatusText { get; set; }
    }
        public class DateWiseActivityCount
    {
        public DateTime? DateValue { get; set; }
        public int ActivityCount { get; set; }
    }

    public class TwitterTopCustomers
    {
        public string CustomerId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public string Gender { get; set; }
        public int Count { get; set; }
        public int MsgCount { get; set; }
        public int PostCount { get; set; }
    }


    public class TwitterTweetThread
    {
        public List<InboxItemModel> ParentTweets { get; set; }
        public InboxItemModel Tweet { get; set; }
        public List<InboxItemModel> ChildTweets { get; set; }
    }

    public class TwitterConversationModel
    {
        public string fromUser { get; set; }
        public string toUser { get; set; }
    }
}