using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.User
{
    public class NotificationViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<Notification> Notificaitons { get; set; }
    }
    public class Notification
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsEmailEnabled { get; set; }
        public bool IsPushNotificationEnabled { get; set; }
    }
}