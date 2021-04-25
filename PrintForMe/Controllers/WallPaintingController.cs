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
using PrintForMe.Models.WallPainting;
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
    public class WallPaintingController : Controller
    {
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        // Prepares the code name (class name) of the custom table to which the data record will be added
        string wallProjectMaster = "PrintForme.WallPrintingProjectMaster";
        string wallProjectDetail = "PrintForme.WallPrintingProjectDetail";

        // GET: Services
        public ActionResult WallPaintingService(int serviceId)
        {
            //Check pending project                                
            // Gets the custom table
            DataClassInfo wallProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectMaster);

            if (wallProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallProjectMaster)
                             .WhereEquals("UserID", User.Identity.GetUserId())
                             .WhereEquals("IsComplete", false)
                             .Columns("ItemID", "SKUID").ToList();

                if (items != null && items.Count > 0)
                {
                    return RedirectToAction("EditWallPaintingProject",
                       new RouteValueDictionary(new
                       {
                           controller = "WallPainting",
                           action = "EditWallPaintingProject",
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
                PaperMaterial = FillComboBox.GetPapaerMaterial(),
                FrameColor = FillComboBox.GetFrameColor(),
                Size = FillComboBox.GetPapaerSize(serviceId),
                ServiceID = serviceId
            };

            return View(model);

        }

        public ActionResult AddWallPaintingProject(PhotoServiceModel photoService)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            //Check pending project          
            // Gets the custom table
            DataClassInfo wallProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectMaster);
            if (wallProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallProjectMaster)
                             .WhereEquals("UserID", User.Identity.GetUserId())
                             .WhereEquals("IsComplete", false)
                             .Columns("ItemID", "PaperMaterialID").ToList();

                if (items != null && items.Count > 0)
                {
                    return RedirectToAction("EditWallPaintingProject",
                       new RouteValueDictionary(new
                       {
                           controller = "WallPainting",
                           action = "EditWallPaintingProject",
                           projectId = ValidationHelper.GetInteger(items[0].GetValue("ItemID"), 0)
                       }));
                }
            }

            var model = new AddWallPaintingProject()
            {
                PaperMaterialID = photoService.SelectedPaperMaterial,
                PaintingSize = photoService.PaintingSize,
                FrameColorID = photoService.SelectedFrameColor,
                PaperMaterial = FillComboBox.GetPapaerMaterial(),
                FrameColor = FillComboBox.GetFrameColor(),
                ServiceID = photoService.ServiceID,
                Size = FillComboBox.GetPapaerSize(photoService.ServiceID),
                SizeID = photoService.SelectedSize
            };

            string path = Server.MapPath("~/PrintForMe/WallPaintingProject/" + User.Identity.GetUserName());
            //var path = Path.GetTempPath() + "PrintForMe/WallPaintingProject/" + User.Identity.GetUserName();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] filePaths = Directory.GetFiles(path);
            string removePath = Server.MapPath("~/PrintForMe/WallPaintingProject/");

            for (int i = 0; i <= filePaths.Length - 1; i++)
            {
                filePaths[i] = filePaths[i].Replace(removePath, "/");
                filePaths[i] = filePaths[i].Replace("\\", "/");
            }

            var modeldetail = filePaths.Select(item => new AddWallPaintingProjectDetail()
            {
                //ImageToString = Convert.ToBase64String(ImageToByteArray(System.Drawing.Image.FromFile(item))),
                ImageUrl = item,
                NoOfCopy = 1,
                Price = model.Size.Where(x => x.ItemID == model.SizeID).Select(x => x.ProductPrice).FirstOrDefault().ToString(),
                IsLowResolution = ImageResolution("~/PrintForMe/WallPaintingProject/" + item) //ImageResolution(removePath.Replace("\\", "/") + item)
            });

            model.AddWallPaintingProjectDetails.AddRange(modeldetail);

            return View(model);

        }

        [HttpPost]
        public ActionResult AddWallPaintingProject(AddWallPaintingProject addWallPaintingProject, string addToCart)
        {
            addWallPaintingProject.PaperMaterial = FillComboBox.GetPapaerMaterial();
            addWallPaintingProject.FrameColor = FillComboBox.GetFrameColor();
            addWallPaintingProject.Size = FillComboBox.GetPapaerSize(addWallPaintingProject.ServiceID);

            if (!ModelState.IsValid || addWallPaintingProject.AddWallPaintingProjectDetails.Count == 0)
            {
                return View(addWallPaintingProject);
            }

            // Gets the custom table
            DataClassInfo wallProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectMaster);
            if (wallProjectMasterInfo != null)
            {
                if (addWallPaintingProject.ProjectID > 0)
                {
                    // Gets the custom table
                    DataClassInfo wallProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectDetail);
                    if (wallProjectDetailInfo != null)
                    {
                        foreach (AddWallPaintingProjectDetail addWallProjectDetail in addWallPaintingProject.AddWallPaintingProjectDetails)
                        {
                            // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                            var wallProjectDetailData = CustomTableItemProvider.GetItems(wallProjectDetail)
                                                                                .WhereEquals("ItemID", addWallProjectDetail.ItemID)
                                                                                .FirstOrDefault();

                            if (wallProjectDetailData != null && !addWallProjectDetail.IsLowResolution)
                            {
                                // Sets the values for the fields of the custom table (ItemText in this case)                                
                                wallProjectDetailData.SetValue("ImageUrl", addWallProjectDetail.ImageUrl);
                                wallProjectDetailData.SetValue("NoOfCopy", addWallProjectDetail.NoOfCopy);
                                wallProjectDetailData.SetValue("ImageSizeID", addWallProjectDetail.SizeID);

                                // Saves the changes to the database
                                wallProjectDetailData.Update();
                            }
                        }

                        //UpdateSKU
                        PriceCalculation(addWallPaintingProject.ProjectID, addWallPaintingProject.SKUID, "", addWallPaintingProject.ServiceID);
                    }
                }
                else
                {
                    // create SKU
                    addWallPaintingProject.SKUID = ManageSKU(0, 0, addWallPaintingProject.AddWallPaintingProjectDetails.Count > 0 ? addWallPaintingProject.AddWallPaintingProjectDetails[0].ImageUrl : "");

                    // Creates a new custom table item
                    CustomTableItem newWallProjectMaster = CustomTableItem.New(wallProjectMaster);

                    // Sets the values for the fields of the custom table (ItemText in this case)
                    newWallProjectMaster.SetValue("PaperMaterialID", addWallPaintingProject.PaperMaterialID);
                    newWallProjectMaster.SetValue("FrameColorID", addWallPaintingProject.FrameColorID);
                    newWallProjectMaster.SetValue("PaintingSize", ValidationHelper.GetString(addWallPaintingProject.PaintingSize, ""));
                    newWallProjectMaster.SetValue("IsComplete", false);
                    newWallProjectMaster.SetValue("UserID", User.Identity.GetUserId());
                    newWallProjectMaster.SetValue("ProjectDate", DateTime.Now);
                    newWallProjectMaster.SetValue("SKUID", addWallPaintingProject.SKUID);
                    newWallProjectMaster.SetValue("ServiceID", addWallPaintingProject.ServiceID);

                    // Save the new custom table record into the database
                    newWallProjectMaster.Insert();

                    addWallPaintingProject.ProjectID = newWallProjectMaster.ItemID;

                    if (ValidationHelper.GetInteger(addWallPaintingProject.ProjectID, 0) != 0)
                    {

                        // Gets the custom table
                        DataClassInfo wallProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectDetail);
                        if (wallProjectDetailInfo != null)
                        {
                            foreach (AddWallPaintingProjectDetail addWallProjectDetail in addWallPaintingProject.AddWallPaintingProjectDetails)
                            {
                                if (!addWallProjectDetail.IsLowResolution)
                                {
                                    // Creates a new custom table item
                                    CustomTableItem newWallProjectDetail = CustomTableItem.New(wallProjectDetail);

                                    // Sets the values for the fields of the custom table (ItemText in this case)
                                    newWallProjectDetail.SetValue("ProjectID", addWallPaintingProject.ProjectID);
                                    newWallProjectDetail.SetValue("ImageUrl", addWallProjectDetail.ImageUrl);
                                    newWallProjectDetail.SetValue("NoOfCopy", addWallProjectDetail.NoOfCopy);
                                    newWallProjectDetail.SetValue("ImageSizeID", addWallProjectDetail.SizeID);

                                    // Save the new custom table record into the database
                                    newWallProjectDetail.Insert();

                                    addWallProjectDetail.ItemID = newWallProjectDetail.ItemID;
                                    addWallProjectDetail.ProjectID = addWallPaintingProject.ProjectID;
                                }
                            }
                        }
                        //UpdateSKU
                        PriceCalculation(addWallPaintingProject.ProjectID, addWallPaintingProject.SKUID, "", addWallPaintingProject.ServiceID);
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
                               itemSkuId = addWallPaintingProject.SKUID
                           }));
            }
            else
            {
                return RedirectToAction("EditWallPaintingProject",
                       new RouteValueDictionary(new
                       {
                           controller = "WallPainting",
                           action = "EditWallPaintingProject",
                           projectId = addWallPaintingProject.ProjectID
                       }));
            }
            //return View(addPhotoProject);

        }

        public ActionResult EditWallPaintingProject(int? projectId, int? SKUId)
        {
            AddWallPaintingProject addWallProject = new AddWallPaintingProject();

            // Gets the custom table
            DataClassInfo wallProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectMaster);
            if (wallProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallProjectMaster)
                             .WhereEquals("ItemID", projectId)
                             .Or()
                             .WhereEquals("SKUID", SKUId)
                             .Columns("ItemID", "PaperMaterialID", "SKUID", "ServiceID", "FrameColorID").ToList();

                if (items != null && items.Count > 0)
                {
                    addWallProject.PaperMaterialID = ValidationHelper.GetInteger(items[0].GetValue("PaperMaterialID"), 0);
                    addWallProject.PaperMaterial = FillComboBox.GetPapaerMaterial();
                    addWallProject.FrameColorID = ValidationHelper.GetInteger(items[0].GetValue("FrameColorID"), 0);
                    addWallProject.FrameColor = FillComboBox.GetFrameColor();
                    addWallProject.Size = FillComboBox.GetPapaerSize(ValidationHelper.GetInteger(items[0].GetValue("ServiceID"), 0));
                    addWallProject.SKUID = ValidationHelper.GetInteger(items[0].GetValue("SKUID"), 0);
                    addWallProject.ProjectID = ValidationHelper.GetInteger(items[0].GetValue("ItemID"), 0);
                    addWallProject.ServiceID = ValidationHelper.GetInteger(items[0].GetValue("ServiceID"), 0);

                    if (addWallProject.ProjectID > 0)
                    {
                        // Gets the custom table
                        DataClassInfo wallProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectDetail);
                        if (wallProjectDetailInfo != null)
                        {
                            string removePath = Server.MapPath("~/PrintForMe/WallPaintingProject/").Replace("\\", "/");

                            // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                            List<CustomTableItem> detailItems = CustomTableItemProvider.GetItems(wallProjectDetail)
                                .WhereEquals("ProjectID", addWallProject.ProjectID)
                                .Columns("ItemID", "ProjectID", "ImageUrl", "NoOfCopy", "ImageSizeID").ToList();

                            // Creates a collection of view models based on the menu item and page data
                            var wallProjectDetailData = detailItems.Select(item => new AddWallPaintingProjectDetail()
                            {
                                ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                                ImageUrl = ValidationHelper.GetString(item.GetValue("ImageUrl"), "").Replace(removePath, "/"),
                                IsLowResolution = ImageResolution(ValidationHelper.GetString(item.GetValue("ImageUrl"), "")),
                                NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0),
                                SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                                ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                                Price = addWallProject.Size.Where(x => x.ItemID == ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0)).Select(x => x.ProductPrice * ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)).FirstOrDefault().ToString(),
                                //ImageToString = System.IO.File.Exists(ValidationHelper.GetString(item.GetValue("ImageUrl"), ""))
                                //                ? Convert.ToBase64String(ImageToByteArray(System.Drawing.Image.FromFile(ValidationHelper.GetString(item.GetValue("ImageUrl"), ""))))
                                //                : deletePhotodetail(ValidationHelper.GetInteger(item.GetValue("ItemID"), 0), addWallProject.SKUID)
                            });

                            if (wallProjectDetailData != null)
                            {
                                addWallProject.AddWallPaintingProjectDetails.AddRange(wallProjectDetailData.Where(x => x.ImageUrl != ""));
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
            return View(addWallProject);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {
                string path = Server.MapPath("~/PrintForMe/WallPaintingProject/" + User.Identity.GetUserName());
                //var path = Path.GetTempPath() + "/PrintForMe/WallPaintingProject/" + User.Identity.GetUserName();

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
                        // Gets the custom table
                        DataClassInfo wallProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectDetail);
                        if (wallProjectDetailInfo != null)
                        {
                            // Creates a new custom table item
                            CustomTableItem newWallProjectDetail = CustomTableItem.New(wallProjectDetail);

                            // Sets the values for the fields of the custom table (ItemText in this case)
                            newWallProjectDetail.SetValue("ProjectID", projectId);
                            newWallProjectDetail.SetValue("ImageUrl", fileName);
                            newWallProjectDetail.SetValue("NoOfCopy", 1);
                            newWallProjectDetail.SetValue("ImageSizeID", 0);

                            // Save the new custom table record into the database
                            newWallProjectDetail.Insert();
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
                    string path = Server.MapPath("~/PrintForMe/WallPaintingProject/" + imageUrl);
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

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
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
                newProduct.SKUName = "Wall Painting Project";
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
                // Gets the custom table
                DataClassInfo wallProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectDetail);
                if (wallProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallProjectDetail)
                    .WhereEquals("ProjectID", projectID)
                    .Columns("ProjectID", "ItemID", "ImageSizeID", "NoOfCopy").ToList();


                    // Creates a collection of view models based on the menu item and page data
                    var wallServiceDetails = items.Select(item => new AddWallPaintingProjectDetail()
                    {
                        ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                        ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                        SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                        NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)
                    });

                    foreach (AddWallPaintingProjectDetail wallServiceDetail in wallServiceDetails)
                    {
                        var item = sizeSettings.FirstOrDefault(s => s.ItemID == wallServiceDetail.SizeID);
                        projectPrice += ValidationHelper.GetDouble(item != null ? item.ProductPrice * wallServiceDetail.NoOfCopy : 0, 0);
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
                // Gets the custom table
                DataClassInfo wallProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallProjectDetail);
                if (wallProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    var wallProjectDetailData = CustomTableItemProvider.GetItems(wallProjectDetail)
                                                                        .WhereEquals("ItemID", itemId)
                                                                        .FirstOrDefault();

                    if (wallProjectDetailData != null)
                    {
                        // Deletes the custom table record from the database
                        wallProjectDetailData.Delete();

                        if (ValidationHelper.GetInteger(SKUID, 0) > 0)
                        {
                            // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                            CustomTableItem item = CustomTableItemProvider.GetItems(wallProjectMaster)
                                         .WhereEquals("SKUID", SKUID)
                                         .Columns("ItemID", "PaperMaterialID", "SKUID", "ServiceID").FirstOrDefault();
                            //UpdateSKU
                            PriceCalculation(ValidationHelper.GetInteger(wallProjectDetailData.GetValue("ProjectID"), 0), SKUID, "", ValidationHelper.GetInteger(item.GetValue("ServiceID"), 0));
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