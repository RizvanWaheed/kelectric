using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Admin
{
    public class PackageModel
    {
        public int SubscriberId { get; set; }
        public List<MembershipPermissionModel> Permissions { get; set; }
        public List<MembershipAccessLevelModel> AccessLevels { get; set; }
        public List<MembershipRestrictionModel> Restrictions { get; set; }
    }

    public class MembershipPermissionModel
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool Enabled { get; set; }
    }

    public class MembershipAccessLevelModel
    {
        public int AccessLevelId { get; set; }
        public string AccessLevelName { get; set; }
        public bool Enabled { get; set; }
    }

    public class MembershipRestrictionModel
    {
        public int RestrictionTypeId { get; set; }
        public string RestrictionTypeName { get; set; }
        public int Count { get; set; }
    }
}