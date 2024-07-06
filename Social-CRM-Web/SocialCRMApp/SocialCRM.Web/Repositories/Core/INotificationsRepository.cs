using System.Collections.Generic;
using SocialCRM.Web.Models.Notifications;
using System.Threading.Tasks;
namespace SocialCRM.Web.Repositories.Core
{
    public interface INotificationsRepository
    {
        Task<IEnumerable<Notification>> GetAllMessages(int RecipientUserId);
        Task<Notification> GetNotificationById(int companyId, int id,int userId);
        Task<List<Notification>> GetUserNotifications(int companyId, int pageSize, ref string nextCall, int userId);
        Task<List<Notification>> GetReadUnReadUserNotifications(bool IsRead, int pageSize, ref string nextCall, int RecipientUserId);
        Task<bool> MarkReadNotification(int companyId,int userId);
        Task<bool> MarkReadNotificationById(int id);
        void SendNotificationEmailUpdates();
    }
}
