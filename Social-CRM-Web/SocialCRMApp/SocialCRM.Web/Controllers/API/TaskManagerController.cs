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
namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Task")]
    public class TaskManagerController : ApiController
    {
        ITaskRepository taskRepository;
        public TaskManagerController(ITaskRepository repo)
        {
            taskRepository = repo;
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create(TaskModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.CreateTask(model, userId);
            return Ok(status);
        }
        [HttpPost]
        [Route("Delete/{activityId}")]
        public async Task<IHttpActionResult> Delete(int activityId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.DeleteById(activityId);
            return Ok(status);
        }
        [HttpPost]
        [Route("AddComment")]
        public async Task<IHttpActionResult> Comment(TaskCommentModel comment)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.AddComment(comment, userId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetLog/{taskId}")]
        public async Task<IHttpActionResult> Logs(int taskId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.GetLogByTaskId(taskId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetComment/{activityId}")]
        public async Task<IHttpActionResult> GetComment(int activityId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.GetCommentByActivityId(activityId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetTaskClassification/{activityId}/{activityTable}/{groupId}")]
        public async Task<IHttpActionResult> GetTaskClassification(int activityId, string activityTable, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await taskRepository.GetTaskClassification(activityId, activityTable, groupId);
            return Ok(status);
        }
        [HttpGet]
        [Route("GetUserTaskByType/{taskTypeId}/{groupId}")]
        public async Task<IHttpActionResult> GetUserTaskByType(int taskTypeId,int groupId)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetUserTaskByType(taskTypeId, groupId, userId);
            /*taskResult.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.ProfileUrl)&&x.ProfileUrl.StartsWith("~"))
                    x.ProfileUrl = System.Web.HttpContext.Current.Request.GetFullUrl(x.ProfileUrl);
            });*/
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("GetFilteredTask")]
        public async Task<IHttpActionResult> GetFilteredTask(TaskSearchModel searchModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || searchModel==null)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetFilteredTask(userId,searchModel);
            taskResult.ForEach(x =>
           {
               if (!string.IsNullOrEmpty(x.UserProfile) && x.UserProfile.StartsWith("~"))
                   x.UserProfile = System.Web.HttpContext.Current.Request.GetFullUrl(x.UserProfile);
           });
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("GetFilteredDraftTask/{GroupId}")]
        public async Task<IHttpActionResult> GetFilteredDraftTask(int GroupId)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetFilteredDraftTask(userId, GroupId);
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("AddBrandKeyword")]
        public async Task<IHttpActionResult> AddBrandKeyword(BrandKeywordModel brandModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || brandModel == null)
            {
                return BadRequest(ModelState);
            }
            brandModel.UserId = userId;
            int taskResult = -1;
            taskResult = await taskRepository.AddBrandKeyword(brandModel);

            return Ok(taskResult);
        }

        [HttpPost]
        [Route("EditBrandKeyword")]
        public async Task<IHttpActionResult> EditBrandKeyword(BrandKeywordModel brandModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || brandModel == null)
            {
                return BadRequest(ModelState);
            }
            brandModel.UserId = userId;
            int taskResult = -1;
            taskResult = await taskRepository.EditBrandKeyword(brandModel);

            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetBrandKeyword/{groupId}")]
        public async Task<IHttpActionResult> GetBrandKeyword(int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetBrandKeywordByGroupId(groupId);
            return Ok(taskResult);
        }
        [HttpGet]
        [Route("GetBrandKeywordByCompanyId/{companyId}")]
        public async Task<IHttpActionResult> GetBrandKeywordByCompanyId(int companyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetBrandKeywordByCompanyId(companyId);
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("RemoveBrandKeyword/{brandKeywordId}")]
        public async Task<IHttpActionResult> RemoveBrandKeyword(int brandKeywordId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.RemoveBrandKeyword(brandKeywordId);
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("UpdateBrandKeyword")]
        public async Task<IHttpActionResult> UpdateBrandKeyword(BrandKeywordModel brandModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || brandModel == null)
            {
                return BadRequest(ModelState);
            }
            brandModel.UserId = userId;
            var taskResult = await taskRepository.UpdateBrandKeyword(brandModel);
            return Ok(taskResult);
        }


        #region Brand Keyword Category

        [HttpPost]
        [Route("AddBrandKeywordCategory")]
        public async Task<IHttpActionResult> AddBrandKeywordCategory(BrandKeywordCategoryModel brandModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || brandModel == null)
            {
                return BadRequest(ModelState);
            }
            brandModel.UserId = userId;
            int taskResult = -1;
            taskResult = await taskRepository.AddBrandKeywordCategory(brandModel);

            return Ok(taskResult);
        }

        [HttpPost]
        [Route("EditBrandKeywordCategory")]
        public async Task<IHttpActionResult> EditBrandKeywordCategory(BrandKeywordCategoryModel brandModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || brandModel == null)
            {
                return BadRequest(ModelState);
            }
            brandModel.UserId = userId;
            int taskResult = -1;
            taskResult = await taskRepository.EditBrandKeywordCategory(brandModel);

            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetBrandKeywordCategory/{groupId}")]
        public async Task<IHttpActionResult> GetBrandKeywordCategory(int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetBrandKeywordCategoryByGroupId(groupId);
            return Ok(taskResult);
        }
        [HttpGet]
        [Route("GetBrandKeywordCategoryByCompanyId/{companyId}")]
        public async Task<IHttpActionResult> GetBrandKeywordCategoryByCompanyId(int companyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetBrandKeywordCategoryByCompanyId(companyId);
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("RemoveBrandKeywordCategory")]
        public async Task<IHttpActionResult> RemoveBrandKeywordCategory(BrandKeywordCategoryModel brandModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.RemoveBrandKeywordCategory(brandModel.Id);
            return Ok(taskResult);
        }
        [HttpPost]
        [Route("UpdateBrandKeywordCategory")]
        public async Task<IHttpActionResult> UpdateBrandKeywordCategory(BrandKeywordCategoryModel brandModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || brandModel == null)
            {
                return BadRequest(ModelState);
            }
            brandModel.UserId = userId;
            var taskResult = await taskRepository.UpdateBrandKeywordCategory(brandModel);
            return Ok(taskResult);
        }

        #endregion



        //api/Task/RemoveComment/{commentId}/{groupId}
        [HttpPost]
        [Route("RemoveComment/{commentId}/{groupId}")]
        public async Task<IHttpActionResult> GetComment(int commentId, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await taskRepository.RemoveCommentById(commentId, groupId, userId);
            return Ok(status);
        }

        [HttpPost]
        [Route("InsertTaskCompleted")]
        public async Task<IHttpActionResult> InsertTaskCompleted(TaskCompletedModel markAsCompleteModel)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid || markAsCompleteModel == null)
            {
                return BadRequest(ModelState);
            }
            markAsCompleteModel.CompletedBy = userId;
            var taskResult = await taskRepository.InsertTaskCompleted(markAsCompleteModel);
            return Ok(taskResult);
        }

        [HttpPost]
        [Route("DeleteTaskCompleted/{ActivityId}/{ActivityTable}/{GroupId}")]
        public async Task<IHttpActionResult> DeleteTaskCompleted(int ActivityId, string ActivityTable, int GroupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.DeleteTaskCompleted(ActivityId, ActivityTable, GroupId);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetProfiles/{UserIds}/{CommaSeperatedPages}/{GroupId}")]
        public async Task<IHttpActionResult> GetSocialProfiles(string UserIds, string CommaSeperatedPages, int GroupId)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<SocialProfile> result = null;
            result = await taskRepository.GetCustomerSocialProfiles(UserIds, CommaSeperatedPages, GroupId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetCustomerTasks/{CustomerProfileId}/{GroupId}")]
        public async Task<IHttpActionResult> GetCustomerTasks(string CustomerProfileId, int GroupId)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<TaskViewModel> result = null;
            result = await taskRepository.GetCustomerTask(CustomerProfileId, GroupId);
            return Ok(result);
        }
    }
}
