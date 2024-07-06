using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedDeck
{
    public class TagDataModel
    {
        public int feedId { get; set; }
        public string feedType { get; set; }
        public int tagId { get; set; }
        public string tagIds { get; set; }
        public int companyId { get; set; }         
    }
}