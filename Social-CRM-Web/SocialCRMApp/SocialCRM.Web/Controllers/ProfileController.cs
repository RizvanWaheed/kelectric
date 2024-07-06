using CRM.Common.Enum;
using CRM.Entities;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SocialCRM.Web.GoogleAnalytics;
using SocialCRM.Web.Models;
using SocialCRM.Web.Models.Profiles;
using SocialCRM.Web.Repositories;
using SocialCRM.Web.Repositories.Core;
using Sparkle.LinkedInNET;
using Sparkle.LinkedInNET.Profiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TweetSharp;

namespace SocialCRM.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private GoogleAnalyticRepository _GoogleAnalyticRepostory;
        public GoogleAnalyticRepository GoogleAnalyticRepostory
        {
            get
            {
                return _GoogleAnalyticRepostory ?? new GoogleAnalyticRepository();
            }
            private set
            {
                _GoogleAnalyticRepostory = value;
            }
        }
        private InstagramRepository _InstagramRepostory;
        public InstagramRepository InstagramRepostory
        {
            get
            {
                return _InstagramRepostory ?? new InstagramRepository();
            }
            private set
            {
                _InstagramRepostory = value;
            }
        }

        public Profile isSocialProfileTypeExistsForCompany(int companyId, int profileTypeId)
        {
            Profile existingProfile = null;
            using (var db = new SocialCRMEntities())
            {
                Company comp = db.Companies.Where(x => x.id == companyId).FirstOrDefault();
                if(comp != null)
                {
                    //if (comp.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                    {
                        existingProfile = db.Profiles.Where(x => x.CompanyId == companyId && x.ProfileTypeId == profileTypeId).FirstOrDefault();
                    }
                }
            }

            return existingProfile;
        }

        public bool isSocialProfileTypeLimitExceededForCompany(int companyId, int profileTypeId)
        {
            bool exceeded = false;
            int userId = User.Identity.GetUserId<int>();
            using (var db = new SocialCRMEntities())
            {
                Company comp = db.Companies.Where(x => x.id == companyId).FirstOrDefault();
                if (comp != null)
                {
                    //if (comp.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                    {
                        int allowedLimit = 0;
                        var user = db.UserCompanies.ToList().Where(x => x.UserId == userId).FirstOrDefault();
                        int existingAttachedProfilesCount = db.Profiles.Where(x => x.CompanyId == companyId && x.ProfileTypeId == profileTypeId).Count();
                        if (profileTypeId == (int)ProfilesType.Facebook)
                        {                            
                            if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                                allowedLimit = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.FBProfile).FirstOrDefault().Count;

                            exceeded = existingAttachedProfilesCount >= allowedLimit;
                        }
                        else if (profileTypeId == (int)ProfilesType.Twitter)
                        {
                            if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                                allowedLimit = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.TWProfile).FirstOrDefault().Count;

                            exceeded = existingAttachedProfilesCount >= allowedLimit;
                        }
                        else if (profileTypeId == (int)ProfilesType.Instagram)
                        {
                            if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                                allowedLimit = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.INProfile).FirstOrDefault().Count;

                            exceeded = existingAttachedProfilesCount >= allowedLimit;
                        }
                    }
                }
            }
            return exceeded;
        }

        public string[] getCompanyPackage(int companyId, int profileTypeId)
        {
            string[] result = new string[2];
            int userId = User.Identity.GetUserId<int>();
            using (var db = new SocialCRMEntities())
            {
                Company comp = db.Companies.Where(x => x.id == companyId).FirstOrDefault();
                if (comp != null)
                    result[0] = comp.SubscriberType.Name;

                int allowedLimit = 0;
                var user = db.UserCompanies.ToList().Where(x => x.UserId == userId).FirstOrDefault();
                if (profileTypeId == (int)ProfilesType.Facebook)
                {
                    if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                        allowedLimit = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.FBProfile).FirstOrDefault().Count;                    
                }
                else if (profileTypeId == (int)ProfilesType.Twitter)
                {
                    if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                        allowedLimit = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.TWProfile).FirstOrDefault().Count;                    
                }
                else if (profileTypeId == (int)ProfilesType.Instagram)
                {
                    if (user.Company.SubscriberType.MembershipRestrictions != null && user.Company.SubscriberType.MembershipRestrictions.Count > 0)
                        allowedLimit = user.Company.SubscriberType.MembershipRestrictions.Where(y => y.RestrictionTypeId == (int)RestrictionType.INProfile).FirstOrDefault().Count;                    
                }
                result[1] = Convert.ToString(allowedLimit);
            }
            return result;
        }

        public AnalyticToken isGoogleAnalyticExistsForCompany(int companyId)
        {
            AnalyticToken existingProfile = null;
            using (var db = new SocialCRMEntities())
            {
                Company comp = db.Companies.Where(x => x.id == companyId).FirstOrDefault();
                if(comp != null)
                {
                    //if (comp.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                    {
                        existingProfile = db.AnalyticTokens.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    }
                }
            }

            return existingProfile;
        }

        public ActionResult Connect_Facebook(int companyId, int groupId, string url, string type)
        {
            return new ChallengeResult("Facebook", Url.Action("ExternalLoginCallback", "Profile", new { CompanyId = companyId, GroupId = groupId, Type = type, URL = url, ReturnUrl = "" }));
            //return View();
        }

        public ActionResult Connect_Facebook_Page(int companyId, int groupId, string url)
        {
            return new ChallengeResult("Facebook", Url.Action("ExternalLoginCallback", "Profile", new { CompanyId = companyId, GroupId = groupId, URL = url, ReturnUrl = "Page" }));
        }
        [AllowAnonymous]
        public ActionResult Connect_Twitter(int companyId, int groupId, string url, string type)
        {
            return new ChallengeResult("Twitter", Url.Action("ExternalLoginCallback", "Profile", new { CompanyId = companyId, GroupId = groupId, Type = type, URL = url, ReturnUrl = "" }));
        }

        public ActionResult Connect_LinkedIn(int companyId, int groupId, string url, string type)
        {
            return new ChallengeResult("LinkedIn", Url.Action("ExternalLoginCallback", "Profile", new { CompanyId = companyId, GroupId = groupId, Type = type, URL = url, ReturnUrl = "" }));
            //return View();
        }

        public ActionResult ConnectGoogleAnalytics(int companyId, int groupId, string url, string type)
        {
            string userInfo = string.Empty;
            if (this.Session["UserAnalyticInfo"] == null)
            {
                var userId = this.User.Identity.GetUserId<int>();
                var userAnalyticAccountNumber = GoogleAnalyticRepostory.GetUserAnalyticAccountNumber(userId);
                userInfo = string.Concat(userId, ",", companyId, ",", groupId, ",", userAnalyticAccountNumber);
                this.Session.Add("UserAnalyticInfo", userInfo);
            }
            var result = new Google.Apis.Auth.OAuth2.Mvc.AuthorizationCodeMvcApp(this, new SocialCRM.Web.GoogleAnalytics.AppFlowMetadata()).AuthorizeAsync(System.Threading.CancellationToken.None).Result;
            if (result.Credential == null)
                return new RedirectResult(result.RedirectUri);
            if (this.Session["UserAnalyticInfo"] != null)
            {
                userInfo = (string)Session["UserAnalyticInfo"];
                this.Session.Remove("UserAnalyticInfo");
            }
            else
            {
                userInfo = string.Concat(this.User.Identity.GetUserId<int>(), ",", companyId, ",", groupId);
            }
            var analyticsDetails = new AnalyticsHelper(result.Credential).GetAnalyticDetails();
            var googleAnalyticSites = GoogleAnalyticRepostory.UpdateAnalyticAccount(analyticsDetails, result.Credential.UserId ?? userInfo);

            if (googleAnalyticSites == null)
            {
                if (url == null)
                    return Redirect("/#/GroupAndTeam?error=SPE");
                else
                    return Redirect("/#/" + url + "?error=SPE");
            }

            if (type != null)
                return Redirect("/#/" + url + "?type=personal&pageType=google&userInfo=" + userInfo);
            else if (url != null)
                return Redirect("/#/" + url + "?pagesAdd=abc&pageType=google&userInfo=" + userInfo);
            else
                return Redirect("/#/GroupAndTeam/pagesAdd?pageType=google&userInfo=" + userInfo);
            // return Redirect("/#/GroupAndTeam/pagesAdd?pageType=google&userInfo=" + userInfo);
        }
        public ActionResult ConnectInstagram(int companyId, int groupId, string url, string type)
        {
            type = string.IsNullOrEmpty(type) ? "public" : type;
            var userInfo = string.Concat(this.User.Identity.GetUserId<int>(), ",", companyId, ",", groupId, ",", type);
            var redirectUrlValue = ConfigurationManager.AppSettings["InstagramRedirectURL"];
            var redirectUrl = string.Format(redirectUrlValue, userInfo);
            return new RedirectResult(redirectUrl);
        }
        [AllowAnonymous]
        public async Task<ActionResult> ConnectRedirectInstagram(string userInfo, string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var redirectUrlValue = ConfigurationManager.AppSettings["InstaCallBackUrl"];
                var redirect_uri = string.Format(redirectUrlValue, userInfo, code);
                System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.instagram.com/oauth/access_token");
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                var clientId = ConfigurationManager.AppSettings["InstagramClientId"];
                var clientSecret = ConfigurationManager.AppSettings["InstagramClientSecret"];
                var parameter = string.Format("client_id={0}&client_secret={1}&grant_type=authorization_code&redirect_uri={2}&code={3}&scope=basic+likes+comments+relationships+public_content", clientId, clientSecret, redirect_uri, code);
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameter);
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length); //Push it out there
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                var message = sr.ReadToEnd().Trim();
                if (!string.IsNullOrEmpty(message))
                {
                    //add user info in db.
                    var response = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Models.Instagram.RootObject>(message);
                    var userInfoArray = userInfo.Split(',');
                    
                    if (userInfoArray.Length >= 3)
                    {
                        /*string companyPackage = "", allowedLimit = "";
                        var companyAndLimit = getCompanyPackage(int.Parse(userInfoArray[1]), (int)ProfilesType.Instagram);
                        if (companyAndLimit.Length > 1)
                        {
                            companyPackage = companyAndLimit[0];
                            allowedLimit = companyAndLimit[1];
                        }
                        Profile prof = isSocialProfileTypeExistsForCompany(int.Parse(userInfoArray[1]), (int)ProfilesType.Instagram);
                        bool isProfileLimitExceeded = isSocialProfileTypeLimitExceededForCompany(int.Parse(userInfoArray[1]), (int)ProfilesType.Instagram);
                        if (isProfileLimitExceeded)
                        {
                            if(response.user.username != prof.UserName)
                                return Redirect("/#/GroupAndTeam?error=TRP&profile=" + prof.UserName + "&package=" + companyPackage + "&limit=" + allowedLimit + "&type=Instagram");
                        }*/

                        Models.Instagram.InstagramModel model = new Models.Instagram.InstagramModel
                        {
                            AccessToken = response.access_token,
                            CompanyId = int.Parse(userInfoArray[1]),
                            GroupId = int.Parse(userInfoArray[2]),
                            UserId = int.Parse(userInfoArray[0]),
                            User = new Models.Instagram.InstagramModel.InstagramUser
                            {
                                Bio = response.user.bio,
                                FullName = response.user.full_name,
                                Id = response.user.id,
                                ProfilePicture = response.user.profile_picture,
                                UserName = response.user.username,
                                WebSite = response.user.website
                            }
                        };
                        var isPersonnel = false;
                        if (userInfoArray.Length == 4)
                        {
                            var perofileType = userInfoArray[3];
                            isPersonnel = perofileType.ToLower() == "personal";
                        }
                        ProfileTransactions transact;
                        transact = new ProfileTransactions(model.CompanyId, model.GroupId, model.UserId, model.User.Id, model.User.UserName, model.User.FullName, model.AccessToken, null, 6, true, model.User.ProfilePicture, string.Format("https://www.instagram.com/{0}/", model.User.UserName), isPersonnel, false, false);
                        int transactId = transact.insertProfile();

                        if (transactId == -1)
                        {
                            return Redirect("/#/GroupAndTeam?error=SPE");
                        }
                        else
                            return Redirect("/#/GroupAndTeam");
                        // await InstagramRepostory.AddAccount(model);
                    }
                }
            }
            else
            {
                Response.Write("Error...");
            }
            return Redirect("/#/GroupAndTeam");
        }
        //public void ConnectOutlook(int companyId, int groupId, string url, string type)
        //{
        //    Response.Redirect("#/_OutlookConnect?groupid="+groupId+"&companyid="+companyId);
        //}

        public ActionResult ConnectPinterest(int companyId, int groupId, string url, string type)
        {
            type = string.IsNullOrEmpty(type) ? "public" : type;
            var userInfo = string.Concat(this.User.Identity.GetUserId<int>(), ",", companyId, ",", groupId, ",", type);
            var redirectUrlValue = ConfigurationManager.AppSettings["PinterestRedirectURL"];
            var redirectUrl = string.Format(redirectUrlValue, userInfo);
            return new RedirectResult(redirectUrl);
        }
        [AllowAnonymous]
        public async Task<ActionResult> ConnectRedirectPinterest(string state, string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.pinterest.com/v1/oauth/token");
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                var clientId = ConfigurationManager.AppSettings["PinterestClientId"];
                var clientSecret = ConfigurationManager.AppSettings["PinterestClientSecret"];
                var parameter = string.Format("client_id={0}&client_secret={1}&grant_type=authorization_code&code={2}", clientId, clientSecret, code);
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameter);
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length); //Push it out there
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                var message = sr.ReadToEnd().Trim();
                if (!string.IsNullOrEmpty(message))
                {
                    //add user info in db.
                    var response = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Models.Pinterest.RootObject>(message);
                    var userInfoArray = state.Split(',');
                    if (userInfoArray.Length >= 3)
                    {
                        Models.Pinterest.PinterestModel model = new Models.Pinterest.PinterestModel
                        {
                            AccessToken = response.access_token,
                            CompanyId = int.Parse(userInfoArray[1]),
                            GroupId = int.Parse(userInfoArray[2]),
                            UserId = int.Parse(userInfoArray[0])
                        };
                        var user = GetPinExtraInfo(model.AccessToken);
                        model.User = new Models.Pinterest.PinterestModel.PinterestUser
                        {
                            Id = user.id,
                            FirstName = user.first_name,
                            LastName = user.last_name,
                            ProfilePicture = user.image.size.url,
                            UserName = user.username,
                            Bio = user.bio,
                            ProfileUrl = user.url
                        };
                            
                            
                        var isPersonnel = false;
                        if (userInfoArray.Length == 4)
                        {
                            var perofileType = userInfoArray[3];
                            isPersonnel = perofileType.ToLower() == "personal";
                        }
                        ProfileTransactions transact;
                        transact = new ProfileTransactions(model.CompanyId, model.GroupId, model.UserId, model.User.Id, model.User.UserName, model.User.FirstName + " " + model.User.LastName, model.AccessToken, null, 7, true, model.User.ProfilePicture, model.User.ProfileUrl, isPersonnel, false, false);
                        int transactId = transact.insertProfile();

                        if (transactId == -1)
                        {
                            return Redirect("http://localhost:2566/#/GroupAndTeam?error=SPE");
                        }
                        else
                            return Redirect("http://localhost:2566/#/GroupAndTeam");
                        // await InstagramRepostory.AddAccount(model);
                    }
                }
            }
            else
            {
                Response.Write("Error...");
            }
            return Redirect("/#/GroupAndTeam");
        }
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl, int companyId, int groupId, string url, string type)
        {
            Profile prof = null;
            string companyPackage = "";
            string allowedLimit = "0";
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            ClaimsIdentity claimsIdentity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
            ProfileTransactions transact;
            int memberUserId = this.User.Identity.GetUserId<int>();
            string userInfo = memberUserId + "," + companyId + "," + groupId;
            var isBaseAccount = "";
            if (type != null)
            {
                isBaseAccount = type.Contains(",") ? type.Split(',')[1] : "";
                if (!string.IsNullOrEmpty(isBaseAccount))
                {
                    type = type.Split(',')[0];
                }
            }

            if (loginInfo != null)
            {
                if (returnUrl == "Page")
                {
                    var claims = claimsIdentity.Claims
                        .Where(c => c.Type.StartsWith("urn:facebook:access_token")).ToArray();

                    string userId = loginInfo.Login.ProviderKey;
                    string userName = loginInfo.DefaultUserName;
                    string name = loginInfo.ExternalIdentity.Name;
                    string accessToken = claims[0].Value;
                    var ExtendedAccessToken = GetExtendedAccessToken(accessToken);
                    ExtraSocialProfileInfo fbProfile = GetFBExtraInfo(ExtendedAccessToken, userId);

                    /*prof = isSocialProfileTypeExistsForCompany(companyId, (int)ProfilesType.Facebook);
                    var companyAndLimit = getCompanyPackage(companyId, (int)ProfilesType.Facebook);
                    if (companyAndLimit.Length > 1)
                    {
                        companyPackage = companyAndLimit[0];
                        allowedLimit = companyAndLimit[1];
                    }
                    bool isProfileLimitExceeded = isSocialProfileTypeLimitExceededForCompany(companyId, (int)ProfilesType.Facebook);
                    if (isProfileLimitExceeded)
                    {
                        if(prof.UserName != userName)
                        {
                            if (url == null)
                                return Redirect("/#/GroupAndTeam?error=TRP&profile=" + prof.UserName + "&package=" + companyPackage + "&limit=" + allowedLimit + "&type=FacebookPage");
                            else
                                return Redirect("/#/" + url + "?error=TRP&profile=" + prof.UserName + "&package=" + companyPackage + "&limit=" + allowedLimit + "&type=FacebookPage");
                        }
                    }*/

                    transact = new ProfileTransactions(companyId, groupId, memberUserId, userId, userName, fbProfile.UserFullName, ExtendedAccessToken, null, 1, false, fbProfile.ProfilePicURL, fbProfile.ProfileURI, false, false, true);
                    int transactId = transact.insertProfile();
                    if (transactId == -1)
                    {
                        if (url == null)
                            return Redirect("/#/GroupAndTeam?error=SPE");
                        else
                            return Redirect("/#/" + url + "?error=SPE");
                    }
                    userInfo = userInfo + "," + transactId;

                    try
                    {
                        var client = new FacebookClient(ExtendedAccessToken);
                        dynamic result = client.Get("/me/accounts?fields=access_token,category,name,id,perms,link");

                        foreach (dynamic page in result.data)
                        {
                            var extendedPageAccessToken = GetExtendedAccessToken(page.access_token);
                            var pageId = page.id;
                            var pageName = page.name;
                            var pageUrl = page.link;
                            ExtraSocialProfileInfo fbPageProfile = GetFBExtraInfo(ExtendedAccessToken, pageId);
                            if (pageHasAllPermissions(page.perms))
                            {
                                transact = new ProfileTransactions(companyId, groupId, transactId, pageId, extendedPageAccessToken, pageName, pageUrl, userId, ExtendedAccessToken, false, fbPageProfile.ProfilePicURL);
                                transact.insertPage();
                            }

                        }
                    }
                    catch (FacebookOAuthException)
                    {
                        // Our access token is invalid or expired
                        // Here we need to do something to handle this.
                    }

                    if (url != null)
                        return Redirect("/#/" + url + "?pagesAdd=abc&pageType=facebook&userInfo=" + userInfo);
                    else
                        return Redirect("/#/GroupAndTeam/pagesAdd?pageType=facebook&userInfo=" + userInfo);
                }
                else if (loginInfo.Login.LoginProvider == "Twitter")
                {
                    var claims = claimsIdentity.Claims
                        .Where(c => c.Type.StartsWith("urn:tokens:")).ToArray();
                    string accessToken = "", accessTokenSecret = "";

                    string userId = loginInfo.Login.ProviderKey;
                    string userName = loginInfo.DefaultUserName;
                    string name = loginInfo.ExternalIdentity.Name;
                    accessToken = claims[0].Value;
                    accessTokenSecret = claims[1].Value;
                    ExtraSocialProfileInfo twitProfile = GetTwitterExtraInfo(accessToken, accessTokenSecret);

                    /*prof = isSocialProfileTypeExistsForCompany(companyId, (int)ProfilesType.Twitter);
                    var companyAndLimit = getCompanyPackage(companyId, (int)ProfilesType.Twitter);
                    if (companyAndLimit.Length > 1)
                    {
                        companyPackage = companyAndLimit[0];
                        allowedLimit = companyAndLimit[1];
                    }
                    bool isProfileLimitExceeded = isSocialProfileTypeLimitExceededForCompany(companyId, (int)ProfilesType.Twitter);
                    if (isProfileLimitExceeded)
                    {
                        if (prof.UserName != userName)
                        {
                            if (url == null)
                                return Redirect("/#/GroupAndTeam?error=TRP&profile=" + prof.UserName + "&package=" + companyPackage + "&limit=" + allowedLimit + "&type=Twitter");
                            else
                                return Redirect("/#/" + url + "?error=TRP&profile=" + prof.UserName + "&package=" + companyPackage + "&limit=" + allowedLimit + "&type=Twitter");
                        }
                    }*/

                    transact = new ProfileTransactions(companyId, groupId, memberUserId, userId, userName, twitProfile.UserFullName, accessToken, accessTokenSecret, 4, true, twitProfile.ProfilePicURL, twitProfile.ProfileURI, type == "personal", "baseaccount" == isBaseAccount.ToLower(), false);
                    int transactId = transact.insertProfile();

                    if (transactId == -1)
                    {
                        if (url == null)
                            return Redirect("/#/GroupAndTeam?error=SPE");
                        else
                            return Redirect("/#/" + url + "?error=SPE");
                    }

                    if (url == null)
                        return Redirect("/#/GroupAndTeam");
                    else
                        return Redirect("/#/" + url);
                }
                else if (loginInfo.Login.LoginProvider == "Facebook")
                {
                    var claims = claimsIdentity.Claims
                        .Where(c => c.Type.StartsWith("urn:facebook:access_token")).ToArray();

                    string userId = loginInfo.Login.ProviderKey;
                    string userName = loginInfo.DefaultUserName;
                    string name = loginInfo.ExternalIdentity.Name;
                    string accessToken = claims[0].Value;
                    var ExtendedAccessToken = GetExtendedAccessToken(accessToken);
                    ExtraSocialProfileInfo fbProfile = GetFBExtraInfo(ExtendedAccessToken, userId);

                    /*prof = isSocialProfileTypeExistsForCompany(companyId, (int)ProfilesType.Facebook);
                    var companyAndLimit = getCompanyPackage(companyId, (int)ProfilesType.Facebook);
                    if (companyAndLimit.Length > 1)
                    {
                        companyPackage = companyAndLimit[0];
                        allowedLimit = companyAndLimit[1];
                    }
                    bool isProfileLimitExceeded = isSocialProfileTypeLimitExceededForCompany(companyId, (int)ProfilesType.Facebook);
                    if (isProfileLimitExceeded)
                    {
                        if (prof.UserName != userName)
                        {
                            if (url == null)
                                return Redirect("/#/GroupAndTeam?error=TRP&profile=" + prof.UserName + "&package=" + companyPackage + "&limit=" + allowedLimit + "&type=Facebook");
                            else
                                return Redirect("/#/" + url + "?error=TRP&profile=" + prof.UserName + "&package=" + companyPackage + "&limit=" + allowedLimit + "&type=Facebook");
                        }
                    }*/

                    transact = new ProfileTransactions(companyId, groupId, memberUserId, userId, userName, fbProfile.UserFullName, ExtendedAccessToken, null, 1, true, fbProfile.ProfilePicURL, fbProfile.ProfileURI, type == "personal", false, false);
                    int transactId = transact.insertProfile();

                    if (transactId == -1)
                    {
                        if (url == null)
                            return Redirect("/#/GroupAndTeam?error=SPE");
                        else
                            return Redirect("/#/" + url + "?error=SPE");
                    }

                    if (url == null)
                        return Redirect("/#/GroupAndTeam");
                    else
                        return Redirect("/#/" + url);
                }
                else if (loginInfo.Login.LoginProvider == "LinkedIn")
                {
                    var claims = claimsIdentity.Claims
                        .Where(c => c.Type.StartsWith("urn:linkedin:access_token")).ToArray();

                    string userId = loginInfo.Login.ProviderKey;
                    string userName = loginInfo.DefaultUserName;
                    string name = loginInfo.ExternalIdentity.Name;
                    string accessToken = claims[0].Value;
                    ExtraSocialProfileInfo linkProfile = GetLinkedInExtraInfo(accessToken);

                    transact = new ProfileTransactions(companyId, groupId, memberUserId, userId, userName, linkProfile.UserFullName, accessToken, null, 3, true, linkProfile.ProfilePicURL, linkProfile.ProfileURI, type == "personal", false, false);
                    int transactId = transact.insertProfile();

                    if (transactId == -1)
                    {
                        if (url == null)
                            return Redirect("/#/GroupAndTeam?error=SPE");
                        else
                            return Redirect("/#/" + url + "?error=SPE");
                    }

                    if (url == null)
                        return Redirect("/#/GroupAndTeam");
                    else
                        return Redirect("/#/" + url);
                }
            }
            if (url == null)
                return Redirect("/#/GroupAndTeam");
            else
                return Redirect("/#/" + url);
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private string GetExtendedAccessToken(string ShortLivedToken)
        {
            FacebookClient client = new FacebookClient();
            string extendedToken = "";
            try
            {
                var clientId = ConfigurationManager.AppSettings["FacebookConsumerKey"].ToString();
                var clientSecret = ConfigurationManager.AppSettings["FacebookConsumerSecret"].ToString();
                dynamic result = client.Get("/oauth/access_token", new
                {
                    grant_type = "fb_exchange_token",
                    client_id = clientId,
                    client_secret = clientSecret,
                    fb_exchange_token = ShortLivedToken
                });
                extendedToken = result.access_token;
            }
            catch
            {
                extendedToken = ShortLivedToken;
            }
            return extendedToken;
        }

        private ExtraSocialProfileInfo GetFBExtraInfo(string accessToken, string Id)
        {
            FacebookClient client = new FacebookClient(accessToken);
            ExtraSocialProfileInfo extraInfo = new ExtraSocialProfileInfo();
            try
            {
                IDictionary<string, object> myProfile = (IDictionary<string, object>)client.Get("/" + Id + "?fields=picture.width(100).height(100),link,name");
                extraInfo.ProfilePicURL = ((JsonObject)((JsonObject)myProfile["picture"])["data"])["url"].ToString();
                extraInfo.ProfileURI = Convert.ToString(myProfile["id"]);
                extraInfo.UserFullName = Convert.ToString(myProfile["name"]);
            }
            catch
            {

            }
            return extraInfo;
        }

        private Models.Pinterest.Data GetPinExtraInfo(string accessToken)
        {
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(string.Format("https://api.pinterest.com/v1/me/?access_token={0}&fields=bio,first_name,last_name,id,image,username,url",accessToken));
                request.Method = "GET";
                string json = string.Empty;
                using (System.Net.WebResponse response = (System.Net.WebResponse)request.GetResponse())
                using (System.IO.Stream dataStream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(dataStream))
                {
                    json = reader.ReadToEnd();
                }
                var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Pinterest.RootObject>(json);
                return userData.data;
            }
            catch (Exception exp)
            {
                if (!exp.Message.Contains("400"))
                    throw;
                return null;
            }
        }

        private ExtraSocialProfileInfo GetLinkedInExtraInfo(string accessToken)
        {
            string LinkedInConsumerKey = ConfigurationManager.AppSettings["LinkedInConsumerKey"].ToString();
            string LinkedInConsumerSecret = ConfigurationManager.AppSettings["LinkedInConsumerSecret"].ToString();
            ExtraSocialProfileInfo extraInfo = new ExtraSocialProfileInfo();
            // create a configuration object
            var config = new LinkedInApiConfiguration(LinkedInConsumerKey, LinkedInConsumerSecret);

            // get the APIs client
            var api = new LinkedInApi(config);
            var user = new UserAuthorization(accessToken);
            Person profile = api.Profiles.GetMyProfile(user, null,
                            FieldSelector.For<Person>().WithPictureUrl().WithFirstName().WithLastName().WithPublicProfileUrl());
            extraInfo.ProfilePicURL = profile.PictureUrl;
            extraInfo.UserFullName = profile.Firstname + " " + profile.Lastname;
            extraInfo.ProfileURI = profile.PublicProfileUrl;

            return extraInfo;
        }

        private ExtraSocialProfileInfo GetTwitterExtraInfo(string accessToken, string accessTokenSecret)
        {
            string TwitterConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"].ToString();
            string TwitterConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"].ToString();
            // Initialize the Twitter client
            var service = new TwitterService(
                                    TwitterConsumerKey,
                                    TwitterConsumerSecret,
                                    accessToken,
                                    accessTokenSecret
                                );

            var profile = service.GetUserProfile(new GetUserProfileOptions());
            ExtraSocialProfileInfo extraInfo = new ExtraSocialProfileInfo();
            if (profile != null)
            {
                extraInfo.ProfilePicURL = profile.ProfileImageUrlHttps.Replace("normal", "reasonably_small");
                extraInfo.ProfileURI = "https://twitter.com/" + profile.ScreenName;
                extraInfo.UserFullName = profile.Name;
            }

            return extraInfo;
        }

        private bool pageHasAllPermissions(dynamic arr)
        {
            bool flag = true;
            string[] allPermissions = { "EDIT_PROFILE", "CREATE_CONTENT", "MODERATE_CONTENT", "CREATE_ADS", "BASIC_ADMIN" };
            foreach (string permit in allPermissions)
            {
                if (arr.Contains(permit))
                    continue;
                else
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}