using CRM.Entities;
using SocialCRM.Web.Models.GeneratSettings;
using SocialCRM.Web.Models.Publisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IGeneralSettingsRepository
    {
        Task<bool> UpdateSampleReply(int id, string replyText);
        List<SocialMediaUserType> GetSocialMediaUserTypes();
        Task<bool> AddUpdateEmailSettings(SocialMediaUsersUpdateReceiver model);
        List<SocialMediaUsersUpdateReceiver> GetSocialMediaUsersUpdateReceiversList();
        Task<bool> DeleteEmailSettings(int Id);
        Task<InfluencerListModel> GetInfluencersList(int userId, int groupId, int pageNumber, bool getOptions);
        Task<string> SaveInfluencer(SaveInfluencerModel model);
        Task<string> DeleteInfluencer(int Id);
        Task<bool> DeleteSampleReply(int Id);
    }
}
