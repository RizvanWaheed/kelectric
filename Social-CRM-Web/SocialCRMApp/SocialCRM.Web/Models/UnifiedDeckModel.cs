using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models
{
    public class UnifiedDeckModel
    {
        public List<InboxItemModel> FacebookPostItems { get; set; }
        public List<InboxItemModel> FacebookCommentItems { get; set; }
        public List<InboxItemModel> FacebookPageChatItems { get; set; }
        public List<InboxItemModel> TwitterPostItems { get; set; }
        public List<InboxItemModel> TwitterMessageItems { get; set; }
        public List<InboxItemModel> UnrespondedItems { get; set; }
    }
}