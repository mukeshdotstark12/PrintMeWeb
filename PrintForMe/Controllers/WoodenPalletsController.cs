using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using Microsoft.AspNet.Identity;
using PrintForMe.Helpers;
using PrintForMe.Models.Services;
using PrintForMe.Models.Store;
using PrintForMe.Models.WoodenPallets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PrintForMe.Controllers
{
    public class WoodenPalletsController : Controller
    {
        // GET: Services
        public ActionResult WoodenService(int serviceId)
        {
            //Check pending project
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string woodenPalletsMaster = "PrintForme.WoodenPalletsMaster";

            // Gets the custom table
            DataClassInfo woodenPalletMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenPalletsMaster);
            if (woodenPalletMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(woodenPalletsMaster)
                             .WhereEquals("UserID", User.Identity.GetUserId())
                             .WhereEquals("IsComplete", false)
                             .Columns("ItemID", "SKUID").ToList();

                if (items != null && items.Count > 0)
                {
                    return RedirectToAction("EditWoodenProject",
                        new RouteValueDictionary(new
                        {
                            controller = "WoodenPallets",
                            action = "EditWoodenProject",
                            projectId = ValidationHelper.GetInteger(items[0].GetValue("ItemID"), 0),
                            SKUId = ValidationHelper.GetInteger(items[0].GetValue("SKUID"), 0)
                        }));
                }
            }

            // Prepares the code name (class name) of the custom table
            string serviceSettingsClassName = "PrintForme.ServiceSettings";
            int minimumPrice = 0;

            // Gets the custom table
            DataClassInfo serviceSettings = DataClassInfoProvider.GetDataClassInfo(serviceSettingsClassName);
            if (serviceSettings != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(serviceSettingsClassName)
                    .WhereEquals("ServiceID", serviceId)
                    .Columns("Code", "Price").ToList();


                // Creates a collection of view models based on the menu item and page data
                var photoSettingModel = items.Select(item => new ServiceSettingModel()
                {
                    Code = ValidationHelper.GetString(item.GetValue("Code"), ""),
                    Price = ValidationHelper.GetString(item.GetValue("Price"), ""),
                });

                if (photoSettingModel != null && photoSettingModel.Count() > 0)
                {
                    minimumPrice = photoSettingModel.Min(p => ValidationHelper.GetInteger(p.Price, 0));
                }
            }


            var model = new PhotoServiceModel()
            {
                MinimumPrice = minimumPrice,
                Size = FillComboBox.GetPapaerSize(serviceId),
                ServiceID = serviceId
            };

            return View(model);

        }

        public ActionResult AddWoodenProject(PhotoServiceModel photoService)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            //Check pending project
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string woodenPalletsProjectMaster = "PrintForme.WoodenPalletsMaster";

            // Gets the custom table
            DataClassInfo woodenPalletsProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenPalletsProjectMaster);
            if (woodenPalletsProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(woodenPalletsProjectMaster)
                             .WhereEquals("UserID", User.Identity.GetUserId())
                             .WhereEquals("IsComplete", false)
                             .Columns("ItemID").ToList();

                if (items != null && items.Count > 0)
                {
                    return RedirectToAction("EditWoodenProject",
                       new RouteValueDictionary(new
                       {
                           controller = "WoodenPallets",
                           action = "EditWoodenProject",
                           projectId = ValidationHelper.GetInteger(items[0].GetValue("ItemID"), 0)
                       }));
                }
            }

            var model = new AddWoodenProject()
            {
                PlankThickness = photoService.PlankThickness,
                SizeID = photoService.SelectedSize,
                Size = FillComboBox.GetPapaerSize(photoService.ServiceID),
                ServiceID = photoService.ServiceID
            };

            string path = Server.MapPath("~/PrintForMe/WoodenProject/" + User.Identity.GetUserName());
            //var path = Path.GetTempPath() + "PrintForMe/WoodenProject/" + User.Identity.GetUserName();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] filePaths = Directory.GetFiles(path);
            string removePath = Server.MapPath("~/PrintForMe/WoodenProject/");

            for (int i = 0; i <= filePaths.Length - 1; i++)
            {
                filePaths[i] = filePaths[i].Replace(removePath, "/");
                filePaths[i] = filePaths[i].Replace("\\", "/");
            }

            //AddPhotoProject model = new AddPhotoProject();
            var modeldetail = filePaths.Select(item => new AddWoodenProjectDetail()
            {
                //ImageToString = Convert.ToBase64String(ImageToByteArray(System.Drawing.Image.FromFile(item))),
                ImageUrl = item,
                NoOfCopy = 1,
                Price = model.Size.Where(x => x.ItemID == model.SizeID).Select(x => x.ProductPrice).FirstOrDefault().ToString(),
                IsLowResolution = ImageResolution("~/PrintForMe/WoodenProject/"+item) //ImageResolution(removePath.Replace("\\", "/") + item)
            });

            model.AddWoodenProjectDetails.AddRange(modeldetail);

            return View(model);

        }

        [HttpPost]
        public ActionResult AddWoodenProject(AddWoodenProject addWoodenProject, string addToCart)
        {
            addWoodenProject.Size = FillComboBox.GetPapaerSize(addWoodenProject.ServiceID);

            if (!ModelState.IsValid || addWoodenProject.AddWoodenProjectDetails.Count == 0)
            {
                return View(addWoodenProject);
            }

            //if (addPhotoProject.AddPhotoProjectDetails.Where(x => x.IsLowResolution).Count() > 0)
            //{                
            //    return View(addPhotoProject);
            //}

            // Prepares the code name (class name) of the custom table to which the data record will be added
            string woodenPalletsProjectMaster = "PrintForme.WoodenPalletsMaster";

            // Gets the custom table
            DataClassInfo woodenPalletsProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenPalletsProjectMaster);
            if (woodenPalletsProjectMasterInfo != null)
            {
                if (addWoodenProject.ProjectID > 0)
                {
                    string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";

                    // Gets the custom table
                    DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
                    if (woodenProjectDetailInfo != null)
                    {
                        foreach (AddWoodenProjectDetail addWoodenProjectDetail in addWoodenProject.AddWoodenProjectDetails)
                        {
                            // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                            var woodenProjectDetailData = CustomTableItemProvider.GetItems(woodenProjectDetail)
                                                                                .WhereEquals("ItemID", addWoodenProjectDetail.ItemID)
                                                                                .FirstOrDefault();

                            if (woodenProjectDetailData != null && !addWoodenProjectDetail.IsLowResolution)
                            {
                                // Sets the values for the fields of the custom table (ItemText in this case)                                
                                woodenProjectDetailData.SetValue("ImageUrl", addWoodenProjectDetail.ImageUrl);
                                woodenProjectDetailData.SetValue("NoOfCopy", addWoodenProjectDetail.NoOfCopy);
                                woodenProjectDetailData.SetValue("ImageSizeID", addWoodenProjectDetail.SizeID);

                                // Saves the changes to the database
                                woodenProjectDetailData.Update();
                            }
                        }

                        //UpdateSKU
                        PriceCalculation(addWoodenProject.ProjectID, addWoodenProject.SKUID, "", addWoodenProject.ServiceID);
                    }
                }
                else
                {
                    // create SKU
                    addWoodenProject.SKUID = ManageSKU(0, 0, addWoodenProject.AddWoodenProjectDetails.Count > 0 ? addWoodenProject.AddWoodenProjectDetails[0].ImageUrl : "");

                    // Creates a new custom table item
                    CustomTableItem newWoodenProjectMaster = CustomTableItem.New(woodenPalletsProjectMaster);

                    // Sets the values for the fields of the custom table (ItemText in this case)
                    newWoodenProjectMaster.SetValue("PlankThickness", ValidationHelper.GetString(addWoodenProject.PlankThickness, ""));
                    newWoodenProjectMaster.SetValue("IsComplete", false);
                    newWoodenProjectMaster.SetValue("UserID", User.Identity.GetUserId());
                    newWoodenProjectMaster.SetValue("ProjectDate", DateTime.Now);
                    newWoodenProjectMaster.SetValue("SKUID", addWoodenProject.SKUID);
                    newWoodenProjectMaster.SetValue("ServiceID", addWoodenProject.ServiceID);

                    // Save the new custom table record into the database
                    newWoodenProjectMaster.Insert();

                    addWoodenProject.ProjectID = newWoodenProjectMaster.ItemID;

                    if (ValidationHelper.GetInteger(addWoodenProject.ProjectID, 0) != 0)
                    {
                        string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";

                        // Gets the custom table
                        DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
                        if (woodenProjectDetailInfo != null)
                        {
                            foreach (AddWoodenProjectDetail addWoodenProjectDetail in addWoodenProject.AddWoodenProjectDetails)
                            {
                                if (!addWoodenProjectDetail.IsLowResolution)
                                {
                                    // Creates a new custom table item
                                    CustomTableItem newWoodenProjectDetail = CustomTableItem.New(woodenProjectDetail);

                                    // Sets the values for the fields of the custom table (ItemText in this case)
                                    newWoodenProjectDetail.SetValue("ProjectID", addWoodenProject.ProjectID);
                                    newWoodenProjectDetail.SetValue("ImageUrl", addWoodenProjectDetail.ImageUrl);
                                    newWoodenProjectDetail.SetValue("NoOfCopy", addWoodenProjectDetail.NoOfCopy);
                                    newWoodenProjectDetail.SetValue("ImageSizeID", addWoodenProjectDetail.SizeID);

                                    // Save the new custom table record into the database
                                    newWoodenProjectDetail.Insert();

                                    addWoodenProjectDetail.ItemID = newWoodenProjectDetail.ItemID;
                                    addWoodenProjectDetail.ProjectID = addWoodenProject.ProjectID;
                                }
                            }
                        }
                        //UpdateSKU
                        PriceCalculation(addWoodenProject.ProjectID, addWoodenProject.SKUID, "", addWoodenProject.ServiceID);
                    }
                }
            }

            if (!string.IsNullOrEmpty(addToCart))
            {
                return RedirectToAction("AddItem",
                           new RouteValueDictionary(new
                           {
                               controller = "Checkout",
                               action = "AddItem",
                               itemSkuId = addWoodenProject.SKUID
                           }));
            }
            else
            {
                return RedirectToAction("EditWoodenProject",
                           new RouteValueDictionary(new
                           {
                               controller = "WoodenPallets",
                               action = "EditWoodenProject",
                               projectId = addWoodenProject.ProjectID
                           }));
            }
            //return View(addPhotoProject);

        }

        public ActionResult EditWoodenProject(int? projectId, int? SKUId)
        {
            AddWoodenProject addWoodenProject = new AddWoodenProject();
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";

            // Gets the custom table
            DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            if (woodenProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(woodenProjectMaster)
                             .WhereEquals("ItemID", projectId)
                             .Or()
                             .WhereEquals("SKUID", SKUId)
                             .Columns("ItemID", "PlankThickness", "SKUID", "ServiceID").ToList();

                if (items != null && items.Count > 0)
                {
                    addWoodenProject.PlankThickness = ValidationHelper.GetString(items[0].GetValue("PlankThickness"), "");
                    addWoodenProject.Size = FillComboBox.GetPapaerSize(ValidationHelper.GetInteger(items[0].GetValue("ServiceID"), 0));
                    addWoodenProject.SKUID = ValidationHelper.GetInteger(items[0].GetValue("SKUID"), 0);
                    addWoodenProject.ProjectID = ValidationHelper.GetInteger(items[0].GetValue("ItemID"), 0);
                    addWoodenProject.ServiceID = ValidationHelper.GetInteger(items[0].GetValue("ServiceID"), 0);

                    if (addWoodenProject.ProjectID > 0)
                    {
                        string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";

                        // Gets the custom table
                        DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
                        if (woodenProjectDetailInfo != null)
                        {
                            string removePath = Server.MapPath("~/PrintForMe/WoodenProject/").Replace("\\", "/");

                            // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                            List<CustomTableItem> detailItems = CustomTableItemProvider.GetItems(woodenProjectDetail)
                                .WhereEquals("ProjectID", addWoodenProject.ProjectID)
                                .Columns("ItemID", "ProjectID", "ImageUrl", "NoOfCopy", "ImageSizeID").ToList();

                            // Creates a collection of view models based on the menu item and page data
                            var woodenProjectDetailData = detailItems.Select(item => new AddWoodenProjectDetail()
                            {
                                ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                                ImageUrl = ValidationHelper.GetString(item.GetValue("ImageUrl"), "").Replace(removePath, "/"),
                                IsLowResolution = ImageResolution(ValidationHelper.GetString(item.GetValue("ImageUrl"), "")),
                                NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0),
                                SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                                ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                                Price = addWoodenProject.Size.Where(x => x.ItemID == ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0)).Select(x => x.ProductPrice * ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)).FirstOrDefault().ToString(),
                                //ImageToString = System.IO.File.Exists(ValidationHelper.GetString(item.GetValue("ImageUrl"), ""))
                                //                ? Convert.ToBase64String(ImageToByteArray(System.Drawing.Image.FromFile(ValidationHelper.GetString(item.GetValue("ImageUrl"), ""))))
                                //                : deletePhotodetail(ValidationHelper.GetInteger(item.GetValue("ItemID"), 0), addWoodenProject.SKUID)
                            });

                            if (woodenProjectDetailData != null)
                            {
                                addWoodenProject.AddWoodenProjectDetails.AddRange(woodenProjectDetailData.Where(x => x.ImageUrl != ""));
                            }
                        }
                    }
                }
            }

            if (ValidationHelper.GetInteger(SKUId, 0) > 0)
            {
                ViewBag.isEditProject = true;
            }
            else
            {
                ViewBag.isEditProject = false;
            }
            return View(addWoodenProject);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {
                string path = Server.MapPath("~/PrintForMe/WoodenProject/" + User.Identity.GetUserName());
                //var path = Path.GetTempPath() + "/PrintForMe/WoodenProject/" + User.Identity.GetUserName();

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    var file = Request.Files[i];
                    string fileName;

                    if (Request.Browser.Browser.ToUpper() == "IE")
                    {
                        string[] selectedFiles = file.FileName.Split(new char[] { '\\' });
                        fileName = selectedFiles[selectedFiles.Length - 1];
                    }
                    else
                    {
                        fileName = file.FileName;
                    }
                    string newFilename = Guid.NewGuid().ToString();
                    FileInfo fInfo = new FileInfo(fileName);
                    newFilename = string.Format("{0}{1}", newFilename, fInfo.Extension);
                    fileName = Path.Combine(path, newFilename).Replace("\\", "/").Replace("//", "/");
                    file.SaveAs(fileName);
                    var projectId = Request.Params["id"];

                    if (!string.IsNullOrEmpty(projectId))
                    {
                        string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";

                        // Gets the custom table
                        DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
                        if (woodenProjectDetailInfo != null)
                        {
                            // Creates a new custom table item
                            CustomTableItem newWoodenProjectDetail = CustomTableItem.New(woodenProjectDetail);

                            // Sets the values for the fields of the custom table (ItemText in this case)
                            newWoodenProjectDetail.SetValue("ProjectID", projectId);
                            newWoodenProjectDetail.SetValue("ImageUrl", fileName);
                            newWoodenProjectDetail.SetValue("NoOfCopy", 1);
                            newWoodenProjectDetail.SetValue("ImageSizeID", 0);

                            // Save the new custom table record into the database
                            newWoodenProjectDetail.Insert();
                        }
                    }
                }

                //return RedirectToAction("AddPhotoProject", "Services");
                return Json("Upload", JsonRequestBehavior.AllowGet);
            }
            return Json("Please Select Valid Images");
        }

        public ActionResult DeletePhoto(string imageUrl, int itemId, int SKUID)
        {
            try
            {
                if (ValidationHelper.GetInteger(itemId, 0) > 0)
                {
                    //delete photo detail and update sku price
                    deletePhotodetail(itemId, SKUID);
                }

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    string path = Server.MapPath("~/PrintForMe/WoodenProject/" + imageUrl);
                    if (System.IO.File.Exists(path))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(path);
                    }
                }

                return Json("Upload", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Service", "Delete", ex);
                return Json("Upload", JsonRequestBehavior.AllowGet);
            }
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public bool ImageResolution(string path)
        {
            //float dx;
            //float dy;
            //System.Drawing.Image imageFile1 = System.Drawing.Image.FromFile(path);
            //Graphics g = Graphics.FromImage(imageFile1);

            //dx = g.DpiX; // // selected file DPI
            //dy = g.DpiY;
            ////g.Dispose();

            if (System.IO.File.Exists(path))
            {
                var OriginalBitmap = new Bitmap(path);
                if (OriginalBitmap.Width < 200 || OriginalBitmap.Height < 200)
                {
                    return true;
                }
            }
            return false;
        }

        public int ManageSKU(double Price, int SKUID, string SKUImagePath)
        {
            // Gets the product
            SKUInfo updateProduct = SKUInfoProvider.GetSKUs()
                                                .WhereEquals("SKUID", SKUID)
                                                .TopN(1)
                                                .FirstOrDefault();

            if (updateProduct != null)
            {
                // Updates the product properties
                updateProduct.SKUPrice = ValidationHelper.GetDecimal(Price, 0);
                updateProduct.SKUImagePath = updateProduct.SKUImagePath == SKUImagePath ? "" : updateProduct.SKUImagePath;

                // Saves the changes to the database
                SKUInfoProvider.SetSKUInfo(updateProduct);

                return updateProduct.SKUID;
            }
            else
            {
                // Gets a department
                DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("PhotoPrinting", SiteContext.CurrentSiteName);

                // Creates a new product object
                SKUInfo newProduct = new SKUInfo();

                // Sets the product properties
                newProduct.SKUName = "Wooden Project";
                newProduct.SKUPrice = ValidationHelper.GetDecimal(Price, 0);
                newProduct.SKUImagePath = SKUImagePath;
                newProduct.SKUEnabled = true;
                if (department != null)
                {
                    newProduct.SKUDepartmentID = department.DepartmentID;
                }
                newProduct.SKUSiteID = SiteContext.CurrentSiteID;

                // Saves the product to the database
                // Note: Only creates the SKU object. You also need to create a connected product page to add the product to the site.
                SKUInfoProvider.SetSKUInfo(newProduct);

                // Gets a TreeProvider instance
                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

                // Gets the parent page
                TreeNode parent = tree.SelectNodes("PrintForMe.Page")
                    .Path("/Store/Products")
                    .OnCurrentSite()
                    .TopN(1)
                    .FirstOrDefault();

                if ((parent != null) && (newProduct != null))
                {
                    // Creates a new product page of the 'CMS.Product' type
                    SKUTreeNode node = (SKUTreeNode)TreeNode.New("PrintForMe.Products", tree);

                    // Sets the product page properties
                    node.DocumentCulture = LocalizationContext.PreferredCultureCode;
                    var name = newProduct.SKUName;
                    node.DocumentName = name;
                    // Synchronize SKU name with document name in a default culture
                    node.DocumentSKUName = name;

                    //// Sets a value for a field of the given product page type
                    node.SetValue("Name", name);

                    // Assigns the product to the page
                    node.NodeSKUID = newProduct.SKUID;

                    // Saves the product page to the database
                    node.Insert(parent);
                }

                return newProduct.SKUID;
            }
        }

        private int PriceCalculation(int projectID, int SKUID, string imageUrl, int serviceId)
        {
            var sizeSettings = FillComboBox.GetPapaerSize(serviceId);
            double projectPrice = 0;

            if (ValidationHelper.GetInteger(projectID, 0) > 0)
            {
                string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";

                // Gets the custom table
                DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
                if (woodenProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(woodenProjectDetail)
                    .WhereEquals("ProjectID", projectID)
                    .Columns("ProjectID", "ItemID", "ImageSizeID", "NoOfCopy").ToList();


                    // Creates a collection of view models based on the menu item and page data
                    var woodenServiceDetails = items.Select(item => new AddWoodenProjectDetail()
                    {
                        ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                        ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                        SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                        NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)
                    });

                    foreach (AddWoodenProjectDetail woodenServiceDetail in woodenServiceDetails)
                    {
                        var item = sizeSettings.FirstOrDefault(s => s.ItemID == woodenServiceDetail.SizeID);
                        projectPrice += ValidationHelper.GetDouble(item != null ? item.ProductPrice * woodenServiceDetail.NoOfCopy : 0, 0);
                    }

                    SKUID = ManageSKU(projectPrice, SKUID, imageUrl);
                }
            }

            return SKUID;
        }

        private string deletePhotodetail(int itemId, int SKUID)
        {
            if (ValidationHelper.GetInteger(itemId, 0) > 0)
            {
                string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
                string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";

                // Gets the custom table
                DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
                if (woodenProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    var woodenProjectDetailData = CustomTableItemProvider.GetItems(woodenProjectDetail)
                                                                        .WhereEquals("ItemID", itemId)
                                                                        .FirstOrDefault();

                    if (woodenProjectDetailData != null)
                    {
                        // Deletes the custom table record from the database
                        woodenProjectDetailData.Delete();

                        if (ValidationHelper.GetInteger(SKUID, 0) > 0)
                        {
                            // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                            CustomTableItem item = CustomTableItemProvider.GetItems(woodenProjectMaster)
                                         .WhereEquals("SKUID", SKUID)
                                         .Columns("ItemID", "SKUID", "ServiceID").FirstOrDefault();
                            //UpdateSKU
                            PriceCalculation(ValidationHelper.GetInteger(woodenProjectDetailData.GetValue("ProjectID"), 0), SKUID, "", ValidationHelper.GetInteger(item.GetValue("ServiceID"), 0));
                        }
                    }
                }
            }
            return "";
        }

        [HttpPost]
        public ActionResult GetPrice(int sizeId, int serviceId, int qty)
        {
            var size = FillComboBox.GetPapaerSize(serviceId);
            if (sizeId > 0)
            {
                try
                {
                    var price = size.Where(x => x.ItemID == sizeId).Select(x => x.ProductPrice).FirstOrDefault() * qty;
                    return Json(price);
                }
                catch (Exception ex)
                {
                    return Json("Undefine");
                }
            }

            return Json("Undefine");
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadCroppedFiles()
        {
            if (Request.Files.Count > 0)
            {
                var imageName = Server.MapPath(Request.Form["imageName"].ToString());
                var imageS = Request.Files;
                for (int i = 0; i < imageS.Count; i++)
                {
                    var file = imageS[i];
                    if (System.IO.File.Exists(imageName))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(imageName);
                    }
                    file.SaveAs(imageName);
                }
                return Json("Your files uploaded successfully", JsonRequestBehavior.AllowGet);
            }
            return Json("Please Select Valid Images", JsonRequestBehavior.AllowGet);
        }
    }
}