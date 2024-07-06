using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using SocialCRM.Web.Repositories.Core;
using SocialCRM.Web.Models.UnifiedContact;
using SocialAPIsWrapper.APIs;
using System.Threading.Tasks;
using LinqToTwitter;
using SocialCRM.Web.Models.UnifiedInbox;
using CRM.Common.Extensions;
using CRM.Entities;
using SocialCRM.Web.Models.Profiles;
using System.Data.Entity;
using SocialAPIsWrapper.Models;
using SocialCRM.Web.Models.Twitter;
using System.Data.Entity.Core.Objects;
using System.Text.RegularExpressions;

namespace SocialCRM.Web.Repositories
{
    public class TwitterRepository : ITwitterRepository
    {
        public async Task<ContactProfileModel> GetTwitterProfile(string fromUser, string ofUser)
        {
            ContactProfileModel result = new ContactProfileModel();
            TwitterUtility utility = new TwitterUtility(fromUser);
            User userProfile = await utility.getTwitterProfile(ofUser);

            if (userProfile != null)
            {
                fillContactInfo(ref result, userProfile);
            }
            return result;
        }

        public async Task<ContactFollowersModel> GetTwitterFollowers(string fromUser, string ofUser, long cursor)
        {
            ContactFollowersModel followersInfo = new ContactFollowersModel();
            List<ContactProfileModel> result = new List<ContactProfileModel>();
            TwitterUtility utility = new TwitterUtility(fromUser);
            Friendship userfriendship = await utility.getListOfFollowers(ofUser, cursor);

            if (userfriendship != null && userfriendship.Users != null && userfriendship.CursorMovement != null)
            {
                foreach (User follower in userfriendship.Users)
                {
                    ContactProfileModel userFollower = new ContactProfileModel();
                    fillContactInfo(ref userFollower, follower);

                    result.Add(userFollower);
                }
            }
            followersInfo.followers = result;
            followersInfo.cursor = userfriendship.CursorMovement.Next;

            return followersInfo;
        }

        public async Task<ContactFollowersModel> GetTwitterFriends(string fromUser, string ofUser, long cursor)
        {
            ContactFollowersModel friendsInfo = new ContactFollowersModel();
            List<ContactProfileModel> result = new List<ContactProfileModel>();
            TwitterUtility utility = new TwitterUtility(fromUser);
            Friendship userfriendship = await utility.getListOfFriends(ofUser, cursor);

            if (userfriendship != null && userfriendship.Users != null && userfriendship.CursorMovement != null)
            {
                foreach (User friend in userfriendship.Users)
                {
                    ContactProfileModel userFriend = new ContactProfileModel();
                    fillContactInfo(ref userFriend, friend);

                    result.Add(userFriend);
                }
            }

            friendsInfo.followers = result;
            friendsInfo.cursor = userfriendship.CursorMovement.Next;
            return friendsInfo;
        }

        public async Task<ContactFollowersModel> GetTwitterListMembers(string fromUser, ulong listId, long cursor)
        {
            ContactFollowersModel followersInfo = new ContactFollowersModel();
            List<ContactProfileModel> result = new List<ContactProfileModel>();
            TwitterUtility utility = new TwitterUtility(fromUser);
            List userfriendship = await utility.getTwitterListMembers(listId, cursor);

            if (userfriendship != null && userfriendship.Users != null && userfriendship.CursorMovement != null)
            {
                foreach (User follower in userfriendship.Users)
                {
                    ContactProfileModel userFollower = new ContactProfileModel();
                    fillContactInfo(ref userFollower, follower);

                    result.Add(userFollower);
                }
            }
            followersInfo.followers = result;
            followersInfo.cursor = userfriendship.CursorMovement.Next;

            return followersInfo;
        }

        public async Task<ContactTweetsModel> GetTwitterTweets(string fromUser, string ofUser, int count, ulong maxId)
        {
            ContactTweetsModel tweetsInfo = new ContactTweetsModel();
            List<InboxItemModel> result = new List<InboxItemModel>();
            TwitterUtility utility = new TwitterUtility(fromUser);
            List<Status> usertweets = await utility.getUserTweets(ofUser, maxId, count);
            ulong currentMaxId = 0;

            if (usertweets != null)
            {
                if (usertweets.Count > 0)
                {
                    currentMaxId = usertweets[usertweets.Count - 1].StatusID - 1;
                }

                foreach (Status tweet in usertweets)
                {
                    InboxItemModel userTweet = new InboxItemModel();
                    fillTweetInfo(ref userTweet, tweet, "sent a tweet | ");

                    result.Add(userTweet);
                }
            }

            tweetsInfo.tweets = result;
            tweetsInfo.maxId = currentMaxId;
            return tweetsInfo;
        }

        public async Task<ContactTweetsModel> GetTwitterHomeTweets(string fromUser, string ofUser, int count, ulong maxId)
        {
            ContactTweetsModel tweetsInfo = new ContactTweetsModel();
            List<InboxItemModel> result = new List<InboxItemModel>();
            TwitterUtility utility = new TwitterUtility(fromUser);
            List<Status> usertweets = await utility.getUserHomeTweets(ofUser, maxId, count);
            ulong currentMaxId = 0;

            if (usertweets != null)
            {
                if (usertweets.Count > 0)
                {
                    currentMaxId = usertweets[usertweets.Count - 1].StatusID - 1;
                }

                foreach (Status tweet in usertweets)
                {
                    InboxItemModel userTweet = new InboxItemModel();
                    fillTweetInfo(ref userTweet, tweet, "sent a tweet | ");

                    result.Add(userTweet);
                }
            }

            tweetsInfo.tweets = result;
            tweetsInfo.maxId = currentMaxId;
            return tweetsInfo;
        }

        public async Task<ContactTweetsModel> GetTwitterListTweets(string fromUser, ulong listId, int count, ulong maxId)
        {
            ContactTweetsModel tweetsInfo = new ContactTweetsModel();
            List<InboxItemModel> result = new List<InboxItemModel>();
            TwitterUtility utility = new TwitterUtility(fromUser);
            List<Status> usertweets = await utility.getTwitterListTweets(listId, maxId, count);
            ulong currentMaxId = 0;

            if (usertweets != null)
            {
                if (usertweets.Count > 0)
                {
                    currentMaxId = usertweets[usertweets.Count - 1].StatusID - 1;
                }

                foreach (Status tweet in usertweets)
                {
                    InboxItemModel userTweet = new InboxItemModel();
                    fillTweetInfo(ref userTweet, tweet, "sent a tweet | ");

                    result.Add(userTweet);
                }
            }

            tweetsInfo.tweets = result;
            tweetsInfo.maxId = currentMaxId;
            return tweetsInfo;
        }

        public async Task<ContactTweetsModel> GetTwitterMentions(string fromUser, string ofUser, int count, ulong maxId)
        {
            ContactTweetsModel tweetsInfo = new ContactTweetsModel();
            List<InboxItemModel> result = new List<InboxItemModel>();
            TwitterUtility utility = new TwitterUtility(fromUser);
            List<Status> usertweets = await utility.getUserMentions(ofUser, maxId, count);
            ulong currentMaxId = 0;

            if (usertweets != null)
            {
                if (usertweets.Count > 0)
                {
                    currentMaxId = usertweets[usertweets.Count - 1].StatusID - 1;
                }

                foreach (Status tweet in usertweets)
                {
                    InboxItemModel userTweet = new InboxItemModel();
                    fillTweetInfo(ref userTweet, tweet, "sent a tweet | ");

                    result.Add(userTweet);
                }
            }

            tweetsInfo.tweets = result;
            tweetsInfo.maxId = currentMaxId;
            return tweetsInfo;
        }

        public async Task<List<InboxItemModel>> GetTweetDirectReplies(string fromUser, string ofUser, ulong tweetId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            List<InboxItemModel> result = new List<InboxItemModel>();
            List<Status> usertweets = await utility.getTweetReplies(ofUser, tweetId);
            foreach (Status tweet in usertweets)
            {
                InboxItemModel userTweet = new InboxItemModel();
                fillTweetInfo(ref userTweet, tweet, "replied to ");

                result.Add(userTweet);
            }

            return result;
        }

        public async Task<List<InboxItemModel>> GetTweetMinnedReplies(string ofUser, ulong tweetId)
        {
            List<InboxItemModel> result = new List<InboxItemModel>();
            string tweetStr = Convert.ToString(tweetId);
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
                        var boom = context.PostsTwitters.Where(y => y.InReplyToScreenName == ofUser && y.InReplyToStatusId == tweetStr).GroupBy(a => a.StatusId).OrderBy(z => z.FirstOrDefault().CreatedDate).ToList();
                        foreach (var x in boom)
                        {
                            result.Add(new InboxItemModel
                            {
                                Id = x.FirstOrDefault().Id,
                                ClassName = "unread_msg",
                                TwtImg = x.FirstOrDefault().AuthorProfilePic,
                                IsVerified = Convert.ToBoolean(x.FirstOrDefault().IsAuthorVerified),
                                User = x.FirstOrDefault().InReplyToScreenName,
                                UserTwit = x.FirstOrDefault().StatusText,
                                ReplyedBy = x.FirstOrDefault().AuthorName,
                                ToUser = x.FirstOrDefault().InReplyToScreenName,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)x.FirstOrDefault().CreatedDate),
                                ActivityType = "replied to ",
                                AccountTypeImg = "t_ico16",
                                MessageType = "TP",
                                IsArchived = false,
                                IsRetweet = Convert.ToBoolean(x.FirstOrDefault().IsRetweet)
                            });
                        }
                    }
                }
            });

            return result;
        }

        public async Task<List<InboxItemModel>> GetTwitterDirectMessages(string fromUser, string toUser)
        {
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
                        //var boom = context.MessagesTwitters.Where(y => ((y.SenderScreenName == fromUser && y.RecipientScreenName == toUser)
                        //                                           || (y.SenderScreenName == toUser && y.RecipientScreenName == fromUser)))
                        //                                           .GroupBy(a => a.MsgId).OrderByDescending(z => z.FirstOrDefault().CreatedDate).Take(50).ToList();
                        //foreach (var x in boom)
                        //{
                        //    result.Add(new InboxItemModel
                        //    {
                        //        Id = x.FirstOrDefault().Id,
                        //        ClassName = "unread_msg",
                        //        TwtImg = x.FirstOrDefault().SenderProfilePic,
                        //        IsVerified = Convert.ToBoolean(x.FirstOrDefault().IsSenderVerified),
                        //        User = x.FirstOrDefault().SenderScreenName,
                        //        UserTwit = x.FirstOrDefault().MsgText,
                        //        ReplyedBy = x.FirstOrDefault().SenderScreenName,
                        //        ReplyedByFullName = x.FirstOrDefault().SenderFullName,
                        //        ToUser = x.FirstOrDefault().RecipientScreenName,
                        //        ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)x.FirstOrDefault().CreatedDate),
                        //        ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)x.FirstOrDefault().CreatedDate),
                        //        ActivityType = "replied to ",
                        //        AccountTypeImg = "t_ico16",
                        //        MessageType = "TP",
                        //        IsArchived = false,
                        //        IsRetweet = false,
                        //        UserId = x.FirstOrDefault().SenderId
                        //    });
                        //}

                        var temp = (from DM in context.MessagesTwitters
                                    where ((DM.SenderScreenName == fromUser && DM.RecipientScreenName == toUser)
                                                                   || (DM.SenderScreenName == toUser && DM.RecipientScreenName == fromUser))
                                    join taskComp in context.TaskCompleteds on DM.Id equals taskComp.ActivityId into gj
                                    from x in gj.Where(x => x.ActivityTable == "TM").DefaultIfEmpty()
                                   // group y by DM.MsgId into gj
                                    orderby DM.CreatedDate descending
                                    select new InboxItemModel
                                    {
                                        Id = DM.Id,
                                        ClassName = "unread_msg",
                                        TwtImg = DM.SenderProfilePic,
                                        IsVerified = DM.IsSenderVerified,
                                        User = DM.SenderScreenName,
                                        UserTwit = DM.MsgText,
                                        ReplyedByUserId = DM.RecipientId,
                                        ReplyedBy = DM.SenderScreenName,
                                        ReplyedByFullName = DM.SenderFullName,
                                        StatusId = DM.MsgId,
                                        ToUser = DM.RecipientScreenName,
                                        ReplyedDate = DM.CreatedDate.ToString(),
                                        ReplyedDateForDeck = DM.CreatedDate.ToString(),
                                        ActivityType = "replied to ",
                                        AccountTypeImg = "t_ico16",
                                        MessageType = "TM",
                                        IsArchived = false,
                                        IsRetweet = false,
                                        UserId = DM.SenderId,
                                        IsCompleted = x.IsCompleted,
                                        Images = DM.Attachment,
                                        ImageType = DM.AttachmentType
                                    }).Take(50).ToList();

                        foreach (var x in temp)
                        {
                            var item = new InboxItemModel
                            {
                                Id = x.Id,
                                ClassName = x.ClassName,
                                TwtImg = x.TwtImg,
                                IsVerified = x.IsVerified,
                                User = x.User,
                                UserTwit = x.UserTwit,
                                ReplyedByUserId = x.ReplyedByUserId,
                                ReplyedBy = x.ReplyedBy,
                                ReplyedByFullName = x.ReplyedByFullName,
                                StatusId = x.StatusId,
                                ToUser = x.ToUser,
                                ReplyedDate = DatetimeExtensions.formatMsgDatetime(Convert.ToDateTime(x.ReplyedDate)),
                                ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck(Convert.ToDateTime(x.ReplyedDateForDeck)),
                                ActivityType = x.ActivityType,
                                AccountTypeImg = x.AccountTypeImg,
                                MessageType = x.MessageType,
                                IsArchived = x.IsArchived,
                                IsRetweet = x.IsRetweet,
                                UserId = x.UserId,
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

        public async Task<ContactTweetsModel> GetTwitterHistory(int userId, string ofUser, int pageNumber, int pageSize, int groupId, int companyId, bool isPersonal)
        {
            ContactTweetsModel tweetsInfo = new ContactTweetsModel();
            List<InboxItemModel> result = new List<InboxItemModel>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();

                    foreach (var data in context.UnifiedContactHistory(userId, ofUser, pageNumber, pageSize, teamMemberId, groupId, isPersonal))
                    {
                        string type = "t_ico16";
                        string activityType = "";
                        if (data.ActivityType == "TP")
                        {
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
                            activityType = "sent a direct message to ";
                        }

                        result.Add(new InboxItemModel
                        {
                            Id = data.Id,
                            ClassName = "unread_msg",
                            TwtImg = data.ProfilePic,
                            IsVerified = Convert.ToBoolean(data.IsVerified),
                            User = data.UserName,
                            UserTwit = data.TextValue,
                            ReplyedBy = data.FromUser,
                            ToUser = data.ToUser,
                            ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)data.CreatedDate),
                            ActivityType = activityType,
                            AccountTypeImg = type,
                            MessageType = data.ActivityType,
                            IsArchived = Convert.ToBoolean(data.IsArchived),
                            IsRetweet = Convert.ToBoolean(data.IsRetweet)
                        });
                    }
                }
            });

            tweetsInfo.tweets = result;
            tweetsInfo.maxId = (ulong)pageNumber + 1;
            return tweetsInfo;
        }

        public async Task<List<ContactFollowStatusModel>> GetTwitterFriendshipStatus(string fromUser, string ofUser, int userId, int groupId, int companyId, bool isPersonal)
        {
            ProfileRepository proRep = new ProfileRepository();
            List<ContactFollowStatusModel> result;
            if (isPersonal)
            {
                result = await proRep.GetSocialProfilesPersonalMode(userId, 4);
            }
            else
            {
                result = await proRep.GetSocialProfilesByGroupId(userId, groupId, companyId, 4);
            }

            foreach (ContactFollowStatusModel followStatus in result)
            {
                TwitterUtility utility = new TwitterUtility(followStatus.UserName);
                Relationship userRelationship = await utility.GetTwitterFriendshipStatus(ofUser);
                followStatus.IsFollowingContact = userRelationship.Following;
                followStatus.IsFollowedByContact = userRelationship.FollowedBy;
            }

            return result;
        }

        public async Task<User> TwitterFollowUser(string fromUser, string ofUser)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.followUser(ofUser);
        }

        public async Task<User> TwitterUnfollowUser(string fromUser, string ofUser)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.unfollowUser(ofUser);
        }

        public async Task<Status> TwitterNewTweet(string fromUser, string status, List<MediaModelTwitter> mediaInfo, string lat, string lng, int teamId, int companyId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            string profileId = "", postId = "";
            using (SocialCRMEntities context = new SocialCRMEntities())
            {
                profileId = context.Profiles.Where(z => z.UserName == fromUser && z.ProfileTypeId == 4).FirstOrDefault().ProfileExternalId;
            }
            List<ulong> mediaIds = null;
            if (mediaInfo!=null)
                mediaIds = await utility.UploadMedia(mediaInfo);

            Status response;
            if(!String.IsNullOrEmpty(lat) && !String.IsNullOrEmpty(lng))
            {
                try
                {
                    response = await utility.newTweetWithLocation(status, mediaIds, Decimal.Parse(lat), Decimal.Parse(lng));
                }
                catch (Exception ex) {
                    throw ex;
                }
            }
            else
            {
                response = await utility.newTweet(status, mediaIds);
            }

            postId = Convert.ToString(response.StatusID);
            await new MiningRepository().AddSocialActivity(profileId, "", postId, "TP", 3, "", status, teamId, companyId, "", "", "");
            return response;
        }
        public async Task<Status> TwitterDeleteTweet(string fromUser, ulong tweetId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.deleteTweet(tweetId);
        }

        public async Task<DirectMessage> TwitterSendDirectMessage(string fromUser, string toUser, string status)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.sendDirectMessage(toUser, status);
        }

        public async Task<DirectMessage> TwitterSendDM(string userId, string fromUser, string toUser, string status, int teamId, int companyId)
        {
            SocialCRMEntities db = new SocialCRMEntities();
            var fromUserName = db.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault().UserName;
            string toUserName = fromUserName == fromUser ? toUser : fromUser;
            TwitterUtility utility = new TwitterUtility(fromUserName);
            var response = await utility.sendDirectMessage(toUserName, status);
            string profileId = db.Profiles.Where(z => z.UserName == fromUserName && z.ProfileTypeId == 4).FirstOrDefault().ProfileExternalId;
            await new MiningRepository().AddSocialActivity(userId, "", "", "TM", 2, "", status, teamId, companyId, fromUserName, toUserName, response.IDString);
            return response;
        }

        public async Task<DirectMessage> TwitterDeleteDirectMessage(string fromUser, ulong messageId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.deleteDirectMessage(messageId);
        }

        public async Task<Status> TwitterReplyTweet(string fromUser, ulong tweetId, string status, List<MediaModelTwitter> mediaInfo, int userId, int companyId)
        {
            string profileId = "";
            TwitterUtility utility = new TwitterUtility(fromUser);
            using (SocialCRMEntities context = new SocialCRMEntities())
            {
                var userProfile=context.Profiles.Where(z => z.UserName == fromUser && z.ProfileTypeId == 4).FirstOrDefault();
                if (userProfile != null)
                    profileId = userProfile.ProfileExternalId;
                else
                    return new Status();
            }
            if (mediaInfo == null)
            {
                var response = await utility.tweetReply(tweetId, status, null);
                string commentId = Convert.ToString(response.StatusID);
                await new MiningRepository().AddSocialActivity(profileId, "", tweetId.ToString(), "TP", 2, "", status, userId, companyId, "", "", commentId);
                return response;
            }
            else
            {
                var mediaIds = await utility.UploadMedia(mediaInfo);
                var response = await utility.tweetReply(tweetId, status, mediaIds);
                string commentId = Convert.ToString(response.StatusID);
                await new MiningRepository().AddSocialActivity(profileId, "", tweetId.ToString(), "TP", 2, "", status, userId, companyId, "", "", commentId);
                return response;
            }
        }

        public async Task<Status> TwitterRetweet(string fromUser, ulong tweetId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.retweet(tweetId);
            /*using (var context = new SocialCRMEntities())
            {
                MiningRepository rep = new MiningRepository();

                int ProfileId = context.Profiles.Where(x => x.UserName == fromUser).FirstOrDefault().Id;
                string tweetIdStr = Convert.ToString(tweetId);
                int feedId = context.PostsTwitters.Where(x => x.StatusId == tweetIdStr).FirstOrDefault().Id;
                result = await rep.AddSocialActivity(ProfileId, null, feedId, "TP", 2, newTweet.StatusID.ToString());
            }*/
        }

        public async Task<string> TwitterIsRetweeted(string fromUser, ulong tweetId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.isRetweeted(tweetId);
        }

        public async Task<Status> TwitterUnRetweet(string fromUser, ulong retweetedStatusId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.deleteTweet(retweetedStatusId);
            /*using (var context = new SocialCRMEntities())
            {
                MiningRepository rep = new MiningRepository();
                await rep.RemoveSocialActivity(activityId);
            }*/
        }

        public async Task<Status> TwitterRetweetWithComment(string fromUser, string authorUserName, ulong tweetId, string status, List<MediaModelTwitter> mediaInfo, int userId, int companyId)
        {
            string profileId = "", commentId = "";
            using (SocialCRMEntities context = new SocialCRMEntities())
            {
                profileId = context.Profiles.Where(z => z.UserName == fromUser && z.ProfileTypeId == 4).FirstOrDefault().ProfileExternalId;
            }
            TwitterUtility utility = new TwitterUtility(fromUser);
            status = string.Format("{0} http://twitter.com/{1}/status/{2}", status, authorUserName, tweetId);
            if (mediaInfo == null)
            {
                var response = await utility.newTweet(status, null);
                //commentId = Convert.ToString(response.StatusID);
                //await new MiningRepository().AddSocialActivity(profileId, "", tweetId.ToString(), "TP", 2, "", status, userId, companyId, "", "", commentId,activityFeedId, groupId,replyFrom);
                return response;
            }
            else
            {
                var mediaIds = await utility.UploadMedia(mediaInfo);
                var response = await utility.newTweet(status, mediaIds);
                //commentId = Convert.ToString(response.StatusID);
                //await new MiningRepository().AddSocialActivity(profileId, "", tweetId.ToString(), "TP", 2, "", status, userId, companyId, "", "", commentId,activityFeedId, groupId,replyFrom);
                return response;
            }
        }

        public async Task<Status> TwitterMarkFavorite(string fromUser, string tweetId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.markFavorite(tweetId);
            /*using (var context = new SocialCRMEntities())
            {
                MiningRepository rep = new MiningRepository();
                
                int ProfileId = context.Profiles.Where(x => x.UserName == fromUser).FirstOrDefault().Id;
                string tweetIdStr = Convert.ToString(tweetId);
                int feedId = context.PostsTwitters.Where(x => x.StatusId == tweetIdStr).FirstOrDefault().Id;
                result = await rep.AddSocialActivity(ProfileId, null, feedId, "TP", 1);
            }*/
        }

        public async Task<bool> TwitterIsFavorited(string fromUser, string tweetId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.isFavorited(tweetId);
        }

        public async Task<Status> TwitterUnmarkFavorite(string fromUser, string tweetId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.unmarkFavorite(tweetId);
            /*using (var context = new SocialCRMEntities())
            {
                MiningRepository rep = new MiningRepository();
                await rep.RemoveSocialActivity(activityId);
            }*/
        }

        public async Task<User> TwitterReportSpam(string fromUser, ulong userId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.reportSpam(userId);
        }

        public async Task<ContactExtraInfoModel> GetUserContactInfo(string username, int groupId, int companyId)
        {
            ContactExtraInfoModel userInfo = null;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    userInfo = context.SocialProfileContactInfoes.Where(x => x.UserName == username && x.CompanyId == companyId && x.GroupId == groupId)
                    .Select(y => new ContactExtraInfoModel
                    {
                        Id = y.Id,
                        ProfileExternalId = y.ProfileExternalId,
                        UserName = y.UserName,
                        Company = y.Company,
                        Title = y.Title,
                        Email = y.Email,
                        Phone = y.Phone,
                        CompanyId = (int)y.CompanyId,
                        GroupId = (int)y.GroupId,
                        Comments = y.SocialProfileContactComments.Select(z => new ContactCommentModel
                        {
                            Id = z.Id,
                            Title = z.Title,
                            Description = z.Description,
                            CreatedBy = z.CreatedBy,
                            CreatedAt = (DateTime)z.CreatedAt
                        }).ToList()
                    }).SingleOrDefault();
                }
            });

            return userInfo;
        }

        public async Task<bool> UpdateUserContactInfo(ContactExtraInfoModel contactObj)
        {
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    var existingContact = context.SocialProfileContactInfoes
                                                 .Where(
                                                        x => x.ProfileExternalId == contactObj.ProfileExternalId &&
                                                        x.CompanyId == contactObj.CompanyId &&
                                                        x.GroupId == contactObj.GroupId
                                                  )
                                                 .SingleOrDefault();
                    if (existingContact != null)
                    {
                        if (contactObj.Company != null)
                            existingContact.Company = contactObj.Company;
                        if (contactObj.Title != null)
                            existingContact.Title = contactObj.Title;
                        if (contactObj.Email != null)
                            existingContact.Email = contactObj.Email;
                        if (contactObj.Phone != null)
                            existingContact.Phone = contactObj.Phone;

                        context.SocialProfileContactInfoes.Attach(existingContact);
                        context.Entry(existingContact).State = EntityState.Modified;
                    }
                    else
                    {
                        SocialProfileContactInfo contactDBObj = new SocialProfileContactInfo();
                        contactDBObj.ProfileExternalId = contactObj.ProfileExternalId;
                        contactDBObj.UserName = contactObj.UserName;
                        contactDBObj.CompanyId = contactObj.CompanyId;
                        contactDBObj.GroupId = contactObj.GroupId;
                        contactDBObj.Company = contactObj.Company;
                        contactDBObj.Title = contactObj.Title;
                        contactDBObj.Email = contactObj.Email;
                        contactDBObj.Phone = contactObj.Phone;

                        context.SocialProfileContactInfoes.Add(contactDBObj);
                    }

                    context.SaveChanges();
                }
            });

            return true;
        }

        public async Task<ContactCommentModel> InsertUserContactComment(ContactExtraInfoModel contactObj)
        {
            SocialProfileContactComment contactCommDBObj = new SocialProfileContactComment();
            contactCommDBObj.Title = contactObj.Comments[0].Title;
            contactCommDBObj.Description = contactObj.Comments[0].Description;
            contactCommDBObj.CreatedBy = contactObj.Comments[0].CreatedBy;
            contactCommDBObj.CreatedAt = contactObj.Comments[0].CreatedAt;
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    var existingContact = context.SocialProfileContactInfoes
                                                 .Where(
                                                        x => x.ProfileExternalId == contactObj.ProfileExternalId &&
                                                        x.CompanyId == contactObj.CompanyId &&
                                                        x.GroupId == contactObj.GroupId
                                                  )
                                                 .SingleOrDefault();

                    if (existingContact != null)
                    {
                        existingContact.SocialProfileContactComments.Add(contactCommDBObj);
                        context.SocialProfileContactInfoes.Attach(existingContact);
                        context.Entry(existingContact).State = EntityState.Modified;
                    }
                    else
                    {
                        SocialProfileContactInfo contactDBObj = new SocialProfileContactInfo();
                        contactDBObj.ProfileExternalId = contactObj.ProfileExternalId;
                        contactDBObj.UserName = contactObj.UserName;
                        contactDBObj.CompanyId = contactObj.CompanyId;
                        contactDBObj.GroupId = contactObj.GroupId;
                        contactDBObj.Company = contactObj.Company;
                        contactDBObj.Title = contactObj.Title;
                        contactDBObj.Email = contactObj.Email;
                        contactDBObj.Phone = contactObj.Phone;
                        contactDBObj.SocialProfileContactComments.Add(contactCommDBObj);

                        var obj = context.SocialProfileContactInfoes.Add(contactDBObj);
                    }

                    context.SaveChanges();
                }
            });

            return new ContactCommentModel()
            {
                Id = contactCommDBObj.Id,
                Title = contactCommDBObj.Title,
                Description = contactCommDBObj.Description,
                CreatedBy = contactCommDBObj.CreatedBy,
                CreatedAt = (DateTime)contactCommDBObj.CreatedAt
            };
        }

        public async Task<bool> DeleteUserContactComment(int commentId)
        {
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    SocialProfileContactComment entityToDelete = context.SocialProfileContactComments.Find(commentId);
                    if (context.Entry(entityToDelete).State == EntityState.Detached)
                    {
                        context.SocialProfileContactComments.Attach(entityToDelete);
                    }
                    context.SocialProfileContactComments.Remove(entityToDelete);
                    context.SaveChanges();
                }
            });

            return true;
        }

        private void fillContactInfo(ref ContactProfileModel result, User userProfile)
        {
            result.contactName = userProfile.Name;
            result.contactExternalId = userProfile.UserIDResponse;
            result.contactTwitName = userProfile.ScreenNameResponse;
            result.contactMsg = userProfile.Description;
            result.contactCountry = userProfile.Location;
            if (string.IsNullOrEmpty(userProfile.Url))
            {
                result.contactURL = "NO URL";
            }
            else
            {
                result.contactURL = userProfile.Url;
            }
            result.totalFollower = userProfile.FollowersCount;
            result.totalFollowing = userProfile.FriendsCount;
            result.contactProfilePic = userProfile.ProfileImageUrlHttps.Replace("normal", "reasonably_small");
            result.isContactVerified = userProfile.Verified;
        }

        private void fillTweetInfo(ref InboxItemModel result, Status userTweet, string activityType)
        {
            //int offset = userTweet.User.UtcOffset / 3600;
            result.Id = (int)userTweet.StatusID;
            result.StatusId = userTweet.StatusID.ToString();
            result.ClassName = "unread_msg";
            result.TwtImg = userTweet.User.ProfileImageUrlHttps.Replace("normal", "reasonably_small");
            result.IsVerified = userTweet.User.Verified;
            result.User = userTweet.User.ScreenNameResponse;
            result.UserTwit = userTweet.Text;
            result.ReplyedBy = userTweet.User.ScreenNameResponse;
            result.ToUser = userTweet.InReplyToScreenName;
            result.ReplyedDate = DatetimeExtensions.formatMsgDatetime(userTweet.CreatedAt.ToLocalTime());
            result.ActivityType = activityType;
            result.AccountTypeImg = "t_ico16";
            result.MessageType = "TP";
            result.IsArchived = false;
        }

        public async Task<List<InboxItemModel>> GetTweetsFeedByStatusId(string StatusId)
        {
            var result = new List<InboxItemModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.PostsTwitters.Where(x => x.InReplyToStatusId == StatusId).Select(y => new InboxItemModel
                    {
                        AccountTypeImg = y.AuthorProfilePic,
                        IsVerified = Convert.ToBoolean(y.IsAuthorVerified),
                        Id = y.Id,
                        ReplyedBy = y.AuthorName,
                        ToUser = y.InReplyToScreenName,
                        TwtImg = y.AuthorProfilePic,
                        UserTwit = y.StatusText,
                        ReplyedDate = DatetimeExtensions.formatMsgDatetime(Convert.ToDateTime(y.CreatedDate))
                    }).ToList();
                    db.Dispose();
                }
            });
            return result;
        }

        public async Task<ContactTweetsModel> GetTwitterConversation(int userId, string ofUser, int pageNumber, int pageSize, int groupId, int companyId, string statusId, string Mtype)
        {
            ContactTweetsModel tweetsInfo = new ContactTweetsModel();
            List<InboxItemModel> result = new List<InboxItemModel>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    int teamMemberId = context.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).Select(z => z.Id).FirstOrDefault();

                    foreach (var data in context.UnifiedContactConversation(userId, ofUser, pageNumber, pageSize, teamMemberId, groupId, statusId, Mtype))
                    {
                        string type = "t_ico16";
                        string activityType = "";
                        if (data.ActivityType == "TP")
                        {
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
                            activityType = "sent a direct message to ";
                        }

                        result.Add(new InboxItemModel
                        {
                            Id = data.Id,
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
                            IsArchived = Convert.ToBoolean(data.IsArchived),
                            IsRetweet = Convert.ToBoolean(data.IsRetweet)
                        });
                    }
                }
            });

            tweetsInfo.tweets = result;
            tweetsInfo.maxId = (ulong)pageNumber + 1;
            return tweetsInfo;
        }

        public async Task<List<ContactTwitListModel>> GetUsersOwnedTwitterLists(string ofUser, int userId, int groupId, int companyId, bool isPersonal, bool checkMembership)
        {
            ProfileRepository proRep = new ProfileRepository();
            List<ContactFollowStatusModel> result;
            if (isPersonal)
            {
                result = await proRep.GetSocialProfilesPersonalMode(userId, 4);
            }
            else
            {
                result = await proRep.GetSocialProfilesByGroupId(userId, groupId, companyId, 4);
            }

            List<ContactTwitListModel> twitLists = new List<ContactTwitListModel>();
            foreach (ContactFollowStatusModel followStatus in result)
            {
                TwitterUtility utility = new TwitterUtility(followStatus.UserName);
                List<List> userLists = await utility.getUserTwitterLists(followStatus.UserName, 1000);
                foreach (List ulist in userLists)
                {
                    ContactTwitListModel listModel = new ContactTwitListModel();
                    listModel.Id = ulist.ListIDResponse.ToString();
                    listModel.Name = ulist.Name;
                    listModel.Description = ulist.Description;
                    listModel.OwnerName = ulist.Users[0].Name;
                    listModel.OwnerUserName = followStatus.UserName;
                    listModel.OwnerId = followStatus.UserId;
                    listModel.Slug = ulist.SlugResponse;
                    listModel.Mode = ulist.Mode;
                    if (checkMembership)
                    {
                        listModel.IsMember = await utility.isUserMemberOfList(ofUser, ulist.ListIDResponse);
                    }
                    else
                    {
                        listModel.IsMember = false;
                    }

                    twitLists.Add(listModel);
                }
            }

            return twitLists;
        }

        public async void AddContactToUserLists(string ofUser, List<ContactTwitListModel> listIds)
        {
            foreach (ContactTwitListModel ulist in listIds)
            {
                TwitterUtility utility = new TwitterUtility(ulist.OwnerUserName);
                await utility.addMemberToTwitterList(ofUser, ulist.Slug, ulong.Parse(ulist.OwnerId), ulist.OwnerUserName);
            }
        }

        public async void RemoveContactFromUserList(string ofUser, ContactTwitListModel ulist)
        {
            TwitterUtility utility = new TwitterUtility(ulist.OwnerUserName);
            await utility.deleteMemberfromTwitterList(ofUser, ulist.Slug, ulong.Parse(ulist.OwnerId), ulist.OwnerUserName);
        }

        public async Task<ContactTwitListModel> CreateNewList(string fromUser, string ofUser, string listName, string mode, string description)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            List newList = await utility.createNewTwitterList(listName, mode, description);
            ContactTwitListModel listModel = new ContactTwitListModel();
            listModel.Id = newList.ListIDResponse.ToString();
            listModel.Name = newList.Name;
            listModel.Description = newList.Description;
            listModel.OwnerName = newList.Users[0].Name;
            listModel.OwnerUserName = newList.Users[0].ScreenNameResponse;
            listModel.OwnerId = newList.Users[0].UserIDResponse;
            listModel.Slug = newList.SlugResponse;
            listModel.Mode = newList.Mode;

            await utility.addMemberToTwitterList(ofUser, listModel.Slug, ulong.Parse(listModel.OwnerId), listModel.OwnerUserName);
            listModel.IsMember = true;

            return listModel;
        }

        public async void DeleteTwitterList(string fromUser, ulong listId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            List newList = await utility.deleteUserTwitterList(listId);
        }

        public async void EditTwitterList(ContactTwitListModel listModel)
        {
            TwitterUtility utility = new TwitterUtility(listModel.OwnerUserName);
            await utility.updateTwitterList(ulong.Parse(listModel.Id), listModel.Name, listModel.Mode, listModel.Description);
        }

        public async Task<List<UserOwnedSocialProfiles>> GetContactOwnedSocialProfiles(string ofUser)
        {
            List<UserOwnedSocialProfiles> userOwnedProfiles = new List<UserOwnedSocialProfiles>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    userOwnedProfiles = context.Profiles.Where(x => x.UserName != ofUser && x.MemberUserId == context.Profiles.Where(y => y.UserName == ofUser && y.ProfileTypeId == 4).Select(z => z.MemberUserId).FirstOrDefault())
                    .Select(t => new UserOwnedSocialProfiles
                    {
                        ProfileTypeId = t.ProfileTypeId,
                        ProfileURL = t.ProfileURl
                    }).ToList();

                    context.SaveChanges();
                }
            });

            return userOwnedProfiles;
        }

        public async Task<TwitterProfileReportModel> GetProfilesReportStats(string[] profiles, DateTime fromDate, DateTime toDate)
        {
            try
            {
                TwitterProfileReportModel twitReportObj = new TwitterProfileReportModel();
                var dates = new List<DateWiseActivityCount>();
                DateTime lastFromDate = fromDate.AddDays((fromDate - toDate).Days);

                for (var dt = fromDate; dt <= toDate; dt = dt.AddDays(1))
                {
                    dates.Add(new DateWiseActivityCount { DateValue = dt, ActivityCount = 0 });
                }

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
                            twitReportObj.TotalTweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).Count();
                            twitReportObj.TotalReplies = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId != "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).Count();
                            twitReportObj.TotalTweetsSent = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).Count();
                            twitReportObj.TweetsSentSpan = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.TweetsSentSpan = (from dt in dates
                                                            join pt in twitReportObj.TweetsSentSpan
                                                            on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                            from dum in joinPtDt.DefaultIfEmpty()
                                                            select new DateWiseActivityCount()
                                                            {
                                                                ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                                DateValue = dt.DateValue
                                                            }).ToList();

                            twitReportObj.TotalMentionsReceived = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsMention == true).Count();
                            twitReportObj.MentionsReceivedSpan = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsMention == true)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.MentionsReceivedSpan = (from dt in dates
                                                                  join pt in twitReportObj.MentionsReceivedSpan
                                                                  on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                                  from dum in joinPtDt.DefaultIfEmpty()
                                                                  select new DateWiseActivityCount()
                                                                  {
                                                                      ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                                      DateValue = dt.DateValue
                                                                  }).ToList();

                            twitReportObj.TotalDMsSent = context.MessagesTwitters.Where(x => profiles.Contains(x.SenderId) && profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).Count();
                            twitReportObj.DMsSentSpan = context.MessagesTwitters.Where(x => profiles.Contains(x.SenderId) && profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.DMsSentSpan = (from dt in dates
                                                         join pt in twitReportObj.DMsSentSpan
                                                         on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                         from dum in joinPtDt.DefaultIfEmpty()
                                                         select new DateWiseActivityCount()
                                                         {
                                                             ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                             DateValue = dt.DateValue
                                                         }).ToList();

                            twitReportObj.TotalDMsReceived = context.MessagesTwitters.Where(x => profiles.Contains(x.RecipientId) && profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).Count();
                            twitReportObj.DMsReceivedSpan = context.MessagesTwitters.Where(x => profiles.Contains(x.RecipientId) && profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.DMsReceivedSpan = (from dt in dates
                                                             join pt in twitReportObj.DMsReceivedSpan
                                                             on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                             from dum in joinPtDt.DefaultIfEmpty()
                                                             select new DateWiseActivityCount()
                                                             {
                                                                 ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                                 DateValue = dt.DateValue
                                                             }).ToList();

                            twitReportObj.PhotoTweetsCount = (from dt in context.PostsTwitters
                                                              join pt in context.FeedsMediaAttachments
                                                              on dt.Id equals pt.MediaId
                                                              where pt.MediaType == "TP" &&
                                                              (pt.Description == "animated_gif" || pt.Description == "photo") &&
                                                              profiles.Contains(dt.AuthorUserId) &&
                                                              profiles.Contains(dt.UserId) &&
                                                              (dt.CreatedDate >= fromDate && dt.CreatedDate <= toDate)
                                                              select dt.Id).Count();
                            twitReportObj.VideoTweetsCount = (from dt in context.PostsTwitters
                                                              join pt in context.FeedsMediaAttachments
                                                              on dt.Id equals pt.MediaId
                                                              where pt.MediaType == "TP" &&
                                                              pt.Description == "video" &&
                                                              profiles.Contains(dt.AuthorUserId) &&
                                                              profiles.Contains(dt.UserId) &&
                                                              (dt.CreatedDate >= fromDate && dt.CreatedDate <= toDate)
                                                              select dt.Id).Count();
                            twitReportObj.ShareTweetsCount = (from dt in context.PostsTwitters
                                                              join pt in context.FeedsMediaAttachments
                                                              on dt.Id equals pt.MediaId
                                                              where pt.MediaType == "TP" &&
                                                              pt.Description == "share" &&
                                                              profiles.Contains(dt.AuthorUserId) &&
                                                              profiles.Contains(dt.UserId) &&
                                                              (dt.CreatedDate >= fromDate && dt.CreatedDate <= toDate)
                                                              select dt.Id).Count();
                            twitReportObj.BasicTweetsCount = twitReportObj.TotalTweetsSent - (twitReportObj.PhotoTweetsCount + twitReportObj.VideoTweetsCount);

                            twitReportObj.TotalProfileTweets = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).Count();
                            twitReportObj.TotalProfileDMs = context.MessagesTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).Count();

                            twitReportObj.TotalEngTweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == false).Count();
                            twitReportObj.EngTweetsSpan = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == false)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.EngTweetsSpan = (from dt in dates
                                                           join pt in twitReportObj.EngTweetsSpan
                                                           on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                           from dum in joinPtDt.DefaultIfEmpty()
                                                           select new DateWiseActivityCount()
                                                           {
                                                               ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                               DateValue = dt.DateValue
                                                           }).ToList();

                            twitReportObj.TotalEngReplies = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId != "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == false).Count();
                            twitReportObj.EngRepliesSpan = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId != "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == false)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.EngRepliesSpan = (from dt in dates
                                                            join pt in twitReportObj.EngRepliesSpan
                                                            on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                            from dum in joinPtDt.DefaultIfEmpty()
                                                            select new DateWiseActivityCount()
                                                            {
                                                                ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                                DateValue = dt.DateValue
                                                            }).ToList();

                            twitReportObj.TotalEngRetweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == true).Count();
                            twitReportObj.EngRetweetsSpan = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == true)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.EngRetweetsSpan = (from dt in dates
                                                             join pt in twitReportObj.EngRetweetsSpan
                                                             on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                             from dum in joinPtDt.DefaultIfEmpty()
                                                             select new DateWiseActivityCount()
                                                             {
                                                                 ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                                 DateValue = dt.DateValue
                                                             }).ToList();

                            twitReportObj.TopFiveCustomers = (from m in context.PostsTwitters
                                                              join p in context.CustomerSocialProfiles
                                                              on m.AuthorUserId equals p.ExternalId
                                                              where profiles.Contains(m.UserId) && !profiles.Contains(m.AuthorUserId) && p.Type == "Twitter" && m.CreatedDate >= fromDate && m.CreatedDate <= toDate
                                                              group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender } into g //, m.AuthorUserId
                                                              select new TwitterTopCustomers
                                                              {
                                                                  //UserId = g.Key.AuthorUserId,
                                                                  CustomerId = g.Key.ExternalId,
                                                                  UserName = g.Key.UserName,
                                                                  Name = g.Key.Name,
                                                                  ProfilePic = g.Key.ProfilePic,
                                                                  Gender = g.Key.Gender,
                                                                  Count = g.Count(),
                                                                  MsgCount = 0,
                                                                  PostCount = g.Count()
                                                              }
                                        ).Union(from m in context.MessagesTwitters
                                                join p in context.CustomerSocialProfiles
                                                on m.SenderId equals p.ExternalId
                                                where profiles.Contains(m.RecipientId) && p.Type == "Twitter" && m.CreatedDate >= fromDate && m.CreatedDate <= toDate
                                                group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender } into g //, m.RecipientId
                                        select new TwitterTopCustomers
                                                {
                                            //UserId = g.Key.RecipientId,
                                            CustomerId = g.Key.ExternalId,
                                                    UserName = g.Key.UserName,
                                                    Name = g.Key.Name,
                                                    ProfilePic = g.Key.ProfilePic,
                                                    Gender = g.Key.Gender,
                                                    Count = g.Count(),
                                                    MsgCount = g.Count(),
                                                    PostCount = 0
                                                }
                                        ).GroupBy(p => new { p.CustomerId, p.UserName, p.Name, p.ProfilePic, p.Gender })
                                        .Select(
                                            g => new TwitterTopCustomers
                                            {
                                                CustomerId = g.Key.CustomerId,
                                                UserName = g.Key.UserName,
                                                Name = g.Key.Name,
                                                ProfilePic = g.Key.ProfilePic,
                                                Gender = g.Key.Gender,
                                                Count = g.Sum(s => s.Count),
                                                MsgCount = g.Sum(s => s.MsgCount),
                                                PostCount = g.Sum(s => s.PostCount)
                                            }).OrderByDescending(x => x.Count).Take(5).ToList();

                            //twitReportObj.TopFiveCustomers = twitReportObj.TopFiveCustomers.OrderByDescending(x => x.Count).Take(5).ToList();

                            //foreach (var obj in twitReportObj.TopFiveCustomers)
                            //{
                            //    obj.MsgCount = (from m in context.MessagesTwitters
                            //                    join p in context.CustomerSocialProfiles
                            //                    on m.SenderId equals p.ExternalId
                            //                    where profiles.Contains(m.RecipientId) && p.ExternalId == obj.CustomerId && p.Type == "Twitter" && m.CreatedDate >= fromDate && m.CreatedDate <= toDate select p).Count();

                            //    obj.PostCount = (from m in context.PostsTwitters
                            //                     join p in context.CustomerSocialProfiles
                            //                     on m.AuthorUserId equals p.ExternalId
                            //                     where profiles.Contains(m.UserId) && p.ExternalId == obj.CustomerId && !profiles.Contains(m.AuthorUserId) && p.Type == "Twitter" && m.CreatedDate >= fromDate && m.CreatedDate <= toDate
                            //                     select p).Count();
                            //}

                            //var messagesT = (from m in context.MessagesTwitters
                            //                 join p in context.CustomerSocialProfiles
                            //                 on m.SenderId equals p.ExternalId
                            //                 //where profiles.Contains(m.RecipientId) && p.Type == "Twitter" //&& m.CreatedDate >= fromDate && m.CreatedDate <= toDate
                            //                 group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender, m.RecipientId } into g
                            //                 select new 
                            //                 {
                            //                     UserId = g.Key.RecipientId,
                            //                     ExternalId = g.Key.ExternalId,
                            //                     UserName = g.Key.UserName,
                            //                     Name = g.Key.Name,
                            //                     ProfilePic = g.Key.ProfilePic,
                            //                     Gender = g.Key.Gender,
                            //                     Count = g.Count()
                            //                 }
                            //        ).ToList();

                            //var postsT = (from m in context.PostsTwitters
                            //              join p in context.CustomerSocialProfiles
                            //              on m.AuthorUserId equals p.ExternalId
                            //             // where profiles.Contains(m.AuthorUserId) && !profiles.Contains(m.UserId) && p.Type == "Twitter" //&& m.CreatedDate >= fromDate && m.CreatedDate <= toDate
                            //              group p by new { p.ExternalId, p.UserName, p.Name, p.ProfilePic, p.Gender, m.AuthorUserId } into g
                            //              select new
                            //              {
                            //                  UserId = g.Key.AuthorUserId,
                            //                  ExternalId = g.Key.ExternalId,
                            //                  UserName = g.Key.UserName,
                            //                  Name = g.Key.Name,
                            //                  ProfilePic = g.Key.ProfilePic,
                            //                  Gender = g.Key.Gender,
                            //                  Count = g.Count()
                            //              }
                            //        ).ToList();

                            twitReportObj.LastTotalTweetsSent = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate).Count();
                            twitReportObj.LastTotalMentionsReceived = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && profiles.Contains(x.UserId) && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate && x.IsMention == true).Count();
                            twitReportObj.LastTotalDMsSent = context.MessagesTwitters.Where(x => profiles.Contains(x.SenderId) && profiles.Contains(x.UserId) && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate).Count();
                            twitReportObj.LastTotalDMsReceived = context.MessagesTwitters.Where(x => profiles.Contains(x.RecipientId) && profiles.Contains(x.UserId) && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate).Count();
                            twitReportObj.LastTotalEngTweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate && x.IsRetweet == false).Count();
                            twitReportObj.LastTotalEngReplies = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId != "0" && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate && x.IsRetweet == false).Count();
                            twitReportObj.LastTotalEngRetweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate && x.IsRetweet == true).Count();
                        }
                    }
                });

                return twitReportObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TwitterProfileReportModel> GetProfilesReportStatsED(string[] profiles, DateTime fromDate, DateTime toDate)
        {
            try
            {
                TwitterProfileReportModel twitReportObj = new TwitterProfileReportModel();
                var dates = new List<DateWiseActivityCount>();
                DateTime lastFromDate = fromDate.AddDays((fromDate - toDate).Days);

                for (var dt = fromDate; dt <= toDate; dt = dt.AddDays(1))
                {
                    dates.Add(new DateWiseActivityCount { DateValue = dt, ActivityCount = 0 });
                }

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

                            twitReportObj.TotalMentionsReceived = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsMention == true).Count();
                            twitReportObj.MentionsReceivedSpan = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsMention == true)
                                .GroupBy(z => DbFunctions.TruncateTime(z.CreatedDate)).Select(y => new DateWiseActivityCount
                                {
                                    DateValue = y.Key,
                                    ActivityCount = y.Count()
                                }).ToList();
                            twitReportObj.MentionsReceivedSpan = (from dt in dates
                                                                  join pt in twitReportObj.MentionsReceivedSpan
                                                                  on dt.DateValue.Value.Date equals pt.DateValue.Value.Date into joinPtDt
                                                                  from dum in joinPtDt.DefaultIfEmpty()
                                                                  select new DateWiseActivityCount()
                                                                  {
                                                                      ActivityCount = dum != null ? dum.ActivityCount : dt.ActivityCount,
                                                                      DateValue = dt.DateValue
                                                                  }).ToList();
                           // twitReportObj.TotalEngTweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == false).Count();
                            twitReportObj.TotalEngReplies = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId != "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == false).Count();
                            twitReportObj.TotalEngRetweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == true).Count();
                            twitReportObj.LastTotalEngTweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate && x.IsRetweet == false).Count();
                            twitReportObj.LastTotalEngReplies = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId != "0" && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate && x.IsRetweet == false).Count();
                            twitReportObj.LastTotalEngRetweets = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= lastFromDate && x.CreatedDate <= fromDate && x.IsRetweet == true).Count();
                        }
                    }
                });

                return twitReportObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TwitterProfileDrillDownReportModel>> GetProfilesReportStatsEDDrillDown(string[] profiles, DateTime fromDate, DateTime toDate, string ReportType, int PageNumber, int PageSize)
        {
            try
            {
                List<DateWiseActivityCount> MentionsReceivedSpan = new List<DateWiseActivityCount>();
                var dates = new List<DateWiseActivityCount>();
                DateTime lastFromDate = fromDate.AddDays((fromDate - toDate).Days);

                for (var dt = fromDate; dt <= toDate; dt = dt.AddDays(1))
                {
                    dates.Add(new DateWiseActivityCount { DateValue = dt, ActivityCount = 0 });
                }

                List<TwitterProfileDrillDownReportModel> twitReportObj = new List<TwitterProfileDrillDownReportModel>();
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
                            if(ReportType == "Tweets")
                                twitReportObj = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsMention == true).Select(
                                            g => new TwitterProfileDrillDownReportModel
                                            {
                                                StatusText = g.StatusText,
                                                InsertionDate = g.InsertionDate.Value
                                            }).OrderBy(x => x.InsertionDate).Skip((PageNumber) * PageSize).Take(PageSize).ToList();
                            if (ReportType == "Replies")
                                twitReportObj = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId != "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == false).Select(
                                            g => new TwitterProfileDrillDownReportModel
                                            {
                                                StatusText = g.StatusText,
                                                InsertionDate = g.InsertionDate.Value
                                            }).OrderBy(x => x.InsertionDate).Skip((PageNumber) * PageSize).Take(PageSize).ToList();
                            if (ReportType == "Retweets")
                                twitReportObj = context.PostsTwitters.Where(x => profiles.Contains(x.AuthorUserId) && profiles.Contains(x.UserId) && x.InReplyToStatusId == "0" && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsRetweet == true).Select(
                                            g => new TwitterProfileDrillDownReportModel
                                            {
                                                StatusText = g.StatusText,
                                                InsertionDate = g.InsertionDate.Value
                                            }).OrderBy(x => x.InsertionDate) .Skip((PageNumber) * PageSize).Take(PageSize).ToList();

                            if (ReportType == "Mention")
                            {
                                twitReportObj = context.PostsTwitters.Where(x => profiles.Contains(x.UserId) && x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsMention == true)
                                  .Select(
                                            g => new TwitterProfileDrillDownReportModel
                                            {
                                                StatusText = g.StatusText,
                                                InsertionDate = g.InsertionDate.Value
                                            }).OrderBy(x => x.InsertionDate).Skip((PageNumber) * PageSize).Take(PageSize).ToList();                              
                            }
                        }
                    }
                });
                return twitReportObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public async Task<List<InboxItemModel>> GetTweetsMessagesByStatusId(string StatusId)
        //{
        //    var result = new List<InboxItemModel>();
        //    await Task.Run(() =>
        //    {
        //        using (var db = new SocialCRMEntities())
        //        {
        //            result = db.MessagesTwitters.Where(x => x. == StatusId).Select(y => new InboxItemModel
        //            {
        //                AccountTypeImg = y.AuthorProfilePic,
        //                Id = y.Id,
        //                ReplyedBy = y.AuthorName,
        //                ToUser = y.InReplyToScreenName,
        //                TwtImg = y.AuthorProfilePic,
        //                UserTwit = y.StatusText,
        //                ReplyedDate = DatetimeExtensions.formatMsgDatetime(Convert.ToDateTime(y.CreatedDate))
        //            }).ToList();
        //            db.Dispose();
        //        }
        //    });
        //    return result;
        //}

        public async Task<TwitterTweetThread> GetTweetThread(string ofUser, ulong tweetId)
        {
            TwitterTweetThread result = new TwitterTweetThread();
            string tweetStr = Convert.ToString(tweetId);
            await Task.Run(() =>
            {
                try
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
                            var tweet = context.PostsTwitters.Where(x => x.StatusId == tweetStr && x.AuthorName == ofUser).FirstOrDefault();
                            var dateBefore = tweet.CreatedDate.Value.AddDays(-3);
                            var dateAfter = tweet.CreatedDate.Value.AddDays(3);
                            var allTweets = context.PostsTwitters.Where(x => x.KeywordId.HasValue == false && x.CreatedDate >= dateBefore && x.CreatedDate <= dateAfter).ToList();
                            if (tweet != null)
                            {
                                // actual tweet
                                var actualTweet = new InboxItemModel();
                                fillTweetObj(ref actualTweet, tweet, "tweet");
                                result.Tweet = actualTweet;

                                // child tweets
                                result.ChildTweets = new List<InboxItemModel>();
                                var childTweets = new List<PostsTwitter>();
                                PopulateChildTweets(tweetStr, childTweets, allTweets);
                                foreach (var x in childTweets)
                                {
                                    var children = new InboxItemModel();
                                    fillTweetObj(ref children, x, "reply");
                                    result.ChildTweets.Add(children);
                                }
                                result.ChildTweets = result.ChildTweets.GroupBy(g => new { g.StatusId, g.MessageType }).Select(g => g.First()).OrderBy(x => x.CreatedDate).ToList();

                                // parent tweets
                                result.ParentTweets = new List<InboxItemModel>();
                                var parentTweetList = new List<InboxItemModel>();
                                RenderParentTweets(ref parentTweetList, allTweets, tweet);
                                result.ParentTweets.AddRange(parentTweetList.GroupBy(g => new { g.StatusId, g.MessageType }).Select(g => g.First()).OrderBy(x => x.CreatedDate));
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

        private void RenderParentTweets(ref List<InboxItemModel> parentTweetList, List<PostsTwitter> allTweets, PostsTwitter tweet)
        {
            try
            {
                if(!String.IsNullOrEmpty(tweet.InReplyToStatusId))
                {
                    var parentTweet = allTweets.Where(x => x.StatusId == tweet.InReplyToStatusId).FirstOrDefault();
                    if (parentTweet != null)
                    {
                        var tweetObj = new InboxItemModel();
                        fillTweetObj(ref tweetObj, parentTweet, "tweet");
                        parentTweetList.Add(tweetObj);
                        RenderParentTweets(ref parentTweetList, allTweets, parentTweet);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fillTweetObj (ref InboxItemModel tweetObj, PostsTwitter fromTweet, string tweetType)
        {
            if (fromTweet != null)
            {
                tweetObj.Id = fromTweet.Id;
                tweetObj.ClassName = "unread_msg";
                tweetObj.TwtImg = fromTweet.AuthorProfilePic;
                tweetObj.IsVerified = Convert.ToBoolean(fromTweet.IsAuthorVerified);
                tweetObj.User = fromTweet.InReplyToScreenName;
                tweetObj.UserTwit = fromTweet.StatusText;
                tweetObj.ReplyedBy = fromTweet.AuthorName;
                tweetObj.ToUser = fromTweet.InReplyToScreenName;
                tweetObj.ReplyedByFullName = fromTweet.AuthorFullName;
                tweetObj.ReplyedByUserId = fromTweet.InReplyToUserId;
                tweetObj.ReplyedDateForDeck = DatetimeExtensions.formatMsgDatetimeForDeck((DateTime)fromTweet.CreatedDate);
                tweetObj.ReplyedDate = DatetimeExtensions.formatMsgDatetime((DateTime)fromTweet.CreatedDate);
                tweetObj.CreatedDate = (DateTime)fromTweet.CreatedDate;
                tweetObj.ActivityType = tweetType == "tweet" ? "sent a tweet " : "replied to";
                tweetObj.AccountTypeImg = "t_ico16";
                tweetObj.MessageType = "TP";
                tweetObj.IsArchived = false;
                tweetObj.IsRetweet = Convert.ToBoolean(fromTweet.IsRetweet);
                tweetObj.StatusId = fromTweet.StatusId;
                tweetObj.Urls = new List<string>();
                using (SocialCRMEntities db = new SocialCRMEntities())
                {
                    if (db.FeedsMediaAttachments.ToList().Where(x => x.MediaId == fromTweet.Id && x.MediaType == "TP").Count() > 0)
                    {
                        tweetObj.Urls = db.FeedsMediaAttachments.ToList().Where(x => x.MediaId == fromTweet.Id && x.MediaType == "TP").Select(y => y.Description + "|" + y.AttachmentUrl).ToList<string>();
                    }
                }

                UnifiedInboxUtility.CheckForAttachments(ref tweetObj);
            }
        }

        private void PopulateChildTweets(string parent, List<PostsTwitter> children, List<PostsTwitter> allItems)
        {
            var newChildren = allItems.Where(x => x.InReplyToStatusId == parent).ToList();
            if (newChildren.Count() > 0)
            {
                children.AddRange(newChildren);
                foreach (PostsTwitter child in newChildren)
                {
                    PopulateChildTweets(child.StatusId, children, allItems);
                }
            }
        }

        public async Task<bool> TwitterDeleteDirectMessageEvent(string fromUser, ulong messageId)
        {
            TwitterUtility utility = new TwitterUtility(fromUser);
            return await utility.deleteDirectMessageEvent(messageId);
        }

        public async Task<DMEvent> TwitterSendDMEvent(string userId, string fromUser, string toUser, string status, int teamId, int companyId)
        {
            DMEvent response = null;
            SocialCRMEntities db = new SocialCRMEntities();
            var fromProfile = db.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault();
            var fromUserName = fromProfile.UserName;
            ulong senderId=0;
            var senderTwitterAccount = db.MessagesTwitters.Where(x=>x.SenderScreenName==toUser).FirstOrDefault();
            if (senderTwitterAccount != null)
            {
                ulong.TryParse(senderTwitterAccount.SenderId, out senderId);
            }
            else
            {
                senderTwitterAccount = db.MessagesTwitters.Where(x => x.RecipientScreenName == toUser).FirstOrDefault();
                if(senderTwitterAccount!=null)
                    ulong.TryParse(senderTwitterAccount.RecipientId, out senderId);
            }

            if (senderTwitterAccount != null&& senderId != 0)
            {
                string toUserName = fromUserName == fromUser ? toUser : fromUser;
                TwitterUtility utility = new TwitterUtility(fromUserName);
                 response = await utility.sendDirectMessageEvent(senderId, status);
                string profileId = db.Profiles.Where(z => z.UserName == fromProfile.UserName && z.ProfileTypeId == 4).FirstOrDefault().ProfileExternalId;
                var id = response.ID;
                await new MiningRepository().AddSocialActivity(userId, "", "", "TM", 2, "", status, teamId, companyId, fromUserName, toUserName, id);
            }
            return response;
        }

        public async Task<DMEvent> TwitterSendDirectMessageEvent(string fromUser, string toUser, string status)
        {
            DMEvent response = null;
            TwitterUtility utility = new TwitterUtility(fromUser);
            SocialCRMEntities db = new SocialCRMEntities();
            ulong senderId = 0;
            var senderTwitterAccount = db.MessagesTwitters.Where(x => x.SenderScreenName == toUser).FirstOrDefault();
            if (senderTwitterAccount != null)
            {
                ulong.TryParse(senderTwitterAccount.SenderId, out senderId);
            }
            else
            {
                senderTwitterAccount = db.MessagesTwitters.Where(x => x.RecipientScreenName == toUser).FirstOrDefault();
                if (senderTwitterAccount != null)
                    ulong.TryParse(senderTwitterAccount.RecipientId, out senderId);
            }
            if(senderId!=0)
                response = await utility.sendDirectMessageEvent(senderId, status);
            return response;
        }
    }
}