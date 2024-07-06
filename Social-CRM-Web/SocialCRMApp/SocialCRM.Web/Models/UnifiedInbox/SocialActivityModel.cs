using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedInbox
{
    public class SocialActivityModel
    {
        public int Id { get; set; }
        public string RetweetedStatusId { get; set; }
    }
}