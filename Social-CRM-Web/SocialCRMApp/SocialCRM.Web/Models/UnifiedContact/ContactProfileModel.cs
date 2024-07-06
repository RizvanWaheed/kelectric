using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedContact
{
    public class ContactProfileModel
    {
        public string contactProfilePic { get; set; }
        public string contactExternalId { get; set; }
        public string contactName { get; set; }
        public string contactTwitName { get; set; }
        public string contactMsg { get; set; }
        public string contactCountry { get; set; }
        public string contactURL { get; set; }
        public int totalFollower { get; set; }
        public int totalFollowing { get; set; }
        public bool isContactVerified { get; set; }
    }
}