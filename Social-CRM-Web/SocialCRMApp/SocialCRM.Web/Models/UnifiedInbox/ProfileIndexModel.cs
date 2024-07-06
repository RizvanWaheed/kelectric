using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedInbox
{
    public class ProfileIndexModel
    {
        public string ProfileIco { get; set; }
        public string HandleName { get; set; }
        public int Id { get; set; }
    }

    public class TagModel
    {
        public int Id { get; set; }
        public string KeywordText { get; set; }
        public string KeywordName { get; set; }
        public string FontColor { get; set; }
        public string BGColor { get; set; }
        public int GroupId { get; set; }
    }
}