using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.BO
{
    public class TwitterTweet
    {
        public Object StatusId { get; set; }
        public Object AuthorUserId { get; set; }
        public Object StatusText { get; set; }
        public Object AuthorName { get; set; }
        public Object InReplyToStatusId { get; set; }
        public Object InReplyToUserId { get; set; }
        public Object InReplyToScreenName { get; set; }
        public Object CreatedDate { get; set; }
        public Object AuthorProfilePic { get; set; }
        public Object IsAuthorVerified { get; set; }
        public Object IsMention { get; set; }
        public Object IsRetweet { get; set; }
    }
}
