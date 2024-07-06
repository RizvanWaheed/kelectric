using SocialCRM.Web.Models.Feedly;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using FeelyAPI = CRM.APIS.Feedly.FeedlySharp;
using FeelyModelAPI= CRM.APIS.Feedly.Models;
using System.Globalization;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Feedly")]
    public class FeedlyController : ApiController
    {
        IFeedlyRepository repository;
        public FeedlyController(IFeedlyRepository repo)
        {
            repository = repo;
        }
        //api/Feedly/AddSearchTerm/Post.
        [HttpPost]
        [Route("AddSearchTerm")]
        public async Task<IHttpActionResult> AddSearchTerm(FeedlyModel searchModel)
        {
            if (searchModel == null)
            {
                return BadRequest("Search Term Can not Be Empty.");
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.AddKeyword(searchModel, userId);
            return Ok(status);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteSearchTerm(int id)
        {
            if (id == 0)
            {
                return BadRequest("Search Term Can not Be Empty.");
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.DeleteKeyword(id, userId);
            return Ok(status);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetFeed(FeedlySearchModel searchModel)//int groupId, bool isSearchTerm, int pageIndex)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Search Term Can not Be Empty.");
            }
            var searchQuery = await repository.GetKeywordByGroup(searchModel);
            if (searchQuery.Count == 0)
                return Ok("No Search Term Defined against Group.");
            //Getting Response from Feedly API
            FeelyAPI.FeedlyClient client = new FeelyAPI.FeedlyClient(FeelyAPI.CloudEnvironment.Production, "", "", "");
            List<FeelyModelAPI.FeedlySearchFeed> feeds = null;
            FeelyModelAPI.FeedlyFeed rssFeed = null;
            List<FeedlyViewModel> apiResponse = new List<FeedlyViewModel>();
            if (searchModel.IsSearchTerm)
            {
                foreach (var searchterm in searchQuery)
                {
                    feeds = client.FindFeeds(searchterm.SearchKeyword, 10000).Result;
                    if (feeds != null)
                    {
                        foreach (FeelyModelAPI.FeedlySearchFeed feed in feeds)
                        {
                           var feedResult = SocialCRM.Web.Models.Feedly.Sources.DataCollection.SearchFeeds(searchterm.SearchKeyword,client, feed);
                           if (feedResult != null && feedResult.Count>0)
                            {
                                apiResponse.AddRange(MapFeedResponse(feedResult,true));
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var searchterm in searchQuery)
                {
                    rssFeed = client.GetFeed("feed/" + searchterm.SearchKeyword).Result;
                    feeds = new List<FeelyModelAPI.FeedlySearchFeed>();
                    feeds.Add(Map(rssFeed));
                    if (feeds != null)
                    {
                        foreach (FeelyModelAPI.FeedlySearchFeed feed in feeds)
                        {
                            var feedResult = SocialCRM.Web.Models.Feedly.Sources.DataCollection.SearchFeeds(searchterm.SearchKeyword, client, feed,true);
                            if (feedResult != null && feedResult.Count > 0)
                            {
                                apiResponse.AddRange(MapFeedResponse(feedResult,false));
                            }
                        }
                    }
                }
            }
            return Ok(apiResponse.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize));
        }
        [HttpGet]
        [Route("GetSearchTerm/{groupId}/{isSearchTerm}")]
        public async Task<IHttpActionResult> GetSearchTerm(int groupId, bool isSearchTerm)
        {
            if (groupId == 0)
            {
                return BadRequest("GroupId is invalid.");
            }
            var response = await repository.GetSearchTermByGroupId(groupId, isSearchTerm);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetMinnedFeed(FeedlySearchModel searchModel)//int groupId, bool isSearchTerm, int pageIndex)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Search Term Can not Be Empty.");
            }
            var searchQueryResult = await repository.GetMinnedFeedByGroup(searchModel);
            //if (searchQueryResult.Count == 0)
                //return Ok("No Search Term Defined against Group.");
            return Ok(searchQueryResult);
        }
        private IEnumerable<FeedlyViewModel> MapFeedResponse(List<FeedlyItem> feedResult, bool isSearchTerm)
        {
           return feedResult.Select(x => new FeedlyViewModel 
           {
               CrawledDate = x.CrawledDate.HasValue ? x.CrawledDate.Value.ToString("MMMM dd yyyy h:mm tt") : "",
               FacebookLikes = x.Feed.FacebookLikes.HasValue ? x.Feed.FacebookLikes.Value.ToString(CultureInfo.InvariantCulture) : "",
               FacebookUserName=x.Feed.FacebookUserName,
               TwitterFollowers = x.Feed.TwitterFollowers.HasValue ? x.Feed.TwitterFollowers.Value.ToString(CultureInfo.InvariantCulture) : "",
               TwitterScreenName=x.Feed.TwitterScreenName,
               MessageHeading = x.Title,
               MessageShortBody = x.Summary_Text.Length > 500 ? x.Summary_Text.Substring(0, 499) : x.Summary_Text,
               MessageBody = x.Summary_Text,
               HeadingURL =isSearchTerm?string.IsNullOrEmpty(x.Feed.Website)?x.BaseUri:x.Feed.Website: x.OriginId,
               ImageCoverUrl=x.Feed.CoverUrl
           }).ToList();
        }
        private FeelyModelAPI.FeedlySearchFeed Map(FeelyModelAPI.FeedlyFeed model)
        {
            FeelyModelAPI.FeedlySearchFeed ret = new FeelyModelAPI.FeedlySearchFeed();
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
