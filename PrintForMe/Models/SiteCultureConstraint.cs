using CMS.SiteProvider;
using System.Web;
using System.Web.Routing;


public class SiteCultureConstraint : IRouteConstraint
{
    public bool Match(HttpContextBase httpContext,
                    Route route,
                    string parameterName,
                    RouteValueDictionary values,
                    RouteDirection routeDirection)
    {
        string cultureCodeName = values[parameterName]?.ToString();
        return CultureSiteInfoProvider.IsCultureOnSite(cultureCodeName, SiteContext.CurrentSiteName);
    }
}