using CMS.Helpers;
using CMS.Membership;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PrintForMe.Models.User;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers.Admin
{
    [Authorize]
    public class ProfileController : Controller
    {
        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }

        // GET: Admin/Profile        
        public ActionResult Profile()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            var u = UserInfoProvider.GetUserInfo(user.UserName);
            UserModel model = new UserModel();
            // Checks whether the model contains any records
            if (!DataHelper.DataSourceIsEmpty(u))
            {
                model = new UserModel(u);
            }
            ViewBag.Gender = new string[] { "Male", "Female" };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Profile(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Fill the missing info");
                return View("Profile", "Profile");
            }
            try
            {
                var userid = UserManager.FindByName(User.Identity.Name).Id;

                var user = UserInfoProvider.GetUserInfo(userid);
                // update user info
                //user.FullName = model.FullName;
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.SetValue("MobileNumber", model.MobileNumber);
                user.SetValue("Gender", model.Gender);
                user.Email = model.Email;

                //Save the changes to the db
                UserInfoProvider.SetUserInfo(user);
                ModelState.Clear();
                ViewBag.Success = "User Profile Updated Successfully";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                ViewBag.Success = "Error Updating User Profile. Please try after sometime.";
                return View();
            }
        }
    }
}