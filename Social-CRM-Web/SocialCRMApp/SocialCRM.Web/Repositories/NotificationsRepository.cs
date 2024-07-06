using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SocialCRM.Web.Models.Notifications;
using SocialCRM.Web.Repositories.Core;
using System.Threading.Tasks;
using CRM.Entities;
using CRM.Common;
using SocialCRM.Web.Models.Company;

namespace SocialCRM.Web.Repositories
{
    public class NotificationsRepository : INotificationsRepository
    {
        readonly string _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public void SendNotificationEmailUpdates()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                var queries = new[] { @"SELECT [ID], [NotificationText], [NotificationType], [EntryDate], [IsRead] FROM [dbo].[UserNotification] where [IsEmailSent] = 0" };

                using (var command = new SqlCommand(string.Join("; ", queries), connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);

                    dependency.OnChange += new OnChangeEventHandler(dependency_EmailNotification);
                    command.ExecuteReader();
                }

            }
        }
        public async Task<IEnumerable<Notification>> GetAllMessages(int RecipientUserId)
        {
            List<Notification> messages = new List<Notification>();
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                var queries = new[] { @"SELECT [ID], [NotificationText], [NotificationType], [EntryDate], [IsRead] FROM [dbo].[UserNotification] where [RecipientUserId] = " + RecipientUserId + " and [IsNotificationEnabled] = 1 Order By EntryDate desc" };
                       //@"SELECT [Id], [UserId], [MsgId], [MsgText], [SenderScreenName] FROM [dbo].[MessagesTwitter] Order By Id desc",
                       //@"SELECT [Id], [UserId], [StatusText], [AuthorName] FROM [dbo].[PostsTwitter] Order By Id desc",
                       //@"SELECT [Id], [UserId], [PostId], [Message] FROM [dbo].[PostsFB] Order By Id desc",
                       //@"SELECT [Id], [CommentId], [PostId], [Message] FROM [dbo].[CommentsFB] Order By Id desc",
                       //@"SELECT [Id], [PageId], [MsgId], [MsgText] FROM [dbo].[PageChatsFB] Order By Id desc",
                       //@"SELECT [Id], [PostId] FROM [dbo].[InstagramMinnedData] Order By Id desc"};


                using (var command = new SqlCommand(string.Join("; ", queries), connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        messages.Add(item: new Notification { MessageID = (int)reader["ID"], Message = (string)reader["NotificationText"], EmptyMessage =  reader["NotificationType"] != DBNull.Value ? (string) reader["NotificationType"] : "", MessageDate = Convert.ToDateTime(reader["EntryDate"]), IsRead = Convert.ToBoolean(reader["IsRead"]) });
                    }
                }
              
            }
            return messages;
           
            
        }
        public async Task<Notification> GetNotificationById(int companyId, int id, int userId)
        {
            Notification messages=null;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    messages = db.GetNotificationById(companyId, id, userId).Select(n=>new Notification
                    {
                        MessageDate=n.EntryDate,
                        Message=n.NotificationText,
                        EmptyMessage=n.NotificationType,
                        MessageID=n.Id,
                        IsRead=n.IsRead
                    }).FirstOrDefault();
                    db.Dispose();
                }
                catch (Exception exp)
                {
                    messages = null;
                }
            }
            return messages;
        }
        public  Task<List<Notification>> GetUserNotifications(int companyId, int pageSize, ref string nextCall, int userId)
        {
            List<Notification> messages = new List<Notification>();
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    DateTime notificationPatitionDate;
                    int pageIndex = 1;
                    if(!string.IsNullOrEmpty(nextCall))
                        nextCall = nextCall.Replace("\"", "");
                    if (string.IsNullOrEmpty(nextCall) || !nextCall.Contains(":") || !int.TryParse(nextCall.Split(':')[1], out pageIndex)
                        || !DateTime.TryParseExact(nextCall.Split(':')[0], "yyyyMMddHHmmssFFFF", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out notificationPatitionDate))
                    {
                        notificationPatitionDate = DateTime.Now;
                        pageIndex = 1;
                    }
                    PagingHelper.blockSize = pageSize;
                    var partionDate = notificationPatitionDate;
                    messages = db.GetUserNotifications(companyId,userId).Where(x => x.EntryDate.Ticks < notificationPatitionDate.Ticks)
                        .Skip(PagingHelper.StartIndex(pageIndex))
                        .Take(PagingHelper.blockSize)
                        .Select(n => new Notification
                        {
                            MessageDate = n.EntryDate,
                            Message = n.NotificationText,
                            EmptyMessage = n.NotificationType,
                            MessageID = n.Id,
                            IsRead = n.IsRead
                        }).ToList();
                    nextCall = messages.Count > 0 ? messages.Count == PagingHelper.blockSize ? string.Concat(messages.OrderBy(x => x.MessageDate).First().MessageDate.ToString("yyyyMMddHHmmssFFFF"),":",(pageIndex)) : "End" : "End";
                    db.Dispose();
                }
                catch (Exception exp)
                {
                    messages = null;
                }
            }
            return Task.FromResult<List<Notification>>(messages);
        }
        public Task<List<Notification>> GetReadUnReadUserNotifications(bool IsRead, int pageSize, ref string nextCall, int RecipientUserId)
        {
            List<Notification> messages = new List<Notification>();
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    DateTime notificationPatitionDate;
                    int pageIndex = 1;
                    if (!string.IsNullOrEmpty(nextCall))
                        nextCall = nextCall.Replace("\"", "");
                    if (string.IsNullOrEmpty(nextCall) || !nextCall.Contains(":") || !int.TryParse(nextCall.Split(':')[1], out pageIndex)
                        || !DateTime.TryParseExact(nextCall.Split(':')[0], "yyyyMMddHHmmssFFFF", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out notificationPatitionDate))
                    {
                        notificationPatitionDate = DateTime.Now;
                        pageIndex = 1;
                    }
                    PagingHelper.blockSize = pageSize;
                    var partionDate = notificationPatitionDate;
                    messages = db.UserNotifications.Where(x => x.IsNotificationEnabled == true && x.RecipientUserId == RecipientUserId && x.IsRead == IsRead && x.EntryDate < notificationPatitionDate)
                        .OrderByDescending(u => u.EntryDate)
                        .Skip(PagingHelper.StartIndex(pageIndex))
                        .Take(PagingHelper.blockSize)
                        .Select(n => new Notification
                        {
                            MessageDate = n.EntryDate,
                            Message = n.NotificationText,
                            EmptyMessage = n.NotificationType,
                            MessageID = n.Id,
                            IsRead = n.IsRead
                        }).ToList();
                    nextCall = messages.Count > 0 ? messages.Count == PagingHelper.blockSize ? string.Concat(messages.OrderBy(x => x.MessageDate).First().MessageDate.ToString("yyyyMMddHHmmssFFFF"), ":", (pageIndex)) : "End" : "End";
                    db.Dispose();
                }
                catch (Exception exp)
                {
                    messages = null;
                }
            }
            return Task.FromResult<List<Notification>>(messages);
        }
        public async Task<bool> MarkReadNotification(int companyId, int userId)
        {
            bool isRead=false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                   var rowEffect= db.MarkReadUserNotification(companyId, userId);
                    isRead = rowEffect > 0;
                    db.Dispose();
                }
                catch (Exception exp)
                {
                    isRead = false;
                }
            }
            return isRead;
        }
        public async Task<bool> MarkReadNotificationById(int id)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var notif = db.UserNotifications.Where(g => g.Id ==id).FirstOrDefault();
                if (notif != null)
                {
                    notif.IsRead = true;
                }
                result = await db.SaveChangesAsync() > 0;
                db.Dispose();
            }
            return result;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            
            if (e.Type == SqlNotificationType.Change)
            {
                NotificationHub.SendMessages();
            }
        }

        private void dependency_EmailNotification(object sender, SqlNotificationEventArgs e)
        {

            if (e.Type == SqlNotificationType.Change)
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        List<BulkEmailModel> notifications = db.UserNotifications.Where(x => x.IsEmailSent == false).Select(y => new BulkEmailModel
                        {
                            ToUserEmail = db.AspNetUsers.Where(m => m.Id == y.RecipientUserId).FirstOrDefault().Email,
                            EmailSubject = "360 Insight CRM- " + y.NotificationType,
                            EmailBody = y.NotificationText,
                            notif = y
                        }).ToList();
                        foreach(BulkEmailModel notif in notifications)
                        {
                            var userEmail = ConfigurationManager.AppSettings["EmailUserName"];
                            var status = new EmailHandler().SendEmail(userEmail, notif.ToUserEmail, notif.EmailSubject, notif.EmailBody);
                            if(status == true)
                            {
                                notif.notif.IsEmailSent = true;
                            }
                        }

                        db.SaveChanges();

                        NotificationHub.EmailUpdates();
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
        }
    }
}