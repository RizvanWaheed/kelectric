using SocialAPIsWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Publisher
{
    public class QueuedMessage
    {
        public string MessageText { get; set; }
        public List<int> SelectedProfiles { get; set; }
        public int QueueOption { get; set; }
        public int CreatorUserId { get; set; }
        public int CompanyId { get; set; }
        public List<int> MessageTypes { get; set; }
        public int PageNumber { get; set; }
        public bool getLatest { get; set; }
        public List<MediaModelTwitter> mediaInfo { get; set; }
    }

    public class QueuedMessageReschedule
    {
        public int ScheduledMessageId { get; set; }
        public int SelectedProfiles { get; set; }        
        public int CreatorUserId { get; set; }
        public int CompanyId { get; set; }
    }
}