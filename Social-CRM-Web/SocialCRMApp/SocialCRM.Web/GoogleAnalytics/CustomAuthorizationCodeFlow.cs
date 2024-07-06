using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.GoogleAnalytics
{
    public class CustomAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
        public CustomAuthorizationCodeFlow(GoogleAuthorizationCodeFlow.Initializer initializer) : base(initializer) { }
        public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(String redirectUri)
        {
            return new GoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
            {
                ClientId = ClientSecrets.ClientId,
                Scope = string.Join(" ", Scopes),
                RedirectUri = redirectUri,
                AccessType = "offline",
                ApprovalPrompt = "force"
            };
        }
    }
}