using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace Insight360.Common.Util
{
    public class EmailSender
    {
        private static SmtpClient smtp;
        private static MailAddress fromAddress;
        private static void InitializeEmailServer()
        {
            string fromUserName = ConfigurationManager.AppSettings["EmailUserName"];
            string fromEmail = ConfigurationManager.AppSettings["EmailUserName"];
            string fromUserPassword = ConfigurationManager.AppSettings["EmailPassword"];
            fromAddress = new MailAddress(fromEmail);

            string host = ConfigurationManager.AppSettings["SmtpHost"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

            smtp = new System.Net.Mail.SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromUserName, fromUserPassword),
                Timeout = 500000
            };
        }

        public static bool SendEmail(List<string> emailIds, string[] cc, string subject, string body, List<Attachment> attachmentList)
        {
            bool response = false;
            try
            {
                //ThreadPool.QueueUserWorkItem(o => SendEmailInThread(emailIds, cc, subject, body, attachmentList));
                SendEmailInThread(emailIds, cc, subject, body, attachmentList);
                response = true;
            }
            catch (Exception ex)
            {
                response = false;
            }
            return response;
        }

        public static bool SendEmailAV(List<string> emailIds, string[] cc, string subject, string body, List<Attachment> attachmentList, AlternateView alternateView)
        {
            bool response = false;
            try
            {
                ThreadPool.QueueUserWorkItem(o => SendEmailInThreadWithAlternateView(emailIds, cc, subject, body, attachmentList, alternateView));
                response = true;
            }
            catch (Exception ex)
            {
                response = false;
            }
            return response;
        }

        private static void SendEmailInThread(List<string> emailIds, string[] cc, string subject, string body, List<Attachment> attachmentList)
        {
            try
            {
                InitializeEmailServer();

                MailMessage message = new MailMessage();
                message.From = fromAddress;
                foreach (string email in emailIds)
                {
                    var toAddress = new MailAddress(email);
                    message.To.Add(toAddress);
                }
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;

                if (cc != null && cc.Length > 0)
                {
                    foreach (string item in cc)
                    {
                        if (UtilityMethods.IsValidEmailAddress(item))
                        {
                            message.CC.Add(new MailAddress(item));
                        }
                    }
                }
                if (attachmentList != null)
                {

                    if (attachmentList.Count > 0)
                    {
                        foreach (var attachment in attachmentList)
                        {
                            message.Attachments.Add(attachment);
                        }
                    }
                }
                smtp.Send(message);
                smtp.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        private static void SendEmailInThreadWithAlternateView(List<string> emailIds, string[] cc, string subject, string body, List<Attachment> attachmentList, AlternateView alternateView)
        {
            try
            {
                InitializeEmailServer();

                MailMessage message = new MailMessage();
                message.From = fromAddress;
                foreach (string email in emailIds)
                {
                    var toAddress = new MailAddress(email);
                    message.To.Add(toAddress);
                }
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.AlternateViews.Add(alternateView);

                if (cc != null && cc.Length > 0)
                {
                    foreach (string item in cc)
                    {
                        if (UtilityMethods.IsValidEmailAddress(item))
                        {
                            message.CC.Add(new MailAddress(item));
                        }
                    }
                }
                if (attachmentList != null)
                {

                    if (attachmentList.Count > 0)
                    {
                        foreach (var attachment in attachmentList)
                        {
                            message.Attachments.Add(attachment);
                        }
                    }
                }
                smtp.Send(message);
                smtp.Dispose();
            }
            catch (Exception ex)
            {

            }
        }


    }
    
}
