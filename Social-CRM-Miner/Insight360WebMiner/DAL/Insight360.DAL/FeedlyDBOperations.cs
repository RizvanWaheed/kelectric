using Insight360.Common.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.BulkInsert.Extensions;
using System.Transactions;

namespace Insight360.DAL
{
    public class FeedlyDBOperations : IDisposable
    {
        public List<FeedlyKeywords> GetFeedlyKeywords()
        {
            var feedKeywords = new List<FeedlyKeywords>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    feedKeywords = db.FeedlyKeywords.Where(x => x.IsActive).Where(y => y.IsSearchTerm == true).Select(s => new FeedlyKeywords
                    {
                        GroupId = s.GroupId,
                        Id = s.Id,
                        IsSearchTerm = s.IsSearchTerm,
                        SearchKeyword = s.SearchKeyword,
                        LastFeatchDateTime=s.LastFeatchDateTime
                    }).ToList();
                }
                catch (Exception exp)
                {
                    feedKeywords = null;
                }
                db.Dispose();
            }
            return feedKeywords;
        }

        public List<FeedlyKeywords> GetFeedlyPages()
        {
            var feedKeywords = new List<FeedlyKeywords>();
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    feedKeywords = db.FeedlyKeywords.Where(x => x.IsActive).Where(y => y.IsSearchTerm == false).Select(s => new FeedlyKeywords
                    {
                        GroupId = s.GroupId,
                        Id = s.Id,
                        IsSearchTerm = s.IsSearchTerm,
                        SearchKeyword = s.SearchKeyword,
                        LastFeatchDateTime = s.LastFeatchDateTime
                    }).ToList();
                }
                catch (Exception exp)
                {
                    feedKeywords = null;
                }
                db.Dispose();
            }
            return feedKeywords;
        }

        public void DumpFeedlyFeed(List<FeedlyViewModel> apiResponse, int groupId, int keywordId, bool isSearchTerm)
        {
            var status = false;
            using (var db = new SocialCRMMinningEntities())
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    try
                    {
                        var feedKeyword = db.FeedlyKeywords.Where(x => x.Id == keywordId).FirstOrDefault();
                        var ascDate = apiResponse.OrderBy(t => t.LastFeatchDate).FirstOrDefault().LastFeatchDate;
                        var descDate = apiResponse.OrderByDescending(t => t.LastFeatchDate).FirstOrDefault().LastFeatchDate;
                        var date = apiResponse.OrderByDescending(t => t.LastFeatchDate).FirstOrDefault().LastFeatchDate;
                        feedKeyword.LastFeatchDateTime = date;
                        var quotient = 1000;
                        var remainder = 0;
                        var total = apiResponse.Count;
                        var startIndex = 0;
                        quotient = total / 1000;
                        remainder = total % 1000;
                        for (int i = 0; i < quotient; i++)
                        {
                            db.BulkInsert(apiResponse.Skip(startIndex).Take(2500).Select(x => new FeedlyMinnedData
                            {
                                CrawledDate = x.CrawledDate,
                                FacebookLikes = x.FacebookLikes,
                                FacebookUserName = x.FacebookUserName,
                                GroupId = groupId,
                                HeadingURL = x.HeadingURL,
                                ImageCoverUrl = x.ImageCoverUrl,
                                KeywordId = keywordId,
                                MessageBody = x.MessageBody,
                                MessageHeading = x.MessageHeading,
                                MessageShortBody = x.MessageShortBody,
                                TwitterFollowers = x.TwitterFollowers,
                                TwitterScreenName = x.TwitterScreenName,
                                isSearchTerm = isSearchTerm,
                                FeedlyId = x.FeedlyId
                            }));
                            //db.FeedlyMinnedDatas.AddRange(apiResponse.Skip(startIndex).Take(2500).Select(x => new FeedlyMinnedData
                            //{
                            //    CrawledDate = x.CrawledDate,
                            //    FacebookLikes = x.FacebookLikes,
                            //    FacebookUserName = x.FacebookUserName,
                            //    GroupId = groupId,
                            //    HeadingURL = x.HeadingURL,
                            //    ImageCoverUrl = x.ImageCoverUrl,
                            //    KeywordId = keywordId,
                            //    MessageBody = x.MessageBody,
                            //    MessageHeading = x.MessageHeading,
                            //    MessageShortBody = x.MessageShortBody,
                            //    TwitterFollowers = x.TwitterFollowers,
                            //    TwitterScreenName = x.TwitterScreenName,
                            //    isSearchTerm = isSearchTerm,
                            //    FeedlyId = x.FeedlyId
                            //}));
                            startIndex += 2500;
                        }
                        if(remainder!=0)
                        {
                            //db.FeedlyMinnedDatas.AddRange(apiResponse.Skip(startIndex).Take(2500).Select(x => new FeedlyMinnedData
                            //{
                            //    CrawledDate = x.CrawledDate,
                            //    FacebookLikes = x.FacebookLikes,
                            //    FacebookUserName = x.FacebookUserName,
                            //    GroupId = groupId,
                            //    HeadingURL = x.HeadingURL,
                            //    ImageCoverUrl = x.ImageCoverUrl,
                            //    KeywordId = keywordId,
                            //    MessageBody = x.MessageBody,
                            //    MessageHeading = x.MessageHeading,
                            //    MessageShortBody = x.MessageShortBody,
                            //    TwitterFollowers = x.TwitterFollowers,
                            //    TwitterScreenName = x.TwitterScreenName,
                            //    isSearchTerm = isSearchTerm,
                            //    FeedlyId = x.FeedlyId
                            //}));
                            db.BulkInsert(apiResponse.Skip(startIndex).Take(2500).Select(x => new FeedlyMinnedData
                            {
                                CrawledDate = x.CrawledDate,
                                FacebookLikes = x.FacebookLikes,
                                FacebookUserName = x.FacebookUserName,
                                GroupId = groupId,
                                HeadingURL = x.HeadingURL,
                                ImageCoverUrl = x.ImageCoverUrl,
                                KeywordId = keywordId,
                                MessageBody = x.MessageBody,
                                MessageHeading = x.MessageHeading,
                                MessageShortBody = x.MessageShortBody,
                                TwitterFollowers = x.TwitterFollowers,
                                TwitterScreenName = x.TwitterScreenName,
                                isSearchTerm = isSearchTerm,
                                FeedlyId = x.FeedlyId
                            }));
                        }
                        db.SaveChanges();
                        transactionScope.Complete();
                        status = true;
                    }
                    catch (Exception exp)
                    {
                        status = false;
                    }
                }                   
                db.Dispose();
            }
        }

        public void Dispose()
        {
           
        }
    }
}
