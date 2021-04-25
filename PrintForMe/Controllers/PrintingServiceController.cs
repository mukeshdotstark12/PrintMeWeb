using CMS.DocumentEngine.Types.PrintForMe;
using PrintForMe.Models.PrintingService;
using System.Linq;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class PrintingServiceController : Controller
    {
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        // GET: PrintingService
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetPrintingService()
        {
            var serviceItems = PrintingServicesProvider.GetPrintingServices()
               .Culture("en-US")
               .Columns("Name", "Image", "Link", "ColorCode", "PrintingServicesID")
               .CombineWithDefaultCulture()
               .OrderBy("NodeOrder");

            // Creates a collection of view models based on the menu item and page data
            var model = serviceItems.Select(item => new PrintingServiceModel()
            {
                Name = item.Name,
                Image = item.Image,
                Link = "/" + mCultureName + item.Link + "/?serviceId=" + item.PrintingServicesID,
                ColorCode = item.ColorCode
            });

            return PartialView("_printingService", model);
        }
    }
}