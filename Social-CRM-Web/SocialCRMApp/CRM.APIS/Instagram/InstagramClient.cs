using CRM.APIS.Instagram.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CRM.APIS.Instagram
{
    public class InstagramClient
    {
        public AccountResult FeatchReport(string startDate, string endDate, string accessTokens)
        {
            var result = new AccountResult();
            var startDat = string.IsNullOrEmpty(startDate) ? DateTime.Now.AddDays(-90) : DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var endDat = string.IsNullOrEmpty(endDate) ? DateTime.Now : DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var min_timestamp = ConvertToTimestamp(startDat);
            var max_timestamp = ConvertToTimestamp(endDat);
            //Live Account acces token For testing :::3137705600.1fb234f.a5f1e00dde2d48ffac4362e2df750ccc
           // accessTokens = "3137705600.1fb234f.a5f1e00dde2d48ffac4362e2df750ccc";
            var userSelfProfileUrl = string.Format("{0}{1}", "https://api.instagram.com/v1/users/self/?access_token=", accessTokens);
           // var userSelfFeedUrl = string.Format("https://api.instagram.com/v1/users/self/feed?min_timestamp={0}&max_timestamp={1}&access_token={2}", min_timestamp, max_timestamp, accessTokens);
            var userSelfMediaUrl = string.Format("https://api.instagram.com/v1/users/self/media/recent?access_token={0}", accessTokens);
            //foreach accesstoken
            //var url = string.Format("https://api.instagram.com/v1/users/self/feed?min_timestamp={0}&max_timestamp={1}&access_token=", min_timestamp, max_timestamp);
            // foreach (var token in accessTokens)
            // {
            //accessTokens = "3137705600.1fb234f.a5f1e00dde2d48ffac4362e2df750ccc";

            /*This is Not used info already fethc in userSelfMediaUrl */
            //var profileInfoResponse = GetRequest(userSelfProfileUrl);
            var userSelfProfileResponse = GetRequest(userSelfProfileUrl);
            var userSelfMediaResponse = GetRequest(userSelfMediaUrl);
            //var profile = new JavaScriptSerializer().Deserialize<UserProfileResponse.RootObject>(profileInfoResponse);
            var selfFeed = new JavaScriptSerializer().Deserialize<UserFeedResponse.RootObject>(userSelfProfileResponse);
            var recentMedia = new JavaScriptSerializer().Deserialize<UserRecentMediaResponse.RootObject>(userSelfMediaResponse);
            var instagramPost = selfFeed.data.FirstOrDefault();
            if (instagramPost != null)
            {
                /*result.TopPost = new TopInstagramPost
                {
                    TotalFollowed = profile.data.counts.followed_by,
                    TotalFollowers = profile.data.counts.follows,
                    TotalFollowersGained = 1,
                    TotalFollowerIncreasedBy = 0,
                    InstagramPost = new List<InstagramPost>()
                };
                result.StatsByProfiles.Add(new ProfileStatics
                {
                    ProfileName = profile.data.full_name,
                    ProfilePicUrl = profile.data.profile_picture,
                    TotalFollowers = profile.data.counts.follows
                });*/
            }

            //  }
            return result;
        }
        private string GetRequest(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                string json = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    json = reader.ReadToEnd();
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
        private  string ConvertToTimestamp(DateTime value)
        {
            /*TimeZoneInfo NYTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime NyTime = TimeZoneInfo.ConvertTime(value, NYTimeZone);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            System.Globalization.DaylightTime dst = localZone.GetDaylightChanges(NyTime.Year);
            NyTime = NyTime.AddHours(-1);
            */
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0,DateTimeKind.Local).ToLocalTime();
            TimeSpan span = (value - epoch);
            return ((long)Convert.ToDouble(span.TotalSeconds)).ToString();
        }
    }
}
