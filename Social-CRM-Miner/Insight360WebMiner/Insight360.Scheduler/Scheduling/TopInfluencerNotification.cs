using Insight360.Common.BO;
using Insight360.Common.Util;
using Insight360.DAL;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Insight360.Scheduler.Scheduling
{
    [DisallowConcurrentExecution]
    public class TopInfluencerNotification: IJob
    {
        readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            using (var db = new SocialCRMMinningEntities())
            {
                Util.Log.WriteOperationStarted(typeof(TopInfluencerNotification), "TopInfluencerNotification - SendNotificationEmails", "Thread Enter");
                try
                {
                    List<string> emailList = new List<string>();
                    var emailIdList = db.SocialMediaUsersUpdateReceivers.ToList();
                    //bool is10kFollower = false, isPoliticalFigure = false, isVerfiedAcc = false, isTopInfluencer = false;
                    FeedDetail feedModel = new FeedDetail();
                    var notificationList = db.SocialMediaUpdates.ToList().Where(x => !x.EmailSent).ToList();

                    //Change Request
                    string emailSubject = "";
                    //E: @abc DM
                    //E: @abc Tweet

                    foreach (var feed in notificationList)
                    {
                        emailList.Clear();
                        feedModel = new FeedDetail();
                        emailList = emailIdList.Where(x => (
                                                            (x.Over10KFollowers == true && feed.Over10KFollowers == true) ||
                                                            (x.PoliticalFigures == true && feed.PoliticalFigures == true) ||
                                                            (x.VerifiedAccounts == true && feed.VerifiedAccounts == true) ||
                                                            (x.TopInfluencers == true && feed.TopInfluencers == true)
                                                            ) &&
                                                            x.CompanyId == feed.CompanyId).Select(y => y.Email).Distinct().ToList<string>();
                       switch (feed.FeedType)
                        {
                            case "TP":
                                //var twPostInfo = db.PostsTwitters.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                                var twPostInfo = (from p in db.PostsTwitters
                                                  where p.Id == feed.FeedId                                                     
                                                  join ma in db.FeedsMediaAttachments.Where(x => x.MediaType == "TP")
                                                  on p.Id equals ma.MediaId into gj
                                                  from ma in gj.DefaultIfEmpty()
                                                  select new FeedDetail
                                                  {
                                                      Name = p.AuthorName.ToString(),
                                                      StatusId = p.StatusId.ToString(),
                                                      Text = p.StatusText.ToString(),
                                                      AuthorProfileImage = p.AuthorProfilePic.ToString(),
                                                      AuthorFollowers = p.AuthorFollowers.ToString(),
                                                      CreatedDate = p.CreatedDate,
                                                      PostImageURL = ma.AttachmentUrl
                                                    }).FirstOrDefault();

                                if (twPostInfo != null)
                                {
                                    feedModel.Name = twPostInfo.Name;
                                    feedModel.ProfileUrl = string.Format("https://twitter.com/{0}", twPostInfo.Name);
                                    feedModel.FeedUrl = string.Format("https://twitter.com/{0}/status/{1}", twPostInfo.Name, twPostInfo.StatusId);
                                    feedModel.Text = twPostInfo.Text;
                                    feedModel.AuthorProfileImage = twPostInfo.AuthorProfileImage;
                                    feedModel.AuthorFollowers = twPostInfo.AuthorFollowers;
                                    feedModel.CreatedDate = twPostInfo.CreatedDate;
                                    feedModel.PostImageURL = twPostInfo.PostImageURL;                                    
                                }

                                emailSubject = "E: @" + feedModel.Name + " Tweet";
                                break;
                            case "TM":
                                //var twMsgInfo = db.MessagesTwitters.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                                var twMsgInfo = (from p in db.MessagesTwitters
                                                 where p.Id == feed.FeedId
                                                 join ma in db.FeedsMediaAttachments.Where(x => x.MediaType == "TM")
                                                 on p.Id equals ma.MediaId into gj
                                                 from ma in gj.DefaultIfEmpty()
                                                 select new FeedDetail
                                                 {
                                                     Name = p.SenderScreenName.ToString(),
                                                     Text = p.MsgText.ToString(),
                                                     AuthorProfileImage = p.SenderProfilePic.ToString(),
                                                     AuthorFollowers = p.SenderFollowers.ToString(),
                                                     CreatedDate = p.CreatedDate,
                                                     PostImageURL = ma.AttachmentUrl,
                                                     AuthorUserId = p.SenderId,
                                                     ReceiverId = p.RecipientId
                                                 }).FirstOrDefault();

                                if (twMsgInfo != null)
                                {
                                    feedModel.Name = twMsgInfo.Name;
                                    feedModel.ProfileUrl = string.Format("https://twitter.com/{0}", twMsgInfo.Name);
                                    feedModel.FeedUrl = string.Format("https://twitter.com/messages/{0}-{1}", twMsgInfo.AuthorUserId, twMsgInfo.ReceiverId);
                                    feedModel.Text = twMsgInfo.Text;
                                    feedModel.AuthorProfileImage = twMsgInfo.AuthorProfileImage;
                                    feedModel.AuthorFollowers = twMsgInfo.AuthorFollowers;
                                    feedModel.CreatedDate = Convert.ToDateTime(twMsgInfo.CreatedDate);
                                }
                                emailSubject = "E: @" + feedModel.Name + " DM";
                                break;
                            case "FP":
                                //var fbPostInfo = db.PostsFBs.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                                var fbPostInfo = (from p in db.PostsFBs
                                                  where p.Id == feed.FeedId
                                                  join ma in db.FeedsMediaAttachments.Where(x => x.MediaType == "FP")
                                                  on p.Id equals ma.MediaId into gj
                                                  from ma in gj.DefaultIfEmpty()
                                                  select new FeedDetail
                                                  {
                                                      Name = p.AuthorUserName.ToString(),
                                                      Text = p.Message.ToString(),
                                                      AuthorProfileImage = p.AuthorProfilePic.ToString(),
                                                      AuthorUserId = p.AuthorUserId.ToString(),
                                                      CreatedDate = p.CreatedDate,
                                                      StatusId = p.PostId.ToString(),
                                                      PostImageURL = ma.AttachmentUrl
                                                  }).FirstOrDefault();

                                if (fbPostInfo != null)
                                {
                                    feedModel.Name = fbPostInfo.Name;
                                    feedModel.ProfileUrl = string.Format("https://www.facebook.com/{0}", fbPostInfo.AuthorUserId);
                                    feedModel.FeedUrl = string.Format("https://www.facebook.com/{0}", fbPostInfo.StatusId);
                                    feedModel.Text = fbPostInfo.Text;
                                    feedModel.AuthorProfileImage = fbPostInfo.AuthorProfileImage;
                                    feedModel.AuthorFollowers = "0";
                                    feedModel.CreatedDate = Convert.ToDateTime(fbPostInfo.CreatedDate);
                                }
                                emailSubject = "E: " + feedModel.Name + " Facebook Post";
                                break;
                            case "FC":
                                //var fbCommentInfo = db.CommentsFBs.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                                var fbCommentInfo = (from p in db.CommentsFBs
                                                     where p.Id == feed.FeedId
                                                     join ma in db.FeedsMediaAttachments.Where(x => x.MediaType == "FC")
                                                     on p.Id equals ma.MediaId into gj
                                                     from ma in gj.DefaultIfEmpty()
                                                     select new FeedDetail
                                                     {
                                                         Name = p.FromName.ToString(),
                                                         Text = p.Message.ToString(),
                                                         AuthorProfileImage = p.FromUserProfilePic.ToString(),
                                                         AuthorUserId = p.FromId.ToString(),
                                                         CreatedDate = p.CreatedDate,
                                                         StatusId = p.PostId.ToString(),
                                                         PostImageURL = ma.AttachmentUrl
                                                     }).FirstOrDefault();
                                if (fbCommentInfo != null)
                                {
                                    feedModel.Name = fbCommentInfo.Name;
                                    feedModel.ProfileUrl = string.Format("https://www.facebook.com/{0}", fbCommentInfo.AuthorUserId);
                                    feedModel.FeedUrl = string.Format("https://www.facebook.com/{0}", fbCommentInfo.StatusId);
                                    feedModel.Text = fbCommentInfo.Text;
                                    feedModel.AuthorProfileImage = fbCommentInfo.AuthorProfileImage;
                                    feedModel.AuthorFollowers = "0";
                                    feedModel.CreatedDate = Convert.ToDateTime(fbCommentInfo.CreatedDate);
                                }
                                emailSubject = "E: " + feedModel.Name + " Facebook Comment";
                                break;
                            case "FCP":
                                //var fbChatInfo = db.PageChatsFBs.Where(x => x.Id == feed.FeedId).FirstOrDefault();
                                var fbChatInfo = (from p in db.PageChatsFBs
                                                  where p.Id == feed.FeedId
                                                  join ma in db.FeedsMediaAttachments.Where(x => x.MediaType == "PCF")
                                                  on p.Id equals ma.MediaId into gj
                                                  from ma in gj.DefaultIfEmpty()
                                                  select new FeedDetail
                                                  {
                                                      Name = p.FromName.ToString(),
                                                      Text = p.MsgText.ToString(),
                                                      AuthorProfileImage = p.FromProfilePic.ToString(),
                                                      AuthorUserId = p.FromId.ToString(),
                                                      CreatedDate = p.CreatedDate,
                                                      ReceiverId = p.ToId.ToString(),
                                                      PostImageURL = ma.AttachmentUrl
                                                  }).FirstOrDefault();
                                if (fbChatInfo != null)
                                {
                                    feedModel.Name = fbChatInfo.Name;
                                    feedModel.ProfileUrl = string.Format("https://www.facebook.com/{0}", fbChatInfo.AuthorUserId);
                                    feedModel.FeedUrl = "";//string.Format("https://www.facebook.com/{0}/messages", fbChatInfo.ReceiverId);
                                    feedModel.Text = fbChatInfo.Text;
                                    feedModel.AuthorProfileImage = fbChatInfo.AuthorProfileImage;
                                    feedModel.AuthorFollowers = "0";
                                    feedModel.CreatedDate = Convert.ToDateTime(fbChatInfo.CreatedDate);
                                }
                                emailSubject = "E: " + feedModel.Name + " Facebook Chat";
                                break;
                            default:
                                break;
                        }
                        StringBuilder builder = new StringBuilder();

                        //Implement Email Template here
                        StringBuilder HeaderTemplate = new StringBuilder(File.ReadAllText(HttpRuntime.AppDomainAppPath + "/Views/EmailTemplates/HeaderEmailAlertsTemplate.html"));
                        StringBuilder BodyTemplate = new StringBuilder(File.ReadAllText(HttpRuntime.AppDomainAppPath + "/Views/EmailTemplates/BodyEmailAlertsTemplate.html"));
                        StringBuilder processEmailBody = new StringBuilder(BodyTemplate.ToString());

                        processEmailBody.Replace("#Icon", ""); //showing social meida icon is not working
                        processEmailBody.Replace("#AuthorProfileImage", feedModel.AuthorProfileImage.ToString().Trim());
                        processEmailBody.Replace("#AuthorName", feedModel.Name.ToString().Trim());
                        processEmailBody.Replace("#AuthorNameInSmall", feedModel.Name.ToString().Trim());
                        processEmailBody.Replace("#PostedOn", Convert.ToDateTime(feedModel.CreatedDate).ToString().Trim());
                        processEmailBody.Replace("#AuthorFollowers", feedModel.AuthorFollowers);
                        //processEmailBody.Replace("#AuthorFollowing", "0");
                        processEmailBody.Replace("#AuthorProfileUri", feedModel.ProfileUrl);
                        if (feedModel.PostImageURL != null){
                            feedModel.Text += "<br /><img src='" + feedModel.PostImageURL + "' alt='Image' style='max-width: 80%;height: auto;'/>";
                        }
                        processEmailBody.Replace("#PostText", feedModel.Text);  //ConvertUrlsToLinks(row["Text"].ToString().Trim().Replace(Term, "<b style='color:#3883a7'>" + Term + "</b>"))
                        processEmailBody.Replace("#PostLink", feedModel.FeedUrl);

                        HeaderTemplate.Replace("#EmailAlertsBody", processEmailBody.ToString());

                        //ends

                        //builder.Append("<div id='parent'>");
                        //builder.Append(string.Format("<a href='{0}' target='_blank'>{1}</a><br>",feedModel.ProfileUrl,feedModel.Name));
                        //builder.Append(string.Format("<span>{0}</span> <br>", feedModel.Text));
                        //builder.Append(string.Format("<a href='{0}' target='_blank'>Click here to view</a><br></div>", feedModel.FeedUrl));
                        //string emailTemplate = builder.ToString();
                        
                        bool isEmailSent = EmailSender.SendEmail(emailList, null, emailSubject, HeaderTemplate.ToString(), null);
                        if (isEmailSent)
                        {
                            feed.EmailSent = isEmailSent;
                            db.SocialMediaUpdates.Attach(feed);
                            db.Entry(feed).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.Log.WriteException(typeof(TopInfluencerNotification), "TopInfluencerNotification - SendNotificationEmails", ex);
                }
                finally
                {
                    Util.Log.WriteOperationStarted(typeof(TopInfluencerNotification), "TopInfluencerNotification - SendNotificationEmails", "Thread Exit");
                }
            }
        }


    }
}