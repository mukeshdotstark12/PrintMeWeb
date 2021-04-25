using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Ecommerce;
using Newtonsoft.Json;
using PrintForMe.Helpers;
using PrintForMe.Models.Album;
using PrintForMe.Models.OrderManagement;
using PrintForMe.Models.PrintingService;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Net;
using CMS.Helpers;
using Ionic.Zip;

namespace PrintForMe.Controllers.Admin
{
    public class OrderManagementController : Controller
    {
        private static readonly int FileSizeinMB = 1024 * 1024 * 1024;
        private readonly IShoppingService shoppingService;
        public OrderManagementController()
        {
            // Initializes an instance of IShoppingService used to facilitate shopping cart interactions
            // For real-world projects, we recommend using a dependency injection
            // container to initialize service instances
            shoppingService = Service.Resolve<IShoppingService>();
        }

        // GET: OrderManagement
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetOrders()
        {
            var orders = OrderInfoProvider.GetOrders();
            List<OrdersListViewModel> orderList = new List<OrdersListViewModel>();
            if (orders != null)
            {
                foreach (var order in orders)
                {
                    orderList.Add(new OrdersListViewModel(order));
                }
            }

            var statuses = OrderStatusInfoProvider.GetOrderStatuses().ToList();
            List<StatusList> statusList = new List<StatusList>();
            foreach (var status in statuses)
            {
                StatusList statusObj = new StatusList();
                statusObj.StatusID = status.StatusID;
                statusObj.StatusName = ResHelper.GetString(status.StatusName);
                statusList.Add(statusObj);
            }

            ViewBag.DropDownData = statusList;

            //return View("OrderList", orderList.OrderByDescending(x=>x.OrderDate));
            return View("OrderList", orderList.OrderByDescending(x => x.OrderID));
        }


        public ActionResult OrderListServicesSettings()
        {
            var orders = OrderInfoProvider.GetOrders();
            if (orders != null)
            {
                // Creates a collection of view models based on the menu item and page data
                List<OrderListSettingModel> orderListSetting = new List<OrderListSettingModel>();
                var statuses = OrderStatusInfoProvider.GetOrderStatuses();
                List<SelectListItem> orderstatus = new List<SelectListItem>();

                foreach (var order in orders)
                {
                    OrderListSettingModel orderListSet = new OrderListSettingModel();
                    orderListSet.OrderID = order.OrderID;
                    orderListSet.OrderDate = order.OrderDate;
                    var customerInfo = CustomerInfoProvider.GetCustomerInfo(order.OrderCustomerID);
                    orderListSet.CustomerName = string.Format("{0} {1}", customerInfo.CustomerFirstName, customerInfo.CustomerLastName);
                    orderListSet.CustomerMobile = customerInfo.CustomerPhone;
                    orderListSet.PaymentOption = PaymentOptionInfoProvider.GetPaymentOptionInfo(order.OrderPaymentOptionID)?.PaymentOptionDisplayName;
                    orderListSet.FormattedTotalPrice = String.Format(CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID).CurrencyFormatString, order.OrderTotalPrice);

                    List<SelectListItem> status = new List<SelectListItem>();
                    foreach (OrderStatusInfo modifyStatus in statuses)
                    {
                        status.Add(new SelectListItem
                        { Text = modifyStatus.StatusName, Value = modifyStatus.StatusID.ToString() });
                        status.Add(new SelectListItem { Text = modifyStatus.StatusName, Value = modifyStatus.StatusID.ToString() });
                    }

                    var statusFinal = status.Where(x => x.Value == order.OrderStatusID.ToString()).FirstOrDefault();
                    orderListSet.statusName = statusFinal.Text;
                    orderListSet.StatusID = Convert.ToInt32(statusFinal.Value);

                    //orderListSet.orderStatus = orderstatus;
                    //orderListSet.StatusID = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusID;
                    // orderListSet.Statuses = OrderStatusInfoProvider.GetOrderStatuses(order.OrderStatusID).FirstOrDefault();
                    orderListSetting.Add(orderListSet);

                }

                List<SelectListItem> items = new List<SelectListItem>();
                foreach (OrderStatusInfo modifyStatus in statuses)
                {
                    items.Add(new SelectListItem
                    { Text = ResHelper.GetString(modifyStatus.StatusName), Value = modifyStatus.StatusID.ToString() });
                }
                ViewBag.DropDownData = items;
                ViewBag.Culture = ResHelper.GetString("ltechpro.LanguageSwitcherCurrentCulture").ToLower();
                return PartialView("_OrdersListSettings", orderListSetting.OrderByDescending(x => x.OrderID));
            }
            return PartialView("_OrdersListSettings");
        }

        [HttpPost]
		public ActionResult DownloadProjectItems(int SKUID, int OrderID)
		{
			try
			{
				int projectID = 0;
				int ServiceID = 0;
				string ServiceName = String.Empty;
				var paperMaterials = FillComboBox.GetPapaerMaterialForDescription();
				var frameColors = FillComboBox.GetFrameColorForDescription();

				var album = new Album();
				album = AlbumDetailwithPrice.GetAlbumbySkuID(SKUID);

				string path = "";
				// Get Project ID using SKUID
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

				var dirPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadFilePath"] + "\\");
				if (!string.IsNullOrEmpty(dirPath))
				{
					if (Directory.Exists(dirPath))
					{
						Directory.Delete(dirPath.ToString(), recursive: true);
					}
				}
				if (photoProjectMasterInfo != null)
				{
					var item = CustomTableItemProvider.GetItems(photoProjectMaster).WhereEquals("SKUID", SKUID).FirstOrDefault();

					// Get Project Images using Project ID            
					if (photoProjectDetailInfo != null && item != null)
					{
						projectID = item.GetValue("ItemID", 0);
						ServiceID = item.GetValue("ServiceID", 0);

						if (projectID > 0 && ServiceID > 0)
						{
							//Getting Service Name From Enum
							ServiceName = getPrintingService(ServiceID);
							var photoSize = FillComboBox.GetPapaerSize(ServiceID);

							var imageUrls = CustomTableItemProvider.GetItems(photoProjectDetail).WhereEquals("ProjectID", projectID).Columns("ProjectID", "ImageUrl", "ItemID", "ImageSizeID", "NoOfCopy");
							foreach (CustomTableItem image in imageUrls)
							{
								// Upload Images to Azure
								path = image.GetValue("ImageUrl", "");

								string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
								downloadsPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadFilePath"] + "/");
								downloadsPath = downloadsPath + "/PrintForMe/" + OrderID.ToString() + "/" + ServiceName + "/";

								//Check whether Directory (Folder) exists.
								if (!Directory.Exists(downloadsPath))
								{
									//If Directory (Folder) does not exists. Create it.
									Directory.CreateDirectory(downloadsPath);
								}

								var size = photoSize.Where(x => x.ItemID == image.GetValue("ImageSizeID", 0)).Select(x => x.Code.Replace(x.Description, "")).FirstOrDefault();
								var paperMaterialName = paperMaterials.Where(x => x.ItemID == item.GetValue("PaperMaterialID", 0)).Select(x => x.Code).FirstOrDefault();
								string fileName = size.Replace(" ", "") + "_" + paperMaterialName.Replace(" ", "")
												  + "_" + image.GetValue("NoOfCopy", 0) + "_" + path.Split('/').Last();

								System.Net.HttpWebRequest request = null;
								System.Net.HttpWebResponse response = null;
								byte[] retBytes = null;

								request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(path);
								response = (System.Net.HttpWebResponse)request.GetResponse();
								if (request.HaveResponse)
								{
									if (response.StatusCode == System.Net.HttpStatusCode.OK)
									{
										System.IO.Stream receiveStream = response.GetResponseStream();
										using (System.IO.BinaryReader br = new System.IO.BinaryReader(receiveStream))
										{
											retBytes = br.ReadBytes(FileSizeinMB);
											br.Close();

											downloadsPath = downloadsPath + fileName;
											//Save the Byte Array as File
											System.IO.File.WriteAllBytes(downloadsPath, retBytes);
										}
									}
								}
							}


						}
					}
				}

				if (woodenProjectMasterInfo != null)
				{
					var item = CustomTableItemProvider.GetItems(woodenProjectMaster).WhereEquals("SKUID", SKUID).FirstOrDefault();

					// Get Project Images using Project ID            
					if (woodenProjectDetailInfo != null && item != null)
					{
						projectID = item.GetValue("ItemID", 0);
						ServiceID = item.GetValue("ServiceID", 0);
						if (projectID > 0 && ServiceID > 0)
						{
							var photoSize = FillComboBox.GetPapaerSize(ServiceID);
							//Getting Service Name From Enum
							ServiceName = getPrintingService(ServiceID);

							var imageUrls = CustomTableItemProvider.GetItems(woodenProjectDetail).WhereEquals("ProjectID", projectID);
							foreach (CustomTableItem image in imageUrls)
							{
								// Upload Images to Azure
								path = image.GetValue("ImageUrl", "");

								string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
								downloadsPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadFilePath"] + "/");
								downloadsPath = downloadsPath + "/PrintForMe/" + OrderID.ToString() + "/" + ServiceName + "/";

								//Check whether Directory (Folder) exists.
								if (!Directory.Exists(downloadsPath))
								{
									//If Directory (Folder) does not exists. Create it.
									Directory.CreateDirectory(downloadsPath);
								}

								var size = photoSize.Where(x => x.ItemID == image.GetValue("ImageSizeID", 0)).Select(x => x.Code.Replace(x.Description, "")).FirstOrDefault();
								var thickness = item.GetValue("PlankThickness", "");
								string fileName = size.Replace(" ", "") + "_" + thickness.Replace(" ", "")
												  + "_" + image.GetValue("NoOfCopy", 0) + "_" + path.Split('/').Last();

								System.Net.HttpWebRequest request = null;
								System.Net.HttpWebResponse response = null;
								byte[] retBytes = null;

								request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(path);
								response = (System.Net.HttpWebResponse)request.GetResponse();
								if (request.HaveResponse)
								{
									if (response.StatusCode == System.Net.HttpStatusCode.OK)
									{
										System.IO.Stream receiveStream = response.GetResponseStream();
										using (System.IO.BinaryReader br = new System.IO.BinaryReader(receiveStream))
										{
											retBytes = br.ReadBytes(FileSizeinMB);
											br.Close();

											downloadsPath = downloadsPath + fileName;
											//Save the Byte Array as File
											System.IO.File.WriteAllBytes(downloadsPath, retBytes);
										}
									}
								}
							}
						}
					}
				}

				if (wallPrintingProjectMasterInfo != null)
				{
					var item = CustomTableItemProvider.GetItems(wallPrintingProjectMaster).WhereEquals("SKUID", SKUID).FirstOrDefault();

					// Get Project Images using Project ID            
					if (wallPrintingProjectDetailInfo != null && item != null)
					{
						projectID = item.GetValue("ItemID", 0);
						ServiceID = item.GetValue("ServiceID", 0);
						if (projectID > 0 && ServiceID > 0)
						{
							//Getting Service Name From Enum
							ServiceName = getPrintingService(ServiceID);
							var photoSize = FillComboBox.GetPapaerSize(ServiceID);

							var imageUrls = CustomTableItemProvider.GetItems(wallPrintingProjectDetail).WhereEquals("ProjectID", projectID);
							foreach (CustomTableItem image in imageUrls)
							{
								// Upload Images to Azure
								path = image.GetValue("ImageUrl", "");

								string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
								downloadsPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadFilePath"] + "/");
								downloadsPath = downloadsPath + "/PrintForMe/" + OrderID.ToString() + "/" + ServiceName + "/";

								//Check whether Directory (Folder) exists.
								if (!Directory.Exists(downloadsPath))
								{
									//If Directory (Folder) does not exists. Create it.
									Directory.CreateDirectory(downloadsPath);
								}

								var size = photoSize.Where(x => x.ItemID == image.GetValue("ImageSizeID", 0)).Select(x => x.Code.Replace(x.Description, "")).FirstOrDefault();
								var paperMaterialName = paperMaterials.Where(x => x.ItemID == item.GetValue("PaperMaterialID", 0)).Select(x => x.Code).FirstOrDefault();
								var frameColor = frameColors.Where(x => x.ItemID == item.GetValue("FrameColorID", 0)).Select(x => x.Code).FirstOrDefault();
								var paintingSize = item.GetValue("PaintingSize", "");
								string fileName = size.Replace(" ", "") + "_" + paperMaterialName.Replace(" ", "")
												  + "_" + frameColor.Replace(" ", "") + "_" + paintingSize.Replace(" ", "")
												  + "_" + image.GetValue("NoOfCopy", 0) + "_" + path.Split('/').Last();

								System.Net.HttpWebRequest request = null;
								System.Net.HttpWebResponse response = null;
								byte[] retBytes = null;

								request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(path);
								response = (System.Net.HttpWebResponse)request.GetResponse();
								if (request.HaveResponse)
								{
									if (response.StatusCode == System.Net.HttpStatusCode.OK)
									{
										System.IO.Stream receiveStream = response.GetResponseStream();
										using (System.IO.BinaryReader br = new System.IO.BinaryReader(receiveStream))
										{
											retBytes = br.ReadBytes(FileSizeinMB);
											br.Close();

											downloadsPath = downloadsPath + fileName;
											//Save the Byte Array as File
											System.IO.File.WriteAllBytes(downloadsPath, retBytes);
										}
									}
								}
							}
						}
					}
				}

				if (album != null)
				{
					if (album.AlbumID != 0)
					{
						QueryDataParameters parameters = new QueryDataParameters();
						parameters.Add("@albumid", album.AlbumID);
						DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_getdownloadpdf", parameters, QueryTypeEnum.StoredProcedure);
						List<AlbumPagePhotos> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumPagePhotos
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

						int i = 1;
						foreach (var image in item)
						{
							// Upload Images to Azure
							if (!string.IsNullOrEmpty(image.PhotoLocation))
							{
								path = image.PhotoLocation;
							}
							else
							{
								path = "http://localhost:2537/Content/Images/No-Image.jpg";
							}

							string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
							downloadsPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadFilePath"] + "/");
							downloadsPath = downloadsPath + "/PrintForMe/" + OrderID.ToString() + "/Album/" + image.Row_num + "/";

							//Check whether Directory (Folder) exists.
							if (!Directory.Exists(downloadsPath))
							{
								//If Directory (Folder) does not exists. Create it.
								Directory.CreateDirectory(downloadsPath);
							}

							var layout = "";
							var imageposition = "";
							if (image.APLItemID == 1)
							{
								if (image.Sequence == 1)
								{
									imageposition = "Left-Side";
								}
								if (image.Sequence == 2)
								{
									imageposition = "Right-Side";
								}
								layout = "Layout-1";
							}
							if (image.APLItemID == 2)
							{
								if (image.Sequence == 1)
								{
									imageposition = "Center";
								}
								layout = "Layout-2";
							}
							if (image.APLItemID == 3)
							{
								if (image.Sequence == 1)
								{
									imageposition = "Top-Side";
								}
								if (image.Sequence == 2)
								{
									imageposition = "Bottom-Side";
								}
								layout = "Layout-3";
							}
							if (image.APLItemID == 4)
							{
								if (image.Sequence == 1)
								{
									imageposition = "Top-Side";
								}
								if (image.Sequence == 2)
								{
									imageposition = "Bottom-Left";
								}
								if (image.Sequence == 3)
								{
									imageposition = "Bottom-Right";
								}
								layout = "Layout-4";
							}
							if (image.APLItemID == 5)
							{
								if (image.Sequence == 1)
								{
									imageposition = "Top-Left";
								}
								if (image.Sequence == 2)
								{
									imageposition = "Top-Right";
								}
								if (image.Sequence == 3)
								{
									imageposition = "Bottom-Left";
								}
								if (image.Sequence == 4)
								{
									imageposition = "Bottom-Right";
								}
								layout = "Layout-5";
							}
							if (image.APLItemID == 6)
							{
								if (image.Sequence == 1)
								{
									imageposition = "Top-Left";
								}
								if (image.Sequence == 2)
								{
									imageposition = "Top-Right";
								}
								if (image.Sequence == 3)
								{
									imageposition = "Middle-Left";
								}
								if (image.Sequence == 4)
								{
									imageposition = "Middle-Right";
								}
								if (image.Sequence == 5)
								{
									imageposition = "Bottom-Left";
								}
								if (image.Sequence == 6)
								{
									imageposition = "Bottom-Right";
								}
								layout = "Layout-6";
							}
							var PaperSize = album.AlbumSize;
							var papermaterial = album.AlbumPageType;
							var NoofPages = album.AlbumPageCountCode;
							string fileName = PaperSize.Replace(" ", "") + "_" + papermaterial.Replace(" ", "")
												  + "_" + layout + "_" + imageposition + "_" + path.Split('/').Last();

							System.Net.HttpWebRequest request = null;
							System.Net.HttpWebResponse response = null;
							byte[] retBytes = null;

							request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(path);

							response = (System.Net.HttpWebResponse)request.GetResponse();
							if (request.HaveResponse)
							{
								if (response.StatusCode == System.Net.HttpStatusCode.OK)
								{
									System.IO.Stream receiveStream = response.GetResponseStream();
									using (System.IO.BinaryReader br = new System.IO.BinaryReader(receiveStream))
									{
										retBytes = br.ReadBytes(FileSizeinMB);
										br.Close();

										downloadsPath = downloadsPath + fileName;
										//Save the Byte Array as File
										System.IO.File.WriteAllBytes(downloadsPath, retBytes);
									}
								}
							}
							i++;
						}
					}
				}

				using (ZipFile zip = new ZipFile())
				{
					zip.AlternateEncodingUsage = ZipOption.AsNecessary;
					var pathName = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadFilePath"]);

					if (Directory.Exists(pathName))
					{
						zip.AddDirectory(pathName);

						string zipName = String.Format("Documents_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd"));
						zip.Save(pathName + "\\" + zipName);


						return Json("TempImg/Images/" + zipName);
					}

				}


				return Json("");

			}
			catch (Exception ex)
			{
				return Json(ex.Message);
			}
		}

		public ActionResult GetOrderDetail(int orderID)
        {
            if (orderID < 0)
            {
                return HttpNotFound();
            }

            var order = OrderInfoProvider.GetOrderInfo(orderID);

            if (order == null)
            {
                return HttpNotFound();
            }
            var currency = CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID);

            return View("AdminOrderDetail", new OrderDetailViewModel(currency.CurrencyFormatString)
            {
                OrderDate = order.OrderDate,
                OrderID = order.OrderID,
                InvoiceNumber = order.OrderInvoiceNumber,
                TotalPrice = order.OrderTotalPrice,
                StatusName = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusDisplayName,
                OrderAddress = new OrderAddressViewModel(order.OrderBillingAddress),
                PaymentOption = PaymentOptionInfoProvider.GetPaymentOptionInfo(order.OrderPaymentOptionID)?.PaymentOptionDisplayName,
                OrderNote = order.OrderNote,
                OrderGiftWrapping = order.GetValue("OrderGiftWrapping", false),
                OrderGrandTotal = order.OrderGrandTotal,
                OrderTotalTax = order.OrderTotalTax,
                OrderTotalShipping = order.OrderTotalShipping,
                OrderItems = OrderItemInfoProvider.GetOrderItems(order.OrderID).Select(orderItem =>
                {
                    return new OrderItemViewModel
                    {
                        SKUID = orderItem.OrderItemSKUID,
                        SKUName = orderItem.OrderItemSKUName,
                        SKUSize = SKUInfoProvider.GetSKUInfo(orderItem.OrderItemSKUID) != null ?
                        SKUInfoProvider.GetSKUInfo(orderItem.OrderItemSKUID).SKUNumber : string.Empty,
                        SKUImagePath = orderItem.OrderItemSKU.SKUImagePath,
                        TotalPriceInMainCurrency = orderItem.OrderItemTotalPriceInMainCurrency,
                        UnitCount = orderItem.OrderItemUnitCount,
                        UnitPrice = orderItem.OrderItemUnitPrice,
                        serviceDetail = ServiceInformation.GetProjectInformation(orderItem.OrderItemSKUID, "", true)
                    };
                })
            });
        }

        [HttpPost]
        public ActionResult UpdateOrder(OrdersListViewModel order)
        {
            if (order != null)
            {
                try
                {
                    var existingOrder = OrderInfoProvider.GetOrderInfo(order.OrderID);
                    //existingOrder.OrderInvoiceNumber = order.OrderInvoiceNumber;
                    //existingOrder.OrderDate = DateTime.ParseExact(order.DisplayOrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    existingOrder.OrderTrackingNumber = order.OrderTrackingNumber;
                    //existingOrder.OrderTotalPrice = order.TotalPrice;
                    existingOrder.OrderStatusID = order.StatusID != null ? order.StatusID.Value : existingOrder.OrderStatusID;
                    existingOrder.Update();
                }
                catch (Exception ex)
                {
                    return Json("Plese try again , error occured!");
                }
            }

            return Json("Order Updated Sucessfully");

        }

        public String getPrintingService(int PrintingServiceID)
        {
            string printingService = Enum.GetName(typeof(PrintingService), PrintingServiceID);
            return printingService;
        }

        [HttpPost]
        public ActionResult UpdateOrderFromGrid(int OrderID, String StatusName)
        {
            if (OrderID != 0)
            {
                try
                {
                    var statuses = OrderStatusInfoProvider.GetOrderStatuses();
                    var statusFinal = statuses.Where(x => x.StatusName.Trim() == StatusName.Trim()).FirstOrDefault();
                    if (statusFinal != null)
                    {
                        var existingOrder = OrderInfoProvider.GetOrderInfo(OrderID);
                        existingOrder.OrderStatusID = statusFinal.StatusID;
                        existingOrder.Update();
                        return Json(existingOrder);
                    }
                }
                catch (Exception ex)
                {
                    return Json("Plese try again , error occured!");
                }
            }
            return Json(null);
        }

        public ActionResult UpdateOrderFromGrid(OrderListSettingModel value)
        {
            //update record
            try
            {
                var existingOrder = OrderInfoProvider.GetOrderInfo(value.OrderID);
                existingOrder.OrderID = value.OrderID;
                existingOrder.OrderDate = value.OrderDate;
                existingOrder.OrderStatusID = (int)value.StatusID;
                existingOrder.OrderTotalPrice = Convert.ToDecimal(value.FormattedTotalPrice);
                existingOrder.Update();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Json(value, JsonRequestBehavior.AllowGet);
        }
    }

    public class StatusList
    {
        [JsonProperty("StatusID")]
        public int StatusID { get; set; }

        [JsonProperty("StatusName")]
        public string StatusName { get; set; }
    }
}