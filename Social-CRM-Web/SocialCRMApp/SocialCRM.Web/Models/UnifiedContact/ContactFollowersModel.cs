using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedContact
{
    public class ContactFollowersModel
    {
        public List<ContactProfileModel> followers { get; set; }
        public long cursor { get; set; }
    }
}