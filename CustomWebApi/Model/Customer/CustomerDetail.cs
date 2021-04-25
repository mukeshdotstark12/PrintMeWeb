using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.Customer
{
    public class CustomerDetail
    {
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerFax { get; set; }
        public int CustomerUserID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerInfoName { get; set; }
        public DateTime CustomerCreated { get; set; }
        public string CustomerOrganizationID { get; set; }
        public string CustomerTaxRegistrationID { get; set; }
        public string CustomerGUID { get; set; }
        public string CustomerCompany { get; set; }
    }
}