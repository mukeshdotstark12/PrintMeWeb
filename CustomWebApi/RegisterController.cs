using CMS.DataEngine;
using CMS;
using System.Web.Http;


[assembly: RegisterModule(typeof(CustomWebApi.RegisterController))]
namespace CustomWebApi
{
    public class RegisterController : Module
    {
        // Module class constructor, the system registers the module under the name "CMSWebAPI"
        public RegisterController()
        : base("CustomWebApi")
        {
        }

        // Contains initialization code that is executed when the application starts
        protected override void OnInit()
        {
            base.OnInit();

            // Registers a "customapi" route
            GlobalConfiguration.Configuration.Routes.MapHttpRoute("api", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
        }
    }
}
