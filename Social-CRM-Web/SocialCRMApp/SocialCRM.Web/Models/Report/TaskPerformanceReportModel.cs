using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Report
{
    public class TaskInDates
    {
        public int NumOfTasks { get; set; }
        public string Date { get; set; }
    }

    public class TaskPerformanceByMember
    {
        public int TeamMemberId { get; set; }
        public string TeamMemberName { get; set; }
        public double TotalAssignedTasks { get; set; }
        public double TotalCompletedTasks { get; set; }
        public string FirstTouch { get; set; }
        public string ActiveTaskTime { get; set; }
        public string TaskResolution { get; set; }
        public double CompletionRatio { get; set; }
    }
    public class TaskPerformanceReportModel
    {
        public List<TaskInDates> TasksByDate { get; set; }
        public int TotalAssignedTasks { get; set; }
        public int TotalCompletedTasks { get; set; }
        public string AssignedTasksIncreased { get; set; }
        public List<TaskPerformanceByMember> TaskPerformance { get; set; }
    }

    public class ProfileReport
    {
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileUserName { get; set; }
        public string ProfileType { get; set; }
        public string ProfilePic { get; set; }
        public int TotalReplies { get; set; }
        public int CommentCount { get; set; }
        public int ChatCount { get; set; }
        public int TotalPosts { get; set; }
        public double ReplyThreadSize { get; set; }
        public string AvgReplyTime { get; set; }
        public string MedianReplyTime { get; set; }
        public string QuickestReply { get; set; }
        public string SlowestReply { get; set; }
    }

    public class ProfileAndTeamReportNew
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileType { get; set; }
        public string ProfilePic { get; set; }
        public int TotalReplies { get; set; }
        public int CommentCount { get; set; }
        public int ChatCount { get; set; }
        public int TotalPosts { get; set; }
        public double ReplyThreadSize { get; set; }
        public string AvgReplyTime { get; set; }
        public string MedianReplyTime { get; set; }
        public string QuickestReply { get; set; }
        public string SlowestReply { get; set; }
        public double ChangeInResponse { get; set; }
        public List<ProfileReport> ProfilesReport { get; set; }
        public int FirstShiftOutbound { get; set; }
        public int SecondShiftOutbound { get; set; }
        public int ThirdShiftOutbound { get; set; }
    }

    public class PublishReport
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileType { get; set; }
        public int TotalPosts { get; set; }
        public double DailyAvg { get; set; }
        public int Replies { get; set; }
        public double ReplyToPostRatio { get; set; }
    }
    public class TeamReportModel
    {
        public List<ProfileAndTeamReportNew> ReplyByTeamMember { get; set; }
        public List<ProfileAndTeamReportNew> ReplyByProfileAndTeamMemberNew { get; set; }
        public List<PublishReport> PublishByTeamMember { get; set; }
        public TeamActivityModel TeamStats { get; set; }
        public List<TeamActivityModel> TeamStatsEvolution { get; set; }
    }
    public class TeamActivityModel
    {
        public DateTime ActivityDate { get; set; }
        public int TotalFBPosts { get; set; }
        public int TotalFBComments { get; set; }
        public int TotalFBNestComments { get; set; }
        public int TotalFBPageChats { get; set; }
        public int TotalTwitTweets { get; set; }
        public int TotalTwitReplies { get; set; }
        public int TotalTwitDMs { get; set; }
        public int TotalInstaComments { get; set; }
        public int TotalDelFBPosts { get; set; }
        public int TotalDelFBComments { get; set; }
        public int TotalDelFBNestComments { get; set; }
        public int TotalDelTwitTweets { get; set; }
        public int TotalDelTwitDMs { get; set; }
        public int TotalDelInstaComments { get; set; }
        public int TotalMarkCompleted { get; set; }
        public int TotalBannedUsers { get; set; }
        public int TotalWarnedUsers { get; set; }
        public int TotalTagsAdded { get; set; }
        public int TotalSentimentsAdded { get; set; }
    }

    public class TimeModel
    {
        public double Time { get; set; }
        public string Type { get; set; }
        public string ActivityType { get; set; }
        public string ProfileId { get; set; }
        public int TeamMemberId { get; set; }
        public string Id { get; set; }
    }

    public class ThreadModel
    {
        public int Size { get; set; }
        public string Type { get; set; }
        public string ActivityType { get; set; }
        public string ProfileId { get; set; }
        public int TeamMemberId { get; set; }
    }

    public class CommentAndChatModel
    {
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileType { get; set; }
        public string ProfilePicture { get; set; }
        public string PostLink { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public ToUser ToUserProfile { get; set; }
    }

    public class ReplyModel
    {
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileType { get; set; }
        public string ProfilePicture { get; set; }
        public string PostLink { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string ResponseTime { get; set; }
        public ToUser ToUserProfile { get; set; }
    }

    public class OnDutyAgentModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CrisisReportDataModel
    {
        public int Id { get; set; }
        public bool ReportStatus { get; set; }
        public string LastOnTime { get; set; }
        public string LastOffTime { get; set; }
    }

    public class ToUser
    {
        public string ToProfileId { get; set; }
        public string ToProfileName { get; set; }
        public string ToProfileType { get; set; }
        public string ToProfilePicture { get; set; }
        public string ToProfileLink { get; set; }
    }
}