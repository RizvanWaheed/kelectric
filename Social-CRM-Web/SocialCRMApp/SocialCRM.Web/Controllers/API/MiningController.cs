using CRM.Common;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Models.UnifiedInbox;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace SocialCRM.Web.Controllers.API
{
    [RoutePrefix("api/Mining")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MiningController : ApiController
    {
        // GET: Mining
        IMiningRepository repository;
        IProfileRepository proRepository;
        public MiningController(IMiningRepository repo, IProfileRepository proRepo)
        {
            repository = repo;
            proRepository = proRepo;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetFBPosts()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetFBPosts();
            return Ok(status);
        }

        public async Task<IHttpActionResult> GetFBComments()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetFBComments();
            return Ok(status);
        }

        public async Task<IHttpActionResult> GetTwitterTweets()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterTweets();
            return Ok(status);
        }

        public async Task<IHttpActionResult> GetTwitterMsgs()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterMsgs();
            return Ok(status);
        }

        [HttpGet]
        [Route("GetProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ProfileModel> status;
            if (isPersonal)
            {
                status = await proRepository.GetProfilesForUnifiedInboxPersonalMode(userId);
            }
            else
            {
                status = await proRepository.GetProfilesByGroupIdForUnifiedInbox(userId, groupId, companyId);
            }

            return Ok(status);
        }

        [HttpGet]
        [Route("GetProfilesByCompanyId/{companyId}")]
        public async Task<IHttpActionResult> GetProfilesByCompanyId(int companyId)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ProfileModel> status;
            status = await proRepository.GetProfilesByCompanyId(companyId);

            return Ok(status);
        }

        [HttpGet]
        [Route("GetProfilesWithOutGroupByCompanyId/{companyId}")]
        public async Task<IHttpActionResult> GetProfilesWithOutGroupByCompanyId(int companyId)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ProfileModel> status;
            status = await proRepository.GetAllProfilesWithoutGroupByCompanyId(companyId);

            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetUnifiedInboxForSelectedProfiles(InboxItemFilterModel model)
        {
            var userId = 0;
            if (model.userId.HasValue)
                userId = model.userId.Value;
            else
                userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<InboxItemModel> status = null;
            int pageTypeId = 0;
            if (model.pageType == "Unified")
                pageTypeId = 1;
            else if (model.pageType == "Archive")
                pageTypeId = 2;
            else if (model.pageType == "Completed")
                pageTypeId = 3;

            model.startDate = model.startDate.Date;
            model.endDate = model.endDate.Date.Add(new TimeSpan(23, 59, 59));

            status = await repository.GetUnifiedInboxForUserSelectedProfiles(pageTypeId, userId, model.profileData, model.profilePageData, model.msgTypeData, 
                model.tagTypeData, model.brandTypeData, model.sentTypeData, model.pageNumber, model.pageSize, model.teamData, model.groupId, model.companyId, model.isAllTask,
                model.consumerId, model.showComplaints, model.authorName, model.authorHandle, model.startDate, model.endDate);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetFeedsById/{ActivityId}/{ActivityType}")]
        public async Task<IHttpActionResult> GetFeedsById(int ActivityId, string ActivityType)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<InboxItemModel> status = null;

            status = await repository.GetFeedsById(ActivityId, ActivityType);


            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddMsgToArchiveList(MsgArchiveModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.UserId = userId;
            var status = await repository.AddMsgToArchiveList(model);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> RemoveMsgFromArchiveList(MsgArchiveModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.UserId = userId;
            var status = await repository.RemoveMsgFromArchiveList(model);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetSocialProfiles/{UserIds}/{CommaSeperatedPages}/{GroupId}")]
        public async Task<IHttpActionResult> GetSocialProfiles(string UserIds, string CommaSeperatedPages, int GroupId)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<SelectListItem> result = null;

            result = await repository.GetCustomerSocialProfiles(UserIds,CommaSeperatedPages,GroupId);


            return Ok(result);
        }

        [HttpGet]
        [Route("SocialActivityExists/{userId}/{isPage}/{postId}/{feedType}/{activityTypeId}")]
        public async Task<IHttpActionResult> SocialActivityExists(string userId, bool isPage, string postId, string feedType, int activityTypeId)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await repository.SocialActivityExists(userId, isPage, postId, feedType, activityTypeId);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetConsumerNumbersByProfile/{ProfileId}/{CommaSeperatedProfiles}/{CommaSeperatedPages}/{GroupId}")]
        public async Task<IHttpActionResult> GetConsumerNumbersByProfile(string ProfileId, string CommaSeperatedProfiles, string CommaSeperatedPages, int GroupId)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<SelectListItem> result = null;
            result = await repository.GetConsumerNumbers(ProfileId, CommaSeperatedProfiles, CommaSeperatedPages, GroupId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetLatestUnifiedInboxDataForSelectedProfiles(InboxItemFilterModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            model.startDate = model.startDate.Date;
            model.endDate = model.endDate.Date.Add(new TimeSpan(23, 59, 59));

            var data = await repository.GetData(userId,model.profileData, model.profilePageData, model.pageType, model.msgTypeData, model.tagTypeData, 
                model.brandTypeData, model.sentTypeData, model.pageNumber, model.pageSize, model.teamData, model.groupId, model.companyId, model.isAllTask, model.showComplaints,
                model.consumerId, model.authorName, model.authorHandle, model.startDate, model.endDate, model.maxIds);
            return Ok(data);
        }

        [HttpGet]
        [Route("DeleteTag/{tagId}")]
        public async Task<IHttpActionResult> DeleteTag(int tagId)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool result = false;
            result = await repository.DeleteTag(tagId);
            return Ok(result);
        }

        [HttpPost]
        [Route("EditTag")]
        public async Task<IHttpActionResult> EditTag(TagModel model)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool result = false;
            result = await repository.EditTag(model);
            return Ok(result);
        }

        [HttpGet]
        [Route("DeleteKeyword/{keywordId}/{groupId}")]
        public async Task<IHttpActionResult> DeleteKeyword(int keywordId, int groupId)
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool result = false;
            result = await repository.DeleteKeyword(keywordId, groupId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetActivePattern")]
        public async Task<IHttpActionResult> GetActivePattern()
        {
            //   var userId = User.Identity.GetUserId<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }             
            string result = await repository.GetActivePattern();
            return Ok(result);
        }

    }
}