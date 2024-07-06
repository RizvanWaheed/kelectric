using CRM.Common.Enum;
using SocialAPIsWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedInbox
{
    public class PostTweetModel
    {
        public string fromUser { get; set; }
        public string status { get; set; }
        public string authorUserName { get; set; }
        public string mediaString { get; set; }
        public ulong tweetId { get; set; }
        public List<MediaModelTwitter> mediaInfo { get; set; }
        public int companyId { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class FacebookPostModel
    {
        public string userId { get; set; }
        public bool isPage { get; set; }
        public string status { get; set; }
        public string filePath { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public string postId { get; set; }
        public int companyId { get; set; }
        public int ReplyType { get; set; }
        public string placeId { get; set; }
    }

}