using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.UnifiedInbox
{
    public class FBPostInfoModel
    {
        public FBPostModel fbPost { get; set; }
        public List<FBCommentModel> fbPostComments { get; set; }
    }
}