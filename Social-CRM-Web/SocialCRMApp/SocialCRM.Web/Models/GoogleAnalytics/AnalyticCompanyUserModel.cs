using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.GoogleAnalytics
{
    public class AnalyticCompanyUserModel
    {
        public int AnalyticId { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public int TeamMemberId { get; set; }
        public bool IsConnected { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}