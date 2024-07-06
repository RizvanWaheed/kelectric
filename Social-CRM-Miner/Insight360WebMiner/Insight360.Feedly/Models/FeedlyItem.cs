using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.Feedly.Models
{
    public class FeedlyItem
    {

        public FeedlyItem()
        {
            Feed = new FeedlyFeed();
            Links = new List<FeedlyItemLink>();
        }

        public long Staging_Id { get; set; }
        public string Staging_OrgName { get; set; }
        public int Staging_DocumentSource_Id { get; set; }
        public int Staging_SearchTerm_Id { get; set; }
        public DateTime Staging_InsertTime { get; set; }
        public DateTime Staging_UpdateTime { get; set; }
        public int Staging_ProcessingState { get; set; }
        public bool Staging_Refresh { get; set; }

        public string Id { get; set; }
        public string BaseUri { get; set; }
        public string FingerPrint { get; set; }
        public string OriginId { get; set; }
        public string Title { get; set; }
        public string Summary_Text { get; set; }
        public string Content_Text { get; set; }
        public string Keywords { get; set; }
        public string Author { get; set; }
        public bool? IsUnread { get; set; }

        public DateTime? UpdateDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? CrawledDate { get; set; }

        public int? Sentiment { get; set; }
        public string LanguageCode { get; set; }

        public FeedlyFeed Feed { get; set; }
        public List<FeedlyItemLink> Links { get; set; }
    }
    public class FeedlyFeed
    {
        public virtual string Id { get; set; }

        [JsonProperty("title")]
        public string Name { get; set; }

        [JsonProperty("visualUrl")]
        public Uri Image { get; set; }

        public string CoverColor { get; set; }

        [JsonProperty("coverUrl")]
        public Uri CoverImage { get; set; }

        [JsonProperty("iconUrl")]
        public Uri IconImage { get; set; }

        [JsonProperty("website")]
        public Uri Uri { get; set; }

        public string[] Topics { get; set; }

        public string Language { get; set; }

        public string Description { get; set; }

        public int? Subscribers { get; set; }

        public double? Velocity { get; set; }

        [JsonProperty("facebookUsername")]
        public string FacebookName { get; set; }

        public int? FacebookLikes { get; set; }

        [JsonProperty("twitterScreenName")]
        public string TwitterName { get; set; }

        public int? TwitterFollowers { get; set; }
    }
    public class FeedlyItemLink
    {
        public long Staging_Id { get; set; }
        public long Staging_Feedly_Item_Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public string MediaType { get; set; }
        public string RelationshipType { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
