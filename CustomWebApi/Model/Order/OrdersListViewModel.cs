using CMS.Ecommerce;
using CustomWebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Order
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
        public string FormatedPriceWithoutShippingFee { get; set; }
        public string FormatedShippingFee { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PriceWithoutShippingFee { get; set; }
        public decimal ShippingFee { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string OrderTrackingNumber { get; set; }
        public string PaymentOption { get; set; }
        public List<OrderStatusInfo> Statuses { get; set; }
        public OrderAddressViewModel OrderAddress { get; set; }
        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }

        public OrdersListViewModel(OrderInfo order)
        {           
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
            PriceWithoutShippingFee = order.OrderTotalPrice - order.OrderTotalShipping;
            FormatedPriceWithoutShippingFee = String.Format(CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID).CurrencyFormatString, PriceWithoutShippingFee); ;
            ShippingFee = order.OrderTotalShipping;
            FormatedShippingFee = String.Format(CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID).CurrencyFormatString, order.OrderTotalShipping); ;
            var customerInfo = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID);
            CustomerName = string.Format("{0} {1}", customerInfo.CustomerFirstName, customerInfo.CustomerLastName);
            CustomerEmail = customerInfo.CustomerEmail;
            StatusID = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusID;
            //Statuses = OrderStatusInfoProvider.GetOrderStatuses().ToList();
            ItemCount = OrderItemInfoProvider.GetOrderItems(order.OrderID).Count;
            PaymentOption = PaymentOptionInfoProvider.GetPaymentOptionInfo(order.OrderPaymentOptionID)?.PaymentOptionDisplayName;
            CustomerMobile = customerInfo.CustomerPhone;
            OrderAddress = new OrderAddressViewModel(order.OrderBillingAddress);
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
            }).Take(4);
            
        }
    }    
}
