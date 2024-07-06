using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialCRM.Web.Models.Dashboard;
using System.Threading.Tasks;
using CRM.Entities;
using SocialCRM.Web.Models.Profiles;
using CRM.Common.Enum;
using SocialAPIsWrapper.APIs;
using LinqToTwitter;
using SocialCRM.Web.Models.TaskManager;
using System.Globalization;
using Facebook;
using System.Text.RegularExpressions;
using CRM.Common.Extensions;
using System.Text;

namespace SocialCRM.Web.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        public async Task<DashboardModel> GetDashboardInformation(int userId, int companyId, int groupId)
        {
            DashboardModel result = new DashboardModel();
            List<ProfileModel> resultProfiles = null;
            try
            {
                List<ProfileModel> profiles = null;
                await Task.Run(async () =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        //int teamMemberId = db.Teams.Where(x => x.UserId == userId && x.CompanyId == companyId).FirstOrDefault().Id;                        
                        //Profiles  
                        profiles = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfileId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfileId).Select(z => new ProfileModel
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
                        }).ToList();

                        profiles = profiles.Where(x => x.IsConnected).ToList();

                        string profileIds = string.Join(",", profiles.Select(y => y.ProfileId).ToArray());

                        //Profile Pages
                        var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId.HasValue && x.Teams.Any(t => t.UserId == userId)).GroupBy(g => g.ProfilePageId).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.FirstOrDefault().Id,
                            ProfileId = z.FirstOrDefault().ProfilePage.Id,
                            ProfileName = z.FirstOrDefault().ProfilePage.PageName,
                            ProfileType = (ProfilesType)z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z.FirstOrDefault().ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z.FirstOrDefault().CompanyId,
                            IsConnected = z.FirstOrDefault().Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.FirstOrDefault().ProfilePage.ProfilePic,
                            AccountName = z.FirstOrDefault().Profile.UserName,
                            IsPage = true,
                            UserId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            PageId = z.FirstOrDefault().ProfilePage.ProfilePageID,
                            Id = z.FirstOrDefault().ProfilePage.Id
                        }).ToList();

                        profilePages = profilePages.Where(x => x.IsConnected).ToList();

                        string profilePageIds = string.Join(",", profilePages.Select(y => y.ProfileId).ToArray());

                        profiles.AddRange(profilePages);

                        if (profiles != null && profiles.Count > 0)
                        {
                            List<TopInfluencerModel> lstTopInfluencersModel = new List<TopInfluencerModel>();

                            //Twitter Group Key Stats
                            string[] twitterProfilesId = profiles.Where(x => x.ProfileType == ProfilesType.Twitter).Select(y => y.UserId).ToArray();
                            result.gksTwNumOfTweets = db.PostsTwitters.Where(x => twitterProfilesId.Contains(x.UserId)).Count();

                            var twitterProfilesUsernames = profiles.Where(x => x.ProfileType == ProfilesType.Twitter).ToList<ProfileModel>();
                            result.gksTwNumOfFollowers = 0;
                            foreach (ProfileModel profile in twitterProfilesUsernames)
                            {
                                TwitterUtility twitterUtility = new TwitterUtility(profile.AccountName);
                                Friendship userfriendship = await twitterUtility.getListOfFollowers(profile.AccountName, -1);
                                result.gksTwNumOfFollowers += userfriendship.Users.Count;
                                TopInfluencerModel influencerObj = new TopInfluencerModel();
                                influencerObj.firstName = profile.ProfileName;
                                influencerObj.statusCount = userfriendship.Users.Count;

                                lstTopInfluencersModel.Add(influencerObj);
                            }

                            string[] fbProfilesId = profiles.Where(x => x.ProfileType == ProfilesType.Facebook && x.IsPage == false).Select(y => y.UserId).ToArray();
                            result.gksFbProfileNumOfPosts = db.PostsFBs.Where(x => fbProfilesId.Contains(x.UserId)).Count();

                            var fbProfiles = profiles.Where(x => x.ProfileType == ProfilesType.Facebook && x.IsPage == false).ToList<ProfileModel>();

                            result.gksTFbProfileNumOfFollowers = 0; // not available

                            result.gksTFbProfileNumOfFriends = 0;
                            foreach (var profile in fbProfiles)
                            {
                                FacebookUtility fbUtility = new FacebookUtility(profile.UserId, false);
                                var data = await fbUtility.geUserFriendsCount(profile.UserId);
                                if (data != null)
                                {
                                    var dataResponse = data["summary"] as JsonObject;
                                    result.gksTFbProfileNumOfFriends += Convert.ToInt32(dataResponse["total_count"]);

                                    TopInfluencerModel influencerObj = new TopInfluencerModel();
                                    influencerObj.firstName = profile.ProfileName;
                                    influencerObj.statusCount = Convert.ToInt32(dataResponse["total_count"]);

                                    lstTopInfluencersModel.Add(influencerObj);
                                }

                            }

                            string[] fbProfilePageIds = profiles.Where(x => x.ProfileType == ProfilesType.Facebook && x.IsPage == true).Select(y => y.UserId).ToArray();
                            result.gksFbPageNumOfPosts = db.PostsFBs.Where(x => fbProfilePageIds.Contains(x.PageId)).Count();

                            var fbPages = profiles.Where(x => x.ProfileType == ProfilesType.Facebook && x.IsPage == true).ToList<ProfileModel>();

                            result.gksFbPageNumOfLikes = 0;
                            foreach (var page in fbPages)
                            {
                                FacebookUtility fbUtility = new FacebookUtility(page.UserId, true);
                                var data = await fbUtility.gePageLikes(page.UserId);
                                if (data != null)
                                {
                                    result.gksFbPageNumOfLikes += Convert.ToInt32(data["likes"]);

                                    TopInfluencerModel influencerObj = new TopInfluencerModel();
                                    influencerObj.firstName = page.ProfileName;
                                    influencerObj.statusCount = Convert.ToInt32(data["likes"]);

                                    lstTopInfluencersModel.Add(influencerObj);
                                }
                                    
                            }

                            int[] instaProfileIds = profiles.Where(x => x.ProfileType == ProfilesType.Instagram).Select(y => y.Id).ToArray();
                            result.gksInstaNumOfPosts = db.InstagramMinnedDatas.Where(x => instaProfileIds.Contains(x.InstagramAccountId)).Count();

                            var instagramUserAccessTokens = db.Profiles.Where(x => instaProfileIds.Contains(x.Id)).ToList();
                            foreach (Profile instaAccount in instagramUserAccessTokens)
                            {
                                int count = 0;
                                InstagramUtility instagramUtility = new InstagramUtility();
                                count = instagramUtility.getInstagramFollowers(instaAccount.AcessToken);
                                result.gksInstaNumOfFollowers += instagramUtility.getInstagramFollowers(instaAccount.AcessToken);

                                TopInfluencerModel influencerObj = new TopInfluencerModel();
                                influencerObj.firstName = instaAccount.Name;
                                influencerObj.statusCount = count;

                                lstTopInfluencersModel.Add(influencerObj);
                            }

                            result.TopInfluencers = lstTopInfluencersModel.OrderByDescending(x => x.statusCount).Take(5).ToList();

                            result.DemographicMaleCount = result.DemographicFemaleCount = result.DemographicUnspecifiedCount = 0;

                            string ids = string.Join("-", profiles.Select(y => y.UserId).Distinct().ToArray());
                            var demographicData = db.GetDemographicData(ids);
                            foreach (var item in demographicData)
                            {
                                if (item.Gender.ToLower() == "male")
                                    result.DemographicMaleCount = item.TotalCount.HasValue ? item.TotalCount.Value : 0;
                                else if (item.Gender.ToLower() == "female")
                                    result.DemographicFemaleCount = item.TotalCount.HasValue ? item.TotalCount.Value : 0;
                                else if (item.Gender.ToLower() == "unspecified")
                                    result.DemographicUnspecifiedCount = item.TotalCount.HasValue ? item.TotalCount.Value : 0;
                            }

                            //Word Cloud Data
                            var wordCloudResponse = db.GetMinnedData(userId, profileIds, profilePageIds).ToList<string>();
                            StringBuilder sb = new StringBuilder();
                            foreach (var item in wordCloudResponse)
                            {
                                sb.Append(item + " ");
                            }
                            string wordCloudData = sb.ToString();
                            wordCloudData = StopwordTool.RemoveStopwords(wordCloudData);
                            if (!String.IsNullOrEmpty(wordCloudData))
                            {
                                List<WordCloudModel> wordCloud = new List<WordCloudModel>();
                                List<string> wordList = wordCloudData.Split(' ').ToList<string>();
                                List<string> wordListUnique = wordList.Where(x => x.Length > 3).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
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
                                        int frequency = Regex.Matches(wordCloudData, wordPattern, RegexOptions.IgnoreCase).Count;
                                        wordCloud.Add(new WordCloudModel
                                        {
                                            Text = word,
                                            Frequency = frequency,
                                            TextWithFrequency = string.Format("{0}({1})", word, frequency),
                                            Link = "#"
                                        });
                                    }
                                    result.WordCloudList = wordCloud.OrderByDescending(x => x.Frequency).Take(40).ToList();
                                }
                            }
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public async Task<DashboardModel> FilterGroupKeyStatistics(int userId, int companyId, int groupId, DateTime? dateFrom, DateTime? dateTo)
        {
            DashboardModel result = new DashboardModel();
            List<ProfileModel> profiles = null;
            try
            {
                //DateTime? fromDate, toDate = null;

                //if((!String.IsNullOrEmpty(dateFrom)))
                //    fromDate = DateTime.ParseExact(dateFrom, "d", CultureInfo.InvariantCulture);

                //if ((!String.IsNullOrEmpty(dateTo)))
                //    toDate = DateTime.ParseExact(dateTo, "d", CultureInfo.InvariantCulture);
                await Task.Run(() =>
                {
                    using (var db = new SocialCRMEntities())
                    {
                        //Profiles  
                        profiles = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.CompanyId == companyId && x.ProfileId.HasValue && x.Teams.Any(u => u.UserId == userId)).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.Id,
                            ProfileId = z.Profile.Id,
                            ProfileName = z.Profile.Name,
                            ProfileType = (ProfilesType)z.Profile.ProfileTypeId,
                            ProfileTypeId = z.Profile.ProfileTypeId,
                            CompanyId = z.CompanyId,
                            IsConnected = z.Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.Profile.ProfilePic,
                            AccountName = z.Profile.UserName,
                            IsPage = false,
                            UserId = z.Profile.ProfileExternalId,
                            Id = z.Profile.Id
                        }).ToList();

                        //Profile Pages


                        var profilePages = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.CompanyId == companyId && x.ProfilePageId.HasValue && x.Teams.Any(u => u.UserId == userId)).Select(z => new ProfileModel
                        {
                            ProfileCompnayGroupId = z.Id,
                            ProfileId = z.ProfilePage.Id,
                            ProfileName = z.ProfilePage.PageName,
                            ProfileType = (ProfilesType)z.ProfilePage.Profile.ProfileTypeId,
                            ProfileTypeId = z.ProfilePage.Profile.ProfileTypeId,
                            CompanyId = z.CompanyId,
                            IsConnected = z.Teams.Any(t => t.UserId == userId),
                            ProfilePicURL = z.ProfilePage.ProfilePic,
                            AccountName = z.Profile.UserName,
                            IsPage = true,
                            UserId = z.ProfilePage.ProfilePageID,
                            PageId = z.ProfilePage.ProfilePageID,
                            Id = z.ProfilePage.Id
                        }).ToList();

                        profiles.AddRange(profilePages);

                        //Twitter Group Key Stats
                        string[] twitterProfilesId = profiles.Where(x => x.ProfileType == ProfilesType.Twitter).Select(y => y.UserId).ToArray();
                        result.gksTwNumOfTweets = db.PostsTwitters.Where(x => twitterProfilesId.Contains(x.UserId)
                                                                              //&& x.UserId != x.AuthorUserId
                                                                              && ((dateFrom != null) ? (x.CreatedDate >= dateFrom) : true)
                                                                              && ((dateTo != null) ? (x.CreatedDate <= dateTo) : true)).Count();



                        string[] fbProfilesId = profiles.Where(x => x.ProfileType == ProfilesType.Facebook && x.IsPage == false).Select(y => y.UserId).ToArray();
                        result.gksFbProfileNumOfPosts = db.PostsFBs.Where(x => fbProfilesId.Contains(x.UserId)
                                                                               //&& x.UserId != x.AuthorUserId
                                                                               && ((dateFrom != null) ? (x.CreatedDate >= dateFrom) : true)
                                                                              && ((dateTo != null) ? (x.CreatedDate <= dateTo) : true)).Count();

                        string[] fbProfilePageIds = profiles.Where(x => x.ProfileType == ProfilesType.Facebook && x.IsPage == true).Select(y => y.UserId).ToArray();
                        result.gksFbPageNumOfPosts = db.PostsFBs.Where(x => fbProfilePageIds.Contains(x.PageId)
                                                                            //&& x.PageId != x.AuthorUserId
                                                                            && ((dateFrom != null) ? (x.CreatedDate >= dateFrom) : true)
                                                                            && ((dateTo != null) ? (x.CreatedDate <= dateTo) : true)).Count();

                        int[] instaProfileIds = profiles.Where(x => x.ProfileType == ProfilesType.Instagram).Select(y => y.Id).ToArray();
                        result.gksInstaNumOfPosts = db.InstagramMinnedDatas.Where(x => instaProfileIds.Contains(x.InstagramAccountId)
                                                                                        && ((dateFrom != null) ? (x.CreatedDate >= dateFrom) : true)
                                                                                        && ((dateTo != null) ? (x.CreatedDate <= dateTo) : true)).Count();
                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
    }
}