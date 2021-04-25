using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PrintForMe.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace PrintForMe.Controllers
{
    public class OrderController : Controller
    {
        private readonly IShoppingService shoppingService;

        public UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }

        //DocSection:Constructor
        /// <summary>
        /// Constructor for the OrderController class.
        /// </summary>
        public OrderController()
        {
            // Initializes an instance of IShoppingService used to facilitate shopping cart interactions
            // For real-world projects, we recommend using a dependency injection
            // container to initialize service instances
            shoppingService = Service.Resolve<IShoppingService>();
        }
        //EndDocSection:Constructor


        //DocSection:OrderDetails
        /// <summary>
        /// Displays a page where order details can be listed.
        /// </summary>
        public ActionResult OrderDetail()
        {
            OrderViewModel order = null;

            return View(order);
        }


        /// <summary>
        /// Displays details about an order specified with its ID.
        /// </summary>
        /// <param name="Id">Order ID as a string</param>

        public ActionResult GetOrderDetail(string Id)
        {
            // Gets the order based on the entered order ID
            OrderViewModel order = GetOrderViewModel(Id);

            return View("OrderDetail", order);
        }


        /// <summary>
        /// Returns the order view model wrapper based on the entered ID.
        /// </summary>
        /// <param name="textBoxValue">String containing the user-entered order ID</param>
        /// <returns>View model object of the order</returns>
        private OrderViewModel GetOrderViewModel(string textBoxValue)
        {
            OrderInfo info = GetOrder(textBoxValue);

            OrderViewModel order = (info == null) ? null : new OrderViewModel(info, true);

            return order;
        }


        /// <summary>
        /// Returns the order based on the entered order ID.
        /// </summary>
        /// <param name="textOrderID">String containing the user-entered order ID</param>
        /// <returns>Order object of the order</returns>
        private OrderInfo GetOrder(string textOrderID)
        {
            Int32.TryParse(textOrderID, out int orderID);

            // If the text value is not a number, returns null
            if (orderID <= 0)
            {
                return null;
            }

            // Gets the order based on the order ID
            OrderInfo order = OrderInfoProvider.GetOrderInfo(orderID);

            // Gets the current user
            var user = UserManager.FindByName(User.Identity.Name);

            // Gets the current customer
            CustomerInfo customer = shoppingService.GetCurrentCustomer();

            // Validates that the order was created on the current site and that it belongs to the current customer
            if ((order?.OrderSiteID != SiteContext.CurrentSiteID) || (order?.OrderCompletedByUserID != user?.Id))
            {
                order = null;
            }

            return order;
        }
        //EndDocSection:OrderDetails


        //DocSection:SetAsPaid
        /// <summary>
        /// Marks an order specified by an order ID as paid.
        /// </summary>
        /// <param name="textBoxValue">Order ID as a string</param>
        [HttpPost]
        public ActionResult MarkOrderAsPaid(string textBoxValue)
        {
            // Gets the order based on the entered order ID
            OrderInfo order = GetOrder(textBoxValue);

            // Sets the order as paid
            order.OrderIsPaid = true;
            OrderInfoProvider.SetOrderInfo(order);

            return RedirectToAction("OrderDetail");
        }
        //EndDocSection:SetAsPaid


        //DocSection:MyOrders
        /// <summary>
        /// Displays a listing of the current user's orders.
        /// </summary>
        public ActionResult MyOrders()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            var currentCustomer = UserInfoProvider.GetUserInfo(user.UserName);
            // Gets the current customer
            //CustomerInfo currentCustomer = shoppingService.GetCurrentCustomer();

            // If the customer does not exist, returns error 404
            if (currentCustomer == null)
            {
                return HttpNotFound();
            }
            //string Albummaster = "PrintForme.Album";
            //DataClassInfo albummasterInfo = DataClassInfoProvider.GetDataClassInfo(Albummaster);
            //List<CustomTableItem> albumItem = CustomTableItemProvider.GetItems(Albummaster)
            //                 .WhereEquals("UserID", user.Id).ToList();

            //if(albumItem != null)
            //{
            //    foreach(var album in albumItem)
            //    {
            //        string SKUID = album.GetValue("SKUID", "");
            //        QueryDataParameters parameter = new QueryDataParameters();
            //        parameter.Add("@SKUID", SKUID);

            //        DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_UpdateSKUImage", parameter, QueryTypeEnum.StoredProcedure);
            //    }
            //}

            // Retrieves from the database a collection of all orders made by the current customer 
            var orders = OrderInfoProvider.GetOrders(SiteContext.CurrentSiteID)
                                                           .WhereEquals("OrderCreatedByUserID", user.Id)
                                                           .OrderByDescending(orderInfo => orderInfo.OrderDate);

            // Creates a list of view models representing the collection of a customer's orders
            IList<OrderViewModel> model = orders.Select(order => new OrderViewModel(order, true)).ToList();

            
            return View(model);
        }
        //EndDocSection:MyOrders


        //DocSection:Reorder
        /// <summary>
        /// Recreates shopping cart content based on a specified order.
        /// </summary>
        /// <param name="orderId">ID of an order to repurchase</param>
        [HttpPost]
        public ActionResult Reorder(int orderId)
        {
            // Gets the current shopping cart
            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            // Adds products from the specified order to the current shopping cart
            // If the operation was successful, redirects to the shopping cart
            if (ShoppingCartInfoProvider.UpdateShoppingCartFromOrder(cart, orderId))
            {
                // Displays the shopping cart
                return RedirectToAction(nameof(CheckoutController.ShoppingCart), "Checkout");
            }

            // If the reorder was unsuccessful, returns back to the list of customer's orders
            return RedirectToAction(nameof(OrderController.MyOrders), nameof(OrderController));
        }
        //EndDocSection:Reorder
    }
}