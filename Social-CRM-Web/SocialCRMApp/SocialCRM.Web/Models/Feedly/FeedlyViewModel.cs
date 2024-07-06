using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Feedly
{
    public class FeedlyViewModel
    {
        public int Id { get; set; }
        public string MessageHeading { get; set; }
        public string MessageBody { get; set; }
        public string MessageShortBody { get; set; }
        public string HeadingURL { get; set; }
        public string CrawledDate { get; set; }

        public string FacebookUserName { get; set; }
        public string FacebookLikes { get; set; }
        public string TwitterScreenName { get; set; }
        public string TwitterFollowers { get; set; }
        public string ImageCoverUrl { get; set; }
        public string CssClass { get; set; }
    }
    public class FeedlySearchModel
    {
        public int GroupId { get; set; }
        public bool IsSearchTerm { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int MaxFeedId { get; set; }
        public List<int> SearchTerm { get; set; }
    }
}