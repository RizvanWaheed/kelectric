using CRM.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Report
{
    public class FilterModel
    {
        public int companyId { get; set; }
        public int classificationId { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public int groupId { get; set; }
    }

    public class GroupKeyStatsFilterModel
    {
        public int companyId { get; set; }
        public int groupId { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }

    public class TaskPerformanceReportFilterModel
    {        
        public int companyId { get; set; }
        public int groupId { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public DateFilter dateRange { get; set; }
        public string selectedTeamMembers { get; set; }
        public bool isCurrUserAdmin { get; set; }
        public int currUserTeamId { get; set; }
    }

    public class ReportModel
    {
        public string reportType { get; set; }
        public string reportData { get; set; }
    }

    public class TeamReportFilterModel
    {
        public int teamId { get; set; }
        public string profileId { get; set; }
        public string profileType { get; set; }
        public string activityType { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }        
        public DateFilter dateRange { get; set; }
    }

    public class TeamReportReplyFilterModel
    {
        public int teamId { get; set; }
        public string profileId { get; set; }
        public string profileType { get; set; }
        public string replyType { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public DateFilter dateRange { get; set; }
    }

    public class InOutBoundFilterModel
    {
        [Required]
        public int companyId { get; set; }
        [Required]
        public int groupId { get; set; }
        [Required]
        public string profileIds { get; set; }
        [Required]
        public string profilePageIds { get; set; }
        [Required]
        public string msgIds { get; set; }
        [Required]
        public string tagIds { get; set; }
        [Required]
        public string sentimentIds { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        [Required]
        public string report { get; set; }
        [Required]
        public string reportType { get; set; }
        public int? userId { get; set; }
        public int? XAxis { get; set; }
        public string XAxis2 { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class ShiftFilterModel
    {
        [Required]
        public int shiftId { get; set; }
        [Required]
        public int companyId { get; set; }
        [Required]
        public int groupId { get; set; }
        [Required]
        public DateTime date { get; set; }
        public List<int> tagIds { get; set; }
    }

    public class EmailShiftReportModel
    {
        [Required]
        public int companyId { get; set; }
        [Required]
        public string shiftReportImage { get; set; }
        [Required]
        public Shifts shift { get; set; }
        [Required]
        public string shiftTiming { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public int totalInboundTraffic { get; set; }
        public string top3Areas { get; set; }
        public string onDutyAgents { get; set; }
        public List<TagOccurance> tagOccurances { get; set; }
        [Required]
        public List<ShiftWeekData> WeekData { get; set; }
    }

    public class EmailCrisisReportModel
    {
        public int companyId { get; set; }
        public string crisisReportImage { get; set; }
        public string reportOnTime { get; set; }
        public string top3Areas { get; set; }
        public List<CrisisData> crisisFBInboundData { get; set; }
        public List<CrisisData> crisisTWInboundData { get; set; }
        public List<CrisisData> crisisTotalInboundData { get; set; }
    }


    public class InOutBoundStats
    {
        public int InboundSum { get; set; }
        public int OutboundSum { get; set; }
        public int Answered { get; set; }
        public int UnAnswered { get; set; }
    }
    public class InOutBoundReportContainer
    {
        public List<InOutBoundReport> InOutBoundReport { get; set; }
        public List<InOutBoundReport> CrisisReport { get; set; }
        public List<SentimentsPerChannel> SentimentsPerChannel { get; set; }
        public List<TagsPerChannel> TagsPerChannel { get; set; }
        public List<InOutBoundChannelReport> ChannelReport { get; set; }
        public InOutBoundStats InOutStats { get; set; }
        public string CrisisReport_Ontime { get; set; }
        public string CrisisReport_Offtime { get; set; }
    }
    public class InOutBoundReport
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public int Hour { get; set; }
        public int Month { get; set; }

        public List<SentimentsPerChannel> SentimentsPerChannel { get; set; }
        public List<TagsPerChannel> TagsPerChannel { get; set; }
    }
    public class InOutBoundReportDrillDown
    {
        public string PostType { get; set; }
        public string Volume { get; set; }
        public string StatusText { get; set; }
        public DateTime Date { get; set; }
    }
    public class InOutBoundChannelReport
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public int Hour { get; set; }
        public int Month { get; set; }
        
        public string Volume { get; set; }
    }
    public class SentimentsPerChannel
    {
        public Profiles.ProfileModel Profile { get; set; }
        public int Negative { get; set; }
        public int Neutral { get; set; }
        public int Positive { get; set; }
    }
    public class TagsPerChannel
    {
        public Profiles.ProfileModel Profile { get; set; }
        public List<string> Tags { get; set; }
        public List<int> TagCounts { get; set; }
    }
    public class ShiftReport
    {
        public int TotalInboundTraffic { get; set; }
        public List<TagOccurance> TagOccourances { get; set; }
        public string OnDutyAgents { get; set; }

        public int TwAverage { get; set; }
        public int FbAverage { get; set; }

        public List<ShiftWeekData> WeekData { get; set; }
    }
    public class TagOccurance
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Count { get; set; }
    }

    public class ShiftWeekData
    {
        public string Date { get; set; }
        public int TwData { get; set; }
        public int FbData { get; set; }
    }

    public class CrisisData
    {
        public int y { get; set; }
        public int hour { get; set; }
    }

}