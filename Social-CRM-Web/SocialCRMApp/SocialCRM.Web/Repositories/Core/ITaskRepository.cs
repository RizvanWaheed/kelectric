using SocialCRM.Web.Models.TaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface ITaskRepository
    {
        Task<bool> CreateTask(TaskModel model, int userId);
        Task<bool> DeleteById(int activityId);
        Task<bool> AddComment(TaskCommentModel comment,int userId);
        Task<List<TaskLogModel>> GetLogByTaskId(int taskId);
        Task<List<TaskViewModel>> GetUserTaskByType(int taskTypeId,int groupId, int userId);
        Task<List<TaskCommentModel>> GetCommentByActivityId(int activityId);
        Task<List<TaskViewModel>> GetFilteredTask(int userId,TaskSearchModel searchModel);
        Task<List<TaskViewModel>> GetFilteredDraftTask(int userId, int GroupId);
        Task<int> AddBrandKeyword(BrandKeywordModel brandModel);
        Task<int> EditBrandKeyword(BrandKeywordModel brandModel);
        Task<List<BrandKeywordViewModel>> GetBrandKeywordByGroupId(int groupId);
        Task<List<BrandKeywordViewModel>> GetBrandKeywordByCompanyId(int companyId);
        Task<bool> RemoveBrandKeyword(int brandKeywordId);
        Task<bool> UpdateBrandKeyword(BrandKeywordModel brandModel);

        Task<int> AddBrandKeywordCategory(BrandKeywordCategoryModel brandModel);
        Task<int> EditBrandKeywordCategory(BrandKeywordCategoryModel brandModel);
        Task<List<BrandKeywordCategoryModel>> GetBrandKeywordCategoryByGroupId(int groupId);
        Task<List<BrandKeywordCategoryModel>> GetBrandKeywordCategoryByCompanyId(int companyId);
        Task<bool> RemoveBrandKeywordCategory(int brandKeywordId);
        Task<bool> UpdateBrandKeywordCategory(BrandKeywordCategoryModel brandModel);


        Task<TaskClassificationModel> GetTaskClassification(int activityId, string activityTable, int groupId);
        Task<bool> RemoveCommentById(int commentId, int groupId,int userId);
        Task<bool> InsertTaskCompleted(TaskCompletedModel markAsCompleteModel);
        Task<bool> InsertAllTaskCompleted(TaskCompletedModel markAsCompleteModel);
        Task<bool> DeleteTaskCompleted(int ActivityId, string ActivityTable, int GroupId);
        Task<List<TaskGroupPerformanceModel>> GetGroupPerformance(int userId, int companyId, int classificationId, DateTime? dateFrom, DateTime? dateTo);
        Task<List<TopInfluencerModel>> GetTopInfluencers(int companyId, DateTime? dateFrom, DateTime? dateTo);
        Task<List<SocialProfile>> GetCustomerSocialProfiles(string userIds, string commaSeperatedPages, int groupId);
        Task<List<TaskViewModel>> GetCustomerTask(string customerProfileId, int groupId);
    }
}
