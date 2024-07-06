using System.Collections.Generic;

namespace CRM.APIS.Feedly.Models
{
  public class FeedlyStreamEntryIdsResponse
  {
    public string Continuation { get; set; }

    public List<string> Ids { get; set; }
  }
}
