using CRM.Entities;
using LinqToTwitter;
using SocialAPIsWrapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SocialAPIsWrapper.APIs
{
    public class TwitterUtility
    {
        private TwitterContext twitterCtx;

        public TwitterUtility(string authorizedUserName)
        {
            using (var context = new SocialCRMEntities())
            {
                string consumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"].ToString();
                string consumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"].ToString();
                var user = context.Profiles.Where(z => z.UserName == authorizedUserName && z.ProfileTypeId == 4).FirstOrDefault();
                if(user == null)
                {
                    user = context.Profiles.Where(z => z.ProfileTypeId == 4).FirstOrDefault();
                }
                string token = user.AcessToken;
                string tokenSecret = user.AcessTokenSecret;

                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = consumerKey,
                        ConsumerSecret = consumerSecret,
                        AccessToken = token,
                        AccessTokenSecret = tokenSecret
                    }
                };
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                twitterCtx = new TwitterContext(auth);
            }
        }
        public async Task<User> getTwitterProfile(string userScreenName)
        {
            User user = new User();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                user =
                await
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Show &&
                       tweet.ScreenName == userScreenName
                 select tweet)
                .SingleOrDefaultAsync();

                return user;
            }
            catch (TwitterQueryException ex)
            {
                return user;
            }
        }

        public async Task<Relationship> GetTwitterFriendshipStatus(string userNames)
        {
            Relationship relationship = new Relationship();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var relationships =
                await
                (from look in twitterCtx.Friendship
                 where look.Type == FriendshipType.Lookup &&
                       look.ScreenName == userNames
                 select look.Relationships)
                .SingleOrDefaultAsync();

                return relationships[0];
            }
            catch (TwitterQueryException ex)
            {
                return relationship;
            }
        }

        public async Task<Friendship> getListOfFollowers(string userScreenName, long cursor = -1)
        {
            Friendship friendship = new Friendship();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                friendship =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.FollowersList &&
                       friend.ScreenName == userScreenName &&
                       friend.Cursor == cursor &&
                       friend.SkipStatus == true
                 select friend)
                .SingleOrDefaultAsync();

                return friendship;
            }
            catch (TwitterQueryException ex)
            {
                return friendship;
            }
        }

        public async Task<Friendship> getListOfFriends(string userScreenName, long cursor = -1)
        {
            Friendship friendship = new Friendship();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                friendship =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.FriendsList &&
                       friend.ScreenName == userScreenName &&
                       friend.Cursor == cursor &&
                       friend.SkipStatus == true
                 select friend)
                .SingleOrDefaultAsync();

                return friendship;
            }
            catch (TwitterQueryException ex)
            {
                return friendship;
            }
        }

        public async Task<List<Status>> getUserTweets(string userScreenName, ulong maxId, int count)
        {
            List<Status> tweets = new List<Status>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                if (maxId == 0)
                {
                    tweets =
                    await
                    (from tweet in twitterCtx.Status
                     where tweet.Type == StatusType.User &&
                           tweet.ScreenName == userScreenName &&
                           tweet.Count == count
                     select tweet)
                    .ToListAsync();
                }
                else
                {
                    tweets =
                    await
                    (from tweet in twitterCtx.Status
                     where tweet.Type == StatusType.User &&
                           tweet.ScreenName == userScreenName &&
                           tweet.Count == count &&
                           tweet.MaxID == maxId
                     select tweet)
                    .ToListAsync();
                }

                return tweets;
            }
            catch (TwitterQueryException ex)
            {
                return tweets;
            }
        }

        public async Task<List<Status>> getUserHomeTweets(string userScreenName, ulong maxId, int count)
        {
            List<Status> tweets = new List<Status>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                if (maxId == 0)
                {
                    tweets =
                    await
                    (from tweet in twitterCtx.Status
                     where tweet.Type == StatusType.Home &&
                           tweet.ScreenName == userScreenName &&
                           tweet.Count == count
                     select tweet)
                    .ToListAsync();
                }
                else
                {
                    tweets =
                    await
                    (from tweet in twitterCtx.Status
                     where tweet.Type == StatusType.Home &&
                           tweet.ScreenName == userScreenName &&
                           tweet.Count == count &&
                           tweet.MaxID == maxId
                     select tweet)
                    .ToListAsync();
                }
            
                return tweets;
            }
            catch (TwitterQueryException ex)
            {
                return tweets;
            }
        }

        public async Task<List<Status>> getTwitterListTweets(ulong listId, ulong maxId, int count)
        {
            List<Status> statuses = new List<Status>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                List tweets;
                if (maxId == 0)
                {
                    tweets =
                    await
                    (from tweet in twitterCtx.List
                     where tweet.Type == ListType.Statuses &&
                           tweet.ListID == listId &&
                           tweet.Count == count
                     select tweet)
                    .SingleOrDefaultAsync();
                }
                else
                {
                    tweets =
                    await
                    (from tweet in twitterCtx.List
                     where tweet.Type == ListType.Statuses &&
                           tweet.ListID == listId &&
                           tweet.Count == count &&
                           tweet.MaxID == maxId
                     select tweet)
                    .SingleOrDefaultAsync();
                }

                return tweets.Statuses;
            }
            catch (TwitterQueryException ex)
            {
                return statuses;
            }
        }

        public async Task<List<Status>> getUserMentions(string userScreenName, ulong maxId, int count)
        {
            List<Status> statuses = new List<Status>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                Search searchResult;
                userScreenName = string.Format("@{0}", userScreenName);
                if (maxId == 0)
                {
                    searchResult =
                    await
                    (from tweet in twitterCtx.Search
                     where tweet.Type == SearchType.Search &&
                           tweet.Query == userScreenName &&
                           tweet.Count == count
                     select tweet)
                    .SingleOrDefaultAsync();
                }
                else
                {
                    searchResult =
                    await
                    (from tweet in twitterCtx.Search
                     where tweet.Type == SearchType.Search &&
                           tweet.Query == userScreenName &&
                           tweet.Count == count &&
                           tweet.MaxID == maxId
                     select tweet)
                    .SingleOrDefaultAsync();
                }

                return searchResult.Statuses;
            }
            catch (TwitterQueryException ex)
            {
                return statuses;
            }
        }

        public async Task<User> followUser(string userScreenName)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.CreateFriendshipAsync(userScreenName, true);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<User> unfollowUser(string userScreenName)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.DestroyFriendshipAsync(userScreenName);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<Status> tweetReply(ulong tweetId, string status, IEnumerable<ulong> mediaIds)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.ReplyAsync(tweetId, status, mediaIds);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<Status> newTweet(string status, IEnumerable<ulong> mediaIds)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                Status tweet = await twitterCtx.TweetAsync(status, mediaIds);
                
                return tweet;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<Status> newTweetWithLocation(string status, IEnumerable<ulong> mediaIds, decimal lat, decimal lng)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                Status tweet = await twitterCtx.TweetAsync(status, lat, lng, null, true, true, mediaIds);

                return tweet;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<List<ulong>> UploadMedia(List<MediaModelTwitter> mediaInfo)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var imageUploadTasks = new List<Task<Media>>();
                
                foreach (var image in mediaInfo)
                {
                    //string imgPath = "C:\\Users\\moaslam\\Downloads\\VID-20160629-WA0001.mp4";
                    //string ext = Path.GetExtension(imgPath);
                    //string media_type = "";
                    //if (ext == ".png" || ext == ".jpg" || ext == ".gif" || ext == ".webp")
                    //{
                    //    media_type = "image/" + ext.Substring(1);
                    //}
                    //else if (ext == ".mov" || ext == ".mp4")
                    //{
                    //    media_type = "video/" + ext.Substring(1);
                    //}
                    if (image.Base64String.Contains(','))
                    {
                        image.Base64String = image.Base64String.Split(',')[1];
                    }
                    byte[] byteArr = Convert.FromBase64String(image.Base64String);

                    imageUploadTasks.Add(twitterCtx.UploadMediaAsync(byteArr, image.ContentType,""));
                    //imageUploadTasks.Add(twitterCtx.UploadMediaAsync(File.ReadAllBytes(imgPath), media_type));
                }

                await Task.WhenAll(imageUploadTasks);

                var mediaIds = (from tsk in imageUploadTasks
                                select tsk.Result.MediaID)
                                .ToList();

                return mediaIds;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Status>> getTweetReplies(string userScreenName, ulong tweetId)
        {
            List<Status> usertweets = new List<Status>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                ulong currentMaxId = 0;
                int iterate = 0;
                while (iterate < 5)
                {
                    List<Status> tempTweets = await getUserMentions(userScreenName, currentMaxId, 100);
                    foreach(Status tweet in tempTweets)
                    {
                        if(tweet.InReplyToStatusID == tweetId)
                        {
                            usertweets.Add(tweet);
                        }
                    }
                    
                    if (tempTweets != null)
                    {
                        if (tempTweets.Count > 0)
                        {
                            currentMaxId = tempTweets[tempTweets.Count - 1].StatusID - 1;
                        }
                        else if (tempTweets.Count == 0)
                        {
                            break;
                        }
                    }

                    iterate++;
                }
                return usertweets;
            }
            catch (TwitterQueryException ex)
            {
                return usertweets;
            }
        }

        public async Task<Status> deleteTweet(ulong tweetId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.DeleteTweetAsync(tweetId);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<DirectMessage> sendDirectMessage(string screenName, string text)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.NewDirectMessageAsync(screenName, text);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<DirectMessage> deleteDirectMessage(ulong messageId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.DestroyDirectMessageAsync(messageId, false);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }

        }

        public async Task<Status> retweet(ulong tweetId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.RetweetAsync(tweetId);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<Status> markFavorite(string tweetId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                ulong Id = Convert.ToUInt64(tweetId);
                var result = await twitterCtx.CreateFavoriteAsync(Id);

                if (result != null)
                {
                    using (var context = new SocialCRMEntities())
                    {
                        PostsTwitter tw = context.PostsTwitters.Where(x => x.StatusId == tweetId).FirstOrDefault();
                        tw.isLikedByAdmin = true;
                        context.PostsTwitters.Attach(tw);
                        context.Entry(tw).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                return result;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Status> unmarkFavorite(string tweetId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                ulong Id = Convert.ToUInt64(tweetId);
                var result = await twitterCtx.DestroyFavoriteAsync(Id);

                if (result != null)
                {
                    using (var context = new SocialCRMEntities())
                    {
                        PostsTwitter tw = context.PostsTwitters.Where(x => x.StatusId == tweetId).FirstOrDefault();
                        tw.isLikedByAdmin = false;
                        context.PostsTwitters.Attach(tw);
                        context.Entry(tw).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                return result;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> isFavorited(string tweetId)
        {
            try
            {
                bool result = false;

                //commented on October 11, 2017. Functionality is changed on client's request
                //var friendTweets =  await
                //                    (from tweet in twitterCtx.Status
                //                     where tweet.Type == StatusType.Show &&
                //                           tweet.ID == tweetId
                //                     select tweet)
                //                    .ToListAsync();
                //if (friendTweets != null)
                //{
                //    friendTweets.ForEach(tweet =>
                //    {
                //        if (tweet != null && tweet.User != null)
                //            result = tweet.Favorited;
                //    });
                //}
                using (var context = new SocialCRMEntities())
                {
                    result = Convert.ToBoolean(context.PostsTwitters.Where(x => x.StatusId == tweetId).FirstOrDefault().isLikedByAdmin);
                }

                return result;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<string> isRetweeted(ulong tweetId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                string result = "";
                var friendTweets = await
                                    (from tweet in twitterCtx.Status
                                     where tweet.Type == StatusType.Show &&
                                           tweet.ID == tweetId &&
                                           tweet.IncludeMyRetweet == true
                                     select tweet)
                                    .ToListAsync();
                if (friendTweets != null)
                {
                    friendTweets.ForEach(tweet =>
                    {
                        if (tweet != null && tweet.User != null)
                        {
                            if (tweet.Retweeted)
                                result = tweet.CurrentUserRetweet.ToString();
                        }
                    });
                }
                return result;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<User> reportSpam(ulong userId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.ReportSpamAsync(userId);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<List<List>> getUserTwitterLists(string userScreenName, int count, long cursor = -1)
        {
            List<List> lists = new List<List>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Ownerships &&
                       list.ScreenName == userScreenName &&
                       list.Cursor == cursor &&
                       list.Count == count
                 select list)
                .ToListAsync();

                return lists;
            }
            catch (TwitterQueryException ex)
            {
                return lists;
            }
        }

        public async Task<List> createNewTwitterList(string listName, string mode, string description)
        {
            List list = new List();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                list = await twitterCtx.CreateListAsync(listName, mode, description);
                return list;
            }
            catch (TwitterQueryException ex)
            {
                return list;
            }
        }

        public async Task<List> updateTwitterList(ulong listId, string listName, string mode, string description)
        {
            List list = new List();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                list = await twitterCtx.UpdateListAsync(listId, null, listName, 0, null, mode, description);
                return list;
            }
            catch (TwitterQueryException ex)
            {
                return list;
            }
        }

        public async Task<List> deleteUserTwitterList(ulong listId)
        {
            List list = new List();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                list = await twitterCtx.DeleteListAsync(listId, null, 0, null);
                return list;
            }
            catch (TwitterQueryException ex)
            {
                return list;
            }
        }

        public async Task<List> getTwitterListMembers(ulong listId, long cursor = -1)
        {
            List lists = new List();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Members &&
                       list.ListID == listId &&
                       list.Cursor == cursor &&
                       list.SkipStatus == true
                 select list)
                .SingleOrDefaultAsync();

                return lists;
            }
            catch (TwitterQueryException ex)
            {
                return lists;
            }
        }

        public async Task<List> addMemberToTwitterList(string screenName, string slug, ulong ownerId, string ownerScreenName)
        {
            List list = new List();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                list = await twitterCtx.AddMemberToListAsync(screenName, 0, slug, ownerId, ownerScreenName);
                return list;
            }
            catch (TwitterQueryException ex)
            {
                return list;
            }
        }

        public async Task<List> deleteMemberfromTwitterList(string screenName, string slug, ulong ownerId, string ownerScreenName)
        {
            List list = new List();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                list = await twitterCtx.DeleteMemberFromListAsync(0, screenName, 0, slug, ownerId, ownerScreenName);
                return list;
            }
            catch (TwitterQueryException ex)
            {
                return list;
            }
        }

        public async Task<List<List>> getListsThatUserIsAMemberOf(string userScreenName, long cursor = -1)
        {
            List<List> lists = new List<List>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Memberships &&
                       list.FilterToOwnedLists == true &&
                       list.ScreenName == userScreenName // user to get memberships for
                 select list)
                .ToListAsync();

                return lists;
            }
            catch (TwitterQueryException ex)
            {
                return lists;
            }
        }

        public async Task<bool> isUserMemberOfList(string userScreenName, ulong listId)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var subscribedList =
                    await
                    (from list in twitterCtx.List
                     where list.Type == ListType.IsMember &&
                           list.ScreenName == userScreenName &&
                           list.ListID == listId
                     select list)
                    .SingleOrDefaultAsync();

                return true;
            }
            // whenever user is not a member of the specified list, Twitter
            // returns an HTTP 404, Not Found, response.  LINQ to Twitter 
            // intercepts the HTTP response and wraps it in a TwitterQueryException 
            // where you can read the error message from Twitter via the Message property.
            catch (TwitterQueryException ex)
            {
                return false;
            }
        }

        public async Task<User> blockUser(string screenName)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.CreateBlockAsync(0, screenName, true);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<User> unBlockUser(string screenName)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return await twitterCtx.DestroyBlockAsync(0, screenName, true);
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<DMEvent> sendDirectMessageEvent(ulong userId, string text)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                DirectMessageEvents message =
              await twitterCtx.NewDirectMessageEventAsync(
                  userId,text);
                DMEvent dmEvent = message?.Value?.DMEvent;
                return dmEvent;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
        }

        public async Task<bool> deleteDirectMessageEvent(ulong dmID)
        {
            var status = false;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                await twitterCtx.DeleteDirectMessageEventAsync(dmID);
                status = true;
            }
            catch (TwitterQueryException ex)
            {
                throw ex;
            }
            return status;
        }
    }
}
