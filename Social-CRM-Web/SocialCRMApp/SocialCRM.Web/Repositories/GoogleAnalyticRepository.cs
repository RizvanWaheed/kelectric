using CRM.Common.Enum;
using CRM.Entities;
using Newtonsoft.Json;
using SocialCRM.Web.Models.GoogleAnalytics;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class GoogleAnalyticRepository : IGoogleAnalyticRepository
    {
        //CRUD Analytics Token
        public async Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            //var accountName = GetAnalyticsAccountName(key.Replace("oauth_", ""));
            var accountName = key;// GetAnalyticsAccountName(key);
            if (key.StartsWith("oauth_", StringComparison.OrdinalIgnoreCase))
            {
                var user = key;// "oauth_" + GetUserName();
                using (var context = new SocialCRMEntities())
                {
                    var item = context.AnalyticTokens.Where(x => x.AccountName.StartsWith(key) && x.AccountName == user).ToList();
                    if (item != null)
                    {
                        foreach (var i in item)
                            context.AnalyticTokens.Remove(i);
                        await context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                var userDetails = key.Split(',').Select(Int32.Parse).ToArray();//userId,CompanyId,GroupId,AccountNumber GetAnalyticsAccountName(key);
                var userId = userDetails[0];
                var companyId = userDetails[1];
                var groupId = userDetails[2];
                var accountNumber = userDetails[3]; 
                using (var context = new SocialCRMEntities())
                {
                    var item = context.AnalyticTokens.Where(x => (x.UserId == userId && x.CompanyId == companyId && x.GroupId == groupId && x.AccountNumber == accountNumber));
                    context.AnalyticTokens.RemoveRange(item);
                    await context.SaveChangesAsync();
                }
            }
        }
        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            if (key.StartsWith("oauth_"))
            {
                using (var context = new SocialCRMEntities())
                {
                    //var generatedKey = GenerateStoredKey(key, typeof(T));
                    var item = context.AnalyticTokens.FirstOrDefault(x => x.AccountName.StartsWith(key));
                    T value = item == null ? default(T) : JsonConvert.DeserializeObject<T>(item.oAuthTokens);
                    return Task.FromResult<T>(value);
                }
            }
            var userDetails = key.Split(',').Select(Int32.Parse).ToArray();//userId,CompanyId,GroupId,AccountNumber GetAnalyticsAccountName(key);
            var userId = userDetails[0];
            var companyId = userDetails[1];
            var groupId = userDetails[2];
            var accountNumber = userDetails[3];
            using (var context = new SocialCRMEntities())
            {
                var item = context.AnalyticTokens.FirstOrDefault(x => (x.AccountNumber == accountNumber));//x.UserId == userId && x.CompanyId == companyId && x.GroupId == groupId && x.AccountNumber == accountNumber));
                T value = item == null ? default(T) : JsonConvert.DeserializeObject<T>(item.oAuthTokens);
                return Task.FromResult<T>(value);
            }
        }
        public  Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            var accountName = key;// GetAnalyticsAccountName(key);
            var userName = key;// GetUserName(key);
            bool status = false;
            if (!string.IsNullOrEmpty(key) && key.StartsWith("oauth_"))
            {
                using (var context = new SocialCRMEntities())
                {
                    string json = JsonConvert.SerializeObject(value);
                    var item = context.AnalyticTokens.SingleOrDefault(x => x.AccountName == accountName);
                    if (item == null)
                    {
                        AnalyticToken analyticsSuscriber = new AnalyticToken() { IsActive = true, DateCreated = DateTime.Now, oAuthTokens = json, AccountName = accountName };
                        context.AnalyticTokens.Add(analyticsSuscriber);
                    }
                    else
                    {
                        item.oAuthTokens = json;
                    }
                    context.SaveChanges();
                    status = true;
                }
            }
            else
            {
                var userDetails = key.Split(',').Select(Int32.Parse).ToArray();//userId,CompanyId,GroupId,AccountNumber GetAnalyticsAccountName(key);
                var userId = userDetails[0];
                var companyId = userDetails[1];
                var groupId = userDetails[2];
                var accountNumber = userDetails[3]; 
                using (var context = new SocialCRMEntities())
                {
                    string json = JsonConvert.SerializeObject(value);
                    var item = context.AnalyticTokens.SingleOrDefault(x => (x.UserId == userId && x.CompanyId == companyId && x.GroupId == groupId && x.AccountNumber == accountNumber));
                    if (item == null)
                    {
                        AnalyticToken analyticsSuscriber = new AnalyticToken()
                        {
                            IsActive = true,
                            DateCreated = DateTime.Now,
                            oAuthTokens = json,
                            AccountName = accountName,
                            UserId = userDetails[0],
                            AccountNumber = userDetails[3],
                            CompanyId = userDetails[1],
                            GroupId = userDetails[2]
                        };
                        context.AnalyticTokens.Add(analyticsSuscriber);
                    }
                    else
                    {
                        item.oAuthTokens = json;
                    }
                    context.SaveChanges();
                    status = true;
                }
            }
            return Task.FromResult<bool>(status);
        }
        //other Operation on Google Analytics
        public int GetUserAnalyticAccountNumber(int userId)
        {
            using (var context = new SocialCRMEntities())
            {
                var userAccounts = context.AnalyticTokens.Where(x=>x.UserId==userId&&x.IsActive).OrderByDescending(o=>o.AccountNumber).FirstOrDefault();
                return userAccounts==null? 1:(++userAccounts.AccountNumber);
            }
        }
        public List<AnalyticsModel> UpdateAnalyticAccount(List<AnalyticsModel> analytics, string user)
        {
            if (analytics == null)
                return new List<AnalyticsModel>();

            var userDetails = user.Split(',').Select(Int32.Parse).ToArray();//userId,CompanyId,GroupId,AccountNumber GetAnalyticsAccountName(key);
            var userId = userDetails[0];
            var companyId = userDetails[1];
            var groupId = userDetails[2];
            var accountNumber = userDetails[3];
            using (var context = new SocialCRMEntities())
            {
                var result = analytics.Select(x => new UserAnalyticAccount
                {
                    AccountId = x.AccountId,
                    AccountName = x.AccountName,
                    AccountUrl = x.AccountUrl,
                    CompanyId = companyId,
                    GroupId = groupId,
                    UserId = userId,
                    IsConnected = true,//at the moment by default all accounts are connected in future it should by configurable.
                    AccountNumber=accountNumber,
                    Id=0,
                    ProfileId=x.ProfileId
                }).ToList();
                var existingAccounts = context.UserAnalyticAccounts.AsEnumerable().Where(x => (
                     //x.UserId == userId && (x.GroupId.HasValue && x.GroupId.Value == groupId) && (x.CompanyId.HasValue && x.CompanyId == companyId) &&
                     result.Select(y => y.AccountId).Contains(x.AccountId))).ToList();
                var newAccounts = context.UserAnalyticAccounts.AsEnumerable().Where(x => (
                     //x.UserId == userId && (x.GroupId.HasValue && x.GroupId.Value == groupId) && (x.CompanyId.HasValue && x.CompanyId == companyId) &&
                     !result.Select(y => y.AccountId).Contains(x.AccountId))).ToList();
                if (existingAccounts.Count() == 0)
                {
                    context.UserAnalyticAccounts.AddRange(result);
                    context.SaveChanges();
                    result.ForEach(p => context.Entry(p).Reload()); 
                    analytics = result.Select(x => new AnalyticsModel
                    {
                        AccountId = x.AccountId,
                        AccountName = x.AccountName,
                        AccountUrl = x.AccountUrl,
                        IsConnected = x.IsConnected,
                        Id = x.Id,
                        ProfileId = x.ProfileId
                    }).ToList();
                }
                else if (newAccounts.Count == 0)
                {
                    //here we need to remove entry to new acceess token.
                    var removeNewToken = context.AnalyticTokens.Where(x => //x.UserId == userId && x.CompanyId == companyId && x.GroupId == groupId && 
                        x.AccountNumber == accountNumber);
                    context.AnalyticTokens.RemoveRange(removeNewToken);
                    context.SaveChanges();

                    //If connected from another account and not from the current account
                    if(existingAccounts.First().UserId != userId)
                        analytics = null;
                    else
                    {
                        analytics = existingAccounts.Select(x => new AnalyticsModel
                        {
                            AccountId = x.AccountId,
                            AccountName = x.AccountName,
                            AccountUrl = x.AccountUrl,
                            IsConnected = x.IsConnected,
                            Id = x.Id,
                            ProfileId = x.ProfileId
                        }).ToList();
                    }
                }
                else
                {
                    //Remove previous access token
                    var previuostokens = context.AnalyticTokens.Where(x => //x.UserId == userId && x.CompanyId == companyId && x.GroupId == groupId && 
                        x.AccountNumber == existingAccounts.First().AccountNumber);
                    context.AnalyticTokens.RemoveRange(previuostokens);
                    context.UserAnalyticAccounts.AddRange(newAccounts);
                    context.SaveChanges();
                    newAccounts.ForEach(p => context.Entry(p).Reload());//Generating Id's of newly created list.
                    newAccounts.AddRange(existingAccounts);
                    analytics = newAccounts.Select(x => new AnalyticsModel
                  {
                      AccountId = x.AccountId,
                      AccountName = x.AccountName,
                      AccountUrl = x.AccountUrl,
                      IsConnected = x.IsConnected,
                      Id = x.Id,
                      ProfileId = x.ProfileId
                  }).ToList();
                }
            }
            return analytics;
        }
        public bool AssignAnalytic(List<AnalyticCompanyGroupModel> analytics)
        {
            bool isAssigned = false;
            using (var context = new SocialCRMEntities())
            {
                var result = analytics.Select(x => new AnalyticCompanyGroup
                {
                    AnalyticId=x.AnalyticId,
                    CompanyId=x.CompanyId,
                    GroupId=x.GroupId,
                    isGroupConnected=x.isGroupConnected
                }).ToList();
                context.AnalyticCompanyGroups.AddRange(result);
                isAssigned=context.SaveChanges()>0;
            }
            return isAssigned;
        }
        public bool AssignAnalyticUser(List<AnalyticCompanyGroupModel> analytics, List<AnalyticCompanyUserModel> user)
        {
            bool isAssigned = false;
            using (var context = new SocialCRMEntities())
            {
                var users = user.Select(x=>x.TeamMemberId).ToList();
                var result = analytics.Select(x => new AnalyticCompanyGroup
                {
                    AnalyticId = x.AnalyticId,
                    CompanyId = x.CompanyId,
                    GroupId = x.GroupId,
                    isGroupConnected = x.isGroupConnected,
                    Teams = context.Teams.Where(z => users.Contains(z.Id)).ToList()
                }).ToList();
                context.AnalyticCompanyGroups.AddRange(result);

                //var users = user.Select(x => new 
                //{
                //    AnalyticId = x.AnalyticId,
                //    CompanyId = x.CompanyId,
                //    IsConnected=x.IsConnected,
                //    ActivityDate=DateTime.Now,
                //    TeamMemberId=x.TeamMemberId
                //}).ToList();
                var getCompanyAdminUser = context.Teams.Where(x => x.CompanyId == 1 && x.Premissions.Any(y => y.Id == (int)PremissionType.GrantManagerAccess)).ToList();
                isAssigned = context.SaveChanges() > 0;
            }
            return isAssigned;
        }
        public AnalyticsResultModel GetAnalyticResult(int accountId)
        {
            throw new NotImplementedException();
        }
        public UserAnalyticAccount GetUserAnalyticAccountById(int analyticAccountId)
        {
            UserAnalyticAccount account = null;
            using (var context = new SocialCRMEntities())
            {
                account = context.UserAnalyticAccounts.Where(x => x.Id == analyticAccountId).FirstOrDefault();
            }
            return account;
        }
        public List<AnalyticsModel> GetUserAnalyticAccounts(string user, int companyId, List<int> groupId)
        {
            List<AnalyticsModel> accounts=null;
            using (var context = new SocialCRMEntities())
            {
                var userInfo = context.AspNetUsers.Where(x => x.UserName == user).FirstOrDefault();
                var assignedAccounts = context.AnalyticCompanyGroups.Where(x => x.Teams.Any(z=>z.Id==userInfo.Id) && x.CompanyId == companyId && groupId.Contains(x.GroupId)).Select(x => x.AnalyticId).ToList();
                accounts = (context.UserAnalyticAccounts.Where(x => (x.UserId == userInfo.Id && x.CompanyId == companyId && (!x.GroupId.HasValue || groupId.Contains(x.GroupId.Value) || groupId.Count==0) && x.IsConnected == true) || assignedAccounts.Contains(x.Id))
                    .Select(userAccounts => new AnalyticsModel
                    {
                        AccountId = userAccounts.AccountId,
                        AccountName = userAccounts.AccountName,
                        AccountUrl = userAccounts.AccountUrl,
                        IsConnected = userAccounts.IsConnected,
                        ProfileId = userAccounts.ProfileId,
                        Id = userAccounts.Id,
                        OwnerUserId = userAccounts.UserId
                    })).ToList();
            }
            return accounts;
        }

        public List<AnalyticsModel> GetUserAnalyticAccounts(int userId, int companyId, int groupId)
        {
            List<AnalyticsModel> accounts = null;
            int accountNumber = GetUserAnalyticAccountNumber(userId);
            accountNumber--;
            using (var context = new SocialCRMEntities())
            {
                accounts = context.UserAnalyticAccounts
                    .Where(x => x.UserId == userId && x.CompanyId == companyId && x.AccountNumber == accountNumber)// && x.GroupId == groupId)
                    .Select(userAccounts => new AnalyticsModel
                    {
                        AccountId = userAccounts.AccountId,
                        AccountName = userAccounts.AccountName,
                        AccountUrl = userAccounts.AccountUrl,
                        IsConnected = userAccounts.AnalyticCompanyGroups.Any(t => t.CompanyId == companyId && t.GroupId == groupId && t.AnalyticId == userAccounts.Id),
                        ProfileId = userAccounts.ProfileId,
                        Id = userAccounts.Id
                    }).ToList();
            }
            return accounts;
        }
        public List<AnalyticsModel> GetUserAnalyticAccounts(int userId,int groupId)
        {
            List<AnalyticsModel> accounts = null;
            using (var context = new SocialCRMEntities())
            {
                accounts = context.GetAnalyticAccountByGroupAndUserId(groupId,userId)
                    .Select(userAccounts => new AnalyticsModel
                    {
                        AccountId = userAccounts.AccountId,
                        AccountName = userAccounts.Account_Name,
                        AccountUrl = userAccounts.AccountUrl,
                        IsConnected = userAccounts.IsConnected,
                        ProfileId = userAccounts.ProfileId,
                        Id = userAccounts.AnalyticId,
                        OwnerUserId=userAccounts.AnalyticOnnerUserId,
                        AccountNumber=userAccounts.AccountNumber.HasValue?userAccounts.AccountNumber.Value:0,
                        CompanyId=userAccounts.CompanyId.HasValue?userAccounts.CompanyId.Value:0
                    }).ToList();
            }
            return accounts;
        }
        public List<AnalyticsModel> ActivateAnalyticAccount(List<AnalyticCompanyUserModel> analytics)
        {
            List<AnalyticsModel> analyticsAccount = null;
            var groupId = analytics.First().GroupId;
            var companyId = analytics.First().CompanyId;
            var userId = analytics.First().TeamMemberId;
            using (var context = new SocialCRMEntities())
            {
                //var filterIds = analytics.Select(x => x.AnalyticId);
                // var updatedaccounts = context.UserAnalyticAccounts.Where(z => filterIds.Contains(z.Id)).ToList();
                //foreach (var account in updatedaccounts)
                //{
                //    account.IsConnected = true;
                //}
                //var alreadyAddedAccounts = context.AnalyticCompanyGroups.Where(x => x.GroupId == groupId && 
                //    x.CompanyId == companyId && x.Teams.Any(a => a.Id == userId) && filterIds.Contains(x.AnalyticId));
                //context.AnalyticCompanyGroups.RemoveRange(alreadyAddedAccounts);
                var ana = analytics.Where(y => y.IsConnected == true).Select(x => new AnalyticCompanyGroup
                {
                    AnalyticId = x.AnalyticId,
                    CompanyId = x.CompanyId,
                    GroupId = x.GroupId,
                    isGroupConnected=true,
                    Teams = context.Teams.Where(z => z.UserId.HasValue&&z.UserId == userId && z.CompanyId == companyId).ToList()
                }).ToList();

                var analyticCompanygroupIds = (from acg in context.AnalyticCompanyGroups.Where(c => c.CompanyId == companyId && c.GroupId == groupId).AsEnumerable()
                                               from userAnalytic in ana
                                              where acg.AnalyticId== userAnalytic.AnalyticId 
                                              && acg.CompanyId==userAnalytic.CompanyId&& acg.GroupId == userAnalytic.GroupId &&
                                              acg.isGroupConnected == true && acg.Teams.Any(z => z.UserId.HasValue && z.UserId == userId && z.CompanyId == companyId)
                                              select acg.Id).ToList();
                if (analyticCompanygroupIds.Count>0)
                {
                    var analyticCompanyGroup = context.AnalyticCompanyGroups.Where(a => analyticCompanygroupIds.Contains(a.Id));
                    context.AnalyticCompanyGroups.RemoveRange(analyticCompanyGroup);
                }
                context.AnalyticCompanyGroups.AddRange(ana);
                context.SaveChanges();
                 analyticsAccount=context.AnalyticCompanyGroups.Where(x=>x.UserAnalyticAccount.UserId==userId&&x.GroupId==groupId&&x.CompanyId==companyId)
                    .Select(y => new AnalyticsModel
                    {
                        AccountId = y.UserAnalyticAccount.AccountId,
                        Id = y.Id,
                        AccountName = y.UserAnalyticAccount.AccountName,
                        AccountUrl = y.UserAnalyticAccount.AccountUrl,
                        ProfileId = y.UserAnalyticAccount.ProfileId,
                        IsConnected = y.UserAnalyticAccount.IsConnected
                    }).ToList();
            }
            return analyticsAccount;
        }
    }
}