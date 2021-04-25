using CMS.Ecommerce;
using PrintForMe.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrintForMe.Models.Checkout
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCartItemViewModel> CartItems { get; set; }

        public string ShoppingCartNote { get; set; }
        public string CurrencyFormatString { get; set; }

        public IEnumerable<string> CouponCodes { get; set; }

        public decimal TotalTax { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal CartTotalDiscount { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal RemainingAmountForFreeShipping { get; set; }

        public bool GiftWrapping { get; set; }
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Constructor for the ShoppingCartViewModel. 
        /// </summary>
        /// <param name="cart">A shopping cart object.</param>
        public ShoppingCartViewModel(ShoppingCartInfo cart,string path)
        {
            // Creates a collection containing all lines from the given shopping cart
            CartItems = cart.CartProducts.Select((cartItemInfo) =>
            {
                var SKU = SKUInfoProvider.GetSKUs().WhereEquals("SKUID", cartItemInfo.SKUID).LastOrDefault();
                TotalPrice += SKU.SKUPrice * cartItemInfo.CartItemUnits;
                CartTotalDiscount += cartItemInfo.TotalDiscount;
               if(cartItemInfo.SKU.SKUDepartmentID == 3)
                {
                    cartItemInfo.SKU.SKUImagePath = cartItemInfo.SKU.SKUImagePath;
                }
                else
                {
                    cartItemInfo.SKU.SKUImagePath = Convert.ToBase64String(ImageToByteArray(System.Drawing.Image.FromFile(ServiceInformation.GetProjectInformation(cartItemInfo.SKUID, path,false).ImagePath)));
                }

                return new ShoppingCartItemViewModel()
                {
                    CartItemUnits = cartItemInfo.CartItemUnits,
                    SKUName = cartItemInfo.SKU.SKUName,
                    ListPrice = cartItemInfo.TotalPrice,
                    OldPrice = cartItemInfo.TotalPrice,
                    TotalDiscount = cartItemInfo.TotalDiscount,
                    CartItemID = cartItemInfo.CartItemID,
                    SKUID = cartItemInfo.SKUID,
                    SKUImage = cartItemInfo.SKU.SKUImagePath,
                    SKUDepartmentID = cartItemInfo.SKU.SKUDepartmentID
                };
            });
            ShoppingCartNote = cart.ShoppingCartNote;
            CurrencyFormatString = cart.Currency.CurrencyFormatString;
            CouponCodes = cart.CouponCodes.AllAppliedCodes.Select(x => x.Code);
            TotalTax = cart.TotalTax;
            TotalShipping = cart.TotalShipping;
            GrandTotal = cart.GrandTotal;
            RemainingAmountForFreeShipping = cart.CalculateRemainingAmountForFreeShipping();
            GiftWrapping = cart.GetBooleanValue("ShoppingCartGiftWrapping", false);
            IsEmpty = cart.IsEmpty;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
    }
}