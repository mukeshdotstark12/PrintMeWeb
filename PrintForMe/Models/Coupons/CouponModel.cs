using CMS.Ecommerce;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrintForMe.Models
{
    public class CouponModel
    {
        /// <summary>
        /// /
        /// </summary>
        public int MultiBuyDiscountID { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public int DiscountID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Discount Name is required")]
        public string CouponCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DiscountCoupon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Type is required")]
        public string TypeOf { get; set; }


        /// <summary>
        /// /
        /// </summary>
        [Required(ErrorMessage = "Please enter a vaild value.")]
        public decimal TypeOfValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? OnePerCustomer { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public bool? NeverExpires { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Select coupon category.")]
        public string AppliesTo { get; set; }

        /// <summary>
        /// /
        /// </summary>
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime Start { get; set; } = DateTime.Now;

        /// <summary>
        /// /
        /// </summary>
        public DateTime Expried { get; set; } = DateTime.Now;

        /// <summary>
        /// /
        /// </summary>
        public List<SKUInfo> Products
        {
            get
            {
                return SKUInfoProvider.GetSKUs().ToList();
            }
        }


        public List<DataObject> Applies
        {
            get
            {
                List<DataObject> lstApplies = new List<DataObject>
                {
                   //new DataObject() { Code = "Products", Text = "Products" },
                    new DataObject() { Code = "Departments", Text = "Products in Departments" },
                    //new DataObject() { Code = "Brands", Text = "Products with Brand" },
                    //new DataObject() { Code = "Collections", Text = "Products in Collections" }
                };

                return lstApplies;
            }
        }


        public List<DataObject> Types
        {
            get
            {
                List<DataObject> lstApplies = new List<DataObject>
                {
                    new DataObject() { Code = "AmountOff", Text = "Amount Off" },
                    new DataObject() { Code = "Percentage", Text = "Percentage Amount" },
                };

                return lstApplies;
            }
        }

        public List<DataObject> CouponTypes
        {
            get
            {
                List<DataObject> lstApplies = new List<DataObject>
                {
                    new DataObject() { Code = "Order", Text = "Order Coupon" },
                    new DataObject() { Code = "Shipping", Text = "Shipping Coupon"},
                };

                return lstApplies;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string SelectedCategory { get; set; }


        public Decimal DiscountOrderAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CollectionInfo> Collections
        {
            get
            {
                return CollectionInfoProvider.GetCollections().ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SelectedCollection { get; set; }

        /// <summary>
        /// //
        /// </summary>
        public List<DepartmentInfo> Departments
        {
            get
            {
                return DepartmentInfoProvider.GetDepartments().ToList();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<BrandInfo> Brands
        {
            get
            {
                return BrandInfoProvider.GetBrands().ToList();
            }
        }


    }

    public class DataObject
    {
        public string Code { get; set; }

        public string Text { get; set; }
    }
}