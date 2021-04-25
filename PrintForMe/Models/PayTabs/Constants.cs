namespace PrintForMe.Models
{
    static class Constants
    {
        public const string ECSHash = "D9D045D9C7711D72EFAF75407C8EAA9ACC5DC70F9BE0087099131B7ED22CBB9D";
        public const string ViolettaAdminUrl = "http://localhost:51872/";

        // PayTaps information
        public const string PayTapsMerchantEmail = "sunil@ltechpro.com";
        public const string PayTapsSecretKey = "snx28qbhsfIlDq8azQ0dzRnBTSct84BAr9DAzOS24AoH1xRAnSOWJcxC4K6cOINgRqQrfJtkkS2lpUWNMKJvWh812feRX17PsYgI";
        public const string PayTabsSiteUrl = "http://adminprintme.ltechpro.com";
        public const string PayTabsCallBack = "http://adminprintme.ltechpro.com/api/paytabs/VerifyPayTabsPayment";
        public const string PaymentGatewayAPRURL = "https://secure.paytabs.sa/payment/request";//"https://www.paytabs.com/apiv2/";
        public const string PayTabsReturnUrl = "http://printme.ltechpro.com/en-us/payment/VerifyPayment?orderID={0}";        


        public const string PayTabsServerKey = "SHJNN92ZBJ-JBHRKNR2ZD-LHJKLJM6WK";
        public const string PayTabsClientKey = "CDKMMT-PRN662-HRKQRM-BNHT6D";
        public const string PayTabsProfileId = "63562";


        //public static string ECSHash = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".ECSHash");
        //public static string ViolettaAdminUrl = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".ViolettaAdminUrl");
        //public static string PayTapsMerchantEmail = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".PayTabsMerchantEmail");
        //public static string PayTapsSecretKey = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".PayTabsSecretKey");
        //public static string PayTabsSiteUrl = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".PayTabsSiteUrl");
        //public static string PayTabsReturnUrl = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".PayTabsReturnUrl");
        //public static string PaymentGatewayAPRURL = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".PaymentGatewayAppURL");
    }
}
