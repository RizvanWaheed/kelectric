using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWSTest.Models
{
    public class Emails
    {
        public string Date { get; set; }
        public string From { get; set; }
        public string[] To { get; set; }
        public string Subject { get; set; }
        public string Has_Attachments { get; set; }
        public string[] Attachments { get; set; }
        public string Id { get; set; }

        public string Text { get; set; }
    }
}