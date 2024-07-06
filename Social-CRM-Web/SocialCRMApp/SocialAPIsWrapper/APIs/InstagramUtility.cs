using CRM.APIS.Instagram.Models;
using SocialAPIsWrapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SocialAPIsWrapper.APIs
{
    public class InstagramUtility
    {
        public int getInstagramFollowers(string accessToken)
        {
            var userSelfProfileUrl = string.Format("https://api.instagram.com/v1/users/self/?access_token={0}", accessToken);
            var userSelfProfilResponse = GetRequest(userSelfProfileUrl);
            var selfProfile = new JavaScriptSerializer().Deserialize<UserProfileResponse.RootObject>(userSelfProfilResponse);
            if (selfProfile != null && selfProfile.data != null)
            {
                return selfProfile.data.counts.followed_by;
            }
            return 0;
        }

        public static async Task<string> PostInstagramComment(string postId, string accessToken, string comment)
        {
            string url = string.Format("https://api.instagram.com/v1/media/{0}/comments", postId);
            string postData = "access_token=" + accessToken + "&text=" + comment;
            var userMediaPostCommentResponse = await PostRequest(url, postData);
            return userMediaPostCommentResponse;
        }

        public static async Task<string> DeleteInstagramComment(string postId, string accessToken, string commentId)
        {
            var url = string.Format("https://api.instagram.com/v1/media/{0}/comments/{1}?access_token={2}", postId, commentId, accessToken);
            var userMediaDeleteCommentResponse = await DeleteRequest(url);
            return userMediaDeleteCommentResponse;
        }

        public static async Task<string> LikeInstagramPost(string postId, string accessToken)
        {
            var userMediaPostLikeUrl = string.Format("https://api.instagram.com/v1/media/{0}/likes", postId);
            string postData = "access_token=" + accessToken;
            var userMediaPostLikeResponse = await PostRequest(userMediaPostLikeUrl, postData);
            return userMediaPostLikeResponse;
        }

        public static async Task<string> UnlikeInstagramPost(string postId, string accessToken)
        {
            var userMediaPostUnlikeUrl = string.Format("https://api.instagram.com/v1/media/{0}/likes?access_token={1}", postId, accessToken);
            var userMediaPostUnikeResponse = await DeleteRequest(userMediaPostUnlikeUrl);
            return userMediaPostUnikeResponse;
        }

        public static bool IsInstagramPostLiked(string postId, string accessToken)
        {
            bool result = false;
            var userMediaIsLikedUrl = string.Format("https://api.instagram.com/v1/media/{0}?fields=likes&access_token={1}", postId, accessToken);
            var userMediaIsLikedResponse = GetRequest(userMediaIsLikedUrl);
            var selfProfile = new JavaScriptSerializer().Deserialize<UserProfileResponse.RootObject>(userMediaIsLikedResponse);
            if (selfProfile != null && selfProfile.data != null)
            {
                result = selfProfile.data.user_has_liked.HasValue ? selfProfile.data.user_has_liked.Value : false;
            }
            return result;
        }

        private static string GetRequest(string url)
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
        private static async Task<string> PostRequest(string url, string postData)
        {
            var result = "";
            try
            {                
                HttpClient Client;
                Client = new HttpClient();
                Client.Timeout = TimeSpan.FromSeconds(60 * 5);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringContent content = new StringContent(postData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await Client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var res =  await response.Content.ReadAsStringAsync();
                    var feed = new JavaScriptSerializer().Deserialize<CommentsRootObject>(res);
                    result = "success-" + feed.data.id;

                }
                else
                {
                    //result = response.RequestMessage.Content.ReadAsStringAsync().Result;
                    //result = response.ToString();
                    result = "fail-" + await response.Content.ReadAsStringAsync();
                    throw new Exception(result);
                }
                return result;
            }
            catch (Exception exp)
            {
                return result;
            }
        }
        private static async Task<string> DeleteRequest(string url)
        {
            var result = "";
            try
            {
                HttpClient Client;
                Client = new HttpClient();
                Client.Timeout = TimeSpan.FromSeconds(60 * 5);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await Client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    result = "success-" + await response.Content.ReadAsStringAsync();
                }
                else
                {
                    //result = response.RequestMessage.Content.ReadAsStringAsync().Result;
                    result = response.ToString();
                    throw new Exception(result);
                }
                return result;
            }
            catch (Exception exp)
            {
                return result;
            }
        }
    }
}
