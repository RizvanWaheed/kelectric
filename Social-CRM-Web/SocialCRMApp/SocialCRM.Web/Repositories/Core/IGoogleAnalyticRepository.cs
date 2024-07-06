using SocialCRM.Web.Models.GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
   public interface IGoogleAnalyticRepository
    {
        Task DeleteAsync<T>(string key);
        Task<T> GetAsync<T>(string key);
        Task StoreAsync<T>(string key, T value);
        int GetUserAnalyticAccountNumber(int userId);
        List<AnalyticsModel> UpdateAnalyticAccount(List<AnalyticsModel> analytics,string user);
       /// <summary>
       /// This Method assign analytic to specific company and group.
       /// </summary>
       /// <param name="analtyics"></param>
       /// <returns></returns>
        bool AssignAnalytic(List<AnalyticCompanyGroupModel> analytics);
        bool AssignAnalyticUser(List<AnalyticCompanyGroupModel> analytics, List<AnalyticCompanyUserModel> user);
        AnalyticsResultModel GetAnalyticResult(int accountId);
        
    }
}
