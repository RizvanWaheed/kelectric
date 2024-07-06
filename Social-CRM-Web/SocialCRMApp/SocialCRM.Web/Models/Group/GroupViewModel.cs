using CRM.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Group
{
    public class GroupViewModel
    {
        public string GroupName { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public int GroupId { get; set; }
        public int TotalProfiles { get; set; }
        public int TotalMembers { get; set; }
        public string CompanyName { get; set; }
    }
    public class GroupModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsConnected { get; set; }
    }
    public class RegisterGroup
    {
        public string GroupName { get; set; }
        public int CompanyId { get; set; }
        public List<ProfileGroup> Profiles { get; set; }
        public class ProfileGroup
        {
            public ProfilesType ProfileType { get; set; }
            public int ProfileId { get; set; }
        }
    }
}