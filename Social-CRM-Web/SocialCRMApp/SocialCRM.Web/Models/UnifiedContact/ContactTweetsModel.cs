using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialCRM.Web.Models.UnifiedInbox;

namespace SocialCRM.Web.Models.UnifiedContact
{
    public class ContactTweetsModel
    {
        public List<InboxItemModel> tweets { get; set; }
        public ulong maxId { get; set; }
    }
}