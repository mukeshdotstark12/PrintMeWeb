using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CustomWebApi.Helpers;
using CustomWebApi.Model.PhotoService;
using CustomWebApi.Model.Shared;
using CustomWebApi.Model.WoodenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CustomWebApi.Controllers
{
    public class WoodenServiceController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveWoodenProject(WoodenServiceModel woodenServiceModel)
        {
            if (ValidationHelper.GetInteger(woodenServiceModel.ProjectID, 0) > 0 && woodenServiceModel.WoodenServiceDetailModel.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Images Not Found",
                });
            }

            if (ValidationHelper.GetString(woodenServiceModel.PlankThickness, "") == "")
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Plank Thickness Not Found",
                });
            }

            if (ValidationHelper.GetInteger(woodenServiceModel.ServiceId, 0) == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "ServiceId Not Found",
                });
            }

            UserInfo users = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(woodenServiceModel.UserID, 0));

            if (users == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "User Not Found",
                });
            }

            // Prepares the code name (class name) of the custom table to which the data record will be added
            string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
            string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";

            // Gets the custom table
            DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
            DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);

            if (ValidationHelper.GetInteger(woodenServiceModel.ProjectID, 0) == 0 || ValidationHelper.GetInteger(woodenServiceModel.SKUID, 0) == 0) //Update Project
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(woodenProjectMaster)
                         .WhereEquals("UserID", woodenServiceModel.UserID)
                         .WhereEquals("IsComplete", false)
                         .Columns("ItemID", "PlankThickness", "SKUID").LastOrDefault();


                if (userProjectDetail != null && ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0) > 0)
                {
                    woodenServiceModel.ProjectID = ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0);
                    woodenServiceModel.PlankThickness = ValidationHelper.GetString(userProjectDetail.GetValue("PlankThickness"), "");
                    woodenServiceModel.SKUID = ValidationHelper.GetInteger(userProjectDetail.GetValue("SKUID"), 0);
                }
            }

            if (ValidationHelper.GetInteger(woodenServiceModel.ProjectID, 0) > 0) //Update Project
            {
                if (woodenProjectMasterInfo != null)
                {
                    CustomTableItem item = CustomTableItemProvider.GetItems(woodenProjectMaster)
                                                        .WhereEquals("ItemID", woodenServiceModel.ProjectID)
                                                        .WhereEquals("UserID", woodenServiceModel.UserID)
                                                        //.WhereEquals("IsComplete", false)
                                                        .Columns("ItemID", "UserID", "SKUID")
                                                        .FirstOrDefault();

                    if (item == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                        {
                            status = HttpStatusCode.NotFound,
                            errorCode = HttpStatusCode.NotFound.ToString(),
                            description = "ProjectId not found for this user",
                        });
                    }
                    else if (woodenServiceModel.SKUID != ValidationHelper.GetInteger(item.GetValue("SKUID"), 0))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                        {
                            status = HttpStatusCode.NotFound,
                            errorCode = HttpStatusCode.NotFound.ToString(),
                            description = "SKUID not found for this project",
                        });
                    }

                    //if (item != null)
                    //{
                    //    // Sets a new 'IsComplete' value based on the old one
                    //    item.SetValue("IsComplete", ValidationHelper.GetBoolean(woodenServiceModel.IsComplete, false));

                    //    // Saves the changes to the database
                    //    item.Update();
                    //}
                }

                if (woodenProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(woodenProjectDetail)
                    .WhereEquals("ProjectID", woodenServiceModel.ProjectID)
                    .Columns("ProjectID", "ItemID", "ImageSizeID", "NoOfCopy").ToList();


                    //// Creates a collection of view models based on the menu item and page data
                    //var photoServiceDetails = items.Select(item => new PhotoServiceDetailModel()
                    //{
                    //    ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                    //    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    //    SizeID = ValidationHelper.GetInteger(item.GetValue("SizeID"), 0),
                    //    NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)
                    //});

                    foreach (var item in items)
                    {
                        // Deletes the custom table record from the database
                        item.Delete();
                    }

                    foreach (WoodenServiceDetailModel woodenServiceDetail in woodenServiceModel.WoodenServiceDetailModel)
                    {
                        // Creates a new custom table item
                        CustomTableItem newphotoProjectDetail = CustomTableItem.New(woodenProjectDetail);

                        // Sets the values for the fields of the custom table (ItemText in this case)
                        newphotoProjectDetail.SetValue("ProjectID", woodenServiceModel.ProjectID);
                        newphotoProjectDetail.SetValue("ImageUrl", woodenServiceDetail.ImageUrl);
                        newphotoProjectDetail.SetValue("NoOfCopy", woodenServiceDetail.NoOfCopy);
                        newphotoProjectDetail.SetValue("ImageSizeID", woodenServiceDetail.SizeID);

                        // Save the new custom table record into the database
                        newphotoProjectDetail.Insert();

                        woodenServiceDetail.ItemID = newphotoProjectDetail.ItemID;
                        woodenServiceDetail.ProjectID = woodenServiceModel.ProjectID;
                    }
                }

                //SKU Price Calculation
                PriceCalculation(woodenServiceModel.ProjectID, woodenServiceModel.SKUID, woodenServiceModel.ServiceId);

                List<WoodenServiceModel> projectArray = new List<WoodenServiceModel>();
                projectArray.Add(woodenServiceModel);

                return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                {
                    data = projectArray,
                    status = HttpStatusCode.OK,
                    message = "Success"
                });
            }
            else  //Create New Project
            {
                // create SKU
                woodenServiceModel.SKUID = ManageSKU(0, 0, "");

                // Creates a new custom table item
                CustomTableItem newwoodenProjectMaster = CustomTableItem.New(woodenProjectMaster);

                // Sets the values for the fields of the custom table (ItemText in this case)
                newwoodenProjectMaster.SetValue("PlankThickness", woodenServiceModel.PlankThickness);
                newwoodenProjectMaster.SetValue("IsComplete", false);
                newwoodenProjectMaster.SetValue("UserID", woodenServiceModel.UserID);
                newwoodenProjectMaster.SetValue("ProjectDate", DateTime.Now);
                newwoodenProjectMaster.SetValue("SKUID", woodenServiceModel.SKUID);
                newwoodenProjectMaster.SetValue("ServiceID", woodenServiceModel.ServiceId);

                // Save the new custom table record into the database
                newwoodenProjectMaster.Insert();

                woodenServiceModel.ProjectID = newwoodenProjectMaster.ItemID;

                if (ValidationHelper.GetInteger(woodenServiceModel.ProjectID, 0) != 0)
                {
                    if (woodenProjectDetailInfo != null && woodenServiceModel.WoodenServiceDetailModel.Count > 0)
                    {
                        foreach (WoodenServiceDetailModel woodenServiceDetail in woodenServiceModel.WoodenServiceDetailModel)
                        {
                            // Creates a new custom table item
                            CustomTableItem newwoodenProjectDetail = CustomTableItem.New(woodenProjectDetail);

                            // Sets the values for the fields of the custom table (ItemText in this case)
                            newwoodenProjectDetail.SetValue("ProjectID", woodenServiceModel.ProjectID);
                            newwoodenProjectDetail.SetValue("ImageUrl", woodenServiceDetail.ImageUrl);
                            newwoodenProjectDetail.SetValue("NoOfCopy", woodenServiceDetail.NoOfCopy);
                            newwoodenProjectDetail.SetValue("ImageSizeID", woodenServiceDetail.SizeID);

                            // Save the new custom table record into the database
                            newwoodenProjectDetail.Insert();

                            woodenServiceDetail.ItemID = newwoodenProjectDetail.ItemID;
                            woodenServiceDetail.ProjectID = woodenServiceModel.ProjectID;
                        }

                        //SKU Price Calculation
                        PriceCalculation(woodenServiceModel.ProjectID, woodenServiceModel.SKUID, woodenServiceModel.ServiceId);
                    }
                }

                //PhotoServiceResponse projectData = new PhotoServiceResponse
                //{
                //    ProjectId = photoServiceModel.ProjectID,
                //    SKUID = photoServiceModel.SKUID
                //};

                List<WoodenServiceModel> projectArray = new List<WoodenServiceModel>();
                projectArray.Add(woodenServiceModel);

                return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                {
                    data = projectArray,
                    status = HttpStatusCode.OK,
                    message = "Success"
                });
            }
        }

        private int ManageSKU(double Price, int SKUID, string SKUImagePath)
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
                updateProduct.SKUImagePath = updateProduct.SKUImagePath;

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

        private int PriceCalculation(int projectID, int SKUID, int serviceId)
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
                    var woodenServiceDetails = items.Select(item => new WoodenServiceDetailModel()
                    {
                        ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                        ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                        SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                        NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)
                    });

                    foreach (WoodenServiceDetailModel woodenServiceDetail in woodenServiceDetails)
                    {
                        var item = sizeSettings.FirstOrDefault(s => s.ItemID == woodenServiceDetail.SizeID);
                        projectPrice += ValidationHelper.GetDouble(item != null ? item.ProductPrice * woodenServiceDetail.NoOfCopy : 0, 0);
                    }

                    SKUID = ManageSKU(projectPrice, SKUID, "");
                }
            }

            return SKUID;
        }
    }
}
