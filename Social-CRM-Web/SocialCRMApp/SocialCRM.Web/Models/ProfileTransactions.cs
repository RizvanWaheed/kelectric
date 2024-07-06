using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repositories.UnitOfWork;
using CRM.Entities;
using CRM.Entities.Repository;
using System.Data.SqlClient;
using System.Data;

namespace SocialCRM.Web.Models
{
    public class ProfileTransactions
    {
        private UnitOfWork workObj = new UnitOfWork();
        private CRM.Entities.Profile userObj;
        private ProfilePage userPage;
        private PostUserStatusFB userStatusFB;
        private PostUserStatusTwitter userStatusTwitter;
        private ProfileComapnyGroup profileGroupObj;
        private PostUserStatusInstagram userStatusInstagram;
        private CommonRepository comRep = new CommonRepository();
        private bool isFBPage = false;

        public ProfileTransactions(int companyId, int groupId, int memberUserId, string userId, string userName, string name, string accessToken, string accessTokenSecret, int profileTypeId, bool isActive, string profilePic, string profileURI, bool IsPersonal, bool isBaseAccount, bool isPage)
        {
            userObj = new CRM.Entities.Profile();
            userObj.MemberUserId = memberUserId;
            userObj.ProfileExternalId = userId;
            userObj.UserName = userName;
            userObj.Name = name;
            userObj.AcessToken = accessToken;
            userObj.AcessTokenSecret = accessTokenSecret;
            userObj.ProfileTypeId = profileTypeId;
            userObj.CompanyId = companyId;
            userObj.IsActive = isActive;
            userObj.ProfilePic = profilePic;
            userObj.ProfileURl = profileURI;
            userObj.IsPersonal = IsPersonal;
            userObj.IsBaseAccount = isBaseAccount;
            isFBPage = isPage;
            profileGroupObj = new ProfileComapnyGroup();
            profileGroupObj.CompanyId = companyId;
            profileGroupObj.GroupId = groupId;

            if (profileTypeId == 4)
            {
                userStatusTwitter = new PostUserStatusTwitter();
                userStatusTwitter.UserId = userId;
                userStatusTwitter.MaxMentionStatusId = "0";
                userStatusTwitter.KeywordId = null;
                userStatusTwitter.MaxStatusId = "0";
                userStatusTwitter.MaxMsgSendById = "0";
                userStatusTwitter.MaxMsgSendToId = "0";
            }
            else if (profileTypeId == 1)
            {
                userStatusFB = new PostUserStatusFB();
                userStatusFB.UserId = userId;
                userStatusFB.PageId = null;
                userStatusFB.MaxPostCreationDate = null;
                userStatusFB.MaxConUpdatedTime = null;
            }
            else if (profileTypeId == 6)
            {
                userStatusInstagram = new PostUserStatusInstagram
                {
                    InstaUserId = userId,
                    LastPostCreatedDate = string.Empty
                };
            }
        }

        public ProfileTransactions(int companyId, int groupId, int profileTransactionId, string userPageId, string userPageAccessToken, string userPageName, string userPageUrl, string userId, string userAcessToken, bool isConnected, string profilePic)
        {
            userPage = new ProfilePage();
            userPage.ProfileTransactionId = profileTransactionId;
            userPage.ProfileExternalId = userId;
            userPage.ProfileAccessToken = userAcessToken;
            userPage.ProfilePageID = userPageId;
            userPage.PageAccessToken = userPageAccessToken;
            userPage.PageName = userPageName;
            userPage.PageUrl = userPageUrl;
            userPage.IsConnected = isConnected;
            userPage.ProfilePic = profilePic;

            userStatusFB = new PostUserStatusFB();
            userStatusFB.UserId = userId;
            userStatusFB.PageId = userPageId;
            userStatusFB.MaxPostCreationDate = null;
        }

        public int insertProfile()
        {
            CRM.Entities.Profile existingUserObj = comRep.GetProfileByProfileExternalId(userObj.ProfileExternalId);
            int id;
            if (existingUserObj != null)
            {
                //In case a profile is already associated with another social CRM account we do not update it again and return an error that it is not allowed
                if (existingUserObj.MemberUserId != userObj.MemberUserId)
                {
                    return -1;
                }

                existingUserObj.MemberUserId = userObj.MemberUserId;
                existingUserObj.Name = userObj.Name;
                existingUserObj.UserName = userObj.UserName;
                existingUserObj.AcessToken = userObj.AcessToken;
                existingUserObj.AcessTokenSecret = userObj.AcessTokenSecret;
                existingUserObj.ProfileTypeId = userObj.ProfileTypeId;
                existingUserObj.CompanyId = userObj.CompanyId;
                existingUserObj.ProfilePic = userObj.ProfilePic;
                existingUserObj.ProfileURl = userObj.ProfileURl;
                if (userObj.CompanyId == -1)
                    existingUserObj.IsPersonal = userObj.IsPersonal;
                existingUserObj.IsBaseAccount = userObj.IsBaseAccount;
                //If the user is not already active
                if (existingUserObj.IsActive == false)
                {
                    existingUserObj.IsActive = userObj.IsActive;
                }
                workObj.ProfileRepository.Update(existingUserObj);
                workObj.Save();
                id = existingUserObj.Id;
            }
            else
            {
                CRM.Entities.Profile obj = workObj.ProfileRepository.InsertWithEntity(userObj);
                if (userObj.ProfileTypeId == 1)
                    workObj.PostStatusFBRepository.Insert(userStatusFB);
                else if (userObj.ProfileTypeId == 4)
                    workObj.PostStatusTwitterRepository.Insert(userStatusTwitter);
                workObj.Save();
                id = obj.Id;
                if (userObj.ProfileTypeId == 6)
                {
                    userStatusInstagram.ProfileId = id;
                    workObj.PostStatusInstagramRepository.Insert(userStatusInstagram);
                }
            }

            if ((userObj.IsPersonal == false || userObj.IsBaseAccount == true) && isFBPage == false)
            {
                profileGroupObj.ProfileId = id;
                bool hasValue = comRep.ProfileCompanyGroupRelationExists(profileGroupObj.CompanyId, profileGroupObj.GroupId, id);
                if (!hasValue)
                {
                    //Assign Profile to the selected group
                    workObj.ProfileComapnyGroupRepository.ExecStoreProcedure("exec dbo.AssignProfileComapnyGroup @profileId,@groupId,@companyId,@userId,@isProfilePage", new SqlParameter("@profileId", SqlDbType.Int) { Value = id }, new SqlParameter("@groupId", SqlDbType.Int) { Value = profileGroupObj.GroupId }, new SqlParameter("@companyId", SqlDbType.Int) { Value = profileGroupObj.CompanyId }, new SqlParameter("@userId", SqlDbType.Int) { Value = userObj.MemberUserId }, new SqlParameter("@isProfilePage", SqlDbType.Bit) { Value = false });
                }
            }
            workObj.Save();
            return id;
        }

        public void insertPage()
        {
            ProfilePage existingUserPageObj = comRep.GetProfilePageByCompositeId(userPage.ProfileExternalId, userPage.ProfilePageID);
            if (existingUserPageObj != null)
            {
                existingUserPageObj.ProfileTransactionId = userPage.ProfileTransactionId;
                existingUserPageObj.ProfileExternalId = userPage.ProfileExternalId;
                existingUserPageObj.ProfileAccessToken = userPage.ProfileAccessToken;
                existingUserPageObj.ProfilePageID = userPage.ProfilePageID;
                existingUserPageObj.PageAccessToken = userPage.PageAccessToken;
                existingUserPageObj.PageName = userPage.PageName;
                existingUserPageObj.PageUrl = userPage.PageUrl;
                existingUserPageObj.ProfilePic = userPage.ProfilePic;
                //If the page is not already connected
                if (existingUserPageObj.IsConnected == false)
                {
                    existingUserPageObj.IsConnected = userPage.IsConnected;
                }

                workObj.ProfilePageRepository.Update(existingUserPageObj);
                workObj.Save();
            }
            else
            {
                ProfilePage obj = workObj.ProfilePageRepository.InsertWithEntity(userPage);
                workObj.PostStatusFBRepository.Insert(userStatusFB);
                workObj.Save();
            }
        }
    }
}