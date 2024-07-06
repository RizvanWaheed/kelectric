using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedContact
{
    public class ContactExtraInfoModel
    {
        public int Id { get; set; }
        public string ProfileExternalId { get; set; }
        public string UserName { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public List<ContactCommentModel> Comments { get; set; }
    }

    public class UserOwnedSocialProfiles
    {
        public int ProfileTypeId { get; set; }
        public string ProfileURL { get; set; }
    }
}