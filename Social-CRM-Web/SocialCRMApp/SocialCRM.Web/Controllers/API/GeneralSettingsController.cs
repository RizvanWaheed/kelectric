using CRM.Entities;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.GeneratSettings;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/GeneralSettings")]
    public class GeneralSettingsController: ApiController
    {
        IGeneralSettingsRepository repository;
        public GeneralSettingsController(IGeneralSettingsRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        [Route("UpdateSampleReply")]
        public async Task<IHttpActionResult> UpdateSampleReply(SampleReply model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdateSampleReply(model.Id, model.ReplyText);
            return Ok(status);
        }

        [HttpPost]
        [Route("DeleteSampleReply/{id}")]
        public async Task<IHttpActionResult> DeleteSampleReply(int id)
        {
            var status = await repository.DeleteSampleReply(id);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetSocialMediaUserTypes")]
        public async Task<IHttpActionResult> GetSocialMediaUserTypes()
        {
            var resultSet = repository.GetSocialMediaUserTypes();
            return Ok(resultSet);
        }

        [HttpPost]
        [Route("AddUpdateEmailSettings")]
        public async Task<IHttpActionResult> AddUpdateEmailSettings(SocialMediaUsersUpdateReceiver model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.AddUpdateEmailSettings(model);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetSocialMediaUsersUpdateReceiversList")]
        public async Task<IHttpActionResult> GetSocialMediaUsersUpdateReceiversList()
        {
            var resultSet = repository.GetSocialMediaUsersUpdateReceiversList();
            return Ok(resultSet);
        }

        [HttpPost]
        [Route("DeleteEmailSettings")]
        public async Task<IHttpActionResult> DeleteEmailSettings(SocialMediaUsersUpdateReceiver model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.DeleteEmailSettings(model.ID);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetInfluencersList/{groupId}/{pageNumber}/{getOptions}")]
        public async Task<IHttpActionResult> GetInfluencersList(int groupId, int pageNumber, bool getOptions)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultSet = await repository.GetInfluencersList(userId, groupId, pageNumber, getOptions);
            return Ok(resultSet);
        }

        [HttpPost]
        [Route("SaveInfluencer")]
        public async Task<IHttpActionResult> SaveInfluencer(SaveInfluencerModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.SaveInfluencer(model);
            return Ok(status);
        }

        [HttpGet]
        [Route("DeleteInfluencer/{Id}")]
        public async Task<IHttpActionResult> DeleteInfluencer(int Id)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultSet = await repository.DeleteInfluencer(Id);
            return Ok(resultSet);
        }

    }
}