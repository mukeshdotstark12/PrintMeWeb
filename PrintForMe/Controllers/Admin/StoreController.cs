using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.WorkflowEngine;
using PrintForMe.Models;
using PrintForMe.Models.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers.Admin
{
    public class StoreController : Controller
    {
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        public ActionResult StoreInfo()
        {

            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            var pages = tree.SelectNodes().Type("PrintForMe.CompanyInfo")
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .Published()
                .FirstOrDefault();

            var model = new StoreViewModel();

            model.StoreInfoID = pages.GetValue<int>("CompanyInfoID", 0);
            model.SiteID = pages.GetValue<int>("SiteID", 0);
            model.StoreName = pages.GetValue<string>("Name", string.Empty);
            model.StoreLogoPath = pages.GetValue<string>("Logo", string.Empty);
            model.WhatsappNumber = pages.GetValue<string>("WhatsappNumber", string.Empty);
            model.PhoneNumber = pages.GetValue<string>("PhoneNumber", string.Empty);
            model.Email = pages.GetValue<string>("Email", string.Empty);

            return View(model);
        }

        [HttpPost]
        public ActionResult StoreInfo(StoreViewModel store)
        {

            // Creates an instance of the Tree provider
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Gets the published version of pages stored under the "/Contact-us/" path
            // The pages are retrieved from the Dancing Goat site and in the "en-us" culture
            var pages = tree.SelectNodes().Type("PrintForMe.CompanyInfo")
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .FirstOrDefault();
            string filePath = string.Empty;
            if (store.Logo != null)
            {
                foreach (HttpPostedFileBase objFile in store.Logo)
                {
                    if (objFile != null)
                    {
                        filePath = SaveFile(objFile, store.StoreLogoPath);
                    }
                }
            }

            pages.SetValue("Name", store.StoreName);
            if (!string.IsNullOrEmpty(filePath))
                pages.SetValue("Logo", filePath);
            pages.SetValue("WhatsappNumber", store.WhatsappNumber);
            pages.SetValue("PhoneNumber", store.PhoneNumber);
            pages.SetValue("Email", store.Email);
            //pages.CheckOut();
            pages.Update();
            //pages.CheckIn();
            // Gets the page's workflow
            WorkflowManager workflowManager = WorkflowManager.GetInstance(tree);
            WorkflowInfo workflow = workflowManager.GetNodeWorkflow(pages);

            // Checks if the page uses workflow
            if (workflow != null)
            {
                // Publishes the page with a comment.
                pages.Publish();
            }


            return RedirectToAction("StoreInfo");
        }


        public string SaveFile(HttpPostedFileBase fileToSave, string OldLogoPath)
        {
            if (fileToSave != null)
            {
                // AzureOprHelper AzureHelper = new AzureOprHelper();


                // AzureHelper.storageAccountName = "ltechpro";
                // AzureHelper.storageEndPoint = "core.windows.net";

                //// AzureHelper.srcPath = fileToSave.;

                // AzureHelper.containerName = "appcontainer";

                // AzureHelper.blobName = string.Format("ECS/Media/" + Path.GetFileName(fileToSave.FileName));
                // AzureHelperExtensions.UploadFile(AzureHelper);

                string path = Server.MapPath("~/PrintForMe/media/Images/StoreLogo/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                DirectoryInfo directory = new DirectoryInfo(path);

                directory.EnumerateFiles().ToList().ForEach(f => f.Delete());


                fileToSave.SaveAs(path + Path.GetFileName(fileToSave.FileName));
                return $"~/PrintForMe/media/Images/StoreLogo/{Path.GetFileName(fileToSave.FileName)}";
            }

            return string.Empty;
        }

        public ActionResult PhotoServicesSettings()
        {
            // Prepares the code name (class name) of the custom table
            string serviceSettingsClassName = "PrintForme.ServiceSettings";

            // Gets the custom table
            DataClassInfo serviceSettings = DataClassInfoProvider.GetDataClassInfo(serviceSettingsClassName);
            if (serviceSettings != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(serviceSettingsClassName)
                    //.WhereEquals("Culture", cultureName)
                    .WhereEquals("ServiceID", 5)
                    .ToList();


                // Creates a collection of view models based on the menu item and page data
                var model = items.Select(item => new ServiceSettingModel()
                {
                    Code = ValidationHelper.GetString(item.GetValue("Code"), ""),
                    Description = ValidationHelper.GetString(item.GetValue("Description"), ""),
                    Price = ValidationHelper.GetString(item.GetValue("Price"), ""),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), false),
                    ServiceID = ValidationHelper.GetInteger(item.GetValue("ServiceID"), 0),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Height = ValidationHelper.GetInteger(item.GetValue("Height"), 0),
                    Width = ValidationHelper.GetInteger(item.GetValue("Width"), 0)
                });

                return PartialView("_PhotoServiceSettings", model);
            }

            return PartialView("_PhotoServiceSettings");
        }

        [HandleError]
        public ActionResult AlbumServicesSettings()
        {
            // Prepares the code name (class name) of the custom table
            string serviceSettingsClassName = "PrintForMe.AlbumPageSize";

            // Gets the custom table
            DataClassInfo serviceSettings = DataClassInfoProvider.GetDataClassInfo(serviceSettingsClassName);
            if (serviceSettings != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(serviceSettingsClassName)
                    .WhereEquals("Culture", mCultureName)
                    //.WhereEquals("ServiceID", 4)
                    .ToList();


                // Creates a collection of view models based on the menu item and page data
                var model = items.Select(item => new AlbumFormat()
                {
                    AlbumPageSize = ValidationHelper.GetString(item.GetValue("AlbumPageSize"), ""),
                    ItemGuid = ValidationHelper.GetGuid(item.GetValue("ItemGUID"), Guid.NewGuid()),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    AlbumPageSizeCode = ValidationHelper.GetString(item.GetValue("AlbumPageSizeCode"), ""),
                    Culture = ValidationHelper.GetString(item.GetValue("Culture"), ""),
                    Price = ValidationHelper.GetInteger(item.GetValue("Price"), 0),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true),
                    Height = ValidationHelper.GetInteger(item.GetValue("Height"), 0),
                    Width = ValidationHelper.GetInteger(item.GetValue("Width"), 0),
                });

                return PartialView("_AlbumServiceSettings", model);
            }

            return PartialView("_AlbumServiceSettings");
        }

        public ActionResult WoodenPalletsServicesSettings()
        {
            // Prepares the code name (class name) of the custom table
            string serviceSettingsClassName = "PrintForme.ServiceSettings";

            // Gets the custom table
            DataClassInfo serviceSettings = DataClassInfoProvider.GetDataClassInfo(serviceSettingsClassName);
            if (serviceSettings != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(serviceSettingsClassName)
                    //.WhereEquals("Culture", cultureName)
                    .WhereEquals("ServiceID", 2)
                    .ToList();


                // Creates a collection of view models based on the menu item and page data
                var model = items.Select(item => new ServiceSettingModel()
                {
                    Code = ValidationHelper.GetString(item.GetValue("Code"), ""),
                    Description = ValidationHelper.GetString(item.GetValue("Description"), ""),
                    Price = ValidationHelper.GetString(item.GetValue("Price"), ""),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), false),
                    ServiceID = ValidationHelper.GetInteger(item.GetValue("ServiceID"), 0),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Height = ValidationHelper.GetInteger(item.GetValue("Height"), 0),
                    Width = ValidationHelper.GetInteger(item.GetValue("Width"), 0)
                });

                return PartialView("_WoodenPalletsServicesSettings", model);
            }

            return PartialView("_WoodenPalletsServicesSettings");
        }

        public ActionResult WallPaintingServicesSettings()
        {
            // Prepares the code name (class name) of the custom table
            string serviceSettingsClassName = "PrintForme.ServiceSettings";

            // Gets the custom table
            DataClassInfo serviceSettings = DataClassInfoProvider.GetDataClassInfo(serviceSettingsClassName);
            if (serviceSettings != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(serviceSettingsClassName)
                    //.WhereEquals("Culture", cultureName)
                    .WhereEquals("ServiceID", 3)
                    .ToList();


                // Creates a collection of view models based on the menu item and page data
                var model = items.Select(item => new ServiceSettingModel()
                {
                    Code = ValidationHelper.GetString(item.GetValue("Code"), ""),
                    Description = ValidationHelper.GetString(item.GetValue("Description"), ""),
                    Price = ValidationHelper.GetString(item.GetValue("Price"), ""),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), false),
                    ServiceID = ValidationHelper.GetInteger(item.GetValue("ServiceID"), 0),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Height = ValidationHelper.GetInteger(item.GetValue("Height"), 0),
                    Width = ValidationHelper.GetInteger(item.GetValue("Width"), 0)
                });

                return PartialView("_WallPaintingServicesSettings", model);
            }

            return PartialView("_WallPaintingServicesSettings");
        }

        [HttpPost]
        public ActionResult AddServicesSettings(AlbumFormat value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string serviceSettings = "PrintForMe.AlbumPageSize";

            // Gets the custom table
            DataClassInfo serviceSettingsInfo = DataClassInfoProvider.GetDataClassInfo(serviceSettings);
            if (serviceSettingsInfo != null)
            {
                // Creates a new custom table item
                CustomTableItem newserviceSetting = CustomTableItem.New(serviceSettings);

                // Sets the values for the fields of the custom table (ItemText in this case)
                newserviceSetting.SetValue("ItemGUID", Guid.NewGuid());
                newserviceSetting.SetValue("AlbumPageSize", value.AlbumPageSize);
                newserviceSetting.SetValue("AlbumPageSizeCode", value.AlbumPageSize);
                newserviceSetting.SetValue("Culture", mCultureName);
                newserviceSetting.SetValue("Price", value.Price);
                newserviceSetting.SetValue("Availability", true);
                newserviceSetting.SetValue("Height", value.Height);
                newserviceSetting.SetValue("Width", value.Width);
                // Save the new custom table record into the database
                newserviceSetting.Insert();
            }

            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [HandleError]
        [HttpPost]
        public ActionResult EditServicesSettings(AlbumFormat value)
        {
            try
            {
                // Prepares the code name (class name) of the custom table to which the data record will be Updated
                string serviceSettings = "PrintForMe.AlbumPageSize";

                // Gets the custom table
                DataClassInfo serviceSettingsInfo = DataClassInfoProvider.GetDataClassInfo(serviceSettings);
                if (serviceSettingsInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    var serviceSettingsData = CustomTableItemProvider.GetItems(serviceSettings)
                                                                        .WhereEquals("ItemID", value.ItemID)
                                                                        .FirstOrDefault();

                    if (serviceSettingsData != null)
                    {
                        // Sets a new 'ItemText' value based on the old one
                        serviceSettingsData.SetValue("AlbumPageSize", value.AlbumPageSize);
                        serviceSettingsData.SetValue("AlbumPageSizeCode", value.AlbumPageSize);
                        serviceSettingsData.SetValue("Culture", mCultureName);
                        serviceSettingsData.SetValue("Price", value.Price);
                        serviceSettingsData.SetValue("Availability", value.Price);
                        serviceSettingsData.SetValue("Height", value.Price);
                        serviceSettingsData.SetValue("Width", value.Price);
                        // Saves the changes to the database
                        serviceSettingsData.Update();
                    }
                }

                return RedirectToAction("StoreInfo");
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Store", "Edit Settings", ex);
                ErrorModel errorModel = new ErrorModel()
                {
                    Error = ex.Message,
                    Source = "Store setting edit"
                };
                return View("ErrorView", errorModel);
            }
        }

        [HttpPost]
        public ActionResult DeleteServicesSettings(AlbumFormat value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be Deleted
            string serviceSettings = "PrintForMe.AlbumPageSize";

            // Gets the custom table
            DataClassInfo serviceSettingsInfo = DataClassInfoProvider.GetDataClassInfo(serviceSettings);
            if (serviceSettingsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemText' starts with 'New text'
                CustomTableItem serviceSetting = CustomTableItemProvider.GetItems(serviceSettings)
                                                                    .WhereStartsWith("ItemID", value.ItemID.ToString())
                                                                    .TopN(1)
                                                                    .FirstOrDefault();

                if (serviceSetting != null)
                {
                    // Deletes the custom table record from the database
                    serviceSetting.Delete();
                }
            }

            return RedirectToAction("StoreInfo");
        }

        public ActionResult FrameColorSettings()
        {

            // Prepares the code name (class name) of the custom table
            string frameColors = "PrintForme.FrameColors";

            // Gets the custom table
            DataClassInfo frameColorsInfo = DataClassInfoProvider.GetDataClassInfo(frameColors);
            if (frameColorsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(frameColors)
                    .WhereEquals("Culture", mCultureName)
                    .Columns("ColorName", "Availability", "ItemID").ToList();

                // Creates a collection of view models based on the menu item and page data
                var model = items.Select(item => new FrameColorModel()
                {
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    ColorName = ValidationHelper.GetString(item.GetValue("ColorName"), ""),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true)
                });

                return PartialView("_FrameColorSettings", model);
            }

            return PartialView("_FrameColorSettings");
        }

        [HttpPost]
        public ActionResult AddFrameColor(FrameColorModel value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string frameColors = "PrintForme.FrameColors";

            // Gets the custom table
            DataClassInfo frameColorsInfo = DataClassInfoProvider.GetDataClassInfo(frameColors);
            if (frameColorsInfo != null)
            {
                // Creates a new custom table item
                CustomTableItem newframeColors = CustomTableItem.New(frameColors);

                // Sets the values for the fields of the custom table (ItemText in this case)
                newframeColors.SetValue("ColorName", value.ColorName);
                newframeColors.SetValue("Availability", value.Availability);
                newframeColors.SetValue("Culture", mCultureName);

                // Save the new custom table record into the database
                newframeColors.Insert();
            }

            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditFrameColor(FrameColorModel value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be Updated
            string frameColors = "PrintForme.FrameColors";

            // Gets the custom table
            DataClassInfo frameColorsInfo = DataClassInfoProvider.GetDataClassInfo(frameColors);
            if (frameColorsInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                var frameColorsData = CustomTableItemProvider.GetItems(frameColors)
                                                                    .WhereEquals("ItemID", value.ItemID).FirstOrDefault();

                if (frameColorsData != null)
                {
                    // Sets a new 'ItemText' value based on the old one
                    frameColorsData.SetValue("ColorName", value.ColorName);
                    frameColorsData.SetValue("Availability", value.Availability);

                    // Saves the changes to the database
                    frameColorsData.Update();
                }
            }

            return RedirectToAction("StoreInfo");
        }

        [HttpPost]
        public ActionResult DeleteFrameColor(FrameColorModel value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be Deleted
            string frameColors = "PrintForme.FrameColors";

            // Gets the custom table
            DataClassInfo frameColorsInfo = DataClassInfoProvider.GetDataClassInfo(frameColors);
            if (frameColorsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemText' starts with 'New text'
                CustomTableItem frameColor = CustomTableItemProvider.GetItems(frameColors)
                                                                    .WhereStartsWith("ItemID", value.ItemID.ToString())
                                                                    .TopN(1)
                                                                    .FirstOrDefault();

                if (frameColor != null)
                {
                    // Deletes the custom table record from the database
                    frameColor.Delete();
                }
            }

            return RedirectToAction("StoreInfo");
        }


        public ActionResult PaparMaterialSettings()
        {

            // Prepares the code name (class name) of the custom table
            string paperMaterials = "PrintForme.PaperMaterials";

            // Gets the custom table
            DataClassInfo paperMaterialsInfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            if (paperMaterialsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(paperMaterials)
                    .WhereEquals("Culture", mCultureName)
                     .Columns("PageType", "Availability", "ItemID", "ItemGUID").ToList();

                // Creates a collection of view models based on the menu item and page data
                var model = items.Select(item => new PaperMaterialModel()
                {
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    PageType = ValidationHelper.GetString(item.GetValue("PageType"), ""),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true)
                });

                return PartialView("_PaparMaterialSettings", model);
            }

            return PartialView("_PaparMaterialSettings");
        }

        [HttpPost]
        public ActionResult AddPaparMaterial(PaperMaterialModel value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string paperMaterials = "PrintForme.PaperMaterials";

            // Gets the custom table
            DataClassInfo paperMaterialInfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            if (paperMaterialInfo != null)
            {
                // Creates a new custom table item
                CustomTableItem newpaperMaterial = CustomTableItem.New(paperMaterials);

                // Sets the values for the fields of the custom table (ItemText in this case)
                newpaperMaterial.SetValue("PageType", value.PageType);
                newpaperMaterial.SetValue("Availability", value.Availability);
                newpaperMaterial.SetValue("Culture", mCultureName);

                // Save the new custom table record into the database
                newpaperMaterial.Insert();
            }

            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditPaparMaterial(PaperMaterialModel value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be Updated
            string paperMaterials = "PrintForme.PaperMaterials";

            // Gets the custom table
            DataClassInfo paperMaterialInfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            if (paperMaterialInfo != null)
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                var paperMaterialData = CustomTableItemProvider.GetItems(paperMaterials)
                                                                    .WhereEquals("ItemID", value.ItemID).FirstOrDefault();

                if (paperMaterialData != null)
                {
                    // Sets a new 'ItemText' value based on the old one
                    paperMaterialData.SetValue("PageType", value.PageType);
                    paperMaterialData.SetValue("Availability", value.Availability);

                    // Saves the changes to the database
                    paperMaterialData.Update();
                }
            }

            return RedirectToAction("StoreInfo");
        }

        [HttpPost]
        public ActionResult DeletePaparMaterial(int key)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be Deleted
            string paperMaterials = "PrintForme.PaperMaterials";

            // Gets the custom table
            DataClassInfo paperMaterialsInfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            if (paperMaterialsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemText' starts with 'New text'
                CustomTableItem paperMaterial = CustomTableItemProvider.GetItems(paperMaterials)
                                                                    .WhereStartsWith("ItemID", key.ToString())
                                                                    .TopN(1)
                                                                    .FirstOrDefault();

                if (paperMaterial != null)
                {
                    // Deletes the custom table record from the database
                    paperMaterial.Delete();
                }
            }

            return RedirectToAction("StoreInfo");
        }


        public ActionResult AlbumQuantity()
        {
            // Prepares the code name (class name) of the custom table
            string serviceAlbumClassName = "Printforme.AlbumSize";

            // Gets the custom table
            DataClassInfo sizeSettings = DataClassInfoProvider.GetDataClassInfo(serviceAlbumClassName);
            if (sizeSettings != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(serviceAlbumClassName)
                    .WhereEquals("Culture", mCultureName)
                    .ToList();


                // Creates a collection of view models based on the menu item and page data
                var model = items.Select(item => new AlbumNoofPages()
                {
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Size = ValidationHelper.GetString(item.GetValue("Size"), ""),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true),
                    AlbumSizeCode = ValidationHelper.GetGuid(item.GetValue("ItemGUID"), Guid.NewGuid()),
                    Culture = ValidationHelper.GetString(item.GetValue("Culture"), ""),
                });

                return PartialView("_PartialAlbumSize", model);
            }

            return PartialView("_PartialAlbumSize");
        }

        public ActionResult EditAlbumQuantity(AlbumNoofPages value)
        {
            try
            {
                // Prepares the code name (class name) of the custom table to which the data record will be Updated
                string serviceSettings = "Printforme.AlbumSize";

                // Gets the custom table
                DataClassInfo serviceSettingsInfo = DataClassInfoProvider.GetDataClassInfo(serviceSettings);
                if (serviceSettingsInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    var serviceSettingsData = CustomTableItemProvider.GetItems(serviceSettings)
                                                                        .WhereEquals("ItemID", value.ItemID)
                                                                        .FirstOrDefault();

                    if (serviceSettingsData != null)
                    {
                        // Sets a new 'ItemText' value based on the old one
                        serviceSettingsData.SetValue("Size", value.Size);
                        serviceSettingsData.SetValue("Availability", value.Availability);
                        serviceSettingsData.SetValue("Culture", mCultureName);

                        // Saves the changes to the database
                        serviceSettingsData.Update();
                    }
                }

                return RedirectToAction("StoreInfo");
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Store", "Edit Settings", ex);
                ErrorModel errorModel = new ErrorModel()
                {
                    Error = ex.Message,
                    Source = "Store setting edit"
                };
                return View("ErrorView", errorModel);
            }
        }

        public ActionResult DeleteAlbumQuantity(AlbumNoofPages value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be Deleted
            string serviceSettings = "Printforme.AlbumSize";

            // Gets the custom table
            DataClassInfo serviceSettingsInfo = DataClassInfoProvider.GetDataClassInfo(serviceSettings);
            if (serviceSettingsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemText' starts with 'New text'
                CustomTableItem serviceSetting = CustomTableItemProvider.GetItems(serviceSettings)
                                                                    .WhereStartsWith("ItemID", value.ItemID.ToString())
                                                                    .TopN(1)
                                                                    .FirstOrDefault();

                if (serviceSetting != null)
                {
                    // Deletes the custom table record from the database
                    serviceSetting.Delete();
                }
            }

            return RedirectToAction("StoreInfo");
        }

        public ActionResult AddAlbumQuantity(AlbumNoofPages value)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string serviceSettings = "Printforme.AlbumSize";

            // Gets the custom table
            DataClassInfo serviceSettingsInfo = DataClassInfoProvider.GetDataClassInfo(serviceSettings);
            if (serviceSettingsInfo != null)
            {
                // Creates a new custom table item
                CustomTableItem newserviceSetting = CustomTableItem.New(serviceSettings);

                // Sets the values for the fields of the custom table (ItemText in this case)
                newserviceSetting.SetValue("Size", value.Size);
                newserviceSetting.SetValue("Availability", value.Availability);
                newserviceSetting.SetValue("ItemGUID", Guid.NewGuid());
                newserviceSetting.SetValue("Culture", mCultureName);

                // Save the new custom table record into the database
                newserviceSetting.Insert();
            }

            return Json(value, JsonRequestBehavior.AllowGet);
        }

    }
}