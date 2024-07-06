using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight360.BO;
using System.Data.Entity;

namespace Insight360.DAL
{
    public static class FacebookDBOperations
    {
        public static List<FacebookUserAccount> GetFacebookAccounts(bool isPage)
        {
            var account = new List<FacebookUserAccount>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    if (isPage)
                    {
                        var acc = db.ProfilePages.Where(x => x.ProfileComapnyGroups.Count > 0)
                            .Select(x => new FacebookUserAccount
                        {
                            ProfileId = x.Profile.Id,
                            ProfilePageId = x.Id,
                            UserId = x.ProfileExternalId,
                            PageId = x.ProfilePageID,
                            AccessToken = x.PageAccessToken,
                            ProfilePic = x.ProfilePic,
                            CompanyId = x.Profile.CompanyId
                        }).FirstOrDefault();
                        account.Add(acc);
                    }
                    else
                    {
                        account = db.Profiles.Where(y => y.ProfileTypeId == 1 && y.IsActive == true)
                                    .Select(x => new FacebookUserAccount
                                    {
                                        ProfileId = x.Id,
                                        ProfilePageId = null,
                                        UserId = x.ProfileExternalId,
                                        AccessToken = x.AcessToken,
                                        PageId = null,
                                        ProfilePic = x.ProfilePic,
                                        CompanyId = x.CompanyId
                                    }).ToList();
                    }
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return account;
        }

        public static List<CommentRelationInfo> GetCommentRelationInfo(string userId, string pageId)
        {
            var commRel = new List<CommentRelationInfo>();
            DateTime previousNonKEDay = DateTime.Today.AddDays(-1);
            DateTime previousKEDay = DateTime.Today.AddDays(-30);
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    commRel = db.PostsFBs.Where(x => x.UserId == userId && x.PageId == pageId && !(x.AuthorUserId == userId || x.AuthorUserId == pageId) && x.CreatedDate > previousNonKEDay)
                                .Select(y => new CommentRelationInfo
                                {
                                    PostId = y.PostId,
                                    MaxCommentId = y.MaxCommentId
                                }).ToList();
                    commRel.AddRange(
                        db.PostsFBs.Where(x => x.UserId == userId && x.PageId == pageId && (x.AuthorUserId == userId || x.AuthorUserId == pageId) && x.CreatedDate > previousKEDay)
                            .Select(y => new CommentRelationInfo
                            {
                                PostId = y.PostId,
                                MaxCommentId = y.MaxCommentId
                            }).ToList()
                    );
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return commRel;
        }

        public static List<CommentRelationInfo> GetNestedCommentRelationInfo(string userId, string pageId)
        {
            var commRel = new List<CommentRelationInfo>();
            DateTime previousNonKEDay = DateTime.Today.AddDays(-1);
            DateTime previousKEDay = DateTime.Today.AddDays(-30);
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    commRel = (from cf in db.CommentsFBs
                                join pf in db.PostsFBs on cf.PostId equals pf.PostId
                                where pf.UserId == userId && pf.PageId == pageId 
                                        && !(pf.AuthorUserId == userId || pf.AuthorUserId == pageId)
                                        && pf.CreatedDate > previousNonKEDay
                                        && cf.CreatedDate > previousNonKEDay
                                select new CommentRelationInfo
                                {
                                    PostId = cf.CommentId,
                                    MaxCommentId = cf.MaxCommentId
                                }).ToList();

                    commRel.AddRange(
                        (from cf in db.CommentsFBs
                         join pf in db.PostsFBs on cf.PostId equals pf.PostId
                         where pf.UserId == userId && pf.PageId == pageId
                                 && (pf.AuthorUserId == userId || pf.AuthorUserId == pageId)
                                 && pf.CreatedDate > previousKEDay
                                 && cf.CreatedDate > previousNonKEDay
                         select new CommentRelationInfo
                         {
                             PostId = cf.CommentId,
                             MaxCommentId = cf.MaxCommentId
                         }).ToList()
                    );
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return commRel;
        }

        public static LastPostChatInfo GetLastPostChatInfo(string userId, string pageId)
        {
            var postChatInfo = new LastPostChatInfo();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    postChatInfo = db.PostUserStatusFBs.Where(x => x.UserId == userId && x.PageId == pageId)
                                            .Select(y => new LastPostChatInfo
                                            {
                                                MaxPostCreationDate = y.MaxPostCreationDate,
                                                MaxConUpdatedTime = y.MaxConUpdatedTime
                                            }).FirstOrDefault();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return postChatInfo;
        }

        public static List<ConvMaxDates> GetConvMaxDates()
        {
            List<ConvMaxDates> conMsgMaxDates = new List<ConvMaxDates>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    conMsgMaxDates = db.PageChatsFBs.GroupBy(i => i.ConId)
                                        .Select(g => new ConvMaxDates
                                        {
                                            ConId = g.Key,
                                            MaxCreatedDate = g.Max(row => row.CreatedDate)
                                        }).ToList();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return conMsgMaxDates;
        }

        public static List<FBPost> GetPostsWithoutGrades(string pageId)
        {
            var fbPosts = new List<FBPost>();
            DateTime previousSecondthDay = DateTime.Now.AddDays(-2);
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    fbPosts = (from pf in db.PostsFBs
                                where pf.PageId == pageId
                                    && pf.CreatedDate < previousSecondthDay
                                    && pf.PostGrade == null
                                    && pf.AuthorUserId == pageId
                                join ma in db.FeedsMediaAttachments.Where(x => x.MediaType == "FP")
                                on pf.Id equals ma.MediaId into gj
                                from ma in gj.DefaultIfEmpty()
                                select new FBPost
                                {
                                    PostId = pf.PostId,
                                    CreatedDate = pf.CreatedDate,
                                    AuthorUserId = pf.AuthorUserId,
                                    UserId = pf.UserId,
                                    PageId = pf.PageId,
                                    PostImageURL = ma.AttachmentUrl,
                                    PostType = (ma == null ? "Text" : (ma.Description.Contains("video") ? "Video" : (ma.Description.Contains("photo") ? "Photo" : "Text"))),
                                    AuthorProfilePic = pf.AuthorProfilePic,
                                    AuthorUserName = pf.AuthorUserName,
                                    Message = pf.Message
                                }).ToList();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return fbPosts;
        }

        public static List<string> GetEmailIdsForCompany(int companyId)
        {
            var emailIds = new List<string>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    emailIds = db.SocialMediaUsersUpdateReceivers.Where(x => x.CompanyId == companyId && x.ReceiveReport == true).Select(y => y.Email).ToList();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return emailIds;
        }

        public static List<FBPost> GetPreviousSixMonthsPostGradesFromPost(FBPost post)
        {
            var fbPosts = new List<FBPost>();
            DateTime previousSixthMonth = post.CreatedDate.Value.AddMonths(-6);
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    fbPosts = (from pf in db.PostsFBs
                                where pf.PageId == post.AuthorUserId
                                    && pf.CreatedDate < post.CreatedDate
                                    && pf.CreatedDate > previousSixthMonth
                                    && pf.PostGrade != null
                                    && pf.AuthorUserId == post.AuthorUserId
                                join ma in db.FeedsMediaAttachments.Where(x => x.MediaType == "FP")
                                on pf.Id equals ma.MediaId into gj
                                from ma in gj.DefaultIfEmpty()
                                select new FBPost
                                {
                                    PostId = pf.PostId,
                                    CreatedDate = pf.CreatedDate,
                                    AuthorUserId = pf.PageId,
                                    EngagementRate = pf.EngagementRate.HasValue ? (int)pf.EngagementRate : 0,
                                    PostType = (ma == null ? "Text" : (ma.Description.Contains("video") ? "Video" : (ma.Description.Contains("photo") ? "Photo" : "Text")))
                                }).ToList();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
            return fbPosts;
        }

        public static void updateCommentStatus(string lastFetchedCommentId, string userId, string pageId, string postId)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var existingContacts = db.PostsFBs
                                            .Where(x => x.UserId == userId && x.PageId == pageId && x.PostId == postId)
                                            .ToList();
                    if (existingContacts != null)
                    {
                        foreach(PostsFB contact in existingContacts)
                        {
                            contact.MaxCommentId = lastFetchedCommentId;

                            db.PostsFBs.Attach(contact);
                            db.Entry(contact).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception exp) { }
                db.Dispose();
            }
        }

        public static void updateNestedCommentStatus(string lastFetchedCommentId, string commentId)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    //In case of nested comments of Comments that is coming from two connected profiles on Facebook
                    var existingContacts = db.CommentsFBs
                                            .Where(x => x.CommentId == commentId)
                                            .ToList();
                    if (existingContacts != null)
                    {
                        foreach(CommentsFB contact in existingContacts)
                        {
                            contact.MaxCommentId = lastFetchedCommentId;

                            db.CommentsFBs.Attach(contact);
                            db.Entry(contact).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception exp) { }
                db.Dispose();
            }
        }

        public static void updatePostStatus(DateTime? lastFetchedCreationTime, string userId, string pageId)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var existingContact = db.PostUserStatusFBs
                                            .Where(x => x.UserId == userId && x.PageId == pageId)
                                            .SingleOrDefault();
                    if (existingContact != null)
                    {
                        existingContact.MaxPostCreationDate = lastFetchedCreationTime;

                        db.PostUserStatusFBs.Attach(existingContact);
                        db.Entry(existingContact).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception exp) { }
                db.Dispose();
            }
        }

        public static void updatePostGrading(FBPost post)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var existingContacts = db.PostsFBs
                                            .Where(x => x.PostId == post.PostId && x.PostGrade == null)
                                            .ToList();
                    if (existingContacts != null)
                    {
                        foreach (PostsFB contact in existingContacts)
                        {
                            contact.EngagementRate = post.EngagementRate;
                            contact.PostGrade = post.Grade;

                            db.PostsFBs.Attach(contact);
                            db.Entry(contact).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception exp) { }
                db.Dispose();
            }
        }

        public static void updateChatStatus(DateTime? lastFetchedUpdationTime, string userId, string pageId)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var existingContact = db.PostUserStatusFBs
                                            .Where(x => x.UserId == userId && x.PageId == pageId)
                                            .SingleOrDefault();
                    if (existingContact != null)
                    {
                        existingContact.MaxConUpdatedTime = lastFetchedUpdationTime;

                        db.PostUserStatusFBs.Attach(existingContact);
                        db.Entry(existingContact).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception exp) { }
                db.Dispose();
            }
        }

        public static void InsertFeeds(List<FBPost> currentPosts, string userId, string pageId, int? ProfileId, int? ProfilePageId)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var commIds = currentPosts.Select(i => i.PostId).ToList();
                    var commIdsInDb = db.PostsFBs.Where(i => commIds.Contains(i.PostId)).Select(i => i.PostId).ToList();
                    var commIdsNotInDb = new HashSet<string>(commIds.Except(commIdsInDb));
                    var itemsNotInDb = currentPosts.Where(i => commIdsNotInDb.Contains(i.PostId)).ToList();

                    foreach (FBPost obj in itemsNotInDb)
                    {
                        var result = db.PostsFBs.Add(new PostsFB
                        {
                            ProfileId = ProfileId,
                            ProfilePageId = ProfilePageId,
                            UserId = userId,
                            PageId = pageId,
                            Message = obj.Message,
                            CreatedDate = obj.CreatedDate,
                            PostId = obj.PostId,
                            MaxCommentId = obj.MaxCommentId,
                            AuthorUserId = obj.AuthorUserId,
                            AuthorUserName = obj.AuthorUserName,
                            AuthorProfilePic = obj.AuthorProfilePic,
                            IsAuthorVerified = obj.IsAuthorVerified,
                            InsertionDate = DateTime.Now
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

        public static void InsertComments(List<FBComment> currentComments, string postId)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var commIds = currentComments.Select(i => i.CommentId).ToList();
                    var commIdsInDb = db.CommentsFBs.Where(i => commIds.Contains(i.CommentId)).Select(i => i.CommentId).ToList();
                    var commIdsNotInDb = new HashSet<string>(commIds.Except(commIdsInDb));
                    var itemsNotInDb = currentComments.Where(i => commIdsNotInDb.Contains(i.CommentId)).ToList();

                    foreach (FBComment obj in itemsNotInDb)
                    {
                        db.CommentsFBs.Add(new CommentsFB
                        {
                            PostId = postId,
                            CommentId = obj.CommentId,
                            Message = obj.Message,
                            CreatedDate = obj.CreatedDate,
                            FromId = obj.FromId,
                            FromName = obj.FromName,
                            FromUserProfilePic = obj.FromUserProfilePic,
                            IsFromVerified = obj.IsFromVerified,
                            MaxCommentId = obj.MaxCommentId,
                            InsertionDate = DateTime.Now,
                            Attachment = obj.Attachment,
                            AttachmentType = obj.AttachmentType,
                            ParentId = obj.ParentId
                        });
                    }
                    db.SaveChanges();
                }
                catch (Exception exp) { }
                db.Dispose();
            }
        }

        public static void InsertPageChats(List<FBPageChat> currentPosts, string userId, string pageId, string profilePic, int? ProfileId, int? ProfilePageId)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var commIds = currentPosts.Select(i => i.MsgId).ToList();
                    var commIdsInDb = db.PageChatsFBs.Where(i => commIds.Contains(i.MsgId)).Select(i => i.MsgId).ToList();
                    var commIdsNotInDb = new HashSet<string>(commIds.Except(commIdsInDb));
                    var itemsNotInDb = currentPosts.Where(i => commIdsNotInDb.Contains(i.MsgId)).ToList();
                    foreach (FBPageChat obj in itemsNotInDb)
                    {
                        var result = db.PageChatsFBs.Add(new PageChatsFB
                        {
                            ProfileId = ProfileId,
                            ProfilePageId = ProfilePageId,
                            UserId = userId,
                            PageId = pageId,
                            FromProfilePic = profilePic,
                            ConId = obj.ConId,
                            MsgId = obj.MsgId,
                            FromId = obj.FromId,
                            FromName = obj.FromName,
                            ToId = obj.ToId,
                            ToName = obj.ToName,
                            MsgText = obj.MsgText,
                            CreatedDate = obj.CreatedDate,
                            IsFromVerified = obj.IsFromVerified,
                            InsertionDate = DateTime.Now
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
}
