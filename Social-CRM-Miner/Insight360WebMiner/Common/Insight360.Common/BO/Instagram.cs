using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.Common
{
    public class UserFeedResponse
    {
        public class Comments
        {
            public int count { get; set; }
        }

        public class Likes
        {
            public int count { get; set; }
        }

        public class LowResolution
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Thumbnail
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class StandardResolution
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Images
        {
            public LowResolution low_resolution { get; set; }
            public Thumbnail thumbnail { get; set; }
            public StandardResolution standard_resolution { get; set; }
        }

        public class From
        {
            public string username { get; set; }
            public string profile_picture { get; set; }
            public string id { get; set; }
            public string full_name { get; set; }
        }

        public class Caption
        {
            public string created_time { get; set; }
            public string text { get; set; }
            public From from { get; set; }
            public string id { get; set; }
        }

        public class User
        {
            public string username { get; set; }
            public string profile_picture { get; set; }
            public string id { get; set; }
            public string full_name { get; set; }
        }

        public class InstagramUserMinnedFeed
        {
            //public string UserName { get; set; }
            //public string UserProfilePic { get; set; }
            public bool UserHasLiked { get; set; }
            public string PostType { get; set; }
            public string PostTags { get; set; }
            public int TotalLikes { get; set; }
            public string PostId { get; set; }
            public int TotalComments { get; set; }
            public string LowResolutionImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public string StandardResolutionImageUrl { get; set; }
            public int InstagramAccountId { get; set; }
            public string PostCreatedDate { get; set; }
            public string PostLink { get; set; }
            public long PostTimeTicks { get; set; }
            public string CaptionText { get; set; }
        }

        public class InstagramPostComments
        {
            public int Id { get; set; }
            public string PostId { get; set; }
            public string CommentId { get; set; }
            public string Message { get; set; }
            public string FromUserId { get; set; }
            public string FromUserName { get; set; }
            public string FromFullName { get; set; }
            public string FromProfilePicture { get; set; }
            public DateTime CreatedDate { get; set; }
            public string Insertiondate { get; set; }
        }
    }
    public class InstagramUserAccount
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string InstagramUserId { get; set; }
        public DateTime LastMinnedDateTime { get; set; }
        public string InstaUserName { get; set; }
    }
}
