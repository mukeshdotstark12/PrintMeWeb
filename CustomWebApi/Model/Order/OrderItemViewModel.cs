using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Model.Order
{
    public class OrderItemViewModel
    {
        public int SKUID { get; set; }

        public string SKUImagePath { get; set; }

        public string SKUName { get; set; }

        public int UnitCount { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPriceInMainCurrency { get; set; }
        public ServiceDetail serviceDetail { get; set; }
    }
}