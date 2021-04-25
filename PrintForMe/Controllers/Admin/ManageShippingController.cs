using CMS.Ecommerce;
using CMS.EventLog;
using CMS.Helpers;
using PrintForMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers.Admin
{
    public class ManageShippingController : Controller
    {
        // GET: ManageShipping
        public ActionResult Index(int id = 0)
        {
            ManageShippingModel model = new ManageShippingModel();
            if (id == 0)
            {
                model.ShippingOptionID = ShippingOptionInfoProvider.GetShippingOptions().FirstOrDefault().ShippingOptionID;
            }
            else
            {
                model.ShippingOptionID = id;
            }
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(int id)
        {
            ManageShippingModel model = new ManageShippingModel();
            model.ShippingOptionID = id;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(ManageShippingModel model)
        {
            ShippingCostInfo info = new ShippingCostInfo
            {
                ShippingCostShippingOptionID = model.ShippingOptionID,
                ShippingCostMinWeight = model.ShippingCostMinWeight,
                ShippingCostValue = model.ShippingCostValue
            };

            ShippingCostInfoProvider.SetShippingCostInfo(info);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/ManageShipping?id=" + model.ShippingOptionID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {

                ShippingCostInfo info = ShippingCostInfoProvider.GetShippingCostInfo(id);
                ManageShippingModel model = new ManageShippingModel
                {
                    ShippingOptionID = info.ShippingCostShippingOptionID,
                    ShippingCostMinWeight = info.ShippingCostMinWeight,
                    ShippingCostValue = Math.Round(info.ShippingCostValue, 0.00),
                    ShippingCostID = info.ShippingCostID
                };
                return View(model);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("edit", "manageshipping", ex);
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ManageShippingModel model)
        {
            ShippingCostInfo info = ShippingCostInfoProvider.GetShippingCostInfo(model.ShippingCostID);
            info.ShippingCostMinWeight = model.ShippingCostMinWeight;
            info.ShippingCostValue = Math.Round(model.ShippingCostValue,0.00);
            info.ShippingCostShippingOptionID = model.ShippingOptionID;
            ShippingCostInfoProvider.SetShippingCostInfo(info);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/ManageShipping?id=" + model.ShippingOptionID);
        }

        public ActionResult Delete(int id)
        {
            ShippingCostInfo info = ShippingCostInfoProvider.GetShippingCostInfo(id);
            var shippingOption = info.ShippingCostShippingOptionID;
            ShippingCostInfoProvider.DeleteShippingCostInfo(info);
            return RedirectPermanent("/" + CMS.Localization.LocalizationContext.CurrentCulture.CultureCode + "/ManageShipping?id=" + shippingOption);

        }
    }
}