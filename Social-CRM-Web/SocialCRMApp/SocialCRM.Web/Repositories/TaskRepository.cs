using CRM.Common.Enum;
using CRM.Entities;
using SocialCRM.Web.Models.TaskManager;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CRM.Common.Extensions;
using SocialCRM.Web.Models.Dashboard;
using System.Data.Entity;

namespace SocialCRM.Web.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        public void AddUserNotifications(int groupId, int userId, string notifText, string notifType, List<int> allMembersInChain)
        {
            using (var db = new SocialCRMEntities())
            {
                var userNotifs = db.UserNotificationSettings.Where(x => x.GroupId == groupId && (x.IsNotificationEnable == true || x.IsEmailEnable == true)).ToList();
                foreach (UserNotificationSetting notif in userNotifs)
                {
                    Team teamMember = db.Teams.Where(x => x.UserId == notif.UserId).FirstOrDefault();

                    if (allMembersInChain.Contains(teamMember.Id) || teamMember.Premissions.Any(x => x.Id == 5))
                    {
                        bool isRead, isEmailSent;
                        if (notif.IsNotificationEnable)
                            isRead = false;
                        else
                            isRead = true;

                        if (notif.IsEmailEnable)
                            isEmailSent = false;
                        else
                            isEmailSent = true;

                        var newNotif = new UserNotification
                        {
                            RecipientUserId = notif.UserId,
                            GroupId = groupId,
                            PerformedByUserId = userId,
                            EntryDate = DateTime.Now,
                            NotificationText = notifText,
                            NotificationType = notifType,
                            IsRead = isRead,
                            IsEmailSent = isEmailSent,
                            IsNotificationEnabled = notif.IsNotificationEnable
                        };
                        db.UserNotifications.Add(newNotif);
                    }
                }

                db.SaveChanges();
            }
        }

        public async Task<bool> CreateTask(TaskModel model, int userId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var createdTask = db.TaskManagers.FirstOrDefault(t => t.ActivityId == model.ActivityId);
                    var teamMemberId = db.GetTeamMemberIdUsingGroupId(model.GroupId, userId).FirstOrDefault();
                    if (!teamMemberId.HasValue)
                        teamMemberId = 0;
                    var isProfilePage = false;
                    var profileId = 0;
                    string profileExternalId = "";
                    var group = db.Groups.Where(g => g.Id == model.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    var taskNotificationText = "";
                    var taskNotificaitonType = "";
                    var allMembersInChain = new List<int>();
                    switch (model.ActivityTable.ToUpper())
                    {
                        case "TP":
                            var data = (from postTwitter in db.PostsTwitters
                                        join userProfiles in db.Profiles
                                        on postTwitter.UserId equals userProfiles.ProfileExternalId
                                        where postTwitter.Id == model.ActivityId
                                        select userProfiles).FirstOrDefault();
                            if (data != null)
                            {
                                profileId = data.Id;
                                profileExternalId = data.ProfileExternalId;
                            }

                            break;
                        case "TM":
                            data = (from postTwitter in db.MessagesTwitters
                                    join userProfiles in db.Profiles
                                    on postTwitter.UserId equals userProfiles.ProfileExternalId
                                    where postTwitter.Id == model.ActivityId
                                    select userProfiles).FirstOrDefault();
                            if (data != null)
                            {
                                profileId = data.Id;
                                profileExternalId = data.ProfileExternalId;
                            }
                            break;
                        case "FP":
                            var post = db.PostsFBs.Where(f => f.Id == model.ActivityId).FirstOrDefault();
                            if (post != null)
                            {
                                if (string.IsNullOrEmpty(post.PageId))
                                {
                                    var profile = db.Profiles.Where(p => p.ProfileExternalId == post.UserId).FirstOrDefault();
                                    if (profile != null)
                                    {
                                        profileId = profile.Id;
                                        profileExternalId = profile.ProfileExternalId;
                                    }
                                }
                                else
                                {
                                    isProfilePage = true;
                                    var profile = db.ProfilePages.Where(p => p.ProfilePageID == post.PageId).FirstOrDefault();
                                    if (profile != null)
                                    {
                                        profileId = profile.Id;
                                        profileExternalId = profile.ProfileExternalId;
                                    }

                                }
                            }
                            break;
                        case "FC":
                            var commentPost = (from commentFb in db.CommentsFBs
                                               join postfb in db.PostsFBs
                                                   on commentFb.PostId equals postfb.PostId
                                               where commentFb.Id == model.ActivityId
                                               select new { postfb.PageId, postfb.UserId }).FirstOrDefault();
                            if (commentPost != null)
                            {
                                if (string.IsNullOrEmpty(commentPost.PageId))
                                {
                                    var profile = db.Profiles.Where(p => p.ProfileExternalId == commentPost.UserId).FirstOrDefault();
                                    if (profile != null)
                                    {
                                        profileId = profile.Id;
                                        profileExternalId = profile.ProfileExternalId;
                                    }
                                }
                                else
                                {
                                    isProfilePage = true;
                                    var profile = db.ProfilePages.Where(p => p.ProfilePageID == commentPost.PageId).FirstOrDefault();
                                    if (profile != null)
                                    {
                                        profileId = profile.Id;
                                        profileExternalId = profile.ProfileExternalId;
                                    }
                                }
                            }
                            break;
                        case "FCP":
                            var chatData = (from chatFB in db.PageChatsFBs
                                            join userProfiles in db.ProfilePages
                                            on chatFB.PageId equals userProfiles.ProfilePageID
                                            where chatFB.Id == model.ActivityId
                                            select userProfiles).FirstOrDefault();

                            if (chatData != null)
                            {
                                profileId = chatData.Id;
                                profileExternalId = chatData.ProfileExternalId;
                            }
                            break;
                        case "IM":
                            var InstaPost = (from postInsta in db.InstagramMinnedDatas
                                             join userProfiles in db.Profiles
                                             on postInsta.InstagramAccountId equals userProfiles.Id
                                             where postInsta.Id == model.ActivityId
                                             select userProfiles).FirstOrDefault();
                            if (InstaPost != null)
                            {
                                profileId = InstaPost.Id;
                                profileExternalId = InstaPost.ProfileExternalId;
                            }
                            break;

                        case "IC":
                            var instaCommentPost = (from commentInsta in db.InstagramComments
                                                    join postInsta in db.InstagramMinnedDatas
                                                        on commentInsta.PostId equals postInsta.PostId
                                                    where commentInsta.Id == model.ActivityId
                                                    select new { postInsta.InstagramAccountId }).FirstOrDefault();
                            if (instaCommentPost != null)
                            {
                                var profile = db.Profiles.Where(p => p.Id == instaCommentPost.InstagramAccountId).FirstOrDefault();
                                if (profile != null)
                                {
                                    profileId = profile.Id;
                                    profileExternalId = profile.ProfileExternalId;
                                }
                            }
                            break;
                    }
                    if (createdTask == null)
                    {
                        allMembersInChain.AddRange(model.Assignee);
                        if (!allMembersInChain.Contains(teamMemberId.Value))
                            allMembersInChain.Add(teamMemberId.Value);

                        var logs = new List<TaskLog>();
                        foreach (var member in model.Assignee)
                        {
                            logs.Add(new TaskLog
                            {
                                ActivityDate = DateTime.Now,
                                ActivityPerformBy = teamMemberId.Value,
                                AssigneeMemberId = member,
                                LogTypeId = (int)LogType.Assigned,
                                Text = "Task Assigned"
                            });
                        }
                        var task = db.TaskManagers.Add(new TaskManager
                        {
                            ActivityId = model.ActivityId,
                            ClassificationId = (int)model.ClassificaitonOfTask,
                            ActivityTable = model.ActivityTable,
                            CreatedDate = DateTime.Now,
                            PriorityTypeId = (int)model.TaskPriority,
                            StatusId = (int)model.Status,
                            TaskLogs = logs,
                            TaskMessage = model.TaskMessage,
                            CompanyId = companyId,
                            CreatedBy = teamMemberId.Value,
                            ProfileId = profileExternalId
                        });
                        //Task assignee's
                        foreach (var id in model.Assignee)
                        {
                            db.TaskAssignees.Add(new TaskAssignee
                            {
                                TaskManager = task,
                                ProfileId = profileId,
                                IsProfilePage = isProfilePage,
                                TeamMemberId = id
                            });
                        }
                        taskNotificationText = model.TaskMessage;
                        taskNotificaitonType = companyName + " - " + groupName + " - " + "Task Created";
                    }
                    else if (createdTask.StatusId == 0)//becayse use just add comment only agaist this task
                    {
                        var logs = new List<TaskLog>();
                        foreach (var member in model.Assignee)
                        {
                            logs.Add(new TaskLog
                            {
                                ActivityDate = DateTime.Now,
                                ActivityPerformBy = teamMemberId.Value,
                                AssigneeMemberId = member,
                                LogTypeId = (int)LogType.Assigned,
                                Text = "Task Assigned"
                            });
                        }
                        var task = db.TaskManagers.Add(new TaskManager
                        {
                            ActivityId = model.ActivityId,
                            ClassificationId = (int)model.ClassificaitonOfTask,
                            ActivityTable = model.ActivityTable,
                            CreatedDate = DateTime.Now,
                            PriorityTypeId = (int)model.TaskPriority,
                            StatusId = (int)model.Status,
                            TaskLogs = logs,
                            TaskMessage = model.TaskMessage,
                            CompanyId = companyId,
                            ProfileId = profileExternalId
                        });
                        //Task assignee's
                        foreach (var id in model.Assignee)
                        {
                            db.TaskAssignees.Add(new TaskAssignee
                            {
                                TaskManager = task,
                                ProfileId = profileId,
                                IsProfilePage = isProfilePage,
                                TeamMemberId = id
                            });
                        }
                        taskNotificationText = model.TaskMessage;
                        taskNotificaitonType = companyName + " - " + groupName + " - " + "Commented on Task";
                    }
                    else //if (createdTask.StatusId == (int)CRM.Common.Enum.TaskStatus.Close && model.Status == CRM.Common.Enum.TaskStatus.InProgress)
                    {
                        var logs = new List<TaskLog>();
                        var assigedMember = createdTask.TaskAssignees.Select(t => t.TeamMemberId).ToList();
                        var newMembers = model.Assignee.Except(assigedMember).ToList();//.Where(a => assigedMember.Any(t => t != a));

                        IEnumerable<int> loggedMember = createdTask.TaskLogs.Where(p => p.LogTypeId == 1).Select(t => t.AssigneeMemberId.Value);
                        allMembersInChain.AddRange(loggedMember);
                        allMembersInChain.AddRange(model.Assignee.Except(assigedMember));
                        if (!allMembersInChain.Contains(createdTask.CreatedBy))
                            allMembersInChain.Add(createdTask.CreatedBy);
                        foreach (var member in newMembers)
                        {
                            logs.Add(new TaskLog
                            {
                                ActivityDate = DateTime.Now,
                                ActivityPerformBy = teamMemberId.Value,
                                AssigneeMemberId = member,
                                LogTypeId = (int)LogType.Assigned,
                                Text = "Task Assigned",
                                TaskManager = createdTask
                            });
                        }
                        if (logs.Count > 0)
                            db.TaskLogs.AddRange(logs);

                        createdTask.ClassificationId = (int)model.ClassificaitonOfTask;
                        createdTask.PriorityTypeId = (int)model.TaskPriority;
                        if (createdTask.StatusId == 3)
                        {
                            createdTask.CreatedBy = teamMemberId.Value;
                        }
                        createdTask.StatusId = (int)model.Status;
                        if (model.Status == CRM.Common.Enum.TaskStatus.Close)
                        {
                            createdTask.UpdatedDate = DateTime.Now;
                        }

                        //Check if user remove Assignee
                        // if (assigedMember.Count > model.Assignee.Count)
                        //   {
                        //Taceout the Deleted member.
                        var deltedMember = assigedMember.Except(model.Assignee).ToList();
                        var deletedAssignees = db.TaskAssignees.Where(p => p.ProfileId == profileId && p.IsProfilePage == isProfilePage && p.TaskManagerId == createdTask.Id && deltedMember.Contains(p.TeamMemberId)).ToList();
                        db.TaskAssignees.RemoveRange(deletedAssignees);
                        //  }
                        //Task assignee's
                        foreach (var id in newMembers)
                        {
                            db.TaskAssignees.Add(new TaskAssignee
                            {
                                TaskManager = createdTask,
                                ProfileId = profileId,
                                IsProfilePage = isProfilePage,
                                TeamMemberId = id
                            });
                        }
                        taskNotificationText = model.TaskMessage;
                        taskNotificaitonType = companyName + " - " + groupName + " - " + String.Format("Task #{0} Updated", createdTask.Id);

                    }
                    /*else if (createdTask.StatusId == (int)CRM.Common.Enum.TaskStatus.Draft)
                    {
                        // await DeleteById(createdTask.Id);
                        var logs = new List<TaskLog>();
                        var assigedMember = createdTask.TaskAssignees.Select(t => t.TeamMemberId).ToList();
                        var newMembers = model.Assignee.Where(a => assigedMember.Any(t => t != a));
                        foreach (var member in newMembers)
                        {
                            logs.Add(new TaskLog
                            {
                                ActivityDate = DateTime.Now,
                                ActivityPerformBy = teamMemberId.Value,
                                AssigneeMemberId = member,
                                LogTypeId = (int)LogType.Assigned,
                                Text = "Task Assigned"
                            });
                        }
                        createdTask.ClassificationId = (int)model.ClassificaitonOfTask;
                        createdTask.PriorityTypeId = (int)model.TaskPriority;
                        createdTask.StatusId = (int)model.Status;

                        //Task assignee's
                        foreach (var id in newMembers)
                        {
                            db.TaskAssignees.Add(new TaskAssignee
                            {
                                TaskManager = createdTask,
                                ProfileId = profileId,
                                IsProfilePage = isProfilePage,
                                TeamMemberId = id
                            });
                        }
                    }
                    else if (createdTask.StatusId == (int)CRM.Common.Enum.TaskStatus.InProgress)
                    {
                       var logs = new List<TaskLog>();
                       var assigedMember = createdTask.TaskAssignees.Select(t => t.TeamMemberId).ToList();
                        var newMembers=model.Assignee.Where(a => assigedMember.Any(t=>t!=a));
                       foreach (var member in newMembers)
                        {
                            logs.Add(new TaskLog
                            {
                                ActivityDate = DateTime.Now,
                                ActivityPerformBy = teamMemberId.Value,
                                AssigneeMemberId = member,
                                LogTypeId = (int)LogType.Assigned,
                                Text = "Task Assigned"
                            });
                        }
                        createdTask.ClassificationId = (int)model.ClassificaitonOfTask;
                        createdTask.PriorityTypeId = (int)model.TaskPriority;
                        createdTask.StatusId = (int)model.Status;

                        //Task assignee's
                        foreach (var id in newMembers)
                        {
                            db.TaskAssignees.Add(new TaskAssignee
                            {
                                TaskManager = createdTask,
                                ProfileId = profileId,
                                IsProfilePage = isProfilePage,
                                TeamMemberId = id
                            });
                        }
                    }*/
                    //Adding Notification.
                    AddUserNotifications(model.GroupId, userId, taskNotificationText, taskNotificaitonType, allMembersInChain);
                    //db.AddUserNotification(model.GroupId, userId, taskNotificationText, taskNotificaitonType);
                    var rowcount = db.SaveChanges();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> DeleteById(int activityId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var task = db.TaskManagers.Where(t => t.ActivityId == activityId).FirstOrDefault();
                    if (task != null)
                    {
                        var taskLog = db.TaskLogs.Where(l => l.TaskId == task.Id);
                        var taskComments = db.TaskComments.Where(c => c.TaskManagerId == task.Id);
                        var taskAssignee = db.TaskAssignees.Where(a => a.TaskManagerId == task.Id);
                        db.TaskAssignees.RemoveRange(taskAssignee);
                        db.TaskComments.RemoveRange(taskComments);
                        db.TaskLogs.RemoveRange(taskLog);
                        db.TaskManagers.Remove(task);
                        var rowcount = await db.SaveChangesAsync();
                        result = rowcount > 0;
                    }
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> AddComment(TaskCommentModel model, int userId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    //First Check Task Exist against ActivityId.if task not exist create new task. and add log & comment.
                    var task = db.TaskManagers.FirstOrDefault(t => t.ActivityId == model.ActivityId);
                    var group = db.Groups.Where(g => g.Id == model.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    // var group=db.Groups.Where(g=>g.Id==model.GroupId).FirstOrDefault();
                    // var teamMemberId = 0;
                    //if(group!=null)
                    //{
                    //    var teamMember = group.Company.Teams.Where(t=>t.UserId==userId).FirstOrDefault();
                    //    teamMemberId = teamMember.Id;
                    //}

                    var teamMemberId = 0;
                    var memberId = db.GetTeamMemberIdUsingGroupId(model.GroupId, userId).FirstOrDefault();
                    if (memberId.HasValue)
                        teamMemberId = memberId.Value;
                    if (task == null)
                    {
                        var taskManager = new TaskManager()
                        {
                            ActivityId = model.ActivityId,
                            ActivityTable = string.Empty,
                            CreatedDate = DateTime.Now
                        };
                        var commentLog = new TaskLog
                        {
                            ActivityDate = DateTime.Now,
                            ActivityPerformBy = teamMemberId,
                            Text = "Comment Added",
                            LogTypeId = (int)LogType.Comment,
                            TaskManager = taskManager
                        };
                        var userComment = new TaskComment
                        {
                            Description = model.CommentDescription,
                            IsActive = true,
                            PerformedDate = DateTime.Now,
                            Title = model.CommentTitle,
                            PerformBy = teamMemberId,
                            TaskManager = taskManager
                        };
                        db.TaskManagers.Add(taskManager);
                        db.TaskComments.Add(userComment);
                        db.TaskLogs.Add(commentLog);
                    }
                    else
                    {
                        var commentLog = new TaskLog
                        {
                            ActivityDate = DateTime.Now,
                            ActivityPerformBy = teamMemberId,
                            Text = model.CommentDescription,
                            LogTypeId = (int)LogType.Comment,
                            TaskManager = task
                        };
                        var userComment = new TaskComment
                        {
                            Description = model.CommentDescription,
                            IsActive = true,
                            PerformedDate = DateTime.Now,
                            Title = model.CommentTitle,
                            PerformBy = teamMemberId,
                            TaskManager = task
                        };
                        db.TaskComments.Add(userComment);
                        db.TaskLogs.Add(commentLog);
                    }

                    //Add User Notification
                    var allMembersInChain = new List<int>();
                    IEnumerable<int> loggedMember = task.TaskLogs.Where(p => p.LogTypeId == 1).Select(t => t.AssigneeMemberId.Value);
                    allMembersInChain.AddRange(loggedMember);
                    if (!allMembersInChain.Contains(task.CreatedBy))
                        allMembersInChain.Add(task.CreatedBy);
                    var notifType = companyName + " - " + groupName + " - " + string.Format("Comments Added For Task #{0}", task.Id);
                    AddUserNotifications(model.GroupId, userId, model.CommentTitle, notifType, allMembersInChain);
                    //db.AddUserNotification(model.GroupId, userId, model.CommentTitle, "Comments Added");
                    var rowcount = db.SaveChanges();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }

        public async Task<List<TaskCommentModel>> GetCommentsByTaskId(int taskId)
        {
            List<TaskCommentModel> result = null;
            await Task.Run(() =>
             {
                 using (var db = new SocialCRMEntities())
                 {
                     try
                     {
                         var taskManger = db.TaskManagers.Where(t => t.ActivityId == taskId).FirstOrDefault();
                         if (taskManger != null)
                         {
                             result = db.TaskComments.Where(c => c.TaskManagerId == taskManger.Id).Select(s => new TaskCommentModel
                             {
                                 Id = s.Id,
                                 TaskId = s.TaskManagerId,
                                 ActivityId = taskManger.ActivityId,
                                 CommentTitle = s.Title,
                                 CommentDescription = s.Description
                             }).ToList();
                         }
                     }
                     catch (Exception exp)
                     {
                         result = null;
                     }
                     db.Dispose();
                 }
             });
            return result;
        }
        public async Task<List<TaskLogModel>> GetLogByTaskId(int activityId)
        {
            List<TaskLogModel> result = new List<TaskLogModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        var taskManger = db.TaskManagers.Where(t => t.ActivityId == activityId).FirstOrDefault();
                        if (taskManger != null)
                        {
                            var taskLog = db.TaskLogs.Where(l => l.TaskId == taskManger.Id).OrderByDescending(o => o.ActivityDate);
                            foreach (var log in taskLog)
                            {
                                var activityPerformBy = db.Teams.Where(t => t.Id == log.ActivityPerformBy).Select(m => string.Concat(m.FirstName, " ", m.LastName)).FirstOrDefault();
                                var assigneeTo = log.AssigneeMemberId.HasValue ? db.Teams.Where(t => t.Id == log.AssigneeMemberId).Select(m => string.Concat(m.FirstName, " ", m.LastName)).FirstOrDefault() : string.Empty;
                                result.Add(
                                     new TaskLogModel
                                     {
                                         LogId = log.Id,
                                         CreatedDate = log.ActivityDate.ToString("MMM dd, yyyy | hh:mm tt"),
                                         ActivityPerformBy = string.IsNullOrEmpty(activityPerformBy) ? string.Empty : activityPerformBy,
                                         AssigneeTo = assigneeTo,
                                         LogTypeId = log.LogTypeId,
                                         Text = string.IsNullOrEmpty(log.Text) ? string.Empty : log.Text,
                                         TaskId = log.TaskId,
                                         ActivityLogType = (LogType)log.LogTypeId
                                     });
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        result = null;
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<TaskViewModel>> GetUserTaskByType(int taskTypeId, int groupId, int userId)
        {
            List<TaskViewModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.GetUserTaskTemp(taskTypeId, userId, groupId)
                                    .GroupBy(t => t.TaskId).Select(r => new TaskViewModel
                                    {
                                        AssignedTo = r.First().AssignedTo,
                                        LastUpdated = r.First().LastUpdated,
                                        ProfileName = r.First().ProfileName,
                                        ProfileUrl = r.First().ProfileUrl,
                                        TaskClassification = (Classificaiton)r.First().TaskClassification,
                                        TaskId = r.First().TaskId.Value,
                                        TaskMessage = r.First().TaskMessage,
                                        TaskPriority = (Priority)r.First().TaskPriority,
                                        Status = (CRM.Common.Enum.TaskStatus)r.First().TaskStatusId,
                                        IsRead = r.First().IsRead.HasValue ? r.First().IsRead.Value : false
                                    }).ToList();
                    }
                    catch (Exception exp)
                    {
                        result = null;
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<TaskCommentModel>> GetCommentByActivityId(int activityId)
        {
            List<TaskCommentModel> result = new List<TaskCommentModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        var taskManger = db.TaskManagers.Where(t => t.ActivityId == activityId).FirstOrDefault();
                        if (taskManger != null)
                        {
                            result = db.GetTaskCommentsByActivityId(taskManger.Id)
                                .Select(c => new TaskCommentModel
                                {
                                    Id = c.Id,
                                    CommentDescription = c.CommentDescription,
                                    CommentTitle = c.CommentTitle,
                                    TaskId = c.TaskId,
                                    PerformedBy = c.PerformedBy,
                                    ActivityId = activityId
                                }).ToList();
                        }
                    }
                    catch (Exception exp)
                    {
                        result = null;
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<TaskViewModel>> GetFilteredTask(int userId, TaskSearchModel searchModel)
        {
            List<TaskViewModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        //For Implicit Created Tasks Priority will be zero
                        //searchModel.SelectedPriorities.Add(0);
                        //searchModel.SelectedTeamAssigneeMemberId.Add(-1);
                        //searchModel.ProfileId.Add(-1);
                        result = db.GetTaskForSearch(userId, searchModel.GroupId)
                                   .Where(c => (searchModel.SelectedClassifications == null || searchModel.SelectedClassifications.Count == 0 || searchModel.SelectedClassifications.Contains(c.TaskClassification.Value)))
                                   .Where(p => (searchModel.SelectedPriorities == null || searchModel.SelectedPriorities.Count == 0 || searchModel.SelectedPriorities.Contains(p.TaskPriority.Value)))
                                   .Where(a => (searchModel.SelectedTeamAssigneeMemberId == null || searchModel.SelectedTeamAssigneeMemberId.Count == 0 || searchModel.SelectedTeamAssigneeMemberId.Contains(a.AssigneeMemberId.Value)))
                                   .Where(p => (searchModel.ProfileId == null || searchModel.ProfileId.Count == 0 || searchModel.ProfileId.Contains(p.SelectedSearchedProfileId.Value)))
                                   .Where(s => (searchModel.SelectedStatus == null || searchModel.SelectedStatus.Count == 0 || searchModel.SelectedStatus.Contains(s.TaskStatusId.Value)))
                                   .GroupBy(t => t.TaskId).Select(r => new TaskViewModel
                                   {
                                       AssignedTo = String.Join(",", r.GroupBy(a => a.AssigneeMemberId).Select(gg => gg.First()).Select(t => t.AssignedTo)),
                                       LastUpdated = r.Last().LastUpdated,
                                       ProfileName = r.Last().ProfileName,
                                       CustomerProfileName = r.Last().CustomerProfileName,
                                       ProfileUrl = r.Last().ProfileUrl,
                                       TaskClassification = (Classificaiton)r.Last().TaskClassification,
                                       TaskId = r.Last().TaskId.Value,
                                       TaskMessage = r.Last().TaskMessage,
                                       TaskPriority = (Priority)r.Last().TaskPriority,
                                       Status = (CRM.Common.Enum.TaskStatus)r.Last().TaskStatusId,
                                       IsRead = r.Last().IsRead.HasValue ? r.Last().IsRead.Value : false,
                                       Profile = (ProfilesType)r.First().ProfileTypeId,
                                       UserProfile = r.First().UserProfile,
                                       ActivityId = r.Last().ActivityId.Value,
                                       ActivityTable = r.Last().ActivityTable,
                                       AssignedBy = r.Last().AssignedBy,
                                       CreatedDate = r.Last().CreatedDate
                                   }).ToList();
                    }
                    catch (Exception exp)
                    {
                        result = null;
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<TaskViewModel>> GetFilteredDraftTask(int userId, int GroupId)
        {
            List<TaskViewModel> result = null;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.GetDraftTasks(userId, GroupId)
                                   .Select(r => new TaskViewModel
                                   {
                                       AssignedTo = r.AssignedTo,
                                       LastUpdated = r.LastUpdated,
                                       ProfileName = r.ProfileName,
                                       CustomerProfileName = r.CustomerProfileName,
                                       ProfileUrl = r.ProfileUrl,
                                       TaskClassification = (Classificaiton)r.TaskClassification,
                                       TaskId = r.TaskId.Value,
                                       TaskMessage = r.TaskMessage,
                                       TaskPriority = (Priority)r.TaskPriority,
                                       Status = (CRM.Common.Enum.TaskStatus)r.TaskStatusId,
                                       IsRead = r.IsRead.HasValue ? r.IsRead.Value : false,
                                       Profile = (ProfilesType)r.ProfileTypeId,
                                       UserProfile = r.UserProfile,
                                       ActivityId = r.ActivityId.Value,
                                       ActivityTable = r.ActivityTable,
                                       AssignedBy = r.AssignedBy
                                   }).ToList();
                    }
                    catch (Exception exp)
                    {
                        result = null;
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<int> AddBrandKeyword(BrandKeywordModel brandModel)
        {
            int result = -1;
            using (var db = new SocialCRMEntities())
            {
                /*int brandsCount = 0;
                string packageName = "";
                var user = db.UserCompanies.ToList().Where(x => x.UserId == brandModel.UserId).FirstOrDefault();
                packageName = user.Company.SubscriberType.Name;
                int allowedTags = 0, allowedKeywords = 0;
                if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                {
                    allowedTags = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.Tag).FirstOrDefault().Count;
                    allowedKeywords = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.Keyword).FirstOrDefault().Count;
                }
                    

                AspNetUser dbUser = db.AspNetUsers.Where(x => x.Id == brandModel.UserId).FirstOrDefault();
                UserCompany comp = dbUser.UserCompanies.FirstOrDefault();
                if (comp != null)
                {
                    //if (comp.Company.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                    {
                        var companyId = comp.Company.id;
                        brandsCount = db.BrandKeywords.Where(x => db.Groups.Where(y => y.CompanyId == companyId).Any(p => p.Id == x.GroupId) && x.IsTag == brandModel.IsTag).Count();
                    }
                }

                if((brandsCount < allowedKeywords && brandModel.IsTag == false) || (brandsCount < allowedTags && brandModel.IsTag ==  true))
                {*/
                int companyId = db.Groups.Where(x => x.Id == brandModel.GroupId).FirstOrDefault().CompanyId;
                List<int> companyGroups = db.Groups.Where(x => x.CompanyId == companyId).Select(x => x.Id).ToList();
                bool brandKeywordExist = db.BrandKeywords.Any(x => x.IsTag == brandModel.IsTag && x.KeywordText == brandModel.KeywordText && companyGroups.Contains(x.GroupId));
                if (!brandKeywordExist)
                {
                    var brand = new BrandKeyword
                    {
                        BrandKeywordCategoryId = brandModel.BrandKeywordCategoryId,
                        GroupId = brandModel.GroupId,
                        KeywordName = brandModel.KeywordName,
                        KeywordText = brandModel.KeywordText,
                        LocationName = brandModel.LocationName,
                        LocationLat = brandModel.LocationLat,
                        LocationLng = brandModel.LocationLng,
                        AddedByUserId = brandModel.UserId,
                        IsTag = brandModel.IsTag,
                        FontColor = brandModel.FontColor,
                        BGColor = brandModel.BGColor

                    };
                    db.BrandKeywords.Add(brand);
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";
                    if (brandModel.IsTag)
                    {
                        notifText = string.Format("Tag: {0} has been added in Company: {1}", (brandModel.KeywordName != "" ? brandModel.KeywordName : brandModel.KeywordText), companyName);
                        notifType = "Tag Added";
                    }
                    else
                    {
                        notifText = string.Format("Brand Keyword: {0} has been added in Company: {1}", (brandModel.KeywordName != "" ? brandModel.KeywordName : brandModel.KeywordText), companyName);
                        notifType = "Keyword Added";
                    }
                    db.AddUserNotification(brandModel.GroupId, brandModel.UserId, notifText, notifType);
                    var rowcount = await db.SaveChangesAsync();
                    result = brand.Id;

                    if (brand.IsTag == false)
                    {
                        var userStatusTwitter = new PostUserStatusTwitter
                        {
                            UserId = null,
                            MaxMentionStatusId = "0",
                            KeywordId = brand.Id,
                            MaxStatusId = "0",
                            MaxMsgSendById = "0",
                            MaxMsgSendToId = "0"
                        };
                        db.PostUserStatusTwitters.Add(userStatusTwitter);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new Exception("Tag/Keyword with the name '" + brandModel.KeywordText + "' already exists in this company");
                }
                /*}
                else
                {
                    throw new Exception("Keywords Limit Reached for "+ packageName + " Account");
                }*/
                db.Dispose();
            }
            return result;
        }

        public async Task<int> EditBrandKeyword(BrandKeywordModel brandModel)
        {
            int result = -1;
            using (var db = new SocialCRMEntities())
            {
                BrandKeyword objBrandKeyword = db.BrandKeywords.Where(x => x.Id == brandModel.Id).FirstOrDefault();
                objBrandKeyword.BrandKeywordCategoryId = brandModel.BrandKeywordCategoryId;
                objBrandKeyword.KeywordName = brandModel.KeywordName;
                objBrandKeyword.BGColor = brandModel.BGColor;
                objBrandKeyword.FontColor = brandModel.FontColor;

                db.BrandKeywords.Attach(objBrandKeyword);
                db.Entry(objBrandKeyword).State = EntityState.Modified;
                await db.SaveChangesAsync();
                result = objBrandKeyword.Id;

                db.Dispose();
            }
            return result;
        }
        public async Task<List<BrandKeywordViewModel>> GetBrandKeywordByGroupId(int groupId)
        {
            List<BrandKeywordViewModel> result = new List<BrandKeywordViewModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.BrandKeywords.Where(g => g.GroupId == groupId)
                            .Select(r => new BrandKeywordViewModel
                            {
                                Id = r.Id,
                                BrandKeywordCategoryId = r.BrandKeywordCategoryId,
                                KeywordName = r.KeywordName,
                                KeywordText = r.KeywordText,
                                LocationName = r.LocationName,
                                LocationLat = r.LocationLat,
                                LocationLng = r.LocationLng,
                                IsTag = r.IsTag,
                                FontColor = r.FontColor,
                                BGColor = r.BGColor,
                                BrandKeywordCategoryText = r.BrandKeywordCategory.BrandKeywordCategoryText
                            }).ToList();
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<BrandKeywordViewModel>> GetBrandKeywordByCompanyId(int companyId)
        {
            List<BrandKeywordViewModel> result = new List<BrandKeywordViewModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        var groupIds = db.Groups.Where(x => x.CompanyId == companyId).Select(r => r.Id).ToList();
                        result = db.BrandKeywords.Where(g => groupIds.Contains(g.GroupId))
                            .Select(r => new BrandKeywordViewModel
                            {
                                Id = r.Id,
                                BrandKeywordCategoryId = r.BrandKeywordCategoryId,
                                KeywordName = r.KeywordName,
                                KeywordText = r.KeywordText,
                                LocationName = r.LocationName,
                                LocationLat = r.LocationLat,
                                LocationLng = r.LocationLng,
                                IsTag = r.IsTag,
                                FontColor = r.FontColor,
                                BGColor = r.BGColor,
                                BrandKeywordCategoryText = r.BrandKeywordCategory.BrandKeywordCategoryText
                            }).ToList();
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> RemoveBrandKeyword(int brandKeywordId) //not being used. Tag/Keywords are removing from Mining Repository
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var brand = db.BrandKeywords.Where(b => b.Id == brandKeywordId).FirstOrDefault();
                    db.BrandKeywords.Remove(brand);

                    var group = db.Groups.Where(g => g.Id == brand.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";
                    if (brand.IsTag)
                    {
                        notifText = string.Format("Tag: {0} has been removed in Company: {1}", (brand.KeywordName != "" ? brand.KeywordName : brand.KeywordText), companyName);
                        notifType = "Tag Removed";
                    }
                    else
                    {
                        notifText = string.Format("Brand Keyword: {0} has been removed in Company: {1}", (brand.KeywordName != "" ? brand.KeywordName : brand.KeywordText), companyName);
                        notifType = "Keyword Removed";
                    }

                    db.AddUserNotification(brand.GroupId, brand.AddedByUserId, notifText, notifType);
                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> UpdateBrandKeyword(BrandKeywordModel brandModel)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var brand = db.BrandKeywords.Where(b => b.Id == brandModel.Id).FirstOrDefault();
                    if (brand != null)
                    {
                        brand.KeywordName = brandModel.KeywordName;
                        brand.KeywordText = brandModel.KeywordText;
                        brand.LocationName = brandModel.LocationName;
                        brand.LocationLat = brandModel.LocationLat;
                        brand.LocationLng = brandModel.LocationLng;
                        brand.AddedByUserId = brandModel.UserId;
                        brand.BrandKeywordCategoryId = brandModel.BrandKeywordCategoryId;
                    }

                    var group = db.Groups.Where(g => g.Id == brandModel.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";
                    if (brandModel.IsTag)
                    {
                        notifText = string.Format("Tag: {0} has been updated in Group: {1} of Company: {2}", brandModel.KeywordName, groupName, companyName);
                        notifType = "Tag Updated";
                    }
                    else
                    {
                        notifText = string.Format("Brand Keyword: {0} has been updated in Group: {1} of Company: {2}", (brandModel.KeywordName != "" ? brandModel.KeywordName : brandModel.KeywordText), groupName, companyName);
                        notifType = "Keyword Updated";
                    }

                    db.AddUserNotification(brand.GroupId, brand.AddedByUserId, notifText, notifType);
                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        #region brand keyword category

        public async Task<int> AddBrandKeywordCategory(BrandKeywordCategoryModel brandModel)
        {
            int result = -1;
            using (var db = new SocialCRMEntities())
            {

                int companyId = db.Groups.Where(x => x.Id == brandModel.GroupId).FirstOrDefault().CompanyId;
                List<int> companyGroups = db.Groups.Where(x => x.CompanyId == companyId).Select(x => x.Id).ToList();
                bool brandKeywordExist = db.BrandKeywordCategories.Any(x => x.BrandKeywordCategoryText == brandModel.BrandKeywordCategoryText && companyGroups.Contains(x.GroupId));
                if (!brandKeywordExist)
                {
                    var brand = new BrandKeywordCategory
                    {
                        BrandKeywordCategoryText = brandModel.BrandKeywordCategoryText,
                        GroupId = brandModel.GroupId,
                        BGColor = brandModel.BGColor,
                        FontColor = brandModel.FontColor

                    };
                    db.BrandKeywordCategories.Add(brand);
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";

                    notifText = string.Format("Brand Keyword: {0} has been added in Company: {1}", brandModel.BrandKeywordCategoryText, companyName);
                    notifType = "Keyword Added";

                    db.AddUserNotification(brandModel.GroupId, brandModel.UserId, notifText, notifType);
                    var rowcount = await db.SaveChangesAsync();
                    result = brand.Id;
                }
                else
                {
                    throw new Exception("Keyword Category with the name '" + brandModel.BrandKeywordCategoryText + "' already exists in this company");
                }
                /*}
                else
                {
                    throw new Exception("Keywords Limit Reached for "+ packageName + " Account");
                }*/
                db.Dispose();
            }
            return result;
        }

        public async Task<int> EditBrandKeywordCategory(BrandKeywordCategoryModel brandModel)
        {
            int result = -1;
            using (var db = new SocialCRMEntities())
            {
                BrandKeywordCategory objBrandKeyword = db.BrandKeywordCategories.Where(x => x.Id == brandModel.Id).FirstOrDefault();

                objBrandKeyword.BrandKeywordCategoryText = brandModel.BrandKeywordCategoryText;
                objBrandKeyword.BGColor = brandModel.BGColor;
                objBrandKeyword.FontColor = brandModel.FontColor;
                db.BrandKeywordCategories.Attach(objBrandKeyword);
                db.Entry(objBrandKeyword).State = EntityState.Modified;
                await db.SaveChangesAsync();
                result = objBrandKeyword.Id;

                db.Dispose();
            }
            return result;
        }
        public async Task<List<BrandKeywordCategoryModel>> GetBrandKeywordCategoryByGroupId(int groupId)
        {
            List<BrandKeywordCategoryModel> result = new List<BrandKeywordCategoryModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        result = db.BrandKeywordCategories.Where(g => g.GroupId == groupId)
                            .Select(r => new BrandKeywordCategoryModel
                            {
                                Id = r.Id,
                                BrandKeywordCategoryText = r.BrandKeywordCategoryText,
                                BGColor = r.BGColor,
                                FontColor = r.FontColor
                            }).ToList();
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<List<BrandKeywordCategoryModel>> GetBrandKeywordCategoryByCompanyId(int companyId)
        {
            List<BrandKeywordCategoryModel> result = new List<BrandKeywordCategoryModel>();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        var groupIds = db.Groups.Where(x => x.CompanyId == companyId).Select(r => r.Id).ToList();
                        result = db.BrandKeywordCategories.Where(g => groupIds.Contains(g.GroupId))
                            .Select(r => new BrandKeywordCategoryModel
                            {
                                Id = r.Id,
                                BrandKeywordCategoryText = r.BrandKeywordCategoryText,
                                BGColor = r.BGColor,
                                FontColor = r.FontColor
                            }).ToList();
                    }
                    catch (Exception exp) { }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> RemoveBrandKeywordCategory(int brandKeywordCategoryId) //not being used. Tag/Keywords are removing from Mining Repository
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var brand = db.BrandKeywordCategories.Where(b => b.Id == brandKeywordCategoryId).FirstOrDefault();
                    db.BrandKeywordCategories.Remove(brand);

                    var group = db.Groups.Where(g => g.Id == brand.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";
                    notifText = string.Format("Brand Keyword: {0} has been removed in Company: {1}", brand.BrandKeywordCategoryText, companyName);
                    notifType = "Keyword Removed";
                    // db.AddUserNotification(brand.GroupId, brand.AddedByUserId, notifText, notifType);
                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> UpdateBrandKeywordCategory(BrandKeywordCategoryModel brandModel)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var brand = db.BrandKeywordCategories.Where(b => b.Id == brandModel.Id).FirstOrDefault();
                    if (brand != null)
                    {
                        brand.BrandKeywordCategoryText = brandModel.BrandKeywordCategoryText;
                        brand.BGColor = brandModel.BGColor;
                        brand.FontColor = brandModel.FontColor;
                    }

                    var group = db.Groups.Where(g => g.Id == brandModel.GroupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    string notifText = "", notifType = "";

                    notifText = string.Format("Brand Keyword: {0} has been updated in Group: {1} of Company: {2}", brandModel.BrandKeywordCategoryText, groupName, companyName);
                    notifType = "Keyword Updated";
                    //  db.AddUserNotification(brand.GroupId, brand.AddedByUserId, notifText, notifType);
                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }

        #endregion



        public async Task<TaskClassificationModel> GetTaskClassification(int activityId, string activityTable, int groupId)
        {
            TaskClassificationModel result = new TaskClassificationModel();
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    try
                    {
                        var taskManger = db.TaskManagers.Where(t => t.ActivityId == activityId && t.ActivityTable == activityTable).FirstOrDefault();
                        if (taskManger != null)
                        {
                            result.ClassificaitonOfTask = (Classificaiton)taskManger.ClassificationId;
                            result.TaskPriority = (Priority)taskManger.PriorityTypeId;
                            result.Status = (CRM.Common.Enum.TaskStatus)taskManger.StatusId;
                            result.Assignee = db.GetTeamMembersByGroupId(groupId).Select(s => new TaskAssigneeModel
                            {
                                TeamMemberId = s.TeamMemberId,
                                TeamMemberName = s.TeamMemberName,
                                IsAssigned = taskManger.TaskAssignees.Any(a => a.TeamMemberId == s.TeamMemberId)
                            }).ToList();
                        }
                    }
                    catch (Exception exp)
                    {
                        result = null;
                    }
                    db.Dispose();
                }
            });
            return result;
        }
        public async Task<bool> RemoveCommentById(int commentId, int groupId, int userId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var teamMemberId = db.GetTeamMemberIdUsingGroupId(groupId, userId).FirstOrDefault();
                    var group = db.Groups.Where(g => g.Id == groupId).FirstOrDefault();
                    var groupName = group.Name;
                    var companyId = group != null ? group.CompanyId : 0;
                    var companyName = db.Companies.Where(x => x.id == companyId).FirstOrDefault().CompanyName;
                    var comment = db.TaskComments.Where(c => c.Id == commentId).FirstOrDefault();
                    if (comment != null && teamMemberId.HasValue)
                    {
                        comment.IsActive = false;
                        comment.DeletedByMemberId = teamMemberId;
                        comment.DeletedDate = DateTime.Now;
                        var log = new TaskLog
                        {
                            ActivityDate = DateTime.Now,
                            ActivityPerformBy = teamMemberId.Value,
                            LogTypeId = (int)LogType.CommentDeleted,
                            Text = "Comment Deleted : " + comment.Description,
                            TaskId = comment.TaskManagerId
                        };
                        db.TaskLogs.Add(log);
                    }

                    //Add User Notification
                    var allMembersInChain = new List<int>();
                    IEnumerable<int> loggedMember = comment.TaskManager.TaskLogs.Where(p => p.LogTypeId == 1).Select(t => t.AssigneeMemberId.Value);
                    allMembersInChain.AddRange(loggedMember);
                    if (!allMembersInChain.Contains(comment.TaskManager.CreatedBy))
                        allMembersInChain.Add(comment.TaskManager.CreatedBy);
                    var notifType = companyName + " - " + groupName + " - " + string.Format("Comment Removed For Task #{0}", comment.TaskManager.Id);
                    AddUserNotifications(groupId, userId, comment.Title, notifType, allMembersInChain);
                    //db.AddUserNotification(groupId, userId, comment.Title, "Comment Removed");
                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }

        public async Task<bool> InsertTaskCompleted(TaskCompletedModel markAsCompleteModel)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var alreadyCompletedTask = db.TaskCompleteds.Where(x => x.ActivityId == markAsCompleteModel.ActivityId && x.ActivityTable == markAsCompleteModel.ActivityTable).FirstOrDefault();
                    if (alreadyCompletedTask != null)
                    {
                        result = true;
                    }
                    else
                    {
                        int teamMemberId = db.Teams.Where(x => x.UserId == markAsCompleteModel.CompletedBy && x.CompanyId == markAsCompleteModel.CompanyId).FirstOrDefault().Id;
                        var taskCompleted = new TaskCompleted
                        {
                            ActivityId = markAsCompleteModel.ActivityId,
                            ActivityTable = markAsCompleteModel.ActivityTable,
                            IsCompleted = markAsCompleteModel.IsCompleted,
                            CompletedBy = teamMemberId,
                            GroupId = markAsCompleteModel.GroupId,
                            InsertedDate = DateTime.Now
                        };
                        db.TaskCompleteds.Add(taskCompleted);
                        //db.AddUserNotification(markAsCompleteModel.GroupId, taskCompleted.CompletedBy, "Feed : " + markAsCompleteModel.FeedData, "Insert Task Completed");
                        var rowcount = await db.SaveChangesAsync();
                        result = rowcount > 0;

                        NotificationHub.updateTaskCompletedItems(taskCompleted.ActivityId, taskCompleted.ActivityTable);
                        if (markAsCompleteModel.IsPage)
                            await new MiningRepository().AddSocialActivity("", markAsCompleteModel.ProfileId, markAsCompleteModel.StatusId, markAsCompleteModel.ActivityTable, 4, "", markAsCompleteModel.FeedData, markAsCompleteModel.CompletedBy, markAsCompleteModel.CompanyId, "", "", "");
                        else
                            await new MiningRepository().AddSocialActivity(markAsCompleteModel.ProfileId, "", markAsCompleteModel.StatusId, markAsCompleteModel.ActivityTable, 4, "", markAsCompleteModel.FeedData, markAsCompleteModel.CompletedBy, markAsCompleteModel.CompanyId, "", "", "");
                    }
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }

        public async Task<bool> InsertAllTaskCompleted(TaskCompletedModel markAsCompleteModel)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    int teamMemberId = db.Teams.Where(x => x.UserId == markAsCompleteModel.CompletedBy && x.CompanyId == markAsCompleteModel.CompanyId).FirstOrDefault().Id;
                    var taskCompleted = new TaskCompleted
                    {
                        ActivityId = markAsCompleteModel.ActivityId,
                        ActivityTable = markAsCompleteModel.ActivityTable,
                        IsCompleted = markAsCompleteModel.IsCompleted,
                        CompletedBy = teamMemberId,
                        GroupId = markAsCompleteModel.GroupId,
                        InsertedDate = DateTime.Now
                    };
                    db.TaskCompleteds.Add(taskCompleted);
                    //db.AddUserNotification(markAsCompleteModel.GroupId, taskCompleted.CompletedBy, "Feed : " + markAsCompleteModel.FeedData, "Insert Task Completed");
                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;

                    if (markAsCompleteModel.IsPage)
                        await new MiningRepository().AddSocialActivity("", markAsCompleteModel.ProfileId, markAsCompleteModel.StatusId, markAsCompleteModel.ActivityTable, 4, "", markAsCompleteModel.FeedData, markAsCompleteModel.CompletedBy, markAsCompleteModel.CompanyId, "", "", "");
                    else
                        await new MiningRepository().AddSocialActivity(markAsCompleteModel.ProfileId, "", markAsCompleteModel.StatusId, markAsCompleteModel.ActivityTable, 4, "", markAsCompleteModel.FeedData, markAsCompleteModel.CompletedBy, markAsCompleteModel.CompanyId, "", "", "");
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }
        public async Task<bool> DeleteTaskCompleted(int ActivityId, string ActivityTable, int GroupId)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var taskCompleted = db.TaskCompleteds.Where(b => b.ActivityId == ActivityId && b.ActivityTable == ActivityTable && b.GroupId == GroupId).FirstOrDefault();
                    db.TaskCompleteds.Remove(taskCompleted);
                    db.AddUserNotification(taskCompleted.GroupId, taskCompleted.CompletedBy, "Feed", "Delete Task Completed");
                    var rowcount = await db.SaveChangesAsync();
                    result = rowcount > 0;
                }
                catch (Exception exp)
                {
                    result = false;
                }
                db.Dispose();
            }
            return result;
        }

        public async Task<List<TaskGroupPerformanceModel>> GetGroupPerformance(int userId, int companyId, int groupId, DateTime? dateFrom, DateTime? dateTo)
        {
            List<TaskGroupPerformanceModel> lstGroupPerfModel = new List<TaskGroupPerformanceModel>();

            using (var db = new SocialCRMEntities())
            {
                try
                {
                    //string [] profileIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId).Select(y => y.Profile.ProfileExternalId).ToArray();
                    //string[] pageIds = db.ProfileComapnyGroups.Where(x => x.GroupId == groupId && x.ProfilePageId != null).Select(y => y.ProfilePage.ProfilePageID).ToArray();
                    TaskSearchModel model = new TaskSearchModel();
                    model.GroupId = groupId;
                    var data = await GetFilteredTask(userId, model);
                    int[] status = { 0, 3 };

                    var query = (from t in data.ToList()
                                 where !status.Contains(Convert.ToInt32(t.Status))
                                        //&& (profileIds.Length > 0 ? profileIds.Contains(t.ProfileId) : true) 
                                        //&& (pageIds.Length > 0 ? pageIds.Contains(t.PageId) : true)
                                        //&& t.CompanyId == companyId
                                        && ((dateFrom != null) ? (t.CreatedDate.Value.Date >= dateFrom.Value.Date) : true)
                                        && ((dateTo != null) ? (t.CreatedDate.Value.Date <= dateTo.Value.Date) : true)
                                 group t by Convert.ToInt32(t.Status) into g
                                 select new GroupPerformanceModel
                                 {
                                     StatusId = g.Key,
                                     StatusCount = g.Count()
                                 }).ToList();

                    //var result = db.GetGroupPerformance(companyId, classificationId, dateFrom, dateTo).ToList();

                    foreach (var obj in query)
                    {
                        TaskGroupPerformanceModel tempGroupPerfModel = new TaskGroupPerformanceModel();
                        tempGroupPerfModel.statusId = obj.StatusId;
                        if (tempGroupPerfModel.statusId == (int)CRM.Common.Enum.TaskStatus.Close)
                        {
                            tempGroupPerfModel.statusText = CRM.Common.Enum.TaskStatus.Close.ToString();
                        }
                        else if (tempGroupPerfModel.statusId == (int)CRM.Common.Enum.TaskStatus.InProgress)
                        {
                            tempGroupPerfModel.statusText = CRM.Common.Enum.TaskStatus.InProgress.ToString();
                        }
                        else if (tempGroupPerfModel.statusId == (int)CRM.Common.Enum.TaskStatus.Draft)
                        {
                            tempGroupPerfModel.statusText = CRM.Common.Enum.TaskStatus.Draft.ToString();
                        }

                        tempGroupPerfModel.statusCount = obj.StatusCount;

                        lstGroupPerfModel.Add(tempGroupPerfModel);
                    }
                }
                catch (Exception exp)
                {

                }
                db.Dispose();
            }

            return lstGroupPerfModel;
        }

        public async Task<List<TopInfluencerModel>> GetTopInfluencers(int companyId, DateTime? dateFrom, DateTime? dateTo)
        {
            List<TopInfluencerModel> lstTopInfluencersModel = new List<TopInfluencerModel>();

            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var result = db.GetTopInfluencers(companyId).ToList();
                    //result = result.Where(x => x.)

                    foreach (var obj in result)
                    {
                        TopInfluencerModel tempTopInfluencerModel = new TopInfluencerModel();
                        tempTopInfluencerModel.teamMemberId = obj.TeamMemberId;
                        tempTopInfluencerModel.firstName = obj.FirstName;
                        tempTopInfluencerModel.lastName = obj.LastName;
                        tempTopInfluencerModel.statusId = obj.StatusId;
                        tempTopInfluencerModel.statusCount = obj.statusCount;

                        lstTopInfluencersModel.Add(tempTopInfluencerModel);
                    }
                }
                catch (Exception exp)
                {
                    throw exp;
                }
                db.Dispose();
            }

            return lstTopInfluencersModel;
        }

        public async Task<List<SocialProfile>> GetCustomerSocialProfiles(string userIds, string commaSeperatedPages, int groupId)
        {
            List<SocialProfile> result = new List<SocialProfile>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    /*result = context.GetSocialProfilesWithComplaints(userIds, commaSeperatedPages, groupId).Select(r => new SocialProfile
                    {
                        CustomerSocialProfileId = r.CustomerProfileId,
                        ProfileName = r.CustomerProfileName,
                        ProfileType = r.ProfileType,
                        ComplaintCount = Convert.ToInt32(r.ComplaintCount)
                    }).ToList();*/

                }
            });
            return result;
        }

        public async Task<List<TaskViewModel>> GetCustomerTask(string customerProfileId, int groupId)
        {
            List<TaskViewModel> result = new List<TaskViewModel>();
            await Task.Run(() =>
            {
                using (var context = new SocialCRMEntities())
                {
                    /*result = context.GetTasksByCustomer(customerProfileId,groupId).Select(r => new TaskViewModel
                    {
                        AssignedTo = r.AssignedTo,
                        LastUpdated = r.LastUpdated,
                        ProfileName = r.ProfileName,
                        ProfileUrl = r.ProfileUrl,
                        TaskClassification = (Classificaiton)r.TaskClassification,
                        TaskId = r.TaskId.Value,
                        TaskMessage = r.TaskMessage,
                        TaskPriority = (Priority)r.TaskPriority,
                        Status = (CRM.Common.Enum.TaskStatus)r.TaskStatusId,
                        IsRead = r.IsRead.HasValue ? r.IsRead.Value : false,
                        Profile = (ProfilesType)r.ProfileTypeId,
                        UserProfile = r.UserProfile,
                        ActivityId = r.ActivityId.Value,
                        ActivityTable = r.ActivityTable,
                        AssignedBy = r.AssignedBy,
                        CustomerProfileName = r.CustomerProfileName
                    }).ToList();*/
                }
            });
            return result;
        }

        //public async Task<List<TaskViewModel>> GetLatestDraftTask(int userId, int groupId, int lastTaskId)
        //{
        //    List<TaskViewModel> result = new List<TaskViewModel>();
        //    result = await GetFilteredDraftTask(userId, groupId);
        //    result = result.Where(x => x.TaskId > lastTaskId).ToList();
        //    return result;
        //}

        //public async Task<List<TaskViewModel>> GetLatestFilteredTask(int userId, TaskSearchModel searchModel, int lastTaskId)
        //{
        //    List<TaskViewModel> result = new List<TaskViewModel>();
        //    result = await GetFilteredTask(userId, searchModel);
        //    result = result.Where(x => x.TaskId > lastTaskId).ToList();
        //    return result;
        //}
    }
}