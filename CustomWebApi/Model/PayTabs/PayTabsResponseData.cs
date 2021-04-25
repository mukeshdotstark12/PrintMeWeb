using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.PayTabs
{
    public class PayTabsResponseData
    {
        public string result { get; set; }
        public string response_code { get; set; }
        public string error_code { get; set; }
        public string pt_invoice_id { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string reference_no { get; set; }
        public string transaction_id { get; set; }
        public int order_id { get; set; }
        public int orderStatusID { get; set; }
    }
}