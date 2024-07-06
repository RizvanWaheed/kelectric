using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedContact
{
    public class ContactFollowStatusModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string ProfileExternalId { get; set; }
        public string ProfilePicURL { get; set; }
        public bool IsFollowingContact { get; set; }
        public bool IsFollowedByContact { get; set; }
        public bool IsPage { get; set; }
    }
}