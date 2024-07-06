using CRM.Common.Enum;
using CRM.Entities;
using SocialAPIsWrapper.Models;
using SocialCRM.Web.Models.Publisher;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SocialCRM.Web.Models.GeneratSettings;
using SocialCRM.Web.Models.Profiles;
using SocialAPIsWrapper.APIs;
using SocialCRM.Web.Models.Profile;
using Facebook;

namespace SocialCRM.Web.Repositories
{
    public class GeneralSettingsRepository : IGeneralSettingsRepository
    {
        public async Task<bool> UpdateSampleReply(int id, string replyText)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    if (id > 0)
                    {
                        var reply = db.SampleReplies.Find(id);
                        if (reply != null)
                        {
                            reply.ReplyText = replyText;
                            db.SampleReplies.Attach(reply);
                            db.Entry(reply).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        var reply = new SampleReply();
                        reply.ReplyText = replyText;
                        db.SampleReplies.Add(reply);
                        db.SaveChanges();
                    }
                    var resultset = await db.SaveChangesAsync();
                    result = resultset > 0;
                    db.Dispose();
                }
                catch (Exception ex)
                {

                    throw ex;
                }                
            }
            return result;
        }

        public List<SocialMediaUserType> GetSocialMediaUserTypes()
        {
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    List<SocialMediaUserType> lstSocialMediaUserType = new List<SocialMediaUserType>();
                    var userTypes = db.SocialMediaUserTypes.ToList();
                    foreach(var userType in userTypes)
                    {
                        SocialMediaUserType socialMediaUserTypeObj = new SocialMediaUserType();
                        socialMediaUserTypeObj.Id = userType.Id;
                        socialMediaUserTypeObj.Name = userType.Name;
                        lstSocialMediaUserType.Add(socialMediaUserTypeObj);
                    }

                    return lstSocialMediaUserType;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<bool> AddUpdateEmailSettings(SocialMediaUsersUpdateReceiver model)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    if (model.ID > 0)
                    {
                        var SocialMediaUsersUpdateReceiverObj = db.SocialMediaUsersUpdateReceivers.Find(model.ID);
                        if (SocialMediaUsersUpdateReceiverObj != null)
                        {
                            SocialMediaUsersUpdateReceiverObj.Name = model.Name;
                            SocialMediaUsersUpdateReceiverObj.Email = model.Email;
                            SocialMediaUsersUpdateReceiverObj.Over10KFollowers = model.Over10KFollowers;
                            SocialMediaUsersUpdateReceiverObj.PoliticalFigures = model.PoliticalFigures;
                            SocialMediaUsersUpdateReceiverObj.VerifiedAccounts = model.VerifiedAccounts;
                            SocialMediaUsersUpdateReceiverObj.TopInfluencers = model.TopInfluencers;
                            SocialMediaUsersUpdateReceiverObj.ReceiveReport = model.ReceiveReport;
                            SocialMediaUsersUpdateReceiverObj.CompanyId = model.CompanyId;
                            
                            db.SocialMediaUsersUpdateReceivers.Attach(SocialMediaUsersUpdateReceiverObj);
                            db.Entry(SocialMediaUsersUpdateReceiverObj).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        var SocialMediaUsersUpdateReceiverObj = new SocialMediaUsersUpdateReceiver();
                        SocialMediaUsersUpdateReceiverObj.Name = model.Name;
                        SocialMediaUsersUpdateReceiverObj.Email = model.Email;
                        SocialMediaUsersUpdateReceiverObj.Over10KFollowers = model.Over10KFollowers;
                        SocialMediaUsersUpdateReceiverObj.PoliticalFigures = model.PoliticalFigures;
                        SocialMediaUsersUpdateReceiverObj.VerifiedAccounts = model.VerifiedAccounts;
                        SocialMediaUsersUpdateReceiverObj.TopInfluencers = model.TopInfluencers;
                        SocialMediaUsersUpdateReceiverObj.ReceiveReport = model.ReceiveReport;
                        SocialMediaUsersUpdateReceiverObj.CompanyId = model.CompanyId;

                        db.SocialMediaUsersUpdateReceivers.Add(SocialMediaUsersUpdateReceiverObj);
                        db.SaveChanges();
                    }
                    var resultset = await db.SaveChangesAsync();
                    result = resultset > 0;
                    db.Dispose();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return result;
        }

        public async Task<bool> DeleteEmailSettings(int Id)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var socialMediaUpdateReceiver = db.SocialMediaUsersUpdateReceivers.Find(Id);
                    if (socialMediaUpdateReceiver != null)
                    {
                        db.SocialMediaUsersUpdateReceivers.Remove(socialMediaUpdateReceiver);
                        var resultset = await db.SaveChangesAsync();
                        result = resultset > 0;
                        db.Dispose();
                    }

                    
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return result;
        }

        public async Task<bool> DeleteSampleReply(int Id)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var sampleReply = db.SampleReplies.Find(Id);
                    if (sampleReply != null)
                    {
                        db.SampleReplies.Remove(sampleReply);
                        var resultset = await db.SaveChangesAsync();
                        result = resultset > 0;
                        db.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }

        public List<SocialMediaUsersUpdateReceiver> GetSocialMediaUsersUpdateReceiversList()
        {
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    List<SocialMediaUsersUpdateReceiver> lstSocialMediaUsersUpdateReceiver = new List<SocialMediaUsersUpdateReceiver>();
                    var updateReceivers = db.SocialMediaUsersUpdateReceivers.ToList();
                    foreach (var updateReceiver in updateReceivers)
                    {
                        SocialMediaUsersUpdateReceiver socialMediaUpdateReceiverObj = new SocialMediaUsersUpdateReceiver();
                        socialMediaUpdateReceiverObj.ID = updateReceiver.ID;
                        socialMediaUpdateReceiverObj.Name = updateReceiver.Name;
                        socialMediaUpdateReceiverObj.Email = updateReceiver.Email;
                        socialMediaUpdateReceiverObj.Over10KFollowers = updateReceiver.Over10KFollowers;
                        socialMediaUpdateReceiverObj.PoliticalFigures = updateReceiver.PoliticalFigures;
                        socialMediaUpdateReceiverObj.VerifiedAccounts = updateReceiver.VerifiedAccounts;
                        socialMediaUpdateReceiverObj.TopInfluencers = updateReceiver.TopInfluencers;
                        socialMediaUpdateReceiverObj.ReceiveReport = updateReceiver.ReceiveReport;
                        socialMediaUpdateReceiverObj.CompanyId = updateReceiver.CompanyId;

                        lstSocialMediaUsersUpdateReceiver.Add(socialMediaUpdateReceiverObj);
                    }

                    return lstSocialMediaUsersUpdateReceiver;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<InfluencerListModel> GetInfluencersList(int userId, int groupId, int pageNumber, bool getOptions)
        {
            InfluencerListModel result = new InfluencerListModel();
            result.UserList = new List<InfluencersModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        int pageSize = 10;
                        pageNumber = pageNumber * pageSize;
                        List<ProfileModel> profiles = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfileId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            ProfileId = z.FirstOrDefault().Profile.Id,
                            ProfileName = z.FirstOrDefault().Profile.Name,
                            ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                            ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                            CompanyId = z.FirstOrDefault().CompanyId,
                            IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                            AccountName = z.FirstOrDefault().Profile.UserName,
                            IsPage = false,
                            UserId = z.FirstOrDefault().Profile.ProfileExternalId,
                            Id = z.FirstOrDefault().Profile.Id
                        }).ToList();

                        List<ProfileModel> profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            Id = z.FirstOrDefault().ProfilePage.Id,
                            PageName = z.FirstOrDefault().ProfilePage.PageName,
                            ProfileType = ProfilesType.Facebook,
                            ProfileTypeId = 1,
                            CompanyId = z.FirstOrDefault().CompanyId,
                            IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                            AccountName = z.FirstOrDefault().ProfilePage.PageName,
                            IsPage = true,
                            PageId = z.FirstOrDefault().ProfilePage.ProfileExternalId                            
                        }).ToList();

                        profiles = profiles.Where(x => x.IsConnected && x.ProfileTypeId == 4).ToList();
                        profilePages = profilePages.Where(x => x.IsConnected).ToList();

                        int[] profileIds = profiles.Select(x => x.ProfileId).ToArray();
                        int[] profilePageIds = profilePages.Select(x => x.Id).ToArray();

                        var influencersList = db.CustomerProfileTypes.ToList().Where(x => profileIds.Contains(x.ProfileToCustSocialProfile.ProfileId)).ToList();
                        var influnecersListPage = db.CustomerProfileTypes.ToList().Where(x => profilePageIds.Contains(x.ProfileToCustSocialProfile.ProfilePageId)).ToList();

                        influencersList.AddRange(influnecersListPage);
                        result.TotalRecords = influencersList.Count;

                        influencersList = influencersList.OrderByDescending(x => x.Id).Skip(pageNumber).Take(pageSize).ToList();
                        List<ProfilePage> dbProfilePages = db.ProfilePages.ToList();
                        foreach (var item in influencersList)
                        {
                            InfluencersModel model = new InfluencersModel();
                            model.Id = item.Id; 
                            if(item.ProfileToCustSocialProfile.ProfilePageId == 0) //Twitter case
                            {
                                model.ProfileId = item.ProfileCustomerId.HasValue ? item.ProfileToCustSocialProfile.ProfileId : 0;
                                model.ProfileName = item.ProfileCustomerId.HasValue ? item.ProfileToCustSocialProfile.Profile.Name : "";
                                model.PlatformId = (int)ProfilesType.Twitter;
                                model.profilePicURL = item.ProfileToCustSocialProfile.CustomerSocialProfile.ProfilePic != null ? item.ProfileToCustSocialProfile.CustomerSocialProfile.ProfilePic.ToString() : "";
                            } 
                            else if (item.ProfileToCustSocialProfile.ProfilePageId != 0) //Facebook case
                            {
                                //Rethink
                                model.ProfileId = item.ProfileCustomerId.HasValue ? item.ProfileToCustSocialProfile.ProfilePageId : 0;
                                string pageName = dbProfilePages.Where(x => x.Id == model.ProfileId).FirstOrDefault().PageName;
                                model.ProfileName = item.ProfileCustomerId.HasValue ? pageName : "";
                                model.PlatformId = (int)ProfilesType.Facebook;
                                model.profilePicURL = item.ProfileToCustSocialProfile.CustomerSocialProfile.ProfilePic != null? item.ProfileToCustSocialProfile.CustomerSocialProfile.ProfilePic.ToString() : "";
                            }

                            model.AccountTypeId = item.AccountTypeId.HasValue ? item.AccountTypeId.Value : 0;
                            model.CustomerName = item.ProfileToCustSocialProfile.CustomerSocialProfile.UserName != null ? item.ProfileToCustSocialProfile.CustomerSocialProfile.UserName : "X";
                            model.AccountType = item.SocialMediaUserType.Name;

                            result.UserList.Add(model);
                        }

                        result.AccountTypeList = new List<SelectItemList>();
                        result.ProfilesList = new List<SelectItemList>();
                        if (getOptions)
                        {
                            db.SocialMediaUserTypes.ToList().ForEach(delegate (SocialMediaUserType obj)
                            {
                                result.AccountTypeList.Add(new SelectItemList()
                                {
                                    Value = obj.Id,
                                    Text = obj.Name,
                                    ProfileType = 0
                                });
                            });

                            profiles.ForEach(delegate (ProfileModel obj)
                            {
                                result.ProfilesList.Add(new SelectItemList()
                                {
                                    Value = obj.ProfileId,
                                    Text = obj.ProfileName,
                                    ProfileType = 4
                                });
                            });

                            profilePages.ForEach(delegate (ProfileModel obj)
                            {
                                result.ProfilesList.Add(new SelectItemList()
                                {
                                    Value = obj.Id,
                                    Text = obj.PageName,
                                    ProfileType = 1
                                });
                            });
                        }
                    }
                });
                
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public async Task<string> SaveInfluencer(SaveInfluencerModel model)
        {
            string result = "";
            try
            {
                
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var form = new CustomerProfileType();
                        CustomerSocialProfile profileObj = new CustomerSocialProfile();
                        if (model.Id > 0)
                        {
                            //Edit case
                            form = db.CustomerProfileTypes.Find(model.Id);
                            form.AccountTypeId = model.AccountTypeId;
                            

                            db.CustomerProfileTypes.Attach(form);
                            db.Entry(form).State = EntityState.Modified;                                
                            result = "success|" + form.Id;
                            db.SaveChanges();
                        }
                        else
                        {
                            bool isAlreadyExist = false;
                            bool isValidLink = false;                           
                            var customerSocialProfileObj = new CustomerSocialProfile();

                            //New implementation
                            string facebookAPIError = "";
                            var userName = "";
                            if (model.PlatformId == 4)
                            {
                                userName = db.Profiles.Where(x => x.Id == model.ProfileId).FirstOrDefault().UserName;

                                TwitterUtility twitterUtil = new TwitterUtility(userName);
                                var response = await twitterUtil.getTwitterProfile(model.CustomerProfileHandle);
                                isValidLink = !String.IsNullOrEmpty(response.ScreenName);

                                if (isValidLink)
                                {
                                    profileObj.Name = response.ScreenName;
                                    profileObj.ProfilePic = response.ProfileImageUrl;
                                    profileObj.Is_Verified = response.Verified;
                                    profileObj.ExternalId = response.UserIDResponse;
                                    profileObj.Followers = response.FollowersCount;                                   
                                }
                            }
                            else if (model.PlatformId == 1)
                            {
                                userName = db.ProfilePages.Where(x => x.Id == model.ProfileId).FirstOrDefault().ProfilePageID;
                                FacebookUtility facebookUtil = new FacebookUtility(userName, true);
                                var response = await facebookUtil.getFacebookPageInfoById(model.CustomerProfileHandle);
                                isValidLink = response.ContainsKey("id");
                                if (!isValidLink)
                                {
                                    if (response.ContainsKey("error"))
                                    {
                                        facebookAPIError = (response["error"] as JsonObject)["message"].ToString();
                                    }
                                    else if (response.ContainsKey("APIerror"))
                                    {
                                        facebookAPIError = response["APIerror"].ToString();
                                    }
                                }
                                else
                                {
                                    //Update some info of customer as now isValid = true                                
                                    profileObj.Name = response["name"].ToString();
                                    profileObj.ProfilePic = ((response["picture"] as JsonObject)["data"] as JsonObject)["url"].ToString();
                                    profileObj.Is_Verified = response["is_verified"].ToString() == "true" ? true : false;
                                    profileObj.ExternalId = response["id"].ToString();
                                }

                            }


                            if (!isValidLink && result == "")
                            {
                                result = model.PlatformId == 4 ? "Invalid Twitter Handle" : facebookAPIError;
                            }

                            
                            if (isValidLink)
                            {
                                model.ExternalId = profileObj.ExternalId;
                                customerSocialProfileObj = db.CustomerSocialProfiles.Where(x => x.ExternalId == model.ExternalId).FirstOrDefault();
                                isAlreadyExist = customerSocialProfileObj != null;
                                int customerProfileId = 0;

                                if (!isAlreadyExist)
                                {
                                    profileObj.UserName = model.CustomerProfileHandle;
                                    profileObj.Type = model.PlatformId == 4 ? "Twitter" : "Facebook";
                                    //profileObj.ExternalId = 
                                    var responseCustomerSocialProfile = db.CustomerSocialProfiles.Add(profileObj);
                                    db.SaveChanges();

                                    customerProfileId = responseCustomerSocialProfile.Id;
                                }
                                else
                                {
                                    customerProfileId = customerSocialProfileObj.Id;
                                }

                                bool isMappingExist = false;
                                var mappingObj = new ProfileToCustSocialProfile();
                                if (model.PlatformId == 4)
                                {
                                    mappingObj = db.ProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.ProfileId == model.ProfileId).FirstOrDefault();
                                }
                                else if (model.PlatformId == 1)
                                {
                                    mappingObj = db.ProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.ProfilePageId == model.ProfileId).FirstOrDefault();
                                }

                                isMappingExist = mappingObj != null;
                                int profileCustId = 0;
                                if (!isMappingExist)
                                {
                                    ProfileToCustSocialProfile custProfile = new ProfileToCustSocialProfile();
                                    custProfile.CustSocialProfileId = customerProfileId;
                                    if (model.PlatformId == 4) //Twitter
                                    {
                                        custProfile.ProfileId = model.ProfileId;
                                        custProfile.ProfilePageId = 0;
                                    }
                                    else if (model.PlatformId == 1) //Facebook
                                    {
                                        string profileExternalId = db.ProfilePages.Where(x => x.Id == model.ProfileId).FirstOrDefault().ProfileExternalId.ToString();
                                        custProfile.ProfileId = db.Profiles.Where(x => x.ProfileExternalId == profileExternalId).FirstOrDefault().Id;
                                        custProfile.ProfilePageId = model.ProfileId;
                                    }

                                    var responseMapping = db.ProfileToCustSocialProfiles.Add(custProfile);
                                    db.SaveChanges();

                                    profileCustId = responseMapping.Id;
                                }
                                else
                                {
                                    profileCustId = mappingObj.Id;
                                }

                                bool isProfileAlreadyExist = db.CustomerProfileTypes.Where(x => x.ProfileCustomerId == profileCustId).Count() > 0;
                                bool isValid = !isProfileAlreadyExist;
                                if (isProfileAlreadyExist)
                                {
                                    result = "User is already added";
                                }

                                if (isValid)
                                {
                                    form.ProfileCustomerId = profileCustId;
                                    form.AccountTypeId = model.AccountTypeId;

                                    db.CustomerProfileTypes.Add(form);
                                    var responseId = db.SaveChanges();
                                    result = "success|" + form.Id;
                                }
                            }                           
                        }
                    }
                });
                
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<string> DeleteInfluencer(int Id)
        {
            string result = "";
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var user = db.CustomerProfileTypes.Find(Id);
                        if (user != null)
                        {                            
                            db.CustomerProfileTypes.Remove(user);
                            db.SaveChanges();
                            result = "success";
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}