using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.BO
{
    public class DawnNews
    {
        public int NewsId { get; set; }
        public string NewsTitle { get; set; }
        public string NewsDescription { get; set; }
        public string NewsUrl { get; set; }
        public string NewsAuthor { get; set; }
        public DateTime NewsCreatedAt { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public int DocumentSourceId { get; set; }
        public int SearchTermId { get; set; }
        public string LanguageCode { get; set; }
        public int? Sentiment { get; set; }
        public string[] NewsDetails { get; set; }

    }
}
