using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.GoogleAnalytics
{
    public class AnalyticsResultModel
    {
        public string Visits { get; set; }
        public string Bounces { get; set; }
        public string BounceRate { get; set; }
        public string Users { get; set; }
        public string Traffic { get; set; }
    }
}