using CRM.Common.Enum;
using CRM.Common.Extensions;
using CRM.Entities;
using SocialAPIsWrapper.APIs;
using SocialCRM.Web.Models.Instagram;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class InstagramRepository : IInstagramRepository
    {
        public async Task<bool> AddAccount(InstagramModel model)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var rowOperation = db.AddInstagramAccount(model.AccessToken, model.User.Id, model.User.UserName,
                        model.User.FullName, model.User.ProfilePicture, model.User.Bio, model.User.WebSite, model.UserId, model.CompanyId, model.GroupId);
                    result = true;// rowOperation > 0;
                    await db.SaveChangesAsync();
                }
                catch (Exception exp)
                {
                    var errorMes = exp.Message;
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        public List<InstagramModel> GetAccountByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }
        public List<InstagramModel> GetAccountByCompanyId(int companyId)
        {
            throw new NotImplementedException();
        }
        public async Task<InstagramQueryModel> BulidAccountFilterQuery(InstagramFilterModel searchModel)
        {
            InstagramQueryModel query = new InstagramQueryModel();
            switch (searchModel.DateRange)
            {
                case DateFilter.Last15Days:
                    query.StartDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                    query.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Last30Days:
                    query.StartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    query.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Last60Days:
                    query.StartDate = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                    query.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Last90Days:
                    query.StartDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                    query.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Custom:
                    query.StartDate = DateTime.ParseExact(searchModel.StartDate, "ddMMyyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    query.EndDate = DateTime.ParseExact(searchModel.EndDate, "ddMMyyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    break;
                default:
                    query.StartDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                    query.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
            }
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    query.AccessTokens = db.Profiles.Where(p => p.Id == searchModel.InstagramAccountId).Select(x => x.AcessToken).FirstOrDefault();
                }
                catch (Exception exp)
                {
                    var errorMes = exp.Message;
                }
                db.Dispose();
            }
            return query;
        }
        public async Task<InstagramReportViewModel> GetInstagramReports(InstagramFilterModel searchModel, int userId)
        {
            InstagramReportViewModel result = null;
            switch (searchModel.DateRange)
            {
                case DateFilter.Last15Days:
                    searchModel.StartDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                    searchModel.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Last30Days:
                    searchModel.StartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    searchModel.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Last60Days:
                    searchModel.StartDate = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
                    searchModel.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Last90Days:
                    searchModel.StartDate = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                    searchModel.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case DateFilter.Custom:
                    searchModel.StartDate = DateTime.ParseExact(searchModel.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    searchModel.EndDate = DateTime.ParseExact(searchModel.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    break;
                default:
                    searchModel.StartDate = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd");
                    searchModel.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
            }
            var resultDS = new DataSet();
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var cmd = db.Database.Connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetInstagramReportByAccountId";
                    SqlParameter instagramAccountId = new SqlParameter("instagramAccountId", searchModel.InstagramAccountId);
                    SqlParameter _startDate = new SqlParameter("startDateRange", searchModel.StartDate);
                    SqlParameter _endDate = new SqlParameter("endDateRange", searchModel.EndDate);
                    cmd.Parameters.Add(instagramAccountId);
                    cmd.Parameters.Add(_startDate);
                    cmd.Parameters.Add(_endDate);
                    try
                    {
                        // executes
                        db.Database.Connection.Open();
                        var reader = cmd.ExecuteReader();
                        // loop through all resultsets (considering that it's possible to have more than one)
                        do
                        {
                            // loads the DataTable (schema will be fetch automatically)
                            var tb = new DataTable();
                            tb.Load(reader);
                            resultDS.Tables.Add(tb);

                        } while (!reader.IsClosed);
                    }
                    finally
                    {
                        // closes the connection
                        db.Database.Connection.Close();
                    }

                    if (resultDS != null && resultDS.Tables.Count > 0)
                    {

                        result = new InstagramReportViewModel();
                        result.TopPostGraph = resultDS.Tables[0].AsEnumerable().Select(dataRow => new TopInstagramPost
                        {
                            AccountName = dataRow.Field<string>("AccountName"),
                            Engagements = dataRow.Field<int?>("Engagements") != null ? dataRow.Field<int>("Engagements") : 0,
                            LowResolutionImageUrl = dataRow.Field<string>("LowResolutionImageUrl"),
                            PostType = dataRow.Field<string>("PostType"),
                            StandardResolutionImageUrl = dataRow.Field<string>("StandardResolutionImageUrl")
                        }).ToList();
                        result.PublisingBehaviourGraph = resultDS.Tables[1].AsEnumerable().Select(dataRow => new PublisingBehaviour
                        {
                            MediaSent = dataRow.Field<int?>("MediaSent") != null ? dataRow.Field<int>("MediaSent") : 0,
                            PublishedDate = dataRow.Field<DateTime>("PublishedDate").ToString("dd/MM/yyyy"),
                            TotalMedia = dataRow.Field<int?>("TotalMedia") != null ? dataRow.Field<int>("TotalMedia") : 0
                        }).ToList();
                        result.AudienceEngagementGraph = resultDS.Tables[2].AsEnumerable().Select(dataRow => new AudienceEngagement
                        {
                            CommentReceived = dataRow.Field<int?>("CommentReceived") != null ? dataRow.Field<int>("CommentReceived") : 0,
                            LikesReceived = dataRow.Field<int?>("LikesReceived") != null ? dataRow.Field<int>("LikesReceived") : 0,
                            PublishedDate = dataRow.Field<DateTime>("PublishedDate").ToString("dd/MM/yyyy"),
                            MediaSent = dataRow.Field<int>("MediaSent")
                        }).ToList();
                        result.GeneralStatics = resultDS.Tables[3].AsEnumerable().Select(dataRow => new OverAllStats
                        {
                            TotalCommentReceived = dataRow.Field<int?>("TotalCommentReceived") != null ? dataRow.Field<int>("TotalCommentReceived") : 0,
                            TotalFollowers = dataRow.Field<int?>("TotalFollowers") != null ? dataRow.Field<int>("TotalFollowers") : 0,
                            TotalFollows = dataRow.Field<int?>("TotalFollows") != null ? dataRow.Field<int>("TotalFollows") : 0,
                            TotalLikesReceived = dataRow.Field<int?>("TotalLikesReceived") != null ? dataRow.Field<int>("TotalLikesReceived") : 0,
                            TotalPhotos = dataRow.Field<int?>("TotalPhotos") != null ? dataRow.Field<int>("TotalPhotos") : 0,
                            TotalPosts = dataRow.Field<int?>("TotalPosts") != null ? dataRow.Field<int>("TotalPosts") : 0,
                            TotalVideos = dataRow.Field<int?>("TotalVideos") != null ? dataRow.Field<int>("TotalVideos") : 0,
                            AccountName = dataRow.Field<string>("AccountName"),
                            AccountPicURL = dataRow.Field<string>("AccountPicURL"),
                            AccessToken = dataRow.Field<string>("AccessToken")
                        }).FirstOrDefault();



                        var followersData = db.ProfilesHistories.Where(x => x.ProfileId == searchModel.InstagramAccountId && x.ProfileType == 6).OrderBy(x => x.CreatedDate).ToList();
                        var startDate = DateTime.Parse(searchModel.StartDate);
                        var currentStartDate = startDate;
                        var endDate = DateTime.Parse(searchModel.EndDate);
                        var prevStartDate = startDate.AddDays((startDate - endDate).Days);
                        result.AudienceGrowthData = new List<FollowersInDates>();
                        for (DateTime date = currentStartDate; currentStartDate <= endDate; currentStartDate = currentStartDate.AddDays(1))
                        {
                            FollowersInDates followers = new FollowersInDates();
                            followers.Date = currentStartDate.ToString("dd/MM/yyyy");
                            var profileData = followersData.Where(x => x.CreatedDate.Date == currentStartDate.AddDays(-1).Date).FirstOrDefault();
                            int prevDayFollowers = profileData != null ? profileData.NumOfFollowers : 0;
                            profileData = followersData.Where(x => x.CreatedDate.Date == currentStartDate.Date).FirstOrDefault();
                            int currentDayFollowers = profileData != null ? profileData.NumOfFollowers : 0;
                            followers.NumOfFollowers = currentDayFollowers - prevDayFollowers;
                            result.AudienceGrowthData.Add(followers);
                        }
                        result.AudienceGrowthData = result.AudienceGrowthData.Where(x => x.NumOfFollowers > 0).ToList();
                        double followersAtStartDate = followersData.Where(x => x.CreatedDate.Date == startDate.Date).FirstOrDefault() != null ? followersData.Where(x => x.CreatedDate.Date == startDate.Date).FirstOrDefault().NumOfFollowers : 0;
                        double followersAtEndDate = followersData.Where(x => x.CreatedDate.Date == endDate.Date).FirstOrDefault() != null ? followersData.Where(x => x.CreatedDate.Date == endDate.Date).FirstOrDefault().NumOfFollowers : 0;
                        result.GeneralStatics.FollowersGained = Convert.ToInt32(followersAtEndDate - followersAtStartDate);

                        double percent = (followersAtStartDate == 0) ? followersAtEndDate : ((followersAtEndDate - followersAtStartDate) / followersAtStartDate) * 100;
                        result.GeneralStatics.FollowersIncreased = Convert.ToString(Math.Round(percent,2));

                        var allPosts = db.InstagramMinnedDatas.ToList().Where(x => x.InstagramAccountId == searchModel.InstagramAccountId && x.CreatedDate.Value.Date <= startDate.Date && x.CreatedDate.Value.Date >= prevStartDate.Date).ToList();
                        double prevMonthCount = allPosts != null ? allPosts.Count : 0;
                        allPosts = db.InstagramMinnedDatas.ToList().Where(x => x.InstagramAccountId == searchModel.InstagramAccountId && x.CreatedDate.Value.Date >= startDate.Date && x.CreatedDate.Value.Date <= endDate.Date).ToList();
                        double currentMonthCount = allPosts != null ? allPosts.Count : 0;

                        percent = (prevMonthCount == 0) ? currentMonthCount : ((currentMonthCount - prevMonthCount) / prevMonthCount) * 100;
                        result.GeneralStatics.MediaIncreased = Convert.ToString(Math.Round(percent,2));

                        //most used hashtags
                        result.MostUsedHashTags = new List<HashTags>();
                        var posts = db.InstagramMinnedDatas.ToList().Where(x => x.InstagramAccountId == searchModel.InstagramAccountId && !String.IsNullOrEmpty(x.PostTags) && x.CreatedDate.Value.Date >= startDate.Date && x.CreatedDate.Value.Date <= endDate.Date).ToList();
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        foreach (var item in posts)
                        {
                            sb.Append(item.PostTags + ",");
                        }
                        string tagData = sb.ToString();
                           
                        if (!String.IsNullOrEmpty(tagData))
                        {
                            tagData = tagData.Substring(0, tagData.Length - 1);
                            List<HashTags> hashTags = new List<HashTags>();
                            List<string> wordList = tagData.Split(',').ToList<string>();
                            List<string> wordListUnique = wordList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
                            if (wordListUnique != null && wordListUnique.Count > 0)
                            {
                                foreach (var word in wordListUnique)
                                {
                                    var wordPattern = "";// @"\b" + word + @"\b";
                                    if (StopwordTool.HasSpecialChars(word))
                                    {
                                        wordPattern = Regex.Escape(word);
                                    }
                                    else
                                    {
                                        wordPattern = @"\b" + word + @"\b";
                                    }
                                    int frequency = Regex.Matches(tagData, wordPattern, RegexOptions.IgnoreCase).Count;
                                    hashTags.Add(new HashTags
                                    {
                                        Tag = word,
                                        Frequency = frequency
                                    });
                                }
                                result.MostUsedHashTags = hashTags.OrderByDescending(x => x.Frequency).Take(5).ToList();
                            }
                        }

                        // most engaged tags
                        result.MostEngagedHashTags = new List<HashTags>();
                        var engagedPosts = posts.OrderByDescending(x => (x.TotalLikes + x.TotalComments)).ToList();
                        foreach (var post in engagedPosts)
                        {
                            int engagement = (post.TotalLikes.HasValue ? post.TotalLikes.Value : 0) + (post.TotalComments.HasValue ? post.TotalComments.Value : 0);
                            string[] tags = post.PostTags.Split(',');
                            foreach (var tag in tags)
                            {
                                result.MostEngagedHashTags.Add(new HashTags
                                {
                                    Tag = tag,
                                    Frequency = engagement
                                });
                            }
                        }

                        result.MostEngagedHashTags = result.MostEngagedHashTags.OrderByDescending(x => x.Frequency).Take(5).ToList();

                        double prevMonthEngagements = db.InstagramMinnedDatas.ToList().Where(x => x.InstagramAccountId == searchModel.InstagramAccountId && x.CreatedDate.Value.Date <= startDate.Date && x.CreatedDate.Value.Date >= prevStartDate.Date).ToList().Select(y => y.TotalLikes.HasValue ? y.TotalLikes.Value : 0).ToArray().Sum();
                        prevMonthEngagements += db.InstagramMinnedDatas.ToList().Where(x => x.InstagramAccountId == searchModel.InstagramAccountId && x.CreatedDate.Value.Date <= startDate.Date && x.CreatedDate.Value.Date >= prevStartDate.Date).ToList().Select(y => y.TotalComments.HasValue ? y.TotalComments.Value : 0).ToArray().Sum();

                        double currentMonthEngagements = result.GeneralStatics.TotalCommentReceived + result.GeneralStatics.TotalLikesReceived;

                        percent = (prevMonthEngagements == 0) ? currentMonthEngagements : ((currentMonthEngagements - prevMonthEngagements) / prevMonthEngagements) * 100;
                        result.GeneralStatics.EngagementIncreased = Convert.ToString(Math.Round(percent,2));

                        result.GeneralStatics.EngagementsPerFollower = (result.GeneralStatics.TotalFollowers == 0) ? result.GeneralStatics.TotalFollowers : Math.Round(currentMonthEngagements / result.GeneralStatics.TotalFollowers,2);
                        result.GeneralStatics.EngagementsPerMedia = (result.GeneralStatics.TotalPosts == 0) ? result.GeneralStatics.TotalPosts : Math.Round(currentMonthEngagements / result.GeneralStatics.TotalPosts,2);

                        double prevMonthPosts = db.InstagramMinnedDatas.ToList().Where(x => x.InstagramAccountId == searchModel.InstagramAccountId && x.CreatedDate.Value.Date <= startDate.Date && x.CreatedDate.Value.Date >= prevStartDate.Date).ToList().Count;
                        double prevMonthEngPerMedia = (prevMonthPosts == 0) ? prevMonthPosts : prevMonthEngagements / prevMonthPosts;
                        percent = (prevMonthEngPerMedia == 0) ? result.GeneralStatics.EngagementsPerMedia : ((result.GeneralStatics.EngagementsPerMedia - prevMonthEngPerMedia) / prevMonthEngPerMedia) * 100;
                        result.GeneralStatics.EngagementPerMediaIncreased = Convert.ToString(Math.Round(percent,2));

                        // Instagram Profile Stats
                        result.InstagramProfilesStats = new List<InstagramProfiles>();
                        int teamMemberId = db.Teams.Where(x => x.UserId == userId).FirstOrDefault().Id;
                        int [] profileIdArray = db.ProfileComapnyGroups.Where(x => x.CompanyId == searchModel.CompanyId && x.GroupId == searchModel.GroupId && x.ProfileId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            ProfileId = z.FirstOrDefault().Profile.Id,
                            ProfileName = z.FirstOrDefault().Profile.Name,
                            ProfileType = (ProfilesType)z.FirstOrDefault().Profile.ProfileTypeId,
                            ProfileTypeId = z.FirstOrDefault().Profile.ProfileTypeId,
                            CompanyId = z.FirstOrDefault().CompanyId,
                            IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.FirstOrDefault().Profile.ProfilePic,
                            AccountName = z.FirstOrDefault().Profile.UserName,
                            IsPage = false,
                            UserId = z.FirstOrDefault().Profile.ProfileExternalId,
                            Id = z.FirstOrDefault().Profile.Id
                        }).ToList().Where(x => x.ProfileType == ProfilesType.Instagram).Select(y => y.Id).ToArray();


                        var profileHistories = db.ProfilesHistories.ToList().Where(x => x.CreatedDate.Date >= startDate.Date && x.CreatedDate.Date <= endDate.Date).OrderByDescending(y => y.CreatedDate).ToList();
                        var instaPosts = db.InstagramMinnedDatas.ToList().Where(x => x.CreatedDate.Value.Date >= startDate.Date && x.CreatedDate.Value.Date <= endDate.Date).OrderByDescending(y => y.CreatedDate).ToList();
                        var instaComments = db.InstagramComments.ToList().Where(x => x.CreatedDate.Value.Date >= startDate.Date && x.CreatedDate.Value.Date <= endDate.Date).OrderByDescending(y => y.CreatedDate).ToList();
                        foreach (var profile in db.Profiles.Where(x => x.ProfileTypeId == 6 && profileIdArray.Contains(x.Id)).ToList())
                        {
                            InstagramProfiles instaProfile = new InstagramProfiles();
                            instaProfile.ProfileId = profile.Id;
                            instaProfile.ProfileName = profile.Name;
                            instaProfile.ProfilePic = profile.ProfilePic;
                            instaProfile.TotalFollowers = profileHistories.Where(x => x.ProfileId == profile.Id).FirstOrDefault().NumOfFollowers;
                            var startDateStats = profileHistories.Where(x => x.ProfileId == profile.Id && x.CreatedDate.Date == startDate.Date).FirstOrDefault();
                            double subfollowersAtStartDate = startDateStats != null ? startDateStats.NumOfFollowers : 0;
                            double subfollowersAtEndDate = instaProfile.TotalFollowers;
                            instaProfile.FollowersGained = Convert.ToInt32(subfollowersAtEndDate - subfollowersAtStartDate);

                            percent = (subfollowersAtStartDate == 0) ? subfollowersAtEndDate : ((subfollowersAtEndDate - subfollowersAtStartDate) / subfollowersAtStartDate) * 100;
                            instaProfile.FollowersIncreased = Convert.ToString(Math.Round(percent,2));
                            
                            string[] postIds = instaPosts.Where(x => x.InstagramAccountId == profile.Id).Select(y => y.PostId).ToArray();
                            instaProfile.MediaSent = postIds.Count();
                            instaProfile.CommentsSent = instaComments.Where(x => postIds.Contains(x.PostId)).ToList().Count;
                            double totalEngagement = instaPosts.Where(x => x.InstagramAccountId == profile.Id).Select(y => y.TotalLikes.Value).ToArray().Sum() + instaPosts.Where(x => x.InstagramAccountId == profile.Id).Select(y => y.TotalComments.Value).ToArray().Sum();
                            instaProfile.TotalEngagements = totalEngagement;
                            instaProfile.EngagementsPerMeida = (instaProfile.MediaSent == 0) ? instaProfile.MediaSent : Math.Round(totalEngagement / instaProfile.MediaSent, 2);
                            instaProfile.EngagementsPerFollower = (instaProfile.TotalFollowers == 0) ? instaProfile.TotalFollowers : Math.Round(totalEngagement / instaProfile.TotalFollowers,2);

                            result.InstagramProfilesStats.Add(instaProfile);
                        }
                    }

                }
                catch (Exception exp)
                {
                    var errorMes = exp.Message;
                    resultDS = null;
                }
                db.Dispose();
            }
            //Here we Map DataSet into Object


            return result;
        }

        public async Task<InstagramPostModel> GetInstagramPostDetails(string postId)
        {
            InstagramPostModel model = new InstagramPostModel();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    var instagramObj = context.InstagramMinnedDatas.Where(x => x.PostId == postId).FirstOrDefault();
                    if (instagramObj != null)
                    {
                        model.PostId = postId;
                        model.PostUrl = instagramObj.StandardResolutionImageUrl;
                        model.PostDate = DatetimeExtensions.ConvertToDateTime(instagramObj.PostCreatedTime);
                        model.CreatedDate = DatetimeExtensions.formatMsgDatetime(model.PostDate);
                        model.CreatedDateForDeck = DatetimeExtensions.formatMsgDatetime(model.PostDate);
                        model.PostCaption = instagramObj.PostCaption == null ? "No Caption" : instagramObj.PostCaption.ToString();
                        var authorDetail = context.Profiles.Where(x => x.Id == instagramObj.InstagramAccountId && x.ProfileTypeId == 6).FirstOrDefault();
                        if (authorDetail != null)
                        {
                            model.AuthorId = authorDetail.ProfileExternalId;
                            model.AuthorName = authorDetail.Name;
                            model.AuthorProfilePicture = authorDetail.ProfilePic;
                        }
                        model.CommentsList = context.InstagramComments.Where(x => x.PostId == postId).ToList<InstagramComment>();
                    }
                }
            });
            return model;
        }

        public async Task<string> PostInstagramComment(string postId, string userId, string comment, int teamId,int companyId)
        {
            string response = "";
            try
            {
                using (var context = new SocialCRMEntities())
                {
                    int int_userId = Convert.ToInt32(userId);
                    int profileCompanyGroupId = context.ProfileComapnyGroups.Where(x => x.ProfileId == int_userId).FirstOrDefault().Id;
                    string profileId = context.Profiles.Where(x => x.Id == int_userId).FirstOrDefault().ProfileExternalId;
                    var accountObj = context.ProfileComapnyGroups.Where(x => x.Id == profileCompanyGroupId).FirstOrDefault();
                    if (accountObj != null)
                    {
                        string accessToken = accountObj.Profile.AcessToken;
                        response = await InstagramUtility.PostInstagramComment(postId, accessToken, comment);
                        if (response.Contains("success"))
                        {
                            string commentId = response.Split('-')[1];
                            await new MiningRepository().AddSocialActivity(profileId, "", postId, "IC", 2, "", comment, teamId,companyId, "", "", commentId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<string> DeleteInstagramComment(string postId, string userId, string commentId)
        {
            string response = "";
            try
            {
                using (var context = new SocialCRMEntities())
                {
                    var accountObj = context.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault();
                    if (accountObj != null)
                    {
                        string accessToken = accountObj.AcessToken;
                        response = await InstagramUtility.DeleteInstagramComment(postId, accessToken, commentId);
                        if (response.Contains("success"))
                        {
                            var commentObj = context.InstagramComments.Where(x => x.PostId == postId && x.CommentId == commentId).FirstOrDefault();
                            context.InstagramComments.Remove(commentObj);
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<string> LikeInstagramPost(string postId, string userId)
        {
            string response = "";
            try
            {
                using (var context = new SocialCRMEntities())
                {
                    var accountObj = context.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault();
                    if (accountObj != null)
                    {
                        string accessToken = accountObj.AcessToken;
                        response = await InstagramUtility.LikeInstagramPost(postId, accessToken);

                        /*MiningRepository rep = new MiningRepository();

                        int ProfileId = accountObj.Id;
                        int feedId = context.InstagramMinnedDatas.Where(x => x.PostId == postId).FirstOrDefault().Id;
                        result = await rep.AddSocialActivity(ProfileId, null, feedId, "IM", 1);*/
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<bool> IsInstagramPostLiked(string postId, string userId)
        {
            bool response = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new SocialCRMEntities())
                    {
                        var accountObj = context.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault();
                        if (accountObj != null)
                        {
                            string accessToken = accountObj.AcessToken;
                            response = InstagramUtility.IsInstagramPostLiked(postId, accessToken);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<string> UnlikeInstagramPost(string postId, string userId)
        {
            string response = "";
            try
            {
                using (var context = new SocialCRMEntities())
                {
                    var accountObj = context.Profiles.Where(x => x.ProfileExternalId == userId).FirstOrDefault();
                    if (accountObj != null)
                    {
                        string accessToken = accountObj.AcessToken;
                        response = await InstagramUtility.UnlikeInstagramPost(postId, accessToken);
                    }

                    /*MiningRepository rep = new MiningRepository();
                    await rep.RemoveSocialActivity(activityId);*/
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
    }
}