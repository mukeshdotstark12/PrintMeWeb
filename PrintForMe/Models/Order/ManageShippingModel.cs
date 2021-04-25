using CMS.Ecommerce;
using System.Collections.Generic;
using System.Linq;

namespace PrintForMe.Models
{
    public class ManageShippingModel
    {

        public int ShippingCostID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ShippingOptionID { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public string ShippingOptionName { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public double ShippingCostMinWeight { get; set; }


        /// <summary>
        /// /
        /// </summary>
        public decimal ShippingCostValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ShippingCostInfo> ShippingCosts
        {
            get
            {
                return ShippingCostInfoProvider.GetShippingCosts().WhereEquals("ShippingCostShippingOptionID", ShippingOptionID).ToList();

            }

        }

        /// <summary>
        /// /
        /// </summary>
        public List<ShippingOptionInfo> ShippingOption
        {
            get
            {
                return ShippingOptionInfoProvider.GetShippingOptions().ToList();

            }

        }
    }
}