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
    public class PhotoServiceController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetInCompleteProject(PhotoServiceModel photoServiceModel)
        {
            // Prepares the code name (class name) of the custom table to which the data record will be added
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);

            if (photoProjectMasterInfo != null) //Update Project
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(photoProjectMaster)
                         .WhereEquals("UserID", photoServiceModel.UserID)
                         .WhereEquals("IsComplete", false)
                         .Columns("ItemID", "PaperMaterialID", "SKUID").LastOrDefault();


                if (userProjectDetail != null && ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0) > 0)
                {
                    photoServiceModel.ProjectID = ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0);
                    photoServiceModel.PaperMaterialID = ValidationHelper.GetInteger(userProjectDetail.GetValue("PaperMaterialID"), 0);
                    photoServiceModel.SKUID = ValidationHelper.GetInteger(userProjectDetail.GetValue("SKUID"), 0);

                    List<PhotoServiceModel> projectArray = new List<PhotoServiceModel>();
                    projectArray.Add(photoServiceModel);

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = projectArray,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });
                }
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "Project Not Found",
            });
        }


        [HttpPost]
        public HttpResponseMessage SavePhotoProject(PhotoServiceModel photoServiceModel)
        {
            if (ValidationHelper.GetInteger(photoServiceModel.ProjectID, 0) > 0 && photoServiceModel.PhotoServiceDetailModel.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Images Not Found!",
                });
            }

            if (ValidationHelper.GetInteger(photoServiceModel.PaperMaterialID, 0) == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Paper Material Not Found!",
                });
            }

            UserInfo users = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(photoServiceModel.UserID, 0));

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
            string photoProjectMaster = "PrintForme.PhotoProjectMaster";
            string photoProjectDetail = "PrintForme.PhotoProjectDetail";

            // Gets the custom table
            DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
            DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);

            if (ValidationHelper.GetInteger(photoServiceModel.ProjectID, 0) == 0 || ValidationHelper.GetInteger(photoServiceModel.SKUID, 0) == 0) //Update Project
            {
                // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                CustomTableItem userProjectDetail = CustomTableItemProvider.GetItems(photoProjectMaster)
                         .WhereEquals("UserID", photoServiceModel.UserID)
                         .WhereEquals("IsComplete", false)
                         .Columns("ItemID", "PaperMaterialID", "SKUID").LastOrDefault();


                if (userProjectDetail != null && ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0) > 0)
                {
                    photoServiceModel.ProjectID = ValidationHelper.GetInteger(userProjectDetail.GetValue("ItemID"), 0);
                    photoServiceModel.PaperMaterialID = ValidationHelper.GetInteger(userProjectDetail.GetValue("PaperMaterialID"), 0);
                    photoServiceModel.SKUID = ValidationHelper.GetInteger(userProjectDetail.GetValue("SKUID"), 0);
                }
            }

            if (ValidationHelper.GetInteger(photoServiceModel.ProjectID, 0) > 0) //Update Project
            {
                if (photoProjectMasterInfo != null)
                {
                    CustomTableItem item = CustomTableItemProvider.GetItems(photoProjectMaster)
                                                        .WhereEquals("ItemID", photoServiceModel.ProjectID)
                                                        .WhereEquals("UserID", photoServiceModel.UserID)
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
                    else if (photoServiceModel.SKUID != ValidationHelper.GetInteger(item.GetValue("SKUID"), 0))
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
                    //    item.SetValue("IsComplete", ValidationHelper.GetBoolean(photoServiceModel.IsComplete, false));

                    //    // Saves the changes to the database
                    //    item.Update();
                    //}
                }

                if (photoProjectDetailInfo != null)
                {
                    // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(photoProjectDetail)
                    .WhereEquals("ProjectID", photoServiceModel.ProjectID)
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

                    foreach (PhotoServiceDetailModel photoServiceDetail in photoServiceModel.PhotoServiceDetailModel)
                    {
                        // Creates a new custom table item
                        CustomTableItem newphotoProjectDetail = CustomTableItem.New(photoProjectDetail);

                        // Sets the values for the fields of the custom table (ItemText in this case)
                        newphotoProjectDetail.SetValue("ProjectID", photoServiceModel.ProjectID);
                        newphotoProjectDetail.SetValue("ImageUrl", photoServiceDetail.ImageUrl);
                        newphotoProjectDetail.SetValue("NoOfCopy", photoServiceDetail.NoOfCopy);
                        newphotoProjectDetail.SetValue("ImageSizeID", photoServiceDetail.SizeID);

                        // Save the new custom table record into the database
                        newphotoProjectDetail.Insert();

                        photoServiceDetail.ItemID = newphotoProjectDetail.ItemID;
                        photoServiceDetail.ProjectID = photoServiceModel.ProjectID;
                    }
                }

                //SKU Price Calculation
                PriceCalculation(photoServiceModel.ProjectID, photoServiceModel.SKUID, 5);

                List<PhotoServiceModel> projectArray = new List<PhotoServiceModel>();
                projectArray.Add(photoServiceModel);

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
                photoServiceModel.SKUID = ManageSKU(0, 0, "");

                // Creates a new custom table item
                CustomTableItem newPhotoProjectMaster = CustomTableItem.New(photoProjectMaster);

                // Sets the values for the fields of the custom table (ItemText in this case)
                newPhotoProjectMaster.SetValue("PaperMaterialID", photoServiceModel.PaperMaterialID);
                newPhotoProjectMaster.SetValue("IsComplete", false);
                newPhotoProjectMaster.SetValue("UserID", photoServiceModel.UserID);
                newPhotoProjectMaster.SetValue("ProjectDate", DateTime.Now);
                newPhotoProjectMaster.SetValue("SKUID", photoServiceModel.SKUID);
                newPhotoProjectMaster.SetValue("ServiceID", photoServiceModel.ServiceId);

                // Save the new custom table record into the database
                newPhotoProjectMaster.Insert();

                photoServiceModel.ProjectID = newPhotoProjectMaster.ItemID;

                if (ValidationHelper.GetInteger(photoServiceModel.ProjectID, 0) != 0)
                {
                    if (photoProjectDetailInfo != null && photoServiceModel.PhotoServiceDetailModel.Count > 0)
                    {
                        foreach (PhotoServiceDetailModel photoServiceDetail in photoServiceModel.PhotoServiceDetailModel)
                        {
                            // Creates a new custom table item
                            CustomTableItem newphotoProjectDetail = CustomTableItem.New(photoProjectDetail);

                            // Sets the values for the fields of the custom table (ItemText in this case)
                            newphotoProjectDetail.SetValue("ProjectID", photoServiceModel.ProjectID);
                            newphotoProjectDetail.SetValue("ImageUrl", photoServiceDetail.ImageUrl);
                            newphotoProjectDetail.SetValue("NoOfCopy", photoServiceDetail.NoOfCopy);
                            newphotoProjectDetail.SetValue("ImageSizeID", photoServiceDetail.SizeID);

                            // Save the new custom table record into the database
                            newphotoProjectDetail.Insert();

                            photoServiceDetail.ItemID = newphotoProjectDetail.ItemID;
                            photoServiceDetail.ProjectID = photoServiceModel.ProjectID;
                        }

                        //SKU Price Calculation
                        PriceCalculation(photoServiceModel.ProjectID, photoServiceModel.SKUID, 5);
                    }
                }

                //PhotoServiceResponse projectData = new PhotoServiceResponse
                //{
                //    ProjectId = photoServiceModel.ProjectID,
                //    SKUID = photoServiceModel.SKUID
                //};

                List<PhotoServiceModel> projectArray = new List<PhotoServiceModel>();
                projectArray.Add(photoServiceModel);

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

        private int PriceCalculation(int projectID, int SKUID, int serviceId)
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
                    var photoServiceDetails = items.Select(item => new PhotoServiceDetailModel()
                    {
                        ProjectID = ValidationHelper.GetInteger(item.GetValue("ProjectID"), 0),
                        ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                        SizeID = ValidationHelper.GetInteger(item.GetValue("ImageSizeID"), 0),
                        NoOfCopy = ValidationHelper.GetInteger(item.GetValue("NoOfCopy"), 0)
                    });

                    foreach (PhotoServiceDetailModel photoServiceDetail in photoServiceDetails)
                    {
                        var item = sizeSettings.FirstOrDefault(s => s.ItemID == photoServiceDetail.SizeID);
                        projectPrice += ValidationHelper.GetDouble(item != null ? item.ProductPrice * photoServiceDetail.NoOfCopy : 0, 0);
                    }

                    SKUID = ManageSKU(projectPrice, SKUID, "");
                }
            }

            return SKUID;
        }
    }

    public class PhotoServiceResponse
    {
        public int ProjectId { get; set; }
        public int SKUID { get; set; }
    }
}
