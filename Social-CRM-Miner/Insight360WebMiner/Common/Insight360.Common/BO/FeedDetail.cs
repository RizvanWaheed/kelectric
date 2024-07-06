using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.Common.BO
{
    public class FeedDetail
    {
        public string Name { get; set; }
        public string StatusId { get; set; }
        public string ProfileUrl { get; set; }
        public string FeedUrl { get; set; }
        public string Text { get; set; }    
        public string AuthorProfileImage { get; set; }

        public string AuthorUserId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string AuthorFollowers { get; set; }
        public string AuthorFollowing { get; set; }
        public string PostImageURL { get; set; }       
    }
}
