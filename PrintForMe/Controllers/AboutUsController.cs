using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.PrintForme;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using PrintForMe.Models.AboutUs;
using System.Linq;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        // GET: AboutUs
        public ActionResult Index()
        {
            ViewBag.rtl = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;

            // Loads all menu items using the page type's generated provider
            // Uses the menu item order from the content tree in the Kentico 'Pages' application
            var aboutSections = AboutUsProvider.GetAboutUs()
                .Columns("Name", "Heading", "SubHeading", "Text", "Image")
                .Culture(mCultureName)
                .CombineWithDefaultCulture();

            // Loads the pages selected within the menu items
            // The data only contains values of the NodeGUID identifier column
            var pages = DocumentHelper.GetDocuments()
                .WhereIn("NodeGUID", aboutSections.Select(item => item.Name).ToList())
                .Columns("NodeGUID");


            // Creates a collection of view models based on the about us and page data
            var model = aboutSections.Select(item => new AboutUsViewModel()
            {
                Index = item.NodeOrder,
                Heading = item.Heading,
                SubHeading = item.SubHeading,
                Text = item.Text,
                ImageUrl = item.Image,

            });

            // Retrieves the page from the Kentico database
            TreeNode page = DocumentHelper.GetDocuments()
                .Path("/About-Us")
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

            return View("AboutUs", model);
        }


        // GET: AbouUsInGeneral
        public ActionResult GetFirstAboutSection()
        {
            // Loads all menu items using the page type's generated provider
            // Uses the menu item order from the content tree in the Kentico 'Pages' application
            var aboutSections = AboutUsProvider.GetAboutUs()
                .Columns("Name", "Heading", "Text", "Image")
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder")
                .TopN(1);

            // Loads the pages selected within the menu items
            // The data only contains values of the NodeGUID identifier column
            var pages = DocumentHelper.GetDocuments()
                .WhereIn("NodeGUID", aboutSections.Select(item => item.Name).ToList())
                .Columns("NodeGUID");

            // Creates a collection of view models based on the about us and page data
            var model = aboutSections.Select(item => new AboutUsViewModel()
            {
                Heading = item.Heading,
                SubHeading = item.SubHeading,
                Text = item.Text,
                ImageUrl = item.Image,

            });
            return PartialView("_aboutUs", model);
        }
    }
}