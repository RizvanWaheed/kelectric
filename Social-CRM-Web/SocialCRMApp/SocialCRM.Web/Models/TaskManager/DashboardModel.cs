using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.TaskManager
{
    public class TaskGroupPerformanceModel
    {
        public int statusId { get; set; }
        public string statusText { get; set; }
        public int? statusCount { get; set; }
    }

    public class TopInfluencerModel
    {
        public int teamMemberId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int statusId { get; set; }
        public int? statusCount { get; set; }
    }
}