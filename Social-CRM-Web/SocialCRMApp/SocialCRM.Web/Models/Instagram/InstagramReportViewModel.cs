using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Instagram
{
    public class FollowersInDates
    {
        public int NumOfFollowers { get; set; }
        public string Date { get; set; }
    }

    public class HashTags
    {
        public string Tag { get; set; }
        public int Frequency { get; set; }
    }
    public class InstagramProfiles
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfilePic { get; set; }
        public int TotalFollowers { get; set; }
        public string FollowersIncreased { get; set; }
        public int FollowersGained { get; set; }
        public int MediaSent { get; set; }
        public int CommentsSent { get; set; }
        public double TotalEngagements { get; set; }
        public double EngagementsPerMeida { get; set; }
        public double EngagementsPerFollower { get; set; }
    }
    public class InstagramReportViewModel
    {
        public List<TopInstagramPost> TopPostGraph { get; set; }
        public List<PublisingBehaviour> PublisingBehaviourGraph { get; set; }
        public List<AudienceEngagement> AudienceEngagementGraph { get; set; }
        public OverAllStats GeneralStatics { get; set; }
        public List<InfluencersEngaged> TopFollower { get; set; }
        public List<FollowersInDates> AudienceGrowthData { get; set; }
        public List<HashTags> MostUsedHashTags { get; set; }
        public List<HashTags> MostEngagedHashTags { get; set; }
        public List<InstagramProfiles> InstagramProfilesStats { get; set; }
    }
    public class OverAllStats
    {
        public int TotalFollowers { get; set; }
        public int TotalFollows { get; set; }
        public int FollowersGained { get; set; }
        public int TotalPosts { get; set; }
        public int TotalPhotos { get; set; }
        public int TotalVideos { get; set; }
        public int TotalLikesReceived { get; set; }
        public int TotalCommentReceived { get; set; }
        public string AccountName { get; set; }
        public string AccountPicURL { get; set; }
        public string AccessToken { get; set; }
        public string FollowersIncreased { get; set; }
        public string MediaIncreased { get; set; }
        public string EngagementIncreased { get; set; }
        public double EngagementsPerFollower { get; set; }
        public double EngagementsPerMedia { get; set; }
        public string EngagementPerMediaIncreased { get; set; }

    }
    public class TopInstagramPost
    {
        public int Engagements { get; set; }
        public string AccountName { get; set; }
        public string LowResolutionImageUrl { get; set; }
        public string StandardResolutionImageUrl { get; set; }
        public string PostType { get; set; }
    }
    public class PublisingBehaviour
    {
        public string PublishedDate { get; set; }
        public int MediaSent { get; set; }
        public int TotalMedia { get; set; }
    }
    public class AudienceEngagement
    {
        public string PublishedDate { get; set; }
        public int LikesReceived { get; set; }
        public int CommentReceived { get; set; }
        public int MediaSent { get; set; }
    }
    public class InfluencersEngaged
    {
        public int TotalFollower { get; set; }
        public string AccountName { get; set; }
        public int AccountUrl { get; set; }
    }
}