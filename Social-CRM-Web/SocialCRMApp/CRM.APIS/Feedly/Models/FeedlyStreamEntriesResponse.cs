using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.APIS.Feedly.Models
{
  public class FeedlyStreamEntriesResponse
  {
    public string Continuation { get; set; }

    public List<FeedlyEntry> Items { get; set; }
  }
}
