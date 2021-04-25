using CMS.Ecommerce;
using CustomWebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.ShoppingCart
{
    public class ShoppingCartModel
    {
        public IEnumerable<ShoppingCartItemModel> CartItems { get; set; }

        public string CurrencyFormatString { get; set; }

        public IEnumerable<string> CouponCodes { get; set; }

        public decimal TotalTax { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal RemainingAmountForFreeShipping { get; set; }

        public bool IsEmpty { get; set; }
        public int ShoppingCartID { get; set; }

        public int ShoppingCartCustomerID { get; set; }
        public int ShoppingCartCurrencyID { get; set; }
        public int ShoppingCartPaymentOptionID { get; set; }
        public int ShoppingCartShippingOptionID { get; set; }
        public int ShoppingCartBillingAddressID { get; set; }
        public int ShoppingCartShippingAddressID { get; set; }

        /// <summary>
        /// Constructor for the ShoppingCartViewModel. 
        /// </summary>
        /// <param name="cart">A shopping cart object.</param>
        public ShoppingCartModel(ShoppingCartInfo cart)
        {
            // Creates a collection containing all lines from the given shopping cart
            CartItems = cart.CartProducts.Select((cartItemInfo) =>
            {
                var SKU = SKUInfoProvider.GetSKUs().WhereEquals("SKUID", cartItemInfo.SKUID).FirstOrDefault();
                TotalPrice += SKU.SKUPrice * cartItemInfo.CartItemUnits;
                return new ShoppingCartItemModel()
                {
                    CartItemUnits = cartItemInfo.CartItemUnits,
                    SKUName = cartItemInfo.SKU.SKUName,
                    TotalPrice = cartItemInfo.TotalPrice,
                    CartItemID = cartItemInfo.CartItemID,
                    SKUID = cartItemInfo.SKUID,
                    SKUPrice = cartItemInfo.UnitPrice,
                    SKUImagePath = SKU.SKUImagePath,
                    ProjectInfo = ServiceInformation.GetProjectInformationForShoppinCart(cartItemInfo.SKUID)
                };
            });
            CurrencyFormatString = cart.Currency.CurrencyFormatString;
            CouponCodes = cart.CouponCodes.AllAppliedCodes.Select(x => x.Code);
            TotalTax = cart.TotalTax;
            TotalShipping = cart.TotalShipping;
            GrandTotal = cart.GrandTotal;
            RemainingAmountForFreeShipping = cart.CalculateRemainingAmountForFreeShipping();
            IsEmpty = cart.IsEmpty;
            ShoppingCartID = cart.ShoppingCartID;

            ShoppingCartCustomerID = cart.ShoppingCartCustomerID;
            ShoppingCartCurrencyID = cart.ShoppingCartCurrencyID;
            ShoppingCartPaymentOptionID = cart.ShoppingCartPaymentOptionID;
            ShoppingCartShippingOptionID = cart.ShoppingCartShippingOptionID;
            ShoppingCartBillingAddressID = cart.ShoppingCartBillingAddressID;
            ShoppingCartShippingAddressID = cart.ShoppingCartShippingAddressID;
        }
    }
}