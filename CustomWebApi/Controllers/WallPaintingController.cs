using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CustomWebApi.Helpers;
using CustomWebApi.Model.Shared;
using CustomWebApi.Model.WallPaintingService;
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
    public class WallPaintingController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage SaveWallPaintingProject(WallPaintingService wallPaintingService)
        {
            if (ValidationHelper.GetInteger(wallPaintingService.ProjectID, 0) > 0 && wallPaintingService.WallPaintingServiceDetail.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Images Not Found",
                });
            }

            if (ValidationHelper.GetInteger(wallPaintingService.PaperMaterialID, 0) == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Paper Material Not Found",
                });
            }

            if (ValidationHelper.GetInteger(wallPaintingService.FrameColorID, 0) == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Frame Color Not Found",
                });
            }

            if (ValidationHelper.GetInteger(wallPaintingService.ServiceId, 0) == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "ServiceId Not Found",
                });
            }

            if (ValidationHelper.GetString(wallPaintingService.PaintingSize, "") == "")
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Enter Painting Size",
                });
            }

            UserInfo users = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(wallPaintingService.UserID, 0));

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
            string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";
            string wallPrintingProjectDetail = "PrintForme.WallPrintingProjectDetail";

            // Gets the custom table
            DataClassInfo wallPrintingProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);
            DataClassInfo wallPrintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectDetail);

            if (ValidationHelper.GetInteger(wallPaintingService.ProjectID, 0) == 0 || ValidationHelper.GetInteger(wallPaintingService.SKUID, 0) == 0) //Update Project
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                         .WhereEquals("UserID", wallPaintingService.UserID)
                         .WhereEquals("IsComplete", false)
                         .Columns("ItemID", "PaintingSize", "FrameColorID", "PaperMaterialID", "SKUID").LastOrDefault();


                if (userProjectDetail != null && ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0) > 0)
                {
                    wallPaintingService.ProjectID = ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0);
                    wallPaintingService.PaintingSize = ValidationHelper.GetString(userProjectDetail.GetValue("PaintingSize"), "");
                    wallPaintingService.PaperMaterialID = ValidationHelper.GetInteger(userProjectDetail.GetValue("PaperMaterialID"), 0);
                    wallPaintingService.FrameColorID = ValidationHelper.GetInteger(userProjectDetail.GetValue("FrameColorID"), 0);
                    wallPaintingService.SKUID = ValidationHelper.GetInteger(userProjectDetail.GetValue("SKUID"), 0);
                }
            }

            if (ValidationHelper.GetInteger(wallPaintingService.ProjectID, 0) > 0) //Update Project
            {
                if (wallPrintingProjectMasterInfo != null)
                {
                    CustomTableItem item = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                                                        .WhereEquals("ItemID", wallPaintingService.ProjectID)
                                                        .WhereEquals("UserID", wallPaintingService.UserID)
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
                    else if (wallPaintingService.SKUID != ValidationHelper.GetInteger(item.GetValue("SKUID"), 0))
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

                if (wallPrintingProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallPrintingProjectDetail)
                    .WhereEquals("ProjectID", wallPaintingService.ProjectID)
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

                    foreach (WallPaintingServiceDetailModel wallPaintingServiceDetail in wallPaintingService.WallPaintingServiceDetail)
                    {
                        // Creates a new custom table item
                        CustomTableItem newWallPaintingProjectDetail = CustomTableItem.New(wallPrintingProjectDetail);

                        // Sets the values for the fields of the custom table (ItemText in this case)
                        newWallPaintingProjectDetail.SetValue("ProjectID", wallPaintingService.ProjectID);
                        newWallPaintingProjectDetail.SetValue("ImageUrl", wallPaintingServiceDetail.ImageUrl);
                        newWallPaintingProjectDetail.SetValue("NoOfCopy", wallPaintingServiceDetail.NoOfCopy);
                        newWallPaintingProjectDetail.SetValue("ImageSizeID", wallPaintingServiceDetail.SizeID);

                        // Save the new custom table record into the database
                        newWallPaintingProjectDetail.Insert();

                        wallPaintingServiceDetail.ItemID = newWallPaintingProjectDetail.ItemID;
                        wallPaintingServiceDetail.ProjectID = wallPaintingService.ProjectID;
                    }
                }

                //SKU Price Calculation
                PriceCalculation(wallPaintingService.ProjectID, wallPaintingService.SKUID, wallPaintingService.ServiceId);

                List<WallPaintingService> projectArray = new List<WallPaintingService>();
                projectArray.Add(wallPaintingService);

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
                wallPaintingService.SKUID = ManageSKU(0, 0, "");

                // Creates a new custom table item
                CustomTableItem newWallPaintingProjectMaster = CustomTableItem.New(wallPrintingProjectMaster);

                // Sets the values for the fields of the custom table (ItemText in this case)
                newWallPaintingProjectMaster.SetValue("PaintingSize", wallPaintingService.PaintingSize);
                newWallPaintingProjectMaster.SetValue("PaperMaterialID", wallPaintingService.PaperMaterialID);
                newWallPaintingProjectMaster.SetValue("FrameColorID", wallPaintingService.FrameColorID);
                newWallPaintingProjectMaster.SetValue("IsComplete", false);
                newWallPaintingProjectMaster.SetValue("UserID", wallPaintingService.UserID);
                newWallPaintingProjectMaster.SetValue("ProjectDate", DateTime.Now);
                newWallPaintingProjectMaster.SetValue("SKUID", wallPaintingService.SKUID);
                newWallPaintingProjectMaster.SetValue("ServiceID", wallPaintingService.ServiceId);

                // Save the new custom table record into the database
                newWallPaintingProjectMaster.Insert();

                wallPaintingService.ProjectID = newWallPaintingProjectMaster.ItemID;

                if (ValidationHelper.GetInteger(wallPaintingService.ProjectID, 0) != 0)
                {
                    if (wallPrintingProjectDetailInfo != null && wallPaintingService.WallPaintingServiceDetail.Count > 0)
                    {
                        foreach (WallPaintingServiceDetailModel wallPaintingServiceDetail in wallPaintingService.WallPaintingServiceDetail)
                        {
                            // Creates a new custom table item
                            CustomTableItem newWallPaintingProjectDetail = CustomTableItem.New(wallPrintingProjectDetail);

                            // Sets the values for the fields of the custom table (ItemText in this case)
                            newWallPaintingProjectDetail.SetValue("ProjectID", wallPaintingService.ProjectID);
                            newWallPaintingProjectDetail.SetValue("ImageUrl", wallPaintingServiceDetail.ImageUrl);
                            newWallPaintingProjectDetail.SetValue("NoOfCopy", wallPaintingServiceDetail.NoOfCopy);
                            newWallPaintingProjectDetail.SetValue("ImageSizeID", wallPaintingServiceDetail.SizeID);

                            // Save the new custom table record into the database
                            newWallPaintingProjectDetail.Insert();

                            wallPaintingServiceDetail.ItemID = newWallPaintingProjectDetail.ItemID;
                            wallPaintingServiceDetail.ProjectID = wallPaintingService.ProjectID;
                        }

                        //SKU Price Calculation
                        PriceCalculation(wallPaintingService.ProjectID, wallPaintingService.SKUID, 5);
                    }
                }

                //PhotoServiceResponse projectData = new PhotoServiceResponse
                //{
                //    ProjectId = photoServiceModel.ProjectID,
                //    SKUID = photoServiceModel.SKUID
                //};

                List<WallPaintingService> projectArray = new List<WallPaintingService>();
                projectArray.Add(wallPaintingService);

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

        private int PriceCalculation(int projectID, int SKUID, int serviceId)
        {
            var sizeSettings = FillComboBox.GetPapaerSize(serviceId);
            double projectPrice = 0;

            if (ValidationHelper.GetInteger(projectID, 0) > 0)
            {
                string wallPaintingProjectDetail = "PrintForme.WallPrintingProjectDetail";

                // Gets the custom table
                DataClassInfo wallPaintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPaintingProjectDetail);
                if (wallPaintingProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(wallPaintingProjectDetail)
                    .WhereEquals("ProjectID", projectID)
                    .Columns("ProjectID", "ItemID", "ImageSizeID", "NoOfCopy").ToList();


                    // Creates a collection of view models based on the menu item and page data
                    var wallPaintingServiceDetails = items.Select(item => new WallPaintingServiceDetailModel()
                    {
                        ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                        ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                        SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                        NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)
                    });

                    foreach (WallPaintingServiceDetailModel wallPaintingServiceDetail in wallPaintingServiceDetails)
                    {
                        var item = sizeSettings.FirstOrDefault(s => s.ItemID == wallPaintingServiceDetail.SizeID);
                        projectPrice += ValidationHelper.GetDouble(item != null ? item.ProductPrice * wallPaintingServiceDetail.NoOfCopy : 0, 0);
                    }

                    SKUID = ManageSKU(projectPrice, SKUID, "");
                }
            }

            return SKUID;
        }
    }
}
