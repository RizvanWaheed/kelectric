using SocialCRM.Web.Models.Publisher;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Publisher")]
    public class PublisherController : ApiController
    {
        IPublisherRepository repository;
        public PublisherController(IPublisherRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        [Route("ScheduleMessage")]
        public async Task<IHttpActionResult> ScheduleMessage(ScheduledMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.CreatorUserId = User.Identity.GetUserId<int>();
            var status = await repository.AddMessage(model);
            return Ok(status);
        }

        [HttpPost]
        [Route("UpdateScheduledMessage")]
        public async Task<IHttpActionResult> UpdateScheduledMessage(ScheduledMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.CreatorUserId = User.Identity.GetUserId<int>();
            var status = await repository.UpdateScheduledMessage(model);
            return Ok(status);
        }

        [HttpPost]
        [Route("ScheduleMessageDateRange")]
        public async Task<IHttpActionResult> ScheduleMessageDateRange(ScheduledMessageModel model)
        {
            bool errorOccured = false;
            model.CreatorUserId = User.Identity.GetUserId<int>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //model.CreatorUserId = User.Identity.GetUserId<int>();
            //For each posting time place another message.

            try
            {
                List<string> receivedPostTimes = new List<string>();
                foreach (string postTimeObj in model.PostTimes)
                {
                    receivedPostTimes.Add(postTimeObj);
                }
                
                foreach (string postTime in receivedPostTimes)
                {
                    model.PostTimes.Clear();
                    model.PostTimes.Add(postTime);
                    var status = await repository.AddMessage(model);
                }
            }
            catch (Exception ex)
            {
                errorOccured = true;
                throw ex;
            }

            return Ok(!errorOccured);
        }

        [HttpGet]
        public async Task<IHttpActionResult> FilterPublishingPost(ScheduledMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var status = await repository.DeleteGroup(groupId);
            return Ok();
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetProfilesQueue(ScheduledMessageModel searchFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var status = await repository.DeleteGroup(groupId);
            return Ok();
        }
        [HttpGet]
        [Route("GetProfilesQueue/{groupId}")]
        public async Task<IHttpActionResult> GetProfilesQueue(int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultSet = await repository.GetProfilesQueue(groupId);
            return Ok(resultSet);
        }
        [HttpGet]
        [Route("GetScheduledMessageLog/{profileCompanyGroupId}")]
        public async Task<IHttpActionResult> GetScheduledMessageLog(int profileCompanyGroupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultSet = await repository.GetScheduledMessageLog(profileCompanyGroupId);
            return Ok(resultSet);
        }

        [HttpPost]
        [Route("GetAllScheduledMessages")]
        public async Task<IHttpActionResult> GetAllScheduledMessages(ScheduledMessageModel model)
        {           
            model.CreatorUserId = User.Identity.GetUserId<int>();
            var status = await repository.GetAllScheduledMessages(model);            
            return Ok(status);
        }

        [HttpPost]
        [Route("AddTimeToQueue")]
        public async Task<IHttpActionResult> AddTimeToQueue(ProfileQueueModel model)
        {
            try
            {
                List<ProfileQueueViewModel> lstProfileQueue = JsonConvert.DeserializeObject<List<ProfileQueueViewModel>>(model.PostData);

                int Creator = User.Identity.GetUserId<int>();
                foreach (ProfileQueueViewModel obj in lstProfileQueue)
                {
                    obj.CreatorUserId = Creator;
                }

                var status = await repository.AddTimeToQueue(lstProfileQueue);
                return Ok(true);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("DeleteAllSlotsOfQueue")]
        public async Task<IHttpActionResult> DeleteAllSlotsOfQueue(ProfileQueueModel model)
        {
            try
            {
                ProfileQueueViewModel ProfileQueue = JsonConvert.DeserializeObject<ProfileQueueViewModel>(model.PostData);
                
                int Creator = User.Identity.GetUserId<int>();

                ProfileQueue.CreatorUserId = Creator;

                var status = await repository.DeleteAllSlotsOfQueue(ProfileQueue);
                return Ok(status);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("GetAllQueues")]
        public async Task<IHttpActionResult> GetAllQueues(ProfileQueueViewModel model)
        {
            model.CreatorUserId = User.Identity.GetUserId<int>();
            var status = repository.GetAllQueues(model);
            return Ok(status);
        }

        [HttpPost]
        [Route("ScheduleQueueMessage")]
        public async Task<IHttpActionResult> ScheduleQueueMessage(QueuedMessage model)
        {
            model.CreatorUserId = User.Identity.GetUserId<int>();
            var status = repository.ScheduleQueueMessage(model);            
            return Ok(status);
        }

        [HttpPost]
        [Route("GetAllQueuedMessages")]
        public async Task<IHttpActionResult> GetAllQueuedMessages(QueuedMessage model)
        {
            //model.CreatorUserId = User.Identity.GetUserId<int>();
            var status = await repository.GetAllQueuedMessages(model);
            return Ok(status);
        }

        [HttpPost]
        [Route("LoadPreviouslySavedOrScheduledMessage")]
        public async Task<IHttpActionResult> LoadPreviouslySavedOrScheduledMessage(ProfileQueueModel model)
        {
            int ScheduledMessageId = JsonConvert.DeserializeObject<int>(model.PostData);
            var status = await repository.LoadPreviouslySavedOrScheduledMessage(ScheduledMessageId);
            return Ok(status);
        }

    }
}
