using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CustomWebApi.Helpers;
using CustomWebApi.Model.Shared;
using CustomWebApi.Model.ShoppingCart;
using CustomWebApi.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CustomWebApi.Controllers
{
    public class ShoppingCartController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage AddToCart(AddToCartModel addToCart)
        {
            try
            {

                //get the user info
                var user = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(addToCart.UserID, 0));
                if (user != null)
                {
                    // Gets the product
                    SKUInfo skuinfo = SKUInfoProvider.GetSKUs()
                                                        .WhereEquals("SKUID", addToCart.SKUID)
                                                        .TopN(1)
                                                        .FirstOrDefault();
                    if (skuinfo != null)
                    {

                        // Gets the shopping cart info based on the entered user ID
                        var mCurrentShoppingCart = Service.Resolve<ICurrentShoppingCartService>().GetCurrentShoppingCart(user, SiteContext.CurrentSiteID);

                        // Creates the shopping cart in the database if it does not exist yet
                        if (mCurrentShoppingCart.ShoppingCartID == 0)
                        {
                            mCurrentShoppingCart.ShoppingCartNote = "Add to cart";
                            ShoppingCartInfoProvider.SetShoppingCartInfo(mCurrentShoppingCart);
                        }
                        else
                        {
                            // Gets the first product from the current shopping cart
                            List<ShoppingCartItemInfo> cartInfo = mCurrentShoppingCart.CartItems.ToList();
                            var cartItem = cartInfo != null ? cartInfo.Where(x => x.SKUID == addToCart.SKUID).FirstOrDefault() : null;

                            if (cartItem != null)
                            {                                                               
                                // Removes the item from the shopping cart
                                ShoppingCartInfoProvider.RemoveShoppingCartItem(mCurrentShoppingCart, cartItem.CartItemID);

                                // Removes the item from the database
                                ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(cartItem);

                                // Evaluates and recalculates the shopping cart (discounts, shipping, price totals, etc).
                                mCurrentShoppingCart.Evaluate();
                            }
                        }

                        if (addToCart.SKUID != 0)
                        {                                                      
                            // Prepares a shopping cart item representing 1 unit of the product
                            ShoppingCartItemParameters parameters = new ShoppingCartItemParameters(addToCart.SKUID, ValidationHelper.GetInteger(addToCart.SKUUnits, 1));
                            ShoppingCartItemInfo cartItem = ShoppingCartInfoProvider.SetShoppingCartItem(mCurrentShoppingCart, parameters);

                            // Saves the shopping cart item to the shopping cart
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(cartItem);

                            // Evaluates and recalculates the shopping cart (discounts, shipping, price totals, etc).
                            mCurrentShoppingCart.Evaluate();
                        }

                        // Creates the model representing the collection of a customer's shopping cart items + info
                        ShoppingCartModel model = new ShoppingCartModel(mCurrentShoppingCart);

                        List<ShoppingCartModel> shoppingCart = new List<ShoppingCartModel>();
                        shoppingCart.Add(model);

                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            data = shoppingCart,
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
                            description = "SKU Not Found",
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "User Not Found",
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message.ToString(),
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage RemoveToCart(AddToCartModel addToCart)
        {
            try
            {
                //get the user info
                var user = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(addToCart.UserID, 0));
                if (user != null)
                {
                    // Gets the product
                    SKUInfo skuinfo = SKUInfoProvider.GetSKUs()
                                                        .WhereEquals("SKUID", addToCart.SKUID)
                                                        .TopN(1)
                                                        .FirstOrDefault();
                    if (skuinfo != null)
                    {

                        // Gets the shopping cart info based on the entered user ID
                        var mCurrentShoppingCart = Service.Resolve<ICurrentShoppingCartService>().GetCurrentShoppingCart(user, SiteContext.CurrentSiteID);

                        // Creates the shopping cart in the database if it does not exist yet
                        if (mCurrentShoppingCart.ShoppingCartID == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                            {
                                status = HttpStatusCode.NotFound,
                                errorCode = HttpStatusCode.NotFound.ToString(),
                                description = "ShoppingCart not found",
                            });
                            //ShoppingCartInfoProvider.SetShoppingCartInfo(mCurrentShoppingCart);
                        }
                        else
                        {
                            // Gets the first product from the current shopping cart
                            List<ShoppingCartItemInfo> cartInfo = mCurrentShoppingCart.CartItems.ToList();
                            var cartItem = cartInfo != null ? cartInfo.Where(x => x.SKUID == addToCart.SKUID).FirstOrDefault() : null;

                            if (cartItem != null)
                            {
                                // Removes the item from the shopping cart
                                ShoppingCartInfoProvider.RemoveShoppingCartItem(mCurrentShoppingCart, cartItem.CartItemID);

                                // Removes the item from the database
                                ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(cartItem);

                                // Evaluates and recalculates the shopping cart (discounts, shipping, price totals, etc).
                                mCurrentShoppingCart.Evaluate();

                                DeleteProjectDetail(addToCart.SKUID);
                            }
                        }

                        // Creates the model representing the collection of a customer's shopping cart items + info
                        ShoppingCartModel model = new ShoppingCartModel(mCurrentShoppingCart);

                        List<ShoppingCartModel> shoppingCart = new List<ShoppingCartModel>();
                        shoppingCart.Add(model);

                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            data = shoppingCart,
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
                            description = "SKU Not Found",
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "User Not Found",
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message.ToString(),
                });
            }
        }

        private void DeleteProjectDetail(int SKUID)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
            string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";

            string photoProjectDetail = "PrintForme.PhotoProjectDetail";
            string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";
            string wallPrintingProjectDetail = "PrintForme.WallPrintingProjectDetail";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            DataClassInfo wallPrintingProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);

            DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
            DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
            DataClassInfo wallPrintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectDetail);

            if (SKUID > 0)
            {
                if (photoProjectMasterInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem item = CustomTableItemProvider.GetItems(photoProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (item != null)
                    {
                        if (ValidationHelper.GetInteger(item.GetValue("ItemID"), 0) > 0)
                        {
                            int projectID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0);

                            if (photoProjectDetailInfo != null)
                            {
                                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                                List<CustomTableItem> items = CustomTableItemProvider.GetItems(photoProjectDetail)
                                .WhereEquals("ProjectID", projectID)
                                .Columns("ProjectID", "ImageUrl", "ItemID").ToList();

                                foreach (var itemDetail in items)
                                {                                   
                                    // Delete detail data
                                    itemDetail.Delete();
                                }
                            }

                            // Delete master data
                            item.Delete();
                        }
                    }
                }

                if (woodenProjectMasterInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem item = CustomTableItemProvider.GetItems(woodenProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (item != null)
                    {
                        if (ValidationHelper.GetInteger(item.GetValue("ItemID"), 0) > 0)
                        {
                            int projectID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0);

                            if (woodenProjectDetailInfo != null)
                            {
                                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                                List<CustomTableItem> items = CustomTableItemProvider.GetItems(woodenProjectDetail)
                                .WhereEquals("ProjectID", projectID)
                                .Columns("ProjectID", "ImageUrl", "ItemID").ToList();

                                foreach (var itemDetail in items)
                                {                                    
                                    // Delete detail data
                                    itemDetail.Delete();
                                }
                            }

                            // Delete master data
                            item.Delete();
                        }
                    }
                }

                if (wallPrintingProjectMasterInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                    CustomTableItem item = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                             .WhereEquals("SKUID", SKUID)
                             .WhereEquals("IsComplete", false).LastOrDefault();

                    if (item != null)
                    {
                        if (ValidationHelper.GetInteger(item.GetValue("ItemID"), 0) > 0)
                        {
                            int projectID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0);

                            if (wallPrintingProjectDetailInfo != null)
                            {
                                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                                List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallPrintingProjectDetail)
                                .WhereEquals("ProjectID", projectID)
                                .Columns("ProjectID", "ImageUrl", "ItemID").ToList();

                                foreach (var itemDetail in items)
                                {                                    
                                    // Delete detail data
                                    itemDetail.Delete();
                                }
                            }

                            // Delete master data
                            item.Delete();
                        }
                    }
                }
            }
        }
    }
}
