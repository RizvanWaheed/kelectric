using CRM.Entities;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Models.UnifiedContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IProfileRepository
    {
        Task<bool> AssignProfileToGroup(ProfileComapnyGroup profile, int userId);
        Task<string> RemoveProfileFromGroup(ProfileCompanyGroup profile, int userId);
        Task<bool> DeleteProfile(int profileId, int userId);
        Task<List<ProfileModel>> GetProfilesByGroupId(int userId, int groupId);
        Task<List<ContactFollowStatusModel>> GetSocialProfilesByGroupId(int userId, int groupId, int companyId, int profileTypeId);
        Task<List<ContactFollowStatusModel>> GetSocialProfilesPersonalMode(int userId, int profileTypeId);
        Task<List<ProfileModel>> GetProfilesByGroupIdForUnifiedInbox(int userId, int groupId, int companyId);
        Task<List<OutlookProfileModel>> GetOutlookProfilesByGroupId(int userId, int groupId, int companyId, bool isPersonal);
        Task<List<ProfileModel>> GetProfilesForUnifiedInboxPersonalMode(int userId);
        Task<List<ProfileModel>> GetProfilesByGroupId(int groupId);
        Task<List<ProfileModel>> GetProfilesByCompanyId(int companyId);
        Task<List<ProfileModel>> GetAllProfilesWithoutGroupByCompanyId(int companyId);
        Task<List<ProfileModel>> GetProfilesByCompanyId(int userId, int companyId);
        Task<List<ProfileModel>> GetProfilesByUserId(int userId);
        Task<List<ProfileCompanyGroups>> GetAllProfilesByCompanyId(int companyId);
        Task<bool> UpdateUserGroupProfiles(ProfileAssignementModel model);
        Task<List<ProfileCompanyGroupsDetail>> GetAllProfilesByCompanyAndUserId(int companyId, int userId);
        Task<List<ProfileModel>> GetProfilesByCompanyAndGroupId(int companyId, int groupId);
        Task<List<ProfileModel>> GetProfilesNotInGroup(int companyId, int groupId);
        Task<bool> AssignGroup(List<ProfileCompanyGroup> model);
        Task<List<ProfileCompanyGroupsDetail>> GetAllProfilesByMemberId(int companyId, int teamMemberId);
        Task<bool> UpdatePerssonalProfile(int profileId, int userId);
        Task<List<SearchWordPattern>> GetPattern();
        Task<bool> UpdateConsumerNumPattern(int id, string regex, string pattern);
        Task<bool> UpdateActivePattern(int id);
        Task<bool> DisablePattern();
    }
}
