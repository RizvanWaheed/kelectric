using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialCRM.Web.Models.Report;
using System.Threading.Tasks;
using CRM.Entities;
using SocialCRM.Web.Models.TaskManager;
using SocialCRM.Web.Models.Dashboard;
using SocialCRM.Web.Models.Profiles;
using CRM.Common.Enum;
using CRM.Common.Extensions;
using SocialCRM.Web.Models.Twitter;
using System.Transactions;
using System.Data.Entity;

namespace SocialCRM.Web.Repositories
{
    public class ReportRepository : IReportRepository
    {
        public async Task<TaskPerformanceReportModel> GetPerformanceReport(int userId, int groupId, int companyId, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum)
        {
            TaskPerformanceReportModel result = new TaskPerformanceReportModel();
            TaskRepository taskRepository = new TaskRepository();

            using (var db = new SocialCRMEntities())
            {
                try
                {
                    switch (dateFilterEnum)
                    {
                        case DateFilter.Last15Days:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last30Days:
                            dateFrom = DateTime.Now.AddDays(-30);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last60Days:
                            dateFrom = DateTime.Now.AddDays(-60);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last90Days:
                            dateFrom = DateTime.Now.AddDays(-90);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Custom:
                            break;
                        default:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            break;
                    }

                    TaskSearchModel model = new TaskSearchModel();
                    model.GroupId = groupId;
                    var data = await taskRepository.GetFilteredTask(userId, model);
                    data = data.Where(x => ((dateFrom != null) ? (x.CreatedDate.Value.Date >= dateFrom.Value.Date) : true)
                                            && ((dateTo != null) ? (x.CreatedDate.Value.Date <= dateTo.Value.Date) : true)).ToList();
                    //if (data != null && data.Count > 0)
                    {
                        var currentDate = DateTime.Now.AddDays(-30);
                        result.TasksByDate = new List<TaskInDates>();
                        for (DateTime date = currentDate; currentDate <= DateTime.Now; currentDate = currentDate.AddDays(1))
                        {
                            TaskInDates task = new TaskInDates();
                            task.Date = currentDate.ToString("dd/MM/yyyy");
                            task.NumOfTasks = data.Where(x => x.CreatedDate.Value.Date == currentDate.Date).Count();
                            result.TasksByDate.Add(task);
                        }

                        result.TotalAssignedTasks = data.Count;
                        result.TotalCompletedTasks = data.Where(x => x.Status == CRM.Common.Enum.TaskStatus.Close).Count();

                        double prevMonthCount = data.Where(x => x.CreatedDate >= DateTime.Now.AddMonths(-2) && x.CreatedDate <= DateTime.Now.AddMonths(-1)).Count();
                        double currentMonthCount = data.Where(x => x.CreatedDate >= DateTime.Now.AddMonths(-1)).Count();
                        double percent = prevMonthCount > 0 ? ((currentMonthCount - prevMonthCount) / prevMonthCount) * 100 : 0;
                        result.AssignedTasksIncreased = Convert.ToString(percent);

                        result.TaskPerformance = new List<TaskPerformanceByMember>();
                        var taskIds = data.Select(x => x.TaskId).ToArray();
                        var assignedTasks = db.TaskAssignees.Where(x => taskIds.Contains(x.TaskManagerId)).ToList();
                        var distinctTeamMembers = assignedTasks.Select(x => x.TeamMemberId).Distinct().ToArray();
                        foreach (var id in distinctTeamMembers)
                        {
                            var tasks = assignedTasks.Where(x => x.TeamMemberId == id).ToList();
                            TaskPerformanceByMember member = new TaskPerformanceByMember();
                            member.TeamMemberId = id;
                            member.TeamMemberName = tasks.FirstOrDefault().Team.FirstName + " " + tasks.FirstOrDefault().Team.LastName;
                            member.TotalAssignedTasks = tasks.Count;
                            member.TotalCompletedTasks = tasks.Where(x => x.TaskManager.StatusId == 2).Count();
                            member.CompletionRatio = Math.Round((member.TotalCompletedTasks / member.TotalAssignedTasks) * 100, 2);
                            member.FirstTouch = "";
                            double time = 0;
                            foreach (var item in tasks.Where(x => x.TaskManager.StatusId == 2))
                            {
                                time += (item.TaskManager.UpdatedDate.Value - item.TaskManager.CreatedDate).Minutes;
                            }
                            member.TaskResolution = string.Format("{0} hrs", Math.Round(time / 60, 2));

                            time = 0;
                            foreach (var item in tasks.Where(x => x.TaskManager.StatusId != 2))
                            {
                                time += (DateTime.Now - item.TaskManager.CreatedDate).Minutes;
                            }
                            member.ActiveTaskTime = string.Format("{0} hrs", Math.Round(time / 60, 2));

                            result.TaskPerformance.Add(member);
                        }
                    }
                }
                catch (Exception exp)
                {
                    result = null;
                }
            }
            return result;
        }

        public async Task<TeamReportModel> GetTeamReport(int userId, int groupId, int companyId, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum, string selectedTeamMembers, bool isCurrUserAdmin, int currUserTeamId)
        {
            TeamReportModel result = new TeamReportModel();
            TaskRepository taskRepository = new TaskRepository();
            DateTime previousFromDate = DateTime.MinValue;
            DateTime previousToDate = DateTime.MinValue ;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    switch (dateFilterEnum)
                    {
                        case DateFilter.Last15Days:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            previousToDate = dateTo.Value.AddDays(-1);
                            previousFromDate = previousToDate.AddDays(-15);
                            break;
                        case DateFilter.Last30Days:
                            dateFrom = DateTime.Now.AddDays(-30);
                            dateTo = DateTime.Now;
                            previousToDate = dateTo.Value.AddDays(-1);
                            previousFromDate = previousToDate.AddDays(-30);
                            break;
                        case DateFilter.Last60Days:
                            dateFrom = DateTime.Now.AddDays(-60);
                            dateTo = DateTime.Now;
                            previousToDate = dateTo.Value.AddDays(-1);
                            previousFromDate = previousToDate.AddDays(-60);
                            break;
                        case DateFilter.Last90Days:
                            dateFrom = DateTime.Now.AddDays(-90);
                            dateTo = DateTime.Now;
                            previousToDate = dateTo.Value.AddDays(-1);
                            previousFromDate = previousToDate.AddDays(-90);
                            break;
                        case DateFilter.Custom:
                            int range = (dateTo.Value.Date - dateFrom.Value.Date).Days;
                            previousToDate = dateTo.Value.AddDays(-1);
                            previousFromDate = previousToDate.AddDays(-range);
                            break;
                        default:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            previousToDate = dateTo.Value.AddDays(-1);
                            previousFromDate = previousToDate.AddDays(-15);
                            break;
                    }

                    var teamMemberIds = db.Teams.Where(x => x.CompanyId == companyId && (isCurrUserAdmin == true || currUserTeamId == x.Id)).Select(y => y.Id).ToList();
                    result.ReplyByTeamMember = new List<ProfileAndTeamReportNew>();
                    var teams = db.Teams.ToList();
                    List<TimeModel> totaltimeList = new List<TimeModel>();
                    List<TimeModel> prevTotaltimeList = new List<TimeModel>();
                    List<ThreadModel> totalthreadList = new List<ThreadModel>();
                    double avgReplyTime = 0, medianReplyTime = 0, quickestReply = 0, replyThreadSize = 0, slowestReply = 0;
                    int totalReplies = 0;
                    double prevTotalReplies = 0;

                    #region Reply By Team Report
                    previousFromDate = new DateTime(previousFromDate.Year, previousFromDate.Month, previousFromDate.Day, 0, 0, 0, 0);
                    previousToDate = new DateTime(previousToDate.Year, previousToDate.Month, previousToDate.Day, 23, 59, 59, 999);
                    foreach (var team in teamMemberIds)
                    {
                        /*
                        List<SocialActivity> activitiesList = db.SocialActivities.Where(x => x.TeamId == team && x.ActivityTypeId == 2).ToList();

                        var activities = activitiesList.Where(x => ((dateFrom.HasValue) ? (x.CreatedDate.Value.Date >= dateFrom.Value.Date) : true) && ((dateTo.HasValue) ? (x.CreatedDate.Value.Date <= dateTo.Value.Date) : true)).ToList();
                        var prevActivities = activitiesList.Where(x => ((previousFromDate.HasValue) ? (x.CreatedDate.Value.Date >= previousFromDate.Value.Date) : true) && ((previousToDate.HasValue) ? (x.CreatedDate.Value.Date <= previousToDate.Value.Date) : true)).ToList();
                        //var activities = activitiesList.ToList().Where(x => x.CreatedDate.Value.Date >= dateFrom.Value.Date).ToList();
                        List<TimeModel> timeList = new List<TimeModel>();
                        List<ThreadModel> threadList = new List<ThreadModel>();

                        List<TimeModel> prevTimeList = new List<TimeModel>();
                        List<ThreadModel> prevThreadList = new List<ThreadModel>();

                        //FC Profile Comment
                        var facebookReplies = activities.Where(x => x.FeedType == "FC" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] commentIds = activities.Where(x => x.FeedType == "FC" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in facebookReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {
                                DateTime postDate = postDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FC" && x.FeedId == postId && String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfileId;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments6 = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments6.Count, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                var comments = comments6.Where(x => (x.PostId == postId && x.FromId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {

                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //FC Profile Nested Comment
                        avgReplyTime = 0;
                        var facebookNestedReplies = activities.Where(x => x.FeedType == "FCN" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] nestedCommentIds = activities.Where(x => x.FeedType == "FCN" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in facebookNestedReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.CommentId == commentId).FirstOrDefault();
                            if (commentDetails != null)
                            {
                                DateTime postDate = commentDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FCN" && x.FeedId == commentId && String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfileId;
                                bool isOurPost = commentDetails.FromId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || nestedCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }


                        // prev date FC Profile Comment
                        avgReplyTime = 0;
                        var prevFacebookReplies = prevActivities.Where(x => x.FeedType == "FC" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] prevCommentIds = prevActivities.Where(x => x.FeedType == "FC" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in prevFacebookReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {
                                DateTime postDate = postDetails.CreatedDate.Value;
                                string profileId = prevActivities.Where(x => x.FeedType == "FC" && x.FeedId == postId && String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfileId;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                prevThreadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == postId && x.FromId != profileId) || prevCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {

                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //prev FC Profile Nested Comment
                        avgReplyTime = 0;
                        var prevFacebookNestedReplies = prevActivities.Where(x => x.FeedType == "FCN" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] prevNestedCommentIds = prevActivities.Where(x => x.FeedType == "FCN" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in prevFacebookNestedReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.CommentId == commentId).FirstOrDefault();
                            if (commentDetails != null)
                            {

                                DateTime postDate = commentDetails.CreatedDate.Value;
                                string profileId = prevActivities.Where(x => x.FeedType == "FCN" && x.FeedId == commentId && String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfileId;
                                bool isOurPost = commentDetails.FromId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || prevNestedCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        //

                        //FC Page
                        avgReplyTime = 0;
                        var facebookPageReplies = activities.Where(x => x.FeedType == "FC" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        commentIds = activities.Where(x => x.FeedType == "FC" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in facebookPageReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && !String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {

                                DateTime postDate = postDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FC" && x.FeedId == postId && !String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfilePageId;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == postId && x.FromId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //FC Page Nested Replies
                        avgReplyTime = 0;
                        var nestedFacebookPageReplies = activities.Where(x => x.FeedType == "FCN" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        nestedCommentIds = activities.Where(x => x.FeedType == "FCN" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in nestedFacebookPageReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.CommentId == commentId).FirstOrDefault(); ;
                            if (commentDetails != null)
                            {

                                DateTime postDate = commentDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FCN" && x.FeedId == commentId && !String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfilePageId;
                                bool isOurPost = commentDetails.FromId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || nestedCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }


                        // prev date FC Page
                        avgReplyTime = 0;
                        var prevFacebookPageReplies = prevActivities.Where(x => x.FeedType == "FC" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        prevCommentIds = prevActivities.Where(x => x.FeedType == "FC" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in prevFacebookPageReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && !String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {

                                DateTime postDate = postDetails.CreatedDate.Value;
                                string profileId = prevActivities.Where(x => x.FeedType == "FC" && x.FeedId == postId && !String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfilePageId;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                comments = comments.Where(x => (x.PostId == postId && x.FromId != profileId) || prevCommentIds.Contains(x.CommentId)).ToList();
                                prevThreadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //FC Page Nested Replies
                        avgReplyTime = 0;
                        var prevNestedFacebookPageReplies = prevActivities.Where(x => x.FeedType == "FCN" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        prevNestedCommentIds = prevActivities.Where(x => x.FeedType == "FCN" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in prevNestedFacebookPageReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.CommentId == commentId).FirstOrDefault(); ;
                            if (commentDetails != null)
                            {

                                DateTime postDate = commentDetails.CreatedDate.Value;
                                string profileId = prevActivities.Where(x => x.FeedType == "FCN" && x.FeedId == commentId && !String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfilePageId;
                                bool isOurPost = commentDetails.FromId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || prevNestedCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }


                        //

                        //FB Page Chat
                        avgReplyTime = 0;
                        var pageChatReplies = activities.Where(x => x.FeedType == "FCP").Select(y => y.FeedId).Distinct().ToArray();
                        string[] msgIds = activities.Where(x => x.FeedType == "FCP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in pageChatReplies)
                        {
                            string profileId = activities.Where(x => x.FeedType == "FCP" && x.FeedId == postId).FirstOrDefault().ProfilePageId;
                            var msgs = db.PageChatsFBs.Where(x => x.ConId == postId).OrderBy(y => y.CreatedDate).ToList();
                            //threadList.Add(msgs.Count);
                            threadList.Add(new ThreadModel { Size = msgs.Count, Type = "FB_PAGE", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                            msgs = msgs.Where(x => (x.ConId == postId && x.FromId != profileId) || msgIds.Contains(x.MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FromId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FromId != msgs[i - 1].FromId)
                                        {
                                            avgReplyTime = (msgs[i].CreatedDate.Value - msgs[i - 1].CreatedDate.Value).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                                        }
                                    }
                                }
                            }
                        }


                        //prev date FB Page Chat
                        avgReplyTime = 0;
                        var prevPageChatReplies = prevActivities.Where(x => x.FeedType == "FCP").Select(y => y.FeedId).Distinct().ToArray();
                        string[] prevMsgIds = prevActivities.Where(x => x.FeedType == "FCP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in prevPageChatReplies)
                        {
                            string profileId = prevActivities.Where(x => x.FeedType == "FCP" && x.FeedId == postId).FirstOrDefault().ProfilePageId;
                            var msgs = db.PageChatsFBs.Where(x => x.ConId == postId).OrderBy(y => y.CreatedDate).ToList();
                            //threadList.Add(msgs.Count);
                            prevThreadList.Add(new ThreadModel { Size = msgs.Count, Type = "FB_PAGE", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                            msgs = msgs.Where(x => (x.ConId == postId && x.FromId != profileId) || prevMsgIds.Contains(x.MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FromId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FromId != msgs[i - 1].FromId)
                                        {
                                            avgReplyTime = (msgs[i].CreatedDate.Value - msgs[i - 1].CreatedDate.Value).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                                        }
                                    }
                                }
                            }
                        }


                        //

                        //IC
                        avgReplyTime = 0;
                        var instaReplies = activities.Where(x => x.FeedType == "IC").Select(y => y.FeedId).Distinct().ToArray();
                        commentIds = activities.Where(x => x.FeedType == "IC").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in instaReplies)
                        {
                            var postDetails = db.InstagramMinnedDatas.Where(x => x.PostId == postId).FirstOrDefault();
                            DateTime postDate = postDetails.CreatedDate.Value;
                            string profileId = activities.Where(x => x.FeedType == "IC" && x.FeedId == postId).FirstOrDefault().ProfileId;
                            bool isOurPost = db.Profiles.Where(x => x.Id == postDetails.InstagramAccountId).FirstOrDefault().ProfileExternalId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.InstagramComments.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                            //threadList.Add(comments.Count);
                            threadList.Add(new ThreadModel { Size = comments.Count, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                            comments = comments.Where(x => (x.PostId == postId && x.FromUserId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].FromUserId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].FromUserId != comments[i - 1].FromUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!isPostDateAdded)
                                        {
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            timeList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].FromUserId != comments[i - 1].FromUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                }

                            }
                        }


                        // prev date IC
                        avgReplyTime = 0;
                        var prevInstaReplies = prevActivities.Where(x => x.FeedType == "IC").Select(y => y.FeedId).Distinct().ToArray();
                        prevCommentIds = prevActivities.Where(x => x.FeedType == "IC").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in prevInstaReplies)
                        {
                            var postDetails = db.InstagramMinnedDatas.Where(x => x.PostId == postId).FirstOrDefault();
                            if (postDetails != null)
                            {

                                DateTime postDate = postDetails.CreatedDate.Value;
                                string profileId = prevActivities.Where(x => x.FeedType == "IC" && x.FeedId == postId).FirstOrDefault().ProfileId;
                                bool isOurPost = db.Profiles.Where(x => x.Id == postDetails.InstagramAccountId).FirstOrDefault().ProfileExternalId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.InstagramComments.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                //threadList.Add(comments.Count);
                                prevThreadList.Add(new ThreadModel { Size = comments.Count, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == postId && x.FromUserId != profileId) || prevCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromUserId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromUserId != comments[i - 1].FromUserId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!isPostDateAdded)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromUserId != comments[i - 1].FromUserId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //

                        //TM
                        avgReplyTime = 0;
                        var twitterMsgReplies = activities.Where(x => x.FeedType == "TM")
                            .Select(y => new TwitterConversationModel { fromUser = y.FromUser, toUser = y.ToUser }).Distinct().ToArray();
                        msgIds = activities.Where(x => x.FeedType == "TM").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var con in twitterMsgReplies)
                        {
                            string profileId = activities.Where(x => x.FeedType == "TM" && x.FromUser == con.fromUser && x.ToUser == con.toUser).FirstOrDefault().ProfileId;
                            var msgs = db.MessagesTwitters.Where(y => (y.SenderScreenName == con.fromUser && y.RecipientScreenName == con.toUser)
                                                               || (y.SenderScreenName == con.toUser && y.RecipientScreenName == con.fromUser))
                                                               .GroupBy(a => a.MsgId).OrderBy(z => z.FirstOrDefault().CreatedDate).ToList();
                            //threadList.Add(msgs.Count);
                            threadList.Add(new ThreadModel { Size = msgs.Count, Type = "TW", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                            msgs = msgs.Where(x => (x.FirstOrDefault().SenderId != profileId) || msgIds.Contains(x.FirstOrDefault().MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FirstOrDefault().SenderId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FirstOrDefault().SenderId != msgs[i - 1].FirstOrDefault().SenderId)
                                        {
                                            avgReplyTime = (msgs[i].FirstOrDefault().CreatedDate.Value - msgs[i - 1].FirstOrDefault().CreatedDate.Value).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                                        }
                                    }
                                }
                            }
                        }

                        // prev date TM
                        avgReplyTime = 0;
                        var prevTwitterMsgReplies = prevActivities.Where(x => x.FeedType == "TM")
                            .Select(y => new TwitterConversationModel { fromUser = y.FromUser, toUser = y.ToUser }).Distinct().ToArray();
                        prevMsgIds = prevActivities.Where(x => x.FeedType == "TM").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var con in prevTwitterMsgReplies)
                        {
                            string profileId = prevActivities.Where(x => x.FeedType == "TM" && x.FromUser == con.fromUser && x.ToUser == con.toUser).FirstOrDefault().ProfileId;
                            var msgs = db.MessagesTwitters.Where(y => (y.SenderScreenName == con.fromUser && y.RecipientScreenName == con.toUser)
                                                               || (y.SenderScreenName == con.toUser && y.RecipientScreenName == con.fromUser))
                                                               .GroupBy(a => a.MsgId).OrderBy(z => z.FirstOrDefault().CreatedDate).ToList();
                            //threadList.Add(msgs.Count);
                            prevThreadList.Add(new ThreadModel { Size = msgs.Count, Type = "TW", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                            msgs = msgs.Where(x => (x.FirstOrDefault().SenderId != profileId) || prevMsgIds.Contains(x.FirstOrDefault().MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FirstOrDefault().SenderId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FirstOrDefault().SenderId != msgs[i - 1].FirstOrDefault().SenderId)
                                        {
                                            avgReplyTime = (msgs[i].FirstOrDefault().CreatedDate.Value - msgs[i - 1].FirstOrDefault().CreatedDate.Value).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                                        }
                                    }
                                }
                            }
                        }

                        //

                        //TP
                        avgReplyTime = 0;
                        var twitterReplies = activities.Where(x => x.FeedType == "TP").Select(y => y.FeedId).Distinct().ToArray();
                        commentIds = activities.Where(x => x.FeedType == "TP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in twitterReplies)
                        {
                            var postDetails = db.PostsTwitters.Where(x => x.StatusId == postId).FirstOrDefault();
                            DateTime postDate = postDetails.CreatedDate.Value;
                            string profileId = activities.Where(x => x.FeedType == "TP" && x.FeedId == postId).FirstOrDefault().ProfileId;
                            bool isOurPost = postDetails.AuthorUserId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.PostsTwitters.Where(x => x.InReplyToStatusId == postId).OrderBy(y => y.CreatedDate).ToList();
                            comments = comments.Where(x => (x.InReplyToStatusId == postId && x.AuthorUserId != profileId) || commentIds.Contains(x.StatusId)).ToList();
                            threadList.Add(new ThreadModel { Size = comments.Count, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].AuthorUserId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!isPostDateAdded)
                                        {
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        //prev date TP
                        avgReplyTime = 0;
                        var prevTwitterReplies = prevActivities.Where(x => x.FeedType == "TP").Select(y => y.FeedId).Distinct().ToArray();
                        prevCommentIds = prevActivities.Where(x => x.FeedType == "TP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in prevTwitterReplies)
                        {
                            var postDetails = db.PostsTwitters.Where(x => x.StatusId == postId).FirstOrDefault();
                            DateTime postDate = postDetails.CreatedDate.Value;
                            string profileId = prevActivities.Where(x => x.FeedType == "TP" && x.FeedId == postId).FirstOrDefault().ProfileId;
                            bool isOurPost = postDetails.AuthorUserId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.PostsTwitters.Where(x => x.InReplyToStatusId == postId).OrderBy(y => y.CreatedDate).ToList();
                            comments = comments.Where(x => (x.InReplyToStatusId == postId && x.AuthorUserId != profileId) || prevCommentIds.Contains(x.StatusId)).ToList();
                            prevThreadList.Add(new ThreadModel { Size = comments.Count, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].AuthorUserId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!isPostDateAdded)
                                        {
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                prevTimeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        totaltimeList.AddRange(timeList);
                        totalthreadList.AddRange(threadList);
                        prevTotaltimeList.AddRange(prevTimeList);
                        for time begin Comment*/

                        /*if (timeList.Count > 0)
                        {
                            var allTimeList = timeList.Select(x => x.Time).ToList<double>();
                            avgReplyTime = allTimeList.Sum() / allTimeList.Count;
                            medianReplyTime = MathExtentions.GetMedian(allTimeList);
                            quickestReply = allTimeList.Min();
                            slowestReply = allTimeList.Max();
                        }
                        if (threadList.Count > 0)
                            replyThreadSize = threadList.Select(x => x.Size).Sum() / threadList.Count;

                        totalReplies = timeList.Count;
                        prevTotalReplies = prevTimeList.Count;

                        ProfileAndTeamReportNew report = new ProfileAndTeamReportNew();
                        report.ProfileId = team;
                        report.ProfileName = teams.Where(x => x.Id == team).FirstOrDefault().FirstName + " " + teams.Where(x => x.Id == team).FirstOrDefault().LastName;
                        report.AvgReplyTime = string.Format("{0} hrs", Math.Round(avgReplyTime / 3600, 2));
                        report.MedianReplyTime = string.Format("{0} hrs", Math.Round(medianReplyTime / 3600, 2));
                        report.QuickestReply = string.Format("{0} mins", Math.Round(quickestReply / 60, 2));
                        report.ReplyThreadSize = replyThreadSize;
                        report.SlowestReply = string.Format("{0} mins", Math.Round(slowestReply / 60, 2));
                        report.TotalReplies = totalReplies;

                        string percent = (prevTotalReplies > 0) ? Convert.ToString(Math.Round(((Convert.ToDouble(totalReplies) - prevTotalReplies) / prevTotalReplies) * 100,0)) : "0";
                        report.ChangeInResponse = percent == "Infinity" ? 0 : Convert.ToDouble(percent);

                        result.ReplyByTeamMember.Add(report);
                        avgReplyTime = 0;
                        medianReplyTime = 0;
                        quickestReply = 0;
                        replyThreadSize = 0;
                        slowestReply = 0;
                        totalReplies = 0;*/

                        var agentPerformance = db.AgentPerformanceReports.Where(x => x.ProfileId == team &&
                        x.CreatedDate >= previousFromDate && x.CreatedDate <= previousToDate).ToList();
                        var teamMember = db.Teams.Where(x => x.Id == team).FirstOrDefault();
                        if (teamMember != null&& agentPerformance.Count()>0)
                        {
                            ProfileAndTeamReportNew report = new ProfileAndTeamReportNew
                            {
                                ChatCount = agentPerformance.Sum(c => c.ChatCount),
                                CommentCount = agentPerformance.Sum(c => c.CommentCount),
                                FirstShiftOutbound = agentPerformance.Sum(c => c.FirstShiftOutbound),
                                TotalReplies = agentPerformance.Sum(c => c.TotalReplies),
                                TotalPosts = agentPerformance.Sum(c => c.TotalPosts),
                                ThirdShiftOutbound = agentPerformance.Sum(c => c.ThirdShiftOutbound),
                                SecondShiftOutbound = agentPerformance.Sum(c => c.SecondShiftOutbound),
                                ReplyThreadSize = agentPerformance.Sum(c => c.ReplyThreadSize),
                                ProfileId = team,
                                ChangeInResponse = agentPerformance.Sum(c => c.ChangeInResponse),
                                MedianReplyTime = string.Format("{0} hrs", Math.Round(Convert.ToDouble(MathExtentions.GetMedian(agentPerformance.Select(e => e.MedianReplyTime).ToList())) / 3600, 2)),
                                QuickestReply = string.Format("{0} mins", Math.Round(Convert.ToDouble(agentPerformance.Min(m => m.QuickestReply)) / 60, 2)),
                                SlowestReply = string.Format("{0} mins", Math.Round(Convert.ToDouble(agentPerformance.Max(m => m.QuickestReply)) / 60, 2)),
                                ProfileName=teamMember.FirstName +" "+teamMember.LastName,
                                ProfilePic=teamMember.AspNetUser.ProfileImage,
                            };
                            if (agentPerformance.Count() > 0)
                                report.AvgReplyTime = string.Format("{0} hrs", Math.Round(Convert.ToDouble(agentPerformance.Sum(x => x.AvgReplyTime)) / 3600, 2));
                            else
                                report.AvgReplyTime = "00 hrs";

                            result.ReplyByTeamMember.Add(report);
                        }
                    }
                    #endregion
                    
                    #region Reply By Team and Profile Report
                    result.ReplyByProfileAndTeamMemberNew = new List<ProfileAndTeamReportNew>();
                    totalthreadList = totalthreadList.Where(x => x.Size > 0).ToList();

                    List<string> types = new List<string>() { "FB_PROFILE", "FB_PAGE", "IC", "TW" };
                    var dbProfiles = db.Profiles.ToList();
                    var dbPages = db.ProfilePages.ToList();
                    avgReplyTime = 0;
                    medianReplyTime = 0;
                    quickestReply = 0;
                    replyThreadSize = 0;
                    slowestReply = 0;
                    totalReplies = 0;
                    //var distinctTeamIds = timeList.Select(x => x.TeamMemberId).Distinct().ToArray();
                    var distinctTeamIds = db.Teams.Where(x => x.CompanyId == companyId).Select(y => y.Id).ToArray();
                    TimeSpan timeFirst = new TimeSpan(8, 0, 0);
                    TimeSpan timeSecond = new TimeSpan(16, 0, 0);
                    TimeSpan timeThird = new TimeSpan(23, 59, 59);
                    TimeSpan timeThirdNextDay = new TimeSpan(00, 00, 00);
                    foreach (int team in distinctTeamIds)
                    {
                        
                        //From Publisher
                        var smptot = db.MessagePostingTimes.Where(x => x.IsPublished
                                                                && x.ScheduledMessageProfile.TeamId == team
                                                                && ((dateFrom != null) ? (x.PublishDate >= dateFrom) : true)
                                                                && ((dateTo != null) ? (x.PublishDate <= dateTo) : true)
                                                                ).Select(x => x.ScheduledMessageProfile).ToList();
                        //From Compose ActivityTypeId==2> Retweet && 3>Publish
                        var allTeamSocialActivities = db.SocialActivities.Where(x => x.TeamId == team
                                                                && x.CreatedDate >= dateFrom
                                                                && x.CreatedDate <= dateTo).ToList();
                        var prevAllTeamSocialActivities = db.SocialActivities.Where(x => x.TeamId == team
                                                                && x.CreatedDate >= previousFromDate
                                                                && x.CreatedDate <= previousToDate).ToList();
                        var pbact = allTeamSocialActivities.Where(x => x.ActivityTypeId == 3).ToList();
                        var rpact = allTeamSocialActivities.Where(x => x.ActivityTypeId == 2).ToList();
                        var prevrpact = prevAllTeamSocialActivities.Where(x => x.ActivityTypeId == 2).ToList();
                        var outact = allTeamSocialActivities.Where(x => x.ActivityTypeId == 3 || x.ActivityTypeId == 2).ToList();
                        

                        var allTeamProfilesTime = totaltimeList.Where(x => x.TeamMemberId == team).ToList();
                        var allTeamPrevProfilesTime = prevTotaltimeList.Where(x => x.TeamMemberId == team).ToList();
                        var allTeamProfilesThread = totalthreadList.Where(x => x.TeamMemberId == team).ToList();
                        
                        ProfileAndTeamReportNew teamRpt = new ProfileAndTeamReportNew();
                        teamRpt = result.ReplyByTeamMember.Where(t=>t.ProfileId==team).FirstOrDefault();
                        if(teamRpt==null) teamRpt = new ProfileAndTeamReportNew();
                        /*teamRpt.ProfileId = team;
                        var teamMember = teams.Where(x => x.Id == team).FirstOrDefault();
                        teamRpt.ProfileName = teamMember.FirstName + " " + teamMember.LastName;
                        teamRpt.ProfilePic = teamMember.AspNetUser.ProfileImage;

                        if (allTeamProfilesTime.Count > 0)
                        {
                            var allTimeList = allTeamProfilesTime.Where(p => p.TeamMemberId == team).Select(x => x.Time).ToList<double>();
                            avgReplyTime = allTimeList.Sum() / allTimeList.Count;
                            medianReplyTime = MathExtentions.GetMedian(allTimeList);
                            quickestReply = allTimeList.Min();
                            slowestReply = allTimeList.Max();
                        }
                        if (allTeamProfilesThread.Count > 0)
                            replyThreadSize = allTeamProfilesThread.Where(p => p.TeamMemberId == team).Select(x => x.Size).Sum() / allTeamProfilesThread.Where(p => p.TeamMemberId == team).ToList().Count;
                        totalReplies = rpact.Count;

                        teamRpt.FirstShiftOutbound = outact.Where(x => x.CreatedDate.Value.TimeOfDay >= timeFirst && x.CreatedDate.Value.TimeOfDay < timeSecond).ToList().Count;
                        teamRpt.FirstShiftOutbound += smptot.Where(x => x.MessagePostingTimes.Any(y => y.PublishTime >= timeFirst && y.PublishTime < timeSecond)).ToList().Count;
                        teamRpt.SecondShiftOutbound = outact.Where(x => x.CreatedDate.Value.TimeOfDay >= timeSecond && x.CreatedDate.Value.TimeOfDay < timeThird).ToList().Count;
                        teamRpt.SecondShiftOutbound += smptot.Where(x => x.MessagePostingTimes.Any(y => y.PublishTime >= timeSecond && y.PublishTime < timeThird)).ToList().Count;
                        teamRpt.ThirdShiftOutbound = outact.Where(x => x.CreatedDate.Value.TimeOfDay >= timeThirdNextDay && x.CreatedDate.Value.TimeOfDay < timeFirst).ToList().Count;
                        teamRpt.ThirdShiftOutbound += smptot.Where(x => x.MessagePostingTimes.Any(y => y.PublishTime >= timeThirdNextDay && y.PublishTime < timeFirst)).ToList().Count;
                        teamRpt.AvgReplyTime = string.Format("{0} hrs", Math.Round(avgReplyTime / 3600, 2));
                        teamRpt.MedianReplyTime = string.Format("{0} hrs", Math.Round(medianReplyTime / 3600, 2));
                        teamRpt.QuickestReply = string.Format("{0} mins", Math.Round(quickestReply / 60, 2));
                        teamRpt.ReplyThreadSize = replyThreadSize;
                        teamRpt.SlowestReply = string.Format("{0} mins", Math.Round(slowestReply / 60, 2));
                        teamRpt.TotalReplies = totalReplies;
                        teamRpt.TotalPosts = smptot.Count;
                        teamRpt.TotalPosts += pbact.Count;
                        prevTotalReplies = prevrpact.Count;
                        string percent = (prevTotalReplies > 0) ? Convert.ToString(Math.Round(((Convert.ToDouble(totalReplies) - prevTotalReplies) / prevTotalReplies) * 100, 0)) : "0";
                        teamRpt.ChangeInResponse = percent == "Infinity" ? 0 : Convert.ToDouble(percent);

                        teamRpt.CommentCount = rpact.Where(x => x.FeedType == "FC" || x.FeedType == "FCN" || x.FeedType == "TP" || x.FeedType == "IC").Count();
                        teamRpt.ChatCount = rpact.Where(x => x.FeedType == "FCP" || x.FeedType == "TM").Count();
                        */
                        teamRpt.ProfilesReport = new List<ProfileReport>();
                        foreach (var profileType in types)
                        {
                            var profileTimelist = allTeamProfilesTime.Where(x => x.Type == profileType).ToList();
                            var profileThreadlist = allTeamProfilesThread.Where(x => x.Type == profileType).ToList();
                            string[] distinctProfilesTimeList = null;
                            var distinctProfilesThreadList = profileThreadlist.Select(x => x.ProfileId).Distinct().ToArray();
                            if (profileType == "FB_PROFILE")
                            {
                                distinctProfilesTimeList = profileTimelist.Select(x => x.ProfileId).Union(pbact.Where(x => x.ProfilePageId == "" && x.FeedType == "FP").Select(x => x.ProfileId)).Distinct().ToArray();
                            }
                            else if (profileType == "FB_PAGE")
                            {
                                distinctProfilesTimeList = profileTimelist.Select(x => x.ProfileId).Union(pbact.Where(x => x.ProfileId == "" && x.FeedType == "FP").Select(x => x.ProfilePageId)).Distinct().ToArray();
                            }
                            else if (profileType == "TW")
                            {
                                distinctProfilesTimeList = profileTimelist.Select(x => x.ProfileId).Union(pbact.Where(x => x.FeedType == "TP").Select(x => x.ProfileId)).Distinct().ToArray();
                                pbact.Where(x => x.FeedType == "TP").Select(x => x.ProfileId);
                            }
                            else if (profileType == "IC")
                            {
                                distinctProfilesTimeList = profileTimelist.Select(x => x.ProfileId).Union(pbact.Where(x => x.FeedType == "IC").Select(x => x.ProfileId)).Distinct().ToArray();
                            }

                            foreach (var profile in distinctProfilesTimeList)
                            {
                                ProfileReport report = new ProfileReport();
                                report.ProfileId = profile;
                                if (profileType == "FB_PAGE")
                                {
                                    var FBProfilePage = dbPages.Where(x => x.ProfilePageID == profile).FirstOrDefault();
                                    report.ProfileName = FBProfilePage.PageName;
                                    report.ProfileUserName = FBProfilePage.PageName;
                                    report.ProfileType = "FB_PAGE";
                                    report.ProfilePic = FBProfilePage.ProfilePic;
                                    report.TotalPosts = smptot.Where(x => x.ProfileComapnyGroup.ProfilePageId == FBProfilePage.Id).ToList().Count;
                                    report.TotalPosts += pbact.Where(x => x.ProfilePageId == profile).ToList().Count;
                                    report.TotalReplies = rpact.Where(p => p.ProfilePageId == profile).ToList().Count;
                                }
                                else
                                {
                                    if (profileType == "FB_PROFILE")
                                    {
                                        report.ProfileType = "FB_PROFILE";
                                    }
                                    else
                                        report.ProfileType = profileType;

                                    var Profile = dbProfiles.Where(x => x.ProfileExternalId == profile).FirstOrDefault();
                                    report.ProfileName = Profile.Name;
                                    report.ProfileUserName = Profile.UserName;
                                    report.ProfilePic = Profile.ProfilePic;
                                    report.TotalPosts = smptot.Where(x => x.ProfileComapnyGroup.ProfileId == Profile.Id).ToList().Count;
                                    report.TotalPosts += pbact.Where(x => x.ProfileId == profile).ToList().Count;
                                    report.TotalReplies = rpact.Where(p => p.ProfileId == profile).ToList().Count;
                                }

                                if (profileTimelist.Count > 0)
                                {
                                    var allTimeList = profileTimelist.Where(p => p.ProfileId == profile).Select(x => x.Time).ToList<double>();
                                    avgReplyTime = allTimeList.Sum() / allTimeList.Count;
                                    medianReplyTime = MathExtentions.GetMedian(allTimeList);
                                    quickestReply = allTimeList.Min();
                                    slowestReply = allTimeList.Max();
                                }
                                if (profileThreadlist.Count > 0)
                                    replyThreadSize = profileThreadlist.Where(p => p.ProfileId == profile).Select(x => x.Size).Sum() / profileThreadlist.Where(p => p.ProfileId == profile).ToList().Count;

                                report.AvgReplyTime = string.Format("{0} hrs", Math.Round(avgReplyTime / 3600, 2));
                                report.MedianReplyTime = string.Format("{0} hrs", Math.Round(medianReplyTime / 3600, 2));
                                report.QuickestReply = string.Format("{0} mins", Math.Round(quickestReply / 60, 2));
                                report.ReplyThreadSize = replyThreadSize;
                                report.SlowestReply = string.Format("{0} mins", Math.Round(slowestReply / 60, 2));

                                if (profileType == "FB_PROFILE")
                                {
                                    report.CommentCount = rpact.Where(x => (x.ProfileId == profile) && (x.FeedType == "FC" || x.FeedType == "FCN")).Count();
                                    report.ChatCount = 0;
                                }
                                else if (profileType == "FB_PAGE")
                                {
                                    report.CommentCount = rpact.Where(x => (x.ProfilePageId == profile) && (x.FeedType == "FC" || x.FeedType == "FCN")).Count();
                                    report.ChatCount = rpact.Where(x => (x.ProfilePageId == profile) && x.FeedType == "FCP").Count();
                                }
                                else if (profileType == "TW")
                                {
                                    report.CommentCount = rpact.Where(x => x.ProfileId == profile && x.FeedType == "TP").Count();
                                    report.ChatCount = rpact.Where(x => x.ProfileId == profile && x.FeedType == "TM").Count();
                                }
                                else if (profileType == "IC")
                                {
                                    report.CommentCount = rpact.Where(x => x.ProfileId == profile && x.FeedType == "IC").Count();
                                    report.ChatCount = 0;
                                }

                                teamRpt.ProfilesReport.Add(report);
                            }
                        }
                        result.ReplyByProfileAndTeamMemberNew.Add(teamRpt);

                    }
                    #endregion

                    #region Reply By Publishing, Team Member

                    result.PublishByTeamMember = new List<PublishReport>();

                    var scheduledMessageProfiles = db.MessagePostingTimes.ToList().Where(x => x.IsPublished
                                                                                              && teamMemberIds.Contains(x.ScheduledMessageProfile.TeamId.Value)
                                                                                              && ((dateFrom != null) ? (x.PublishDate.Date >= dateFrom.Value.Date) : true)
                                                                                              && ((dateTo != null) ? (x.PublishDate.Date <= dateTo.Value.Date) : true)
                                                                                              ).Select(x => x.ScheduledMessageProfile).ToList();
                    double numOfDays = 1;
                    if (dateTo.HasValue && dateFrom.HasValue)
                    {
                        numOfDays = (dateTo.Value.Date - dateFrom.Value.Date).Days;
                    }
                    else
                    {
                        DateTime startDate = scheduledMessageProfiles.OrderBy(x => x.MessagePostingTimes.FirstOrDefault().PublishDate).FirstOrDefault().MessagePostingTimes.FirstOrDefault().PublishDate;
                        DateTime endDate = scheduledMessageProfiles.OrderByDescending(x => x.MessagePostingTimes.FirstOrDefault().PublishDate).FirstOrDefault().MessagePostingTimes.FirstOrDefault().PublishDate;
                        numOfDays = (endDate.Date - startDate.Date).Days;
                    }
                    foreach (var team in result.ReplyByTeamMember)
                    {
                        var messages = scheduledMessageProfiles.Where(x => x.TeamId == team.ProfileId).ToList();
                        PublishReport publishRpt = new PublishReport();
                        publishRpt.ProfileId = team.ProfileId;
                        publishRpt.ProfileName = team.ProfileName;
                        publishRpt.ProfileType = team.ProfileType;
                        publishRpt.TotalPosts = messages.Count;
                        publishRpt.DailyAvg = Math.Round(Convert.ToDouble(messages.Count) / numOfDays,2);
                        publishRpt.Replies = team.TotalReplies;

                        double percent =  (scheduledMessageProfiles.Count > 0) ? Math.Round((Convert.ToDouble(messages.Count) * 100) / Convert.ToDouble(scheduledMessageProfiles.Count),2) : 0;
                        publishRpt.ReplyToPostRatio = percent;

                        result.PublishByTeamMember.Add(publishRpt);
                    }
                    #endregion

                    selectedTeamMembers = selectedTeamMembers == "0" ? "" : selectedTeamMembers;
                    var selectedTeamIds = new List<int>();
                    if (!String.IsNullOrEmpty(selectedTeamMembers))
                        selectedTeamIds = selectedTeamMembers.Split(',').Select(x => Convert.ToInt32(x)).ToList<int>();
                    else
                        selectedTeamIds = teamMemberIds;

                    result.TeamStats = new TeamActivityModel();
                    var smp = db.MessagePostingTimes.Where(x => x.IsPublished
                                                                    && selectedTeamIds.Contains(x.ScheduledMessageProfile.TeamId.Value)
                                                                    && ((dateFrom != null) ? (x.PublishDate >= dateFrom) : true)
                                                                    && ((dateTo != null) ? (x.PublishDate <= dateTo) : true)
                                                                    ).Select(x => x.ScheduledMessageProfile).ToList();

                    var allSocialActivities = db.SocialActivities.Where(x => x.CreatedDate >= dateFrom && x.CreatedDate <= dateTo && selectedTeamIds.Contains(x.TeamId.Value)).ToList();
                    var allProfileFeedTags = db.ProfileFeedsTags.Where(x => x.AdditionDate >= dateFrom && x.AdditionDate <= dateTo && x.AddedBy != null && selectedTeamIds.Contains(x.AddedBy.Value)).ToList();
                    var allFeedSentiments = db.FeedsSentiments.Where(x => x.AdditionDate >= dateFrom && x.AdditionDate <= dateTo && x.AddedBy != null && selectedTeamIds.Contains(x.AddedBy.Value)).ToList();
                    var allWarnBannedUsers = db.WarnBanCustomers.Where(x => x.CreatedDate >= dateFrom && x.CreatedDate <= dateTo && selectedTeamIds.Contains(x.BannedById.Value)).ToList();

                    result.TeamStats.TotalFBPosts = allSocialActivities.Where(x => x.ActivityTypeId == 3 && x.FeedType == "FP").ToList().Count;
                    result.TeamStats.TotalFBPosts += smp.Where(x => x.ProfileComapnyGroup.Profile.ProfileTypeId == (int)ProfilesType.Twitter).ToList().Count;
                    result.TeamStats.TotalFBComments = allSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "FC").ToList().Count;
                    result.TeamStats.TotalFBNestComments = allSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "FCN").ToList().Count;
                    result.TeamStats.TotalFBPageChats = allSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "FCP").ToList().Count;
                    result.TeamStats.TotalTwitTweets = allSocialActivities.Where(x => x.ActivityTypeId == 3 && x.FeedType == "TP").ToList().Count;
                    result.TeamStats.TotalTwitTweets += smp.Where(x => x.ProfileComapnyGroup.Profile.ProfileTypeId == (int)ProfilesType.Facebook).ToList().Count;
                    result.TeamStats.TotalTwitReplies = allSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "TP").ToList().Count;
                    result.TeamStats.TotalTwitDMs = allSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "TM").ToList().Count;
                    result.TeamStats.TotalDelFBPosts = allSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "FP").ToList().Count;
                    result.TeamStats.TotalDelFBComments = allSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "FC").ToList().Count;
                    result.TeamStats.TotalDelFBNestComments = allSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "FCN").ToList().Count;
                    result.TeamStats.TotalDelTwitTweets = allSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "TP").ToList().Count;
                    result.TeamStats.TotalDelTwitDMs = allSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "TM").ToList().Count;
                    result.TeamStats.TotalDelInstaComments = allSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "IC").ToList().Count;
                    result.TeamStats.TotalWarnedUsers = allWarnBannedUsers.Where(x => x.StatusId == 1).ToList().Count;
                    result.TeamStats.TotalBannedUsers = allWarnBannedUsers.Where(x => x.StatusId == 2).ToList().Count;
                    result.TeamStats.TotalTagsAdded = allProfileFeedTags.Count();
                    result.TeamStats.TotalSentimentsAdded = allFeedSentiments.Count();

                    result.TeamStatsEvolution = new List<TeamActivityModel>();
                    foreach (DateTime day in EachDay(dateFrom.Value, dateTo.Value))
                    {
                        var selSmp = smp.Where(x => x.MessagePostingTimes.Any(y => y.PublishDate >= day && y.PublishDate <= day.AddHours(24))).ToList();
                        var selSocialActivities = allSocialActivities.Where(y => y.CreatedDate >= day && y.CreatedDate <= day.AddHours(24)).ToList();
                        var selProfileFeedTags = allProfileFeedTags.Where(y => y.AdditionDate >= day && y.AdditionDate <= day.AddHours(24)).ToList();
                        var selFeedSentiments = allFeedSentiments.Where(y => y.AdditionDate >= day && y.AdditionDate <= day.AddHours(24)).ToList();
                        var selWarnBannedUsers = allWarnBannedUsers.Where(y => y.CreatedDate >= day && y.CreatedDate <= day.AddHours(24)).ToList();

                        var teamStats = new TeamActivityModel();
                        teamStats.ActivityDate = day;
                        teamStats.TotalFBPosts = selSocialActivities.Where(x => x.ActivityTypeId == 3 && x.FeedType == "FP").ToList().Count;
                        teamStats.TotalFBPosts += selSmp.Where(x => x.ProfileComapnyGroup.Profile.ProfileTypeId == (int)ProfilesType.Twitter).ToList().Count;
                        teamStats.TotalFBComments = selSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "FC").ToList().Count;
                        teamStats.TotalFBNestComments = selSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "FCN").ToList().Count;
                        teamStats.TotalFBPageChats = selSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "FCP").ToList().Count;
                        teamStats.TotalTwitTweets = selSocialActivities.Where(x => x.ActivityTypeId == 3 && x.FeedType == "TP").ToList().Count;
                        teamStats.TotalTwitTweets += selSmp.Where(x => x.ProfileComapnyGroup.Profile.ProfileTypeId == (int)ProfilesType.Facebook).ToList().Count;
                        teamStats.TotalTwitReplies = selSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "TP").ToList().Count;
                        teamStats.TotalTwitDMs = selSocialActivities.Where(x => x.ActivityTypeId == 2 && x.FeedType == "TM").ToList().Count;
                        teamStats.TotalDelFBPosts = selSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "FP").ToList().Count;
                        teamStats.TotalDelFBComments = selSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "FC").ToList().Count;
                        teamStats.TotalDelFBNestComments = selSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "FCN").ToList().Count;
                        teamStats.TotalDelTwitTweets = selSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "TP").ToList().Count;
                        teamStats.TotalDelTwitDMs = selSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "TM").ToList().Count;
                        teamStats.TotalDelInstaComments = selSocialActivities.Where(x => x.ActivityTypeId == 1 && x.FeedType == "IC").ToList().Count;
                        teamStats.TotalWarnedUsers = selWarnBannedUsers.Where(x => x.StatusId == 1).ToList().Count;
                        teamStats.TotalBannedUsers = selWarnBannedUsers.Where(x => x.StatusId == 2).ToList().Count;
                        teamStats.TotalTagsAdded = selProfileFeedTags.Count();
                        teamStats.TotalSentimentsAdded = selFeedSentiments.Count();

                        result.TeamStatsEvolution.Add(teamStats);
                    }
                }
                catch (Exception exp)
                {
                    Console.Write(exp);
                    result = null;
                }
            }
            return result;
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private List<ProfileModel> GetProfilesByTeam(int userId, int groupId)
        {
            List<ProfileModel> profiles = new List<ProfileModel>();
            try
            {
                using (SocialCRMEntities db = new SocialCRMEntities())
                {
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

                    //string profileIds = string.Join(",", profiles.Select(y => y.ProfileId).ToArray());

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

                    //string profilePageIds = string.Join(",", profilePages.Select(y => y.ProfileId).ToArray());

                    profiles.AddRange(profilePages);
                }
            }
            catch (Exception ex)
            {
                profiles = new List<ProfileModel>();
            }
            return profiles;
        }

        public List<KeyValuePair<string, int>> GetTwitterProfilePostsAndConversation(FilterModel model)
        {
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            using (var db = new SocialCRMEntities())
            {
                var profiles = db.Profiles.Where(x => x.CompanyId == model.companyId).ToList();
                var tweetSent = db.PostsTwitters.Where(x => x.UserId == x.AuthorUserId && x.CreatedDate >= model.dateFrom && x.CreatedDate <= model.dateTo).ToList();
                var directmessageSent = db.MessagesTwitters.Where(x => x.SenderId == x.UserId && x.CreatedDate >= model.dateFrom && x.CreatedDate <= model.dateTo).ToList();
                var mentionsReceived = db.PostsTwitters.Where(x => x.IsMention == true && x.CreatedDate >= model.dateFrom && x.CreatedDate <= model.dateTo).ToList();
                var directmessageReceived = db.MessagesTwitters.Where(x => x.RecipientId == x.UserId && x.CreatedDate >= model.dateFrom && x.CreatedDate <= model.dateTo).ToList();

                int countTweetSent = 0;
                int countDMSent = 0;
                int countMentionsReceived = 0;
                int countdirectmessageReceived = 0;

                foreach (var profile in profiles)
                {
                    foreach (var item in tweetSent)
                    {
                        if (item.UserId == profile.ProfileExternalId)
                        {
                            countTweetSent++;
                        }
                    }

                    foreach (var item in directmessageSent)
                    {
                        if (item.UserId == profile.ProfileExternalId)
                        {
                            countDMSent++;
                        }
                    }

                    foreach (var item in mentionsReceived)
                    {
                        if (item.UserId == profile.ProfileExternalId)
                        {
                            countMentionsReceived++;
                        }
                    }

                    foreach (var item in directmessageReceived)
                    {
                        if (item.UserId == profile.ProfileExternalId)
                        {
                            countdirectmessageReceived++;
                        }
                    }
                }

                result.Add(new KeyValuePair<string, int>("countTweetSent", countTweetSent));
                result.Add(new KeyValuePair<string, int>("countDMSent", countDMSent));
                result.Add(new KeyValuePair<string, int>("countMentionsReceived", countMentionsReceived));
                result.Add(new KeyValuePair<string, int>("countdirectmessageReceived", countdirectmessageReceived));
            }

            return result;
        }

        public async Task<List<CommentAndChatModel>> GetCommentAndChatByTeam(int teamId, string profileType, string activityType, string profileId, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum)
        {
            List<CommentAndChatModel> result = new List<CommentAndChatModel>();
            try
            {
                using (SocialCRMEntities db = new SocialCRMEntities())
                {
                    switch (dateFilterEnum)
                    {
                        case DateFilter.Last15Days:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last30Days:
                            dateFrom = DateTime.Now.AddDays(-30);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last60Days:
                            dateFrom = DateTime.Now.AddDays(-60);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last90Days:
                            dateFrom = DateTime.Now.AddDays(-90);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Custom:
                            break;
                        default:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            break;
                    }

                    var activities = new List<SocialActivity>();
                    //string feedType = "";
                    List<string> feedType = new List<string>();
                    if (profileType == "FB_PROFILE" || (profileType == "FB_PAGE" && activityType == "CM"))
                    {
                        feedType.Add("FC");
                        feedType.Add("FCN");
                    }
                    else if (profileType == "FB_PAGE" && activityType == "MSG")
                    {
                        feedType.Add("FCP");
                    }
                    else if (profileType == "TW" && activityType == "MSG")
                    {
                        feedType.Add("TM");
                    }
                    else if (profileType == "TW" && activityType == "CM")
                    {
                        feedType.Add("TP");
                    }
                    else if (profileType == "IC" && activityType == "CM")
                    {
                        feedType.Add("IC");
                    }

                    activities = db.SocialActivities.ToList().Where(x => x.TeamId == teamId && x.ActivityTypeId == 2 && feedType.Contains(x.FeedType)
                                                                    && ((dateFrom.HasValue) ? (x.CreatedDate.Value.Date >= dateFrom.Value.Date) : true)
                                                                    && ((dateTo.HasValue) ? (x.CreatedDate.Value.Date <= dateTo.Value.Date) : true)).ToList();
                    #region Reply Distinct

                    if (profileType == "FB_PAGE")
                    {
                        activities = activities.Where(x => x.ProfilePageId == profileId).ToList();
                    }
                    else
                    {
                        activities = activities.Where(x => x.ProfileId == profileId).ToList();
                    }

                    #endregion

                    var profiles = db.Profiles.ToList();
                    var pages = db.ProfilePages.ToList();
                    if (activities != null && activities.Count > 0)
                    {
                        foreach (var activity in activities)
                        {
                            CommentAndChatModel model = new CommentAndChatModel();
                            model.ProfileId = profileType == "FB_PAGE" ? activity.ProfilePageId : activity.ProfileId;
                            model.ProfileName = profileType == "FB_PAGE" ? pages.Where(x => x.ProfilePageID == model.ProfileId).FirstOrDefault().PageName : profiles.Where(x => x.ProfileExternalId == model.ProfileId).FirstOrDefault().Name;
                            model.ProfileType = profileType;
                            model.ProfilePicture = profileType == "FB_PAGE" ? pages.Where(x => x.ProfilePageID == model.ProfileId).FirstOrDefault().ProfilePic : profiles.Where(x => x.ProfileExternalId == model.ProfileId).FirstOrDefault().ProfilePic;
                            model.Text = activity.MsgText;
                            model.Date = DatetimeExtensions.formatMsgDatetime((DateTime)activity.CreatedDate);
                            string postLink = "";
                            if (profileType == "FB_PROFILE" || (profileType == "FB_PAGE" && activityType == "CM"))
                            {
                                postLink = string.Format("https://www.facebook.com/{0}", activity.FeedId);
                            }
                            else if (profileType == "FB_PAGE" && activityType == "MSG")
                            {
                                postLink = string.Format("https://www.facebook.com/{0}/messages", activity.ProfilePageId);
                            }
                            else if (profileType == "TW" && activityType == "MSG")
                            {
                                postLink = string.Format("https://twitter.com/messages");
                            }
                            else if (profileType == "TW" && activityType == "CM")
                            {
                                string twitterScreenName = db.PostsTwitters.Where(x => x.StatusId == activity.FeedId).FirstOrDefault().AuthorName;
                                postLink = string.Format("https://twitter.com/{0}/status/{1}", twitterScreenName, activity.FeedId);
                            }

                            if (activity.FeedType == "FCP")
                            {
                                var conversationId = db.PageChatsFBs.Where(x => x.MsgId == activity.CommentId).FirstOrDefault().ConId;
                                var toUserDetail = db.PageChatsFBs.ToList().Where(x => x.ConId == conversationId && x.FromId != activity.ProfilePageId).FirstOrDefault();
                                if (toUserDetail != null)
                                {
                                    ToUser user = new ToUser();
                                    user.ToProfileId = toUserDetail.FromId;
                                    user.ToProfileName = toUserDetail.FromName;
                                    user.ToProfilePicture = toUserDetail.FromProfilePic;
                                    user.ToProfileType = "FB_PAGE";
                                    user.ToProfileLink = string.Format("https://www.facebook.com/{0}", toUserDetail.FromId);
                                    model.ToUserProfile = user;
                                }
                            }
                            else if (activity.FeedType == "TM")
                            {
                                var toUserDetail = db.MessagesTwitters.ToList().Where(x => x.SenderScreenName == activity.ToUser
                                                                                            && x.RecipientScreenName == activity.FromUser).FirstOrDefault();
                                if (toUserDetail != null)
                                {
                                    ToUser user = new ToUser();
                                    user.ToProfileId = toUserDetail.SenderId;
                                    user.ToProfileName = toUserDetail.SenderScreenName;
                                    user.ToProfilePicture = toUserDetail.SenderProfilePic;
                                    user.ToProfileType = "TW";
                                    user.ToProfileLink = string.Format("https://www.twitter.com/{0}", toUserDetail.SenderScreenName);
                                    model.ToUserProfile = user;
                                }
                            }

                            model.PostLink = postLink;
                            result.Add(model);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                result = new List<CommentAndChatModel>();
            }
            return result;
        }

        public async Task<ReplyModel> GetQuickOrSlowReplyByTeam(int teamId, string profileType, string profileId, string replyType, DateTime? dateFrom, DateTime? dateTo, DateFilter dateFilterEnum)
        {
            ReplyModel result = new ReplyModel();
            try
            {
                using (SocialCRMEntities db = new SocialCRMEntities())
                {
                    switch (dateFilterEnum)
                    {
                        case DateFilter.Last15Days:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last30Days:
                            dateFrom = DateTime.Now.AddDays(-30);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last60Days:
                            dateFrom = DateTime.Now.AddDays(-60);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Last90Days:
                            dateFrom = DateTime.Now.AddDays(-90);
                            dateTo = DateTime.Now;
                            break;
                        case DateFilter.Custom:
                            break;
                        default:
                            dateFrom = DateTime.Now.AddDays(-15);
                            dateTo = DateTime.Now;
                            break;
                    }

                    var activities = new List<SocialActivity>();
                    //string feedType = "";
                    //if (profileType == "FB_PROFILE" || (profileType == "FB_PAGE" && activityType == "CM"))
                    //{
                    //    feedType = "FC";
                    //}
                    //else if (profileType == "FB_PAGE" && activityType == "MSG")
                    //{
                    //    feedType = "FCP";
                    //}
                    //else if (profileType == "TW" && activityType == "MSG")
                    //{
                    //    feedType = "TM";
                    //}
                    //else if (profileType == "TW" && activityType == "CM")
                    //{
                    //    feedType = "TP";
                    //}
                    //else if (profileType == "IC" && activityType == "CM")
                    //{
                    //    feedType = "TP";
                    //}

                    activities = db.SocialActivities.ToList().Where(x => x.TeamId == teamId && x.ActivityTypeId == 2
                                                                    && ((dateFrom.HasValue) ? (x.CreatedDate.Value.Date >= dateFrom.Value.Date) : true)
                                                                    && ((dateTo.HasValue) ? (x.CreatedDate.Value.Date <= dateTo.Value.Date) : true)).ToList();
                    #region Reply Distinct
                    SocialActivity resultant = new SocialActivity();

                    List<TimeModel> idList = new List<TimeModel>();
                    double avgReplyTime = 0;
                    if (profileType == "FB_PROFILE")
                    {
                        var tempActivities = activities.Where(x => x.ProfileId == profileId && x.FeedType == "FC").ToList();
                        var facebookReplies = tempActivities.Where(x => String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] commentIds = tempActivities.Where(x => String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in facebookReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {
                                DateTime postDate = postDetails.CreatedDate.Value;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                comments = comments.Where(x => (x.PostId == postId && x.FromId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //idList.Add(comments[i].CommentId,avgReplyTime);
                                                    idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        var tempNestedActivities = activities.Where(x => x.ProfileId == profileId && x.FeedType == "FCN").ToList();
                        var facebookNestedReplies = tempNestedActivities.Where(x => String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] nestedCommentIds = tempNestedActivities.Where(x => String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in facebookNestedReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.PostId == commentId).FirstOrDefault();
                            DateTime postDate = commentDetails.CreatedDate.Value;
                            bool isOurPost = commentDetails.FromId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                            comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || nestedCommentIds.Contains(x.CommentId)).ToList();
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].FromId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].FromId != comments[i - 1].FromId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //idList.Add(comments[i].CommentId,avgReplyTime);
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((!isPostDateAdded) && i < 1)
                                        {
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].FromId != comments[i - 1].FromId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        activities = new List<SocialActivity>();
                        activities.AddRange(tempActivities);
                        activities.AddRange(tempNestedActivities);

                    }
                    else if (profileType == "FB_PAGE")
                    {
                        var commentActivities = activities.Where(x => x.ProfilePageId == profileId && x.FeedType == "FC").ToList();
                        idList = new List<TimeModel>();
                        var facebookPageReplies = commentActivities.Where(x => !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] commentIds = commentActivities.Where(x => !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in facebookPageReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && !String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {
                                DateTime postDate = postDetails.CreatedDate.Value;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                comments = comments.Where(x => (x.PostId == postId && x.FromId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                        }

                        // nested page replies
                        commentActivities = activities.Where(x => x.ProfilePageId == profileId && x.FeedType == "FCN").ToList();
                        var facebookPageNestedReplies = commentActivities.Where(x => !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] nestedCommentIds = commentActivities.Where(x => !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in facebookPageNestedReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.PostId == commentId).FirstOrDefault();
                            DateTime postDate = commentDetails.CreatedDate.Value;
                            bool isOurPost = commentDetails.FromId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                            comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || nestedCommentIds.Contains(x.CommentId)).ToList();
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].FromId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].FromId != comments[i - 1].FromId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((!isPostDateAdded) && i < 1)
                                        {
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].FromId != comments[i - 1].FromId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        //


                        var pageChateReplies = activities.Where(x => x.FeedType == "FCP").Select(y => y.FeedId).Distinct().ToArray();
                        string[] msgIds = activities.Where(x => x.FeedType == "FCP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in pageChateReplies)
                        {
                            var msgs = db.PageChatsFBs.Where(x => x.ConId == postId).OrderBy(y => y.CreatedDate).ToList();
                            msgs = msgs.Where(x => (x.ConId == postId && x.FromId != profileId) || msgIds.Contains(x.MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FromId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FromId != msgs[i - 1].FromId)
                                        {
                                            avgReplyTime = (msgs[i].CreatedDate.Value - msgs[i - 1].CreatedDate.Value).Seconds;
                                            idList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "MSG", TeamMemberId = teamId, ProfileId = profileId, Id = msgs[i].MsgId });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (profileType == "TW")
                    {
                        activities = activities.Where(x => x.ProfileId == profileId).ToList();
                        idList = new List<TimeModel>();
                        var twitterReplies = activities.Where(x => x.FeedType == "TP").Select(y => y.FeedId).Distinct().ToArray();
                        string[] commentIds = activities.Where(x => x.FeedType == "TP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in twitterReplies)
                        {
                            var postDetails = db.PostsTwitters.Where(x => x.StatusId == postId).FirstOrDefault();
                            DateTime postDate = postDetails.CreatedDate.Value;
                            bool isOurPost = postDetails.AuthorUserId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.PostsTwitters.Where(x => x.InReplyToStatusId == postId).OrderBy(y => y.CreatedDate).ToList();
                            comments = comments.Where(x => (x.InReplyToStatusId == postId && x.AuthorUserId != profileId) || commentIds.Contains(x.StatusId)).ToList();
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].AuthorUserId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].StatusId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!isPostDateAdded)
                                        {
                                            //avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            idList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].StatusId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].StatusId });
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        var twitterMsgReplies = activities.Where(x => x.FeedType == "TM").Select(y => y.FeedId).Distinct().ToArray();
                        string[] msgIds = activities.Where(x => x.FeedType == "TM").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in twitterMsgReplies)
                        {
                            string fromUser = activities.Where(x => x.FeedType == "TM" && x.FeedId == postId).FirstOrDefault().FromUser;
                            string toUser = activities.Where(x => x.FeedType == "TM" && x.FeedId == postId).FirstOrDefault().ToUser;
                            var msgs = db.MessagesTwitters.Where(y => (y.SenderScreenName == fromUser && y.RecipientScreenName == toUser)
                                                               || (y.SenderScreenName == toUser && y.RecipientScreenName == fromUser))
                                                               .GroupBy(a => a.MsgId).OrderBy(z => z.FirstOrDefault().CreatedDate).ToList();
                            msgs = msgs.Where(x => (x.FirstOrDefault().SenderId != profileId) || msgIds.Contains(x.FirstOrDefault().MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FirstOrDefault().SenderId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FirstOrDefault().SenderId != msgs[i - 1].FirstOrDefault().SenderId)
                                        {
                                            avgReplyTime = (msgs[i].FirstOrDefault().CreatedDate.Value - msgs[i - 1].FirstOrDefault().CreatedDate.Value).Seconds;
                                            idList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "MSG", TeamMemberId = teamId, ProfileId = profileId, Id = msgs[i].FirstOrDefault().MsgId });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (profileType == "IC")
                    {
                        activities = activities.Where(x => x.ProfileId == profileId).ToList();
                        idList = new List<TimeModel>();
                        var instaReplies = activities.Where(x => x.FeedType == "IC").Select(y => y.FeedId).Distinct().ToArray();
                        string[] commentIds = activities.Where(x => x.FeedType == "IC").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in instaReplies)
                        {
                            var postDetails = db.InstagramMinnedDatas.Where(x => x.PostId == postId).FirstOrDefault();
                            DateTime postDate = postDetails.CreatedDate.Value;
                            bool isOurPost = db.Profiles.Where(x => x.Id == postDetails.InstagramAccountId).FirstOrDefault().ProfileExternalId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.InstagramComments.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                            comments = comments.Where(x => (x.PostId == postId && x.FromUserId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].FromUserId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].FromUserId != comments[i - 1].FromUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!isPostDateAdded)
                                        {
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            idList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].FromUserId != comments[i - 1].FromUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                idList.Add(new TimeModel { Time = avgReplyTime, Type = "IC", ActivityType = "CM", TeamMemberId = teamId, ProfileId = profileId, Id = comments[i].CommentId });
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }

                    #endregion
                    double time = 0;
                    bool isChat = false;
                    if (replyType == "Q")
                    {
                        var reply = idList.OrderBy(x => x.Time).FirstOrDefault();
                        isChat = reply.ActivityType == "MSG";
                        time = reply.Time;
                        resultant = activities.Where(x => x.CommentId == reply.Id).FirstOrDefault();
                    }
                    else
                    {
                        var reply = idList.OrderByDescending(x => x.Time).FirstOrDefault();
                        time = reply.Time;
                        isChat = reply.ActivityType == "MSG";
                        resultant = activities.Where(x => x.CommentId == reply.Id).FirstOrDefault();
                    }

                    var profiles = db.Profiles.ToList();
                    var pages = db.ProfilePages.ToList();

                    //ReplyModel model = new ReplyModel();
                    result.ProfileId = profileType == "FB_PAGE" ? resultant.ProfilePageId : resultant.ProfileId;
                    result.ProfileName = profileType == "FB_PAGE" ? pages.Where(x => x.ProfilePageID == result.ProfileId).FirstOrDefault().PageName : profiles.Where(x => x.ProfileExternalId == result.ProfileId).FirstOrDefault().Name;
                    result.ProfileType = profileType;
                    result.ProfilePicture = profileType == "FB_PAGE" ? pages.Where(x => x.ProfilePageID == result.ProfileId).FirstOrDefault().ProfilePic : profiles.Where(x => x.ProfileExternalId == result.ProfileId).FirstOrDefault().ProfilePic;
                    result.Text = resultant.MsgText;
                    result.Date = DatetimeExtensions.formatMsgDatetime((DateTime)resultant.CreatedDate);
                    string postLink = "";
                    if (profileType == "FB_PROFILE" || (profileType == "FB_PAGE" && !isChat))
                    {
                        postLink = string.Format("https://www.facebook.com/{0}", resultant.FeedId);
                    }
                    else if (profileType == "FB_PAGE" && isChat)
                    {
                        postLink = string.Format("https://www.facebook.com/{0}/messages", resultant.ProfilePageId);
                    }
                    else if (profileType == "TW" && isChat)
                    {
                        postLink = string.Format("https://twitter.com/messages");
                    }
                    else if (profileType == "TW" && !isChat)
                    {
                        string twitterScreenName = db.PostsTwitters.Where(x => x.StatusId == resultant.FeedId).FirstOrDefault().AuthorName;
                        postLink = string.Format("https://twitter.com/{0}/status/{1}", twitterScreenName, resultant.FeedId);
                    }
                    else if (profileType == "IC")
                    {
                        int id = db.InstagramMinnedDatas.Where(x => x.PostId == resultant.FeedId).FirstOrDefault().InstagramAccountId;
                        string instaId = db.Profiles.Where(x => x.Id == id).FirstOrDefault().ProfileExternalId;
                        postLink = string.Format("https://instagram.com/{0}/status/{1}", instaId, resultant.FeedId);
                    }

                    if (resultant.FeedType == "FCP")
                    {
                        var conversationId = db.PageChatsFBs.Where(x => x.MsgId == resultant.CommentId).FirstOrDefault().ConId;
                        var toUserDetail = db.PageChatsFBs.ToList().Where(x => x.ConId == conversationId && x.FromId != resultant.ProfilePageId).FirstOrDefault();
                        if (toUserDetail != null)
                        {
                            ToUser user = new ToUser();
                            user.ToProfileId = toUserDetail.FromId;
                            user.ToProfileName = toUserDetail.FromName;
                            user.ToProfilePicture = toUserDetail.FromProfilePic;
                            user.ToProfileType = "FB_PAGE";
                            user.ToProfileLink = string.Format("https://www.facebook.com/{0}", toUserDetail.FromId);
                            result.ToUserProfile = user;
                        }

                    }
                    else if (resultant.FeedType == "TM")
                    {
                        var toUserDetail = db.MessagesTwitters.ToList().Where(x => x.SenderScreenName == resultant.ToUser
                                                                                   && x.RecipientScreenName == resultant.FromUser).FirstOrDefault();
                        if (toUserDetail != null)
                        {
                            ToUser user = new ToUser();
                            user.ToProfileId = toUserDetail.SenderId;
                            user.ToProfileName = toUserDetail.SenderScreenName;
                            user.ToProfilePicture = toUserDetail.SenderProfilePic;
                            user.ToProfileType = "TW";
                            user.ToProfileLink = string.Format("https://www.twitter.com/{0}", toUserDetail.SenderScreenName);
                            result.ToUserProfile = user;
                        }
                    }

                    result.PostLink = postLink;
                    result.ResponseTime = string.Format("{0} mins", Math.Round(time / 60, 2));
                }
            }
            catch (Exception ex)
            {
                result = new ReplyModel();
            }
            return result;
        }

        public List<GetInOutboundReport_Result> GetInOutBoundReport(InOutBoundFilterModel model)
        {
            List<GetInOutboundReport_Result> result = new List<GetInOutboundReport_Result>();
            using (var db = new SocialCRMEntities())
            {
                result = db.GetInOutboundReport(model.profileIds, model.profilePageIds, model.msgIds, model.tagIds, model.sentimentIds, model.dateFrom, model.dateTo, model.groupId).ToList();
            }

            return result;
        }
        public List<GetInOutboundReportDrillDown_Result> GetInOutBoundReportDrillDown(InOutBoundFilterModel model)
        {
            List<GetInOutboundReportDrillDown_Result> result = new List<GetInOutboundReportDrillDown_Result>();
            using (var db = new SocialCRMEntities())
            {
                result = db.GetInOutboundReportDrillDown(model.profileIds, model.profilePageIds, model.msgIds, model.tagIds, model.sentimentIds, model.dateFrom, model.dateTo, model.groupId, model.XAxis,model.XAxis2,model.PageNumber,model.PageSize).ToList();
            }

            return result;
        }
        public CrisisReportDataModel CrisisReportData()
        {
            CrisisReportDataModel status = new CrisisReportDataModel();

            using (var db = new SocialCRMEntities())
            {
                status = (from a in db.CrisisReportDatas
                         where a.Report_Status == true
                         select new CrisisReportDataModel()
                         {
                             ReportStatus = a.Report_Status,
                             LastOnTime = a.LastOnTime.ToString(),
                             LastOffTime = a.LastOffTime.ToString()
                         }).FirstOrDefault();
            }

            return status;
        }
        public List<OnDutyAgentModel> GetOnDutyAgents(int companyId, DateTime date, TimeSpan fromShift, TimeSpan toShift)
        {
            List<OnDutyAgentModel> result = new List<OnDutyAgentModel>();

            using (new TransactionScope(
                                        TransactionScopeOption.Required,
                                        new TransactionOptions
                                        {
                                            IsolationLevel = IsolationLevel.ReadUncommitted
                                        }))
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = (from a in db.SocialActivities
                                  join t in db.Teams
                                  on a.TeamId equals t.Id
                                  where (a.ActivityTypeId == 2 || a.ActivityTypeId == 4) &&
                                  t.CompanyId == companyId &&
                                  a.CreatedDate.HasValue &&
                                  System.Data.Entity.DbFunctions.TruncateTime(a.CreatedDate.Value) == date.Date &&
                                  System.Data.Entity.DbFunctions.CreateTime(a.CreatedDate.Value.Hour, a.CreatedDate.Value.Minute, a.CreatedDate.Value.Second) >= fromShift &&
                                  System.Data.Entity.DbFunctions.CreateTime(a.CreatedDate.Value.Hour, a.CreatedDate.Value.Minute, a.CreatedDate.Value.Second) <= toShift
                                  group t by new { t.FirstName, t.LastName, t.Id } into g
                                  select new OnDutyAgentModel()
                                  {
                                      Id = g.Key.Id,
                                      FirstName = g.Key.FirstName,
                                      LastName = g.Key.LastName
                                  }
                                ).ToList();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return result;
        }

        public List<string> GetEmails(int companyId)
        {
            List<string> result = new List<string>();
            using (var db = new SocialCRMEntities())
            {
                result = db.SocialMediaUsersUpdateReceivers.Where(x => x.CompanyId == companyId && x.ReceiveReport == true).Select(x => x.Email).ToList();
            }

            return result;
        }

        public bool UpdateCrisisData(bool isCrisis)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    CrisisReportData cr = new CrisisReportData();
                    if (!isCrisis)
                    {
                        cr.LastOnTime = DateTime.Now;
                        cr.LastOffTime = DateTime.Now.AddHours(12);
                        cr.Report_Status = true;

                        db.CrisisReportDatas.Add(cr);
                    }
                    else
                    {
                        cr = db.CrisisReportDatas.Where(x => x.Report_Status == true).FirstOrDefault();
                        cr.Report_Status = false;
                        cr.LastOffTime = DateTime.Now;
                        db.CrisisReportDatas.Attach(cr);
                        db.Entry(cr).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    result = true;
                }
                catch(Exception ex)
                {
                    result = false;
                }
            }

            return result;
        }

        public void BuildAgentPerformanceReport(string startDate = "")
        {
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    TeamReportModel result = new TeamReportModel();
                    TaskRepository taskRepository = new TaskRepository();
                    var teamMemberIds = db.Teams.Select(y => y.Id).Distinct().ToList();
                    result.ReplyByTeamMember = new List<ProfileAndTeamReportNew>();
                    List<AgentPerformanceReport> reportModel = new List<AgentPerformanceReport>();
                    var teams = db.Teams.ToList();
                    List<TimeModel> totaltimeList = new List<TimeModel>();
                    List<ThreadModel> totalthreadList = new List<ThreadModel>();
                    double avgReplyTime = 0, medianReplyTime = 0, quickestReply = 0, replyThreadSize = 0, slowestReply = 0;
                    int totalReplies = 0;

                    DateTime currentDateTime = DateTime.Now.AddDays(-1);

                    if (!string.IsNullOrWhiteSpace(startDate))
                    {
                        //dd_mm_yyyy
                        var dateFormat = startDate.Replace("_", "/");
                        if (!DateTime.TryParseExact(dateFormat, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out currentDateTime))
                        {
                            return;
                        }
                    }
                    DateTime lastDayMinTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0, 0);
                    DateTime lastDayMaxTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 23, 59, 59, 999);
                    TimeSpan timeFirst = new TimeSpan(8, 0, 0);
                    TimeSpan timeSecond = new TimeSpan(16, 0, 0);
                    TimeSpan timeThird = new TimeSpan(23, 59, 59);
                    TimeSpan timeThirdNextDay = new TimeSpan(00, 00, 00);

                    #region Reply By Team Report
                    foreach (var team in teamMemberIds)
                    {
                        var activities = db.SocialActivities.Where(x => x.TeamId == team && x.ActivityTypeId == 2 && x.CreatedDate >= lastDayMinTime && x.CreatedDate <= lastDayMaxTime).ToList();
                        List<TimeModel> timeList = new List<TimeModel>();
                        List<ThreadModel> threadList = new List<ThreadModel>();
                        //FC Profile Comment
                        var facebookReplies = activities.Where(x => x.FeedType == "FC" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] commentIds = activities.Where(x => x.FeedType == "FC" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in facebookReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {
                                DateTime postDate = postDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FC" && x.FeedId == postId && String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfileId;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments6 = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments6.Count, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                var comments = comments6.Where(x => (x.PostId == postId && x.FromId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //FC Profile Nested Comment
                        avgReplyTime = 0;
                        var facebookNestedReplies = activities.Where(x => x.FeedType == "FCN" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        string[] nestedCommentIds = activities.Where(x => x.FeedType == "FCN" && String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in facebookNestedReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.CommentId == commentId).FirstOrDefault();
                            if (commentDetails != null)
                            {
                                DateTime postDate = commentDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FCN" && x.FeedId == commentId && String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfileId;
                                bool isOurPost = commentDetails.FromId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || nestedCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PROFILE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }                        
                        //FC Page
                        avgReplyTime = 0;
                        var facebookPageReplies = activities.Where(x => x.FeedType == "FC" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        commentIds = activities.Where(x => x.FeedType == "FC" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in facebookPageReplies)
                        {
                            var postDetails = db.PostsFBs.Where(x => x.PostId == postId && !String.IsNullOrEmpty(x.PageId)).FirstOrDefault();
                            if (postDetails != null)
                            {

                                DateTime postDate = postDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FC" && x.FeedId == postId && !String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfilePageId;
                                bool isOurPost = postDetails.AuthorUserId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == postId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == postId && x.FromId != profileId) || commentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //FC Page Nested Replies
                        avgReplyTime = 0;
                        var nestedFacebookPageReplies = activities.Where(x => x.FeedType == "FCN" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.FeedId).Distinct().ToArray();
                        nestedCommentIds = activities.Where(x => x.FeedType == "FCN" && !String.IsNullOrEmpty(x.ProfilePageId)).Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var commentId in nestedFacebookPageReplies)
                        {
                            var commentDetails = db.CommentsFBs.Where(x => x.CommentId == commentId).FirstOrDefault(); ;
                            if (commentDetails != null)
                            {

                                DateTime postDate = commentDetails.CreatedDate.Value;
                                string profileId = activities.Where(x => x.FeedType == "FCN" && x.FeedId == commentId && !String.IsNullOrEmpty(x.ProfilePageId)).FirstOrDefault().ProfilePageId;
                                bool isOurPost = commentDetails.FromId == profileId;
                                bool isPostDateAdded = false;
                                var comments = db.CommentsFBs.Where(x => x.PostId == commentId).OrderBy(y => y.CreatedDate).ToList();
                                threadList.Add(new ThreadModel { Size = comments.Count, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                comments = comments.Where(x => (x.PostId == commentId && x.FromId != profileId) || nestedCommentIds.Contains(x.CommentId)).ToList();
                                for (int i = 0; i < comments.Count; i++)
                                {
                                    if (comments[i].FromId == profileId)
                                    {
                                        if (isOurPost)
                                        {
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((!isPostDateAdded) && i < 1)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                isPostDateAdded = true;
                                            }
                                            if (i > 0)
                                            {
                                                if (comments[i].FromId != comments[i - 1].FromId)
                                                {
                                                    avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                    //timeList.Add(avgReplyTime);
                                                    timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "CMN", TeamMemberId = team, ProfileId = profileId });
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //FB Page Chat
                        avgReplyTime = 0;
                        var pageChatReplies = activities.Where(x => x.FeedType == "FCP").Select(y => y.FeedId).Distinct().ToArray();
                        string[] msgIds = activities.Where(x => x.FeedType == "FCP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in pageChatReplies)
                        {
                            string profileId = activities.Where(x => x.FeedType == "FCP" && x.FeedId == postId).FirstOrDefault().ProfilePageId;
                            var msgs = db.PageChatsFBs.Where(x => x.ConId == postId).OrderBy(y => y.CreatedDate).ToList();
                            //threadList.Add(msgs.Count);
                            threadList.Add(new ThreadModel { Size = msgs.Count, Type = "FB_PAGE", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                            msgs = msgs.Where(x => (x.ConId == postId && x.FromId != profileId) || msgIds.Contains(x.MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FromId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FromId != msgs[i - 1].FromId)
                                        {
                                            avgReplyTime = (msgs[i].CreatedDate.Value - msgs[i - 1].CreatedDate.Value).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            timeList.Add(new TimeModel { Time = avgReplyTime, Type = "FB_PAGE", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                                        }
                                    }
                                }
                            }
                        }
                        //TM
                        avgReplyTime = 0;
                        var twitterMsgReplies = activities.Where(x => x.FeedType == "TM")
                            .Select(y => new TwitterConversationModel { fromUser = y.FromUser, toUser = y.ToUser }).Distinct().ToArray();
                        msgIds = activities.Where(x => x.FeedType == "TM").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var con in twitterMsgReplies)
                        {
                            string profileId = activities.Where(x => x.FeedType == "TM" && x.FromUser == con.fromUser && x.ToUser == con.toUser).FirstOrDefault().ProfileId;
                            var msgs = db.MessagesTwitters.Where(y => (y.SenderScreenName == con.fromUser && y.RecipientScreenName == con.toUser)
                                                               || (y.SenderScreenName == con.toUser && y.RecipientScreenName == con.fromUser))
                                                               .GroupBy(a => a.MsgId).OrderBy(z => z.FirstOrDefault().CreatedDate).ToList();
                            //threadList.Add(msgs.Count);
                            threadList.Add(new ThreadModel { Size = msgs.Count, Type = "TW", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                            msgs = msgs.Where(x => (x.FirstOrDefault().SenderId != profileId) || msgIds.Contains(x.FirstOrDefault().MsgId)).ToList();
                            for (int i = 0; i < msgs.Count; i++)
                            {
                                if (msgs[i].FirstOrDefault().SenderId == profileId)
                                {
                                    if (i > 0)
                                    {
                                        if (msgs[i].FirstOrDefault().SenderId != msgs[i - 1].FirstOrDefault().SenderId)
                                        {
                                            avgReplyTime = (msgs[i].FirstOrDefault().CreatedDate.Value - msgs[i - 1].FirstOrDefault().CreatedDate.Value).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "MSG", TeamMemberId = team, ProfileId = profileId });
                                        }
                                    }
                                }
                            }
                        }
                        //TP
                        avgReplyTime = 0;
                        var twitterReplies = activities.Where(x => x.FeedType == "TP").Select(y => y.FeedId).Distinct().ToArray();
                        commentIds = activities.Where(x => x.FeedType == "TP").Select(y => y.CommentId).Distinct().ToArray();
                        foreach (var postId in twitterReplies)
                        {
                            var postDetails = db.PostsTwitters.Where(x => x.StatusId == postId).FirstOrDefault();
                            DateTime postDate = postDetails.CreatedDate.Value;
                            string profileId = activities.Where(x => x.FeedType == "TP" && x.FeedId == postId).FirstOrDefault().ProfileId;
                            bool isOurPost = postDetails.AuthorUserId == profileId;
                            bool isPostDateAdded = false;
                            var comments = db.PostsTwitters.Where(x => x.InReplyToStatusId == postId).OrderBy(y => y.CreatedDate).ToList();
                            comments = comments.Where(x => (x.InReplyToStatusId == postId && x.AuthorUserId != profileId) || commentIds.Contains(x.StatusId)).ToList();
                            threadList.Add(new ThreadModel { Size = comments.Count, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                            for (int i = 0; i < comments.Count; i++)
                            {
                                if (comments[i].AuthorUserId == profileId)
                                {
                                    if (isOurPost)
                                    {
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!isPostDateAdded)
                                        {
                                            avgReplyTime = (comments[i].CreatedDate.Value - postDate).Seconds;
                                            //timeList.Add(avgReplyTime);
                                            timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            isPostDateAdded = true;
                                        }
                                        if (i > 0)
                                        {
                                            if (comments[i].AuthorUserId != comments[i - 1].AuthorUserId)
                                            {
                                                avgReplyTime = (comments[i].CreatedDate.Value - comments[i - 1].CreatedDate.Value).Seconds;
                                                //timeList.Add(avgReplyTime);
                                                timeList.Add(new TimeModel { Time = avgReplyTime, Type = "TW", ActivityType = "CM", TeamMemberId = team, ProfileId = profileId });
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        totaltimeList.AddRange(timeList);
                        totalthreadList.AddRange(threadList);
                        
                        if (timeList.Count > 0)
                        {
                            var allTimeList = timeList.Select(x => x.Time).ToList<double>();
                            avgReplyTime = allTimeList.Sum() / allTimeList.Count;
                            medianReplyTime = MathExtentions.GetMedian(allTimeList);
                            quickestReply = allTimeList.Min();
                            slowestReply = allTimeList.Max();
                        }
                        if (threadList.Count > 0)
                            replyThreadSize = threadList.Select(x => x.Size).Sum() / threadList.Count;

                        totalReplies = timeList.Count;
                        
                        ProfileAndTeamReportNew report = new ProfileAndTeamReportNew();
                        report.ProfileId = team;
                        report.ProfileName = teams.Where(x => x.Id == team).FirstOrDefault().FirstName + " " + teams.Where(x => x.Id == team).FirstOrDefault().LastName;
                        //report.AvgReplyTime = string.Format("{0} hrs", Math.Round(avgReplyTime / 3600, 2));
                        //report.MedianReplyTime = string.Format("{0} hrs", Math.Round(medianReplyTime / 3600, 2));
                        //report.QuickestReply = string.Format("{0} mins", Math.Round(quickestReply / 60, 2));
                        report.ReplyThreadSize = replyThreadSize;
                        // report.SlowestReply = string.Format("{0} mins", Math.Round(slowestReply / 60, 2));
                        report.TotalReplies = totalReplies;

                        var lastdayReport = db.AgentPerformanceReports.Where(x => x.ProfileId == team).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        string percent = "Infinity";
                        if (lastdayReport != null)
                        {
                            percent = lastdayReport.TotalReplies > 0 ? Convert.ToString(Math.Round(((Convert.ToDouble(totalReplies) - lastdayReport.TotalReplies) / lastdayReport.TotalReplies) * 100, 0)) : "0";
                        }
                        report.ChangeInResponse = percent == "Infinity" ? 0 : Convert.ToDouble(percent);
                        result.ReplyByTeamMember.Add(report);

                        var smptot = db.MessagePostingTimes.Where(x => x.IsPublished
                                                              && x.ScheduledMessageProfile.TeamId == team
                                                              && x.PublishDate >= lastDayMinTime
                                                              && x.PublishDate <= lastDayMaxTime
                                                              ).Select(x => x.ScheduledMessageProfile).ToList();
                        var FirstShiftOutbound = activities.Where(x => (x.ActivityTypeId == 3 || x.ActivityTypeId == 2)&& x.CreatedDate.Value.TimeOfDay >= timeFirst && x.CreatedDate.Value.TimeOfDay < timeSecond).ToList().Count;
                        FirstShiftOutbound += smptot.Where(x => x.MessagePostingTimes.Any(y => y.PublishTime >= timeFirst && y.PublishTime < timeSecond)).ToList().Count;
                        var SecondShiftOutbound = activities.Where(x => (x.ActivityTypeId == 3 || x.ActivityTypeId == 2) && x.CreatedDate.Value.TimeOfDay >= timeSecond && x.CreatedDate.Value.TimeOfDay < timeThird).ToList().Count;
                        SecondShiftOutbound += smptot.Where(x => x.MessagePostingTimes.Any(y => y.PublishTime >= timeSecond && y.PublishTime < timeThird)).ToList().Count;
                        var ThirdShiftOutbound = activities.Where(x => (x.ActivityTypeId == 3 || x.ActivityTypeId == 2) && x.CreatedDate.Value.TimeOfDay >= timeThirdNextDay && x.CreatedDate.Value.TimeOfDay < timeFirst).ToList().Count;
                        ThirdShiftOutbound += smptot.Where(x => x.MessagePostingTimes.Any(y => y.PublishTime >= timeThirdNextDay && y.PublishTime < timeFirst)).ToList().Count;


                        reportModel.Add(new AgentPerformanceReport
                        {
                            AvgReplyTime= Convert.ToInt64(avgReplyTime),
                            ChangeInResponse= percent == "Infinity" ? 0 : Convert.ToInt64(percent),
                            CreatedDate= lastDayMinTime,
                            MedianReplyTime = Convert.ToInt64(medianReplyTime),
                            ProfileId= team,
                            ProfileName = report.ProfileName,
                            ProfilePic= "",
                            ProfileType="",
                            QuickestReply= Convert.ToInt64(quickestReply),
                            ReplyThreadSize= Convert.ToInt64(report.ReplyThreadSize),
                            SlowestReply = Convert.ToInt64(slowestReply),
                            TotalPosts= activities.Count(),
                            TotalReplies= timeList.Count,
                            ChatCount = activities.Where(x => x.ActivityTypeId == 2 &&(x.FeedType == "FCP" || x.FeedType == "TM")).Count(),
                            CommentCount = activities.Where(x => x.ActivityTypeId == 2 &&( x.FeedType == "FC" || x.FeedType == "FCN" || x.FeedType == "TP" || x.FeedType == "IC")).Count(),
                            FirstShiftOutbound = FirstShiftOutbound,
                            SecondShiftOutbound = SecondShiftOutbound,
                            ThirdShiftOutbound = ThirdShiftOutbound
                        });
                        avgReplyTime = 0;
                        medianReplyTime = 0;
                        quickestReply = 0;
                        replyThreadSize = 0;
                        slowestReply = 0;
                        totalReplies = 0;
                    }
                    #endregion

                    #region Dumping data in database by job
                    db.AgentPerformanceReports.AddRange(reportModel);
                    db.SaveChanges();
                    #endregion
                }
                catch (Exception e)
                {
                    
                }
            }
        }
    }
}