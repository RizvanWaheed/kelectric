using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedInbox
{
    public class FBCommentModel
    {
        public string Id { get; set; }
        public int FeedId { get; set; }
        public string Msg { get; set; }
        public string MsgType { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateForDeck { get; set; }
        public bool IsLiked { get; set; }
        public string str_CreatedDate { get; set; }
        public string AuthorProfilePic { get; set; }
        public AttachmentModel Attachment { get; set; }
        public List<FBCommentModel> fbCommentComments { get; set; }
        public string postAuthorId { get; set; }
        public string ProfilePage { get; set; }
    }
}