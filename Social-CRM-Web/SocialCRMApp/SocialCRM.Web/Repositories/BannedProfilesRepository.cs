using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialCRM.Web.Models.Profile;
using System.Threading.Tasks;
using CRM.Entities;
using SocialCRM.Web.Models.Profiles;
using CRM.Common.Enum;
using System.Data.Entity;
using SocialAPIsWrapper.APIs;
using SocialCRM.Web.Models.UnifiedDeck;
using System.Drawing;
using System.IO;

namespace SocialCRM.Web.Repositories
{
    public class BannedProfilesRepository : IBannedProfilesRepository
    {
        public async Task<BannedProfilesListModel> GetWarnBanUsersList(int userId, string reasonIds, string platformIds, string name, string userHandle, int groupId, bool getOptions, int pageNumber, int type)
        {
            BannedProfilesListModel result = new BannedProfilesListModel();
            result.BannedProfilesList = new List<BannedProfiles>();
            result.WarnedProfilesList = new List<BannedProfiles>();
            result.StatusWiseData = new Dictionary<string, int>();
            result.ReasonWiseData = new List<WarnBanReasonReportModel>();

            int pageSize = 5;
            pageNumber = pageNumber * pageSize;
            reasonIds = reasonIds == "0" ? "" : reasonIds;
            platformIds = platformIds == "0" ? "" : platformIds;

            //Seeding Method
            //string temp = await UploadTwitterUsers();

            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        //string resp = await UploadTwitterUsers();

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


                        profiles = profiles.Where(x => x.IsConnected && x.ProfileTypeId == 4).ToList();
                        int[] profileIds = profiles.Select(x => x.ProfileId).ToArray();

                        var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            ProfileId = z.FirstOrDefault().ProfilePage.Id,
                            ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                            ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z.FirstOrDefault().CompanyId,
                            IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                            AccountName = z.FirstOrDefault().Profile.UserName,
                            IsPage = true,
                            UserId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            PageId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            Id = z.FirstOrDefault().ProfilePage.Id
                        }).ToList();

                        profilePages = profilePages.Where(x => x.IsConnected).ToList();
                        int[] pageIds = profilePages.Select(x => x.ProfileId).ToArray();


                        var reasons = new List<int>();
                        if (!String.IsNullOrEmpty(reasonIds))
                            reasons = reasonIds.Split(',').Select(x => Convert.ToInt32(x)).ToList<int>();

                        var platforms = new List<int>();
                        if (!String.IsNullOrEmpty(platformIds))
                            platforms = platformIds.Split(',').Select(x => Convert.ToInt32(x)).ToList<int>();

                        var tempList = new List<WarnBanCustomer>();
                        tempList = db.WarnBanCustomers.ToList();
                        //tempList = db.WarnBanCustomers.Where(x => (platforms.Count == 0 || platforms.Contains(x.PlatformId.Value)) &&
                        //                                            (reasons.Count == 0 || reasons.Contains(x.ReasonId.Value)) &&
                        //                                            (String.IsNullOrEmpty(name) || x.ProfileToCustSocialProfile.CustomerSocialProfile.Name.Contains(userHandle)) &&
                        //                                            (String.IsNullOrEmpty(userHandle) || x.Name.Contains(userHandle)) &&
                        //                                            profileIds.Contains(x.ProfileToCustSocialProfile.ProfileId) &&
                        //                                            (x.ProfileToCustSocialProfile.ProfilePageId == 0 || pageIds.Contains(x.ProfileToCustSocialProfile.ProfilePageId))).ToList();

                        for(int i=0; i < tempList.Count; i++)
                        {
                            tempList[i].PostUrl = GetPostLink(tempList[i].Id);
                        }

                        result.TotalBannedRecords = tempList.Where(t => t.ProfileStatus.StatusName == "Ban").ToList().Count;
                        result.TotalWarnedRecords = tempList.Where(t => t.ProfileStatus.StatusName == "Warn").ToList().Count;

                        // reporting data
                        result.StatusWiseData.Add("Ban", result.TotalBannedRecords);
                        result.StatusWiseData.Add("Warn", result.TotalWarnedRecords);

                        foreach (var reason in db.CustomerWarnBanReasons.ToList())
                        {
                            int count = tempList.Where(t => (t.CustomerWarnBanReason != null) ? t.CustomerWarnBanReason.ReasonDescription == reason.ReasonDescription : false).ToList().Count;
                            result.ReasonWiseData.Add(new WarnBanReasonReportModel
                            {
                                XAxisColumn = reason.ReasonDescription,
                                NumOfRecords = count
                            });
                        }
                        //

                        if (type == 1)
                        {
                            var banList = tempList.Where(t => t.ProfileStatus.StatusName == "Ban").OrderByDescending(x => x.Id).Skip(pageNumber).Take(pageSize).ToList();
                            var warnList = tempList.Where(t => t.ProfileStatus.StatusName == "Warn").OrderByDescending(x => x.Id).Skip(pageNumber).Take(pageSize).ToList();

                            tempList.Clear();
                            tempList.AddRange(banList);
                            tempList.AddRange(warnList);
                        }
                        else if (type == 2) // ban users
                        {
                            tempList = tempList.Where(t => t.ProfileStatus.StatusName == "Ban").OrderByDescending(x => x.Id).Skip(pageNumber).Take(pageSize).ToList();
                        }
                        else if (type == 3) // warn users
                        {
                            tempList = tempList.Where(t => t.ProfileStatus.StatusName == "Warn").OrderByDescending(x => x.Id).Skip(pageNumber).Take(pageSize).ToList();
                        }

                        var attachments = db.WarnBanCustomersAttachments.ToList();

                        foreach (var item in tempList)
                        {
                            BannedProfiles model = new BannedProfiles();
                            model.Id = item.Id;
                            if (item.PlatformId == 1)
                            {
                                model.Name = item.Name;
                                model.UserName = item.ProfileToCustSocialProfile.CustomerSocialProfile.Name;
                            }
                            else
                            {
                                model.Name = item.Name;
                            }
                            model.UserHandle = item.Name;
                            model.Platform = item.ProfileType.Name;
                            model.PlatformId = item.PlatformId.Value;
                            model.ProfileUrl = item.ProfileUrl;
                            model.PostUrl = item.PostUrl;
                            model.Reason = item.CustomerWarnBanReason != null ? item.CustomerWarnBanReason.ReasonDescription : "";
                            model.ReasonId = item.ReasonId.HasValue ? item.ReasonId.Value : 0;
                            model.Details = item.Details;
                            model.BannedBy = item.Team != null ? (item.Team.FirstName + " " + item.Team.LastName) : item.BannedByName;
                            model.Status = item.ProfileStatus.StatusName;
                            model.StatusId = item.StatusId.Value;
                            model.IsAttachmentUploaded = attachments.Where(x => x.CustomerId == item.Id).Count() > 0;
                            if (model.IsAttachmentUploaded)
                            {
                                model.AttachmentName = attachments.Where(x => x.CustomerId == item.Id).FirstOrDefault().FileName;
                            }
                            model.ProfileId = model.PlatformId == 1 ? item.ProfileToCustSocialProfile.ProfilePageId : item.ProfileToCustSocialProfile.ProfileId;

                            if (item.ProfileStatus.StatusName == "Ban")
                                result.BannedProfilesList.Add(model);
                            else
                                result.WarnedProfilesList.Add(model);
                        }


                        if (getOptions)
                        {
                            result.ReasonList = new List<SelectItemList>();
                            db.CustomerWarnBanReasons.ToList().ForEach(delegate (CustomerWarnBanReason obj)
                            {
                                result.ReasonList.Add(new SelectItemList()
                                {
                                    Value = obj.ReasonId,
                                    Text = obj.ReasonDescription,
                                    ProfileType = 0
                                });
                            });
                            result.PlatformList = new List<SelectItemList>();
                            decimal[] profileTypes = { 1, 4 };
                            db.ProfileTypes.Where(x => profileTypes.Contains(x.Id)).ToList().ForEach(delegate (ProfileType obj)
                            {
                                result.PlatformList.Add(new SelectItemList()
                                {
                                    Value = obj.Id,
                                    Text = obj.Name,
                                    ProfileType = 0
                                });
                            });

                            result.StatusList = new List<SelectItemList>();
                            db.ProfileStatuses.ToList().ForEach(delegate (ProfileStatus obj)
                            {
                                result.StatusList.Add(new SelectItemList()
                                {
                                    Value = obj.StatusId,
                                    Text = obj.StatusName,
                                    ProfileType = 0
                                });
                            });

                            //List<ProfileModel> profiles = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfileId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                            //{
                            //    ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            //    ProfileId = z.FirstOrDefault().Profile.Id,
                            //    ProfileName = z.FirstOrDefault().Profile.Name,
                            //    ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                            //    ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                            //    CompanyId = z.FirstOrDefault().CompanyId,
                            //    IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            //    ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                            //    AccountName = z.FirstOrDefault().Profile.UserName,
                            //    IsPage = false,
                            //    UserId = z.FirstOrDefault().Profile.ProfileExternalId,
                            //    Id = z.FirstOrDefault().Profile.Id
                            //}).ToList();

                            //profiles = profiles.Where(x => x.IsConnected && x.ProfileTypeId == 4).ToList();

                            //Profile Pages
                            //var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                            //{
                            //    ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            //    ProfileId = z.FirstOrDefault().ProfilePage.Id,
                            //    ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                            //    ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            //    ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            //    CompanyId = z.FirstOrDefault().CompanyId,
                            //    IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            //    ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                            //    AccountName = z.FirstOrDefault().Profile.UserName,
                            //    IsPage = true,
                            //    UserId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            //    PageId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            //    Id = z.FirstOrDefault().ProfilePage.Id
                            //}).ToList();

                            //profilePages = profilePages.Where(x => x.IsConnected).ToList();

                            result.ProfilesList = new List<SelectItemList>();
                            profiles.ForEach(delegate (ProfileModel obj)
                            {
                                result.ProfilesList.Add(new SelectItemList()
                                {
                                    Value = obj.Id,
                                    Text = obj.ProfileName,
                                    ProfileType = obj.ProfileTypeId
                                });
                            });
                            profilePages.ForEach(delegate (ProfileModel obj)
                            {
                                result.ProfilesList.Add(new SelectItemList()
                                {
                                    Value = obj.Id,
                                    Text = obj.ProfileName,
                                    ProfileType = (int)ProfilesType.Facebook
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

        public string GetPostLink(int id)
        {
            string result = "";

            try
            {
                //await Task.Run(async () =>
                //{
                using (SocialCRMEntities db = new SocialCRMEntities())
                {
                    WarnBanCustomerFeed feed = db.WarnBanCustomerFeeds.Where(x => x.WarnBanId == id).FirstOrDefault();

                    if (feed != null)
                    {
                        if (feed.FeedType == "TP")
                        {
                            var TP = db.PostsTwitters.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                            result = string.Format("https://www.twitter.com/statuses/{0}", TP.StatusId);
                        }
                        else if (feed.FeedType == "TM")
                        {
                            var TM = db.MessagesTwitters.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                            result = string.Format("https://twitter.com/messages/{0}-{1}", TM.SenderId, TM.RecipientId);
                        }
                        else if (feed.FeedType == "FCP")
                        {
                            var FCP = db.PageChatsFBs.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                            result = string.Format("https://www.facebook.com/{0}/messages/?threadid={1}", FCP.PageId, FCP.ConId.Split('_')[1]);
                        }
                        else if (feed.FeedType == "FP" || feed.FeedType == "FC")
                        {
                            var FP = db.PostsFBs.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                            result = string.Format("https://www.facebook.com/{0}/posts/{1}", FP.PageId, FP.PostId.Split('_')[1]);
                        }
                    }
                }

               // });
            }
            catch (Exception ex)
            { }

            return result;
        }

        public async Task<string> SaveWarnBanUser(BannedProfilesSaveModel model, int userId)
        {
            string result = "";
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var form = new WarnBanCustomer();
                        if (model.Id > 0)
                        {
                            form = db.WarnBanCustomers.Find(model.Id);

                            form.Details = model.Details;
                            form.ProfileUrl = model.ProfileUrl;
                            form.ReasonId = model.ReasonId;
                            form.StatusId = model.StatusId;
                            int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == model.CompanyId).Select(z => z.Id).FirstOrDefault();
                            form.BannedById = teamMemberId;

                            //if (model.Attachment != null)
                            //{
                            //    if (!String.IsNullOrEmpty(model.Attachment.FileName))
                            //    {
                            //        var attachmentItem = db.WarnBanCustomersAttachments.Find(model.Id);
                            //        if (attachmentItem != null)
                            //        {
                            //            db.WarnBanCustomersAttachments.Remove(attachmentItem);
                            //            db.SaveChanges();
                            //        }
                            //    }
                            //}

                            db.WarnBanCustomers.Attach(form);
                            db.Entry(form).State = EntityState.Modified;

                            var responseId = db.SaveChanges();

                            if (model.Attachment != null)
                            {
                                if (!String.IsNullOrEmpty(model.Attachment.FileName))
                                {
                                    var attachmentItem = db.WarnBanCustomersAttachments.Find(model.Id);
                                    if (attachmentItem != null)
                                    {
                                        db.WarnBanCustomersAttachments.Remove(attachmentItem);
                                        db.SaveChanges();
                                    }
                                    if (model.Attachment.Base64String.Contains(','))
                                    {
                                        model.Attachment.Base64String = model.Attachment.Base64String.Split(',')[1];
                                    }
                                    WarnBanCustomersAttachment attachmentObj = new WarnBanCustomersAttachment();
                                    byte[] byteArr = Convert.FromBase64String(model.Attachment.Base64String);
                                    attachmentObj.CustomerId = form.Id;
                                    attachmentObj.FileContent = byteArr;
                                    attachmentObj.FileName = model.Attachment.FileName;
                                    attachmentObj.ContentType = model.Attachment.ContentType;

                                    db.WarnBanCustomersAttachments.Add(attachmentObj);
                                    db.SaveChanges();
                                }
                            }
                            result = "success|" + form.Id;
                        }
                        else
                        {
                            CustomerSocialProfile profileObj = new CustomerSocialProfile();
                            bool isValidLink = new bool();
                            if (model.PlatformId == 4)
                            {
                                var userName = db.Profiles.Where(x => x.Id == model.Profile).FirstOrDefault().UserName;
                                TwitterUtility twitterUtil = new TwitterUtility(userName);
                                var response = await twitterUtil.getTwitterProfile(model.Name);
                                isValidLink = !String.IsNullOrEmpty(response.ScreenName);
                                if (!isValidLink && result == "")
                                {
                                    result = "Invalid Twitter Handle";
                                }
                                if (!String.IsNullOrEmpty(response.ScreenName))
                                {
                                    profileObj.Followers = response.FollowersCount;
                                    profileObj.Is_Verified = response.Verified;
                                    profileObj.ProfilePic = response.ProfileImageUrlHttps;
                                    profileObj.Name = response.Name;
                                    profileObj.ExternalId = Convert.ToString(response.UserID);
                                    //db.CustomerSocialProfiles.Attach(profileObj);
                                    //db.Entry(profileObj).State = EntityState.Modified;
                                    //db.SaveChanges();
                                }
                            }

                            if (isValidLink)
                            {
                                bool isAlreadyExist = false;
                                var customerSocialProfileObj = new CustomerSocialProfile();
                                if (model.PlatformId == 1)
                                {
                                    customerSocialProfileObj = db.CustomerSocialProfiles.Where(x => x.ExternalId == model.Name).FirstOrDefault();
                                    isAlreadyExist = customerSocialProfileObj != null;
                                }
                                else
                                {
                                    customerSocialProfileObj = db.CustomerSocialProfiles.Where(x => x.UserName == model.Name).FirstOrDefault();
                                    isAlreadyExist = customerSocialProfileObj != null;
                                }
                                int customerProfileId = 0;

                                if (!isAlreadyExist)
                                {

                                    if (model.PlatformId == 1)
                                        profileObj.ExternalId = model.Name;
                                    else
                                        profileObj.UserName = model.Name;
                                    profileObj.Type = model.PlatformId == 1 ? "Facebook" : "Twitter";
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
                                if (model.PlatformId == 1)
                                {
                                    mappingObj = db.ProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.ProfilePageId == model.ProfilePage).FirstOrDefault();
                                    isMappingExist = mappingObj != null;
                                }
                                else
                                {
                                    mappingObj = db.ProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.ProfileId == model.Profile).FirstOrDefault();
                                    isMappingExist = mappingObj != null;
                                }
                                int profileCustId = 0;
                                if (!isMappingExist)
                                {
                                    if (model.ProfilePage > 0)
                                    {
                                        model.Profile = db.ProfilePages.Find(model.ProfilePage).Profile.Id;
                                    }
                                    ProfileToCustSocialProfile custProfile = new ProfileToCustSocialProfile();
                                    custProfile.CustSocialProfileId = customerProfileId;
                                    custProfile.ProfileId = model.Profile;
                                    custProfile.ProfilePageId = model.ProfilePage;
                                    var responseMapping = db.ProfileToCustSocialProfiles.Add(custProfile);
                                    db.SaveChanges();

                                    profileCustId = responseMapping.Id;
                                }
                                else
                                {
                                    profileCustId = mappingObj.Id;
                                }

                                bool isAlreadyBanned = db.WarnBanCustomers.Where(x => x.ProfileCustomerId == profileCustId && x.StatusId == 2).Count() > 0;
                                bool isValid = false;
                                if (model.StatusId == 1)
                                {
                                    isValid = db.WarnBanCustomers.Where(x => x.ProfileCustomerId == profileCustId).Count() == 0;
                                    if (!isValid)
                                    {
                                        result = "User is already Warned or Banned";
                                    }
                                }
                                else
                                {
                                    isValid = !isAlreadyBanned;
                                    if (!isValid)
                                    {
                                        result = "User is already Banned";
                                    }
                                }

                                if (isValid)
                                {
                                    int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == model.CompanyId).Select(z => z.Id).FirstOrDefault();
                                    form.ProfileCustomerId = profileCustId;
                                    form.BannedById = teamMemberId;
                                    form.Name = model.Name;
                                    form.CreatedDate = DateTime.Now;
                                    form.PlatformId = model.PlatformId;
                                    form.StatusId = model.StatusId;

                                    form.Details = model.Details;
                                    form.ProfileUrl = model.ProfileUrl;
                                    form.ReasonId = model.ReasonId;

                                    db.WarnBanCustomers.Add(form);
                                    var responseId = db.SaveChanges();

                                    if (model.Attachment != null)
                                    {
                                        if (!String.IsNullOrEmpty(model.Attachment.FileName))
                                        {
                                            if (model.Attachment.Base64String.Contains(','))
                                            {
                                                model.Attachment.Base64String = model.Attachment.Base64String.Split(',')[1];
                                            }
                                            WarnBanCustomersAttachment attachmentObj = new WarnBanCustomersAttachment();
                                            byte[] byteArr = Convert.FromBase64String(model.Attachment.Base64String);
                                            attachmentObj.CustomerId = form.Id;
                                            attachmentObj.FileContent = byteArr;
                                            attachmentObj.FileName = model.Attachment.FileName;
                                            attachmentObj.ContentType = model.Attachment.ContentType;

                                            db.WarnBanCustomersAttachments.Add(attachmentObj);
                                            db.SaveChanges();
                                        }
                                    }
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

        public async Task<bool> DeleteUser(int id)
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var user = db.WarnBanCustomers.Find(id);
                        if (user != null)
                        {
                            if (user.WarnBanCustomersAttachment != null)
                            {
                                db.WarnBanCustomersAttachments.Remove(user.WarnBanCustomersAttachment);
                                db.SaveChanges();
                            }

                            db.WarnBanCustomerFeeds.Remove(user.WarnBanCustomerFeed);
                            db.SaveChanges();

                            db.WarnBanCustomers.Remove(user);
                            db.SaveChanges();
                            result = true;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<string> BanUser(int id)
        {
            string result = "";
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var warnBanUser = db.WarnBanCustomers.Find(id);
                        if (warnBanUser != null)
                        {
                            var profileMappingObj = warnBanUser.ProfileToCustSocialProfile;
                            if (profileMappingObj != null)
                            {
                                bool isPage = profileMappingObj.ProfilePageId > 0;
                                if (isPage)
                                {
                                    var profilePageId = db.ProfilePages.ToList().Where(x => x.Id == profileMappingObj.ProfilePageId).FirstOrDefault().ProfilePageID;
                                    List<string> userIds = new List<string>();
                                    userIds.Add(profileMappingObj.CustomerSocialProfile.ExternalId);
                                    FacebookUtility fbUtility = new FacebookUtility(profilePageId, true);
                                    var response = await fbUtility.blockUsers(profilePageId, null, userIds);
                                    if (response.ContainsKey(profileMappingObj.CustomerSocialProfile.ExternalId))
                                    {
                                        result = Convert.ToString(response[profileMappingObj.CustomerSocialProfile.ExternalId]) == "True" ? "success" : "Ban Failed";
                                    }
                                }
                                else
                                {
                                    TwitterUtility twUtility = new TwitterUtility(profileMappingObj.Profile.UserName);
                                    var response = await twUtility.blockUser(profileMappingObj.CustomerSocialProfile.UserName);
                                    if (response != null)
                                    {
                                        result = "success";
                                    }
                                }
                                if (result == "success")
                                {
                                    var user = profileMappingObj.WarnBanCustomers.ToList().FirstOrDefault();
                                    user.StatusId = 2;
                                    db.WarnBanCustomers.Attach(user);
                                    db.Entry(user).State = EntityState.Modified;
                                    db.SaveChanges();
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

        public async Task<string> UnBanUser(int id)
        {
            string result = "";
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var warnBanUser = db.WarnBanCustomers.Find(id);
                        if (warnBanUser != null)
                        {
                            var profileMappingObj = warnBanUser.ProfileToCustSocialProfile;
                            if (profileMappingObj != null)
                            {
                                bool isPage = profileMappingObj.ProfilePageId > 0;
                                if (isPage)
                                {
                                    var profilePageId = db.ProfilePages.ToList().Where(x => x.Id == profileMappingObj.ProfilePageId).FirstOrDefault().ProfilePageID;
                                    FacebookUtility fbUtility = new FacebookUtility(profilePageId, true);
                                    var response = await fbUtility.unblockUser(profilePageId, null, profileMappingObj.CustomerSocialProfile.ExternalId);
                                    if (response.ContainsKey("success"))
                                    {
                                        result = Convert.ToString(response["success"]) == "True" ? "success" : "Unban Failed";
                                    }
                                }
                                else
                                {
                                    TwitterUtility twUtility = new TwitterUtility(profileMappingObj.Profile.UserName);
                                    var response = await twUtility.unBlockUser(profileMappingObj.CustomerSocialProfile.UserName);
                                    if (response != null)
                                    {
                                        result = "success";
                                    }
                                }
                                if (result == "success")
                                {
                                    var customer = profileMappingObj.WarnBanCustomers.FirstOrDefault();
                                    if (customer != null)
                                    {
                                        if (customer.WarnBanCustomersAttachment != null)
                                        {
                                            db.WarnBanCustomersAttachments.Remove(customer.WarnBanCustomersAttachment);
                                            db.SaveChanges();
                                        }
                                        db.WarnBanCustomerFeeds.Remove(customer.WarnBanCustomerFeed);
                                        db.SaveChanges();

                                        db.WarnBanCustomers.Remove(customer);
                                        db.SaveChanges();
                                    }
                                    db.Entry(profileMappingObj).State = EntityState.Modified;
                                    db.SaveChanges();
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

        private async Task<Dictionary<string, string>> BanFbUser(string fbId, List<int> pageIds)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var profilePages = db.ProfilePages.ToList();
                        List<string> fbIds = new List<string>();
                        fbIds.Add(fbId);
                        foreach (var page in pageIds)
                        {
                            var pageId = profilePages.Where(x => x.Id == page).FirstOrDefault().ProfilePageID;
                            FacebookUtility fbUtility = new FacebookUtility(pageId, true);
                            var response = await fbUtility.blockUsers(pageId, null, fbIds);
                            if (response.ContainsKey("success"))
                            {
                                result.Add(pageId, Convert.ToString(response["success"]));
                            }
                            else
                            {
                                result.Add(pageId, "False");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result.Add("Exception", ex.Message);
            }
            return result;
        }

        private async Task<Dictionary<string, bool>> BanTwitterUser(string twScreenName, List<int> profileIds)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var profiles = db.Profiles.ToList();
                        foreach (var profileId in profileIds)
                        {
                            string userName = profiles.Where(x => x.Id == profileId).FirstOrDefault().UserName;
                            TwitterUtility twUtility = new TwitterUtility(userName);
                            var response = await twUtility.blockUser(twScreenName);
                            result.Add(userName, response != null);
                        }
                        return result;
                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private async Task<Dictionary<string, string>> UnBanFbUser(List<int> pageIds, string fbId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var profilePages = db.ProfilePages.ToList();
                        foreach (var page in pageIds)
                        {
                            var pageId = profilePages.Where(x => x.Id == page).FirstOrDefault().ProfilePageID;
                            FacebookUtility fbUtility = new FacebookUtility(pageId, true);
                            var response = await fbUtility.unblockUser(pageId, null, fbId);
                            if (response.ContainsKey("success"))
                            {
                                result.Add(pageId, Convert.ToString(response["success"]));
                            }
                            else
                            {
                                result.Add(pageId, "False");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result.Add("Exception", ex.Message);
            }
            return result;
        }

        private async Task<Dictionary<string, bool>> UnBanTwitterUser(string twScreenName, List<int> profileIds)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var profiles = db.Profiles.ToList();
                        foreach (var profileId in profileIds)
                        {
                            string userName = profiles.Where(x => x.Id == profileId).FirstOrDefault().UserName;
                            TwitterUtility twUtility = new TwitterUtility(userName);
                            var response = await twUtility.unBlockUser(twScreenName);
                            result.Add(userName, response != null);
                        }
                        return result;
                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public async Task<BannedProfilesOptionsListModel> GetBannedProfilesOptions(int userId, int groupId)
        {
            BannedProfilesOptionsListModel result = new BannedProfilesOptionsListModel();
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        result.ReasonList = new List<SelectItemList>();
                        db.CustomerWarnBanReasons.ToList().ForEach(delegate (CustomerWarnBanReason obj)
                        {
                            result.ReasonList.Add(new SelectItemList()
                            {
                                Value = obj.ReasonId,
                                Text = obj.ReasonDescription,
                                ProfileType = 0
                            });
                        });
                        result.PlatformList = new List<SelectItemList>();
                        decimal[] profileTypes = { 1, 4 };
                        db.ProfileTypes.Where(x => profileTypes.Contains(x.Id)).ToList().ForEach(delegate (ProfileType obj)
                        {
                            result.PlatformList.Add(new SelectItemList()
                            {
                                Value = obj.Id,
                                Text = obj.Name,
                                ProfileType = 0
                            });
                        });

                        result.StatusList = new List<SelectItemList>();
                        db.ProfileStatuses.ToList().ForEach(delegate (ProfileStatus obj)
                        {
                            result.StatusList.Add(new SelectItemList()
                            {
                                Value = obj.StatusId,
                                Text = obj.StatusName,
                                ProfileType = 0
                            });
                        });

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

                        profiles = profiles.Where(x => x.IsConnected && x.ProfileTypeId == 4).ToList();

                        //Profile Pages
                        var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            ProfileId = z.FirstOrDefault().ProfilePage.Id,
                            ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                            ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z.FirstOrDefault().CompanyId,
                            IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                            AccountName = z.FirstOrDefault().Profile.UserName,
                            IsPage = true,
                            UserId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            PageId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            Id = z.FirstOrDefault().ProfilePage.Id
                        }).ToList();

                        profilePages = profilePages.Where(x => x.IsConnected).ToList();

                        result.ProfilesList = new List<SelectItemList>();
                        profiles.ForEach(delegate (ProfileModel obj)
                        {
                            result.ProfilesList.Add(new SelectItemList()
                            {
                                Value = obj.Id,
                                Text = obj.ProfileName,
                                ProfileType = obj.ProfileTypeId
                            });
                        });
                        profilePages.ForEach(delegate (ProfileModel obj)
                        {
                            result.ProfilesList.Add(new SelectItemList()
                            {
                                Value = obj.Id,
                                Text = obj.ProfileName,
                                ProfileType = (int)ProfilesType.Facebook
                            });
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public async Task<string> GetAttachmentAsBase64String(int id)
        {
            string result = "";
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var attachment = db.WarnBanCustomersAttachments.Find(id);
                        if (attachment != null)
                        {
                            byte[] bytes = attachment.FileContent;
                            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                            result = "data:" + attachment.ContentType + ";base64," + base64String;
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        public async Task<List<WarnBanReasonReportModel>> GetWarnBanUserReport(int userId, int groupId, DateTime? fromDate, DateTime? toDate, TimeSpan? fromTime, TimeSpan? toTime)
        {
            List<WarnBanReasonReportModel> result = new List<WarnBanReasonReportModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var tempList = new List<WarnBanCustomer>();
                        tempList = db.WarnBanCustomers.ToList();//.Where(x => 
                                                                // ((fromDate.HasValue) ? (x.CreatedDate.Value.Date >= fromDate.Value.Date) : true) &&
                                                                //((toDate.HasValue) ? (x.CreatedDate.Value.Date <= toDate.Value.Date) : true) &&
                                                                //((fromTime.HasValue) ? (x.CreatedDate.Value.Hour >= fromTime.Value.Hours) : true) &&
                                                                //((toTime.HasValue) ? (x.CreatedDate.Value.Hour <= toTime.Value.Hours) : true)).ToList();

                        for (DateTime date = fromDate.Value; fromDate.Value <= toDate.Value; fromDate = fromDate.Value.AddDays(1))
                        {
                            WarnBanReasonReportModel model = new WarnBanReasonReportModel();
                            model.XAxisColumn = fromDate.Value.ToString("dd/MM/yyyy");
                            model.NumOfRecords = tempList.Where(x => x.CreatedDate.Value.Date == fromDate.Value.Date &&
                                                                      ((fromTime.HasValue) ? (x.CreatedDate.Value.Hour >= fromTime.Value.Hours) : true) &&
                                                                      ((toTime.HasValue) ? (x.CreatedDate.Value.Hour <= toTime.Value.Hours) : true)).ToList().Count;
                            result.Add(model);
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

        public async Task<string> WarnBanUser(ConsumerProfilesSaveModel model, int userId)
        {
            string result = "";
            try
            {
                await Task.Run(async () =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        var form = new WarnBanCustomer();
                        int profileId = 0;
                        int pageId = 0;
                        if (model.StatusId == 2)
                        {
                            result = await BanUser(model.Id.Value);
                            if (result == "success")
                            {
                                var warnBanUser = db.WarnBanCustomers.Find(model.Id.Value);
                                warnBanUser.WarnBanCustomerFeed = new WarnBanCustomerFeed();
                                warnBanUser.WarnBanCustomerFeed.FeedId = model.FeedId;
                                warnBanUser.WarnBanCustomerFeed.FeedType = model.FeedType;
                                warnBanUser.WarnBanCustomerFeed.WarnBanId = model.Id.Value;

                                db.WarnBanCustomerFeeds.Attach(warnBanUser.WarnBanCustomerFeed);
                                db.SaveChanges();

                                db.WarnBanCustomers.Attach(warnBanUser);
                                db.Entry(warnBanUser).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            //if (response == "success")
                            //{
                            //    form = db.WarnBanCustomers.Find(model.Id);

                            //    form.StatusId = model.StatusId;
                            //    form.ReasonId = null;
                            //    int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == model.CompanyId).Select(z => z.Id).FirstOrDefault();
                            //    form.BannedById = teamMemberId;

                            //    db.WarnBanCustomers.Attach(form);
                            //    db.Entry(form).State = EntityState.Modified;
                            //    db.SaveChanges();
                            //    result = "success";
                            //}
                        }
                        else
                        {
                            bool isAlreadyExist = false;
                            var customerSocialProfileObj = new CustomerSocialProfile();
                            if (model.PlatformId == 1)
                            {
                                customerSocialProfileObj = db.CustomerSocialProfiles.Where(x => x.ExternalId == model.Name).FirstOrDefault();
                                isAlreadyExist = customerSocialProfileObj != null;
                            }
                            else
                            {
                                customerSocialProfileObj = db.CustomerSocialProfiles.Where(x => x.UserName == model.Name).FirstOrDefault();
                                isAlreadyExist = customerSocialProfileObj != null;
                            }
                            int customerProfileId = 0;
                            if (!isAlreadyExist)
                            {
                                CustomerSocialProfile profileObj = new CustomerSocialProfile();
                                if (model.PlatformId == 1)
                                    profileObj.ExternalId = model.Name;
                                else
                                    profileObj.UserName = model.Name;
                                profileObj.Type = model.PlatformId == 1 ? "Facebook" : "Twitter";
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

                            if (model.PlatformId == 1)
                            {
                                var page = db.ProfilePages.Where(x => x.ProfilePageID == model.ProfilePage).FirstOrDefault();
                                mappingObj = db.ProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.ProfilePageId == page.Id).FirstOrDefault();
                                isMappingExist = mappingObj != null;
                            }
                            else
                            {
                                mappingObj = db.ProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.Profile.UserName == model.Profile).FirstOrDefault();
                                isMappingExist = mappingObj != null;
                            }
                            int profileCustId = 0;
                            if (!isMappingExist)
                            {
                                if (!String.IsNullOrEmpty(model.ProfilePage))
                                {
                                    var page = db.ProfilePages.Where(x => x.ProfilePageID == model.ProfilePage).FirstOrDefault();
                                    pageId = page.Profile.Id;
                                }
                                else
                                {
                                    var profile = db.Profiles.Where(x => x.UserName == model.Profile).FirstOrDefault();
                                    profileId = profile.Id;
                                }
                                ProfileToCustSocialProfile custProfile = new ProfileToCustSocialProfile();
                                custProfile.CustSocialProfileId = customerProfileId;
                                custProfile.ProfileId = profileId;
                                custProfile.ProfilePageId = pageId;
                                var responseMapping = db.ProfileToCustSocialProfiles.Add(custProfile);
                                db.SaveChanges();

                                profileCustId = responseMapping.Id;
                            }
                            else
                            {
                                profileCustId = mappingObj.Id;
                            }

                            bool isAlreadyBanned = db.WarnBanCustomers.Where(x => x.ProfileCustomerId == profileCustId && x.StatusId == 2).Count() > 0;
                            bool isValid = db.WarnBanCustomers.Where(x => x.ProfileCustomerId == profileCustId).Count() == 0;
                            if (!isValid)
                            {
                                result = "User is already Warned or Banned";
                            }
                            //if (model.StatusId == 1)
                            //{
                            //    isValid = db.WarnBanCustomers.Where(x => x.ProfileCustomerId == profileCustId).Count() == 0;
                            //    if (!isValid)
                            //    {
                            //        result = "User is already Warned or Banned";
                            //    }
                            //}
                            //else
                            //{
                            //    isValid = !isAlreadyBanned;
                            //    if (!isValid)
                            //    {
                            //        result = "User is already Banned";
                            //    }
                            //}

                            if (isValid)
                            {
                                int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == model.CompanyId).Select(z => z.Id).FirstOrDefault();
                                form.ProfileCustomerId = profileCustId;
                                form.BannedById = teamMemberId;
                                form.Name = model.Name;
                                form.CreatedDate = DateTime.Now;
                                form.PlatformId = model.PlatformId;
                                form.StatusId = model.StatusId;

                                form.Details = model.Details;
                                form.ProfileUrl = model.ProfileUrl;
                                form.ReasonId = null;

                                form.WarnBanCustomerFeed = new WarnBanCustomerFeed();
                                form.WarnBanCustomerFeed.FeedId = model.FeedId;
                                form.WarnBanCustomerFeed.FeedType = model.FeedType;

                                db.WarnBanCustomers.Add(form);
                                var responseId = db.SaveChanges();

                                if (model.Attachment != null)
                                {
                                    if (!String.IsNullOrEmpty(model.Attachment.FileName))
                                    {
                                        if (model.Attachment.Base64String.Contains(','))
                                        {
                                            model.Attachment.Base64String = model.Attachment.Base64String.Split(',')[1];
                                        }
                                        WarnBanCustomersAttachment attachmentObj = new WarnBanCustomersAttachment();
                                        byte[] byteArr = Convert.FromBase64String(model.Attachment.Base64String);
                                        attachmentObj.CustomerId = form.Id;
                                        attachmentObj.FileContent = byteArr;
                                        attachmentObj.FileName = model.Attachment.FileName;
                                        attachmentObj.ContentType = model.Attachment.ContentType;

                                        db.WarnBanCustomersAttachments.Add(attachmentObj);
                                        db.SaveChanges();
                                    }
                                }
                                result = "success";
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

        public async Task<string> UploadTwitterUsers_Old()
        {
            string result = "";
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        string saveLocation = "E:\\KE_BannedProfiles\\Twitter.xlsx";
                        int profileId = 1;
                        int statusId = 2;
                        var excelData = new LinqToExcel.ExcelQueryFactory(@saveLocation);
                        string workSheet = "Sheet1";
                        var query = (from c in excelData.Worksheet(workSheet)
                                     select c).ToList();
                        var columnNamesList = excelData.GetColumnNames(workSheet).ToList();
                        var reasonList = db.CustomerWarnBanReasons.ToList();
                        var profileTypes = db.ProfileTypes.ToList();
                        var statusList = db.ProfileStatuses.ToList();
                        List<CustomerSocialProfile> listToAdd = new List<CustomerSocialProfile>();


                        //Getting Column Names
                        var columnNames = query[0];
                        List<string> lstColNames = new List<string>();
                        foreach (var column in columnNames)
                        {
                            lstColNames.Add(column.ToString());
                        }
                        int _bannedUsersIndex = -1;
                        int _reasonIndex = -1;
                        int _detailsIndex = -1;
                        int _ByIndex = -1;

                        if (lstColNames.Contains("Banned"))
                        {
                            _bannedUsersIndex = lstColNames.IndexOf("Banned");
                        }

                        if (lstColNames.Contains("Reason"))
                        {
                            _reasonIndex = lstColNames.IndexOf("Reason");
                        }

                        if (lstColNames.Contains("Details"))
                        {
                            _detailsIndex = lstColNames.IndexOf("Details");
                        }

                        if (lstColNames.Contains("By"))
                        {
                            _ByIndex = lstColNames.IndexOf("By");
                        }

                        query.RemoveAt(0);
                        List<WarnBanCustomer> dbWarnBanCustomers = db.WarnBanCustomers.ToList();
                        List<string> lstDbWarnBanCustomers = dbWarnBanCustomers.Select(x => x.Name).ToList();

                        /*
                        //Temporarily
                        List<WarnBanCustomer> DbWarnBanCustomers = db.WarnBanCustomers.ToList();
                        List<WarnBanCustomersAttachment> tempInserter = new List<WarnBanCustomersAttachment>();
                        int[] prohibitors = new int[] { 142, 143, 144, 145 };
                        */

                        List<CustomerSocialProfile> dbCustomerSocialProfile = db.CustomerSocialProfiles.ToList();
                        List<ProfileToCustSocialProfile> dbProfileToCustSocialProfiles = db.ProfileToCustSocialProfiles.ToList();

                        foreach (var item in query)
                        {

                            if (!String.IsNullOrEmpty(item[_bannedUsersIndex]))
                            {
                                //Check if user already exists or not
                                if (lstDbWarnBanCustomers.Contains(item[_bannedUsersIndex].ToString()))
                                {
                                    //Already exists dont do anything let it go!

                                    /*
                                    //Temporarily                                   
                                    if (File.Exists("E:\\KE_BannedProfiles\\Reasons\\@" + item[_bannedUsersIndex] + ".jpg"))
                                    {
                                        WarnBanCustomersAttachment uploadImage = new WarnBanCustomersAttachment();
                                        uploadImage.FileName = item[_bannedUsersIndex];
                                        uploadImage.ContentType = "image/jpeg";
                                        uploadImage.FileContent = ImageToBinary("E:\\KE_BannedProfiles\\Reasons\\@" + item[_bannedUsersIndex] + ".jpg");
                                        uploadImage.CustomerId = DbWarnBanCustomers.Where(x => x.Name == item[_bannedUsersIndex]).FirstOrDefault().Id;

                                        if(!prohibitors.Contains(uploadImage.CustomerId))
                                            tempInserter.Add(uploadImage);
                                    }
                                    */
                                }
                                else if (!(lstDbWarnBanCustomers.Contains(item[_bannedUsersIndex].ToString())))
                                {
                                    //New Implementation Start
                                    int? customerProfileId = null;
                                    bool isAlreadyExist = false;
                                    CustomerSocialProfile profile = dbCustomerSocialProfile.Where(x => x.UserName == item[_bannedUsersIndex]).FirstOrDefault();
                                    ProfileToCustSocialProfile custProfile = new ProfileToCustSocialProfile();

                                    isAlreadyExist = profile != null;
                                    if (!isAlreadyExist)
                                    {
                                        //create
                                        profile = new CustomerSocialProfile();
                                        profile.UserName = item[_bannedUsersIndex];
                                        profile.Type = "Twitter";

                                        custProfile.ProfileId = profileId;
                                        custProfile.ProfilePageId = 0;
                                    }
                                    else
                                    {
                                        //give id
                                        customerProfileId = profile.Id;
                                    }
                                    //end

                                    //New start
                                    bool isMappingExist = false;
                                    int? profileCustId = null;
                                    if (customerProfileId != null)
                                    {
                                        custProfile = dbProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.Profile.UserName == item[_bannedUsersIndex]).FirstOrDefault();
                                        isMappingExist = custProfile != null;
                                    }

                                    if (!isMappingExist)
                                    {
                                        custProfile = new ProfileToCustSocialProfile();
                                        custProfile.ProfileId = profileId;
                                        custProfile.ProfilePageId = 0;
                                    }
                                    else
                                    {
                                        profileCustId = custProfile.Id;
                                    }


                                    bool isValid = dbWarnBanCustomers.Where(x => x.ProfileCustomerId == profileCustId).Count() == 0;

                                    //New end
                                    if (isValid)
                                    {
                                        WarnBanCustomer banCustomer = new WarnBanCustomer();

                                        if (!String.IsNullOrEmpty(item[_reasonIndex]))
                                        {
                                            int? reasonId = null;
                                            try
                                            {
                                                reasonId = reasonList.Where(x => x.ReasonDescription.ToLower() == item[_reasonIndex].ToString().ToLower()).FirstOrDefault().ReasonId;
                                                if (reasonId != null)
                                                {
                                                    banCustomer.ReasonId = reasonId;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                        banCustomer.Details = item[_detailsIndex];
                                        banCustomer.BannedByName = item[_ByIndex];
                                        banCustomer.PlatformId = 4;
                                        banCustomer.ProfileType = profileTypes.Where(x => x.Id == 4).FirstOrDefault();
                                        banCustomer.Name = item[_bannedUsersIndex];
                                        banCustomer.ProfileUrl = "https://www.twitter.com/" + item[_bannedUsersIndex];
                                        banCustomer.StatusId = statusId;
                                        banCustomer.ProfileStatus = statusList.Where(x => x.StatusId == statusId).FirstOrDefault();
                                        banCustomer.CreatedDate = DateTime.Now;

                                        //If file exists to upload
                                        if (File.Exists("E:\\KE_BannedProfiles\\Reasons\\@" + banCustomer.Name + ".jpg"))
                                        {
                                            WarnBanCustomersAttachment uploadImage = new WarnBanCustomersAttachment();
                                            uploadImage.FileName = banCustomer.Name;
                                            uploadImage.ContentType = "image/jpeg";
                                            uploadImage.FileContent = ImageToBinary("E:\\KE_BannedProfiles\\Reasons\\@" + banCustomer.Name + ".jpg");

                                            banCustomer.WarnBanCustomersAttachment = uploadImage;
                                        }

                                        custProfile.WarnBanCustomers.Add(banCustomer);

                                        profile.ProfileToCustSocialProfiles.Add(custProfile);

                                        listToAdd.Add(profile);

                                        lstDbWarnBanCustomers.Add(item[_bannedUsersIndex].ToString().Trim());
                                    }
                                    else
                                    {
                                        //Already present in our db, its not valid so not pushing it.
                                    }
                                }
                            }
                        }

                        if (listToAdd.Count > 0)
                        {
                            db.CustomerSocialProfiles.AddRange(listToAdd);
                            db.SaveChanges();
                        }

                        /*
                        //Temporarily
                        if(tempInserter.Count> 0)
                        {
                            db.WarnBanCustomersAttachments.AddRange(tempInserter);
                            db.SaveChanges();
                        }
                        */

                    }
                });
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        public static byte[] ImageToBinary(string imagePath)
        {
            FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            return buffer;
        }

        public async Task<string> UploadTwitterUsers()
        {
            string result = "";
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        #region Excel Setting
                        string saveLocation = "E:\\KE_BannedProfiles\\Twitter.xlsx";
                        int profileId = 1;
                        int statusId = 2;
                        var excelData = new LinqToExcel.ExcelQueryFactory(@saveLocation);
                        string workSheet = "Sheet1";
                        var query = (from c in excelData.Worksheet(workSheet)
                                     select c).ToList();
                        var columnNamesList = excelData.GetColumnNames(workSheet).ToList();
                        var reasonList = db.CustomerWarnBanReasons.ToList();
                        var profileTypes = db.ProfileTypes.ToList();
                        var statusList = db.ProfileStatuses.ToList();
                        List<CustomerSocialProfile> listToAdd = new List<CustomerSocialProfile>();

                        //Getting Column Names
                        var columnNames = query[0];
                        List<string> lstColNames = new List<string>();
                        foreach (var column in columnNames)
                        {
                            lstColNames.Add(column.ToString());
                        }
                        int _bannedUsersIndex = -1;
                        int _reasonIndex = -1;
                        int _detailsIndex = -1;
                        int _ByIndex = -1;

                        if (lstColNames.Contains("Banned"))
                        {
                            _bannedUsersIndex = lstColNames.IndexOf("Banned");
                        }

                        if (lstColNames.Contains("Reason"))
                        {
                            _reasonIndex = lstColNames.IndexOf("Reason");
                        }

                        if (lstColNames.Contains("Details"))
                        {
                            _detailsIndex = lstColNames.IndexOf("Details");
                        }

                        if (lstColNames.Contains("By"))
                        {
                            _ByIndex = lstColNames.IndexOf("By");
                        }

                        query.RemoveAt(0);
                        List<WarnBanCustomer> dbWarnBanCustomers = db.WarnBanCustomers.ToList();
                        List<string> lstDbWarnBanCustomers = dbWarnBanCustomers.Select(x => x.Name).ToList();

                        /*
                        //Temporarily
                        List<WarnBanCustomer> DbWarnBanCustomers = db.WarnBanCustomers.ToList();
                        List<WarnBanCustomersAttachment> tempInserter = new List<WarnBanCustomersAttachment>();
                        int[] prohibitors = new int[] { 142, 143, 144, 145 };
                        */

                        #endregion

                        List<CustomerSocialProfile> dbCustomerSocialProfile = db.CustomerSocialProfiles.ToList();
                        List<ProfileToCustSocialProfile> dbProfileToCustSocialProfiles = db.ProfileToCustSocialProfiles.ToList();

                        foreach (var item in query)
                        {

                            if (!String.IsNullOrEmpty(item[_bannedUsersIndex]))
                            {
                                //Check if user already exists or not
                                if (lstDbWarnBanCustomers.Contains(item[_bannedUsersIndex].ToString()))
                                {
                                    #region Temp
                                    //Already exists dont do anything let it go!

                                    /*
                                    //Temporarily                                   
                                    if (File.Exists("E:\\KE_BannedProfiles\\Reasons\\@" + item[_bannedUsersIndex] + ".jpg"))
                                    {
                                        WarnBanCustomersAttachment uploadImage = new WarnBanCustomersAttachment();
                                        uploadImage.FileName = item[_bannedUsersIndex];
                                        uploadImage.ContentType = "image/jpeg";
                                        uploadImage.FileContent = ImageToBinary("E:\\KE_BannedProfiles\\Reasons\\@" + item[_bannedUsersIndex] + ".jpg");
                                        uploadImage.CustomerId = DbWarnBanCustomers.Where(x => x.Name == item[_bannedUsersIndex]).FirstOrDefault().Id;

                                        if(!prohibitors.Contains(uploadImage.CustomerId))
                                            tempInserter.Add(uploadImage);
                                    }
                                    */
                                    #endregion
                                }
                                else if (!(lstDbWarnBanCustomers.Contains(item[_bannedUsersIndex].ToString())))
                                {
                                    #region check

                                    bool isAlreadyExist = false;
                                    var customerSocialProfileObj = new CustomerSocialProfile();

                                    customerSocialProfileObj = dbCustomerSocialProfile.Where(x => x.UserName == item[_bannedUsersIndex].ToString()).FirstOrDefault();
                                    isAlreadyExist = customerSocialProfileObj != null;

                                    int customerProfileId = 0;
                                    if (!isAlreadyExist)
                                    {
                                        CustomerSocialProfile profileObj = new CustomerSocialProfile();
                                        profileObj.UserName = item[_bannedUsersIndex].ToString();
                                        profileObj.Type = "Twitter";
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

                                    mappingObj = dbProfileToCustSocialProfiles.Where(x => x.CustSocialProfileId == customerProfileId && x.Profile.UserName == "AghaRameezAhmed").FirstOrDefault();
                                    isMappingExist = mappingObj != null;

                                    int profileCustId = 0;
                                    if (!isMappingExist)
                                    {
                                        ProfileToCustSocialProfile custProfile = new ProfileToCustSocialProfile();
                                        custProfile.CustSocialProfileId = customerProfileId;
                                        custProfile.ProfileId = profileId;
                                        custProfile.ProfilePageId = 0;
                                        var responseMapping = db.ProfileToCustSocialProfiles.Add(custProfile);
                                        db.SaveChanges();

                                        profileCustId = responseMapping.Id;
                                    }
                                    else
                                    {
                                        profileCustId = mappingObj.Id;
                                    }

                                    bool isValid = db.WarnBanCustomers.Where(x => x.ProfileCustomerId == profileCustId).Count() == 0;
                                    #endregion

                                    if (isValid)
                                    {
                                        WarnBanCustomer banCustomer = new WarnBanCustomer();

                                        if (!String.IsNullOrEmpty(item[_reasonIndex]))
                                        {
                                            int? reasonId = null;
                                            try
                                            {
                                                reasonId = reasonList.Where(x => x.ReasonDescription.ToLower() == item[_reasonIndex].ToString().ToLower()).FirstOrDefault().ReasonId;
                                                if (reasonId != null)
                                                {
                                                    banCustomer.ReasonId = reasonId;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                        banCustomer.ProfileCustomerId = profileCustId;
                                        banCustomer.Details = item[_detailsIndex];
                                        banCustomer.BannedByName = item[_ByIndex];
                                        banCustomer.PlatformId = 4;
                                        banCustomer.ProfileType = profileTypes.Where(x => x.Id == 4).FirstOrDefault();
                                        banCustomer.Name = item[_bannedUsersIndex];
                                        banCustomer.ProfileUrl = "https://www.twitter.com/" + item[_bannedUsersIndex];
                                        banCustomer.StatusId = statusId;
                                        banCustomer.ProfileStatus = statusList.Where(x => x.StatusId == statusId).FirstOrDefault();
                                        banCustomer.CreatedDate = DateTime.Now;

                                        db.WarnBanCustomers.Add(banCustomer);
                                        var responseId = db.SaveChanges();
                                        lstDbWarnBanCustomers.Add(item[_bannedUsersIndex].ToString().Trim());

                                        //If file exists to upload
                                        if (File.Exists("E:\\KE_BannedProfiles\\Reasons\\@" + banCustomer.Name + ".jpg"))
                                        {
                                            WarnBanCustomersAttachment uploadImage = new WarnBanCustomersAttachment();
                                            uploadImage.FileName = banCustomer.Name;
                                            uploadImage.ContentType = "image/jpeg";
                                            uploadImage.FileContent = ImageToBinary("E:\\KE_BannedProfiles\\Reasons\\@" + banCustomer.Name + ".jpg");
                                            uploadImage.CustomerId = banCustomer.Id;

                                            db.WarnBanCustomersAttachments.Add(uploadImage);
                                            db.SaveChanges();
                                        }
                                        result = "success";
                                    }

                                }
                            }
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }
    }
}