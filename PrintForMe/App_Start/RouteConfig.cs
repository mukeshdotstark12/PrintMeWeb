using Kentico.Web.Mvc;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace PrintForMe
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var defaultCulture = CultureInfo.GetCultureInfo("en-US");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Maps routes to Kentico HTTP handlers and features enabled in ApplicationConfig.cs
            // Always map the Kentico routes before adding other routes. Issues may occur if Kentico URLs are matched by a general route, for example images might not be displayed on pages
            routes.Kentico().MapRoutes();

            var route = routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { culture = defaultCulture.Name, controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { culture = new SiteCultureConstraint() }
            );

            // Assigns a custom route handler to the route
            route.RouteHandler = new MultiCultureMvcRouteHandler();
        }
    }
}
