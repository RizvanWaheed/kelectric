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

namespace SocialCRM.Web.Controllers.API
{
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        ITaskRepository taskRepository;
        IDashboardRepository dashboardRepository;

        public DashboardController(ITaskRepository repo, IDashboardRepository dashboardRepo)
        {
            taskRepository = repo;
            dashboardRepository = dashboardRepo;
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetGroupPerformance(FilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.GetGroupPerformance(userId,model.companyId, model.classificationId, model.dateFrom, model.dateTo);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetTopInfluencers(FilterModel model)
        {           

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.GetTopInfluencers(model.companyId, model.dateFrom, model.dateTo);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetDashboardInfo/{companyId}/{groupId}")]
        public async Task<IHttpActionResult> GetDashboardInfo(int companyId, int groupId)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await dashboardRepository.GetDashboardInformation(userId, companyId, groupId);
            return Ok(taskResult);
        }

        [HttpPost]
        [Route("FilterGroupKeyStatistics/")]
        public async Task<IHttpActionResult> FilterGroupKeyStatistics(GroupKeyStatsFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            DateTime? fromDate = null, toDate = null;

            if ((!String.IsNullOrEmpty(model.dateFrom)))
                fromDate = DateTime.ParseExact(model.dateFrom, "d", CultureInfo.InvariantCulture);

            if ((!String.IsNullOrEmpty(model.dateTo)))
                toDate = DateTime.ParseExact(model.dateTo, "d", CultureInfo.InvariantCulture);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await dashboardRepository.FilterGroupKeyStatistics(userId, model.companyId, model.groupId, fromDate, toDate);
            return Ok(taskResult);
        }
        
    }
}