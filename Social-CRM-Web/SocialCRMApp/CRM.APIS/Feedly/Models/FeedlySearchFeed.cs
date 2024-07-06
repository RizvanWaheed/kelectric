using Newtonsoft.Json;
using System;

namespace CRM.APIS.Feedly.Models
{
  public class FeedlySearchFeed : FeedlyFeed
  {
    [JsonProperty("feedId")]
    public new string Id { get; set; }
  }
}
