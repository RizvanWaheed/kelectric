using SocialCRM.Web.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface ITeamRepository
    {
        Task<TeamViewModel> GetByCompanyId(int companyId);
        Task<TeamViewModel> GetByGroupId(int groupId);
        Task<TeamViewModel> GetByGroupIdForTaskList(int groupId);
        Task<TeamMemberViewModel> GetByUserId(int userId, int companyId);
        Task<int> AddTeamMember(MemberViewModel member, int userid);
        Task<bool> LinkUserAccount(int teamMemberId,int userId);
        Task<TeamViewModel> GetTeamMembersNotInGroup(int companyId, int groupId);
        Task<bool> AssignGroup(List<AssignGroupModel> teamGroup);
        Task<TeamMemberViewModel> GetTeamMemberProfile(int teamMemberId);
        Task<TeamMemberViewModel> GetTeamMemberProfileByGroup(int teamMemberId, int groupId);
        Task<bool> RemoveMemberFromCompany(int teamMemberId, int companyId);
        Task<bool> AddMemberToCompany(int teamMemberId, int companyId);
        Task<bool> UpdateTeamMember(MemberUpdateModel groupInfo);
    }
}
