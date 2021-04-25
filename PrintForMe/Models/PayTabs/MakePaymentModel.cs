using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for Models
/// </summary>

namespace PrintForMe.Models.PayTabs
{
    public class MakePaymentModel
    {

        #region "Variables"


        #endregion



        #region "Constructor"

        public MakePaymentModel()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #endregion

        public class Settings
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SecretKey { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool InvalidSecretKey { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string EmailAddress { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SiteUrl { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string LastPaymentReferenceNumber { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string CurrentActivePaymentID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<PayPageRequest> PageRequestList { get; set; }

            #endregion
        }

        public class PayTabsVerifyPaymentResponse
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string result { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string response_code { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string error_code { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string pt_invoice_id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string amount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string reference_no { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string transaction_id { get; set; }

            #endregion
        }

        public class PayTabsCreatePaymentResponse
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string result { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string response_code { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string payment_url { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string p_id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string error_code { get; set; }

            #endregion
        }

        public class VerifySecretKeyRequest
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string MerchantEmail { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SecretKey { get; set; }

            #endregion
        }

        public class VerifySecretKeyResponse
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string result { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string response_code { get; set; }

            #endregion
        }

        public class VerifyPaymentRequest
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string MerchantEmail { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string MerchantPassword { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string ReferenceNumber { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SecretKey { get; set; }

            #endregion
        }

        public class VerifyPaymentResponse
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string result { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string response_code { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string error_code { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string pt_invoice_id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string amount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string reference_no { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string transaction_id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string shipping_address { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string shipping_city { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string shipping_country { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string shipping_state { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string shipping_postalcode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string phone_num { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string customer_name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string email { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string detail { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string reference_id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string invoice_id { get; set; }

            #endregion
        }

        public class ReportRequest
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string MerchantEmail { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SecretKey { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string StartDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string EndDate { get; set; }

            #endregion
        }

        public class RefundRequest
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string MerchantEmail { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SecretKey { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string PageId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string RefundAmount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string RefundReason { get; set; }

            #endregion
        }

        public class RefundResponse
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string MerchantEmail { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string MerchantPassword { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string PageId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string RefundAmount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string RefundReason { get; set; }

            #endregion
        }

        public class Country
        {
            #region "Properties"

            /// <summary>
            /// 
            /// </summary>
            public string CountryName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string CountryCode { get; set; }

            #endregion
        }

        public class PayPageRequest
        {
            #region "Properties"


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

            #endregion
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

        public class PaymentVerication
        {
            public int profile_id { get; set; }
            public string tran_ref { get; set; }
        }

        public class User_defined
        {
            public string udf9 { get; set; }
            public string udf3 { get; set; }
        }


        //public class PaymentResultInfo
        //{
        //    public string card_type {​​​​​​​​ get; set; }​​​​​​​​
        //    public string card_scheme {​​​​​​​​ get; set; }​​​​​​​​
        //    public string payment_description {​​​​​​​​ get; set; }​​​​​​​​
        //}

        public class PayPageResponse
        {
            #region "Properties"

            public string result { get; set; }
            public string response_code { get; set; }
            public string redirect_url { get; set; }
            public string tran_ref { get; set; }
            public string error_code { get; set; }

            #endregion
        }

        public class PayTabsMakePaymentResponse
        {
            #region "Properties"


            public string message { get; set; }
            public string response { get; set; }
            public string error_code { get; set; }
            public string payment_url { get; set; }
            public string api_key { get; set; }

            #endregion
        }



        public class PaymentsInfo
        {
            #region "Properties"

            public string Name { get; set; }
            public string Amount { get; set; }
            public string Email { get; set; }
            public string AddressShipping { get; set; }

            #endregion
        }

        public class PaymentsList
        {
            #region "Properties"

            public List<PayPageRequest> PayPageRequests { get; set; }

            #endregion
        }

        public class ReportResponse
        {
            #region "Properties"

            public string status { get; set; }
            public string transaction_id { get; set; }
            public string transaction_title { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string datetime { get; set; }

            #endregion
        }
    }

}