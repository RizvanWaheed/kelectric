using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Common.Enum;

namespace SocialCRM.Web.Models.TaskManager
{
    public class TaskSearchModel
    {
        public List<int> SelectedClassifications { get; set; }
        public List<int> SelectedPriorities { get; set; }
        public List<int> SelectedTeamAssigneeMemberId { get; set; }
       // public List<SearchedProfile> SelectedSearchedProfile { get; set; }
        public List<int> ProfileId { get; set; }
        public List<int> SelectedStatus { get; set; }
        public int GroupId { get; set; }
    }
    public class SearchedProfile
    {
        public int ProfileId { get; set; }
        public bool IsPage { get; set; }
    }
}