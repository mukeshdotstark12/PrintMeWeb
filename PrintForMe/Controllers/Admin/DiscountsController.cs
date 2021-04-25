using Castle.Components.DictionaryAdapter.Xml;
using CMS.Ecommerce;
using CMS.SiteProvider;
using PrintForMe.Models;
using PrintForMe.Models.Coupons;
using Syncfusion.EJ2.DropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PrintForMe.Controllers.Admin
{
    public class DiscountsController : Controller
    {
        // GET: Discounts
        public ActionResult Index()
        {
            //List<DiscountInfo> lstFreeShippingDiscount = DiscountInfoProvider.GetDiscounts(SiteContext.CurrentSiteID, true).WhereEquals("DiscountApplyTo", DiscountApplicationEnum.Shipping.ToString()).ToList();
            //List<DiscountInfo> lstOrderDiscount = DiscountInfoProvider.GetDiscounts(SiteContext.CurrentSiteID, true).WhereEquals("DiscountApplyTo", DiscountApplicationEnum.Order.ToString()).ToList();
            List<MultiBuyDiscountInfo> productCoupon = MultiBuyDiscountInfoProvider.GetProductCouponDiscounts(SiteContext.CurrentSiteID).ToList();
            List<CMS.Ecommerce.MultiBuyCouponCodeInfo> productCouponCodes = MultiBuyCouponCodeInfoProvider.GetMultiBuyCouponCodes(SiteContext.CurrentSiteID).ToList();

            List<PrintForMe.Models.Coupons.MultiBuyDiscountModel> discountModel = new List<PrintForMe.Models.Coupons.MultiBuyDiscountModel>();

            if (productCoupon != null)
            {
                foreach (var MultiBuyDiscountInfo in productCoupon)
                {
                    MultiBuyDiscountModel model = new MultiBuyDiscountModel();
                    model.MultiBuyDiscountID = MultiBuyDiscountInfo.MultiBuyDiscountID;
                    model.MultiBuyDiscountName = MultiBuyDiscountInfo.MultiBuyDiscountName;
                    model.MultiBuyDiscountValidFrom = MultiBuyDiscountInfo.MultiBuyDiscountValidFrom;
                    model.MultiBuyDiscountValidTo = MultiBuyDiscountInfo.MultiBuyDiscountValidTo;
                    model.MultiBuyDiscountValue = MultiBuyDiscountInfo.MultiBuyDiscountValue;
                    model.NumberofUse = Convert.ToString(MultiBuyDiscountInfo.MultiBuyDiscountLimitPerOrder);
                    model.ApplyOn = "Products";
                    discountModel.Add(model);
                }
                ViewBag.ProductCouponData = discountModel;
            }

            if (productCouponCodes != null)
            {
                List<PrintForMe.Models.Coupons.MultiBuyCouponCodeModel> productCoup = new List<PrintForMe.Models.Coupons.MultiBuyCouponCodeModel>();
                foreach (var MultiBuyCouponCodeInfo in productCouponCodes){
                    MultiBuyCouponCodeModel model = new MultiBuyCouponCodeModel();
                    model.MultiBuyCouponCodeID = MultiBuyCouponCodeInfo.MultiBuyCouponCodeID;
                    model.MultiBuyCouponCodeCode = MultiBuyCouponCodeInfo.MultiBuyCouponCodeCode;
                    model.MultiBuyCouponCodeUseLimit = MultiBuyCouponCodeInfo.MultiBuyCouponCodeUseLimit;
                    model.MultiBuyCouponCodeUseCount = MultiBuyCouponCodeInfo.MultiBuyCouponCodeUseCount;
                    model.MultiBuyCouponCodeLastModified = MultiBuyCouponCodeInfo.MultiBuyCouponCodeLastModified;
                    if (discountModel != null){
                        var category = discountModel.Where(i => i.MultiBuyDiscountID == MultiBuyCouponCodeInfo.MultiBuyCouponCodeMultiBuyDiscountID).SingleOrDefault();
                        if (category != null){
                            model.MultiBuyCouponCodeFor = category.MultiBuyDiscountName;
                        }
                    }
                    productCoup.Add(model);
                }
                ViewBag.ProductCoupons = productCoup;
                //ViewBag.ShippingData = lstFreeShippingDiscount;
                //ViewBag.OrderData = lstOrderDiscount;
            }
            return View();
        }

        public ActionResult AddProductCoupon()
        {
            CouponModel model = new CouponModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddProductCoupon(CouponModel model)
        {
            switch (model.AppliesTo)
            {
                case "Products":
                    AddUpdateProductCoupon(model);
                    break;
                case "Departments":
                    AddUpdateProductCouponDepartments(model);
                    break;
                case "Brands":
                    AddUpdateProductCouponBrands(model);
                    break;
                case "Collections":
                    AddUpdateProductCouponCollections(model);
                    break;

            }
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }


        public ActionResult EditProductCoupon(int id)
        {
            MultiBuyDiscountInfo modelData = MultiBuyDiscountInfoProvider.GetMultiBuyDiscountInfo(id);

            CouponModel model = new CouponModel
            {
                CouponCode = modelData.MultiBuyDiscountDisplayName,
                TypeOfValue = modelData.MultiBuyDiscountValue,
                Start = modelData.MultiBuyDiscountValidFrom,
                Expried = modelData.MultiBuyDiscountValidTo,
                MultiBuyDiscountID = modelData.MultiBuyDiscountID
            };

            if (modelData.MultiBuyDiscountLimitPerOrder == 1)
            {
                model.OnePerCustomer = true;
            }

            if (modelData.MultiBuyDiscountIsFlat)
            {
                model.TypeOf = "AmountOff";
            }
            else
            {
                model.TypeOf = "Percentage";
            }

            if (modelData.MultiBuyDiscountProducts.Count() > 0)
            {
                model.AppliesTo = "Products";
            }
            if (modelData.MultiBuyDiscountDepartments.Count() > 0)
            {
                model.AppliesTo = "Departments";
            }

            if (modelData.MultiBuyDiscountBrands.Count() > 0)
            {  
                model.AppliesTo = "Brands";
            }

            if (modelData.MultiBuyDiscountCollections.Count() > 0)
            {
                model.AppliesTo = "Collections";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProductCoupon(CouponModel model)
        {
            MultiBuyDiscountInfo modelData = MultiBuyDiscountInfoProvider.GetMultiBuyDiscountInfo(model.MultiBuyDiscountID);

            modelData.MultiBuyDiscountName = model.CouponCode.Replace(" ", string.Empty);
            modelData.MultiBuyDiscountDisplayName = model.CouponCode;
            modelData.MultiBuyDiscountEnabled = true;
            modelData.MultiBuyDiscountSiteID = SiteContext.CurrentSiteID;

            // Configures the object as a Product coupon (not a Buy X Get Y discount)
            modelData.MultiBuyDiscountIsProductCoupon = true;
            modelData.MultiBuyDiscountUsesCoupons = true;
            modelData.MultiBuyDiscountApplyFurtherDiscounts = true;
            modelData.MultiBuyDiscountAutoAddEnabled = false;
            // Sets the coupon's discount value to a fixed amount of 10
            modelData.MultiBuyDiscountIsFlat = false;
            modelData.MultiBuyDiscountValue = model.TypeOfValue;
            modelData.MultiBuyDiscountMinimumBuyCount = 1;
            modelData.MultiBuyDiscountValidFrom = model.Start;
            modelData.MultiBuyDiscountValidTo = model.Expried;
            // Makes the coupon available for all store visitors
            modelData.MultiBuyDiscountCustomerRestriction = DiscountCustomerEnum.RegisteredUsers;

            if (model.OnePerCustomer.HasValue && model.OnePerCustomer.Value)
                modelData.MultiBuyDiscountLimitPerOrder = 1;
            if (model.TypeOf == "AmountOff")
                modelData.MultiBuyDiscountIsFlat = true;
            MultiBuyDiscountInfoProvider.SetMultiBuyDiscountInfo(modelData);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCoupon()
        {
            CouponModel model = new CouponModel();
            return View(model);

        }

        [HttpPost]
        public ActionResult AddCoupon(CouponModel model)
        {
            CreateDiscount(model);

            return View(model);
        }

        public ActionResult AddCouponCode()
        {
            AddCouponCodeModel Model = new AddCouponCodeModel();
            return View(Model);
        }
        [HttpPost]
        public ActionResult AddCouponCode(AddCouponCodeModel model)
        {
            MultiBuyDiscountInfo productCoupon = MultiBuyDiscountInfoProvider.GetProductCouponDiscounts(SiteContext.CurrentSiteID)
                                                            .WhereContains("MultiBuyDiscountName", model.MultiBuyCouponName)
                                                             .TopN(1)
                                                            .FirstOrDefault();

            if (productCoupon != null)
            {
                // Prepares a query that gets all existing coupon codes from the current site
                var existingCouponCodeQuery = ECommerceHelper.GetAllCouponCodesQuery(SiteContext.CurrentSiteID);

                // Creates a cache of coupon codes on the current site
                var existingCodes = existingCouponCodeQuery.GetListResult<string>();

                // Prepares an instance of a class that checks against existing coupon codes to avoid duplicates
                var coudeUniquenessChecker = new CodeUniquenessChecker(existingCodes);

                // Initializes a coupon code generator

                if (coudeUniquenessChecker.IsUnique(model.MultiBuyCouponCodeName))
                {
                    for (int i = 0; i < 1; i++)
                    {
                        MultiBuyCouponCodeInfoProvider.CreateCoupon(productCoupon, model.MultiBuyCouponCodeName, model.MultiBuyCouponCodeUseLimit);
                    }

                }
            }
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteDiscount(int id)
        {
            DiscountInfo discount = DiscountInfoProvider.GetDiscountInfo(id);
            DiscountInfoProvider.DeleteDiscountInfo(discount);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProductDiscount(int id)
        {
            MultiBuyDiscountInfo modelData = MultiBuyDiscountInfoProvider.GetMultiBuyDiscountInfo(id);
            MultiBuyDiscountInfoProvider.DeleteMultiBuyDiscountInfo(modelData);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCouponCode(int id)
        {
            MultiBuyCouponCodeInfo couponCode = MultiBuyCouponCodeInfoProvider.GetMultiBuyCouponCodeInfo(id);
            MultiBuyCouponCodeInfoProvider.DeleteMultiBuyCouponCodeInfo(couponCode);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditCouponCode(int id)
        {
            MultiBuyCouponCodeInfo couponCode = MultiBuyCouponCodeInfoProvider.GetMultiBuyCouponCodeInfo(id);
            CouponCodeModel model = new CouponCodeModel
            {
                MultiBuyCouponCodeUseLimit = couponCode.MultiBuyCouponCodeUseLimit,
                MultiBuyCouponCodeName = couponCode.MultiBuyCouponCodeCode,
                MultiBuyCouponCodeID = couponCode.MultiBuyCouponCodeID
            };
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCouponCode(CouponCodeModel couponCode)
        {
            MultiBuyCouponCodeInfo cCode = MultiBuyCouponCodeInfoProvider.GetMultiBuyCouponCodeInfo(couponCode.MultiBuyCouponCodeID);
            cCode.MultiBuyCouponCodeUseLimit = couponCode.MultiBuyCouponCodeUseLimit;
            MultiBuyCouponCodeInfoProvider.SetMultiBuyCouponCodeInfo(cCode);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EditCoupon(int id)
        {
            DiscountInfo discount = DiscountInfoProvider.GetDiscountInfo(id);
            CouponModel model = new CouponModel
            {
                CouponCode = discount.DiscountDisplayName,
                TypeOfValue = discount.DiscountValue,
                Start = discount.DiscountValidFrom,
                Expried = discount.DiscountValidTo,
                DiscountID = discount.DiscountID
            };
            if (discount.DiscountApplyTo == DiscountApplicationEnum.Shipping)

            {
                model.TypeOf = "Shipping";
            }
            else
            {
                model.TypeOf = "Order";
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult EditCoupon(CouponModel model)
        {
            DiscountInfo discount = DiscountInfoProvider.GetDiscountInfo(model.DiscountID);
            discount.DiscountName = model.CouponCode.Replace(" ", string.Empty);
            discount.DiscountDisplayName = model.CouponCode;
            discount.DiscountIsFlat = false;
            discount.DiscountValue = model.TypeOfValue;
            discount.DiscountOrderAmount = 100;
            discount.DiscountEnabled = true;
            discount.DiscountOrder = 1;
            discount.DiscountApplyFurtherDiscounts = true;
            discount.DiscountUsesCoupons = false;
            discount.DiscountSiteID = SiteContext.CurrentSiteID;
            discount.DiscountValidFrom = model.Start;
            discount.DiscountValidTo = model.Expried;

            if (model.TypeOf == "Shipping")
                discount.DiscountApplyTo = DiscountApplicationEnum.Shipping;
            else
                discount.DiscountApplyTo = DiscountApplicationEnum.Order;
            // Saves the free shipping offer to the database
            DiscountInfoProvider.SetDiscountInfo(discount);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/discounts");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public JsonResult GetObject(string objectType)
        {
            JavaScriptSerializer convert = new JavaScriptSerializer();

            switch (objectType)
            {
                case "Products":
                    var pData = SKUInfoProvider.GetSKUs().Select(x => new JsonData()
                    {
                        ID = x.SKUID,
                        Name = x.SKUName
                    }).ToList();

                    return Json(convert.Serialize(pData), JsonRequestBehavior.AllowGet);
                case "Departments":
                    var dData = DepartmentInfoProvider.GetDepartments().Select(x => new JsonData()
                    {
                        ID = x.DepartmentID,
                        Name = x.DepartmentName
                    }).ToList();
                    return Json(convert.Serialize(dData), JsonRequestBehavior.AllowGet);
                case "Brands":
                    var bData = BrandInfoProvider.GetBrands().Select(x => new JsonData()
                    {
                        ID = x.BrandID,
                        Name = x.BrandName
                    }).ToList();
                    return Json(convert.Serialize(bData), JsonRequestBehavior.AllowGet);
                case "Collections":
                    var cData = CollectionInfoProvider.GetCollections().Select(x => new JsonData()
                    {
                        ID = x.CollectionID,
                        Name = x.CollectionName
                    }).ToList();
                    return Json(convert.Serialize(cData), JsonRequestBehavior.AllowGet);
                default:
                    // code block
                    break;
            }

            return Json(new { Results = new List<JsonData>(), JsonRequestBehavior.AllowGet });

        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="model"></param>
        public void AddUpdateProductCoupon(CouponModel model)
        {
            List<SKUInfo> lstSkuInfo = new List<SKUInfo>();
            lstSkuInfo = SKUInfoProvider.GetSKUs().Where("SKUID IN(" + model.SelectedCategory.Trim(',') + ")").ToList();

            MultiBuyDiscountInfo newProductCoupon = CreateMultiBuyCoupon(model);
            foreach (SKUInfo obj in lstSkuInfo)
            {
                // Configures the coupon to apply to the retrieved product
                MultiBuyDiscountSKUInfo couponSkuBinding = new MultiBuyDiscountSKUInfo
                {
                    MultiBuyDiscountID = newProductCoupon.MultiBuyDiscountID,
                    SKUID = obj.SKUID
                };

                // Saves the coupon-product relationship to the database
                MultiBuyDiscountSKUInfoProvider.SetMultiBuyDiscountSKUInfo(couponSkuBinding);
            }

        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="model"></param>
        public void AddUpdateProductCouponDepartments(CouponModel model)
        {

            MultiBuyDiscountInfo newProductCoupon = CreateMultiBuyCoupon(model);
            // Gets all departments on the current site
            var departments = DepartmentInfoProvider.GetDepartments(SiteContext.CurrentSiteID).Where("DepartmentID IN(" + model.SelectedCategory.Trim(',') + ")").ToList();

            // Loops through the departments
            foreach (DepartmentInfo department in departments)
            {
                // Creates a relationship between the product coupon and department
                MultiBuyDiscountDepartmentInfoProvider.AddMultiBuyDiscountToDepartment(newProductCoupon.MultiBuyDiscountID, department.DepartmentID);
            }
            // Clears the product coupon's cache record
            CMS.Helpers.CacheHelper.TouchKey(MultiBuyDiscountInfo.OBJECT_TYPE_PRODUCT_COUPON + "|byid|" + newProductCoupon.MultiBuyDiscountID);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="model"></param>
        public void AddUpdateProductCouponBrands(CouponModel model)
        {

            MultiBuyDiscountInfo newProductCoupon = CreateMultiBuyCoupon(model);
            // Gets all brands on the current site
            var brands = BrandInfoProvider.GetBrands().OnSite(SiteContext.CurrentSiteID).Where("BrandID IN(" + model.SelectedCategory.Trim(',') + ")").ToList();

            // Loops through the brands
            foreach (BrandInfo brand in brands)
            {
                // Creates a relationship between the product coupon and brand
                // Tip: To add excluded brands to the product coupon, set the method's 'included' parameter to false
                MultiBuyDiscountBrandInfoProvider.AddMultiBuyDiscountToBrand(newProductCoupon.MultiBuyDiscountID, brand.BrandID, included: true);
            }

            // Clears the product coupon's cache record
            CMS.Helpers.CacheHelper.TouchKey(MultiBuyDiscountInfo.OBJECT_TYPE_PRODUCT_COUPON + "|byid|" + newProductCoupon.MultiBuyDiscountID);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="model"></param>
        public void AddUpdateProductCouponCollections(CouponModel model)
        {
            MultiBuyDiscountInfo newProductCoupon = CreateMultiBuyCoupon(model);

            // Gets all collections on the current site
            var collections = CollectionInfoProvider.GetCollections().OnSite(SiteContext.CurrentSiteID).Where("CollectionID IN(" + model.SelectedCategory.Trim(',') + ")").ToList(); ;

            // Loops through the collections
            foreach (CollectionInfo collection in collections)
            {
                // Creates a relationship between the product coupon and collection
                // Tip: To add excluded collections to the product coupon, set the method's 'included' parameter to false
                MultiBuyDiscountCollectionInfoProvider.AddMultiBuyDiscountToCollection(newProductCoupon.MultiBuyDiscountID, collection.CollectionID, included: true);
            }

            // Clears the product coupon's cache record
            CMS.Helpers.CacheHelper.TouchKey(MultiBuyDiscountInfo.OBJECT_TYPE_PRODUCT_COUPON + "|byid|" + newProductCoupon.MultiBuyDiscountID);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private MultiBuyDiscountInfo CreateMultiBuyCoupon(CouponModel model)
        {
            MultiBuyDiscountInfo newProductCoupon = new MultiBuyDiscountInfo()
            {
                MultiBuyDiscountName = model.CouponCode.Replace(" ", string.Empty),
                MultiBuyDiscountDisplayName = model.CouponCode,
                MultiBuyDiscountEnabled = true,
                MultiBuyDiscountSiteID = SiteContext.CurrentSiteID,

                // Configures the object as a Product coupon (not a Buy X Get Y discount)
                MultiBuyDiscountIsProductCoupon = true,
                MultiBuyDiscountUsesCoupons = true,
                MultiBuyDiscountApplyFurtherDiscounts = true,                                                                                                                       
                MultiBuyDiscountAutoAddEnabled = false,
                // Sets the coupon's discount value to a fixed amount of 10
                MultiBuyDiscountIsFlat = false,
                MultiBuyDiscountValue = model.TypeOfValue,
                MultiBuyDiscountMinimumBuyCount = 1,
                MultiBuyDiscountValidFrom = model.Start,
                MultiBuyDiscountValidTo = model.Expried,
                // Makes the coupon available for all store visitors
                MultiBuyDiscountCustomerRestriction = DiscountCustomerEnum.RegisteredUsers,
            };
            if (model.OnePerCustomer.HasValue && model.OnePerCustomer.Value)
                newProductCoupon.MultiBuyDiscountLimitPerOrder = 1;
            if (model.TypeOf == "AmountOff")
                newProductCoupon.MultiBuyDiscountIsFlat = true;

            // Saves the product coupon to the database
            MultiBuyDiscountInfoProvider.SetMultiBuyDiscountInfo(newProductCoupon);
            return newProductCoupon;
        }

        public void CreateDiscount(CouponModel model)
        {
            // Creates a new free shipping offer object and sets its properties
            DiscountInfo newDiscount = new DiscountInfo()
            {
                DiscountName = model.CouponCode.Replace(" ", string.Empty),
                DiscountDisplayName = model.CouponCode,
                DiscountIsFlat = false,
                DiscountValue = model.TypeOfValue,
                DiscountOrderAmount = 100,
                DiscountEnabled = true,
                DiscountOrder = 1,
                DiscountApplyFurtherDiscounts = true,
                DiscountUsesCoupons = false,
                DiscountSiteID = SiteContext.CurrentSiteID,
                DiscountValidFrom = model.Start,
                DiscountValidTo = model.Expried
            };
            if (model.TypeOf == "Shipping")
                newDiscount.DiscountApplyTo = DiscountApplicationEnum.Shipping;
            else
                newDiscount.DiscountApplyTo = DiscountApplicationEnum.Order;
            // Saves the free shipping offer to the database
            DiscountInfoProvider.SetDiscountInfo(newDiscount);
        }
    }
}