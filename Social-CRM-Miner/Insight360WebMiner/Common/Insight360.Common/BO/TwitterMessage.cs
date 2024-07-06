using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.BO
{
    public class TwitterMessage
    {
        public object MsgId { get; set; }
        public object RecipientId { get; set; }
        public object RecipientScreenName { get; set; }
        public object SenderId { get; set; }
        public object SenderScreenName { get; set; }
        public object MsgText { get; set; }
        public object CreatedDate { get; set; }
        public object SenderProfilePic { get; set; }
        public object IsSenderVerified { get; set; }
    }
}
