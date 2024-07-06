using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Threading;
using Facebook;
using System.Configuration;
using Insight360.Common;
using System.Diagnostics;
using Insight360.BO;
namespace Insight360.FacebookLib
{
    class FacebookClient
    {
        ManualResetEvent stopEvent;

        public FacebookClient(ManualResetEvent stopEvent)
        {
            this.stopEvent = stopEvent;
        }

        // BO.SearchTermInfo config, 
        public DateTime FetchPostsAndPersist(BO.DocumentSourceInfo documentSource)
        {
            bool exit = false;
            exit = stopEvent.WaitOne(1);

            if (exit)
                return DateTime.Now;

            try
            {
                Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist", "Miner started for Source: " + documentSource.DisplayName);

                string fbFields = "?fields=id,message,icon,link,name,type,targeting,picture,caption,description,created_time,updated_time,from,shares,comments.limit(0).summary(true), likes.limit(0).summary(true)";
                DateTime maximumTime = documentSource.SearchTerms.Min(x => x.LastFetchTime);
                DateTime? untilTime = null;

                //if (AppSettings.UntilHoursCount != 0 && (DateTime.Now - config.LastFetchTime.AddHours(AppSettings.UntilHoursCount)).TotalHours > AppSettings.UntilHoursCount)
                //    untilTime = config.LastFetchTime.AddHours(AppSettings.UntilHoursCount);

                Dictionary<int, List<JsonObject>> searchTermChannels = new Dictionary<int, List<JsonObject>>();
                //List<JsonObject> channels = new List<JsonObject>();


                Dictionary<string, List<int>> channels = new Dictionary<string, List<int>>();

                // Add All channels to single dictionary
                foreach (var item in documentSource.SearchTerms)
                {
                    var result = Insight360.Sources.Facebook.DataCollection.GetAllChannels(documentSource, item);

                    foreach (var channelObject in result)
                    {
                        string channelKey = channelObject;
                        if (channels.Keys.Contains(channelKey))
                        {
                            channels[channelKey].Add(item.Id);
                        }
                        else
                        {
                            channels.Add(channelKey, new List<int>() { item.Id });
                        }
                    }
                }

                Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist", "Total Channels to Crawl: " + channels.Count);
                string accessToken = accessToken = Insight360.Sources.Facebook.DataCollection.GetToken();

                foreach (string channel in channels.Keys)
                {
                    //bool IsLocationIncluded = channel.Keys.Contains("location") && ((channel["location"] as JsonObject).Keys.Contains("city") || (channel["location"] as JsonObject).Keys.Contains("country"));
                    string channelPath = channel + "/feed" + fbFields;

                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    Dictionary<string, object> Pparams = new Dictionary<string, object>();

                    parameters.Add("since", maximumTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    parameters.Add("limit", "250");
                    bool nextPageAvailable = true;

                    Pparams = parameters;

                    Stopwatch watch = Stopwatch.StartNew();

                    while (nextPageAvailable)
                    {
                        parameters = Pparams;
                        List<JsonObject> feeds = Insight360.Sources.Facebook.DataCollection.GetPagedItems(channelPath, accessToken, parameters, maximumTime, untilTime, out nextPageAvailable, out Pparams);

                        var Posts = new List<BO.FacebookPost>();
                        int feedCount = feeds.Count;

                        if (feedCount > 0)
                        {
                            for (int i = 0; i < feedCount; i++)
                            {
                                var fbMessage = feeds[i].Keys.Contains("message") ? feeds[i]["message"].ToString().ToLowerInvariant() : "";
                                var fbPostDate = Insight360.Sources.Facebook.DataCollection.GetDateValue(feeds[i], "created_time");
                                var termWithinDateRange = documentSource.SearchTerms.Where(x => channels[channel].Contains(x.Id) && x.LastFetchTime < fbPostDate);

                                if (termWithinDateRange.Any(x => fbMessage.Contains(x.SearchTerm.ToLowerInvariant())))
                                {
                                    //Adding Channel location so that it takes it to the database
                                    //if (IsLocationIncluded)
                                    //{
                                    //    feeds[i].Add("location", channel["location"]);
                                    //}

                                    var post = Insight360.Sources.Facebook.DataCollection.CreateLocalPost(feeds[i], accessToken, AppSettings.GetCommentsAndLikes, maximumTime, out maximumTime);

                                    foreach (var term in termWithinDateRange)
                                    {
                                        if (fbMessage.Contains(term.SearchTerm.ToLowerInvariant()))
                                        {
                                            if (post != null)
                                            {
                                                post.Staging.Followup = false; //(config.Type == Insight360.BO.SearchTermInfo.Types.FacebookEngagementUser);
                                                //post.Staging.OrgName = organizationName;
                                                post.Staging.DocumentSourceId = documentSource.DocumentSourceId;
                                                post.Staging.SearchTermId = term.Id;

                                                Posts.Add(post);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist", "No Term matched for Channel Id " + channel);

                        if (untilTime != null)
                            maximumTime = untilTime.Value > maximumTime ? untilTime.Value : maximumTime;

                        if (Posts == null || Posts.Count == 0)
                        {
                            //DAL.ConfigDAL.UpdateDocumentSourceConfiguration(config, info.DocumentSourceId);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(PersistPosts, new object[] { Posts, documentSource.DocumentSourceId, documentSource });

                            //Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist",
                            //    "Queued Persisting Posts " + Posts.Count + " searchterm=" + config.SearchTerm);
                        }

                        //Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist", config.Id + "-" + config.SearchTerm + "Execution Time: " + watch.Elapsed.Minutes);

                    }
                }

                foreach (var term in documentSource.SearchTerms)
                {
                    term.LastFetchTime = maximumTime;
                    DAL.ConfigDAL.UpdateDocumentSourceConfiguration(term, documentSource.DocumentSourceId);
                }

                //config.LastFetchTime = maximumTime;


                Util.Log.WriteOperationCompleted(this.GetType(), "FetchPostsAndPersist", "Process completed for Source");
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(this.GetType(), "FetchPostsAndPersist " + config.SearchTerm, ex);
            }

            return DateTime.Now;
        }

        //public DateTime FetchPostsAndPersist(BO.SearchTermInfo config, BO.DocumentSourceInfo info)
        //{
        //    bool exit = false;
        //    exit = stopEvent.WaitOne(1);

        //    if (exit)
        //        return config.LastFetchTime;

        //    try
        //    {
        //        string fbFields = "?fields=id,message,icon,link,name,type,targeting,picture,caption,description,created_time,updated_time,from,shares,comments.limit(0).summary(true), likes.limit(0).summary(true)";

        //        Dictionary<string, object> parameters = Insight360.Sources.Facebook.DataCollection.GetParameters(config.SearchTerm, config.LastFetchTime, info);
        //        string accessToken = Insight360.Sources.Facebook.DataCollection.GetToken();

        //        DateTime maximumTime = config.LastFetchTime;
        //        List<BO.FacebookPost> posts = null;
        //        DateTime? untilTime = null;

        //        List<string> lstChannels = new List<string>();

        //        if (AppSettings.UntilHoursCount != 0 && (DateTime.Now - config.LastFetchTime.AddHours(AppSettings.UntilHoursCount)).TotalHours > AppSettings.UntilHoursCount)
        //            untilTime = config.LastFetchTime.AddHours(AppSettings.UntilHoursCount);

        //        Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist",
        //            config.Id + "-" + config.SearchTerm + "-" + config.Type + "-" + config.LastFetchTime + "-" + untilTime);

        //        if (config.SearchTerm.StartsWith("#"))
        //        {
        //            config.SearchTerm = config.SearchTerm.Replace("#", "%23");
        //        }


        //        if ((BO.FaceBookProfileTypes)info.FacebookProfileType != BO.FaceBookProfileTypes.FacebookChannel)
        //        {
        //            // Gather channels here into a single collection
        //            Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist", config.Id + "-" + config.SearchTerm + "Getting all Pages.");

        //            string path = "search";
        //            List<JsonObject> channels = Insight360.Sources.Facebook.DataCollection.GetAllChannels(path, accessToken, parameters, config.LastFetchTime, untilTime);

        //            if (channels != null && channels.Count > 0)
        //            {
        //                lstChannels.AddRange(channels.Select(x => x["id"].ToString() + "/feed" + fbFields));
        //            }
        //        }
        //        else
        //        {
        //            string pageName = info.Url.Replace("https://www.facebook.com/", "");
        //            pageName = pageName.Replace("/", "");
        //            string path = pageName + "/feed";

        //            lstChannels.Add(path + fbFields);
        //        }

        //        foreach (string channelPath in lstChannels)
        //        {
        //            Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist", config.Id + "-" + config.SearchTerm + "Getting data from Page:" + channelPath);
        //            Dictionary<string, object> Pparams = new Dictionary<string, object>();

        //            parameters.Clear();

        //            //parameters.Add("since", maximumTime.ToString("yyyy-MM-dd HH:mm:ss"));
        //            parameters.Add("limit", "250");
        //            bool nextPageAvailable = true;

        //            Pparams = parameters;

        //            do
        //            {
        //                parameters = Pparams;
        //                List<JsonObject> feeds = Insight360.Sources.Facebook.DataCollection.GetPagedItems(channelPath, accessToken, parameters, config.LastFetchTime, untilTime, out nextPageAvailable, out Pparams);

        //                int feedCount = feeds.Count;

        //                if (feedCount > 0)
        //                {
        //                    for (int i = 0; i < feedCount; i++)
        //                    {
        //                        var post = Insight360.Sources.Facebook.DataCollection.CreateLocalPost(feeds[i], accessToken, AppSettings.GetCommentsAndLikes, maximumTime, out maximumTime);

        //                        if (post != null)
        //                        {
        //                            post.Staging.Followup = (config.Type == Insight360.BO.SearchTermInfo.Types.FacebookEngagementUser);
        //                            //post.Staging.OrgName = organizationName;
        //                            post.Staging.DocumentSourceId = info.DocumentSourceId;
        //                            post.Staging.SearchTermId = config.Id;
        //                            //config.LastFetchTime = maximumTime;

        //                            ThreadPool.QueueUserWorkItem(PersistPost, new object[] { config, post, info.DocumentSourceId, false, info });
        //                            Util.Log.WriteOperationStarted(this.GetType(), "FetchPostsAndPersist", "Queued Persisting Posts " + post.Id + " searchterm=" + config.SearchTerm);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (untilTime != null)
        //                        maximumTime = untilTime.Value > maximumTime ? untilTime.Value : maximumTime;

        //                    config.LastFetchTime = maximumTime;
        //                    if (posts == null || posts.Count == 0)
        //                    {
        //                        DAL.ConfigDAL.UpdateDocumentSourceConfiguration(config, info.DocumentSourceId);

        //                        //if (untilTime != null)
        //                        //{
        //                        //    if ((DateTime.Now - config.LastFetchTime).TotalHours > Properties.Settings.Default.UntilHoursCount)
        //                        //        FetchPostsAndPersist(config, info);
        //                        //}
        //                    }
        //                }
        //            }
        //            while (nextPageAvailable);
        //        }

        //        Util.Log.WriteOperationCompleted(this.GetType(), "FetchPostsAndPersist",
        //            config.Id + "-" + config.SearchTerm + "-" + config.Type + "-" + config.LastFetchTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        Util.Log.WriteException(this.GetType(), "FetchPostsAndPersist", ex);
        //    }

        //    return config.LastFetchTime;
        //}

        //private void PersistPosts(object param)
        //{
        //    lock (this)
        //    {
        //        try
        //        {
        //            BO.SearchTermInfo config = ((object[])param)[0] as BO.SearchTermInfo;
        //            List<BO.FacebookPost> posts = ((object[])param)[1] as List<BO.FacebookPost>;
        //            int documentSourceId = (int)((object[])param)[2];
        //            //int organizationId = (int)((object[])param)[3];
        //            //string organizationName = (string)((object[])param)[4];
        //            bool recurse = (bool)((object[])param)[3];
        //            BO.DocumentSourceInfo info = ((object[])param)[4] as BO.DocumentSourceInfo;

        //            Util.Log.WriteOperationStarted(this.GetType(), "PersistPosts", "Persisting Posts " + posts.Count + " searchterm=" + config.SearchTerm);
        //            bool error = false;
        //            foreach (BO.FacebookPost post in posts)
        //            {
        //                //post.Staging.Followup = config.Type == Insight360.BO.SearchTermInfo.Types.FacebookEngagementUser;
        //                //post.Staging.OrgName = organizationName;
        //                //post.Staging.DocumentSourceId = documentSourceId;
        //                //post.Staging.SearchTermId = config.Id;
        //                if (!DAL.StagingDAL.PersistFacebookPost(post, AppSettings.GetCommentsAndLikes, true, config.Id))
        //                {
        //                    error = true;
        //                    break;
        //                }
        //                //, config.LastFetchTime == post.UpdatedTime);
        //            }

        //            if (!error)
        //            {
        //                //if (untilTime != null)
        //                //    maximumTime = untilTime.Value > maximumTime ? untilTime.Value : maximumTime;

        //                //config.LastFetchTime = maximumTime;
        //                DAL.ConfigDAL.UpdateDocumentSourceConfiguration(config, documentSourceId);
        //                if (recurse)
        //                {
        //                    if ((DateTime.Now - config.LastFetchTime).TotalHours > AppSettings.UntilHoursCount)
        //                        FetchPostsAndPersist(config, info);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Util.Log.WriteException(this.GetType(), "PersistPosts", ex);
        //        }
        //    }
        //}

        private void PersistPosts(object param)
        {
            lock (this)
            {
                try
                {
                    //BO.SearchTermInfo config = ((object[])param)[0] as BO.SearchTermInfo;
                    List<BO.FacebookPost> posts = ((object[])param)[0] as List<BO.FacebookPost>;
                    int documentSourceId = (int)((object[])param)[1];
                    //int organizationId = (int)((object[])param)[3];
                    //string organizationName = (string)((object[])param)[4];

                    //bool recurse = (bool)((object[])param)[3];
                    //BO.DocumentSourceInfo info = ((object[])param)[4] as BO.DocumentSourceInfo;

                    //Util.Log.WriteOperationStarted(this.GetType(), "PersistPosts", "Persisting Posts " + posts.Count + " searchterm=" + config.SearchTerm);
                    bool error = false;

                    foreach (BO.FacebookPost post in posts)
                    {
                        if (!DAL.StagingDAL.PersistFacebookPost(post, AppSettings.GetCommentsAndLikes, true, post.Staging.SearchTermId))
                        {
                            error = true;
                            break;
                        }
                    }

                    //if (!error)
                    //{
                    //    DAL.ConfigDAL.UpdateDocumentSourceConfiguration(config, documentSourceId);

                    //    if (recurse)
                    //    {
                    //        if ((DateTime.Now - config.LastFetchTime).TotalHours > AppSettings.UntilHoursCount)
                    //            FetchPostsAndPersist(info);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(this.GetType(), "PersistPosts", ex);
                }
            }
        }

        private void PersistPost(object param)
        {
            lock (this)
            {
                try
                {
                    BO.SearchTermInfo config = ((object[])param)[0] as BO.SearchTermInfo;
                    BO.FacebookPost post = ((object[])param)[1] as BO.FacebookPost;
                    int documentSourceId = (int)((object[])param)[2];

                    //int organizationId = (int)((object[])param)[3];
                    //string organizationName = (string)((object[])param)[4];

                    bool recurse = (bool)((object[])param)[3];
                    BO.DocumentSourceInfo info = ((object[])param)[4] as BO.DocumentSourceInfo;

                    Util.Log.WriteOperationStarted(this.GetType(), "PersistPosts", "Persisting Post " + post.Id + " searchterm=" + config.SearchTerm);
                    bool error = false;

                    if (!DAL.StagingDAL.PersistFacebookPost(post, AppSettings.GetCommentsAndLikes, true, config.Id))
                    {
                        error = true;
                    }

                    if (!error)
                    {
                        DAL.ConfigDAL.UpdateDocumentSourceConfiguration(config, documentSourceId);

                        if (recurse)
                        {
                            if ((DateTime.Now - config.LastFetchTime).TotalHours > AppSettings.UntilHoursCount)
                                FetchPostsAndPersist(info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(this.GetType(), "PersistPosts", ex);
                }
            }
        }

    }
}
