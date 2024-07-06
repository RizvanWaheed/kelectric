using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common.Enum
{
    public enum Classificaiton
    {
        SalesLead=1,
        AfterSales=2,
        Compliment=3,
        Complaint=4,
        GeneralInquiry=5,
        Referral=6,
        Review=7
    }
    public enum Priority
    {
        High=1,
        Medium=2,
        Low=3
    }
    public enum TaskStatus 
    {
        InProgress=1,
        Close=2,
        Draft=3
    }
    public enum LogType
    {
        Assigned=1,
        ReAssigned=2,
        Comment=3,
        CommentDeleted=4
    }
}
