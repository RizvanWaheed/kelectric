using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialAPIsWrapper.Models
{
    public class Meta
    {
        public int code { get; set; }
    }

    public class Users
    {
        public string username { get; set; }
        public string profile_picture { get; set; }
        public string id { get; set; }
        public string full_name { get; set; }
    }

    public class CommentResponseDatum
    {
        public string created_time { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public Users from { get; set; }
    }

    public class CommentsRootObject
    {
        public Meta meta { get; set; }
        public CommentResponseDatum data { get; set; }
    }
}
