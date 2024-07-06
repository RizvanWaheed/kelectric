using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Admin
{
    public class GroupModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }

    public class TeamMemberModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int GroupId { get; set; }
    }

    public class SocialProfileModel
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileUserName { get; set; }
        public string ProfilePic { get; set; }
        public string ProfileType { get; set; }
        public int GroupId { get; set; }
    }

    public class CompanyDetailsModel
    {
        public decimal CompanyId { get; set; }
        public string CompannyName { get; set; }
        public string CreatedDate { get; set; }
        public int GroupCount { get; set; }
        public int TeamMemberCount { get; set; }
        public int MembershipDaysRemaining { get; set; }
        public string MembershipPlan { get; set; }
        public string CompanyStatus { get; set; }
        public string MembershipStatus { get; set; }
        public List<GroupModel> GroupList { get; set; }
        public List<TeamMemberModel> TeamList { get; set; }
        public List<SocialProfileModel> ProfileList { get; set; }
    }
}