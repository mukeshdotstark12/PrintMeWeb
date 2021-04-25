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
    public class MultiBuyDiscountModel
    {
        [JsonProperty("MultiBuyDiscountID")]
        public int MultiBuyDiscountID { get; set; }

        [JsonProperty("MultiBuyDiscountName")]
        public string MultiBuyDiscountName { get; set; }

        [JsonProperty("ApplyOn")]
        public string ApplyOn { get; set; }

        [JsonProperty("MultiBuyDiscountValidFrom")]
        public DateTime MultiBuyDiscountValidFrom { get; set; }

        [JsonProperty("MultiBuyDiscountValidTo")]
        public DateTime MultiBuyDiscountValidTo { get; set; }

        [JsonProperty("MultiBuyDiscountValue")]
        public Decimal MultiBuyDiscountValue { get; set; }

        [JsonProperty("NumberofUse")]
        public string NumberofUse { get; set; }

        public MultiBuyDiscountModel()     
        {}
    }
}