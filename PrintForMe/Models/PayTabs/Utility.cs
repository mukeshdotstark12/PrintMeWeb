using Newtonsoft.Json;
using PrintForMe.Models;
using PrintForMe.Models.PayTabs;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for Utility
/// </summary>
public class Utility : System.Web.UI.Page
{

    #region "Variables"

    /// <summary>
    /// 
    /// </summary>
    public const string ConstCreatePayPage = "create_pay_page";

    /// <summary>
    /// 
    /// </summary>
    public const string ConstValidateKey = "validate_secret_key";

    /// <summary>
    /// 
    /// </summary>
    public const string ConstRefundProcess = "refund_process";

    /// <summary>
    /// 
    /// </summary>
    public const string ConstTransactionReports = "transaction_reports";

    /// <summary>
    /// 
    /// </summary>
    public const string ConstGenerateIPN = "Generate_IPN";

    /// <summary>
    /// 
    /// </summary>
    public const string ConstVerifyPayment = "verify_payment";

    /// <summary>
    /// 
    /// </summary>
    public enum PayTabRequestType
    {
        ValidateSecretKey = 1,
        CreatePayPage,
        VerifyPayment,
        RefundTransactions,
        PayTabsIPIN,
        TransactionsReports
    }

    #endregion

    #region "Properties"

    #endregion

    #region "Constructor"

    public Utility()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    #endregion

    #region "Constructor"

    /// <summary>
    /// 
    /// </summary>
    /// <param name="methodCall"></param>
    /// <param name="formContent"></param>
    /// <param name="paymentGatewayAPRURL"></param>
    /// <returns></returns>
    public string MakeWebServiceCall(string methodCall, string formContent, string paymentGatewayUrl)
    {
        string ApiURL = paymentGatewayUrl;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + methodCall);
        request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequired;
        request.Method = "POST";

        byte[] byteArray;
        WebResponse response;
        StreamReader reader;
        Stream dataStream;
        string responseFromServer;

        byteArray = Encoding.UTF8.GetBytes(formContent);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteArray.Length;
        dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();
        response = request.GetResponse();
        dataStream = response.GetResponseStream();
        reader = new StreamReader(dataStream);
        responseFromServer = HttpUtility.UrlDecode(reader.ReadToEnd());

        reader.Close();
        dataStream.Close();
        response.Close();
        return responseFromServer;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="formContent"></param>
    /// <param name="clientWebSiteURL"></param>
    /// <returns></returns>
    public string MakeWebSiteCallFromHandler(string formContent, string clientWebSiteURL)
    {
        string ApiURL = clientWebSiteURL;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL + "/" + "IPNListener.aspx");
        request.Method = "POST";

        byte[] byteArray;
        WebResponse response;
        StreamReader reader;
        Stream dataStream;
        string responseFromServer;

        byteArray = Encoding.UTF8.GetBytes(formContent);
        //request.ContentType = "application/x-www-form-urlencoded";

        request.ContentType = "application/json;";
        //request.Headers[HttpRequestHeader.ContentType] = "application/json;";
        request.ContentLength = byteArray.Length;
        dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();
        response = request.GetResponse();
        dataStream = response.GetResponseStream();
        reader = new StreamReader(dataStream);
        responseFromServer = HttpUtility.UrlDecode(reader.ReadToEnd());

        reader.Close();
        dataStream.Close();
        response.Close();

        return responseFromServer;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GenerateReferenceNumber()
    {
        return "PT" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="QueryString"></param>
    /// <param name="Parameter"></param>
    /// <returns></returns>
    public string ReturnQueryParameterValue(string QueryString, string Parameter)
    {

        NameValueCollection myCol = HttpUtility.ParseQueryString(QueryString);
        string ParamValue = QueryString.Contains("=") ? myCol[Parameter] : QueryString;

        return ParamValue;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public string GetCreatePayPageRespons(int Key)
    {
        string ResponseString = "";
        switch (Key)
        {
            case 0: ResponseString = " User does not have access to Application"; break;
            case 1: ResponseString = " Incorrect Username or Password"; break;
            case 2: ResponseString = " Invalid User ID"; break;
            case 3: ResponseString = " You do not have the access rights."; break;
        }

        return ResponseString;
    }

    //public void CheckSession()
    //{
    //    if (Session["ActiveClient"] == null)
    //    {
    //        Response.Redirect("~/Default.aspx");
    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objPayPageRequest"></param>
    /// <returns></returns>
    public string CreatePayPage(Models.PayPageRequest objPayPageRequest)
    {
        return "merchant_email=" + objPayPageRequest.MerchantEmail
                            + "&secret_key=" + objPayPageRequest.SecretKey
                            + "&currency=" + objPayPageRequest.Currency
                            + "&amount=" + objPayPageRequest.Amount
                            + "&site_url=" + objPayPageRequest.SiteUrl
                            + "&title=" + objPayPageRequest.ProductsPerTitle
                            + "&quantity=" + objPayPageRequest.Quantity
                            + "&unit_price=" + objPayPageRequest.UnitPrice
                            + "&products_per_title=" + objPayPageRequest.ProductsPerTitle
                            + "&return_url=" + objPayPageRequest.ReturnUrl
                            + "&cc_first_name=" + objPayPageRequest.CcFirstNname
                            + "&cc_last_name=" + objPayPageRequest.CcLastName
                            + "&cc_phone_number=" + objPayPageRequest.CcPhoneNumber
                            + "&phone_number=" + objPayPageRequest.CcPhoneNumber
                            + "&billing_address=" + objPayPageRequest.BillingAddress
                            + "&city=" + objPayPageRequest.City
                            + "&state=" + objPayPageRequest.State
                            + "&postal_code=" + objPayPageRequest.PostalCode
                            + "&country=" + objPayPageRequest.Country
                            + "&email=" + objPayPageRequest.Email
                            + "&ip_customer=" + System.Net.Dns.GetHostName()
                            + "&ip_merchant=100.100.100.100"
                            + "&address_shipping=" + objPayPageRequest.AddressShipping
                            + "&city_shipping=" + objPayPageRequest.CityShipping
                            + "&state_shipping=" + objPayPageRequest.StateShipping
                            + "&postal_code_shipping=" + objPayPageRequest.PostalCodeShipping
                            + "&country_shipping=" + objPayPageRequest.CountryShipping
                            + "&other_charges=0"
                            + "&discount=0"
                            + "&reference_no=" + GenerateReferenceNumber()
                            + "&msg_lang=English"
                            + "&cms_with_version=API";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objRefundRequest"></param>
    /// <returns></returns>
    public string MakeRefund(Models.RefundRequest objRefundRequest)
    {
        return "merchant_email=" + objRefundRequest.MerchantEmail
                + "&secret_key=" + objRefundRequest.SecretKey
                + "&paypage_id=" + objRefundRequest.PageId
                + "&refund_amount=" + objRefundRequest.RefundAmount
                + "&refund_reason=" + objRefundRequest.RefundReason;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objReportRequest"></param>
    /// <returns></returns>
    public string ReturnTransactionReport(Models.ReportRequest objReportRequest)
    {
        return "merchant_email=" + objReportRequest.MerchantEmail
               + "&secret_key=" + objReportRequest.SecretKey
               + "&startdate=" + objReportRequest.StartDate //DateTime.Now.Date.AddDays(-10).ToString("d")
               + "&enddate=" + objReportRequest.EndDate; // DateTime.Now.Date.ToString("d");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objReportRequest"></param>
    /// <returns></returns>
    public string PayTabsIPN(Models.ReportRequest objReportRequest)
    {
        return "merchant_email=" + objReportRequest.MerchantEmail
                + "&secret_key=" + objReportRequest.SecretKey;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objRequest"></param>
    /// <returns></returns>
    public string ValidateSecretKey(Models.VerifySecretKeyRequest objRequest)
    {
        return "merchant_email=" + objRequest.MerchantEmail
               + "&secret_key=" + objRequest.SecretKey;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objPaymentRequest"></param>
    /// <param name="paymentGatewayAPRURL"></param>
    /// <returns></returns>
    public Models.VerifyPaymentResponse VerifyPayment(Models.VerifyPaymentRequest objPaymentRequest, string paymentGatewayAPRURL)
    {

        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        string ApiURL = paymentGatewayAPRURL;

        string response = client.UploadString(ApiURL + "/" + ConstVerifyPayment.ToString(),
            "secret_key=" + objPaymentRequest.SecretKey
            + "&merchant_email=" + objPaymentRequest.MerchantEmail
            + "&merchant_password=" + objPaymentRequest.MerchantPassword
            + "&payment_reference=" + objPaymentRequest.ReferenceNumber);

        return JsonConvert.DeserializeObject<Models.VerifyPaymentResponse>(response);
    }

    public MakePaymentModel.VerifyPaymentResponse VerifyPayment(MakePaymentModel.VerifyPaymentRequest objPaymentRequest)
    {

        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        //string ApiURL = ConfigurationManager.AppSettings["PaymentGatewayAPRURL"];
        string ApiURL = Constants.PaymentGatewayAPRURL;

        string response = client.UploadString(ApiURL + "/" + ConstVerifyPayment.ToString(),
            "merchant_email=" + objPaymentRequest.MerchantEmail
            + "&secret_key=" + objPaymentRequest.SecretKey
            + "&payment_reference=" + objPaymentRequest.ReferenceNumber);

        return JsonConvert.DeserializeObject<MakePaymentModel.VerifyPaymentResponse>(response);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestType"></param>
    /// <param name="Key"></param>
    /// <param name="erroMessageOther"></param>
    /// <returns></returns>
    public string GetPayTabResponseMessage(PayTabRequestType requestType, string Key, string erroMessageOther = "") //When this method is called for requestType = VerifyPayment then pass third parameter as error received from PayTabs
    {
        string ResponseString = "Unassigned";
        switch (requestType)
        {
            case PayTabRequestType.ValidateSecretKey:
                switch (Key)
                {
                    case "4000": ResponseString = "Valid Secret Key"; break;
                    case "4001": ResponseString = "Missing secret_key or merchant_email parameter"; break;
                    case "4002": ResponseString = "Invalid Secret Key"; break;
                }
                break;
            case PayTabRequestType.CreatePayPage:
                switch (Key)
                {
                    case "4012": ResponseString = "PayPage created successfully"; break;
                    case "4404": ResponseString = "You dont have permissions to create an Invoice"; break;
                    case "4001": ResponseString = "Variable not found"; break;
                    case "4002": ResponseString = "Invalid Credentials."; break;
                    case "4007": ResponseString = "'currency' code used is invalid. Only 3 character ISO currency codes are valid."; break;
                    case "4008": ResponseString = "Your SITE URL is not matching with your profile URL"; break;
                    case "4013": ResponseString = "Your 'amount' post variable should be between 0.27 and 5000.00 USD"; break;
                    case "4014": ResponseString = "Products titles,Prices,quantity are not matching"; break;
                    case "4094": ResponseString = "Your total amount is not matching with the sum of unit price amount per quantity."; break;
                }
                break;
            case PayTabRequestType.VerifyPayment:
                switch (Key)
                {
                    case "4001": ResponseString = "Missing parameters"; break;
                    case "4002": ResponseString = "Invalid Credentials"; break;
                    case "0404": ResponseString = "You dont have permissions"; break;
                    case "400": ResponseString = "There are no transactions available."; break;
                    case "100": ResponseString = "Payment is completed."; break;
                    case "481":
                    case "482": ResponseString = "This transaction may be suspicious. If this transaction is genuine, please contact PayTabs customer service to enquire about the feasibility of processing this transaction."; break;
                    default: ResponseString = erroMessageOther; break;
                }
                break;
            case PayTabRequestType.RefundTransactions:
                switch (Key)
                {
                    case "4001": ResponseString = "Missing parameters"; break;
                    case "4002": ResponseString = "Invalid Credentials"; break;
                    case "810": ResponseString = "You already requested Refund for this Transaction ID"; break;
                    case "811": ResponseString = "Refund amount you requested is greater then transaction amount or Your balance is insufficient to cover the Refund Amount"; break;
                    case "812": ResponseString = "Refund request is sent to Operation for Approval. You can track the Status"; break;
                    case "813": ResponseString = "You are not authorized to view this transaction"; break;
                }
                break;
            case PayTabRequestType.PayTabsIPIN:
                switch (Key)
                {
                    case "5000": ResponseString = "Payment has been rejected"; break;
                    case "5001": ResponseString = "Payment has been accepted successfully"; break;
                    case "5002": ResponseString = "Payment has been forcefully accepted"; break;
                    case "5003": ResponseString = "Payment has been refunded"; break;
                }
                break;
            case PayTabRequestType.TransactionsReports:
                switch (Key)
                {
                    case "4001": ResponseString = "Missing parameters"; break;
                    case "4002": ResponseString = "Invalid Credentials"; break;
                    case "4006": ResponseString = "Your time interval should be less then 60 days"; break;
                    case "4090": ResponseString = "Data Found"; break;
                    case "4091": ResponseString = "Transaction Count is 0"; break;
                }
                break;
        }

        return ResponseString;
    }



    #endregion
}