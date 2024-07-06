using Insight360.Common;
using Insight360.DAL;
using Insight360.Scheduling;
using Insight360.Util;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class Sentiments : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                Util.Log.WriteOperationStarted(typeof(Sentiments), "Sentiments - Mark Sentiments", "Thread Enter");
                try
                {
                    var sentiments = CommonDBOperations.GetPostsWithUnsetSentiment().ToList();
                    if (sentiments != null && sentiments.Count > 0)
                    {
                        sentiments.ForEach(delegate (FeedsSentiment sentiment)
                        {
                            string postText = CommonDBOperations.GetPostText(sentiment.FeedId, sentiment.FeedType);
                            int sentimentResult = Helpers.SentimentScore(postText);
                            sentiment.SentimentId = sentimentResult;
                        });

                       CommonDBOperations.UpdateSentiments(sentiments);
                    }
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(typeof(Sentiments), "Sentiments - Mark Sentiments", ex);
                }
                finally
                {
                    Util.Log.WriteOperationStarted(typeof(Sentiments), "Sentiments - Mark Sentiments", "Thread Exit");
                }
            }
        }
    }
}