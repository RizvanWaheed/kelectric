using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SocialCRM.Web.Repositories;
using System.Web.Mvc;
using SocialCRM.Web.GoogleAnalytics;
using CRM.Common.Enum;
using Microsoft.AspNet.Identity;
using System.Web.Script.Serialization;
using SocialCRM.Web.Models.GoogleAnalytics;

namespace SocialCRM.Web.Controllers.API
{
    [Authorize]
    [RoutePrefix("Analytics")]
    public class AnalyticsController : Controller
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
        /// <summary>
        /// this action return the google analytic data, users,sessions,bounce rate etc.
        /// </summary>
        /// <param name="analyticAccountId">companyId</param>
        /// <param name="dateFilter">groupId</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAnalytics(int parm1, DateFilter parm2)
        {
            string userInfo = string.Empty;
            var analyticAccountId = parm1;
            var dateFilter = parm2;
            var analyticAccounts = GoogleAnalyticRepostory.GetUserAnalyticAccountById(analyticAccountId);
            if (analyticAccounts == null)
            {
                return Json(new Exception("No Account Found."), JsonRequestBehavior.AllowGet);
            }
            userInfo = string.Concat(analyticAccounts.UserId, ",", analyticAccounts.CompanyId, ",", analyticAccounts.GroupId, ",", analyticAccounts.AccountNumber);
            if (this.Session["UserAnalyticInfo"] == null)
            {
                this.Session.Add("UserAnalyticInfo", userInfo);
            }
            var result = new Google.Apis.Auth.OAuth2.Mvc.AuthorizationCodeMvcApp(this, new SocialCRM.Web.GoogleAnalytics.AppFlowMetadata()).AuthorizeAsync(System.Threading.CancellationToken.None).Result;
            if (result.Credential == null)
            {
                return new RedirectResult(result.RedirectUri);
            }
            this.Session.Remove("UserAnalyticInfo");
            var analyticsDetails = new AnalyticsHelper(result.Credential).GetAnalyticsByAccountId(analyticAccounts, dateFilter);
            return Json(analyticsDetails, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This action return the google analytics account that are associated to company and specific group.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAnalyticAccounts(int parm1, string parm2)
        {
            var user = User.Identity.Name;
            var companyId = parm1;
            List<int> groupIds = string.IsNullOrEmpty(parm2.Replace("\"\"","")) ? new List<int>() : parm2.Split(',').Select(int.Parse).ToList();
            var analyticAccounts = GoogleAnalyticRepostory.GetUserAnalyticAccounts(user, companyId, groupIds);
            if (analyticAccounts == null)
            {
                return Json(new Exception("No Account Found."), JsonRequestBehavior.AllowGet);
            }
            return Json(analyticAccounts, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This action is used to fetch the graph data.
        /// </summary>
        /// <param name="analyticAccountId">parm1</param>
        /// <param name="dateFilter">parm2</param>
        /// <returns>GraphResultSet</returns>
        [HttpGet]
        //[Route("GetAnalyticsChart/{analyticAccountId}/{dateFilter}/{customDateFilter}")]
        public ActionResult GetAnalyticsChart(int parm1, DateFilter parm2, string parm3)
        {
            string userInfo = string.Empty;
            var analyticAccountId = parm1;
           var dateFilter = parm2;
           var customDateFilter = parm3;
            var analyticAccounts = GoogleAnalyticRepostory.GetUserAnalyticAccountById(analyticAccountId);
            if (analyticAccounts == null)
            {
                return Json(new Exception("No Account Found."), JsonRequestBehavior.AllowGet);
            }
            userInfo = string.Concat(analyticAccounts.UserId, ",", analyticAccounts.CompanyId, ",", analyticAccounts.GroupId, ",", analyticAccounts.AccountNumber);
            if (this.Session["UserAnalyticInfo"] == null)
            {
                this.Session.Add("UserAnalyticInfo", userInfo);
            }
            var result = new Google.Apis.Auth.OAuth2.Mvc.AuthorizationCodeMvcApp(this, new SocialCRM.Web.GoogleAnalytics.AppFlowMetadata()).AuthorizeAsync(System.Threading.CancellationToken.None).Result;
            if (result.Credential == null)
            {
                return new RedirectResult(result.RedirectUri);
            }
            this.Session.Remove("UserAnalyticInfo");

            var analyticsDetails = new AnalyticsHelper(result.Credential).GetAnalyticsChartsByAccountId(analyticAccounts, dateFilter,customDateFilter);
            return Json(analyticsDetails, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get user analytics account information against selected company & group.
        /// </summary>
        /// <param name="parm1"></param>
        /// <param name="parm2"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetUserAnalyticAccounts(string parm1)
        {
            if (string.IsNullOrEmpty(parm1) || !parm1.Contains(","))
                return Json("Invalid Request.",JsonRequestBehavior.AllowGet);
            var requestInfo = parm1.Split(',').Select(int.Parse).ToArray();
            //User,Company,Group
            if(requestInfo.Count()<3)
                return Json("Invalid Request.", JsonRequestBehavior.AllowGet);
            var userId = this.User.Identity.GetUserId<int>();
            if(userId!=requestInfo[0])
                return Json("Invalid Request.", JsonRequestBehavior.AllowGet);
            var companyId = requestInfo[1];
            var groupId = requestInfo[2];
            var analyticAccounts = GoogleAnalyticRepostory.GetUserAnalyticAccounts(userId, companyId, groupId);//.Select(x => new { PageName = x.AccountName, PageURL = x.AccountUrl, id = x.Id,Isconnected=x.IsConnected, PagePicURL = "/Content/images/soc_ico.jpg" });
            if (analyticAccounts == null)
            {
                return Json(new Exception("No Account Found."), JsonRequestBehavior.AllowGet);
            }
            return Json(analyticAccounts, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddAnalyticsAccount(string parm1)
        {
            var result = new JavaScriptSerializer().Deserialize<List<AnalyticCompanyUserModel>>(parm1);
            if (result.Count > 0)
            {
                var response = GoogleAnalyticRepostory.ActivateAnalyticAccount(result);
                return Json(new { Status = "success", Result = response }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = "error", Result = "account info req." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("GetAnalyticsChartAndAccount/{groupId}")]
        public ActionResult GetAnalyticsChartAndAccount(int groupId)
        {
            var userId = User.Identity.GetUserId<int>();
            var accounts = GoogleAnalyticRepostory.GetUserAnalyticAccounts(userId,groupId);
            if (accounts == null || accounts.Count==0)
            {
                return Json(new Exception("No Account Found."), JsonRequestBehavior.AllowGet);
            }
            string userInfo = string.Empty;
            var FirstAccount = accounts.First();
            userInfo = string.Concat(FirstAccount.OwnerUserId, ",", FirstAccount.CompanyId, ",", groupId, ",", FirstAccount.AccountNumber);
            if (this.Session["UserAnalyticInfo"] == null)
            {
                this.Session.Add("UserAnalyticInfo", userInfo);
            }
            var result = new Google.Apis.Auth.OAuth2.Mvc.AuthorizationCodeMvcApp(this, new SocialCRM.Web.GoogleAnalytics.AppFlowMetadata()).AuthorizeAsync(System.Threading.CancellationToken.None).Result;
            if (result.Credential == null)
            {
                return new RedirectResult(result.RedirectUri);
            }
            this.Session.Remove("UserAnalyticInfo");
            var analyticsDetails = new AnalyticsHelper(result.Credential).GetAnalyticsChartsByAccountId(FirstAccount.AccountId,FirstAccount.ProfileId, DateFilter.Last15Days);
            return Json(new {Analytics=analyticsDetails, Accounts=accounts }, JsonRequestBehavior.AllowGet);
        }
    }
}
