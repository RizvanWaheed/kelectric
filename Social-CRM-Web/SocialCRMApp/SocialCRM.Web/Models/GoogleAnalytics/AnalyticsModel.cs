using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.GoogleAnalytics
{
    public class AnalyticsModel
    {
        public int Id { get; set; }
        public long AccountId { get; set; }
        public string ProfileId { get; set; }
        public string AccountName { get; set; }
        public string AccountUrl { get; set; }
        public bool IsConnected { get; set; }
        public int OwnerUserId { get; set; }
        public int AccountNumber { get; set; }
        public int CompanyId { get; set; }
    }
}