using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SocialCRM.Web.Repositories;
using System.Web.Mvc;
using SocialCRM.Web.GoogleAnalytics;
using CRM.Common.Enum;
using Microsoft.AspNet.Identity;
using System.Web.Script.Serialization;
using SocialCRM.Web.Models.Facebook;
using SocialCRM.Web.Models.UnifiedInbox;
using System.Web.Http.Cors;

namespace SocialCRM.Web.Controllers.API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FacebookController : Controller
    {
        private FacebookRepository _FacebookRepository;
        public FacebookRepository FacebookRepository
        {
            get
            {
                return _FacebookRepository ?? new FacebookRepository();
            }
            private set
            {
                _FacebookRepository = value;
            }
        }

        /// <summary>
        /// This action return the facebook pages for a specific company and group.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetFacebookPages(string parm1)
        {
            if (string.IsNullOrEmpty(parm1) || !parm1.Contains(","))
                return Json("Invalid Request.", JsonRequestBehavior.AllowGet);
            var requestInfo = parm1.Split(',').Select(int.Parse).ToArray();
            //User,Company,Group
            if (requestInfo.Count() < 4)
                return Json("Invalid Request.", JsonRequestBehavior.AllowGet);
            var userId = this.User.Identity.GetUserId<int>();
            if (userId != requestInfo[0])
                return Json("Invalid Request.", JsonRequestBehavior.AllowGet);
            var companyId = requestInfo[1];
            var groupId = requestInfo[2];
            var profileTransactId = requestInfo[3];

            var fbPages = FacebookRepository.GetUserFBPages(companyId, groupId, profileTransactId);
            if (fbPages == null)
            {
                return Json(new Exception("No Pages Found."), JsonRequestBehavior.AllowGet);
            }
            return Json(fbPages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddFacebookPages(string parm1, int companyId, int groupId)
        {
            var userId = this.User.Identity.GetUserId<int>();
            var result = new JavaScriptSerializer().Deserialize<List<FacebookPageModel>>(parm1);
            if (result.Count > 0)
            {
                var response = FacebookRepository.ActivateFacebookPages(result, companyId, groupId, userId);
                return Json(new { Status = "success", Result = response }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = "error", Result = "account info req." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> FacebookLikePost(string userId, bool isPage, string postId, bool isPost)
        {
            try
            {
                await FacebookRepository.FacebookLikePost(userId, isPage, postId, isPost);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Like was successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> FacebookIsPostLiked(string userId, bool isPage, string postId, bool isPost)
        {
            try
            {
                bool result = await FacebookRepository.FacebookIsPostLiked(userId, isPage, postId, isPost);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> FacebookUnLikePost(string userId, bool isPage, string postId, bool isPost)
        {
            try
            {
                await FacebookRepository.FacebookUnLikePost(userId, isPage, postId, isPost);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Unlike was successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> FacebookCommentPost(FacebookPostModel model)
        {
            try
            {
                var userId = User.Identity.GetUserId<int>();
                if (model.status == null)
                    model.status = "";
                await FacebookRepository.FacebookCommentPost(model.userId, model.isPage, model.postId, model.status, model.filePath, model.contentType, model.fileName,userId,model.companyId,(FacbookReplyType)model.ReplyType);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Comment Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        //[Route("FacebookNewPost/{userId}/{isPage}/{status}")]
        public async Task<ActionResult> FacebookNewPost(FacebookPostModel model)
        {
            try {
                var userId = User.Identity.GetUserId<int>();
                await FacebookRepository.FacebookNewPost(model.userId, model.isPage, model.status, model.filePath, model.contentType, model.fileName, model.placeId, userId, model.companyId);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Post Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        //[Route("FacebookNewPost/{userId}/{isPage}/{status}")]
        public async Task<ActionResult> FacebookNewPageChat(FacebookPageChatMessageModel model)
        {
            try {
                var userId = User.Identity.GetUserId<int>();
                await FacebookRepository.FacebookNewPageChat(model.userId, model.conId, model.isPage, model.status,userId,model.companyId);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Page Chat Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> FacebookDeleteObject(string userId, bool isPage, string objectId)
        {
            try
            {
                await FacebookRepository.FacebookDeleteObject(userId, isPage, objectId);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Delete Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> FacebookUpdateMessage(string userId, bool isPage, string objectId, string message, string msgType)
        {
            try
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("message", message);
                await FacebookRepository.FacebookUpdateMessage(userId, isPage, objectId, dict, msgType);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Update Message Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> FacebookHideUnhidePost(string userId, bool isPage, string objectId, bool hide)
        {
            try
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("is_hidden", hide);
                await FacebookRepository.FacebookHideUnhidePost(userId, isPage, objectId, dict);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = "Update Message Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> FacebookIsPostHidden(string userId, bool isPage, string postId)
        {
            try
            {
                bool result = await FacebookRepository.FacebookIsPostHidden(userId, isPage, postId);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { success = true, Message = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> FacebookGetAllPostInfo(string userId, bool isPage, string postId)
        {
            var data = await FacebookRepository.FacebookGetAllPostInfo(userId, isPage, postId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> FacebookGetUserConversation(string conId)
        {
            var data = await FacebookRepository.FacebookGetUserConversation(conId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> FacebookGetPageReportStats(FacebookPageReportInputModel model)
        {
            //string pageId = "441450792716626";
            //DateTime to = new DateTime(2015, 12, 27);
            //DateTime from = new DateTime(2015, 10, 01);
            var data = await FacebookRepository.FacebookGetPageReportStats(model.PageId, model.From, model.To);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("FacebookGetPostInfo/{postId}/{commentId}")]
        public async Task<ActionResult> FacebookGetPostInfo(string postId, string commentId)
        {
            var data = await FacebookRepository.FacebookGetPostInfo(postId, commentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetFacebookLocation/{userId}/{isPage}/{locationQuery}")]
        public async Task<ActionResult> GetFacebookLocation(string userId, bool isPage, string locationQuery)
        {

            dynamic data = await FacebookRepository.GetFacebookPlaces(userId, isPage, locationQuery);
            var list = (from dynamic location in (IList<object>)data["data"]
                       select new
                       {
                           id = (string)location.id,
                           name = (string)location.name
                       });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> FacebookGetMoreRecentPosts(FacebookPageReportInputModel model)
        {
            var data = await FacebookRepository.LoadMoreRecentPosts(model.PageId ,model.lastPostId, model.From, model.To);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}