using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.Coupon
{
    public class CouponModel
    {
        public string CouponCode { get; set; }
        public decimal MultiBuyDiscountValue { get; set; }
        public bool MultiBuyDiscountIsFlat { get; set; }
    }
}