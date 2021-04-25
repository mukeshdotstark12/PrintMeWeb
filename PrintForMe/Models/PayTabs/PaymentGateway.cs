using CMS.EventLog;
using CMS.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using static PrintForMe.Models.PayTabs.MakePaymentModel;

namespace PrintForMe.Models.PayTabs
{
    public class PaymentGateway
    {
        public PaymentGateway()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string MakeWebServiceCall(string methodCall, string formContent)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string ApiURL = Constants.PaymentGatewayAPRURL;
            //string ApiURL = ConfigurationManager.AppSettings["PaymentGatewayAPRURL"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiURL);
            request.Method = "POST";
            request.Headers.Add("Authorization", Constants.PayTabsServerKey);

            byte[] byteArray;
            WebResponse response;
            StreamReader reader;
            Stream dataStream;
            string responseFromServer;

            byteArray = Encoding.UTF8.GetBytes(formContent.Replace("_return", "return"));
            request.ContentType = "application/json";
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

        public string GenerateReferenceNumber()
        {
            return "PT" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }

        public const string ConstCreatePayPage = "create_pay_page";
        public const string ConstValidateKey = "validate_secret_key";
        public const string ConstRefundProcess = "refund_process";
        public const string ConstTransactionReports = "transaction_reports";
        public const string ConstGenerateIPN = "Generate_IPN";
        public const string ConstVerifyPayment = "verify_payment";

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

        public string CreatePayPage(MakePaymentModel.PayPageRequest objPayPageRequest)
        {
            decimal otherCharges = objPayPageRequest.Tax + objPayPageRequest.TotalShipping;
            decimal amount = objPayPageRequest.TotalPrice + otherCharges;//(unit_price * quantity)) + other_charges. 
            decimal discount = amount - objPayPageRequest.GrandTotal;

            return "profile_id=" + objPayPageRequest.profile_id
                                + "&tran_type=" + objPayPageRequest.tran_type
                                + "&tran_class=" + objPayPageRequest.tran_class
                                + "&cart_id=" + objPayPageRequest.cart_id
                                + "&cart_currency=" + objPayPageRequest.cart_currency
                                + "&cart_amount=" + amount
                                + "&cart_description=" + objPayPageRequest.cart_description
                                + "&paypage_lang=" + objPayPageRequest.paypage_lang
                                + "&customer_details=" + objPayPageRequest.ProductsPerTitle
                                + "&return_url=" + Constants.PayTabsReturnUrl;
        }

        public string ValidateSecretKey(MakePaymentModel.VerifySecretKeyRequest objRequest)
        {
            return "merchant_email=" + objRequest.MerchantEmail
                   + "&secret_key=" + objRequest.SecretKey;
        }

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

        public enum PayTabRequestType
        {
            ValidateSecretKey = 1,
            CreatePayPage,
            VerifyPayment,
            RefundTransactions,
            PayTabsIPIN,
            TransactionsReports
        }

        public MakePaymentModel.PayPageResponse MakePayment(MakePaymentModel.PayPageRequest data)
        {
            var objrequest = new MakePaymentModel.PayPageRequest();
            var tmp = new MakePaymentModel.PayPageResponse();
            decimal otherCharges = data.Tax + data.TotalShipping;
            decimal amount = data.TotalPrice + otherCharges;//(unit_price * quantity)) + other_charges. 
            decimal discount = amount - data.GrandTotal;
            try
            {
                var paymentUtility = new PaymentGateway();
                objrequest = new MakePaymentModel.PayPageRequest
                {
                    profile_id = Constants.PayTabsProfileId,
                    tran_type = "sale",
                    tran_class = "ecom",
                    cart_id = data.OrderID,
                    cart_currency = "SAR",
                    cart_amount = amount,
                    cart_description = "Description of the items/services",
                    paypage_lang = "en",
                    customer_details = new Customer_details
                    {
                        name = data.CcFirstName + " " + data.CcLastName,
                        email = data.Email,
                        street1 = data.AddressShipping,
                        city = data.City,
                        state = data.State,
                        country = data.Country,
                        zip = data.PostalCode
                    },
                    shipping_details = new Shipping_details
                    {
                        name = data.CcFirstName + " " + data.CcLastName,
                        email = data.Email,
                        street1 = data.AddressShipping,
                        city = data.City,
                        state = data.State,
                        country = data.Country,
                        zip = data.PostalCode
                    },
                    callback = Constants.PayTabsCallBack,
                    _return = string.Format(Constants.PayTabsReturnUrl, data.OrderID)                   
                };

                string serviceResponse = paymentUtility.MakeWebServiceCall(PaymentGateway.ConstCreatePayPage, JsonConvert.SerializeObject(objrequest));
                tmp = JsonConvert.DeserializeObject<MakePaymentModel.PayPageResponse>(serviceResponse);

                return tmp;
            }
            catch (Exception ex)
            {
                // Logs an information event into the event log
                EventLogProvider.LogEvent(EventType.INFORMATION, "Payment Response", "MakePayment", eventDescription: ex.Message);
                return null;
            }
        }
        private string GetIPAddress()
        {
            string ipaddress = RequestContext.UserHostAddress;
            return ipaddress;
        }
        public MakePaymentModel.VerifySecretKeyResponse ValidateKey()
        {
            var objRequest = new MakePaymentModel.VerifySecretKeyRequest()
            {
                MerchantEmail = Constants.PayTapsMerchantEmail,
                SecretKey = Constants.PayTapsSecretKey,
            };

            var paymentUtility = new PaymentGateway();

            string serviceResponse = "";
            var tmp = new MakePaymentModel.VerifySecretKeyResponse();

            try
            {
                serviceResponse = paymentUtility.MakeWebServiceCall(PaymentGateway.ConstValidateKey, paymentUtility.ValidateSecretKey(objRequest));

                tmp = JsonConvert.DeserializeObject<MakePaymentModel.VerifySecretKeyResponse>(serviceResponse);

                return tmp;
            }
            catch (Exception ex)
            {
                // Logs an information event into the event log
                EventLogProvider.LogEvent(EventType.INFORMATION, "Validate Key", "ValidateKey", eventDescription: ex.Message);
                return null;
            }

        }
    }
}
