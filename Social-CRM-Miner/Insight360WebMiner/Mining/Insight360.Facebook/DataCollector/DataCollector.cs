using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Globalization;
using Insight360.DAL;
using Insight360.BO;
using Insight360.Common;
using Facebook;
using System.Data.SqlClient;
using Insight360.Sources.Facebook;
using Insight360.Common.Util;
using System.IO;
using System.Web;

namespace Insight360.Facebook
{
    public class DataCollector
    {
        public void FetchFBCommentsThread(FacebookUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesCommentsThread", "Thread Enter");
            try
            {
                fetchFBCommentsAndPersist(account.AccessToken, account.UserId, account.PageId);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Facebook - FetchFBPagesCommentsThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesCommentsThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        public void FetchFBNestedCommentsThread(FacebookUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesNestedCommentsThread", "Thread Enter");
            try
            {
                fetchFBNestedCommentsAndPersist(account.AccessToken, account.UserId, account.PageId);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Facebook - FetchFBPagesNestedCommentsThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesNestedCommentsThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        public void FetchFBPostsThread(FacebookUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesPostsThread", "Thread Enter");
            try
            {
                fetchFBPostsAndPersist(account);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Facebook - FetchFBPagesPostsThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesPostsThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        public void FetchFBPagesChatsThread(FacebookUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesChatsThread", "Thread Enter");
            try
            {
                fetchFBPageChatsAndPersist(account);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Facebook - FetchFBPagesChatsThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - FetchFBPagesChatsThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        public void CalculateFBPostGradesThread(FacebookUserAccount account)
        {
            Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - CalculateFBPostGradesThread", "Thread Enter");
            try
            {
                calculateFBPostGradesAndPersist(account.AccessToken, account.PageId, account.CompanyId);
            }
            catch (Exception ex)
            {
                //Util.Log.WriteException(ex);
                Util.Log.WriteException(typeof(DataCollector), "Facebook - CalculateFBPostGradesThread", ex);
            }
            finally
            {
                Util.Log.WriteOperationStarted(typeof(DataCollector), "Facebook - CalculateFBPostGradesThread", "Thread Exit");
                //Util.Log.WriteOperationStarted("Thread Exit");
            }
        }

        void fetchFBCommentsAndPersist(string accessToken, string userId, string pageId)
        {
            List<CommentRelationInfo> commRels = FacebookDBOperations.GetCommentRelationInfo(userId, pageId);
            foreach(CommentRelationInfo commRel in commRels)
            {
                string postId = commRel.PostId;
                string lastFetchedCommentId = commRel.MaxCommentId;

                List<FBComment> currentPostComments = DataCollection.GetFBComments(accessToken, ref lastFetchedCommentId, postId);

                if(currentPostComments.Count > 0)
                {
                    FacebookDBOperations.InsertComments(currentPostComments, postId);
                    FacebookDBOperations.updateCommentStatus(lastFetchedCommentId, userId, pageId, postId);
                }
            }
                    
        }

        void fetchFBNestedCommentsAndPersist(string accessToken, string userId, string pageId)
        {
            List<CommentRelationInfo> commRels = FacebookDBOperations.GetNestedCommentRelationInfo(userId, pageId);
            foreach (CommentRelationInfo commRel in commRels)
            {
                string commentId = commRel.PostId;
                string lastFetchedCommentId = commRel.MaxCommentId;

                List<FBComment> currentPostComments = DataCollection.GetFBComments(accessToken, ref lastFetchedCommentId, commentId);

                if (currentPostComments.Count > 0)
                {
                    FacebookDBOperations.InsertComments(currentPostComments, commentId);
                    FacebookDBOperations.updateNestedCommentStatus(lastFetchedCommentId, commentId);
                }
            }
        }

        void fetchFBPostsAndPersist(FacebookUserAccount account)
        {
            DateTime? lastFetchedCreationTime = FacebookDBOperations.GetLastPostChatInfo(account.UserId, account.PageId).MaxPostCreationDate;
            List<FBPost> currentPosts;
            if (account.PageId == null)
            {
                currentPosts = DataCollection.GetFBFeeds(account.AccessToken, ref lastFetchedCreationTime, account.UserId);
            }
            else
            {
                currentPosts = DataCollection.GetFBFeeds(account.AccessToken, ref lastFetchedCreationTime, account.PageId);
            }

            if (currentPosts.Count > 0) {
                FacebookDBOperations.InsertFeeds(currentPosts, account.UserId, account.PageId, account.ProfileId, account.ProfilePageId);
                FacebookDBOperations.updatePostStatus(lastFetchedCreationTime, account.UserId, account.PageId);
            }
        }

        void calculateFBPostGradesAndPersist(string accessToken, string pageId, int companyId)
        {
            List<FBPost> postsToGrade = FacebookDBOperations.GetPostsWithoutGrades(pageId);
            List<string> emailIds = FacebookDBOperations.GetEmailIdsForCompany(companyId);

            foreach (FBPost post in postsToGrade)
            {
                post.EngagementRate = (int)DataCollection.GetEngagementRateByPostId(accessToken, post.PostId);
                List<FBPost> oldPostGrades = FacebookDBOperations.GetPreviousSixMonthsPostGradesFromPost(post);
                int postsAbove = oldPostGrades.Where(x => x.PostType == post.PostType && x.EngagementRate > post.EngagementRate).Count();
                int postsBelow = oldPostGrades.Where(x => x.PostType == post.PostType && x.EngagementRate < post.EngagementRate).Count();
                int postEqual = oldPostGrades.Where(x => x.PostType == post.PostType && x.EngagementRate == post.EngagementRate).Count();
                int total = postsBelow + postEqual + postsAbove;

                //Equivalent Avg
                //double ranking = ((double)(postsBelow + (double)((postEqual/2))) / (total + 1)) * 100;

                //Weighted Avg
                double ranking = ((double)(postsBelow + (double)((postEqual * post.EngagementRate)/(total+1))) / (total + 1)) * 100;
                string Grade;
                if (postsAbove == 0)
                    Grade = "A+";
                else if (ranking >= 90)
                    Grade = "A";
                else if (ranking >= 75)
                    Grade = "B";
                else if (ranking >= 50)
                    Grade = "C";
                else
                    Grade = "D";
                post.Grade = Grade;

                //CR: Send every email
                //if(Grade != "A+")
                //{
                StringBuilder builder = new StringBuilder();

                //New Implementation
                StringBuilder HeaderTemplate = new StringBuilder(File.ReadAllText(HttpRuntime.AppDomainAppPath + "/Views/EmailTemplates/HeaderEmailAlertsTemplate.html"));
                StringBuilder BodyTemplate = new StringBuilder(File.ReadAllText(HttpRuntime.AppDomainAppPath + "/Views/EmailTemplates/BodyEmailAlertsTemplate.html"));
                StringBuilder processEmailBody = new StringBuilder(BodyTemplate.ToString());

                processEmailBody.Replace("#Icon", ""); //showing social meida icon is not working
                processEmailBody.Replace("#AuthorProfileImage", post.AuthorProfilePic.ToString().Trim());
                processEmailBody.Replace("#AuthorName", post.AuthorUserName.ToString().Trim());
                processEmailBody.Replace("#AuthorNameInSmall", post.AuthorUserName.ToString().Trim());
                processEmailBody.Replace("#PostedOn", Convert.ToDateTime(post.CreatedDate).ToString().Trim());
                processEmailBody.Replace("#AuthorFollowers", "0");
                //processEmailBody.Replace("#AuthorFollowing", "0");                   
                //processEmailBody.Replace("#AuthorProfileUriText", !string.IsNullOrEmpty(row["AuthorLink"].ToString()) ? row["AuthorLink"].ToString().Trim() : "N/A");
                processEmailBody.Replace("#AuthorProfileUri", "https://www.facebook.com/" + post.AuthorUserId.ToString().Trim());

                if (post.PostImageURL != null)
                {
                    post.Message += "<br /><img src='" + post.PostImageURL + "' alt='Image' style='max-width: 100%;height: auto;' />";
                }

                processEmailBody.Replace("#PostText", post.Message);  //ConvertUrlsToLinks(row["Text"].ToString().Trim().Replace(Term, "<b style='color:#3883a7'>" + Term + "</b>"))
                processEmailBody.Replace("#PostLink", "https://www.facebook.com/" + post.PageId.ToString().Trim() + "/posts/" + post.PostId.Split('_')[1].ToString());
                //processEmailBody.Replace("#CommentStats", row["type"].ToString() == "Comment" ? " | <span style='color:#15F015'> " + row["pos_comments"].ToString() + "</span> Comment(s) | <span style='color:#F40000'>" + row["neg_comments"].ToString() + "</span> Comment(s) " + (row["hidden_comments"].ToString() == "0" ? "" : "[ including <span style='color:#F40000'>" + row["hidden_comments"].ToString() + "</span> Hidden Comments ]") + " | <span style='color:#999'>" + row["neu_comments"].ToString() + "</span> Comment(s)" : "");

                HeaderTemplate.Replace("#EmailAlertsBody", processEmailBody.ToString());
                EmailSender.SendEmail(emailIds, null, "E: Content Categorization – Grade " + Grade, HeaderTemplate.ToString(), null);
                //ends

                //builder.Append("<div id='parent'>");
                //builder.Append(string.Format("<a href='{0}' target='_blank'>{1}</a><br>", "https://www.facebook.com/" + pageId, post.AuthorUserName));
                //builder.Append(string.Format("<span>{0}</span> <br>", post.Message));
                //builder.Append(string.Format("<a href='{0}' target='_blank'>Click here to view</a><br></div>", "https://www.facebook.com/" + post.PostId));
                //string emailTemplate = builder.ToString();
                //EmailSender.SendEmail(emailIds, null, "Promote Content - Grade " + Grade, emailTemplate, null);
                //}
                FacebookDBOperations.updatePostGrading(post);
            }
        }

        void fetchFBPageChatsAndPersist(FacebookUserAccount account)
        {
            List<ConvMaxDates> conMsgMaxDates = FacebookDBOperations.GetConvMaxDates();

            DateTime? lastFetchedUpdationTime = FacebookDBOperations.GetLastPostChatInfo(account.UserId, account.PageId).MaxConUpdatedTime;

            List<FBPageChat> currentChats;
            currentChats = DataCollection.GetFBPageChats(account.AccessToken, ref lastFetchedUpdationTime, account.PageId.ToString(), conMsgMaxDates);

            if (currentChats.Count > 0) {
                FacebookDBOperations.InsertPageChats(currentChats, account.UserId, account.PageId, account.ProfilePic, account.ProfileId, account.ProfilePageId);
                FacebookDBOperations.updateChatStatus(lastFetchedUpdationTime, account.UserId, account.PageId);
            }
        }
    }
}
