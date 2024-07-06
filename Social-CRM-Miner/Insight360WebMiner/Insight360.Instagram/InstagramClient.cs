using Insight360.DAL;
using Insight360.Instagram.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static Insight360.Common.UserFeedResponse;

namespace Insight360.Instagram
{
    public class InstagramClient
    {
        public void FeatchReport(Insight360.Common.InstagramUserAccount account)
        {
            //var min_timestamp = ConvertToTimestamp(account.LastMinnedDateTime);
            //var max_timestamp = ConvertToTimestamp(DateTime.Now);
            var userSelfFeedUrl = string.Format("https://api.instagram.com/v1/users/self/media/recent/?access_token={0}", account.AccessToken);
            //var userSelfFeedUrl = string.Format("https://api.instagram.com/v1/users/self/media/recent/?access_token={0}", account.AccessToken);
            var userSelfFeedResponse = GetRequest(userSelfFeedUrl);
            var userSelfProfileUrl = string.Format("https://api.instagram.com/v1/users/self/?access_token={0}", account.AccessToken);

            var userSelfProfilResponse = GetRequest(userSelfProfileUrl);
            var selfFeed = new JavaScriptSerializer().Deserialize<UserFeedResponse.RootObject>(userSelfFeedResponse);
            var selfProfile = new JavaScriptSerializer().Deserialize<UserProfileResponse.RootObject>(userSelfProfilResponse);
            if (selfFeed!=null&& selfFeed.data!=null&&selfFeed.data.Count > 0)
            {
                var userProfileImage = selfFeed.data.First().user.profile_picture;
                var totalFollowers = selfProfile.data.counts.followed_by;
                var totalPosts = selfProfile.data.counts.media;
                var totalFollows = selfProfile.data.counts.follows;
                new InstagramDBOperations().DumpMinnedResult(selfFeed.data.Select(s => new InstagramUserMinnedFeed
                {
                    InstagramAccountId = account.Id,
                    LowResolutionImageUrl = s.type.ToLower() == "image" ? s.images.low_resolution.url : s.videos.low_resolution.url,
                    StandardResolutionImageUrl = s.type.ToLower() == "image" ? s.images.standard_resolution.url : s.videos.standard_resolution.url,
                    ThumbnailImageUrl = s.type.ToLower() == "image" ? s.images.thumbnail.url : s.videos.low_bandwidth.url,
                    PostId = s.id,
                    PostTags = s.tags != null ? String.Join(",", s.tags) : "",
                    PostType = s.type,
                    UserHasLiked = s.user_has_liked,
                    TotalComments = s.comments.count,
                    TotalLikes = s.likes.count,
                    PostLink = s.link,
                    PostCreatedDate = s.created_time,
                    PostTimeTicks = long.Parse(s.created_time),
                    CaptionText = s.caption == null ? "" : s.caption.text
                }).ToList(), account.Id, userProfileImage, totalFollowers, totalPosts, totalFollows);
            }
        }

        public void FetchComments(Insight360.Common.InstagramUserAccount account)
        {
            try
            {
                var posts = new InstagramDBOperations().GetInstagramPostsById(account.Id);
                List<InstagramComment> commentsList = new List<InstagramComment>();
                foreach (var post in posts)
                {
                    var userMediaCommentUrl = string.Format("https://api.instagram.com/v1/media/{0}/comments?access_token={1}", post.PostId, account.AccessToken);
                    var userMediaCommentResponse = GetRequest(userMediaCommentUrl);

                    var commentFeed = new JavaScriptSerializer().Deserialize<UserFeedResponse.CommentsRootObject>(userMediaCommentResponse);
                    if (commentFeed != null && commentFeed.data != null && commentFeed.data.Count > 0)
                    {
                        foreach (var item in commentFeed.data)
                        {
                            InstagramComment commentObj = new InstagramComment();
                            commentObj.PostId = post.PostId;
                            commentObj.CommentId = item.id;
                            commentObj.Message = item.text;
                            commentObj.CreatedDate = InstagramDBOperations.ConvertToDateTime(item.created_time);
                            commentObj.FromUserId = item.from.id;
                            commentObj.FromUserName = item.from.username;
                            commentObj.FromFullName = item.from.full_name;
                            commentObj.FromProfilePicture = item.from.profile_picture;
                            commentObj.InsertionDate = DateTime.Now;

                            commentsList.Add(commentObj);
                        }
                    } 
                }
                if (commentsList.Count > 0)
                {
                    new InstagramDBOperations().DumpMinnedCommentResult(commentsList);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public void FetchProfileInformation(List<Insight360.Common.InstagramUserAccount> profiles)
        {
            try
            {
                List<ProfilesHistory> profileList = new List<ProfilesHistory>();
                foreach (var account in profiles)
                {
                    var userUrl = string.Format("https://api.instagram.com/v1/users/self/?access_token={0}", account.AccessToken);
                    var userResponse = GetRequest(userUrl);
                    var selfProfile = new JavaScriptSerializer().Deserialize<UserProfileResponse.RootObject>(userResponse);
                    ProfilesHistory profile = new ProfilesHistory();
                    profile.ProfileId = account.Id;
                    profile.ProfileType = 6;
                    profile.CreatedDate = DateTime.Now;
                    profile.NumOfFollowers = selfProfile.data.counts.followed_by;
                    profile.NumOfFollows = selfProfile.data.counts.follows;
                    profile.PostStatusCount = selfProfile.data.counts.media;

                    profileList.Add(profile);
                }

                if (profileList.Count > 0)
                {
                    new InstagramDBOperations().DumpMinnedProfileResult(profileList);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private string GetRequest(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                string json = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response != null)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            if (dataStream != null)
                            {
                                using (StreamReader reader = new StreamReader(dataStream))
                                {
                                    json = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                return json;
            }
            catch (Exception exp)
            {
                if (!exp.Message.Contains("400"))
                    throw;
                return "";
            }
        }

        private string ConvertToTimestamp(DateTime value)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            TimeSpan span = (value - epoch);
            return ((long)Convert.ToDouble(span.TotalSeconds)).ToString();
        }
    }
}