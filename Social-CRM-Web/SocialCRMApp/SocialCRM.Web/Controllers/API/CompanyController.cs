using CRM.Common;
using SocialCRM.Web.Models.Company;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using System.Web;
using SocialCRM.Web.Models.User;
using System.Text.RegularExpressions;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Company")]
    public class CompanyController : ApiController
    {
        ICompanyRepository repository;
        INotificationsRepository Nrepository;
        public CompanyController(ICompanyRepository repo, INotificationsRepository Nrepo)
        {
            repository = repo;
            Nrepository = Nrepo;
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        public async Task<IHttpActionResult> Register(CompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var status = await repository.AddGroup(new CRM.Entities.Group { Name = model.GroupName, CompanyId = model.CompanyId });
            return Ok();
        }

       // [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        public async Task<IHttpActionResult> Update(CompanyModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var status = await repository.UpdateCompany(model);
            var user = HttpContext.Current.Session["SignUpModel"] as RegisterViewModel;
            user.CompanyName = model.CompanyName;
            HttpContext.Current.Session["SignUpModel"] = user;
            return Ok();
        }
        [ResponseType(typeof(RequestStatus))]
        [HttpPost]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var groupId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var status = await repository.DeleteGroup(groupId);
            return Ok();
        }
        [HttpGet]
        [ResponseType(typeof(List<CompanyModel>))]
        public async Task<IHttpActionResult> GetByUserId(int id)
        {
            var userId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetByUserId(userId);
            return Ok(status);
        }
        [HttpGet]
        [ResponseType(typeof(List<CompanyModel>))]
        public async Task<IHttpActionResult> GetByGroupId(int id)
        {
            var userId = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetByGroupId(userId);
            return Ok(status);
        }
        [HttpGet]
       // [AllowAnonymous]
        [Route("GetDropDown")]
        public async Task<IHttpActionResult> GetDropDown()
        {
            var userId = User.Identity.GetUserId<int>();
            var rightsDDL = await repository.GetUserRights(userId);
            var reportsDDL = await repository.GetReportRights(userId);
            var dropdownValues = new { Rights = rightsDDL, VisibleReports = reportsDDL };
            return Ok(dropdownValues);
        }
        [HttpGet]
        [Route("GetTeamGroupProfileCount/{companyId}")]
        public async Task<IHttpActionResult> GetTeamGroupProfileCount(int companyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTeamGroupProfileCount(companyId);
            return Ok(status);
        }
        [HttpPost]
        [Route("SendEmail")]
        public async Task<IHttpActionResult> SendEmail(BulkEmailModel model)
        {
            var status = false;
            await Task.Run(() =>
             {
                 var userEmail = User.Identity.GetUserName();
                  status = new EmailHandler().SendEmail(userEmail, model.ToUserEmail, model.EmailSubject, Regex.Replace(model.EmailBody, @"\r\n?|\n", "<br />"));
             });
             return Ok(status);
        }

        [HttpPost]
        [Route("ReplyToEmail")]
        public async Task<IHttpActionResult> ReplyToEmail(BulkEmailModel model)
        {
            var status = false;
            await Task.Run(() =>
            {
                status = new EmailHandler().SendEmail(model.FromUserEmail, model.ToUserEmail, model.EmailSubject, Regex.Replace(model.EmailBody, @"\r\n?|\n", "<br />"), model.CCUserEmail);
            });
            return Ok(status);
        }

        [HttpGet]
        [Route("GetCompanyName")]
        public async Task<IHttpActionResult> GetCompanyName()
        {
            var userEmail = User.Identity.GetUserName();
            var status = await repository.GetByUserEmail(userEmail);
            return Ok(status);
        }
        [HttpPost]
        [Route("ActivateCompany/{companyId}")]
        public async Task<IHttpActionResult> ActivateCompany(int companyId)
        {
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.ActivateCompany(userId,companyId);
            return Ok(status);
        }
        [HttpPost]
        [Route("DeActivateCompany/{companyId}/{isFirstTime}")]
        public async Task<IHttpActionResult> DeActivateCompany(int companyId, bool isFirstTime)
        {
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.DeActivateCompany(userId, companyId, isFirstTime);
            return Ok(status);
        }
        [HttpGet]
        [Route("IsCompanyActivatedForUser/{companyId}")]
        public async Task<IHttpActionResult> IsCompanyActivatedForUser(int companyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.IsCompanyActivated(companyId, userId);
            return Ok(status);
        }

    }
}
