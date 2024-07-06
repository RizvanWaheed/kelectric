using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedDeck
{
    public class SentimentDataModel
    {
        public int feedId { get; set; }
        public string feedType { get; set; }
        public string sentiment { get; set; }
        public int companyId { get; set; }
    }
}