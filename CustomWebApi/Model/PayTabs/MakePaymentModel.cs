using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.PayTabs
{
    public class MakePaymentModel
    {
        public class PayPageRequest
        {
            public string profile_id { get; set; }
            public string tran_type { get; set; }
            public string tran_class { get; set; }
            public string cart_id { get; set; }

            public string cart_currency { get; set; }
            public decimal cart_amount { get; set; }
            public string cart_description { get; set; }
            public string paypage_lang { get; set; }

            public Customer_details customer_details { get; set; }

            public Shipping_details shipping_details { get; set; }

            public string Currency { get; set; }
            public string Amount { get; set; }

            public decimal TotalPrice { get; set; }
            public decimal TotalShipping { get; set; }
            public decimal GrandTotal { get; set; }
            public decimal Tax { get; set; }

            public string SiteUrl { get; set; }
            public string Title { get; set; }

            public string callback { get; set; }
            public string _return { get; set; }

            public User_defined user_defined { get; set; }
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
            public string OrderID { get; set; }
        }

        public class Customer_details
        {
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string street1 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string zip { get; set; }
        }

        public class Shipping_details
        {
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string street1 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string zip { get; set; }
        }

        public class User_defined
        {
            public string udf9 { get; set; }
            public string udf3 { get; set; }
        }

        public class PayPageResponse
        {
            public string result { get; set; }
            public string response_code { get; set; }
            public string redirect_url { get; set; }
            public string tran_ref { get; set; }
            public string error_code { get; set; }
        }

        public class VerifySecretKeyRequest
        {
            public string MerchantEmail { get; set; }
            public string SecretKey { get; set; }
        }

        public class VerifySecretKeyResponse
        {
            public string result { get; set; }
            public string response_code { get; set; }
        }
        public class PayTabsVerifyPaymentResponse
        {
            public string result { get; set; }
            public string response_code { get; set; }
            public string error_code { get; set; }
            public string pt_invoice_id { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string reference_no { get; set; }
            public string transaction_id { get; set; }
        }
        public class VerifyPaymentResponse
        {
            public string result { get; set; }
            public string response_code { get; set; }
            public string error_code { get; set; }
            public string pt_invoice_id { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string reference_no { get; set; }
            public string transaction_id { get; set; }

            public string shipping_address { get; set; }
            public string shipping_city { get; set; }
            public string shipping_country { get; set; }
            public string shipping_state { get; set; }
            public string shipping_postalcode { get; set; }
            public string phone_num { get; set; }
            public string customer_name { get; set; }
            public string email { get; set; }
            public string detail { get; set; }
            public string reference_id { get; set; }
            public string invoice_id { get; set; }
        }

        public class VerifyPaymentRequest
        {
            public string MerchantEmail { get; set; }
            public string MerchantPassword { get; set; }
            public string ReferenceNumber { get; set; }
            public string SecretKey { get; set; }
        }

        public class ReportRequest
        {
            public string MerchantEmail { get; set; }
            public string SecretKey { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
        }
        public class RefundRequest
        {
            public string MerchantEmail { get; set; }
            public string SecretKey { get; set; }
            public string PageId { get; set; }
            public string RefundAmount { get; set; }
            public string RefundReason { get; set; }
        }

    }
}
