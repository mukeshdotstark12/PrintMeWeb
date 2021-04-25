using CMS.Ecommerce;
using System;
using System.Collections.Generic;

namespace PrintForMe.Models.OrderManagement
{
    public class OrdersListViewModel
    {
        public int OrderID { get; set; }


        public string OrderInvoiceNumber { get; set; }
        public int ItemCount { get; set; }

        public DateTime OrderDate { get; set; }
        public string DisplayOrderDate { get; set; }


        public string StatusName { get; set; }
        public int? StatusID { get; set; }


        public string FormattedTotalPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string OrderTrackingNumber { get; set; }
        public string PaymentOption { get; set; }
        public IEnumerable<OrderStatusInfo> Statuses { get; set; }

        public OrdersListViewModel()
        {
        }


        public OrdersListViewModel(OrderInfo order)
        {
            if (order == null)
            {
                return;
            }

            OrderID = order.OrderID;
            OrderInvoiceNumber = order.OrderInvoiceNumber;
            OrderDate = order.OrderDate;
            if (OrderInfoProvider.GetOrderInfo(order.OrderID).GetValue("OrderTrackingNumber") != null)
            {
                OrderTrackingNumber = OrderInfoProvider.GetOrderInfo(order.OrderID).GetValue("OrderTrackingNumber").ToString();
            }
            StatusName = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusDisplayName;
            TotalPrice = order.OrderTotalPrice;
            FormattedTotalPrice = String.Format(CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID).CurrencyFormatString, order.OrderTotalPrice);
            var customerInfo = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID);
            CustomerName = string.Format("{0} {1}", customerInfo.CustomerFirstName, customerInfo.CustomerLastName);
            CustomerEmail = customerInfo.CustomerEmail;
            StatusID = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusID;
            //Statuses = OrderStatusInfoProvider.GetOrderStatuses().ToList();
            ItemCount = OrderItemInfoProvider.GetOrderItems(order.OrderID).Count;
            PaymentOption = PaymentOptionInfoProvider.GetPaymentOptionInfo(order.OrderPaymentOptionID)?.PaymentOptionDisplayName;
            CustomerMobile = customerInfo.CustomerPhone;
        }
    }
}