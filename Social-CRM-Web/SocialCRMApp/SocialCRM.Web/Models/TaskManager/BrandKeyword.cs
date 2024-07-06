using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialCRM.Web.Models.TaskManager
{
    public class BrandKeywordViewModel
    {
        public int Id { get; set; }
        public int BrandKeywordCategoryId { get; set; }
        public string KeywordText { get; set; }
        public string KeywordName { get; set; }
        public string LocationName { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLng { get; set; }
        public bool IsTag { get; set; }
        public string FontColor { get; set; }
        public string BGColor { get; set; }
        public string BrandKeywordCategoryText { get; set; }
}
    public class BrandKeywordModel
    {
        public string KeywordText { get; set; }
        public string KeywordName { get; set; }
        public string LocationName { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLng { get; set; }
        public int GroupId { get; set; }
        public int Id { get; set; }
        public int BrandKeywordCategoryId { get; set; }
        public int UserId { get; set; }
        public bool IsTag { get; set; }
        public string FontColor { get; set; }
        public string BGColor { get; set; }
    }

    public class BrandKeywordCategoryModel
    {
        public int Id { get; set; }
        public string BrandKeywordCategoryText { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string FontColor { get; set; }
        public string BGColor { get; set; }
    }
}