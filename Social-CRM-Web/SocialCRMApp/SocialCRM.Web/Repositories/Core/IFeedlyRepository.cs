using SocialCRM.Web.Models.Feedly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IFeedlyRepository
    {
        Task<bool> AddKeyword(FeedlyModel feedKeyword, int userId);
        Task<bool> DeleteKeyword(int feedlyKewordId, int userId);
        Task<List<FeedlyModel>> GetKeywordByGroup(FeedlySearchModel searchModel);
        Task<List<FeedlyModel>> GetSearchTermByGroupId(int groupId, bool isSearchTerm);
        Task<List<FeedlyViewModel>> GetMinnedFeedByGroup(FeedlySearchModel searchModel);
    }
}
