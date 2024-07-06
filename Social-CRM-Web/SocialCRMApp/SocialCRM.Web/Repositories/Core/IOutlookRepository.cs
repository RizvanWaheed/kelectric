using CRM.Entities;
using LinqToTwitter;
using SocialAPIsWrapper.Models;
using SocialCRM.Web.Models.UnifiedContact;
using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IOutlookRepository
    {
        Task<List<OutlookItemModel>> GetOutlookInboxData(string email, string lastdt, string index, string maxdt, string st);
        Task<List<OutlookItemModel>> GetOutlookSentData(string email, string lastdt, string index, string maxdt, string st);
        Task<bool> SetReadStatus(string email, string email_date);
    }
}