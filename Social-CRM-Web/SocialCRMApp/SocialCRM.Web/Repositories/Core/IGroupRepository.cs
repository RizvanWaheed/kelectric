using CRM.Entities;
using SocialCRM.Web.Models.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IGroupRepository
    {
        Task<int> AddGroup(Group groupInfo,int currentUserId);
        Task<bool> UpdateGroup(GroupModel groupInfo);
        Task<string> DeleteGroup(int groupId);
        Task<GroupViewModel> GetGroupById(int groupId);
        Task<List<GroupViewModel>> GetUserGroups(int userId);
        Task<List<GroupViewModel>> GetCompanyGroups(int companyId);
        Task<bool> RemoveMember(int teamMemberId, int groupId);
        Task<List<GroupViewModel>> GetUserCompanyGroups(int userId);
    }
}
