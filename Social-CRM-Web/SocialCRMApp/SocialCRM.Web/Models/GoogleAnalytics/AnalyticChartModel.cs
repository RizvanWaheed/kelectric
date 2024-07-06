using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SocialCRM.Web.Models.GoogleAnalytics
{
    public class AnalyticChartModel
    {
        public int TotalWebsiteTraffic { get; set; }
        public string TotalBounces { get; set; }
        public string BounceRate { get; set; }
        public List<GraphModel> AnalyticGraph { get; set; }        
    }
    public class GraphModel
    {
         [ScriptIgnore]
        public string StartDate { get; set; }
         [ScriptIgnore]
        public string EndDate { get; set; }
        public string TrafficDate { get; set; }
        public int WebsiteTraffic { get; set; }
    }
}