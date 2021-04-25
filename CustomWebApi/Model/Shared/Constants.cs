using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Shared
{
    static class Constants
    {
        public const string PrintForMeHash = "45fb0e3be369a90e97d077e917a5e9badc7a81c9c10bc7de681cc70fb3cffdc7";/*"D9D045D9C7711D72EFAF75407C8EAA9ACC5DC70F9BE0087099131B7ED22CBB9D";*/
        public const string PrintForMeAdminUrl = "http://adminprintme.ltechpro.com/";
        public const string PrintForMeUrl = "http://printme.ltechpro.com/"; //"http://localhost:2537/";
        public const string PrintForMeDefaultSMTP = "Testdev@ltechpro.com";

        // PayTaps information
        //public const string PayTapsMerchantEmail = "kamal@ltechpro.com";
        //public const string PayTapsSecretKey = "5jWBSdJZQCy42QxDXc5ok0Jdf1gWF9BLtymlM6lCITHLJfoGru5MvPZLQfHwpjhSREdEKzgssYdcdCTF7ZsDOroFH6b6ptZh7cVR";
        //public const string PayTabsSiteUrl = "-http://localhost:44397/";
        //public const string PayTabsReturnUrl = "-http://adminprintme.ltechpro.com/ar-SA/Payment/Receipt";
        //public const string PaymentGatewayAPRURL = "-https://www.paytabs.com/apiv2/";


        public const string PayTapsMerchantEmail = "sunil@ltechpro.com";
        public const string PayTapsSecretKey = "snx28qbhsfIlDq8azQ0dzRnBTSct84BAr9DAzOS24AoH1xRAnSOWJcxC4K6cOINgRqQrfJtkkS2lpUWNMKJvWh812feRX17PsYgI";
        public const string PayTabsSiteUrl = "http://adminprintme.ltechpro.com";
        public const string PayTabsCallBack = "http://adminprintme.ltechpro.com/api/paytabs/VerifyPayTabsPayment";
        public const string PaymentGatewayAPRURL = "https://secure.paytabs.sa/payment/request";//"https://www.paytabs.com/apiv2/";
        public const string PayTabsReturnUrl = "http://printme.ltechpro.com/en-us/payment/VerifyPayment?orderID={0}";


        public const string PayTabsServerKey = "SHJNN92ZBJ-JBHRKNR2ZD-LHJKLJM6WK";
        public const string PayTabsClientKey = "CDKMMT-PRN662-HRKQRM-BNHT6D";
        public const string PayTabsProfileId = "63562";
    }
}
