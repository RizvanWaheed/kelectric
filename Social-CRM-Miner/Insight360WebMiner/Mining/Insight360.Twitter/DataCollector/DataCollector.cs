using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Insight360.Common;
using Insight360.DAL;
using Insight360.BO;
using Insight360.Sources.Twitter;
using TweetSharp;
using System.Data.SqlClient;
using LinqToTwitter;

namespace Insight360.Twitter
{
    public class DataCollector
    {
        public void UserDirectMessageEventsThread(TwitterUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - UserMsgsEventsThread", "Thread Enter");
            try
            {
                var cursor = account.Cursor;
                List<BO.TwitterMessage> userMsgs = DataCollection.GetMessageEvents(ref cursor, account.AccessToken, account.AccessTokenSecret);
                TwitterDBOperations.InsertUserMsgs(userMsgs, account.ProfileExternalId, account.ProfileId);
                TwitterDBOperations.UpdateUserCursor(cursor, account.ProfileExternalId);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Twitter - UserMsgsEventsThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - UserMsgsEventsThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        public void UserMsgsThread(TwitterUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - UserMsgsThread", "Thread Enter");
            try
            {
                var MaxMsgSendToId = ulong.Parse(account.MaxMsgSendToId);
                var MaxMsgSendById = ulong.Parse(account.MaxMsgSendById);
                List<BO.TwitterMessage> userMsgs = DataCollection.GetMsgsNewWay(ref MaxMsgSendToId, account.AccessToken, account.AccessTokenSecret, "sentTo");
                TwitterDBOperations.InsertUserMsgs(userMsgs, account.ProfileExternalId, account.ProfileId);

                userMsgs = DataCollection.GetMsgsNewWay(ref MaxMsgSendById, account.AccessToken, account.AccessTokenSecret, "sentBy");
                TwitterDBOperations.InsertUserMsgs(userMsgs, account.ProfileExternalId, account.ProfileId);
                TwitterDBOperations.UpdateTweetStatus(0, 0, MaxMsgSendToId, MaxMsgSendById, account.ProfileExternalId, null);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Twitter - UserMsgsThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - UserMsgsThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        public void UserTweetsThread(TwitterUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - UserTweetsThread", "Thread Enter");
            try
            {
                var MaxStatusId = ulong.Parse(account.MaxStatusId);
                var MaxMentionStatusId = ulong.Parse(account.MaxMentionStatusId);
                List<BO.TwitterTweet> userTweets = DataCollection.GetTweetsNewWay(ref MaxStatusId, account.AccessToken, account.AccessTokenSecret, "user", account.ProfileExternalId);
                TwitterDBOperations.InsertUserTweets(userTweets, account.ProfileExternalId, null, account.ProfileId);

                userTweets = DataCollection.GetTweetsNewWay(ref MaxMentionStatusId, account.AccessToken, account.AccessTokenSecret, "mention", account.ProfileExternalId);
                TwitterDBOperations.InsertUserTweets(userTweets, account.ProfileExternalId, null, account.ProfileId);
                TwitterDBOperations.UpdateTweetStatus(MaxStatusId, MaxMentionStatusId, 0, 0, account.ProfileExternalId, null);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Twitter - UserTweetsThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - UserTweetsThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        public void KeywordsSearchThread(TwitterUserAccount account, SearchKeyword keyword)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - KeywordsSearchThread", "Thread Enter");
            try
            {
                var MaxStatusId = ulong.Parse(keyword.MaxStatusId);
                List<BO.TwitterTweet> userTweets = DataCollection.SearchTweetsNewWay(ref MaxStatusId, account.AccessToken, account.AccessTokenSecret, account.ProfileExternalId, keyword);
                
                TwitterDBOperations.InsertUserTweets(userTweets, null, keyword.KeywordId, account.ProfileId);
                TwitterDBOperations.UpdateTweetStatus(MaxStatusId, ulong.Parse(account.MaxMentionStatusId), 0, 0, null, keyword.KeywordId);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Twitter - KeywordsSearchThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Twitter - KeywordsSearchThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }
    }
}
