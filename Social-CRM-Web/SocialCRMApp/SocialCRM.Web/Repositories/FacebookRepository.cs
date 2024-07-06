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
using SocialCRM.Web.Models.Facebook;
using System.Data.Entity;
using SocialAPIsWrapper.APIs;
using Facebook;
using SocialCRM.Web.Models.UnifiedInbox;
using CRM.Common.Extensions;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Transactions;

namespace SocialCRM.Web.Repositories
{
    public class FacebookRepository : IFacebookRepository
    {
        public List<FacebookPageModel> GetUserFBPages(int companyId, int groupId, int profileTransactId)
        {
            List<FacebookPageModel> pages = null;
            using (var context = new SocialCRMEntities())
            {
                pages = context.ProfilePages.Where(z => z.ProfileTransactionId == profileTransactId).Select(pp => new FacebookPageModel
                {
                    AccountId = 0,
                    AccountName = pp.PageName,
                    AccountUrl = pp.PageUrl,
                    IsConnected = pp.ProfileComapnyGroups.Any(t => t.CompanyId == companyId && t.GroupId == groupId && t.ProfilePageId == pp.Id),
                    ProfileId = pp.ProfilePageID,
                    Id = pp.Id
                }).ToList();
            }
            return pages;
        }

        public List<FacebookPageModel> ActivateFacebookPages(List<FacebookPageModel> pages, int companyId, int groupId, int userId)
        {
            using (var context = new SocialCRMEntities())
            {
                List<ProfileComapnyGroup> profileCompanyGroups = new List<ProfileComapnyGroup>();
                var filterIds = pages.Where(s => s.IsConnected == true).Select(x => x.Id);
                foreach (var profilePageId in filterIds)
                {
                    bool hasValue = context.ProfileComapnyGroups.Any(t => t.CompanyId == companyId && t.GroupId == groupId && t.ProfilePageId == profilePageId);

                    if (!hasValue)
                    {
                        ProfileComapnyGroup profileGroupObj = new ProfileComapnyGroup();
                        profileGroupObj.CompanyId = companyId;
                        profileGroupObj.GroupId = groupId;
                        profileGroupObj.ProfilePageId = profilePageId;
                        profileGroupObj.Teams.Add(context.Teams.Where(x => x.UserId == userId).FirstOrDefault());
                        profileCompanyGroups.Add(profileGroupObj);
                    }
                }

                context.ProfileComapnyGroups.AddRange(profileCompanyGroups);
                var teams = context.Teams.Where(t => t.CompanyId == companyId && t.AspNetUser.AspNetRoles.Any(y => y.Name == "Admin")).ToList();
                foreach (Team team in teams)
                {
                    team.ProfileComapnyGroups = profileCompanyGroups;
                }
                context.SaveChanges();
            }
            return pages;
        }

        public async Task<JsonObject> FacebookLikePost(string userId, bool isPage, string postId, bool isPost = true)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            return await utility.likePost(postId, isPost);
            /*using (var context = new SocialCRMEntities())
            {
                MiningRepository rep = new MiningRepository();
                int feedId = 0;
                if(feedType == "FP")
                {
                    feedId = context.PostsFBs.Where(x => x.PostId == postId).FirstOrDefault().Id;
                }
                else if(feedType == "FC")
                {
                    feedId = context.CommentsFBs.Where(x => x.CommentId == postId).FirstOrDefault().Id;
                }

                if (isPage)
                {
                    int ProfilePageId = context.ProfilePages.Where(x => x.ProfilePageID == userId).FirstOrDefault().Id;
                    result = await rep.AddSocialActivity(null, ProfilePageId, feedId, feedType, 1);
                }
                else
                {
                    int ProfileId = context.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault().Id;
                    result = await rep.AddSocialActivity(ProfileId, null, feedId, feedType, 1);
                }
            }*/
        }
        public async Task<bool> FacebookIsPostLiked(string userId, bool isPage, string postId, bool isPost = true)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            return await utility.isPostLiked(postId, isPost);
        }

        public async Task<JsonObject> FacebookUnLikePost(string userId, bool isPage, string postId, bool isPost = true)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            return await utility.unlikePost(postId, isPost);
            /*using (var context = new SocialCRMEntities())
            {
                MiningRepository rep = new MiningRepository();
                await rep.RemoveSocialActivity(activityId);
            }*/
        }

        public async Task<JsonObject> FacebookCommentPost(string userId, bool isPage, string postId, string comment, string filePath, string contentType, string fileName, int teamId, int companyId, FacbookReplyType replyType)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);

            if (replyType == FacbookReplyType.NestedComment)
            {
                using (SocialCRMEntities db = new SocialCRMEntities())
                {
                    var mainComment = db.CommentsFBs.Where(x => x.CommentId == postId && x.FromId.Trim() != userId.Trim()).FirstOrDefault();
                    if (mainComment != null)
                    {
                        comment = "@["+mainComment.FromId+"] " + comment;
                    }
                }
            }

            var responseObj = await utility.newCommentPost(postId, comment, filePath, contentType, fileName);
            if (responseObj.ContainsKey("id"))
            {
                string commentId = responseObj["id"].ToString();
                string feedType = "";
                switch (replyType)
                {
                    case FacbookReplyType.Comment:
                        feedType = "FC";
                        break;
                    case FacbookReplyType.NestedComment:
                        feedType = "FCN";
                        using (SocialCRMEntities db = new SocialCRMEntities())
                        {
                            postId = db.CommentsFBs.Where(x => x.CommentId == postId).FirstOrDefault().PostId;
                        }
                        break;
                    default:
                        feedType = "N/A";
                        break;
                }
                if (!isPage)
                    await new MiningRepository().AddSocialActivity(userId, "", postId, feedType, 2, "", comment, teamId,companyId, "", "", commentId);
                else
                    await new MiningRepository().AddSocialActivity("", userId, postId, feedType, 2, "", comment, teamId,companyId, "", "", commentId);
            }
            return responseObj;
        }

        public async Task<JsonObject> FacebookNewPost(string userId, bool isPage, string status, string filePath, string contentType, string fileName, string placeId, int teamId, int companyId)
        {
            status = String.IsNullOrEmpty(status) ? string.Empty : status;
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            JsonObject response;
            if(filePath != "null" && filePath != null)
            {
                string picFBId = await utility.newPhotoUpload(userId, filePath,contentType, fileName);
                response = await utility.newPost(userId, status, picFBId, placeId);
            }
            else
            {
                response = await utility.newPost(userId, status, null, placeId);
            }

            if (response.ContainsKey("id"))
            {
                string postId = response["id"].ToString();
                if (!isPage)
                    await new MiningRepository().AddSocialActivity(userId, "", postId, "FP", 3, "", status, teamId, companyId, "", "", "");
                else
                    await new MiningRepository().AddSocialActivity("", userId, postId, "FP", 3, "", status, teamId, companyId, "", "", "");
            }

            return response;
        }

        public async Task<JsonObject> GetFacebookPlaces(string userId, bool isPage, string locationQuery)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            return await utility.getPlaces(locationQuery);
        }

        public async Task<JsonObject> FacebookNewPageChat(string userId, string conId, bool isPage, string status, int teamId,int companyId)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            var responseObj = await utility.newPageChat(conId, status);
            if (responseObj.ContainsKey("id"))
            {
                string msgId = responseObj["id"].ToString();
                await new MiningRepository().AddSocialActivity("", userId, conId, "FCP", 2, "", status, teamId,companyId, "", "", msgId);
            }
            return responseObj;
        }

        public async Task<JsonObject> FacebookDeleteObject(string userId, bool isPage, string objectId)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            return await utility.deleteObject(objectId);
        }

        public async Task<JsonObject> FacebookUpdateMessage(string userId, bool isPage, string objectId, Dictionary<string, object> dict, string msgType)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            var response = await utility.updateObject(objectId, dict);
            using (var context = new SocialCRMEntities())
            {
                if (msgType == "FP")
                {
                    PostsFB updatedPost = context.PostsFBs.Where(x => x.PostId == objectId).FirstOrDefault();
                    updatedPost.Message = dict["message"].ToString();
                }
                else if (msgType == "FC")
                {
                    CommentsFB updatedComment = context.CommentsFBs.Where(x => x.CommentId == objectId).FirstOrDefault();
                    updatedComment.Message = dict["message"].ToString();
                }
                await context.SaveChangesAsync();
            }

            return response;
        }

        public async Task<JsonObject> FacebookHideUnhidePost(string userId, bool isPage, string objectId, Dictionary<string, object> dict)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            var response = await utility.updateObject(objectId, dict);

            return response;
        }

        public async Task<bool> FacebookIsPostHidden(string userId, bool isPage, string postId)
        {
            FacebookUtility utility = new FacebookUtility(userId, isPage);
            return await utility.isPostHidden(postId);
        }

        public async Task<List<InboxItemModel>> FacebookGetUserConversation(string conId)
        {
           // List<FBPostModel> result = new List<FBPostModel>();
            List<InboxItemModel> result = new List<InboxItemModel>();
            await Task.Run(() =>
            {
                using (new TransactionScope(
                                        TransactionScopeOption.Required,
                                        new TransactionOptions
                                        {
                                            IsolationLevel = IsolationLevel.ReadUncommitted
                                        }))
                {
                    using (var context = new SocialCRMEntities())
                    {
                        //var boom = context.PageChatsFBs.Where(x => x.ConId == conId).OrderByDescending(z => z.CreatedDate).Take(50).ToList();

                        //foreach (var x in tempArray)
                        //{
                        //    result.Add(new FBPostModel
                        //    {
                        //        Id = x.Id.ToString(),
                        //        StatusId = x.MsgId,
                        //        AuhtorId = x.FromId,
                        //        AuthorName = x.FromName,
                        //        AuthorProfilePic = x.FromProfilePic,
                        //        Msg = x.MsgText,
                        //        MsgType = "FCP",
                        //        ConversationDate = DatetimeExtensions.formatMsgDatetime(x.CreatedDate.Value),
                        //        ConversationDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck(x.CreatedDate.Value),
                        //        ToUserId = x.ToId,
                        //        ReplyedByUserId = x.FromId
                        //    });
                        //}

                        var temp = (from pFC in context.PageChatsFBs
                                     where pFC.ConId == conId
                                    join media in context.FeedsMediaAttachments on pFC.Id equals media.MediaId into med
                                    from y in med.Where(x => x.MediaType == "PCF").DefaultIfEmpty() 
                                    join taskComp in context.TaskCompleteds on pFC.Id equals taskComp.ActivityId into gj
                                    from x in gj.Where(x => x.ActivityTable == "FCP").DefaultIfEmpty()
                                    orderby pFC.CreatedDate descending
                                    select new FBPostModel
                                                       {
                                                           Id = pFC.Id.ToString(),
                                                           StatusId = pFC.MsgId,
                                                           ConId = pFC.ConId,
                                                           AuhtorId = pFC.FromId,
                                                           AuthorName = pFC.FromName,
                                                           AuthorProfilePic = pFC.FromProfilePic,
                                                           Msg = pFC.MsgText,
                                                           MsgType = "FCP",
                                                           ConversationDate = pFC.CreatedDate.ToString(), 
                                                           ConversationDateForDeck = pFC.CreatedDate.ToString(), 
                                                           ToUserId = pFC.ToId,
                                                           ReplyedByUserId = pFC.FromId,
                                                           IsCompleted = x.IsCompleted,
                                                           Images = y.AttachmentUrl,
                                                           ImageType = y.Description
                                    }).Take(50).ToList();

                        foreach (var x in temp)
                        {
                            var item = new InboxItemModel //FBPostModel
                            {
                                Id = Convert.ToInt32(x.Id),
                                StatusId = x.StatusId,
                                ThreadId = x.ConId.Split('_')[1],
                                UserId = x.AuhtorId,  //AuhtorId = x.AuhtorId,
                                ReplyedBy = x.AuthorName,
                                User = x.AuthorName,//AuthorName = x.AuthorName,
                                TwtImg = x.AuthorProfilePic,//AuthorProfilePic = x.AuthorProfilePic,
                                UserTwit = x.Msg, // Msg = x.Msg,
                                MessageType = x.MsgType, //MsgType = x.MsgType,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime(Convert.ToDateTime(x.ConversationDate)), //ConversationDate = DatetimeExtensions.formatMsgDatetime(Convert.ToDateTime(x.ConversationDate)),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck(Convert.ToDateTime(x.ConversationDateForDeck)),//ConversationDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck(Convert.ToDateTime(x.ConversationDateForDeck)),
                                ToUserId = x.ToUserId,
                                ReplyedByUserId = x.ReplyedByUserId,
                                IsCompleted = x.IsCompleted,
                                Urls = String.IsNullOrEmpty(x.Images) ? new List<string>() : (x.ImageType + '|' + x.Images).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList<string>()
                            };
                            UnifiedInboxUtility.CheckForAttachments(ref item);
                            result.Add(item);
                        }
                    }
                }
            });

            return result;
        }

        public async Task<FacebookPageReportModel> FacebookGetPageReportStats(string pageId, DateTime from, DateTime to)
        {
            try
            {
                DateTime fromDateTemp = from.Date;
                DateTime toDateTemp = to.Date.Add(new TimeSpan(23, 59, 59));
                from = from.AddDays(1);
                to = to.AddDays(2);
                DateTime lastFrom = from.AddDays((from - to).Days);
                FacebookUtility utility = new FacebookUtility(pageId, true);
                FacebookPageReportModel fbPostReportObj = new FacebookPageReportModel();
                JsonArray tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_fan_adds", "day");
                fbPostReportObj.PageLikeSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.TotalPageLikes = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        DateWiseActivityCount newTemp = new DateWiseActivityCount();
                        newTemp.ActivityCount = tempValue.ContainsKey("value") ? Convert.ToInt32(tempValue["value"]) : 0;
                        newTemp.DateValue = tempValue.ContainsKey("end_time") ? Convert.ToDateTime(tempValue["end_time"]) : from;

                        fbPostReportObj.PageLikeSpan.Add(newTemp);
                        fbPostReportObj.TotalPageLikes += newTemp.ActivityCount;
                    }
                }
                tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_fan_removes", "day");
                fbPostReportObj.PageUnlikeSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.TotalPageUnLikes = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        DateWiseActivityCount newTemp = new DateWiseActivityCount();
                        newTemp.ActivityCount = tempValue.ContainsKey("value") ? Convert.ToInt32(tempValue["value"]) : 0;
                        newTemp.DateValue = tempValue.ContainsKey("end_time") ? Convert.ToDateTime(tempValue["end_time"]) : from;

                        fbPostReportObj.PageUnlikeSpan.Add(newTemp);
                        fbPostReportObj.TotalPageUnLikes += newTemp.ActivityCount;
                    }
                }
                tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_positive_feedback_by_type", "day");
                fbPostReportObj.PostLikeSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.PostCommentSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.PostShareSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.TotalPostLikes = 0;
                fbPostReportObj.TotalPostComments = 0;
                fbPostReportObj.TotalPostShares = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        DateWiseActivityCount newTemp = new DateWiseActivityCount();
                        JsonObject stats = tempValue["value"] as JsonObject;
                        if (stats != null)
                        {
                            newTemp.ActivityCount = stats.ContainsKey("like") ? Convert.ToInt32(stats["like"]):0;
                            newTemp.DateValue = tempValue.ContainsKey("end_time") ? Convert.ToDateTime(tempValue["end_time"]):from;

                            fbPostReportObj.PostLikeSpan.Add(newTemp);
                            fbPostReportObj.TotalPostLikes += newTemp.ActivityCount;

                            newTemp = new DateWiseActivityCount();
                            newTemp.ActivityCount = stats.ContainsKey("comment") ? Convert.ToInt32(stats["comment"]):0;
                            newTemp.DateValue = tempValue.ContainsKey("end_time") ? Convert.ToDateTime(tempValue["end_time"]):from;

                            fbPostReportObj.PostCommentSpan.Add(newTemp);
                            fbPostReportObj.TotalPostComments += newTemp.ActivityCount;

                            newTemp = new DateWiseActivityCount();
                            newTemp.ActivityCount = stats.ContainsKey("link") ? Convert.ToInt32(stats["link"]):0;
                            newTemp.DateValue = tempValue.ContainsKey("end_time") ? Convert.ToDateTime(tempValue["end_time"]):from;

                            fbPostReportObj.PostShareSpan.Add(newTemp);
                            fbPostReportObj.TotalPostShares += newTemp.ActivityCount;
                        }
                    }
                }
                //New Implementation starts 
                //Page Impressions : Total People who viewed
                tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_impressions", "day");
                fbPostReportObj.TotalPeopleWhoViewedSpan = new List<DateWiseActivityCount>();
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        DateWiseActivityCount newTemp = new DateWiseActivityCount();
                        newTemp.ActivityCount = tempValue.ContainsKey("value") ? Convert.ToInt32(tempValue["value"]):0;
                        newTemp.DateValue = tempValue.ContainsKey("end_time") ? Convert.ToDateTime(tempValue["end_time"]):from;
                        fbPostReportObj.TotalPeopleWhoViewedSpan.Add(newTemp);
                    }
                }
                //Reactions span
                fbPostReportObj.pageReactionsSpan = new List<pageReactionsTotal>();
                tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_actions_post_reactions_total", "day");
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        pageReactionsTotal newTemp = new pageReactionsTotal();
                        string reactionsString = tempValue[0].ToString();
                        var receivedReactionsObj = JObject.Parse(reactionsString);
                        if (receivedReactionsObj != null)
                        {
                            newTemp.like = receivedReactionsObj.ContainsKey("like") ? Convert.ToInt32(receivedReactionsObj["like"].ToString()) : 0;
                            newTemp.love = receivedReactionsObj.ContainsKey("love") ? Convert.ToInt32(receivedReactionsObj["love"].ToString()) : 0;
                            newTemp.wow = receivedReactionsObj.ContainsKey("wow") ? Convert.ToInt32(receivedReactionsObj["wow"].ToString()) : 0;
                            newTemp.haha = receivedReactionsObj.ContainsKey("haha") ? Convert.ToInt32(receivedReactionsObj["haha"].ToString()) : 0;
                            newTemp.sorry = receivedReactionsObj.ContainsKey("sorry") ? Convert.ToInt32(receivedReactionsObj["sorry"].ToString()) : 0;
                            newTemp.anger = receivedReactionsObj.ContainsKey("anger") ? Convert.ToInt32(receivedReactionsObj["anger"].ToString()) : 0;
                            if (receivedReactionsObj.ContainsKey("end_time"))
                                newTemp.totalReactionsDateValue = Convert.ToDateTime(tempValue["end_time"]);
                            fbPostReportObj.pageReactionsSpan.Add(newTemp);
                        }
                    }
                }
                //Page Reachability
                tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_posts_impressions_unique", "day");
                fbPostReportObj.pageReachSpan = new List<DateWiseActivityCount>();
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        DateWiseActivityCount newTemp = new DateWiseActivityCount();
                        newTemp.ActivityCount = tempValue.ContainsKey("value") ? Convert.ToInt32(tempValue["value"]) : 0;
                        if (tempValue.ContainsKey("end_time"))
                            newTemp.DateValue = Convert.ToDateTime(tempValue["end_time"]);
                        fbPostReportObj.pageReachSpan.Add(newTemp);
                    }
                }
                //Recent Posts
                var context = new SocialCRMEntities();                
                List<recentPost> lstRecentPosts = (from pFB in context.PostsFBs
                                        where pFB.PageId == pageId
                                        && pFB.AuthorUserId == pageId
                                        && pFB.CreatedDate >= fromDateTemp
                                        && pFB.CreatedDate <= toDateTemp
                                        join mA in context.FeedsMediaAttachments
                                        on pFB.Id equals mA.MediaId into gj
                                        from mA in gj.DefaultIfEmpty()
                                        orderby pFB.CreatedDate descending
                                        select new recentPost
                                        {
                                            Id = pFB.Id,
                                            pictureURL = mA.AttachmentUrl,
                                            messageText = pFB.Message,
                                            postId = pFB.PostId,
                                            createdDate = pFB.CreatedDate?? DateTime.Now,                                            
                                            type = (mA == null ? "Text" : (mA.Description.Contains("video") ? "Video" : (mA.Description.Contains("photo") ? "Photo" : "Text"))),
                                            postGrade = pFB.PostGrade
                                        }).Take(5).ToList();
                
                foreach (recentPost post in lstRecentPosts)
                {
                    string postIdOfRecentPost = post.postId;

                    post.strCreatedDate = post.createdDate.ToString("dd/MM/yyyy");
                    post.strCreatedTime = post.createdDate.ToString("hh:mm tt");
                    
                    JsonObject reachabilityAndConsumption = await utility.getPostReachabilityAndConsumption(postIdOfRecentPost);
                    int Reachability = 0, LinkClicks = 0, EngagedUsers = 0;

                    if (reachabilityAndConsumption != null && reachabilityAndConsumption.ContainsKey("insights"))
                    {
                        Reachability = Convert.ToInt32(((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[0] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"] as JsonObject)["total"]);
                        LinkClicks = Convert.ToInt32((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[1] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"]);
                        EngagedUsers = Convert.ToInt32((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[2] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"]);
                    }

                    post.postReach = Reachability;
                    post.postClicks = LinkClicks;

                    JsonObject reactionsCommentsSharesObj = await utility.getReactionsCommentsShares(postIdOfRecentPost);
                    int Reactions = 0, Comments = 0, Sharedposts = 0;
                    if(reachabilityAndConsumption != null && reactionsCommentsSharesObj.ContainsKey("likes"))
                    {
                        Reactions = Convert.ToInt32(((reactionsCommentsSharesObj["likes"] as JsonObject)["summary"] as JsonObject)["total_count"]);
                    }

                    if (reachabilityAndConsumption != null && reactionsCommentsSharesObj.ContainsKey("comments"))
                    {
                        Comments = Convert.ToInt32(((reactionsCommentsSharesObj["comments"] as JsonObject)["summary"] as JsonObject)["total_count"]);
                    }

                    if (reachabilityAndConsumption != null && reactionsCommentsSharesObj.ContainsKey("shares"))
                    {
                        Sharedposts = Convert.ToInt32((reactionsCommentsSharesObj["shares"] as JsonObject)["count"]);
                    }

                    double engagementRate = 0;
                    if (Reachability != 0)
                        engagementRate = (double)EngagedUsers / Reachability;

                    if (EngagedUsers > Reachability)
                    {
                        engagementRate = 1;
                    }

                    post.postReactions = Reactions;
                    post.postComments = Comments;
                    post.postShares = Sharedposts;
                    post.engagementRate = Math.Round(engagementRate * 100);                    
                }
                fbPostReportObj.lstRecentPosts = lstRecentPosts;
                //fbPostReportObj.loadMoreRecentPostsLink = _pagging["next"].ToString();
                if(lstRecentPosts.Count>0)
                    fbPostReportObj.lastPostId = lstRecentPosts[lstRecentPosts.Count - 1].Id;

                //External References
                List<externalLink> lstExternalLinks = new List<externalLink>();
                tempArray = await utility.getPageReportStats(pageId, from.AddDays(-2).Date, to.AddDays(-2).Date, "page_views_external_referrals", "day");
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        if (tempValue.ContainsKey("value"))
                        {
                            JsonObject arrValues = tempValue["value"] as JsonObject;
                            //Code to Test
                            foreach (KeyValuePair<string, object> obj in arrValues.ToArray())
                            {
                                bool containsItem = false;
                                if (obj.Key.Contains("http"))
                                {
                                    Uri url = new Uri(obj.Key);
                                    containsItem = lstExternalLinks.Any(x => x.link == url.Host);
                                    if (!containsItem)
                                    {
                                        externalLink exLinkObj = new externalLink();
                                        exLinkObj.link = url.Host;//obj.Key;

                                        exLinkObj.count = Convert.ToInt32(obj.Value);
                                        lstExternalLinks.Add(exLinkObj);
                                    }
                                    else
                                    {
                                        //Update count
                                        lstExternalLinks.Where(x => x.link == url.Host).First().count += Convert.ToInt32(obj.Value);
                                    }
                                }
                                else //internal facebook
                                {
                                    containsItem = false;
                                    containsItem = lstExternalLinks.Any(x => x.link == "www.facebook.com");

                                    if (!containsItem)
                                    {
                                        externalLink exLinkObj = new externalLink();
                                        exLinkObj.link = "www.facebook.com";
                                        exLinkObj.count = Convert.ToInt32(obj.Value);
                                        lstExternalLinks.Add(exLinkObj);
                                    }
                                    else
                                    {
                                        //Update count
                                        lstExternalLinks.Where(x => x.link == "www.facebook.com").First().count += Convert.ToInt32(obj.Value);
                                    }
                                }



                            }
                        }
                    }
                }
                //Sending it to view
                fbPostReportObj.lstExternalReferrals = lstExternalLinks.OrderByDescending(x=>x.count).Take(10).ToList();

                //New implementation area ends
                tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_stories_by_story_type", "day");
                fbPostReportObj.PublishedPagePostSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.PublishedUserPostSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.TotalPublishedPagePosts = 0;
                fbPostReportObj.TotalPublishedUserPosts = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        DateWiseActivityCount newTemp = new DateWiseActivityCount();
                        JsonObject stats = tempValue["value"] as JsonObject;
                        if (stats != null)
                        {
                            newTemp.ActivityCount = stats.ContainsKey("page post") ? Convert.ToInt32(stats["page post"]) : 0;
                            if (tempValue.ContainsKey("end_time"))
                                newTemp.DateValue = Convert.ToDateTime(tempValue["end_time"]);

                            fbPostReportObj.PublishedPagePostSpan.Add(newTemp);
                            fbPostReportObj.TotalPublishedPagePosts += newTemp.ActivityCount;

                            newTemp = new DateWiseActivityCount();
                            newTemp.ActivityCount = stats.ContainsKey("user post") ? Convert.ToInt32(stats["user post"]) : 0;
                            if (tempValue.ContainsKey("end_time"))
                                newTemp.DateValue = Convert.ToDateTime(tempValue["end_time"]);

                            fbPostReportObj.PublishedUserPostSpan.Add(newTemp);
                            fbPostReportObj.TotalPublishedUserPosts += newTemp.ActivityCount;
                        }
                    }
                }

                tempArray = await utility.getPageReportStats(pageId, from.Date, to.Date, "page_consumptions_by_consumption_type", "day");
                fbPostReportObj.LinkClicksSpan = new List<DateWiseActivityCount>();
                fbPostReportObj.TotalLinkClicks = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        DateWiseActivityCount newTemp = new DateWiseActivityCount();
                        JsonObject stats = tempValue["value"] as JsonObject;
                        if (stats != null)
                        {
                            newTemp.ActivityCount = stats.ContainsKey("link clicks") ? Convert.ToInt32(stats["link clicks"]) : 0;
                            if (tempValue.ContainsKey("end_time"))
                                newTemp.DateValue = Convert.ToDateTime(tempValue["end_time"]);

                            fbPostReportObj.LinkClicksSpan.Add(newTemp);
                            fbPostReportObj.TotalLinkClicks += newTemp.ActivityCount;
                        }
                    }
                }
                tempArray = await utility.getPageReportStats(pageId, to.AddDays(-3).Date, to.AddDays(-3).Date, "page_fans_gender_age", "lifetime");
                fbPostReportObj.PageFansGenderAge = new List<DemographicModel>();
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        JsonObject stats = tempValue["value"] as JsonObject;
                        if (stats != null)
                        {
                            foreach (string key in stats.Keys)
                            {
                                String[] arr = key.Split('.');
                                DemographicModel demoIns = fbPostReportObj.PageFansGenderAge.Where(x => x.AgeRange == arr[1]).FirstOrDefault();
                                if (demoIns != null)
                                {
                                    if (arr[0] == "M")
                                        demoIns.MaleCount = Convert.ToInt32(stats[key]);
                                    else if (arr[0] == "F")
                                        demoIns.FemaleCount = Convert.ToInt32(stats[key]);
                                }
                                else
                                {
                                    DemographicModel newTemp = new DemographicModel();
                                    newTemp.AgeRange = arr[1];
                                    if (arr[0] == "M")
                                    {
                                        newTemp.MaleCount = Convert.ToInt32(stats[key]);
                                        newTemp.FemaleCount = 0;
                                    }
                                    else if (arr[0] == "F")
                                    {
                                        newTemp.FemaleCount = Convert.ToInt32(stats[key]);
                                        newTemp.MaleCount = 0;
                                    }

                                    fbPostReportObj.PageFansGenderAge.Add(newTemp);
                                }
                            }
                        }
                    }
                }
                tempArray = await utility.getPageReportStats(pageId, lastFrom.Date, from.Date, "page_fan_adds", "day");
                fbPostReportObj.LastTotalPageLikes = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        fbPostReportObj.LastTotalPageLikes = tempValue.ContainsKey("value") ? Convert.ToInt32(tempValue["value"]) : 0;
                    }
                }
                tempArray = await utility.getPageReportStats(pageId, lastFrom.Date, from.Date, "page_positive_feedback_by_type", "day");
                fbPostReportObj.LastTotalPostLikes = 0;
                fbPostReportObj.LastTotalPostComments = 0;
                fbPostReportObj.LastTotalPostShares = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        JsonObject stats = tempValue["value"] as JsonObject;
                        if (stats != null)
                        {
                            fbPostReportObj.LastTotalPostLikes += (stats.ContainsKey("like") ? Convert.ToInt32(stats["like"]) : 0);
                            fbPostReportObj.LastTotalPostComments += (stats.ContainsKey("comment") ? Convert.ToInt32(stats["comment"]) : 0);
                            fbPostReportObj.LastTotalPostShares += (stats.ContainsKey("link") ? Convert.ToInt32(stats["link"]) : 0);
                        }
                    }
                }

                tempArray = await utility.getPageReportStats(pageId, lastFrom.Date, from.Date, "page_stories_by_story_type", "day");
                fbPostReportObj.LastTotalPublishedPagePosts = 0;
                fbPostReportObj.LastTotalPublishedUserPosts = 0;
                if (tempArray != null)
                {
                    foreach (JsonObject tempValue in tempArray)
                    {
                        JsonObject stats = tempValue["value"] as JsonObject;
                        if (stats != null)
                        {
                            fbPostReportObj.LastTotalPublishedPagePosts += (stats.ContainsKey("page post") ? Convert.ToInt32(stats["page post"]) : 0);
                            fbPostReportObj.LastTotalPublishedUserPosts += (stats.ContainsKey("user post") ? Convert.ToInt32(stats["user post"]) : 0);
                        }
                    }
                }

                /*fbPostReportObj.TopFiveCustomers = (from m in context.CommentsFBs
                                   join com in context.CommentsFBs
                                   on m.CommentId equals com.PostId
                                   join po in context.PostsFBs
                                   on m.PostId equals po.PostId
                                   join p in context.CustomerSocialProfiles
                                   on m.FromId equals p.ExternalId
                                   where po.PageId == pageId && p.Type == "Facebook" && m.CreatedDate >= fromDateTemp && m.CreatedDate <= toDateTemp
                                   group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender } into g
                                   select new FacebookTopCustomers
                                   {
                                       //UserId = g.Key.AuthorUserId,
                                       CustomerId = g.Key.ExternalId,
                                       UserName = g.Key.UserName,
                                       Name = g.Key.Name,
                                       ProfilePic = g.Key.ProfilePic,
                                       Gender = g.Key.Gender,
                                       Count = g.Count(),
                                       PostCount = 0,
                                       CommentCount = g.Count(),
                                       MsgCount = 0
                                   }
                ).Union(from m in context.PageChatsFBs
                        join p in context.CustomerSocialProfiles
                        on m.FromId equals p.ExternalId
                        where m.PageId == pageId && p.Type == "Facebook" && m.CreatedDate >= fromDateTemp && m.CreatedDate <= toDateTemp
                        group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender } into g
                        select new FacebookTopCustomers
                        {
                            //UserId = g.Key.AuthorUserId,
                            CustomerId = g.Key.ExternalId,
                            UserName = g.Key.UserName,
                            Name = g.Key.Name,
                            ProfilePic = g.Key.ProfilePic,
                            Gender = g.Key.Gender,
                            Count = g.Count(),
                            PostCount = 0,
                            CommentCount = 0,
                            MsgCount = g.Count()
                        }
                ).Union(from m in context.PostsFBs
                        join p in context.CustomerSocialProfiles
                        on m.AuthorUserId equals p.ExternalId
                        where m.PageId == pageId && p.Type == "Facebook" && m.CreatedDate >= fromDateTemp && m.CreatedDate <= toDateTemp
                        group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender } into g
                        select new FacebookTopCustomers
                        {
                            //UserId = g.Key.AuthorUserId,
                            CustomerId = g.Key.ExternalId,
                            UserName = g.Key.UserName,
                            Name = g.Key.Name,
                            ProfilePic = g.Key.ProfilePic,
                            Gender = g.Key.Gender,
                            Count = g.Count(),
                            PostCount = g.Count(),
                            CommentCount = 0,
                            MsgCount = 0
                        }
                ).Union(from m in context.CommentsFBs
                        join po in context.PostsFBs
                        on m.PostId equals po.PostId
                        join p in context.CustomerSocialProfiles
                        on m.FromId equals p.ExternalId
                        where po.PageId == pageId && p.Type == "Facebook" && m.CreatedDate >= fromDateTemp && m.CreatedDate <= toDateTemp
                        group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender } into g
                        select new FacebookTopCustomers
                        {
                            //UserId = g.Key.AuthorUserId,
                            CustomerId = g.Key.ExternalId,
                            UserName = g.Key.UserName,
                            Name = g.Key.Name,
                            ProfilePic = g.Key.ProfilePic,
                            Gender = g.Key.Gender,
                            Count = g.Count(),
                            PostCount = 0,
                            CommentCount = g.Count(),
                            MsgCount = 0
                        }
                ).GroupBy(p => new { p.CustomerId, p.UserName, p.Name, p.ProfilePic, p.Gender })
                                .Select(
                                    g => new FacebookTopCustomers
                                    {
                                        CustomerId = g.Key.CustomerId,
                                        UserName = g.Key.UserName,
                                        Name = g.Key.Name,
                                        ProfilePic = g.Key.ProfilePic,
                                        Gender = g.Key.Gender,
                                        Count = g.Sum(s => s.Count),
                                        PostCount = g.Sum(s => s.PostCount),
                                        CommentCount = g.Sum(s => s.CommentCount),
                                        MsgCount = g.Sum(s => s.MsgCount)
                                    }).OrderByDescending(x => x.Count).Take(5).ToList();*/

                return fbPostReportObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<FBPostInfoModel> FacebookGetAllPostInfo(string userId, bool isPage, string postId)
        {
            try
            {
                FacebookUtility utility = new FacebookUtility(userId, isPage);
                FBPostInfoModel fbPostInfoObj = new FBPostInfoModel();
                FBPostModel fbPostObj = new FBPostModel();

                JsonObject postObj = await utility.getPostInfo(postId);
                fillPostObject(ref fbPostObj, postObj);

                fbPostInfoObj.fbPost = fbPostObj;
                JsonArray comments = await utility.getCommentsForObject(postId, 500);
                List<FBCommentModel> fbComms = new List<FBCommentModel>();
                foreach (JsonObject comm in comments)
                {
                    FBCommentModel commObj = new FBCommentModel();
                    fillCommentObject(ref commObj, comm);

                    JsonArray nestedComms = await utility.getCommentsForObject(commObj.Id, 500);
                    List<FBCommentModel> fbNestedComms = new List<FBCommentModel>();
                    foreach (JsonObject nestedComm in nestedComms)
                    {
                        FBCommentModel nestedCommObj = new FBCommentModel();
                        fillCommentObject(ref nestedCommObj, nestedComm);
                        nestedCommObj.fbCommentComments = null;

                        fbNestedComms.Add(nestedCommObj);
                    }
                    commObj.fbCommentComments = fbNestedComms;

                    fbComms.Add(commObj);
                }
                fbPostInfoObj.fbPostComments = fbComms;

                return fbPostInfoObj;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public async Task<FBPostInfoModel> FacebookGetPostInfo(string postId, string commentId)
        {
            FBPostInfoModel fbPostInfoObj = new FBPostInfoModel();
            await Task.Run(() =>
            {
                try
                {
                    FBPostModel fbPostObj = new FBPostModel();
                    using (new TransactionScope(
                                            TransactionScopeOption.Required,
                                            new TransactionOptions
                                            {
                                                IsolationLevel = IsolationLevel.ReadUncommitted
                                            }))
                    {
                        using (SocialCRMEntities db = new SocialCRMEntities())
                        {
                            var mediaAttachments = db.FeedsMediaAttachments.ToList();
                            var postInfo = db.PostsFBs.Where(x => x.PostId == postId).FirstOrDefault();
                            bool isPage = postInfo.PageId != null;
                            if (postInfo != null)
                            {
                                fbPostObj.AuhtorId = postInfo.AuthorUserId;
                                fbPostObj.AuthorName = postInfo.AuthorUserName;
                                fbPostObj.AuthorProfilePic = postInfo.AuthorProfilePic;
                                fbPostObj.CreatedDate = postInfo.CreatedDate.Value;
                                fbPostObj.Id = postInfo.Id.ToString();
                                fbPostObj.Msg = postInfo.Message;
                                fbPostObj.StatusId = postInfo.PostId;
                                fbPostObj.MsgType = "FP";
                                fbPostObj.ConversationDate = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)postInfo.CreatedDate.Value);
                                if (isPage)
                                {
                                    fbPostObj.ToUserId = postInfo.PageId;
                                    fbPostObj.ToUserName = db.ProfilePages.Where(x => x.ProfilePageID == postInfo.PageId).FirstOrDefault().PageName;
                                }
                                else
                                {
                                    fbPostObj.ToUserId = postInfo.UserId;
                                    fbPostObj.ToUserName = db.Profiles.Where(x => x.ProfileExternalId == postInfo.UserId).FirstOrDefault().Name;
                                }


                                fbPostObj.Attachments = new List<AttachmentModel>();

                                var urls = new List<string>();
                                if (mediaAttachments.Where(x => x.MediaId == postInfo.Id && x.MediaType == "FP").Count() > 0)
                                {
                                    urls = mediaAttachments.Where(x => x.MediaId == postInfo.Id && x.MediaType == "FP").Select(y => y.Description + "|" + y.AttachmentUrl).ToList<string>();
                                }
                                InboxItemModel item = new InboxItemModel();
                                item.Attachments = new List<AttachmentModel>();
                                item.Urls = urls;
                                item.IsWarned = false;
                                item.MessageType = fbPostObj.MsgType;
                                UnifiedInboxUtility.CheckForAttachments(ref item);
                                fbPostObj.Attachments.AddRange(item.Attachments);
                                fbPostInfoObj.fbPost = fbPostObj;

                                fbPostInfoObj.fbPostComments = new List<FBCommentModel>();
                                var dateTo = postInfo.CreatedDate.Value.AddDays(30);
                                var allComments = db.CommentsFBs.Where(x => x.CreatedDate >= postInfo.CreatedDate && x.CreatedDate <= dateTo).ToList();
                                List<CommentsFB> commentsObj = null;
                                if (commentId != "0")
                                {
                                    commentsObj = allComments.Where(x => x.CommentId == commentId).ToList();
                                }
                                else
                                {
                                    commentsObj = allComments.Where(x => x.PostId == postId).ToList();
                                }

                                if (commentsObj != null)
                                {
                                    foreach (var comment in commentsObj)
                                    {
                                        FBCommentModel commentModel = new FBCommentModel();
                                        commentModel.postAuthorId = postInfo.AuthorUserId;
                                        commentModel.AuthorId = comment.FromId;
                                        commentModel.AuthorName = comment.FromName;
                                        commentModel.AuthorProfilePic = comment.FromUserProfilePic;
                                        commentModel.CreatedDate = comment.CreatedDate.Value;
                                        commentModel.str_CreatedDate = DatetimeExtensions.formatMsgDatetime(comment.CreatedDate.Value);
                                        commentModel.Id = comment.CommentId;
                                        commentModel.FeedId = comment.Id;
                                        commentModel.Msg = comment.Message;
                                        commentModel.MsgType = "FC";
                                        commentModel.IsLiked = false;
                                        commentModel.ProfilePage = postInfo.PageId; //db.ProfilePages.Where(x => x.ProfilePageID == postInfo.PageId).FirstOrDefault().PageName;
                                        commentModel.CreatedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)comment.CreatedDate.Value);

                                    // Adding Attachments
                                    commentModel.Attachment = new AttachmentModel();

                                        urls = new List<string>();
                                        if (!String.IsNullOrEmpty(comment.Attachment))
                                        {
                                            urls.Add(comment.AttachmentType + "|" + comment.Attachment);
                                        }
                                        item = new InboxItemModel();
                                        item.Attachments = new List<AttachmentModel>();
                                        item.Urls = urls;
                                        item.IsWarned = false;
                                        item.MessageType = commentModel.MsgType;
                                        UnifiedInboxUtility.CheckForAttachments(ref item);
                                        commentModel.Attachment = item.Attachments.FirstOrDefault();
                                    //

                                    var nestedComments = allComments.Where(x => x.PostId == comment.CommentId).ToList();
                                        if (nestedComments != null)
                                        {
                                            commentModel.fbCommentComments = new List<FBCommentModel>();
                                            foreach (var nestedComment in nestedComments)
                                            {
                                                FBCommentModel nestedCommentModel = new FBCommentModel();
                                                nestedCommentModel.postAuthorId = postInfo.AuthorUserId;
                                                nestedCommentModel.AuthorId = nestedComment.FromId;
                                                nestedCommentModel.AuthorName = nestedComment.FromName;
                                                nestedCommentModel.AuthorProfilePic = nestedComment.FromUserProfilePic;
                                                nestedCommentModel.CreatedDate = nestedComment.CreatedDate.Value;
                                                nestedCommentModel.str_CreatedDate = DatetimeExtensions.formatMsgDatetime(nestedComment.CreatedDate.Value);
                                                nestedCommentModel.Id = nestedComment.CommentId;
                                                nestedCommentModel.FeedId = nestedComment.Id;
                                                nestedCommentModel.Msg = nestedComment.Message;
                                                nestedCommentModel.MsgType = "FC";
                                                nestedCommentModel.IsLiked = false;
                                                commentModel.ProfilePage = postInfo.PageId; //db.ProfilePages.Where(x => x.ProfilePageID == postInfo.PageId).FirstOrDefault().PageName;
                                                nestedCommentModel.CreatedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)nestedComment.CreatedDate.Value);

                                            // Adding Attachments
                                            nestedCommentModel.Attachment = new AttachmentModel();

                                                urls = new List<string>();
                                                if (!String.IsNullOrEmpty(nestedComment.Attachment))
                                                {
                                                    urls.Add(nestedComment.AttachmentType + "|" + nestedComment.Attachment);
                                                }
                                                item = new InboxItemModel();
                                                item.Attachments = new List<AttachmentModel>();
                                                item.Urls = urls;
                                                item.IsWarned = false;
                                                item.MessageType = nestedCommentModel.MsgType;
                                                UnifiedInboxUtility.CheckForAttachments(ref item);
                                                nestedCommentModel.Attachment = item.Attachments.FirstOrDefault();
                                            //

                                            commentModel.fbCommentComments.Add(nestedCommentModel);
                                            }
                                        }
                                        fbPostInfoObj.fbPostComments.Add(commentModel);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    fbPostInfoObj =  null;
                }
            });
            return fbPostInfoObj;
        }

        void fillPostObject(ref FBPostModel fbPostObj, JsonObject postObj)
        {
            JsonObject from = postObj["from"] as JsonObject;
            fbPostObj.AuhtorId = from["id"].ToString();
            fbPostObj.AuthorName = from["name"].ToString();
            fbPostObj.AuthorProfilePic = ((from["picture"] as JsonObject)["data"] as JsonObject)["url"].ToString();
            if (postObj.ContainsKey("message"))
                fbPostObj.Msg = postObj["message"].ToString();
            else
                fbPostObj.Msg = postObj["story"].ToString();
            fbPostObj.CreatedDate = Convert.ToDateTime(postObj["created_time"].ToString());
            fbPostObj.ConversationDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck(Convert.ToDateTime(postObj["created_time"].ToString()));
            fbPostObj.Id = postObj["id"].ToString();
        }

        void fillCommentObject(ref FBCommentModel commObj, JsonObject comm)
        {
            JsonObject fromComment = comm["from"] as JsonObject;
            commObj.AuthorId = fromComment["id"].ToString();
            commObj.AuthorName = fromComment["name"].ToString();
            commObj.AuthorProfilePic = ((fromComment["picture"] as JsonObject)["data"] as JsonObject)["url"].ToString();
            commObj.Msg = comm["message"].ToString();
            commObj.CreatedDate = Convert.ToDateTime(comm["created_time"].ToString());            
            commObj.str_CreatedDate = DatetimeExtensions.formatMsgDatetime(Convert.ToDateTime(comm["created_time"].ToString()));           
            commObj.Id = comm["id"].ToString();
            commObj.IsLiked = Convert.ToBoolean(((comm["likes"] as JsonObject)["summary"] as JsonObject)["has_liked"]);
        }

        public async Task<FacebookPageReportModel> LoadMoreRecentPosts(string pageId, int lastPostId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                FacebookPageReportModel fbPostReportObj = new FacebookPageReportModel();
                FacebookUtility utility = new FacebookUtility(pageId, true);
                var context = new SocialCRMEntities();
                List<recentPost> lstRecentPosts = (from pFB in context.PostsFBs
                                                   where pFB.PageId == pageId
                                                   && pFB.AuthorUserId == pageId
                                                   && pFB.CreatedDate >= fromDate
                                                   && pFB.CreatedDate <= toDate
                                                   && pFB.Id < lastPostId
                                                   join mA in context.FeedsMediaAttachments
                                                   on pFB.Id equals mA.MediaId into gj
                                                   from mA in gj.DefaultIfEmpty()
                                                   orderby pFB.CreatedDate descending
                                                   select new recentPost
                                                   {
                                                       Id = pFB.Id,
                                                       pictureURL = mA.AttachmentUrl,
                                                       messageText = pFB.Message,
                                                       postId = pFB.PostId,
                                                       createdDate = pFB.CreatedDate ?? DateTime.Now,
                                                       type = (mA == null ? "Text" : (mA.Description.Contains("video") ? "Video" : (mA.Description.Contains("photo") ? "Photo" : "Text"))),
                                                       postGrade = pFB.PostGrade
                                                   }).Take(5).ToList();

                foreach (recentPost post in lstRecentPosts)
                {
                    string postIdOfRecentPost = post.postId;

                    post.strCreatedDate = post.createdDate.ToString("dd/MM/yyyy");
                    post.strCreatedTime = post.createdDate.ToString("hh:mm tt");

                    JsonObject reachabilityAndConsumption = await utility.getPostReachabilityAndConsumption(postIdOfRecentPost);
                    int Reachability = 0, LinkClicks = 0, EngagedUsers = 0;

                    if (reachabilityAndConsumption != null && reachabilityAndConsumption.ContainsKey("insights"))
                    {
                        //Reachability = Convert.ToInt32((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[0] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"]);
                        Reachability = Convert.ToInt32(((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[0] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"] as JsonObject)["total"]);
                        LinkClicks = Convert.ToInt32((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[1] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"]);
                        EngagedUsers = Convert.ToInt32((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[2] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"]);
                    }

                    post.postReach = Reachability;
                    post.postClicks = LinkClicks;

                    JsonObject reactionsCommentsSharesObj = await utility.getReactionsCommentsShares(postIdOfRecentPost);
                    int Reactions = 0, Comments = 0, Sharedposts = 0;
                    if (reactionsCommentsSharesObj != null && reactionsCommentsSharesObj.ContainsKey("likes"))
                    {
                        Reactions = Convert.ToInt32(((reactionsCommentsSharesObj["likes"] as JsonObject)["summary"] as JsonObject)["total_count"]);
                    }

                    if (reactionsCommentsSharesObj != null && reactionsCommentsSharesObj.ContainsKey("comments"))
                    {
                        Comments = Convert.ToInt32(((reactionsCommentsSharesObj["comments"] as JsonObject)["summary"] as JsonObject)["total_count"]);
                    }

                    if (reactionsCommentsSharesObj != null && reactionsCommentsSharesObj.ContainsKey("shares"))
                    {
                        Sharedposts = Convert.ToInt32((reactionsCommentsSharesObj["shares"] as JsonObject)["count"]);
                    }

                    double engagementRate = 0;
                    if (Reachability != 0)
                        engagementRate = (double)EngagedUsers / Reachability;

                    if (EngagedUsers > Reachability)
                    {
                        engagementRate = 1;
                    }

                    post.postReactions = Reactions;
                    post.postComments = Comments;
                    post.postShares = Sharedposts;
                    post.engagementRate = Math.Round(engagementRate * 100);
                }
                fbPostReportObj.lstRecentPosts = lstRecentPosts;
                if (lstRecentPosts.Count > 0)
                    fbPostReportObj.lastPostId = lstRecentPosts[lstRecentPosts.Count - 1].Id;

                return fbPostReportObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*private Dictionary<string, string> GetCalculatedPostGrade(List<string> lstPostsIdsOnly)
        {
            Dictionary<string, string> dictPostGrades = new Dictionary<string, string>();

            using (var db = new SocialCRMEntities())
            {
                try
                {
                    DateTime monthLimit = DateTime.Now.AddMonths(-6);
                    DateTime limit48Hours = DateTime.Now.AddDays(-2);

                    List<PostsFB> lstPostsFB = db.PostsFBs.Where(x => (x.CreatedDate > monthLimit) && (x.EngagementRate != null)).ToList();
                    List<FeedsMediaAttachment> lstMediaAttachments = db.FeedsMediaAttachments.ToList();

                    //Collection
                    List<recentPosts> allPosts = (from post in lstPostsFB
                                                  join mediaFeeds in lstMediaAttachments on post.Id equals mediaFeeds.MediaId into gj
                                                  from mediaFeeds in gj.DefaultIfEmpty()
                                                  select new recentPosts()
                                                  {
                                                      Id = Convert.ToInt32(post.Id),
                                                      Message = post.Message,
                                                      PageId = post.PageId,
                                                      Description = mediaFeeds == null ? "Text" : mediaFeeds.Description,
                                                      EngagmentRate = post.EngagementRate,
                                                      CreatedDate = Convert.ToDateTime(post.CreatedDate),
                                                      PostId = post.PostId
                                                  }).ToList();

                    //Filteration
                    List<recentPosts> allTextPosts = allPosts.Where(x => x.Description == "Text").OrderBy(x => x.EngagmentRate).ToList();
                    List<recentPosts> allPhotoPosts = allPosts.Where(x => x.Description == "photo").OrderBy(x => x.EngagmentRate).ToList();
                    List<recentPosts> allVideoPosts = allPosts.Where(x => x.Description == "video").OrderBy(x => x.EngagmentRate).ToList();

                    //Saturation - Removing duplications
                    allPhotoPosts = allPhotoPosts.GroupBy(x => x.PostId).Select(y => y.First()).ToList();
                    allVideoPosts = allVideoPosts.GroupBy(x => x.PostId).Select(y => y.First()).ToList();

                    List<string> lstPageIds = allPosts.Select(x => x.PageId).Distinct().ToList();
                    //List<FacebookUserAccount> fbPageAccounts = FacebookDBOperations.GetFacebookAccounts(true);
                    List<FacebookUserAccount> fbPageAccounts = new List<FacebookUserAccount>();
                    fbPageAccounts = db.ProfilePages.Select(x => new FacebookUserAccount
                    {
                        UserId = x.ProfileExternalId,
                        PageId = x.ProfilePageID,
                        AccessToken = x.PageAccessToken,
                        ProfilePic = x.ProfilePic
                    }).ToList();

                    Dictionary<string, string> tempDict = new Dictionary<string, string>();
                    tempDict = CalculateGradeByPostType(allTextPosts, lstPostsIdsOnly, "Text");
                    foreach (KeyValuePair<string, string> tempObj in tempDict)
                    {
                        dictPostGrades.Add(tempObj.Key, tempObj.Value);
                    }

                    tempDict = CalculateGradeByPostType(allPhotoPosts, lstPostsIdsOnly, "photo");
                    foreach (KeyValuePair<string, string> tempObj in tempDict)
                    {
                        dictPostGrades.Add(tempObj.Key, tempObj.Value);
                    }

                    tempDict = CalculateGradeByPostType(allVideoPosts, lstPostsIdsOnly, "video");
                    foreach (KeyValuePair<string, string> tempObj in tempDict)
                    {
                        dictPostGrades.Add(tempObj.Key, tempObj.Value);
                    }
                }
                catch (Exception ex)
                { }
            }

            return dictPostGrades;
        }

        public Dictionary<string, string> CalculateGradeByPostType(List<recentPosts> allPosts, List<string> lstPostsIdsOnly, string postType)
        {
            string postGrade = "Not-Assigned";
            Dictionary<string, string> dictPostGrades = new Dictionary<string, string>();
            List<recentPosts> lstOldPosts = new List<recentPosts>();
            List<recentPosts> lstRecentPosts = new List<recentPosts>();

            foreach (recentPosts post in allPosts)
            {
                if (lstPostsIdsOnly.Contains(post.PostId))
                {
                    lstRecentPosts.Add(post);
                }
                else
                {
                    lstOldPosts.Add(post);
                }
            }

            int greaterCount = 0;
            double gradePercentage = new double();
            if (lstRecentPosts.Count > 0)
            {
                //Check with old ones
                foreach (recentPosts _recentpost in lstRecentPosts)
                {
                    greaterCount = 0;

                    //Check with older ones
                    foreach (recentPosts _oldpost in lstOldPosts)
                    {
                        if (Convert.ToDouble(_recentpost.EngagmentRate) > Convert.ToDouble(_oldpost.EngagmentRate) || _oldpost.EngagmentRate == null)
                        {
                            greaterCount++;
                        }
                    }

                    //Check with latest ones
                    foreach (recentPosts currentPost in lstRecentPosts)
                    {
                        if (currentPost.PostId != _recentpost.PostId)
                        {
                            if (Convert.ToDouble(_recentpost.EngagmentRate) > Convert.ToDouble(currentPost.EngagmentRate) || currentPost.EngagmentRate == null)
                            {
                                greaterCount++;
                            }
                        }
                    }

                    //Finding Percentage
                    gradePercentage = Math.Round(Convert.ToDouble(Convert.ToDouble(greaterCount) / (lstRecentPosts.Count + lstOldPosts.Count) * 100), 2);

                    if (gradePercentage > 90)
                    {
                        postGrade = "A+";
                    }
                    else if (gradePercentage > 80 && gradePercentage < 90)
                    {
                        postGrade = "A";
                    }
                    else if (gradePercentage > 70 && gradePercentage < 80)
                    {
                        postGrade = "B";
                    }
                    else if (gradePercentage > 60 && gradePercentage < 70)
                    {
                        postGrade = "C";
                    }
                    else if (gradePercentage < 60)
                    {
                        postGrade = "D";
                    }

                    dictPostGrades.Add(_recentpost.PostId, postGrade);
                }

            }
            return dictPostGrades;
        }*/
    }
}