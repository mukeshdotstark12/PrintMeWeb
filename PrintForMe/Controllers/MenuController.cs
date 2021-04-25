using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.PrintForme;
using CMS.DocumentEngine.Types.PrintForMe;
using PrintForMe.Models;
using PrintForMe.Models.Menu;
using PrintForMe.Models.PrintingService;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class MenuController : Controller
    {
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        // GET: Menu
        public ActionResult GetMenu(int num = 1)
        {
            UserModel.storeLogoPath = Helpers.ContentHelper.GetStoreImage();

            var path = "/Home";
            if (num == 2)
            {
                path = "/Admin";
            }
            else if (num == 3)
            {
                path = "/Client";
            }
            var menuItems = MenuItemProvider.GetMenuItems()
                .Path(path, PathTypeEnum.Children)
                .Culture(mCultureName)
                .WhereEquals("MenuItemEnabled",true)
                .Columns("MenuItemText", "MenuItemPage", "MenuItemIcon", "MenuItemLink")
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder");

            // Creates a collection of view models based on the menu item and page data
            var model = menuItems.Select(item => new MenuItemViewModel()
            {
                MenuItemText = item.MenuItemText,
                MenuItemIcon = item.MenuItemIcon,
                MenuItemLink = "/" + mCultureName + item.MenuItemLink,
                MenuItems = item.MenuItemText.Equals("Store") || item.MenuItemText.Equals("خدماتنا") ? GetMenuItems() : null
                // Gets the URL for the page whose GUID matches the given menu item's selected page
                //MenuItemRelativeUrl = pages.FirstOrDefault(page => page.NodeGUID == item.MenuItemPage).RelativeURL
            });
            ViewBag.Type = num;
            return PartialView("_siteMenu", model);
        }

        private IEnumerable<PrintingServiceModel> GetMenuItems()
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

            return model;

        }
    }
}