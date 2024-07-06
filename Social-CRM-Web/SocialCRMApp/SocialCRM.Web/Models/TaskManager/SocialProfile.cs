using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.TaskManager
{
    public class SocialProfile
    {
        public string CustomerSocialProfileId { get; set; }
        public string ProfileName { get; set; }
        public int ComplaintCount { get; set; }
        public string ProfileType { get; set; }
    }
}