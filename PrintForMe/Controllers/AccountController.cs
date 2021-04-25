using CMS.ContactManagement;
using CMS.Ecommerce;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PrintForMe.Models;
using PrintForMe.Models.Account;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Provides access to the Kentico.Membership.SignInManager instance.
        /// </summary>

        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }

        public SignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<SignInManager>();
            }
        }
        /// <summary>
        /// Provides access to the Microsoft.Owin.Security.IAuthenticationManager instance.
        /// </summary>
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        /// <summary>
        /// Basic action that displays the sign-in form.
        /// </summary>
        public ActionResult SignIn()
        {

            return View();
        }

        /// <summary>
        /// Handles authentication when the sign-in form is submitted. Accepts parameters posted from the sign-in form via the SignInViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> SignIn(SignInViewModel model, string returnUrl)
        {
            // Validates the received user credentials based on the view model
            if (!ModelState.IsValid)
            {
                // Displays the sign-in form if the user credentials are invalid
                return View();
            }

            // Attempts to authenticate the user against the Kentico database
            SignInStatus signInResult = SignInStatus.Failure;
            try
            {
                signInResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.SignInIsPersistent, false);
            }
            catch (Exception ex)
            {
                // Logs an error into the Kentico event log if the authentication fails
                EventLogProvider.LogException("MvcApplication", "SignIn", ex);
            }

            // If the authentication was not successful, displays the sign-in form with an "Authentication failed" message
            if (signInResult != SignInStatus.Success)
            {
                ModelState.AddModelError(String.Empty, "Authentication failed");
                return View();
            }

            // If the authentication was successful, redirects to the return URL when possible or to a different default action
            string decodedReturnUrl = Server.UrlDecode(returnUrl);
            if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
            {
                return Redirect(decodedReturnUrl);
            }

            var userid = UserManager.FindByName(model.UserName).Id;

            var user = UserInfoProvider.GetUserInfo(userid);

            Session["Data"] = user;

            //Setting Static Variables for User Info
            UserModel.userName = user.FirstName;
            UserModel.userID = user.UserID;
            //Setting Store Image Url
            UserModel.storeLogoPath = Helpers.ContentHelper.GetStoreImage();

            if (user.IsInRole("Admin",SiteContext.CurrentSiteName))
            {
                return RedirectToAction("Dashboard", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Action for signing out users. The Authorize attribute allows the action only for users who are already signed in.
        /// </summary>
        [Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            Session.Abandon();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["Data"] = null;
            UserModel.userID = 0;
            UserModel.userName = string.Empty;
            // Redirects to a home page after the sign-out
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/RetrievePassword
        public ActionResult RetrievePassword()
        {
            return View("RetrievePassword");
        }
        // POST: Account/RetrievePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetrievePassword(RetrievePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RetrievePassword", model);
            }

            var user = UserManager.FindById(ValidationHelper.GetInteger(User.Identity.GetUserId(), 0));
            if (user.Email.ToUpper().Trim() == model.Email.ToUpper().Trim())
            {
                var token = UserManager.GeneratePasswordResetToken(user.Id);
                var url = Url.Action("ResetPassword", "Account", new { userId = user.Id, token }, RequestContext.URL.Scheme);
                //MacroContext.GlobalResolver.SetNamedSourceData("ReturnUrl", url);
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.EmailFormat = EmailFormatEnum.Default;
                emailMessage.Recipients = user.Email;
                emailMessage.From = "Testdev@ltechpro.com"; //SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress");            
                emailMessage.Body = "Please change your password by clicking <a target=\"_blank\" href='" + url + "'>here</a>";
                emailMessage.Subject = ResHelper.GetString("PrintForMe.PasswordReset.Email.Subject");
                EmailSender.SendEmail(SiteContext.CurrentSiteName, emailMessage, true);
            }
            return Content(ResHelper.GetString("PrintForMe.PasswordReset.EmailSent"));
        }

        // GET: Account/ResetPassword
        public ActionResult ResetPassword(int? userId, string token)
        {
            if (!userId.HasValue || String.IsNullOrEmpty(token))
            {
                return HttpNotFound();
            }

            if (!VerifyPasswordResetToken(userId.Value, token))
            {
                return View("ResetPasswordInvalidToken");
            }

            var model = new ResetPasswordViewModel()
            {
                UserId = userId.Value,
                Token = token
            };

            return View("~/Views/PasswordReset/ResetPassword.cshtml");
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Register(RegisterCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usersEmail = UserInfoProvider.GetUsers().Column("Email").WhereEquals("Email", model.Email.Trim());
            if (usersEmail.Count > 0)
            {
                string errorMessage = ResHelper.GetString("PrintForMe.EmailAlreadyExist");
                ModelState.AddModelError("", errorMessage);

                return View(model);
            }

            if (model.UserName == "" || model.UserName == null)
            {
                model.UserName = model.MobileNumber;
            }

            var user = new User
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };
            var registerResult = new IdentityResult();

            try
            {
                registerResult = await UserManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("AccountController", "Register", ex);
                ModelState.AddModelError(String.Empty, ResHelper.GetString("register_failuretext"));
            }

            if (registerResult.Succeeded)
            {
                var u = UserInfoProvider.GetUserInfo(model.UserName);

                UserInfoProvider.AddUserToRole(user.UserName, "Customer", SiteContext.CurrentSiteName);

                u.SetValue("MobileNumber", model.MobileNumber);
                UserInfoProvider.SetUserInfo(u);

                //await SignInManager.SignInAsync(user, true, false);

                ContactManagementContext.UpdateUserLoginContact(model.UserName);

                if (u != null)
                {
                    SendRegistrationEmail(u);
                }
                //// Creates and sends the confirmation email to the user's address
                //await UserManager.SendEmailAsync(user.Id, "Confirm your new account",
                //    String.Format("Please confirm your new account by clicking <a target=\"_blank\" href=\"{0}\">here</a>", confirmationUrl));

                // Displays a view asking the visitor to check their email and confirm the new account
                return View("CheckYourEmail", model);


                //AuthenticationHelper.SendRegistrationEmails(u, "User Registration", true , true);

                //return RedirectToAction("Index", "Home");
            }

            foreach (var error in registerResult.Errors)
            {
                string errorMessage = error;
                if (error.EndsWith("is already taken."))
                {
                    errorMessage = ResHelper.GetString("PrintForMe.MobileAlreadyExist");
                    ModelState.AddModelError("", errorMessage);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, error);
                }

            }

            return View(model);
        }

        /// <summary>
        /// Action for confirming new user accounts. Handles the links that users click in confirmation emails.
        /// </summary>
        public async Task<ActionResult> ConfirmUser(int userId, string token)
        {
            IdentityResult confirmResult;

            if (string.IsNullOrEmpty(token) && ValidationHelper.GetInteger(userId, 0) != 0)
            {
                token = await UserManager.GenerateEmailConfirmationTokenAsync(ValidationHelper.GetInteger(userId, 0));
            }

            try
            {
                // Verifies the confirmation parameters and enables the user account if successful
                confirmResult = await UserManager.ConfirmEmailAsync(userId, token);
            }
            catch (InvalidOperationException)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                confirmResult = IdentityResult.Failed("User not found.");
            }

            if (confirmResult.Succeeded)
            {
                var userInfo = UserInfoProvider.GetUserInfo(userId);

                // Creates a new customer object
                CustomerInfo newCustomer = new CustomerInfo
                {
                    CustomerFirstName = userInfo.FirstName,
                    CustomerLastName = userInfo.LastName,
                    CustomerEmail = userInfo.Email,
                    CustomerSiteID = SiteContext.CurrentSiteID,
                    CustomerUserID = userInfo.UserID,
                    CustomerPhone = userInfo.UserName
                };

                // Saves the registered customer to the database
                CustomerInfoProvider.SetCustomerInfo(newCustomer);

                // If the verification was successful, displays a view informing the user that their account was activated
                return RedirectToAction("SignIn", "Account");
            }

            // Returns a view informing the user that the email confirmation failed
            return View("EmailConfirmationFailed");
        }

        /// <summary>
        /// Verifies if user's password reset token is valid.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="token">Password reset token.</param>
        /// <returns>True if user's password reset token is valid, false when user was not found or token is invalid or has expired.</returns>
        private bool VerifyPasswordResetToken(int userId, string token)
        {
            try
            {
                return UserManager.VerifyUserToken(userId, "ResetPassword", token);
            }
            catch (InvalidOperationException)
            {
                // User with given userId was not found
                return false;
            }
        }


        /// <summary>
        /// Reset user's password.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="token">Password reset token.</param>
        /// <param name="password">New password.</param>
        private IdentityResult ResetUserPassword(int userId, string token, string password)
        {
            try
            {
                return UserManager.ResetPassword(userId, token, password);
            }
            catch (InvalidOperationException)
            {
                // User with given userId was not found
                return IdentityResult.Failed("UserId not found.");
            }
        }

        public bool IsAdmin(int currentUser)
        {

            // Gets the user
            UserInfo user = UserInfoProvider.GetUserInfo("Username");

            bool checkGlobalRoles = true;
            bool checkMembership = true;

            // Checks whether the user is assigned to a role with the "Rolename" code name
            // The role can be assigned for the current site, as a global role, or indirectly through a membership
            bool result = user.IsInRole("Rolename", SiteContext.CurrentSiteName, checkGlobalRoles, checkMembership);

            return result;
        }

        private async void SendRegistrationEmail(UserInfo ui)
        {
            bool error = false;
            EmailTemplateInfo template = null;

            // Email message
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.EmailFormat = EmailFormatEnum.Default;
            emailMessage.Recipients = ui.Email;
            emailMessage.From = "Testdev@ltechpro.com"; //SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress");            

            template = EmailTemplateProvider.GetEmailTemplate("RegistrationConfirmation", SiteContext.CurrentSiteName);
            emailMessage.Subject = "Confirm your new account";

            if (template != null)
            {
                try
                {
                    string token = await UserManager.GenerateEmailConfirmationTokenAsync(ui.UserID);

                    // Fill in the name of your controller
                    string confirmationUrl = Url.Action("ConfirmUser", "Account", new { userId = ui.UserID, token = token }, protocol: Request.Url.Scheme);

                    MacroContext.GlobalResolver.SetNamedSourceData("ReturnUrl", confirmationUrl);

                    EmailSender.SendEmailWithTemplateText(SiteContext.CurrentSiteName, emailMessage, template, null, true);
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("Email", "RegistrationForm - SendEmail", ex);
                    error = true;
                }
            }

            // If there was some error, user must be deleted
            if (error)
            {

                // Email was not send, user can't be approved - delete it
                UserInfoProvider.DeleteUser(ui);
            }
        }
    }
}