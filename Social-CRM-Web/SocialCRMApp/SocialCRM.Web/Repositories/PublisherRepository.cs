using CRM.Common.Enum;
using CRM.Entities;
using SocialAPIsWrapper.Models;
using SocialCRM.Web.Models.Publisher;
using SocialCRM.Web.Repositories.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SocialCRM.Web.Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        private int GetDayId()
        {
            var day = DateTime.Now.ToString("ddd");

            int dayId = 0;
            switch (day.ToUpper())
            {
                case "MON":
                    dayId = 1;
                    break;
                case "TUE":
                    dayId = 2;
                    break;
                case "WED":
                    dayId = 3;
                    break;
                case "THU":
                    dayId = 4;
                    break;
                case "FRI":
                    dayId = 5;
                    break;
                case "SAT":
                    dayId = 6;
                    break;
                case "SUN":
                    dayId = 7;
                    break;
            };
            return dayId;
        }
        private int GetDayIdByDayString(string str_day)
        {
            var day = str_day;

            int dayId = 0;
            switch (day)
            {
                case "Monday":
                    dayId = 1;
                    break;
                case "Tuesday":
                    dayId = 2;
                    break;
                case "Wednesday":
                    dayId = 3;
                    break;
                case "Thursday":
                    dayId = 4;
                    break;
                case "Friday":
                    dayId = 5;
                    break;
                case "Saturday":
                    dayId = 6;
                    break;
                case "Sunday":
                    dayId = 7;
                    break;
            };
            return dayId;
        }
        private string GetDayStringByDayId(int dayId)
        {
            string strDay = "";
            switch (dayId)
            {
                case 1:
                    strDay = "Monday";
                    break;
                case 2:
                    strDay = "Tuesday";
                    break;
                case 3:
                    strDay = "Wednesday";
                    break;
                case 4:
                    strDay = "Thursday";
                    break;
                case 5:
                    strDay = "Friday";
                    break;
                case 6:
                    strDay = "Saturday";
                    break;
                case 0:
                    strDay = "Sunday";
                    break;
            };
            return strDay;
        }
        //public void CreateLog(SocialCRMEntities dbInstance, int logCodeId, string logDescription, int authorId, int profileCompanyGroupId)
        //{
        //    Log logObj = new Log();
        //    logObj.logcodeid = logCodeId;
        //    logObj.logDescription = logDescription;
        //    logObj.CreatedBy = authorId;
        //    logObj.ProfileCompanyGroupId = profileCompanyGroupId;
        //    logObj.CreatedDate = DateTime.Now;

        //    dbInstance.Logs.Add(logObj);
        //}
        public async Task<bool> AddMessage(ScheduledMessageModel model)
        {
            bool result = false;
            using (var db = new SocialCRMEntities())
            {
                var teamMember = db.Teams.Where(t => t.UserId == model.CreatorUserId && t.CompanyId == model.CompanyId).FirstOrDefault();
                if (teamMember != null)
                {
                    var message = new ScheduledMessage();
                    //if (teamMember.Premissions.Any(t => t.Id == (int)PremissionType.PublishWithOutApproval))
                    //{
                    //    message.MessageStatus = (int)MessageStatus.Approved;
                    //}
                    //else
                    //{
                    //    message.MessageStatus = (int)MessageStatus.Pending;
                    //}
                    if (model.MessageStatus == 0)
                        message.MessageStatus = (int)MessageStatus.Draft;
                    else if (model.MessageStatus == 1)
                        message.MessageStatus = (int)MessageStatus.Scheduled;
                    else if (model.MessageStatus == 2)
                        message.MessageStatus = (int)MessageStatus.Approved;

                    message.AuthorTeamMemberId = teamMember.Id;
                    message.CreatedDate = DateTime.Now;
                    message.MessageText = model.MessageText;
                    message.IsDrafted = model.IsDraft;
                    if (model.IsScheduledMessage)
                    {
                        #region Scheduled Message
                        message.MessageType = (int)MessageType.Scheduled;

                        //Asjad


                        //Asjad
                        var scheduledMessage = new List<ScheduledMessageProfile>();
                        foreach (var profileId in model.SelectedProfiles)
                        {
                            ScheduledMessageProfile smp = new ScheduledMessageProfile();
                            smp.ScheduledMessage = message;
                            smp.ProfileCompanyGroupId = profileId;
                            smp.TeamId = teamMember.Id;

                            var messagePostingTime = new List<MessagePostingTime>();
                            foreach (var myobj in model.PostTimes)
                            {
                                string[] datetime = myobj.Split('|');
                                MessagePostingTime publishInfo = new MessagePostingTime();
                                publishInfo.PublishDate = DateTime.ParseExact(datetime[0].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                                publishInfo.IsPublished = false;

                                TimeSpan ts = Convert.ToDateTime(datetime[1].ToString()).TimeOfDay;
                                publishInfo.PublishTime = ts;
                                messagePostingTime.Add(publishInfo);
                            }

                            smp.MessagePostingTimes = messagePostingTime.ToList();

                            scheduledMessage.Add(smp);
                        }
                        message.ScheduledMessageProfiles = scheduledMessage;

                        //model.SelectedProfiles.ForEach(p =>
                        //{
                        //    scheduledMessage.Add(new ScheduledMessageProfile { ScheduledMessage = message, ProfileCompanyGroupId = p, MessagePostingTimes = messagePostingTime.ToList() });
                        //});

                        try
                        {
                            //db.ScheduledMessageProfiles.AddRange(scheduledMessage);
                            db.ScheduledMessages.Add(message);

                            //foreach (var profileId in model.SelectedProfiles)
                            //{
                            //    //Add to logs
                            //    CreateLog(db, (int)LogCodes.MessageScheduled, "Message Scheduled using scheduler.", (int)teamMember.Id, (int)profileId);
                            //}

                            //Check if media exists
                            if (model.mediaInfo.Count > 0)
                            {
                                db.SaveChanges();
                                //Now save the files for Scheduled Message
                                foreach (var item in model.mediaInfo)
                                {
                                    PublisherUploadedFile PublisherUploadedFileObj = new PublisherUploadedFile();
                                    PublisherUploadedFileObj.FileName = item.FileName;
                                    PublisherUploadedFileObj.ContentType = item.ContentType;

                                    string[] fileContents = item.Base64String.Split(',');

                                    PublisherUploadedFileObj.FileContent = Convert.FromBase64String(fileContents[1].ToString());
                                    string checkup = Convert.ToBase64String(PublisherUploadedFileObj.FileContent);
                                    PublisherUploadedFileObj.ScheduledMessageId = message.Id;

                                    db.PublisherUploadedFiles.Add(PublisherUploadedFileObj);

                                    //foreach (var profileId in model.SelectedProfiles)
                                    //{
                                    //    //Add to logs
                                    //    CreateLog(db, (int)LogCodes.MediaUploaded, "Media Uploaded using scheduler.", (int)teamMember.Id, (int)profileId);
                                    //}
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Queued Message

                        //message.MessageType = (int)MessageType.Queued;
                        //var queuedMessage = new List<QueuedMessageProfile>();
                        //var today = GetDayId();
                        //DateTime activityDateTime = DateTime.Now;
                        //if (!model.IsPreConfiguredQueue)
                        //{
                        //    //Profile Queue Time.
                        //    var queueTiming = new List<ProfileQueueTime>();
                        //    model.SelectedProfiles.ForEach(p =>
                        //    {
                        //        model.PostTimes.ForEach(time =>
                        //        {
                        //            var QueuedateTime = time.Split('@');
                        //            var queueTime = QueuedateTime.Last();
                        //            var queueDays = QueuedateTime.First().Split(',').ToList();
                        //            queueDays.ForEach(d =>
                        //            {
                        //                int dayId = Convert.ToInt16(d);
                        //                //switch (d.ToUpper())
                        //                //{
                        //                //    case "MON":
                        //                //        dayId = 1;
                        //                //        break;
                        //                //    case "TUE":
                        //                //        dayId = 2;
                        //                //        break;
                        //                //    case "WED":
                        //                //        dayId = 3;
                        //                //        break;
                        //                //    case "THR":
                        //                //        dayId = 4;
                        //                //        break;
                        //                //    case "FRI":
                        //                //        dayId = 5;
                        //                //        break;
                        //                //    case "SAT":
                        //                //        dayId = 6;
                        //                //        break;
                        //                //    case "SUN":
                        //                //        dayId = 7;
                        //                //        break;
                        //                //};
                        //                DateTime t_dateTime = DateTime.ParseExact(queueTime, "h:mm tt", CultureInfo.InvariantCulture);
                        //                TimeSpan t_span = t_dateTime.TimeOfDay;
                        //                //var queueExeTime = TimeSpan.ParseExact(queueTime, "h:mm tt", CultureInfo.InvariantCulture);
                        //                var queueExeTime = t_span;

                        //                queueTiming.Add(new ProfileQueueTime
                        //                {
                        //                    ProfileCompanyGroupId = p,
                        //                    IsActive = true,
                        //                    DayId = dayId,
                        //                    Time = queueExeTime
                        //                });
                        //                if (today == dayId && DateTime.Now.TimeOfDay <= queueExeTime)
                        //                {
                        //                    activityDateTime = DateTime.Now.AddDays(7).Date + queueExeTime;
                        //                }
                        //                else if (today == dayId && DateTime.Now.TimeOfDay > queueExeTime)
                        //                {
                        //                    activityDateTime = DateTime.Now.Date + queueExeTime;
                        //                }
                        //                else if (dayId > today)
                        //                {
                        //                    activityDateTime = DateTime.Now.AddDays(dayId - today).Date + queueExeTime;
                        //                }
                        //                else
                        //                {
                        //                    var daydiff = dayId + (7 - today);
                        //                    activityDateTime = DateTime.Now.AddDays(daydiff).Date + queueExeTime;
                        //                }
                        //                //setting activityDateTime
                        //                queuedMessage.Add(new QueuedMessageProfile
                        //                {
                        //                    ActivityDateTime = activityDateTime,
                        //                    IsSent = false,
                        //                    ScheduledMessage = message,
                        //                    ProfileCompanyGroupId = p
                        //                });
                        //            });
                        //        });
                        //    });
                        //    db.ProfileQueueTimes.AddRange(queueTiming);
                        //}
                        //else
                        //{
                        //    //use Configured Queue.
                        //    if (model.IsQueueAtBeginning)
                        //    {
                        //        model.SelectedProfiles.ForEach(p =>
                        //        {
                        //            var todaysExecutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId == today && s.Time > DateTime.Now.TimeOfDay).OrderBy(o => o.Time).FirstOrDefault();
                        //            if (todaysExecutionTime != null)
                        //            {
                        //                activityDateTime = activityDateTime.Date + todaysExecutionTime.Time;
                        //            }
                        //            else
                        //            {
                        //                //check future day.
                        //                var futureDayExecutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId > today).OrderBy(o => o.Time).FirstOrDefault();
                        //                if (futureDayExecutionTime != null)
                        //                {
                        //                    var dayDiff = futureDayExecutionTime.DayId - today;
                        //                    activityDateTime = DateTime.Now.AddDays(dayDiff).Date + futureDayExecutionTime.Time;
                        //                }
                        //                else
                        //                {
                        //                    //Check Comming Week time.
                        //                    var nextWeekExecutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId < today).OrderBy(o => o.Time).FirstOrDefault();
                        //                    if (nextWeekExecutionTime != null)
                        //                    {
                        //                        var dayDiff = nextWeekExecutionTime.DayId + (7 - today);
                        //                        activityDateTime = DateTime.Now.AddDays(dayDiff).Date + nextWeekExecutionTime.Time;
                        //                    }
                        //                    else
                        //                    {
                        //                        //same day past time.
                        //                        var todayPastExecutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId == today && s.Time <= DateTime.Now.TimeOfDay).OrderBy(o => o.Time).FirstOrDefault();
                        //                        if (todayPastExecutionTime != null)
                        //                        {
                        //                            activityDateTime = DateTime.Now.AddDays(7).Date + todayPastExecutionTime.Time;
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //            queuedMessage.Add(new QueuedMessageProfile
                        //            {
                        //                ActivityDateTime = activityDateTime,
                        //                IsSent = false,
                        //                ScheduledMessage = message,
                        //                ProfileCompanyGroupId = p
                        //            });
                        //        });
                        //    }
                        //    else
                        //    {
                        //        //For Queue at End.
                        //        model.SelectedProfiles.ForEach(p =>
                        //        {
                        //            var thisWeekLastExcutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId > today).OrderByDescending(o => o.Time).FirstOrDefault();
                        //            if (thisWeekLastExcutionTime != null)
                        //            {
                        //                activityDateTime = activityDateTime.Date + thisWeekLastExcutionTime.Time;
                        //            }
                        //            else
                        //            {
                        //                var todayLastExecutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId == today && s.Time > DateTime.Now.TimeOfDay).OrderByDescending(o => o.Time).FirstOrDefault();
                        //                if (todayLastExecutionTime != null)
                        //                {
                        //                    activityDateTime = activityDateTime.Date + todayLastExecutionTime.Time;
                        //                }
                        //                else
                        //                {
                        //                    //Today past Matching time
                        //                    var todayPastExecutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId == today && s.Time < DateTime.Now.TimeOfDay).OrderByDescending(o => o.Time).FirstOrDefault();
                        //                    if (todayPastExecutionTime != null)
                        //                    {
                        //                        activityDateTime = activityDateTime.AddDays(7).Date + todayPastExecutionTime.Time;
                        //                    }
                        //                    else
                        //                    {
                        //                        var pastDaysLastExcutionTime = db.ProfileQueueTimes.Where(s => s.ProfileCompanyGroupId == p && s.DayId < today).OrderByDescending(o => o.Time).FirstOrDefault();
                        //                        if (pastDaysLastExcutionTime != null)
                        //                        {
                        //                            var dayDiff = pastDaysLastExcutionTime.DayId + (7 - today);
                        //                            activityDateTime = DateTime.Now.AddDays(dayDiff).Date + pastDaysLastExcutionTime.Time;
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //            queuedMessage.Add(new QueuedMessageProfile
                        //            {
                        //                ActivityDateTime = activityDateTime,
                        //                IsSent = false,
                        //                ScheduledMessage = message,
                        //                ProfileCompanyGroupId = p
                        //            });
                        //        });
                        //    }
                        //}
                        //db.QueuedMessageProfiles.AddRange(queuedMessage);
                        #endregion
                    }
                }
                try
                {
                    var resultset = await db.SaveChangesAsync();
                    result = resultset > 0;
                    db.Dispose();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                //var resultset = await db.SaveChangesAsync();

            }
            return result;
        }
        public async Task<bool> UpdateScheduledMessage(ScheduledMessageModel model)
        {
            bool result = false;
            bool isRejected = false;
            int ScheduledMessageId = model.ScheduledMessageId;
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    var teamMember = db.Teams.Where(t => t.UserId == model.CreatorUserId && t.CompanyId == model.CompanyId).FirstOrDefault();
                    if (teamMember != null)
                    {
                        var ScheduledMessage_db = db.ScheduledMessages.Where(x => x.Id == ScheduledMessageId).FirstOrDefault();
                        var message = new ScheduledMessage();

                        if (model.IsDraft == false && ScheduledMessage_db.IsDrafted == true && ScheduledMessage_db.MessageStatus == (int)MessageStatus.Draft && model.MessageStatus == (int)MessageStatus.Scheduled)
                        {
                            ScheduledMessage_db.IsDrafted = model.IsDraft;
                            ScheduledMessage_db.MessageStatus = (int)MessageStatus.Scheduled;
                        }
                        else if (model.IsDraft == false && ScheduledMessage_db.IsDrafted == true && ScheduledMessage_db.MessageStatus == (int)MessageStatus.Draft && model.MessageStatus == (int)MessageStatus.Approved)
                        {
                            ScheduledMessage_db.IsDrafted = model.IsDraft;
                            ScheduledMessage_db.MessageStatus = (int)MessageStatus.Approved;
                        }
                        else if (model.IsDraft == true && ScheduledMessage_db.IsDrafted == true && ScheduledMessage_db.MessageStatus == (int)MessageStatus.Draft)
                        {
                            //no change
                        }
                        else if (model.IsDraft == false && model.MessageStatus == (int)MessageStatus.Approved)
                        {
                            ScheduledMessage_db.IsDrafted = model.IsDraft;
                            ScheduledMessage_db.MessageStatus = (int)MessageStatus.Approved;
                        }
                        else if (model.IsDraft == false && ScheduledMessage_db.MessageStatus == (int)MessageStatus.Scheduled && model.MessageStatus == (int)MessageStatus.Reject)
                        {
                            ScheduledMessage_db.MessageStatus = (int)MessageStatus.Reject;
                            db.ScheduledMessages.Attach(ScheduledMessage_db);
                            db.Entry(ScheduledMessage_db).State = EntityState.Modified;
                            isRejected = true;
                        }


                        //Update Message
                        //db.ScheduledMessages.Attach(ScheduledMessage_db);
                        //db.SaveChanges();

                        ////message.Id = ScheduledMessageId;
                        //message.IsDrafted = model.IsDraft;
                        //message.MessageStatus = model.MessageStatus;
                        //message.MessageText = model.MessageText;
                        //message.MessageType = ScheduledMessage_db.MessageType;
                        //message.CreatedDate = ScheduledMessage_db.CreatedDate;
                        //message.AuthorTeamMemberId = ScheduledMessage_db.AuthorTeamMemberId;

                        if (!isRejected)
                        {
                            //Message Text
                            ScheduledMessage_db.MessageText = model.MessageText;

                            var ScheduleProfiles = db.ScheduledMessageProfiles.Where(x => x.ScheduledMessageId == ScheduledMessageId).ToList();
                            List<int> Profiles = new List<int>();
                            List<int> MsgIds = new List<int>();

                            foreach (var obj in ScheduleProfiles)
                            {
                                Profiles.Add(obj.Id);
                            }

                            //Deleting old posting times
                            var MsgPostingTimes = db.MessagePostingTimes.Where(x => Profiles.Contains(x.ScheduledMessageProfileId)).ToList();
                            foreach (var mpt in MsgPostingTimes)
                            {
                                MsgIds.Add(mpt.Id);
                                db.MessagePostingTimes.Remove(mpt);
                            }

                            //Deleting old profiles
                            foreach (var scheduleprofile in ScheduleProfiles)
                            {
                                db.ScheduledMessageProfiles.Remove(scheduleprofile);
                            }

                            //Delete old Images to add new
                            var db_PublisherUploadedFiles = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == model.ScheduledMessageId).ToList();
                            db.PublisherUploadedFiles.RemoveRange(db_PublisherUploadedFiles);

                            db.SaveChanges();

                            //Asjad
                            var scheduledMessage = new List<ScheduledMessageProfile>();
                            foreach (var profileId in model.SelectedProfiles)
                            {
                                ScheduledMessageProfile smp = new ScheduledMessageProfile();
                                smp.ScheduledMessage = ScheduledMessage_db;
                                smp.ProfileCompanyGroupId = profileId;
                                smp.TeamId = teamMember.Id;

                                var messagePostingTime = new List<MessagePostingTime>();
                                foreach (var myobj in model.PostTimes)
                                {
                                    string[] datetime = myobj.Split('|');
                                    MessagePostingTime publishInfo = new MessagePostingTime();
                                    publishInfo.PublishDate = DateTime.ParseExact(datetime[0].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                                    publishInfo.IsPublished = false;

                                    TimeSpan ts = Convert.ToDateTime(datetime[1].ToString()).TimeOfDay;
                                    publishInfo.PublishTime = ts;
                                    messagePostingTime.Add(publishInfo);
                                }

                                smp.MessagePostingTimes = messagePostingTime.ToList();

                                scheduledMessage.Add(smp);

                                //Log
                               // CreateLog(db, (int)LogCodes.MessageUpdated, "Message Status of Scheduled Message Id " + smp.ScheduledMessageId + " updated to " + smp.ScheduledMessage.MessageStatus, (int)teamMember.Id, (int)profileId);
                            }

                            ScheduledMessage_db.ScheduledMessageProfiles = scheduledMessage;
                            db.ScheduledMessages.Attach(ScheduledMessage_db);

                            foreach (var item in model.mediaInfo)
                            {
                                PublisherUploadedFile PublisherUploadedFileObj = new PublisherUploadedFile();
                                PublisherUploadedFileObj.FileName = item.FileName;
                                PublisherUploadedFileObj.ContentType = item.ContentType;

                                if (item.Base64String.Contains(","))
                                {
                                    string[] fileContents = item.Base64String.Split(',');
                                    PublisherUploadedFileObj.FileContent = Convert.FromBase64String(fileContents[1].ToString());
                                }
                                else
                                {
                                    PublisherUploadedFileObj.FileContent = Convert.FromBase64String(item.Base64String);
                                }

                                PublisherUploadedFileObj.ScheduledMessageId = model.ScheduledMessageId;
                                db.PublisherUploadedFiles.Add(PublisherUploadedFileObj);

                                //foreach (var profileId in model.SelectedProfiles)
                                //{
                                //    //Add to logs
                                //    CreateLog(db, (int)LogCodes.MediaUploaded, "Media Uploaded using scheduler for Message Id: " + model.ScheduledMessageId + ".", (int)teamMember.Id, (int)profileId);
                                //}
                            }
                        }
                    }

                    var resultset = db.SaveChanges();
                    result = resultset > 0;
                    db.Dispose();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                //var resultset = await db.SaveChangesAsync();

            }
            return result;
        }
        public Task<ProfileQueueViewModel> GetProfilesQueue(int groupId)
        {
            throw new NotImplementedException();
        }
        public Task<ScheduledMessageLogViewModel> GetScheduledMessageLog(int profileCompanyGroupId)
        {
            throw new NotImplementedException();
        }
        public async Task<List<ScheduledMessages>> GetAllScheduledMessages(ScheduledMessageModel model)
        {
            int pageSize = 30;
            int pageNumber = model.PageNumber;
            bool getLatest = model.getLatest;
            bool result = false;
            List<ScheduledMessages> lstScheduledMessages = new List<ScheduledMessages>();
            List<ScheduledMessages> lstScheduledMessagesToBeSend = new List<ScheduledMessages>();
            //DateTime mydate = DateTime.Now.AddDays(-2);
            try
            {
                using (var db = new SocialCRMEntities())
                {
                    DateTime currentDate = DateTime.Now.Date;
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;
                    String timespan = currentTime.Hours + ":" + currentTime.Minutes + ":" + currentTime.Seconds;
                    var teamMember = db.Teams.Where(t => t.UserId == model.CreatorUserId && t.CompanyId == model.CompanyId).FirstOrDefault();
                    var db_data = db.MessagePostingTimes.Where(x => x.ScheduledMessageProfile.ScheduledMessage.IsDrafted == false).OrderByDescending(x => x.PublishDate).ThenByDescending(x => x.PublishTime).ToList();
                    var db_data_drafts = db.MessagePostingTimes.Where(x => x.ScheduledMessageProfile.ScheduledMessage.IsDrafted == true && x.ScheduledMessageProfile.ScheduledMessage.AuthorTeamMemberId == teamMember.Id).OrderByDescending(x => x.PublishDate).ThenByDescending(x => x.PublishTime).ToList();
                    db_data.AddRange(db_data_drafts);
                    db_data = db_data.OrderByDescending(x => x.PublishDate).ThenByDescending(x => x.PublishTime).ToList();

                    var data = db_data;

                    #region Message Filters
                    //Don't show Drafts
                    if (!model.MessageTypes.Contains(1))
                    {
                        data = data.Where(x => x.ScheduledMessageProfile.ScheduledMessage.IsDrafted == false).ToList();
                    }

                    //Don't show Sent messages
                    if (!model.MessageTypes.Contains(2))
                    {
                        data = data.Where(x => x.IsPublished == false && x.ScheduledMessageProfile.ScheduledMessage.MessageStatus != Convert.ToInt32(MessageStatus.Sent)).ToList();
                    }

                    //Don't show Approved Messages
                    if (!model.MessageTypes.Contains(3))
                    {
                        data = data.Where(x => x.ScheduledMessageProfile.ScheduledMessage.MessageStatus != Convert.ToInt32(MessageStatus.Approved)).ToList();
                    }

                    //Don't show Rejected Messages
                    if (!model.MessageTypes.Contains(4))
                    {
                        data = data.Where(x => x.ScheduledMessageProfile.ScheduledMessage.MessageStatus != Convert.ToInt32(MessageStatus.Reject)).ToList();
                    }

                    //Don't show Scheduled Messages
                    if (!model.MessageTypes.Contains(5))
                    {
                        data = data.Where(x => x.ScheduledMessageProfile.ScheduledMessage.MessageStatus != Convert.ToInt32(MessageStatus.Scheduled)).ToList();
                    }
                    #endregion

                    foreach (var MessagePostingTimeObj in data)
                    {
                        int ProfileCompanyGroupId = MessagePostingTimeObj.ScheduledMessageProfile.ProfileCompanyGroupId;

                        if (model.SelectedProfiles.Contains(ProfileCompanyGroupId))
                        {

                            ScheduledMessages obj = new ScheduledMessages();
                            obj.Id = MessagePostingTimeObj.ScheduledMessageProfile.ScheduledMessage.Id;
                            obj.MessageText = HttpUtility.UrlDecode(MessagePostingTimeObj.ScheduledMessageProfile.ScheduledMessage.MessageText);
                            obj.CreatedDate = MessagePostingTimeObj.ScheduledMessageProfile.ScheduledMessage.CreatedDate.ToShortDateString();
                            obj.AuthorTeamMemberId = MessagePostingTimeObj.ScheduledMessageProfile.ScheduledMessage.AuthorTeamMemberId;
                            //obj.PostingDate = MessagePostingTimeObj.PublishDate.ToShortDateString();
                            obj.PostingDate = MessagePostingTimeObj.PublishDate.ToString("dd/MM/yyyy");
                            obj.MessageStatus = (int)MessagePostingTimeObj.ScheduledMessageProfile.ScheduledMessage.MessageStatus;
                            obj.MessagePostingTimeId = MessagePostingTimeObj.Id;

                            string ampm = MessagePostingTimeObj.PublishTime.Hours >= 12 ? "PM" : "AM";
                            int HourVal = MessagePostingTimeObj.PublishTime.Hours % 12;
                            if (HourVal == 0) HourVal = 12;
                            string Hour = MessagePostingTimeObj.PublishTime.Hours % 12 >= 10 ? HourVal.ToString() : "0" + HourVal.ToString();
                            if (MessagePostingTimeObj.PublishTime.Hours % 12 == 0)
                                Hour = "12";

                            string Minute = MessagePostingTimeObj.PublishTime.Minutes >= 10 ? MessagePostingTimeObj.PublishTime.Minutes.ToString() : "0" + MessagePostingTimeObj.PublishTime.Minutes.ToString();

                            obj.PostingTime = Hour + ":" + Minute + " " + ampm;
                            obj.PostingDay = GetDayStringByDayId((int)MessagePostingTimeObj.PublishDate.DayOfWeek);

                            //If Profile Id
                            if (MessagePostingTimeObj.ScheduledMessageProfile.ProfileComapnyGroup.Profile != null)
                            {
                                obj.ProfileTypeId = MessagePostingTimeObj.ScheduledMessageProfile.ProfileComapnyGroup.Profile.ProfileTypeId;
                                obj.Username = MessagePostingTimeObj.ScheduledMessageProfile.ProfileComapnyGroup.Profile.Name;
                                obj.ProfileURL = MessagePostingTimeObj.ScheduledMessageProfile.ProfileComapnyGroup.Profile.ProfilePic;                                
                            }
                            else //if Profile Page
                            {
                                obj.ProfileTypeId = 1;
                                obj.Username = MessagePostingTimeObj.ScheduledMessageProfile.ProfileComapnyGroup.ProfilePage.PageName;
                                obj.ProfileURL = MessagePostingTimeObj.ScheduledMessageProfile.ProfileComapnyGroup.ProfilePage.ProfilePic;
                            }

                            var authorDetails = db.Teams.Where(x => x.Id == MessagePostingTimeObj.ScheduledMessageProfile.ScheduledMessage.AuthorTeamMemberId).FirstOrDefault();
                            obj.AuthorName = authorDetails.FirstName + " " + authorDetails.LastName;

                            //Attaching Images to message
                            var db_images = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == obj.Id).ToList();
                            List<MediaModelTwitter> lstMediaInfo = new List<MediaModelTwitter>();
                            foreach (var item in db_images)
                            {
                                lstMediaInfo.Add(new MediaModelTwitter
                                {
                                    FileName = item.FileName,
                                    ContentType = item.ContentType,
                                    Base64String = Convert.ToBase64String(item.FileContent)
                                });
                            }
                            obj.mediaInfo = lstMediaInfo;

                            lstScheduledMessages.Add(obj);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //If I dont want pagination, I get -1 as page Number
            if (pageNumber == -1)
                return lstScheduledMessages;

            //Pagination
            int fromRecord = (pageNumber - 1) * pageSize;
            int toRecord = (pageNumber * pageSize);

            if (getLatest == true)
            {
                fromRecord = 0;
            }

            if (fromRecord >= lstScheduledMessages.Count)
            {
                return null;
            }

            if (toRecord > lstScheduledMessages.Count)
            {
                toRecord = lstScheduledMessages.Count;
            }

            for (int loop = fromRecord; loop < toRecord; loop++)
            {
                lstScheduledMessagesToBeSend.Add(lstScheduledMessages[loop]);
            }


            return lstScheduledMessagesToBeSend;
        }
        /*  
            *   This method Add/Remove/Edit/Transforms queue, Deletes all time slots and 
            *   Add them again into the list. This method is the core of queuing. Rescheduling
            *   of messages is the main part of it.
            *   ______________________________________________________________________________
            *   Comment By: Asjad Azeem
        */
        public async Task<bool> AddTimeToQueue(List<ProfileQueueViewModel> model)
        {
            bool result = false;
            bool isAlreadyQueue = false;
            int CreatorUserId = model[0].CreatorUserId;
            int CompanyId = model[0].CompanyId;
            int ProfileCompanyGroupId = model[0].ProfileCompanyGroupId;

            using (var db = new SocialCRMEntities())
            {
                var queuedProfile = new QueuedMessageProfile();
                var teamMember = db.Teams.Where(t => t.UserId == CreatorUserId && t.CompanyId == CompanyId).FirstOrDefault();
                if (teamMember != null)
                {
                    //Get QueuedMessageProfile Id (if Exists)
                    var Queue = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == ProfileCompanyGroupId).FirstOrDefault();

                    if (Queue == null)
                    {
                        queuedProfile.ProfileCompanyGroupId = ProfileCompanyGroupId;
                        queuedProfile.CreatedDateTime = DateTime.Now;
                    }
                    else
                    {
                        isAlreadyQueue = true;
                        queuedProfile = Queue;
                    }

                    try
                    {
                        //Saving older ProfileQueueTimes Time & QueueMessageProfileId
                        List<int> QueueTimeIds_Old = new List<int>();
                        //int QueueMessageProfileId = Queue.Id;
                        foreach (var queueTime in queuedProfile.ProfileQueueTimes)
                        {
                            QueueTimeIds_Old.Add(queueTime.Id);
                        }

                        //Clear previous items of ProfileQueueTimes
                        db.ProfileQueueTimes.RemoveRange(queuedProfile.ProfileQueueTimes);
                        db.SaveChanges();

                        foreach (ProfileQueueViewModel ProfileQueueVMobj in model)
                        {
                            //Queue Data
                            var profileQueueTimeObj = new ProfileQueueTime();
                            profileQueueTimeObj.DayId = Convert.ToInt32(ProfileQueueVMobj.Day);

                            DateTime dateTime = DateTime.ParseExact(ProfileQueueVMobj.Time, "hh:mm tt", CultureInfo.InvariantCulture);
                            TimeSpan span = dateTime.TimeOfDay;

                            profileQueueTimeObj.Time = span;
                            profileQueueTimeObj.IsActive = false;

                            queuedProfile.ProfileQueueTimes.Add(profileQueueTimeObj);
                        }

                        if (isAlreadyQueue)
                        {
                            db.QueuedMessageProfiles.Attach(queuedProfile);
                        }
                        else
                        {
                            db.QueuedMessageProfiles.Add(queuedProfile);
                        }


                        var resultset = await db.SaveChangesAsync();
                        result = resultset > 0;

                        //Reschedule Messages

                        // 1-Get all unpublished messages
                        var unpublishedMessages = db.QueuePostingTimes.Where(x => x.IsPublished == false && QueueTimeIds_Old.Contains(x.ProfileQueueTimeId)).OrderBy(x => x.Precedence).ToList();

                        // 2-Get precedence & scheduledMessagesId
                        List<int> precedence = new List<int>();
                        List<int> scheduledMessageIds = new List<int>();

                        foreach (var message in unpublishedMessages)
                        {
                            precedence.Add(message.Precedence);
                            scheduledMessageIds.Add(message.ScheduledMessageId);
                        }

                        // 3-Remove Messages
                        db.QueuePostingTimes.RemoveRange(unpublishedMessages);

                        // Logs
                       // CreateLog(db, (int)LogCodes.QueueTimeSlotsModified, "Queue time slots modified.", teamMember.Id, ProfileCompanyGroupId);

                        db.SaveChanges();

                        // 4-Reschedule
                        // Create QueueMessage List/obj
                        List<QueuedMessageReschedule> lst_queuedMessages = new List<QueuedMessageReschedule>();

                        int loop_precedenceOrder = 0;
                        foreach (int precedenceOrder in precedence)
                        {
                            QueuedMessageReschedule QMobj = new QueuedMessageReschedule();
                            QMobj.CompanyId = CompanyId;
                            QMobj.CreatorUserId = CreatorUserId;
                            QMobj.SelectedProfiles = ProfileCompanyGroupId;
                            QMobj.ScheduledMessageId = scheduledMessageIds[loop_precedenceOrder];

                            loop_precedenceOrder++;
                            lst_queuedMessages.Add(QMobj);
                        }

                        // 5-Calling Reschedular
                        if (lst_queuedMessages.Count > 0)
                        {
                            var trial = BulkRescheduling(lst_queuedMessages);
                        }

                        bool AllowBulkReschedule = db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == Queue.Id).ToList().Count > 0;
                        if (lst_queuedMessages.Count == 0 && AllowBulkReschedule)
                        {
                            //This will fetch waiting messages from table and place it on slots.
                            result = SetTimingAlgorithmForWaitingMessages(ProfileCompanyGroupId);
                        }

                        db.Dispose();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }

            return result;
        }
        public async Task<bool> DeleteAllSlotsOfQueue(ProfileQueueViewModel model)
        {
            bool result = false;
            bool isAlreadyQueue = false;
            int CreatorUserId = model.CreatorUserId;
            int CompanyId = model.CompanyId;
            int ProfileCompanyGroupId = model.ProfileCompanyGroupId;

            using (var db = new SocialCRMEntities())
            {
                var teamMember = db.Teams.Where(t => t.UserId == CreatorUserId && t.CompanyId == CompanyId).FirstOrDefault();
                if (teamMember != null)
                {
                    //Get QueuedMessageProfile Id (if Exists)
                    var QueueProfile = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == ProfileCompanyGroupId).FirstOrDefault();

                    List<int> ProfileQueueTimeIds = new List<int>();
                    List<int> ScheduledMessageIds = new List<int>();

                    foreach (var obj in QueueProfile.ProfileQueueTimes)
                    {
                        ProfileQueueTimeIds.Add(obj.Id);
                    }

                    var QueuePostingTimes = db.QueuePostingTimes.Where(x => ProfileQueueTimeIds.Contains(x.ProfileQueueTimeId) && x.IsPublished == false).ToList();

                    foreach (var obj in QueuePostingTimes)
                    {
                        ScheduledMessageIds.Add(obj.ScheduledMessageId);
                    }

                    List<QueueWaitingMessage> lst_queueWaitingMessage = new List<QueueWaitingMessage>();
                    foreach (var scheduledMessageId in ScheduledMessageIds)
                    {
                        QueueWaitingMessage QueueWaitingMessageObj = new QueueWaitingMessage();
                        QueueWaitingMessageObj.QueuedMessageProfileId = QueueProfile.Id;
                        QueueWaitingMessageObj.ScheduledMessageId = scheduledMessageId;
                        lst_queueWaitingMessage.Add(QueueWaitingMessageObj);
                    }

                    var temp_WaitingMessagesRecords = db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == QueueProfile.Id).ToList();

                    try
                    {
                        //Changes in Database
                        //Delete data from queueTimes and queuePostingTimes
                        db.ProfileQueueTimes.RemoveRange(QueueProfile.ProfileQueueTimes);
                        db.QueuePostingTimes.RemoveRange(QueuePostingTimes);

                        //Temporarily remove old data for sorting purpose
                        db.QueueWaitingMessages.RemoveRange(temp_WaitingMessagesRecords);
                        db.SaveChanges();

                        //Add to Waiting Messages Slot
                        db.QueueWaitingMessages.AddRange(lst_queueWaitingMessage);

                        //Again add that temporarily removed items
                        db.QueueWaitingMessages.AddRange(temp_WaitingMessagesRecords);

                        //logs
                        //CreateLog(db, (int)LogCodes.DeleteAllQueueSlots, "Delete All Queue slots and save them in waiting state", teamMember.Id, ProfileCompanyGroupId);

                        var resultset = await db.SaveChangesAsync();
                        result = resultset > 0;
                        db.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return result;
        }
        public List<ProfileQueueViewModel> GetAllQueues(ProfileQueueViewModel model)
        {
            using (var db = new SocialCRMEntities())
            {
                try
                {
                    List<ProfileQueueViewModel> lstProfileQueue = new List<ProfileQueueViewModel>();
                    //List<QueuedMessageProfile> queuedProfile = new List<QueuedMessageProfile>();
                    var Queues = db.QueuedMessageProfiles.Where(x => x.ProfileComapnyGroup.CompanyId == model.CompanyId).ToList();

                    foreach (var Queue in Queues)
                    {
                        //QueuedMessageProfile QueueProfileObj = new QueuedMessageProfile();

                        //QueueProfileObj.ProfileCompanyGroupId = Queue.ProfileCompanyGroupId;
                        //QueueProfileObj.CreatedDateTime = Queue.CreatedDateTime;

                        //QueueProfileObj.ProfileComapnyGroup = Queue.ProfileComapnyGroup;

                        //foreach (var PostTime in Queue.ProfileQueueTimes)
                        //{
                        //    QueueProfileObj.ProfileQueueTimes.Add(PostTime);
                        //}

                        //queuedProfile.Add(QueueProfileObj);
                        Queue.ProfileQueueTimes = Queue.ProfileQueueTimes.OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();

                        foreach (var PostTime in Queue.ProfileQueueTimes)
                        {
                            ProfileQueueViewModel ProfileQueueVM = new ProfileQueueViewModel();
                            ProfileQueueVM.CompanyId = Queue.ProfileComapnyGroup.CompanyId;
                            ProfileQueueVM.ProfileCompanyGroupId = Queue.ProfileCompanyGroupId;
                            ProfileQueueVM.Day = PostTime.DayId.ToString();
                            ProfileQueueVM.Time = PostTime.Time.ToString();

                            lstProfileQueue.Add(ProfileQueueVM);
                        }


                    }

                    return lstProfileQueue;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<bool> ScheduleQueueMessage(QueuedMessage model)
        {
            int scheduledMessageId = 0;
            bool result = false;
            DateTime activityDateTime = DateTime.Now;
            int selectedProfileQueueTimeRecordId = 0;
            int receivedPrecedence = 0;
            bool haveToWait = false;
            bool oneOrMoreProfileQueueNotFound = false;

            using (var db = new SocialCRMEntities())
            {
                var teamMember = db.Teams.Where(t => t.UserId == model.CreatorUserId && t.CompanyId == model.CompanyId).FirstOrDefault();
                if (teamMember != null)
                {
                    foreach (int profileId in model.SelectedProfiles)
                    {
                        var currentProfileQueue = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == profileId).FirstOrDefault();
                        if (currentProfileQueue == null && oneOrMoreProfileQueueNotFound == false)
                        {
                            oneOrMoreProfileQueueNotFound = true;
                            break;
                        }

                    }

                    if (!oneOrMoreProfileQueueNotFound)
                    {
                        var message = new ScheduledMessage();
                        message.AuthorTeamMemberId = teamMember.Id;
                        message.CreatedDate = DateTime.Now;
                        message.IsDrafted = false;
                        message.MessageStatus = Convert.ToInt16(MessageStatus.Approved);
                        message.MessageText = model.MessageText;
                        message.MessageType = Convert.ToInt16(MessageType.Queued);

                        try
                        {
                            var temp = db.ScheduledMessages.Add(message);

                            // Log
                            //foreach (int profileId in model.SelectedProfiles)
                            //{
                            //    CreateLog(db, (int)LogCodes.ScheduleQueueMessage, "Schedule Queue Message for profile", teamMember.Id, profileId);
                            //}

                            var resultset = db.SaveChanges();
                            result = resultset > 0;
                            //db.Dispose();

                            //Id
                            scheduledMessageId = temp.Id;

                            //Now save the files for Scheduled Message
                            foreach (var item in model.mediaInfo)
                            {
                                PublisherUploadedFile PublisherUploadedFileObj = new PublisherUploadedFile();
                                PublisherUploadedFileObj.FileName = item.FileName;
                                PublisherUploadedFileObj.ContentType = item.ContentType;

                                if (item.Base64String.Contains(","))
                                {
                                    string[] fileContents = item.Base64String.Split(',');
                                    PublisherUploadedFileObj.FileContent = Convert.FromBase64String(fileContents[1].ToString());
                                }
                                else
                                {
                                    PublisherUploadedFileObj.FileContent = Convert.FromBase64String(item.Base64String);
                                }

                                PublisherUploadedFileObj.ScheduledMessageId = scheduledMessageId;
                                db.PublisherUploadedFiles.Add(PublisherUploadedFileObj);

                                // Log
                                //foreach (int profileId in model.SelectedProfiles)
                                //{
                                //    CreateLog(db, (int)LogCodes.MediaUploaded, "Media Uploaded for Schedule Queue Message", teamMember.Id, profileId);
                                //}
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        int today = GetDayId();
                        //Queue Algorithm
                        foreach (int profileId in model.SelectedProfiles)
                        {
                            var profileQueue = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == profileId).FirstOrDefault();

                            //for queue Last & queue Next
                            //Both are done this way so I can remove this if
                            if (model.QueueOption == 2 || model.QueueOption == 1)
                            {
                                #region Queue Last
                                DateTime currentDateTime = DateTime.Now;
                                TimeSpan t_span = currentDateTime.TimeOfDay;

                                bool timingSet = false;

                                int i = 0;
                                ProfileQueueTime queueFirstRecord = null;

                                var db_profileQueueTimes = db.ProfileQueueTimes.Where(x => x.QueuedMessageProfileId == profileQueue.Id && x.IsActive == false).OrderBy(x => x.Time).ToList();

                                //Present
                                var profileQueueTimes = db_profileQueueTimes.Where(x => x.IsActive == false && x.DayId == today).OrderBy(x => x.Time).ToList();
                                foreach (var queueTime in profileQueueTimes)
                                {
                                    if (queueFirstRecord == null)
                                    {
                                        queueFirstRecord = queueTime;
                                    }
                                    if (timingSet == false)
                                    {
                                        //agar aaj k din ki he bat hai to time dkhen ge k karskte hen ya nae
                                        if (today == queueTime.DayId)
                                        {
                                            //Time abhi hai
                                            if (DateTime.Now.TimeOfDay < queueTime.Time)
                                            {
                                                TimeSpan ts = queueTime.Time;
                                                activityDateTime = activityDateTime.Date + ts;
                                                timingSet = true;
                                                selectedProfileQueueTimeRecordId = queueTime.Id;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                //Future
                                profileQueueTimes = db_profileQueueTimes.Where(x => x.IsActive == false && x.DayId > today).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                                foreach (var queueTime in profileQueueTimes)
                                {
                                    if (timingSet == false)
                                    {
                                        if (today < queueTime.DayId)
                                        {
                                            int diffOfDays = queueTime.DayId - today;
                                            activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                            timingSet = true;
                                            selectedProfileQueueTimeRecordId = queueTime.Id;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                //Past
                                profileQueueTimes = db_profileQueueTimes.Where(x => x.IsActive == false && x.DayId < today).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                                foreach (var queueTime in profileQueueTimes)
                                {
                                    if (timingSet == false)
                                    {
                                        if (today > queueTime.DayId)
                                        {
                                            int diffOfDays = 7 - today + queueTime.DayId;
                                            activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                            timingSet = true;
                                            selectedProfileQueueTimeRecordId = queueTime.Id;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                profileQueueTimes = db_profileQueueTimes.Where(x => x.IsActive == false && x.DayId == today).OrderBy(x => x.Time).ToList();
                                //When Any slot found available (specially when the date time has passed)
                                if (profileQueueTimes.Count > 0)
                                {
                                    //Again Present + 7 Days
                                    if (timingSet == false)
                                    {
                                        TimeSpan ts = queueFirstRecord.Time;
                                        activityDateTime = activityDateTime.AddDays(7).Date + ts;
                                        timingSet = true;
                                        selectedProfileQueueTimeRecordId = queueFirstRecord.Id;
                                    }

                                    //Get Max Number of Precedence
                                    var tempPrece = db.QueuePostingTimes.Max(x => (int?)x.Precedence) ?? 0;

                                    if (tempPrece != null)
                                        receivedPrecedence = Convert.ToInt32(tempPrece);
                                }
                                else //When there's no slot availabe and message have to be pushed into waiting messages line
                                {
                                    var db_QueuedMessageProfile = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == profileId).FirstOrDefault();

                                    QueueWaitingMessage QueueWaitingMessageObj = new QueueWaitingMessage();
                                    QueueWaitingMessageObj.QueuedMessageProfileId = db_QueuedMessageProfile.Id;
                                    QueueWaitingMessageObj.ScheduledMessageId = scheduledMessageId;
                                    //In case of QueueLast
                                    if (model.QueueOption == 2)
                                    {
                                        db.QueueWaitingMessages.Add(QueueWaitingMessageObj);
                                    }
                                    haveToWait = true;
                                }


                                #endregion
                            }

                            try
                            {
                                if (!haveToWait)
                                {
                                    //Attaching schedule id to Posting time
                                    var queuePostingTime = new QueuePostingTime();

                                    queuePostingTime.PublishDate = activityDateTime;
                                    queuePostingTime.IsPublished = false;
                                    queuePostingTime.ProfileQueueTimeId = selectedProfileQueueTimeRecordId;
                                    queuePostingTime.ScheduledMessageId = scheduledMessageId;
                                    queuePostingTime.Precedence = receivedPrecedence + 1;
                                    queuePostingTime.ProfileCompanyGroupId = profileId;

                                    var recordToBeUpdated = db.ProfileQueueTimes.Where(x => x.Id == selectedProfileQueueTimeRecordId).FirstOrDefault();
                                    //recordToBeUpdated.ScheduledMessageId = scheduledMessageId;
                                    recordToBeUpdated.IsActive = true;
                                    //db.ProfileQueueTimes.Attach(recordToBeUpdated);

                                    db.QueuePostingTimes.Add(queuePostingTime);
                                    db.SaveChanges();
                                }
                                else
                                {

                                }

                                // if Queue Next
                                if (model.QueueOption == 1)
                                {
                                    #region Same for Both
                                    var Latest_profileQueueTimes = db.ProfileQueueTimes.Where(x => x.QueuedMessageProfileId == profileQueue.Id && x.IsActive == true).OrderBy(x => x.Time).ToList();
                                    List<int> lst_profileQueueTime = new List<int>();
                                    foreach (var obj in Latest_profileQueueTimes)
                                    {
                                        lst_profileQueueTime.Add(obj.Id);
                                    }

                                    //All scheduled messages
                                    var Latest_queuePostings = db.QueuePostingTimes.Where(x => lst_profileQueueTime.Contains(x.ProfileQueueTimeId) && x.IsPublished == false).ToList();
                                    List<int> lst_scheduledMessageIds = new List<int>();
                                    List<int> lst_precedenceOrder = new List<int>();

                                    foreach (var obj in Latest_queuePostings)
                                    {
                                        lst_scheduledMessageIds.Add(obj.ScheduledMessageId);
                                        lst_precedenceOrder.Add(obj.Precedence);
                                    }

                                    //Insert At first place
                                    lst_scheduledMessageIds.Insert(0, scheduledMessageId);
                                    #endregion

                                    //No Message in Waiting state
                                    if (db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == profileQueue.Id).ToList().Count <= 0)
                                    {
                                        #region Queue Next

                                        //Saving Last element : will be used if needed
                                        int lastScheduledMessage = lst_scheduledMessageIds[lst_scheduledMessageIds.Count - 1];
                                        if (lst_scheduledMessageIds[0] != lst_scheduledMessageIds[lst_scheduledMessageIds.Count - 1])
                                        {
                                            QueueWaitingMessage obj = new QueueWaitingMessage();
                                            obj.QueuedMessageProfileId = profileQueue.Id;
                                            obj.ScheduledMessageId = lastScheduledMessage;
                                            db.QueueWaitingMessages.Add(obj);
                                        }

                                        //Remove Last element
                                        lst_scheduledMessageIds.RemoveAt(lst_scheduledMessageIds.Count - 1);

                                        //Sort Precedence
                                        lst_precedenceOrder.Sort();

                                        int loop = 0;
                                        foreach (var obj in Latest_queuePostings)
                                        {
                                            obj.ScheduledMessageId = lst_scheduledMessageIds[loop];
                                            obj.Precedence = lst_precedenceOrder[loop];
                                            loop++;
                                        }
                                        #endregion
                                    }
                                    //Some Message(s) in Waiting state
                                    else
                                    {
                                        int scheduledMessageThatWillGoInWaitingQueue = lst_scheduledMessageIds[lst_scheduledMessageIds.Count - 1];

                                        //Remove Last element
                                        lst_scheduledMessageIds.RemoveAt(lst_scheduledMessageIds.Count - 1);

                                        //Sort Precedence
                                        lst_precedenceOrder.Sort();

                                        int loop = 0;
                                        foreach (var obj in Latest_queuePostings)
                                        {
                                            obj.ScheduledMessageId = lst_scheduledMessageIds[loop];
                                            obj.Precedence = lst_precedenceOrder[loop];
                                            loop++;
                                        }

                                        //Now sort the waiting messages put this unlucky one on the first node of queue
                                        var db_WaitingQueuedMessages = db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == profileQueue.Id).ToList();
                                        List<int> lst_waitingQueuedMessages = new List<int>();

                                        lst_waitingQueuedMessages.Add(scheduledMessageThatWillGoInWaitingQueue);

                                        foreach (var msg in db_WaitingQueuedMessages)
                                        {
                                            lst_waitingQueuedMessages.Add(msg.ScheduledMessageId);
                                            db.QueueWaitingMessages.Remove(msg);
                                        }

                                        db.SaveChanges();

                                        foreach (int msgId in lst_waitingQueuedMessages)
                                        {
                                            QueueWaitingMessage QWMobj = new QueueWaitingMessage();
                                            QWMobj.QueuedMessageProfileId = profileQueue.Id;
                                            QWMobj.ScheduledMessageId = msgId;
                                            db.QueueWaitingMessages.Add(QWMobj);
                                        }
                                    }
                                }

                                var resultset = db.SaveChanges();
                                if (haveToWait)
                                {
                                    result = resultset > 0;
                                }
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }
                        }
                        db.Dispose();
                    }//oneOrmoreProfile ends
                }
            }

            if (oneOrMoreProfileQueueNotFound)
            {
                return false;
            }
            else
            {
                return result;
            }

        }
        public async Task<bool> BulkRescheduling(List<QueuedMessageReschedule> model)
        {
            bool result = false;
            int CreatorUserId = model[0].CreatorUserId;
            int CompanyId = model[0].CompanyId;
            int ProfileCompanyGroupId = model[0].SelectedProfiles;

            using (var db = new SocialCRMEntities())
            {
                var teamMember = db.Teams.Where(t => t.UserId == CreatorUserId && t.CompanyId == CompanyId).FirstOrDefault();
                if (teamMember != null)
                {
                    try
                    {
                        var profileQueue = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == ProfileCompanyGroupId).FirstOrDefault();

                        //Get Max Number of Precedence
                        var tempPrece = db.QueuePostingTimes.Max(x => (int?)x.Precedence) ?? 0;

                        int receivedPrecedence = 0;
                        if (tempPrece != null)
                            receivedPrecedence = Convert.ToInt32(tempPrece);

                        DateTime currentDateTime = DateTime.Now;
                        TimeSpan t_span = currentDateTime.TimeOfDay;
                        bool timingSet = false;

                        int i = 0;
                        ProfileQueueTime queueFirstRecord = null;
                        int today = GetDayId();
                        DateTime activityDateTime = DateTime.Now;
                        int selectedProfileQueueTimeRecordId = 0;

                        var db_profileQueueTimes = db.ProfileQueueTimes.Where(x => x.QueuedMessageProfileId == profileQueue.Id && x.IsActive == false).OrderBy(x => x.Time).ToList();

                        //Deleting Old Messages that were scheduled
                        List<int> profileQueueTimesIds = new List<int>();
                        foreach (var profileQueueTimeObj in db_profileQueueTimes)
                        {
                            profileQueueTimesIds.Add(profileQueueTimeObj.Id);
                        }
                        var old_messages = db.QueuePostingTimes.Where(x => profileQueueTimesIds.Contains(x.ProfileQueueTimeId)).ToList();
                        //db.QueuePostingTimes.RemoveRange(old_messages);
                        //db.SaveChanges();

                        //Place for new messages
                        List<QueuePostingTime> lst_QueuePostingTime = new List<QueuePostingTime>();

                        bool runOneTime = false;
                        List<QueueWaitingMessage> lstPreviousQueueWaitingMessages = new List<QueueWaitingMessage>();

                        //Runs for Unpublished Queued Messages
                        for (int QueueMessage = 0; QueueMessage < model.Count; QueueMessage++)
                        {
                            #region Timing Algorithm
                            timingSet = false;
                            //Present
                            #region Present
                            var profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId == today && x.IsActive == false).OrderBy(x => x.Time).ToList();
                            foreach (var queueTime in profileQueueTimes)
                            {
                                if (queueFirstRecord == null)
                                {
                                    queueFirstRecord = queueTime;
                                }
                                if (timingSet == false)
                                {
                                    //agar aaj k din ki he bat hai to time dkhen ge k karskte hen ya nae
                                    if (today == queueTime.DayId)
                                    {
                                        //Time abhi hai
                                        if (DateTime.Now.TimeOfDay < queueTime.Time)
                                        {
                                            TimeSpan ts = queueTime.Time;
                                            activityDateTime = activityDateTime.Date + ts;
                                            timingSet = true;
                                            selectedProfileQueueTimeRecordId = queueTime.Id;
                                        }

                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion
                            //Future
                            #region Furture
                            profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId > today && x.IsActive == false).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                            foreach (var queueTime in profileQueueTimes)
                            {
                                if (timingSet == false)
                                {
                                    if (today < queueTime.DayId)
                                    {
                                        int diffOfDays = queueTime.DayId - today;
                                        activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                        timingSet = true;
                                        selectedProfileQueueTimeRecordId = queueTime.Id;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion
                            //Past
                            #region Past
                            profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId < today && x.IsActive == false).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                            foreach (var queueTime in profileQueueTimes)
                            {
                                if (timingSet == false)
                                {
                                    if (today > queueTime.DayId)
                                    {
                                        int diffOfDays = 7 - today + queueTime.DayId;
                                        activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                        timingSet = true;
                                        selectedProfileQueueTimeRecordId = queueTime.Id;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion
                            //Again Present + 7 Days
                            profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId == today && x.IsActive == false).OrderBy(x => x.Time).ToList();
                            if (timingSet == false && profileQueueTimes.Count > 0)
                            {
                                TimeSpan ts = queueFirstRecord.Time;
                                activityDateTime = activityDateTime.AddDays(7).Date + ts;
                                timingSet = true;
                                selectedProfileQueueTimeRecordId = queueFirstRecord.Id;
                            }
                            #endregion

                            //Ultimately timingSet is true if queuing slots are available                            
                            if (timingSet == true)
                            {
                                //Attaching schedule id to Posting time
                                var queuePostingTime = new QueuePostingTime();

                                queuePostingTime.PublishDate = activityDateTime;
                                queuePostingTime.IsPublished = false;
                                queuePostingTime.ProfileQueueTimeId = selectedProfileQueueTimeRecordId;
                                queuePostingTime.ScheduledMessageId = model[QueueMessage].ScheduledMessageId;
                                queuePostingTime.ProfileCompanyGroupId = ProfileCompanyGroupId;

                                receivedPrecedence++;
                                queuePostingTime.Precedence = receivedPrecedence;

                                var recordToBeUpdated = db_profileQueueTimes.Where(x => x.Id == selectedProfileQueueTimeRecordId).FirstOrDefault();
                                recordToBeUpdated.IsActive = true;
                                //db.SaveChanges();

                                lst_QueuePostingTime.Add(queuePostingTime);
                            }
                            //If timingSet is false, that means there's no slot available and we are short of
                            //available slots. In this case we need to push extra messages to QueueWaitingMessages Table
                            else
                            {
                                QueueWaitingMessage QWBobj = new QueueWaitingMessage();
                                QWBobj.QueuedMessageProfileId = profileQueue.Id;
                                QWBobj.ScheduledMessageId = model[QueueMessage].ScheduledMessageId;
                                if (runOneTime == false)
                                {
                                    lstPreviousQueueWaitingMessages = db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == profileQueue.Id).ToList();
                                    db.QueueWaitingMessages.RemoveRange(lstPreviousQueueWaitingMessages);
                                    db.SaveChanges();
                                    runOneTime = true;
                                }
                                profileQueue.QueueWaitingMessages.Add(QWBobj);
                                db.SaveChanges();
                            }

                            //In case needed
                            queueFirstRecord = null;
                            activityDateTime = DateTime.Now;
                        }

                        //After successful run of published messages
                        if (runOneTime == true)
                        {
                            db.QueueWaitingMessages.AddRange(lstPreviousQueueWaitingMessages);
                            db.SaveChanges();
                        }

                        //Look if we have any slot remaining
                        if (db_profileQueueTimes.Where(x => x.IsActive == false).ToList().Count > 0)
                        {
                            //Look if we have any messages in Waiting State
                            var db_WaitingMessages = db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == profileQueue.Id).ToList();
                            if (db_WaitingMessages.Count > 0)
                            {
                                //loop on messages
                                foreach (var message in db_WaitingMessages)
                                {
                                    //If there are no slots then break the loop
                                    if (db_profileQueueTimes.Where(x => x.IsActive == false).ToList().Count <= 0)
                                        break;

                                    #region Timing Algorithm
                                    timingSet = false;
                                    //Present
                                    #region Present
                                    var profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId == today && x.IsActive == false).OrderBy(x => x.Time).ToList();
                                    foreach (var queueTime in profileQueueTimes)
                                    {
                                        if (queueFirstRecord == null)
                                        {
                                            queueFirstRecord = queueTime;
                                        }
                                        if (timingSet == false)
                                        {
                                            //agar aaj k din ki he bat hai to time dkhen ge k karskte hen ya nae
                                            if (today == queueTime.DayId)
                                            {
                                                //Time abhi hai
                                                if (DateTime.Now.TimeOfDay < queueTime.Time)
                                                {
                                                    TimeSpan ts = queueTime.Time;
                                                    activityDateTime = activityDateTime.Date + ts;
                                                    timingSet = true;
                                                    selectedProfileQueueTimeRecordId = queueTime.Id;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    #endregion
                                    //Future
                                    #region Furture
                                    profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId > today && x.IsActive == false).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                                    foreach (var queueTime in profileQueueTimes)
                                    {
                                        if (timingSet == false)
                                        {
                                            if (today < queueTime.DayId)
                                            {
                                                int diffOfDays = queueTime.DayId - today;
                                                activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                                timingSet = true;
                                                selectedProfileQueueTimeRecordId = queueTime.Id;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    #endregion
                                    //Past
                                    #region Past
                                    profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId < today && x.IsActive == false).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                                    foreach (var queueTime in profileQueueTimes)
                                    {
                                        if (timingSet == false)
                                        {
                                            if (today > queueTime.DayId)
                                            {
                                                int diffOfDays = 7 - today + queueTime.DayId;
                                                activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                                timingSet = true;
                                                selectedProfileQueueTimeRecordId = queueTime.Id;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    #endregion
                                    //Again Present + 7 Days
                                    profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId == today && x.IsActive == false).OrderBy(x => x.Time).ToList();
                                    if (timingSet == false && profileQueueTimes.Count > 0)
                                    {
                                        TimeSpan ts = queueFirstRecord.Time;
                                        activityDateTime = activityDateTime.AddDays(7).Date + ts;
                                        timingSet = true;
                                        selectedProfileQueueTimeRecordId = queueFirstRecord.Id;
                                    }
                                    #endregion

                                    //Ultimately timingSet is true if queuing slots are available                            
                                    if (timingSet == true)
                                    {
                                        //Attaching schedule id to Posting time
                                        var queuePostingTime = new QueuePostingTime();

                                        queuePostingTime.PublishDate = activityDateTime;
                                        queuePostingTime.IsPublished = false;
                                        queuePostingTime.ProfileQueueTimeId = selectedProfileQueueTimeRecordId;
                                        queuePostingTime.ScheduledMessageId = message.ScheduledMessageId;
                                        queuePostingTime.ProfileCompanyGroupId = ProfileCompanyGroupId;

                                        receivedPrecedence++;
                                        queuePostingTime.Precedence = receivedPrecedence;

                                        var recordToBeUpdated = db_profileQueueTimes.Where(x => x.Id == selectedProfileQueueTimeRecordId).FirstOrDefault();
                                        recordToBeUpdated.IsActive = true;

                                        //Remove from waiting
                                        db.QueueWaitingMessages.Remove(message);

                                        //Add to Queue
                                        lst_QueuePostingTime.Add(queuePostingTime);
                                    }

                                    //In case needed
                                    queueFirstRecord = null;
                                    activityDateTime = DateTime.Now;
                                }
                            }
                        }
                        //Adding QueuePostingTimes to database
                        db.QueuePostingTimes.AddRange(lst_QueuePostingTime);
                        var resultset = db.SaveChanges();
                        result = resultset > 0;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return result;
        }
        public async Task<List<ScheduledMessages>> GetAllQueuedMessages(QueuedMessage model)
        {
            int pageSize = 30;
            int pageNumber = model.PageNumber;
            bool getLatest = model.getLatest;
            bool result = false;
            List<ScheduledMessages> lstScheduledMessages = new List<ScheduledMessages>();
            List<ScheduledMessages> lstScheduledMessagesToBeSend = new List<ScheduledMessages>();


            //DateTime mydate = DateTime.Now.AddDays(-2);
            try
            {
                using (var db = new SocialCRMEntities())
                {
                    //var data = db.MessagePostingTimes.Where(x => x.IsPublished == true && x.PublishDate >= mydate).ToList();
                    DateTime currentDate = DateTime.Now.Date;
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;
                    String timespan = currentTime.Hours + ":" + currentTime.Minutes + ":" + currentTime.Seconds;
                    //var data = db.QueuePostingTimes.Where(x => x.PublishDate >= currentDate).OrderByDescending(x => x.PublishDate).ToList();
                    var data = db.QueuePostingTimes.OrderByDescending(x => x.PublishDate).ToList();

                    //Both Sent & Pending
                    if (model.MessageTypes.Contains(1) && model.MessageTypes.Contains(2))
                    {
                        //OK
                    }
                    //Only Pending
                    else if (model.MessageTypes.Contains(1) && !model.MessageTypes.Contains(2))
                    {
                        data = data.Where(x => x.IsPublished == false).ToList();
                    }
                    //Only Sent
                    else if (!model.MessageTypes.Contains(1) && model.MessageTypes.Contains(2))
                    {
                        data = data.Where(x => x.IsPublished == true).ToList();
                    }
                    else if (!model.MessageTypes.Contains(1) && !model.MessageTypes.Contains(2))
                    {
                        data.Clear();
                    }

                    int profileId = 0;
                    foreach (var MessagePostingTimeObj in data)
                    {

                        profileId = (int)MessagePostingTimeObj.ProfileCompanyGroupId;
                        //if (model.SelectedProfiles.Contains(ProfileCompanyGroupId))
                        if (model.SelectedProfiles.Contains(profileId))
                        {

                            ScheduledMessages obj = new ScheduledMessages();
                            obj.Id = MessagePostingTimeObj.ScheduledMessageId;

                            var rec_ScheduledMessage = db.ScheduledMessages.Where(x => x.Id == MessagePostingTimeObj.ScheduledMessageId).FirstOrDefault();

                            obj.MessageText = HttpUtility.UrlDecode(rec_ScheduledMessage.MessageText);
                            obj.CreatedDate = rec_ScheduledMessage.CreatedDate.ToShortDateString();
                            obj.AuthorTeamMemberId = rec_ScheduledMessage.AuthorTeamMemberId;
                            obj.PublishDate = MessagePostingTimeObj.PublishDate;
                            obj.PostingDate = MessagePostingTimeObj.PublishDate.ToString("dd/MM/yyyy");
                            obj.PostingDay = GetDayStringByDayId((int)MessagePostingTimeObj.PublishDate.DayOfWeek);

                            obj.MessageStatus = Convert.ToInt32(rec_ScheduledMessage.MessageStatus);

                            //string Hour = MessagePostingTimeObj.PublishTime.Hours >= 10 ? MessagePostingTimeObj.PublishTime.Hours.ToString() : "0" + MessagePostingTimeObj.PublishTime.Hours.ToString();
                            //string Minute = MessagePostingTimeObj.PublishTime.Minutes >= 10 ? MessagePostingTimeObj.PublishTime.Minutes.ToString() : "0" + MessagePostingTimeObj.PublishTime.Minutes.ToString();

                            TimeSpan PublishTime = MessagePostingTimeObj.PublishDate.TimeOfDay;

                            string ampm = PublishTime.Hours >= 12 ? "PM" : "AM";
                            int HourVal = PublishTime.Hours % 12;
                            string Hour = "";
                            if (HourVal == 0)
                            {
                                HourVal = 12;
                                Hour = HourVal.ToString();
                            }
                            else
                            {
                                Hour = PublishTime.Hours % 12 >= 10 ? HourVal.ToString() : "0" + HourVal.ToString();
                            }
                            string Minute = PublishTime.Minutes >= 10 ? PublishTime.Minutes.ToString() : "0" + PublishTime.Minutes.ToString();

                            obj.PostingTime = Hour + ":" + Minute + " " + ampm;

                            //Attaching Images to message
                            var db_images = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == obj.Id).ToList();
                            List<MediaModelTwitter> lstMediaInfo = new List<MediaModelTwitter>();
                            foreach (var item in db_images)
                            {
                                lstMediaInfo.Add(new MediaModelTwitter
                                {
                                    FileName = item.FileName,
                                    ContentType = item.ContentType,
                                    Base64String = Convert.ToBase64String(item.FileContent)
                                });
                            }
                            obj.mediaInfo = lstMediaInfo;

                            //try
                            var db_profile = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == profileId).FirstOrDefault();


                            //If Profile Id
                            if (db_profile.ProfileComapnyGroup.Profile != null)
                            {
                                obj.ProfileTypeId = db_profile.ProfileComapnyGroup.Profile.ProfileTypeId;
                                obj.Username = db_profile.ProfileComapnyGroup.Profile.Name;
                                obj.ProfileURL = db_profile.ProfileComapnyGroup.Profile.ProfilePic;
                            }
                            else //if Profile Page
                            {
                                obj.ProfileTypeId = 1;
                                obj.Username = db_profile.ProfileComapnyGroup.ProfilePage.PageName;
                                obj.ProfileURL = db_profile.ProfileComapnyGroup.ProfilePage.ProfilePic;
                            }

                            var authorDetails = db.Teams.Where(x => x.Id == rec_ScheduledMessage.AuthorTeamMemberId).FirstOrDefault();
                            obj.AuthorName = authorDetails.FirstName + " " + authorDetails.LastName;

                            lstScheduledMessages.Add(obj);
                        }
                    }


                    //Thinking about Waiting Messages now
                    var db_QueuedMessageProfiles = db.QueuedMessageProfiles.Where(x => model.SelectedProfiles.Contains(x.ProfileCompanyGroupId)).ToList();
                    List<int> lst_QueuedMessageProfiles = new List<int>();
                    List<int> lst_QueuedProfileCompanyGroupIds = new List<int>();
                    foreach (var QueueMsgProfileObj in db_QueuedMessageProfiles)
                    {
                        lst_QueuedMessageProfiles.Add(QueueMsgProfileObj.Id);
                        lst_QueuedProfileCompanyGroupIds.Add(QueueMsgProfileObj.ProfileCompanyGroupId);
                    }

                    var db_WaitingMessages = db.QueueWaitingMessages.Where(x => lst_QueuedMessageProfiles.Contains(x.QueuedMessageProfileId)).ToList();
                    var db_ProfileQueueTimes = db.ProfileQueueTimes.Where(x => lst_QueuedMessageProfiles.Contains(x.QueuedMessageProfileId)).ToList();

                    //These are messages that are in WaitingMessages table but queue has somehow some slots
                    if (db_WaitingMessages.Count > 0 && model.MessageTypes.Contains(1))
                    {
                        List<ScheduledMessages> lstWaitingMessages = new List<ScheduledMessages>();

                        //For every selected profile
                        for (int profileLoop = 0; profileLoop < lst_QueuedMessageProfiles.Count; profileLoop++)
                        {
                            var profileWaitingMessages = db_WaitingMessages.Where(x => x.QueuedMessageProfileId == lst_QueuedMessageProfiles[profileLoop]).ToList();
                            var db_queueSlots = db_ProfileQueueTimes.Where(x => x.QueuedMessageProfileId == lst_QueuedMessageProfiles[profileLoop]).ToList();

                            if (db_queueSlots.Count == 0)
                            {
                                continue;
                            }

                            List<int> lstQueueSlotsIds = new List<int>();
                            foreach (var obj in db_queueSlots)
                            {
                                lstQueueSlotsIds.Add(obj.Id);
                            }

                            var db_queuePostingTimes = db.QueuePostingTimes.Where(x => lstQueueSlotsIds.Contains(x.ProfileQueueTimeId)).ToList();

                            //int profileCompanyGroupId = model.SelectedProfiles[profileLoop];
                            int profileCompanyGroupId = lst_QueuedProfileCompanyGroupIds[profileLoop];
                            var db_latestPostingDate = db.QueuePostingTimes.Where(x => x.ProfileCompanyGroupId == profileCompanyGroupId).OrderByDescending(x => x.Id).FirstOrDefault();
                            DateTime PostingDate = new DateTime();

                            int msgsCount = profileWaitingMessages.Count;
                            int slotsCount = db_queuePostingTimes.Count;
                            //double tempWeekLoopValue = msgsCount / slotsCount;
                            //double weekLoopCount = Math.Ceiling(tempWeekLoopValue);

                            int weekLoop = 1;
                            int slotLoop = 0;
                            for (int msgLoop = 0; msgLoop < profileWaitingMessages.Count; msgLoop++)
                            {
                                if (slotLoop == slotsCount)
                                {
                                    slotLoop = 0;
                                    weekLoop++;
                                }
                                int ScheduledMessageId = profileWaitingMessages[msgLoop].ScheduledMessageId;
                                ScheduledMessages obj = new ScheduledMessages();
                                obj.Id = ScheduledMessageId;
                                var rec_ScheduledMessage = db.ScheduledMessages.Where(x => x.Id == ScheduledMessageId).FirstOrDefault();

                                obj.MessageText = HttpUtility.UrlDecode(rec_ScheduledMessage.MessageText);
                                obj.CreatedDate = rec_ScheduledMessage.CreatedDate.ToShortDateString();
                                obj.AuthorTeamMemberId = rec_ScheduledMessage.AuthorTeamMemberId;
                                obj.PublishDate = db_queuePostingTimes[slotLoop].PublishDate.AddDays(weekLoop * 7);
                                PostingDate = db_queuePostingTimes[slotLoop].PublishDate;

                                DateTime EstimatedTime = PostingDate.AddDays(weekLoop * 7);

                                obj.PostingDate = EstimatedTime.ToString("dd/MM/yyyy");
                                obj.PostingDay = GetDayStringByDayId((int)db_queuePostingTimes[slotLoop].PublishDate.AddDays(weekLoop * 7).DayOfWeek);

                                obj.MessageStatus = Convert.ToInt32(rec_ScheduledMessage.MessageStatus);

                                TimeSpan PublishTime = EstimatedTime.TimeOfDay;

                                string ampm = PublishTime.Hours >= 12 ? "PM" : "AM";
                                int HourVal = PublishTime.Hours % 12;
                                string Hour = "";
                                if (HourVal == 0)
                                {
                                    HourVal = 12;
                                    Hour = HourVal.ToString();
                                }
                                else
                                {
                                    Hour = PublishTime.Hours % 12 >= 10 ? HourVal.ToString() : "0" + HourVal.ToString();
                                }
                                string Minute = PublishTime.Minutes >= 10 ? PublishTime.Minutes.ToString() : "0" + PublishTime.Minutes.ToString();

                                obj.PostingTime = Hour + ":" + Minute + " " + ampm;

                                //Attaching Images to message
                                var db_images = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == obj.Id).ToList();
                                List<MediaModelTwitter> lstMediaInfo = new List<MediaModelTwitter>();
                                foreach (var item in db_images)
                                {
                                    lstMediaInfo.Add(new MediaModelTwitter
                                    {
                                        FileName = item.FileName,
                                        ContentType = item.ContentType,
                                        Base64String = Convert.ToBase64String(item.FileContent)
                                    });
                                }
                                obj.mediaInfo = lstMediaInfo;

                                //try
                                var db_profile = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == profileCompanyGroupId).FirstOrDefault();


                                //If Profile Id
                                if (db_profile.ProfileComapnyGroup.Profile != null)
                                {
                                    obj.ProfileTypeId = db_profile.ProfileComapnyGroup.Profile.ProfileTypeId;
                                    obj.Username = db_profile.ProfileComapnyGroup.Profile.UserName;
                                    obj.ProfileURL = db_profile.ProfileComapnyGroup.Profile.ProfilePic;
                                }
                                else //if Profile Page
                                {
                                    obj.ProfileTypeId = 1;
                                    obj.Username = db_profile.ProfileComapnyGroup.ProfilePage.PageName;
                                    obj.ProfileURL = db_profile.ProfileComapnyGroup.ProfilePage.ProfilePic;
                                }

                                var authorDetails = db.Teams.Where(x => x.Id == rec_ScheduledMessage.AuthorTeamMemberId).FirstOrDefault();
                                obj.AuthorName = authorDetails.FirstName + " " + authorDetails.LastName;

                                lstWaitingMessages.Add(obj);
                                slotLoop++;
                            }
                        }
                        //lstWaitingMessages = lstWaitingMessages.OrderByDescending(x => x.PostingDate).ThenBy(x => x.PostingTime).ToList();

                        lstWaitingMessages = lstWaitingMessages.OrderByDescending(x => x.PublishDate).ToList();

                        lstScheduledMessages.AddRange(lstWaitingMessages);

                        lstScheduledMessages = lstScheduledMessages.OrderByDescending(x => x.PublishDate).ToList();
                        //lstScheduledMessages = lstScheduledMessages.Sort()
                    }


                    //These are messages that are in WaitingMessages table but there is no queue slot available for them
                    //so we can't decide their timings - they were left in 'continue;' part
                    if (db_WaitingMessages.Count > 0 && model.MessageTypes.Contains(3))
                    {
                        List<ScheduledMessages> lstWaitingMessages = new List<ScheduledMessages>();

                        //For every selected profile
                        for (int profileLoop = 0; profileLoop < lst_QueuedMessageProfiles.Count; profileLoop++)
                        {
                            var db_queueSlots = db_ProfileQueueTimes.Where(x => x.QueuedMessageProfileId == lst_QueuedMessageProfiles[profileLoop]).ToList();

                            var profileWaitingMessages = db_WaitingMessages.Where(x => x.QueuedMessageProfileId == lst_QueuedMessageProfiles[profileLoop]).ToList();
                            //int profileCompanyGroupId2 = model.SelectedProfiles[profileLoop];
                            int profileCompanyGroupId2 = lst_QueuedProfileCompanyGroupIds[profileLoop];

                            int msgsCount = profileWaitingMessages.Count;

                            if (db_queueSlots.Count == 0)
                            {
                                for (int msgLoop = 0; msgLoop < profileWaitingMessages.Count; msgLoop++)
                                {
                                    int ScheduledMessageId = profileWaitingMessages[msgLoop].ScheduledMessageId;
                                    ScheduledMessages obj = new ScheduledMessages();
                                    obj.Id = ScheduledMessageId;
                                    var rec_ScheduledMessage = db.ScheduledMessages.Where(x => x.Id == ScheduledMessageId).FirstOrDefault();

                                    obj.MessageText = HttpUtility.UrlDecode(rec_ScheduledMessage.MessageText);
                                    obj.CreatedDate = rec_ScheduledMessage.CreatedDate.ToShortDateString();
                                    obj.AuthorTeamMemberId = rec_ScheduledMessage.AuthorTeamMemberId;

                                    obj.PostingDate = "";
                                    obj.MessageStatus = Convert.ToInt32(rec_ScheduledMessage.MessageStatus);
                                    obj.PostingTime = "";
                                    obj.PostingDay = "";

                                    //try
                                    var db_profile = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == profileCompanyGroupId2).FirstOrDefault();

                                    //If Profile Id
                                    if (db_profile.ProfileComapnyGroup.Profile != null)
                                    {
                                        obj.ProfileTypeId = db_profile.ProfileComapnyGroup.Profile.ProfileTypeId;
                                        obj.Username = db_profile.ProfileComapnyGroup.Profile.UserName;
                                        obj.ProfileURL = db_profile.ProfileComapnyGroup.Profile.ProfilePic;
                                    }
                                    else //if Profile Page
                                    {
                                        obj.ProfileTypeId = 1;
                                        obj.Username = db_profile.ProfileComapnyGroup.ProfilePage.PageName;
                                        obj.ProfileURL = db_profile.ProfileComapnyGroup.ProfilePage.ProfilePic;
                                    }

                                    var authorDetails = db.Teams.Where(x => x.Id == rec_ScheduledMessage.AuthorTeamMemberId).FirstOrDefault();
                                    obj.AuthorName = authorDetails.FirstName + " " + authorDetails.LastName;

                                    //Attaching Images to message
                                    var db_images = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == obj.Id).ToList();
                                    List<MediaModelTwitter> lstMediaInfo = new List<MediaModelTwitter>();
                                    foreach (var item in db_images)
                                    {
                                        lstMediaInfo.Add(new MediaModelTwitter
                                        {
                                            FileName = item.FileName,
                                            ContentType = item.ContentType,
                                            Base64String = Convert.ToBase64String(item.FileContent)
                                        });
                                    }
                                    obj.mediaInfo = lstMediaInfo;

                                    lstWaitingMessages.Add(obj);
                                }
                            }


                        }

                        lstWaitingMessages.Reverse();
                        lstScheduledMessages.AddRange(lstWaitingMessages);
                        //lstScheduledMessages = lstScheduledMessages.OrderByDescending(x => x.PublishDate).ToList();
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //I have sent -1 in case when I need all data
            if (pageNumber == -1)
                return lstScheduledMessages;

            //Pagination
            int fromRecord = (pageNumber - 1) * pageSize;
            int toRecord = (pageNumber * pageSize);

            if (getLatest == true)
            {
                fromRecord = 0;
            }

            if (fromRecord >= lstScheduledMessages.Count)
            {
                return null;
            }

            if (toRecord > lstScheduledMessages.Count)
            {
                toRecord = lstScheduledMessages.Count;
            }

            for (int loop = fromRecord; loop < toRecord; loop++)
            {
                lstScheduledMessagesToBeSend.Add(lstScheduledMessages[loop]);
            }
            //lstScheduledMessagesToBeSend.AddRange(lstScheduledMessages.Skip(fromRecord).Take(toRecord));            

            return lstScheduledMessagesToBeSend;
        }
        public async Task<ScheduledMessageModel> LoadPreviouslySavedOrScheduledMessage(int ScheduledMessageId)
        {
            ScheduledMessageModel ScheduledMessage = new ScheduledMessageModel();
            try
            {
                using (var db = new SocialCRMEntities())
                {
                    var db_ScheduledMessage = db.ScheduledMessages.Where(x => x.Id == ScheduledMessageId).FirstOrDefault();
                    var db_MediaFiles = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == ScheduledMessageId).ToList();

                    ScheduledMessage.ScheduledMessageId = db_ScheduledMessage.Id;
                    ScheduledMessage.MessageText = db_ScheduledMessage.MessageText;
                    ScheduledMessage.IsDraft = db_ScheduledMessage.IsDrafted;
                    string PostDateTime = "";
                    List<int> SelectedProfiles = new List<int>();
                    List<bool> SelectedProfilesType = new List<bool>();
                    List<string> PostTimes = new List<string>();

                    foreach (var obj in db_ScheduledMessage.ScheduledMessageProfiles)
                    {
                        SelectedProfiles.Add((int)obj.ProfileCompanyGroupId);

                        if (obj.ProfileComapnyGroup.ProfileId != null && obj.ProfileComapnyGroup.ProfilePageId == null)
                        {
                            SelectedProfilesType.Add(false);
                        }
                        else
                        {
                            SelectedProfilesType.Add(true);
                        }

                        DateTime PubDate = obj.MessagePostingTimes.FirstOrDefault().PublishDate.Date;
                        string PubTime = obj.MessagePostingTimes.FirstOrDefault().PublishTime.ToString();
                        PostDateTime = PubDate.ToString("MM/dd/yyyy") + "|" + PubTime.Substring(0, 5);
                    }

                    ScheduledMessage.SelectedProfiles = SelectedProfiles;
                    ScheduledMessage.SelectedProfilesType = SelectedProfilesType;

                    PostTimes.Add(PostDateTime);
                    ScheduledMessage.PostTimes = PostTimes;

                    //Populating Media Items
                    List<MediaModelTwitter> mediaInfo = new List<MediaModelTwitter>();
                    foreach (var item in db_MediaFiles)
                    {
                        mediaInfo.Add(new MediaModelTwitter
                        {
                            FileName = item.FileName,
                            ContentType = item.ContentType,
                            Base64String = Convert.ToBase64String(item.FileContent)
                        });
                    }
                    ScheduledMessage.mediaInfo = mediaInfo;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ScheduledMessage;
        }
        public bool SetTimingAlgorithmForWaitingMessages(int ProfileCompanyGroupId)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan t_span = currentDateTime.TimeOfDay;
            bool timingSet = false;
            ProfileQueueTime queueFirstRecord = null;
            int today = GetDayId();
            DateTime activityDateTime = DateTime.Now;
            int selectedProfileQueueTimeRecordId = 0;
            //Place for new messages
            List<QueuePostingTime> lst_QueuePostingTime = new List<QueuePostingTime>();

            using (var db = new SocialCRMEntities())
            {
                try
                {
                    //Get Max Number of Precedence
                    var tempPrece = db.QueuePostingTimes.Max(x => (int?)x.Precedence) ?? 0;
                    int receivedPrecedence = 0;
                    if (tempPrece != null)
                        receivedPrecedence = Convert.ToInt32(tempPrece);
                    var profileQueue = db.QueuedMessageProfiles.Where(x => x.ProfileCompanyGroupId == ProfileCompanyGroupId).FirstOrDefault();
                    var db_profileQueueTimes = db.ProfileQueueTimes.Where(x => x.QueuedMessageProfileId == profileQueue.Id && x.IsActive == false).OrderBy(x => x.Time).ToList();

                    //Look if we have any messages in Waiting State
                    var db_WaitingMessages = db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == profileQueue.Id).ToList();
                    if (db_WaitingMessages.Count > 0)
                    {
                        //loop on messages
                        foreach (var message in db_WaitingMessages)
                        {
                            //If there are no slots then break the loop
                            if (db_profileQueueTimes.Where(x => x.IsActive == false).ToList().Count <= 0)
                                break;

                            #region Timing Algorithm
                            timingSet = false;

                            //Present
                            #region Present                        

                            var profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId == today && x.IsActive == false).OrderBy(x => x.Time).ToList();
                            foreach (var queueTime in profileQueueTimes)
                            {
                                if (queueFirstRecord == null)
                                {
                                    queueFirstRecord = queueTime;
                                }
                                if (timingSet == false)
                                {
                                    //agar aaj k din ki he bat hai to time dkhen ge k karskte hen ya nae
                                    if (today == queueTime.DayId)
                                    {
                                        //Time abhi hai
                                        if (DateTime.Now.TimeOfDay < queueTime.Time)
                                        {
                                            TimeSpan ts = queueTime.Time;
                                            activityDateTime = activityDateTime.Date + ts;
                                            timingSet = true;
                                            selectedProfileQueueTimeRecordId = queueTime.Id;
                                        }

                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion
                            //Future
                            #region Furture
                            profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId > today && x.IsActive == false).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                            foreach (var queueTime in profileQueueTimes)
                            {
                                if (timingSet == false)
                                {
                                    if (today < queueTime.DayId)
                                    {
                                        int diffOfDays = queueTime.DayId - today;
                                        activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                        timingSet = true;
                                        selectedProfileQueueTimeRecordId = queueTime.Id;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion
                            //Past
                            #region Past
                            profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId < today && x.IsActive == false).OrderBy(x => x.DayId).ThenBy(x => x.Time).ToList();
                            foreach (var queueTime in profileQueueTimes)
                            {
                                if (timingSet == false)
                                {
                                    if (today > queueTime.DayId)
                                    {
                                        int diffOfDays = 7 - today + queueTime.DayId;
                                        activityDateTime = activityDateTime.AddDays(diffOfDays).Date + queueTime.Time;
                                        timingSet = true;
                                        selectedProfileQueueTimeRecordId = queueTime.Id;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion

                            profileQueueTimes = db_profileQueueTimes.Where(x => x.DayId == today && x.IsActive == false).OrderBy(x => x.Time).ToList();
                            //Again Present + 7 Days
                            if (timingSet == false && profileQueueTimes.Count > 0)
                            {
                                TimeSpan ts = queueFirstRecord.Time;
                                activityDateTime = activityDateTime.AddDays(7).Date + ts;
                                timingSet = true;
                                selectedProfileQueueTimeRecordId = queueFirstRecord.Id;
                            }
                            #endregion

                            //Ultimately timingSet is true if queuing slots are available                            
                            if (timingSet == true)
                            {
                                //Attaching schedule id to Posting time
                                var queuePostingTime = new QueuePostingTime();

                                queuePostingTime.PublishDate = activityDateTime;
                                queuePostingTime.IsPublished = false;
                                queuePostingTime.ProfileQueueTimeId = selectedProfileQueueTimeRecordId;
                                queuePostingTime.ScheduledMessageId = message.ScheduledMessageId;
                                queuePostingTime.ProfileCompanyGroupId = ProfileCompanyGroupId;

                                receivedPrecedence++;
                                queuePostingTime.Precedence = receivedPrecedence;

                                var recordToBeUpdated = db_profileQueueTimes.Where(x => x.Id == selectedProfileQueueTimeRecordId).FirstOrDefault();
                                recordToBeUpdated.IsActive = true;

                                //Remove from waiting
                                db.QueueWaitingMessages.Remove(message);

                                //Add to Queue
                                lst_QueuePostingTime.Add(queuePostingTime);
                            }

                            //In case needed
                            queueFirstRecord = null;
                            activityDateTime = DateTime.Now;
                        }
                    }
                    //Adding QueuePostingTimes to database
                    db.QueuePostingTimes.AddRange(lst_QueuePostingTime);
                    var resultset = db.SaveChanges();
                    return resultset > 0;

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
    }
}