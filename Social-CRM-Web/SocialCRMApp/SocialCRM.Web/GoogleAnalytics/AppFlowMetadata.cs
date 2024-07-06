using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Google.Apis.Analytics.v3; // Maybe this one too

namespace SocialCRM.Web.GoogleAnalytics
{
    public class AppFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow =
          new CustomAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
          {
              ClientSecrets = new ClientSecrets
              {
                  ClientId = ConfigurationManager.AppSettings["GAnalyticsClientId"].ToString(),
                  ClientSecret = ConfigurationManager.AppSettings["GAnalyticsClientSecret"].ToString()
              },
              Scopes = new String[] { AnalyticsService.Scope.AnalyticsReadonly, "email"},
              DataStore = new AnalyticsDataStore()
          });
        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
        public override String GetUserId(Controller controller)
        {
            // In this sample we use the session to store the user identifiers.
            // That's not the best practice, because you should have a logic to identify
            // a user. You might want to use "OpenID Connect".
            // You can read more about the protocol in the following link:
            // https://developers.google.com/accounts/docs/OAuth2Login.
            string user = "";

            try
            {
                //var userInfo = controller.User.Identity.GetUserId<int>();
                //if (HttpContext.Current != null)
                //{
                //    user = HttpContext.Current.Session["AccountName"] != null ? (string)HttpContext.Current.Session["AccountName"] : controller.User.Identity.GetUserName();
                  
                //}
                //else if (controller != null && controller.User != null && controller.User.Identity != null)
                //{
                //    user = controller.User.Identity.GetUserName();
                //}
                if (controller.Session["UserAnalyticInfo"] != null)
                {
                    user = (string)controller.Session["UserAnalyticInfo"];
                }
            }
            catch (Exception exp)
            {
                if (controller != null && controller.User != null && controller.User.Identity != null)
                    user = controller.User.Identity.GetUserName();
            }
            return user;
        }
    }
}