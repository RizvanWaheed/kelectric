using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedInbox
{
    public class FBPostModel
    {
        public string AuhtorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorProfilePic { get; set; }
        public string Msg { get; set; }
        public string MsgType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Id { get; set; }
        public string StatusId { get; set; }
        public string ConId { get; set; }
        public List<string> Urls { get; set; }
        public string ConversationDate { get; set; }
        public string ToUserId { get; set; }
        public string ReplyedByUserId { get; set; }        
        public string ToUserName { get; set; }
        public string ConversationDateForDeck { get; set; }
        public bool? IsCompleted { get; set; }
        public string Images { get; set; }
        public string ImageType { get; set; }
        public List<AttachmentModel> Attachments { get; set; }

    }
}