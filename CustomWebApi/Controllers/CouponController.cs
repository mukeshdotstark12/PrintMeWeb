using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CustomWebApi.Model.Shared;
using CustomWebApi.Models.Coupon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CustomWebApi.Controllers
{
    public class CouponController : ApiController
    {        
        #region GET 
        [HttpGet]
        public IHttpActionResult GetCouponsCode()
        {

            var couponsWithValues = MultiBuyCouponCodeInfoProvider.GetMultiBuyCouponCodes()
                .Source(c => c.Join<MultiBuyDiscountInfo>("MultiBuyCouponCodeMultiBuyDiscountID", "MultiBuyDiscountID"))
                .Columns("MultiBuyCouponCodeCode", "MultiBuyDiscountValue").Result;

            CouponModel model = new CouponModel();

            return Json("");
        }

        [HttpGet]
        public HttpResponseMessage GetCouponsInfo()
        {
            try
            {
                var couponsWithValues = MultiBuyCouponCodeInfoProvider.GetMultiBuyCouponCodes()
                .Source(c => c.Join<MultiBuyDiscountInfo>("MultiBuyCouponCodeMultiBuyDiscountID", "MultiBuyDiscountID"))
                .Columns("MultiBuyCouponCodeCode", "MultiBuyDiscountValue", "MultiBuyDiscountIsFlat");

                if (couponsWithValues != null)
                {
                    List<CouponModel> couponArray = new List<CouponModel>();

                    foreach (DataTable table in couponsWithValues.Tables)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            CouponModel couponDetails = new CouponModel
                            {
                                MultiBuyDiscountValue = ValidationHelper.GetDecimal(dr["MultiBuyDiscountValue"], 0),
                                CouponCode = ValidationHelper.GetString(dr["MultiBuyCouponCodeCode"], ""),
                                MultiBuyDiscountIsFlat = ValidationHelper.GetBoolean(dr["MultiBuyCouponCodeCode"], false)
                            };
                            couponArray.Add(couponDetails);
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = couponArray,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "Coupon not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Try again!"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCouponInfo(string coupon)
        {
            try
            {
                var couponsWithValues = MultiBuyCouponCodeInfoProvider.GetMultiBuyCouponCodes()
                .Source(c => c.Join<MultiBuyDiscountInfo>("MultiBuyCouponCodeMultiBuyDiscountID", "MultiBuyDiscountID"))
                .Columns("MultiBuyCouponCodeCode", "MultiBuyDiscountValue", "MultiBuyDiscountIsFlat")
                .WhereEquals("MultiBuyCouponCodeCode", coupon)
                .Result;

                if (!DataHelper.DataSourceIsEmpty(couponsWithValues))
                {
                    List<CouponModel> couponArray = new List<CouponModel>();

                    foreach (DataTable table in couponsWithValues.Tables)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            CouponModel couponDetails = new CouponModel
                            {
                                MultiBuyDiscountValue = ValidationHelper.GetDecimal(dr["MultiBuyDiscountValue"], 0),
                                CouponCode = ValidationHelper.GetString(dr["MultiBuyCouponCodeCode"], ""),
                                MultiBuyDiscountIsFlat = ValidationHelper.GetBoolean(dr["MultiBuyCouponCodeCode"], false)
                            };
                            couponArray.Add(couponDetails);
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = couponArray,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "Coupon not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Try again!"
                });
            }
        }

        #endregion

        #region POST 
       
        [HttpPost]
        public HttpResponseMessage AddCouponToCart(CouponData couponData)
        {
            try
            {               
                string couponCode = couponData.UserCouponCode;

                var user = UserInfoProvider.GetUserInfo(couponData.UserID);
                if(user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "User not found"
                    });
                }

                var CurrentShoppingCart = Service.Resolve<ICurrentShoppingCartService>().GetCurrentShoppingCart(user, SiteContext.CurrentSiteID);

                if(CurrentShoppingCart != null)
                {
                    var coupons = CurrentShoppingCart.CouponCodes;

                    int couponsCodes = coupons.AllAppliedCodes.Count();

                    if (couponsCodes >= 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                        {
                            status = HttpStatusCode.BadRequest,
                            errorCode = HttpStatusCode.BadRequest.ToString(),
                            description = "You can enter 1 code at a time"
                        });                        
                    }

                    bool isCouponCodeApplied = CurrentShoppingCart.AddCouponCode(couponCode);

                    if (!string.IsNullOrEmpty(couponCode) && isCouponCodeApplied)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            status = HttpStatusCode.OK,
                            message = "Success"
                        });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                        {
                            status = HttpStatusCode.NotFound,
                            errorCode = HttpStatusCode.NotFound.ToString(),
                            description = "Coupon not valid"
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "Shoppingcard not found"
                    });
                }                                
            }
            catch (Exception ex)
            {              
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = ex.Message
                });
            }
        }

        #endregion

    }
}
