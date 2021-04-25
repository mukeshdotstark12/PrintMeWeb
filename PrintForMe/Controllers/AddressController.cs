using CMS.Ecommerce;
using CMS.Globalization;
using CMS.Membership;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PrintForMe.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class AddressController : Controller
    {
        // GET: Address
        public ActionResult Index()
        {
            return View();
        }

        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }

        public ActionResult MyAddresses()
        {
            //Current user
            var user = UserManager.FindByName(User.Identity.Name);
            var currentUser = UserInfoProvider.GetUserInfo(user.UserName).UserID;
            var customerID = CustomerInfoProvider.GetCustomerInfoByUserID(user.Id).CustomerID;

            List<BillingAddressViewModel> AddressObj = new List<BillingAddressViewModel>();

            SelectList countries = new SelectList(CountryInfoProvider.GetCountries(), "CountryID", "CountryDisplayName", 457);
            var addresses = AddressInfoProvider.GetAddresses()
                .WhereEquals("AddressCustomerID", customerID);

            //addresses.Select(a => new BillingAddressViewModel() { });

            AddressObj = addresses.Select(address => new BillingAddressViewModel()
            {
                Line1 = address.AddressLine1,
                Line2 = address.AddressLine2,
                City = address.AddressCity,
                PostalCode = address.AddressZip,
                CountryID = address.AddressCountryID,
                StateID = address.AddressStateID,
                AddressID = address.AddressID,
                Phone = address.AddressPhone,
                Countries = countries,
                PersonalName = address.AddressPersonalName,
                AddressName = address.AddressName,
                UserID = address.GetValue("AddressUserID", 0),
                MainAddress = address.GetValue("MainAddress", false)
            }).ToList();
            return View(AddressObj);
        }

        public ActionResult UpdateMyAddress(int AddressID)
        {
            var EditAddressObj = AddressInfoProvider.GetAddressInfo(AddressID);
            SelectList countries = new SelectList(CountryInfoProvider.GetCountries(), "CountryID", "CountryDisplayName", 457);

            var model = new BillingAddressViewModel()
            {
                Line1 = EditAddressObj.AddressLine1,
                Line2 = EditAddressObj.AddressLine2,
                City = EditAddressObj.AddressCity,
                PostalCode = EditAddressObj.AddressZip,
                CountryID = EditAddressObj.AddressCountryID,
                StateID = EditAddressObj.AddressStateID,
                AddressID = EditAddressObj.AddressID,
                Phone = EditAddressObj.AddressPhone,
                Countries = countries,
                PersonalName = EditAddressObj.AddressPersonalName,
                AddressName = EditAddressObj.AddressName,
                UserID = EditAddressObj.GetValue("AddressUserID", 0),
                MainAddress = EditAddressObj.GetValue("MainAddress", false)
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateMyAddress(BillingAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Fill the missing info");
                return View(model);
            }

            try
            {
                var user = UserManager.FindByName(User.Identity.Name);
                var currentUser = UserInfoProvider.GetUserInfo(user.UserName).UserID;
                var customer = CustomerInfoProvider.GetCustomers().WhereEquals("CustomerUserID", currentUser).FirstOrDefault();
                if (customer != null)
                {
                    var EditAddress = AddressInfoProvider.GetAddressInfo(model.AddressID);
                    EditAddress.AddressName = model.Line1 + " " + model.Line2;
                    EditAddress.AddressLine1 = model.Line1;
                    EditAddress.AddressLine2 = model.Line2;
                    EditAddress.AddressCity = model.City;
                    EditAddress.AddressZip = model.PostalCode;
                    EditAddress.AddressCountryID = model.CountryID;
                    EditAddress.AddressStateID = model.StateID;
                    EditAddress.AddressPhone = model.Phone;
                    EditAddress.AddressPersonalName = model.PersonalName;
                    EditAddress.AddressName = model.AddressName;
                    EditAddress.AddressCustomerID = customer.CustomerID;
                    EditAddress.SetValue("MainAddress", model.MainAddress);
                    EditAddress.SetValue("AddressUserID", currentUser);

                    // Only one main address
                    if (model.MainAddress)
                    {
                        var clearOldMainAddress = AddressInfoProvider.GetAddresses();
                        if (clearOldMainAddress != null && clearOldMainAddress.Count > 0)
                        {
                            foreach (var address in clearOldMainAddress)
                            {
                                if (address.AddressCustomerID == customer.CustomerID
                                    && address.AddressID != model.AddressID)
                                {
                                    address.SetValue("MainAddress", false);
                                    AddressInfoProvider.SetAddressInfo(address);
                                }
                            }
                        }
                    }
                    AddressInfoProvider.SetAddressInfo(EditAddress);
                }

                return RedirectToAction("MyAddresses", "Address");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return View();
            }
        }

        public ActionResult RemoveMyAddress(int AddressID)
        {
            var RemoveAddress = AddressInfoProvider.GetAddressInfo(AddressID);
            AddressInfoProvider.DeleteAddressInfo(RemoveAddress);
            return RedirectToAction("MyAddresses", "Address");
        }

        public ActionResult AddNewAddress()
        {
            //Current user
            var user = UserManager.FindByName(User.Identity.Name);
            var currentUser = UserInfoProvider.GetUserInfo(user.UserName).UserID;
            SelectList countries = new SelectList(CountryInfoProvider.GetCountries(), "CountryID", "CountryDisplayName", 457);
            var model = new BillingAddressViewModel();
            model.UserID = currentUser;
            model.Countries = countries;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewAddress(BillingAddressViewModel model)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            var currentUser = UserInfoProvider.GetUserInfo(user.UserName).UserID;
            var customerID = CustomerInfoProvider.GetCustomerInfoByUserID(user.Id).CustomerID;

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Fill the missing info");
                //return RedirectToAction("MyAddresses", "Address");
                return View(model);
            }
            try
            {
                // TODO: Add insert logic here
                AddressInfo newAddress = new AddressInfo();

                //create an address 
                newAddress.AddressLine1 = model.Line1;
                newAddress.AddressLine2 = model.Line2;
                newAddress.AddressCity = model.City;
                newAddress.AddressZip = model.PostalCode;
                newAddress.AddressCountryID = model.CountryID;
                newAddress.AddressStateID = model.StateID;
                newAddress.AddressPhone = model.Phone;
                newAddress.AddressPersonalName = model.PersonalName;
                newAddress.AddressCustomerID = customerID;
                newAddress.AddressName = $"{model.PersonalName},{model.Line1},{model.City}";
                newAddress.SetValue("AddressUserID", model.UserID);
                newAddress.SetValue("MainAddress", model.MainAddress);

                // Only one main address
                if (model.MainAddress)
                {
                    var clearOldMainAddress = AddressInfoProvider.GetAddresses();
                    if (clearOldMainAddress != null && clearOldMainAddress.Count > 0)
                    {
                        foreach (var address in clearOldMainAddress)
                        {
                            if (address.AddressCustomerID == customerID)
                            {
                                address.SetValue("MainAddress", false);
                                AddressInfoProvider.SetAddressInfo(address);
                            }
                        }
                    }
                }

                AddressInfoProvider.SetAddressInfo(newAddress);
                return RedirectToAction("MyAddresses", "Address");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return RedirectToAction("MyAddresses", "Address");
            }
        }
    }
}