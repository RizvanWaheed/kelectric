using CRM.Common.Enum;
using CRM.Common;
using CRM.Entities;
using CRM.Entities.Repositories;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Models.User;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
//using System.Net;
//using System.Net.Sockets;

namespace SocialCRM.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<bool> RegisterUser(RegisterViewModel user)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                if (!string.IsNullOrEmpty(user.ImageUrl) && user.UserId.HasValue)
                {
                    var currentUser = db.AspNetUsers.Where(x => x.Id == user.UserId.Value).FirstOrDefault();
                    currentUser.ProfileImage = user.ImageUrl;
                }
                var company = new Company { CompanyName = user.CompanyName, SubscriberAccountType = (int)user.AccountType, CreatedDate = DateTime.Now, Status = 1 };
                db.Companies.Add(company);

                List<UserAccessLevel> lstUserAccessLevel = new List<UserAccessLevel>();
                int loop = 1;
                foreach (AccessLevel item in db.AccessLevels.ToList())
                {
                    UserAccessLevel obj = new UserAccessLevel();
                    obj.AccessLevel = item;
                    obj.OrderId = loop;
                    lstUserAccessLevel.Add(obj);
                    loop++;
                }

                var teamMember = new Team
                {
                    IsActive = true,
                    EmailId = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedDate = DateTime.Now,
                    IsVerified = true,
                    UserId = user.UserId,
                    Company = company,
                    Premissions = db.Premissions.ToList(),
                    //AccessLevels = db.AccessLevels.ToList()
                    UserAccessLevels = lstUserAccessLevel
                };
                var userCompany = new UserCompany { Company = company, UserId = user.UserId.Value, RegistrationDate = DateTime.Now, CompanyTypeId = (int)CompanyTypes.Owned };
                db.UserCompanies.Add(userCompany);
                db.Teams.Add(teamMember);
                var teamCollection = new List<Team>();
                teamCollection.Add(teamMember);
                var group = new Group
                {
                    Name = "First Group",
                    Company = company,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    Teams = teamCollection
                };
                db.Groups.Add(group);
                var notificationSetting = new UserNotificationSetting
                {
                    Company = company,
                    Group = group,
                    UserId = user.UserId.HasValue ? user.UserId.Value : 0,
                    IsEmailEnable = true,
                    IsNotificationEnable = true
                };
                db.UserNotificationSettings.Add(notificationSetting);
                var resultset = await db.SaveChangesAsync();
                result = resultset > 0;
                db.Dispose();
            }
            return result;
        }

        public async Task<List<UserLogsModel>> GetUserLogs()
        {
            List<UserLogsModel> result = null;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    result =
                         (from l in db.Logs
                          join lc in db.LogCodes on l.LogCodeId equals lc.Id
                          join usr in db.AspNetUsers on l.UserId equals usr.Id
                          select new UserLogsModel()
                          {
                              UserName = usr.Name,
                              Email = usr.Email,
                              IpAddress = l.IpAddress,
                              LogAction = lc.Description,
                              LogActionDate = l.CreatedDate.ToString()
                          }).Take(40).ToList();
                }
                catch (Exception exp)
                {
                    throw exp; 
                }
            }
            return result;
        }

        public async Task<RegisterViewModel> GetUserByConfirmationCodeAsync(string code)
        {
            RegisterViewModel userModel = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var user = db.AspNetUsers.Where(x => string.Compare(x.ConfirmationCode, code) == 0).FirstOrDefault();
                    if (user != null)
                        userModel = new RegisterViewModel
                        {
                            FirstName = user.Name.Split(' ')[0],
                            Email = user.Email,
                            ImageUrl = user.ProfileImage,
                            LastName = user.Name.Split(' ')[1]
                        };
                    db.Dispose();
                }
            });
            return userModel;
        }
        public async Task<RegisterViewModel> GetUserByEmailId(string emailId, int companyId)
        {
            RegisterViewModel userModel = null;
            try
            {

                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var user = db.AspNetUsers.Where(x => string.Compare(x.Email, emailId) == 0).FirstOrDefault();
                        if (user != null)
                        {
                            userModel = new RegisterViewModel
                            {
                                FirstName = user.Name.Split(' ')[0],
                                Email = user.Email,
                                ImageUrl = user.ProfileImage,
                                LastName = user.Name.Split(' ')[1],
                                UserId = user.Id
                            };
                            var userAccountType = (from uc in db.UserCompanies
                                                   join c in db.Companies
                                                   on uc.CompanyId equals c.id
                                                   where uc.UserId == user.Id && uc.CompanyTypeId == (int)CompanyTypes.Owned
                                                   select c).FirstOrDefault();
                            if (userAccountType == null)
                            {
                                userAccountType = (from uc in db.UserCompanies
                                                   join c in db.Companies
                                                   on uc.CompanyId equals c.id
                                                   where uc.UserId == user.Id && uc.CompanyTypeId == (int)CompanyTypes.Assigned
                                                   select c).FirstOrDefault();
                            }
                            if (userAccountType != null)
                            {
                                userModel.AccountType = (AccountSubscriberType)userAccountType.SubscriberAccountType;
                                userModel.CompanyName = userAccountType.CompanyName;
                                userModel.PackageName = userAccountType.SubscriberType.Name;

                                var defaultGroup = userAccountType.Groups.FirstOrDefault();
                                userModel.GroupId = defaultGroup != null ? defaultGroup.Id : 0;
                                Team teamMember;
                                if (companyId == 0)
                                    teamMember = userAccountType.Teams.Where(u => u.UserId == user.Id).FirstOrDefault();
                                else
                                    teamMember = db.Teams.Where(u => u.UserId == user.Id && u.CompanyId == companyId).FirstOrDefault();
                                if (teamMember != null)
                                {
                                    userModel.AccessRights = teamMember.Premissions.Select(x => x.Id).ToList();
                                    //userModel.AccessReports = teamMember.AccessLevels.Select(x => x.Id).ToList();
                                    userModel.AccessReports = teamMember.UserAccessLevels.Select(x => x.AccessLevel.Id).ToList();
                                    userModel.TeamMemberId = teamMember.Id;
                                }
                                userModel.CompanyId = userAccountType.id;
                            }
                            var userRoles = user.AspNetRoles.FirstOrDefault();
                            if (userRoles != null)
                                userModel.IsAdmin = userRoles.Name == "Admin";
                            else
                                userModel.IsAdmin = false;

                            /*//user Default Company
                            var defaultCompany = user.UserCompanies.Where(c => c.CompanyTypeId == 1).FirstOrDefault();
                            if (defaultCompany != null)
                            {
                                userModel.CompanyId = defaultCompany.CompanyId;
                                if (defaultCompany.Company.Groups != null)
                                {
                                    var defaultGroup = defaultCompany.Company.Groups.FirstOrDefault();
                                    userModel.GroupId = defaultGroup != null ? defaultGroup.Id : 0;
                                }
                            }*/
                        }
                        db.Dispose();
                    }
                });
                return userModel;
            }
            catch (Exception ex)
            {
                return userModel;
            }
        }
        public async Task<List<NotificationViewModel>> GetBusinessProfile(int userId)
        {
            List<NotificationViewModel> notificationModel = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var NotificationSettingInfo = db.UserNotificationSettings.Where(x => x.UserId == userId).ToList();
                    if (NotificationSettingInfo != null && NotificationSettingInfo.Count > 0)
                    {
                        notificationModel = NotificationSettingInfo.GroupBy(y => y.CompanyId).Select(x => new NotificationViewModel
                        {
                            CompanyId = x.First().CompanyId,
                            CompanyName = x.First().Company.CompanyName,
                            Notificaitons = x.Select(z => new Notification
                            {
                                GroupId = z.GroupId,
                                GroupName = z.Group.Name,
                                IsEmailEnabled = z.IsEmailEnable,
                                IsPushNotificationEnabled = z.IsNotificationEnable
                            }).ToList()
                        }).ToList();
                        /*notificationModel = new NotificationViewModel
                        {
                            CompanyId = NotificationSettingInfo.First().CompanyId,
                            CompanyName = NotificationSettingInfo.First().Company.CompanyName,
                            Notificaitons = NotificationSettingInfo.Select(x => new Notification
                            {
                                GroupId = x.GroupId,
                                GroupName = x.Group.Name,
                                IsEmailEnabled = x.IsEmailEnable,
                                IsPushNotificationEnabled = x.IsNotificationEnable
                            }).ToList()
                        };*/
                    }
                    db.Dispose();
                }
            });
            return notificationModel;
        }
        public async Task<bool> IsTrialExpired(int userId)
        {
            bool trialExpired = false;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    AspNetUser dbUser = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
                    UserCompany comp = dbUser.UserCompanies.FirstOrDefault();
                    if (comp != null && dbUser != null)
                    {
                        if (comp.Company.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                        {
                            if ((DateTime.Now - comp.Company.CreatedDate).TotalDays > 30 || comp.Company.Status == 0)
                            {
                                trialExpired = true;
                            }
                        }
                    }
                }
            });
            return trialExpired;
        }
        public async Task<bool> UpdateNotificationSettings(NotificationViewModel model, int userId)
        {
            var result = false;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var NotificationSettingInfo = db.UserNotificationSettings.Where(x => x.UserId == userId).ToList();
                    if (NotificationSettingInfo != null && NotificationSettingInfo.Count > 0)
                    {
                        foreach (var notification in NotificationSettingInfo)
                        {
                            notification.IsEmailEnable = model.Notificaitons.Where(x => x.GroupId == notification.GroupId).First().IsEmailEnabled;
                            notification.IsNotificationEnable = model.Notificaitons.Where(x => x.GroupId == notification.GroupId).First().IsPushNotificationEnabled;
                            var NotificationsGroupUser = db.UserNotifications.Where(x => x.RecipientUserId == userId && x.GroupId == notification.GroupId).ToList();
                            foreach (var existingNotification in NotificationsGroupUser)
                            {
                                existingNotification.IsNotificationEnabled = notification.IsNotificationEnable;
                            }
                        }
                    }
                    var resultCount = db.SaveChanges();
                    result = resultCount > 0;
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> UpdateBusinessProfileName(NotificationViewModel model)
        {
            var result = false;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var company = db.Companies.Where(c => c.id == model.CompanyId).FirstOrDefault();
                    if (company != null)
                    {
                        company.CompanyName = model.CompanyName;
                    }
                    var resultCount = db.SaveChanges();
                    result = resultCount > 0;
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<PersonalProfileModel> GetPersonalProfile(int userId, int companyId)
        {
            PersonalProfileModel personalViewModel = null;
            await Task.Run(() =>
             {
                 using (var db = new SocialCRMEntities())
                 {
                     personalViewModel = new PersonalProfileModel();
                     var user = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
                     if (user != null)
                     {
                        //var company=user.UserCompanies.Where(t=>t.CompanyTypeId==(int)CompanyTypes.Owned).FirstOrDefault();
                        //if(company!=null)
                        //{
                        var personalProfile = db.Profiles.Where(x => x.IsPersonal.HasValue && x.IsPersonal.Value && x.MemberUserId == userId).Select(x => new PersonalProfile
                         {
                             ProfileId = x.Id,
                             ProfileName = x.Name,
                             ProfileType = (ProfilesType)x.ProfileTypeId,
                             ProfileUrl = x.ProfilePic,
                             IsEmailEnabled = x.IsEmailEnabled
                         }).ToList();
                         personalViewModel.ConnectedProfiles = new List<PersonalProfile>();
                         if (personalProfile.Count > 0)
                         {
                             personalViewModel.ConnectedProfiles.AddRange(personalProfile);
                         }
                         personalViewModel.AssignedRights = new List<Models.UserRightModel>();
                         personalViewModel.AssignedReports = new List<Models.UserReportModel>();
                         var assignedRights = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).FirstOrDefault();
                         if (assignedRights != null)
                         {
                             personalViewModel.AssignedRights.AddRange(assignedRights.Premissions.Select(t => new Models.UserRightModel
                             {
                                 Id = t.Id,
                                 Name = t.Name
                             }).ToList());
                            //personalViewModel.AssignedReports.AddRange(assignedRights.AccessLevels.Select(t => new Models.UserReportModel
                            //{
                            //    Id = t.Id,
                            //    Name = t.Name
                            //}).ToList());
                            personalViewModel.AssignedReports.AddRange(assignedRights.UserAccessLevels.Select(t => new Models.UserReportModel
                             {
                                 Id = t.AccessLevel.Id,
                                 Name = t.AccessLevel.Name
                             }).ToList());
                         }
                        //}
                        personalViewModel.Name = user.Name;
                         personalViewModel.Email = user.Email;
                         personalViewModel.ProfileUrl = user.ProfileImage;
                     }
                     db.Dispose();
                 }
             });
            return personalViewModel;
        }
        public async Task<bool> UpdatePersonalProfile(PersonalProfileModel model)
        {
            var success = false;
            await Task.Run(() =>
             {
                 using (var db = new SocialCRMEntities())
                 {
                     var userProfile = db.AspNetUsers.Where(z => z.Email == model.Email).FirstOrDefault();
                     userProfile.Name = model.Name;
                     userProfile.ProfileImage = model.ProfileUrl;
                     if (model.ConnectedProfiles.Count > 0)
                     {
                         var profileIds = model.ConnectedProfiles.Select(x => x.ProfileId);
                         var profiles = db.Profiles.Where(y => profileIds.Contains(y.Id)).ToList();
                         profiles.ForEach(p =>
                             {
                                 p.IsEmailEnabled = model.ConnectedProfiles.Where(z => z.ProfileId == p.Id).First().IsEmailEnabled;
                             });
                     }
                     db.SaveChanges();
                     db.Dispose();
                 }
             });
            return success;
        }

        public async Task<bool> UpdateLog(int userId, int logCode)
        {
            var result = false;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var log = new Log
                    {
                        LogCodeId = logCode,
                        UserId = userId,
                        IpAddress = CRM.Common.GeneralMethods.GetLocalIPAddress(),
                        CreatedDate = DateTime.Now
                    };
                    db.Logs.Add(log);

                    db.SaveChanges();
                    db.Dispose();

                    result = true;
                }
            });
            return result;
        }

        
    }
}