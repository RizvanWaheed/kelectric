using CRM.Common.Enum;
using SocialCRM.Web.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IReportRepository
    {
        Task<TaskPerformanceReportModel> GetPerformanceReport(int userId, int groupId, int companyId, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum);
        Task<TeamReportModel> GetTeamReport(int userId, int groupId, int companyId, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum, string selectedTeamMembers, bool isCurrUserAdmin, int currUserTeamId);
        List<KeyValuePair<string, int>> GetTwitterProfilePostsAndConversation(FilterModel model);
        Task<List<CommentAndChatModel>> GetCommentAndChatByTeam(int teamId, string profileType, string activityType, string profileId, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum);
        Task<ReplyModel> GetQuickOrSlowReplyByTeam(int teamId, string profileType, string profileId, string replyType, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum);
        List<CRM.Entities.GetInOutboundReport_Result> GetInOutBoundReport(InOutBoundFilterModel model);
        CrisisReportDataModel CrisisReportData();
        List<CRM.Entities.GetInOutboundReportDrillDown_Result> GetInOutBoundReportDrillDown(InOutBoundFilterModel model);
        List<OnDutyAgentModel> GetOnDutyAgents(int companyId, DateTime date, TimeSpan fromShift, TimeSpan toShift);
        List<string> GetEmails(int companyId);
        bool UpdateCrisisData(bool isCrisis);
        void BuildAgentPerformanceReport(string startDate="");
    }
}
