using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.Customer
{
    public class UpdateCustomer
    {
        public int CustomerID { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }        
        public int CustomerUserID { get; set; }
        public string CustomerPhone { get; set; }
    }
}