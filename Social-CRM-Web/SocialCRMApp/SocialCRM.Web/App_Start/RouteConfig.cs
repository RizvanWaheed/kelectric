using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SocialCRM.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               name: "GoogleAnalyticsProfile",
               url: "Profile/{action}/{companyId}/{groupId}",
               defaults: new { controller = "Profile", action = "{action}", companyId = UrlParameter.Optional, groupId = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "GoogleAnalyticsAccount",
                url: "Analytics/{action}/{parm1}/{parm2}/{parm3}",
                defaults: new { controller = "Analytics", action = "{action}", parm1 = UrlParameter.Optional, parm2 = UrlParameter.Optional, parm3 = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id =UrlParameter.Optional }
            );
        }
    }
}
