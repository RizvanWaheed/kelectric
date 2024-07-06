using SocialCRM.Web.Models.TaskManager;
using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Dashboard
{
    public class DashboardModel
    {
        public int gksTwNumOfTweets { get; set; }
        public int gksTwNumOfFollowers { get; set; }
        public int gksTwNumOfLikes { get; set; }

        public int gksFbProfileNumOfPosts { get; set; }
        public int gksTFbProfileNumOfFollowers { get; set; }
        public int gksTFbProfileNumOfFriends { get; set; }
        public int gksFbProfileNumOfLikes { get; set; }

        public int gksFbPageNumOfPosts { get; set; }
        public int gksFbPageNumOfLikes { get; set; }

        public int gksInstaNumOfPosts { get; set; }
        public int gksInstaNumOfFollowers { get; set; }
        public int gksInstaNumOfLikes { get; set; }

        public List<TopInfluencerModel> TopInfluencers { get; set; }

        public int DemographicMaleCount { get; set; }
        public int DemographicFemaleCount { get; set; }
        public int DemographicUnspecifiedCount { get; set; }
        public List<WordCloudModel> WordCloudList { get; set; }
    }

    public class GroupPerformanceModel
    {
        public int StatusId { get; set; }
        public int StatusCount { get; set; }
    }

    public class WordCloudModel
    {
        public string Text { get; set; }
        public string TextWithFrequency { get; set; }
        public string Link { get; set; }
        public string ViewClass { get; set; }
        public int Frequency { get; set; }
    }
}