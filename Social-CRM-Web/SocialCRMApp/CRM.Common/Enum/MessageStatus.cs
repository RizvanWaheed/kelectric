using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common.Enum
{
    public enum MessageStatus
    {
        Draft=0,
        Scheduled=1,
        Approved=2,
        Reject=3,
        Sent=4      
    }
    public enum MessageType
    {
        Scheduled = 1,
        Queued = 2
    }
    public enum Days
    {
        MON=1,
        TUE=2,
        WED=3,
        THR=4,
        FRI=5,
        SAT=6,
        SUN=7
    }

    public enum QueueOptions
    {
        QueueNext=1,
        QueueLast=2
    }

    public enum FacbookReplyType
    {
        Post = 1,
        Comment = 2,
        NestedComment = 3
    }
}
