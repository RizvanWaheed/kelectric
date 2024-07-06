using SocialCRM.Web.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IDashboardRepository
    {
        Task<DashboardModel> GetDashboardInformation(int userId, int companyId, int groupId);
        Task<DashboardModel> FilterGroupKeyStatistics(int userId, int companyId, int groupId, DateTime? dateFrom, DateTime? dateTo);
    }
}