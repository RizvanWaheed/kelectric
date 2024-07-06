using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.BO
{
    public class TwitterAuthentication
    {

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
    }

    public class TwitterMessage
    {
        public string MsgId { get; set; }
        public string RecipientId { get; set; }
        public string RecipientScreenName { get; set; }
        public string RecipientFullName { get; set; }
        public string SenderId { get; set; }
        public string SenderScreenName { get; set; }
        public string SenderFullName { get; set; }
        public string MsgText { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SenderProfilePic { get; set; }
        public bool IsSenderVerified { get; set; }
        public string Attachment { get; set; }
        public string AttachmentType { get; set; }
        public int SenderFollowers { get; set; }
    }

    public class FeedMediaAttachment
    {
        public int Id { get; set; }
        public int MediaId { get; set; }
        public string MediaType { get; set; }
        public string Description { get; set; }
        public string AttachmentUrl { get; set; }
    }

    public class TwitterTweet
    {
        public string StatusId { get; set; }
        public string AuthorUserId { get; set; }
        public string StatusText { get; set; }
        public string AuthorName { get; set; }
        public string AuthorFullName { get; set; }
        public string InReplyToStatusId { get; set; }
        public string InReplyToUserId { get; set; }
        public string InReplyToScreenName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorProfilePic { get; set; }
        public bool IsAuthorVerified { get; set; }
        public bool IsMention { get; set; }
        public bool IsRetweet { get; set; }
        public List<FeedMediaAttachment> MediaAttachment { get; set; }
        public int AuthorFollowers { get; set; }
    }

    public class TwitterUserAccount
    {
        public int ProfileId { get; set; }
        public string ProfileExternalId { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string MaxMsgSendToId { get; set; }
        public string MaxMsgSendById { get; set; }
        public string MaxStatusId { get; set; }
        public string MaxMentionStatusId { get; set; }
        public string Cursor { get; set; }
    }

    public class SearchKeyword
    {
        public int KeywordId { get; set; }
        public string KeywordText { get; set; }
        public string LocationName { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLng { get; set; }
        public string MaxStatusId { get; set; }
    }
}
