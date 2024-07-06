using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight360.BO;
using System.Data.Entity;

namespace Insight360.DAL
{
    public static class TwitterDBOperations
    {
        public static List<TwitterUserAccount> GetTwitterAccounts()
        {
            var account = new List<TwitterUserAccount>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    account = (from p in db.Profiles
                                join ust in db.PostUserStatusTwitters on p.ProfileExternalId equals ust.UserId
                                where p.ProfileTypeId == 4
                                select new TwitterUserAccount
                                {
                                    ProfileId = p.Id,
                                    ProfileExternalId = p.ProfileExternalId,
                                    AccessToken = p.AcessToken,
                                    AccessTokenSecret = p.AcessTokenSecret,
                                    MaxMsgSendToId = ust.MaxMsgSendToId,
                                    MaxMsgSendById = ust.MaxMsgSendById,
                                    MaxStatusId = ust.MaxStatusId,
                                    MaxMentionStatusId = ust.MaxMentionStatusId,
                                    Cursor = ust.MaxMsgSendById
                                }).ToList();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return account;
        }

        public static List<SearchKeyword> GetSearchKeywords()
        {
            var account = new List<SearchKeyword>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    account = (from p in db.BrandKeywords
                                join ust in db.PostUserStatusTwitters on p.Id equals ust.KeywordId
                                where p.IsTag == false
                                select new SearchKeyword
                                {
                                    KeywordId = p.Id,
                                    KeywordText = p.KeywordText,
                                    LocationName = p.LocationName,
                                    LocationLat = p.LocationLat,
                                    LocationLng = p.LocationLng,
                                    MaxStatusId = ust.MaxStatusId
                                }).ToList();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return account;
        }

        public static void UpdateTweetStatus(ulong lastFetchedUserId, ulong lastFetchedMentionId, ulong lastFetchedMsgSentToId, ulong lastFetchedMsgSentById, string userId, int? keywordId)
        {
            if (!(lastFetchedUserId == 0 && lastFetchedMentionId == 0 && lastFetchedMsgSentToId == 0 && lastFetchedMsgSentById == 0))
            {
                using (var db = new SocialCRMMinningEntities())
                {
                    try
                    {
                        PostUserStatusTwitter existingContact;
                        if(keywordId == null)
                        {
                            existingContact = db.PostUserStatusTwitters
                                                 .Where(x => x.UserId == userId)
                                                 .SingleOrDefault();
                        }
                        else
                        {
                            existingContact = db.PostUserStatusTwitters
                                                 .Where(x => x.KeywordId == keywordId)
                                                 .SingleOrDefault();
                        }
                        
                        if (existingContact != null)
                        {
                            if (lastFetchedUserId != 0)
                                existingContact.MaxStatusId = lastFetchedUserId.ToString();
                            if (lastFetchedMentionId != 0)
                                existingContact.MaxMentionStatusId = lastFetchedMentionId.ToString();
                            if (lastFetchedMsgSentToId != 0)
                                existingContact.MaxMsgSendToId = lastFetchedMsgSentToId.ToString();
                            if (lastFetchedMsgSentById != 0)
                                existingContact.MaxMsgSendById = lastFetchedMsgSentById.ToString();

                            db.PostUserStatusTwitters.Attach(existingContact);
                            db.Entry(existingContact).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            }
        }

        public static void InsertUserTweets(List<BO.TwitterTweet> currentTweets, string userId, int? keywordId, int ProfileId)
        {
            if(currentTweets.Count > 0)
            {
                using (var db = new SocialCRMMinningEntities())
                {
                    try
                    {
                        var statIds = currentTweets.Select(i => i.StatusId).ToList();
                        var statIdsInDb = db.PostsTwitters.Where(i => statIds.Contains(i.StatusId) && i.UserId == userId).Select(i => i.StatusId).ToList();
                        var statIdsNotInDb = new HashSet<string>(statIds.Except(statIdsInDb));
                        var itemsNotInDb = currentTweets.Where(i => statIdsNotInDb.Contains(i.StatusId)).ToList();
                        foreach (var obj in itemsNotInDb)
                        {
                          var result = db.PostsTwitters.Add(new PostsTwitter
                            {
                                ProfileId = ProfileId,
                                StatusId = obj.StatusId,
                                UserId = userId,
                                AuthorUserId = obj.AuthorUserId,
                                StatusText = obj.StatusText,
                                AuthorName = obj.AuthorName,
                                AuthorFullName = obj.AuthorFullName,
                                InReplyToStatusId = obj.InReplyToStatusId,
                                InReplyToUserId = obj.InReplyToUserId,
                                InReplyToScreenName = obj.InReplyToScreenName,
                                CreatedDate = obj.CreatedDate,
                                AuthorProfilePic = obj.AuthorProfilePic,
                                IsAuthorVerified = obj.IsAuthorVerified,
                                IsMention = obj.IsMention,
                                IsRetweet = obj.IsRetweet,
                                InsertionDate = DateTime.Now,
                                KeywordId = keywordId,
                                AuthorFollowers = obj.AuthorFollowers
                            });
                            

                            if (obj.MediaAttachment != null && obj.MediaAttachment.Count > 0)
                            {
                                db.SaveChanges();

                                obj.MediaAttachment.ForEach(delegate (FeedMediaAttachment attachment)
                                {
                                    attachment.MediaId = result.Id;
                                });

                                foreach (var item in obj.MediaAttachment)
                                {
                                    db.FeedsMediaAttachments.Add(new FeedsMediaAttachment
                                    {
                                        MediaId = item.MediaId,
                                        MediaType = item.MediaType,
                                        Description = item.Description,
                                        AttachmentUrl = item.AttachmentUrl
                                    });
                                }                                
                            }                           
                        }
                        db.SaveChanges();
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            }
        }

        public static void InsertUserMsgs(List<BO.TwitterMessage> currentMsgs, string userId, int ProfileId)
        {
            if(currentMsgs.Count > 0)
            {
                using (var db = new SocialCRMMinningEntities())
                {
                    try
                    {
                        var commIds = currentMsgs.Select(i => i.MsgId).ToList();
                        var commIdsInDb = db.MessagesTwitters.Where(i => commIds.Contains(i.MsgId)).Select(i => i.MsgId).ToList();
                        var commIdsNotInDb = new HashSet<string>(commIds.Except(commIdsInDb));
                        var itemsNotInDb = currentMsgs.Where(i => commIdsNotInDb.Contains(i.MsgId)).ToList();
                        foreach (var obj in itemsNotInDb)
                        {
                            db.MessagesTwitters.Add(new MessagesTwitter
                            {
                                ProfileId = ProfileId,
                                UserId = userId,
                                MsgId = obj.MsgId,
                                RecipientId = obj.RecipientId,
                                RecipientScreenName = obj.RecipientScreenName,
                                RecipientFullName = obj.RecipientFullName,
                                SenderId = obj.SenderId,
                                SenderScreenName = obj.SenderScreenName,
                                SenderFullName = obj.SenderFullName,
                                MsgText = obj.MsgText,
                                CreatedDate = obj.CreatedDate,
                                SenderProfilePic = obj.SenderProfilePic,
                                IsSenderVerified = obj.IsSenderVerified,
                                InsertionDate = DateTime.Now,
                                Attachment = obj.Attachment,
                                AttachmentType = obj.AttachmentType,
                                SenderFollowers = obj.SenderFollowers
                            });
                        }
                        db.SaveChanges();
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            }
        }

        public static void UpdateUserCursor(string cursorValue, string userId)
        {
            if (!string.IsNullOrEmpty(cursorValue)&&!string.IsNullOrEmpty(userId))
            {
                using (var db = new SocialCRMMinningEntities())
                {
                    try
                    {
                        PostUserStatusTwitter existingContact;
                        existingContact = db.PostUserStatusTwitters
                            .Where(x => x.UserId == userId)
                            .SingleOrDefault();

                        if (existingContact != null)
                        {
                            existingContact.MaxMsgSendById = cursorValue;
                            existingContact.MaxMsgSendToId = cursorValue;
                            db.PostUserStatusTwitters.Attach(existingContact);
                            db.Entry(existingContact).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            }
        }
    }
}
