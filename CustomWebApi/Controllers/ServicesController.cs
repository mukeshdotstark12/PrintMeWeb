
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine.Types.PrintForMe;
using CMS.Helpers;
using CustomWebApi.Helpers;
using CustomWebApi.Model.Services;
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
    public class ServicesController : ApiController
    {
        #region GET        
        [HttpGet]
        public HttpResponseMessage GetServices(string cultureName)
        {
            try
            {
                // Gets the printing service
                var services = PrintingServicesProvider.GetPrintingServices()
                .Culture("en-US")
                .Columns("Name", "Image", "Link", "ColorCode", "PrintingServicesID")
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder");

                // Creates a collection of view models based on the menu item and page data              
                List<PrintingServiceModel> model = new List<PrintingServiceModel>();

                foreach (var item in services)
                {
                    PrintingServiceModel printingService = new PrintingServiceModel()
                    {
                        Name = ResHelper.GetString(item.Name, cultureName),
                        Image = item.Image,
                        Link = item.Link,
                        ColorCode = item.ColorCode,
                        ServicesID = item.PrintingServicesID,
                        MinimumPrice = GetMinimumPrice(item.PrintingServicesID)
                    };

                    model.Add(printingService);
                }

                if (model != null && model.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = model,
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
                        description = "Service not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPaperSize(string cultureName, int serviceID)
        {
            try
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
                        .WhereEquals("ServiceID", serviceID)
                        .Columns("Code", "Availability", "Description", "Price", "ServiceID", "ItemID").ToList();

                    // Creates a collection of view models based on the menu item and page data
                    var model = items.Select(item => new ServiceSettingModel()
                    {
                        Code = ValidationHelper.GetString(item.GetValue("Code"), "") + " " + ValidationHelper.GetString(item.GetValue("Description"), ""),
                        Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true),
                        Description = ValidationHelper.GetString(item.GetValue("Description"), ""),
                        Price = ValidationHelper.GetString(item.GetValue("Price"), ""),
                        ServiceID = ValidationHelper.GetInteger(item.GetValue("ServiceID"), 0),
                        ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0)
                    });

                    if (model != null && model.Count() > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            data = model,
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
                            description = "No data found"
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "No data found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPaperMaterial(string cultureName)
        {
            try
            {
                // Prepares the code name (class name) of the custom table
                string paperMaterialsClassName = "PrintForme.PaperMaterials";

                // Gets the custom table
                DataClassInfo paperMaterials = DataClassInfoProvider.GetDataClassInfo(paperMaterialsClassName);
                if (paperMaterials != null)
                {
                    // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(paperMaterialsClassName)
                        .WhereEquals("Culture", cultureName)
                        .Columns("PageType", "Availability", "ItemID").ToList();

                    // Creates a collection of view models based on the menu item and page data
                    var model = items.Select(item => new PaperMaterialModel()
                    {
                        PageType = ValidationHelper.GetString(item.GetValue("PageType"), ""),
                        Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true),
                        Id = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0)
                    });

                    if (model != null && model.Count() > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            data = model,
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
                            description = "No data found"
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "No data found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetFrameColors(string cultureName)
        {
            try
            {
                // Prepares the code name (class name) of the custom table
                string frameColors = "PrintForme.FrameColors";

                // Gets the custom table
                DataClassInfo frameColorsInfo = DataClassInfoProvider.GetDataClassInfo(frameColors);
                if (frameColorsInfo != null)
                {
                    // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                    List<CustomTableItem> items = CustomTableItemProvider.GetItems(frameColors)
                        .WhereEquals("Culture", cultureName)
                        .Columns("ColorName", "Availability", "ItemID").ToList();

                    // Creates a collection of view models based on the menu item and page data
                    var model = items.Select(item => new FrameColorModel()
                    {
                        ColorName = ValidationHelper.GetString(item.GetValue("ColorName"), ""),
                        Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true),
                        Id = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0)
                    });

                    if (model != null && model.Count() > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                        {
                            data = model,
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
                            description = "No data found"
                        });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "No data found"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new CustomResponse
                {
                    status = HttpStatusCode.BadRequest,
                    errorCode = HttpStatusCode.BadRequest.ToString(),
                    description = ex.Message
                });
            }
        }
        #endregion

        #region Methods
        private int GetMinimumPrice(int serviceId)
        {
            // Prepares the code name (class name) of the custom table
            string serviceSettingsClassName = "PrintForme.ServiceSettings";
            int minimumPrice = 0;

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

            return minimumPrice;
        }
        #endregion
    }
}
