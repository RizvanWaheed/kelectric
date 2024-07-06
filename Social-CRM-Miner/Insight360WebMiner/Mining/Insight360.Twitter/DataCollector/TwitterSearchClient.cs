using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight360.Common;
using Insight360.Util;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace Insight360.Twitter
{
    class TwitterSearchClient
    {
        private const int RESULTS_PER_PAGE = 100;



        public DateTime FetchAndPersist(BO.SearchTermInfo config, BO.DocumentSourceInfo source, BO.Location location) /*, int organizationId,string organizationName*/
        {
            try
            {
                int documentSourceId = source.DocumentSourceId;
                Util.Log.WriteOperationStarted(typeof(TwitterSearchClient), "FetchAndPersist", /*organizationName + "-" + */config.SearchTerm + "-" + config.Type);

                if (source.ProfileType != Insight360.BO.ProfileTypes.TwitterHandle)
                {
                    // Tweet Count to use for paging in tweets
                    // To get one page of tweets from api and store it on db before second call
                    int TweetCount = 1;

                    while (TweetCount > 0)
                    {
                        Util.Log.WriteOperationStarted(typeof(TwitterSearchClient), "SearchTerm ", /*organizationName + "-" + */config.SearchTerm + "-" + config.Type);

                        long maximumId = 0;
                        DateTime maximumTime;
                        List<BO.TwitterStatus> allStatuses = Insight360.Sources.Twitter.DataCollection.SearchStatuses(
                            new BO.TwitterAuthentication
                            {
                                ConsumerKey = AppSettings.TwitterMentions_ConsumerKey,
                                ConsumerSecret = AppSettings.TwitterMentions_ConsumerSecret,
                                Token =AppSettings.TwitterMentions_Token,
                                TokenSecret =AppSettings.TwitterMentions_TokenSecret
                            },
                            config.SearchTerm,
                            long.Parse(config.LastFetchId),
                            out maximumId,
                            out maximumTime,
                            config,
                            location);

                        Util.Log.WriteOperationStarted(typeof(TwitterSearchClient), "Status Fetched", /*organizationName + "-" + */config.SearchTerm + "-" + config.Type);

                        //List<BO.TwitterStatus> statuses = new List<BO.TwitterStatus>();
                        //List<BO.Location> locations = DAL.ConfigDAL.GetLocations(config.Id);

                        //Ahsen: All tweets should go in the DB, there will be filter on dashbord to see tweets in a specific location.
                        //if (locations.Count > 0)
                        //{
                        //    foreach (BO.Location location in locations)
                        //    {
                        //        foreach (BO.TwitterStatus tempStatus in allStatuses)
                        //        {
                        //            if (tempStatus.UserDetails.Location.ToLower().Contains(location.name.ToLower()))
                        //            {
                        //                //tempStatus.UserDetails.Location = location.Id.ToString();
                        //                statuses.Add(tempStatus);
                        //            }
                        //        }
                        //        //statuses = statuses.Union(allStatuses.Where(x=>x.UserDetails.Location.ToLower().Contains(location.name.ToLower()))).ToList();
                        //    }
                        //}
                        //else
                        //{
                        //    //make location column 0 here.
                        //    statuses = allStatuses;
                        //}

                        // adi change lucene.net
                        List<Common.BO.KeyTerm> terms = DAL.ConfigDAL.GetKeyTerms(config.Id);
                        string searchQuery = "Text:";
                        int termCount = 0;
                        foreach(var term in terms)
                        {
                            if (termCount > 0)
                                searchQuery += " OR " + "(" + term.Name + ")";
                            else
                                searchQuery += "(" + term.Name + ")";
                            termCount++;
                        }
                        try
                        {
                            if (allStatuses != null && allStatuses.Count > 0 && termCount > 0)
                                allStatuses = search(searchQuery, allStatuses);
                        }
                        catch (Exception ex) {
                            Util.Log.WriteException(typeof(TwitterSearchClient), "FetchAndPersist LUCENE.NET ", ex);
                        }
                        // adi change lucene.net end

                        if (allStatuses != null && allStatuses.Count > 0)
                        {
                            foreach (BO.TwitterStatus status in allStatuses)
                            {
                                //status.Staging.OrgName = organizationName;
                                status.UserDetails.Gender = Helpers.DetectGender(status.UserDetails.Name);
                                DAL.StagingDAL.PersistTwitterStatus(status, config.Id, false, true);
                            }

                            config.LastFetchId = maximumId.ToString();
                            config.LastFetchTime = maximumTime;

                            DAL.ConfigDAL.UpdateDocumentSourceConfiguration(config, documentSourceId/*, organizationId*/);
                            Util.Log.WriteOperationStarted(typeof(TwitterSearchClient), "FetchAndPersist", /*organizationName + "-" + */config.SearchTerm + "-" + config.Type + "-" + maximumId);
                        }

                        TweetCount = allStatuses != null ? allStatuses.Count : 0;
                        //Util.Log.WriteOperationCompleted(organizationName + "-" + config.SearchTerm + "-" + config.Type + "-" + statuses.Count + "-" + maximumId);
                    }
                }
                else
                {
                    //if (config.Type == Insight360.BO.SearchTermInfo.Types.TwitterUser
                    //|| config.Type == Insight360.BO.SearchTermInfo.Types.TwitterEngagementUser)

                    long maximumId = long.Parse(config.LastFetchId);
                    long maximumId2 = long.Parse(config.LastFetchId2);

                    if (maximumId == 0) maximumId = 1;
                    if (maximumId2 == 0) maximumId2 = 1;

                    DateTime maximumTime = config.LastFetchTime;
                    DateTime maximumTime2 = config.LastFetchTime;

                    List<BO.TwitterStatus> timeline = Insight360.Sources.Twitter.DataCollection.GetUserTimeline(
                        new BO.TwitterAuthentication
                        {
                            ConsumerKey = AppSettings.TwitterMentions_ConsumerKey,
                            ConsumerSecret = AppSettings.TwitterMentions_ConsumerSecret,
                            Token = AppSettings.TwitterMentions_Token,
                            TokenSecret = AppSettings.TwitterMentions_TokenSecret
                        },
                        config.SearchTerm.ToLowerInvariant(),
                        source.Url,
                        maximumId,
                        out maximumId,
                        out maximumTime);

                    //timeline = timeline.Where(s => s.Text.ToLower().Contains(config.SearchTerm.ToLower())).ToList();

                    List<BO.TwitterStatus> mentions = null;
                    //if (config.Type == Insight360.BO.SearchTermInfo.Types.TwitterEngagementUser)
                    //{
                    //    mentions = Insight360.Sources.Twitter.DataCollection.GetUserMentions(
                    //        new BO.TwitterAuthentication
                    //        {
                    //            ConsumerKey = Properties.Settings.Default.TwitterMentions_ConsumerKey,
                    //            ConsumerSecret = Properties.Settings.Default.TwitterMentions_ConsumerSecret,
                    //            Token = Properties.Settings.Default.TwitterMentions_Token,
                    //            TokenSecret = Properties.Settings.Default.TwitterMentions_TokenSecret
                    //        },
                    //        maximumId2,
                    //        out maximumId2,
                    //        out maximumTime2);
                    //}                    

                    
                    if (mentions != null)
                    {
                        foreach (BO.TwitterStatus status in mentions)
                        {
                            status.Sentiment = Helpers.SentimentScore(status.Text);
                            status.LanguageCode = Helpers.DetectLanguage(status.Text);

                            if (status.UserDetails.Gender != "")
                                status.UserDetails.Gender = Helpers.DetectGender(status.UserDetails.Name);

                            //status.Staging.OrgName = organizationName;
                            status.Staging.Followup = config.Type == Insight360.BO.SearchTermInfo.Types.TwitterEngagementUser;
                            DAL.StagingDAL.PersistTwitterStatus(status, config.Id, false, true);
                        }
                    }

                    if (timeline != null)
                    {
                        foreach (BO.TwitterStatus status in timeline)
                        {
                            //status.Staging.OrgName = organizationName;
                            status.Sentiment = Helpers.SentimentScore(status.Text);
                            status.LanguageCode = Helpers.DetectLanguage(status.Text);

                            if (status.UserDetails.Gender != "")
                                status.UserDetails.Gender = Helpers.DetectGender(status.UserDetails.Name);

                            status.Staging.Followup = config.Type == Insight360.BO.SearchTermInfo.Types.TwitterEngagementUser;

                            DAL.StagingDAL.PersistTwitterStatus(status, config.Id, false, true);
                        }
                    }


                    // adi change lucene.net
                    List<Common.BO.KeyTerm> terms = DAL.ConfigDAL.GetKeyTerms(config.Id);
                    string searchQuery = "Text:";
                    int termCount = 0;
                    foreach (var term in terms)
                    {
                        if (termCount > 0)
                            searchQuery += " OR " + "(" + term.Name + ")";
                        else
                            searchQuery += "(" + term.Name + ")";
                        termCount++;
                    }
                    try
                    {
                        if (mentions != null && mentions.Count > 0 && termCount > 0)
                            mentions = search(searchQuery, mentions);
                        if (timeline != null && timeline.Count > 0 && termCount > 0)
                            timeline = search(searchQuery, timeline);
                    }
                    catch (Exception ex) {
                        Util.Log.WriteException(typeof(TwitterSearchClient), "FetchAndPersist TIMELINE LUCENE.NET ", ex);
                    }
                    // adi change lucene.net end

                    if ((timeline != null && timeline.Count > 0) || (mentions != null && mentions.Count > 0))
                    {
                        config.LastFetchId = maximumId.ToString();
                        config.LastFetchId2 = maximumId2.ToString();
                        config.LastFetchTime = maximumTime > maximumTime2 ? maximumTime : maximumTime2;

                        DAL.ConfigDAL.UpdateDocumentSourceConfiguration(config, documentSourceId/*, organizationId*/);
                    }

                    //Util.Log.WriteOperationCompleted(organizationName + "-" + config.SearchTerm + "-" + config.Type
                    //+ "-" + (timeline != null ? timeline.Count : 0) + "-" + (mentions != null ? mentions.Count : 0) + "-" + maximumId + "-" + maximumId2);

                    Util.Log.WriteOperationStarted(typeof(TwitterSearchClient), "FetchAndPersist", /*organizationName + "-" + */config.SearchTerm + "-" + config.Type
                            + "-" + (timeline != null ? timeline.Count : 0) + "-" + (mentions != null ? mentions.Count : 0) + "-" + maximumId + "-" + maximumId2);
                }
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(TwitterSearchClient), "FetchAndPersist", ex);
            }

            return config.LastFetchTime;
        }


        #region Search Methods

        Directory createIndex(List<BO.TwitterStatus> tweets)
        {
            var directory = new RAMDirectory();

            using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            using (var writer = new IndexWriter(directory, analyzer, new IndexWriter.MaxFieldLength(10000)))
            { // the writer and analyzer will popuplate the directory with documents

                foreach (BO.TwitterStatus tweet in tweets)
                {
                    var document = new Document();

                    document.Add(new Field("Text", tweet.Text.ToLower(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Id", tweet.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    //document.Add(new Field("FullText",
                    //    string.Format("{0}", tweet.Text)
                    //    , Field.Store.YES, Field.Index.ANALYZED));
                    writer.AddDocument(document);
                }

                writer.Optimize();
                writer.Flush(true, true, true);
            }

            return directory;
        }

        List<BO.TwitterStatus> search(string textSearch, List<BO.TwitterStatus> tweets)
        {
            List<BO.TwitterStatus> filteredTweets = new List<BO.TwitterStatus>();
            //var table = Sample.Clone();

            Directory Index = createIndex(tweets);

            using (var reader = IndexReader.Open(Index, true))
            using (var searcher = new IndexSearcher(reader))
            {
                using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                {
                    var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Text", analyzer);

                    queryParser.AllowLeadingWildcard = true;

                    var query = queryParser.Parse(textSearch);

                    var collector = TopScoreDocCollector.Create(1000, true);

                    searcher.Search(query, collector);

                    var matches = collector.TopDocs().ScoreDocs;

                    foreach (var item in matches)
                    {
                        var id = item.Doc;
                        var doc = searcher.Doc(id);

                        var selected = tweets.Where(x => x.Id.ToString() == doc.GetField("Id").StringValue).First();
                        if(selected!=null)
                            filteredTweets.Add(selected);
                    }
                }
            }

            return filteredTweets;

        }

        #endregion


    }
}
