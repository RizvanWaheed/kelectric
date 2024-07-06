using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using SocialCRM.Web.Models.User;
using CRM.Entities;
using CRM.Common.Enum;
using SocialCRM.Web.Repositories.Core;
using CRM.Common;

namespace SocialCRM.Web.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider()
        { }

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
         
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "Email or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed && !string.IsNullOrEmpty(user.ConfirmationCode))
            {
                context.SetError("activation_required", "user not activated through email.");
                return;
            }           
            var isAccountActivated = (!(user.EmailConfirmed) && string.IsNullOrEmpty(user.ConfirmationCode));
            //check if user is inactive hold it here and return error popup
            using (var db = new SocialCRMEntities())
            {
                var team = db.Teams.Where(x => x.UserId == user.Id).FirstOrDefault();
                if (team !=null && team.IsActive == false)
                {
                    context.SetError("activation_required", "This User is InActive, please contact Kelectric Administrator.");
                    return;
                }
            }

            bool isAdmin = false;
            var roles = userManager.GetRoles(user.Id);
            if (roles.Count > 0)
                if (roles[0] == "Admin")
                    isAdmin = true;

            bool trialExpired = false;
            await Task.Run(() =>
            {
                using (var db = new SocialCRMEntities())
                {
                    AspNetUser dbUser = db.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault();
                    UserCompany comp = dbUser.UserCompanies.FirstOrDefault();
                    if(comp != null)
                    {
                        if (comp.Company.SubscriberAccountType == (int)AccountSubscriberType.Trial)
                        {
                            if ((DateTime.Now - comp.Company.CreatedDate).TotalDays > 30 || comp.Company.Status == 0)
                            {
                                trialExpired = true;
                            }
                        }
                    }
                }
            });
            if (trialExpired)
            {
                context.SetError("activation_required", "User Trial Account Period Expired. Upgrade to the Paid version.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);
            AuthenticationProperties properties = CreateProperties(user.UserName, isAccountActivated);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);

            //Log Entry
            await Task.Run(() =>
            {
                    using (var db = new SocialCRMEntities())
                    {
                        var log = new Log
                        {
                            LogCodeId = Convert.ToInt32(CRM.Common.Enum.LogCodes.Signin),
                            UserId = user.Id,
                            IpAddress = CRM.Common.GeneralMethods.GetLocalIPAddress(),
                            CreatedDate = DateTime.Now
                        };
                        db.Logs.Add(log);

                        db.SaveChanges();
                        db.Dispose();
                    }
            });
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, bool isAccountActivated=true)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            data.Add("isAccountActivationRequired", isAccountActivated.ToString());
            return new AuthenticationProperties(data);
        }
    }
}