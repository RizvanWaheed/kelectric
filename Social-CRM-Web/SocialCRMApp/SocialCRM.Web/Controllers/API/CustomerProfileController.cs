using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.Profile;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SocialCRM.Web.Controllers.API
{
    [RoutePrefix("api/CustomerProfile")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerProfileController : ApiController
    {
        IBannedProfilesRepository profileRepository;
        ICustomerProfileRepository customerRepository;
        public CustomerProfileController(IBannedProfilesRepository repo, ICustomerProfileRepository repo2)
        {
            profileRepository = repo;
            customerRepository = repo2;
        }

        [HttpPost]
        [Route("GetUsersList/")]
        public async Task<IHttpActionResult> GetUsersList(BannedUserSearchModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taskResult = await profileRepository.GetWarnBanUsersList(userId, model.ReasonIds, model.PlatformId, model.Name, model.UserHandle, model.GroupId, model.GetOptions, model.PageNumber, model.Type);
            return Ok(taskResult);
        }

        [HttpPost]
        [Route("SaveBannedProfiles/")]
        public async Task<IHttpActionResult> SaveBannedProfiles(BannedProfilesSaveModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await profileRepository.SaveWarnBanUser(model, userId);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("DeleteBannedProfile/{id}")]
        public async Task<IHttpActionResult> DeleteBannedProfile(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await profileRepository.DeleteUser(id);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("BanUnbanUser/{id}/{actionType}")]
        public async Task<IHttpActionResult> BanUnbanUser(int id, string actionType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (actionType == "ban")
            {
                var taskResult = await profileRepository.BanUser(id);
                return Ok(taskResult);
            }
            else
            {
                var taskResult = await profileRepository.UnBanUser(id);
                return Ok(taskResult);
            }
            
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadKECustomers(CustomerFileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.UploadKECustomers(model, HttpContext.Current.Request.PhysicalApplicationPath);
            return Ok(taskResult);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadKESocialCustomers(CustomerFileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.UploadKESocialCustomers(model, HttpContext.Current.Request.PhysicalApplicationPath);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetBannedProfilesOptions/{groupId}")]
        public async Task<IHttpActionResult> GetBannedProfilesOptions(int groupId)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var taskResult = await profileRepository.GetBannedProfilesOptions(userId, groupId);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetAttachmentAsBase64String/{id}")]
        public async Task<IHttpActionResult> GetAttachmentAsBase64String(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await profileRepository.GetAttachmentAsBase64String(id);
            return Ok(taskResult);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetWarnBanUserReport(GetWarnBanUserReportFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DateTime? fromDate = null, toDate = null;
            TimeSpan? fromTime = null, toTime = null;

            if ((!String.IsNullOrEmpty(model.fromDate)))
                fromDate = DateTime.ParseExact(model.fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if ((!String.IsNullOrEmpty(model.toDate)))
                toDate = DateTime.ParseExact(model.toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if ((!String.IsNullOrEmpty(model.fromTime)))
                fromTime = TimeSpan.Parse(model.fromTime);

            if ((!String.IsNullOrEmpty(model.toTime)))
                toTime = TimeSpan.Parse(model.toTime);

            var status = await profileRepository.GetWarnBanUserReport(userId, model.groupId, fromDate, toDate, fromTime, toTime);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetCustomerAllInfo(CustomerFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.GetCustomerAllInfo(model);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetCustomerRegions")]
        public async Task<IHttpActionResult> GetCustomerRegions()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.GetCustomerRegions();
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetCustomerAreas/{regionId}")]
        public async Task<IHttpActionResult> GetCustomerAreas(int regionId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.GetCustomerAreas(regionId);
            return Ok(taskResult);
        }

        [HttpPost]
        [Route("WarnBanUser/")]
        public async Task<IHttpActionResult> WarnBanUser(ConsumerProfilesSaveModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await profileRepository.WarnBanUser(model, userId);
            return Ok(taskResult);
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteCustomer(CustomerAllInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.DeleteCustomer(model.ContractAccount);
            return Ok(taskResult);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateCustomer(CustomerAllInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.UpdateCustomer(model);
            return Ok(taskResult);
        }

        [HttpPost]
        [Route("GetRegionReport")]
        public async Task<IHttpActionResult> GetRegionReport(RegionReportFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.GetRegionReport(model);
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("GetAreaReport")]
        public async Task<IHttpActionResult> GetAreaReport(RegionReportFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.GetAreaReport(model);
            return Ok(taskResult);
        }
        
        [HttpPost]
        [Route("GetRegionDrillDownReport")]
        public async Task<IHttpActionResult> GetRegionDrillDownReport(RegionReportFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await customerRepository.GetRegionDrillDownReport(model);
            return Ok(taskResult);
        }
    }
}
