using CRM.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SocialCRM.Web.Models.Pinterest
{
    public class PinterestModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("user")]
        public PinterestUser User { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public class PinterestUser
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("username")]
            public string UserName { get; set; }
            [JsonProperty("first_name")]
            public string FirstName { get; set; }
            [JsonProperty("last_name")]
            public string LastName { get; set; }
            [JsonProperty("image")]
            public string ProfilePicture { get; set; }
            [JsonProperty("bio")]
            public string Bio { get; set; }
            [JsonProperty("url")]
            public string ProfileUrl { get; set; }
        }
    }

    public class Dimension
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    public class Image
    {
        [JsonProperty("60x60")]
        public Dimension size { get; set; }
    }

    public class Data
    {
        public string username { get; set; }
        public string bio { get; set; }
        public string url { get; set; }
        public Image image { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string id { get; set; }
    }

    public class RootObject
    {
        public string access_token { get; set; }
        public Data data { get; set; }
    }

    public class PinterestPostModel
    {
        public string PostId { get; set; }
        public string PostUrl { get; set; }
        public string PostCaption { get; set; }
        public DateTime PostDate { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorProfilePicture { get; set; }
        public List<InstagramComment> CommentsList { get; set; }

    }
}