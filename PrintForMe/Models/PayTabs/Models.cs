using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for Models
/// </summary>
public class Models
{

    #region "Variables"


    #endregion



    #region "Constructor"

    public Models()
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
        public string Currency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UnitPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProductsPerTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CcFirstNname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CcLastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CcPhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Phonenumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BillingAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public string Country { get; set; }
        public string Email { get; set; }
        public string AddressShipping { get; set; }
        public string CityShipping { get; set; }
        public string StateShipping { get; set; }
        public string PostalCodeShipping { get; set; }
        public string CountryShipping { get; set; }
        public string PaymentReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public object OrderID { get; set; }

        #endregion
    }

    public class PayPageResponse
    {
        #region "Properties"

        public string result { get; set; }
        public string response_code { get; set; }
        public string payment_url { get; set; }
        public string p_id { get; set; }
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