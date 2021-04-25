using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PrintForMe.Helpers;
using PrintForMe.Models;
using PrintForMe.Models.Album;
using PrintForMe.Models.Checkout;
using PrintForMe.Models.PayTabs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using static PrintForMe.Models.PayTabs.MakePaymentModel;

namespace PrintForMe.Controllers
{
    public class PaymentController : Controller
    {

        /// <summary>
        /// Fictitious method for creating a payment response information and paying the order.
        /// </summary>
        /// <param name="orderID">ID of the paid order.</param>
        public ActionResult Index(int orderID)
        {
            // Gets the order
            OrderInfo order = OrderInfoProvider.GetOrderInfo(orderID);

            // Validates the retrieved order
            if (order?.OrderSiteID != SiteContext.CurrentSiteID)
            {
                // Redirects back to the order review step if validation fails
                return RedirectToAction("PreviewAndPay");
            }

            #region Upload To Azure
            var orderInfo = OrderItemInfoProvider.GetOrderItems(orderID);
            if (orderInfo != null)
            {
                UploadToAzure(orderInfo, order);
            }
            else
            {
                // Redirects back to the order review step if validation fails
                return RedirectToAction("PreviewAndPay");
            }
            #endregion


            #region PayTabs
            //////////////////////////////PayTabs//////////////////////////

            if (order.OrderPaymentOptionID.Equals(2))
            {
                OrderModel orderModel = new OrderModel(order);

                var orderInfoJSON = JsonConvert.SerializeObject(orderModel, Formatting.Indented);

                var utility = new PaymentGateway();

                //MakePaymentModel.VerifySecretKeyResponse secretKeyResponse = utility.ValidateKey();

                var orderData = JsonConvert.DeserializeObject<MakePaymentModel.PayPageRequest>(orderInfoJSON.ToString());

                var payPageResponse = utility.MakePayment(orderData);

                //var data = JsonConvert.DeserializeObject<MakePaymentModel.PayPageRequest>(payPageResponse.ToString());                   
                if (payPageResponse == null)
                {
                    return RedirectToAction("Failed");
                }

                var newPayment = new PaymentInfo
                {
                    OrderID = order.OrderID,
                    Amount = order.OrderTotalPrice,
                    PayTabsPaymentID = payPageResponse.tran_ref,
                    //SenderID = _user.UserID,
                    //Type = "Full Payment",
                    PaymentMethodID = order.OrderPaymentOptionID,
                    //Fees = _processingCharges,
                    PaymentDatetime = DateTime.Now,
                    Status = "Pending",
                    //ReceiverID = isMembership ? 0 : _order.GetIntegerValue("DriverID", 0)
                };
                PaymentInfoProvider.SetPaymentInfo(newPayment);

                return Redirect(payPageResponse.redirect_url);
            }

            ///////////////////////////ENDS///////////////////////////////
            #endregion

            // Creates a fictitious response
            ResponseViewModel response = new ResponseViewModel()
            {
                InvoiceNo = order.OrderID,
                Message = "Successfully paid",
                Completed = true,
                TransactionID = new Random().Next(100000, 200000).ToString(),
                Amount = order.OrderTotalPrice,
                ResponseCode = "",
                Approved = true
            };

            // Validates the response and pays the order
            Validate(response);

            // Redirects to the thank-you page
            return RedirectToAction("ThankYou", "Checkout", new { OrderID = orderID });
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Failed()
        {
            return View();
        }
        /// <summary>
        /// Pays the specified order with a fictitious response.
        /// </summary>
        /// <param name="response">Fictitious response about payment.</param>
        private void Validate(ResponseViewModel response)
        {
            //DocSection:PaymentValidation
            if (response != null)
            {
                // Gets the order based on the invoice number from the response
                OrderInfo order = OrderInfoProvider.GetOrderInfo(response.InvoiceNo);
                if (order?.OrderSiteID != SiteContext.CurrentSiteID)
                {
                    order = null;
                }

                // Checks whether the paid amount of money matches the order price
                // and whether the payment was approved
                if (order != null && response.Amount == order.OrderTotalPrice && response.Approved)
                {
                    // Creates a payment result object that will be viewable in Kentico
                    PaymentResultInfo result = new PaymentResultInfo
                    {
                        PaymentDate = DateTime.Now,
                        PaymentDescription = response.Message,
                        PaymentIsCompleted = response.Completed,
                        PaymentTransactionID = response.TransactionID,
                        PaymentStatusValue = response.ResponseCode,
                        PaymentMethodName = "PaymentName"
                    };

                    // Saves the payment result to the database
                    order.UpdateOrderStatus(result);
                }
            }
            //EndDocSection:PaymentValidation
        }

        public ActionResult VerifyPayment(int orderID)
        {
            var paymentInfo = PaymentInfoProvider.GetPayments().WhereEquals("OrderID", orderID).LastOrDefault();

            if (paymentInfo == null)
            {
                return new HttpNotFoundResult();
            }
            PaymentVerficationResult paymentObj = VerfiyPayment(paymentInfo.PayTabsPaymentID);
            if (paymentObj.payment_result.response_status == "A")
            {
                paymentInfo.Status = "Paid";
                PaymentInfoProvider.SetPaymentInfo(paymentInfo);

                return RedirectToAction("ThankYou", "Checkout", new { OrderID = orderID });
            }
            else
            {

                paymentInfo.Status = "Rejected";
                paymentInfo.Comment = paymentObj.payment_result.response_message;
                PaymentInfoProvider.SetPaymentInfo(paymentInfo);
            }

            return RedirectToAction("Failed", "Payment");
        }

        public PaymentVerficationResult VerfiyPayment(string transID)
        {
            HttpClient client = new HttpClient();
            string verficationUrl = "https://secure.paytabs.sa/payment/query";
            var objPayment = new PaymentVerication()
            {
                profile_id = 63562,
                tran_ref = transID
            };
            var json = JsonConvert.SerializeObject(objPayment);
            json = json.Replace("_return", "return");
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(verficationUrl),
                Headers = {
           { HttpRequestHeader.Authorization.ToString(),"SHJNN92ZBJ-JBHRKNR2ZD-LHJKLJM6WK" },
           { HttpRequestHeader.ContentType.ToString(), "application/json" }
       },
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = client.SendAsync(httpRequestMessage).Result;
            PaymentVerficationResult objResult = JsonConvert.DeserializeObject<PaymentVerficationResult>(response.Content.ReadAsStringAsync().Result);
            return objResult;
        }

        private void CompletePhotoProject(int SKUID)
        {

            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
            string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";
            string Albummaster = "PrintForme.Album";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            DataClassInfo wallPrintingProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);
            DataClassInfo albummasterInfo = DataClassInfoProvider.GetDataClassInfo(Albummaster);

            //// Gets the product
            //SKUInfo deleteProduct = SKUInfoProvider.GetSKUs()
            //                                    .WhereEquals("SKUID", SKUID)                                                
            //                                    .TopN(1)
            //                                    .FirstOrDefault();

            //// Gets a TreeProvider instance
            //TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            //// Gets the product page
            //TreeNode node = tree.SelectNodes()
            //    .Path("/Store/Products")
            //    //.WhereEquals("NodeSKUID",SKUID)
            //    .OnCurrentSite()
            //    .CombineWithDefaultCulture()
            //    .TopN(1)
            //    .FirstOrDefault();

            //if (node != null)
            //{
            //    // Deletes the product page
            //    DocumentHelper.DeleteDocument(node, tree, true, true);
            //}

            //if (deleteProduct != null)
            //{
            //    // Deletes the product
            //    SKUInfoProvider.DeleteSKUInfo(deleteProduct);
            //}

            if (photoProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                var item = CustomTableItemProvider.GetItems(photoProjectMaster)
                                                                    .WhereEquals("SKUID", SKUID)
                                                                    .FirstOrDefault();

                if (item != null)
                {
                    // Sets a new 'ItemText' value based on the old one
                    item.SetValue("IsComplete", true);

                    // Saves the changes to the database
                    item.Update();

                    //var path = Path.GetTempPath() + "/PrintForMe/PhotoProject/" + User.Identity.GetUserName();
                    string path = Server.MapPath("~/PrintForMe/PhotoProject/" + User.Identity.GetUserName());

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (Directory.Exists(path))
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            string[] files = Directory.GetFiles(path);

                            foreach (string file in files)
                            {
                                if (System.IO.File.Exists(file))
                                {
                                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                                    System.IO.File.Delete(file);
                                }
                            }
                        }
                    }
                    return;
                }
            }

            if (woodenProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                var item = CustomTableItemProvider.GetItems(woodenProjectMaster)
                                                                    .WhereEquals("SKUID", SKUID)
                                                                    .FirstOrDefault();

                if (item != null)
                {
                    // Sets a new 'ItemText' value based on the old one
                    item.SetValue("IsComplete", true);

                    // Saves the changes to the database
                    item.Update();

                    string path = Server.MapPath("~/PrintForMe/WoodenProject/" + User.Identity.GetUserName());
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (Directory.Exists(path))
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            string[] files = Directory.GetFiles(path);

                            foreach (string file in files)
                            {
                                if (System.IO.File.Exists(file))
                                {
                                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                                    System.IO.File.Delete(file);
                                }
                            }
                        }
                    }
                    return;
                }
            }

            if (wallPrintingProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                var item = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                                                                    .WhereEquals("SKUID", SKUID)
                                                                    .FirstOrDefault();

                if (item != null)
                {
                    // Sets a new 'ItemText' value based on the old one
                    item.SetValue("IsComplete", true);

                    // Saves the changes to the database
                    item.Update();

                    string path = Server.MapPath("~/PrintForMe/WallPaintingProject/" + User.Identity.GetUserName());
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (Directory.Exists(path))
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            string[] files = Directory.GetFiles(path);

                            foreach (string file in files)
                            {
                                if (System.IO.File.Exists(file))
                                {
                                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                                    System.IO.File.Delete(file);
                                }
                            }
                        }
                    }
                    return;
                }
            }

            if (albummasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                var item = CustomTableItemProvider.GetItems(Albummaster)
                                                                    .WhereEquals("SKUID", SKUID)
                                                                    .FirstOrDefault();

                if (item != null)
                {
                    // Sets a new 'ItemText' value based on the old one
                    item.SetValue("State", false);

                    // Saves the changes to the database
                    item.Update();

                    QueryDataParameters parameters = new QueryDataParameters();
                    parameters.Add("@Albumid", item.ItemID);

                    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_DeletePhotoQueuebyID", parameters, QueryTypeEnum.StoredProcedure);

                    var path = Server.MapPath("~/Album/" + item.ItemID + "/Queue");
                    if (Directory.Exists(path))
                    {
                        try
                        {
                            Directory.Delete(path, recursive: true);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            Directory.Delete(path, recursive: true);
                        }
                    }
                    return;
                }
            }
        }

        private void UploadToAzure(ICollection<OrderItemInfo> orderItems, OrderInfo order)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
            string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";
            string Albummaster = "PrintForme.Album";

            string photoProjectDetail = "PrintForme.PhotoProjectDetail";
            string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";
            string wallPrintingProjectDetail = "PrintForme.WallPrintingProjectDetail";
            //string albumpagephoto = "PrintForMe_AlbumPagePhoto";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            DataClassInfo wallPrintingProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);
            DataClassInfo albummasterInfo = DataClassInfoProvider.GetDataClassInfo(Albummaster);

            DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
            DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
            DataClassInfo wallPrintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectDetail);
            //DataClassInfo albumpagephotolInfo = DataClassInfoProvider.GetDataClassInfo(albumpagephoto);

            if (orderItems != null)
            {
                foreach (var item in orderItems)
                {
                    if (photoProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem photoItem = CustomTableItemProvider.GetItems(photoProjectMaster)
                                 .WhereEquals("SKUID", item.OrderItemSKUID)
                                 .WhereEquals("IsComplete", false).LastOrDefault();

                        if (photoItem != null)
                        {
                            int projectID = photoItem.GetValue("ItemID", 0);

                            if (photoProjectDetailInfo != null)
                            {
                                var imageUrls = CustomTableItemProvider.GetItems(photoProjectDetail).WhereEquals("ProjectID", projectID).Columns("ProjectID", "ImageUrl", "ItemID");

                                foreach (CustomTableItem image in imageUrls)
                                {
                                    // Upload Images to Azure

                                    string path = Server.MapPath("~/PrintForMe/PhotoProject") + image.GetValue("ImageUrl", "");
                                    string[] strExtention = path.Split('.');

                                    if (System.IO.File.Exists(path))
                                    {
                                        using (Stream fileStream = System.IO.File.OpenRead(@path))
                                        {
                                            string fileName = $"CID{order.OrderCustomerID}OID{order.OrderID}/{DateTime.Now.Ticks.ToString()}.{strExtention[1]}";
                                            var uploadedResult = UploadToBlob.UploadFileToBlob(fileName, fileStream);

                                            if (ValidationHelper.GetString(uploadedResult.Uri, "") != "")
                                            {
                                                // Sets a new 'ItemText' value based on the old one
                                                image.SetValue("ImageUrl", uploadedResult.Uri.ToString());

                                                // Saves the changes to the database
                                                image.Update();
                                            }
                                        }
                                    }
                                }
                            }
                            //Complete project
                            CompletePhotoProject(item.OrderItemSKUID);
                        }
                    }

                    if (woodenProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem woodenItem = CustomTableItemProvider.GetItems(woodenProjectMaster)
                                 .WhereEquals("SKUID", item.OrderItemSKUID)
                                 .WhereEquals("IsComplete", false).LastOrDefault();

                        if (woodenItem != null)
                        {
                            int projectID = woodenItem.GetValue("ItemID", 0);

                            if (woodenProjectDetailInfo != null)
                            {
                                var imageUrls = CustomTableItemProvider.GetItems(woodenProjectDetail).WhereEquals("ProjectID", projectID).Columns("ProjectID", "ImageUrl", "ItemID");

                                foreach (CustomTableItem image in imageUrls)
                                {
                                    // Upload Images to Azure

                                    string path = Server.MapPath("~/PrintForMe/WoodenProject") + image.GetValue("ImageUrl", "");
                                    string[] strExtention = path.Split('.');

                                    if (System.IO.File.Exists(path))
                                    {
                                        using (Stream fileStream = System.IO.File.OpenRead(@path))
                                        {
                                            string fileName = $"CID{order.OrderCustomerID}OID{order.OrderID}/{DateTime.Now.Ticks.ToString()}.{strExtention[1]}";
                                            var uploadedResult = UploadToBlob.UploadFileToBlob(fileName, fileStream);

                                            if (ValidationHelper.GetString(uploadedResult.Uri, "") != "")
                                            {
                                                // Sets a new 'ItemText' value based on the old one
                                                image.SetValue("ImageUrl", uploadedResult.Uri.ToString());

                                                // Saves the changes to the database
                                                image.Update();
                                            }
                                        }
                                    }
                                }
                            }
                            //Complete project
                            CompletePhotoProject(item.OrderItemSKUID);
                        }
                    }

                    if (wallPrintingProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem wallItem = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                                 .WhereEquals("SKUID", item.OrderItemSKUID)
                                 .WhereEquals("IsComplete", false).LastOrDefault();

                        if (wallItem != null)
                        {
                            int projectID = wallItem.GetValue("ItemID", 0);

                            if (wallPrintingProjectDetailInfo != null)
                            {
                                var imageUrls = CustomTableItemProvider.GetItems(wallPrintingProjectDetail).WhereEquals("ProjectID", projectID).Columns("ProjectID", "ImageUrl", "ItemID");

                                foreach (CustomTableItem image in imageUrls)
                                {
                                    // Upload Images to Azure

                                    string path = Server.MapPath("~/PrintForMe/WallPaintingProject") + image.GetValue("ImageUrl", "");
                                    string[] strExtention = path.Split('.');

                                    if (System.IO.File.Exists(path))
                                    {
                                        using (Stream fileStream = System.IO.File.OpenRead(@path))
                                        {
                                            string fileName = $"CID{order.OrderCustomerID}OID{order.OrderID}/{DateTime.Now.Ticks.ToString()}.{strExtention[1]}";
                                            var uploadedResult = UploadToBlob.UploadFileToBlob(fileName, fileStream);

                                            if (ValidationHelper.GetString(uploadedResult.Uri, "") != "")
                                            {
                                                // Sets a new 'ItemText' value based on the old one
                                                image.SetValue("ImageUrl", uploadedResult.Uri.ToString());

                                                // Saves the changes to the database
                                                image.Update();
                                            }
                                        }
                                    }
                                }
                            }
                            //Complete project
                            CompletePhotoProject(item.OrderItemSKUID);
                        }
                    }

                    if (albummasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem albumItem = CustomTableItemProvider.GetItems(Albummaster)
                                 .WhereEquals("SKUID", item.OrderItemSKUID)
                                 .WhereEquals("State", true).LastOrDefault();

                        if (albumItem != null)
                        {
                            int projectID = albumItem.GetValue("ItemID", 0);

                            if (albummasterInfo != null)
                            {
                                QueryDataParameters parameters = new QueryDataParameters();
                                parameters.Add("@albumid", projectID);
                                DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_getdownloadpdf", parameters, QueryTypeEnum.StoredProcedure);
                                List<AlbumPagePhotos> imageUrl = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumPagePhotos
                                {
                                    Row_num = dataRow.Field<string>("row_num"),
                                    Albumpageid = dataRow.Field<string>("AlbumPageID"),
                                    Name = dataRow.Field<string>("Name"),
                                    PhotoLocation = dataRow.Field<string>("PhotoLocation"),
                                    Sequence = dataRow.Field<int>("Sequence"),
                                    AlbumPagePhotoID = dataRow.Field<int>("ItemID"),
                                    APLItemID = dataRow.Field<int>("AlbumPageLayoutID"),
                                    APAlbumID = dataRow.Field<int>("AlbumID"),
                                }).ToList();

                                foreach (var image in imageUrl)
                                {
                                    // Upload Images to Azure
                                    int PhotoID = image.AlbumPagePhotoID;
                                    string path = image.PhotoLocation;
                                    string[] strExtention = path.Split('.');

                                    if (System.IO.File.Exists(path))
                                    {
                                        using (Stream fileStream = System.IO.File.OpenRead(@path))
                                        {
                                            string fileName = $"CID{order.OrderCustomerID}OID{order.OrderID}/{DateTime.Now.Ticks.ToString()}.{strExtention[1]}";
                                            var uploadedResult = UploadToBlob.UploadFileToBlob(fileName, fileStream);

                                            if (ValidationHelper.GetString(uploadedResult.Uri, "") != "")
                                            {
                                                QueryDataParameters parameter = new QueryDataParameters();
                                                parameter.Add("@photolocation", uploadedResult.Uri.AbsoluteUri.ToString());
                                                parameter.Add("@PagePhotoID", PhotoID);

                                                DataSet ds1 = ConnectionHelper.ExecuteQuery("SP_Printforme_UploadtoAzure", parameter, QueryTypeEnum.StoredProcedure);
                                            }
                                        }
                                    }
                                }
                            }
                            //Complete project
                            CompletePhotoProject(item.OrderItemSKUID);
                        }
                    }
                }
            }
        }
    }
}