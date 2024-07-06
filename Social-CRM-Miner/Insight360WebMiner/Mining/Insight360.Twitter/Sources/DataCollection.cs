using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TweetSharp;
using Insight360.Util;
using System.Threading;
using LinqToTwitter;
using System.Threading.Tasks;
using Insight360.Common;

namespace Insight360.Sources.Twitter
{
    public class DataCollection
    {
        private const int RESULTS_PER_PAGE = 200;
        private const int RESULTS_PER_PAGE_ForMsgEvent = 10;
        private static string consumerKey = AppSettings.TwitterDetails_ConsumerKey;
        private static string consumerSecret = AppSettings.TwitterDetails_ConsumerSecret;

        public static List<BO.TwitterStatus> SearchStatuses(BO.TwitterAuthentication authentication,
            string searchTerm, long sinceId, out long maximumId, out DateTime maximumTime, BO.SearchTermInfo config, BO.Location location)
        {
            List<BO.TwitterStatus> results = new List<BO.TwitterStatus>();
            int currentPage = 1;
            long currentId = 0;
            maximumId = sinceId;
            maximumTime = new DateTime(2014, 1, 28);

            TwitterClientInfo clientInfo = new TwitterClientInfo();
            clientInfo.ConsumerKey = authentication.ConsumerKey;
            clientInfo.ConsumerSecret = authentication.ConsumerSecret;
            //  clientInfo.
            //while (results.Count < 15) //while (currentPage<2)// (results.Count<15)//!string.IsNullOrEmpty(nextPage))
            {
                TwitterService service = new TwitterService(authentication.ConsumerKey, authentication.ConsumerSecret);
                service.AuthenticateWith(authentication.Token, authentication.TokenSecret);
                SearchOptions so = new SearchOptions();

                string place = "";
                if(location != null && !String.IsNullOrEmpty(location.TwitterPlaceId))
                {
                    place = " place:" + location.TwitterPlaceId;
                    //List<TwitterPlace> places = service.GeoSearch(new GeoSearchOptions() { Query = location.name, Granularity = location.TwitterGranuality }).ToList();
                    //if (places.Count > 0)
                    //{
                    //place = " place:"+places.First().Id;
                    //}
                }

                //if place id then append otherwise it will be empty string.
                so.Q = searchTerm + place;
                so.Count = 100;
                so.Resulttype = TwitterSearchResultType.Recent;

                if (sinceId != 0)
                {
                    so.SinceId = sinceId;
                }
                //so.Count = RESULTS_PER_PAGE; // gives arthmetic error!!!
                if (currentId != 0)
                {
                    so.MaxId = (currentId > 0) ? currentId - 1 : currentId;// maximumId;
                }

                TwitterSearchResult searchResult = service.Search(so);//Twi.AsSearchResult(ts2); 
                //ListTweetsOnUserTimelineOptions options = new ListTweetsOnUserTimelineOptions();
                //options.UserId = 589921934; //487771130; 
                //var res = service.ListTweetsOnUserTimeline(options);

                if (service.Response.StatusCode.ToString() == "429")//Rate Limit Exceeded
                {
                    string a = "";
                    //  break;
                }
                else if (service.Response.StatusCode == HttpStatusCode.OK)
                {
                    //TODO: t2 to be reviewed
                    //if (service.Response.StatusCode == 403)
                    //{
                    //    break; //break the while loop
                    //}

                    if (searchResult != null && searchResult.Statuses != null)
                    {
                        bool check = false;

                        foreach (TwitterStatus ts in searchResult.Statuses)
                        {
                            //if (ts.Text.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()))
                            {
                                check = true;
                                BO.TwitterStatus obj = new BO.TwitterStatus();
                                obj.CreatedAt = ts.CreatedDate;
                                obj.Favorited = ts.IsFavorited;
                                obj.FromUser = ts.User.ScreenName;
                                obj.FromUserId = ts.User.Id.ToString();
                                obj.Geo = ts.Location == null || ts.Location.Coordinates == null ? "" : ts.Location.Coordinates.Latitude.ToString() + ", " + ts.Location.Coordinates.Longitude.ToString();
                                obj.Id = ts.Id.ToString();
                                // obj.  Parent_id = ts.RetweetedStatus != null ? ts.RetweetedStatus.Id : 0,
                                obj.IsoLanguageCode = ts.Language;

                                obj.Source = ts.Source;
                                obj.Text = ts.Text;
                                obj.ToUserId = ts.InReplyToUserId.ToString();
                                obj.ToUser = ts.InReplyToScreenName;
                                obj.RetweetCount = ts.RetweetCount;
                                obj.Sentiment = Helpers.SentimentScore(ts.Text);
                                obj.LanguageCode = Helpers.DetectLanguage(ts.Text);

                                BO.TwitterUser usr = new BO.TwitterUser();

                                usr.Location = "";
                                if (ts.Place != null)
                                {
                                    usr.Location = ts.Place.FullName;
                                }

                                if (ts.User.Location != "")
                                    usr.Location = ts.User.Location;

                                usr.CreatedAt = ts.User.CreatedDate;
                                usr.Description = ts.User.Description;
                                usr.FavouritesCount = ts.User.FavouritesCount;
                                usr.FollowersCount = ts.User.FollowersCount;
                                usr.Following = true;// ts.User.iIsFollowing,
                                usr.FriendsCount = ts.User.FriendsCount;
                                usr.GeoEnabled = ts.User.IsGeoEnabled;
                                usr.Id = ts.User.Id.ToString();
                                usr.Lang = ts.User.Language;
                                usr.Name = ts.User.Name;
                                usr.ProfileImageUrl = ts.User.ProfileImageUrl;
                                usr.Protected = ts.User.IsProtected;
                                usr.ScreenName = ts.User.ScreenName;
                                usr.StatusesCount = ts.User.StatusesCount;
                                usr.TimeZone = ts.User.TimeZone;
                                usr.Url = ts.User.Url;
                                usr.UtcOffset = ts.User.UtcOffset;
                                usr.IsVerified = ts.User.IsVerified;
                                usr.ProfileBgColor = ts.User.ProfileBackgroundColor;
                                usr.ProfileBgImageUrl = ts.User.ProfileBackgroundImageUrl;
                                usr.ProfileBgTiled = ts.User.IsProfileBackgroundTiled;
                                usr.ProfileLinkColor = ts.User.ProfileLinkColor;
                                usr.ProfileTxtColor = ts.User.ProfileTextColor;

                                obj.UserDetails = usr;

                                results.Add(obj);
                                //{

                                //    CreatedAt = ts.CreatedDate,
                                //    Favorited = ts.IsFavorited,
                                //    FromUser = ts.User.ScreenName,
                                //    FromUserId = ts.User.Id.ToString(),
                                //    Geo = ts.Location == null || ts.Location.Coordinates == null ? "" : ts.Location.Coordinates.Latitude.ToString() + ", " + ts.Location.Coordinates.Longitude.ToString(),
                                //    Id = ts.Id.ToString(),
                                // //   Parent_id = ts.RetweetedStatus != null ? ts.RetweetedStatus.Id : 0,
                                //    IsoLanguageCode = ts.Language,

                                //    Source = ts.Source,
                                //    Text = ts.Text,
                                //    ToUserId = ts.InReplyToUserId.ToString(),
                                //    ToUser = ts.InReplyToScreenName,
                                //    RetweetCount = ts.RetweetCount,
                                //    Sentiment = Helpers.SentimentScore(ts.Text),
                                //    LanguageCode = Helpers.DetectLanguage(ts.Text),
                                //    UserDetails = new BO.TwitterUser()
                                //    {
                                //        CreatedAt = ts.User.CreatedDate,
                                //        Description = ts.User.Description,
                                //        FavouritesCount = ts.User.FavouritesCount,
                                //        FollowersCount = ts.User.FollowersCount,
                                //        Following = true,// ts.User.iIsFollowing,
                                //        FriendsCount = ts.User.FriendsCount,
                                //        GeoEnabled = ts.User.IsGeoEnabled,
                                //        Id = ts.User.Id.ToString(),
                                //        Lang = ts.User.Language,
                                //        Location = ts.User.Location,
                                //        Name = ts.User.Name,
                                //        ProfileImageUrl = ts.User.ProfileImageUrl,
                                //        Protected = ts.User.IsProtected,
                                //        ScreenName = ts.User.ScreenName,
                                //        StatusesCount = ts.User.StatusesCount,
                                //        TimeZone = ts.User.TimeZone,
                                //        Url = ts.User.Url,
                                //        UtcOffset = ts.User.UtcOffset,
                                //        IsVerified = ts.User.IsVerified,
                                //        ProfileBgColor = ts.User.ProfileBackgroundColor,
                                //        ProfileBgImageUrl = ts.User.ProfileBackgroundImageUrl,
                                //        ProfileBgTiled = ts.User.IsProfileBackgroundTiled,
                                //        ProfileLinkColor = ts.User.ProfileLinkColor,
                                //        ProfileTxtColor = ts.User.ProfileTextColor,
                                //    }
                                //    //GetUser(authentication, ts.User.ScreenName.ToString())
                                //});

                                //   sinceId = ts.Id;

                                currentId = ts.Id;
                                if (ts.Id > maximumId)
                                {
                                    maximumId = ts.Id;
                                }

                                if (ts.CreatedDate > maximumTime)
                                    maximumTime = ts.CreatedDate;
                            }
                        }

                        currentPage++;
                        //   if (!check) break;
                        //   nextPage = ts.Statuses.NextPage;
                    }
                }
                else
                {
                    //string a = "";
                    //    break;
                }
            }

            return results;
        }

        public static BO.TwitterUser GetUser(BO.TwitterAuthentication authentication, string user_id)
        {
            BO.TwitterUser user = null;


            TwitterService service = new TwitterService(authentication.ConsumerKey, authentication.ConsumerSecret);
            service.AuthenticateWith(authentication.Token, authentication.TokenSecret);
            SearchForUserOptions suo = new SearchForUserOptions();
            suo.Q = user_id.ToString();

            var twUser = service.SearchForUser(suo);

            if (service.Response.StatusCode.ToString() == "429")//Rate Limit Exceeded
            {
                string a = "";

            }

            else
            {
                if (service.Response.StatusCode == HttpStatusCode.OK)
                {
                    foreach (TwitterUser tu in twUser)
                    {

                        TwitterUser twitter_user = tu;
                        //if (user_id.ToLower() == twitter_user.Name.ToLower()) //if an exact match, just return it, do not bother to process the rest of the matching users
                        //{
                        user = new BO.TwitterUser()
                        {
                            CreatedAt = twitter_user.CreatedDate,
                            Description = twitter_user.Description,
                            FavouritesCount = twitter_user.FavouritesCount,
                            FollowersCount = twitter_user.FollowersCount,
                            Following = true,// twitter_user.IsFollowing,
                            FriendsCount = twitter_user.FriendsCount,
                            GeoEnabled = twitter_user.IsGeoEnabled,
                            Id = twitter_user.Id.ToString(),
                            Lang = twitter_user.Language,
                            Location = twitter_user.Location,
                            Name = twitter_user.Name,
                            ProfileImageUrl = twitter_user.ProfileImageUrl,
                            Protected = twitter_user.IsProtected,
                            ScreenName = twitter_user.ScreenName,
                            StatusesCount = twitter_user.StatusesCount,
                            TimeZone = twitter_user.TimeZone,
                            Url = twitter_user.Url,
                            UtcOffset = twitter_user.UtcOffset,
                            IsVerified = twitter_user.IsVerified,
                            ProfileBgColor = twitter_user.ProfileBackgroundColor,
                            ProfileBgImageUrl = twitter_user.ProfileBackgroundImageUrl,
                            ProfileBgTiled = twitter_user.IsProfileBackgroundTiled,
                            ProfileLinkColor = twitter_user.ProfileLinkColor,
                            ProfileTxtColor = twitter_user.ProfileTextColor,
                        };
                        //return user;
                        //}
                    }

                }
            }

            return user;
        }

        public static BO.TwitterStatus GetDetailedStatus(BO.TwitterAuthentication authentication,
            long statusId)
        {
            BO.TwitterStatus status = new BO.TwitterStatus();
            try
            {
                TwitterClientInfo clientInfo = new TwitterClientInfo();
                clientInfo.ConsumerKey = authentication.ConsumerKey;
                clientInfo.ConsumerSecret = authentication.ConsumerSecret;

                TwitterService service = new TwitterService(authentication.ConsumerKey, authentication.ConsumerSecret);
                service.AuthenticateWith(authentication.Token, authentication.TokenSecret);
                GetTweetOptions gto = new GetTweetOptions();
                gto.Id = statusId;

                TwitterStatus ts = new TwitterStatus();
                ts = service.GetTweet(gto);

                if (service.Response.StatusCode.ToString() == "429")//Rate Limit Exceeded
                {
                    status = new BO.TwitterStatus() { SkippedDueToRateLimiting = true };
                }

                //else if (service.Response.Error.Code == 63) {//user account has been suspended
                //    status = new BO.TwitterStatus() { SkippedDueToRateLimiting = false };
                //}
                else
                {
                    if (service.Response.StatusCode == HttpStatusCode.OK)
                    {

                        //    TwitterStatus s =new TwitterStatus();
                        //    s. twitterResult.AsStatus();

                        status = new BO.TwitterStatus()
                        {
                            Id = ts.Id.ToString(),
                            //      Parent_id = ts.RetweetedStatus != null ? ts.RetweetedStatus.Id : 0,
                            Text = ts.Text,
                            CreatedAt = ts.CreatedDate,
                            Source = ts.Source,
                            Geo = ts.Location == null || ts.Location.Coordinates == null ? "" : ts.Location.Coordinates.Latitude.ToString() + ", " + ts.Location.Coordinates.Longitude.ToString(),
                            Favorited = ts.IsFavorited,
                            FromUser = ts.User.ScreenName,
                            FromUserId = ts.User.Id.ToString(),
                            ToUserId = ts.InReplyToUserId.ToString(),
                            ToUser = ts.InReplyToScreenName,
                            InReplyToScreenName = ts.InReplyToScreenName,
                            InReplyToStatusId = ts.InReplyToStatusId.ToString(),
                            InReplyToUserId = ts.InReplyToUserId.ToString(),
                            Truncated = ts.IsTruncated,
                            RetweetCount = ts.RetweetCount,
                            SkippedDueToRateLimiting = false,
                            Sentiment = Helpers.SentimentScore(ts.Text),
                            LanguageCode = Helpers.DetectLanguage(ts.Text),
                            UserDetails = new BO.TwitterUser()
                            {
                                CreatedAt = ts.User.CreatedDate,
                                Description = ts.User.Description,
                                FavouritesCount = ts.User.FavouritesCount,
                                FollowersCount = ts.User.FollowersCount,
                                Following = true,// ts.User.iIsFollowing,
                                FriendsCount = ts.User.FriendsCount,
                                GeoEnabled = ts.User.IsGeoEnabled,
                                Id = ts.User.Id.ToString(),
                                Lang = ts.User.Language,
                                Location = ts.User.Location,
                                Name = ts.User.Name,
                                ProfileImageUrl = ts.User.ProfileImageUrl,
                                Protected = ts.User.IsProtected,
                                ScreenName = ts.User.ScreenName,
                                StatusesCount = ts.User.StatusesCount,
                                TimeZone = ts.User.TimeZone,
                                Url = ts.User.Url,
                                UtcOffset = ts.User.UtcOffset,
                                IsVerified = ts.User.IsVerified,
                                ProfileBgColor = ts.User.ProfileBackgroundColor,
                                ProfileBgImageUrl = ts.User.ProfileBackgroundImageUrl,
                                ProfileBgTiled = ts.User.IsProfileBackgroundTiled,
                                ProfileLinkColor = ts.User.ProfileLinkColor,
                                ProfileTxtColor = ts.User.ProfileTextColor,
                            }
                        };
                    }
                }

                return status;
            }
            catch (Exception ex)
            {
                Util.Log.WriteException(typeof(DataCollection), "Exception in Fetching Status", ex);
                return new BO.TwitterStatus();
            }
        }


        public static List<BO.TwitterStatus> SearchStatusesFromAndTo(BO.TwitterAuthentication authentication,
            string userName, long sinceId, out long maximumId, out DateTime maximumTime)
        {
            //// Nabeel

            ////List<BO.TwitterStatus> results = new List<BO.TwitterStatus>();
            ////int currentPage = 1;

            ////maximumId = sinceId;
            ////maximumTime = new DateTime(2000, 1, 1);

            ////string nextPage = "1";
            ////TwitterService service = new TwitterService(authentication.ConsumerKey, authentication.ConsumerSecret);
            ////service.AuthenticateWith(authentication.Token, authentication.TokenSecret);
            ////// while (!string.IsNullOrEmpty(nextPage))

            ////while (true)
            ////{
            ////    TwitterResult twitterResult = FluentTwitter.CreateRequest()
            ////       //.AuthenticateAs("TRG_TS", "TRGTECH")
            ////       .AuthenticateWith(
            ////               authentication.ConsumerKey,
            ////               authentication.ConsumerSecret,
            ////               authentication.Token,
            ////               authentication.TokenSecret
            ////               )
            ////        .Search()
            ////        .Query()
            ////        .FromUser(userName)
            ////        //.ToUser(userName)
            ////        .Take(RESULTS_PER_PAGE)
            ////        .Skip(currentPage)
            ////        .Since(sinceId)
            ////        .AsJson().
            ////        .Request();
            ////    TweetSharp.ListTweetsOnListOptions option = new ListTweetsOnListOptions();
            ////    option.OwnerScreenName = userName;
            ////    option.Count = RESULTS_PER_PAGE;
            ////    option.SinceId = sinceId;





            ////    var twitterResult = service.ListTweetsOnList(option);


            //     TwitterSearchResult searchResult = TwitterExtensions.AsSearchResult(twitterResult);



            //// Nabeel
            ////    if (twitterResult != null)// && twitterResult.IsTwitterError == false && twitterResult.IsNetworkError == false)
            ////    {
            ////        foreach (TwitterStatus ts in twitterResult)
            ////        {
            ////            results.Add(new BO.TwitterStatus()
            ////            {
            ////                CreatedAt = ts.CreatedDate,
            ////                FromUser = ts.User.ScreenName,
            ////                FromUserId = ts.User.Id.ToString(),
            ////                Id = ts.Id.ToString(),
            ////                IsoLanguageCode = ts.Language,
            ////                //ProgfileImageUrl = ts.ProfileImageUrl,
            ////                Source = ts.Source,
            ////                Text = ts.Text,
            ////                ToUserId = ts.ToUserId.ToString(),
            ////                ToUser = ts.ToUserScreenName,
            ////                UserDetails = new BO.TwitterUser()
            ////                {
            ////                    Id = ts.FromUserId.ToString(),
            ////                    ScreenName = ts.FromUserScreenName,
            ////                    ProfileImageUrl = ts.ProfileImageUrl
            ////                }
            ////            });


            ////            if (ts.Id > maximumId)
            ////                maximumId = ts.Id;

            ////            if (ts.CreatedDate > maximumTime)
            ////                maximumTime = ts.CreatedDate;
            ////        }
            ////        currentPage++;
            ////        nextPage = searchResult.NextPage;

            ////    }
            ////    else
            ////    {
            ////        nextPage = "";
            ////        Util.Log.WriteOperationCompleted(typeof(DataCollection), "SearchStatusesFromAndTo", "SearchResults");
            ////        if (twitterResult.IsTwitterError || twitterResult.IsNetworkError)
            ////            return null;
            ////    }
            ////}

            maximumId = 0;
            maximumTime = DateTime.Now;

            return null;
        }

        public static List<BO.TwitterStatus> GetUserMentions(BO.TwitterAuthentication authentication, string searchTerm, long sinceId, out long maximumId, out DateTime maximumTime)
        {
            List<BO.TwitterStatus> results = new List<BO.TwitterStatus>();
            int currentPage = 1;
            long currentId = 0;
            maximumId = sinceId;
            maximumTime = new DateTime(2010, 1, 1);

            TwitterService service = new TwitterService(authentication.ConsumerKey, authentication.ConsumerSecret);
            service.AuthenticateWith(authentication.Token, authentication.TokenSecret);
            while (true)
            {
                // List option = new ListTweetsMentioningMeOptions();
                SearchOptions options = new SearchOptions();
                options.Q = searchTerm;
                options.Count = 200;
                if (sinceId != 0)
                {
                    options.SinceId = sinceId;
                }

                if (currentId != 0)
                {
                    options.MaxId = (currentId > 0) ? currentId - 1 : currentId;// maximumId;
                }



                TwitterSearchResult result = service.Search(options);
                if (service.Response.StatusCode.ToString() == "429")//Rate Limit Exceeded
                {
                    string a = "";
                    break;
                    //   SkippedDueToRateLimiting = true;

                }
                // var result = twitterResult.AsStatuses();
                else if (service.Response.StatusCode == HttpStatusCode.OK)
                {
                    if (result != null)// && twitterResult.IsTwitterError == false && twitterResult.IsNetworkError == false)
                    {
                        bool check = false;
                        foreach (var ts in result.Statuses)
                        {
                            check = true;


                            results.Add(new BO.TwitterStatus()
                            {
                                CreatedAt = ts.CreatedDate,
                                FromUser = ts.User.ScreenName,
                                FromUserId = ts.User.Id.ToString(),
                                Id = ts.Id.ToString(),
                                //   Parent_id = ts.RetweetedStatus != null ? ts.RetweetedStatus.Id : 0,
                                IsoLanguageCode = ts.User.Language,
                                //ProgfileImageUrl = ts.User.ProfileImageUrl,
                                Source = ts.Source,
                                Text = ts.Text,
                                ToUserId = ts.InReplyToUserId.ToString(),
                                ToUser = ts.InReplyToScreenName,
                                RetweetCount = ts.RetweetCount,
                                Sentiment = Helpers.SentimentScore(ts.Text),
                                LanguageCode = Helpers.DetectLanguage(ts.Text),
                                UserDetails = new BO.TwitterUser()
                                {
                                    CreatedAt = ts.User.CreatedDate,
                                    Description = ts.User.Description,
                                    FavouritesCount = ts.User.FavouritesCount,
                                    FollowersCount = ts.User.FollowersCount,
                                    Following = true,// ts.User.iIsFollowing,
                                    FriendsCount = ts.User.FriendsCount,
                                    GeoEnabled = ts.User.IsGeoEnabled,
                                    Id = ts.User.Id.ToString(),
                                    Lang = ts.User.Language,
                                    Location = ts.User.Location,
                                    Name = ts.User.Name,
                                    ProfileImageUrl = ts.User.ProfileImageUrl,
                                    Protected = ts.User.IsProtected,
                                    ScreenName = ts.User.ScreenName,
                                    StatusesCount = ts.User.StatusesCount,
                                    TimeZone = ts.User.TimeZone,
                                    Url = ts.User.Url,
                                    UtcOffset = ts.User.UtcOffset,
                                    IsVerified = ts.User.IsVerified,
                                    ProfileBgColor = ts.User.ProfileBackgroundColor,
                                    ProfileBgImageUrl = ts.User.ProfileBackgroundImageUrl,
                                    ProfileBgTiled = ts.User.IsProfileBackgroundTiled,
                                    ProfileLinkColor = ts.User.ProfileLinkColor,
                                    ProfileTxtColor = ts.User.ProfileTextColor,
                                }
                                //UserDetails = new BO.TwitterUser()
                                //{
                                //    Id = ts.User.Id.ToString(),
                                //    ScreenName = ts.User.ScreenName,
                                //    ProfileImageUrl = ts.User.ProfileImageUrl
                                //}
                            });

                            currentId = ts.Id;
                            if (ts.Id > maximumId)
                                maximumId = ts.Id;

                            if (ts.CreatedDate > maximumTime)
                                maximumTime = ts.CreatedDate;
                        }
                        currentPage++;
                        if (!check) break;


                    }
                }
                else
                {
                    string a = "";
                    break;
                }
            }

            return results;
        }

        public static List<BO.TwitterStatus> GetUserTimeline(BO.TwitterAuthentication authentication, string searchTerm, string user, long sinceId, out long maximumId, out DateTime maximumTime)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollection), "GetUserTimeline", "Fetching User Timeline");

            List<BO.TwitterStatus> results = new List<BO.TwitterStatus>();
            int currentPage = 1;
            long currentId = 0;
            maximumId = sinceId;
            maximumTime = new DateTime(2013, 11, 1);

            TwitterService service = new TwitterService(authentication.ConsumerKey, authentication.ConsumerSecret);
            service.AuthenticateWith(authentication.Token, authentication.TokenSecret);

            while (true)
            {
                ListTweetsOnUserTimelineOptions option = new ListTweetsOnUserTimelineOptions();
                option.ScreenName = user;
                //  option.
                // option.SinceId = sinceId;
                option.Count = RESULTS_PER_PAGE;

                if (sinceId != 0)
                {
                    option.SinceId = sinceId;
                }

                if (currentId != 0)
                {
                    option.MaxId = (currentId > 0) ? currentId - 1 : currentId;// maximumId;
                }

                List<TwitterStatus> result = service.ListTweetsOnUserTimeline(option).ToList();// twitterResult.AsStatuses();

                if (service.Response.StatusCode == HttpStatusCode.OK)
                {
                    if (result != null)//&& twitterResult.IsTwitterError == false && twitterResult.IsNetworkError == false)
                    {
                        Util.Log.WriteOperationStarted(typeof(DataCollection), "GetUserTimeline", "New results Count" + result.Count);

                        bool check = false;
                        foreach (var ts in result)
                        {
                            if (ts.Text.ToLowerInvariant().Contains(searchTerm))
                            {
                                check = true;
                                if (ts.CreatedDate <= maximumTime)
                                {
                                    check = false;
                                    break;
                                }

                                results.Add(new BO.TwitterStatus()
                                {
                                    CreatedAt = ts.CreatedDate,
                                    FromUser = ts.User.ScreenName,
                                    FromUserId = ts.User.Id.ToString(),
                                    Id = ts.Id.ToString(),
                                    //     Parent_id = ts.RetweetedStatus != null ? ts.RetweetedStatus.Id : 0,
                                    IsoLanguageCode = ts.User.Language,
                                    //ProgfileImageUrl = ts.User.ProfileImageUrl,
                                    Source = ts.Source,
                                    Text = ts.Text,
                                    ToUserId = ts.InReplyToUserId.ToString(),
                                    ToUser = ts.InReplyToScreenName,
                                    RetweetCount = ts.RetweetCount,

                                    UserDetails = new BO.TwitterUser()
                                    {
                                        CreatedAt = ts.User.CreatedDate,
                                        Description = ts.User.Description,
                                        FavouritesCount = ts.User.FavouritesCount,
                                        FollowersCount = ts.User.FollowersCount,
                                        Following = true,// ts.User.iIsFollowing,
                                        FriendsCount = ts.User.FriendsCount,
                                        GeoEnabled = ts.User.IsGeoEnabled,
                                        Id = ts.User.Id.ToString(),
                                        Lang = ts.User.Language,
                                        Location = ts.User.Location,
                                        Name = ts.User.Name,
                                        ProfileImageUrl = ts.User.ProfileImageUrl,
                                        Protected = ts.User.IsProtected,
                                        ScreenName = ts.User.ScreenName,
                                        StatusesCount = ts.User.StatusesCount,
                                        TimeZone = ts.User.TimeZone,
                                        Url = ts.User.Url,
                                        UtcOffset = ts.User.UtcOffset,
                                        IsVerified = ts.User.IsVerified,
                                        ProfileBgColor = ts.User.ProfileBackgroundColor,
                                        ProfileBgImageUrl = ts.User.ProfileBackgroundImageUrl,
                                        ProfileBgTiled = ts.User.IsProfileBackgroundTiled,
                                        ProfileLinkColor = ts.User.ProfileLinkColor,
                                        ProfileTxtColor = ts.User.ProfileTextColor,
                                    }
                                });

                                currentId = ts.Id;
                            }

                            if (ts.Id > maximumId)
                                maximumId = ts.Id;
                        }

                        Util.Log.WriteOperationStarted(typeof(DataCollection), "GetUserTimeline", "Total results Count" + results.Count);

                        currentPage++;
                        if (!check) break;
                    }
                }
                else
                {
                    string a = "";
                    break;
                }
            }

            return results;
        }

        public static List<BO.TwitterTweet> GetTweetsNewWay(ref ulong sinceId, string token, string tokenSecret, string TweetType, string userId)
        {
            List<BO.TwitterTweet> results = new List<BO.TwitterTweet>();
            int currentPage = 1;
            ulong currentId = 0;

            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    AccessToken = token,
                    AccessTokenSecret = tokenSecret
                }
            };
            var twitterCtx = new TwitterContext(auth);

            while (true)
            {
                try
                {
                    StatusType type;
                    if (TweetType == "user")
                    {
                        type = StatusType.User;
                    }
                    else if (TweetType == "mention")
                    {
                        type = StatusType.Mentions;
                    }
                    else{
                        type = StatusType.Home;
                    }
                    ulong sinceID = sinceId;
                    ulong MaxID = (currentId > 0) ? currentId - 1 : currentId;

                    Task<List<Status>> result;
                
                        if (MaxID != 0)
                        {
                            if (sinceID != 0)
                            {
                                result = (from tweet in twitterCtx.Status
                                          where tweet.Type == type &&
                                          tweet.Count == RESULTS_PER_PAGE && 
                                          tweet.SinceID == sinceID &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.MaxID == MaxID
                                          select tweet)
                                          .ToListAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.Status
                                          where tweet.Type == type &&
                                          tweet.Count == 1 &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.MaxID == MaxID
                                          select tweet)
                                          .ToListAsync();
                            }
                        }
                        else
                        {
                            if (sinceID != 0)
                            {
                                result = (from tweet in twitterCtx.Status
                                          where tweet.Type == type &&
                                          tweet.Count == RESULTS_PER_PAGE &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.SinceID == sinceID
                                          select tweet)
                                          .ToListAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.Status
                                          where tweet.Type == type &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.Count == 1
                                          select tweet)
                                          .ToListAsync();
                            }
                        }

                    if (result == null){
                        break;
                    }
                    else if (result.Result.Count == 0)
                    {
                        break;
                    }
                    else{
                        int count = 0;
                        foreach (var ts in result.Result)
                        {
                            //int offset = ts.User.UtcOffset/3600;
                            string profilePic = ts.User.ProfileImageUrlHttps.Replace("normal", "reasonably_small");
                            results.Add(new BO.TwitterTweet()
                            {
                                StatusId = ts.StatusID.ToString(),
                                AuthorUserId = ts.User.UserIDResponse,
                                StatusText = ts.FullText,
                                AuthorName = ts.User.ScreenNameResponse,
                                AuthorFullName = ts.User.Name,
                                InReplyToStatusId = ts.InReplyToStatusID.ToString(),
                                InReplyToUserId = ts.InReplyToUserID.ToString(),
                                InReplyToScreenName = ts.InReplyToScreenName,
                                CreatedDate = ts.CreatedAt.ToLocalTime(),
                                AuthorProfilePic = profilePic,
                                IsAuthorVerified = ts.User.Verified,
                                IsMention = ts.Entities.UserMentionEntities.Any(USE => USE.Id.ToString() == userId),
                                IsRetweet = (ts.RetweetedStatus.StatusID == 0) ? false : true,
                                AuthorFollowers = ts.User.FollowersCount
                            });
                            bool isRetweetedStatus = ts.QuotedStatusID > 0;

                            if (isRetweetedStatus)
                            {
                                var status = ts.QuotedStatus.ExtendedEntities.MediaEntities;
                                if (status != null && status.Count > 0)
                                {
                                    results[count].MediaAttachment = new List<BO.FeedMediaAttachment>();
                                    foreach (var item in status)
                                    {
                                        if (item.Type == "photo")
                                        {
                                            results[count].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "TP",
                                                Description = item.Type,
                                                AttachmentUrl = item.MediaUrlHttps
                                            });
                                        }
                                        else if (item.Type == "video")
                                        {
                                            results[count].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "TP",
                                                Description = item.Type,
                                                AttachmentUrl = item.VideoInfo.Variants.Where(x => x.ContentType.Contains("mp4")).FirstOrDefault().Url
                                            });
                                        }
                                        else if (item.Type == "animated_gif")
                                        {
                                            results[count].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "TP",
                                                Description = item.Type,
                                                AttachmentUrl = item.VideoInfo.Variants.Where(x => x.ContentType.Contains("mp4")).FirstOrDefault().Url
                                            });
                                        }
                                    } 
                                }
                            }
                            else
                            {
                                if (ts.ExtendedEntities.MediaEntities != null && ts.ExtendedEntities.MediaEntities.Count > 0)
                                {
                                    results[count].MediaAttachment = new List<BO.FeedMediaAttachment>();
                                    foreach (var item in ts.ExtendedEntities.MediaEntities)
                                    {
                                        if (item.Type == "photo")
                                        {
                                            results[count].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "TP",
                                                Description = item.Type,
                                                AttachmentUrl = item.MediaUrlHttps
                                            });
                                        }
                                        else if (item.Type == "video")
                                        {
                                            results[count].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "TP",
                                                Description = item.Type,
                                                AttachmentUrl = item.VideoInfo.Variants.Where(x => x.ContentType.Contains("mp4")).FirstOrDefault().Url
                                            });
                                        }
                                        else if (item.Type == "animated_gif")
                                        {
                                            results[count].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                            {
                                                MediaType = "TP",
                                                Description = item.Type,
                                                AttachmentUrl = item.VideoInfo.Variants.Where(x => x.ContentType.Contains("mp4")).FirstOrDefault().Url
                                            });
                                        }
                                    }
                                }
                                else if (ts.Entities.UrlEntities != null && ts.Entities.UrlEntities.Count > 0)
                                {
                                    results[count].MediaAttachment = new List<BO.FeedMediaAttachment>();
                                    foreach (var item in ts.Entities.UrlEntities)
                                    {
                                        results[count].MediaAttachment.Add(new BO.FeedMediaAttachment()
                                        {
                                            MediaType = "TP",
                                            Description = "share",
                                            AttachmentUrl = item.ExpandedUrl
                                        });
                                    }
                                }
                            }
                            
                            count++;
                            currentId = ts.StatusID;
                        }

                        currentPage++;
                    }

                    if (sinceId == 0)
                        break;
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(ex.GetType(), "GetUserTimeline", ex);
                    break;
                }
            }
            if(results.Count > 0)
                sinceId = ulong.Parse(results[0].StatusId.ToString());

            return results;
        }

        public static List<BO.TwitterTweet> SearchTweetsNewWay(ref ulong sinceId, string token, string tokenSecret, string userId, BO.SearchKeyword keyword)
        {
            List<BO.TwitterTweet> results = new List<BO.TwitterTweet>();
            int currentPage = 1;
            ulong currentId = 0;

            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    AccessToken = token,
                    AccessTokenSecret = tokenSecret
                }
            };
            var twitterCtx = new TwitterContext(auth);
            var geocode = keyword.LocationLat + "," + keyword.LocationLng + "," + "25mi";

            while (true)
            {
                try
                {
                    ulong sinceID = sinceId;
                    ulong MaxID = (currentId > 0) ? currentId - 1 : currentId;

                    Task<Search> result;

                    if (MaxID != 0)
                    {
                        if (sinceID != 0)
                        {
                            if(keyword.LocationName == "0")
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.Count == RESULTS_PER_PAGE &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.SinceID == sinceID &&
                                          tweet.MaxID == MaxID
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.GeoCode == geocode &&
                                          tweet.Count == RESULTS_PER_PAGE &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.SinceID == sinceID &&
                                          tweet.MaxID == MaxID
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                        }
                        else
                        {
                            if (keyword.LocationName == "0")
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.Count == 1 &&
                                          tweet.MaxID == MaxID
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.GeoCode == geocode &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.Count == 1 &&
                                          tweet.MaxID == MaxID
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                        }
                    }
                    else
                    {
                        if (sinceID != 0)
                        {
                            if (keyword.LocationName == "0")
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.Count == RESULTS_PER_PAGE &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.SinceID == sinceID
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.GeoCode == geocode &&
                                          tweet.Count == RESULTS_PER_PAGE &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.SinceID == sinceID
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                        }
                        else
                        {
                            if (keyword.LocationName == "0")
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.Count == 1
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.Search
                                          where tweet.Type == SearchType.Search &&
                                          tweet.Query == keyword.KeywordText &&
                                          tweet.TweetMode == TweetMode.Extended &&
                                          tweet.GeoCode == geocode &&
                                          tweet.Count == 1
                                          select tweet)
                                      .SingleOrDefaultAsync();
                            }
                        }
                    }

                    if (result.Result == null)
                    {
                        break;
                    }
                    else if (result.Result.Statuses.Count == 0)
                    {
                        break;
                    }
                    else {
                        foreach (var ts in result.Result.Statuses)
                        {
                            //int offset = ts.User.UtcOffset/3600;
                            string profilePic = ts.User.ProfileImageUrlHttps.Replace("normal", "reasonably_small");
                            results.Add(new BO.TwitterTweet()
                            {
                                StatusId = ts.StatusID.ToString(),
                                AuthorUserId = ts.User.UserIDResponse,
                                StatusText = ts.FullText,
                                AuthorName = ts.User.ScreenNameResponse,
                                AuthorFullName = ts.User.Name,
                                InReplyToStatusId = ts.InReplyToStatusID.ToString(),
                                InReplyToUserId = ts.InReplyToUserID.ToString(),
                                InReplyToScreenName = ts.InReplyToScreenName,
                                CreatedDate = ts.CreatedAt.ToLocalTime(),
                                AuthorProfilePic = profilePic,
                                IsAuthorVerified = ts.User.Verified,
                                IsMention = ts.Entities.UserMentionEntities.Any(USE => USE.Id.ToString() == userId),
                                IsRetweet = (ts.RetweetedStatus.StatusID == 0) ? false : true,
                                AuthorFollowers = ts.User.FollowersCount
                            });
                            currentId = ts.StatusID;
                        }

                        currentPage++;
                    }

                    if (sinceId == 0)
                        break;
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(ex.GetType(), "SearchTweetsNewWay", ex);
                    break;
                }
            }
            if (results.Count > 0)
                sinceId = ulong.Parse(results[0].StatusId.ToString());

            return results;
        }

        public static List<BO.TwitterMessage> GetMsgsNewWay(ref ulong sinceId, string token, string tokenSecret, string msgType)
        {
            List<BO.TwitterMessage> results = new List<BO.TwitterMessage>();
            int currentPage = 1;
            ulong currentId = 0;

            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    AccessToken = token,
                    AccessTokenSecret = tokenSecret
                }
            };
            var twitterCtx = new TwitterContext(auth);

            while (true)
            {
                try {
                    DirectMessageType type;
                    if (msgType == "sentBy")
                    {
                        type = DirectMessageType.SentBy;
                    }
                    else
                    {
                        type = DirectMessageType.SentTo;
                    }
                    ulong sinceID = sinceId;
                    ulong MaxID = (currentId > 0) ? currentId - 1 : currentId;

                    Task<List<DirectMessage>> result;
                
                        if (MaxID != 0)
                        {
                            if (sinceID != 0)
                            {
                                result = (from tweet in twitterCtx.DirectMessage
                                          where tweet.Type == type &&
                                          tweet.Count == RESULTS_PER_PAGE && 
                                          tweet.SinceID == sinceID &&
                                          tweet.MaxID == MaxID &&
                                          tweet.FullText == true
                                          select tweet)
                                          .ToListAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.DirectMessage
                                          where tweet.Type == type &&
                                          tweet.Count == 1 &&
                                          tweet.MaxID == MaxID &&
                                          tweet.FullText == true
                                          select tweet)
                                         .ToListAsync();
                            }
                        }
                        else
                        {
                            if (sinceID != 0)
                            {
                                result = (from tweet in twitterCtx.DirectMessage
                                          where tweet.Type == type &&
                                          tweet.Count == RESULTS_PER_PAGE && 
                                          tweet.SinceID == sinceID &&
                                          tweet.FullText == true
                                          select tweet)
                                          .ToListAsync();
                            }
                            else
                            {
                                result = (from tweet in twitterCtx.DirectMessage
                                          where tweet.Type == type &&
                                          tweet.Count == 1 &&
                                          tweet.FullText == true
                                          select tweet)
                                          .ToListAsync();
                            }
                        }

                    if (result == null)
                    {
                        break;
                    }
                    else if (result.Result.Count == 0)
                    {
                        break;
                    }
                    else {
                        foreach (var ts in result.Result)
                        {
                            //int offset = ts.Sender.UtcOffset / 3600;
                            string profilePic = ts.Sender.ProfileImageUrlHttps.Replace("normal", "reasonably_small");

                            results.Add(new BO.TwitterMessage()
                            {
                                MsgId = ts.IDString,
                                RecipientId = ts.RecipientID.ToString(),
                                RecipientScreenName = ts.RecipientScreenName,
                                RecipientFullName = ts.Recipient.Name,
                                SenderId = ts.SenderID.ToString(),
                                SenderScreenName = ts.SenderScreenName,
                                SenderFullName = ts.Sender.Name,
                                MsgText = ts.Text,
                                CreatedDate = ts.CreatedAt.ToLocalTime(),
                                SenderProfilePic = profilePic,
                                IsSenderVerified = ts.Sender.Verified,
                                Attachment = (ts.Entities.MediaEntities != null && ts.Entities.MediaEntities.Count > 0) ? ((ts.Entities.MediaEntities[0].Type == "photo") ? ts.Entities.MediaEntities[0].MediaUrlHttps : (ts.Entities.MediaEntities[0].VideoInfo.Variants != null && ts.Entities.MediaEntities[0].VideoInfo.Variants.Count > 0) ?  ts.Entities.MediaEntities[0].VideoInfo.Variants.Where(x => x.ContentType.Contains("mp4")).FirstOrDefault().Url : string.Empty) : string.Empty,
                                AttachmentType = (ts.Entities.MediaEntities != null && ts.Entities.MediaEntities.Count > 0) ? ts.Entities.MediaEntities[0].Type : string.Empty,
                                SenderFollowers = ts.Sender.FollowersCount
                            });

                            currentId = ts.IDResponse;
                        }

                        currentPage++;
                    }

                    if (sinceId == 0)
                        break;
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(ex.GetType(), "GetUserMessages", ex);
                    break;
                }
            }
            if (results.Count > 0)
                sinceId = ulong.Parse(results[0].MsgId.ToString());

            return results;
        }

        public static List<BO.TwitterMessage> GetMessageEvents(ref string cursor, string token, string tokenSecret)
        {
            List<BO.TwitterMessage> results = new List<BO.TwitterMessage>();
            string lastCursor = string.Empty;
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    AccessToken = token,
                    AccessTokenSecret = tokenSecret
                }
            };
            var twitterCtx = new TwitterContext(auth);
            List<DMEvent> allDmEvents = new List<DMEvent>();
            DirectMessageEvents dmResponse=null;
            var cursorLastValue = cursor;

            //For rate limit issue we only get first 20 msg per call
            dmResponse = (
                from dm in twitterCtx.DirectMessageEvents
                where dm.Type == DirectMessageEventsType.List && dm.Count == RESULTS_PER_PAGE_ForMsgEvent
                select dm
            ).SingleOrDefault();
            allDmEvents.AddRange(dmResponse.Value.DMEvents);
            cursorLastValue = cursor = dmResponse.Value.NextCursor;

            /*
            //for very first time 
            if (string.IsNullOrEmpty(cursor))
            {
                dmResponse = (
                     from dm in twitterCtx.DirectMessageEvents
                     where dm.Type == DirectMessageEventsType.List && dm.Count == RESULTS_PER_PAGE_ForMsgEvent
                     select dm 
                     ).SingleOrDefault();
                allDmEvents.AddRange(dmResponse.Value.DMEvents);
                cursorLastValue=cursor = dmResponse.Value.NextCursor;

                while (!string.IsNullOrWhiteSpace(cursorLastValue))
                {
                    dmResponse =
                        (from dm in twitterCtx.DirectMessageEvents
                            where dm.Type == DirectMessageEventsType.List &&
                                  dm.Count == RESULTS_PER_PAGE_ForMsgEvent &&
                                  dm.Cursor == cursorLastValue
                            select dm).SingleOrDefault();
                    allDmEvents.AddRange(dmResponse.Value.DMEvents);
                    if (!string.IsNullOrEmpty(dmResponse.Value.NextCursor))
                    {
                        cursorLastValue = cursor = dmResponse.Value.NextCursor;
                    }
                    else
                    {
                        cursorLastValue = dmResponse.Value.NextCursor;
                    }
                }
            }
            else
            {
                while (!string.IsNullOrWhiteSpace(cursorLastValue))
                {
                    dmResponse =
                        (from dm in twitterCtx.DirectMessageEvents
                         where dm.Type == DirectMessageEventsType.List &&
                                  dm.Count == RESULTS_PER_PAGE_ForMsgEvent &&
                                  dm.Cursor == cursorLastValue
                            select dm).SingleOrDefault();
                    allDmEvents.AddRange(dmResponse.Value.DMEvents);
                    if (!string.IsNullOrEmpty(dmResponse.Value.NextCursor))
                    {
                        cursorLastValue = cursor = dmResponse.Value.NextCursor;
                    }
                    else
                    {
                        cursorLastValue = dmResponse.Value.NextCursor;
                    }
                }
            }
            */
            var hashsetUserIds = new HashSet<ulong>();
            allDmEvents.ForEach(ts =>
            {
                hashsetUserIds.Add(ulong.Parse(ts.MessageCreate.SenderID));
                hashsetUserIds.Add(ulong.Parse(ts.MessageCreate.Target.RecipientID));
            });

            List<User> twitterUsers = new List<User>();
            foreach (var twitterUserId in hashsetUserIds)
            {
                var user = twitterCtx.User.Where(x => x.UserID == twitterUserId && x.Type == UserType.Show)
                    .FirstOrDefault();
                if(user!=null)
                    twitterUsers.Add(user);
            }

            allDmEvents.ForEach(ts =>
            {
                var sender = twitterUsers.Where(s => s.UserID == ulong.Parse(ts.MessageCreate.SenderID)).FirstOrDefault();
                var receiver = twitterUsers.Where(s => s.UserID == ulong.Parse(ts.MessageCreate.Target.RecipientID)).FirstOrDefault();

                results.Add(new BO.TwitterMessage
                {
                    MsgId = ts.ID,
                    RecipientId = ts.MessageCreate.Target.RecipientID,
                    RecipientScreenName = string.IsNullOrEmpty(receiver.ScreenNameResponse) ? receiver.UserID.ToString() : receiver.ScreenNameResponse,
                    RecipientFullName = receiver.Name,
                    SenderId = ts.MessageCreate.SenderID,
                    SenderScreenName = string.IsNullOrEmpty(sender.ScreenNameResponse) ?sender.UserID.ToString():sender.ScreenNameResponse,
                    SenderFullName = sender.Name,
                    MsgText = ts.MessageCreate.MessageData.Text,
                    CreatedDate = ts.CreatedAt.ToLocalTime(),
                    SenderProfilePic = sender.ProfileImageUrl,
                    IsSenderVerified = sender.Verified,
                    Attachment = (ts.MessageCreate.MessageData.Entities.MediaEntities != null && ts.MessageCreate.MessageData.Entities.MediaEntities.Count > 0) ? ((ts.MessageCreate.MessageData.Entities.MediaEntities[0].Type == "photo") ? ts.MessageCreate.MessageData.Entities.MediaEntities[0].MediaUrlHttps : (ts.MessageCreate.MessageData.Entities.MediaEntities[0].VideoInfo.Variants != null && ts.MessageCreate.MessageData.Entities.MediaEntities[0].VideoInfo.Variants.Count > 0) ? ts.MessageCreate.MessageData.Entities.MediaEntities[0].VideoInfo.Variants.Where(x => x.ContentType.Contains("mp4")).FirstOrDefault().Url : string.Empty) : string.Empty,
                    AttachmentType = (ts.MessageCreate.MessageData.Entities.MediaEntities != null && ts.MessageCreate.MessageData.Entities.MediaEntities.Count > 0) ? ts.MessageCreate.MessageData.Entities.MediaEntities[0].Type : string.Empty,
                    SenderFollowers = sender.FollowersCount
                });
            });
            return results;
        }
    }
}
