using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialCRM.Web.Repositories.Core;
using SocialCRM.Web.Models.UnifiedContact;
using SocialAPIsWrapper.APIs;
using System.Threading.Tasks;
using LinqToTwitter;
using SocialCRM.Web.Models.UnifiedInbox;
using CRM.Common.Extensions;
using CRM.Entities;
using SocialCRM.Web.Models.Profiles;
using System.Data.Entity;
using SocialAPIsWrapper.Models;
using SocialCRM.Web.Controllers.API;
using System.IO;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using System.Web.Http;


namespace SocialCRM.Web.Repositories
{
    public class OutlookRepository : IOutlookRepository
    {
        ExchangeService exchange;
        FileAttachment[] attachments = null;

        public OutlookRepository()
        {
            exchange = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
            // exchange.Url = new Uri(ConfigurationManager.AppSettings["EWSUrl"]);
        }

        string UploadAttachments(Microsoft.Exchange.WebServices.Data.Attachment attachment, int index, string sHTMLCOntent, DateTime dt, string email_type)
        {
            if (attachment.IsInline)
            {
                string inline_path = "~/Content/img/email";
                string dir_inline_path = HttpContext.Current.Server.MapPath(inline_path); //Inline Attachment Folder path

                if (!Directory.Exists(dir_inline_path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(dir_inline_path); //Create Inline Folder, if it doesn't exist
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }

                string sType = attachment.ContentType.ToLower();
                if (sType.Contains("image"))
                {
                    attachments[index] = (FileAttachment)attachment;
                    string sID = attachments[index].ContentId;
                    sType = sType.Replace("image/", "");
                    string sFilename = sID + "." + sType;

                    string sPathPlusFilename = inline_path + "/" + sFilename;

                    if (!File.Exists(dir_inline_path + @"\" + sFilename))
                    {
                        if (attachments[index].ContentId != null)
                            attachments[index].Load(HttpContext.Current.Server.MapPath(sPathPlusFilename));
                    }

                    string oldString = "cid:" + sID;
                    sHTMLCOntent = sHTMLCOntent.Replace(oldString, "../Content/img/email/" + sFilename);
                }
            }
            else
            {
                string file_path = "~/Content/img/email/" + email_type;
                string dir_file_path = HttpContext.Current.Server.MapPath(file_path); //File Attachment Folder path

                if (!Directory.Exists(dir_file_path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(dir_file_path); //Create File Attachment Folder, if it doesn't exist
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }

                string email_folder_name = dt.ToString("ddMMyyyyHHmmss");
                string email_folder_path = file_path + "/" + email_folder_name;
                string dir_email_folder_path = HttpContext.Current.Server.MapPath(email_folder_path);

                if (!Directory.Exists(dir_email_folder_path))
                    Directory.CreateDirectory(dir_email_folder_path); //Create File Attachment Folder for email, if it doesn't exist

                attachments[index] = (FileAttachment)attachment;
                string sFilename = attachments[index].Name;
                string sPathPlusFilename = email_folder_path + "/" + sFilename;

                if (!File.Exists(dir_email_folder_path + @"\" + sFilename))
                    attachments[index].Load(HttpContext.Current.Server.MapPath(sPathPlusFilename));
            }

            return sHTMLCOntent;
        }

        public Task<List<OutlookItemModel>> GetOutlookInboxData(string email, string lastdt, string upper_bound, string maxdt, string status)
        {
            var query = new List<OutlookItemModel>();
            int index = upper_bound == "null" ? -1 : Convert.ToInt32(upper_bound);
            bool is_connected = GetOutlookProfile(email);

            //  await System.Threading.Tasks.Task.Run(() =>
            // {
            if (is_connected)
            {
                try
                {
                    // exchange.AutodiscoverUrl(email);
                    //exchange.Url = new Uri("https://webmail.trgworld.com/EWS/Exchange.asmx");

                    int no_of_emails; FindItemsResults<Item> findResults;

                    if (lastdt == "null" || lastdt == "")
                    {
                        no_of_emails = 6;
                        findResults = exchange.FindItems(WellKnownFolderName.Inbox, new ItemView(no_of_emails));
                    }
                    else if (status == "new")
                    {
                        no_of_emails = 10;
                        SearchFilter.IsGreaterThan filter = new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, Convert.ToDateTime(maxdt));
                        findResults = exchange.FindItems(WellKnownFolderName.Inbox, filter, new ItemView(no_of_emails));
                    }
                    else
                    {
                        no_of_emails = 3;
                        SearchFilter.IsLessThan filter2 = new SearchFilter.IsLessThan(ItemSchema.DateTimeReceived, Convert.ToDateTime(lastdt));
                        findResults = exchange.FindItems(WellKnownFolderName.Inbox, filter2, new ItemView(no_of_emails));
                    }

                    ServiceResponseCollection<GetItemResponse> items =
                        exchange.BindToItems(findResults.Select(item => item.Id), new PropertySet(BasePropertySet.FirstClassProperties,
                        EmailMessageSchema.From, EmailMessageSchema.ToRecipients, EmailMessageSchema.IsRead));

                    foreach (var item in items)
                    {
                        string sHTMLCOntent = item.Item.Body.Text;

                        if (item.Item.Attachments.Count != 0)
                        {
                            attachments = new FileAttachment[item.Item.Attachments.Count];
                            for (int i = 0; i < item.Item.Attachments.Count; i++)
                            {
                                try
                                {
                                    sHTMLCOntent = UploadAttachments(item.Item.Attachments[i], i, sHTMLCOntent, item.Item.DateTimeReceived, "inbox_attachment");
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(ex);
                                }
                            }
                            item.Item.Body.Text = sHTMLCOntent;
                        }
                    }

                    query = items.Select(item =>
                    {
                        return new OutlookItemModel()
                        {
                            Id = ++index,
                            Date = item.Item.DateTimeReceived.ToString(),
                            Formatted_Date = item.Item.DateTimeReceived.ToString("ddMMyyyyHHmmss"),
                            From = ((EmailAddress)item.Item[EmailMessageSchema.From]).Address,
                            To = ((EmailAddressCollection)item.Item[EmailMessageSchema.ToRecipients]).Select(recipient => recipient.Address).ToArray(),
                            CC = ((EmailAddressCollection)item.Item[EmailMessageSchema.CcRecipients]).Select(recipient => recipient.Address).ToArray(),
                            Subject = item.Item.Subject,
                            Has_Attachments = ((item.Item.HasAttachments) ? "Yes" : "No"),
                            Attachments = item.Item.Attachments.Where(x => !x.IsInline).ToArray(),
                            Full_Text = item.Item.Body.Text,
                            IsRead = ((EmailMessage)item.Item).IsRead
                        };
                    }).ToList<OutlookItemModel>();
                }

                catch (Exception ex)
                {

                }
            }
            // });

            return System.Threading.Tasks.Task.FromResult<List<OutlookItemModel>>(query);
            // return query;
        }

        public Task<List<OutlookItemModel>> GetOutlookSentData(string email, string lastdt, string upper_bound, string maxdt, string status)
        {
            var query = new List<OutlookItemModel>();
            int index = upper_bound == "null" ? -1 : Convert.ToInt32(upper_bound);
            bool is_connected = GetOutlookProfile(email);

            // await System.Threading.Tasks.Task.Run(() =>
            // {
            if (is_connected)
            {
                try
                {
                    // exchange.AutodiscoverUrl(email);//exchange.Url = new Uri("https://webmail.trgworld.com/EWS/Exchange.asmx");
                    int no_of_emails; FindItemsResults<Item> findResults;

                    if (lastdt == "null" || lastdt == "")
                    {
                        no_of_emails = 6;
                        findResults = exchange.FindItems(WellKnownFolderName.SentItems, new ItemView(no_of_emails));
                    }
                    else if (status == "new")
                    {
                        no_of_emails = 10;
                        SearchFilter.IsGreaterThan filter = new SearchFilter.IsGreaterThan(ItemSchema.DateTimeSent, Convert.ToDateTime(maxdt));
                        findResults = exchange.FindItems(WellKnownFolderName.SentItems, filter, new ItemView(no_of_emails));
                    }
                    else
                    {
                        no_of_emails = 3;
                        SearchFilter.IsLessThan filter2 = new SearchFilter.IsLessThan(ItemSchema.DateTimeSent, Convert.ToDateTime(lastdt));
                        findResults = exchange.FindItems(WellKnownFolderName.SentItems, filter2, new ItemView(no_of_emails));
                    }

                    ServiceResponseCollection<GetItemResponse> items =
                            exchange.BindToItems(findResults.Select(item => item.Id), new PropertySet(BasePropertySet.FirstClassProperties,
                            EmailMessageSchema.From, EmailMessageSchema.ToRecipients));

                    foreach (var item in items)
                    {
                        string sHTMLCOntent = item.Item.Body.Text;

                        if (item.Item.Attachments.Count != 0)
                        {
                            attachments = new FileAttachment[item.Item.Attachments.Count];
                            for (int i = 0; i < item.Item.Attachments.Count; i++)
                            {
                                try
                                {
                                    sHTMLCOntent = UploadAttachments(item.Item.Attachments[i], i, sHTMLCOntent, item.Item.DateTimeSent, "sent_attachment");
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(ex);
                                }
                            }
                            item.Item.Body.Text = sHTMLCOntent;
                        }
                    }

                    query = items.Select(item =>
                    {
                        return new OutlookItemModel()
                        {
                            Id = ++index,
                            Date = item.Item.DateTimeSent.ToString(),
                            Formatted_Date = item.Item.DateTimeSent.ToString("ddMMyyyyHHmmss"),
                            From = ((EmailAddress)item.Item[EmailMessageSchema.From]).Address,
                            To = ((EmailAddressCollection)item.Item[EmailMessageSchema.ToRecipients]).Select(recipient => recipient.Address).ToArray(),
                            CC = ((EmailAddressCollection)item.Item[EmailMessageSchema.CcRecipients]).Select(recipient => recipient.Address).ToArray(),
                            Subject = item.Item.Subject,
                            Has_Attachments = ((item.Item.HasAttachments) ? "Yes" : "No"),
                            Attachments = item.Item.Attachments.Where(x => !x.IsInline).ToArray(),
                            Full_Text = item.Item.Body.Text
                        };
                    }).ToList<OutlookItemModel>();
                }

                catch (Exception ex)
                {

                }
            }
            // });

            return System.Threading.Tasks.Task.FromResult<List<OutlookItemModel>>(query);
            //return query;
        }

        public async Task<bool> SetReadStatus(string email, string email_date)
        {
            bool status = false;
            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    bool is_connected = GetOutlookProfile(email);

                    if (is_connected)
                    {
                        SearchFilter.IsGreaterThanOrEqualTo filter = new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.DateTimeReceived, Convert.ToDateTime(email_date));
                        FindItemsResults<Item> findResults = exchange.FindItems(WellKnownFolderName.Inbox, filter, new ItemView(100));

                        if (findResults.Items.Count > 0)
                        {
                            ServiceResponseCollection<GetItemResponse> items = exchange.BindToItems(findResults.Select(item => item.Id), new PropertySet(BasePropertySet.FirstClassProperties));
                            ((EmailMessage)items[findResults.Items.Count - 1].Item).IsRead = true;
                            ((EmailMessage)items[findResults.Items.Count - 1].Item).Update(ConflictResolutionMode.AutoResolve);

                            status = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                }
            });

            return status;
        }

        public bool GetOutlookProfile(string email)
        {
            using (var db = new SocialCRMEntities())
            {
                var outlook_user = db.Profiles.Where(x => x.ProfileExternalId == email && x.ProfileType.Id == 8).FirstOrDefault();

                try
                {
                    if (outlook_user != null)
                    {
                        exchange.Url = new Uri(outlook_user.ProfileURl);
                        exchange.Credentials = new WebCredentials(outlook_user.UserName, outlook_user.AcessToken, outlook_user.AcessTokenSecret);
                        return true;
                    }
                }

                catch
                {
                    return false;
                }

                return false;
            }
        }
    }
}