using SocialAPIsWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Publisher
{
    /// <summary>
    /// This model is used for schedule a message.
    /// </summary>
    public class ScheduledMessageModel
    {
        public string MessageText { get; set; }
        public List<int> SelectedProfiles { get; set; }
        public List<bool> SelectedProfilesType { get; set; }

        public List<string> PostTimes { get; set; }
        public bool IsDraft { get; set; }
        public bool IsScheduledMessage { get; set; }
        public bool IsPreConfiguredQueue { get; set; }
        public bool IsQueueAtBeginning { get; set; }
        public int CreatorUserId { get; set; }
        public int CompanyId { get; set; }
        public List<int> MessageTypes { get; set; }
        public int MessageStatus { get; set; }
        public int ScheduledMessageId { get; set; }
        public int PageNumber { get; set; }
        public bool getLatest { get; set; }
        public List<MediaModelTwitter> mediaInfo { get; set; }
    }
}