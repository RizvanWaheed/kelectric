using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Insight360.BO
{
    public class DocumentSourceInfo
    {
        public DocumentSourceInfo()
        {
            SearchTerms = new List<SearchTermInfo>();
        }

        //public string LastFetchId { get; set; }
        //public DateTime LastFetchTime { get; set; }
        public int DocumentSourceId { get; set; }
        public string Url { get; set; }
        public int DocumentSourceTypeId { get; set; }

        public List<SearchTermInfo> SearchTerms { get; private set; }
        public int Frequency { get; set; }
        public int IsEnabled { get; set; } //Dec 03, 2015 Ahsen Mehmood: Is Network enabled or not
        public string DisplayName { get; set; } //Dec 28, 2015 Ahsen Mehmood: To know the source types of Facebook on runtime for mining

        public ProfileTypes ProfileType { get; set; }
    }

    public enum ProfileTypes
    {
        FacebookPage = 1,
        FacebookUser = 2,
        FacebookEvent = 3,
        FacebookGroup = 4,
        FacebookPlace = 5,
        FacebookChannel = 6,
        TwitterHandle = 99,
        YoutubeChannel = 43,
        YoutubeGlobal = 42

    }
}
