using CRM.APIS.Instagram;
using CRM.Common.Enum;
using Microsoft.AspNet.Identity;
using SocialCRM.Web.Models.Instagram;
using SocialCRM.Web.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/Instagram")]
    public class InstagramController : ApiController
    {
        private InstagramRepository _InstagramRepostory;
        public InstagramRepository InstagramRepostory
        {
            get
            {
                return _InstagramRepostory ?? new InstagramRepository();
            }
            private set
            {
                _InstagramRepostory = value;
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetInstagramReports(InstagramFilterModel searchFilter)
        {
            var userId = User.Identity.GetUserId<int>();
            var report =  InstagramRepostory.GetInstagramReports(searchFilter,userId).Result;
            //Here we get TOP INFLUENCERS ENGAGED by API.
           /* if (!string.IsNullOrEmpty(report.GeneralStatics.AccessToken))
            {
                report.TopFollower = new List<InfluencersEngaged>();
                // Create a request for the URL. 
                WebRequest request = WebRequest.Create(
                  string.Format("https://api.instagram.com/v1/users/self/follows?access_token={0}",report.GeneralStatics.AccessToken));
                // If required by the server, set the credentials.
                request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                var displayStatus = ((HttpWebResponse)response).StatusDescription;
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine(responseFromServer);
                // Clean up the streams and the response.
                reader.Close();
                response.Close();
            }*/
            return Ok(report);
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

        [HttpPost]
        [Route("PostComment/")]
        public async Task<IHttpActionResult> PostComment(InstagramCommentModel model)
        {
            try
            {
                //   var userId = User.Identity.GetUserId<int>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.Identity.GetUserId<int>();
                var result = await InstagramRepostory.PostInstagramComment(model.PostId, model.UserId, model.Comment, userId,model.CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("DeleteComment/{PostId}/{UserId}/{CommentId}")]
        public async Task<IHttpActionResult> DeleteComment(string PostId, string UserId, string CommentId)
        {
            try
            {
                //   var userId = User.Identity.GetUserId<int>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await InstagramRepostory.DeleteInstagramComment(PostId, UserId, CommentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("LikePost/{PostId}/{UserId}")]
        public async Task<IHttpActionResult> LikePost(string PostId, string UserId)
        {
            try
            {
                //   var userId = User.Identity.GetUserId<int>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await InstagramRepostory.LikeInstagramPost(PostId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("UnlikePost/{PostId}/{UserId}")]
        public async Task<IHttpActionResult> UnlikePost(string PostId, string UserId)
        {
            try
            {
                //   var userId = User.Identity.GetUserId<int>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await InstagramRepostory.UnlikeInstagramPost(PostId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("IsPostLiked/{PostId}/{UserId}")]
        public async Task<IHttpActionResult> IsPostLiked(string PostId, string UserId)
        {
            try
            {
                //   var userId = User.Identity.GetUserId<int>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await InstagramRepostory.IsInstagramPostLiked(PostId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
