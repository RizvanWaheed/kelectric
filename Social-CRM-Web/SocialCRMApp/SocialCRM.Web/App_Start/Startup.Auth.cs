using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using SocialCRM.Web.Models.User;
using Microsoft.Owin.Security.OAuth;
using SocialCRM.Web.Providers;
using Microsoft.Owin.Security.Facebook;
using System.Security.Claims;
using Microsoft.Owin.Security.Twitter;
using Microsoft.Owin.Security;
using Owin.Security.Providers.LinkedIn;
using System.Configuration;

namespace SocialCRM.Web
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }
        public string TwitterConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"].ToString();
        public string TwitterConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"].ToString();
        public string TwitterAccessTokenClaimType = "urn:tokens:twitter:accesstoken";
        public string TwitterAccessTokenSecretClaimType = "urn:tokens:twitter:accesstokensecret";

        public string FacebookConsumerKey = ConfigurationManager.AppSettings["FacebookConsumerKey"].ToString();
        public string FacebookConsumerSecret = ConfigurationManager.AppSettings["FacebookConsumerSecret"].ToString();
        public const string FacebookAccessTokenClaimType = "urn:facebook:access_token";
        public const string LinkedInAccessTokenClaimType = "urn:linkedin:access_token";
        public string LinkedInConsumerKey = ConfigurationManager.AppSettings["LinkedInConsumerKey"].ToString();
        public string LinkedInConsumerSecret = ConfigurationManager.AppSettings["LinkedInConsumerSecret"].ToString();
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.MapSignalR();
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            var opt = new CookieAuthenticationOptions() {
            };
            opt.CookieName = ".AspNet.CRMCookies";
            app.UseCookieAuthentication(opt);
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);



            app.UseTwitterAuthentication(new TwitterAuthenticationOptions
            {
                ConsumerKey = TwitterConsumerKey,
                ConsumerSecret = TwitterConsumerSecret,
                BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(
                new[]
                {
                    "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                    "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                    "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                    "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                    "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
                    "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
                    "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
                })
                ,
                Provider = new TwitterAuthenticationProvider()
                {
                    OnAuthenticated = async context =>
                    {
                        context.Identity.AddClaim(new Claim(TwitterAccessTokenClaimType, context.AccessToken));
                        context.Identity.AddClaim(new Claim(TwitterAccessTokenSecretClaimType, context.AccessTokenSecret));
                    }
                }
            });

            var options = new FacebookAuthenticationOptions
            {
                AppId = FacebookConsumerKey,
                AppSecret = FacebookConsumerSecret,
            };

            options.Scope.Add("email,manage_pages,publish_pages,read_page_mailboxes,read_insights,user_posts");

            options.Provider = new FacebookAuthenticationProvider
            {
                OnAuthenticated = async context =>
                {
                    string accessToken = context.AccessToken;
                    context.Identity.AddClaim(new Claim(FacebookAccessTokenClaimType, context.AccessToken));

                }
            };

            app.UseFacebookAuthentication(options);

            var optionslinkedin = new LinkedInAuthenticationOptions
            {
                ClientId = LinkedInConsumerKey,
                ClientSecret = LinkedInConsumerSecret,
            };

            optionslinkedin.Provider = new LinkedInAuthenticationProvider
            {
                OnAuthenticated = async context =>
                {
                    string accessToken = context.AccessToken;
                    context.Identity.AddClaim(new Claim(LinkedInAccessTokenClaimType, context.AccessToken));

                }
            };

            app.UseLinkedInAuthentication(optionslinkedin);
        }
    }
}