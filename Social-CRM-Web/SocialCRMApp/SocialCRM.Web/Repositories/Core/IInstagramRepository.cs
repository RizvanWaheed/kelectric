using SocialCRM.Web.Models.Instagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM.Web.Repositories.Core
{
    public interface IInstagramRepository
    {
         Task<bool> AddAccount(InstagramModel model);
         List<InstagramModel> GetAccountByGroupId(int groupId);
         List<InstagramModel> GetAccountByCompanyId(int companyId);
         Task<InstagramQueryModel> BulidAccountFilterQuery(InstagramFilterModel search);
         Task<InstagramPostModel> GetInstagramPostDetails(string postId);
         Task<string> PostInstagramComment(string postId, string userId, string comment, int teamId, int companyId);
         Task<string> DeleteInstagramComment(string postId, string userId, string commentId);
         Task<string> LikeInstagramPost(string postId, string userId);
         Task<string> UnlikeInstagramPost(string postId, string userId);
         Task<bool> IsInstagramPostLiked(string postId, string userId);
    }
}
