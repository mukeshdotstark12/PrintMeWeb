using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.Customer
{
    public class UpdateAddressDto
    {
        public int customerID { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string addressName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressCity { get; set; }
        public string addressZip { get; set; }
        public string addressPhone { get; set; }
    }
}