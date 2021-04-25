using CMS.Helpers;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(PrintForMe.App_Start.Startup))]
namespace PrintForMe.App_Start
{
    public class Startup
    {
        // Cookie name prefix used by OWIN when creating authentication cookies
        private const string OWIN_COOKIE_PREFIX = ".AspNet.";

        public void Configuration(IAppBuilder app)
        {
            // Registers the Kentico.Membership identity implementation
            app.CreatePerOwinContext(() => UserManager.Initialize(app, new UserManager(new UserStore(CMS.SiteProvider.SiteContext.CurrentSiteName))));
            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);

            // Configures the authentication cookie
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                // Fill in the name of your sign-in action and controller
                LoginPath = new PathString(urlHelper.Action("SignIn", "Account")),
                Provider = new CookieAuthenticationProvider
                {
                    // Sets the return URL for the sign-in page redirect (fill in the name of your sign-in action and controller)
                    OnApplyRedirect = context => context.Response.Redirect(urlHelper.Action("SignIn", "Account")
                                                 + new Uri(context.RedirectUri).Query)
                }
            });

            // Registers the authentication cookie with the 'Essential' cookie level
            // Ensures that the cookie is preserved when changing a visitor's allowed cookie level below 'Visitor'
            CookieHelper.RegisterCookie(OWIN_COOKIE_PREFIX + DefaultAuthenticationTypes.ApplicationCookie, CookieLevel.Essential);

            // Uses a cookie to temporarily store information about users signing in via external authentication services
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Registers the Facebook authentication service
            //app.UseFacebookAuthentication(
            //    new FacebookAuthenticationOptions
            //    {
            //        // Fill in the application ID and secret of your Facebook authentication application
            //        AppId = "placeholder",
            //        AppSecret = "placeholder"
            //    });

            // Registers the Google authentication service
            app.UseGoogleAuthentication(
                new GoogleOAuth2AuthenticationOptions
                {
                    // Fill in the client ID and secret of your Google authentication application
                    ClientId = "183204408584-6ctguos8f6logpo393hkl1lrofltetem.apps.googleusercontent.com",
                    ClientSecret = "XhVr4zucd_IS0ZEfdC96vN7L"
                });

            // Registers the Twitter authentication service
            //app.UseTwitterAuthentication(
            //    new TwitterAuthenticationOptions
            //    {
            //        // Fill in the Consumer ID and secret of your Twitter authentication application
            //        ConsumerKey = "placeholder",
            //        ConsumerSecret = "placeholder"
            //    });

        }
    }
}