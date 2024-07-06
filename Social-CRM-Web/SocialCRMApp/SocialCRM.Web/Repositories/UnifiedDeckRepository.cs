using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SocialCRM.Web.Models;
using SocialCRM.Web.Models.UnifiedInbox;
using System.Text.RegularExpressions;
using CRM.Common.Extensions;
using CRM.Entities;
using System.Data.Entity;
using CRM.Common.Enum;
using SocialAPIsWrapper.APIs;
using SocialCRM.Web.Models.UnifiedDeck;
using System.Transactions;

namespace SocialCRM.Web.Repositories
{
    public class UnifiedDeckRepository : IUnifiedDeckRepository
    {
        public async Task<UnifiedDeckModel> GetUnifiedDeckForUserSelectedProfiles(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, string pageNum, int pageSize, int groupId, int companyId, bool isAllTask, int mediaType)
        {
            UnifiedDeckModel model = new UnifiedDeckModel();
            model.FacebookPostItems = model.FacebookCommentItems = model.FacebookPageChatItems = model.TwitterPostItems = model.TwitterMessageItems = model.UnrespondedItems = new List<InboxItemModel>();
            try
            {
                using (SocialCRMEntities db = new SocialCRMEntities())
                {
                    if (mediaType == 1)
                    {
                        int pageNumber = Convert.ToInt32(pageNum);
                        model.FacebookPostItems = await GetUnifiedInboxFbPosts(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        model.FacebookCommentItems = await GetUnifiedInboxFbComments(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        model.FacebookPageChatItems = await GetUnifiedInboxFbChats(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        model.TwitterPostItems = await GetUnifiedInboxTwPosts(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        model.TwitterMessageItems = await GetUnifiedInboxTwMessages(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        model.UnrespondedItems = await GetUnifiedInboxUnrespondedItems(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, false);
                    }
                    else if (mediaType == 2)
                    {
                        int pageNumber = Convert.ToInt32(pageNum);
                        model.FacebookPostItems = await GetUnifiedInboxFbPosts(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                    }
                    else if (mediaType == 3)
                    {
                        int pageNumber = Convert.ToInt32(pageNum);
                        model.FacebookCommentItems = await GetUnifiedInboxFbComments(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                    }
                    else if (mediaType == 4)
                    {
                        int pageNumber = Convert.ToInt32(pageNum);
                        model.FacebookPageChatItems = await GetUnifiedInboxFbChats(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                    }
                    else if (mediaType == 5)
                    {
                        int pageNumber = Convert.ToInt32(pageNum);
                        model.TwitterPostItems = await GetUnifiedInboxTwPosts(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                    }
                    else if (mediaType == 6)
                    {
                        int pageNumber = Convert.ToInt32(pageNum);
                        model.TwitterMessageItems = await GetUnifiedInboxTwMessages(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                    }
                    else if (mediaType == 7)
                    {
                        //TP|TM|FP|FC|FCP
                        int pageNumber = 0;
                        string[] profileDataArray = profileData.Split('|');
                        string[] profilePageDataArray = profilePageData.Split('|');
                        string[] msgTypeDataArray = msgTypeData.Split('|');
                        string[] tagTypeDataArray = tagTypeData.Split('|');
                        string[] sentTypeDataArray = sentTypeData.Split('|');
                        string[] pageNumberArray = pageNum.Split('|');

                        profileData = profileDataArray[2];
                        profilePageData = profilePageDataArray[2];
                        msgTypeData = msgTypeDataArray[0];
                        tagTypeData = tagTypeDataArray[2];
                        sentTypeData = sentTypeDataArray[2];
                        pageNumber = Convert.ToInt32(pageNumberArray[2]);

                        model.FacebookPostItems = await GetUnifiedInboxFbPosts(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        
                        profileData = profileDataArray[3];
                        profilePageData = profilePageDataArray[3];
                        msgTypeData = msgTypeDataArray[0];
                        tagTypeData = tagTypeDataArray[3];
                        sentTypeData = sentTypeDataArray[3];
                        pageNumber = Convert.ToInt32(pageNumberArray[3]);

                        model.FacebookCommentItems = await GetUnifiedInboxFbComments(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        
                        profileData = profileDataArray[4];
                        profilePageData = profilePageDataArray[4];
                        msgTypeData = msgTypeDataArray[0];
                        tagTypeData = tagTypeDataArray[4];
                        sentTypeData = sentTypeDataArray[4];
                        pageNumber = Convert.ToInt32(pageNumberArray[4]);

                        model.FacebookPageChatItems = await GetUnifiedInboxFbChats(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        
                        profileData = profileDataArray[0];
                        profilePageData = profilePageDataArray[0];
                        msgTypeData = msgTypeDataArray[0];
                        tagTypeData = tagTypeDataArray[0];
                        sentTypeData = sentTypeDataArray[0];
                        pageNumber = Convert.ToInt32(pageNumberArray[0]);

                        model.TwitterPostItems = await GetUnifiedInboxTwPosts(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        
                        profileData = profileDataArray[1];
                        profilePageData = profilePageDataArray[1];
                        msgTypeData = msgTypeDataArray[0];
                        tagTypeData = tagTypeDataArray[1];
                        sentTypeData = sentTypeDataArray[1];
                        pageNumber = Convert.ToInt32(pageNumberArray[1]);

                        model.TwitterMessageItems = await GetUnifiedInboxTwMessages(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask);
                        //model.UnrespondedItems = await GetUnifiedInboxUnrespondedItems(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, minFbPostId, minFbCommentId, minFbChatId, minTwPostId, minTwMessageId);
                    }
                }
            }
            catch (Exception ex)
            {
                model = null;
            }
            return model;
        }

        public async Task<List<InboxItemModel>> GetUnifiedInboxFbPosts(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();

                        foreach (var data in context.UnifiedInboxOfSelectedProfilesFbPost(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, teamMemberId, groupId, isAllTask))
                        {
                            string type = "";
                            string activityType = "";
                            if (data.ActivityType == "FP")
                            {
                                type = "fb_ico16";
                                activityType = "wall post | ";
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
                                ReplyedByFullName = data.FromUser,
                                ReplyedByUserId = data.FromUserId,
                                ToUser = data.ToUser,
                                ToUserFullName = data.ToUser,
                                ToUserId = data.UserId,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)data.CreatedDate),
                                CreatedDate = (DateTime)data.CreatedDate,
                                ActivityType = activityType,
                                AccountTypeImg = type,
                                MessageType = data.ActivityType,
                                IsArchived = Convert.ToBoolean(data.IsArchived),
                                StatusId = data.StatusId,
                                IsPage = Convert.ToBoolean(data.IsPage),
                                IsTaskCompleted = Convert.ToBoolean(data.IsTaskCompleted),
                                IsRetweet = Convert.ToBoolean(data.IsRetweet),
                                IsLikedByAdmin = Convert.ToBoolean(data.IsLikedByAdmin),
                                Tags = (data.Keywords == null) ? new List<string>() : data.Keywords.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                TagIds = (data.KeywordIds == null) ? new List<string>() : data.KeywordIds.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Urls = String.IsNullOrEmpty(data.Attachments) ? new List<string>() : data.Attachments.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Sentiment = data.Sentiment,
                                IsWarned = Convert.ToBoolean(data.IsWarned)
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

        public async Task<List<InboxItemModel>> GetUnifiedInboxFbComments(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();

                        foreach (var data in context.UnifiedInboxOfSelectedProfilesFbComment(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, teamMemberId, groupId, isAllTask))
                        {
                            string type = "";
                            string activityType = "";
                            if (data.ActivityType == "FC")
                            {
                                type = "fb_ico16";
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
                                ReplyedByFullName = data.FromUser,
                                ReplyedByUserId = data.FromUserId,
                                ToUser = data.ToUser,
                                ToUserFullName = data.ToUser,
                                ToUserId = data.ToUserId,
                                ParentId = data.ParentId,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)data.CreatedDate),
                                CreatedDate = (DateTime)data.CreatedDate,
                                ActivityType = activityType,
                                AccountTypeImg = type,
                                MessageType = data.ActivityType,
                                IsArchived = Convert.ToBoolean(data.IsArchived),
                                StatusId = data.StatusId,
                                IsPage = Convert.ToBoolean(data.IsPage),
                                IsTaskCompleted = Convert.ToBoolean(data.IsTaskCompleted),
                                IsLikedByAdmin = Convert.ToBoolean(data.IsLikedByAdmin),
                                IsRetweet = Convert.ToBoolean(data.IsRetweet),
                                Tags = (data.Keywords == null) ? new List<string>() : data.Keywords.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                TagIds = (data.KeywordIds == null) ? new List<string>() : data.KeywordIds.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Urls = String.IsNullOrEmpty(data.Attachments) ? new List<string>() : data.Attachments.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Sentiment = data.Sentiment,
                                IsWarned = Convert.ToBoolean(data.IsWarned)
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

        public async Task<List<InboxItemModel>> GetUnifiedInboxFbChats(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();

                        foreach (var data in context.UnifiedInboxOfSelectedProfilesFbChat(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, teamMemberId, groupId, isAllTask))
                        {
                            string type = "";
                            string activityType = "";
                            if (data.ActivityType == "FCP")
                            {
                                type = "fb_ico16";
                                activityType = "sent a direct message to ";
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
                                ReplyedByFullName = data.FromUser,
                                ReplyedByUserId = data.FromUserId,
                                ToUser = data.ToUser,
                                ToUserFullName = data.ToUser,
                                ToUserId = data.ToUserId,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)data.CreatedDate),
                                CreatedDate = (DateTime)data.CreatedDate,
                                ActivityType = activityType,
                                AccountTypeImg = type,
                                MessageType = data.ActivityType,
                                IsArchived = Convert.ToBoolean(data.IsArchived),
                                StatusId = data.StatusId,
                                ThreadId = data.StatusId.Split('_')[1],
                                IsPage = Convert.ToBoolean(data.IsPage),
                                IsTaskCompleted = Convert.ToBoolean(data.IsTaskCompleted),
                                IsRetweet = Convert.ToBoolean(data.IsRetweet),
                                Tags = (data.Keywords == null) ? new List<string>() : data.Keywords.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                TagIds = (data.KeywordIds == null) ? new List<string>() : data.KeywordIds.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Urls = String.IsNullOrEmpty(data.Attachments) ? new List<string>() : data.Attachments.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Sentiment = data.Sentiment,
                                IsWarned = Convert.ToBoolean(data.IsWarned)
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

        public async Task<List<InboxItemModel>> GetUnifiedInboxTwPosts(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();

                        if (pageNumber > 0)
                        {
                            string a = "";
                        }

                        foreach (var data in context.UnifiedInboxOfSelectedProfilesTwPost(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, teamMemberId, groupId, isAllTask))
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
                                ToUser = data.ToUser,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)data.CreatedDate),
                                CreatedDate = (DateTime)data.CreatedDate,
                                ActivityType = activityType,
                                AccountTypeImg = type,
                                MessageType = data.ActivityType,
                                IsArchived = Convert.ToBoolean(data.IsArchived),
                                StatusId = data.StatusId,
                                IsPage = Convert.ToBoolean(data.IsPage),
                                IsTaskCompleted = Convert.ToBoolean(data.IsTaskCompleted),
                                IsLikedByAdmin = Convert.ToBoolean(data.IsLikedByAdmin),
                                IsRetweet = Convert.ToBoolean(data.IsRetweet),
                                Tags = (data.Keywords == null) ? new List<string>() : data.Keywords.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                TagIds = (data.KeywordIds == null) ? new List<string>() : data.KeywordIds.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Urls = String.IsNullOrEmpty(data.Attachments) ? new List<string>() : data.Attachments.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Sentiment = data.Sentiment,
                                IsWarned = Convert.ToBoolean(data.IsWarned)
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

        public async Task<List<InboxItemModel>> GetUnifiedInboxTwMessages(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();

                        foreach (var data in context.UnifiedInboxOfSelectedProfilesTwMessages(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, teamMemberId, groupId, isAllTask))
                        {
                            string type = "";
                            string activityType = "";
                            if (data.ActivityType == "TM")
                            {
                                type = "t_ico16";
                                activityType = "sent a direct message to ";
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
                                ReplyedByUserId = data.FromUserId,
                                ReplyedByFullName = data.FromUserFullName,
                                ToUser = data.ToUser,
                                ToUserFullName = data.ToUserFullName,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)data.CreatedDate),
                                CreatedDate = (DateTime)data.CreatedDate,
                                ActivityType = activityType,
                                AccountTypeImg = type,
                                MessageType = data.ActivityType,
                                IsArchived = Convert.ToBoolean(data.IsArchived),
                                StatusId = data.StatusId,
                                IsPage = Convert.ToBoolean(data.IsPage),
                                IsTaskCompleted = Convert.ToBoolean(data.IsTaskCompleted),
                                IsRetweet = Convert.ToBoolean(data.IsRetweet),
                                Tags = (data.Keywords == null) ? new List<string>() : data.Keywords.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                TagIds = (data.KeywordIds == null) ? new List<string>() : data.KeywordIds.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Urls = String.IsNullOrEmpty(data.Attachments) ? new List<string>() : data.Attachments.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>(),
                                Sentiment = data.Sentiment,
                                IsWarned = Convert.ToBoolean(data.IsWarned)
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

        public async Task<List<InboxItemModel>> GetUnifiedInboxUnrespondedItems(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask, bool markCompleted)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();
                        var unrespondedData = context.UnifiedInboxOfSelectedProfilesAllUnresponded(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, teamMemberId, groupId, isAllTask, markCompleted);
                        foreach (var data in unrespondedData)
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

                            InboxItemModel item = new InboxItemModel();
                            item.Id = data.Id;
                            item.UserId = data.UserId;
                            item.ClassName = "unread_msg";
                            item.TwtImg = data.ProfilePic;
                            item.IsVerified = Convert.ToBoolean(data.IsVerified);
                            item.User = data.UserName;
                            item.UserTwit = data.TextValue;
                            item.ParentId = data.ParentId;

                            if (data.ActivityType == "FP" || data.ActivityType == "FC" || data.ActivityType == "FCP")
                            {
                                item.ReplyedBy = data.FromUser;
                                item.ReplyedByFullName = data.FromUser;
                                item.ReplyedByUserId = data.FromUserId;
                                item.ToUser = data.ToUser;
                                item.ToUserFullName = data.ToUser;
                                item.ToUserId = data.UserId;

                                if (data.ActivityType == "FCP")
                                    item.ThreadId = data.ThreadId.Split('_')[1];
                            }
                            else if (data.ActivityType == "TM")
                            {
                                item.ReplyedByUserId = data.FromUserId;
                                item.ReplyedBy = data.FromUser;
                                //item.ReplyedByFullName = data.FromUserFullName;
                                item.ToUser = data.ToUser;
                                //item.ToUserFullName = data.ToUserFullName,
                            }
                            else if (data.ActivityType == "TP")
                            {
                                item.ReplyedBy = data.FromUser;
                                //item.ReplyedByFullName = data.FromUserFullName;
                                item.ToUser = data.ToUser;
                            }
                            item.ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate);
                            item.ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)data.CreatedDate);
                            item.CreatedDate = (DateTime)data.CreatedDate;
                            item.ActivityType = activityType;
                            item.AccountTypeImg = type;
                            item.MessageType = data.ActivityType;
                            item.IsArchived = Convert.ToBoolean(data.IsArchived);
                            item.StatusId = data.StatusId;
                            item.IsPage = Convert.ToBoolean(data.IsPage);
                            item.IsTaskCompleted = Convert.ToBoolean(data.IsTaskCompleted);
                            item.IsRetweet = Convert.ToBoolean(data.IsRetweet);
                            item.IsLikedByAdmin = Convert.ToBoolean(data.IsLikedByAdmin);
                            item.Tags = (data.Keywords == null) ? new List<string>() : data.Keywords.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                            item.TagIds = (data.KeywordIds == null) ? new List<string>() : data.KeywordIds.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                            item.Urls = String.IsNullOrEmpty(data.Attachments) ? new List<string>() : data.Attachments.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                            item.Sentiment = data.Sentiment;
                            item.IsWarned = Convert.ToBoolean(data.IsWarned);
                            UnifiedInboxUtility.CheckForAttachments(ref item);

                            result.Add(item);

                        }
                    }
                });

                result = result.GroupBy(g => new { g.StatusId, g.MessageType }).Select(g => g.First()).ToList();

                if (markCompleted)
                    NotificationHub.markAllUnrespondedItemsCompleted(groupId);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<UnifiedDeckModel> GetRecentUnifiedDeckForUserSelectedProfiles(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, string pageNumber, int pageSize, int groupId, int companyId, bool isAllTask, int mediaType, string postIds)
        {
            //UnifiedDeckModel model = new UnifiedDeckModel();
            UnifiedDeckModel finalResult = new UnifiedDeckModel();

            try
            {
                int[] ids = Array.ConvertAll(postIds.Split('|'), s => int.Parse(s));
                int MessagesTWId = ids[0];
                int PageChatsFBId = ids[1];
                int PostsFBId = ids[2];
                int PostsTWId = ids[3];
                int CommentsFBId = ids[4];

                finalResult = await GetUnifiedDeckForUserSelectedProfiles(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType);

                /*finalResult.TwitterPostItems = new List<InboxItemModel>();
                finalResult.TwitterMessageItems = new List<InboxItemModel>();
                finalResult.FacebookPostItems = new List<InboxItemModel>();
                finalResult.FacebookCommentItems = new List<InboxItemModel>();
                finalResult.FacebookPageChatItems = new List<InboxItemModel>();
                finalResult.TwitterPostItems.AddRange(model.TwitterPostItems.Where(x => x.MessageType == "TP" && x.Id > PostsTWId));
                finalResult.TwitterMessageItems.AddRange(model.TwitterMessageItems.Where(x => x.MessageType == "TM" && x.Id > MessagesTWId));
                finalResult.FacebookPostItems.AddRange(model.FacebookPostItems.Where(x => x.MessageType == "FP" && x.Id > PostsFBId));
                finalResult.FacebookCommentItems.AddRange(model.FacebookCommentItems.Where(x => x.MessageType == "FC" && x.Id > CommentsFBId));
                finalResult.FacebookPageChatItems.AddRange(model.FacebookPageChatItems.Where(x => x.MessageType == "FCP" && x.Id > PageChatsFBId));*/
            }
            catch (Exception ex)
            {
                finalResult = null;
            }
            
            return finalResult;
        }

        public async Task<Boolean> IsPostMarkedAsCompleted(string msgTypeData, string ActivityId)
        {
            bool isCompleted = false;

            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        if(msgTypeData == "TP")
                            isCompleted = context.TaskCompleteds.Where(x => x.ActivityTable == "TP" && x.ActivityId.ToString() == ActivityId).Select(z => z.IsCompleted).FirstOrDefault();

                        if (msgTypeData == "TM")
                            isCompleted = context.TaskCompleteds.Where(x => x.ActivityTable == "TM" && x.ActivityId.ToString() == ActivityId).Select(z => z.IsCompleted).FirstOrDefault();

                        if (msgTypeData == "FP")
                            isCompleted = context.TaskCompleteds.Where(x => x.ActivityTable == "FP" && x.ActivityId.ToString() == ActivityId).Select(z => z.IsCompleted).FirstOrDefault();

                        if (msgTypeData == "FC")
                            isCompleted = context.TaskCompleteds.Where(x => x.ActivityTable == "FC" && x.ActivityId.ToString() == ActivityId).Select(z => z.IsCompleted).FirstOrDefault();

                        if (msgTypeData == "FCP")
                            isCompleted = context.TaskCompleteds.Where(x => x.ActivityTable == "FCP" && x.ActivityId.ToString() == ActivityId).Select(z => z.IsCompleted).FirstOrDefault();

                    }
                });
            }
            catch (Exception ex)
            {
            }

            return isCompleted;
        }

        public async Task<List<InboxItemModel>> GetNextUnifiedDeckForUserSelectedProfilesUnresponded(int userId, string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize, int groupId, int companyId, bool isAllTask, string maxUnrespondedPostIds, bool markCompleted)
        {
            List<InboxItemModel> model = new List<InboxItemModel>();
            List<InboxItemModel> finalResult = new List<InboxItemModel>();
            int maxFbPostId = 0, maxFbCommentId = 0, maxFbChatId = 0, maxTwPostId = 0, maxTwMessageId = 0;
            try
            {
                int[] ids = Array.ConvertAll(maxUnrespondedPostIds.Split('|'), s => int.Parse(s));
                maxTwMessageId = ids[0];
                maxFbChatId = ids[1];
                maxFbPostId = ids[2];
                maxTwPostId = ids[3];
                maxFbCommentId = ids[4];
                
                pageNumber = pageNumber * pageSize;

                model = await GetUnifiedInboxUnrespondedItems(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, markCompleted);
                                
                finalResult.AddRange(model.Where(x =>
                                            (x.MessageType == "TM" && x.Id > maxTwMessageId) ||
                                            (x.MessageType == "FCP" && x.Id > maxFbChatId) ||
                                            (x.MessageType == "FP" && x.Id > maxFbPostId) ||
                                            (x.MessageType == "TP" && x.Id > maxTwPostId) ||
                                            (x.MessageType == "FC" && x.Id > maxFbCommentId)));         
            }
            catch (Exception ex)
            {
                finalResult = null;
            }

            return finalResult;
        }

        public async Task<bool> InsertTagsForFeed(int feedId, string feedType, string tagIds, int userId, int companyId)
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var prevTags = db.ProfileFeedsTags.Where(x => x.FeedId == feedId && x.FeedType == feedType).ToList();
                        int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).FirstOrDefault().Id;
                        if (prevTags != null)
                        {
                            string ids = string.Join(",", prevTags.Select(x => x.Id).ToArray());
                            int response = db.RemoveTagFromFeed(ids);
                            //foreach (var tag in prevTags)
                            //{
                            //    db.ProfileFeedsTags.Remove(tag);
                            //    db.Entry(tag).State = EntityState.Modified;
                            //}
                            //db.SaveChanges(); 
                        }

                        if(tagIds != "")
                        {
                            int[] tags = tagIds.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                            foreach (var tag in tags)
                            {
                                ProfileFeedsTag tagItem = new ProfileFeedsTag();
                                tagItem.FeedId = feedId;
                                tagItem.FeedType = feedType;
                                tagItem.KeywordId = tag;
                                tagItem.AdditionDate = DateTime.Now;
                                tagItem.AddedBy = teamMemberId;

                                db.ProfileFeedsTags.Add(tagItem);
                            }
                        }
                        db.SaveChanges();
                    }                    
                });
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> InsertTagsForFeed(int feedId, string feedType, int tagId, int userId, int companyId)
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var alreadyAddedTag = db.ProfileFeedsTags.Where(x => x.FeedId == feedId && x.FeedType == feedType && x.KeywordId == tagId).FirstOrDefault();
                        if (alreadyAddedTag == null)
                        {
                            ProfileFeedsTag tagItem = new ProfileFeedsTag();
                            int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).FirstOrDefault().Id;
                            tagItem.FeedId = feedId;
                            tagItem.FeedType = feedType;
                            tagItem.KeywordId = tagId;
                            tagItem.AdditionDate = DateTime.Now;
                            tagItem.AddedBy = teamMemberId;

                            db.ProfileFeedsTags.Add(tagItem);
                            db.SaveChanges();
                        }
                    }
                });
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> RemoveTagFromFeed(int feedId, string feedType, int tagId)
        {
            bool result = false;
            try
            {
                using (var db = new SocialCRMEntities())
                {
                    await Task.Run(() =>
                    {
                        var tag = db.ProfileFeedsTags.Where(x => x.FeedId == feedId && x.FeedType == feedType && x.KeywordId == tagId).FirstOrDefault();
                        if (tag != null)
                        {
                            string ids = tag.Id.ToString();
                            int response = db.RemoveTagFromFeed(ids);
                            result = true;
                        }
                    });                    
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> InsertSentimentForFeed(int feedId, string feedType, int sentimentId, int userId, int companyId)
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).FirstOrDefault().Id;
                        var feedSentiment = db.FeedsSentiments.Where(x => x.FeedId == feedId && x.FeedType == feedType).FirstOrDefault();
                        var alreadyExist = feedSentiment != null;
                        if (alreadyExist)
                        {
                            feedSentiment.SentimentId = sentimentId;
                            feedSentiment.AddedBy = teamMemberId;
                            feedSentiment.AdditionDate = DateTime.Now;
                            db.FeedsSentiments.Attach(feedSentiment);
                            db.Entry(feedSentiment).State = EntityState.Modified;
                        }
                        else
                        {
                            if(feedSentiment == null)
                            {
                                feedSentiment = new FeedsSentiment();
                            }
                            feedSentiment.FeedId = feedId;
                            feedSentiment.FeedType = feedType;
                            feedSentiment.SentimentId = sentimentId;
                            feedSentiment.AddedBy = teamMemberId;
                            feedSentiment.AdditionDate = DateTime.Now;
                            db.FeedsSentiments.Add(feedSentiment);
                        }
                        db.SaveChanges();
                    }
                });
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        //For Asjad color zones
        public async Task<List<UnifiedDeckColumnSetting>> GetColumnSettings()
        {
            List<UnifiedDeckColumnSetting> result = new List<UnifiedDeckColumnSetting>();
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        result = db.UnifiedDeckColumnSettings.ToList();
                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public async Task<bool> UpdateColumnSettings(List<UnifiedDeckColumnSetting> model)
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var settings = db.UnifiedDeckColumnSettings.ToList();
                        foreach (var setting in model)
                        {
                            var settingObj = settings.Where(x => x.Id == setting.Id).FirstOrDefault();
                            if (settingObj != null)
                            {
                                settingObj.ZoneName = setting.ZoneName;
                                settingObj.Time = setting.Time;
                                settingObj.ColorCode = setting.ColorCode;

                                db.UnifiedDeckColumnSettings.Attach(settingObj);
                                db.Entry(settingObj).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                });
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<string> DeleteFacebookItem(string userId, bool isPage, string id, FacbookReplyType type, int teamId)
        {
            string result = "";
            try
            {
                await Task.Run(async() =>
                {
                    using (var db = new SocialCRMEntities())
                    {   
                        switch (type)
                        {
                            case FacbookReplyType.Post:
                                result = await DeleteFbPost(id, true, false, false, userId, isPage);
                                if (isPage)
                                {
                                    await AddSocialActivityForDelete("", userId, id, "FP", teamId, "");
                                }
                                else
                                {
                                    await AddSocialActivityForDelete(userId, "", id, "FP", teamId, "");
                                }
                                break;
                            case FacbookReplyType.Comment:
                                result = await DeleteFbPost(id, false, true, false, userId, isPage);
                                if (isPage)
                                {
                                    await AddSocialActivityForDelete("", userId, id, "FC", teamId, "");
                                }
                                else
                                {
                                    await AddSocialActivityForDelete(userId, "", id, "FC", teamId, "");
                                }
                                break;
                            case FacbookReplyType.NestedComment:
                                result = await DeleteFbPost(id, false, false, true, userId, isPage);
                                if (isPage)
                                {
                                    await AddSocialActivityForDelete("", userId, id, "FCN", teamId, "");
                                }
                                else
                                {
                                    await AddSocialActivityForDelete(userId, "", id, "FCN", teamId, "");
                                }
                                break;
                            default:
                                break;
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

        

        private async Task<string> DeleteFbPost(string postId, bool deletePost, bool deleteComments, bool deleteNestedComments, string userId, bool isPage)
        {
            string result = "";
            List<int> postIds = new List<int>();
            List<int> commentIds = new List<int>();
            List<int> tagIds = new List<int>();
            List<int> activityIds = new List<int>();
            List<int> taskIds = new List<int>();
            try
            {
                await Task.Run(async() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        FacebookUtility fbutil = new FacebookUtility(userId, isPage);
                        if (deletePost)
                        {                            
                            var post = db.PostsFBs.Where(x => x.PostId == postId).FirstOrDefault();
                            var response = await fbutil.deleteObjectTemp(post.PostId);
                            //if (response.ContainsKey("success"))
                            if (response.Contains("True"))
                            {
                                var responseStatus = response;// Convert.ToString(response["success"]);
                                if (responseStatus == "True")
                                {
                                    result = "success";
                                    var tags = db.ProfileFeedsTags.Where(x => x.FeedId == post.Id && x.FeedType == "FP").ToList();
                                    tagIds.AddRange(tags.Select(x => x.Id));
                                    var tasks = db.TaskCompleteds.Where(x => x.ActivityId == post.Id && x.ActivityTable == "FP").ToList();
                                    taskIds.AddRange(tasks.Select(x => x.Id));
                                    postIds.Add(post.Id);

                                    var comments = db.CommentsFBs.Where(x => x.PostId == postId).ToList();
                                    foreach (var comment in comments)
                                    {
                                        commentIds.Add(comment.Id);

                                        var tagComments = db.ProfileFeedsTags.Where(x => x.FeedId == comment.Id && x.FeedType == "FC").ToList();
                                        tagIds.AddRange(tagComments.Select(x => x.Id));

                                        var taskComments = db.TaskCompleteds.Where(x => x.ActivityId == comment.Id && x.ActivityTable == "FC").ToList();
                                        taskIds.AddRange(taskComments.Select(x => x.Id));

                                        var activityComment = db.SocialActivities.Where(x => x.CommentId == comment.CommentId).FirstOrDefault();
                                        if (activityComment != null)
                                        {
                                            activityIds.Add(activityComment.Id);
                                        }

                                        if (!String.IsNullOrEmpty(comment.MaxCommentId))
                                        {
                                            var nestedComments = db.CommentsFBs.Where(x => x.PostId == comment.CommentId).ToList();

                                            foreach (var nestedComment in nestedComments)
                                            {
                                                commentIds.Add(nestedComment.Id);

                                                var tagNestedComments = db.ProfileFeedsTags.Where(x => x.FeedId == nestedComment.Id && x.FeedType == "FC").ToList();
                                                tagIds.AddRange(tagNestedComments.Select(x => x.Id));

                                                var taskNestedComments = db.TaskCompleteds.Where(x => x.ActivityId == nestedComment.Id && x.ActivityTable == "FC").ToList();
                                                taskIds.AddRange(taskNestedComments.Select(x => x.Id));

                                                var activityNestedComments = db.SocialActivities.Where(x => x.CommentId == nestedComment.CommentId).FirstOrDefault();
                                                if (activityNestedComments != null)
                                                {
                                                    activityIds.Add(activityNestedComments.Id);
                                                }                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (deleteComments)
                        {
                            var comment = db.CommentsFBs.Where(x => x.CommentId == postId).FirstOrDefault();
                            var response = await fbutil.deleteObjectTemp(comment.CommentId);
                            //if (response.ContainsKey("success"))
                            if (response.Contains("True"))
                            {
                                var responseStatus = response;// Convert.ToString(response["success"]);
                                
                                if (responseStatus == "True")
                                {
                                    result = "success";
                                    commentIds.Add(comment.Id);

                                    var tagComments = db.ProfileFeedsTags.Where(x => x.FeedId == comment.Id && x.FeedType == "FC").ToList();
                                    tagIds.AddRange(tagComments.Select(x => x.Id));

                                    var taskComments = db.TaskCompleteds.Where(x => x.ActivityId == comment.Id && x.ActivityTable == "FC").ToList();
                                    taskIds.AddRange(taskComments.Select(x => x.Id));

                                    var activityComment = db.SocialActivities.Where(x => x.CommentId == comment.CommentId).FirstOrDefault();
                                    if (activityComment != null)
                                    {
                                        activityIds.Add(activityComment.Id);
                                    }

                                    if (!String.IsNullOrEmpty(comment.MaxCommentId))
                                    {
                                        var nestedComments = db.CommentsFBs.Where(x => x.PostId == comment.CommentId).ToList();

                                        foreach (var nestedComment in nestedComments)
                                        {
                                            commentIds.Add(nestedComment.Id);

                                            var tagNestedComments = db.ProfileFeedsTags.Where(x => x.FeedId == nestedComment.Id && x.FeedType == "FC").ToList();
                                            tagIds.AddRange(tagNestedComments.Select(x => x.Id));

                                            var taskNestedComments = db.TaskCompleteds.Where(x => x.ActivityId == nestedComment.Id && x.ActivityTable == "FC").ToList();
                                            taskIds.AddRange(taskNestedComments.Select(x => x.Id));

                                            var activityNestedComments = db.SocialActivities.Where(x => x.CommentId == nestedComment.CommentId).FirstOrDefault();
                                            if (activityNestedComments != null)
                                            {
                                                activityIds.Add(activityNestedComments.Id);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (deleteNestedComments)
                        {
                            var nestedComment = db.CommentsFBs.Where(x => x.CommentId == postId).FirstOrDefault();
                            var response = await fbutil.deleteObjectTemp(nestedComment.CommentId);
                            //if (response.ContainsKey("success"))
                            if (response.Contains("True"))
                            {
                                var responseStatus = response;// Convert.ToString(response["success"]);
                                if (responseStatus == "True")
                                {
                                    result = "success";
                                    commentIds.Add(nestedComment.Id);

                                    var tagComments = db.ProfileFeedsTags.Where(x => x.FeedId == nestedComment.Id && x.FeedType == "FC").ToList();
                                    tagIds.AddRange(tagComments.Select(x => x.Id));

                                    var taskNestedComments = db.TaskCompleteds.Where(x => x.ActivityId == nestedComment.Id && x.ActivityTable == "FC").ToList();
                                    taskIds.AddRange(taskNestedComments.Select(x => x.Id));

                                    var activityComment = db.SocialActivities.Where(x => x.CommentId == nestedComment.CommentId).FirstOrDefault();
                                    if (activityComment != null)
                                    {
                                        activityIds.Add(activityComment.Id);
                                    }
                                }
                            }
                        }
                        
                        string postStr = string.Join(",", postIds);
                        string commentStr = string.Join(",", commentIds);
                        string tagStr = string.Join(",", tagIds);
                        string activityStr = string.Join(",", activityIds);
                        string taskStr = string.Join(",", taskIds);

                        int resp = db.DeleteFbItem(postStr, commentStr, tagStr, activityStr, taskStr);
                        
                    }
                });
                
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public async Task<bool> DeleteFacebookPageChatItem(string userId, string id)
        {
            bool result = false;
            try
            {
                await Task.Run( () =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var msg = db.PageChatsFBs.Where(x => x.MsgId == id).FirstOrDefault();
                        if (msg != null)
                        {
                            var tags = db.ProfileFeedsTags.Where(x => x.FeedId == msg.Id && x.FeedType == "FCP").ToList();
                            foreach (var tag in tags)
                            {
                                db.ProfileFeedsTags.Remove(tag);
                            }
                            var activity = db.SocialActivities.Where(x => x.CommentId == msg.MsgId).FirstOrDefault();
                            if (activity != null)
                            {
                                db.SocialActivities.Remove(activity);
                            }
                            db.PageChatsFBs.Remove(msg);
                            db.SaveChanges();
                        }
                    }
                });
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<string> DeleteTwitterTweet(string userName, string tweetId, int teamId)
        {
            string result = "";
            List<int> tagIds = new List<int>();
            List<int> activityIds = new List<int>();
            List<int> taskIds = new List<int>();
            string tweetStr = tweetId;
            try
            {
                await Task.Run(async () =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var allTweets = db.PostsTwitters.Where(x => x.KeywordId.HasValue == false).ToList();
                        var tweet = db.PostsTwitters.Where(x => x.StatusId == tweetStr).FirstOrDefault();
                        if (tweet != null)
                        {
                            TwitterUtility twitterUtil = new TwitterUtility(userName);
                            var response = await twitterUtil.deleteTweet(ulong.Parse(tweet.StatusId));
                            if (response != null)
                            {
                                var tags = db.ProfileFeedsTags.Where(x => x.FeedId == tweet.Id && x.FeedType == "TP").ToList();
                                tagIds.AddRange(tags.Select(x => x.Id));

                                var tasks = db.TaskCompleteds.Where(x => x.ActivityId == tweet.Id && x.ActivityTable == "TP").ToList();
                                taskIds.AddRange(tasks.Select(x => x.Id));

                                var activity = db.SocialActivities.Where(x => x.CommentId == tweet.StatusId).FirstOrDefault();
                                if (activity != null)
                                {
                                    activityIds.Add(activity.Id);
                                }
                                string tagStr = string.Join(",", tagIds);
                                string activityStr = string.Join(",", activityIds);
                                string taskStr = string.Join(",", taskIds);
                                int resp = db.DeleteTwPostItem(tweetStr, tagStr, activityStr, taskStr);
                                await AddSocialActivityForDelete(tweet.UserId, "", tweet.StatusId, "TP", teamId, tweet.StatusText);
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

        private async void DeleteTweets(PostsTwitter parent, List<PostsTwitter> allItems, string userName)
        {
            var childTweets = allItems.Where(x => x.InReplyToStatusId == parent.StatusId).ToList();
            if (childTweets.Count() > 0)
            {
                foreach (PostsTwitter child in childTweets)
                {
                    DeleteTweets(child, allItems, userName);
                }
            }
            using (SocialCRMEntities db = new SocialCRMEntities())
            {
                
            }
        }

        public async Task<string> DeleteTwitterDM(string userName, string msgId, int teamId)
        {
            string result = "";
            List<int> tagIds = new List<int>();
            List<int> activityIds = new List<int>();
            List<int> taskIds = new List<int>();
            try
            {
                await Task.Run(async() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        TwitterUtility twitterUtil = new TwitterUtility(userName);
                        var response = await twitterUtil.deleteDirectMessage(ulong.Parse(msgId));
                        if (response != null)
                        {
                            var msg = db.MessagesTwitters.ToList().Where(x => x.MsgId == Convert.ToString(msgId)).FirstOrDefault();
                            if (msg != null)
                            {
                                var tags = db.ProfileFeedsTags.ToList().Where(x => x.FeedId == msg.Id && x.FeedType == "TM").ToList();
                                tagIds.AddRange(tags.Select(x => x.Id));

                                var tasks = db.TaskCompleteds.Where(x => x.ActivityId == msg.Id && x.ActivityTable == "TM").ToList();
                                taskIds.AddRange(tasks.Select(x => x.Id));

                                var activity = db.SocialActivities.Where(x => x.CommentId == msg.MsgId).FirstOrDefault();
                                if (activity != null)
                                {
                                    activityIds.Add(activity.Id);
                                }
                                string tagStr = string.Join(",", tagIds);
                                string activityStr = string.Join(",", activityIds);
                                string taskStr = string.Join(",", taskIds);
                                int resp = db.DeleteTwMsgItem(msg.Id, tagStr, activityStr, taskStr);
                                await AddSocialActivityForDelete(msg.UserId, "", msg.MsgId, "TM", teamId, msg.MsgText);
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

        public async Task<string> DeleteInstagramComment(string userId, string commentId, int teamId)
        {
            string result = "";
            List<int> tagIds = new List<int>();
            List<int> activityIds = new List<int>();
            try
            {
                await Task.Run(async() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        string accessToken = db.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault().AcessToken;
                        var comment = db.InstagramComments.Where(x => x.CommentId == commentId).FirstOrDefault();
                        var post = db.InstagramMinnedDatas.Where(x => x.PostId == comment.PostId).FirstOrDefault();
                        var profileId = db.Profiles.Where(x => x.Id == post.InstagramAccountId).FirstOrDefault().ProfileExternalId;
                        if (comment != null)
                        {
                            var response = await InstagramUtility.DeleteInstagramComment(comment.PostId, accessToken, commentId);
                            if (response.Contains("success"))
                            {
                                var tags = db.ProfileFeedsTags.Where(x => x.FeedId == comment.Id && x.FeedType == "IC").ToList();
                                tagIds.AddRange(tags.Select(x => x.Id));
                                var activity = db.SocialActivities.Where(x => x.CommentId == comment.CommentId).FirstOrDefault();
                                if (activity != null)
                                {
                                    activityIds.Add(activity.Id);
                                }
                                string tagStr = string.Join(",", tagIds);
                                string activityStr = string.Join(",", activityIds);
                                int resp = db.DeleteInstaCommentItem(comment.Id, tagStr, activityStr);
                                await AddSocialActivityForDelete(profileId, "", comment.CommentId, "IC", teamId, comment.Message);
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

        public async Task<List<SampleReply>> GetSampleReplies()
        {
            List<SampleReply> result = new List<SampleReply>();
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        result = db.SampleReplies.ToList();
                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private async Task<bool> AddSocialActivityForDelete(string profileId, string profilePageId, string postId, string feedType, int userId, string msgText)
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var userAccountType = (from uc in db.UserCompanies
                                               join c in db.Companies
                                               on uc.CompanyId equals c.id
                                               where uc.UserId == userId && uc.CompanyTypeId == (int)CompanyTypes.Owned
                                               select c).FirstOrDefault();

                        int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == userAccountType.id).FirstOrDefault().Id;

                        var activityInfo = new SocialActivity
                        {
                            ProfileId = profileId,
                            ProfilePageId = profilePageId,
                            FeedId = postId,
                            FeedType = feedType,
                            ActivityTypeId = 1,
                            RetweetedStatusId = null,
                            MsgText = msgText,
                            CreatedDate = DateTime.Now,
                            TeamId = teamMemberId,
                            FromUser = "",
                            ToUser = "",
                            CommentId = ""
                        };

                        db.SocialActivities.Add(activityInfo);
                        db.SaveChanges();

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

        public async Task<ConsumerProfileModel> GetConsumerData(string profile, string onProfile, bool isPage, string feedType)
        {
            ConsumerProfileModel result = new ConsumerProfileModel();
            await Task.Run(async() =>
            {
                try
                {
                    using (var db = new SocialCRMEntities())
                    {
                        var consumerProfile = new CustomerSocialProfile();
                        if (feedType == "FCP" || feedType == "FP" || feedType == "FC")
                        {
                            result.Name = profile;
                            result.Url = string.Format("https://www.facebook.com/{0}", profile);
                            result.NumOfFollowers = 0;
                            result.NumOfFollowing = 0;
                            result.UserStatus = "";
                            if (isPage)
                            {
                                result.TotalTweets = db.PostsFBs.Where(x => x.PageId == onProfile && x.AuthorUserId == profile).ToList().Count();
                                result.TotalTweets += db.CommentsFBs.Where(x => x.FromId == profile).ToList().Count();
                                result.TotalTweets += db.PageChatsFBs.Where(x => x.FromId == profile && x.PageId == onProfile).ToList().Count();
                            }
                            else
                            {
                                result.TotalTweets = db.PostsFBs.Where(x => x.UserId == onProfile && x.AuthorUserId == profile).ToList().Count();
                                result.TotalTweets += db.CommentsFBs.Where(x => x.FromId == profile).ToList().Count();
                            }

                            consumerProfile = db.CustomerSocialProfiles.Where(x => x.ExternalId == profile).FirstOrDefault();
                            if (consumerProfile != null)
                            {
                                result.Name = !String.IsNullOrEmpty(consumerProfile.Name) ? consumerProfile.Name : result.Name;
                                result.IBC = consumerProfile.KECustomers != null ? string.Join(",", consumerProfile.KECustomers.Select(x => x.KEArea != null ? x.KEArea.Name : "")) : string.Empty;
                                if (isPage)
                                {
                                    if (consumerProfile.ProfileToCustSocialProfiles != null)
                                    {
                                        int pageId = db.ProfilePages.Where(x => x.ProfilePageID == onProfile).FirstOrDefault().Id;
                                        var consumerMapping = consumerProfile.ProfileToCustSocialProfiles.Where(x => x.ProfilePageId == pageId).FirstOrDefault();
                                        if (consumerMapping != null)
                                        {
                                            var userStatus = consumerMapping.WarnBanCustomers.FirstOrDefault();
                                            if (userStatus != null)
                                            {
                                                result.UserStatus = userStatus.ProfileStatus.StatusName;
                                                result.WarnBanId = userStatus.Id;
                                                if (userStatus.WarnBanCustomerFeed != null)
                                                {
                                                    var feedDetail = await GetFeedDetail(userStatus.WarnBanCustomerFeed.FeedId, userStatus.WarnBanCustomerFeed.FeedType);
                                                    result.PostUrl = feedDetail.FeedUrl;
                                                    result.PostText = feedDetail.Text;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    if (consumerProfile.ProfileToCustSocialProfiles != null)
                                    {
                                        var consumerMapping = consumerProfile.ProfileToCustSocialProfiles.Where(x => x.Profile.ProfileExternalId == onProfile).FirstOrDefault();
                                        if (consumerMapping != null)
                                        {
                                            var userStatus = consumerMapping.WarnBanCustomers.FirstOrDefault();
                                            if (userStatus != null)
                                            {
                                                result.UserStatus = userStatus.ProfileStatus.StatusName;
                                                result.WarnBanId = userStatus.Id;
                                                if (userStatus.WarnBanCustomerFeed != null)
                                                {
                                                    var feedDetail = await GetFeedDetail(userStatus.WarnBanCustomerFeed.FeedId, userStatus.WarnBanCustomerFeed.FeedType);
                                                    result.PostUrl = feedDetail.FeedUrl;
                                                    result.PostText = feedDetail.Text;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (consumerProfile.KECustomers != null)
                                {
                                    result.MobileNum = string.Join(", ", consumerProfile.KECustomers.Select(x => x.MobileNum));
                                    result.AccountNum = string.Join(", ", consumerProfile.KECustomers.Select(x => x.ContractAccount));
                                    result.Address = string.Join("| ", consumerProfile.KECustomers.Select(x => x.Address));
                                    result.Feeder = consumerProfile.KECustomers.Select(x => x.Feeder).FirstOrDefault();
                                    result.PMT = consumerProfile.KECustomers.Select(x => x.PMT).FirstOrDefault();
                                }
                            }
                        }
                        else// Twitter
                        {
                            result.Name = profile;
                            result.Url = string.Format("https://www.twitter.com/{0}", profile);
                            result.NumOfFollowers = 0;
                            result.NumOfFollowing = 0;
                            result.UserStatus = "";
                            TwitterUtility twitterUtil = new TwitterUtility(onProfile);
                            var userInfo = await twitterUtil.getTwitterProfile(profile);
                            if (userInfo != null)
                            {
                                result.NumOfFollowers = userInfo.FollowersCount;
                                result.NumOfFollowing = userInfo.FriendsCount;
                            }

                            if (feedType == "TP")
                                result.TotalTweets = db.PostsTwitters.Where(x => x.AuthorName == profile && x.InReplyToScreenName == onProfile).ToList().Count();
                            else
                                result.TotalTweets = db.MessagesTwitters.Where(x => x.SenderScreenName == profile && x.RecipientScreenName == onProfile).ToList().Count();

                            consumerProfile = db.CustomerSocialProfiles.Where(x => x.UserName == profile).FirstOrDefault();
                            if (consumerProfile != null)
                            {
                                result.Name = !String.IsNullOrEmpty(consumerProfile.Name) ? consumerProfile.Name : result.Name;
                                result.IBC = consumerProfile.KECustomers != null ? string.Join(", ", consumerProfile.KECustomers.Select(x => x.KEArea != null ? x.KEArea.Name : "")) : string.Empty;

                                if (consumerProfile.ProfileToCustSocialProfiles != null)
                                {
                                    var consumerMapping = consumerProfile.ProfileToCustSocialProfiles.Where(x => x.Profile.UserName == onProfile).FirstOrDefault();
                                    if (consumerMapping != null)
                                    {
                                        var userStatus = consumerMapping.WarnBanCustomers.FirstOrDefault();
                                        if (userStatus != null)
                                        {
                                            result.UserStatus = userStatus.ProfileStatus.StatusName;
                                            result.WarnBanId = userStatus.Id;
                                            if (userStatus.WarnBanCustomerFeed != null)
                                            {
                                                var feedDetail = await GetFeedDetail(userStatus.WarnBanCustomerFeed.FeedId, userStatus.WarnBanCustomerFeed.FeedType);
                                                result.PostUrl = feedDetail.FeedUrl;
                                                result.PostText = feedDetail.Text;
                                            }
                                        }
                                    }
                                }

                                if (consumerProfile.KECustomers != null)
                                {
                                    result.MobileNum = string.Join(", ", consumerProfile.KECustomers.Select(x => x.MobileNum));
                                    result.AccountNum = string.Join(", ", consumerProfile.KECustomers.Select(x => x.ContractAccount));
                                    result.Address = string.Join("| ", consumerProfile.KECustomers.Select(x => x.Address));
                                    result.Feeder = consumerProfile.KECustomers.Select(x => x.Feeder).FirstOrDefault();
                                    result.PMT = consumerProfile.KECustomers.Select(x => x.PMT).FirstOrDefault();
                                }
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    result = null;
                }
            });
            return result;
        }

        public async Task<FeedDetail> GetFeedDetail(int feedId, string feedType)
        {
            FeedDetail feedModel = new FeedDetail();
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        switch (feedType)
                        {
                            case "TP":
                                var twPostInfo = db.PostsTwitters.Where(x => x.Id == feedId).FirstOrDefault();
                                if (twPostInfo != null)
                                {
                                    feedModel.Name = twPostInfo.AuthorName;
                                    feedModel.ProfileUrl = string.Format("https://twitter.com/{0}", twPostInfo.AuthorName);
                                    feedModel.FeedUrl = string.Format("https://twitter.com/{0}/status/{1}", twPostInfo.AuthorName, twPostInfo.StatusId);
                                    feedModel.Text = twPostInfo.StatusText;
                                }
                                break;
                            case "TM":
                                var twMsgInfo = db.MessagesTwitters.Where(x => x.Id == feedId).FirstOrDefault();
                                if (twMsgInfo != null)
                                {
                                    feedModel.Name = twMsgInfo.SenderScreenName;
                                    feedModel.ProfileUrl = string.Format("https://twitter.com/{0}", twMsgInfo.SenderScreenName);
                                    feedModel.FeedUrl = string.Format("https://twitter.com/messages");
                                    feedModel.Text = twMsgInfo.MsgText;
                                }
                                break;
                            case "FP":
                                var fbPostInfo = db.PostsFBs.Where(x => x.Id == feedId).FirstOrDefault();
                                if (fbPostInfo != null)
                                {
                                    feedModel.Name = fbPostInfo.AuthorUserName;
                                    feedModel.ProfileUrl = string.Format("https://www.facebook.com/{0}", fbPostInfo.AuthorUserId);
                                    feedModel.FeedUrl = string.Format("https://www.facebook.com/{0}", fbPostInfo.PostId);
                                    feedModel.Text = fbPostInfo.Message;
                                }
                                break;
                            case "FC":
                                var fbCommentInfo = db.CommentsFBs.Where(x => x.Id == feedId).FirstOrDefault();
                                if (fbCommentInfo != null)
                                {
                                    feedModel.Name = fbCommentInfo.FromName;
                                    feedModel.ProfileUrl = string.Format("https://www.facebook.com/{0}", fbCommentInfo.FromId);
                                    feedModel.FeedUrl = string.Format("https://www.facebook.com/{0}", fbCommentInfo.CommentId);
                                    feedModel.Text = fbCommentInfo.Message;
                                }
                                break;
                            case "FPC":
                                var fbChatInfo = db.PageChatsFBs.Where(x => x.Id == feedId).FirstOrDefault();
                                if (fbChatInfo != null)
                                {
                                    feedModel.Name = fbChatInfo.FromName;
                                    feedModel.ProfileUrl = string.Format("https://www.facebook.com/{0}", fbChatInfo.FromId);
                                    feedModel.FeedUrl = string.Format("https://www.facebook.com/{0}/messages", fbChatInfo.ToId);
                                    feedModel.Text = fbChatInfo.MsgText;
                                }
                                break;
                            default:
                                break;
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                feedModel = null;
            }
            return feedModel;
        }

        public async Task<List<KeyValuePair<int, int>>> GetColumnsOrderForResponder(string userId, int companyId)
        {
            var result = new List<KeyValuePair<int, int>>();
            List<UserAccessLevel> lstUserAccessLevel = new List<UserAccessLevel>();
            int _userId = Convert.ToInt32(userId);
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        int teamMemberId = db.Teams.Where(x => x.UserId == _userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();
                        lstUserAccessLevel = db.UserAccessLevels.Where(x => x.TeamMemberId == teamMemberId).OrderBy(x=>x.OrderId).ToList();

                        foreach(UserAccessLevel item in lstUserAccessLevel)
                        {
                            result.Add(new KeyValuePair<int, int>(item.OrderId, item.AccessLevelId));
                        }
                    }
                });
            }
            catch (Exception ex)
            {                
            }
            return result;
        }

        public async Task<List<KeyValuePair<int, int>>> UpdateColumnsOrderForResponder(int userId, int companyId, List<KeyValuePair<int, int>> receivedOrder)
        {
            var result = new List<KeyValuePair<int, int>>();
            List<UserAccessLevel> lstUserAccessLevel = new List<UserAccessLevel>();            
            try
            {
                await Task.Run(() =>
                {
                    using (SocialCRMEntities db = new SocialCRMEntities())
                    {
                        int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();
                        lstUserAccessLevel = db.UserAccessLevels.Where(x => x.TeamMemberId == teamMemberId).OrderBy(x => x.OrderId).ToList();

                        foreach(UserAccessLevel item in lstUserAccessLevel)
                        {
                            foreach(KeyValuePair<int,int> innerItem in receivedOrder)
                            {
                                if(item.AccessLevelId == innerItem.Value && item.OrderId != innerItem.Key)
                                {
                                    item.OrderId = innerItem.Key;
                                    UserAccessLevel userAccess = db.UserAccessLevels.Where(x => x.TeamMemberId == teamMemberId && x.AccessLevelId == item.AccessLevelId).FirstOrDefault();
                                    userAccess.OrderId = innerItem.Key;
                                    db.Entry(userAccess).State = EntityState.Modified;
                                }
                            }
                        }
                        db.SaveChanges();
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