using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.BO
{
    public class FacebookUserAccount
    {
        public int? ProfileId { get; set; }
        public int? ProfilePageId { get; set; }
        public string UserId { get; set; }
        public string PageId { get; set; }
        public string AccessToken { get; set; }
        public string ProfilePic { get; set; }
        public int CompanyId { get; set; }
    }

    public class CommentRelationInfo
    {
        public string PostId { get; set; }
        public string MaxCommentId { get; set; }
    }

    public class LastPostChatInfo
    {
        public DateTime? MaxPostCreationDate { get; set; }
        public DateTime? MaxConUpdatedTime { get; set; }
    }

    public class ConvMaxDates
    {
        public string ConId { get; set; }
        public DateTime? MaxCreatedDate { get; set; }
    }

    public class FBPost
    {
        public string Message { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string PostId { get; set; }
        public string MaxCommentId { get; set; }
        public string AuthorUserId { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorProfilePic { get; set; }
        public bool IsAuthorVerified { get; set; }
        public List<FeedMediaAttachment> MediaAttachment { get; set; }
        public int EngagementRate { get; set; }
        public string Grade { get; set; }
        public string PostType { get; set; }
        public string PageId { get; set; }
        public string UserId { get; set; }
        public string PostImageURL { get; set; }
    }

    public class FBComment
    {
        public string CommentId { get; set; }
        public string Message { get; set; }
        public string FromId { get; set; }
        public string FromName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FromUserProfilePic { get; set; }
        public bool IsFromVerified { get; set; }
        public string MaxCommentId { get; set; }
        public string Attachment { get; set; }
        public string ParentId { get; set; }
        public string AttachmentType { get; set; }
    }

    public class FBPageChat
    {
        public string ConId { get; set; }
        public string MsgId { get; set; }
        public string FromId { get; set; }
        public string FromName { get; set; }
        public string ToId { get; set; }
        public string ToName { get; set; }
        public string MsgText { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsFromVerified { get; set; }
        public List<FeedMediaAttachment> MediaAttachment { get; set; }
    }
}
