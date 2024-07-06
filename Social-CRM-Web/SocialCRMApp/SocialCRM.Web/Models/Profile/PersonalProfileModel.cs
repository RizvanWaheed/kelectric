using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Common.Enum;
namespace SocialCRM.Web.Models.Profiles
{
    public class PersonalProfileModel
    {
        public List<UserRightModel> AssignedRights { get; set; }
        public List<UserReportModel> AssignedReports { get; set; }
        public List<PersonalProfile> ConnectedProfiles { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileUrl { get; set; }
    }
    public class PersonalProfile
    {
        public string ProfileName { get; set; }
        public string ProfileUrl { get; set; }
        public int ProfileId { get; set; }
        public ProfilesType ProfileType { get; set; }
        public bool IsEmailEnabled { get; set; }
        public bool IsConnected { get; set; }
    }
}