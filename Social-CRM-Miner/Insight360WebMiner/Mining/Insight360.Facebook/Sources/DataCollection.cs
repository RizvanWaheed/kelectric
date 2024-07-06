using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook;
using System.Collections.Specialized;
using System.Web;
using System.Threading;
using System.Runtime.Serialization;
using Insight360.Util;
using System.Threading.Tasks;
using Insight360.Common;
using Insight360.BO;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Insight360.Sources.Facebook
{
    public class DataCollection
    {
        static string GetValue(JsonObject obj, string id)
        {
            return (obj.Keys.Contains(id) ? obj[id].ToString() : string.Empty);
        }

        public static DateTime GetDateValue(JsonObject obj, string id)
        {
            DateTime dt;
            if (!DateTime.TryParse(GetValue(obj, id), out dt))
                dt = new DateTime(2000, 1, 1);
            dt = dt.ToUniversalTime();
            return dt;
        }

        internal static BO.FacebookPost CreatePost(JsonObject obj)
        {
            BO.FacebookPost post = new Insight360.BO.FacebookPost();

            post.Attribution = GetValue(obj, "attribution");
            post.Caption = GetValue(obj, "caption");
            post.CreatedTime = GetDateValue(obj, "created_time");
            post.Description = GetValue(obj, "description");

            post.From = new Insight360.BO.FacebookUserInfo()
            {
                Id = obj.Keys.Contains("from") ? GetValue(obj["from"] as JsonObject, "id") : string.Empty,
                Name = obj.Keys.Contains("from") ? GetValue(obj["from"] as JsonObject, "name") : string.Empty,
            };

            post.To = new Insight360.BO.FacebookUserInfo()
            {
                Id = obj.Keys.Contains("to") ? GetValue(((((obj["to"] as JsonObject))["data"] as JsonArray)[0] as JsonObject), "id") : string.Empty,
                Name = obj.Keys.Contains("to") ? GetValue(((((obj["to"] as JsonObject))["data"] as JsonArray)[0] as JsonObject), "name") : string.Empty,
            };

            post.Icon = GetValue(obj, "icon");
            post.Id = GetValue(obj, "id");
            post.Link = GetValue(obj, "link");
            post.Message = GetValue(obj, "message");
            post.Name = GetValue(obj, "name");
            post.Picture = GetValue(obj, "picture");

            post.Targeting = new Insight360.BO.FacebookPost.TargetingInfo()
            {
                City = obj.Keys.Contains("targeting") ? GetValue(obj["targeting"] as JsonObject, "city") : string.Empty,
                Country = obj.Keys.Contains("targeting") ? GetValue(obj["targeting"] as JsonObject, "country") : string.Empty,
                Locale = obj.Keys.Contains("targeting") ? GetValue(obj["targeting"] as JsonObject, "locale") : string.Empty,
                Region = obj.Keys.Contains("targeting") ? GetValue(obj["targeting"] as JsonObject, "region") : string.Empty,
            };

            post.Type = GetValue(obj, "type");
            post.UpdatedTime = GetDateValue(obj, "updated_time");

            post.Comments = new List<Insight360.BO.FacebookCommentInfo>();
            post.Likes = new List<Insight360.BO.FacebookLikeInfo>();

            if ((obj.Keys.Contains("comments"))
                && ((obj["comments"] as JsonObject).ContainsKey("summary"))
                && ((((obj["comments"] as JsonObject)["summary"]) as JsonObject).ContainsKey("total_count")))
            {
                post.CommentsCount = Convert.ToInt32((((obj["comments"] as JsonObject)["summary"]) as JsonObject)["total_count"]);
            }

            if ((obj.Keys.Contains("likes"))
                && ((obj["likes"] as JsonObject).ContainsKey("summary"))
                && ((((obj["likes"] as JsonObject)["summary"]) as JsonObject).ContainsKey("total_count")))
            {
                post.LikesCount = Convert.ToInt32((((obj["likes"] as JsonObject)["summary"]) as JsonObject)["total_count"]);
            }

            if ((obj.Keys.Contains("shares"))
                && ((obj["shares"] as JsonObject).ContainsKey("count")))
            {
                post.SharesCount = Convert.ToInt32(((obj["shares"] as JsonObject)["count"]));
            }

            if (obj.Keys.Contains("location"))
            {
                post.PageLocation.name = obj["location"].ToString();
            }

            if (obj.Keys.Contains("location") && (obj["location"] as JsonObject).Keys.Contains("city"))
            {
                post.PageLocation.name = ((obj["location"] as JsonObject)["city"]).ToString();
            }

            if (obj.Keys.Contains("location") && (obj["location"] as JsonObject).Keys.Contains("country"))
            {
                post.PageLocation.name += ", " + ((obj["location"] as JsonObject)["country"]).ToString();
            }

            if (obj.Keys.Contains("location") && (obj["location"] as JsonObject).Keys.Contains("longitude"))
            {
                post.PageLocation.longitude = Convert.ToDecimal(((obj["location"] as JsonObject)["longitude"]));
            }

            if (obj.Keys.Contains("location") && (obj["location"] as JsonObject).Keys.Contains("latitude"))
            {
                post.PageLocation.latitude = Convert.ToDecimal(((obj["location"] as JsonObject)["latitude"]));
            }

            return post;
        }

        public static List<string> GetAllChannels(DocumentSourceInfo documentSource, SearchTermInfo term)
        {
            string path = "search?fields=id,name,location";

            if (term.SearchTerm.StartsWith("#"))
            {
                term.SearchTerm = term.SearchTerm.Replace("#", "%23");
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string accessToken = "";

            switch (documentSource.ProfileType)
            {
                case ProfileTypes.FacebookPage:

                    accessToken = Insight360.Sources.Facebook.DataCollection.GetToken();

                    parameters.Clear();

                    parameters.Add("type", "page");
                    parameters.Add("q", term.SearchTerm);

                    return DAL.ConfigDAL.GetFacebookPagesBySearchTermId(term.Id);

                    break;

                case ProfileTypes.FacebookUser:

                    accessToken = "CAAYTM03EbZCgBAOiX667GS1Su4TFXOc9CuZBMKFc6tOaFHSALzZB1LqP3TqRWwyTSnq6RdSmYcZCoPP6XZCt0PhNh655ZBzCKnp1fIMFc2dE6kJrxYpxR1RZBsSXyFGOSctZAjvnWKoGAQ2ZBi8zFuUmnT2BEeW2VkZBhuGFisDZAa3HmOBIrZAWyJc0";

                    parameters.Clear();

                    parameters.Add("type", "user");
                    parameters.Add("q", term.SearchTerm);

                    return DAL.ConfigDAL.GetFacebookPagesBySearchTermId(term.Id);
                    //return GetAllPagedItems(path, accessToken, parameters, null, null);

                    break;

                case ProfileTypes.FacebookEvent:

                    accessToken = "CAAYTM03EbZCgBAOiX667GS1Su4TFXOc9CuZBMKFc6tOaFHSALzZB1LqP3TqRWwyTSnq6RdSmYcZCoPP6XZCt0PhNh655ZBzCKnp1fIMFc2dE6kJrxYpxR1RZBsSXyFGOSctZAjvnWKoGAQ2ZBi8zFuUmnT2BEeW2VkZBhuGFisDZAa3HmOBIrZAWyJc0";

                    parameters.Clear();

                    parameters.Add("type", "event");
                    parameters.Add("q", term.SearchTerm);

                    return DAL.ConfigDAL.GetFacebookPagesBySearchTermId(term.Id);
                    //return GetAllPagedItems(path, accessToken, parameters, null, null);

                    break;

                case ProfileTypes.FacebookGroup:

                    accessToken = "CAAYTM03EbZCgBAOiX667GS1Su4TFXOc9CuZBMKFc6tOaFHSALzZB1LqP3TqRWwyTSnq6RdSmYcZCoPP6XZCt0PhNh655ZBzCKnp1fIMFc2dE6kJrxYpxR1RZBsSXyFGOSctZAjvnWKoGAQ2ZBi8zFuUmnT2BEeW2VkZBhuGFisDZAa3HmOBIrZAWyJc0";

                    parameters.Clear();
                    parameters.Add("type", "group");
                    parameters.Add("q", term.SearchTerm);

                    return DAL.ConfigDAL.GetFacebookPagesBySearchTermId(term.Id);
                    //return GetAllPagedItems(path, accessToken, parameters, null, null);

                    break;

                case ProfileTypes.FacebookPlace:

                    accessToken = Insight360.Sources.Facebook.DataCollection.GetToken();

                    parameters.Clear();

                    parameters.Add("type", "place");
                    parameters.Add("q", term.SearchTerm);

                    //List<Location> locations = DAL.ConfigDAL.GetLocations(item.Id);

                    //if (locations != null && locations.Count > 0)
                    //{
                    //    parameters.Add("center", locations[0].longitude + "," + locations[0].latitude);
                    //    //parameters.Add("distance", "16000");
                    //}

                    return DAL.ConfigDAL.GetFacebookPagesBySearchTermId(term.Id);
                    //return GetAllPagedItems(path, accessToken, parameters, null, null);

                    break;

                case ProfileTypes.FacebookChannel:

                    string pageName = documentSource.Url.Replace("https://www.facebook.com/", "");
                    pageName = pageName.Replace("/", "");

                    JsonObject obj = new JsonObject();
                    obj.Add("id", pageName);
                    List<string> list = new List<string>();
                    list.Add(pageName);
                    return list;
                    //channels.Add(obj);

                    break;
            }

            return null;
        }

        public static Dictionary<string, object> GetParameters(string searchTerm, DateTime sinceTime, BO.DocumentSourceInfo info, List<Location> locations)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("since", sinceTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //if(locations != null && locations.Count > 0)
            //{
            //    parameters.Add("center", "40.714623,-74.006605");
            //    parameters.Add("distance", "16000");
            //}

            BO.ProfileTypes profileType = (BO.ProfileTypes)info.ProfileType;

            switch (profileType)
            {
                case BO.ProfileTypes.FacebookPage:

                    parameters.Add("q", searchTerm);
                    parameters.Add("type", "page");

                    break;

                case BO.ProfileTypes.FacebookUser:

                    parameters.Add("q", searchTerm);
                    parameters.Add("type", "user");

                    break;

                case BO.ProfileTypes.FacebookEvent:

                    parameters.Add("q", searchTerm);
                    parameters.Add("type", "event");

                    break;

                case BO.ProfileTypes.FacebookGroup:

                    parameters.Add("q", searchTerm);
                    parameters.Add("type", "group");

                    break;

                case BO.ProfileTypes.FacebookPlace:

                    parameters.Add("q", searchTerm);
                    parameters.Add("type", "place");

                    //if(locations != null && locations.Count > 0)
                    //{
                    //    parameters.Add("center", "40.714623,-74.006605");
                    //    parameters.Add("distance", "16000");
                    //}

                    break;

                case BO.ProfileTypes.FacebookChannel:
                    break;
            }

            return parameters;
        }

        private static List<BO.FacebookPost> GetAllPagedPosts(string path, string accessToken, Dictionary<string, object> parameters, DateTime sinceTime, DateTime? untilTime, bool getCommentAndLikes, out DateTime maximumTime, string url)
        {
            maximumTime = sinceTime;
            List<BO.FacebookPost> posts = new List<Insight360.BO.FacebookPost>();
            List<JsonObject> jsonPosts = new List<JsonObject>();
            List<JsonObject> pages = GetAllPagedItems(path, accessToken, parameters, sinceTime, untilTime);

            /********************************************************************Ahsen Mehmood*********************************************************/
            // in case of facebook channels
            if (url != "http://www.facebook.com")
            {
                jsonPosts = GetAllPagedItems(path, accessToken, parameters, sinceTime, untilTime);
                Util.Log.WriteOperationStarted(typeof(DataCollection), "SearchPosts", "Getting comments and likes of channel" + path);
            }
            // in case of all profiles
            else if (url == "http://www.facebook.com")
            {
                /// getting all pages
                //  now get posts in each page
                foreach (var page in pages)
                {
                    parameters.Clear();

                    var feeds = GetAllPagedItems(page["id"].ToString() + "/feed", accessToken, parameters, sinceTime, untilTime);

                    jsonPosts.AddRange(feeds);
                    //#if DEBUG
                    //                    if (jsonPosts.Count > 0)
                    //                        break;
                    //#endif
                }
                Util.Log.WriteOperationStarted(typeof(DataCollection), "SearchPosts", "Getting comments and likes " + pages.Count);

            }

            /******************************************************************************************************************************************/
            for (int i = 0; i < jsonPosts.Count; i++)
            {
                var post = CreatePost(jsonPosts[i]);
                bool exists = false;
                foreach (var p in posts)
                {
                    if (p.Id == post.Id)
                    {
                        Util.Log.WriteOperationCompleted(typeof(DataCollection), "SearchPosts", "Duplicate post found");
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    post.CommentsCount = GetPostCommentsCount(post.Id, accessToken);
                    post.LikesCount = GetLikesCount(post.Id, accessToken);

                    posts.Add(post);
                    if (post.CreatedTime > maximumTime)
                        maximumTime = post.CreatedTime;

                    if (getCommentAndLikes)
                    {
                        post.Comments = GetPostComments(post.Id, accessToken);
                        post.Likes = GetLikes(post.Id, accessToken);
                    }
                    else
                    {
                        post.Comments = new List<Insight360.BO.FacebookCommentInfo>();
                        post.Likes = new List<Insight360.BO.FacebookLikeInfo>();
                    }

                    post.user = GetUserDetails(accessToken, new List<string>() { post.From.Id })[0];
                    if (!string.IsNullOrEmpty(post.To.Id))
                    {
                        post.toUser = GetUserDetails(accessToken, new List<string>() { post.To.Id })[0];
                    }
                    else post.toUser.UpdatedTime = DateTime.Now;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(i + 1);
                    sb.Append("/");
                    sb.Append(pages.Count);
                    sb.Append(" completed");
                    FillPathAndParameters(sb, path, parameters);
                    Util.Log.WriteOperationCompleted(typeof(DataCollection), "SearchPosts", sb.ToString());
                }
            }

            return posts;
        }

        public static List<BO.FacebookPost> CreatePosts(List<JsonObject> jsonPosts, string accessToken, bool getCommentAndLikes, DateTime sinceTime, out DateTime maximumTime)
        {
            maximumTime = sinceTime;
            List<BO.FacebookPost> posts = new List<Insight360.BO.FacebookPost>();

            for (int i = 0; i < jsonPosts.Count; i++)
            {
                var post = CreatePost(jsonPosts[i]);
                bool exists = false;
                foreach (var p in posts)
                {
                    if (p.Id == post.Id)
                    {
                        Util.Log.WriteOperationCompleted(typeof(DataCollection), "SearchPosts", "Duplicate post found");
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    post.CommentsCount = GetPostCommentsCount(post.Id, accessToken);
                    post.LikesCount = GetLikesCount(post.Id, accessToken);

                    posts.Add(post);
                    if (post.CreatedTime > maximumTime)
                        maximumTime = post.CreatedTime;

                    if (getCommentAndLikes)
                    {
                        post.Comments = GetPostComments(post.Id, accessToken);
                        post.Likes = GetLikes(post.Id, accessToken);
                    }
                    else
                    {
                        post.Comments = new List<Insight360.BO.FacebookCommentInfo>();
                        post.Likes = new List<Insight360.BO.FacebookLikeInfo>();
                    }

                    post.user = GetUserDetails(accessToken, new List<string>() { post.From.Id })[0];

                    if (!string.IsNullOrEmpty(post.To.Id))
                    {
                        post.toUser = GetUserDetails(accessToken, new List<string>() { post.To.Id })[0];
                    }
                    else post.toUser.UpdatedTime = DateTime.Now;
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append(i + 1);
                    //sb.Append("/");
                    //sb.Append(pages.Count);
                    //sb.Append(" completed");
                    //FillPathAndParameters(sb, path, parameters);
                    Util.Log.WriteOperationCompleted(typeof(DataCollection), "SearchPosts", post.Id.ToString());
                }
            }

            return posts;
        }

        public static BO.FacebookPost CreateLocalPost(JsonObject jsonPost, string accessToken, bool getCommentAndLikes, DateTime sinceTime, out DateTime maximumTime)
        {
            maximumTime = sinceTime;

            try
            {
                BO.FacebookPost post = CreatePost(jsonPost);

                var taskSentiment = Helpers.SentimentScoreAsync(post.Message);
                var taskLanguage = Helpers.DetectLanguageAsync(post.Message);

                //post.CommentsCount = GetPostCommentsCount(post.Id, accessToken);
                //post.LikesCount = GetLikesCount(post.Id, accessToken);

                if (post.CreatedTime > maximumTime)
                    maximumTime = post.CreatedTime;

                if (getCommentAndLikes)
                {
                    post.Comments = GetPostComments(post.Id, accessToken);
                    post.Likes = GetLikes(post.Id, accessToken);
                }

                post.user = GetUserDetails(accessToken, new List<string>() { post.From.Id })[0];

                if (!string.IsNullOrEmpty(post.To.Id))
                {
                    post.toUser = GetUserDetails(accessToken, new List<string>() { post.To.Id })[0];
                }
                else post.toUser.UpdatedTime = DateTime.Now;

                var taskgender = Helpers.DetectGenderAsync(post.user.Name);

                Task.WaitAll(taskSentiment, taskLanguage, taskgender);

                post.Sentiment = taskSentiment.Result == null ? 3 : Convert.ToInt32(taskSentiment.Result.ToString());
                post.LanguageCode = taskLanguage.Result == null ? "" : taskLanguage.Result.ToString();
                post.user.Gender = taskgender.Result;

                Util.Log.WriteOperationCompleted(typeof(DataCollection), "SearchPosts", post.Id.ToString());

                return post;
            }
            catch (Exception ex)
            {
                Util.Log.WriteOperationCompleted(typeof(DataCollection), "FetchPostsAndPersist", "Create Local Post");
                return null;
            }
        }

        static int GetPostCommentsCount(string postId, string accessToken)
        {
            try
            {
                FacebookClient app = new FacebookClient();
                app.AccessToken = accessToken;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("access_token", accessToken);
                var result = (IDictionary<string, object>)app.Get(postId + "/comments?summary=true", parameters);
                JsonObject data = result["summary"] as JsonObject;
                return Convert.ToInt32(GetValue(data, "total_count"));
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(DataCollection), "Get Post Comments Count", ex);
                return 0;
            }
        }

        static int GetLikesCount(string postId, string accessToken)
        {
            try
            {
                FacebookClient app = new FacebookClient();
                app.AccessToken = accessToken;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("access_token", accessToken);
                var result = (IDictionary<string, object>)app.Get(postId + "/likes?summary=true", parameters);
                JsonObject data = result["summary"] as JsonObject;
                return Convert.ToInt32(GetValue(data, "total_count"));
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(DataCollection), "Get Post Likes Count", ex);
                return 0;
            }
        }

        static BO.FacebookUser CreateUser(JsonObject obj, string id, string followersCount, string followingCount)
        {
            BO.FacebookUser user = new Insight360.BO.FacebookUser()
            {
                Id = GetValue(obj, "id"),
                Name = GetValue(obj, "name"),
                FirstName = GetValue(obj, "first_name"),
                LastName = GetValue(obj, "last_name"),
                Gender = GetValue(obj, "gender"),
                Link = GetValue(obj, "link"),
                Locale = GetValue(obj, "locale"),
                Picture = GetValue(obj, "picture"),
                ThirdPartyId = GetValue(obj, "third_party_id"),
                UpdatedTime = GetDateValue(obj, "updated_time"),
                TimeZone = obj.Keys.Contains("timezone") ? GetValue(obj, "timezone") : null,
                FollowersCount = followersCount,
                FollowingCount = followingCount,

            };

            return user;
        }

        //https://graph.facebook.com/?ids=100000755914395,100001253508531&access_token=169095506475698|2e66dc828704143b13c3d8b1-100002075274719|tjsEs3-4AcbRU7UAfT1v6W5lAeA&fields=id,name,first_name,last_name,gender,locale,link,third_party_id,updated_time,verified,picture
        public static List<BO.FacebookUser> GetUserDetails(string accessToken, List<string> userIds)
        {
            List<BO.FacebookUser> results = new List<Insight360.BO.FacebookUser>();

            FacebookClient app = new FacebookClient();
            app.AccessToken = accessToken;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder sb = new StringBuilder();

            foreach (string userId in userIds)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(userId);
            }

            parameters.Add("ids", sb.ToString());
            parameters.Add("access_token", accessToken);
            parameters.Add("fields", "location,name,id");
            //parameters.Add("fields", "id,name,first_name,last_name,gender,locale,link,third_party_id,updated_time,verified,picture");

            var result = (IDictionary<string, object>)app.Get("", parameters);
            foreach (var r in result.Values)
            {
                //Ahsen Mehmood Nov 26, 2015 fields= subscribedto and susbscribers is depricated so not calling the following methods and returning 0
                //string followersCount = (!obj.Keys.Contains("likes") ? GetUserFollowersCount(sb.ToString(), accessToken) : "0");
                //string followingCount = (!obj.Keys.Contains("likes") ? GetUsersFollowingCount(sb.ToString(), accessToken) : "0");
                string followersCount = "0";
                string followingCount = "0";
                results.Add(CreateUser(r as JsonObject, null, followersCount, followingCount));
            }


            return results;
        }

        public static string GetUserFollowersCount(string userId, string accessToken)
        {
            try
            {
                FacebookClient app = new FacebookClient();
                app.AccessToken = accessToken;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("access_token", accessToken);
                var result = (IDictionary<string, object>)app.Get(userId + "/subscribers", parameters);
                if (result.Keys.Contains("summary"))
                {
                    JsonObject data = result["summary"] as JsonObject;
                    return GetValue(data, "total_count");
                }
                else return "0";
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(DataCollection), "Get User Followers Count", ex);
                return "0";
            }

        }

        public static string GetUsersFollowingCount(string userId, string accessToken)
        {
            try
            {
                FacebookClient app = new FacebookClient();
                app.AccessToken = accessToken;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("access_token", accessToken);
                var result = (IDictionary<string, object>)app.Get(userId + "?fields=subscribedto%2Csubscribers", parameters);
                if (result.Keys.Contains("subscribedto"))
                {
                    JsonObject data = result["subscribedto"] as JsonObject;

                    return GetValue(data["summary"] as JsonObject, "total_count");
                }
                else return "0";
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(DataCollection), "Get User Following Count", ex);
                return "0";
            }

        }

        static BO.FacebookLikeInfo CreateLike(JsonObject obj)
        {
            BO.FacebookLikeInfo likeInfo = new Insight360.BO.FacebookLikeInfo()
            {
                Id = GetValue(obj as JsonObject, "id"),
                Name = GetValue(obj as JsonObject, "name"),
            };
            return likeInfo;
        }

        //https://graph.facebook.com/100002171651511_103326983083023_19607/likes
        //https://graph.facebook.com/100001292791875_185783794775630/likes
        static List<BO.FacebookLikeInfo> GetLikes(string id, string accessToken)
        {
            List<BO.FacebookLikeInfo> likes = new List<Insight360.BO.FacebookLikeInfo>();
            List<JsonObject> items = GetAllPagedItems(id + "/likes", accessToken, new Dictionary<string, object>(), null, null);
            for (int i = 0; i < items.Count; i++)
            {
                likes.Add(CreateLike(items[i]));
            }
            return likes;
        }

        static BO.FacebookCommentInfo CreateComment(JsonObject obj)
        {
            BO.FacebookCommentInfo commentInfo = new Insight360.BO.FacebookCommentInfo()
            {
                Id = GetValue(obj as JsonObject, "id"),
                From = new Insight360.BO.FacebookUserInfo()
                {
                    Name = obj.Keys.Contains("from") ? GetValue(obj["from"] as JsonObject, "name") : string.Empty,
                    Id = obj.Keys.Contains("from") ? GetValue(obj["from"] as JsonObject, "id") : string.Empty,
                },
                CreatedTime = GetDateValue(obj, "created_time"),
                Message = GetValue(obj as JsonObject, "message"),
                Sentiment = Helpers.SentimentScore(GetValue(obj as JsonObject, "message")),
                LanguageCode = Helpers.DetectLanguage(GetValue(obj as JsonObject, "message"))
            };
            return commentInfo;
        }

        //https://graph.facebook.com/100001292791875_185783794775630/comments
        static List<BO.FacebookCommentInfo> GetPostComments(string postId, string accessToken)
        {
            List<BO.FacebookCommentInfo> comments = new List<Insight360.BO.FacebookCommentInfo>();
            List<JsonObject> items = GetAllPagedItems(postId + "/comments", accessToken, new Dictionary<string, object>(), null, null);
            for (int i = 0; i < items.Count; i++)
            {
                var comment = CreateComment(items[i]);
                bool exists = false;
                foreach (var c in comments)
                {
                    if (c.Id == comment.Id)
                    {
                        Util.Log.WriteOperationCompleted(typeof(DataCollection), "GetPostComments", "Duplicate comment found");
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    comments.Add(comment);
                    comment.Likes = GetLikes(comment.Id, accessToken);
                }
            }
            return comments;
        }

        //https://graph.facebook.com/100001292791875_185783794775630
        public static BO.FacebookPost GetPost(string accessToken, string postId, bool getCommentAndLikes)
        {
            BO.FacebookPost post = null;
            try
            {
                FacebookClient app = new FacebookClient();
                app.AccessToken = accessToken;
                var result = (IDictionary<string, object>)app.Get(postId, null);

                post = CreatePost(result as JsonObject);

                var taskSentiment = Helpers.SentimentScoreAsync(post.Message);
                var taskLanguage = Helpers.DetectLanguageAsync(post.Message);

                if (getCommentAndLikes)
                {
                    post.Comments = GetPostComments(post.Id, accessToken);
                    post.Likes = GetLikes(post.Id, accessToken);
                }

                post.user = GetUserDetails(accessToken, new List<string>() { post.From.Id })[0];

                if (post.To.Id != "")
                {
                    post.toUser = GetUserDetails(accessToken, new List<string>() { post.To.Id })[0];
                }

                Task.WaitAll(taskSentiment, taskLanguage);

                post.Sentiment = taskSentiment.Result == null ? 3 : Convert.ToInt32(taskSentiment.Result.ToString());
                post.LanguageCode = taskLanguage.Result == null ? "" : taskLanguage.Result.ToString();
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(DataCollection), "GetPost-" + accessToken + "-" + postId, ex);
            }
            return post;
        }

        static Dictionary<string, object> FillNextPageParameters(IDictionary<string, object> result)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (result!=null&&result.Keys.Contains("paging"))
            {
                JsonObject y = result["paging"] as JsonObject;

                if (y.Keys.Contains("next"))
                {
                    Uri uri = new Uri(y["next"].ToString());
                    NameValueCollection query = new NameValueCollection();
                    query = HttpUtility.ParseQueryString(uri.Query);

                    for (int i = 0; i < query.Keys.Count; i++)
                    {
                        parameters.Add(query.Keys[i], query[query.Keys[i]]);
                    }
                }
            }

            return parameters;
        }

        public static List<JsonObject> GetAllPagedItems(string path, string accessToken, Dictionary<string, object> parameters, DateTime? since, DateTime? until)
        {
            FacebookClient app = new FacebookClient();
            app.AccessToken = accessToken;

            List<JsonObject> items = new List<JsonObject>();
            bool nextPageAvailable = true;
            parameters.Add("access_token", accessToken);

            //parameters.Add("limit", 200);
            //     if (until != null && !parameters.Keys.Contains("until"))
            //       parameters.Add("until", until.Value.Date.ToString("yyyy-MM-dd HH:mm:ss")); 

            while (nextPageAvailable)
            {
                //facebook removes since from pervious query params
                if (since != null && !parameters.Keys.Contains("since"))
                {
                    parameters.Add("since", since.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }

                try
                {
                    var result = (IDictionary<string, object>)app.Get(path, parameters);
                    JsonArray data = result["data"] as JsonArray;

                    int count = 0;

                    for (int i = 0; i < data.Count; i++)
                    {
                        bool exists = false;
                        if ((data[i] as JsonObject).Keys.Contains("id"))
                        {
                            foreach (var item in items)
                            {
                                if (GetValue(data[i] as JsonObject, "id") == GetValue(item, "id"))
                                {
                                    Util.Log.WriteOperationCompleted(typeof(DataCollection), "GetAllPagedItems", "Duplicate found");
                                    exists = true;
                                    break;
                                }
                            }
                        }

                        if (!exists)
                        {
                            count++;
                            items.Add(data[i] as JsonObject);
                        }
                    }

                    var oldParams = parameters;
                    parameters = FillNextPageParameters(result);
                    nextPageAvailable = parameters.Count > 0 && data.Count > 0 && count > 0;
                    //int c = (from y in items select GetValue(y, "id")).Distinct().Count();
                    //Util.Log.WriteOperationCompleted(x.Count + "-" + c + "-" + items.Count);

                    StringBuilder sb = new StringBuilder();
                    sb.Append(count);
                    sb.Append(" - ");
                    sb.Append(data.Count);
                    sb.Append(" - ");
                    sb.Append(items.Count);
                    sb.Append(" - ");

                    try
                    {
                        if (items.Count > 0)
                            sb.Append(GetDateValue(items[items.Count - 1], "created_time"));
                    }
                    catch { }

                    FillPathAndParameters(sb, path, oldParams);

                    if (count <= 5)
                        Thread.Sleep(1000);
                    else
                        Thread.Sleep(400);

                }
                catch (FacebookApiException ex)
                {
                    //((Facebook.FacebookApiException)ex).ErrorType
                    Util.Log.WriteException(ex.GetType(), "Facebook GetAllPagedItems", ex);
                    nextPageAvailable = false;
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(ex.GetType(), "Facebook GetAllPagedItems", ex);
                    nextPageAvailable = false;
                }
            }

            return items;
        }

        public static List<JsonObject> GetPagedItems(string path, string accessToken, Dictionary<string, object> parameters, DateTime? since, DateTime? until, out bool nextPageAvailable, out Dictionary<string, object> oldParams)
        {
            FacebookClient app = new FacebookClient();
            app.AccessToken = accessToken;

            List<JsonObject> items = new List<JsonObject>();

            if (!parameters.Keys.Contains("access_token"))
                parameters.Add("access_token", accessToken);

            nextPageAvailable = true;

            //     if (until != null && !parameters.Keys.Contains("until"))
            //       parameters.Add("until", until.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));

            //facebook removes since from pervious query params
            if (since != null && !parameters.Keys.Contains("since"))
                parameters.Add("since", since.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));

            try
            {
                var result = (IDictionary<string, object>)app.Get(path, parameters);
                JsonArray data = result["data"] as JsonArray;

                int count = data.Count;
                data.ForEach(item => items.Add(item as JsonObject));

                oldParams = parameters;
                parameters = FillNextPageParameters(result);
                nextPageAvailable = parameters.Count > 0 && data.Count > 0;

                StringBuilder sb = new StringBuilder();
                sb.Append(count);
                sb.Append(" - ");
                sb.Append(data.Count);
                sb.Append(" - ");
                sb.Append(items.Count);
                sb.Append(" - ");

                try
                {
                    if (items.Count > 0)
                        sb.Append(GetDateValue(items[items.Count - 1], "created_time"));
                }
                catch { }

                FillPathAndParameters(sb, path, oldParams);
                Util.Log.WriteOperationCompleted(typeof(DataCollection), "GetAllPagedItems", sb.ToString());

                if (count <= 5)
                    Thread.Sleep(1000);
                else
                    Thread.Sleep(400);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(path);
                Util.Log.WriteException(typeof(FacebookClient), "GetAllPagedItems", ex);

            }

            //oldParams.Clear();
            oldParams = parameters;

            return items;
        }

        static void FillPathAndParameters(StringBuilder sb, string path, Dictionary<string, object> parameters)
        {
            sb.Append(" - ");
            sb.Append(path);
            foreach (var x in parameters.Keys)
            {
                sb.Append(" - ");
                sb.Append(x);
                sb.Append(":");
                sb.Append(parameters[x].ToString());
            }
        }

        public static string GetToken()
        {

            var fb = new FacebookClient();
            var result = fb.Get("oauth/access_token", new
            {
                client_id = AppSettings.Facebook_ClientId,
                client_secret = AppSettings.Facebook_ClientSecret,
                grant_type = "client_credentials"
            }) as JsonObject;
            return result["access_token"].ToString();

            //FacebookClient fbClient = new FacebookClient();
            //Dictionary<string, object> fbParams = new Dictionary<string, object>();
            //fbParams["client_id"] = "749622071786940";
            //fbParams["grant_type"] = "fb_exchange_token";
            //fbParams["client_secret"] = "5058c117fcb3ba8695ec79aa1384f687";
            //if (string.IsNullOrEmpty(currentAccessToken))
            //{
            //    currentAccessToken = System.Configuration.ConfigurationSettings.AppSettings["Facebook_AccessToken_User"];
            //}
            //fbParams["fb_exchange_token"] = currentAccessToken;
            //JsonObject publishedResponse = fbClient.Get("/oauth/access_token", fbParams) as JsonObject;
            //currentAccessToken = publishedResponse["access_token"].ToString();

        }

        static string SaveImageWithNewUrl(string oldUrl, string mediaId)
        {
            String newUrl = AppSettings.MinerAPP_URL + "/FBImages/" + mediaId + ".jpg";

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(oldUrl);

                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        using (var yourImage = Image.FromStream(mem))
                        {
                            // If you want it as Png
                            //yourImage.Save("path_to_your_file.png", ImageFormat.Png);

                            // If you want it as Jpeg
                            var imgPath = AppDomain.CurrentDomain.BaseDirectory + "FBImages\\" + mediaId + ".jpg";
                            yourImage.Save(imgPath, ImageFormat.Jpeg);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                newUrl = oldUrl;
            }

            return newUrl;
        }

        public static List<FBPost> GetFBFeeds(string accessToken, ref DateTime? since, string fbId)
        {
            FacebookClient app = new FacebookClient();
            app.AccessToken = accessToken;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<FBPost> items = new List<FBPost>();
            bool nextPageAvailable = true;
            parameters.Add("access_token", accessToken);
            int RESULTS_PER_PAGE = 100;
            int fetchedPosts = 0;

            //facebook removes since from pervious query params
            if (since == null)
            {
                parameters.Add("limit", 1);
            }
            else
            {
                parameters.Add("limit", RESULTS_PER_PAGE);
                parameters.Add("since", since.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            while (nextPageAvailable)
            {
                try
                {
                    var result = (IDictionary<string, object>)app.Get("/" + fbId + "/feed?fields=from{picture.width(100).height(100),id,name,is_verified},message,story,created_time,id,attachments", parameters);
                    JsonArray data = result["data"] as JsonArray;
                    int count = 0;

                    foreach (JsonObject obj in data)
                    {
                        var Message = (!obj.ContainsKey("message")) ? (obj.ContainsKey("story") ? obj["story"].ToString() : string.Empty) : obj["message"].ToString();
                        var PostId = obj["id"].ToString();
                        var CreatedDate = obj.ContainsKey("created_time") ? Convert.ToDateTime(obj["created_time"].ToString()):DateTime.UtcNow;
                        JsonObject from = obj["from"] as JsonObject;
                        var AuthorUserId = from.ContainsKey("id")?Convert.ToString(from["id"]):string.Empty;
                        var AuthorUserName = from.ContainsKey("name") ? Convert.ToString(from["name"]) : string.Empty;
                        var IsAuthorVerified = from.ContainsKey("is_verified") ? Convert.ToBoolean(from["is_verified"]):false;

                        string AuthorProfilePic=string.Empty;
                        if (from.ContainsKey("picture"))
                        {
                            JsonObject pictureObj = (JsonObject)((JsonObject)from["picture"])["data"];
                            AuthorProfilePic = Convert.ToString(pictureObj["url"]);
                        }
                        items.Add(new FBPost
                        {
                            Message = Message,
                            PostId = PostId,
                            CreatedDate = CreatedDate,
                            MaxCommentId = "0",
                            AuthorUserId = AuthorUserId,
                            AuthorUserName = AuthorUserName,
                            AuthorProfilePic = AuthorProfilePic,
                            IsAuthorVerified = IsAuthorVerified
                        });


                        if (obj.ContainsKey("attachments"))
                        {
                            JsonObject attachment = obj["attachments"] as JsonObject;
                            if(attachment!=null&&attachment.ContainsKey("data"))
                            {
                                JsonArray attachmentArray = attachment["data"] as JsonArray;
                                foreach (JsonObject item in attachmentArray)
                                {
                                    bool isSingleAttachment = item.ContainsKey("media");
                                    items[fetchedPosts].MediaAttachment = new List<BO.FeedMediaAttachment>();
                                    if (isSingleAttachment)
                                    {
                                        if (item.ContainsKey("type"))
                                        {
                                            var type = item["type"].ToString();
                                            var attachmentUrl = (type == "photo" || type == "stream_publish") ? ((JsonObject)((JsonObject)item["media"])["image"])["src"].ToString() : item["url"].ToString();
                                            var description = type;// (item.ContainsKey("description")) ? item["description"].ToString() : string.Empty;
                                            if (type == "photo" || type == "stream_publish")
                                                attachmentUrl = SaveImageWithNewUrl(attachmentUrl, PostId);
                                            items[fetchedPosts].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "FP",
                                                Description = description,
                                                AttachmentUrl = attachmentUrl
                                            });
                                        }
                                    }
                                    else
                                    {
                                        JsonObject subAttachment = (JsonObject)item["subattachments"];
                                        JsonArray subAttachmentArray = subAttachment["data"] as JsonArray;
                                        foreach (JsonObject sub in subAttachmentArray)
                                        {
                                            var type = sub["type"].ToString();
                                            var attachmentUrl = (type == "photo") ? ((JsonObject)((JsonObject)sub["media"])["image"])["src"].ToString() : sub["url"].ToString();
                                            var description = type;//(sub.ContainsKey("description"))? sub["description"].ToString() : string.Empty;
                                            if (type == "photo")
                                                attachmentUrl = SaveImageWithNewUrl(attachmentUrl, PostId);
                                            items[fetchedPosts].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "FP",
                                                Description = description,
                                                AttachmentUrl = attachmentUrl
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        count++;
                        fetchedPosts++;
                    }

                    parameters = FillNextPageParameters(result);
                    nextPageAvailable = data.Count > 0 && since != null && parameters.Count > 0;
                }
                catch (FacebookApiException ex)
                {
                    //((Facebook.FacebookApiException)ex).ErrorType
                    Util.Log.WriteException(ex.GetType(), "Facebook GetFBFeeds", ex);
                    nextPageAvailable = false;
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(ex.GetType(), "Facebook GetFBFeeds", ex);
                    nextPageAvailable = false;
                }
            }

            if (items.Count > 0)
                since = items[0].CreatedDate;

            return items;
        }

        public static List<FBPageChat> GetFBPageChats(string accessToken, ref DateTime? since, string fbId, List<ConvMaxDates> conMsgMaxDates)
        {
            FacebookClient app = new FacebookClient();
            app.AccessToken = accessToken;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<FBPageChat> conversations = new List<FBPageChat>();
            bool nextPageAvailable = true;
            parameters.Add("access_token", accessToken);
            int RESULTS_PER_PAGE = 100;
            bool firstTime = true;
            int fetchedPosts = 0;
            DateTime? tempSince = null;

            //facebook removes since from pervious query params
            if (since == null)
            {
                parameters.Add("limit", 1);
            }
            else
            {
                parameters.Add("limit", RESULTS_PER_PAGE);
                parameters.Add("since", since.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }

            while (nextPageAvailable)
            {
                try
                {
                    var result = (IDictionary<string, object>)app.Get("/" + fbId + "/conversations", parameters);
                    JsonArray data = result["data"] as JsonArray;
                    int MSG_RESULTS_PER_PAGE = 100;

                    int convCount = 0;
                    foreach (JsonObject item in data)
                    {
                        DateTime updatedTime = item.ContainsKey("updated_time") ? Convert.ToDateTime(item["updated_time"].ToString()) : DateTime.UtcNow;
                        if (updatedTime <= since)
                            break;

                        Dictionary<string, object> msgParameters = new Dictionary<string, object>();
                        msgParameters.Add("access_token", accessToken);
                        //To get the updated time of the conversation for just the first time
                        if (firstTime)
                        {
                            tempSince = item.ContainsKey("updated_time") ? Convert.ToDateTime(item["updated_time"].ToString()):DateTime.UtcNow.AddDays(-1);
                            firstTime = false;
                        }

                        var msgFbId = item.ContainsKey("id") ? item["id"].ToString():string.Empty;
                        DateTime? msgSince = null;
                        DateTime now = DateTime.Now.Date;

                        if (conMsgMaxDates.Any(x => x.ConId == msgFbId))
                        {
                            msgSince = conMsgMaxDates.Where(x => x.ConId == msgFbId).Select(y => y.MaxCreatedDate).FirstOrDefault();
                            msgParameters.Add("limit", MSG_RESULTS_PER_PAGE);
                        }

                        bool msgNextPageAvailable = true;
                        while (msgNextPageAvailable)
                        {
                            try
                            {
                                var msgResult = (IDictionary<string, object>)app.Get("/" + msgFbId + "/messages?fields=created_time,from,id,message,to,attachments", msgParameters);
                                JsonArray msgData = msgResult["data"] as JsonArray;

                                int count = 0;
                                foreach (JsonObject msgItem in msgData)
                                {
                                    var MsgText = msgItem.ContainsKey("message") ? msgItem["message"].ToString() : string.Empty;
                                    var MsgId = msgItem.ContainsKey("id") ? msgItem["id"].ToString() : string.Empty;
                                    var CreatedDate = msgItem.ContainsKey("created_time") ? Convert.ToDateTime(msgItem["created_time"].ToString()) : DateTime.UtcNow;

                                    JsonObject from = msgItem["from"] as JsonObject;
                                    if (from != null)
                                    {
                                        var FromId = from.ContainsKey("id") ? from["id"].ToString():string.Empty;
                                        var FromName = from.ContainsKey("name") ? from["name"].ToString() : string.Empty;
                                        var IsFromVerified = false;
                                        if(msgItem.ContainsKey("to"))
                                        {
                                            JsonObject to = (JsonObject)((JsonArray)((JsonObject)msgItem["to"])["data"])[0];
                                            var ToId = to.ContainsKey("id") ? to["id"].ToString() : string.Empty;
                                            var ToName = to.ContainsKey("name") ? to["name"].ToString() : string.Empty;

                                            if ((msgSince == null && CreatedDate < now) || CreatedDate <= msgSince)
                                                break;
                                            conversations.Add(new FBPageChat
                                            {
                                                ConId = msgFbId,
                                                MsgText = MsgText,
                                                MsgId = MsgId,
                                                CreatedDate = CreatedDate,
                                                FromId = FromId,
                                                FromName = FromName,
                                                IsFromVerified = IsFromVerified,
                                                ToId = ToId,
                                                ToName = ToName
                                            });
                                        }
                                        
                                    }

                                    if (msgItem.ContainsKey("attachments"))
                                    {
                                        JsonObject attachment = msgItem["attachments"] as JsonObject;
                                        JsonArray attachmentArray = attachment["data"] as JsonArray;
                                        conversations[fetchedPosts].MediaAttachment = new List<BO.FeedMediaAttachment>();
                                        var attachmentUrl = "";
                                        var type = "";
                                        foreach (JsonObject attachmentItem in attachmentArray)
                                        {
                                            type = attachmentItem["mime_type"].ToString();
                                            if (type.Contains("image"))
                                            {
                                                attachmentUrl = ((JsonObject)attachmentItem["image_data"])["url"].ToString();
                                                attachmentUrl = SaveImageWithNewUrl(attachmentUrl, MsgId);
                                            }                                                
                                            else if (type.Contains("video"))
                                                attachmentUrl = ((JsonObject)attachmentItem["video_data"])["url"].ToString();

                                            conversations[fetchedPosts].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "PCF",
                                                Description = type,
                                                AttachmentUrl = attachmentUrl
                                            });
                                        }
                                    }
                                    count++;
                                    fetchedPosts++;
                                }

                                msgParameters = FillNextPageParameters(msgResult);
                                msgNextPageAvailable = msgData.Count > 0 && count == msgData.Count && msgSince != null && msgParameters.Count > 0;
                            }
                            catch (FacebookApiException ex)
                            {
                                //((Facebook.FacebookApiException)ex).ErrorType
                                Util.Log.WriteException(ex.GetType(), "Facebook GetFBPageChatMsgs", ex);
                                msgNextPageAvailable = false;
                            }
                            catch (Exception ex)
                            {
                                Util.Log.WriteException(ex.GetType(), "Facebook GetFBPageChatMsgs", ex);
                                msgNextPageAvailable = false;
                            }
                        }
                        convCount++;
                    }

                    parameters = FillNextPageParameters(result);
                    nextPageAvailable = data.Count > 0 && convCount == data.Count && since != null && parameters.Count > 0;
                }
                catch (FacebookApiException ex)
                {
                    //((Facebook.FacebookApiException)ex).ErrorType
                    Util.Log.WriteException(ex.GetType(), "Facebook GetFBPageChats", ex);
                    nextPageAvailable = false;
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(ex.GetType(), "Facebook GetFBPageChats", ex);
                    nextPageAvailable = false;
                }
            }

            if (tempSince != null)
                since = tempSince;

            return conversations;
        }

        public static List<FBComment> GetFBComments(string accessToken, ref string lastFetchedCommentId, string postId)
        {
            FacebookClient app = new FacebookClient();
            app.AccessToken = accessToken;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<FBComment> items = new List<FBComment>();
            bool nextPageAvailable = true;
            parameters.Add("access_token", accessToken);

            int RESULTS_PER_PAGE = 100;
            parameters.Add("order", "reverse_chronological");
            parameters.Add("limit", RESULTS_PER_PAGE);

            while (nextPageAvailable)
            {
                try
                {
                    var result = (IDictionary<string, object>)app.Get("/" + postId + "/comments?fields=from{picture.width(100).height(100),id,name,is_verified},parent{id},message,created_time,id,attachment", parameters);
                    JsonArray data = result["data"] as JsonArray;

                    int i = 0;

                    foreach (JsonObject obj in data)
                    {
                        var CommentId = Convert.ToString(obj["id"]);
                        if (CommentId == lastFetchedCommentId)
                            break;

                        var Message = obj.ContainsKey("message") ? obj["message"].ToString() : string.Empty;
                        var CreatedDate = obj.ContainsKey("created_time") ? Convert.ToDateTime(obj["created_time"].ToString()):DateTime.Now;
                        JsonObject from = obj.ContainsKey("from") ? (JsonObject)obj["from"]:new JsonObject();
                        var FromId = from.ContainsKey("id") ? Convert.ToString(from["id"]) : string.Empty;
                        var FromName = from.ContainsKey("name") ? Convert.ToString(from["name"]) : string.Empty;
                        var IsFromVerified = from.ContainsKey("is_verified") ? Convert.ToBoolean(from["is_verified"]):false;
                        var FromUserProfilePic = string.Empty;
                        if (from.ContainsKey("picture"))
                        {
                            JsonObject pictureObj = (JsonObject)((JsonObject)from["picture"])["data"];
                            FromUserProfilePic = pictureObj["url"].ToString();
                        }
                        var attachmentUrl = "";
                        var type = "";
                        if (obj.ContainsKey("attachment"))
                        {
                            type = ((JsonObject)obj["attachment"])["type"].ToString();
                            attachmentUrl = (type == "photo") ? ((JsonObject)((JsonObject)((JsonObject)((JsonObject)obj["attachment"])["media"]))["image"])["src"].ToString() : ((JsonObject)obj["attachment"])["url"].ToString();
                            if(type == "photo")
                                attachmentUrl = SaveImageWithNewUrl(attachmentUrl, CommentId);
                        }
                        var ParentId = "";
                        if (obj.ContainsKey("parent"))
                        {
                            JsonObject parent = (JsonObject)obj["parent"];
                            ParentId = parent.ContainsKey("id") ? Convert.ToString(parent["id"]):string.Empty;
                        }

                            items.Add(new FBComment
                        {
                            Message = Message,
                            CommentId = CommentId,
                            MaxCommentId = "0",
                            CreatedDate = CreatedDate,
                            FromId = FromId,
                            FromName = FromName,
                            IsFromVerified = IsFromVerified,
                            FromUserProfilePic = FromUserProfilePic,
                            Attachment = attachmentUrl,
                            AttachmentType = type,
                            ParentId = ParentId
                            });
                        i++;
                    }

                    parameters = FillNextPageParameters(result);
                    nextPageAvailable = data.Count > 0 && i == data.Count && parameters.Count > 0;
                }
                catch (FacebookApiException ex)
                {
                    //((Facebook.FacebookApiException)ex).ErrorType
                    Util.Log.WriteException(ex.GetType(), "Facebook API GetFBComments", ex);
                    nextPageAvailable = false;
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(ex.GetType(), "Facebook GetFBComments", ex);
                    nextPageAvailable = false;
                }
            }

            if (items.Count > 0)
                lastFetchedCommentId = items[0].CommentId;

            return items;
        }

        public static double GetEngagementRateByPostId(string accessToken, string postId)
        {
            FacebookClient app = new FacebookClient();
            app.AccessToken = accessToken;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("access_token", accessToken);

            double engagementRate = 0;

            try
            {
                //getPostReachabilityAndConsumption
                JsonObject reachabilityAndConsumption = (IDictionary<string, object>)app.Get("/" + postId + "?fields=insights.metric(post_impressions_unique,post_engaged_users)", parameters) as JsonObject;
                int Reachability = 0, EngagedUsers = 0;
                if (reachabilityAndConsumption.ContainsKey("insights"))
                {
                    Reachability = Convert.ToInt32((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[0] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"]);
                    EngagedUsers = Convert.ToInt32((((((reachabilityAndConsumption["insights"] as JsonObject)["data"] as JsonArray)[1] as JsonObject)["values"] as JsonArray)[0] as JsonObject)["value"]);
                }

                if (Reachability != 0)
                {
                    engagementRate = (double)EngagedUsers / Reachability;

                    if (EngagedUsers > Reachability)
                    {
                        engagementRate = 1;
                    }
                    engagementRate = Math.Round(engagementRate * 100);
                }

                return engagementRate;
                //JsonArray data = result["data"] as JsonArray;
            }
            catch (FacebookApiException ex)
            {
                Util.Log.WriteException(ex.GetType(), "Facebook GetEngagementRateByPostId", ex);
                return engagementRate;
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(ex.GetType(), "Facebook GetEngagementRateByPostId", ex);
                return engagementRate;
            }
        }
    }
}
