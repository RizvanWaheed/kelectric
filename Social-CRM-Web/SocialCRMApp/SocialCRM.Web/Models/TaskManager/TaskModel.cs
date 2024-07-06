using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Common.Enum;

namespace SocialCRM.Web.Models.TaskManager
{
    public class TaskModel
    {
        public Classificaiton ClassificaitonOfTask { get; set; }
        public Priority TaskPriority { get; set; }
        public TaskStatus Status { get; set; }
        public List<int> Assignee { get; set; }
        public int ActivityId { get; set; }
        public string ActivityTable { get; set; }
        public int GroupId { get; set; }
        public string TaskMessage { get; set; }
    }
    public class TaskCommentModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int ActivityId { get; set; }
        public string CommentTitle { get; set; }
        public string CommentDescription { get; set; }
        public int GroupId { get; set; }
        public string PerformedBy { get; set; }
    }
    public class TaskLogModel
    {
        public int LogId { get; set; }
        public string AssigneeTo { get; set; }
        public string AssignedBy { get; set; }
        public string CreatedDate { get; set; }
        public int TaskId { get; set; }
        public string ActivityPerformBy { get; set; }
        public string Text { get; set; }
        public int LogTypeId { get; set; }
        public LogType ActivityLogType { get; set; }
    }
    public class TaskViewModel
    {
        public string ProfileUrl { get; set; }
        public string ProfileName { get; set; }
        public string CustomerProfileName { get; set; }
        public string TaskMessage { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public Priority TaskPriority { get; set; }
        public Classificaiton TaskClassification { get; set; }
        public string LastUpdated { get; set; }
        public int TaskId { get; set; }
        public TaskStatus Status { get; set; }
        public bool IsRead { get; set; }
        public ProfilesType Profile { get; set; }
        public string UserProfile { get; set; }
        public int ActivityId { get; set; }
        public string ActivityTable { get; set; }
        public string CssClass { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class TaskClassificationModel
    {
        public Classificaiton ClassificaitonOfTask { get; set; }
        public Priority TaskPriority { get; set; }
        public TaskStatus Status { get; set; }
        public List<TaskAssigneeModel> Assignee { get; set; }
    }
    public class TaskAssigneeModel 
    {
        public int TeamMemberId { get; set; }
        public string TeamMemberName { get; set; }
        public bool IsAssigned { get; set; }
    }
    public class TaskCompletedModel
    {
        public string ProfileId { get; set; }
        public bool IsPage { get; set; }
        public string StatusId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityTable { get; set; }
        public string FeedData { get; set; }
        public bool IsCompleted { get; set; }
        public int CompletedBy { get; set; }
        public int GroupId { get; set; }
        public int CompanyId { get; set; }
    }
}