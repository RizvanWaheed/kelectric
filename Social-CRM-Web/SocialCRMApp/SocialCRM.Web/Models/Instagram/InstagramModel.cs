using CRM.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.Instagram
{
    public class InstagramModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("user")]
        public InstagramUser User { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public class InstagramUser
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("username")]
            public string UserName { get; set; }
            [JsonProperty("full_name")]
            public string FullName { get; set; }
            [JsonProperty("profile_picture")]
            public string ProfilePicture { get; set; }
            [JsonProperty("bio")]
            public string Bio { get; set; }
            [JsonProperty("website")]
            public string WebSite { get; set; }
        }
    }
   
    public class User
    {
        public string username { get; set; }
        public string bio { get; set; }
        public string website { get; set; }
        public string profile_picture { get; set; }
        public string full_name { get; set; }
        public string id { get; set; }
    }

    public class RootObject
    {
        public string access_token { get; set; }
        public User user { get; set; }
    }

    public class InstagramPostModel
    {
        public string PostId { get; set; }
        public string PostUrl { get; set; }
        public string PostCaption { get; set; }
        public DateTime PostDate { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedDateForDeck { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorProfilePicture { get; set; }
        public List<InstagramComment> CommentsList { get; set; }

    }
}