
using Google.Apis.Auth.OAuth2.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SocialCRM.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Mining()
        {
            return View("~/Views/Mining/MiningDataView.cshtml");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View("~/Views/Account/Login.cshtml");
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

      
    }
}