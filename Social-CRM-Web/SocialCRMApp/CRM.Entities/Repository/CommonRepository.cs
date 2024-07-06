using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Repository
{
    public class CommonRepository
    {
        public Profile GetProfileByProfileExternalId(string ProfileExternalId)
        {
            using (SocialCRMEntities context = new SocialCRMEntities())
            {
                var query = (from e in context.Profiles
                             where e.ProfileExternalId == ProfileExternalId
                             select e);
                return query.ToList<Profile>().FirstOrDefault();
            }
        }

        public ProfilePage GetProfilePageByCompositeId(string userId, string pageId)
        {
            using (SocialCRMEntities context = new SocialCRMEntities())
            {
                var query = (from e in context.ProfilePages
                             where e.ProfileExternalId == userId &&
                             e.ProfilePageID == pageId 
                             select e);
                return query.ToList<ProfilePage>().FirstOrDefault();
            }
        }

        public bool ProfileCompanyGroupRelationExists(int companyId, int groupId, int profileId)
        {
            using (SocialCRMEntities context = new SocialCRMEntities())
            {
                return context.ProfileComapnyGroups.Any(t => t.CompanyId == companyId && t.GroupId == groupId && t.ProfileId == profileId);
            }
        }

    }
}
