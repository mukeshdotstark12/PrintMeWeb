using Kentico.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PrintForMe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjM5MTI4QDMxMzgyZTMxMmUzMFFuUG5ibkZtZjR6eTNmZkZYOVg5dExFZU52ekRzbGdVcHhlWnhKRFlhZWM9");
            // Enables and configures selected Kentico ASP.NET MVC integration features
            ApplicationConfig.RegisterFeatures(ApplicationBuilder.Current);

            // Registers routes including system routes for enabled features
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Registers enabled bundles
            BundleConfig.RegisterBundles(BundleTable.Bundles);


        }
    }
}
