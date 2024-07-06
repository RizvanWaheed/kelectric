using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
namespace SocialCRM.Web
{
    public static class WebApiConfig
    {
        public static string UrlPrefix { get { return "api"; } }
        public static string UrlPrefixRelative { get { return "~/api"; } }
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(
                name: "DefaultApi2",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "{controller}", action = "{action}", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ApiRoute2Parm",
                routeTemplate: "api/{controller}/{action}/{id}/{param}",
                defaults: new { controller = "{controller}", action = "{action}", id = RouteParameter.Optional, param=RouteParameter.Optional }
            );
            EnableCrossSiteRequests(config);
        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*");
            config.EnableCors(cors);
        }
    }
}
