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
using SocialCRM.Web.Models.Twitter;
using SocialAPIsWrapper.Models;
using System.Web.Http.Cors;

namespace SocialCRM.Web.Controllers.API
{
    [RoutePrefix("api/Twitter")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TwitterController : ApiController
    {
        ITwitterRepository repository;
        public TwitterController(ITwitterRepository repo)
        {
            repository = repo;
        }

        [HttpGet]
        [Route("GetTwitterProfile/{fromUser}/{ofUser}")]
        public async Task<IHttpActionResult> GetTwitterProfile(string fromUser, string ofUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterProfile(fromUser, ofUser);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetFriendshipInfo/{fromUser}/{ofUser}/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetFriendshipInfo(string fromUser, string ofUser, int groupId, int companyId, bool isPersonal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.GetTwitterFriendshipStatus(fromUser, ofUser, userId, groupId, companyId, isPersonal);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterFriends/{fromUser}/{ofUser}/{cursor}")]
        public async Task<IHttpActionResult> GetTwitterFriends(string fromUser, string ofUser, long cursor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterFriends(fromUser, ofUser, cursor);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterFollowers/{fromUser}/{ofUser}/{cursor}")]
        public async Task<IHttpActionResult> GetTwitterFollowers(string fromUser, string ofUser, long cursor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterFollowers(fromUser, ofUser, cursor);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterListMembers/{fromUser}/{listId}/{cursor}")]
        public async Task<IHttpActionResult> GetTwitterListMembers(string fromUser, ulong listId, long cursor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterListMembers(fromUser, listId, cursor);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterTweets/{fromUser}/{ofUser}/{count}/{maxId}")]
        public async Task<IHttpActionResult> GetTwitterTweets(string fromUser, string ofUser, int count, ulong maxId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterTweets(fromUser, ofUser, count, maxId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTweetDirectReplies/{fromUser}/{ofUser}/{tweetId}")]
        public async Task<IHttpActionResult> GetTweetDirectReplies(string fromUser, string ofUser, ulong tweetId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTweetDirectReplies(fromUser, ofUser, tweetId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTweetMinnedReplies/{ofUser}/{tweetId}")]
        public async Task<IHttpActionResult> GetTweetMinnedReplies(string ofUser, ulong tweetId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTweetMinnedReplies(ofUser, tweetId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterDirectMessages/{fromUser}/{ofUser}")]
        public async Task<IHttpActionResult> GetTwitterDirectMessages(string fromUser, string ofUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterDirectMessages(fromUser, ofUser);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterHomeTweets/{fromUser}/{ofUser}/{count}/{maxId}")]
        public async Task<IHttpActionResult> GetTwitterHomeTweets(string fromUser, string ofUser, int count, ulong maxId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterHomeTweets(fromUser, ofUser, count, maxId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterListTweets/{fromUser}/{listId}/{count}/{maxId}")]
        public async Task<IHttpActionResult> GetTwitterListTweets(string fromUser, ulong listId, int count, ulong maxId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterListTweets(fromUser, listId, count, maxId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterMentions/{fromUser}/{ofUser}/{count}/{maxId}")]
        public async Task<IHttpActionResult> GetTwitterMentions(string fromUser, string ofUser, int count, ulong maxId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTwitterMentions(fromUser, ofUser, count, maxId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetTwitterHistory/{fromUser}/{ofUser}/{count}/{pageNumber}/{groupId}/{companyId}/{isPersonal}")]
        public async Task<IHttpActionResult> GetTwitterHistory(string fromUser, string ofUser, int count, int pageNumber, int groupId, int companyId, bool isPersonal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.GetTwitterHistory(userId, ofUser, pageNumber, count, groupId, companyId, isPersonal);
            return Ok(status);
        }

        [HttpPost]
        [Route("TwitterFollowUser/{fromUser}/{ofUser}")]
        public async Task<IHttpActionResult> TwitterFollowUser(string fromUser, string ofUser)
        {
            try
            {
                await repository.TwitterFollowUser(fromUser, ofUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("TwitterUnfollowUser/{fromUser}/{ofUser}")]
        public async Task<IHttpActionResult> TwitterUnfollowUser(string fromUser, string ofUser)
        {
            try
            {
                await repository.TwitterUnfollowUser(fromUser, ofUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> TwitterNewTweet(PostTweetModel data)
        {
            try
            {
                var userId = User.Identity.GetUserId<int>();
                await repository.TwitterNewTweet(data.fromUser, data.status, data.mediaInfo, data.lat, data.lng, userId, data.companyId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*[HttpPost]
        public async Task<IHttpActionResult> TwitterNewTweetMiner(PostTweetModel data)
        {
            List<MediaModelTwitter> mediaInfo = new List<MediaModelTwitter>();
            if (data.mediaString != null)
            {
                string[] files = data.mediaString.Split('|');
                foreach (var item in files)
                {
                    string[] properties = item.Split(',');
                    mediaInfo.Add(new MediaModelTwitter
                    {
                        FileName = properties[0],
                        ContentType = properties[1],
                        Base64String = properties[2]
                    });
                }
                data.mediaInfo = mediaInfo;
            }

            try
            {
                await repository.TwitterNewTweet(data.fromUser, data.status, data.mediaInfo, data.lat, data.lng);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/

        [HttpPost]
        [Route("TwitterDeleteTweet/{fromUser}/{tweetId}")]
        public async Task<IHttpActionResult> TwitterDeleteTweet(string fromUser, ulong tweetId)
        {
            try
            {
                await repository.TwitterDeleteTweet(fromUser, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("TwitterSendDirectMessage/{fromUser}/{toUser}/{status}")]
        public async Task<IHttpActionResult> TwitterSendDirectMessage(string fromUser, string toUser, string status)
        {
            try
            {
                await repository.TwitterSendDirectMessageEvent(fromUser, toUser, status);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        //[Route("TwitterSendDM/{userId}/{fromUser}/{toUser}/{status}")]
        public async Task<IHttpActionResult> TwitterSendDM(TwitterDirectMessageModel model)
        {
            try
            {
                var userId = User.Identity.GetUserId<int>();
                await repository.TwitterSendDMEvent(model.userId, model.fromUser, model.toUser, model.status, userId, model.companyId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPost]
        [Route("TwitterDeleteDirectMessage/{fromUser}/{messageId}")]
        public async Task<IHttpActionResult> TwitterDeleteDirectMessage(string fromUser, ulong messageId)
        {
            try
            {
                await repository.TwitterDeleteDirectMessageEvent(fromUser, messageId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> TwitterReplyTweet(PostTweetModel data)
        {
            try
            {
                var userId = User.Identity.GetUserId<int>();
                await repository.TwitterReplyTweet(data.fromUser, data.tweetId, data.status, data.mediaInfo, userId, data.companyId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTwitterConversation/{fromUser}/{ofUser}/{count}/{pageNumber}/{groupId}/{companyId}/{statusId}/{Mtype}")]
        public async Task<IHttpActionResult> GetTwitterConversation(string fromUser, string ofUser, int count, int pageNumber, int groupId, int companyId, string statusId, string Mtype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.GetTwitterConversation(userId, ofUser, pageNumber, count, groupId, companyId, statusId, Mtype);
            return Ok(status);
        }

        [HttpPost]
        [Route("TwitterRetweet/{fromUser}/{tweetId}")]
        public async Task<IHttpActionResult> TwitterRetweet(string fromUser, ulong tweetId)
        {
            try
            {
                await repository.TwitterRetweet(fromUser, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("TwitterUnRetweet/{fromUser}/{tweetId}")]
        public async Task<IHttpActionResult> TwitterUnRetweet(string fromUser, ulong tweetId)
        {
            try
            {
                await repository.TwitterUnRetweet(fromUser, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> TwitterRetweetWithComment(PostTweetModel data)
        {
            try
            {
                var userId = User.Identity.GetUserId<int>();
                await repository.TwitterRetweetWithComment(data.fromUser, data.authorUserName, data.tweetId, data.status, data.mediaInfo, userId,data.companyId);                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("TwitterIsFavorited/{fromUser}/{tweetId}")]
        public async Task<IHttpActionResult> TwitterIsFavorited(string fromUser, string tweetId)
        {
            try
            {
                bool result = await repository.TwitterIsFavorited(fromUser, tweetId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("TwitterIsRetweeted/{fromUser}/{tweetId}")]
        public async Task<IHttpActionResult> TwitterIsRetweeted(string fromUser, ulong tweetId)
        {
            try
            {
                string result = await repository.TwitterIsRetweeted(fromUser, tweetId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("TwitterMarkFavorite/{fromUser}/{tweetId}")]
        public async Task<IHttpActionResult> TwitterMarkFavorite(string fromUser, string tweetId)
        {
            try
            {
                await repository.TwitterMarkFavorite(fromUser, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("TwitterUnmarkFavorite/{fromUser}/{tweetId}")]
        public async Task<IHttpActionResult> TwitterUnmarkFavorite(string fromUser, string tweetId)
        {
            try
            {
                await repository.TwitterUnmarkFavorite(fromUser, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("TwitterReportSpam/{fromUser}/{userId}")]
        public async Task<IHttpActionResult> TwitterReportSpam(string fromUser, ulong userId)
        {
            try
            {
                await repository.TwitterReportSpam(fromUser, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserContactInfo/{ofUser}/{groupId}/{companyId}")]
        public async Task<IHttpActionResult> GetUserContactInfo(string ofUser, int groupId, int companyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetUserContactInfo(ofUser, groupId, companyId);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserContactInfo(ContactExtraInfoModel contactObj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.UpdateUserContactInfo(contactObj);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> InsertUserContactComment(ContactExtraInfoModel contactObj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            using (var context = new SocialCRMEntities())
            {
                contactObj.Comments[0].CreatedBy = context.AspNetUsers.Where(x => x.Id == userId).Select(y => y.Name).FirstOrDefault();
            }
            
            contactObj.Comments[0].CreatedAt = DateTime.Now;
            var status = await repository.InsertUserContactComment(contactObj);
            return Ok(status);
        }

        [HttpPost]
        [Route("DeleteUserContactComment/{CommentId}")]
        public async Task<IHttpActionResult> DeleteUserContactComment(int CommentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.DeleteUserContactComment(CommentId);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetUsersOwnedTwitterLists/{ofUser}/{groupId}/{companyId}/{isPersonal}/{checkMembership}")]
        public async Task<IHttpActionResult> GetUsersOwnedTwitterLists(string ofUser, int groupId, int companyId, bool isPersonal, bool checkMembership)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var status = await repository.GetUsersOwnedTwitterLists(ofUser, userId, groupId, companyId, isPersonal, checkMembership);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddContactToUserLists(AddContactToSelectedListsViewModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.Run(() =>
            {
                repository.AddContactToUserLists(data.OfUser, data.ListIds);
            });
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> RemoveContactFromUserList(AddContactToSelectedListsViewModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.Run(() =>
            {
                repository.RemoveContactFromUserList(data.OfUser, data.ListIds[0]);
            });
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateNewList(NewListModel listModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.CreateNewList(listModel.FromUser, listModel.OfUser, listModel.ListName, listModel.Mode, listModel.Description);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditTwitterList(ContactTwitListModel listModel)
        {
            await Task.Run(() =>
            {
                repository.EditTwitterList(listModel);
            });
            return Ok();
        }

        [HttpPost]
        [Route("DeleteTwitterList/{fromUser}/{listId}")]
        public async Task<IHttpActionResult> DeleteTwitterList(string fromUser, ulong listId)
        {
            await Task.Run(() =>
            {
                repository.DeleteTwitterList(fromUser, listId);
            });
            return Ok();
        }

        [HttpGet]
        [Route("GetContactOwnedSocialProfiles/{ofUser}")]
        public async Task<IHttpActionResult> GetContactOwnedSocialProfiles(string ofUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetContactOwnedSocialProfiles(ofUser);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetProfilesReportStats(TwitterProfileReportInputModel model)
        {
            var status = await repository.GetProfilesReportStats(model.Profiles, model.From, model.To);
            return Ok(status);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetProfilesReportStatsED(TwitterProfileReportInputModel model)
        {
            var status = await repository.GetProfilesReportStatsED(model.Profiles, model.From, model.To);
            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetProfilesReportStatsEDDrillDown(TwitterProfileDrillDownReportInputModel model)
        {
            var status = await repository.GetProfilesReportStatsEDDrillDown(model.Profiles, model.From, model.To,model.ReportType,model.PageNumber,model.PageSize);
            return Ok(status);
        }


        [HttpGet]
        [Route("GetTweetThreadByTweetId/{ofUser}/{tweetId}")]        
        public async Task<IHttpActionResult> GetTweetThreadByTweetId(string ofUser, ulong tweetId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var status = await repository.GetTweetThread(ofUser, tweetId);
            return Ok(status);
        }

    }
}