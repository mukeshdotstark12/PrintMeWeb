using System;
using System.Collections.Generic;

namespace PrintForMe.Models.OrderManagement
{
    public class OrderDetailViewModel
    {
        private readonly string currencyFormatString;


        public string InvoiceNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderID { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal OrderTotalTax { get; set; }
        public decimal OrderTotalShipping { get; set; }
        public decimal OrderGrandTotal { get; set; }
        public string StatusName { get; set; }
        public string OrderNote { get; set; }
        public bool OrderGiftWrapping { get; set; }


        public OrderAddressViewModel OrderAddress { get; set; }

        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }
        public string PaymentOption { get; set; }

        public OrderDetailViewModel(string currencyFormatString)
        {
            if (String.IsNullOrEmpty(currencyFormatString))
            {
                throw new ArgumentException($"{nameof(currencyFormatString)} is not defined.");
            }

            this.currencyFormatString = currencyFormatString;
        }

        public string FormatPrice(decimal price)
        {
            return String.Format(currencyFormatString, price);
        }
    }
}