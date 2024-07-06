using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Publisher
{
    public class ProfileQueueViewModel
    {
        public string Time { get; set; }
        public string Day { get; set; }
        public int ProfileCompanyGroupId { get; set; }
        public int CreatorUserId { get; set; }
        public int CompanyId { get; set; }
    }

    public class ProfileQueueModel
    {
        public string PostData { get; set; }
        public int CompanyId { get; set; }
    }
}