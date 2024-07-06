using CRM.Entities;
using SocialCRM.Web.Models.Profile;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class CustomerProfileRepository : ICustomerProfileRepository
    {
        public async Task<int> UploadKECustomers(CustomerFileModel model, string applicationPath)
        {
            int result = 0;
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        string saveLocation = applicationPath + "Content\\KECustomers\\" + model.FileName;
                        if (File.Exists(@saveLocation))
                        {
                            File.Delete(@saveLocation);
                        }
                        File.WriteAllBytes(saveLocation, Convert.FromBase64String(model.Base64String.Split(',')[1]));
                        
                        var excelData = new LinqToExcel.ExcelQueryFactory(@saveLocation);
                        //var excelData = new LinqToExcel.ExcelQueryFactory(@"D:/KE-Project/KE-CRM-Media/MediaCRM.Web/Files/Channels/Media Noise Index (IBEX).xlsx");

                        string workSheet = "Sheet1";
                        var allRegions = db.KERegions.ToList();
                        var allAreas = db.KEAreas.ToList();
                        var currentDate = DateTime.Now;

                        DataTable KECustomerType = new DataTable();
                        KECustomerType.Columns.Add(new DataColumn("ContractAccount"));
                        KECustomerType.Columns.Add(new DataColumn("EmailId"));
                        KECustomerType.Columns.Add(new DataColumn("Name"));
                        KECustomerType.Columns.Add(new DataColumn("MobileNum"));
                        KECustomerType.Columns.Add(new DataColumn("LandlineNum"));
                        KECustomerType.Columns.Add(new DataColumn("Address"));
                        KECustomerType.Columns.Add(new DataColumn("KEAreaId"));
                        KECustomerType.Columns.Add(new DataColumn("InsertionDate"));
                        KECustomerType.Columns.Add(new DataColumn("Feeder"));
                        KECustomerType.Columns.Add(new DataColumn("PMT"));
                        KECustomerType.PrimaryKey = new DataColumn[] { KECustomerType.Columns["ContractAccount"] };

                        var query = (from c in excelData.Worksheet(workSheet)
                                        select c).ToList();

                        var columnNamesList = excelData.GetColumnNames(workSheet).ToList();
                        foreach (var newsType in query)
                        {
                            var contractAccount = newsType[columnNamesList.IndexOf("Contract Account")].Value.ToString().Trim();
                            if(!KECustomerType.Rows.Contains(contractAccount) && contractAccount != "")
                            {
                                DataRow custRow = KECustomerType.NewRow();
                                custRow["ContractAccount"] = contractAccount;
                                custRow["EmailId"] = newsType[columnNamesList.IndexOf("Email Address")].Value.ToString().Trim();
                                custRow["Name"] = newsType[columnNamesList.IndexOf("Customer Name")].Value.ToString().Trim();
                                custRow["MobileNum"] = newsType[columnNamesList.IndexOf("Mobile Number")].Value.ToString().Trim();
                                custRow["LandlineNum"] = newsType[columnNamesList.IndexOf("LandLine Number")].Value.ToString().Trim();
                                custRow["Address"] = newsType[columnNamesList.IndexOf("Customer Address")].Value.ToString().Trim();
                                custRow["InsertionDate"] = currentDate;
                                custRow["Feeder"] = newsType[columnNamesList.IndexOf("Feeder Name")].Value.ToString().Trim();
                                custRow["PMT"] = newsType[columnNamesList.IndexOf("PMT Name")].Value.ToString().Trim();
                                var IBC = newsType[columnNamesList.IndexOf("IBC")].Value.ToString().Trim();

                                KEArea area = allAreas.Where(x => x.Name == IBC).FirstOrDefault();
                                if(area != null)
                                {
                                    custRow["KEAreaId"] = area.Id;
                                }
                                KECustomerType.Rows.Add(custRow);
                            }
                        }

                        var parameter = new SqlParameter("@ImportedCustomers", SqlDbType.Structured);
                        parameter.Value = KECustomerType;
                        parameter.TypeName = "dbo.KECustomerType";
                        db.Database.ExecuteSqlCommand("exec dbo.AddCustomerProfiles @ImportedCustomers", parameter);

                        result = KECustomerType.Rows.Count;
                    }
                });
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        public async Task<int> UploadKESocialCustomers(CustomerFileModel model, string applicationPath)
        {
            int result = 0;
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        string saveLocation = applicationPath + "Content\\KECustomers\\" + model.FileName;
                        if (File.Exists(@saveLocation))
                        {
                            File.Delete(@saveLocation);
                        }
                        File.WriteAllBytes(saveLocation, Convert.FromBase64String(model.Base64String.Split(',')[1]));

                        var excelData = new LinqToExcel.ExcelQueryFactory(@saveLocation);
                        string workSheet = "Sheet1";
                        var currentDate = DateTime.Now;

                        var query = (from c in excelData.Worksheet(workSheet)
                                     select c).ToList();

                        var columnNamesList = excelData.GetColumnNames(workSheet).ToList();
                        foreach (var newsType in query)
                        {
                            var contractAccount = newsType[columnNamesList.IndexOf("Account Number")].Value.ToString().Trim();
                            int custId = 0;
                            var userName = newsType[columnNamesList.IndexOf("User Profile URL")].Value.ToString().Trim();
                            var fullName = newsType[columnNamesList.IndexOf("Name on FB/Twitter")].Value.ToString().Trim();
                            var existCust = db.KECustomers.Where(x => x.ContractAccount == contractAccount).FirstOrDefault();

                            if (userName.StartsWith("@"))
                                userName = userName.Substring(1);

                            if (existCust == null)
                            {
                                KECustomer obj = new KECustomer();
                                obj.ContractAccount = contractAccount;
                                obj.InsertionDate = currentDate;
                                existCust = db.KECustomers.Add(obj);

                                db.SaveChanges();
                            }

                            var existSocial = db.CustomerSocialProfiles.Where(x => x.UserName == userName).FirstOrDefault();
                            if(existSocial == null)
                            {
                                CustomerSocialProfile obj = new CustomerSocialProfile();
                                obj.UserName = userName;
                                obj.Name = fullName;
                                obj.Type = "Twitter";
                                existSocial = db.CustomerSocialProfiles.Add(obj);

                                db.SaveChanges();
                            }
                            custId = existSocial.Id;

                            var existProfileRel = existSocial.ProfileToCustSocialProfiles.Where(x => x.ProfileId == model.TwitProfileId && x.CustSocialProfileId == custId).FirstOrDefault();
                            if(existProfileRel == null)
                            {
                                ProfileToCustSocialProfile obj = new ProfileToCustSocialProfile();
                                obj.ProfileId = model.TwitProfileId;
                                obj.CustSocialProfileId = custId;
                                obj.ProfilePageId = 0;
                                existSocial.ProfileToCustSocialProfiles.Add(obj);
                            }

                            var existCustRel = existSocial.KECustomers.Where(x => x.ContractAccount == contractAccount).FirstOrDefault();
                            if (existCustRel == null)
                            {
                                existSocial.KECustomers.Add(existCust);
                            }

                            db.CustomerSocialProfiles.Attach(existSocial);
                            db.Entry(existSocial).State = EntityState.Modified;

                            db.SaveChanges();
                            result++;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        public async Task<List<CustomerAllInfoModel>> GetCustomerAllInfo(CustomerFilterModel model)
        {
            List<CustomerAllInfoModel> result = new List<CustomerAllInfoModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        model.StartDate = model.StartDate.Date;
                        model.EndDate = model.EndDate.Date.Add(new TimeSpan(23, 59, 59));
                        var response = context.GetCustomerProfiles(model.PageNumber, model.PageSize, model.GroupId, model.FBProfileId, 
                            model.FBProfilePageId, model.TwitProfileId, model.RegionId, model.AreaId, model.StartDate, model.EndDate, 
                            model.SearchOptionId, model.SearchValue );
                        foreach (var data in response)
                        {
                            CustomerSocialProfileInfoModel newProfile = new CustomerSocialProfileInfoModel();
                            newProfile.SocialId = data.SocialId;
                            newProfile.SocialExternalId = data.SocialExternalId;
                            newProfile.SocialName = data.SocialName;
                            newProfile.SocialProfilePic = data.SocialProfilePic;
                            newProfile.SocialType = data.SocialType;
                            newProfile.SocialUserName = data.SocialUserName;
                            newProfile.WarnStatus = Convert.ToInt32(data.WarnStatus.Split('-')[0]);
                            newProfile.WarnId = Convert.ToInt32(data.WarnStatus.Split('-')[1]);
                            newProfile.InsertionDate = data.InsertionDate;
                            newProfile.InboundCount = data.InboundCount;
                            newProfile.OutboundCount = data.OutboundCount;

                            CustomerAllInfoModel row = result.Where(x => x.ContractAccount == data.ContractAccount).FirstOrDefault();
                            if (row == null)
                            {
                                CustomerAllInfoModel newRow = new CustomerAllInfoModel();
                                newRow.Address = data.Address;
                                newRow.ContractAccount = data.ContractAccount;
                                newRow.EmailId = data.EmailId;
                                newRow.IBC = data.IBC;
                                newRow.IBC_Code = data.IBC_Code;
                                newRow.LandlineNum = data.LandlineNum;
                                newRow.MobileNum = data.MobileNum;
                                newRow.Name = data.Name;
                                newRow.Region = data.Region;

                                newRow.SocialProfiles = new List<CustomerSocialProfileInfoModel>();
                                newRow.SocialProfiles.Add(newProfile);
                                result.Add(newRow);
                            }
                            else
                            {
                                row.SocialProfiles.Add(newProfile);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<bool> UpdateCustomer(CustomerAllInfoModel customer)
        {
            bool result = true;
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        var dbcust = context.KECustomers.Where(x => x.ContractAccount == customer.ContractAccount).FirstOrDefault();
                        dbcust.Address = customer.Address;
                        dbcust.EmailId = customer.EmailId;
                        var selectedArea = context.KEAreas.Where(x => x.Name == customer.IBC).FirstOrDefault();
                        if(selectedArea == null)
                        {
                            dbcust.KEAreaId = null;
                            dbcust.KEArea = null;
                        }
                        else
                        {
                            dbcust.KEArea = selectedArea;
                        }
                        dbcust.LandlineNum = customer.LandlineNum;
                        dbcust.MobileNum = customer.MobileNum;
                        dbcust.Name = customer.Name;

                        context.KECustomers.Attach(dbcust);
                        context.Entry(dbcust).State = EntityState.Modified;

                        context.SaveChanges();
                        result = true;
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool>DeleteCustomer(string contractAccount)
        {
            bool result = true;
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        context.DeleteKECustomer(contractAccount);
                        result = true;
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<List<CustomerRegionModel>> GetCustomerRegions()
        {
            List<CustomerRegionModel> result = new List<CustomerRegionModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        result = context.KERegions.Select(x => new CustomerRegionModel
                        {
                            Id = x.Id,
                            Name = x.Region
                        }).ToList();
                    }
                });
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<List<CustomerIBCModel>> GetCustomerAreas(int regionId)
        {
            List<CustomerIBCModel> result = new List<CustomerIBCModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        result = context.KERegions.Where(x => x.Id == regionId).FirstOrDefault().KEAreas.Select(x => new CustomerIBCModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code
                        }).ToList();
                    }
                });
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public async Task<List<RegionInboundOutboundModel>> GetRegionReport(RegionReportFilterModel model)
        {
            List<RegionInboundOutboundModel> result = new List<RegionInboundOutboundModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        if (!model.IsShiftReport)
                        {
                            model.StartDate = model.StartDate.Date;
                            model.EndDate = model.EndDate.Date.Add(new TimeSpan(23, 59, 59));
                        }
                        var response = context.GetCustomerRegionReport(model.GroupId,model.SocialPlatform,model.StartDate,model.EndDate,model.ReportId,model.ReportTypeId,model.RegionId, model.IsEvolution, model.IsShiftReport, model.IsUnique);

                        foreach(var data in response)
                        {
                            RegionInboundOutboundModel row = new RegionInboundOutboundModel();
                            row.Region = data.Region;
                            row.Total_Inbound = data.InboundCount.HasValue ? data.InboundCount : 0;
                            row.Total_Outbound = data.OutboundCount.HasValue ? data.OutboundCount : 0;
                            row.Name = data.Name;
                            row.InsertionDate = data.InsertionDate;
                            row.InsertionHour = data.InsertionHour;

                            result.Add(row);
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<List<RegionInboundOutboundModel>> GetAreaReport(RegionReportFilterModel model)
        {
            List<RegionInboundOutboundModel> result = new List<RegionInboundOutboundModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        if (!model.IsShiftReport)
                        {
                            model.StartDate = model.StartDate.Date;
                            model.EndDate = model.EndDate.Date.Add(new TimeSpan(23, 59, 59));
                        }
                        var response = context.GetCustomerAreaReport(model.GroupId, model.SocialPlatform, model.StartDate, model.EndDate, model.ReportId, model.ReportTypeId, model.RegionId, model.IsEvolution, model.IsShiftReport, model.IsUnique);

                        foreach (var data in response)
                        {
                            RegionInboundOutboundModel row = new RegionInboundOutboundModel();
                            row.Region = data.Area;
                            row.Total_Inbound = data.InboundCount.HasValue ? data.InboundCount : 0;
                            row.InsertionDate = data.InsertionDate;
                            row.InsertionHour = data.InsertionHour;
                            result.Add(row);
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<List<RegionInboundOutboundDrillDownModel>> GetRegionDrillDownReport(RegionReportFilterModel model)
        {
            List<RegionInboundOutboundDrillDownModel> result = new List<RegionInboundOutboundDrillDownModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        if (!model.IsShiftReport)
                        {
                            model.StartDate = model.StartDate.Date;
                            model.EndDate = model.EndDate.Date.Add(new TimeSpan(23, 59, 59));
                        }
                        var response = context.GetCustomerRegionDrillDownReport(model.GroupId, model.SocialPlatform, model.StartDate, model.EndDate, model.ReportId, model.ReportTypeId, model.RegionId, model.IsEvolution, model.IsShiftReport, model.IsUnique, model.XAxis, model.PageNumber, model.PageSize);

                        foreach (var data in response)
                        {
                            RegionInboundOutboundDrillDownModel row = new RegionInboundOutboundDrillDownModel();
                            row.Region = data.Region;
                            row.StatusText = data.StatusText;
                            row.PostType = data.PostType;
                            row.CustUserName = data.CustUserName;
                            row.InsertionDate = data.InsertionDate;
                            row.IBC = data.IBC;
                            row.RowNum = data.RowNum;
                            row.Total_Rows = data.TotalRows;

                            result.Add(row);
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}