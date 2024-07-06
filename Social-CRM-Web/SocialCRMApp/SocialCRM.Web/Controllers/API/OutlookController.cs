using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SocialCRM.Web.Repositories.Core;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.UnifiedContact;
using CRM.Entities;
using Microsoft.Owin;
using SocialCRM.Web.Models.UnifiedInbox;
using SocialCRM.Web.Models.Twitter;
using SocialAPIsWrapper.Models;

namespace SocialCRM.Web.Controllers.API
{
    [RoutePrefix("api/Outlook")]
    public class OutlookController : ApiController
    {
        IOutlookRepository repository;
        public OutlookController(IOutlookRepository repo)
        {
            repository = repo;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetOutlookInboxEmails(string email, string lastdt, string index, string maxdt, string st)
        {
           if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetOutlookInboxData(email, lastdt, index, maxdt, st);
            return Ok(status);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetOutlookNewInboxEmails(string email, string lastdt, string index, string maxdt, string st)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetOutlookInboxData(email, lastdt, index, maxdt, st);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> SetReadStatus(OutlookEmailRead email)
        {
            try
            {
                var status = await repository.SetReadStatus(email.EmailAddress, email.EmailRecDate);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetOutlookSentEmails(string email, string lastdt, string index, string maxdt, string st)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetOutlookSentData(email, lastdt, index, maxdt, st);
            return Ok(status);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetOutlookNewSentEmails(string email, string lastdt, string index, string maxdt, string st)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetOutlookSentData(email, lastdt, index, maxdt, st);
            return Ok(status);
        }
    }
}