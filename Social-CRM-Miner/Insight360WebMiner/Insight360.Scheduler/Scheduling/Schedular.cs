using Insight360.DAL;
using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class Schedular : IJob
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
                        var result = db.MessagePostingTimes.Where(x => x.PublishDate == CurrentDateTime.Date && x.IsPublished == false).ToList();
                        for (int i = 0; i < result.Count; i++)
                        {
                            if(result[i].ScheduledMessageProfile.ScheduledMessage.IsDrafted == true)
                            {
                                continue;
                            }

                            if(result[i].ScheduledMessageProfile.ScheduledMessage.MessageStatus == (int)Util.MessageStatus.Draft
                                || result[i].ScheduledMessageProfile.ScheduledMessage.MessageStatus == (int)Util.MessageStatus.Scheduled
                                || result[i].ScheduledMessageProfile.ScheduledMessage.MessageStatus == (int)Util.MessageStatus.Reject)
                            {
                                continue;
                            }

                            if (result[i].PublishTime.Hours == CurrentDateTime.TimeOfDay.Hours
                            && result[i].PublishTime.Minutes == CurrentDateTime.TimeOfDay.Minutes)
                            {
                                //Method call to schedule post
                                string status = System.Web.HttpUtility.UrlDecode(result[i].ScheduledMessageProfile.ScheduledMessage.MessageText);
                                string siteURL = System.Configuration.ConfigurationManager.AppSettings["SiteURL"];
                                string platformURL = "";

                                //Foreach profile
                                foreach (var profileObj in result[i].ScheduledMessageProfile.ScheduledMessage.ScheduledMessageProfiles)
                                {
                                    //FacebookProfile or Page
                                    bool isPage = false;

                                    if (profileObj.ProfileComapnyGroup.Profile == null)
                                    {
                                        isPage = true;
                                    }

                                    string userId, userExternalId, profileType;

                                    if (!isPage)
                                    {
                                        userId = profileObj.ProfileComapnyGroup.Profile.UserName;
                                        userExternalId = profileObj.ProfileComapnyGroup.Profile.ProfileExternalId;
                                        profileType = profileObj.ProfileComapnyGroup.Profile.ProfileType.Name;
                                        //result[i].ScheduledMessageProfile.ProfileComapnyGroup.Profile.ProfileType.Name;
                                    }
                                    else
                                    {
                                        userId = profileObj.ProfileComapnyGroup.ProfilePage.PageName;
                                        userExternalId = profileObj.ProfileComapnyGroup.ProfilePage.ProfilePageID;
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
                                    int scheduleMessageId = result[i].ScheduledMessageProfile.ScheduledMessage.Id;
                                    var db_Images = db.PublisherUploadedFiles.Where(x => x.ScheduledMessageId == scheduleMessageId).ToList();
                                    string filePath = "";
                                    string contentType = "";
                                    string fileName = "";

                                    //List<MediaModelTwitter> lstMediaModel = new List<MediaModelTwitter>();
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
                                                if(loop < db_Images.Count-1)
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
                                            //var query = db.MessagePostingTimes.Where(x => x.Id == result[i].Id).FirstOrDefault();
                                            //if(query != null) {
                                            result[i].IsPublished = true;
                                            result[i].ScheduledMessageProfile.ScheduledMessage.MessageStatus = (int)Util.MessageStatus.Sent;
                                            db.SaveChanges();
                                            //}
                                        }
                                    }
                                }

                                //string userId = result[i].ScheduledMessageProfile.ProfileComapnyGroup.Profile.UserName;
                                //string userExternalId = result[i].ScheduledMessageProfile.ProfileComapnyGroup.Profile.ProfileExternalId;                                
                            }
                        }
                    }
                    else
                    {
                        //db.ExtractSocialProfilesCustomers(null);
                    }
                }
                catch (Exception ex)
                { }
            }
        }


    }
    public class MediaModelTwitter
    {
        public string Base64String { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}