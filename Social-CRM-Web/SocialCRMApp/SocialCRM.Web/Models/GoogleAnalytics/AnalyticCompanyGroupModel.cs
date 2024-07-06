using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.GoogleAnalytics
{
    public class AnalyticCompanyGroupModel
    {
        public int AnalyticId { get; set; }
        public int GroupId { get; set; }
        public int CompanyId { get; set; }
        public bool isGroupConnected { get; set; }
    }
}