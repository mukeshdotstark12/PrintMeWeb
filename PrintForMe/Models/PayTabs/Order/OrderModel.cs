using CMS.Base;
using CMS.Ecommerce;
using PrintForMe.Models.OrderManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrintForMe.Models
{
    public class OrderModel
    {
        public int OrderID { get; set; }
        public string MerchantEmail { get; set; }
        public string SecretKey { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public string SiteUrl { get; set; }
        public string Title { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string ProductsPerTitle { get; set; }
        public string ReturnUrl { get; set; }
        public string CcFirstName { get; set; }
        public string CcLastName { get; set; }
        public string CcPhoneNumber { get; set; }
        public string Phonenumber { get; set; }
        public string BillingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string AddressShipping { get; set; }
        public string CityShipping { get; set; }
        public string StateShipping { get; set; }
        public string PostalCodeShipping { get; set; }
        public string CountryShipping { get; set; }
        public string PaymentReference { get; set; }
        public DateTime PaymentDate { get; set; }

        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }
        public OrderAddressViewModel OrderAddress { get; set; }
        /// <summary>
        /// Constructor for the OrderViewModel. 
        /// </summary>
        /// <param name="order">A Order object.</param>
        public OrderModel(OrderInfo order)
        {
            // Creates a collection containing all lines from the given shopping cart
            OrderItems = OrderItemInfoProvider.GetOrderItems(order.OrderID).Select(orderItem =>
            {
                return new OrderItemViewModel
                {
                    SKUID = orderItem.OrderItemSKUID,
                    SKUName = orderItem.OrderItemSKUName,
                    SKUImagePath = orderItem.OrderItemSKU.SKUImagePath,
                    TotalPriceInMainCurrency = orderItem.OrderItemTotalPriceInMainCurrency,
                    UnitCount = orderItem.OrderItemUnitCount,
                    UnitPrice = orderItem.OrderItemUnitPrice
                };
            });
            OrderID = order.OrderID;
            TotalPrice = OrderItems.Select(x => x.UnitCount * x.UnitPrice).Sum();
            MerchantEmail = Constants.PayTapsMerchantEmail;
            SecretKey = Constants.PayTapsSecretKey;
            Currency = CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID).CurrencyName;

            Amount = order.OrderTotalPrice;
            GrandTotal = order.OrderGrandTotal;
            TotalShipping = order.OrderTotalShipping;
            Tax = order.OrderTotalTax;

            SiteUrl = Constants.PayTabsSiteUrl;
            Title = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID)?.CustomerFirstName + " " + CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID)?.CustomerLastName;

            //Quantity = OrderItems.Select(x => x.UnitCount).Join(x => x.UnitCount, " || ");
            //UnitPrice = "1155.000000000";
            //ProductsPerTitle = "T-Shirt";

            Quantity = string.Join(" || ", OrderItems.Where(s => s != null).Select(i => i.UnitCount));

            UnitPrice = string.Join(" || ", OrderItems.Where(s => s != null).Select(i => i.UnitPrice.TrimEnd()));

            ProductsPerTitle = string.Join(" || ", OrderItems.Where(s => s != null).Select(i => i.SKUName.Trim()));

            ReturnUrl = Constants.PayTabsReturnUrl;
            CcFirstName = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID)?.CustomerFirstName;
            CcLastName = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID)?.CustomerLastName;
            Phonenumber = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID)?.CustomerPhone;
            CcPhoneNumber = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID)?.CustomerPhone;

            OrderAddress = new OrderAddressViewModel(order.OrderBillingAddress);
            BillingAddress = $"{OrderAddress.AddressLine1} {OrderAddress.AddressLine2}";
            City = OrderAddress.AddressCity;
            State = OrderAddress.AddressState;
            PostalCode = OrderAddress.AddressPostalCode;
            Country = "ARE";// OrderAddress.AddressCountry;
            Email = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID)?.CustomerEmail;
            AddressShipping = BillingAddress;
            CityShipping = OrderAddress.AddressCity;
            StateShipping = OrderAddress.AddressState;
            PostalCodeShipping = OrderAddress.AddressPostalCode;
            CountryShipping = "ARE";//OrderAddress.AddressCountry;
            PaymentDate = DateTime.Now;
        }
    }
}