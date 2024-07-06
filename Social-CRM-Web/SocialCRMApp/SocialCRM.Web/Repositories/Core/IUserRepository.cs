using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IUserRepository
    {
         Task<bool> RegisterUser(RegisterViewModel user);
        Task<List<UserLogsModel>> GetUserLogs();
        Task<RegisterViewModel> GetUserByConfirmationCodeAsync(string code);
         Task<RegisterViewModel> GetUserByEmailId(string emailId, int companyId);
        Task<List<NotificationViewModel>> GetBusinessProfile(int userId);
         Task<bool> UpdateNotificationSettings(NotificationViewModel model,int userId);
         Task<bool> UpdateBusinessProfileName(NotificationViewModel model);
         Task<PersonalProfileModel> GetPersonalProfile(int userId, int companyId);
         Task<bool> UpdatePersonalProfile(PersonalProfileModel model);
        Task<bool> IsTrialExpired(int userId);
        Task<bool> UpdateLog(int UserId, int LogCode);
    }
}
