using CRM.Common;
using CRM.Entities;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Models.UnifiedContact;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.Common.Enum;
using SocialCRM.Web.Controllers;
using Microsoft.Owin.Security;
using SocialCRM.Web.GoogleAnalytics;
using SocialCRM.Web.Models;
using SocialCRM.Web.Repositories;
using Sparkle.LinkedInNET;
using Sparkle.LinkedInNET.Profiles;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Exchange.WebServices.Autodiscover;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Profile")]
    public class ProfileMangementController : ApiController
    {
        IProfileRepository repository;
        ExchangeService exchange;
        public ProfileMangementController(IProfileRepository repo)
        {
            repository = repo;
            exchange = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        [Route("AssignProfileToGroup")]
        public async Task<IHttpActionResult> AssignProfileToGroup(ProfileCompanyGroup model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.AssignProfileToGroup(new CRM.Entities.ProfileComapnyGroup { GroupId = model.GroupId, CompanyId = model.CompanyId, ProfileId = model.ProfileId }, userId);
            return Ok(status);
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        [Route("RemoveProfileToGroup")]
        public async Task<IHttpActionResult> RemoveProfileToGroup(ProfileCompanyGroup model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.RemoveProfileFromGroup(model, userId);
            if (status == "EBUN")
            {
                return BadRequest(status);
            }
            return Ok(status);
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        [Route("RemoveProfile/{id}")]
        public async Task<IHttpActionResult> RemoveProfile(int id)
        {
            var userId = User.Identity.GetUserId<int>();
            var profileId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.DeleteProfile(profileId, userId);
            return Ok(status);
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        [Route("UpdatePerssonalProfile/{id}")]
        public async Task<IHttpActionResult> UpdatePerssonalProfile(int id)
        {
            var userId = User.Identity.GetUserId<int>();
            var profileId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdatePerssonalProfile(profileId, userId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetByUserId/{id}")]
        public async Task<IHttpActionResult> GetByUserId(int id)
        {
            var userId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetProfilesByUserId(userId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetByCompanyId/{userId}/{id}")]
        public async Task<IHttpActionResult> GetByCompanyId(int userId, int id)
        {
            var companyId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetProfilesByCompanyId(companyId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetByCompanyId/{id}")]
        public async Task<IHttpActionResult> GetByCompanyId(int id)
        {
            var companyId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetProfilesByCompanyId(companyId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetByGroupId/{userId}/{id}")]
        public async Task<IHttpActionResult> GetByGroupId(int userId, int id)
        {
            var groupId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetProfilesByGroupId(userId, groupId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetByGroupId/{id}")]
        public async Task<IHttpActionResult> GetByGroupId(int id)
        {
            var groupId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetProfilesByGroupId(groupId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetAllProfilesByCompanyId/{id}")]
        public async Task<IHttpActionResult> GetAllProfilesByCompanyId(int id)
        {
            var companyId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetAllProfilesByCompanyId(companyId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetTwitterProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ContactFollowStatusModel> status;
            if (isPersonal)
            {
                status = await repository.GetSocialProfilesPersonalMode(userId, 4);
            }
            else
            {
                status = await repository.GetSocialProfilesByGroupId(userId, groupId, companyId, 4);
            }

            return Ok(status);
        }

        [HttpGet]
        [Route("GetOutlookProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetOutlookProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<OutlookProfileModel> status;

            status = await repository.GetOutlookProfilesByGroupId(userId, groupId, companyId, isPersonal);

            return Ok(status);
        }

        [HttpGet]
        [Route("GetFacebookProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetFacebookProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ContactFollowStatusModel> status;
            if (isPersonal)
            {
                status = await repository.GetSocialProfilesPersonalMode(userId, 1);
            }
            else
            {
                status = await repository.GetSocialProfilesByGroupId(userId, groupId, companyId, 1);
            }
            return Ok(status);
        }

        [HttpGet]
        [Route("GetInstagramProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetInstagramProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ContactFollowStatusModel> status;
            if (isPersonal)
            {
                status = await repository.GetSocialProfilesPersonalMode(userId, 6);
            }
            else
            {
                status = await repository.GetSocialProfilesByGroupId(userId, groupId, companyId, 6);
            }
            return Ok(status);
        }

        [HttpGet]
        [Route("GetAllProfilesByCompanyAndUserId/{companyId}/{userId}")]
        public async Task<IHttpActionResult> GetAllProfilesByCompanyAndUserId(int companyId, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetAllProfilesByCompanyAndUserId(companyId, userId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetAllProfilesByMemberId/{companyId}/{teamMemberId}")]
        public async Task<IHttpActionResult> GetAllProfilesByMemberId(int companyId, int teamMemberId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetAllProfilesByMemberId(companyId, teamMemberId);
            return Ok(status);
        }

        [HttpPost]
        [Route("UpdateUserGroupProfle")]
        public async Task<IHttpActionResult> UpdateUserGroupProfle(ProfileAssignementModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdateUserGroupProfiles(model);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetProfilesByCompanyAndGroupId/{companyId}/{groupId}")]
        public async Task<IHttpActionResult> GetProfilesByCompanyAndGroupId(int companyId, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetProfilesByCompanyAndGroupId(companyId, groupId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetProfilesNotInGroup/{companyId}/{groupId}")]
        public async Task<IHttpActionResult> GetProfilesNotInGroup(int companyId, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetProfilesNotInGroup(companyId, groupId);
            return Ok(status);
        }
        [HttpPost]
        [Route("AssignGroup")]
        public async Task<IHttpActionResult> AssignGroup(List<ProfileCompanyGroup> model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.AssignGroup(model);
            return Ok(status);
        }

        //GetConsumerPattern
        [HttpGet]
        [Route("GetConsumerPattern")]
        public async Task<IHttpActionResult> GetConsumerPattern()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetPattern();
            return Ok(status);
        }

        [HttpPost]
        [Route("UpdatePattern")]
        public async Task<IHttpActionResult> UpdatePattern(SearchWordPattern model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdateConsumerNumPattern(model.Id, model.RegexFor, model.Pattern);
            return Ok(status);
        }

        [HttpGet]
        [Route("UpdateActivePattern/{id}")]
        public async Task<IHttpActionResult> UpdateActivePattern(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdateActivePattern(id);
            return Ok(status);
        }

        [HttpGet]
        [Route("DisablePattern")]
        public async Task<IHttpActionResult> DisablePattern()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.DisablePattern();
            return Ok(status);
        }

        public bool ConnectOutlook(string userid, string password, string domain, string email)
        {
            try
            {
                // exchange.Url = new Uri(ConfigurationManager.AppSettings["EWSUrl"]);

                exchange.Credentials = new WebCredentials(userid, password, domain);
                exchange.AutodiscoverUrl(email);
                return true;
            }
            catch
            {
                return false;
            }

        }

        [HttpPost]
        [Route("AddOutlookAccount")]
        public async Task<IHttpActionResult> AddOutlookAccount(OutlookAccount model)
        {
            try
            {
                bool is_connected = ConnectOutlook(model.UserId, model.Passwrd, model.Domain, model.Email);

                if (is_connected)
                {
                    ProfileTransactions transact;
                    int memberUserId = this.User.Identity.GetUserId<int>();
                    string userInfo = memberUserId + "," + model.CompanyID + "," + model.GroupID;
                   // var isBaseAccount = false;
                    //if (model.Type != null)
                    //{
                    //    isBaseAccount = model.Type.Contains(",") ? model.Type.Split(',')[1] : "";
                    //    if (!string.IsNullOrEmpty(isBaseAccount))
                    //    {
                    //        model.Type = model.Type.Split(',')[0];
                    //    }
                    //}

                    string userId = model.UserId;
                    string userName = model.UserId;
                    string name = model.UserId;
                    string accessToken = model.Passwrd;
                    string accessTokenSecret = model.Domain;

                    transact = new ProfileTransactions(model.CompanyID, model.GroupID, memberUserId, model.Email, userName, name, accessToken, accessTokenSecret, 8, true, "/Content/images/mail2.jpg", exchange.Url.ToString(), Convert.ToBoolean(model.Type), false, false);
                    int transactId = transact.insertProfile();

                    return Ok(transactId);

                }
                else
                {
                    return Ok(-1);
                }
            }
            catch (Exception ex)
            {
                return Ok(-1);
            }
        }
        
    }
}
