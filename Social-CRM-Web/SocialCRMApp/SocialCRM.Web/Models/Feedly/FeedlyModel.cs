using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Feedly
{
    public class FeedlyModel
    {
        public int Id { get; set; }
        public string SearchKeyword { get; set; }
        public int GroupId { get; set; }
       // public int CreatorMemberId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSearchTerm { get; set; }
    }
}