using CRM.Common.Enum;
using CRM.Entities;
using SocialCRM.Web.Models.Company;
using SocialCRM.Web.Models.Team;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        public async Task<CompanyInfo> GetCompany(int companyid)
        {
            var result = new CompanyInfo();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.Companies.Where(x => x.id == companyid).Select(y => new CompanyInfo
                    {
                        CompanyId = y.id,
                        CompanyName = y.CompanyName
                    }).FirstOrDefault();
                    db.Dispose();
                }
            });
            return result;
        }

        public Task<bool> AddCompany(Company companyInfo)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteCompany(int companyId)
        {
            throw new NotImplementedException();
        }
        public async Task<List<CompanyModel>> GetByUserId(int userId)
        {
            var result = new List<CompanyModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.UserCompanies.Where(x => x.UserId == userId).Select(y => new CompanyModel
                    {
                        CompanyId = y.CompanyId,
                        CompanyName = y.Company.CompanyName,
                        CompanyGroups = y.Company.Groups.Where(t => t.IsActive).Select(z => new Groups { GroupId = z.Id, GroupName = z.Name }).ToList()
                    }).ToList();
                    db.Dispose();
                }
            });
            return result;
        }
        public Task<List<CompanyModel>> GetByGroupId(int userId)
        {
            throw new NotImplementedException();
        }
        public async Task<CompanyModel> GetByCompanyId(int companyId)
        {
            var result = new CompanyModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.Companies.Where(x => x.id == companyId).Select(z => new CompanyModel { CompanyId = z.id, CompanyName = z.CompanyName, CompanyGroups = z.Groups.Select(y => new Groups { GroupId = y.Id, GroupName = y.Name }).ToList() }).FirstOrDefault();
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> AddMember(int userId, int companyId, int companyTypeId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {

                var isExist = db.UserCompanies.Where(x => x.CompanyId == companyId && x.UserId == userId).FirstOrDefault();
                if (isExist == null)
                {
                    var member = new UserCompany { UserId = userId, CompanyId = companyId, CompanyTypeId = companyTypeId, RegistrationDate = DateTime.Now };
                    db.UserCompanies.Add(member);
                    var resultset = await db.SaveChangesAsync();
                    result = resultset > 0;
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> EnableGroupNotification(int userId, MemberViewModel model)
        {
            var groupIds = new List<int>();
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var notificaiton = new List<UserNotificationSetting>();
                if (model.IsAdmin == 0)
                {
                    groupIds = model.AssignedGroups;
                }
                else
                {
                    groupIds = db.Groups.Where(x => x.CompanyId == model.CompanyId).Select(y => y.Id).ToList();
                }
                groupIds.ForEach(x => { notificaiton.Add(new UserNotificationSetting { CompanyId = model.CompanyId, GroupId = x, UserId = userId, IsNotificationEnable = true, IsEmailEnable = true }); });
                db.UserNotificationSettings.AddRange(notificaiton);
                var resultset = await db.SaveChangesAsync();
                result = resultset > 0;
            }
            return result;
        }
        public async Task<CompanyViewModel> GetTeamGroupProfileCount(int companyId)
        {
            CompanyViewModel result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var company = db.Companies.Where(c => c.id == companyId).FirstOrDefault();
                    if (company != null)
                    {
                        result = new CompanyViewModel { TotalMembers = company.Teams.Count(), TotalGroups = company.Groups.Count(), TotalProfiles = company.AnalyticCompanyGroups.GroupBy(g => g.AnalyticId).Count() + company.ProfileComapnyGroups.Where(x => x.ProfileId.HasValue).GroupBy(p => p.ProfileId).Count() + company.ProfileComapnyGroups.Where(x => x.ProfilePageId.HasValue).GroupBy(p => p.ProfilePageId).Count() };

                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> UpdateCompany(CompanyModel model)
        {
            var success = false;

            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {

                    var company = db.Companies.Where(c => c.id == model.CompanyId).FirstOrDefault();
                    if (company != null)
                    {
                        var isUserExist = db.AspNetUsers.Where(u => u.Email.ToLower() == model.UserEmail.ToLower()).Any();
                        if (isUserExist)
                        {
                            company.CompanyName = model.CompanyName;
                        }
                    }
                    var resultCount = db.SaveChanges();
                    success = resultCount > 0;
                    db.Dispose();
                }
            });
            return success;
        }
        public async Task<CompanyModel> GetByUserEmail(string userEmail)
        {
            var result = new CompanyModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var teamMember = db.Teams.Where(t => string.Compare(t.EmailId, userEmail, true) == 0 && !t.IsVerified).FirstOrDefault();
                    if (teamMember != null)
                    {
                        result = new CompanyModel
                        {
                            CompanyId = teamMember.Company.id,
                            CompanyName = teamMember.Company.CompanyName,
                            UserEmail = userEmail
                        };
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<Dictionary<int, string>> GetUserRights(int userId)
        {
            var rightsDDL = new Dictionary<int, string>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    /*var user = db.UserCompanies.ToList().Where(x => x.UserId == userId).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Company.SubscriberType.MembershipPermissions != null && user.Company.SubscriberType.MembershipPermissions.Count > 0)
                        {
                            var permissions = user.Company.SubscriberType.MembershipPermissions.Where(y => y.Enabled).Select(x => x.Premission).ToList();*/
                            var permissions = db.Premissions.ToList();
                            foreach (Premission p in permissions)
                            {
                                rightsDDL.Add(p.Id, p.Name);
                            }
                        /*}
                    }*/
                    db.Dispose();
                }
            });
            return rightsDDL;
        }

        public async Task<Dictionary<int, string>> GetReportRights(int userId)
        {
            var reportsDDL = new Dictionary<int, string>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    /*var user = db.UserCompanies.Where(x => x.UserId == userId).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Company.SubscriberType.MembershipAccessLevels != null && user.Company.SubscriberType.MembershipAccessLevels.Count > 0)
                        {
                            var accessLevels = user.Company.SubscriberType.MembershipAccessLevels.Where(y => y.Enabled).Select(x => x.AccessLevel).ToList();*/
                            var accessLevels = db.AccessLevels.ToList();
                            foreach (AccessLevel al in accessLevels)
                            {
                                reportsDDL.Add(al.Id, al.Name);
                            }
                        /*}
                    }*/
                    db.Dispose();
                }
            });
            return reportsDDL;
        }

        public async Task<bool> ActivateCompany(int userId, int companyId)
        {
            var success = false;
            using (var db = new SocialCRMEntities())
            {
                var user = db.AspNetUsers.Where(u => u.Id == userId).FirstOrDefault();
                var team = db.Teams.Where(t => t.UserId == userId && t.CompanyId == companyId).FirstOrDefault();
                if (user != null && team != null)
                {
                    if (!user.EmailConfirmed)
                        user.EmailConfirmed = true;
                    team.IsActive = true;
                    team.IsVerified = true;
                    var isExecutionResult = await db.SaveChangesAsync();
                    success = isExecutionResult > 0;
                }
                db.Dispose();
            }
            return success;
        }
        public async Task<bool> DeActivateCompany(int userId, int companyId, bool isFirstTime)
        {
            var success = false;
            using (var db = new SocialCRMEntities())
            {
                var result = db.DeactivateUserAccount(userId, companyId, isFirstTime).FirstOrDefault();
                var exeStatus = await db.SaveChangesAsync();
                success = result.HasValue ? result.Value : exeStatus > 0;
                db.Dispose();
            }
            return success;
        }
        public async Task<Models.User.RegisterViewModel> IsCompanyActivated(int companyId, int userId)
        {
            Models.User.RegisterViewModel result = new Models.User.RegisterViewModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var teamMember = db.Teams.Where(t => t.CompanyId == companyId && t.UserId == userId).FirstOrDefault();
                    bool isVerified = false;
                    if (teamMember != null)
                    {
                        isVerified = (teamMember.IsVerified);
                        if (isVerified)
                        {
                            var userAccountType = (from uc in db.UserCompanies
                                                   join c in db.Companies
                                                   on uc.CompanyId equals c.id
                                                   where uc.UserId == userId && uc.CompanyTypeId == (int)CompanyTypes.Owned
                                                   select c).FirstOrDefault();

                            result.IsCompanyActivated = true;
                            //result.AccessReports = teamMember.AccessLevels.Select(x => x.Id).ToList();
                            result.AccessReports = teamMember.UserAccessLevels.Select(x=>x.AccessLevel.Id).ToList();
                            result.AccessRights = teamMember.Premissions.Select(x => x.Id).ToList();
                        }
                        else
                        {
                            result.IsCompanyActivated = false;
                        }
                    }
                    else
                    {
                        result.IsCompanyActivated = true;
                    }

                    db.Dispose();
                }
            });
            return result;
        }
    }
}