using SocialAPIsWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Publisher
{
    public class ScheduledMessages
    {
        public int Id { get; set; }
        public string MessageText { get; set; }
        public string CreatedDate { get; set; }
        public DateTime PublishDate { get; set; }
        public string PostingDate { get; set; }
        public string PostingTime { get; set; }
        public string PostingDay { get; set; }
        public int AuthorTeamMemberId { get; set; }
        public string AuthorName { get; set; }        
        public int ProfileTypeId { get; set; }
        public string Username { get; set; }
        public string ProfileURL { get; set; }
        public int MessageStatus { get; set; }
        public int MessagePostingTimeId { get; set; }
        public List<MediaModelTwitter> mediaInfo { get; set; }
    }
}