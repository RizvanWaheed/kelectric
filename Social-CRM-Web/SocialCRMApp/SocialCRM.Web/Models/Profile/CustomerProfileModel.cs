using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Profile
{
    public class CustomerFileModel
    {
        public string Base64String { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public int TwitProfileId { get; set; }
    }

    public class CustomerAllInfoModel
    {
        public string ContractAccount { get; set; }
        public string EmailId { get; set; }
        public string Name { get; set; }
        public string MobileNum { get; set; }
        public string LandlineNum { get; set; }
        public string Address { get; set; }
        public string IBC { get; set; }
        public int? IBC_Code { get; set; }
        public string Region { get; set; }
        public List<CustomerSocialProfileInfoModel> SocialProfiles { get; set; }
    }

    public class CustomerSocialProfileInfoModel
    {
        public int? SocialId { get; set; }
        public string SocialExternalId { get; set; }
        public string SocialName { get; set; }
        public string SocialUserName { get; set; }
        public string SocialType { get; set; }
        public string SocialProfilePic { get; set; }
        public DateTime InsertionDate { get; set; }
        public int WarnStatus { get; set; }
        public int WarnId { get; set; }
        public int? InboundCount { get; set; }
        public int? OutboundCount { get; set; }
    }

    public class CustomerRegionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomerIBCModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Code { get; set; }
    }

    public class CustomerFilterModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int GroupId { get; set; }
        public int FBProfileId { get; set; }
        public int FBProfilePageId { get; set; }
        public int TwitProfileId { get; set; }
        public int RegionId { get; set; }
        public int AreaId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SearchOptionId { get; set; }
        public string SearchValue { get; set; }
    }

    public class RegionReportFilterModel
    {
        public int GroupId { get; set; }
        public int ReportTypeId { get; set; }
        public int ReportId { get; set; }
        public string SocialPlatform { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RegionId { get; set; }
        public bool IsEvolution { get; set; }
        public bool IsShiftReport { get; set; }
        public bool IsUnique { get; set; }
        public string XAxis { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class RegionInboundOutboundModel
    {
        public string Region { get; set; }
        public int? Total_Inbound { get; set; }
        public int? Total_Outbound { get; set; }
        public string Name { get; set; }
        public DateTime? InsertionDate { get; set; }
        public int? InsertionHour { get; set; }
    }
    public class RegionInboundOutboundDrillDownModel
    {
        public string Region { get; set; }
        public string StatusText { get; set; }
        public string PostType { get; set; }
        public string CustUserName { get; set; }
        public DateTime? InsertionDate { get; set; }
        public string IBC { get; set; }
        public long? RowNum { get; set; }
        public int? Total_Rows { get; set; }
    }
}