using SocialCRM.Web.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories.Core
{
    public interface ICustomerProfileRepository
    {
        Task<int> UploadKECustomers(CustomerFileModel model, string applicationPath);
        Task<int> UploadKESocialCustomers(CustomerFileModel model, string applicationPath);
        Task<List<CustomerAllInfoModel>> GetCustomerAllInfo(CustomerFilterModel model);
        Task<List<CustomerRegionModel>> GetCustomerRegions();
        Task<List<CustomerIBCModel>> GetCustomerAreas(int regionId);
        Task<bool> DeleteCustomer(string contractAccount);
        Task<bool> UpdateCustomer(CustomerAllInfoModel customer);
        Task<List<RegionInboundOutboundModel>> GetRegionReport(RegionReportFilterModel model);
        Task<List<RegionInboundOutboundModel>> GetAreaReport(RegionReportFilterModel model);
        
        Task<List<RegionInboundOutboundDrillDownModel>> GetRegionDrillDownReport(RegionReportFilterModel model);
    }
}