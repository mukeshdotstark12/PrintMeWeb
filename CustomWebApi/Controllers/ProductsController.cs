using CMS.Ecommerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CustomWebApi.Controllers
{
    public class ProductsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetBundleProducts(int id)
        {
            //get bundle Products info
            var bundleProducts = SKUInfoProvider.GetSKUs()
                                            .WhereIn("SKUID", BundleInfoProvider.GetBundles()
                                                                                .Column("SKUID")
                                                                                .WhereEquals("BundleID", id));

            // Creates the list representing the bundle Products ids
            var bundleProductsIds = bundleProducts.Select(a => new { a.SKUID, a.SKUImagePath });

            return Json(bundleProductsIds);
        }        
    }
}
