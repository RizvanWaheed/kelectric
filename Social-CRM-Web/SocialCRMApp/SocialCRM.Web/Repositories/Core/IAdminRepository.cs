using SocialCRM.Web.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IAdminRepository
    {
        Task<List<CompanyDetailsModel>> GetCompanyDetails();
        Task<bool> ActivateDeactivateCompany(int companyId, int status);
        Task<PackageModel> GetMembershipPackage(int subscriberId);
        Task<bool> UpdateMembershipPackage(PackageModel package);
    }
}
