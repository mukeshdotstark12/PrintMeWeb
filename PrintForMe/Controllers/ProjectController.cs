using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.PrintForme;
using CMS.Helpers;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PrintForMe.Helpers;
using PrintForMe.Models;
using PrintForMe.Models.Menu;
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class ProjectController : Controller
    {
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyProjects()
        {
            int userid = HttpContext.GetOwinContext().Get<UserManager>().FindByName(User.Identity.Name).Id;
            if (userid == 0)
            {
                return null;
            }

            var model = AlbumDetailwithPrice.GetAlbumwihPrice(userid);
            if (model == null)
            {
                ViewBag.Status = @ResHelper.GetString("PrintForMe.NoAlbumFound");
                return View();
            }
            else
            {
                QueryDataParameters parameters = new QueryDataParameters();
                parameters.Add("@AlbumID", model.AlbumID);

                DataSet ds = ConnectionHelper.ExecuteQuery("Sp_Printforme_GetOrderItem", parameters, QueryTypeEnum.StoredProcedure);

                int count = Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]);
                if(count == 0)
                {
                    ViewBag.Status = @ResHelper.GetString("PrintForMe.NoAlbumFound");
                    return View();
                }
            }
            
            return View(model);
        }

        public ActionResult GetMenuItem(int num)
        {
            //UserModel.storeLogoPath = Helpers.ContentHelper.GetStoreImage();
            var path = "/Client";
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
                .Columns("MenuItemText", "MenuItemPage", "MenuItemIcon", "MenuItemLink")
                .OrderBy("NodeOrder");

            var model = menuItems.Select(item => new MenuItemViewModel()
            {
                MenuItemText = item.MenuItemText,
                MenuItemIcon = item.MenuItemIcon,
                MenuItemLink = "/" + mCultureName + item.MenuItemLink,
            });
            return PartialView("_PartialRightMenu", model);
        }
    }
}