using CMS.Ecommerce;
using PrintForMe.Helpers;
using PrintForMe.Models.OrderManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrintForMe.Models.Checkout
{
    //DocSection:OrderViewModel
    public class OrderViewModel
    {
        public int OrderID { get; set; }

        public string InvoiceNumber { get; set; }

        public int OrderStatusID { get; set; }

        public decimal OrderTotalTax { get; set; }
        public decimal OrderTotalShipping { get; set; }
        public decimal OrderGrandTotal { get; set; }

        public string CurrencyFormatString { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal OrderTotalPrice { get; set; }

        public bool OrderIsPaid { get; set; }

        public OrderPaymentResultViewModel OrderPaymentResult { get; set; }

        public OrderAddressViewModel OrderAddress { get; set; }

        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }
        public string PaymentOption { get; set; }
        public string OrderTrackingNumber { get; set; }
        public string OrderStatusDisplayName { get; set; }

        public OrderViewModel(OrderInfo order, bool isProjectDetail)
        {
            OrderID = order.OrderID;
            InvoiceNumber = order.OrderInvoiceNumber;
            if (OrderInfoProvider.GetOrderInfo(order.OrderID).GetValue("OrderTrackingNumber") != null)
            {
                OrderTrackingNumber = OrderInfoProvider.GetOrderInfo(order.OrderID).GetValue("OrderTrackingNumber").ToString();
            }
            OrderStatusID = order.OrderStatusID;
            CurrencyFormatString = CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID).CurrencyFormatString;
            OrderDate = order.OrderDate;
            OrderTotalPrice = order.OrderTotalPrice;
            OrderGrandTotal = order.OrderGrandTotal;
            OrderTotalTax = order.OrderTotalTax;
            OrderTotalShipping = order.OrderTotalShipping;
            OrderIsPaid = order.OrderIsPaid;
            OrderStatusDisplayName = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusName;
            OrderAddress = new OrderAddressViewModel(order.OrderBillingAddress);
            PaymentOption = PaymentOptionInfoProvider.GetPaymentOptionInfo(order.OrderPaymentOptionID)?.PaymentOptionDisplayName;
            if (order.OrderPaymentResult != null)
            {
                OrderPaymentResult = new OrderPaymentResultViewModel()
                {
                    PaymentMethodName = order.OrderPaymentResult.PaymentMethodName,
                    PaymentIsCompleted = order.OrderPaymentResult.PaymentIsCompleted
                };
            }
            OrderItems = OrderItemInfoProvider.GetOrderItems(order.OrderID).Select(orderItem =>
            {
                return new OrderItemViewModel
                {
                    SKUID = orderItem.OrderItemSKUID,
                    SKUName = orderItem.OrderItemSKUName,
                    SKUDepartmentID= orderItem.OrderItemSKU.SKUDepartmentID,
                    SKUSize = SKUInfoProvider.GetSKUInfo(orderItem.OrderItemSKUID) != null ?
                        SKUInfoProvider.GetSKUInfo(orderItem.OrderItemSKUID).SKUNumber : string.Empty,
                    SKUImagePath = orderItem.OrderItemSKU.SKUImagePath,
                    TotalPriceInMainCurrency = orderItem.OrderItemTotalPriceInMainCurrency,
                    UnitCount = orderItem.OrderItemUnitCount,
                    UnitPrice = orderItem.OrderItemUnitPrice,
                    serviceDetail = isProjectDetail ?
                                    ServiceInformation.GetProjectInformation(orderItem.OrderItemSKUID,"",true) :
                                    new ServiceDetail()
                };
            }).Take(4);
        }
    }
    //EndDocSection:OrderViewModel
}