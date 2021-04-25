using CMS.Base;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CustomWebApi.Model.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomWebApi.Models;
using CustomWebApi.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Collections.ObjectModel;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Core;
using CMS.ContactManagement;
using CMS.WebAnalytics;
using CMS.MacroEngine;
using CMS.Localization;

namespace CustomWebApi.Controllers
{
    public class UserController : ApiController
    {

        #region GET        
        [HttpGet]
        public HttpResponseMessage Getuser(string userName)
        {
            try
            {
                // Gets the user
                UserInfo user = UserInfoProvider.GetUserInfo(userName);

                if (user != null)
                {
                    UserDetail userData = new UserDetail
                    {
                        UserID = user.UserID,
                        UserName = user.UserName,
                        Email = user.Email,
                        Enabled = user.Enabled,
                        FirstName = user.FirstName,
                        FullName = user.FullName,
                        LastLogon = user.LastLogon,
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        PreferredCultureCode = user.PreferredCultureCode,
                        UserCreated = user.UserCreated,
                        UserEnabled = user.UserEnabled,
                        UserGUID = user.UserGUID,
                        UserPicture = user.UserPicture
                    };
                    List<UserDetail> userArray = new List<UserDetail>();
                    userArray.Add(userData);

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = userArray,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "User not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Incorrect UserID..!"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetuserById(int userid)
        {
            try
            {
                // Gets the user
                UserInfo user = UserInfoProvider.GetUserInfo(userid);

                if (user != null)
                {
                    UserDetail userData = new UserDetail
                    {
                        UserID = user.UserID,
                        UserName = user.UserName,
                        Email = user.Email,
                        Enabled = user.Enabled,
                        FirstName = user.FirstName,
                        FullName = user.FullName,
                        LastLogon = user.LastLogon,
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        PreferredCultureCode = user.PreferredCultureCode,
                        UserCreated = user.UserCreated,
                        UserEnabled = user.UserEnabled,
                        UserGUID = user.UserGUID,
                        UserPicture = user.UserPicture,                       
                        UserCustomerID = CustomerInfoProvider.GetCustomerInfoByUserID(userid) == null ? 0 : CustomerInfoProvider.GetCustomerInfoByUserID(userid).CustomerID
                    };
                    List<UserDetail> userArray = new List<UserDetail>();
                    userArray.Add(userData);

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = userArray,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "User not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "User not found"
                });
            }
        }

        #endregion

        #region POST
        [HttpPost]
        public HttpResponseMessage Register(UserRegister userRegister)
        {
            try
            {
                var users = UserInfoProvider.GetUsers().Column("UserName").WhereEquals("UserName", userRegister.MobileNumber);
                if (users.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                    {
                        status = HttpStatusCode.BadRequest,
                        errorCode = HttpStatusCode.BadRequest.ToString(),
                        description = userRegister.MobileNumber + " Mobile number is already registered",
                    });
                }

                var usersEmail = UserInfoProvider.GetUsers().Column("Email").WhereEquals("Email", userRegister.Email);
                if (usersEmail.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                    {
                        status = HttpStatusCode.BadRequest,
                        errorCode = HttpStatusCode.BadRequest.ToString(),
                        description = "Email is already registered",
                    });
                }

                if (!string.IsNullOrEmpty(userRegister.MobileNumber) && !string.IsNullOrEmpty(userRegister.Password))
                {
                    // Creates a new user
                    UserInfo newUser = new UserInfo
                    {
                        // Sets the user properties
                        FullName = userRegister.FirstName + " " + userRegister.LastName,
                        FirstName = userRegister.FirstName,
                        LastName = userRegister.LastName,
                        UserName = userRegister.MobileNumber,
                        Email = userRegister.Email
                    };

                    newUser.SetValue("MobileNumber", userRegister.MobileNumber);

                    // Saves the user to the database
                    UserInfoProvider.SetUserInfo(newUser);
                    //set User Password
                    UserInfoProvider.SetPassword(newUser.UserName, userRegister.Password);

                    // Adds the user to the site
                    UserInfoProvider.AddUserToSite(newUser.UserName, SiteContext.CurrentSiteName);

                    // Creates a new customer object
                    //CustomerInfo newCustomer = new CustomerInfo
                    //{
                    //    CustomerFirstName = newUser.FirstName,
                    //    CustomerLastName = newUser.LastName,
                    //    CustomerEmail = newUser.Email,
                    //    CustomerSiteID = SiteContext.CurrentSiteID,
                    //    CustomerUserID = newUser.UserID,
                    //    CustomerPhone = userRegister.MobileNumber
                    //};

                    // Saves the registered customer to the database
                    //CustomerInfoProvider.SetCustomerInfo(newCustomer);

                    UserDetail userData = new UserDetail
                    {
                        UserID = newUser.UserID,
                        UserName = newUser.UserName,
                        Email = newUser.Email,
                        Enabled = newUser.Enabled,
                        FirstName = newUser.FirstName,
                        FullName = newUser.FullName,
                        LastLogon = newUser.LastLogon,
                        LastName = newUser.LastName,
                        MiddleName = newUser.MiddleName,
                        PreferredCultureCode = newUser.PreferredCultureCode,
                        UserCreated = newUser.UserCreated,
                        UserEnabled = newUser.UserEnabled,
                        UserGUID = newUser.UserGUID,
                        UserPicture = newUser.UserPicture,
                        UserCustomerID = 0//newCustomer.CustomerID
                    };

                    List<UserDetail> userArray = new List<UserDetail>();
                    userArray.Add(userData);

                    SendRegistrationEmail(newUser);

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = userArray,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                    {
                        status = HttpStatusCode.BadRequest,
                        errorCode = HttpStatusCode.BadRequest.ToString(),
                        description = "Please add your mobile number/password",
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage Update([FromBody] UserRegister userRegister)
        {
            try
            {
                if (!string.IsNullOrEmpty(userRegister.Email) && !string.IsNullOrEmpty(userRegister.UserName))
                {
                    UserInfo users = UserInfoProvider.GetUserInfo(userRegister.UserId);
                    if (users == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                        {
                            status = HttpStatusCode.BadRequest,
                            errorCode = HttpStatusCode.BadRequest.ToString(),
                            description = "User not found.",
                        });
                    }

                    // update a user                                          
                    users.FullName = userRegister.FirstName + " " + userRegister.LastName;
                    users.FirstName = userRegister.FirstName;
                    users.LastName = userRegister.LastName;
                    users.UserName = userRegister.UserName;
                    users.Email = userRegister.Email;

                    // Saves the user to the database
                    UserInfoProvider.SetUserInfo(users);

                    // Gets the first customer whose last name is 'Smith'
                    CustomerInfo customer = CustomerInfoProvider.GetCustomers()
                                                                    .WhereEquals("CustomerUserID", users.UserID)
                                                                    .TopN(1)
                                                                    .FirstOrDefault();

                    if (customer != null)
                    {
                        // Updates the customer's properties                        
                        customer.CustomerFirstName = userRegister.FirstName;
                        customer.CustomerLastName = userRegister.LastName;
                        customer.CustomerEmail = userRegister.Email;

                        // Saves the changes to the database
                        CustomerInfoProvider.SetCustomerInfo(customer);
                    }

                    List<UserInfo> user = new List<UserInfo>();
                    user.Add(users);

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = user,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "UserEmail and UserName is required",
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage PasswordUrlPass(int id, [FromBody] JObject dataObj)
        {
            var data = JsonConvert.DeserializeObject<PasswordData>(dataObj.ToString());

            if (data.Hash == Constants.PrintForMeHash)
            {

                //Get the user with the given id
                UserInfo ui = UserInfoProvider.GetUserInfo(id);
                if (ui != null)
                {
                    UserInfo user = null;
                    user = AuthenticationHelper.AuthenticateUser(ui.UserName, data.OldPassword, SiteContext.CurrentSiteName);

                    if (user != null)
                    {
                        UserInfoProvider.SetPassword(ui, data.NewPassword);

                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            status = HttpStatusCode.OK,
                            message = "Your password is set"
                        });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new customRagisterResponse
                        {
                            success = false,
                            result = "Old password is invalid",
                            Status = "Fail"
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new customRagisterResponse
                    {
                        success = false,
                        result = "User id is not valid",
                        Status = "Fail"
                    });
                }

            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new customRagisterResponse
                {
                    success = false,
                    result = "Hash Value is not valid",
                    Status = "Fail"
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage ForgetPassword([FromBody] JObject dataObj)
        {
            var data = JsonConvert.DeserializeObject<EmailData>(dataObj.ToString());

            if (data.Hash == Constants.PrintForMeHash)
            {
                if (!string.IsNullOrEmpty(data.Email) && ValidationHelper.IsEmail(data.Email))
                {
                    // Reset password
                    string siteName = SiteContext.CurrentSiteName;

                    // Prepare URL to which may user return after password reset
                    string returnUrl = "/Login";

                    UserInfo user = UserInfoProvider.GetUsers().WhereEquals("Email", data.Email).FirstOrDefault();
                    if (user == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new customResponse
                        {
                            success = false,
                            result = "Email is not in our database",
                            Status = "Failed"
                        });
                    }
                    AuthenticationHelper.ForgottenEmailRequest(data.Email, siteName, "Logon page", Constants.PrintForMeDefaultSMTP, null, AuthenticationHelper.GetResetPasswordUrl(siteName), returnUrl);
                    return Request.CreateResponse(HttpStatusCode.OK, new customResponse
                    {
                        success = true,
                        result = "Your request to reset passowrd has been sent",
                        Status = "Success"
                    });

                }
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, new customResponse
                    {
                        success = false,
                        result = "Email is not valid",
                        Status = "Failed"
                    });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new customResponse
                {
                    success = false,
                    result = "Hash value is not valid",
                    Status = "Failed"
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage Login([FromBody] UserLogin userLogin)
        {
            try
            {
                if (!string.IsNullOrEmpty(userLogin.MobileNumber))
                {
                    UserInfo user = null;

                    // Attempts to authenticate user credentials (username and password) against the current site
                    user = AuthenticationHelper.AuthenticateUser(userLogin.MobileNumber, userLogin.Password, SiteContext.CurrentSiteName);                    

                    if (user != null)
                    {
                        UserDetail userData = new UserDetail
                        {
                            UserID = user.UserID,
                            UserName = user.UserName,
                            Email = user.Email,
                            Enabled = user.Enabled,
                            FirstName = user.FirstName,
                            FullName = user.FullName,
                            LastLogon = user.LastLogon,
                            LastName = user.LastName,
                            MiddleName = user.MiddleName,
                            PreferredCultureCode = user.PreferredCultureCode,
                            UserCreated = user.UserCreated,
                            UserEnabled = user.UserEnabled,
                            UserGUID = user.UserGUID,
                            UserPicture = user.UserPicture,
                            UserCustomerID = CustomerInfoProvider.GetCustomerInfoByUserID(user.UserID) == null ? 0 : CustomerInfoProvider.GetCustomerInfoByUserID(user.UserID).CustomerID
                        };
                        List<UserDetail> userArray = new List<UserDetail>();
                        userArray.Add(userData);

                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            data = userArray,
                            status = HttpStatusCode.OK,
                            message = "Success"
                        });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new CustomResponse
                        {
                            status = HttpStatusCode.Unauthorized,
                            errorCode = HttpStatusCode.Unauthorized.ToString(),
                            description = "Incorrect Password!"
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "User not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.ToString()
                });
            }
        }


        private void SendRegistrationEmail(UserInfo ui)
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
                    var url = Constants.PrintForMeUrl + "/en-US/Account/ConfirmUser?userId=" + ui.UserID;

                    MacroContext.GlobalResolver.SetNamedSourceData("ReturnUrl", url);

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

        #endregion
    }
    #region Custom class
    public class EmailData
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }

    }
    public class customResponse
    {
        public bool success { get; set; }
        public string Status { get; set; }
        public string result { get; set; }
        public int orderid { get; set; }
    }

    public class customRagisterResponse
    {
        public bool success { get; set; }
        public string Status { get; set; }
        public string result { get; set; }
    }
    #endregion
}
