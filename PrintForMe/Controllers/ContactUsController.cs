using CMS.DocumentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using System.Linq;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class ContactUsController : Controller
    {
        // GET: ContactUs
        public ActionResult Index()
        {
            // Retrieves the page from the Kentico database
            TreeNode page = DocumentHelper.GetDocuments()
                .Path("/Contact-Us")
                .OnCurrentSite()
                .TopN(1)
                .FirstOrDefault();

            // Returns a 404 error when the retrieving is unsuccessful
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Builder = true;
            // Initializes the page builder with the DocumentID of the page
            HttpContext.Kentico().PageBuilder().Initialize(page.DocumentID);

            return View();
        }
    }
}