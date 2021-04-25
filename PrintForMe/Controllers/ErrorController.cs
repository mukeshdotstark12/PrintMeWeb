using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [HandleError]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}