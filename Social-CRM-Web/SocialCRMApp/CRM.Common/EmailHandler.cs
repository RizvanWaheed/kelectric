using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common
{
    public class EmailHandler
    {
        private string smtpHost;
        private int smtpPort;
        public string SMTPHost
        {
            get
            {
                return smtpHost;
            }

        }
        public int SMTPPort
        {
            get
            {
                return smtpPort;
            }

        }
        public EmailHandler()
        {
            EmailClient = InitilizeSMPT();
        }
        public  SmtpClient EmailClient { get; set; }
        public EmailHandler(string host, string port)
        {
            this.smtpHost = host;
            this.smtpPort = Convert.ToInt32(port);
        }

        //commented purposely [causng conflict with overload SendEmail(string from, string recipients, string subject, string body, string cc)]
        //public Boolean SendEmail(string from, string recipients, string subject, string body, List<Attachment> attachmentList)
        //{
        //    try
        //    {
        //        MailMessage mail = new MailMessage();

        //        mail.IsBodyHtml = true;
        //        mail.Subject = subject;

        //        MailAddress fromAdd = new MailAddress(from, from);
        //        mail.From = fromAdd;

        //        var tos = recipients.Split(";".ToCharArray());

        //        foreach (var to in tos)
        //        {
        //            MailAddress toadd = new MailAddress(to);
        //            mail.To.Add(toadd);
        //        }

        //        if (attachmentList != null)
        //        {
        //            foreach (var atch in attachmentList)
        //            {
        //                mail.Attachments.Add(atch);
        //            }
        //        }

        //        if (EmailClient == null)
        //            EmailClient = InitilizeSMPT();
        //        EmailClient.Send(mail);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public Boolean SendEmail(string from, string recipients, string subject, string body, string cc)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.Subject =  subject;
                mail.Body = body;
                MailAddress fromAdd = new MailAddress(from, from);
                mail.From = fromAdd;
                var tos = recipients.Split(";".ToCharArray());
                foreach (var to in tos)
                {
                    if (to != "")
                    {
                        MailAddress toadd = new MailAddress(to);
                        mail.To.Add(toadd);
                    }
                }

                if (cc != "" && cc != null)
                {
                    var CCs = cc.Split(";".ToCharArray());
                    foreach (var CC in CCs)
                    {
                        if (CC != "")
                        {
                            MailAddress ccadd = new MailAddress(CC);
                            mail.CC.Add(ccadd);
                        }
                    }
                }
                if (EmailClient == null)
                    EmailClient = InitilizeSMPT();
                EmailClient.Send(mail);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public Boolean SendEmail(string from, string recipients, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = body;
                MailAddress fromAdd = new MailAddress(from, from);
                mail.From = fromAdd;
                var tos = recipients.Split(";".ToCharArray());
                foreach (var to in tos)
                {
                    MailAddress toadd = new MailAddress(to);
                    mail.To.Add(toadd);
                }

                if (EmailClient == null)
                    EmailClient = InitilizeSMPT();
                EmailClient.Send(mail);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public Boolean SendEmail(string recipients, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = body;
                var tos = recipients.Split(";".ToCharArray());
                foreach (var to in tos)
                {
                    MailAddress toadd = new MailAddress(to);
                    mail.To.Add(toadd);
                }
                var fromAddress = ConfigurationManager.AppSettings["EmailUserName"];
                MailAddress fromAdd = new MailAddress(fromAddress,"KE Social CRM");
                mail.From = fromAdd;
                if (EmailClient == null)
                    EmailClient = InitilizeSMPT();
                EmailClient.Send(mail);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private SmtpClient InitilizeSMPT()
        {

            var host = ConfigurationManager.AppSettings["SmtpHost"];
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            var userName = ConfigurationManager.AppSettings["EmailUserName"];
            var password = ConfigurationManager.AppSettings["EmailPassword"];
           return  new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, password)
            };
        }

        public Boolean SendEmail(string recipients, string subject, AlternateView altView)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                //mail.Body = body;
                mail.AlternateViews.Add(altView);
                var tos = recipients.Split(";".ToCharArray());
                foreach (var to in tos)
                {
                    MailAddress toadd = new MailAddress(to);
                    mail.To.Add(toadd);
                }
                var fromAddress = ConfigurationManager.AppSettings["EmailUserName"];
                MailAddress fromAdd = new MailAddress(fromAddress, "KE Social CRM");
                mail.From = fromAdd;
                if (EmailClient == null)
                    EmailClient = InitilizeSMPT();
                EmailClient.Send(mail);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public AlternateView getEmbeddedImage(String[] filePath, string htmlBody, string[] contentId)
        {
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            for(int i = 0; i < filePath.Count(); i++)
            {
                LinkedResource inline = new LinkedResource(filePath[i]);
                inline.ContentId = contentId[i];
                alternateView.LinkedResources.Add(inline);
            }
            return alternateView;
        }
    }
}
