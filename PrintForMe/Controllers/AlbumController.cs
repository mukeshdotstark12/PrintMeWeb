using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.PrintForMe;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PrintForMe.Helpers;
using PrintForMe.Models.Album;
using PrintForMe.Models.Services;
using PrintForMe.Models.Store;
using Syncfusion.EJ2.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class AlbumController : Controller
    {
        private readonly static string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        //public AlbumViewModel objAlbumViewModel;

        public ActionResult AlbumsService(string serviceId)
        {
            Album objAlbum = GetAlbumByLoggedinUser();
            if (serviceId == "4_")
            {
                var model1 = new AlbumServiceModel()
                {
                    AlbumID = objAlbum.AlbumID,
                MinimumPrice = 300,
                    AlbumSize = FillComboBox.GetAlbumSize(),
                    AlbumFormat = GetAlbumFormat(),
                    PaperMaterial = GetPaperMaterial(),
                    ServiceID = Convert.ToInt32(4)
                };

                
                return View(model1);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");

            }
            
            if (objAlbum != null)
            {
                var deleteperiod = DeleteAlbumProvider.GetDeleteAlbums()
                .Path("/Home/Delete-Album", PathTypeEnum.Children)
                .Culture(mCultureName)
                .Columns("DeleteAlbumID", "DeleteAlbumDate", "DeleteAlbumNoofDays")
                .OrderBy("NodeOrder");


                IQueryable<CMS.DocumentEngine.Types.PrintForMe.DeleteAlbum> deleteperiod1 = deleteperiod.Select(item => new CMS.DocumentEngine.Types.PrintForMe.DeleteAlbum()
                {
                    DeleteAlbumDate = item.DeleteAlbumDate,
                    DeleteAlbumNoofDays = item.DeleteAlbumNoofDays,
                });

                var data = deleteperiod1.FirstOrDefault();
                DateTime date = objAlbum.AlbumCreatedDate.AddDays(Convert.ToDouble(data.DeleteAlbumNoofDays));

                string customTableClassName = "PrintForMe.Album";
                CustomTableItem item1 = null;
                DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
                if (customTable != null)
                { 
                    item1 = CustomTableItemProvider.GetItems(customTableClassName)
                                                                        .WhereEquals("ItemID", objAlbum.AlbumID)
                                                                        .TopN(1)
                                                                        .FirstOrDefault();

                    if(item1 != null)
                    {
                        DateTime itemText = ValidationHelper.GetDateTime(item1.GetValue("ItemModifyDate"), DateTime.Now);

                        if(date <= itemText)
                        {
                            QueryDataParameters parameters = new QueryDataParameters();
                            parameters.Add("@AlbumRowGUID", objAlbum.AlbumRowGUID);

                            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_DeleteAlbum", parameters, QueryTypeEnum.StoredProcedure);

                            //item1.SetValue("Status", false);

                            //item1.Update();

                            var path = Server.MapPath("~/Album/" + objAlbum.AlbumID);
                            if (Directory.Exists(path))
                            {
                                try
                                {
                                    Directory.Delete(path, recursive: true);
                                }
                                catch
                                {
                                    Thread.Sleep(1000);     
                                    Directory.Delete(path, recursive: true);
                                }
                            }
                        }
                        item1.SetValue("ItemModifyDate", DateTime.Now);

                        item1.Update();
                    }
                    
                }

                return RedirectToAction("ArrangeAlbum", "Album");
            }

            var model = new AlbumServiceModel()
            {
                MinimumPrice = 300,
                AlbumSize = FillComboBox.GetAlbumSize(),
                AlbumFormat = GetAlbumFormat(),
                PaperMaterial = GetPaperMaterial(),
                ServiceID = Convert.ToInt32(serviceId)
            };

            if (TempData["ErrorStatus"] != null)
            {
                ViewBag.Error = TempData["ErrorStatus"];
            }
            return View(model);
        }


        [Authorize]
        public ActionResult ArrangeAlbum(AlbumServiceModel albumServiceModel)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (albumServiceModel.ServiceID != 0)
            {
                if (albumServiceModel.SelectedSize == null)
                {
                    TempData["ErrorStatus"] = "Mandatory to start design a new album";
                    return RedirectToAction("AlbumsService", "Album", new { serviceId = 4 });
                }
                if (albumServiceModel.SelectedNoofpages == null)
                {
                    TempData["ErrorStatus"] = "Mandatory to start design a new album";
                    return RedirectToAction("AlbumsService", "Album", new { serviceId = 4 });
                }
                if (albumServiceModel.SelectedPaperMaterial == null)
                {
                    TempData["ErrorStatus"] = "Mandatory to start design a new album";
                    return RedirectToAction("AlbumsService", "Album", new { serviceId = 4 });
                }
            }

            Album objAlbum = GetAlbumByLoggedinUser();
            if (objAlbum == null)
            {
                objAlbum = new Album();
                objAlbum.LoggedinUserID = HttpContext.GetOwinContext().Get<UserManager>().FindByName(User.Identity.Name).Id;

                objAlbum.AlbumPageCountCode = albumServiceModel.SelectedNoofpages;
                objAlbum.AlbumSize = albumServiceModel.SelectedSize;
                objAlbum.AlbumPageType = albumServiceModel.SelectedPaperMaterial;
                AddAlbum(objAlbum);
            }

            if (albumServiceModel.AlbumID != 0)
            {
                int size = Convert.ToInt32(objAlbum.AlbumSize);
                QueryDataParameters parameters = new QueryDataParameters();
                QueryDataParameters parameter = new QueryDataParameters();
                objAlbum = new Album();
                objAlbum.LoggedinUserID = HttpContext.GetOwinContext().Get<UserManager>().FindByName(User.Identity.Name).Id;
                objAlbum.AlbumSize = albumServiceModel.SelectedSize;
                objAlbum.AlbumPageType = albumServiceModel.SelectedPaperMaterial;
                objAlbum.AlbumPageCountCode = albumServiceModel.SelectedNoofpages;
                string customTableClassName = "PrintForMe.AlbumSize";
                int noofpages = 0;
                DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
                if (customTable != null)
                {
                    CustomTableItem item2 = CustomTableItemProvider.GetItems(customTableClassName)
                                                                        .WhereEquals("ItemGUID", albumServiceModel.SelectedNoofpages)
                                                                        .TopN(1)
                                                                        .FirstOrDefault();

                    noofpages = ValidationHelper.GetInteger(item2.GetValue("Size"), 0);
                }
                if(noofpages == size)
                {
                    parameter.Add("@AlbumId", albumServiceModel.AlbumID);
                    parameter.Add("@PageSizeCode", albumServiceModel.SelectedSize);
                    parameter.Add("@PaperMaterialCode", albumServiceModel.SelectedPaperMaterial);
                    parameter.Add("@PaperQuantityCode", albumServiceModel.SelectedNoofpages);

                    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_Update", parameters, QueryTypeEnum.StoredProcedure);
                }
                if (Convert.ToInt32(size) > Convert.ToInt32(noofpages))
                {
                    parameters.Add("@AlbumId", albumServiceModel.AlbumID);
                    parameters.Add("@Msg", "Delete");
                    parameter.Add("@AlbumId", albumServiceModel.AlbumID);
                    parameter.Add("@PageSizeCode", albumServiceModel.SelectedSize);
                    parameter.Add("@PaperMaterialCode", albumServiceModel.SelectedPaperMaterial);
                    parameter.Add("@PaperQuantityCode", albumServiceModel.SelectedNoofpages);

                    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_UpdateAlvum_AddPageSize", parameters, QueryTypeEnum.StoredProcedure);
                    DataSet ds1 = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_Update", parameter, QueryTypeEnum.StoredProcedure);
                }
                if (Convert.ToInt32(size) < Convert.ToInt32(noofpages))
                {
                    parameters.Add("@AlbumId", albumServiceModel.AlbumID);
                    parameters.Add("@Msg", "Add");
                    parameter.Add("@AlbumId", albumServiceModel.AlbumID);
                    parameter.Add("@PageSizeCode", albumServiceModel.SelectedSize);
                    parameter.Add("@PaperMaterialCode", albumServiceModel.SelectedPaperMaterial);
                    parameter.Add("@PaperQuantityCode", albumServiceModel.SelectedNoofpages);

                    DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_UpdateAlvum_AddPageSize", parameters, QueryTypeEnum.StoredProcedure);
                    DataSet ds1 = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_Update", parameter, QueryTypeEnum.StoredProcedure);
                }
            }
            

            AlbumViewModel objAlbumViewModel = GetMasterData();

            ViewBag.QueueImages = "/Album/" + objAlbumViewModel.PhotoAlbum.AlbumID + "/Queue/";
            ViewBag.AlbumImages = "/Album/" + objAlbumViewModel.PhotoAlbum.AlbumID + "/AlbumPhoto/";
            TempData["AlbumViewModel"] = objAlbumViewModel;

            return View(objAlbumViewModel);

        }


        [Authorize]
        private List<PaperMaterialModel> GetPaperMaterial()
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

                return papermaterial;
            }
            return null;
        }

        [Authorize]
        [HttpGet]
        public ActionResult ResetImage(string pagerowguid, string ImageSrc)
        {
            QueryDataParameters parameters = new QueryDataParameters();
            string one = pagerowguid.ToString();
            parameters.Add("@AlbumPagePhotoGUID", one);
            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_GetResetImage", parameters, QueryTypeEnum.StoredProcedure);
            return Json("Reset Your Image", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {

                AlbumViewModel objAlbumViewModel = ((AlbumViewModel)TempData["AlbumViewModel"]);
                TempData.Keep();

                var path = Server.MapPath("~/Album/" + objAlbumViewModel.PhotoAlbum.AlbumID + "/Queue/");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    var file = Request.Files[i];
                    string fileName;

                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
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
                    var fileLength = new FileInfo(fileName).Length;
                    Image image = new Bitmap(fileName, true);
                    float ppi = image.HorizontalResolution;
                    float widthInInches = image.PhysicalDimension.Width / ppi;


                    QueryDataParameters parameters = new QueryDataParameters();
                    parameters.Add("@AlbumRowGUID", objAlbumViewModel.PhotoAlbum.AlbumRowGUID);
                    parameters.Add("@Name", newFilename);
                    parameters.Add("@PhotoLocation", fileName);
                    //if(fileLength > 3145728)
                    //{
                    //    parameters.Add("@IsPPIStatus", 2);
                    //}
                    if (ppi > 240)
                    {
                        parameters.Add("@IsPPIStatus", 1);
                    }
                    else
                    {
                        parameters.Add("@IsPPIStatus", 0);
                    }
                    ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_UploadPhotoQueue", parameters, QueryTypeEnum.StoredProcedure);


                }
                return Json("Your files uploaded successfully", JsonRequestBehavior.AllowGet);
            }
            return Json("Please Select Valid Images", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadFiles2()
        {
            if (Request.Files.Count > 0)
            {
                var PhotoRowGUID = new Guid(Request.Form["PhotoRowGUID"]);
                var ImageSrc = Request.Form["ImageSrc"].ToString();

                AlbumViewModel objAlbumViewModel = ((AlbumViewModel)TempData["AlbumViewModel"]);
                TempData.Keep();

                var path = Server.MapPath("~/Album/" + objAlbumViewModel.PhotoAlbum.AlbumID + "/AlbumPhoto/");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                HttpFileCollectionBase files = Request.Files;

                var file = Request.Files[0];
                string fileName = file.FileName;
                string newFilename = Guid.NewGuid().ToString();
                FileInfo fInfo = new FileInfo(fileName);
                newFilename = string.Format("{0}{1}", newFilename, fInfo.Extension);
                string ext = newFilename + ".jpg";
                fileName = Path.Combine(path, ext).Replace("\\", "/").Replace("//", "/");
                file.SaveAs(fileName);
                //if (ImageSrc.Contains("?"))
                //{
                //    fileName = Path.Combine(path, ImageSrc.Split('?')[0].Split('/')[4]).Replace("\\", "/").Replace("//", "/");
                //}
                //else
                //{
                //    fileName = Path.Combine(path, ImageSrc.Split('/')[4]).Replace("\\", "/").Replace("//", "/");
                //}

                //file.SaveAs(fileName);

                string customTableClassName = "PrintForMe.AlbumPagePhoto";

                DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
                if (customTable != null)
                {
                    var customTableData = CustomTableItemProvider.GetItems(customTableClassName)
                                                            .WhereEquals("ItemGUID", PhotoRowGUID);

                    foreach (CustomTableItem item in customTableData)
                    {
                        string name = ValidationHelper.GetString(item.GetValue("Name"), "");
                        string Photolocation = ValidationHelper.GetString(item.GetValue("PhotoLocation"), "");

                        item.SetValue("Name", ext);
                        //item.SetValue("PhotoLocation", fileName);

                        item.Update();
                    }
                }

                return Json(fileName, JsonRequestBehavior.AllowGet);
            }
            return Json("Please Select Valid Images", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public JsonResult DownloadPhotoQueue(Guid AlbumRowGUID)
        {

            var DownloadPhotoQueue_Result = GetAlbumPhotoQueue(AlbumRowGUID);
            return Json(DownloadPhotoQueue_Result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public JsonResult DeletePhotoFromQueue(Guid albumPhotoQueueRowGUID)
        {
            string albumrowid = albumPhotoQueueRowGUID.ToString();
            var ObjPhotoAlbum_DeletePhotoFromQueue_Result = DeletePhotoFromQueue(albumrowid);
            return Json(ObjPhotoAlbum_DeletePhotoFromQueue_Result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public JsonResult AddPhototoAlbumPageFromQueue(Guid albumPhotoQueueRowGUID, Guid albumPagePhotoGUID)
        {
            QueryDataParameters parameters = new QueryDataParameters();
            string one = albumPhotoQueueRowGUID.ToString();
            string two = albumPagePhotoGUID.ToString();
            parameters.Add("@AlbumPhotoQueueRowGUID", one);
            parameters.Add("@AlbumPagePhotoGUID", two);
            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_AddPhototoAlbumPageFromQueue", parameters, QueryTypeEnum.StoredProcedure);

            int albumid = Convert.ToInt32(ds.Tables[0].Rows[0]["AlbumID"]);
            string imagename = Convert.ToString(ds.Tables[0].Rows[0]["Name"]);
            string imagelocation = Convert.ToString(ds.Tables[0].Rows[0]["PhotoLocation"]);
            var path = Server.MapPath("~/Album/" + albumid + "/AlbumPhoto/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string oldpath = imagelocation;
            string copypath = System.IO.Path.Combine(path, imagename);
            System.IO.File.Copy(oldpath, copypath, true);
            
            AlbumViewModel objAlbumViewModel = GetMasterData();
            return Json(objAlbumViewModel.GetMainAlbumPage_Result, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [HttpGet]
        public JsonResult GetMainAlbumPageResult()
        {
            AlbumViewModel objAlbumViewModel = GetMasterData();
            return Json(objAlbumViewModel.GetMainAlbumPage_Result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public JsonResult ChangeAlbumPageLayout(Guid AlbumPageRowGUID, string PageLayoutCode)
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumPageRowGUID", AlbumPageRowGUID);
            parameters.Add("@PageLayoutCode", PageLayoutCode);
            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_ChangeAlbumPageLayout", parameters, QueryTypeEnum.StoredProcedure);

            AlbumViewModel objAlbumViewModel = GetMasterData();
            return Json(objAlbumViewModel.GetMainAlbumPage_Result, JsonRequestBehavior.AllowGet);

        }

        

        private AlbumViewModel GetMasterData()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }
            int loggedinUserID = HttpContext.GetOwinContext().Get<UserManager>().FindByName(User.Identity.Name).Id;
            Album objAlbum = GetAlbumByLoggedinUser();

            AlbumViewModel albumViewModel = new AlbumViewModel();

            albumViewModel.PhotoAlbum = objAlbum;
            albumViewModel.DownloadPhotoQueue_Result = GetAlbumPhotoQueue(objAlbum.AlbumRowGUID);
            albumViewModel.GetAlbumPages_Result = GetAlbumPages(objAlbum.AlbumRowGUID);
            albumViewModel.GetAlbumPagesWithPhoto_Result = GetAlbumPagesWithPhoto(objAlbum.AlbumRowGUID);

            List<MainAlbumPageo_Result> objListMainAlbumPageo_Result = new List<MainAlbumPageo_Result>();
            MainAlbumPageo_Result objMainAlbumPageo_Result;

            Random rnd = new Random();
            foreach (var item in albumViewModel.GetAlbumPages_Result)
            {
                objMainAlbumPageo_Result = new MainAlbumPageo_Result();
                objMainAlbumPageo_Result.AlbumPageRowGUID = item.AlbumPageRowGUID;
                objMainAlbumPageo_Result.PageLayoutCode = item.PageLayoutCode;
                objMainAlbumPageo_Result.PhotoCount = item.PhotoCount;

                var photo = albumViewModel.GetAlbumPagesWithPhoto_Result.Where(o => o.APItemID == item.APItemID);

                if (item.PageLayoutCode == "LAYOUT01")
                {
                    TemplateLayout1 objTemplateLayout1 = new TemplateLayout1();
                    objTemplateLayout1.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
                    objTemplateLayout1.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
                    objTemplateLayout1.PhotoCount = photo.ElementAt(0).PhotoCount;
                    objTemplateLayout1.Name_1 = photo.ElementAt(0).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout1.Sequence_1 = photo.ElementAt(0).Sequence;
                    objTemplateLayout1.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
                    objTemplateLayout1.Name_2 = photo.ElementAt(1).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout1.Sequence_2 = photo.ElementAt(1).Sequence;
                    objTemplateLayout1.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
                    objMainAlbumPageo_Result.LayoutOne = objTemplateLayout1;
                }

                else if (item.PageLayoutCode == "LAYOUT02")
                {
                    TemplateLayout2 objTemplateLayout2 = new TemplateLayout2();
                    objTemplateLayout2.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
                    objTemplateLayout2.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
                    objTemplateLayout2.PhotoCount = photo.ElementAt(0).PhotoCount;
                    objTemplateLayout2.Name_1 = photo.ElementAt(0).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout2.Sequence_1 = photo.ElementAt(0).Sequence;
                    objTemplateLayout2.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
                    objMainAlbumPageo_Result.LayoutTwo = objTemplateLayout2;

                }
                else if (item.PageLayoutCode == "LAYOUT03")
                {
                    TemplateLayout3 objTemplateLayout3 = new TemplateLayout3();
                    objTemplateLayout3.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
                    objTemplateLayout3.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
                    objTemplateLayout3.PhotoCount = photo.ElementAt(0).PhotoCount;
                    objTemplateLayout3.Name_1 = photo.ElementAt(0).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout3.Sequence_1 = photo.ElementAt(0).Sequence;
                    objTemplateLayout3.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
                    objTemplateLayout3.Name_2 = photo.ElementAt(1).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout3.Sequence_2 = photo.ElementAt(1).Sequence;
                    objTemplateLayout3.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
                    objMainAlbumPageo_Result.LayoutThree = objTemplateLayout3;
                }
                if (item.PageLayoutCode == "LAYOUT04")
                {
                    TemplateLayout4 objTemplateLayout4 = new TemplateLayout4();
                    objTemplateLayout4.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
                    objTemplateLayout4.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
                    objTemplateLayout4.PhotoCount = photo.ElementAt(0).PhotoCount;
                    objTemplateLayout4.Name_1 = photo.ElementAt(0).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout4.Sequence_1 = photo.ElementAt(0).Sequence;
                    objTemplateLayout4.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
                    objTemplateLayout4.Name_2 = photo.ElementAt(1).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout4.Sequence_2 = photo.ElementAt(1).Sequence;
                    objTemplateLayout4.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
                    objTemplateLayout4.Name_3 = photo.ElementAt(2).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout4.Sequence_3 = photo.ElementAt(2).Sequence;
                    objTemplateLayout4.AlbumPagePhotoRowGUID_3 = photo.ElementAt(2).RowGUID;
                    objMainAlbumPageo_Result.LayoutFour = objTemplateLayout4;
                }
                if (item.PageLayoutCode == "LAYOUT05")
                {
                    TemplateLayout5 objTemplateLayout5 = new TemplateLayout5();
                    objTemplateLayout5.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
                    objTemplateLayout5.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
                    objTemplateLayout5.PhotoCount = photo.ElementAt(0).PhotoCount;
                    objTemplateLayout5.Name_1 = photo.ElementAt(0).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout5.Sequence_1 = photo.ElementAt(0).Sequence;
                    objTemplateLayout5.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
                    objTemplateLayout5.Name_2 = photo.ElementAt(1).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout5.Sequence_2 = photo.ElementAt(1).Sequence;
                    objTemplateLayout5.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
                    objTemplateLayout5.Name_3 = photo.ElementAt(2).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout5.Sequence_3 = photo.ElementAt(2).Sequence;
                    objTemplateLayout5.AlbumPagePhotoRowGUID_3 = photo.ElementAt(2).RowGUID;
                    objTemplateLayout5.Name_4 = photo.ElementAt(3).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout5.Sequence_4 = photo.ElementAt(3).Sequence;
                    objTemplateLayout5.AlbumPagePhotoRowGUID_4 = photo.ElementAt(3).RowGUID;

                    objMainAlbumPageo_Result.LayoutFive = objTemplateLayout5;
                }
                if (item.PageLayoutCode == "LAYOUT06")
                {
                    TemplateLayout6 objTemplateLayout6 = new TemplateLayout6();
                    objTemplateLayout6.AlbumPageRowGUID = photo.ElementAt(0).AlbumPageRowGUID;
                    objTemplateLayout6.PageLayoutCode = photo.ElementAt(0).PageLayoutCode;
                    objTemplateLayout6.PhotoCount = photo.ElementAt(0).PhotoCount;
                    objTemplateLayout6.Name_1 = photo.ElementAt(0).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout6.Sequence_1 = photo.ElementAt(0).Sequence;
                    objTemplateLayout6.AlbumPagePhotoRowGUID_1 = photo.ElementAt(0).RowGUID;
                    objTemplateLayout6.Name_2 = photo.ElementAt(1).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout6.Sequence_2 = photo.ElementAt(1).Sequence;
                    objTemplateLayout6.AlbumPagePhotoRowGUID_2 = photo.ElementAt(1).RowGUID;
                    objTemplateLayout6.Name_3 = photo.ElementAt(2).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout6.Sequence_3 = photo.ElementAt(2).Sequence;
                    objTemplateLayout6.AlbumPagePhotoRowGUID_3 = photo.ElementAt(2).RowGUID;
                    objTemplateLayout6.Name_4 = photo.ElementAt(3).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout6.Sequence_4 = photo.ElementAt(3).Sequence;
                    objTemplateLayout6.AlbumPagePhotoRowGUID_4 = photo.ElementAt(3).RowGUID;

                    objTemplateLayout6.Name_5 = photo.ElementAt(4).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout6.Sequence_5 = photo.ElementAt(4).Sequence;
                    objTemplateLayout6.AlbumPagePhotoRowGUID_5 = photo.ElementAt(4).RowGUID;

                    objTemplateLayout6.Name_6 = photo.ElementAt(5).Name + "?id=" + rnd.Next(1, 10000).ToString();
                    objTemplateLayout6.Sequence_6 = photo.ElementAt(5).Sequence;
                    objTemplateLayout6.AlbumPagePhotoRowGUID_6 = photo.ElementAt(5).RowGUID;
                    objMainAlbumPageo_Result.LayoutSix = objTemplateLayout6;
                }
                objListMainAlbumPageo_Result.Add(objMainAlbumPageo_Result);
            }
            albumViewModel.GetMainAlbumPage_Result = objListMainAlbumPageo_Result;
            return albumViewModel;
        }

        public JsonResult DeleteAlbum(Guid AlbumRowGUID)
        {
            Album objAlbum = GetAlbumByLoggedinUser();
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumRowGUID", AlbumRowGUID);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_DeleteAlbum", parameters, QueryTypeEnum.StoredProcedure);
            
            var path = Server.MapPath("~/Album/" + objAlbum.AlbumID);
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, recursive: true);
                }
                catch
                {
                    Thread.Sleep(1000);
                    Directory.Delete(path, recursive: true);
                }
            }
            return Json("Your Album is Deleted", JsonRequestBehavior.AllowGet);
        }


        private int GetAlbumSize(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return 0;

            }

            string noofpages = "Printforme.AlbumSize";
            CustomTableItem item1 = CustomTableItemProvider.GetItem(id, noofpages);
            int sizevalue = ValidationHelper.GetInteger(item1.GetValue("Size"), 0);
            return sizevalue;
        }

        private IEnumerable<AlbumImages> GetAlbumPage(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }

            string albumpageid = "Printforme.AlbumPage";
            DataClassInfo albumDetailInfo = DataClassInfoProvider.GetDataClassInfo(albumpageid);

            List<CustomTableItem> items = CustomTableItemProvider.GetItems(albumpageid)
                    .WhereEquals("AlbumiD", id)
                     .Columns("ItemID").ToList();

            var objalbumservicemodel = items.Select(item => new AlbumImages()
            {
                ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0)
            });
            return objalbumservicemodel;
        }

        private List<AlbumImages> GetAlbumPhotoQueue(Guid AlbumRowGUID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }
            Random rnd = new Random();
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumRowGUID", AlbumRowGUID);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_DownloadPhotoQueue", parameters, QueryTypeEnum.StoredProcedure);


            List<AlbumImages> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumImages
            {
                ItemID = dataRow.Field<int>("ItemID"),
                ItemGUID = dataRow.Field<Guid>("ItemGUID"),
                AlbumID = dataRow.Field<int>("AlbumID"),
                ImagesName = dataRow.Field<string>("Name") + "?id=" + rnd.Next(1, 10000).ToString(),
                AlbumImagesLocation = dataRow.Field<string>("PhotoLocation"),
                Isactive = dataRow.Field<int>("IsPPIStatus")
            }).ToList();

            return item;
        }

        private List<AlbumPage> GetAlbumPages(Guid albumRowGUID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }

            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumRowGUID", albumRowGUID);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_GetAlbumPages", parameters, QueryTypeEnum.StoredProcedure);

            List<AlbumPage> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumPage
            {
                APItemID = dataRow.Field<int>("ItemID"),
                AlbumPageRowGUID = dataRow.Field<Guid>("AlbumPageRowGUID"),
                APAlbumID = dataRow.Field<int>("AlbumID"),
                APLItemID = dataRow.Field<int>("AlbumPageLayoutID"),
                DefaultLayout = dataRow.Field<bool>("DefaultLayout"),
                PageLayoutCode = dataRow.Field<string>("PageLayoutCode"),
                PhotoCount = dataRow.Field<int>("PhotoCount"),
            }).ToList();


            return item;
        }

        private List<AlbumPagePhotos> GetAlbumPagesWithPhoto(Guid albumRowGUID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumRowGUID", albumRowGUID);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_GetAlbumPagesWithPhoto", parameters, QueryTypeEnum.StoredProcedure);

            List<AlbumPagePhotos> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumPagePhotos
            {
                APItemID = dataRow.Field<int>("ItemID"),
                AlbumPageRowGUID = dataRow.Field<Guid>("AlbumPageRowGUID"),
                APAlbumID = dataRow.Field<int>("AlbumID"),
                APLItemID = dataRow.Field<int>("AlbumPageLayoutID"),
                DefaultLayout = dataRow.Field<bool>("DefaultLayout"),
                PageLayoutCode = dataRow.Field<string>("PageLayoutCode"),
                PhotoCount = dataRow.Field<int>("PhotoCount"),
                AlbumPagePhotoID = dataRow.Field<int>("AlbumPagePhotoID"),
                Name = dataRow.Field<string>("Name"),
                PhotoLocation = dataRow.Field<string>("PhotoLocation"),
                Sequence = dataRow.Field<int>("Sequence"),
                RowGUID = dataRow.Field<Guid>("RowGUID")
            }).ToList();


            return item;
        }

        private IEnumerable<AlbumImages> DeletePhotoFromQueue(string guid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }

            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumPhotoQueueRowGUID", guid);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_DeletePhotoFromQueue", parameters, QueryTypeEnum.StoredProcedure);


            IEnumerable<AlbumImages> item = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumImages
            {
                ItemID = dataRow.Field<int>("ItemID"),
                ItemGUID = dataRow.Field<Guid>("ItemGUID"),
                AlbumID = dataRow.Field<int>("AlbumID"),
                ImagesName = dataRow.Field<string>("Name"),
                AlbumImagesLocation = dataRow.Field<string>("PhotoLocation"),
                Isactive = dataRow.Field<int>("IsPPIStatus")
            }).ToList();

            return item;
        }

        public ActionResult CheckAlbum(Guid guid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@AlbumRowGUID", guid);

            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_Count_Albumpagephoto", parameters, QueryTypeEnum.StoredProcedure);

            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]);
            if (count == 0)
            {
                string msg = "";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            return Json("Your Album is not completed. Blank Pages is printed Blank. Do you want to continue ? ", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AlbumAddToCart()
        {
            AlbumAddtoCart objalbumaddtocart = new AlbumAddtoCart();
            int userid = HttpContext.GetOwinContext().Get<UserManager>().FindByName(User.Identity.Name).Id;

            var model = AlbumDetailwithPrice.GetAlbumwihPrice(userid);
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
            if (itemTextValue == 0)
            {
                var image = "Album/" + model.AlbumID + "/AlbumPhoto/" + model.ImagesName;
                objalbumaddtocart.SKUID = ManageSKU(model.Price, model.AlbumID, image);

                item2.SetValue("SKUID", objalbumaddtocart.SKUID);

                item2.Update();
                
                return RedirectToAction("AddItem", "Checkout", new { itemSkuId = objalbumaddtocart.SKUID });
            }
            return RedirectToAction("AddItem", "Checkout", new { itemSkuId = itemTextValue });
        }


        //*****************************************************************

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

                //// Sets a value for a field of the given product page type
                node.SetValue("Name", name);

                // Assigns the product to the page
                node.NodeSKUID = newProduct.SKUID;

                // Saves the product page to the database
                node.Insert(parent);
            }

            return newProduct.SKUID;
        }


        private Album AddAlbum(Album objAlbum)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@LoginUserID", objAlbum.LoggedinUserID);
            parameters.Add("@PageCountCode", objAlbum.AlbumPageCountCode);
            parameters.Add("@PageSizeCode", objAlbum.AlbumSize);
            parameters.Add("@PaperMaterialCode", objAlbum.AlbumPageType);



            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_CreateAlbum", parameters, QueryTypeEnum.StoredProcedure);

            List<Album> album = ds.Tables[0].AsEnumerable().Select(dataRow => new Album
            {

                AlbumRowGUID = dataRow.Field<Guid>("AlbumRowGUID"),
                AlbumCreatedDate = dataRow.Field<DateTime>("AlbumCreatedDate"),
                LoggedinUserID = dataRow.Field<int>("LoggedinUserID"),
                AlbumStatus = dataRow.Field<Boolean>("AlbumStatus"),
                AlbumPageType = dataRow.Field<string>("AlbumPageType"),
                AlbumSize = dataRow.Field<string>("AlbumSize"),
                AlbumPageCountCode = dataRow.Field<string>("AlbumPageCountCode"),
                AlbumID = dataRow.Field<int>("AlbumID")

            }).ToList();

            return album.SingleOrDefault();
        }

        private Album GetAlbumByLoggedinUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;

            }
            int loggedinUserID = HttpContext.GetOwinContext().Get<UserManager>().FindByName(User.Identity.Name).Id;

            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@LoginUserID", loggedinUserID);
            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_PhotoAlbum_GetAlbumByUserID", parameters, QueryTypeEnum.StoredProcedure);

            List<Album> album = ds.Tables[0].AsEnumerable().Select(dataRow => new Album
            {
                AlbumRowGUID = dataRow.Field<Guid>("AlbumRowGUID"),
                AlbumCreatedDate = dataRow.Field<DateTime>("AlbumCreatedDate"),
                LoggedinUserID = dataRow.Field<int>("LoggedinUserID"),
                AlbumStatus = dataRow.Field<Boolean>("AlbumStatus"),
                AlbumPageType = dataRow.Field<string>("AlbumPageType"),
                AlbumSize = dataRow.Field<string>("AlbumSize"),
                AlbumPageCountCode = dataRow.Field<string>("AlbumPageCountCode"),
                AlbumID = dataRow.Field<int>("AlbumID")

            }).ToList();

            return album.SingleOrDefault();
        }

        public static IEnumerable<AlbumFormat> GetAlbumFormat()
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@culture", mCultureName);
            DataSet ds = ConnectionHelper.ExecuteQuery("SP_Printforme_GetAlbumPageSize", parameters, QueryTypeEnum.StoredProcedure);

            IEnumerable<AlbumFormat> albumformat = ds.Tables[0].AsEnumerable().Select(dataRow => new AlbumFormat
            {
                AlbumPageSize = dataRow.Field<string>("AlbumPageSize"),
                ItemGuid = dataRow.Field<Guid>("ItemGUID"),
                ItemID = dataRow.Field<int>("ItemID"),
                AlbumPageSizeCode = dataRow.Field<string>("AlbumPageSizeCode"),
                Culture = dataRow.Field<string>("Culture")

            }).ToList();
            return albumformat;
        }

    }

}
