using CRM.Entities;
using SocialCRM.Web.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.GeneratSettings
{
    public class InfluencersModel
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int AccountTypeId { get; set; }
        public string CustomerName { get; set; }
        public string AccountType { get; set; }
        public string ProfileName { get; set; }
        public int PlatformId { get; set; }
        public string profilePicURL { get; set; }
    }

    public class InfluencerListModel
    {
        public int TotalRecords { get; set; }
        public List<SelectItemList> AccountTypeList { get; set; }
        public List<SelectItemList> ProfilesList { get; set; }
        public List<InfluencersModel> UserList { get; set; }
    }

    public class SaveInfluencerModel
    {
        public int Id { get; set; }
        public string CustomerProfileHandle { get; set; }
        public int ProfileId { get; set; }        
        public int AccountTypeId { get; set; }
        public int PlatformId { get; set; }
        public string ExternalId { get; set; }
    }
}