using CMS.Ecommerce;
using CMS.SiteProvider;
using CustomWebApi.Model.Shared;
using CustomWebApi.Models.PayTabs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CustomWebApi
{
    public class PayTabsController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> VerifyPayTabsPayment()
        {

            dynamic obj = await Request.Content.ReadAsAsync<JObject>();

            if (obj == null)
                return BadRequest("Null Response");

            string paymentReference = obj.payment_reference;
            var objUtility = new Utility();
            var objVerifyPaymentRequest = new MakePaymentModel.VerifyPaymentRequest();

            try
            {
                objVerifyPaymentRequest.MerchantEmail = Constants.PayTapsMerchantEmail;
                objVerifyPaymentRequest.ReferenceNumber = paymentReference;
                objVerifyPaymentRequest.SecretKey = Constants.PayTapsSecretKey;

                var objResponse = objUtility.VerifyPayment(objVerifyPaymentRequest);

                if (objResponse != null)
                {
                    if (objResponse.response_code == "100")
                    {
                        HttpContext.Current.Response.Redirect($"{Constants.PayTabsSiteUrl}/ar-SA/Payment/Success");
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect($"{Constants.PayTabsSiteUrl}/ar-SA/Payment/Failed");
                    }
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
