using Insight360.DAL;
using Insight360.Feedly.FeedlySharp;
using Insight360.Feedly.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class FeedlyProcessor : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //Fetch feedly Search Term user Defines.
                var db = new FeedlyDBOperations();
                var dataMap = context.MergedJobDataMap;
                var myInstance = (string)dataMap["FetchType"];
                var keywordsList = new List<Common.BO.FeedlyKeywords>();
                if(myInstance == "Keywords")
                    keywordsList = db.GetFeedlyKeywords();
                else if(myInstance == "Pages")
                    keywordsList = db.GetFeedlyPages();

                var client = new FeedlyClient(CloudEnvironment.Production, "", "", "");
                List<FeedlySearchFeed> feeds = null;
                FeedlyFeed rssFeed = null;
                bool isSearchTerm = false;
                DateTime? lastFeatchDateTime = null;
                List<Insight360.Common.BO.FeedlyViewModel> apiResponse = new List<Insight360.Common.BO.FeedlyViewModel>();
                foreach (var searchTerm in keywordsList)
                {
                    try
                    {
                        if (searchTerm.LastFeatchDateTime.HasValue)
                            lastFeatchDateTime = searchTerm.LastFeatchDateTime.Value.AddSeconds(1);//this is because feedly api consider >= not >
                        if (searchTerm.IsSearchTerm)
                        {
                            feeds = client.FindFeeds(searchTerm.SearchKeyword, 10000).Result;
                            if (feeds != null)
                            {
                                foreach (FeedlySearchFeed feed in feeds)
                                {
                                    var feedResult = Feedly.Sources.DataCollection.SearchFeeds(searchTerm.SearchKeyword, client, feed, lastFeatchDateTime);
                                    if (feedResult != null && feedResult.Count > 0)
                                    {
                                        apiResponse.AddRange(MapFeedResponse(feedResult, true));
                                    }
                                }
                            }
                            isSearchTerm = true;
                        }
                        else
                        {
                            rssFeed = client.GetFeed("feed/" + searchTerm.SearchKeyword).Result;
                            feeds = new List<FeedlySearchFeed>();
                            feeds.Add(Map(rssFeed));
                            if (feeds != null)
                            {
                                foreach (FeedlySearchFeed feed in feeds)
                                {
                                    var feedResult = Feedly.Sources.DataCollection.SearchFeeds(searchTerm.SearchKeyword, client, feed, lastFeatchDateTime, true);
                                    if (feedResult != null && feedResult.Count > 0)
                                    {
                                        apiResponse.AddRange(MapFeedResponse(feedResult, false));
                                    }
                                }
                            }
                            isSearchTerm = false;
                        }
                        if (apiResponse.Count > 0)
                        {
                            db.DumpFeedlyFeed(apiResponse, searchTerm.GroupId, searchTerm.Id, isSearchTerm);
                        }
                        apiResponse.Clear();
                    }
                    catch(OutOfMemoryException outMemory)
                    {
                        Console.Write(outMemory.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private IEnumerable<Insight360.Common.BO.FeedlyViewModel> MapFeedResponse(List<Feedly.ViewModel.FeedlyItem> feedResult, bool isSearchTerm)
        {
            return feedResult.Select(x => new Insight360.Common.BO.FeedlyViewModel
            {
                CrawledDate = x.CrawledDate.HasValue ? x.CrawledDate.Value.ToString("MMMM dd yyyy h:mm tt") : "",
                FacebookLikes = x.Feed.FacebookLikes.HasValue ? x.Feed.FacebookLikes.Value.ToString(CultureInfo.InvariantCulture) : "",
                FacebookUserName = x.Feed.FacebookUserName,
                TwitterFollowers = x.Feed.TwitterFollowers.HasValue ? x.Feed.TwitterFollowers.Value.ToString(CultureInfo.InvariantCulture) : "",
                TwitterScreenName = x.Feed.TwitterScreenName,
                MessageHeading = x.Title,
                MessageShortBody = x.Summary_Text.Length > 500 ? x.Summary_Text.Substring(0, 499) : x.Summary_Text,
                MessageBody = x.Summary_Text,
                HeadingURL = isSearchTerm ? string.IsNullOrEmpty(x.Feed.Website) ? x.BaseUri : x.Feed.Website : x.OriginId,
                ImageCoverUrl = x.Feed.CoverUrl,
                LastFeatchDate = x.CrawledDate.HasValue ? x.CrawledDate.Value : DateTime.Now.AddDays(-90),
                FeedlyId=x.Id
            }).ToList();
        }
        private FeedlySearchFeed Map(FeedlyFeed model)
        {
            var ret = new FeedlySearchFeed();
            ret.CoverColor = model.CoverColor;
            ret.CoverImage = model.CoverImage;
            ret.Description = model.Description;
            ret.FacebookLikes = model.FacebookLikes;
            ret.FacebookName = model.FacebookName;
            ret.IconImage = model.IconImage;
            ret.Id = model.Id;
            ret.Image = model.Image;
            ret.Language = model.Language;
            ret.Name = model.Name;
            ret.Subscribers = model.Subscribers;
            ret.Topics = model.Topics;
            ret.TwitterFollowers = model.TwitterFollowers;
            ret.TwitterName = model.TwitterName;
            ret.Uri = model.Uri;
            ret.Velocity = model.Velocity;
            return ret;
        }
    }
}