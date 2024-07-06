using CRM.Entities;
using SocialCRM.Web.Models.Feedly;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class FeedlyRepository : IFeedlyRepository
    {
        public async Task<bool> AddKeyword(FeedlyModel feedKeyword, int userId)
        {
            var success = false;
            using (var db = new SocialCRMEntities())
            {
                var teamMemberId = db.GetTeamMemberIdUsingGroupId(feedKeyword.GroupId, userId).FirstOrDefault();
                if (teamMemberId.HasValue)
                {
                    var isSearchExist = db.FeedlyKeywords.Where(k => k.GroupId == feedKeyword.GroupId && k.IsSearchTerm == feedKeyword.IsSearchTerm && string.Compare(k.SearchKeyword, feedKeyword.SearchKeyword) == 0).FirstOrDefault();
                    if (isSearchExist == null)
                    {
                        var feedkeyword = db.FeedlyKeywords.Add(new FeedlyKeyword
                        {
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatorMemberId = teamMemberId.Value,
                            GroupId = feedKeyword.GroupId,
                            IsSearchTerm = feedKeyword.IsSearchTerm,
                            SearchKeyword = feedKeyword.SearchKeyword
                        });
                    }
                    else
                    {
                        isSearchExist.ModifiedDate = DateTime.Now;
                        isSearchExist.UpdatedBy = teamMemberId.Value;
                        isSearchExist.IsActive = true;
                    }
                }
                var affectedRows = await db.SaveChangesAsync();
                success = affectedRows > 0;
                db.Dispose();
            }
            return success;
        }
        public async Task<bool> DeleteKeyword(int feedlyKewordId, int userId)
        {
            var success = false;
            using (var db = new SocialCRMEntities())
            {
                var feedKeyword = db.FeedlyKeywords.Where(f => f.Id == feedlyKewordId).FirstOrDefault();
                if (feedKeyword != null)
                {
                    var teamMemberId = db.GetTeamMemberIdUsingGroupId(feedKeyword.GroupId, userId).FirstOrDefault();
                    if (teamMemberId.HasValue)
                    {
                        feedKeyword.IsActive = false;
                        feedKeyword.ModifiedDate = DateTime.Now;
                        feedKeyword.UpdatedBy = teamMemberId.Value;
                    }
                    var affectedRows = await db.SaveChangesAsync();
                    success = affectedRows > 0;
                }
                db.Dispose();
            }
            return success;
        }
        public async Task<List<FeedlyModel>> GetKeywordByGroup(FeedlySearchModel searchModel)
        {
            List<FeedlyModel> result = new List<FeedlyModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.FeedlyKeywords
                        .Where(k => k.GroupId == searchModel.GroupId && k.IsSearchTerm == searchModel.IsSearchTerm && k.IsActive && searchModel.SearchTerm.Contains(k.Id))
                        .Select(r => new FeedlyModel
                                {
                                    CreatedDate = r.CreatedDate,
                                    GroupId = r.GroupId,
                                    IsActive = r.IsActive,
                                    IsSearchTerm = r.IsSearchTerm,
                                    SearchKeyword = r.SearchKeyword,
                                    Id = r.Id
                                }).ToList();
                    db.Dispose();
                }
            });
            return result;
        }
        public Task<List<FeedlyViewModel>> GetMinnedFeedByGroup(FeedlySearchModel searchModel)
        {
            List<FeedlyViewModel> result=null;
            try
            {
                using (var db = new SocialCRMEntities())
                {
                    var response = db.GetMinnedFeedByGroup(searchModel.GroupId, searchModel.IsSearchTerm, searchModel.PageIndex, searchModel.PageSize).ToList();
                    if (response != null && response.Count > 0)
                    {
                        result = response.Where(r => (searchModel.SearchTerm.Count == 0 || searchModel.SearchTerm.Contains(r.KeywordId))).Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize)
                                        .Select(x => new FeedlyViewModel
                                        {
                                            Id = x.Id,
                                            CrawledDate = x.CrawledDate.HasValue ? x.CrawledDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                                            FacebookLikes = x.FacebookLikes,
                                            FacebookUserName = x.FacebookUserName,
                                            HeadingURL = x.HeadingURL,
                                            ImageCoverUrl = x.ImageCoverUrl,
                                            MessageBody = x.MessageBody,
                                            MessageHeading = x.MessageHeading,
                                            MessageShortBody = x.MessageShortBody,
                                            TwitterFollowers = x.TwitterFollowers,
                                            TwitterScreenName = x.TwitterScreenName
                                        }
                                        ).ToList();
                        if (searchModel.MaxFeedId > 0)
                        {
                            result = result.Where(x => x.Id > searchModel.MaxFeedId).ToList();
                        }
                    }
                    db.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            return Task.FromResult<List<FeedlyViewModel>>(result);
        }
        public async Task<List<FeedlyModel>> GetSearchTermByGroupId(int groupId, bool isSearchTerm)
        {
            List<FeedlyModel> result = new List<FeedlyModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    result = db.FeedlyKeywords
                        .Where(k => k.GroupId == groupId && k.IsActive && k.IsSearchTerm == isSearchTerm)
                        .Select(r => new FeedlyModel
                        {
                            CreatedDate = r.CreatedDate,
                            GroupId = r.GroupId,
                            IsActive = r.IsActive,
                            IsSearchTerm = r.IsSearchTerm,
                            SearchKeyword = r.SearchKeyword,
                            Id = r.Id
                        }).ToList();
                    db.Dispose();
                }
            });
            return result;
        }
    }
}