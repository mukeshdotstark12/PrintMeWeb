using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.Order
{
    public class OrderData
    {
        public int OrderID { get; set; }
        public int PaymentReferenceID { get; set; }
    }
}