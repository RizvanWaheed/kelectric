using CRM.Common;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.Group;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Group")]
    public class GroupController : ApiController
    {
        IGroupRepository repository;
        public GroupController(IGroupRepository repo)
        {
            repository = repo;
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterGroup(RegisterGroup model)
        {
            if (model.CompanyId == null && string.IsNullOrEmpty(model.GroupName))
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var analyticProfile = new List<CRM.Entities.AnalyticCompanyGroup>();
            var profilePage = new List<CRM.Entities.ProfileComapnyGroup>();
            foreach (var profile in model.Profiles)
            {
                switch (profile.ProfileType)
                {
                    case CRM.Common.Enum.ProfilesType.GoogleAnalytics:
                        analyticProfile.Add(new CRM.Entities.AnalyticCompanyGroup {AnalyticId=profile.ProfileId,CompanyId=model.CompanyId});
                        break;
                    case CRM.Common.Enum.ProfilesType.Page:
                        profilePage.Add(new CRM.Entities.ProfileComapnyGroup {ProfilePageId=profile.ProfileId,CompanyId=model.CompanyId });
                        break;
                    case CRM.Common.Enum.ProfilesType.Twitter:
                    case CRM.Common.Enum.ProfilesType.LinkedIn:
                    case CRM.Common.Enum.ProfilesType.Facebook:
                    case CRM.Common.Enum.ProfilesType.Instagram:
                        profilePage.Add(new CRM.Entities.ProfileComapnyGroup {ProfileId=profile.ProfileId, CompanyId = model.CompanyId });
                        break;
                    default:
                        break;
                }
            }
            var status = await repository.AddGroup(new CRM.Entities.Group
            {
                Name = model.GroupName,
                CompanyId = model.CompanyId,
                AnalyticCompanyGroups = analyticProfile,
                ProfileComapnyGroups =profilePage,
                IsActive=true,
                CreatedDate=DateTime.Now
            }, userId);
           return Ok(status);
        }
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        public async Task<IHttpActionResult> UpdateGroup(GroupModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdateGroup(model);
            return Ok(status);
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteGroup(int id)
        {
            var groupId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.DeleteGroup(groupId);
            if (status == "EBUN")
            {
                return BadRequest(status);
            }
            return Ok(status);
        }
        [HttpGet]
        [ResponseType(typeof(GroupViewModel))]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var groupId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetGroupById(groupId);
            return Ok(status);
        }
        [HttpGet]
        [ResponseType(typeof(List<GroupViewModel>))]
        public async Task<IHttpActionResult> GetByUserId(int id)
        {
            var userId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetUserGroups(userId);
            return Ok(status);
        }
        [HttpGet]
        [ResponseType(typeof(List<GroupViewModel>))]
        public async Task<IHttpActionResult> GetUserCompanyGroups()
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetUserCompanyGroups(userId);
            return Ok(status);
        }
        [HttpGet]
        [ResponseType(typeof(List<GroupViewModel>))]
        public async Task<IHttpActionResult> GetByCompanyId(int id)
        {
            var companyId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetCompanyGroups(companyId);
            return Ok(status);
        }
        [HttpGet]
        [Route("RemoveMember/{teamMemberId}/{groupId}")]
        public async Task<IHttpActionResult> RemoveMember(int teamMemberId, int groupId)
        {
            if (teamMemberId == 0 || groupId==0)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.RemoveMember(teamMemberId, groupId);
            return Ok(status);
        }
    }
}
