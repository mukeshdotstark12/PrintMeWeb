using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrintForMe.Models
{
    public class CouponCodeModel
    {
        [Required]
        public int MultiBuyCouponCodeUseLimit { get; set; }

        [Required]
        public string MultiBuyCouponCodeName { get; set; }
        
        public int MultiBuyCouponCodeID { get; set; }
    }
}