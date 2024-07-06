using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Company
{
    public class CompanyModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string UserEmail { get; set; }
        public List<Groups> CompanyGroups { get; set; }
    }
    public class CompanyInfo
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
    public class Groups
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
    public class CompanyViewModel
    {
        public int TotalMembers { get; set; }
        public int TotalGroups { get; set; }
        public int TotalProfiles { get; set; }
    }

    public class BulkEmailModel
    {
        public string FromUserEmail { get; set; }
        public string ToUserEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string CCUserEmail { get; set; }
        public UserNotification notif { get; set; }

        public BulkEmailModel()
        {
            CCUserEmail = "";
        }
    }
}