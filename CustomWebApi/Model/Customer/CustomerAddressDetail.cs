using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.Customer
{
    public class CustomerAddressDetail
    {
        public string AddressCity { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressPersonalName { get; set; }
        public string AddressName { get; set; }
        public int AddressID { get; set; }
        public int AddressCountryID { get; set; }
        public string AddressPhone { get; set; }
        public int AddressCustomerID { get; set; }
        public string AddressZip { get; set; }
        public Guid AddressGUID { get; set; }
        public int AddressStateID { get; set; }
        public DateTime AddressLastModified { get; set; }
    }
}