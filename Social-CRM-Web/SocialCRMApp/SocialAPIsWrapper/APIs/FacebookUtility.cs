using CRM.Entities;
using Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SocialAPIsWrapper.APIs
{
    public class SuccessMessage
    {
        public string success { get; set; }
    }
    public class FacebookUtility
    {
        private FacebookClient app;
        public FacebookUtility(string authorizedUserId, bool isPage)
        {
            using (var context = new SocialCRMEntities())
            {
                app = new FacebookClient();
                if (isPage)
                {
                    var user = context.ProfilePages.Where(z => z.ProfilePageID == authorizedUserId).FirstOrDefault();
                    app.AccessToken = user.PageAccessToken;
                }
                else
                {
                    var user = context.Profiles.Where(z => z.ProfileExternalId == authorizedUserId && z.ProfileTypeId == 1).FirstOrDefault();
                    app.AccessToken = user.AcessToken;
                }
            }
        }

        public async Task<JsonObject> likePost(string postId, bool isPost)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.PostTaskAsync("/" + postId + "/likes", new { }));
                //October 11, 2017. Functionality is changed on client's request
                if (result != null)
                {
                    using (var context = new SocialCRMEntities())
                    {
                        if (isPost)
                        {
                            PostsFB fb = context.PostsFBs.Where(x => x.PostId == postId).FirstOrDefault();
                            fb.isLikedByAdmin = true;
                            context.PostsFBs.Attach(fb);
                            context.Entry(fb).State = EntityState.Modified;
                        }
                        else
                        {
                            CommentsFB fb = context.CommentsFBs.Where(x => x.CommentId == postId).FirstOrDefault();
                            fb.isLikedByAdmin = true;
                            context.CommentsFBs.Attach(fb);
                            context.Entry(fb).State = EntityState.Modified;
                        }
                        context.SaveChanges();
                    }
                }

                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> unlikePost(string objectId, bool isPost)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.DeleteTaskAsync("/" + objectId + "/likes"));

                //October 11, 2017. Functionality is changed on client's request
                if (result != null)
                {
                    using (var context = new SocialCRMEntities())
                    {
                        if (isPost)
                        {
                            PostsFB fb = context.PostsFBs.Where(x => x.PostId == objectId).FirstOrDefault();
                            fb.isLikedByAdmin = false;
                            context.PostsFBs.Attach(fb);
                            context.Entry(fb).State = EntityState.Modified;
                        }
                        else
                        {
                            CommentsFB fb = context.CommentsFBs.Where(x => x.CommentId == objectId).FirstOrDefault();
                            fb.isLikedByAdmin = false;
                            context.CommentsFBs.Attach(fb);
                            context.Entry(fb).State = EntityState.Modified;
                        }
                        context.SaveChanges();
                    }
                }

                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> blockUsers(string pageId, List<String> appScopedIds, List<String> userIds)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                if(appScopedIds != null)
                {
                    parameters["asid"] = appScopedIds;
                }
                else
                {
                    parameters["user"] = userIds;
                }

                var result = (IDictionary<string, object>)(await app.PostTaskAsync("/" + pageId + "/blocked", parameters));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> unblockUser(string pageId, string appScopedId, string userId)
        {
            try
            {
                string toDelete;
                if (!String.IsNullOrEmpty(appScopedId))
                {
                    toDelete = "/" + pageId + "/blocked?asid=" + appScopedId;
                }
                else
                {
                    toDelete = "/" + pageId + "/blocked?user=" + userId;
                }

                var result = (IDictionary<string, object>)(await app.DeleteTaskAsync(toDelete));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> isPostLiked(string objectId, bool isPost)
        {
            try
            {
                bool result = false;
                //commented on October 11, 2017. Functionality is changed on client's request
                //var likes = (IDictionary<string, object>)(await app.GetTaskAsync("/" + objectId + "?fields=likes.summary(true)"));
                //result = Convert.ToBoolean((((likes as JsonObject)["likes"] as JsonObject)["summary"] as JsonObject)["has_liked"]);

                using (var context = new SocialCRMEntities())
                {
                    if(isPost)
                        result = Convert.ToBoolean(context.PostsFBs.Where(x => x.PostId == objectId).FirstOrDefault().isLikedByAdmin);
                    else
                        result = Convert.ToBoolean(context.CommentsFBs.Where(x => x.CommentId == objectId).FirstOrDefault().isLikedByAdmin);
                }

                return result;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> isPostHidden(string objectId)
        {
            try
            {
                bool result = false;
                var response = (IDictionary<string, object>)(await app.GetTaskAsync("/" + objectId + "?fields=is_hidden"));
                result = Convert.ToBoolean((response as JsonObject)["is_hidden"]);
                return result;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> newCommentPost(string postId, string comment, string filePath, string contentType, string fileName)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters["message"] = comment;
                if(!String.IsNullOrEmpty(filePath))
                {
                    if (filePath.Contains(","))
                    {
                        filePath = filePath.Split(',')[1];
                    }

                    byte[] byteArr = Convert.FromBase64String(filePath);
                    var p = new FacebookMediaObject
                    {
                        FileName = Path.GetFileNameWithoutExtension(fileName),
                        ContentType = "multipart/form-data"
                    };
                    p.SetValue(byteArr);

                    //var p = new FacebookMediaObject
                    //{
                    //    FileName = Path.GetFileNameWithoutExtension(filePath),
                    //    ContentType = "multipart/form-data"
                    //};
                    //p.SetValue(File.ReadAllBytes(filePath));
                    parameters["source"] = p;
                }
                var result = (IDictionary<string, object>)(await app.PostTaskAsync("/" + postId + "/comments", parameters));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> newPost(string userId, string status, string pictureFBId, string placeId=null)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters["message"] = status;
                if(pictureFBId != null)
                    parameters["object_attachment"] = pictureFBId;
                if(!String.IsNullOrEmpty(placeId))
                    parameters["place"] = placeId;
                var result = (IDictionary<string, object>)(await app.PostTaskAsync("/" + userId + "/feed", parameters));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> newPhotoUpload(string userId, string filePath,string contentType, string fileName)
        {
            try
            {
                //string converted = filePath.Replace('-', '+');
                //converted = converted.Replace('_', '/');
                if (filePath.Contains(",")){
                    filePath = filePath.Split(',')[1];
                }

                byte[] byteArr = Convert.FromBase64String(filePath);

                var p = new FacebookMediaObject
                {
                    //FileName = Path.GetFileNameWithoutExtension(filePath),
                    //ContentType = "multipart/form-data"
                    FileName = Path.GetFileNameWithoutExtension(fileName),
                    ContentType = "multipart/form-data"
                };
                //p.SetValue(File.ReadAllBytes(filePath));
                p.SetValue(byteArr);

                var parameters = new Dictionary<string, object>();
                parameters["no_story"] = true;
                parameters["source"] = p;
                var result = (IDictionary<string, object>)(await app.PostTaskAsync("/" + userId + "/photos", parameters));
                return (result as JsonObject)["id"].ToString();
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> newPageChat(string conId, string status)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters["message"] = status;
                var result = (IDictionary<string, object>)(await app.PostTaskAsync("/" + conId + "/messages", parameters));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<JsonObject> getPageChatMessages(string conId, int count)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("order", "reverse_chronological");
            parameters.Add("limit", count);
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + conId + "/messages?fields=created_time,from,id,message,to", parameters));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<JsonObject> deleteObject(string objectId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.DeleteTaskAsync("/" + objectId));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;        
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> deleteObjectTemp(string objectId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.DeleteTaskAsync("/" + objectId));
                var response = result as JsonObject;
                if (response.ContainsKey("success"))
                {
                    return Convert.ToString(response["success"]);
                }
                else
                {
                    return "False";
                }
            }
            catch (FacebookApiException ex)
            {
                if (ex.Message.Contains("(OAuthException - #2) An unexpected error has occurred. Please retry your request later."))
                {
                    return "True";
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> updateObject(string objectId, Dictionary<string, object> dict)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.PostTaskAsync("/" + objectId, dict));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonObject> getPostInfo(string postId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + postId + "?fields=from{picture.width(100).height(100),name},message,story,created_time,id,picture,type", new { }));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<JsonArray> getCommentsForObject(string objectId, int count)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("order", "reverse_chronological");
            parameters.Add("limit", count);
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + objectId + "/comments?fields=from{picture.width(100).height(100),name},message,created_time,id,likes.summary(true)", parameters));
                JsonArray data = result["data"] as JsonArray;
                return data;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<JsonArray> getPageReportStats(string objectId, DateTime from, DateTime to, string apiEndpoint, string period)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("since", from.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.Add("until", to.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.Add("period", period);
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + objectId + "/insights/" + apiEndpoint, parameters));
                JsonArray data = result["data"] as JsonArray;
                if (data.Count == 0)
                    return data;
                else
                    return (data[0] as JsonObject)["values"] as JsonArray;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<JsonObject> gePageLikes(string pageId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + pageId + "?fields=likes"));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<JsonObject> geUserFriendsCount(string userId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + userId + "/friends"));
                
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<JsonObject> getPlaces(string searchQuery)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/search?q="+searchQuery+"&type=place"));

                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IDictionary<string, object>> getRecentPosts(string pageId, string limit, DateTime from, DateTime to)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("since", from.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.Add("until", to.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.Add("limit", limit);
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + pageId + "/posts", parameters));
                //JsonArray data = result["data"] as JsonArray;
                IDictionary<string,object> data = result;
                return data;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<JsonObject> getReactionsCommentsShares(string postId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + postId + "?fields=likes.summary(true),comments.summary(true),shares"));
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<JsonObject> getPostReachabilityAndConsumption(string postId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + postId + "?fields=insights.metric(post_impressions_by_paid_non_paid_unique,post_consumptions,post_engaged_users)"));
                //post_impressions_unique
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IDictionary<string, object>> getMoreRecentPosts(string link)
        {
            try
            {
                //https://graph.facebook.com/v2.8/695105827192930/posts?limit=5&format=json&__paging_token=enc_AdDuo2X9eE9wUkGwzGZA3UKOU5K2faLIvefwg0msLEAwjfOe2RTTb7xugPufmJW2694wRSUSVTjeArzfofrZCJJoDUFMjp5P0ZBGjlkL542yPnWVwZDZD&since=1404374468&access_token=EAACEdEose0cBAOSwogReqcWydV31Yq9NHUqtFNOA2nBJTkPgjb1xdECXn8yU9J5Gx6Sev2MmhzQNN8roWpNqtPshrlRGCObb2lkqs9UFCZBSBQC9dI3Yh0bwYVZC5lVTzZChKZAx1FkS97XFsY13t6STo9L7UPbBujQPg1ZCXXdYVXx0kDFz5lcSi6bOGLhoZD&__previous=1
                string theUrl = link.Substring(31);
                var result = (IDictionary<string, object>)(await app.GetTaskAsync(theUrl));
                //JsonArray data = result["data"] as JsonArray;
                IDictionary<string, object> data = result;
                return data;
            }
            catch (FacebookApiException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<JsonObject> getFacebookPageInfoById(string pageId)
        {
            try
            {
                var result = (IDictionary<string, object>)(await app.GetTaskAsync("/" + pageId + "?fields=picture,name,is_verified"));
                //fan_count
                return result as JsonObject;
            }
            catch (FacebookApiException ex)
            {
                JsonObject jObj = new JsonObject();
                jObj.Add(new KeyValuePair<string, object>("APIerror", ex.Message));
                return jObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
