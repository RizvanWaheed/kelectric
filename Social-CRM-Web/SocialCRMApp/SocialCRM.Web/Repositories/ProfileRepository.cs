using CRM.Common.Enum;
using CRM.Common.Extensions;
using CRM.Entities;
using SocialCRM.Web.Models;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Models.Team;
using SocialCRM.Web.Models.UnifiedContact;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        public async Task<bool> AssignProfileToGroup(ProfileComapnyGroup profile, int userId)
        {

            bool result = false;
            using (var db = new SocialCRMEntities())
            {

                var isExist = db.ProfileComapnyGroups.Where(x => x.CompanyId == profile.CompanyId && x.GroupId == profile.GroupId && x.ProfileId == profile.ProfileId && x.ProfilePageId == profile.ProfilePageId).FirstOrDefault();
                if (isExist == null)
                {
                    db.ProfileComapnyGroups.Add(profile);
                }
                db.AddUserNotification(profile.GroupId, userId, "New Profile : " + profile.Profile.Name + " Added In Group : " + db.Groups.Where(x => x.Id == profile.GroupId).FirstOrDefault().Name, "Profile Added");
                var resultset = await db.SaveChangesAsync();
                result = resultset > 0;
                db.Dispose();
            }
            return result;
        }

        public async Task<string> RemoveProfileFromGroup(ProfileCompanyGroup profile, int userId)
        {
            bool result = false;
            bool groupExists = true;
            try
            {

                using (var db = new SocialCRMEntities())
                {
                    if (profile.ProfileType == ProfilesType.Twitter)
                    {
                        var twitterProfile = db.Profiles.Where(p => p.Id == profile.ProfileId).FirstOrDefault();
                        if (twitterProfile.IsBaseAccount.HasValue && twitterProfile.IsBaseAccount.Value)
                        {
                            var totalGroups = db.ProfileComapnyGroups.Where(x => x.ProfileId == profile.ProfileId && x.CompanyId == profile.CompanyId).Count();
                            //Error Base Profile Unable to Delete
                            if (totalGroups == 1)
                                return "EBUN";
                        }
                    }
                    db.RemoveProfileFromGroup(profile.ProfileId, profile.GroupId, profile.CompanyId, (int)profile.ProfileType);

                    groupExists = db.Groups.Where(x => x.Id == profile.GroupId).Count() == 1 ? true : false;
                    result = true;
                }

                using (var db = new SocialCRMEntities())
                {
                    db.AddUserNotification(profile.GroupId, userId, "Profile : " + profile.ProfileType.ToString() != "5" ? db.Profiles.Where(x => x.Id == profile.ProfileId).FirstOrDefault().Name : db.ProfilePages.Where(x => x.Id == profile.ProfileId).FirstOrDefault().PageName + " Removed From Group : " + db.Groups.Where(x => x.Id == profile.GroupId).FirstOrDefault().Name, "Profile Removed");
                }
            }
            catch (Exception ex)
            {

            }
            //Notify About Group Removal
            if (!groupExists)
                return "NGR";
            return result.ToString();
        }
        public async Task<bool> DeleteProfile(int profileId, int userId)
        {

            bool result = false;
            using (var db = new SocialCRMEntities())
            {

                var profile = db.Profiles.Where(x => x.Id == profileId).FirstOrDefault();
                if (profile != null)
                {
                    var profileCompanyGrp = db.ProfileComapnyGroups.Where(x => x.ProfileId == profileId);
                    db.ProfileComapnyGroups.RemoveRange(profileCompanyGrp);
                    db.Profiles.Remove(profile);
                    db.AddUserNotification(profileCompanyGrp.FirstOrDefault().GroupId, userId, "Delete Profile : " + profile.Name, "Profile Deleted");
                    var resultset = await db.SaveChangesAsync();
                    result = resultset > 0;
                }
                db.Dispose();
            }
            return result;
        }

        public async Task<bool> UpdatePerssonalProfile(int profileId, int userId)
        {

            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var profile = db.Profiles.Where(x => x.Id == profileId).FirstOrDefault();
                if (profile != null)
                {
                    profile.IsPersonal = false;
                }
                //db.AddUserNotification(profile.ProfileComapnyGroups.FirstOrDefault().GroupId, userId, "Remove Personal Profile : " + profile.Name, "Remove Personal Profile");
                await db.SaveChangesAsync();
                db.Dispose();

            }
            return result;
        }

        public async Task<List<ContactFollowStatusModel>> GetSocialProfilesByGroupId(int userId, int groupId, int companyId, int profileTypeId)
        {
            List<ContactFollowStatusModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    //Profiles  
                    result = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.CompanyId == companyId && x.ProfileId.HasValue && x.Teams.Any(u => u.UserId == userId) && x.Profile.ProfileTypeId == profileTypeId)
                                .Select(z => new ContactFollowStatusModel
                                {
                                    Id = z.Profile.Id,
                                    UserId = z.Profile.ProfileExternalId,
                                    UserName = z.Profile.UserName,
                                    Name = z.Profile.Name,
                                    ProfileExternalId = z.Profile.ProfileExternalId,
                                    ProfilePicURL = z.Profile.ProfilePic,
                                    IsFollowingContact = false,
                                    IsPage = false
                                }).ToList();

                    if (profileTypeId == 1)
                    {
                        var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.CompanyId == companyId && x.ProfilePageId.HasValue && x.Teams.Any(u => u.UserId == userId))
                                        .Select(z => new ContactFollowStatusModel
                                        {
                                            Id = z.ProfilePage.Id,
                                            UserId = z.ProfilePage.ProfilePageID,
                                            UserName = z.ProfilePage.PageName,
                                            Name = z.ProfilePage.PageName,
                                            ProfileExternalId = z.ProfilePage.ProfilePageID,
                                            ProfilePicURL = z.ProfilePage.ProfilePic,
                                            IsFollowingContact = false,
                                            IsPage = true
                                        }).ToList();

                        result.AddRange(profilePages);
                    }

                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<ContactFollowStatusModel>> GetSocialProfilesPersonalMode(int userId, int profileTypeId)
        {
            List<ContactFollowStatusModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    //Profiles  
                    result = db.Profiles.Where(x => x.MemberUserId == userId && x.IsPersonal == true && x.ProfileTypeId == profileTypeId)
                                .Select(z => new ContactFollowStatusModel
                                {
                                    Id = z.Id,
                                    UserId = z.ProfileExternalId,
                                    UserName = z.UserName,
                                    Name = z.Name,
                                    ProfileExternalId = z.ProfileExternalId,
                                    ProfilePicURL = z.ProfilePic,
                                    IsFollowingContact = false
                                }).ToList();
                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<ProfileModel>> GetProfilesByGroupIdForUnifiedInbox(int userId, int groupId, int companyId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    //var profileIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfileId).ToList();
                    //  var profilePageIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfilePageId).ToList();

                    //Profiles  
                    result = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.CompanyId == companyId && x.Profile.ProfileType.Id != 8 && x.ProfileId.HasValue && x.Teams.Any(u => u.UserId == userId)).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = z.Id,
                        ProfileId = z.Profile.Id,
                        ProfileName = z.Profile.Name,
                        ProfileType = (ProfilesType)z.Profile.ProfileTypeId,
                        ProfileTypeId = z.Profile.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        IsConnected = z.Teams.Any(t => t.UserId == userId),
                        ProfilePicURL = z.Profile.ProfilePic,
                        AccountName = z.Profile.UserName,
                        IsPage = false,
                        UserId = z.Profile.ProfileExternalId
                    }).ToList();
                    //Profile Pages
                    var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.CompanyId == companyId && x.ProfilePageId.HasValue && x.Teams.Any(u => u.UserId == userId)).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = z.Id,
                        ProfileId = z.ProfilePage.Id,
                        ProfileName = z.ProfilePage.PageName,
                        ProfileType = (ProfilesType)z.ProfilePage.Profile.ProfileTypeId,
                        ProfileTypeId = z.ProfilePage.Profile.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        IsConnected = z.Teams.Any(t => t.UserId == userId),
                        ProfilePicURL = z.ProfilePage.ProfilePic,
                        AccountName = z.ProfilePage.PageName,
                        IsPage = true,
                        UserId = z.ProfilePage.ProfilePageID
                    }).ToList();

                    result.AddRange(profilePages);
                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<OutlookProfileModel>> GetOutlookProfilesByGroupId(int userId, int groupId, int companyId, bool isPersonal)
        {
            List<OutlookProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    if (isPersonal)
                        result = db.Profiles.Where(x => x.MemberUserId == userId && x.IsPersonal == true && x.ProfileTypeId == 8).Select(z => new OutlookProfileModel
                        // .Select(z => new ContactFollowStatusModel
                        {
                            ProfileId = z.Id,
                            ProfileName = z.Name,
                            ProfileType = (ProfilesType)z.ProfileTypeId,
                            ProfileTypeId = z.ProfileTypeId,
                            CompanyId = z.CompanyId,
                            IsConnected = true,  //z.Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.ProfilePic,
                            AccountName = z.UserName,
                            UserId = z.UserName,
                            Email = z.ProfileExternalId,
                            Domain = z.AcessTokenSecret,
                        }).ToList();
                    else
                        result = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.CompanyId == companyId && x.Profile.ProfileType.Id == 8 && x.ProfileId.HasValue && x.Teams.Any(u => u.UserId == userId)).Select(z => new OutlookProfileModel
                        {
                            ProfileCompnayGroupId = z.Id,
                            ProfileId = z.Profile.Id,
                            ProfileName = z.Profile.Name,
                            ProfileType = (ProfilesType)z.Profile.ProfileTypeId,
                            ProfileTypeId = z.Profile.ProfileTypeId,
                            CompanyId = z.CompanyId,
                            IsConnected = z.Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.Profile.ProfilePic,
                            AccountName = z.Profile.UserName,
                            UserId = z.Profile.UserName,
                            Email = z.Profile.ProfileExternalId,
                            Domain = z.Profile.AcessTokenSecret,
                        }).ToList();

                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<ProfileModel>> GetProfilesForUnifiedInboxPersonalMode(int userId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    //var profileIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfileId).ToList();
                    //  var profilePageIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfilePageId).ToList();

                    //Profiles  
                    result = db.Profiles.Where(x => x.MemberUserId == userId && x.IsPersonal == true).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = 0,
                        ProfileId = z.Id,
                        ProfileName = z.Name,
                        ProfileType = (ProfilesType)z.ProfileTypeId,
                        ProfileTypeId = z.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        IsConnected = true,
                        ProfilePicURL = z.ProfilePic,
                        IsPage = false,
                        UserId = z.ProfileExternalId,
                        AccountName = z.UserName
                    }).ToList();

                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<ProfileModel>> GetProfilesByGroupId(int userId, int groupId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
             {
                 using (var db = new SocialCRMEntities())
                 {
                     //var profileIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfileId).ToList();
                     //  var profilePageIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfilePageId).ToList();

                     //Profiles  
                     result = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfileId.HasValue).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                     {
                         ProfileCompnayGroupId = z.FirstOrDefault().Id,
                         ProfileId = z.FirstOrDefault().Profile.Id,
                         ProfileName = z.FirstOrDefault().Profile.Name,
                         ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                         ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                         CompanyId = z.FirstOrDefault().CompanyId,
                         IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                         ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                         IsPage = false,
                         AccountName = z.FirstOrDefault().Profile.UserName
                     }).ToList();
                     //Profile Pages
                     var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId.HasValue).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                     {
                         ProfileCompnayGroupId = z.FirstOrDefault().Id,
                         ProfileId = z.FirstOrDefault().ProfilePage.Id,
                         ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                         ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                         ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                         CompanyId = z.FirstOrDefault().CompanyId,
                         IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                         ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                         IsPage = true,
                         AccountName = z.FirstOrDefault().ProfilePage.PageName
                     }).ToList();

                     result.AddRange(profilePages);
                     /////
                     /* result = db.Profiles.Where(x => profileIds.Contains(x.Id)).Select(z => new ProfileModel
                     {
                         ProfileId = z.Id,
                         ProfileName = z.Name,
                         ProfileType = (ProfilesType)z.ProfileTypeId,
                         ProfileTypeId = z.ProfileTypeId,
                         CompanyId = z.CompanyId,
                         IsConnected = z.Teams.Any(t => t.UserId == userId),
                         ProfilePicURL = z.ProfilePic
                      }).ToList();
                      //Profile Pages
                      var profilePages=(from profilePage in db.ProfilePages.Where(x => profilePageIds.Contains(x.Id))
                            join profile in db.Profiles.Where(x => profileIds.Contains(x.Id))
                            on profilePage.ProfileTransactionId equals profile.Id
                            select new ProfileModel
                            {
                                CompanyId=profile.CompanyId,
                                IsConnected=profilePage.IsConnected.Value,
                                PageName=profilePage.PageName,
                                PageUrl=profilePage.PageUrl,
                                ProfileId=profile.Id,
                                ProfileName=profile.Name,
                                ProfileType = ProfilesType.Page,
                                ProfileTypeId=5,
                                ProfilePicURL = profile.ProfilePic

                            }).ToList();
                      result.AddRange(profilePages);     */
                     //adding Google Analytics Profiles
                     var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.GroupId == groupId).GroupBy(g => g.AnalyticId).Select(y => new ProfileModel
                     {
                         ProfileCompnayGroupId = y.FirstOrDefault().Id,
                         CompanyId = y.FirstOrDefault().CompanyId,
                         ProfileId = y.FirstOrDefault().AnalyticId,
                         ProfileName = y.FirstOrDefault().UserAnalyticAccount.AccountName + " " + y.FirstOrDefault().UserAnalyticAccount.AccountUrl,
                         ProfileType = ProfilesType.GoogleAnalytics,
                         ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                         IsConnected = y.FirstOrDefault().Teams.Any(t => t.Id == userId),
                         AccountName = y.FirstOrDefault().UserAnalyticAccount.AccountName
                     }).ToList();
                     result.AddRange(analyticsAccount);
                     /*//adding instagram Profiles
                     var instagramAccount = db.InstagramCompanyGroups.Where(x => x.GroupId == groupId)
                     .GroupBy(g => g.InstagramId).Select(y => new ProfileModel
                     {
                         ProfileCompnayGroupId = y.FirstOrDefault().Id,
                         CompanyId = y.FirstOrDefault().CompanyId,
                         ProfileId = y.FirstOrDefault().InstagramId,
                         ProfileName = y.FirstOrDefault().InstagramAccount.InstaFullName+ " " + y.FirstOrDefault().InstagramAccount.InstaWebSite,
                         ProfileType = ProfilesType.Instagram,
                         ProfileTypeId = (int)ProfilesType.Instagram,
                         IsConnected = y.FirstOrDefault().Teams.Any(t => t.Id == userId),
                         ProfilePicURL=y.FirstOrDefault().InstagramAccount.InstaProfilePic
                     }).ToList();
                     result.AddRange(instagramAccount);*/
                     db.Dispose();
                 }
             });
            return result;
        }
        public async Task<List<ProfileModel>> GetProfilesByGroupId(int groupId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    /*var profileIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfileId).ToList();
                    var profilePageIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(z => z.ProfilePageId).ToList();
                    result = db.Profiles.Where(x => profileIds.Contains(x.Id)).Select(z => new ProfileModel
                    {
                        ProfileId = z.Id,
                        ProfileName = z.Name,
                        ProfileType = (ProfilesType)z.ProfileTypeId,
                        ProfileTypeId = z.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        IsConnected =false,
                        ProfilePicURL = z.ProfilePic
                    }).ToList();
                    //Profile Pages
                    var profilePages = (from profilePage in db.ProfilePages.Where(x => profilePageIds.Contains(x.Id))
                                        join profile in db.Profiles.Where(x => profileIds.Contains(x.Id))
                                        on profilePage.ProfileTransactionId equals profile.Id
                                        select new ProfileModel
                                        {
                                            CompanyId = profile.CompanyId,
                                            IsConnected = profilePage.IsConnected.Value,
                                            PageName = profilePage.PageName,
                                            PageUrl = profilePage.PageUrl,
                                            ProfileId = profile.Id,
                                            ProfileName = profile.Name,
                                            ProfileType = ProfilesType.Page,
                                            ProfileTypeId = 5,
                                            ProfilePicURL = profile.ProfilePic

                                        }).ToList();
                    result.AddRange(profilePages);*/
                    //Profiles  
                    result = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfileId.HasValue).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = z.FirstOrDefault().Id,
                        ProfileId = z.FirstOrDefault().Profile.Id,
                        ProfileName = z.FirstOrDefault().Profile.Name,
                        ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                        ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                        CompanyId = z.FirstOrDefault().CompanyId,
                        IsConnected = false,
                        ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                        IsPage = false,
                        AccountName = z.FirstOrDefault().Profile.UserName
                    }).ToList();
                    //Profile Pages
                    var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId.HasValue).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = z.FirstOrDefault().Id,
                        ProfileId = z.FirstOrDefault().ProfilePage.Id,
                        ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                        ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                        ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                        CompanyId = z.FirstOrDefault().CompanyId,
                        IsConnected = false,
                        ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                        IsPage = true,
                        AccountName = z.FirstOrDefault().ProfilePage.PageName
                    }).ToList();

                    result.AddRange(profilePages);
                    //adding Google Analytics Profiles
                    var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.GroupId == groupId).GroupBy(g => g.AnalyticId).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.FirstOrDefault().Id,
                        CompanyId = y.FirstOrDefault().CompanyId,
                        ProfileId = y.FirstOrDefault().AnalyticId,
                        ProfileName = y.FirstOrDefault().UserAnalyticAccount.AccountName + " " + y.FirstOrDefault().UserAnalyticAccount.AccountUrl,
                        ProfileType = ProfilesType.GoogleAnalytics,
                        ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                        IsConnected = false,
                        AccountName = y.FirstOrDefault().UserAnalyticAccount.AccountName
                    }).ToList();
                    result.AddRange(analyticsAccount);
                    /*//adding instagram  Profiles
                    var instagramAccount = db.InstagramCompanyGroups.Where(x => x.GroupId == groupId)
                    .GroupBy(g => g.InstagramId).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.FirstOrDefault().Id,
                        CompanyId = y.FirstOrDefault().CompanyId,
                        ProfileId = y.FirstOrDefault().InstagramId,
                        ProfileName = y.FirstOrDefault().InstagramAccount.InstaFullName+ " " + y.FirstOrDefault().InstagramAccount.InstaWebSite,
                        ProfileType = ProfilesType.Instagram,
                        ProfileTypeId = (int)ProfilesType.Instagram,
                        IsConnected = false,
                        ProfilePicURL = y.FirstOrDefault().InstagramAccount.InstaProfilePic
                    }).ToList();
                    result.AddRange(instagramAccount);*/
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<ProfileModel>> GetProfilesByCompanyId(int companyId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    /*result = db.Profiles.Where(x => x.CompanyId == companyId).Select(z => new ProfileModel { ProfileId = z.Id, ProfileName = z.Name, ProfileType = (ProfilesType)z.ProfileTypeId, ProfileTypeId = z.ProfileTypeId, CompanyId = z.CompanyId, ProfilePicURL = z.ProfilePic }).ToList();
                    //Profile Pages
                    var profileIds = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId).Select(z => z.ProfileId).ToList();
                    var profilePageIds = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId).Select(z => z.ProfilePageId).ToList();
                    var profilePages = (from profilePage in db.ProfilePages.Where(x => profilePageIds.Contains(x.Id))
                                        join profile in db.Profiles.Where(x => profileIds.Contains(x.Id))
                                        on profilePage.ProfileTransactionId equals profile.Id
                                        select new ProfileModel
                                        {
                                            CompanyId = profile.CompanyId,
                                            IsConnected = profilePage.IsConnected.Value,
                                            PageName = profilePage.PageName,
                                            PageUrl = profilePage.PageUrl,
                                            ProfileId = profile.Id,
                                            ProfileName = profile.Name,
                                            ProfileType = ProfilesType.Page,
                                            ProfileTypeId = 5,
                                            ProfilePicURL = profile.ProfilePic

                                        }).ToList();
                    result.AddRange(profilePages);       */
                    //Profiles  
                    result = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfileId.HasValue).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = z.FirstOrDefault().Id,
                        ProfileId = z.FirstOrDefault().Profile.Id,
                        ProfileName = z.FirstOrDefault().Profile.Name,
                        ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                        ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                        CompanyId = z.FirstOrDefault().CompanyId,
                        IsConnected = false,
                        ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                        AccountName = z.FirstOrDefault().Profile.UserName
                    }).ToList();
                    //Profile Pages
                    var profilePages = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfilePageId.HasValue).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = z.FirstOrDefault().Id,
                        ProfileId = z.FirstOrDefault().ProfilePage.Id,
                        ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                        ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                        ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                        CompanyId = z.FirstOrDefault().CompanyId,
                        IsConnected = false,
                        IsPage = true,
                        ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                        AccountName = z.FirstOrDefault().ProfilePage.PageName
                    }).ToList();
                    result.AddRange(profilePages);
                    //adding Google Analytics Profiles
                    var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId).GroupBy(g => g.AnalyticId).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.FirstOrDefault().Id,
                        CompanyId = y.FirstOrDefault().CompanyId,
                        ProfileId = y.FirstOrDefault().AnalyticId,
                        ProfileName = y.FirstOrDefault().UserAnalyticAccount.AccountName + " " + y.FirstOrDefault().UserAnalyticAccount.AccountUrl,
                        ProfileType = ProfilesType.GoogleAnalytics,
                        ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                        IsConnected = false,
                        AccountName = y.FirstOrDefault().UserAnalyticAccount.AccountName
                    }).ToList();
                    result.AddRange(analyticsAccount);
                    /*//adding Instagram Profiles
                    var instagramAccount = db.InstagramCompanyGroups.Where(x => x.CompanyId == companyId)
                    .GroupBy(g => g.InstagramId).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.FirstOrDefault().Id,
                        CompanyId = y.FirstOrDefault().CompanyId,
                        ProfileId = y.FirstOrDefault().InstagramId,
                        ProfileName = y.FirstOrDefault().InstagramAccount.InstaFullName+ " " + y.FirstOrDefault().InstagramAccount.InstaWebSite,
                        ProfileType = ProfilesType.Instagram,
                        ProfileTypeId = (int)ProfilesType.Instagram,
                        IsConnected = false,
                        ProfilePicURL = y.FirstOrDefault().InstagramAccount.InstaProfilePic
                    }).ToList();
                    result.AddRange(instagramAccount);*/
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<ProfileModel>> GetAllProfilesWithoutGroupByCompanyId(int companyId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    //Profiles  
                    result = db.Profiles.Where(x => x.CompanyId == companyId).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = 0,
                        ProfileId = z.Id,
                        ProfileName = z.Name,
                        ProfileType = (ProfilesType)z.ProfileTypeId,
                        ProfileTypeId = z.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        IsConnected = false,
                        ProfilePicURL = z.ProfilePic,
                        AccountName = z.UserName
                    }).ToList();
                    //Profile Pages
                    var profilePages = db.ProfilePages.Where(x=>x.Profile.CompanyId == companyId).Select(z => new ProfileModel
                    {
                        ProfileCompnayGroupId = 0,
                        ProfileId = z.Id,
                        ProfileName = z.PageName,
                        ProfileType = ProfilesType.Page,
                        ProfileTypeId = (int)ProfilesType.Page,
                        CompanyId = z.Profile.CompanyId,
                        IsConnected = false,
                        ProfilePicURL = z.ProfilePic,
                        AccountName = z.PageName
                    }).ToList();
                    result.AddRange(profilePages);
                    //adding Google Analytics Profiles
                    var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId).GroupBy(g => g.AnalyticId).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.FirstOrDefault().Id,
                        CompanyId = y.FirstOrDefault().CompanyId,
                        ProfileId = y.FirstOrDefault().AnalyticId,
                        ProfileName = y.FirstOrDefault().UserAnalyticAccount.AccountName + " " + y.FirstOrDefault().UserAnalyticAccount.AccountUrl,
                        ProfileType = ProfilesType.GoogleAnalytics,
                        ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                        IsConnected = false,
                        AccountName = y.FirstOrDefault().UserAnalyticAccount.AccountName
                    }).ToList();
                    result.AddRange(analyticsAccount);

                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<ProfileModel>> GetProfilesByCompanyId(int userId, int companyId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    /*result = db.Profiles.Where(x => x.CompanyId == companyId).Select(z => new ProfileModel
                    {
                        ProfileId = z.Id,
                        ProfileName = z.Name,
                        ProfileType = (ProfilesType)z.ProfileTypeId,
                        ProfileTypeId = z.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        IsConnected = z.Teams.Any(t => t.UserId == userId),
                        ProfilePicURL = z.ProfilePic
                    }).ToList();
                    //Profile Pages
                    var profileIds = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId).Select(z => z.ProfileId).ToList();
                    var profilePageIds = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId).Select(z => z.ProfilePageId).ToList();
                    var profilePages = (from profilePage in db.ProfilePages.Where(x => profilePageIds.Contains(x.Id))
                                        join profile in db.Profiles.Where(x => profileIds.Contains(x.Id))
                                        on profilePage.ProfileTransactionId equals profile.Id
                                        select new ProfileModel
                                        {
                                            CompanyId = profile.CompanyId,
                                            IsConnected = profile.Teams.Any(t => t.UserId == userId),
                                            PageName = profilePage.PageName,
                                            PageUrl = profilePage.PageUrl,
                                            ProfileId = profile.Id,
                                            ProfileName = profile.Name,
                                            ProfileType = ProfilesType.Page,
                                            ProfileTypeId = 5,
                                            ProfilePicURL = profile.ProfilePic

                                        }).ToList();
                    result.AddRange(profilePages);*/
                    //Profiles  
                    result = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfileId.HasValue).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                    {
                        ProfileId = z.FirstOrDefault().Profile.Id,
                        ProfileName = z.FirstOrDefault().Profile.Name,
                        ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                        ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                        CompanyId = z.FirstOrDefault().CompanyId,
                        IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                        ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                        AccountName = z.FirstOrDefault().Profile.UserName
                    }).ToList();
                    //Profile Pages
                    var profilePages = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfilePageId.HasValue).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                    {
                        ProfileId = z.FirstOrDefault().ProfilePage.Id,
                        ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                        ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                        ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                        CompanyId = z.FirstOrDefault().CompanyId,
                        IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                        ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                        AccountName = z.FirstOrDefault().ProfilePage.PageName
                    }).ToList();
                    result.AddRange(profilePages);
                    //adding Google Analytics Profiles
                    var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId && x.Teams.Any(a => a.Id == userId)).GroupBy(g => g.AnalyticId).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.FirstOrDefault().Id,
                        CompanyId = y.FirstOrDefault().CompanyId,
                        ProfileId = y.FirstOrDefault().AnalyticId,
                        ProfileName = y.FirstOrDefault().UserAnalyticAccount.AccountName + " " + y.FirstOrDefault().UserAnalyticAccount.AccountUrl,
                        ProfileType = ProfilesType.GoogleAnalytics,
                        ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                        IsConnected = true,
                        AccountName = y.FirstOrDefault().UserAnalyticAccount.AccountName
                    }).ToList();
                    result.AddRange(analyticsAccount);
                    /*//adding instagram Profiles
                    var instagramAccount = db.InstagramCompanyGroups.Where(x => x.CompanyId == companyId && x.Teams.Any(a => a.Id == userId)).GroupBy(g => g.InstagramId)
                    .Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.FirstOrDefault().Id,
                        CompanyId = y.FirstOrDefault().CompanyId,
                        ProfileId = y.FirstOrDefault().InstagramId,
                        ProfileName = y.FirstOrDefault().InstagramAccount.InstaFullName + " " + y.FirstOrDefault().InstagramAccount.InstaWebSite,
                        ProfileType = ProfilesType.Instagram,
                        ProfileTypeId = (int)ProfilesType.Instagram,
                        IsConnected = true,
                        ProfilePicURL = y.FirstOrDefault().InstagramAccount.InstaProfilePic
                    }).ToList();
                    result.AddRange(instagramAccount);*/
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<ProfileModel>> GetProfilesByUserId(int userId)
        {
            List<ProfileModel> result = new List<ProfileModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    var userProfiles = db.ProfileComapnyGroups.Where(x => x.Teams.Any(z => z.UserId == userId));
                    foreach (var userProfile in userProfiles)
                    {
                        if (userProfile.ProfileId != null)
                        {
                            var profile = new ProfileModel
                            {
                                ProfileId = userProfile.Profile.Id,
                                ProfileName = userProfile.Profile.Name,
                                ProfileType = (ProfilesType)userProfile.Profile.ProfileTypeId,
                                ProfileTypeId = userProfile.Profile.ProfileTypeId,
                                CompanyId = userProfile.Profile.CompanyId,
                                ProfilePicURL = userProfile.Profile.ProfilePic,
                                AccountName = userProfile.Profile.UserName
                            };
                            result.Add(profile);
                        }
                        else if (userProfile.ProfilePageId != null)
                        {
                            var profile = new ProfileModel
                            {
                                ProfileId = userProfile.ProfilePage.Id,
                                ProfileName = userProfile.ProfilePage.PageName,
                                ProfileType = (ProfilesType)userProfile.ProfilePage.Profile.ProfileTypeId,
                                ProfileTypeId = userProfile.ProfilePage.Profile.ProfileTypeId,
                                CompanyId = userProfile.ProfilePage.Profile.CompanyId,
                                ProfilePicURL = userProfile.ProfilePage.ProfilePic,
                                AccountName = userProfile.ProfilePage.PageName
                            };
                            result.Add(profile);
                        }
                    }
                    /*var userProfiles = db.Teams.Where(x => x.UserId == userId).Select(y => y.Profiles.Select(z => new ProfileModel
                    {
                        ProfileId = z.Id,
                        ProfileName = z.Name,
                        ProfileType = (ProfilesType)z.ProfileTypeId,
                        ProfileTypeId = z.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        ProfilePicURL = z.ProfilePic
                    }));
                    foreach (var userProfile in userProfiles)
                    {
                        foreach (var profile in userProfile)
                            result.Add(profile);
                    }*/

                    //Profile Pages

                    //adding Google Analytics Profiles
                    var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.Teams.Any(y => y.Id == userId)).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.Id,
                        CompanyId = y.CompanyId,
                        ProfileId = y.AnalyticId,
                        ProfileName = y.UserAnalyticAccount.AccountName + " " + y.UserAnalyticAccount.AccountUrl,
                        ProfileType = ProfilesType.GoogleAnalytics,
                        ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                        AccountName = y.UserAnalyticAccount.AccountName
                    }).ToList();
                    result.AddRange(analyticsAccount);
                    /*//adding Instagram Profiles.
                    var instagramAccount = db.InstagramCompanyGroups.Where(x => x.Teams.Any(y => y.Id == userId)).Select(y => new ProfileModel
                    {
                        ProfileCompnayGroupId = y.Id,
                        CompanyId = y.CompanyId,
                        ProfileId = y.InstagramId,
                        ProfileName = y.InstagramAccount.InstaFullName+ " " + y.InstagramAccount.InstaWebSite,
                        ProfileType = ProfilesType.Instagram,
                        ProfileTypeId = (int)ProfilesType.Instagram,
                        ProfilePicURL = y.InstagramAccount.InstaProfilePic
                    }).ToList();
                    result.AddRange(instagramAccount);*/
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<ProfileCompanyGroups>> GetAllProfilesByCompanyId(int companyId)
        {
            List<ProfileCompanyGroups> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.Groups.Where(x => x.CompanyId == companyId).Select(y => new ProfileCompanyGroups
                    {
                        GroupId = y.Id,
                        GroupName = y.Name,
                        ProfileId = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfileId.HasValue && x.GroupId == y.Id).Select(z => new ProfileModel
                        {
                            ProfileId = z.Profile.Id,
                            ProfileName = z.Profile.Name,
                            ProfileType = (ProfilesType)z.Profile.ProfileTypeId,
                            ProfileTypeId = z.Profile.ProfileTypeId,
                            CompanyId = z.CompanyId,
                            IsConnected = false,
                            ProfilePicURL = z.Profile.ProfilePic,
                            IsPage = false,
                            AccountName = z.Profile.UserName
                        }).AsEnumerable().Union(db.ProfileComapnyGroups.Where(x1 => x1.CompanyId == companyId && x1.ProfilePageId.HasValue && x1.GroupId == y.Id).Select(z1 => new ProfileModel
                        {
                            ProfileId = z1.ProfilePage.Id,
                            ProfileName = z1.ProfilePage.PageName,
                            ProfileType = (ProfilesType)z1.ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z1.ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z1.CompanyId,
                            IsConnected = false,
                            ProfilePicURL = z1.ProfilePage.ProfilePic,
                            IsPage = true,
                            AccountName = z1.ProfilePage.PageName
                        }).AsEnumerable()).Union(db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId && x.GroupId == y.Id).Select(y1 => new ProfileModel
                        {
                            ProfileId = y1.AnalyticId,
                            ProfileName = y1.UserAnalyticAccount.AccountName + " " + y1.UserAnalyticAccount.AccountUrl,
                            ProfileType = ProfilesType.GoogleAnalytics,
                            ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                            CompanyId = y1.CompanyId,
                            IsConnected = false,
                            ProfilePicURL = "",
                            IsPage = true,
                            AccountName = y1.UserAnalyticAccount.AccountName
                        }).AsEnumerable())
                        /* .Union(db.InstagramCompanyGroups.Where(x => x.CompanyId == companyId && x.GroupId == y.Id).Select(y1 => new ProfileModel
                         {
                             ProfileId = y1.InstagramId,
                             ProfileName = y1.InstagramAccount.InstaFullName + " " + y1.InstagramAccount.InstaWebSite,
                             ProfileType = ProfilesType.Instagram,
                             ProfileTypeId = (int)ProfilesType.Instagram,
                             CompanyId = y1.CompanyId,
                             IsConnected = false,
                             ProfilePicURL = y1.InstagramAccount.InstaProfilePic,
                             IsPage = true
                         }).AsEnumerable())*/.ToList()

                    }).ToList();


                    //var profilePages = db.Groups.Where(x => x.CompanyId == companyId).Select(y => new ProfileCompanyGroups
                    //{
                    //    GroupId = y.Id,

                    //    ProfileId = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfilePageId.HasValue && x.GroupId == y.Id).Select(z => new ProfileModel
                    //    {
                    //        ProfileId = z.ProfilePage.Id,
                    //        ProfileName = z.ProfilePage.PageName,
                    //        ProfileType = (ProfilesType)z.ProfilePage.Profile.ProfileTypeId,
                    //        ProfileTypeId = z.ProfilePage.Profile.ProfileTypeId,
                    //        CompanyId = z.CompanyId,
                    //        IsConnected = false,
                    //        ProfilePicURL = z.ProfilePage.Profile.ProfilePic
                    //    }).ToList()

                    //}).ToList();



                    //result.AddRange(profilePages);

                    //var analyticsAccount = db.Groups.Where(x => x.CompanyId == companyId).Select(z => new ProfileCompanyGroups
                    //{
                    //    GroupId = z.Id,

                    //    ProfileId = db.AnalyticCompanyUsers.Where(x => x.CompanyId == companyId && x.GroupId == z.Id).Select(y => new ProfileModel
                    //    {
                    //        CompanyId = y.CompanyId,
                    //        ProfileId = y.AnalyticId,
                    //        ProfileName = y.UserAnalyticAccount.AccountName + " " + y.UserAnalyticAccount.AccountUrl,
                    //        ProfileType = ProfilesType.GoogleAnalytics,
                    //        ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                    //        IsConnected = false,
                    //    }).ToList()

                    //}).ToList();


                    //  result.AddRange(analyticsAccount);
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> UpdateUserGroupProfiles(ProfileAssignementModel model)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var profiledt = new DataTable();
                profiledt.Columns.Add("ProfileCompanyGroupId", typeof(int));
                profiledt.Columns.Add("GroupId", typeof(int));
                profiledt.Columns.Add("ProfileTypeId", typeof(int));
                profiledt.Columns.Add("ProfileId", typeof(int));
                profiledt.Columns.Add("IsConnected", typeof(bool));
                foreach (var row in model.Profiles)
                {
                    profiledt.Rows.Add(row.ProfileCompanyGroupId, row.GroupId, row.ProfileTypeId, row.ProfileId, row.IsConnected);
                }
                try
                {
                    List<UserAccessLevel> lstUserAccessLevel = new List<UserAccessLevel>();
                    var teamMember = db.Teams.Where(x => x.Id == model.TeamMemberId).FirstOrDefault();
                    if (teamMember != null)
                    {
                        if (model.IsAdmin == 1)
                        {
                            teamMember.AspNetUser.AspNetRoles.Clear();
                            teamMember.AspNetUser.AspNetRoles = db.AspNetRoles.Where(x => x.Name == "Admin").ToList();

                            teamMember.Premissions.Clear();
                            //teamMember.AccessLevels.Clear();
                            teamMember.UserAccessLevels.Clear();
                            teamMember.Premissions = db.Premissions.ToList();
                            //teamMember.AccessLevels = db.AccessLevels.ToList();
                            int loop=1;
                            foreach (var item in db.AccessLevels.ToList())
                            {
                                UserAccessLevel temp = new UserAccessLevel();
                                temp.AccessLevel = item;
                                temp.OrderId = loop;
                                lstUserAccessLevel.Add(temp);
                                loop++;
                            }
                            teamMember.UserAccessLevels = lstUserAccessLevel;

                            teamMember.AnalyticCompanyGroups.Clear();
                            teamMember.ProfileComapnyGroups.Clear();
                            teamMember.Groups.Clear();
                            teamMember.AnalyticCompanyGroups = db.AnalyticCompanyGroups.Where(x => x.CompanyId == model.CompanyId).ToList();
                            teamMember.ProfileComapnyGroups = db.ProfileComapnyGroups.Where(x => x.CompanyId == model.CompanyId).ToList();
                            teamMember.Groups = db.Groups.Where(x => x.CompanyId == model.CompanyId).ToList();
                        }
                        else
                        {
                            teamMember.AspNetUser.AspNetRoles.Clear();
                            teamMember.Premissions.Clear();
                            //teamMember.AccessLevels.Clear();
                            teamMember.UserAccessLevels.Clear();
                            teamMember.Premissions = db.Premissions.Where(x => model.AssignedRights.Contains(x.Id)).ToList();
                            //teamMember.AccessLevels = db.AccessLevels.Where(x => model.AssignedReports.Contains(x.Id)).ToList();
                            int loop = 1;
                            foreach (var item in db.AccessLevels.ToList())
                            {
                                if (model.AssignedReports.Contains(item.Id))
                                {
                                    UserAccessLevel temp = new UserAccessLevel();
                                    temp.AccessLevel = item;
                                    temp.OrderId = loop;
                                    lstUserAccessLevel.Add(temp);
                                    loop++;
                                }
                            }
                            teamMember.UserAccessLevels = lstUserAccessLevel;                            
                        }
                    }
                    if(model.IsAdmin == 0)
                    {
                        var profileAssignemntTable = new SqlParameter("@Profiles", SqlDbType.Structured);
                        profileAssignemntTable.Value = profiledt;
                        profileAssignemntTable.TypeName = "dbo.ProfileAssignementTable";
                        result = db.Database.SqlQuery<bool>("exec dbo.UpdateUserGroupProfle @teamMemberId,@CompanyId,@Profiles", new SqlParameter("@teamMemberId", model.TeamMemberId), new SqlParameter("@CompanyId", model.CompanyId), profileAssignemntTable).FirstOrDefault();
                    }
                    var resultset = await db.SaveChangesAsync();
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<List<ProfileModel>> GetProfilesByCompanyAndGroupId(int companyId, int groupId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {

                    result = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfileId.HasValue)
                        .GroupBy(
                        g => new { g.ProfileId, g.CompanyId }).ToList().Select(z => new ProfileModel
                        {
                            ProfileId = z.First().Profile.Id,
                            ProfileName = z.First().Profile.Name,
                            ProfileType = (ProfilesType)z.First().Profile.ProfileTypeId,
                            ProfileTypeId = z.First().Profile.ProfileTypeId,
                            CompanyId = z.First().CompanyId,
                            IsConnected = z.Any(p => p.GroupId == groupId),
                            ProfilePicURL = z.First().Profile.ProfilePic,
                            AccountName = z.First().Profile.UserName
                        }).ToList();
                    //Profile Pages
                    var profilePages = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfilePageId.HasValue)
                        .GroupBy(
                        g => new { g.ProfilePageId, g.CompanyId }).ToList().Select(z => new ProfileModel
                        {
                            ProfileId = z.First().ProfilePage.Id,
                            ProfileName = z.First().ProfilePage.PageName,
                            ProfileType = (ProfilesType)z.First().ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z.First().ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z.First().CompanyId,
                            IsConnected = z.Any(p => p.GroupId == groupId), //z.First().GroupId == groupId,
                            ProfilePicURL = z.First().ProfilePage.ProfilePic,
                            AccountName = z.First().ProfilePage.PageName
                        }).ToList();
                    result.AddRange(profilePages);
                    var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId)
                       .GroupBy(
                       g => new { g.AnalyticId, g.CompanyId }).ToList().Select(z => new ProfileModel
                       {
                           CompanyId = z.First().CompanyId,
                           ProfileId = z.First().AnalyticId,
                           ProfileName = z.First().UserAnalyticAccount.AccountName + " " + z.First().UserAnalyticAccount.AccountUrl,
                           ProfileType = ProfilesType.GoogleAnalytics,
                           ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                           IsConnected = z.Any(p => p.GroupId == groupId),
                           AccountName = z.First().UserAnalyticAccount.AccountName
                       }).ToList();
                    result.AddRange(analyticsAccount);
                    /*//Instagram
                    var instagramAccount = db.InstagramCompanyGroups.Where(x => x.CompanyId == companyId)
                     .GroupBy(
                     g => new { g.InstagramId, g.CompanyId }).ToList().Select(z => new ProfileModel
                     {
                         CompanyId = z.First().CompanyId,
                         ProfileId = z.First().InstagramId,
                         ProfileName = z.First().InstagramAccount.InstaFullName+ " " + z.First().InstagramAccount.InstaWebSite,
                         ProfileType = ProfilesType.Instagram,
                         ProfileTypeId = (int)ProfilesType.Instagram,
                         IsConnected = z.Any(p => p.GroupId == groupId),
                         ProfilePicURL = z.FirstOrDefault().InstagramAccount.InstaProfilePic
                     }).ToList();
                    result.AddRange(instagramAccount);*/
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<ProfileCompanyGroupsDetail>> GetAllProfilesByCompanyAndUserId(int companyId, int userId)
        {
            List<ProfileCompanyGroups> result = null;
            List<ProfileCompanyGroupsDetail> resultNew = new List<ProfileCompanyGroupsDetail>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {

                    var user = db.Teams.Where(x => x.UserId == userId).ToList();
                    var grpIds = new List<int>();
                    user.ForEach(x =>
                    {
                        grpIds.AddRange(x.Groups.Select(y => y.Id).ToList());
                    });
                    result = db.Groups.Where(x => x.CompanyId == companyId && grpIds.Contains(x.Id)).Select(y => new ProfileCompanyGroups
                    {
                        GroupId = y.Id,
                        GroupName = y.Name,
                        ProfileId = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfileId.HasValue && x.GroupId == y.Id).Select(z => new ProfileModel
                        {
                            ProfileId = z.Profile.Id,
                            ProfileName = z.Profile.Name,
                            ProfileType = (ProfilesType)z.Profile.ProfileTypeId,
                            ProfileTypeId = z.Profile.ProfileTypeId,
                            CompanyId = z.CompanyId,
                            IsConnected = z.Teams.Any(u => u.UserId == userId),
                            ProfilePicURL = z.Profile.ProfilePic,
                            IsPage = false,
                            AccountName = z.Profile.UserName
                        }).AsEnumerable().Union(db.ProfileComapnyGroups.Where(x1 => x1.CompanyId == companyId && x1.ProfilePageId.HasValue && x1.GroupId == y.Id).Select(z1 => new ProfileModel
                        {
                            ProfileId = z1.ProfilePage.Id,
                            ProfileName = z1.ProfilePage.PageName,
                            ProfileType = (ProfilesType)z1.ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z1.ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z1.CompanyId,
                            IsConnected = z1.Teams.Any(u => u.UserId == userId),
                            ProfilePicURL = z1.ProfilePage.ProfilePic,
                            IsPage = true,
                            AccountName = z1.ProfilePage.PageName
                        }).AsEnumerable()).Union(db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId && x.GroupId == y.Id).Select(y1 => new ProfileModel
                        {
                            ProfileId = y1.AnalyticId,
                            ProfileName = y1.UserAnalyticAccount.AccountName + " " + y1.UserAnalyticAccount.AccountUrl,
                            ProfileType = ProfilesType.GoogleAnalytics,
                            ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                            CompanyId = y1.CompanyId,
                            IsConnected = y1.Teams.Any(u => u.UserId == userId),
                            ProfilePicURL = "",
                            IsPage = true,
                            AccountName = y1.UserAnalyticAccount.AccountName
                        }).AsEnumerable())
                        /*.Union(db.InstagramCompanyGroups.Where(x => x.CompanyId == companyId && x.GroupId == y.Id).Select(y1 => new ProfileModel
                        {
                            ProfileId = y1.InstagramId,
                            ProfileName = y1.InstagramAccount.InstaFullName+ " " + y1.InstagramAccount.InstaWebSite,
                            ProfileType = ProfilesType.Instagram,
                            ProfileTypeId = (int)ProfilesType.Instagram,
                            CompanyId = y1.CompanyId,
                            IsConnected = y1.Teams.Any(u => u.UserId == userId),
                            ProfilePicURL = y1.InstagramAccount.InstaProfilePic,
                            IsPage = true
                        }).AsEnumerable())*/.ToList(),


                    }).ToList();

                    var Team = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(y1 => new TeamMemberViewModel
                    {
                        Profile = new TeamMember
                        {
                            AssignedProfile = y1.ProfileComapnyGroups.Where(z => z.CompanyId == companyId && z.Teams.Any(j => j.UserId == userId)).Count(),
                            Email = y1.EmailId,
                            Name = y1.FirstName + " " + y1.LastName,
                            FirstName = y1.FirstName,
                            LastName = y1.LastName,
                            ProfileUrl = y1.AspNetUser.ProfileImage,
                            UserId = y1.UserId ?? 0,
                            TeamMemberId = y1.Id,
                            IsActive = y1.IsActive,
                            IsOwner = db.UserCompanies.Where(m => m.UserId == y1.UserId && m.CompanyId == y1.CompanyId).FirstOrDefault().CompanyTypeId == 1 ? true : false,
                            IsAdmin = y1.AspNetUser.AspNetRoles.Any(x => x.Name == "Admin")
                        },
                        Groups = y1.Groups.Select(z => new Models.Group.GroupModel
                        {
                            GroupId = z.Id,
                            GroupName = z.Name
                        }).ToList(),
                        Rights = y1.Premissions.Select(z => new UserRightModel { Id = z.Id, Name = z.Name }).ToList(),
                        //VisibleReports = y1.AccessLevels.Select(z => new UserReportModel { Id = z.Id, Name = z.Name }).ToList()
                        VisibleReports = y1.UserAccessLevels.Select(x=> new UserReportModel { Id = x.AccessLevel.Id, Name = x.AccessLevel.Name}).ToList()
                    }).FirstOrDefault();



                    ProfileCompanyGroupsDetail pcg = new ProfileCompanyGroupsDetail();
                    pcg.ProfileCompanyGroup = result;
                    pcg.Team = Team;
                    resultNew.Add(pcg);

                    db.Dispose();
                }
            });
            return resultNew;
        }
        public async Task<List<ProfileModel>> GetProfilesNotInGroup(int companyId, int groupId)
        {
            List<ProfileModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    //Social Profiles.
                    /*result = db.Profiles.Where(x => x.CompanyId == companyId).Select(z => new ProfileModel
                    {
                        ProfileId =z.Id,
                        ProfileName =z.Name,
                        ProfileType = (ProfilesType)z.ProfileTypeId,
                        ProfileTypeId = z.ProfileTypeId,
                        CompanyId = z.CompanyId,
                        IsConnected = z.Company.ProfileComapnyGroups.Any(t=>t.GroupId==groupId&&t.ProfileId==z.Id),
                        ProfilePicURL = z.ProfilePic
                    }).ToList();*/

                    result = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfileId.HasValue)
                        .GroupBy(
                        g => new { g.ProfileId, g.CompanyId }).ToList().Select(z => new ProfileModel
                        {
                            ProfileId = z.First().Profile.Id,
                            ProfileName = z.First().Profile.Name,
                            ProfileType = (ProfilesType)z.First().Profile.ProfileTypeId,
                            ProfileTypeId = (int)(ProfilesType)z.First().Profile.ProfileTypeId,
                            CompanyId = z.First().CompanyId,
                            IsConnected = z.Any(p => p.GroupId == groupId),
                            ProfilePicURL = z.First().Profile.ProfilePic,
                            AccountName = z.First().Profile.UserName
                        }).ToList();
                    //Profile Pages
                    var profilePages = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfilePageId.HasValue)
                        .GroupBy(
                        g => new { g.ProfilePageId, g.CompanyId }).ToList().Select(z => new ProfileModel
                        {
                            ProfileId = z.First().ProfilePage.Id,
                            ProfileName = z.First().ProfilePage.PageName,
                            ProfileType = ProfilesType.Page,//z.First().ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = (int)ProfilesType.Page,//z.First().ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z.First().CompanyId,
                            IsConnected = z.Any(p => p.GroupId == groupId),
                            ProfilePicURL = z.First().ProfilePage.ProfilePic,
                            AccountName = z.First().ProfilePage.PageName
                        }).ToList();
                    result.AddRange(profilePages);
                    //Google analytics.
                    var analyticsAccount = db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId)
                       .GroupBy(
                       g => new { g.AnalyticId, g.CompanyId }).ToList().Select(z => new ProfileModel
                       {
                           CompanyId = z.First().CompanyId,
                           ProfileId = z.First().AnalyticId,
                           ProfileName = z.First().UserAnalyticAccount.AccountName + " " + z.First().UserAnalyticAccount.AccountUrl,
                           ProfileType = ProfilesType.GoogleAnalytics,
                           ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                           IsConnected = z.Any(p => p.GroupId == groupId),
                           AccountName = z.First().UserAnalyticAccount.AccountName
                       }).ToList();
                    result.AddRange(analyticsAccount);
                    /*//Instagram Accounts.
                    var instagramAccount = db.InstagramCompanyGroups.Where(x => x.CompanyId == companyId)
                      .GroupBy(
                      g => new { g.InstagramId, g.CompanyId }).ToList().Select(z => new ProfileModel
                      {
                          CompanyId = z.First().CompanyId,
                          ProfileId = z.First().InstagramId,
                          ProfileName = z.First().InstagramAccount.InstaFullName+ " " + z.First().InstagramAccount.InstaWebSite,
                          ProfileType = ProfilesType.Instagram,
                          ProfileTypeId = (int)ProfilesType.Instagram,
                          IsConnected = z.Any(p => p.GroupId == groupId),
                          ProfilePicURL = z.FirstOrDefault().InstagramAccount.InstaProfilePic
                      }).ToList();
                    result.AddRange(instagramAccount);*/
                    result.RemoveAll(r => r.IsConnected);
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> AssignGroup_old(List<Models.Profiles.ProfileCompanyGroup> model)
        {
            var profileCompanyGrp = new List<ProfileComapnyGroup>();
            var analyticCompanyGrp = new List<AnalyticCompanyGroup>();
            //var instagramCompanyGrp = new List<InstagramCompanyGroup>();
            var status = true;
            foreach (var item in model)
            {
                switch (item.ProfileType)
                {
                    case ProfilesType.GoogleAnalytics:
                        analyticCompanyGrp.Add(new AnalyticCompanyGroup { AnalyticId = item.ProfileId, CompanyId = item.CompanyId, GroupId = item.GroupId, isGroupConnected = true });
                        break;
                    case ProfilesType.Page:
                        profileCompanyGrp.Add(new ProfileComapnyGroup { CompanyId = item.CompanyId, GroupId = item.GroupId, ProfilePageId = item.ProfileId });
                        break;
                    case ProfilesType.Facebook:
                    case ProfilesType.LinkedIn:
                    case ProfilesType.Twitter:
                    case ProfilesType.Instagram:
                        profileCompanyGrp.Add(new ProfileComapnyGroup { CompanyId = item.CompanyId, GroupId = item.GroupId, ProfileId = item.ProfileId });
                        break;
                        //case ProfilesType.Instagram:
                        //    instagramCompanyGrp.Add(new InstagramCompanyGroup {CompanyId=item.CompanyId,GroupId=item.GroupId,InstagramId=item.ProfileId });
                        //    break;
                }
            }
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    db.ProfileComapnyGroups.AddRange(profileCompanyGrp);
                    db.AnalyticCompanyGroups.AddRange(analyticCompanyGrp);
                    //db.InstagramCompanyGroups.AddRange(instagramCompanyGrp);
                    foreach (var obj in profileCompanyGrp)
                    {
                        db.AddUserNotification(obj.GroupId, 1, "Profile : " + obj.ProfilePageId != null ? obj.ProfilePage.PageName : obj.Profile.Name + " Added in Group : " + db.Groups.Where(x => x.Id == obj.GroupId).FirstOrDefault().Name, "Profile Added");
                    }
                    foreach (var obj in analyticCompanyGrp)
                    {
                        db.AddUserNotification(obj.GroupId, 1, "Profile : " + obj.UserAnalyticAccount.AccountName + " Added in Group : " + db.Groups.Where(x => x.Id == obj.GroupId).FirstOrDefault().Name, "Profile Added");
                    }
                    //foreach (var obj in instagramCompanyGrp)
                    //{
                    //    db.AddUserNotification(obj.GroupId, 1, "Profile : " + obj.InstagramAccount.InstaUserName + " Added in Group : " + db.Groups.Where(x => x.Id == obj.GroupId).FirstOrDefault().Name, "Profile Added");
                    //}
                    await db.SaveChangesAsync();
                }
                catch (Exception exp)
                {
                    status = false;
                }
            }
            return status;
        }
        public async Task<bool> AssignGroup(List<Models.Profiles.ProfileCompanyGroup> model)
        {
            var profileCompanyGrp = new List<ProfileComapnyGroup>();
            var analyticCompanyGrp = new List<AnalyticCompanyGroup>();
            //var instagramCompanyGrp = new List<InstagramCompanyGroup>();
            var status = true;
            using (var db = new SocialCRMEntities())
            {
                foreach (var item in model)
                {
                    switch (item.ProfileType)
                    {
                        case ProfilesType.GoogleAnalytics:
                            if (!db.AnalyticCompanyGroups.Any(x => x.AnalyticId == item.ProfileId && x.CompanyId == item.CompanyId && x.GroupId == item.GroupId))
                                analyticCompanyGrp.Add(new AnalyticCompanyGroup { AnalyticId = item.ProfileId, CompanyId = item.CompanyId, GroupId = item.GroupId, isGroupConnected = true });
                            break;
                        case ProfilesType.Page:
                            if (!db.ProfileComapnyGroups.Any(x => x.ProfilePageId == item.ProfileId && x.CompanyId == item.CompanyId && x.GroupId == item.GroupId))
                                profileCompanyGrp.Add(new ProfileComapnyGroup { CompanyId = item.CompanyId, GroupId = item.GroupId, ProfilePageId = item.ProfileId });
                            break;
                        case ProfilesType.Facebook:
                        case ProfilesType.LinkedIn:
                        case ProfilesType.Twitter:
                        case ProfilesType.Outlook:
                        case ProfilesType.Instagram:
                            if (!db.ProfileComapnyGroups.Any(x => x.ProfileId == item.ProfileId && x.CompanyId == item.CompanyId && x.GroupId == item.GroupId))
                                profileCompanyGrp.Add(new ProfileComapnyGroup { CompanyId = item.CompanyId, GroupId = item.GroupId, ProfileId = item.ProfileId });
                            break;
                            //case ProfilesType.Instagram:
                            //    instagramCompanyGrp.Add(new InstagramCompanyGroup {CompanyId=item.CompanyId,GroupId=item.GroupId,InstagramId=item.ProfileId });
                            //    break;
                    }
                }
                try
                {
                    db.ProfileComapnyGroups.AddRange(profileCompanyGrp);
                    db.AnalyticCompanyGroups.AddRange(analyticCompanyGrp);
                    //db.InstagramCompanyGroups.AddRange(instagramCompanyGrp);
                    if(model.Count > 0)
                    {
                        var companyId = model[0].CompanyId;
                        var teams = db.Teams.Where(t => t.CompanyId == companyId && t.AspNetUser.AspNetRoles.Any(y => y.Name == "Admin")).ToList();
                        foreach(Team team in teams)
                        {
                            team.ProfileComapnyGroups = profileCompanyGrp;
                            team.AnalyticCompanyGroups = analyticCompanyGrp;
                        }
                    }
                    string message = "";
                    foreach (var obj in profileCompanyGrp)
                    {

                        if (obj.ProfilePageId == null)
                        {
                            message = "Profile : " + db.Profiles.Where(p => p.Id == obj.ProfileId).FirstOrDefault().Name + " Added in Group : " + db.Groups.Where(x => x.Id == obj.GroupId).FirstOrDefault().Name;
                        }
                        else
                        {
                            message = "Profile Page : " + db.ProfilePages.Where(p => p.Id == obj.ProfilePageId).FirstOrDefault().PageName + " Added in Group : " + db.Groups.Where(x => x.Id == obj.GroupId).FirstOrDefault().Name;
                        }
                        db.AddUserNotification(obj.GroupId, 1, message, "Profile Added");
                    }
                    foreach (var obj in analyticCompanyGrp)
                    {
                        db.AddUserNotification(obj.GroupId, 1, "Profile : " + db.UserAnalyticAccounts.Where(p => p.Id == obj.AnalyticId).FirstOrDefault().AccountName + " Added in Group : " + db.Groups.Where(x => x.Id == obj.GroupId).FirstOrDefault().Name, "Profile Added");
                    }
                    //foreach (var obj in instagramCompanyGrp)
                    //{
                    //    db.AddUserNotification(obj.GroupId, 1, "Profile : " + obj.InstagramAccount.InstaUserName + " Added in Group : " + db.Groups.Where(x => x.Id == obj.GroupId).FirstOrDefault().Name, "Profile Added");
                    //}
                    await db.SaveChangesAsync();
                }
                catch (Exception exp)
                {
                    status = false;
                }
            }
            return status;
        }
        public async Task<List<ProfileCompanyGroupsDetail>> GetAllProfilesByMemberId(int companyId, int teamMemberId)
        {
            List<ProfileCompanyGroups> result = null;
            List<ProfileCompanyGroupsDetail> resultNew = new List<ProfileCompanyGroupsDetail>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {

                    var user = db.Teams.Where(x => x.Id == teamMemberId).ToList();
                    var grpIds = new List<int>();
                    user.ForEach(x =>
                    {
                        grpIds.AddRange(x.Groups.Select(y => y.Id).ToList());
                    });
                    result = db.Groups.Where(x => x.CompanyId == companyId && grpIds.Contains(x.Id)).Select(y => new ProfileCompanyGroups
                    {
                        GroupId = y.Id,
                        GroupName = y.Name,
                        ProfileId = db.ProfileComapnyGroups.Where(x => x.CompanyId == companyId && x.ProfileId.HasValue && x.GroupId == y.Id).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.Id,
                            ProfileId = z.Profile.Id,
                            ProfileName = z.Profile.Name,
                            ProfileType = (ProfilesType)z.Profile.ProfileTypeId,
                            ProfileTypeId = z.Profile.ProfileTypeId,
                            CompanyId = z.CompanyId,
                            IsConnected = z.Teams.Any(u => u.Id == teamMemberId),
                            ProfilePicURL = z.Profile.ProfilePic,
                            IsPage = false,
                            AccountName = z.Profile.UserName
                        }).AsEnumerable().Union(db.ProfileComapnyGroups.Where(x1 => x1.CompanyId == companyId && x1.ProfilePageId.HasValue && x1.GroupId == y.Id).Select(z1 => new ProfileModel
                        {
                            ProfileCompnayGroupId = z1.Id,
                            ProfileId = z1.ProfilePage.Id,
                            ProfileName = z1.ProfilePage.PageName,
                            ProfileType = (ProfilesType)z1.ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z1.ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z1.CompanyId,
                            IsConnected = z1.Teams.Any(u => u.Id == teamMemberId),
                            ProfilePicURL = z1.ProfilePage.ProfilePic,
                            IsPage = true,
                            AccountName = z1.ProfilePage.PageName
                        }).AsEnumerable()).Union(db.AnalyticCompanyGroups.Where(x => x.CompanyId == companyId && x.GroupId == y.Id).Select(y1 => new ProfileModel
                        {
                            ProfileCompnayGroupId = y1.Id,
                            ProfileId = y1.AnalyticId,
                            ProfileName = y1.UserAnalyticAccount.AccountName + " " + y1.UserAnalyticAccount.AccountUrl,
                            ProfileType = ProfilesType.GoogleAnalytics,
                            ProfileTypeId = (int)ProfilesType.GoogleAnalytics,
                            CompanyId = y1.CompanyId,
                            IsConnected = y1.Teams.Any(u => u.Id == teamMemberId),
                            ProfilePicURL = "",
                            IsPage = true,
                            AccountName = y1.UserAnalyticAccount.AccountName
                        }).AsEnumerable())
                        /*.Union(db.InstagramCompanyGroups.Where(x => x.CompanyId == companyId && x.GroupId == y.Id).Select(y1 => new ProfileModel
                        {
                            ProfileCompnayGroupId = y1.Id,
                            ProfileId = y1.InstagramId,
                            ProfileName = y1.InstagramAccount.InstaFullName+ " " + y1.InstagramAccount.InstaWebSite,
                            ProfileType = ProfilesType.Instagram,
                            ProfileTypeId = (int)ProfilesType.Instagram,
                            CompanyId = y1.CompanyId,
                            IsConnected = y1.Teams.Any(u => u.Id == teamMemberId),
                            ProfilePicURL = y1.InstagramAccount.InstaProfilePic,
                            IsPage = true
                        }).AsEnumerable())*/.ToList(),


                    }).ToList();

                    var Team = db.Teams.Where(x => x.Id == teamMemberId && x.CompanyId == companyId).Select(y1 => new TeamMemberViewModel
                    {
                        Profile = new TeamMember
                        {
                            AssignedProfile = y1.ProfileComapnyGroups.Where(z => z.CompanyId == companyId && z.Teams.Any(j => j.Id == teamMemberId)).Count(),
                            Email = y1.EmailId,
                            Name = y1.FirstName + " " + y1.LastName,
                            FirstName = y1.FirstName,
                            LastName = y1.LastName,
                            ProfileUrl = y1.AspNetUser.ProfileImage,
                            UserId = y1.UserId ?? 0,
                            TeamMemberId = y1.Id,
                            IsActive = y1.IsActive,
                            IsOwner = db.UserCompanies.Where(m => m.UserId == y1.UserId && m.CompanyId == y1.CompanyId).FirstOrDefault().CompanyTypeId == 1 ? true : false,
                            IsAdmin = y1.AspNetUser.AspNetRoles.Any(x => x.Name == "Admin")
                        },
                        Groups = y1.Groups.Select(z => new Models.Group.GroupModel
                        {
                            GroupId = z.Id,
                            GroupName = z.Name
                        }).ToList(),
                        Rights = y1.Premissions.Select(z => new UserRightModel { Id = z.Id, Name = z.Name }).ToList(),
                        //VisibleReports = y1.AccessLevels.Select(z => new UserReportModel { Id = z.Id, Name = z.Name }).ToList()
                        VisibleReports = y1.UserAccessLevels.Select(x => new UserReportModel { Id = x.AccessLevel.Id, Name = x.AccessLevel.Name }).ToList()
                    }).FirstOrDefault();



                    ProfileCompanyGroupsDetail pcg = new ProfileCompanyGroupsDetail();
                    pcg.ProfileCompanyGroup = result;
                    pcg.Team = Team;
                    resultNew.Add(pcg);

                    db.Dispose();
                }
            });
            return resultNew;
        }

        public async Task<List<SearchWordPattern>> GetPattern()
        {
            List<SearchWordPattern> result = new List<SearchWordPattern>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    result = context.SearchWordPatterns.ToList();
                }
            });
            return result;
        }

        public async Task<bool> UpdateConsumerNumPattern(int id, string regex, string pattern)
        {
            bool isSaved = false;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    var patternObj = new SearchWordPattern();
                    if (id > 0)
                    {
                        patternObj = context.SearchWordPatterns.Where(x => x.Id == id).FirstOrDefault();
                        patternObj.RegexFor = regex.Trim();
                        patternObj.RegexLength = regex.Trim().Length;
                        patternObj.Pattern = pattern.Trim();
                        string jsPattern = pattern.Trim().Replace('%', ' ');
                        patternObj.JsPattern = jsPattern;

                        context.SearchWordPatterns.Attach(patternObj);
                        context.Entry(patternObj).State = EntityState.Modified;
                    }
                    else
                    {
                        patternObj.RegexFor = regex.Trim();
                        patternObj.RegexLength = regex.Trim().Length;
                        patternObj.Pattern = RegexExtensions.GenerateRegex(regex, "sql");
                        patternObj.JsPattern = RegexExtensions.GenerateRegex(regex, "js");
                        patternObj.IsActive = 0;

                        context.SearchWordPatterns.Add(patternObj);
                    }
                    context.SaveChanges();
                    isSaved = true;
                }
            });
            return isSaved;
        }

        public async Task<bool> UpdateActivePattern(int id)
        {
            bool isSaved = false;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    foreach (var pattern in context.SearchWordPatterns.Where(x => x.Id != id).ToList())
                    {
                        pattern.IsActive = 0;

                        context.SearchWordPatterns.Attach(pattern);
                        context.Entry(pattern).State = EntityState.Modified;
                    }
                    var patternObj = new SearchWordPattern();
                    if (id > 0)
                    {
                        patternObj = context.SearchWordPatterns.Where(x => x.Id == id).FirstOrDefault();
                        patternObj.IsActive = 1;

                        context.SearchWordPatterns.Attach(patternObj);
                    }
                    context.Entry(patternObj).State = EntityState.Modified;
                    context.SaveChanges();
                    isSaved = true;
                }
            });
            return isSaved;
        }

        public async Task<bool> DisablePattern()
        {
            bool isSaved = false;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    foreach (var pattern in context.SearchWordPatterns.ToList())
                    {
                        pattern.IsActive = 0;

                        context.SearchWordPatterns.Attach(pattern);
                        context.Entry(pattern).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                    isSaved = true;
                }
            });
            return isSaved;
        }
    }
}