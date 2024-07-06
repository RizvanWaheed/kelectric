using CRM.Common.Enum;
using SocialCRM.Web.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SocialCRM.Web.Models.Profiles
{
    public class ProfileModel
    {
        public int Id { get; set; }
        public int ProfileCompnayGroupId { get; set; }
        public int ProfileId { get; set; }
        public int CompanyId { get; set; }
        public ProfilesType ProfileType { get; set; }
        public int ProfileTypeId { get; set; }
        public string ProfileName { get; set; }
        public string AccountName { get; set; }
        public bool IsConnected { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public string ProfilePicURL { get; set; }
        public bool IsPage { get; set; }
        public string UserId { get; set; }
        public string PageId { get; set; }
    }
    public class ProfileCompanyGroup
    {
        public int ProfileId { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public ProfilesType ProfileType { get; set; }
    }
    public class ProfileCompanyGroups
    {
        public List<ProfileModel> ProfileId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
    public class ProfileCompanyGroupsDetail
    {
        public List<ProfileCompanyGroups> ProfileCompanyGroup { get; set; }
        public TeamMemberViewModel Team { get; set; }
    }

    public class ProfileAssignementModel
    {
        public int TeamMemberId { get; set; }
        public int CompanyId { get; set; }
        public List<GroupdProfiles> Profiles { get; set; }
        public List<int> AssignedRights { get; set; }
        public List<int> AssignedReports { get; set; }
        public int IsAdmin { get; set; }
        public class GroupdProfiles
        {
            public int GroupId { get; set; }
            public int ProfileId { get; set; }
            public bool IsConnected { get; set; }
            public int ProfileCompanyGroupId { get; set; }
            public int ProfileTypeId { get; set; }
        }
    }

    public class ExtraSocialProfileInfo
    {
        public string ProfilePicURL { get; set; }
        public string ProfileURI { get; set; }
        public string UserFullName { get; set; }
    }

    public class OutlookProfileModel
    {
        public int Id { get; set; }
        public int ProfileCompnayGroupId { get; set; }
        public int ProfileId { get; set; }
        public int CompanyId { get; set; }
        public ProfilesType ProfileType { get; set; }
        public int ProfileTypeId { get; set; }
        public string ProfileName { get; set; }
        public string AccountName { get; set; }
        public bool IsConnected { get; set; }
        public string ProfilePicURL { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }
    }

    public partial class OutlookAccount
    {
        public string UserId { get; set; }
        public string Passwrd { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }
        public string Type { get; set; }
        public bool IsPersonal { get; set; }
        public int CompanyID { get; set; }
        public int GroupID { get; set; }
    }
}