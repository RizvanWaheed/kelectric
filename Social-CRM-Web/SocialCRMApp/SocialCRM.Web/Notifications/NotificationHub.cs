using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SocialCRM.Web.Models.Notifications;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SocialCRM.Web
{
    public class NotificationHub : Hub
    {
        private static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            context.Clients.All.updateMessages();
        }

        [HubMethodName("emailUpdates")]
        public static void EmailUpdates()
        {
            context.Clients.All.emailUpdates();
        }

        [HubMethodName("updateInboxItems")]
        public static void UpdateInboxItems()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.updateInbox();
        }

        [HubMethodName("updateTaskCompletedItems")]
        public static void updateTaskCompletedItems(int FeedId, string FeedType)
        {
            context.Clients.All.updateTaskCompletedItems(FeedId, FeedType);
        }

        [HubMethodName("markAllUnrespondedItemsCompleted")]
        public static void markAllUnrespondedItemsCompleted(int GroupId)
        {
            context.Clients.All.markAllUnrespondedItemsCompleted(GroupId);
        }
    }
}