using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insight360.BO;
using System.Data.Entity;
using Insight360.Common.BO;
using Insight360.DAL;

namespace Insight360.DAL
{
    public static class CommonDBOperations
    {
        public static List<CustomerSocialProfile> GetCustomerSocialProfiles()
        {
            var account = new List<CustomerSocialProfile>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    account = db.CustomerSocialProfiles.Where(x => x.Name != null && x.Gender == null).ToList();
                }
                catch (Exception exp) { }
            }
            return account;
        }

        public static List<FeedsSentiment> GetPostsWithUnsetSentiment()
        {
            var sentiments = new List<FeedsSentiment>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    sentiments = db.FeedsSentiments.Where(x=>x.SentimentId == (int)Util.Sentiments.Unknown).ToList();
                }
                catch (Exception exp) { }
            }
            return sentiments;
        }

        public static void UpdateSentiments(List<FeedsSentiment> sentiments)
        {

            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    foreach (var item in sentiments)
                    {
                        FeedsSentiment existSentiment = db.FeedsSentiments.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (existSentiment != null)
                        {
                            existSentiment.SentimentId = item.SentimentId;
                            db.Entry(existSentiment).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                catch (Exception exp) { }
            }
        }

        public static string GetPostText(int postId, string postType)
        {
            string postText = "";
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    postText = db.GetFeedsById(postId, postType).FirstOrDefault().TextValue;
                }
                catch (Exception exp) { }
            }
            return postText;
        }

        public static void UpdateCustomerSocialProfile(List<CustomerSocialProfile> profiles)
        {
            
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    foreach (var item in profiles)
                    {
                        CustomerSocialProfile existProfile = db.CustomerSocialProfiles.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (existProfile != null)
                        {
                            existProfile.Gender = item.Gender;
                            db.Entry(existProfile).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                catch (Exception exp) { }
            }
        }
    }
}
