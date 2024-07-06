using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedDeck
{
    public class ConsumerProfileModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int NumOfFollowers { get; set; }
        public int NumOfFollowing { get; set; }
        public int TotalTweets { get; set; }
        public string MobileNum { get; set; }
        public string AccountNum { get; set; }
        public string IBC { get; set; }
        public string Address { get; set; }
        public string UserStatus { get; set; }
        public int WarnBanId { get; set; }
        public string PostUrl { get; set; }
        public string PostText { get; set; }
        public string Feeder { get; set; }
        public string PMT { get; set; }
    }

    public class FeedDetail
    {
        public string Name { get; set; }
        public string ProfileUrl { get; set; }
        public string FeedUrl { get; set; }
        public string Text { get; set; }
    }
}