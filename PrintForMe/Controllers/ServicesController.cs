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
    public class ServicesController : Controller
    {
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        // GET: Services
        public ActionResult PhotoService(int serviceId)
        {
            //Check pending project
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            if (photoProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(photoProjectMaster)
                             .WhereEquals("UserID", User.Identity.GetUserId())
                             .WhereEquals("IsComplete", false)
                             .Columns("ItemID", "SKUID").ToList();

                if (items != null && items.Count > 0)
                {
                    return RedirectToAction("EditPhotoProject",
                       new RouteValueDictionary(new
                       {
                           controller = "Services",
                           action = "EditPhotoProject",
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
                Size = FillComboBox.GetPapaerSize(serviceId),
                ServiceID = serviceId
            };

            return View(model);

        }

        public ActionResult AddPhotoProject(PhotoServiceModel photoService)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            //Check pending project
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            if (photoProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(photoProjectMaster)
                             .WhereEquals("UserID", User.Identity.GetUserId())
                             .WhereEquals("IsComplete", false)
                             .Columns("ItemID", "PaperMaterialID").ToList();

                if (items != null && items.Count > 0)
                {
                    return RedirectToAction("EditPhotoProject",
                       new RouteValueDictionary(new
                       {
                           controller = "Services",
                           action = "EditPhotoProject",
                           projectId = ValidationHelper.GetInteger(items[0].GetValue("ItemID"), 0)
                       }));
                }
            }

            var model = new AddPhotoProject()
            {
                PaperMaterialID = photoService.SelectedPaperMaterial,
                SizeID = photoService.SelectedSize,
                PaperMaterial = FillComboBox.GetPapaerMaterial(),
                Size = FillComboBox.GetPapaerSize(photoService.ServiceID),
                ServiceID = photoService.ServiceID
            };

            string path = Server.MapPath("~/PrintForMe/PhotoProject/" + User.Identity.GetUserName());
            //var path = Path.GetTempPath() + "PrintForMe/PhotoProject/" + User.Identity.GetUserName();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] filePaths = Directory.GetFiles(path);
            string removePath = Server.MapPath("~/PrintForMe/PhotoProject/");

            for (int i = 0; i <= filePaths.Length - 1; i++)
            {
                filePaths[i] = filePaths[i].Replace(removePath, "/");
                filePaths[i] = filePaths[i].Replace("\\", "/");
            }

            //AddPhotoProject model = new AddPhotoProject();
            var modeldetail = filePaths.Select(item => new AddPhotoProjectDetail()
            {
                //ImageToString = Convert.ToBase64String(ImageToByteArray(System.Drawing.Image.FromFile(item))),
                ImageUrl = item,
                NoOfCopy = 1,
                Price = model.Size.Where(x => x.ItemID == model.SizeID).Select(x => x.ProductPrice).FirstOrDefault().ToString(),
                IsLowResolution = ImageResolution("~/PrintForMe/PhotoProject/" + item) //ImageResolution(removePath.Replace("\\", "/") + item)
            });

            model.AddPhotoProjectDetails.AddRange(modeldetail);

            return View(model);

        }

        [HttpPost]
        public ActionResult AddPhotoProject(AddPhotoProject addPhotoProject, string addToCart)
        {
            addPhotoProject.PaperMaterial = FillComboBox.GetPapaerMaterial();
            addPhotoProject.Size = FillComboBox.GetPapaerSize(addPhotoProject.ServiceID);

            if (!ModelState.IsValid || addPhotoProject.AddPhotoProjectDetails.Count == 0)
            {
                return View(addPhotoProject);
            }

            //if (addPhotoProject.AddPhotoProjectDetails.Where(x => x.IsLowResolution).Count() > 0)
            //{                
            //    return View(addPhotoProject);
            //}

            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            if (photoProjectMasterInfo != null)
            {
                if (addPhotoProject.ProjectID > 0)
                {
                    string photoProjectDetail = "PrintForme.PhotoProjectDetail";

                    // Gets the custom table
                    DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
                    if (photoProjectDetailInfo != null)
                    {
                        foreach (AddPhotoProjectDetail addPhotoProjectDetail in addPhotoProject.AddPhotoProjectDetails)
                        {
                            // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                            var photoProjectDetailData = CustomTableItemProvider.GetItems(photoProjectDetail)
                                                                                .WhereEquals("ItemID", addPhotoProjectDetail.ItemID)
                                                                                .FirstOrDefault();

                            if (photoProjectDetailData != null && !addPhotoProjectDetail.IsLowResolution)
                            {
                                // Sets the values for the fields of the custom table (ItemText in this case)                                
                                photoProjectDetailData.SetValue("ImageUrl", addPhotoProjectDetail.ImageUrl);
                                photoProjectDetailData.SetValue("NoOfCopy", addPhotoProjectDetail.NoOfCopy);
                                photoProjectDetailData.SetValue("ImageSizeID", addPhotoProjectDetail.SizeID);

                                // Saves the changes to the database
                                photoProjectDetailData.Update();
                            }
                        }

                        //UpdateSKU
                        PriceCalculation(addPhotoProject.ProjectID, addPhotoProject.SKUID, "", addPhotoProject.ServiceID);
                    }
                }
                else
                {
                    // create SKU
                    addPhotoProject.SKUID = ManageSKU(0, 0, addPhotoProject.AddPhotoProjectDetails.Count > 0 ? addPhotoProject.AddPhotoProjectDetails[0].ImageUrl : "");

                    // Creates a new custom table item
                    CustomTableItem newPhotoProjectMaster = CustomTableItem.New(photoProjectMaster);

                    // Sets the values for the fields of the custom table (ItemText in this case)
                    newPhotoProjectMaster.SetValue("PaperMaterialID", addPhotoProject.PaperMaterialID);
                    newPhotoProjectMaster.SetValue("IsComplete", false);
                    newPhotoProjectMaster.SetValue("UserID", User.Identity.GetUserId());
                    newPhotoProjectMaster.SetValue("ProjectDate", DateTime.Now);
                    newPhotoProjectMaster.SetValue("SKUID", addPhotoProject.SKUID);
                    newPhotoProjectMaster.SetValue("ServiceID", addPhotoProject.ServiceID);

                    // Save the new custom table record into the database
                    newPhotoProjectMaster.Insert();

                    addPhotoProject.ProjectID = newPhotoProjectMaster.ItemID;

                    if (ValidationHelper.GetInteger(addPhotoProject.ProjectID, 0) != 0)
                    {
                        string photoProjectDetail = "PrintForme.PhotoProjectDetail";

                        // Gets the custom table
                        DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
                        if (photoProjectDetailInfo != null)
                        {
                            foreach (AddPhotoProjectDetail addPhotoProjectDetail in addPhotoProject.AddPhotoProjectDetails)
                            {
                                if (!addPhotoProjectDetail.IsLowResolution)
                                {
                                    // Creates a new custom table item
                                    CustomTableItem newphotoProjectDetail = CustomTableItem.New(photoProjectDetail);

                                    // Sets the values for the fields of the custom table (ItemText in this case)
                                    newphotoProjectDetail.SetValue("ProjectID", addPhotoProject.ProjectID);
                                    newphotoProjectDetail.SetValue("ImageUrl", addPhotoProjectDetail.ImageUrl);
                                    newphotoProjectDetail.SetValue("NoOfCopy", addPhotoProjectDetail.NoOfCopy);
                                    newphotoProjectDetail.SetValue("ImageSizeID", addPhotoProjectDetail.SizeID);

                                    // Save the new custom table record into the database
                                    newphotoProjectDetail.Insert();

                                    addPhotoProjectDetail.ItemID = newphotoProjectDetail.ItemID;
                                    addPhotoProjectDetail.ProjectID = addPhotoProject.ProjectID;
                                }
                            }
                        }
                        //UpdateSKU
                        PriceCalculation(addPhotoProject.ProjectID, addPhotoProject.SKUID, "", addPhotoProject.ServiceID);
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
                               itemSkuId = addPhotoProject.SKUID
                           }));
            }
            else
            {
                return RedirectToAction("EditPhotoProject",
                           new RouteValueDictionary(new
                           {
                               controller = "Services",
                               action = "EditPhotoProject",
                               projectId = addPhotoProject.ProjectID
                           }));
            }
            //return View(addPhotoProject);

        }

        public ActionResult EditPhotoProject(int? projectId, int? SKUId)
        {
            AddPhotoProject addPhotoProject = new AddPhotoProject();
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            
            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            if (photoProjectMasterInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(photoProjectMaster)
                             .WhereEquals("ItemID", projectId)
                             .Or()
                             .WhereEquals("SKUID", SKUId)
                             .Columns("ItemID", "PaperMaterialID", "SKUID", "ServiceID").ToList();

                if (items != null && items.Count > 0)
                {
                    addPhotoProject.PaperMaterialID = ValidationHelper.GetInteger(items[0].GetValue("PaperMaterialID"), 0);
                    addPhotoProject.PaperMaterial = FillComboBox.GetPapaerMaterial();
                    addPhotoProject.Size = FillComboBox.GetPapaerSize(ValidationHelper.GetInteger(items[0].GetValue("ServiceID"), 0));
                    addPhotoProject.SKUID = ValidationHelper.GetInteger(items[0].GetValue("SKUID"), 0);
                    addPhotoProject.ProjectID = ValidationHelper.GetInteger(items[0].GetValue("ItemID"), 0);
                    addPhotoProject.ServiceID = ValidationHelper.GetInteger(items[0].GetValue("ServiceID"), 0);

                    if (addPhotoProject.ProjectID > 0)
                    {
                        string photoProjectDetail = "PrintForme.PhotoProjectDetail";

                        // Gets the custom table
                        DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
                        if (photoProjectDetailInfo != null)
                        {
                            string removePath = Server.MapPath("~/PrintForMe/PhotoProject/").Replace("\\","/");

                            // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                            List<CustomTableItem> detailItems = CustomTableItemProvider.GetItems(photoProjectDetail)
                                .WhereEquals("ProjectID", addPhotoProject.ProjectID)
                                .Columns("ItemID", "ProjectID", "ImageUrl", "NoOfCopy", "ImageSizeID").ToList();

                            // Creates a collection of view models based on the menu item and page data
                            var photoProjectDetailData = detailItems.Select(item => new AddPhotoProjectDetail()
                            {
                                ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                                ImageUrl =  ValidationHelper.GetString(item.GetValue("ImageUrl"), "").Replace(removePath, "/"),
                                IsLowResolution = ImageResolution(ValidationHelper.GetString(item.GetValue("ImageUrl"), "")),
                                NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0),
                                SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                                ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                                Price = addPhotoProject.Size.Where(x => x.ItemID == ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0)).Select(x => x.ProductPrice * ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)).FirstOrDefault().ToString(),
                                //ImageToString = System.IO.File.Exists(ValidationHelper.GetString(item.GetValue("ImageUrl"), ""))
                                //                ? Convert.ToBase64String(ImageToByteArray(System.Drawing.Image.FromFile(ValidationHelper.GetString(item.GetValue("ImageUrl"), ""))))
                                //                : deletePhotodetail(ValidationHelper.GetInteger(item.GetValue("ItemID"), 0), addPhotoProject.SKUID)
                            });

                            if (photoProjectDetailData != null)
                            {                                
                                addPhotoProject.AddPhotoProjectDetails.AddRange(photoProjectDetailData.Where(x => x.ImageUrl != ""));
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
            return View(addPhotoProject);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {
                var projectId = Request.Params["id"];

                string path = Server.MapPath("~/PrintForMe/PhotoProject/" + User.Identity.GetUserName());
                //var path = Path.GetTempPath() + "/PrintForMe/PhotoProject/" + User.Identity.GetUserName();

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

                    if (!string.IsNullOrEmpty(projectId))
                    {
                        string photoProjectDetail = "PrintForme.PhotoProjectDetail";

                        // Gets the custom table
                        DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
                        if (photoProjectDetailInfo != null)
                        {
                            // Creates a new custom table item
                            CustomTableItem newphotoProjectDetail = CustomTableItem.New(photoProjectDetail);

                            // Sets the values for the fields of the custom table (ItemText in this case)
                            newphotoProjectDetail.SetValue("ProjectID", projectId);
                            newphotoProjectDetail.SetValue("ImageUrl", fileName);
                            newphotoProjectDetail.SetValue("NoOfCopy", 1);
                            newphotoProjectDetail.SetValue("ImageSizeID", 0);

                            // Save the new custom table record into the database
                            newphotoProjectDetail.Insert();
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
                    string path = Server.MapPath("~/PrintForMe/PhotoProject/" + imageUrl);
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
                newProduct.SKUName = "Photo Project";
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
                string photoProjectDetail = "PrintForme.PhotoProjectDetail";

                // Gets the custom table
                DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
                if (photoProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(photoProjectDetail)
                    .WhereEquals("ProjectID", projectID)
                    .Columns("ProjectID", "ItemID", "ImageSizeID", "NoOfCopy").ToList();


                    // Creates a collection of view models based on the menu item and page data
                    var photoServiceDetails = items.Select(item => new AddPhotoProjectDetail()
                    {
                        ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                        ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                        SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                        NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)
                    });

                    foreach (AddPhotoProjectDetail photoServiceDetail in photoServiceDetails)
                    {
                        var item = sizeSettings.FirstOrDefault(s => s.ItemID == photoServiceDetail.SizeID);
                        projectPrice += ValidationHelper.GetDouble(item != null ? item.ProductPrice * photoServiceDetail.NoOfCopy : 0, 0);
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
                string photoProjectMaster = "PrintForme.PhotoProjectMaster";
                string photoProjectDetail = "PrintForme.PhotoProjectDetail";

                // Gets the custom table
                DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
                if (photoProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    var photoProjectDetailData = CustomTableItemProvider.GetItems(photoProjectDetail)
                                                                        .WhereEquals("ItemID", itemId)
                                                                        .FirstOrDefault();

                    if (photoProjectDetailData != null)
                    {
                        // Deletes the custom table record from the database
                        photoProjectDetailData.Delete();

                        if (ValidationHelper.GetInteger(SKUID, 0) > 0)
                        {
                            // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                            CustomTableItem item = CustomTableItemProvider.GetItems(photoProjectMaster)
                                         .WhereEquals("SKUID", SKUID)
                                         .Columns("ItemID", "PaperMaterialID", "SKUID", "ServiceID").FirstOrDefault();
                            //UpdateSKU
                            PriceCalculation(ValidationHelper.GetInteger(photoProjectDetailData.GetValue("ProjectID"), 0), SKUID, "", ValidationHelper.GetInteger(item.GetValue("ServiceID"), 0));
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