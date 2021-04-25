using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrintForMe.Models.PayTabs
{
    public class PaymentVerficationResult
    {
        public string tran_ref { get; set; }
        public string cart_id { get; set; }
        public string cart_description { get; set; }
        public string cart_currency { get; set; }
        public string cart_amount { get; set; }
        //public CustomerDetails customer_details { get; set; }
        public PaymentResult payment_result { get; set; }
        //public PaymentResultInfo payment_info { get; set; }
    }

    public class PaymentResult
    {
        public string response_status { get; set; }
        public string response_message { get; set; }
    }
}