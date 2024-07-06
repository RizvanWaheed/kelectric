using SocialCRM.Web.Models.Facebook;
using SocialCRM.Web.Models.UnifiedInbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IFacebookRepository
    {
        Task<FacebookPageReportModel> FacebookGetPageReportStats(string pageId, DateTime from, DateTime to);

        Task<FBPostInfoModel> FacebookGetPostInfo(string postId, string commentId);
        Task<List<InboxItemModel>> FacebookGetUserConversation(string conId);
    }
}