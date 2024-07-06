using SocialCRM.Web.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IBannedProfilesRepository
    {
        Task<BannedProfilesListModel> GetWarnBanUsersList(int userId, string reasonIds, string platformId, string name, string userHandle, int groupId, bool getOptions, int pageNumber, int type);
        Task<string> SaveWarnBanUser(BannedProfilesSaveModel model, int userId);
        Task<bool> DeleteUser(int id);
        Task<string> BanUser(int id);
        Task<string> UnBanUser(int id);
        Task<BannedProfilesOptionsListModel> GetBannedProfilesOptions(int userId, int groupId);
        Task<string> GetAttachmentAsBase64String(int id);
        Task<List<WarnBanReasonReportModel>> GetWarnBanUserReport(int userId, int groupId, DateTime? fromDate, DateTime? toDate, TimeSpan? fromTime, TimeSpan? toTime);
        Task<string> WarnBanUser(ConsumerProfilesSaveModel model, int userId);
    }
}
