using System.Web;
using System.Web.Optimization;

namespace SocialCRM.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      
                      "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));


            bundles.Add(new StyleBundle("~/Content/CRMcss").Include(
                 "~/Content/css/bootstrap.css",
                 "~/Content/css/font-awesome.css",
                 "~/Content/css/font-awesome.min.css",
                 "~/Content/css/signup_styles.css"));

            bundles.Add(new ScriptBundle("~/bundles/CRMjs").Include(
                "~/Content/js/jquery.min.js",
                "~/Content/js/bootstrap.js"));


            bundles.Add(new ScriptBundle("~/bundles/CRMApp")
    .IncludeDirectory("~/Scripts/Controllers", "*.js")
    .Include("~/Scripts/CRMApp.js"));



            bundles.Add(new Bundle("~/bundles/Angular/Controllers")
                            .IncludeDirectory("~/Scripts/Controllers", "*.js")

                //.Include("~/Scripts/AdminApp.js")                
                );
            //bundles.Add(new Bundle("~/bundles/Angular/Factories")
            //                            .IncludeDirectory("~/Scripts/Factories", "*.js")
             
            //                );

            bundles.Add(new Bundle("~/bundles/Angular/App")
                .Include("~/Scripts/CRMApp.js")
                );

            bundles.Add(new Bundle("~/bundles/Angular/Services")
                .IncludeDirectory("~/Scripts/Services/", "*.js")
                );

            bundles.Add(new Bundle("~/bundles/Angular/Directives")
                .IncludeDirectory("~/Scripts/Directives/", "*.js")
                );
            //bundles.Add(new Bundle("~/bundles/Angular/Constants")
            //    .IncludeDirectory("~/Scripts/Constants/", "*.js")
            //    );


            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                      "~/Scripts/Angular Resources/angular.min.js",
                      "~/Scripts/Angular Resources/angular-animate.min.js",
                      "~/Scripts/ui-bootstrap-tpls-1.0.3.min.js",
                      "~/Scripts/Angular Resources/angular-messages.min.js",
                      "~/Scripts/Angular Resources/angular-aria.min.js",                      
                      "~/Scripts/Angular Resources/angular-material.min.js",
                      "~/Scripts/Angular Resources/angular-ui-router.js",
                      "~/Scripts/Angular Resources/highcharts-ng.js",
                      "~/Scripts/Angular Resources/angular-cookies.min.js",
                      "~/Scripts/Angular Resources/angular-block-ui.min.js",
                      "~/Scripts/Angular Resources/angular-notify.min.js",
                      "~/Scripts/Angular Resources/ngAutocomplete.js",
                      "~/Scripts/Angular Resources/angular-file-upload.min.js",
                      "~/Scripts/Angular Resources/angular-sanitize.min.js",
                      "~/Scripts/Angular Resources/ngMap.js"
                      ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
