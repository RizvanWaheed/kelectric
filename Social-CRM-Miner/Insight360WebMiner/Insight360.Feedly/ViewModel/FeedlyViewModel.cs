using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.Feedly.Models
{
    public class FeedlyViewModel
    {
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
    }
    public class FeedlySearchModel
    {
        public int GroupId { get; set; }
        public bool IsSearchTerm { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<int> SearchTerm { get; set; }
    }
}
