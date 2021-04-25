using CMS.Ecommerce;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrintForMe.Models.Coupons
{
    public class AddCouponCodeModel
    {
            [Required]
            public int MultiBuyCouponCodeUseLimit { get; set; }

            [Required]
            public string MultiBuyCouponCodeName { get; set; }

            public string MultiBuyCouponName { get; set; }

            public List<DataObject> CouponCodeFor
            {
                get{
                    List<MultiBuyDiscountInfo> productCoupon = MultiBuyDiscountInfoProvider.GetProductCouponDiscounts(SiteContext.CurrentSiteID).ToList();
                    List<DataObject> LstCouponCodeFor = new List<DataObject>();
                    for (int i = 0; i < productCoupon.Count(); i++)
                    {
                        LstCouponCodeFor.Add(new DataObject() { Code = productCoupon[i].MultiBuyDiscountName, Text = productCoupon[i].MultiBuyDiscountDisplayName });
                    }
                    return LstCouponCodeFor;
                }
            }
            public AddCouponCodeModel()
            {}
    }
}