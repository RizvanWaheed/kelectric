using CRM.Common.Extensions;
using CRM.Common.Enum;
using CRM.Entities;
using SocialCRM.Web.Models.Group;
using SocialCRM.Web.Models.UnifiedInbox;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace SocialCRM.Web.Repositories
{
    public class MiningRepository : IMiningRepository
    {
        public async Task<List<PostsFB>> GetFBPosts()
        {
            List<PostsFB> result = new List<PostsFB>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.PostsFBs.ToList();

                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<CommentsFB>> GetFBComments()
        {
            List<CommentsFB> result = new List<CommentsFB>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.CommentsFBs.ToList();

                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<PostsTwitter>> GetTwitterTweets()
        {
            List<PostsTwitter> result = new List<PostsTwitter>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.PostsTwitters.ToList();

                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<MessagesTwitter>> GetTwitterMsgs()
        {
            List<MessagesTwitter> result = new List<MessagesTwitter>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.MessagesTwitters.ToList();

                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<List<InboxItemModel>> GetUnifiedInboxForUserSelectedProfiles(int pageType, int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string brandTypeData, string sentTypeData, int pageNumber, int pageSize, string teamData, int groupId, int companyId, bool isAllTask, string consumerIds, bool showComplaints, string authorName, string authorHandle, DateTime startDate, DateTime endDate)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                       // int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();
                        var response = context.UnifiedInboxOfSelectedProfilesProcedure(pageType, profileData, profilePageData, msgTypeData, tagTypeData, brandTypeData, sentTypeData, pageNumber, pageSize, teamData, groupId, authorName, authorHandle, startDate, endDate);  //, teamMemberId
                        foreach (var data in response)
                        {
                            string type = "";
                            string activityType = "";
                            if (data.ActivityType == "TP" || data.ActivityType == "STP")
                            {
                                type = "t_ico16";
                                if (Convert.ToBoolean(data.IsRetweet))
                                {
                                    activityType = "retweeted";
                                }
                                else if (data.ToUser != null)
                                {
                                    activityType = "replied to ";
                                }
                                else
                                    activityType = "sent a tweet";
                            }
                            else if (data.ActivityType == "TM")
                            {
                                type = "t_ico16";
                                activityType = "sent a direct message to ";
                            }
                            else if (data.ActivityType == "FP")
                            {
                                type = "fb_ico16";
                                activityType = "wall post | ";
                            }
                            else if (data.ActivityType == "FC")
                            {
                                type = "fb_ico16";
                                activityType = "commented on a post of ";
                            }
                            else if (data.ActivityType == "FCP")
                            {
                                type = "fb_ico16";
                                activityType = "sent a direct message to ";
                            }
                            else if (data.ActivityType == "IM")
                            {
                                type = "insta_ico16";
                                activityType = "wall post | ";
                            }
                            else if (data.ActivityType == "IC")
                            {
                                type = "insta_ico16";
                                activityType = "commented on a post of ";
                            }

                            var item = new InboxItemModel
                            {
                                Id = data.Id,
                                UserId = data.UserId,
                                ClassName = "unread_msg",
                                TwtImg = data.ProfilePic,
                                IsVerified = Convert.ToBoolean(data.IsVerified),
                                User = data.UserName,
                                UserTwit = data.TextValue,
                                ReplyedBy = data.FromUser,
                                ReplyedByFullName = data.FromUserFullName,
                                ReplyedByUserId = data.FromUserId,
                                ToUser = data.ToUser,
                                ToUserFullName = data.ToUserFullName,
                                ToUserId = data.ToUserId,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)data.CreatedDate),
                                ActivityType = activityType,
                                AccountTypeImg = type,
                                MessageType = data.ActivityType,
                                IsArchived = Convert.ToBoolean(data.IsArchived),
                                StatusId = data.StatusId,
                                IsPage = Convert.ToBoolean(data.IsPage),
                                IsTaskCreated = 0,
                                IsTaskCompleted = false,
                                IsRetweet = Convert.ToBoolean(data.IsRetweet),
                                Tags = (data.Keywords == null) ? new List<string>() : data.Keywords.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Urls = String.IsNullOrEmpty(data.Attachments) ? new List<string>() : data.Attachments.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Sentiment = data.Sentiment,
                                IsWarned = false,
                                CompletedBy = data.CompletedBy,
                                CompletedDate = Convert.ToDateTime(data.CompletedDate).ToString("dd/MM/yyyy HH:mm tt"),
                                ParentId=string.IsNullOrWhiteSpace(data.ParentId)?string.Empty:data.ParentId
                            };
                            UnifiedInboxUtility.CheckForAttachments(ref item);
                            result.Add(item);
                        }
                    }
                });

                result = result.GroupBy(g => new { g.StatusId, g.MessageType }).Select(g => g.First()).ToList();
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        
        public async Task<List<InboxItemModel>> GetFeedsById(int ActivityId, string ActivityType)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {

                    foreach (var data in context.GetFeedsById(ActivityId, ActivityType))
                    {
                        string type = "";
                        string activityType = "";
                        if (data.ActivityType == "TP")
                        {
                            type = "t_ico16";
                            if (Convert.ToBoolean(data.IsRetweet))
                            {
                                activityType = "retweeted | ";
                            }
                            else if (data.ToUser != null)
                            {
                                activityType = "replied to ";
                            }
                            else
                                activityType = "sent a tweet | ";
                        }
                        else if (data.ActivityType == "TM")
                        {
                            type = "t_ico16";
                            activityType = "sent a direct message to ";
                        }
                        else if (data.ActivityType == "FP")
                        {
                            type = "fb_ico16";
                            activityType = "wall post | ";
                        }
                        else if (data.ActivityType == "FC")
                        {
                            type = "fb_ico16";
                            activityType = "commented on a post of ";
                        }
                        else if (data.ActivityType == "FCP")
                        {
                            type = "fb_ico16";
                            activityType = "sent a direct message to ";
                        }
                        else if (data.ActivityType == "IM")
                        {
                            type = "insta_ico16";
                            activityType = "wall post | ";
                        }
                        else if (data.ActivityType == "IC")
                        {
                            type = "insta_ico16";
                            activityType = "commented on a post of ";
                        }
                        /*var query = (from e in context.ArchivedUnifiedInboxes
                                     where e.TeamMemberId == teamMemberId &&
                                            e.MsgId == data.Id &&
                                            e.MsgType == data.ActivityType &&
                                            e.GroupId == groupId
                                     select e);
                        bool isArchived;
                        if (query.ToList<ArchivedUnifiedInbox>().Count() == 0)
                        {
                            isArchived = false;
                        }
                        else
                        {
                            isArchived = true;
                        }*/

                        result.Add(new InboxItemModel
                        {
                            Id = data.Id,
                            UserId = data.UserId,
                            ClassName = "unread_msg",
                            TwtImg = data.ProfilePic,
                            User = data.UserName,
                            UserTwit = data.TextValue,
                            ReplyedBy = data.FromUser,
                            ToUser = data.ToUser,
                            ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                            ActivityType = activityType,
                            AccountTypeImg = type,
                            MessageType = data.ActivityType,
                            // IsArchived = Convert.ToBoolean(data.IsArchived),
                            StatusId = data.StatusId,
                            IsRetweet = Convert.ToBoolean(data.IsRetweet),
                            IsPage = Convert.ToBoolean(data.IsPage)
                        });
                    }
                }
            });
            return result;
        }
        public async Task<bool> AddMsgToArchiveList(MsgArchiveModel model)
        {
            var status = true;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    try
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == model.UserId && x.CompanyId == model.CompanyId).Select(z => z.Id).FirstOrDefault();

                        ArchivedUnifiedInbox archiveModel = new ArchivedUnifiedInbox();
                        archiveModel.GroupId = model.GroupId;
                        archiveModel.MsgId = model.MsgId;
                        archiveModel.MsgType = model.MsgType;
                        archiveModel.TeamMemberId = teamMemberId;

                        context.ArchivedUnifiedInboxes.Add(archiveModel);
                        context.SaveChanges();
                    }
                    catch (Exception exp)
                    {
                        status = false;
                    }
                }
            });
            return status;
        }

        public async Task<bool> RemoveMsgFromArchiveList(MsgArchiveModel model)
        {
            var status = true;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    try
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == model.UserId && x.CompanyId == model.CompanyId).Select(z => z.Id).FirstOrDefault();

                        var query = (from e in context.ArchivedUnifiedInboxes
                                     where e.TeamMemberId == teamMemberId &&
                                            e.MsgId == model.MsgId &&
                                            e.MsgType == model.MsgType &&
                                            e.GroupId == model.GroupId
                                     select e);
                        ArchivedUnifiedInbox msgToDelete = query.ToList<ArchivedUnifiedInbox>().FirstOrDefault();

                        context.ArchivedUnifiedInboxes.Remove(msgToDelete);
                        context.SaveChanges();
                    }
                    catch (Exception exp)
                    {
                        status = false;
                    }
                }
            });
            return status;
        }

        //public async Task<List<SelectListItem>> GetCustomerSocialProfiles(string userIds, string commaSeperatedPages, int GroupId)
        //{
        //    List<SelectListItem> result = new List<SelectListItem>();
        //    await Task.Run(() =>
        //    {
        //        using (var context = new SocialCRMEntities())
        //        {
        //            //User List for FB and Twitter
        //            int[] idList = Array.ConvertAll(userIds.Split(','), int.Parse);
        //            var userList = context.Profiles.Where(x => idList.Contains(x.Id)).ToList();
        //            var userIdList = userList.Select(y => y.ProfileExternalId).ToArray();

        //            //Page list for FB
        //            int[] pgIdList = Array.ConvertAll(commaSeperatedPages.Split(','), int.Parse);
        //            var pageList = context.ProfilePages.Where(x => pgIdList.Contains(x.Id)).ToList();
        //            var pageIdList = pageList.Select(y => y.ProfilePageID).ToArray();


        //            var profileList = context.CustomerSocialProfiles.ToList();

        //            //result = profileList.Where(x => userIdList.Contains(x.UserId)).Select( x => new SelectListItem
        //            //{
        //            //    Text = userList.Where(y => y.ProfileExternalId == x.UserId).FirstOrDefault().Name + "-" + x.CustomerProfileName,
        //            //    Value = Convert.ToString(x.CustomerProfileId)
        //            //}).GroupBy(g => new { g.Text, g.Value }).Select(g => g.First()).ToList();

        //            result = profileList.Where(x => userIdList.Contains(x.UserId)).Select(x => new SelectListItem
        //            {
        //                Text = x.CustomerProfileName,
        //                Value = x.CustomerProfileId
        //            }).GroupBy(g => new { g.Value, g.Text }).Select(g => g.First()).ToList();

        //            var pageResult = profileList.Where(x => pageIdList.Contains(x.UserId)).Select(x => new SelectListItem
        //            {
        //                Text = pageList.Where(y => y.ProfilePageID == x.UserId).FirstOrDefault().PageName + "-" + x.CustomerProfileName,
        //                Value = Convert.ToString(x.CustomerProfileId)
        //            }).GroupBy(g => new { g.Text, g.Value }).Select(g => g.First()).ToList();

        //            result.AddRange(pageResult);
        //        }
        //    });
        //    return result;
        //}

        public async Task<List<SelectListItem>> GetCustomerSocialProfiles(string userIds, string commaSeperatedPages, int groupId)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    /*result = context.GetSocialProfilesWithComplaints(userIds, commaSeperatedPages, groupId).Select(r => new SelectListItem
                    {
                        Value = r.CustomerProfileId,
                        Text = r.CustomerProfileName
                    }).ToList();*/
                }
            });
            return result;
        }

        public async Task<int> AddSocialActivity(string ProfileId, string ProfilePageId, string FeedId, string FeedType, int ActivityTypeId, string RetweetedStatusId, string Comment, int UserId, int companyId, string fromUser, string toUser, string commentId)
        {
            int result = 0;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    int teamMemberId = context.Teams.Where(x => x.UserId == UserId && x.CompanyId == companyId).FirstOrDefault().Id;
                    var activityInfo = new SocialActivity
                    {
                        ProfileId = ProfileId,
                        ProfilePageId = ProfilePageId,
                        FeedId = FeedId,
                        FeedType = FeedType,
                        ActivityTypeId = ActivityTypeId,
                        RetweetedStatusId = null,
                        MsgText = Comment,
                        CreatedDate = DateTime.Now,
                        TeamId = teamMemberId,
                        FromUser = fromUser,
                        ToUser = toUser,
                        CommentId = commentId
                    };
                    context.SocialActivities.Add(activityInfo);
                    
                    context.SaveChanges();
                    result = activityInfo.Id;
                }
            });
            return result;
        }

        public async Task<int> RemoveSocialActivity(int ActivityId)
        {
            int result = 0;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    context.SocialActivities.Remove(context.SocialActivities.Where(y => y.Id == ActivityId).FirstOrDefault());
                    result = context.SaveChanges();
                }
            });
            return result;
        }

        public async Task<SocialActivityModel> SocialActivityExists(string userId, bool isPage, string postId, string feedType, int activityTypeId)
        {
            SocialActivityModel result = null;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    int? feedId = 0, profilePageId = null, profileId = null;
                    if (feedType == "FP")
                    {
                        feedId = context.PostsFBs.Where(x => x.PostId == postId).FirstOrDefault().Id;
                    }
                    else if (feedType == "FC")
                    {
                        feedId = context.CommentsFBs.Where(x => x.CommentId == postId).FirstOrDefault().Id;
                    }
                    else if (feedType == "TP")
                    {
                        feedId = context.PostsTwitters.Where(x => x.StatusId == postId).FirstOrDefault().Id;
                    }
                    else if (feedType == "IM")
                    {
                        feedId = context.InstagramMinnedDatas.Where(x => x.PostId == postId).FirstOrDefault().Id;
                    }

                    if (isPage)
                    {
                        profilePageId = context.ProfilePages.Where(x => x.ProfilePageID == userId).FirstOrDefault().Id;

                    }
                    else
                    {
                        profileId = context.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault().Id;
                    }

                    //SocialActivity activity = context.SocialActivities.Where(
                    //    x => x.ProfileId == profileId &&
                    //    x.ProfilePageId == profilePageId &&
                    //    x.FeedId == feedId &&
                    //    x.FeedType == feedType &&
                    //    x.ActivityTypeId == activityTypeId).FirstOrDefault();
                    //if(activity != null)
                    //{
                    //    result = new SocialActivityModel
                    //    {
                    //        Id = activity.Id,
                    //        RetweetedStatusId = activity.RetweetedStatusId
                    //    };
                    //}
                }
            });
            return result;
        }
        //public async Task<List<SelectListItem>> GetConsumerNumbers(string profileId, string commaSeperatedProfiles, string commaSeperatedPages)
        //{
        //    List<SelectListItem> result = new List<SelectListItem>();
        //    await Task.Run(() =>
        //    {
        //        using (var context = new SocialCRMEntities())
        //        {
        //            int[] idList = Array.ConvertAll(commaSeperatedProfiles.Split(','), int.Parse);
        //            var userList = context.Profiles.Where(x => idList.Contains(x.Id)).Select(y => y.ProfileExternalId).ToArray();

        //            int[] pageIdList = Array.ConvertAll(commaSeperatedPages.Split(','), int.Parse);
        //            var pageList = context.ProfilePages.Where(x => pageIdList.Contains(x.Id)).Select(y => y.ProfilePageID).ToArray();

        //            var profileList = context.CustomerSocialProfiles.Where(x => x.CustomerProfileId == profileId && (userList.Contains(x.UserId) || pageList.Contains(x.UserId))).ToList();
        //            if (profileList != null && profileList.Count > 0)
        //            {
        //                foreach (var item in profileList)
        //                {
        //                    SelectListItem obj = new SelectListItem();
        //                    obj.Text = item.ConsumerNumber;
        //                    obj.Value = Convert.ToString(item.Id);
        //                    result.Add(obj);
        //                }
        //            }
        //        }
        //    });
        //    return result;
        //}

        public async Task<List<SelectListItem>> GetConsumerNumbers(string profileId, string commaSeperatedProfiles, string commaSeperatedPages, int groupId)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    /*result = context.GetConsumerNumbersByProfile(commaSeperatedProfiles, commaSeperatedPages, groupId, profileId).Select(r => new SelectListItem
                    {
                        Value = Convert.ToString(r.Id),
                        Text = r.ConsumerNumber
                    }).ToList();*/
                }
            });
            return result;
        }

        public async Task<List<InboxItemModel>> GetData(int userId, string profileData, string profilePageData, string pageType, string msgTypeData, string tagTypeData, string brandTypeData, string sentTypeData, int pageNumber, int pageSize, string teamData, int groupId, int companyId, bool isAllTask, bool showComplaints, string consumerIds, string authorName, string authorHandle, DateTime startDate, DateTime endDate, string postIds)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            List<InboxItemModel> finalList = new List<InboxItemModel>();

            int[] ids = Array.ConvertAll(postIds.Split('|'), s => int.Parse(s));
            int MessagesTWId = ids[0];
            int PageChatsFBId = ids[1];
            int PostsFBId = ids[2];
            int PostsTWId = ids[3];
            int CommentsFBId = ids[4];
            int InstagramId = ids[5];
            int SearchPostTWId = ids[6];
            int InstagramCommentId = ids[7];
            int PageTypeId = 0;
            if (pageType == "Unified")
            {
                PageTypeId = 1;
            }
            else if (pageType == "Archive")
            {
                PageTypeId = 2;
            }
            else if (pageType == "Completed")
            {
                PageTypeId = 3;
            }

            result = await GetUnifiedInboxForUserSelectedProfiles(PageTypeId, userId, profileData, profilePageData, msgTypeData, tagTypeData, brandTypeData, sentTypeData, pageNumber, pageSize, teamData, groupId, companyId, isAllTask, consumerIds, showComplaints, authorName, authorHandle, startDate, endDate);
            //if (MessagesTWId > 0)
            //{
            //    finalList.AddRange(result.Where(x => x.MessageType == "TM" && x.Id > MessagesTWId));
            //}
            //if (PageChatsFBId > 0)
            //{
            //    finalList.AddRange(result.Where(x => x.MessageType == "FCP" && x.Id > PageChatsFBId));
            //}
            //if (PostsFBId > 0)
            //{
            //    finalList.AddRange(result.Where(x => x.MessageType == "FP" && x.Id > PostsFBId));
            //}
            //if (PostsTWId > 0)
            //{
            //    finalList.AddRange(result.Where(x => x.MessageType == "TP" && x.Id > PostsTWId));
            //}
            //if (CommentsFBId > 0)
            //{
            //    finalList.AddRange(result.Where(x => x.MessageType == "FC" && x.Id > CommentsFBId));
            //}
            //if (InstagramId > 0)
            //{
            //    finalList.AddRange(result.Where(x => x.MessageType == "IM" && x.Id > InstagramId));
            //}
            //if (SearchPostTWId > 0)
            //{
            //    finalList.AddRange(result.Where(x => x.MessageType == "STP" && x.Id > SearchPostTWId));
            //}

            finalList.AddRange(result.Where(x =>
                                            (x.MessageType == "TM" && x.Id > MessagesTWId) ||
                                            (x.MessageType == "FCP" && x.Id > PageChatsFBId) ||
                                            (x.MessageType == "FP" && x.Id > PostsFBId) ||
                                            (x.MessageType == "TP" && x.Id > PostsTWId) ||
                                            (x.MessageType == "FC" && x.Id > CommentsFBId) ||
                                            (x.MessageType == "IM" && x.Id > InstagramId) ||
                                            (x.MessageType == "STP" && x.Id > SearchPostTWId) ||
                                            (x.MessageType == "IC" && x.Id > InstagramCommentId)));

            return finalList;
        }

        public async Task<bool> DeleteTag(int tagId)
        {
            bool result = false;
            try
            {
                //  await Task.Run(() =>
                //  {
                using (var context = new SocialCRMEntities())
                {
                    //var brandKeyword = context.BrandKeywords.Where(x => x.Id == tagId && x.GroupId == GroupId).FirstOrDefault();

                    //if (brandKeyword != null)
                    //{
                    //    bool isDeleted = DeleteTagReferences(brandKeyword.Id);
                    //    if (isDeleted)
                    //    {
                    //        context.BrandKeywords.Remove(brandKeyword);
                    //        context.Entry(brandKeyword).State = EntityState.Modified;
                    //        context.SaveChanges();
                    //        result = true;
                    //    }
                    //}

                    var brand = context.BrandKeywords.Where(b => b.Id == tagId).FirstOrDefault();
                    context.DeleteTagReferences(tagId);
                    //context.DeleteKeyword(keywordId, groupId);
                    // context.BrandKeywords.Remove(brand);

                    var group = context.Groups.Where(g => g.Id == brand.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = context.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";

                    notifText = string.Format("Tag: {0} has been removed from Company: {1}", (brand.KeywordName != "" ? brand.KeywordName : brand.KeywordText), companyName);
                    notifType = "Tag Removed";

                    context.AddUserNotification(brand.GroupId, brand.AddedByUserId, notifText, notifType);
                    var rowcount = await context.SaveChangesAsync();
                    result = rowcount > 0;
                }
                // });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> EditTag(TagModel model)
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        DeleteTagReferences(model.Id);

                        var brandKeyword = context.BrandKeywords.Where(x => x.Id == model.Id && x.GroupId == model.GroupId).FirstOrDefault();
                        if (brandKeyword != null)
                        {
                            brandKeyword.KeywordName = model.KeywordName;
                            brandKeyword.KeywordText = model.KeywordText;
                            brandKeyword.FontColor = model.FontColor;
                            brandKeyword.BGColor = model.BGColor;

                            context.BrandKeywords.Attach(brandKeyword);
                            context.SaveChanges();

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

        private bool DeleteTagReferences(int keywordId)
        {
            bool result = false;
            try
            {
                using (var context = new SocialCRMEntities())
                {
                    var profileFeedTagObj = context.ProfileFeedsTags.Where(x => x.KeywordId == keywordId).ToList();
                    foreach (var item in profileFeedTagObj)
                    {
                        var taskManagerData = context.TaskManagers.Where(x => x.ActivityId == item.FeedId && x.ActivityTable == item.FeedType && x.StatusId == 3).FirstOrDefault();

                        if (taskManagerData != null)
                        {
                            foreach (var taskAssgneeItem in taskManagerData.TaskAssignees)
                            {
                                context.TaskAssignees.Remove(taskAssgneeItem);
                                context.Entry(taskAssgneeItem).State = EntityState.Modified;
                            }


                            foreach (var taskLogItem in taskManagerData.TaskLogs)
                            {
                                context.TaskLogs.Remove(taskLogItem);
                                context.Entry(taskLogItem).State = EntityState.Modified;
                            }


                            foreach (var taskcommentsItem in taskManagerData.TaskComments)
                            {
                                context.TaskComments.Remove(taskcommentsItem);
                                context.Entry(taskcommentsItem).State = EntityState.Modified;
                            }


                            context.TaskManagers.Remove(taskManagerData);
                            context.Entry(taskManagerData).State = EntityState.Modified;
                        }

                        context.ProfileFeedsTags.Remove(item);
                        context.Entry(item).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> DeleteKeyword(int keywordId, int groupId)
        {
            bool result = false;
            try
            {
                //  await Task.Run(() =>
                // {
                using (var context = new SocialCRMEntities())
                {
                    //var brandKeyword = context.BrandKeywords.Where(x => x.Id == keywordId && x.GroupId == groupId).FirstOrDefault();

                    //if (brandKeyword != null)
                    //{
                    //    var twitterPosts = context.PostsTwitters.Where(x => x.KeywordId == keywordId).ToList();

                    //    foreach (var item in twitterPosts)
                    //        context.PostsTwitters.Remove(item);

                    //    context.BrandKeywords.Remove(brandKeyword);
                    //    context.SaveChanges();
                    //    result = true;
                    //}

                    var brand = context.BrandKeywords.Where(b => b.Id == keywordId).FirstOrDefault();
                    context.DeleteKeyword(keywordId, groupId);
                    // context.BrandKeywords.Remove(brand);

                    var group = context.Groups.Where(g => g.Id == brand.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = context.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";

                    notifText = string.Format("Brand Keyword: {0} has been removed from Group: {1} of Company: {2}", (brand.KeywordName != "" ? brand.KeywordName : brand.KeywordText), groupName, companyName);
                    notifType = "Keyword Removed";

                    context.AddUserNotification(brand.GroupId, brand.AddedByUserId, notifText, notifType);
                    var rowcount = await context.SaveChangesAsync();
                    result = rowcount > 0;
                }
                //  });
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<string> GetActivePattern()
        {
            string result = "";
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        result = context.SearchWordPatterns.Where(x => x.IsActive == 1).FirstOrDefault().JsPattern;
                    }
                });
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }
    }
}