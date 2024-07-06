using CRM.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace SocialCRM.Web.Models.Team
{
    public class TeamViewModel
    {
        public int TotalMember { get; set; }
        public int TotalGroup { get; set; }
        public int TotalProfile { get; set; }
        public string CompanyName { get; set; }
        public List<TeamMember> Team { get; set; }
    }
    public class TeamMemberViewModel
    {
        public TeamMember Profile { get; set; }
        public List<Group.GroupModel> Groups { get; set; }
        public List<UserReportModel> VisibleReports { get; set; }
        public List<UserRightModel> Rights { get; set; }
        public List<Profiles.PersonalProfile> ConnectedProfiles { get; set; }
    }
    public class TeamMember
    {
        public int TeamMemberId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string ProfileUrl { get; set; }
        public string Email { get; set; }
        public int AssignedProfile { get; set; }
        public bool IsAddedInGroup { get; set; }
        public bool IsActive { get; set; }
        public bool IsOwner { get; set; }
        public bool IsAdmin { get; set; }
    }
    public class AssignGroupModel
    {
        public int TeammemberId { get; set; }
        public int GroupId { get; set; }
    }
    public class MemberViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public int IsAdmin { get; set; }
        public List<int> AssignedGroups { get; set; }
        public List<int> AssignedRights { get; set; }
        public List<int> AssignedReports { get; set; }
        public List<Profiles> AssignedProfiles { get; set; }
        public class Profiles
        {
            public ProfilesType ProfileType { get; set; }
            public int ProfileId { get; set; }
            public int GroupId { get; set; }
        }
    }

    public class MemberUpdateModel
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}