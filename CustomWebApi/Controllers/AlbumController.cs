using CMS.Core;
using CMS.Ecommerce;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CMS.Base;
using System.Text;
using CMS.Helpers;
using CustomWebApi.Model;
using CMS.Membership;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CustomWebApi.Helpers;
using CMS.DataEngine;
using CMS.CustomTables;
using CMS.CustomTables.Types.PrintForme;
using CustomWebApi.Model.Shared;
using CustomWebApi.Model.Album;
using System.Web;
using CustomWebApi.Model.Services;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using System.Drawing;
using CMS.DocumentEngine;
using CMS.Localization;
using System.Web.Http.Routing;

namespace CustomWebApi.Controllers
{
    public class AlbumController : ApiController
    {
        private readonly static string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        public AlbumViewModel objalbumViewModel;

        [HttpGet]
        public HttpResponseMessage GetAlbumSize()
        {
            // Prepares the code name (class name) of the custom table
            string albumsize = "PrintForme.AlbumSize";

            // Gets the custom table
            DataClassInfo paperMaterialsInfo = DataClassInfoProvider.GetDataClassInfo(albumsize);
            if (albumsize != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(albumsize)
                    .WhereEquals("Availability", true)
                    .WhereEquals("Culture", mCultureName)
                     .Columns("Size", "ItemID", "Availability", "ItemGUID").ToList();



                var pageModel = items.Select(item => new AlbumNoofPages()
                {
                    Size = ValidationHelper.GetString(item.GetValue("Size"), "") + " " + "Pages",
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), false),
                    AlbumSizeCode = ValidationHelper.GetGuid(item.GetValue("ItemGUID"), Guid.NewGuid())
                });

                return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                {
                    data = pageModel,
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
                    description = "Album Size Not Found",
                });
            }

        }

        [HttpGet]
        public HttpResponseMessage GetPaperSize()
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@culture", mCultureName);
            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_GetAlbumPageSize", parameters, QueryTypeEnum.StoredProcedure);

            if(ds != null)
            {
                List<AlbumFormat> albumformat = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumFormat
                {
                    AlbumPageSize = dataRow.Field<string>("AlbumPageSize"),
                    ItemGuid = dataRow.Field<Guid>("ItemGUID"),
                    ItemID = dataRow.Field<int>("ItemID"),
                    AlbumPageSizeCode = dataRow.Field<string>("AlbumPageSizeCode"),
                    Height = dataRow.Field<int>("Height"),
                    Width = dataRow.Field<int>("Width"),
                    Culture = dataRow.Field<string>("Culture")

                }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                {
                    data = albumformat,
                    status = HttpStatusCode.OK,
                    message = "Success"
                });
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "Paper Size Not Found",
            });
        }

        [HttpGet]
        public HttpResponseMessage GetPaperMaterial()
        {
            string paperMaterials = "PrintForme.PaperMaterials";

            // Gets the custom table
            DataClassInfo paperMaterialsInfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            if (paperMaterialsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(paperMaterials)
                    .WhereEquals("Availability", true)
                    .WhereEquals("Culture", mCultureName)
                     .Columns("PageType", "Availability", "ItemID", "ItemGUID", "PaperMaterialCode").ToList();


                var papermaterial = items.Select(item => new PaperMaterialModel()
                {
                    PageType = ValidationHelper.GetString(item.GetValue("PageType"), ""),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), true),
                    PaperMaterialCode = ValidationHelper.GetGuid(item.GetValue("ItemGUID"), Guid.NewGuid()),
                }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                {
                    data = papermaterial,
                    status = HttpStatusCode.OK,
                    message = "Success"
                });

            }

            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "Paper Material Not Found",
            });
        }

        [HttpGet]
        public HttpResponseMessage GetUncompleteAlbums(int userId)
        {
            string AlbumRowGuid = "";
            if (userId != 0)
            {
                var model = AlbumDetailwithPrice.GetAlbumwihPrice(userId);
                if (model != null)
                {
                    List<Album> responseModel = new List<Album>();
                    responseModel.Add(model);

                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse
                    {
                        data = responseModel,
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
                        description = "User ID Not Found",
                    });
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "User ID Not Found",
            });
        }

        [HttpPost]
        public HttpResponseMessage AlbumAddToCart(int SKUID)
        {
            AlbumAddtoCart objalbumaddtocart = new AlbumAddtoCart();
            if (SKUID != 0)
            {
                var model = GetAlbumbySkuID(SKUID);
                
                    string url1 = "http://adminprintme.ltechpro.com/api/ShoppingCart/AddToCart";
                    WebClient wc1 = new WebClient();

                    wc1.QueryString.Add("UserID", model.LoggedinUserID.ToString());
                    wc1.QueryString.Add("SKUID", objalbumaddtocart.SKUID.ToString());
                    wc1.QueryString.Add("SKUUnits", 1.ToString());
                    var data1 = wc1.UploadValues(url1, "POST", wc1.QueryString);

                    var responseString1 = UnicodeEncoding.UTF8.GetString(data1);

                return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse2
                {
                    status = HttpStatusCode.OK,
                    message = "Your Item Added Succesfully"
                });

            }

            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "SKU ID Not Found",
            });
        }

        [HttpPost]
        public HttpResponseMessage SaveAlbum(AlbumViewModel albumViewModel)
        {
            var SKUID = 0;
            
            if (albumViewModel.GetAlbumDetail.Count() == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Data Not Found!",
                });
            }
            if (albumViewModel.UserID == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "User ID Not Found!",
                });
            }
            foreach (var item in albumViewModel.GetAlbumDetail)
            {
                foreach(var i in item.AlbumPageImageDetail)
                {
                    QueryDataParameters parameters = new QueryDataParameters();
                    parameters.Add("@AlbumPageID", item.AlbumPageID);
                    parameters.Add("@PageCountCode", i.ImageUrl);
                    parameters.Add("@PageSizeCode", i.Position);
                    parameters.Add("@PaperMaterialCode", item.Template);

                    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_SaveAlbumDetail", parameters, QueryTypeEnum.StoredProcedure);
                }
                
            }
            //var model1 = albumViewModel.FirstOrDefault();
            

            var model = GetAlbumwihPrice(albumViewModel.UserID);
            CustomTableItem item2 = null;
            int itemTextValue = 0;

            string customTableClassName = "PrintForMe.Album";

            DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
            if (customTable != null)
            {
                item2 = CustomTableItemProvider.GetItems(customTableClassName)
                                                                    .WhereEquals("ItemID", model.AlbumID)
                                                                    .WhereEquals("State", 1)
                                                                    .TopN(1)
                                                                    .FirstOrDefault();

                if (item2 != null)
                {
                    itemTextValue = ValidationHelper.GetInteger(item2.GetValue("SKUID"), 0);
                }

            }

            QueryDataParameters parameter = new QueryDataParameters();
            parameter.Add("@LoginUserID", albumViewModel.UserID);
            DataSet ds1 = ConnectionHelper.ExecuteQuery("SP_Printforme_GetAlbumforMobile", parameter, QueryTypeEnum.StoredProcedure);
            AlbumMaster result = new AlbumMaster();

            if (itemTextValue == 0)
            {
                SKUID = ManageSKU(model.Price, model.AlbumID, model.ImagesLocation);

                item2.SetValue("SKUID", SKUID);

                item2.Update();

                
                if (ds1 != null)
                {
                    List<AlbumMaster> item = ds1.Tables[0].AsEnumerable().Select(dataRow => new AlbumMaster
                    {
                        ProjectID = dataRow.Field<int>("ItemID"),
                        PaperCount = dataRow.Field<int>("PageCountID"),
                        PaperSize = dataRow.Field<int>("PageSizeID"),
                        PaperMaterial = dataRow.Field<int>("PaperMaterialID"),
                        UserId = dataRow.Field<int>("UserID"),
                        SKUID = dataRow.Field<string>("SKUID"),
                        IsComplete = dataRow.Field<bool>("State"),
                        ProjectDate = dataRow.Field<DateTime>("ItemCreatedWhen"),
                        TotalPrice = dataRow.Field<int>("Price"),
                        ServiceId = dataRow.Field<int>("serviceId")
                    }).ToList();

                    result = item.FirstOrDefault();

                }

                return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse2
                {
                    data = result,
                    status = HttpStatusCode.OK,
                    message = "Success"
                });
            }

            if (ds1 != null)
            {
                List<AlbumMaster> item = ds1.Tables[0].AsEnumerable().Select(dataRow => new AlbumMaster
                {
                    ProjectID = dataRow.Field<int>("ItemID"),
                    PaperCount = dataRow.Field<int>("PageCountID"),
                    PaperSize = dataRow.Field<int>("PageSizeID"),
                    PaperMaterial = dataRow.Field<int>("PaperMaterialID"),
                    UserId = dataRow.Field<int>("UserID"),
                    SKUID = dataRow.Field<string>("SKUID"),
                    IsComplete = dataRow.Field<bool>("State"),
                    ProjectDate = dataRow.Field<DateTime>("ItemCreatedWhen"),
                    TotalPrice = dataRow.Field<int>("Price"),
                    ServiceId = dataRow.Field<int>("serviceId")
                }).ToList();
                result = item.FirstOrDefault();
            }

            return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse2
            {
                data = result,
                status = HttpStatusCode.OK,
                message = "Success"
            });
        }

        [HttpPost]
        public HttpResponseMessage CreateAlbum(AlbumMaster albumMasterModel)
        {
            AlbumMaster objalbumMaster = new AlbumMaster();
            int ALbumId = 0;
            string AlbumRowGuid = "";
            string AlbumPageCountCode = "";
            string AlbumSize = "";
            string AlbumPageType = "";
            string album = "PrintForme.Album";
            string albumsize = "PrintForme.AlbumSize";
            string paperMaterials = "PrintForme.PaperMaterials";
            string AlbumPageSize = "PrintForMe.AlbumPageSize";

            DataClassInfo albumInfo = DataClassInfoProvider.GetDataClassInfo(album);
            DataClassInfo albumsizeInfo = DataClassInfoProvider.GetDataClassInfo(albumsize);
            DataClassInfo paperMaterialsinfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            DataClassInfo AlbumPageSizeinfo = DataClassInfoProvider.GetDataClassInfo(AlbumPageSize);

            if (albumMasterModel.UserId != 0)
            {
                CustomTableItem item2 = CustomTableItemProvider.GetItems(album)
                                                        .WhereEquals("UserID", albumMasterModel.UserId)
                                                        .WhereEquals("State", 1)
                                                        .TopN(1)
                                                        .LastOrDefault();

                if (item2 != null)
                {
                    ALbumId = ValidationHelper.GetInteger(item2.GetValue("ItemId"), 0);
                    AlbumRowGuid = ValidationHelper.GetString(item2.GetValue("ItemGuid"), "");
                }
                if (ALbumId == 0)
                {
                    if (albumMasterModel.PaperCount != 0)
                    {
                        CustomTableItem item1 = CustomTableItemProvider.GetItem(albumMasterModel.PaperCount, albumsize);
                        if (item1 != null)
                        {
                            AlbumPageCountCode = ValidationHelper.GetString(item1.GetValue("ItemGuid"), "");
                        }
                        if (string.IsNullOrEmpty(AlbumPageCountCode))
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                            {
                                status = HttpStatusCode.NotFound,
                                errorCode = HttpStatusCode.NotFound.ToString(),
                                description = "Paper Count Not Found",
                            });
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                        {
                            status = HttpStatusCode.NotFound,
                            errorCode = HttpStatusCode.NotFound.ToString(),
                            description = "Paper Count Not Found",
                        });
                    }

                    if (albumMasterModel.PaperSize != 0)
                    {
                        CustomTableItem item1 = CustomTableItemProvider.GetItem(albumMasterModel.PaperSize, AlbumPageSize);
                        if (item1 != null)
                        {
                            AlbumSize = ValidationHelper.GetString(item1.GetValue("AlbumPageSizeCode"), "");
                        }
                        if (string.IsNullOrEmpty(AlbumPageCountCode))
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                            {
                                status = HttpStatusCode.NotFound,
                                errorCode = HttpStatusCode.NotFound.ToString(),
                                description = "Paper Size Not Found",
                            });
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                        {
                            status = HttpStatusCode.NotFound,
                            errorCode = HttpStatusCode.NotFound.ToString(),
                            description = "Paper Size Not Found",
                        });
                    }

                    if (albumMasterModel.PaperMaterial != 0)
                    {
                        CustomTableItem item1 = CustomTableItemProvider.GetItem(albumMasterModel.PaperMaterial, paperMaterials);
                        if (item1 != null)
                        {
                            AlbumPageType = ValidationHelper.GetString(item1.GetValue("ItemGuid"), "");
                        }
                        if (string.IsNullOrEmpty(AlbumPageCountCode))
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                            {
                                status = HttpStatusCode.NotFound,
                                errorCode = HttpStatusCode.NotFound.ToString(),
                                description = "Paper Material Not Found",
                            });
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                        {
                            status = HttpStatusCode.NotFound,
                            errorCode = HttpStatusCode.NotFound.ToString(),
                            description = "Paper Material Not Found",
                        });
                    }

                    QueryDataParameters parameters = new QueryDataParameters();
                    parameters.Add("@LoginUserID", albumMasterModel.UserId);
                    parameters.Add("@PageCountCode", AlbumPageCountCode);
                    parameters.Add("@PageSizeCode", AlbumSize);
                    parameters.Add("@PaperMaterialCode", AlbumPageType);

                    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_CreateAlbum", parameters, QueryTypeEnum.StoredProcedure);
                    QueryDataParameters parameter = new QueryDataParameters();
                    parameter.Add("@LoginUserID", albumMasterModel.UserId);
                    DataSet ds1 = ConnectionHelper.ExecuteQuery("SP_Printforme_GetAlbumforMobile", parameter, QueryTypeEnum.StoredProcedure);
                    if (ds1 != null)
                    {
                        List<AlbumMaster> item = ds1.Tables[0].AsEnumerable().Select(dataRow => new AlbumMaster
                        {
                            ProjectID = dataRow.Field<int>("ItemID"),
                            PaperCount = dataRow.Field<int>("PageCountID"),
                            PaperSize = dataRow.Field<int>("PageSizeID"),
                            PaperMaterial = dataRow.Field<int>("PaperMaterialID"),
                            UserId = dataRow.Field<int>("UserID"),
                            SKUID = dataRow.Field<string>("SKUID"),
                            IsComplete = dataRow.Field<bool>("State"),
                            ProjectDate = dataRow.Field<DateTime>("ItemCreatedWhen"),
                            TotalPrice = dataRow.Field<int>("Price"),
                            ServiceId = dataRow.Field<int>("serviceId")
                        }).ToList();

                        objalbumMaster = item.FirstOrDefault();

                        objalbumMaster.GetAlbumPage = GetAlbumPages(AlbumRowGuid);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse2
                    {
                        data = objalbumMaster,
                        status = HttpStatusCode.OK,
                        message = "Success"
                    });

                }
                else
                {
                    QueryDataParameters parameter = new QueryDataParameters();
                    parameter.Add("@LoginUserID", albumMasterModel.UserId);
                    DataSet ds1 = ConnectionHelper.ExecuteQuery("SP_Printforme_GetAlbumforMobile", parameter, QueryTypeEnum.StoredProcedure);
                    if (ds1 != null)
                    {
                        List<AlbumMaster> item = ds1.Tables[0].AsEnumerable().Select(dataRow => new AlbumMaster
                        {
                            ProjectID = dataRow.Field<int>("ItemID"),
                            PaperCount = dataRow.Field<int>("PageCountID"),
                            PaperSize = dataRow.Field<int>("PageSizeID"),
                            PaperMaterial = dataRow.Field<int>("PaperMaterialID"),
                            UserId = dataRow.Field<int>("UserID"),
                            SKUID = dataRow.Field<string>("SKUID"),
                            IsComplete = dataRow.Field<bool>("State"),
                            ProjectDate = dataRow.Field<DateTime>("ItemCreatedWhen"),
                            TotalPrice = dataRow.Field<int>("Price"),
                            ServiceId = dataRow.Field<int>("serviceId")
                        }).ToList();

                        objalbumMaster = item.FirstOrDefault();

                        objalbumMaster.GetAlbumPage = GetAlbumPages(AlbumRowGuid);

                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse2
                        {
                            data = objalbumMaster,
                            status = HttpStatusCode.OK,
                            message = "Project Id is already created."
                        });
                    }

                }
            }
            
            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "User ID Not Found",
            });

        }

        [HttpPost]
        public HttpResponseMessage CheckAlbum(int UserId)
        {
            int count = 0;
            AlbumViewModel albumViewModel = new AlbumViewModel();
            AlbumPageDetail objalbumdetail = new AlbumPageDetail();
            List<AlbumPageDetail> albumPageDetail = new List<AlbumPageDetail>();
            List<AlbumImageDetail> AlbumPageImageDetail = new List<AlbumImageDetail>();

            if (UserId != 0)
            {
                int albumid = GetAlbumid(UserId);

                if(albumid != 0)
                {
                    QueryDataParameters parameter = new QueryDataParameters();
                    parameter.Add("@LoginUserID", UserId);
                    DataSet ds1 = ConnectionHelper.ExecuteQuery("SP_Printforme_GetAlbumforMobile", parameter, QueryTypeEnum.StoredProcedure);
                    List<AlbumMaster> item = ds1.Tables[0].AsEnumerable().Select(dataRow => new AlbumMaster
                    {
                        ProjectID = dataRow.Field<int>("ItemID"),
                        PaperCount = dataRow.Field<int>("PageCountID"),
                        PaperSize = dataRow.Field<int>("PageSizeID"),
                        PaperMaterial = dataRow.Field<int>("PaperMaterialID"),
                        UserId = dataRow.Field<int>("UserID"),
                        SKUID = dataRow.Field<string>("SKUID"),
                        IsComplete = dataRow.Field<bool>("State"),
                        ProjectDate = dataRow.Field<DateTime>("ItemCreatedWhen"),
                        TotalPrice = dataRow.Field<int>("Price"),
                        ServiceId = dataRow.Field<int>("serviceId")
                    }).ToList();

                    var result = item.FirstOrDefault();

                    albumViewModel.UserID = result.UserId;
                    albumViewModel.PaperCount = result.PaperCount;
                    albumViewModel.PaperMaterial = result.PaperMaterial;
                    albumViewModel.PaperSize = result.PaperSize;
                    albumViewModel.SKUID = result.SKUID;
                    albumViewModel.ProjectDate = result.ProjectDate;
                    albumViewModel.ProjectID = result.ProjectID;
                    albumViewModel.ServiceId = result.ServiceId;
                    albumViewModel.AlbumPageImageDetail = AlbumPageImageDetail;
                    albumViewModel.albumPageDetail = albumPageDetail;

                    string albumpage = "PrintForme.AlbumPage";
                    string albumpagephoto = "PrintForme.AlbumPagePhoto";

                    // Gets the custom table
                    DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(albumpage);
                    DataClassInfo customTable2 = DataClassInfoProvider.GetDataClassInfo(albumpagephoto);
                    if (customTable != null)
                    {
                        List<CustomTableItem> item2 = CustomTableItemProvider.GetItems(albumpage)
                                                                            .WhereEquals("Albumid", albumid)
                                                                            .ToList();

                        var pageModel = item2.Select(itemvalue => new AlbumPageDetail()
                        {
                            AlbumPageID = ValidationHelper.GetInteger(itemvalue.GetValue("ItemID"), 0),
                            Template = ValidationHelper.GetInteger(itemvalue.GetValue("AlbumPageLayoutId"), 0)

                        });

                        albumViewModel.albumPageDetail = pageModel.ToList();
                        int countlist = pageModel.Count();

                        for (int i = 0; i < countlist; i++)
                        {
                            albumViewModel.albumPageDetail[i].AlbumPageImageDetail = AlbumPageImageDetail;
                        }

                        foreach (var i in pageModel)
                        {
                            AlbumImageDetail albumImageDetail = new AlbumImageDetail();
                            List<CustomTableItem> item3 = CustomTableItemProvider.GetItems(albumpagephoto)
                                                                            .WhereEquals("AlbumPageId", i.AlbumPageID)
                                                                            .ToList();

                            var imageModel = item3.Select(itemvalue => new AlbumImageDetail()
                            {
                                Position = ValidationHelper.GetInteger(itemvalue.GetValue("Sequence"), 0),
                                ImageUrl = ValidationHelper.GetString(itemvalue.GetValue("PhotoLocation"), "")
                            });

                            albumViewModel.albumPageDetail[count].AlbumPageImageDetail = imageModel.ToList();

                            count++;
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, new CustomResponse2
                        {
                            data = albumViewModel,
                            status = HttpStatusCode.OK,
                            message = "Success"
                        });
                    }

                
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse2
                {
                    status = HttpStatusCode.NotFound,
                    errorCode = HttpStatusCode.NotFound.ToString(),
                    description = "Project Id Not Found",
                });

            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse2
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "User ID Not Found",
            });
 
        }

        [HttpPost]
        public HttpResponseMessage DeleteAlbum(int UserId)
        {
            string AlbumRowGuid = "";
            if (UserId != 0)
            {
                string album = "PrintForme.Album";
                DataClassInfo albumInfo = DataClassInfoProvider.GetDataClassInfo(album);

                CustomTableItem item2 = CustomTableItemProvider.GetItems(album)
                                                        .WhereEquals("UserID", UserId)
                                                        .WhereEquals("State", 1)
                                                        .TopN(1)
                                                        .LastOrDefault();
                if (item2 != null)
                {
                    AlbumRowGuid = ValidationHelper.GetString(item2.GetValue("ItemGuid"), "");
                }
                if (AlbumRowGuid != "")
                {
                    QueryDataParameters parameters = new QueryDataParameters();
                    parameters.Add("@AlbumRowGUID", AlbumRowGuid);

                    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_DeleteAlbum", parameters, QueryTypeEnum.StoredProcedure);
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.OK,
                        message = "Success",
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
                    {
                        status = HttpStatusCode.NotFound,
                        errorCode = HttpStatusCode.NotFound.ToString(),
                        description = "Project ID Not Found",
                    });
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new CustomResponse
            {
                status = HttpStatusCode.NotFound,
                errorCode = HttpStatusCode.NotFound.ToString(),
                description = "User ID Not Found",
            });
        }

        //---------------------------------------------------Method for Api--------------------------------------//

        public static Album GetAlbumwihPrice(int userid)
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@LoginUserID", userid);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_GetAlbumDetail", parameters, QueryTypeEnum.StoredProcedure);

            List<Album> item = ds.Tables[0].AsEnumerable().Select(dataRow => new Album
            {
                AlbumID = dataRow.Field<int>("ItemID"),
                AlbumRowGUID = dataRow.Field<Guid>("ItemGUID"),
                AlbumCreatedDate = dataRow.Field<DateTime>("ItemCreatedWhen"),
                LoggedinUserID = dataRow.Field<int>("UserID"),
                AlbumStatus = dataRow.Field<bool>("State"),
                AlbumPageType = dataRow.Field<string>("PageType"),
                AlbumSize = dataRow.Field<string>("AlbumPageSize"),
                AlbumPageCountCode = dataRow.Field<string>("NoofPages"),
                ImagesName = dataRow.Field<string>("imagesname"),
                Price = dataRow.Field<int>("Price"),
                ImagesLocation = dataRow.Field<string>("imageslocation"),
            }).ToList();

            var model = item.FirstOrDefault();

            return model;
        }

        public static Album GetAlbumbySkuID(int SkuID)
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@skuid", SkuID);

            DataSet ds = ConnectionHelper.ExecuteQuery("getuseridbysku", parameters, QueryTypeEnum.StoredProcedure);

            List<Album> item = ds.Tables[0].AsEnumerable().Select(dataRow => new Album
            {
                AlbumID = dataRow.Field<int>("ItemID"),
                AlbumRowGUID = dataRow.Field<Guid>("ItemGUID"),
                AlbumCreatedDate = dataRow.Field<DateTime>("ItemCreatedWhen"),
                LoggedinUserID = dataRow.Field<int>("UserID"),
                AlbumStatus = dataRow.Field<bool>("State"),
                AlbumPageType = dataRow.Field<string>("PageType"),
                AlbumSize = dataRow.Field<string>("AlbumPageSize"),
                AlbumPageCountCode = dataRow.Field<string>("NoofPages"),
                ImagesName = dataRow.Field<string>("imagesname"),
                Price = dataRow.Field<int>("Price"),
                ImagesLocation = dataRow.Field<string>("imageslocation"),
                SKUID = dataRow.Field<string>("SKUID"),
            }).ToList();

            var model = item.FirstOrDefault();

            return model;
        }

        public int ManageSKU(decimal Price, int AlbumID, string SKUImagePath)
        {

            DepartmentInfo department = DepartmentInfoProvider.GetDepartmentInfo("AlbumPrinting", SiteContext.CurrentSiteName);

            // Creates a new product object
            SKUInfo newProduct = new SKUInfo();

            // Sets the product properties
            newProduct.SKUName = "Album Product";
            newProduct.SKUPrice = Price;
            newProduct.SKUImagePath = SKUImagePath;
            newProduct.SKUEnabled = true;
            if (department != null)
            {
                newProduct.SKUDepartmentID = department.DepartmentID;
            }
            newProduct.SKUSiteID = SiteContext.CurrentSiteID;

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

                // Sets a value for a field of the given product page type
                node.SetValue("Name", name);

                // Assigns the product to the page
                node.NodeSKUID = newProduct.SKUID;

                // Saves the product page to the database
                node.Insert(parent);
            }

            return newProduct.SKUID;
        }

        //private AlbumViewModel GetMasterData(int userid)
        //{
        //    AlbumViewModel albumViewModel = new AlbumViewModel();
        //    string albumDetail = "Printforme.Album";

        //    DataClassInfo albumDetailInfo = DataClassInfoProvider.GetDataClassInfo(albumDetail);

        //    CustomTableItem item2 = CustomTableItemProvider.GetItems(albumDetail)
        //                                                .WhereEquals("UserID", userid)
        //                                                .TopN(1)
        //                                                .FirstOrDefault();
        //    AlbumMaster objalbumservicemodel = new AlbumMaster()
        //    {
        //        AlbumID = ValidationHelper.GetInteger(item2.GetValue("ItemID"), 0),
        //        AlbumCreatedDate = ValidationHelper.GetDateTime(item2.GetValue("ItemCreatedWhen"), System.DateTime.Now),
        //        AlbumRowGUID = ValidationHelper.GetGuid(item2.GetValue("ItemGUID"), Guid.NewGuid()),
        //        LoggedinUserID = ValidationHelper.GetInteger(item2.GetValue("UserID"), 0),
        //        AlbumSize = ValidationHelper.GetString(item2.GetValue("PageSizeID"), ""),
        //        AlbumPageType= ValidationHelper.GetString(item2.GetValue("PaperMaterialID"), ""),
        //        AlbumPageCountCode = ValidationHelper.GetString(item2.GetValue("PageCountID"), ""),
        //        AlbumStatus = ValidationHelper.GetBoolean(item2.GetValue("State"), true)
        //    };
        //    albumViewModel.CreateAlbum = objalbumservicemodel;
        //    albumViewModel.CreateAlbum.AlbumID = objalbumservicemodel.AlbumID;
        //    albumViewModel.CreateAlbum.AlbumRowGUID = objalbumservicemodel.AlbumRowGUID;
        //    string albumrowid = albumViewModel.CreateAlbum.AlbumRowGUID.ToString();

        //    albumViewModel.DownloadPhotoQueue_Result = GetAlbumPhotoQueue(albumrowid);
        //    albumViewModel.GetAlbumPages_Result = GetAlbumPages(albumrowid);
        //    albumViewModel.GetAlbumPagesWithPhoto_Result = GetAlbumPagesWithPhoto(albumrowid);

        //    List<MainAlbumPageo_Result> objListMainAlbumPageo_Result = new List<MainAlbumPageo_Result>();
        //    MainAlbumPageo_Result objMainAlbumPageo_Result;


        //    foreach (var item in albumViewModel.GetAlbumPages_Result)
        //    {
        //        objMainAlbumPageo_Result = new MainAlbumPageo_Result();
        //        objMainAlbumPageo_Result.AlbumPageRowGUID = item.AlbumPageRowGUID;
        //        objMainAlbumPageo_Result.PageLayoutCode = item.PageLayoutCode;
        //        objMainAlbumPageo_Result.PhotoCount = item.PhotoCount;

        //        var photo = albumViewModel.GetAlbumPagesWithPhoto_Result.Where(o => o.APItemID == item.APItemID);

        //        if (item.PageLayoutCode == "LAYOUT01")
        //        {
        //            TemplateLayout1 objTemplateLayout1 = new TemplateLayout1();
        //            objTemplateLayout1.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
        //            objTemplateLayout1.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
        //            objTemplateLayout1.PhotoCount = photo.ElementAt(0).PhotoCount;
        //            objTemplateLayout1.Name_1 = photo.ElementAt(0).Name;
        //            objTemplateLayout1.Sequence_1 = photo.ElementAt(0).Sequence;
        //            objTemplateLayout1.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
        //            objTemplateLayout1.Name_2 = photo.ElementAt(1).Name;
        //            objTemplateLayout1.Sequence_2 = photo.ElementAt(1).Sequence;
        //            objTemplateLayout1.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
        //            objMainAlbumPageo_Result.LayoutOne = objTemplateLayout1;
        //        }
        //        else if (item.PageLayoutCode == "LAYOUT02")
        //        {
        //            TemplateLayout2 objTemplateLayout2 = new TemplateLayout2();
        //            objTemplateLayout2.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
        //            objTemplateLayout2.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
        //            objTemplateLayout2.PhotoCount = photo.ElementAt(0).PhotoCount;
        //            objTemplateLayout2.Name_1 = photo.ElementAt(0).Name;
        //            objTemplateLayout2.Sequence_1 = photo.ElementAt(0).Sequence;
        //            objTemplateLayout2.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
        //            objMainAlbumPageo_Result.LayoutTwo = objTemplateLayout2;

        //        }
        //        else if (item.PageLayoutCode == "LAYOUT03")
        //        {
        //            TemplateLayout3 objTemplateLayout3 = new TemplateLayout3();
        //            objTemplateLayout3.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
        //            objTemplateLayout3.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
        //            objTemplateLayout3.PhotoCount = photo.ElementAt(0).PhotoCount;
        //            objTemplateLayout3.Name_1 = photo.ElementAt(0).Name;
        //            objTemplateLayout3.Sequence_1 = photo.ElementAt(0).Sequence;
        //            objTemplateLayout3.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
        //            objTemplateLayout3.Name_2 = photo.ElementAt(1).Name;
        //            objTemplateLayout3.Sequence_2 = photo.ElementAt(1).Sequence;
        //            objTemplateLayout3.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
        //            objMainAlbumPageo_Result.LayoutThree = objTemplateLayout3;
        //        }
        //        if (item.PageLayoutCode == "LAYOUT04")
        //        {
        //            TemplateLayout4 objTemplateLayout4 = new TemplateLayout4();
        //            objTemplateLayout4.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
        //            objTemplateLayout4.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
        //            objTemplateLayout4.PhotoCount = photo.ElementAt(0).PhotoCount;
        //            objTemplateLayout4.Name_1 = photo.ElementAt(0).Name;
        //            objTemplateLayout4.Sequence_1 = photo.ElementAt(0).Sequence;
        //            objTemplateLayout4.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
        //            objTemplateLayout4.Name_2 = photo.ElementAt(1).Name;
        //            objTemplateLayout4.Sequence_2 = photo.ElementAt(1).Sequence;
        //            objTemplateLayout4.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
        //            objTemplateLayout4.Name_3 = photo.ElementAt(2).Name;
        //            objTemplateLayout4.Sequence_3 = photo.ElementAt(2).Sequence;
        //            objTemplateLayout4.AlbumPagePhotoRowGUID_3 = photo.ElementAt(2).RowGUID;
        //            objMainAlbumPageo_Result.LayoutFour = objTemplateLayout4;
        //        }
        //        if (item.PageLayoutCode == "LAYOUT05")
        //        {
        //            TemplateLayout5 objTemplateLayout5 = new TemplateLayout5();
        //            objTemplateLayout5.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
        //            objTemplateLayout5.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
        //            objTemplateLayout5.PhotoCount = photo.ElementAt(0).PhotoCount;
        //            objTemplateLayout5.Name_1 = photo.ElementAt(0).Name;
        //            objTemplateLayout5.Sequence_1 = photo.ElementAt(0).Sequence;
        //            objTemplateLayout5.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
        //            objTemplateLayout5.Name_2 = photo.ElementAt(1).Name;
        //            objTemplateLayout5.Sequence_2 = photo.ElementAt(1).Sequence;
        //            objTemplateLayout5.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
        //            objTemplateLayout5.Name_3 = photo.ElementAt(2).Name;
        //            objTemplateLayout5.Sequence_3 = photo.ElementAt(2).Sequence;
        //            objTemplateLayout5.AlbumPagePhotoRowGUID_3 = photo.ElementAt(2).RowGUID;
        //            objTemplateLayout5.Name_4 = photo.ElementAt(3).Name;
        //            objTemplateLayout5.Sequence_4 = photo.ElementAt(3).Sequence;
        //            objTemplateLayout5.AlbumPagePhotoRowGUID_4 = photo.ElementAt(3).RowGUID;

        //            objMainAlbumPageo_Result.LayoutFive = objTemplateLayout5;
        //        }
        //        if (item.PageLayoutCode == "LAYOUT06")
        //        {
        //            TemplateLayout6 objTemplateLayout6 = new TemplateLayout6();
        //            objTemplateLayout6.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
        //            objTemplateLayout6.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
        //            objTemplateLayout6.PhotoCount = photo.ElementAt(0).PhotoCount;
        //            objTemplateLayout6.Name_1 = photo.ElementAt(0).Name;
        //            objTemplateLayout6.Sequence_1 = photo.ElementAt(0).Sequence;
        //            objTemplateLayout6.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
        //            objTemplateLayout6.Name_2 = photo.ElementAt(1).Name;
        //            objTemplateLayout6.Sequence_2 = photo.ElementAt(1).Sequence;
        //            objTemplateLayout6.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
        //            objTemplateLayout6.Name_3 = photo.ElementAt(2).Name;
        //            objTemplateLayout6.Sequence_3 = photo.ElementAt(2).Sequence;
        //            objTemplateLayout6.AlbumPagePhotoRowGUID_3 = photo.ElementAt(2).RowGUID;
        //            objTemplateLayout6.Name_4 = photo.ElementAt(3).Name;
        //            objTemplateLayout6.Sequence_4 = photo.ElementAt(3).Sequence;
        //            objTemplateLayout6.AlbumPagePhotoRowGUID_4 = photo.ElementAt(3).RowGUID;

        //            objTemplateLayout6.Name_5 = photo.ElementAt(4).Name;
        //            objTemplateLayout6.Sequence_5 = photo.ElementAt(4).Sequence;
        //            objTemplateLayout6.AlbumPagePhotoRowGUID_5 = photo.ElementAt(4).RowGUID;

        //            objTemplateLayout6.Name_6 = photo.ElementAt(5).Name;
        //            objTemplateLayout6.Sequence_6 = photo.ElementAt(5).Sequence;
        //            objTemplateLayout6.AlbumPagePhotoRowGUID_6 = photo.ElementAt(5).RowGUID;
        //            objMainAlbumPageo_Result.LayoutSix = objTemplateLayout6;
        //        }
        //        objListMainAlbumPageo_Result.Add(objMainAlbumPageo_Result);
        //    }
        //    albumViewModel.GetMainAlbumPage_Result = objListMainAlbumPageo_Result;
        //    return albumViewModel;
        //}

        //private List<AlbumPagePhotos> GetAlbumPagesWithPhoto(string id)
        //{
        //    QueryDataParameters parameters = new QueryDataParameters();
        //    parameters.Add("@AlbumRowGUID", id);

        //    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_GetAlbumPagesWithPhoto", parameters, QueryTypeEnum.StoredProcedure);

        //    List<AlbumPagePhotos> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumPagePhotos
        //    {
        //        APItemID = dataRow.Field<int>("ItemID"),
        //        AlbumPageRowGUID = dataRow.Field<Guid>("AlbumPageRowGUID"),
        //        APAlbumID = dataRow.Field<int>("AlbumID"),
        //        APLItemID = dataRow.Field<int>("AlbumPageLayoutID"),
        //        DefaultLayout = dataRow.Field<bool>("DefaultLayout"),
        //        PageLayoutCode = dataRow.Field<string>("PageLayoutCode"),
        //        PhotoCount = dataRow.Field<int>("PhotoCount"),
        //        AlbumPagePhotoID = dataRow.Field<int>("AlbumPagePhotoID"),
        //        Name = dataRow.Field<string>("Name"),
        //        PhotoLocation = dataRow.Field<string>("PhotoLocation"),
        //        Sequence = dataRow.Field<int>("Sequence"),
        //        RowGUID = dataRow.Field<Guid>("RowGUID")
        //    }).ToList();


        //    return item;
        //}

        //private List<AlbumImages> GetAlbumPhotoQueue(string id)
        //{
        //    QueryDataParameters parameters = new QueryDataParameters();
        //    parameters.Add("@AlbumRowGUID", id);

        //    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_DownloadPhotoQueue", parameters, QueryTypeEnum.StoredProcedure);


        //    List<AlbumImages> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumImages
        //    {
        //        ItemID = dataRow.Field<int>("ItemID"),
        //        ItemGUID = dataRow.Field<Guid>("ItemGUID"),
        //        AlbumID = dataRow.Field<int>("AlbumID"),
        //        ImagesName = dataRow.Field<string>("Name"),
        //        AlbumImagesLocation = dataRow.Field<string>("PhotoLocation"),
        //        Isactive = dataRow.Field<bool>("IsPPIStatus")
        //    }).ToList();

        //    return item;
        //}

        private List<AlbumPages> GetAlbumPages(string id)
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumRowGUID", id);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_GetAlbumPages", parameters, QueryTypeEnum.StoredProcedure);

            List<AlbumPages> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumPages
            {
                AlbumPageID = dataRow.Field<int>("ItemID"),
            }).ToList();


            return item;
        }

        private int GetAlbumid(int userid)
        {
            int albumid = 0;
            string albumDetail = "Printforme.Album";

            DataClassInfo albumDetailInfo = DataClassInfoProvider.GetDataClassInfo(albumDetail);

            CustomTableItem item2 = CustomTableItemProvider.GetItems(albumDetail)
                                                        .WhereEquals("UserID", userid)
                                                        .WhereEquals("State", 1)
                                                        .TopN(1)
                                                        .LastOrDefault();
            if (item2 != null)
            {
                albumid = ValidationHelper.GetInteger(item2.GetValue("ItemID"), 0);
            }
            return albumid;
        }

        
    }
}
