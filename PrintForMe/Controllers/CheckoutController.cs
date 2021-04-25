using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PrintForMe.Models.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PrintForMe.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly string siteName = SiteContext.CurrentSiteName;
        private readonly IShoppingService shoppingService;

        //DocSection:Constructor
        /// <summary>
        /// Initializes an instance of the IShoppingService used to facilitate shopping cart interactions.
        /// </summary>
        public CheckoutController()
        {
            // Initializes an instance of a service required to manage the shopping cart
            // For real-world projects, we recommend using a dependency injection
            // container to initialize service instances
            shoppingService = Service.Resolve<IShoppingService>();
        }
        //EndDocSection:Constructor


        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }

        //DocSection:DisplayCart
        /// <summary>
        /// Displays the customer's current shopping cart.
        /// </summary>
        public ActionResult ShoppingCart()
        {
            // Gets the current user's shopping cart
            ShoppingCartInfo currentCart = shoppingService.GetCurrentShoppingCart();

            // Initializes the shopping cart model
            ShoppingCartViewModel model = new ShoppingCartViewModel(currentCart, Server.MapPath("~/PrintForMe"));

            // Displays the shopping cart
            return View(model);
        }

        //DocSection:AddItem
        /// <summary>
        /// Adds products to the customer's current shopping cart.
        /// </summary>
        /// <param name="itemSkuId">ID of the added item (its SKU object).</param>
        /// <param name="itemUnits">Quantity of the item to be added.</param>        
        public ActionResult AddItem(int itemSkuId)
        {
            // Gets the first product from the current shopping cart
            ShoppingCartItemInfo cartInfo = shoppingService
                                                .GetCurrentShoppingCart()
                                                .CartProducts
                                                .Where(x => x.SKUID == itemSkuId).FirstOrDefault();

            if (cartInfo != null)
            {
                // Removes a product from the current shopping cart
                shoppingService.RemoveItemFromCart(cartInfo.CartItemID);
            }

            // Adds the specified number of units of a given product to the current shopping cart
            shoppingService.AddItemToCart(itemSkuId, 1);

            // Displays the shopping cart
            return RedirectToAction("ShoppingCart");
        }
        //EndDocSection:AddItem

        /// <summary>
        /// Removes a shopping cart item from the customer's current shopping cart.
        /// </summary>
        /// <param name="itemID">ID of the item to be removed.</param>
        [HttpPost]
        public ActionResult RemoveItem(int itemID, int SKUID)
        {
            // Removes a specified product from the shopping cart
            shoppingService.RemoveItemFromCart(itemID);
            //Delete all project details
            DeleteProjectDetail(SKUID);

            // Displays the shopping cart
            return RedirectToAction("ShoppingCart");
        }

        //DocSection:CouponCodeAdd
        /// <summary>
        /// Adds the specified coupon code to the shopping cart.
        /// </summary>
        [HttpPost]
        public ActionResult AddCouponCode(string couponCode)
        {
            var coupons = shoppingService.GetCurrentShoppingCart().CouponCodes;

            int couponsCodes = coupons.AllAppliedCodes.Count();

            if (couponsCodes >= 1)
            {
                ModelState.AddModelError("OnlyOneCouponCode", "You can enter 1 code at a time");
            }

            else
            {
                // Adds the coupon code to the shopping cart
                if ((couponCode == "") || !shoppingService.AddCouponCode(couponCode))
                {
                    // Adds an error message to the model state if the entered coupon code is not valid
                    ModelState.AddModelError("CouponCodeError", "The entered coupon code is not valid.");
                }
            }

            // Initializes the shopping cart model
            ShoppingCartViewModel model = new ShoppingCartViewModel(shoppingService.GetCurrentShoppingCart(),"");

            // Displays the shopping cart
            return View("ShoppingCart", model);
        }
        //EndDocSection:CouponCodeAdd


        //DocSection:CouponCodeRemove
        /// <summary>
        /// Removes the specified coupon code from the shopping cart.
        /// </summary>
        [HttpPost]
        public ActionResult RemoveCouponCode(string couponCode)
        {
            // Removes the specified coupon code
            shoppingService.RemoveCouponCode(couponCode);

            // Displays the shopping cart
            return RedirectToAction("ShoppingCart");
        }
        //EndDocSection:CouponCodeRemove

        //DocSection:DisplayDeliveryAddressSelector
        /// <summary>
        /// Displays the customer details checkout process step with an address selector for known customers.
        /// </summary>
        public ActionResult DeliveryDetailsAddressSelector()
        {
            // Gets the current user's shopping cart
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            // If the shopping cart is empty, redirects to the shopping cart view
            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }

            // Gets all countries for the country selector
            SelectList countries = new SelectList(CountryInfoProvider.GetCountries(), "CountryID", "CountryDisplayName");

            // Gets the current customer
            CustomerInfo customer = shoppingService.GetCurrentCustomer();

            // Get all user billing addresses for the address selector
            var user = UserManager.FindByName(User.Identity.Name);
            var currentUser = UserInfoProvider.GetUserInfo(user.UserName);

            // Gets all customer billing addresses for the address selector
            IEnumerable<AddressInfo> customerAddresses = Enumerable.Empty<AddressInfo>();
            //if(user != null)
            //{
            //    customerAddresses = AddressInfoProvider.GetAddresses().WhereEquals("AddressUserID", currentUser.UserID).ToList();
            //}
            //else if (customer != null)
            if (customer != null)
            {
                customerAddresses = AddressInfoProvider.GetAddresses(customer.CustomerID).ToList();
            }

            // Prepares address selector options
            // SelectList addresses = new SelectList(customerAddresses, "AddressID", "AddressName");

            // Gets all enabled shipping options for the shipping option selector

            SelectList shippingOptions = new SelectList(ShippingOptionInfoProvider.GetShippingOptions(SiteContext.CurrentSiteID, true).ToList(), "ShippingOptionID", "ShippingOptionDisplayName");

            // Loads the customer details
            DeliveryDetailsViewModel model = new DeliveryDetailsViewModel
            {
                Customer = new CustomerViewModel(shoppingService.GetCurrentCustomer()),
                BillingAddress = new BillingAddressViewModel(shoppingService.GetBillingAddress(), countries, customerAddresses.ToList()),
                ShippingOption = new ShippingOptionViewModel(ShippingOptionInfoProvider.GetShippingOptionInfo(shoppingService.GetShippingOption()), shippingOptions)
            };


            // Displays the customer details step
            return View(model);
        }
        //EndDocSection:DisplayDeliveryAddressSelector


        //DocSection:PostDelivery
        /// <summary>
        /// Validates the entered customer details and proceeds to the order review step.
        /// </summary>
        /// <param name="model">View model with the customer details.</param>
        [HttpPost]
        public ActionResult DeliveryDetails(DeliveryDetailsViewModel model)
        {
            // Gets the user's current shopping cart
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            // Gets all enabled shipping options for the shipping option selector
            SelectList shippingOptions = new SelectList(ShippingOptionInfoProvider.GetShippingOptions(SiteContext.CurrentSiteID, true).ToList(),
                                                                                                                              "ShippingOptionID",
                                                                                                                              "ShippingOptionDisplayName");
            // Gets the shopping cart's billing address and applies the billing address from the checkout process step
            var address = AddressInfoProvider.GetAddressInfo(model.BillingAddress.AddressID) ?? new AddressInfo();

            if (model.BillingAddress.AddressID == 0)
            {
                model.BillingAddress.ApplyTo(address);
            }
            else
            {
                model.BillingAddress = model.BillingAddress.FillAddress(address);
            }

            CMS.Globalization.CountryInfo ci = CMS.Globalization.CountryInfoProvider.GetCountryInfo("SaudiArabia");
            //457 is the countryId for saudi arabia
            if (address.AddressCountryID == ci.CountryID)
            {
                var shippingData = ShippingOptionInfoProvider.GetShippingOptionInfo("Shipping-SA", SiteContext.CurrentSite.SiteName);
                if (shippingData != null)
                {
                    model.ShippingOption.ShippingOptionID = shippingData.ShippingOptionID;
                    shoppingService.SetShippingOption(shippingData.ShippingOptionID);
                }
            }
            else
            {
                var shippingData = ShippingOptionInfoProvider.GetShippingOptionInfo("Shipping-None-SA", SiteContext.CurrentSite.SiteName);
                if (shippingData != null)
                {
                    model.ShippingOption.ShippingOptionID = shippingData.ShippingOptionID;
                    shoppingService.SetShippingOption(shippingData.ShippingOptionID);
                }
            }

            // If the ModelState is not valid, assembles the country list and the shipping option list and displays the step again
            //if (!ModelState.IsValid)
            //{
            //    model.BillingAddress.Countries = new SelectList(CountryInfoProvider.GetCountries(), "CountryID", "CountryDisplayName");
            //    model.ShippingOption.ShippingOptions = new ShippingOptionViewModel(ShippingOptionInfoProvider.GetShippingOptionInfo(shoppingService.GetShippingOption()), shippingOptions).ShippingOptions;
            //    return View(model);
            //}

            // Gets the shopping cart's customer and applies the customer details from the checkout process step
            var customer = shoppingService.GetCurrentCustomer();

            if (customer == null)
            {
                UserInfo userInfo = cart.User;
                if (userInfo != null)
                {
                    customer = CustomerHelper.MapToCustomer(cart.User);
                }
                else
                {
                    customer = new CustomerInfo();
                }
            }
            model.Customer.ApplyToCustomer(customer);

            // Sets the updated customer object to the current shopping cart
            shoppingService.SetCustomer(customer);



            // Sets the address personal name
            address.AddressPersonalName = $"{customer.CustomerFirstName} {customer.CustomerLastName}";

            // Saves the billing address
            shoppingService.SetBillingAddress(address);
            // Sets the selected shipping option and evaluates the cart
            shoppingService.SetShippingOption(model.ShippingOption.ShippingOptionID);

            // Redirects to the next step of the checkout process
            return RedirectToAction("PreviewAndPay");
        }
        //EndDocSection:PostDelivery

        /// <summary>
        /// Display the preview checkout process step.
        /// </summary>
        public ActionResult PreviewAndPay()
        {
            // If the current shopping cart is empty, returns to the shopping cart action
            if (shoppingService.GetCurrentShoppingCart().IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }
            // Prepares a model from the preview step;
            PreviewAndPayViewModel model = PreparePreviewViewModel();


            // Displays the preview step
            return View(model);
        }



        /// <summary>
        /// Decides whether the specified payment method is valid on the current site.
        /// </summary>
        /// <param name="paymentMethodID">ID of the applied payment method.</param>
        /// <returns>True if the payment method is valid.</returns>
        private bool IsPaymentMethodValid(int paymentMethodID)
        {
            // Gets the current shopping cart
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            // Gets a list of all applicable payment methods to the current user's shopping cart
            List<PaymentOptionInfo> paymentMethods = GetApplicablePaymentMethods(cart).ToList();

            // Returns whether an applicable payment method exists with the entered payment method's ID
            return paymentMethods.Exists(p => p.PaymentOptionID == paymentMethodID);
        }
        //EndDocSection:PreparePreview


        //DocSection:PostPreview
        /// <summary>
        /// Validates all collected information, creates an order,
        /// and redirects the customer to payment.
        /// </summary>
        /// <param name="model">View model with information about the future order.</param>
        [HttpPost]
        public ActionResult PreviewAndPay(PreviewAndPayViewModel model)
        {
            // Gets the current shopping cart
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            // Validates the shopping cart
            var validationErrors = ShoppingCartInfoProvider.ValidateShoppingCart(cart);

            // Gets the selected payment method, assigns it to the shopping cart, and evaluates the cart
            shoppingService.SetPaymentOption(model.PaymentMethod.PaymentMethodID);

            // If the validation was not successful, displays the preview step again
            if (validationErrors.Any() || !IsPaymentMethodValid(model.PaymentMethod.PaymentMethodID))
            {
                // Prepares a view model from the order review step
                PreviewAndPayViewModel viewModel = PreparePreviewViewModel();

                // Displays the order review step again
                return View("PreviewAndPay", viewModel);
            }

            // Creates an order from the current shopping cart
            // If the order was created successfully, empties the cart
            OrderInfo order = shoppingService.CreateOrder();

            if (order.OrderID > 0 && cart.GetValue("ShoppingCartGiftWrapping") != null)
            {
                order.SetValue("OrderGiftWrapping", cart.GetValue("ShoppingCartGiftWrapping"));
                order.Update();
            }

            // Redirects to the payment gateway
            return RedirectToAction("Index", "Payment", new { orderID = order.OrderID });
        }

        //DocSection:ThankYou
        /// <summary>
        /// Displays a thank-you page where user is redirected after creating an order.
        /// </summary>
        /// <param name="orderID">ID of the created order.</param>
        public ActionResult ThankYou(int orderID = 0)
        {
            ViewBag.OrderID = orderID;

            return View();
        }
        //EndDocSection:ThankYou



        /// <summary>
        /// Prepares a view model of the preview checkout process step including the shopping cart,
        /// the customer details, and the payment method.
        /// </summary>
        /// <returns>View model with information about the future order.</returns>
        private PreviewAndPayViewModel PreparePreviewViewModel()
        {
            // Gets the current user's shopping cart
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();


            // Prepares the customer details
            DeliveryDetailsViewModel deliveryDetailsModel = new DeliveryDetailsViewModel
            {
                Customer = new CustomerViewModel(shoppingService.GetCurrentCustomer()),
                BillingAddress = new BillingAddressViewModel(shoppingService.GetBillingAddress(), null, null),
                ShippingOption = new ShippingOptionViewModel()
                {
                    ShippingOptionID = cart.ShippingOption.ShippingOptionID,
                    ShippingOptionDisplayName = ShippingOptionInfoProvider.GetShippingOptionInfo(cart.ShippingOption.ShippingOptionID).ShippingOptionDisplayName
                }
            };

            // Prepares the payment method
            PaymentMethodViewModel paymentViewModel = new PaymentMethodViewModel
            {
                // PaymentMethods = new SelectList(GetApplicablePaymentMethods(cart), "PaymentOptionID", "PaymentOptionDisplayName")
                PaymentMethods = new List<PaymentOptionInfo>(GetApplicablePaymentMethods(cart))
            };

            // Gets the selected payment method
            PaymentOptionInfo paymentMethod = cart.PaymentOption;
            if (paymentMethod != null)
            {
                paymentViewModel.PaymentMethodID = paymentMethod.PaymentOptionID;
            }

            // Prepares a model from the preview step
            PreviewAndPayViewModel model = new PreviewAndPayViewModel
            {
                DeliveryDetails = deliveryDetailsModel,
                Cart = new ShoppingCartViewModel(cart,""),
                PaymentMethod = paymentViewModel
            };

            return model;
        }

        /// <summary>
        /// Gets all applicable payment methods assigned to the current site.
        /// </summary>
        /// <param name="cart">Shopping cart of the site</param>
        /// <returns>Collection of applicable payment methods</returns>
        private IEnumerable<PaymentOptionInfo> GetApplicablePaymentMethods(ShoppingCartInfo cart)
        {
            // Gets all enabled payment methods from Kentico
            IEnumerable<PaymentOptionInfo> enabledPaymentMethods = PaymentOptionInfoProvider.GetPaymentOptions(SiteContext.CurrentSiteID, true).ToList();

            // Returns all applicable payment methods
            return enabledPaymentMethods.Where(paymentMethod => PaymentOptionInfoProvider.IsPaymentOptionApplicable(cart, paymentMethod));
        }
        //EndDocSection:PreparePayment

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }


        //DocSection:LoadingStates
        /// <summary>
        /// Loads states of the specified country.
        /// </summary>
        /// <param name="countryId">ID of the selected country.</param>
        /// <returns>Serialized display names of the loaded states.</returns>
        [HttpPost]
        public JsonResult CountryStates(int countryId)
        {
            // Gets the display names of the country's states
            var responseModel = StateInfoProvider.GetStates().WhereEquals("CountryID", countryId)
                .Select(s => new
                {
                    id = s.StateID,
                    name = HTMLHelper.HTMLEncode(s.StateDisplayName)
                });

            // Returns serialized display names of the states
            return Json(responseModel);
        }
        //EndDocSection:LoadingStates

        //DocSection:LoadingAddress
        /// <summary>
        /// Loads information of an address specified by its ID.
        /// </summary>
        /// <param name="addressID">ID of the address.</param>
        /// <returns>Serialized information of the loaded address.</returns>
        [HttpPost]
        public JsonResult CustomerAddress(int addressID)
        {
            // Gets the address with its ID
            AddressInfo address = AddressInfoProvider.GetAddressInfo(addressID);

            // Checks whether the address was retrieved
            if (address == null)
            {
                return null;
            }

            // Creates a response with all address information
            var responseModel = new
            {
                Line1 = address.AddressLine1,
                Line2 = address.AddressLine2,
                City = address.AddressCity,
                PostalCode = address.AddressZip,
                CountryID = address.AddressCountryID,
                StateID = address.AddressStateID,
                PersonalName = address.AddressPersonalName
            };

            // Returns serialized information of the address
            return Json(responseModel);
        }
        //EndDocSection:LoadingAddress

        public ActionResult AddNewBillingAddress()
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

        /// <summary>
        /// Add New Billing Address
        /// and Billing Address Listing.
        /// </summary>
        /// <param name="model">View model with information about the future order.</param>
        [HttpPost]
        public ActionResult AddNewBillingAddress(BillingAddressViewModel model)
        {
            // Gets the user's current shopping cart
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            var newaddress = new AddressInfo();
            model.ApplyTo(newaddress);

            // Gets the shopping cart's customer and applies the customer details from the checkout process step
            var customer = shoppingService.GetCurrentCustomer();

            if (customer == null)
            {
                UserInfo userInfo = cart.User;
                if (userInfo != null)
                {
                    customer = CustomerHelper.MapToCustomer(cart.User);
                }
                else
                {
                    customer = new CustomerInfo();
                }
            }
            //model.Customer.ApplyToCustomer(customer);

            // Sets the updated customer object to the current shopping cart
            shoppingService.SetCustomer(customer);

            var user = UserManager.FindByName(User.Identity.Name);
            //var currentUser = UserInfoProvider.GetUserInfo(user.UserName).UserID;
            var customerID = CustomerInfoProvider.GetCustomerInfoByUserID(user.Id).CustomerID;

            if (user != null)
            {
                newaddress.SetValue("AddressCustomerID", customerID);
            }

            // Sets the address personal name
            if (string.IsNullOrEmpty(newaddress.AddressPersonalName))
                newaddress.AddressPersonalName = $"{customer.CustomerFirstName} {customer.CustomerLastName}";

            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }

            // Only one main address
            if (model.MainAddress)
            {
                var clearOldMainAddress = AddressInfoProvider.GetAddresses();
                if (clearOldMainAddress != null && clearOldMainAddress.Count > 0)
                {
                    foreach (var address in clearOldMainAddress)
                    {
                        if (address.AddressCustomerID == customer.CustomerID)
                        {
                            address.SetValue("MainAddress", false);
                            AddressInfoProvider.SetAddressInfo(address);
                        }
                    }
                }
            }

            // Saves the billing address
            shoppingService.SetBillingAddress(newaddress);

            // Redirects to the DeliveryDetails Address Selector
            //return RedirectToAction("DeliveryDetailsAddressSelector");
            return RedirectToAction("DeliveryDetailsAddressSelector", "Checkout");
        }

        public ActionResult UpdateOrderGiftWrapping(bool GiftWrapping)
        {
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();
            cart.SetValue("ShoppingCartGiftWrapping", GiftWrapping ? 1 : 0);
            cart.Update();
            return Json("Success");
        }

        [HttpPost]
        public ActionResult UpdateShoppingCartNote(string Note)
        {
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();
            cart.ShoppingCartNote = Note != null ? Note : "";
            cart.Update();
            return Json("Success");
        }

        public ActionResult UpdateAddress(int AddressID)
        {
            var EditAddressObj = AddressInfoProvider.GetAddressInfo(AddressID);
            SelectList countries = new SelectList(CountryInfoProvider.GetCountries(), "CountryID", "CountryDisplayName");

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
        public ActionResult UpdateAddress(BillingAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Fill the missing info");
                return View("MyAddresses", "Address");
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
                    //return RedirectToAction("MyAddresses", "Address");
                }

                // Redirects to the DeliveryDetails Address Selector
                return RedirectToAction("DeliveryDetailsAddressSelector");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return View();
            }
        }

        public ActionResult RemoveAddress(int AddressID)
        {
            var RemoveAddress = AddressInfoProvider.GetAddressInfo(AddressID);
            AddressInfoProvider.DeleteAddressInfo(RemoveAddress);
            return RedirectToAction("DeliveryDetailsAddressSelector");
        }

        private void DeleteProjectDetail(int SKUID)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
            string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";

            string photoProjectDetail = "PrintForme.PhotoProjectDetail";
            string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";
            string wallPrintingProjectDetail = "PrintForme.WallPrintingProjectDetail";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            DataClassInfo wallPrintingProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);

            DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
            DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
            DataClassInfo wallPrintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectDetail);

            if (SKUID > 0)
            {
                if (photoProjectMasterInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem item = CustomTableItemProvider.GetItems(photoProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (item != null)
                    {
                        if (ValidationHelper.GetInteger(item.GetValue("ItemID"), 0) > 0)
                        {
                            int projectID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0);

                            if (photoProjectDetailInfo != null)
                            {
                                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                                List<CustomTableItem> items = CustomTableItemProvider.GetItems(photoProjectDetail)
                                .WhereEquals("ProjectID", projectID)
                                .Columns("ProjectID", "ImageUrl", "ItemID").ToList();

                                foreach (var itemDetail in items)
                                {
                                    string imageUrl = ValidationHelper.GetString(itemDetail.GetValue("ImageUrl"), "");

                                    if (!string.IsNullOrEmpty(imageUrl))
                                    {
                                        string path = Server.MapPath("~/PrintForMe/PhotoProject") + imageUrl;
                                        if (System.IO.File.Exists(path))
                                        {
                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();
                                            System.IO.File.Delete(path);
                                        }
                                    }

                                    // Delete detail data
                                    itemDetail.Delete();
                                }
                            }

                            // Delete master data
                            item.Delete();
                        }
                    }
                }

                if (woodenProjectMasterInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem item = CustomTableItemProvider.GetItems(woodenProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (item != null)
                    {
                        if (ValidationHelper.GetInteger(item.GetValue("ItemID"), 0) > 0)
                        {
                            int projectID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0);

                            if (woodenProjectDetailInfo != null)
                            {
                                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                                List<CustomTableItem> items = CustomTableItemProvider.GetItems(woodenProjectDetail)
                                .WhereEquals("ProjectID", projectID)
                                .Columns("ProjectID", "ImageUrl", "ItemID").ToList();

                                foreach (var itemDetail in items)
                                {
                                    string imageUrl = ValidationHelper.GetString(itemDetail.GetValue("ImageUrl"), "");

                                    if (!string.IsNullOrEmpty(imageUrl))
                                    {
                                        string path = Server.MapPath("~/PrintForMe/WoodenProject") + imageUrl;
                                        if (System.IO.File.Exists(path))
                                        {
                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();
                                            System.IO.File.Delete(path);
                                        }
                                    }

                                    // Delete detail data
                                    itemDetail.Delete();
                                }
                            }

                            // Delete master data
                            item.Delete();
                        }
                    }
                }

                if (wallPrintingProjectMasterInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem item = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (item != null)
                    {
                        if (ValidationHelper.GetInteger(item.GetValue("ItemID"), 0) > 0)
                        {
                            int projectID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0);

                            if (wallPrintingProjectDetailInfo != null)
                            {
                                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                                List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallPrintingProjectDetail)
                                .WhereEquals("ProjectID", projectID)
                                .Columns("ProjectID", "ImageUrl", "ItemID").ToList();

                                foreach (var itemDetail in items)
                                {
                                    string imageUrl = ValidationHelper.GetString(itemDetail.GetValue("ImageUrl"), "");

                                    if (!string.IsNullOrEmpty(imageUrl))
                                    {
                                        string path = Server.MapPath("~/PrintForMe/WallPaintingProject") + imageUrl;
                                        if (System.IO.File.Exists(path))
                                        {
                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();
                                            System.IO.File.Delete(path);
                                        }
                                    }

                                    // Delete detail data
                                    itemDetail.Delete();
                                }
                            }

                            // Delete master data
                            item.Delete();
                        }
                    }
                }
            }
        }

        public ActionResult EditProjectBySkuId(int SKUID)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
            string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            DataClassInfo wallPrintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);

            if (SKUID > 0)
            {
                if (photoProjectMasterInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(photoProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (userProjectDetail != null)
                    {
                        return RedirectToAction("EditPhotoProject",
                               new RouteValueDictionary(new
                               {
                                   controller = "Services",
                                   action = "EditPhotoProject",
                                   SKUId = SKUID
                               }));
                    }
                }

                if (woodenProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(woodenProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (userProjectDetail != null)
                    {
                        return RedirectToAction("EditWoodenProject",
                                new RouteValueDictionary(new
                                {
                                    controller = "WoodenPallets",
                                    action = "EditWoodenProject",
                                    SKUId = SKUID
                                }));
                    }
                }

                if (wallPrintingProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (userProjectDetail != null)
                    {
                        return RedirectToAction("EditWallPaintingProject",
                              new RouteValueDictionary(new
                              {
                                  controller = "WallPainting",
                                  action = "EditWallPaintingProject",
                                  SKUId = SKUID
                              }));
                    }
                }
            }

            // Displays the shopping cart
            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public ActionResult GetUserCartAvailability()
        {
            // Gets the current user's shopping cart
            ShoppingCartInfo currentCart = shoppingService.GetCurrentShoppingCart();
            if (currentCart.IsEmpty)
            {
                return Json(false);
            }
            return Json(true);
        }
    }
}