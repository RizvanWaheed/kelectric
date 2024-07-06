using SocialCRM.Web.Models.TaskManager;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using CRM.Common.Extensions;
using SocialCRM.Web.Models.Report;
using System.Globalization;
using SocialCRM.Web.Models.Admin;
using Newtonsoft.Json;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        IAdminRepository adminRepository;

        public AdminController(IAdminRepository repo)
        {
            adminRepository = repo;
        }

        [HttpGet]
        [Route("GetCompanyDetails")]
        public async Task<IHttpActionResult> GetCompanyDetails()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await adminRepository.GetCompanyDetails();
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("ChangeCompanyStatus/{companyId}/{status}")]
        public async Task<IHttpActionResult> ChangeCompanyStatus(int companyId, int status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await adminRepository.ActivateDeactivateCompany(companyId,status);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetMembershipPackage/{subscriberId}")]
        public async Task<IHttpActionResult> GetMembershipPackage(int subscriberId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await adminRepository.GetMembershipPackage(subscriberId);
            return Ok(taskResult);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateMembershipPackage(PackageModel packageValue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await adminRepository.UpdateMembershipPackage(packageValue);
            return Ok();
        }
    }
}