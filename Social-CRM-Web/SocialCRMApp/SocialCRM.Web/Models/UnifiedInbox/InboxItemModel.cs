using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedInbox
{
    public class InboxItemModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ClassName { get; set; }
        public string TwtImg { get; set; }
        public string User { get; set; }
        public string UserTwit { get; set; }
        public string ReplyedBy { get; set; }
        public string ReplyedByFullName { get; set; }
        public string ReplyedByUserId { get; set; }
        public string ToUser { get; set; }
        public string ToUserFullName { get; set; }
        public string ToUserId { get; set; }
        public string ParentId { get; set; }
        public string ReplyedDate { get; set; }
        public string ReplyedDateForDeck { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ActivityType { get; set; }
        public string AccountTypeImg { get; set; }
        public string MessageType { get; set; }
        public string StatusId { get; set; }
        public string ThreadId { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPage { get; set; }
        public int IsTaskCreated { get; set; }
        public bool IsTaskCompleted { get; set; }
        public bool? IsVerified { get; set; }
        public bool IsRetweet { get; set; }
        public bool IsLikedByAdmin { get; set; }
        public List<string> Tags { get; set; }
        public List<string> TagIds { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
        public List<string> Urls { get; set; }
        public bool IsVideo { get; set; }
        public string Sentiment { get; set; }
        public bool IsWarned { get; set; }
        public string CompletedBy { get; set; }
        public string CompletedDate { get; set; }
        public bool? IsCompleted { get; set; }
        public string Images { get; set; }
        public string ImageType { get; set; }
    }

    public static class UnifiedInboxUtility
    {
        public static void CheckForAttachments(ref InboxItemModel item)
        {
            Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex VimeoVideoRegex = new Regex(@"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex DMVideoRegex = new Regex(@"(dailymotion\.com)/video/([a-zA-Z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            item.Attachments = new List<AttachmentModel>();
            item.IsVideo = false;
            /*item.IsWarned = false;
            
            using (SocialCRMEntities db = new SocialCRMEntities())
            {
                var profiles = db.Profiles.ToList();
                var pages = db.ProfilePages.ToList();
                var customers = db.CustomerSocialProfiles.ToList();               
                if (item.IsPage || item.MessageType == "FCP")
                {
                    var toUserId = item.UserId;
                    var pageId = pages.Where(x => x.ProfilePageID == toUserId).FirstOrDefault().Id;
                    var fromUserId = item.ReplyedByUserId;
                    var customer = customers.Where(x => x.ExternalId == fromUserId).FirstOrDefault();
                    if (customer != null)
                    {
                        var consumerMapping = db.ProfileToCustSocialProfiles.Where(y => y.ProfilePageId == pageId && y.CustSocialProfileId == customer.Id).FirstOrDefault();
                        item.IsWarned = consumerMapping != null ? consumerMapping.WarnBanCustomers.Count > 0 : false;
                    }
                }
                else if (item.MessageType == "TM" || item.MessageType == "TP")
                {
                    var toUser = String.IsNullOrEmpty(item.ToUser) ? item.User : item.ToUser;
                    var twitterName = item.ReplyedBy;
                    var consumer = profiles.Where(x => x.UserName == toUser).FirstOrDefault();
                    if (consumer != null)
                    {
                        var consumerMapping = consumer.ProfileToCustSocialProfiles.Where(y => y.CustomerSocialProfile.UserName == twitterName).FirstOrDefault();
                        item.IsWarned = consumerMapping != null ? consumerMapping.WarnBanCustomers.Count > 0 : false;
                    }
                }
            }*/
            if (item.MessageType == "IM")
            {
                AttachmentModel model = new AttachmentModel();
                model.Url = item.Urls[0];
                model.MediaType = item.MessageType;
                item.Attachments.Add(model);
            }
            else
            {
                foreach (var attachment in item.Urls)
                {
                    if (attachment != "|")
                    {
                        string[] descAndUrlArray = attachment.Trim().Split('|');
                        if (descAndUrlArray.Length > 1 && !String.IsNullOrEmpty(descAndUrlArray[0]) && !String.IsNullOrEmpty(descAndUrlArray[1]))
                        {
                            AttachmentModel model = new AttachmentModel();
                            string type = descAndUrlArray[0];
                            string url = "";
                            if (type.Contains("image") || type.Contains("photo") || type.Contains("stream_publish"))
                            {
                                type = "image";
                                url = Uri.UnescapeDataString(descAndUrlArray[1]);
                                if (url.Contains(".gif"))
                                {
                                    MatchCollection m = Regex.Matches(url, "(http|https)://(\\S+?)\\.(jpg|png|gif)");
                                    if (m.Count > 0)
                                    {
                                        url = m[0].Success ? m[0].Groups[0].Value : url;
                                    }
                                }
                            }
                            else if ((type.Contains("video") && !type.Contains("share")) || type.Contains("animated_gif"))
                            {
                                url = Uri.UnescapeDataString(descAndUrlArray[1]);
                                if (type.Contains("video") && descAndUrlArray[1].Contains("facebook") && item.MessageType != "FCP")
                                {
                                    type = "video_fb";
                                    url = string.Format("https://www.facebook.com/plugins/video.php?href={0}", url);
                                }
                                else if (type.Contains("video") && item.MessageType == "FCP")
                                {
                                    type = "video_download";
                                    //url = string.Format("https://www.facebook.com/plugins/video.php?href={0}", url);
                                }
                                else
                                    type = "twitVid_share";

                            }
                            else if (type.Contains("share"))
                            {

                                url = Uri.UnescapeDataString(descAndUrlArray[1]);
                                if (url.Contains("youtube") || url.Contains("youtu.be"))
                                {
                                    Match youtubeMatch = YoutubeVideoRegex.Match(url);
                                    string id = string.Empty;
                                    if (youtubeMatch.Success)
                                    {
                                        id = youtubeMatch.Groups[1].Value;
                                        url = string.Format("https://www.youtube.com/embed/{0}", id);
                                        type = "video_share";
                                    }
                                }
                                else if (url.Contains("vimeo"))
                                {
                                    Match vimeoMatch = VimeoVideoRegex.Match(url);
                                    string id = string.Empty;
                                    if (vimeoMatch.Success)
                                    {
                                        id = vimeoMatch.Groups[1].Value;
                                        url = string.Format("https://player.vimeo.com/video/{0}?color=ffffff", id);
                                        type = "video_share";
                                    }
                                }
                                else if (url.Contains("dailymotion"))
                                {
                                    Match dailyMotionMatch = DMVideoRegex.Match(url);
                                    string id = string.Empty;
                                    if (dailyMotionMatch.Success)
                                    {
                                        id = dailyMotionMatch.Groups[2].Value;
                                        url = string.Format("http://www.dailymotion.com/embed/video/{0}", id);
                                        type = "video_share";
                                    }
                                }
                                else
                                {
                                    type = "other";
                                }
                            }
                            url = url.Replace("https://l.facebook.com/l.php?u=", "").Replace("https://www.facebook.com/l.php?u=", "");
                            model.Type = type;
                            model.MediaType = item.MessageType;
                            model.Url = url;

                            if (type != "other")
                                item.Attachments.Add(model);
                        }
                    }
                }
            }
        }
    }

    public class MsgArchiveModel
    {
        public string MsgType { get; set; }
        public int MsgId { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }

    public class AttachmentModel
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string MediaType { get; set; }
    }
    public class OutlookItemModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Formatted_Date { get; set; }
        public string From { get; set; }
        public string[] To { get; set; }
        public string[] CC { get; set; }
        public string Subject { get; set; }
        public bool IsRead { get; set; }
        public string Has_Attachments { get; set; }
        public Microsoft.Exchange.WebServices.Data.Attachment[] Attachments { get; set; }
        public string Full_Text { get; set; }
    }

    public class OutlookEmailRead
    {
        public string EmailAddress { get; set; }
        public string EmailRecDate { get; set; }
    }

    public class InboxItemFilterModel
    {
        public string profileData { get; set; }
        public string profilePageData { get; set; }
        public string msgTypeData { get; set; }
        public string tagTypeData { get; set; }
        public string brandTypeData { get; set; }
        public string sentTypeData { get; set; }
        public string teamData { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string pageType { get; set; }
        public int groupId { get; set; }
        public int companyId { get; set; }
        public bool isAllTask { get; set; }
        public string consumerId { get; set; }
        public bool showComplaints { get; set; }
        public string maxIds { get; set; }
        public string authorName { get; set; }
        public string authorHandle { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int? userId { get; set; }
    }
}