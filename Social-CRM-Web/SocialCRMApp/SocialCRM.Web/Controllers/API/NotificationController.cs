using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.Notifications;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Notification")]
    public class NotificationController : ApiController
    {
        INotificationsRepository repository;
        public NotificationController(INotificationsRepository repo)
        {
            repository = repo;
        }
        [Route("GetNotificationById/{companyId}/{id}")]
        public async Task<IHttpActionResult> GetNotificationById(int companyId,int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Parameters.");
            }
            var userId = User.Identity.GetUserId<int>();
            var notification = await repository.GetNotificationById(companyId, id, userId);
            return Ok(notification);
        }
        [Route("GetUserNotifications")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUserNotifications(NotificationPaginationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Parameters.");
            }
            var userId = User.Identity.GetUserId<int>();
            var nextCall = model.NextCall;
            var userNotifications = await repository.GetUserNotifications(model.CompanyId, model.PageSize, ref nextCall, userId);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Message", "Successfull");
            parameters.Add("UserNotifications", userNotifications);
            parameters.Add("NextCall", nextCall);
            return Json(parameters);
        }
        [Route("MarkReadNotification/{companyId}")]
        [HttpPost]
        public async Task<IHttpActionResult> MarkReadNotification(int companyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Parameters.");
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.MarkReadNotification(companyId, userId);
            return Ok(status);
        }
        [Route("GetReadUnReadUserNotifications")]
        [HttpPost]
        public async Task<IHttpActionResult> GetReadUnReadUserNotifications(NotificationPaginationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Parameters.");
            }
            var userId = User.Identity.GetUserId<int>();
            var nextCall = model.NextCall;
            var userNotifications = await repository.GetReadUnReadUserNotifications(model.IsRead,model.PageSize,ref nextCall, userId);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Message", "Successfull");
            parameters.Add("UserNotifications", userNotifications);
            parameters.Add("NextCall", nextCall);
            return Json(parameters);
        }
        [Route("MarkReadNotificationById/{notifId}")]
        [HttpPost]
        public async Task<IHttpActionResult> MarkReadNotificationById(int notifId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Parameters.");
            }
            var status = await repository.MarkReadNotificationById(notifId);
            return Ok(status);
        }
        [Route("LoadNotifcaiton/{companyId}/{nextCall}")]
        public async Task<IHttpActionResult> LoadNotifcaiton(int companyId, string nextCall)
        {
            return Ok();
        }

        [Route("GetAllMessages/{RecipientUserId}")]
        public async Task<IHttpActionResult> GetAllMessages(int RecipientUserId)
        {
            var userId = User.Identity.GetUserId<int>();
            var notification = await repository.GetAllMessages(userId);
            return Ok(notification);
        }

        [Route("SendNotificationEmailUpdates")]
        [HttpGet]
        public async Task<IHttpActionResult> SendNotificationEmailUpdates()
        {
            await Task.Run(() =>
            {
                repository.SendNotificationEmailUpdates();
            });
            return Ok();
        }
    }
}
