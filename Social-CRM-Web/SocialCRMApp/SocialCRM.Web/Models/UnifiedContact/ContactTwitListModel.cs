using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedContact
{
    public class ContactTwitListModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string Mode { get; set; }
        public string OwnerName { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerId { get; set; }
        public bool IsMember { get; set; }
    }

    public class AddContactToSelectedListsViewModel
    {
        public string OfUser { get; set; }
        public List<ContactTwitListModel> ListIds { get; set; }
    }

    public class NewListModel
    {
        public string FromUser { get; set; }
        public string OfUser { get; set; }
        public string ListName { get; set; }
        public string Mode { get; set; }
        public string Description { get; set; }
    }
}