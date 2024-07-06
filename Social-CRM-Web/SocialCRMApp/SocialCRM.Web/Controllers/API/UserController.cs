using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using SocialCRM.Web.Models.User;
using SocialCRM.Web.Repositories.Core;
using CRM.Common;
using System.Configuration;
using System.IO;
using SocialCRM.Web.Models.Profiles;
using CRM.Common.Extensions;
namespace SocialCRM.Web.Controllers.API
{

    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        /*
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        IUserRepository userRepository;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public UserController()
        {

        }
        public UserController(IUserRepository userRepo)
        {
            userRepository = userRepo;
        }
        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IUserRepository userRepo)
        {
            UserManager =userManager;
            SignInManager =  signInManager;
            userRepository = userRepo;
        }

        / *[HttpPost]
        [ResponseType(typeof(RequestStatus))]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            var response = new RequestStatus();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {ConfirmationCode=CodeGenerator.ConfirmationString(10),EmailConfirmed=false, UserName = model.Email, Email = model.Email, Name = string.Concat(model.FirstName, " ", model.LastName), ProfileImage = model.ImageUrl };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    model.UserId = UserManager.FindByEmail(user.Email).Id;
                    var dbOperation = await userRepository.RegisterUser(model);
                    if (dbOperation)
                    {
                        response.StatusCode = "0"; response.Message = "Success";
                        ///  await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        //Email to send link.
                        var emailBody = string.Format("Dear <b>{0}</b>,<br/>Please enter the below code to login to your Social CRM Account.<br/>{1}<br/> Sincerely,<br/>Social CRM Team", user.Name, user.ConfirmationCode);
                        var isEmailGenerated = new EmailHandler().SendEmail(model.Email, "Insight-CRM-360-SignUp", emailBody);
                    }
                    else
                    {
                        response.StatusCode = "1"; response.Message = "Fail";
                    }
                    return Json(response);
                }
            }
            return Json(response);
        }
        * /
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        public async Task<IHttpActionResult> AccountActivate(RegisterViewModel model)
        {
            var response = new RequestStatus();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByEmail(model.Email);
                if (user == null)
                {
                    response.StatusCode = "1";
                    response.Message = "User not Found.";
                    return Json(response);
                }
                else if (string.Compare(user.ConfirmationCode, model.ConfirmationCode) != 0)
                {
                    response.StatusCode = "1";
                    response.Message = "Invalid code.";
                    return Json(response);
                }
                else
                {
                    //User is Authenticated!
                    user.EmailConfirmed = true;
                    user.ConfirmationCode = string.Empty;
                    UserManager.Update(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    response.StatusCode = "0";
                    response.Message = "Success";
                    return Json(response);
                }
            }
            return Json(response);
        }
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        public async Task<IHttpActionResult> Login(LoginViewModel model)
        {
            var response = new RequestStatus();
            if (!ModelState.IsValid)
            {
                response.StatusCode = "1";
                response.Message = "UserName/Password Missing";
                return Json(response);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var user = UserManager.FindByEmail(model.Email);
            if (user==null||!user.EmailConfirmed)
            {
                response.StatusCode = "1";
                response.Message = "Account Verification Required.";
                return Json(response);
            }
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    response.StatusCode = "0";
                    response.Message = "Success";
                    break;
                case SignInStatus.LockedOut:
                    response.StatusCode = "1";
                    response.Message = "Lockout";
                    break;
                case SignInStatus.RequiresVerification:
                    response.StatusCode = "1";
                    response.Message = "RequiresVerification";
                    break;
                case SignInStatus.Failure:
                default:
                    response.StatusCode = "1";
                    response.Message = "Invalid login attempt";
                    break;
            }
            return Json(response);
        }
        [HttpPost]
        [ResponseType(typeof(RequestStatus))]
        public IHttpActionResult LogOut()
        {
            AuthenticationManager.SignOut();
            var response = new RequestStatus() {StatusCode="0",Message="Logout Successfully" };
            return Json(response);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                if (userRepository != null)
                {
                    userRepository = null;
                }
            }
            base.Dispose(disposing);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : System.Web.Mvc.HttpUnauthorizedResult
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

            public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        */

        IUserRepository userRepository;
        public UserController(IUserRepository userRepo)
        {
            userRepository = userRepo;
        }
        [Route("UpdatePersonalProfile")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdatePersonalProfile(PersonalProfileModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!string.IsNullOrEmpty(model.ProfileUrl))
            {
                var uploadedImage = model.ProfileUrl.Split(new[] { ',' }, 2);
                if (uploadedImage.Count() > 1)
                {
                    var serverPath = string.Concat(ConfigurationManager.AppSettings["ProfilePath"] + userId);
                    var filePath = HttpContext.Current.Server.MapPath(serverPath);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    var imageInfo = uploadedImage[0];
                    var imageData = uploadedImage[1];

                    if (imageInfo.ToLower().Contains("base64") && imageInfo.ToLower().Contains("im"))
                    {
                        var fileExtension ="jpeg";
                        if (imageInfo.Contains(";"))
                        {
                            fileExtension = imageInfo.Split('/')[1].Split(';')[0];
                        }
                        // Save the uploaded file to server
                        model.ProfileUrl = string.Format("{0}/{1}.{2}", serverPath, userId, fileExtension);
                        var fileName = string.Format("{0}\\{1}.{2}", filePath, userId, fileExtension);
                        var bytes = Convert.FromBase64String(uploadedImage[1]);
                        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(imageData)))
                        {
                            using (System.Drawing.Bitmap bm2 = new System.Drawing.Bitmap(ms))
                            {
                                bm2.Save(fileName);
                            }
                        }
                    }
                }
            }
            var success = false;
            success =await userRepository.UpdatePersonalProfile(model);
            return Ok(success);
        }

        [Route("GetPersonalProfile/{companyId}")]
        [HttpGet]
        public async Task<IHttpActionResult> PersonalProfile(int companyId)
        {
            var userId = User.Identity.GetUserId<int>();
            if (userId==0)
                return BadRequest("Invalid Operation");
            var personalProfile = await userRepository.GetPersonalProfile(userId, companyId);
            if (!string.IsNullOrEmpty(personalProfile.ProfileUrl))
            {
               personalProfile.ProfileUrl= HttpContext.Current.Request.GetFullUrl(personalProfile.ProfileUrl);
            }
            return Ok(personalProfile);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetUserLogs()
        {
            var usrLogs = await userRepository.GetUserLogs();

            return Ok(usrLogs);
        }

        [Route("GetBusinessProfile")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBusinessProfile()
        {
            var userId = User.Identity.GetUserId<int>();
            if (userId ==0)
                return BadRequest("Invalid Operation");
            var notificaitonDetails = await userRepository.GetBusinessProfile(userId);
            return Ok(notificaitonDetails);
        }
        [Route("UpdateNotificationSettings")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateNotificationSettings(NotificationViewModel model)
        {
            var userId = User.Identity.GetUserId<int>();
            var success = await userRepository.UpdateNotificationSettings(model,userId);
            return Ok(success);
        }

        [Route("UpdateBusinessProfileName")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateBusinessProfileName(NotificationViewModel model)
        {
            var success = await userRepository.UpdateBusinessProfileName(model);
            return Ok(success);
        }
    }
}
