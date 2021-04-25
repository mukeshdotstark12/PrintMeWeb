using System.Web.Optimization;

namespace PrintForMe
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Custom JavaScript files from the ~/Scripts/ directory can be included as well
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/site.js",
                        "~/Scripts/ej2/ej2.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/jquery-unobtrusive-ajax").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/proper.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/addons/rating.min.js"
                      ));

            // Custom CSS files from the ~/Content/ directory can be included as well
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/media-queries.css",
                        "~/Content/bootstrap.css",
                        "~/Content/addons/rating.min.css",
                        "~/Content/icons-style-sheet.css",
                        "~/Content/site.min.css"
                        ));

            bundles.Add(new StyleBundle("~/Content/css1").Include(
                        "~/Content/Site.css",
                        "~/Content/bootstrap.css"
                        ));

            //bundles.Add(new StyleBundle("~/Content/admin-css").Include(
            //"~/Content/admin-style-sheet.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}