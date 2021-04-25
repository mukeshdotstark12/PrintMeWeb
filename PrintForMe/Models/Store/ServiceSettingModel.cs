using Newtonsoft.Json;

namespace PrintForMe.Models.Store
{
    public class ServiceSettingModel
    {
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Price")]
        public string Price { get; set; }
        [JsonProperty("Availability")]
        public bool Availability { get; set; }
        [JsonProperty("ServiceID")]
        public int ServiceID { get; set; }
        [JsonProperty("ItemID")]
        public int ItemID { get; set; }
        [JsonProperty("Height")]
        public int Height { get; set; }
        [JsonProperty("Width")]
        public int Width { get; set; }
        public double ProductPrice { get; set; }
    }
}