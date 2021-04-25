using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

public class MultiCultureMvcRouteHandler : MvcRouteHandler
{
    protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
    {
        // Retrieves the requested culture from the route
        var cultureName = requestContext.RouteData.Values["culture"].ToString();

        try
        {
            // Creates a CultureInfo object from the culture code
            var culture = new CultureInfo(cultureName);

            // Sets the current culture for the MVC application
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }
        catch
        {
            // Handles cases where the culture parameter of the route is invalid
            // Returns a 404 status in this case, but you can also log an error, set a default culture, etc.
            requestContext.HttpContext.Response.StatusCode = 404;
        }

        return base.GetHttpHandler(requestContext);
    }
}