using Newtonsoft.Json;
using System;

namespace PrintForMe.Models.OrderManagement
{
    public class OrderListSettingModel
    {
        [JsonProperty("OrderID")]
        public int OrderID { get; set; }
        [JsonProperty("OrderDate")]
        public DateTime OrderDate { get; set; }
        [JsonProperty("CustomerName")]
        public string CustomerName { get; set; }
        [JsonProperty("CustomerMobile")]

        public string CustomerMobile { get; set; }
        [JsonProperty("PaymentOption")]
        public string PaymentOption { get; set; }
        [JsonProperty("FormattedTotalPrice")]
        public string FormattedTotalPrice { get; set; }

        [JsonProperty("StatusName")]
        public string statusName { get; set; }

        [JsonProperty("StatusID")]
        //public string statusID { get; set; }
        public int StatusID { get; set; }
    }
}