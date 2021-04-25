using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CustomWebApi.Helpers;
using CustomWebApi.Model.Order;
using CustomWebApi.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CustomWebApi.Controllers
{
    public class OrderController : ApiController
    {
        #region GET        
        [HttpGet]
        public HttpResponseMessage GetOrderByUserId(int userId)
        {
            try
            {
                // Gets the user
                UserInfo user = UserInfoProvider.GetUserInfo(userId);

                if (user != null)
                {
                    var orders = OrderInfoProvider.GetOrders().WhereEquals("OrderCreatedByUserID", userId);
                    List<OrdersListViewModel> orderList = new List<OrdersListViewModel>();
                    if (orders != null)
                    {
                        foreach (var order in orders)
                        {
                            orderList.Add(new OrdersListViewModel(order));
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = orderList,
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
        public HttpResponseMessage GetOrderDetail(int orderID)
        {
            try
            {
                // Gets the user
                var order = OrderInfoProvider.GetOrderInfo(ValidationHelper.GetInteger(orderID,0));

                if (order != null)
                {
                    var currency = CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID);

                    OrderDetailViewModel orderDetail = new OrderDetailViewModel(currency.CurrencyFormatString)
                    {
                        OrderDate = order.OrderDate,
                        OrderID = order.OrderID,
                        InvoiceNumber = order.OrderInvoiceNumber,
                        TotalPrice = order.OrderTotalPrice,
                        StatusName = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusDisplayName,
                        OrderAddress = new OrderAddressViewModel(order.OrderBillingAddress),
                        PaymentOption = PaymentOptionInfoProvider.GetPaymentOptionInfo(order.OrderPaymentOptionID)?.PaymentOptionDisplayName,

                        OrderGrandTotal = order.OrderGrandTotal,
                        OrderTotalTax = order.OrderTotalTax,
                        OrderTotalShipping = order.OrderTotalShipping,
                        OrderItems = OrderItemInfoProvider.GetOrderItems(order.OrderID).Select(orderItem =>
                        {
                            return new OrderItemViewModel
                            {
                                SKUID = orderItem.OrderItemSKUID,
                                SKUName = orderItem.OrderItemSKUName,                                
                                SKUImagePath = orderItem.OrderItemSKU.SKUImagePath,
                                TotalPriceInMainCurrency = orderItem.OrderItemTotalPriceInMainCurrency,
                                UnitCount = orderItem.OrderItemUnitCount,
                                UnitPrice = orderItem.OrderItemUnitPrice,
                                serviceDetail = ServiceInformation.GetProjectInformation(orderItem.OrderItemSKUID)
                            };
                        })
                    };

                    List<OrderDetailViewModel> orderDetailArray = new List<OrderDetailViewModel>();
                    orderDetailArray.Add(orderDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = orderDetailArray,
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
                        description = "Order not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Incorrect OrderID..!"
                });
            }
        }

        #endregion
    }   
}
