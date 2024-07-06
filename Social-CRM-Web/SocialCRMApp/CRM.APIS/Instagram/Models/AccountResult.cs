using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.APIS.Instagram.Models
{
    public class AccountResult
    {
        public TopInstagramPost TopPost { get; set; }
        public PublishingBehavior PublishedBehavior { get; set; }
        public AudienceEngagement AudienceEngagement { get; set; }
        public List<ProfileStatics> StatsByProfiles { get; set; }
    }   
    public class TopInstagramPost
    {
        public List<InstagramPost> InstagramPost { get; set; }

        public int TotalFollowers { get; set; }
        public int TotalFollowersGained { get; set; }
        public int TotalFollowed { get; set; }
        public int TotalFollowerIncreasedBy { get; set; }
    }
    public class InstagramPost
    {
        public string PostImageUrl { get; set; }
        public string AccountName { get; set; }
        public int Engagements { get; set; }
    }
    public class PublishingBehavior
    {
        public int Photos { get; set; }
        public int Videos { get; set; }
        public List<HashTags> UsedHashtags { get; set; }
        public List<HashTags> EngagedHashtags { get    ; set; }
    }
    public class HashTags
    {
        public string Name { get; set; }
        public int count { get; set; }
    }
    public class AudienceEngagement
    {
        public int LikesReceived { get; set; }
        public int CommentReceived { get; set; }
        public int EngagementByMedia { get; set; }
        public int EngagementByFollower { get; set; }
    }
    public class ProfileStatics
    {
        public string ProfilePicUrl { get; set; }
        public string ProfileName { get; set; }
        public int TotalFollowers { get; set; }
        public int FollowerIncreased { get; set; }
        public int FollowersGained { get; set; }
        public int MediaSent { get; set; }
        public int CommentSent { get; set; }
        public int TotalEngagement { get; set; }
        public int EngagementByMedia { get; set; }
        public int EngagementByFollower { get; set; }
    }

}
