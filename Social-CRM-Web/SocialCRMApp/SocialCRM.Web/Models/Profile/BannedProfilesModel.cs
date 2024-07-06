using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Profile
{
    public class SelectItemList
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public int ProfileType { get; set; }
    }

    public class AttachmentModel
    {
        public string Base64String { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    public class BannedProfilesSaveModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ProfileUrl { get; set; }
        public int ReasonId { get; set; }
        public int PlatformId { get; set; }
        public string Details { get; set; }
        public int Profile { get; set; }
        public int ProfilePage { get; set; }
        public AttachmentModel Attachment { get; set; }
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
    }

    public class BannedProfilesListModel
    {
        public int TotalBannedRecords { get; set; }
        public int TotalWarnedRecords { get; set; }
        public List<SelectItemList> ReasonList { get; set; }
        public List<SelectItemList> PlatformList { get; set; }
        public List<SelectItemList> StatusList { get; set; }
        public List<SelectItemList> ProfilesList { get; set; }
        public List<BannedProfiles> BannedProfilesList { get; set; }
        public List<BannedProfiles> WarnedProfilesList { get; set; }
        public Dictionary<string, int> StatusWiseData { get; set; }
        public List<WarnBanReasonReportModel> ReasonWiseData { get; set; }
    }


    public class BannedProfilesOptionsListModel
    {
        public List<SelectItemList> ReasonList { get; set; }
        public List<SelectItemList> PlatformList { get; set; }
        public List<SelectItemList> StatusList { get; set; }
        public List<SelectItemList> ProfilesList { get; set; }
    }

    public class BannedProfiles
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string UserHandle { get; set; }
        public string ProfileUrl { get; set; }
        public string PostUrl { get; set; }
        public string Reason { get; set; }
        public int ReasonId { get; set; }
        public string Platform { get; set; }        
        public int PlatformId { get; set; }
        public int ProfileId { get; set; }
        public string Details { get; set; }
        public string BannedBy { get; set; }
        public bool IsAttachmentUploaded { get; set; }
        public string AttachmentName { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }

    }

    public class BannedUserSearchModel
    {
        public string ReasonIds { get; set; }
        public string PlatformId { get; set; }
        public string Name { get; set; }
        public string UserHandle { get; set; }
        public int GroupId { get; set; }
        public bool GetOptions { get; set; }
        public int PageNumber { get; set; }
        public int Type { get; set; }
    }

    public class WarnBanReasonReportModel
    {
        public string XAxisColumn { get; set; }
        public int NumOfRecords { get; set; }
    }

    public class GetWarnBanUserReportFilterModel
    {
        public int groupId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
    }

    public class ConsumerProfilesSaveModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ProfileUrl { get; set; }
        public int ReasonId { get; set; }
        public int PlatformId { get; set; }
        public string Details { get; set; }
        public string Profile { get; set; }
        public string ProfilePage { get; set; }
        public AttachmentModel Attachment { get; set; }
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
        public int FeedId { get; set; }
        public string FeedType { get; set; }
    }
}