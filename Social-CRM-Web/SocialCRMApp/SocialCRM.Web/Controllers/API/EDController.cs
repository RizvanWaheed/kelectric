using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SocialCRM.Web.Repositories.Core;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.UnifiedContact;
using CRM.Entities;
using Microsoft.Owin;
using SocialCRM.Web.Models.UnifiedInbox;
using SocialCRM.Web.Models.Facebook;
using SocialAPIsWrapper.Models;
using System.Web.Http.Cors;
using SocialCRM.Web.Repositories;
using SocialCRM.Web.Models.Report;
using SocialCRM.Web.Models.TaskManager;
using SocialCRM.Web.Models.Profiles;
using CRM.Common.Enum;

namespace SocialCRM.Web.Controllers.API
{
    [RoutePrefix("api/ED")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EDController : ApiController
    {
        IFacebookRepository repository;
        ITaskRepository taskRepository;
        IReportRepository reportRepository;
        IProfileRepository proRepository;
        private InstagramRepository InstagramRepostory;
        IUnifiedDeckRepository unirepository;
        public EDController(IFacebookRepository repo, ITaskRepository repo2, IReportRepository repo3, IProfileRepository repo4, InstagramRepository repo5, IUnifiedDeckRepository repo6)
        {
            repository = repo;
            taskRepository = repo2;
            reportRepository = repo3;
            proRepository = repo4;
            InstagramRepostory = repo5;
            unirepository = repo6;
        }

        [HttpGet]
        [Route("GetConsumerData/{profile}/{onProfile}/{isPage}/{feedType}")]
        public async Task<IHttpActionResult> GetConsumerData(string profile, string onProfile, bool isPage, string feedType)
        {            
            var status = await unirepository.GetConsumerData(profile, onProfile, isPage, feedType);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetInstagramPostDetails/{PostId}")]
        public async Task<IHttpActionResult> GetInstagramPostDetails(string PostId)
        {
            try
            {
                //   var userId = User.Identity.GetUserId<int>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await InstagramRepostory.GetInstagramPostDetails(PostId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetBrandKeywordByCompanyId/{companyId}")]
        public async Task<IHttpActionResult> GetBrandKeywordByCompanyId(int companyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var taskResult = await taskRepository.GetBrandKeywordByCompanyId(companyId);
            return Ok(taskResult);
        }

        [HttpGet]
        [Route("GetProfilesByGroupId/{groupId}/{companyId}/{isPersonal}/{userId}")]
        public async Task<IHttpActionResult> GetProfilesByGroupId(int groupId, int companyId, bool isPersonal , int userId)
        {           
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
        public async Task<IHttpActionResult> FacebookGetUserConversation(string conId)
        {
            var data = await repository.FacebookGetUserConversation(conId);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IHttpActionResult> FacebookGetPageReportStats(FacebookPageReportInputModel model)
        {
          
            var userId = 1;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = await repository.FacebookGetPageReportStats(model.PageId, model.From, model.To);
            return Ok(status);
        }
        [HttpGet]
        [Route("FacebookGetPostInfo/{postId}/{commentId}")]
        public async Task<IHttpActionResult> FacebookGetPostInfo(string postId, string commentId)
        {
            var data = await repository.FacebookGetPostInfo(postId, commentId);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetInOutBoundReport(InOutBoundFilterModel model)
        {
            model.dateFrom = model.dateFrom.Date;
            model.dateTo = model.dateTo.Date.Add(new TimeSpan(23, 59, 59));
            var userId = 0;
            if (model.userId.HasValue)
                userId = model.userId.Value;
            else
                userId = User.Identity.GetUserId<int>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            InOutBoundReportContainer retObj = new InOutBoundReportContainer();

            string report = model.report, reportType = model.reportType;
            var status = reportRepository.GetInOutBoundReport(model);

            

            List<InOutBoundReport> result = new List<InOutBoundReport>();     
            List<SentimentsPerChannel> sentimentsReport = new List<SentimentsPerChannel>();    
         
            List<ProfileModel> profilesTemp = await proRepository.GetProfilesByGroupIdForUnifiedInbox(userId, model.groupId, model.companyId); //await proRepository.GetProfilesByCompanyId(model.companyId);
            List<ProfileModel> profiles = new List<ProfileModel>();

          
            // profiles
            if (model.profileIds != "0")
            {
                var profileIdsList = model.profileIds.Split(',').ToList();
                foreach (var p in profilesTemp)
                {
                    if (profileIdsList.Contains(p.ProfileId.ToString()) && !p.IsPage)
                    {
                        profiles.Add(p);
                    }
                }
            }

            if (model.profilePageIds != "0")
            {
                var profilePageIdsList = model.profilePageIds.Split(',').ToList();
                foreach (var p in profilesTemp)
                {
                    if (profilePageIdsList.Contains(p.ProfileId.ToString()) && p.IsPage)
                    {
                        profiles.Add(p);
                    }
                }
            }

            if (model.profilePageIds == "0" && model.profileIds == "0")
            {
                profiles = profilesTemp;
            }

            if (report == "InOutBound")
            {
                if (reportType == "date")
                {
                    result = status.GroupBy(p => new { Date = p.CreatedDate.Date, p.Volume })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = g.Key.Volume,
                                                Date = g.Key.Date,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Date).ToList();
                }
            }
            else if (report == "Sentiment")
            {
                if (reportType == "date")
                {
                    result = status.Where(x => x.Sentiment != null && x.Sentiment != 4 && x.Volume == "Inbound").GroupBy(p => new { Date = p.CreatedDate.Date, p.Sentiment })
                                        .Select(
                                            g => new InOutBoundReport
                                            {
                                                Name = GetSentiment(g.Key.Sentiment),
                                                Date = g.Key.Date,
                                                Count = g.Count()
                                            }).OrderBy(x => x.Date).ToList();
                }
            }
            

            try
            {
                foreach (var profile in profiles)
                {
                    if (profile.ProfileTypeId == 1 || profile.ProfileTypeId == 5)
                    {
                        profile.ProfileName = profile.ProfileName + "(FB)";
                    }
                    else if (profile.ProfileTypeId == 4)
                    {
                        profile.ProfileName = profile.ProfileName + "(TW)";
                    }
                    else if (profile.ProfileTypeId == 6)
                    {
                        profile.ProfileName = profile.ProfileName + "(IN)";
                    }

                    if (profile.IsPage)
                    {
                        sentimentsReport.Add(
                                    new SentimentsPerChannel()
                                    {
                                        Negative = status.Where(x => x.Sentiment == (int)SentimentType.Negative && x.Volume == "Inbound" && x.ProfilePageId.HasValue && x.ProfilePageId.Value == profile.ProfileId).Count(),
                                        Neutral = status.Where(x => x.Sentiment == (int)SentimentType.Neutral && x.Volume == "Inbound" && x.ProfilePageId.HasValue && x.ProfilePageId.Value == profile.ProfileId).Count(),
                                        Positive = status.Where(x => x.Sentiment == (int)SentimentType.Positive && x.Volume == "Inbound" && x.ProfilePageId.HasValue && x.ProfilePageId.Value == profile.ProfileId).Count(),
                                        Profile = profile
                                    }
                                );
                        
                    }
                    else
                    {
                        sentimentsReport.Add(
                                    new SentimentsPerChannel()
                                    {
                                        Negative = status.Where(x => x.Sentiment == (int)SentimentType.Negative && x.Volume == "Inbound" && x.ProfileId == profile.ProfileId).Count(),
                                        Neutral = status.Where(x => x.Sentiment == (int)SentimentType.Neutral && x.Volume == "Inbound" && x.ProfileId == profile.ProfileId).Count(),
                                        Positive = status.Where(x => x.Sentiment == (int)SentimentType.Positive && x.Volume == "Inbound" && x.ProfileId == profile.ProfileId).Count(),
                                        Profile = profile
                                    }
                                );
                       
                    }
                }
            }
            catch (Exception ex)
            {
            }

            retObj.InOutBoundReport = result;
            retObj.SentimentsPerChannel = sentimentsReport;
        
      

            return Ok(retObj);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetInOutBoundReportDrillDown(InOutBoundFilterModel model)
        {
            model.dateFrom = model.dateFrom.Date;
            model.dateTo = model.dateTo.Date.Add(new TimeSpan(23, 59, 59));
            var userId = 0;
            if (model.userId.HasValue)
                userId = model.userId.Value;
            else
                userId = User.Identity.GetUserId<int>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            InOutBoundReportContainer retObj = new InOutBoundReportContainer();

            string report = model.report, reportType = model.reportType;
            List<InOutBoundReportDrillDown> result = new List<InOutBoundReportDrillDown>();
            try
            {
                var status = reportRepository.GetInOutBoundReportDrillDown(model);
          


           

            List<ProfileModel> profilesTemp = await proRepository.GetProfilesByGroupIdForUnifiedInbox(userId, model.groupId, model.companyId); //await proRepository.GetProfilesByCompanyId(model.companyId);
            List<ProfileModel> profiles = new List<ProfileModel>();


            // profiles
            if (model.profileIds != "0")
            {
                var profileIdsList = model.profileIds.Split(',').ToList();
                foreach (var p in profilesTemp)
                {
                    if (profileIdsList.Contains(p.ProfileId.ToString()) && !p.IsPage)
                    {
                        profiles.Add(p);
                    }
                }
            }

            if (model.profilePageIds != "0")
            {
                var profilePageIdsList = model.profilePageIds.Split(',').ToList();
                foreach (var p in profilesTemp)
                {
                    if (profilePageIdsList.Contains(p.ProfileId.ToString()) && p.IsPage)
                    {
                        profiles.Add(p);
                    }
                }
            }

            if (model.profilePageIds == "0" && model.profileIds == "0")
            {
                profiles = profilesTemp;
            }

            if (report == "InOutBound")
            {
                if (reportType == "date")
                {
                    result = status.Select(g => new InOutBoundReportDrillDown
                                            {
                                                Volume = g.Volume,
                                                Date = g.CreatedDate,
                                                StatusText = g.StatusText,
                                                PostType = g.PostType
                    }).ToList();
                }
            }


                //  retObj.InOutBoundReport = result;
                //  retObj.SentimentsPerChannel = sentimentsReport;

            }
            catch (Exception ex)
            {
                int i = 0;
            }

            return Ok(result);
        }
        [HttpGet]
        [Route("GetTwitterProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetTwitterProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = 1;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ContactFollowStatusModel> status;
            if (isPersonal)
            {
                status = await proRepository.GetSocialProfilesPersonalMode(userId, 4);
            }
            else
            {
                status = await proRepository.GetSocialProfilesByGroupId(userId, groupId, companyId, 4);
            }

            return Ok(status);
        }

        [HttpGet]
        [Route("GetFacebookProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetFacebookProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = 1;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ContactFollowStatusModel> status;
            if (isPersonal)
            {
                status = await proRepository.GetSocialProfilesPersonalMode(userId, 1);
            }
            else
            {
                status = await proRepository.GetSocialProfilesByGroupId(userId, groupId, companyId, 1);
            }
            return Ok(status);
        }

        [HttpGet]
        [Route("GetInstagramProfilesByGroupId/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetInstagramProfilesByGroupId(int groupId, int companyId, bool isPersonal)
        {
            var userId = 1;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ContactFollowStatusModel> status;
            if (isPersonal)
            {
                status = await proRepository.GetSocialProfilesPersonalMode(userId, 6);
            }
            else
            {
                status = await proRepository.GetSocialProfilesByGroupId(userId, groupId, companyId, 6);
            }
            return Ok(status);
        }

        //reportDate => dd_MM_yyyy api/ED/BuildAgentPerformanceReport/18_10_2018
        [HttpGet]
        [AllowAnonymous]
        [Route("BuildAgentPerformanceReport/{reportDate}")]
        public async Task<IHttpActionResult> BuildAgentPerformanceReport(string reportDate)
        {
            if (string.IsNullOrEmpty(reportDate))
            {
                return BadRequest(ModelState);
            }
            reportRepository.BuildAgentPerformanceReport(reportDate);
            return Ok();
        }

        private string GetSentiment(int? sentiment)
        {
            switch (sentiment)
            {
                case (int)SentimentType.Positive:
                    return SentimentType.Positive.ToString();
                case (int)SentimentType.Neutral:
                    return SentimentType.Neutral.ToString();
                case (int)SentimentType.Negative:
                    return SentimentType.Negative.ToString();
                default:
                    return "Unknown";
            }
        }

    }
}