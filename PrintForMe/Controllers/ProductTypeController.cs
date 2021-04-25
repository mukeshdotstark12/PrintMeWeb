using CMS.Core;
using CMS.DocumentEngine.Types.PrintForMe;
using CMS.Ecommerce;
using PrintForMe.Models.Products;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class ProductTypeController : Controller
    {
        private readonly string siteName = "PrintForMe";//SiteContext.CurrentSiteName;
        private readonly IShoppingService shoppingService;
        private readonly ICatalogPriceCalculatorFactory calculatorFactory;

        public ProductTypeController()
        {
            shoppingService = Service.Resolve<IShoppingService>();
            calculatorFactory = Service.Resolve<ICatalogPriceCalculatorFactory>();
        }

        // GET: ProductType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTrendingProductsSection()
        {
            List<Products> products = ProductsProvider.GetProducts()
                .LatestVersion(false)
                .Published(true)
                .OnSite(siteName)
                .CombineWithDefaultCulture()
                .WhereTrue("SKUEnabled")
                .WhereEquals("SKUPublicStatusID", 1)
                .OrderByDescending("SKUInStoreFrom")
                .TopN(4)
                .ToList();

            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            IEnumerable<ProductListItemViewModel> model = products.Select(
                            product => new ProductListItemViewModel(
                                product,
                                GetPrice(product.SKU, cart),
                                product.Product.PublicStatus?.PublicStatusDisplayName));

            return PartialView("_productsWidget", model);
        }

        private ProductCatalogPrices GetPrice(SKUInfo product, ShoppingCartInfo cart)
        {
            return calculatorFactory
                        .GetCalculator(cart.ShoppingCartSiteID)
                        .GetPrices(product, Enumerable.Empty<SKUInfo>(), cart);
        }

        public ActionResult GetNewProductsSection()
        {
            List<Products> products = ProductsProvider.GetProducts()
                .LatestVersion(false)
                .Published(true)
                .OnSite(siteName)
                .CombineWithDefaultCulture()
                .WhereTrue("SKUEnabled")
                .WhereEquals("SKUPublicStatusID", 3)
                .OrderByDescending("SKUInStoreFrom")
                .TopN(4)
                .ToList();

            ShoppingCartInfo cart = shoppingService.GetCurrentShoppingCart();

            IEnumerable<ProductListItemViewModel> model = products.Select(
                            product => new ProductListItemViewModel(
                                product,
                                GetPrice(product.SKU, cart),
                                product.Product.PublicStatus?.PublicStatusDisplayName));

            return PartialView("_productsWidget", model);
        }

        public ActionResult GetNewBrandsSection()
        {
            List<BrandInfo> brands = BrandInfoProvider.GetBrands()
                .OrderBy("BrandLastModified")
                .TopN(4)
                .ToList();

            IEnumerable<BrandListItemViewModel> model = brands.Select(item => new BrandListItemViewModel()
            {
                ID = item.BrandID,
                Name = item.BrandDisplayName,
                ImagePath = "",
                Website = item.BrandHomepage
            });

            return PartialView("_newBrands", model);
        }
    }
}