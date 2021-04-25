using CMS.Ecommerce;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrintForMe.Models.Products
{
    public class ProductListItemViewModel
    {
        public readonly PriceDetailViewModel PriceModel;
        public string Name;
        public string ImagePath;
        public string PublicStatusName;
        public bool Available;
        public Guid ProductPageGuid;
        public string ProductPageAlias;
        public int SKUID;
        public PriceDetailViewModel Price;
        public int DepartmentID;
        public List<string> SizeList;

        public ProductListItemViewModel()
        {
        }

        /// <summary>
        /// Constructor for the ProductListItemViewModel class.
        /// </summary>
        /// <param name="productPage">Product's page.</param>
        /// <param name="priceDetail">Price of the product.</param>
        /// <param name="publicStatusName">Display name of the product's public status.</param>
        public ProductListItemViewModel(SKUTreeNode productPage, ProductCatalogPrices priceDetail, string publicStatusName)
        {
            // Sets the page information
            Name = productPage.DocumentName;
            ProductPageGuid = productPage.NodeGUID;
            ProductPageAlias = productPage.NodeAlias;
            SKUID = productPage.NodeSKUID;
            DepartmentID = SKUInfoProvider.GetSKUInfo(SKUID).SKUDepartmentID;
            // Sets the SKU information
            ImagePath = productPage.SKU.SKUImagePath;
            Available = !productPage.SKU.SKUSellOnlyAvailable || productPage.SKU.SKUAvailableItems > 0;
            PublicStatusName = publicStatusName;
            var variants = VariantHelper.GetVariants(SKUID);
            if (variants != null)
            {
                SizeList = variants.Select(varient => varient.SKUNumber).ToList();
            }

            // Sets the price format information
            PriceModel = new PriceDetailViewModel
            {
                Price = priceDetail.Price,
                ListPrice = priceDetail.ListPrice,
                CurrencyFormatString = priceDetail.Currency.CurrencyFormatString
            };
        }
    }
}