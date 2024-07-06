using CRM.Entities;
using SocialCRM.Web.Models.Company;
using SocialCRM.Web.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface ICompanyRepository
    {
        Task<CompanyInfo> GetCompany(int companyId);
        Task<bool> AddCompany(Company companyInfo);
        Task<bool> DeleteCompany(int companyId);
        Task<CompanyModel> GetByCompanyId(int companyId);
        Task<List<CompanyModel>> GetByGroupId(int userId);
        Task<List<CompanyModel>> GetByUserId(int userId);
        Task<bool> AddMember(int userId, int companyId, int companyTypeId);
        Task<bool> EnableGroupNotification(int userId, MemberViewModel model);
        Task<CompanyViewModel> GetTeamGroupProfileCount(int companyId);
        Task<bool> UpdateCompany(CompanyModel companyInfo);
        Task<CompanyModel> GetByUserEmail(string userEmail);
        Task<bool> ActivateCompany(int userId, int companyId);
        Task<bool> DeActivateCompany(int userId, int companyId, bool isFirstTime);
        Task<Models.User.RegisterViewModel> IsCompanyActivated(int companyId, int userId);
        Task<Dictionary<int, string>> GetUserRights(int userId);
        Task<Dictionary<int, string>> GetReportRights(int userId);
    }
}
