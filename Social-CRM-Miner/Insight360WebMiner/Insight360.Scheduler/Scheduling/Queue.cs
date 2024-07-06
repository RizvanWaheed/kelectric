using Insight360.DAL;
using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class Queue : IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                try
                {
                    DateTime CurrentDateTime = DateTime.Now;
                    string CurrentDate = CurrentDateTime.ToShortDateString();
                    string CurrentTime = CurrentDateTime.ToShortTimeString();

                    DateTimeOffset? previousTime = context.Trigger.GetPreviousFireTimeUtc();
                    if (previousTime.HasValue)
                    {
                        var result = db.QueuePostingTimes.Where(x => DbFunctions.TruncateTime(x.PublishDate) == CurrentDateTime.Date && x.IsPublished == false).ToList();

                        for (int i = 0; i < result.Count; i++)
                        {
                            //If Time has Arrived for Scheduling Queued Message
                            if (result[i].PublishDate.Hour == CurrentDateTime.TimeOfDay.Hours
                            && result[i].PublishDate.Minute == CurrentDateTime.TimeOfDay.Minutes)
                            {
                                //Arranging Data
                                int PQTid = Convert.ToInt32(result[i].ProfileQueueTimeId);
                                var profileQueueTime = db.ProfileQueueTimes.Where(x => x.Id == PQTid).FirstOrDefault();
                                int SMid = Convert.ToInt32(result[i].ScheduledMessageId);
                                var scheduledMessage = db.ScheduledMessages.Where(x => x.Id == SMid).FirstOrDefault();

                                //Method call to schedule post
                                string status = System.Web.HttpUtility.UrlDecode(scheduledMessage.MessageText);                                
                                string siteURL = System.Configuration.ConfigurationManager.AppSettings["SiteURL"];
                                string platformURL = "";

                                //For profile (One to One asscociation)
                                int profileCompanyGroupId = profileQueueTime.QueuedMessageProfile.ProfileCompanyGroupId;
                                var QueuedMessageProfile = profileQueueTime.QueuedMessageProfile;

                                //Facebook Profile or page?
                                bool isPage = false;
                                if (QueuedMessageProfile.ProfileComapnyGroup.Profile == null)
                                {
                                    isPage = true;
                                }

                                string userId, userExternalId, profileType;

                                if (!isPage)
                                {
                                    userId = QueuedMessageProfile.ProfileComapnyGroup.Profile.UserName;
                                    userExternalId = QueuedMessageProfile.ProfileComapnyGroup.Profile.ProfileExternalId;
                                    profileType = QueuedMessageProfile.ProfileComapnyGroup.Profile.ProfileType.Name;
                                    //result[i].ScheduledMessageProfile.ProfileComapnyGroup.Profile.ProfileType.Name;
                                }
                                else
                                {
                                    userId = QueuedMessageProfile.ProfileComapnyGroup.ProfilePage.PageName;
                                    userExternalId = QueuedMessageProfile.ProfileComapnyGroup.ProfilePage.ProfilePageID;
                                    profileType = "Facebook";
                                }

                                //Platform
                                if (profileType == "Twitter")
                                {
                                    platformURL = "/api/Twitter/TwitterNewTweetMiner"; // /" + userId + "/" + status;
                                }
                                else if (profileType == "Facebook")
                                {
                                    platformURL = "/Facebook/FacebookNewPost?userId=" + userExternalId + "&isPage=" + isPage + "&status=" + status + "&filePath=" + "null";
                                }

                                //Media
                                int scheduleMessageId = result[i].ScheduledMessageId;
                                var db_Images = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == scheduleMessageId).ToList();
                                string filePath = "";
                                string contentType = "";
                                string fileName = "";
                                StringBuilder strbuilder = new StringBuilder();

                                if (db_Images.Count > 0)
                                {
                                    if (profileType == "Facebook")
                                    {
                                        filePath = Convert.ToBase64String(db_Images[0].FileContent);
                                        contentType = db_Images[0].ContentType;
                                        fileName = db_Images[0].FileName;
                                    }
                                    else if (profileType == "Twitter")
                                    {
                                        int loop = 0;
                                        foreach (var item in db_Images)
                                        {
                                            strbuilder.Append(item.FileName);
                                            strbuilder.Append("," + item.ContentType);
                                            strbuilder.Append("," + Convert.ToBase64String(item.FileContent));
                                            if (loop < db_Images.Count - 1)
                                                strbuilder.Append("|");
                                            loop++;
                                        }
                                    }
                                }

                                if (platformURL != "")
                                {
                                    using (var client = new WebClient())
                                    {
                                        var values = new NameValueCollection();
                                        if (profileType == "Twitter")
                                        {
                                            values.Add("fromUser", userId);
                                            values.Add("status", status);
                                            values.Add("mediaString", strbuilder.ToString());
                                        }
                                        else if (profileType == "Facebook")
                                        {
                                            values.Add("filePath", filePath);
                                            values.Add("contentType", contentType);
                                            values.Add("fileName", fileName);
                                        }
                                        var response = client.UploadValues(siteURL + platformURL, values);
                                        var responseString = Encoding.Default.GetString(response);
                                        result[i].IsPublished = true;

                                        //Check for Waiting Messages in QueueWaitingMessages Table and get the lowest Id one (FIFO)
                                        var db_WaitingMessage = db.QueueWaitingMessages.Where(x => x.QueuedMessageProfileId == profileQueueTime.QueuedMessageProfileId).OrderBy(x=>x.Id).FirstOrDefault();
                                        if(db_WaitingMessage != null)
                                        {
                                            //New Date
                                            DateTime temp_PublishDate = result[i].PublishDate.AddDays(7);                                            
                                            //Get Max Number of Precedence
                                            int tempPrece = db.QueuePostingTimes.Max(x => (int?)x.Precedence) ?? 0;

                                            QueuePostingTime QPTobj = new QueuePostingTime();
                                            QPTobj.IsPublished = false;
                                            QPTobj.Precedence = ++tempPrece;
                                            //result[i].Precedence = tempPrece;
                                            //result[i].ScheduledMessageId = db_WaitingMessage.ScheduledMessageId;
                                            //result[i].PublishDate = temp_PublishDate;
                                            QPTobj.ProfileCompanyGroupId = result[i].ProfileCompanyGroupId;
                                            QPTobj.ProfileQueueTimeId = result[i].ProfileQueueTimeId;
                                            QPTobj.PublishDate = temp_PublishDate;
                                            QPTobj.ScheduledMessageId = db_WaitingMessage.ScheduledMessageId;
                                            db.QueuePostingTimes.Add(QPTobj);
                                            db.QueueWaitingMessages.Remove(db_WaitingMessage);
                                        }
                                        else
                                        {
                                            profileQueueTime.IsActive = false;
                                        }

                                        scheduledMessage.MessageStatus = (int)Util.MessageStatus.Sent;
                                        db.SaveChanges();
                                    }
                                }

                            }
                        }

                    }

                }//tryend
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


    }
}