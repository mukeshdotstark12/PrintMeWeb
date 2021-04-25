using CMS.Core;
using CMS.Ecommerce;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CMS.Base;
using System.Text;
using CMS.Helpers;
using CustomWebApi.Models.ShoppingCart;
using CMS.Membership;
using CustomWebApi.Models.User;
using CustomWebApi.Models.PayTabs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CustomWebApi.Helpers;
using CustomWebApi.Models.Order;
using CustomWebApi.Model.Shared;
using CMS.DataEngine;
using CMS.CustomTables;

namespace CustomWebApi.Controllers
{
    public class CheckoutController : ApiController
    {
        private readonly IShoppingService shoppingService;

        public CheckoutController()
        {
            shoppingService = Service.Resolve<IShoppingService>();
        }

        [HttpGet]
        public IHttpActionResult GetShoppingCart(int userID)
        {
            int uID = ValidationHelper.GetInteger(userID, 0);

            //get the user info
            var user = UserInfoProvider.GetUserInfo(userID);
            if (uID != 0 && user != null)
            {

                // Gets the shopping cart info based on the entered user ID
                var mCurrentShoppingCart = Service.Resolve<ICurrentShoppingCartService>().GetCurrentShoppingCart(user, 1);

                // Creates the shopping cart in the database if it does not exist yet
                if (mCurrentShoppingCart.ShoppingCartID == 0)
                {
                    ShoppingCartInfoProvider.SetShoppingCartInfo(mCurrentShoppingCart);
                }

                // Creates the model representing the collection of a customer's shopping cart items + info
                ShoppingCartModel model = new ShoppingCartModel(mCurrentShoppingCart);
                return Json(model);
            }
            return Content(HttpStatusCode.BadRequest, "User not found");
        }

        [HttpPost]
        public HttpResponseMessage CreateOrder(UserDataModel userDataModel)
        {
            //get the user info
            UserInfo user = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(userDataModel.UserID, 0));

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "User Not Found",
                });
            }

            var payment = PaymentOptionInfoProvider.GetPaymentOptionInfo(ValidationHelper.GetInteger(userDataModel.PaymentType, 0));
            if (payment == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Payment type not valid",
                });
            }


            CustomerInfo customer = CustomerInfoProvider.GetCustomerInfoByUserID(user.UserID);

            ShoppingCartInfo mCurrentShoppingCart = Service.Resolve<ICurrentShoppingCartService>().GetCurrentShoppingCart(user, 1);
            // Gets the shopping cart info based on the entered user ID
            //ShoppingCartInfo mCurrentShoppingCart = ShoppingCartInfoProvider.GetShoppingCartInfo(userDataModel.ShoppingCartID);

            mCurrentShoppingCart.SetShoppingCartUser(customer.CustomerID);


            mCurrentShoppingCart.Evaluate();

            bool isShopppingCartComplete = mCurrentShoppingCart.IsComplete;

            ShoppingCartInfoProvider.SetShoppingCartInfo(mCurrentShoppingCart);

            // Creates the model representing the collection of a customer's shopping cart items + info
            ShoppingCartModel shoppingCartModel = new ShoppingCartModel(mCurrentShoppingCart);

            if (shoppingCartModel == null || shoppingCartModel.ShoppingCartID == 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shopping cart not available");
            }

            var shoppingcartOrder = OrderInfoProvider.GetOrders().
                                    WhereEquals("ShoppingCartID", shoppingCartModel.ShoppingCartID)
                                    .FirstOrDefault();
            if (shoppingcartOrder != null)
            {
                if (ValidationHelper.GetInteger(shoppingcartOrder.GetValue("OrderID"), 0) > 0)
                {
                    ShoppingCartInfoProvider.DeleteShoppingCartInfo(mCurrentShoppingCart);

                    return Request.CreateResponse(HttpStatusCode.OK, new customResponse
                    {
                        success = true,
                        result = "Order already created for this ShoppingCartID",
                        orderid = ValidationHelper.GetInteger(shoppingcartOrder.GetValue("OrderID"), 0),
                        Status = "Success"
                    });
                }
            }

            //CustomerInfo customer = CustomerInfoProvider.GetCustomers().WhereEquals("CustomerID", shoppingCartModel.ShoppingCartCustomerID).FirstOrDefault();

            // Gets a status for the order
            OrderStatusInfo orderStatus = OrderStatusInfoProvider.GetOrderStatusInfo("NewOrder", SiteContext.CurrentSiteName);

            // Gets a currency for the order
            CurrencyInfo currency = CurrencyInfoProvider.GetCurrencyInfo(shoppingCartModel.ShoppingCartCurrencyID);

            // Gets the customer's address
            //AddressInfo customerAddress = AddressInfoProvider.GetAddresses(shoppingCartModel.ShoppingCartCustomerID).FirstOrDefault();
            //select * from COM_Address where AddressID=23 
            AddressInfo customerAddress = AddressInfoProvider.GetAddressInfo(userDataModel.AddressID);

            var validationErrors = ShoppingCartInfoProvider.ValidateShoppingCart(mCurrentShoppingCart);

            var validator = new CreateOrderValidator(mCurrentShoppingCart);

            if (validationErrors.Any())
            {
                var errors = ProcessCheckResult(validator.Errors);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errors + " Cart is not valid");
            }

            if (customer == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer is null");

            if (orderStatus == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Order Status is null");

            if (currency == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Currency is null");

            if (customerAddress == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Address is null");

            //IShoppingService shoppingService = Service.Resolve<IShoppingService>();

            //// Creates a new order based on the contents of the current shopping cart
            //// Deletes the shopping cart after the order is created
            //shoppingService.CreateOrder();

            if ((customer != null) && (orderStatus != null) && (currency != null) && (customerAddress != null))
            {
                // Creates a new order object and sets its properties
                OrderInfo newOrder = new OrderInfo
                {
                    OrderInvoiceNumber = DateTime.Now.Ticks.ToString(),
                    OrderTotalPrice = shoppingCartModel.TotalPrice == 0 ? shoppingCartModel.GrandTotal : shoppingCartModel.TotalPrice,
                    OrderGrandTotal = shoppingCartModel.GrandTotal,
                    OrderTotalTax = shoppingCartModel.TotalTax,
                    OrderDate = DateTime.Now,
                    OrderStatusID = 2,//orderStatus.StatusID,
                    OrderCustomerID = customer.CustomerID,
                    OrderSiteID = SiteContext.CurrentSiteID,
                    OrderCurrencyID = currency.CurrencyID,
                    OrderCouponCodes = mCurrentShoppingCart.CouponCodes.Serialize(),
                    OrderShippingOptionID = shoppingCartModel.ShoppingCartShippingOptionID == 0 ?
                                            ValidationHelper.GetInteger(userDataModel.ShippingOptionID, 0) :
                                            shoppingCartModel.ShoppingCartShippingOptionID,
                    OrderTotalShipping = shoppingCartModel.TotalShipping,
                    OrderCompletedByUserID = mCurrentShoppingCart.ShoppingCartUserID,
                    OrderPaymentOptionID = userDataModel.PaymentType,
                    OrderGrandTotalInMainCurrency = shoppingCartModel.GrandTotal,
                    OrderTotalPriceInMainCurrency = shoppingCartModel.GrandTotal
                };

                newOrder.SetValue("ShoppingCartID", shoppingCartModel.ShoppingCartID);

                // Saves the order to the database
                OrderInfoProvider.SetOrderInfo(newOrder);

                var addressConverter = Service.Resolve<IAddressConverter>();

                // Prepares the order addresses from the customer's address
                OrderAddressInfo orderBillingAddress = addressConverter.Convert(customerAddress, newOrder.OrderID, AddressType.Billing);
                OrderAddressInfo orderShippingAddress = addressConverter.Convert(customerAddress, newOrder.OrderID, AddressType.Shipping);

                // Sets the order addresses
                OrderAddressInfoProvider.SetAddressInfo(orderBillingAddress);
                OrderAddressInfoProvider.SetAddressInfo(orderShippingAddress);


                // Order Items               
                var cartIems = shoppingCartModel.CartItems;

                if (customer != null)
                {
                    OrderInfo order = OrderInfoProvider.GetOrders().WhereEquals("OrderID", newOrder.OrderID).TopN(1).FirstOrDefault();

                    if ((order != null) && (cartIems != null))
                    {
                        foreach (var orderItem in cartIems)
                        {
                            // Creates a new order item object and sets its properties
                            OrderItemInfo newItem = new OrderItemInfo
                            {
                                OrderItemSKUName = orderItem.SKUName,
                                OrderItemOrderID = order.OrderID,
                                OrderItemSKUID = orderItem.SKUID,
                                OrderItemUnitPrice = orderItem.SKUPrice,
                                OrderItemUnitCount = orderItem.CartItemUnits,
                                OrderItemTotalPrice = orderItem.TotalPrice,
                                OrderItemTotalPriceInMainCurrency = orderItem.TotalPrice
                            };

                            // Saves the order item object to the database
                            OrderItemInfoProvider.SetOrderItemInfo(newItem);
                        }

                        //Complete all user project
                        CompleteCurrentProject(cartIems);
                    }
                }
                // ENDS
                //Remove shoppingcart
                ShoppingCartInfoProvider.DeleteShoppingCartInfo(mCurrentShoppingCart);                

                return Request.CreateResponse(HttpStatusCode.OK, new customResponse
                {
                    success = true,
                    result = "Order Created",
                    orderid = newOrder.OrderID,
                    Status = "Success"
                });
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Order is not Created");
        }

        [HttpPost]
        public HttpResponseMessage UpdateOrder(PayTabsResponseData payTabsResponseData)
        {
            try
            {
                // Gets the order based on the entered order ID
                OrderInfo order = OrderInfoProvider.GetOrderInfo(payTabsResponseData.order_id);

                if (order != null && payTabsResponseData.amount == order.OrderTotalPrice)
                {
                    PaymentResultInfo result = new PaymentResultInfo
                    {
                        PaymentDate = DateTime.Now,
                        PaymentDescription = payTabsResponseData.result,
                        PaymentIsCompleted = payTabsResponseData.response_code == "100" ? true : false,
                        PaymentTransactionID = payTabsResponseData.transaction_id,
                        PaymentStatusValue = payTabsResponseData.response_code,
                        PaymentMethodName = "PayTabs"
                    };

                    // Saves the payment result to the database
                    order.OrderStatusID = ValidationHelper.GetInteger(payTabsResponseData.orderStatusID, 0) != 0 ? payTabsResponseData.orderStatusID : order.OrderStatusID;
                    order.UpdateOrderStatus(result);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Updated");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error: " + ex.Message.ToString());
            }
        }

        [HttpPost]
        public HttpResponseMessage GetPaymentGatewayURL([FromBody] JObject dataObj)
        {
            var utility = new PaymentGatway();
            MakePaymentModel.VerifySecretKeyResponse secretKeyResponse = utility.ValidateKey();

            if (secretKeyResponse.result.ToLower() == "valid")
            {
                var data = JsonConvert.DeserializeObject<MakePaymentModel.PayPageRequest>(dataObj.ToString());

                var payPageResponse = utility.MakePayment(data);

                return Request.CreateResponse(HttpStatusCode.OK, payPageResponse);
                //return Ok(payPageResponse);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Secret Key is not valid");
        }

        [HttpPost]
        public HttpResponseMessage GetPaymentGatewayURLByOrderID(OrderData objOrderData)
        {
            int orderID = objOrderData.OrderID;

            OrderInfo orderInfo = OrderInfoProvider.GetOrderInfo(ValidationHelper.GetInteger(orderID, 0));

            if (orderInfo == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "OrderID is not valid");
            }

            OrderModel orderModel = new OrderModel(orderInfo);

            var orderInfoJSON = JsonConvert.SerializeObject(orderModel, Formatting.Indented);

            var utility = new PaymentGatway();

            if (orderInfoJSON != null)
            {
                var orderData = JsonConvert.DeserializeObject<MakePaymentModel.PayPageRequest>(orderInfoJSON.ToString());

                var payPageResponse = utility.MakePayment(orderData);

                return Request.CreateResponse(HttpStatusCode.OK, payPageResponse);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Secret Key is not valid");
        }

        [HttpPost]
        public HttpResponseMessage GetPaymentGatewayResponse(OrderData orderData)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                //var activeClient = (Models.Settings) Session["ActiveClient"];

                string response = client.UploadString("https://www.paytabs.com/apiv2/verify_payment",
                    "secret_key=" + Constants.PayTapsSecretKey
                    + "&merchant_email=" + Constants.PayTapsMerchantEmail
                    + "&merchant_password=" + "paytabltech"
                    + "&payment_reference=" + orderData.PaymentReferenceID);

                var PTResp = JsonConvert.DeserializeObject<MakePaymentModel.PayTabsVerifyPaymentResponse>(response);

                //Update order based on the result.
                //bool isUpdated = UpdateOrder(PTResp, orderData.OrderID);

                return Request.CreateResponse(HttpStatusCode.OK, PTResp);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error: " + ex.Message.ToString());
            }
        }

        #region Methods
        private decimal GetTotalPrice(IEnumerable<ShoppingCartItemModel> cartItems)
        {
            decimal totalPrice = 0;
            foreach (var item in cartItems)
            {
                totalPrice += item.TotalPrice;
            }
            return totalPrice;
        }
        private string ProcessCheckResult(IEnumerable<IValidationError> validationErrors)
        {
            StringBuilder errorLists = new StringBuilder();
            var itemErrors = validationErrors
                .OfType<ShoppingCartItemValidationError>()
                .GroupBy(g => g.SKUId);

            foreach (var errorGroup in itemErrors)
            {
                var errors = errorGroup
                    .Select(e => e.GetMessage())
                    .Join(" ");

                errorLists.Append(errors);
            }

            return errorLists.ToString();
        }

        private void CompleteCurrentProject(IEnumerable<ShoppingCartItemModel> shoppingCartItems)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
            string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            DataClassInfo wallPrintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);

            if (shoppingCartItems != null)
            {
                foreach (var item in shoppingCartItems)
                {
                    if (photoProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(photoProjectMaster)
                                 .WhereEquals("SKUID", item.SKUID)
                                 .WhereEquals("IsComplete", false).LastOrDefault();

                        if(userProjectDetail != null)
                        {
                            // Sets a new 'IsComplete' value based on the old one
                            userProjectDetail.SetValue("IsComplete", true);

                            // Saves the changes to the database
                            userProjectDetail.Update();                            
                        }
                    }

                    if (woodenProjectDetailInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(woodenProjectMaster)
                                 .WhereEquals("SKUID", item.SKUID)
                                 .WhereEquals("IsComplete", false).LastOrDefault();

                        if (userProjectDetail != null)
                        {
                            // Sets a new 'IsComplete' value based on the old one
                            userProjectDetail.SetValue("IsComplete", true);

                            // Saves the changes to the database
                            userProjectDetail.Update();                            
                        }
                    }

                    if (wallPrintingProjectDetailInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                                 .WhereEquals("SKUID", item.SKUID)
                                 .WhereEquals("IsComplete", false).LastOrDefault();

                        if (userProjectDetail != null)
                        {
                            // Sets a new 'IsComplete' value based on the old one
                            userProjectDetail.SetValue("IsComplete", true);

                            // Saves the changes to the database
                            userProjectDetail.Update();                           
                        }
                    }
                }
            }
        }
        #endregion
    }
}
