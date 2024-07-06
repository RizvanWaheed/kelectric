using CRM.Entities;
using SocialCRM.Web.Models.Publisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IPublisherRepository
    {
        Task<bool> AddMessage(ScheduledMessageModel message);
        Task<bool> UpdateScheduledMessage(ScheduledMessageModel model);
        Task<ProfileQueueViewModel> GetProfilesQueue(int groupId);
        Task<ScheduledMessageLogViewModel> GetScheduledMessageLog(int profileCompanyGroupId);
        Task<List<ScheduledMessages>> GetAllScheduledMessages(ScheduledMessageModel model);
        Task<bool> AddTimeToQueue(List<ProfileQueueViewModel> model);
        Task<bool> DeleteAllSlotsOfQueue(ProfileQueueViewModel model);        
        List<ProfileQueueViewModel> GetAllQueues(ProfileQueueViewModel model);
        Task<bool> ScheduleQueueMessage(QueuedMessage model);
        Task<List<ScheduledMessages>> GetAllQueuedMessages(QueuedMessage model);
        Task<ScheduledMessageModel> LoadPreviouslySavedOrScheduledMessage(int ScheduledMessageId);
    }
}
