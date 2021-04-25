using CMS.Ecommerce;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PrintForMe.Models.Coupons
{
    public class MultiBuyCouponCodeModel
    {
        [JsonProperty("MultiBuyCouponCodeID")]
        public int MultiBuyCouponCodeID { get; set; }
        
        [JsonProperty("MultiBuyCouponCodeUseLimit")]
        public int MultiBuyCouponCodeUseLimit { get; set; }

        [JsonProperty("MultiBuyCouponCodeCode")]
        public string MultiBuyCouponCodeCode { get; set; }

        [JsonProperty("MultiBuyCouponCodeUseCount")]
        public int MultiBuyCouponCodeUseCount { get; set; }

        [JsonProperty("MultiBuyCouponCodeFor")]
        public string MultiBuyCouponCodeFor { get; set; }

        [JsonProperty("MultiBuyCouponCodeLastModified")]
        public DateTime MultiBuyCouponCodeLastModified { get; set; }
        public MultiBuyCouponCodeModel()        
        {}
    }
}