using Newtonsoft.Json;
using System;

namespace Insight360.Feedly.Models
{
  public class FeedlySearchFeed : FeedlyFeed
  {
    [JsonProperty("feedId")]
    public new string Id { get; set; }
  }
}
