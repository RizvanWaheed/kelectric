using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Notifications
{
    public class Notification
    {
        public int MessageID { get; set; }

        public string Message { get; set; }

        public string EmptyMessage { get; set; }

        public DateTime MessageDate { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationPaginationModel
    {
        public bool IsRead { get; set; }
        public int CompanyId { get; set; }

        public int PageSize { get; set; }

        public string NextCall { get; set; }
    }
}