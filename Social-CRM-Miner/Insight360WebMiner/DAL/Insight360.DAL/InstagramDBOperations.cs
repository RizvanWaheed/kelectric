using Insight360.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Insight360.Common.UserFeedResponse;

namespace Insight360.DAL
{
    public class InstagramDBOperations
    {
        public List<InstagramUserAccount> GetInstagramAccounts()
        {
            var account = new List<InstagramUserAccount>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var defaultDate = DateTime.Now.AddDays(-90);
                    account = db.Profiles.Where(x => x.ProfileTypeId == 6).Select(s => new InstagramUserAccount
                    {
                        Id = s.Id,
                        AccessToken = s.AcessToken,
                        InstagramUserId = s.ProfileExternalId,
                        InstaUserName = s.UserName
                    }).ToList();
                }
                catch (Exception exp)
                {
                    account = null;
                }
                db.Dispose();
            }
            return account;
        }
        public bool DumpMinnedResult(List<InstagramUserMinnedFeed> data, int instagramUserAccountId, string userProfileImageUrl, int totalFollowers, int totalPosts, int totalFollows)
        {
            var success = false;
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    if (data.Count > 0)
                    {

                        var userAccount = db.Profiles.Where(a => a.Id == instagramUserAccountId).FirstOrDefault();
                        var lastMinnedDate = db.PostUserStatusInstagrams.Where(x => x.ProfileId == instagramUserAccountId && x.InstaUserId == userAccount.ProfileExternalId).FirstOrDefault();
                        var postIds = data.Select(x => x.PostId).ToList();
                        var minnedData = db.InstagramMinnedDatas.Where(x => postIds.Contains(x.PostId)).ToList();
                        var lastpost = data.OrderByDescending(o => o.PostTimeTicks).FirstOrDefault();
                        //Update Exisiting Record.
                        minnedData.ForEach(m =>
                        {
                            var apiData = data.First(x => x.PostId == m.PostId);
                            m.HasUserLiked = apiData.UserHasLiked;
                            m.InstagramAccountId = apiData.InstagramAccountId;
                            m.LowResolutionImageUrl = apiData.LowResolutionImageUrl;
                            m.PostId = apiData.PostId;
                            m.PostLink = apiData.PostLink;
                            m.PostTags = apiData.PostTags;
                            m.PostType = apiData.PostType;
                            m.StandardResolutionImageUrl = apiData.StandardResolutionImageUrl;
                            m.ThumbnailImageUrl = apiData.ThumbnailImageUrl;
                            m.TotalComments = apiData.TotalComments;
                            m.TotalLikes = apiData.TotalLikes;
                            m.PostCreatedTime = apiData.PostCreatedDate;
                            m.CreatedDate = ConvertToDateTime(apiData.PostCreatedDate);
                            m.PostCaption = apiData.CaptionText;
                            data.Remove(apiData);
                        });
                        //Add New Posts
                        data.OrderBy(o => o.PostCreatedDate).ToList().ForEach(d =>
                        {
                            db.InstagramMinnedDatas
                                .Add(
                                new InstagramMinnedData
                                {
                                    HasUserLiked = d.UserHasLiked,
                                    InstagramAccountId = d.InstagramAccountId,
                                    LowResolutionImageUrl = d.LowResolutionImageUrl,
                                    PostId = d.PostId,
                                    PostLink = d.PostLink,
                                    PostTags = d.PostTags,
                                    PostType = d.PostType,
                                    StandardResolutionImageUrl = d.StandardResolutionImageUrl,
                                    ThumbnailImageUrl = d.ThumbnailImageUrl,
                                    TotalComments = d.TotalComments,
                                    TotalLikes = d.TotalLikes,
                                    PostCreatedTime = d.PostCreatedDate,
                                    CreatedDate = ConvertToDateTime(d.PostCreatedDate),
                                    PostCaption = d.CaptionText
                                }
                                );
                        });
                        /* data.Where(p => (lastMinnedDate==null||string.IsNullOrEmpty(lastMinnedDate.LastPostCreatedDate) || long.Parse(p.PostCreatedDate) > long.Parse(lastMinnedDate.LastPostCreatedDate)))
                                 .OrderBy(o => o.PostCreatedDate).ToList().ForEach(d =>
                           {
                               db.InstagramMinnedDatas
                                   .Add(
                                   new InstagramMinnedData
                             {
                                 HasUserLiked = d.UserHasLiked,
                                 InstagramAccountId = d.InstagramAccountId,
                                 LowResolutionImageUrl = d.LowResolutionImageUrl,
                                 PostId = d.PostId,
                                 PostLink = d.PostLink,
                                 PostTags = d.PostTags,
                                 PostType = d.PostType,
                                 StandardResolutionImageUrl = d.StandardResolutionImageUrl,
                                 ThumbnailImageUrl = d.ThumbnailImageUrl,
                                 TotalComments = d.TotalComments,
                                 TotalLikes = d.TotalLikes,
                                 PostCreatedTime = d.PostCreatedDate
                             }
                                   );
                           }
                             );*/
                        //var when = new DateTime(1970, 1, 1).AddSeconds(lastpost.PostTimeTicks);
                        //userAccount.LastMinnedDateTime = when;
                        //userAccount.LastPostCreatedDate = lastpost.PostTimeTicks.ToString();
                        //userAccount.InstaProfilePic = userProfileImageUrl;
                        if (lastMinnedDate != null)
                        {
                            if ((lastMinnedDate.TotalFollowers != totalFollowers || lastMinnedDate.TotalFollows != totalFollows) && lastMinnedDate.MaxPostCreationDate.HasValue && lastMinnedDate.MaxPostCreationDate.Value.Date != DateTime.Now.Date)
                            {

                                var comparisionDate = lastMinnedDate.MaxPostCreationDate.HasValue ? lastMinnedDate.MaxPostCreationDate.Value.Date : DateTime.Now.Date;
                                var instagramuserFolowtodayHistory = db.InstagramUserFollowHistories.Where(p => p.ProfileId == instagramUserAccountId &&
                               System.Data.Entity.DbFunctions.TruncateTime(p.EntryDate) == comparisionDate.Date).FirstOrDefault();
                                if (instagramuserFolowtodayHistory == null)
                                {
                                    db.InstagramUserFollowHistories.Add(new InstagramUserFollowHistory
                                    {
                                        TotalFollower = lastMinnedDate.TotalFollowers.HasValue ? lastMinnedDate.TotalFollowers.Value : 0,
                                        TotalFollowing = lastMinnedDate.TotalFollows.HasValue ? lastMinnedDate.TotalFollows.Value : 0,
                                        ProfileId = lastMinnedDate.ProfileId.Value,
                                        EntryDate = lastMinnedDate.MaxPostCreationDate.HasValue ? lastMinnedDate.MaxPostCreationDate.Value : DateTime.Now
                                    });
                                }
                                else
                                {
                                    instagramuserFolowtodayHistory.TotalFollower = lastMinnedDate.TotalFollowers.HasValue ? lastMinnedDate.TotalFollowers.Value : 0;
                                    instagramuserFolowtodayHistory.TotalFollowing = lastMinnedDate.TotalFollows.HasValue ? lastMinnedDate.TotalFollows.Value : 0;
                                    instagramuserFolowtodayHistory.ProfileId = lastMinnedDate.ProfileId.Value;
                                    instagramuserFolowtodayHistory.EntryDate = lastMinnedDate.MaxPostCreationDate.HasValue ? lastMinnedDate.MaxPostCreationDate.Value : DateTime.Now;
                                }
                            }
                            lastMinnedDate.TotalFollowers = totalFollowers;
                            lastMinnedDate.TotalFollows = totalFollows;
                            lastMinnedDate.LastPostCreatedDate = lastpost.PostTimeTicks.ToString();
                            lastMinnedDate.MaxPostCreationDate = DateTime.Now;
                            lastMinnedDate.TotalPosts = totalPosts;
                        }
                        else
                        {
                            db.PostUserStatusInstagrams.Add(new PostUserStatusInstagram
                            {
                                InstaUserId = userAccount.ProfileExternalId,
                                ProfileId = userAccount.Id,
                                LastPostCreatedDate = lastpost.PostTimeTicks.ToString(),
                                MaxPostCreationDate = DateTime.Now,
                                TotalFollowers = totalFollowers,
                                TotalFollows = totalFollows,
                                TotalPosts = totalPosts
                            });
                        }
                        userAccount.ProfilePic = userProfileImageUrl;
                        db.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    success = false;
                }
                db.Dispose();
            }
            return success;
        }

        public static DateTime? ConvertToDateTime(string postCreatedDate)
        {
            var longdate = long.Parse(postCreatedDate);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddSeconds(longdate).ToLocalTime();
            return date;
        }

        public static DateTime? ConvertToDateTime(long postCreatedDate)
        {
            // var longdate = long.Parse(postCreatedDate);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddSeconds(postCreatedDate).ToLocalTime();
            return date;
        }

        public List<InstagramMinnedData> GetInstagramPostsById (int instagramAccountId)
        {
            List<InstagramMinnedData> posts = new List<InstagramMinnedData>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    posts = db.InstagramMinnedDatas.Where(x => x.InstagramAccountId == instagramAccountId).ToList();
                }
                catch (Exception exp)
                {
                    posts = null;
                }
                return posts;
            }
        }

        public bool DumpMinnedCommentResult(List<InstagramComment> data)
        {
            var success = false;
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    var existingComments = db.InstagramComments.ToList();
                    foreach (var item in existingComments)
                    {
                        data.Remove(data.Where(x => x.PostId == item.PostId && x.CommentId == item.CommentId).FirstOrDefault());
                        if (data.Count == 0)
                            break;
                    }
                    if (data.Count > 0)
                    {
                        db.InstagramComments.AddRange(data);
                        db.SaveChanges(); 
                    }
                    success = true;
                }
                catch (Exception exp)
                {
                    success = false;
                }
                db.Dispose();
            }
            return success;
        }

        public bool DumpMinnedProfileResult(List<ProfilesHistory> profiles)
        {
            var success = false;
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {                    
                    if (profiles.Count > 0)
                    {
                        db.ProfilesHistories.AddRange(profiles);
                        db.SaveChanges(); 
                    }
                    success = true;
                }
                catch (Exception exp)
                {
                    success = false;
                }
                db.Dispose();
            }
            return success;
        }

    }
}
