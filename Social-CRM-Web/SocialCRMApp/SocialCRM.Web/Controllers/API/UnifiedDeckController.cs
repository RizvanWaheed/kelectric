using SocialCRM.Web.Models.TaskManager;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using CRM.Common.Extensions;
using SocialCRM.Web.Models.Report;
using System.Globalization;
using SocialCRM.Web.Models.Admin;
using Newtonsoft.Json;
using SocialCRM.Web.Models;
using SocialCRM.Web.Models.UnifiedDeck;
using CRM.Common.Enum;
using CRM.Entities;
using SocialCRM.Web.Models.Publisher;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/UnifiedDeck")]
    public class UnifiedDeckController : ApiController
    {

        IUnifiedDeckRepository repository;
        public UnifiedDeckController(IUnifiedDeckRepository repo)
        {
            repository = repo;
        }

        [HttpGet]
        [Route("GetUnifiedDeckForUserSelectedProfiles/{profileData}/{profilePageData}/{msgTypeData}/{tagTypeData}/{sentTypeData}/{pageNumber}/{pageSize}/{groupId}/{companyId}/{isAllTask}/{mediaType}")]
        public async Task<IHttpActionResult> GetUnifiedDeckForUserSelectedProfiles(string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, string pageNumber, int pageSize,
                                                                                int groupId, int companyId, bool isAllTask, int mediaType)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetUnifiedDeckForUserSelectedProfiles(userId, profileData, profilePageData, msgTypeData, tagTypeData,sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType);
            return Ok(status);
        }
        
        //GetLatestUnifiedDeckDataForSelectedProfiles/1,2,3/5,6/1,2,3,4,5,6,7/1,2/0/30/131/44/false/2/56-85-96-78-25
        [HttpGet]
        [Route("GetLatestUnifiedDeckForUserSelectedProfiles/{profileData}/{profilePageData}/{msgTypeData}/{tagTypeData}/{sentTypeData}/{pageNumber}/{pageSize}/{groupId}/{companyId}/{isAllTask}/{mediaType}/{ids}")]
        public async Task<IHttpActionResult> GetLatestUnifiedDeckForUserSelectedProfiles(string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, string pageNumber, int pageSize,
                                                                                int groupId, int companyId, bool isAllTask, int mediaType,string ids)
        {
            var userId = User.Identity.GetUserId<int>();
            var data = await repository.GetRecentUnifiedDeckForUserSelectedProfiles(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, mediaType, ids);
            return Ok(data);
        }

        [HttpGet]
        [Route("IsPostMarkedAsCompleted/{msgTypeData}/{ActivityId}")]
        public async Task<IHttpActionResult> IsPostMarkedAsCompleted(string msgTypeData, string ActivityId)
        {
            var userId = User.Identity.GetUserId<int>();
            var data = await repository.IsPostMarkedAsCompleted(msgTypeData, ActivityId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetNextUnifiedDeckForUserSelectedProfilesUnresponded/{profileData}/{profilePageData}/{msgTypeData}/{tagTypeData}/{sentTypeData}/{pageNumber}/{pageSize}/{groupId}/{companyId}/{isAllTask}/{maxUnrespondedPostIds}/{markCompleted}")]
        public async Task<IHttpActionResult> GetNextUnifiedDeckForUserSelectedProfilesUnresponded(string profileData, string profilePageData, string msgTypeData, string tagTypeData, string sentTypeData, int pageNumber, int pageSize,
                                                                                int groupId, int companyId, bool isAllTask, string maxUnrespondedPostIds, bool markCompleted)
        {
            var userId = User.Identity.GetUserId<int>();
            var data = await repository.GetNextUnifiedDeckForUserSelectedProfilesUnresponded(userId, profileData, profilePageData, msgTypeData, tagTypeData, sentTypeData, pageNumber, pageSize, groupId, companyId, isAllTask, maxUnrespondedPostIds, markCompleted);
            return Ok(data);
        }

        [HttpPost]
        [Route("UpdateTagsForFeed")]
        public async Task<IHttpActionResult> UpdateTagsForFeed(TagDataModel obj)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.InsertTagsForFeed(obj.feedId, obj.feedType, obj.tagIds, userId, obj.companyId);
            return Ok(status);
        }

        [HttpPost]
        [Route("InsertTagsForFeed")]
        //[Route("InsertTagsForFeed/{feedId}/{feedType}/{tagId}")]
        public async Task<IHttpActionResult> InsertTagsForFeed(TagDataModel receivedTagObject)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.InsertTagsForFeed(receivedTagObject.feedId, receivedTagObject.feedType, receivedTagObject.tagId, userId, receivedTagObject.companyId);
            return Ok(status);
        }

        [HttpPost]
        [Route("RemoveTagFromFeed")]
        //[Route("RemoveTagFromFeed/{feedId}/{feedType}/{tagId}")]
        public async Task<IHttpActionResult> RemoveTagFromFeed(TagDataModel receivedTagObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.RemoveTagFromFeed(receivedTagObject.feedId, receivedTagObject.feedType, receivedTagObject.tagId);
            return Ok(status);
        }

        [HttpPost]
        [Route("InsertSentimentForFeed")]
        public async Task<IHttpActionResult> InsertSentimentForFeed(SentimentDataModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int sentimentId = 0;
            if (model.sentiment == "Positive")
                sentimentId = (int)SentimentType.Positive;
            else if (model.sentiment == "Neutral")
                sentimentId = (int)SentimentType.Neutral;
            else if (model.sentiment == "Negative")
                sentimentId = (int)SentimentType.Negative;

            var status = await repository.InsertSentimentForFeed(model.feedId, model.feedType, sentimentId, userId, model.companyId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetUnrespondedQueriesColumnSettings")]
        public async Task<IHttpActionResult> GetUnrespondedQueriesColumnSettings()
        {
            var status = await repository.GetColumnSettings();
            return Ok(status);
        }

        [HttpPost]
        [Route("updateUnrespondedColumnSettings")]
        public async Task<IHttpActionResult> updateUnrespondedColumnSettings(ProfileQueueModel model)
        {
            //List<UnifiedDeckColumnSetting> model
            //PostData string
            List<UnifiedDeckColumnSetting> lstZones = JsonConvert.DeserializeObject<List<UnifiedDeckColumnSetting>>(model.PostData);
            var status = await repository.UpdateColumnSettings(lstZones);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetSampleRepliesText")]
        public async Task<IHttpActionResult> GetSampleRepliesText()
        {
            var status = await repository.GetSampleReplies();
            return Ok(status);
        }

        [HttpGet]
        [Route("DeleteFacebookItem/{userId}/{isPage}/{id}/{type}")]
        public async Task<IHttpActionResult> DeleteFacebookItem(string userId, bool isPage, string id, int type)
        {
            var teamId = User.Identity.GetUserId<int>();
            var status = await repository.DeleteFacebookItem(userId, isPage, id, (FacbookReplyType)type, teamId);
            return Ok(status);
        }

        [HttpGet]
        [Route("DeleteTwitterTweet/{userName}/{tweetId}")]
        public async Task<IHttpActionResult> DeleteTwitterTweet(string userName, string tweetId)
        {
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.DeleteTwitterTweet(userName, tweetId, userId);
            return Ok(status);
        }

        [HttpGet]
        [Route("DeleteTwitterDM/{userName}/{msgId}")]
        public async Task<IHttpActionResult> DeleteTwitterDM(string userName, string msgId)
        {
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.DeleteTwitterDM(userName, msgId, userId);
            return Ok(status);
        }

        [HttpGet]
        [Route("DeleteInstagramComment/{userId}/{commentId}")]
        public async Task<IHttpActionResult> DeleteInstagramComment(string userId, string commentId)
        {
            var teamId = User.Identity.GetUserId<int>();
            var status = await repository.DeleteInstagramComment(userId, commentId, teamId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetConsumerData/{profile}/{onProfile}/{isPage}/{feedType}")]
        public async Task<IHttpActionResult> GetConsumerData(string profile, string onProfile, bool isPage, string feedType)
        {
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.GetConsumerData(profile, onProfile, isPage, feedType);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetColumnsOrderForResponder/{companyId}")]
        public async Task<IHttpActionResult> GetColumnsOrderForResponder(int companyId)
        {
            var userId = User.Identity.GetUserId<int>();
            var data = await repository.GetColumnsOrderForResponder(userId.ToString(), companyId);
            return Ok(data);
        }

        [HttpPost]
        [Route("UpdateColumnsOrderForResponder")]
        public async Task<IHttpActionResult> UpdateColumnsOrderForResponder(ProfileQueueModel model)
        {
            //List<UnifiedDeckColumnSetting> model
            //PostData string
            var userId = User.Identity.GetUserId<int>();
            List<KeyValuePair<int, int>> lstColumnsOrder = JsonConvert.DeserializeObject<List<KeyValuePair<int, int>>>(model.PostData);
            var status = await repository.UpdateColumnsOrderForResponder(userId, model.CompanyId, lstColumnsOrder);
            return Ok(status);
        }
    }
}