using CRM.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Instagram
{
    public class InstagramFilterModel
    {
        public DateFilter DateRange { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int InstagramAccountId { get; set; }
        public int? GroupId { get; set; }
        public int? CompanyId { get; set; }
    }
    public class InstagramQueryModel
    {
        public string AccessTokens { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class InstagramCommentModel
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public int CompanyId { get; set; }
    }
}