using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.APIS.Instagram.Models
{
    public class UserProfileResponse
    {
        public class Meta
        {
            public int code { get; set; }
        }

        public class Counts
        {
            public int media { get; set; }
            public int followed_by { get; set; }
            public int follows { get; set; }
        }

        public class Data
        {
            public string username { get; set; }
            public string bio { get; set; }
            public string website { get; set; }
            public string profile_picture { get; set; }
            public string full_name { get; set; }
            public Counts counts { get; set; }
            public string id { get; set; }
            public bool? user_has_liked { get; set; }
        }

        public class RootObject
        {
            public Meta meta { get; set; }
            public Data data { get; set; }
        }
    }
}
