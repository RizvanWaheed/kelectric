using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models
{
    public class UserReportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
    }
}