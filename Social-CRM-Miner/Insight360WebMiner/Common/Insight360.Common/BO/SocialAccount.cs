using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.Common.BO
{
    public class SocialAccount
    {
        public int Id { get; set; }
        public string TwitterHandle { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string ImageUri { get; set; }
        public string ProfileExternalId { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public int ProfileType { get; set; }
    }
}
