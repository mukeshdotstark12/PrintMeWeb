using System.Collections.Generic;

/// <summary>
/// Summary description for Helper
/// </summary>
public class Helper : System.Web.UI.Page
{
    public static class PayTabsSession
    {
        #region "Variables"

        /// <summary>
        /// 
        /// </summary>
        public static string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string SecretKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static bool InvalidSecretKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string EmailAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string SiteUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string LastPaymentReferenceNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string CurrentActivePaymentID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static List<Models.PayPageRequest> PageRequestList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string ReportSearchResult { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string LastTransactionJson { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ss"></param>
    public static void SetSession(string ss)
    {
        PayTabsSession.ReportSearchResult = ss;
    }
}