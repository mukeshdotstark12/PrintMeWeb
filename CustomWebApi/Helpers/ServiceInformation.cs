using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CustomWebApi.Model.Album;
using CustomWebApi.Model.Order;
using CustomWebApi.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Helpers
{
    public class ServiceInformation
    {
        public static ServiceDetail GetProjectInformation(int SKUID)
        {
            ServiceDetail serviceDetail = new ServiceDetail();
            try
            {
                var album = new Album();
                album = AlbumDetailwithPrice.GetAlbumbySkuID(SKUID);

                // Prepares the code name (class name) of the custom table to which the data record will be added
                string photoProjectMaster = "PrintForme.PhotoProjectMaster";
                string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
                string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";

                string photoProjectDetail = "PrintForme.PhotoProjectDetail";
                string woodenProjectDetail = "PrintForme.WoodenPalletsDetail";
                string wallPrintingProjectDetail = "PrintForme.WallPrintingProjectDetail";

                // Gets the custom table
                DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
                DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
                DataClassInfo wallPrintingProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);

                DataClassInfo photoProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectDetail);
                DataClassInfo woodenProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectDetail);
                DataClassInfo wallPrintingProjectDetailInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectDetail);

                var paperMaterial = FillComboBox.GetPapaerMaterialForDescription();
                var frameColor = FillComboBox.GetFrameColorForDescription();                

                if (ValidationHelper.GetInteger(SKUID, 0) > 0)
                {
                    if (photoProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem photoItem = CustomTableItemProvider.GetItems(photoProjectMaster)
                                 .WhereEquals("SKUID", SKUID).LastOrDefault();

                        if (photoItem != null)
                        {
                            int projectID = photoItem.GetValue("ItemID", 0);
                            serviceDetail.PaperMaterial = paperMaterial.Where(x => x.ItemID == photoItem.GetValue("PaperMaterialID", 0)).Select(x => x.Code).FirstOrDefault();
                            if (photoProjectDetailInfo != null)
                            {
                                var projectDetail = CustomTableItemProvider.GetItems(photoProjectDetail)
                                                                        .WhereEquals("ProjectID", projectID)
                                                                        .Columns("ProjectID", "ImageUrl", "ItemID");
                                if (projectDetail != null)
                                {
                                    serviceDetail.TotalPhotos = projectDetail.Count();
                                    serviceDetail.ImagePath = projectDetail.FirstOrDefault().GetValue("ImageUrl", "") != null ?
                                                              projectDetail.FirstOrDefault().GetValue("ImageUrl", "") : "";
                                }
                            }
                        }
                    }

                    if (woodenProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem woodenItem = CustomTableItemProvider.GetItems(woodenProjectMaster)
                                 .WhereEquals("SKUID", SKUID).LastOrDefault();

                        if (woodenItem != null)
                        {
                            int projectID = woodenItem.GetValue("ItemID", 0);

                            if (woodenProjectDetailInfo != null)
                            {
                                var projectDetail = CustomTableItemProvider.GetItems(woodenProjectDetail).WhereEquals("ProjectID", projectID).Columns("ProjectID", "ImageUrl", "ItemID");
                                if (projectDetail != null)
                                {
                                    serviceDetail.TotalPhotos = projectDetail.Count();
                                    serviceDetail.ThicknessOfPallets = ValidationHelper.GetString(woodenItem.GetValue("PlankThickness", ""),"");
                                    serviceDetail.ImagePath = projectDetail.FirstOrDefault().GetValue("ImageUrl", "") != null ?
                                                              projectDetail.FirstOrDefault().GetValue("ImageUrl", "") : "";
                                }
                            }
                        }
                    }

                    if (wallPrintingProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem wallItem = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                                 .WhereEquals("SKUID", SKUID).LastOrDefault();

                        if (wallItem != null)
                        {
                            int projectID = wallItem.GetValue("ItemID", 0);
                            serviceDetail.PaperMaterial = paperMaterial.Where(x => x.ItemID == wallItem.GetValue("PaperMaterialID", 0)).Select(x => x.Code).FirstOrDefault();
                            serviceDetail.FrameColor = frameColor.Where(x => x.ItemID == wallItem.GetValue("FrameColorID", 0)).Select(x => x.Code).FirstOrDefault();

                            if (wallPrintingProjectDetailInfo != null)
                            {
                                var projectDetail = CustomTableItemProvider.GetItems(wallPrintingProjectDetail).WhereEquals("ProjectID", projectID).Columns("ProjectID", "ImageUrl", "ItemID");
                                if (projectDetail != null)
                                {
                                    serviceDetail.TotalPhotos = projectDetail.Count();
                                    serviceDetail.ImagePath = projectDetail.FirstOrDefault().GetValue("ImageUrl", "") != null ?
                                                              projectDetail.FirstOrDefault().GetValue("ImageUrl", "") : "";
                                }

                            }
                        }
                    }

                    if (album != null)
                    {
                        if (album.AlbumID != 0)
                        {
                            serviceDetail.AlbumID = album.AlbumID;
                            serviceDetail.price = album.Price;
                            serviceDetail.NoOfPages = Convert.ToInt32(album.AlbumPageCountCode);
                            serviceDetail.quantity = 1;
                            serviceDetail.Size = album.AlbumSize;
                            serviceDetail.PaperMaterial = album.AlbumPageType;
                            serviceDetail.ImagePath = album.ImagesName;
                        }
                    }
                }

                return serviceDetail;
            }
            catch (Exception)
            {
                return serviceDetail;
            }
        }

        public static ProjectInfo GetProjectInformationForShoppinCart(int SKUID)
        {
            ProjectInfo projectInfo = new ProjectInfo();
            try
            {
                var album = new Album();
                album = AlbumDetailwithPrice.GetAlbumbySkuID(SKUID);

                // Prepares the code name (class name) of the custom table to which the data record will be added
                string photoProjectMaster = "PrintForme.PhotoProjectMaster";
                string woodenProjectMaster = "PrintForme.WoodenPalletsMaster";
                string wallPrintingProjectMaster = "PrintForme.WallPrintingProjectMaster";

                // Gets the custom table
                DataClassInfo photoProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(photoProjectMaster);
                DataClassInfo woodenProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(woodenProjectMaster);
                DataClassInfo wallPrintingProjectMasterInfo = DataClassInfoProvider.GetDataClassInfo(wallPrintingProjectMaster);             

                var paperMaterial = FillComboBox.GetPapaerMaterialForDescription();
                var frameColor = FillComboBox.GetFrameColorForDescription();

                if (ValidationHelper.GetInteger(SKUID, 0) > 0)
                {
                    if (photoProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem photoItem = CustomTableItemProvider.GetItems(photoProjectMaster)
                                 .WhereEquals("SKUID", SKUID).LastOrDefault();

                        if (photoItem != null)
                        {
                            projectInfo.ProjectID = photoItem.GetValue("ItemID", 0);
                            projectInfo.ServiceID = photoItem.GetValue("ServiceID", 0);
                        }
                    }

                    if (woodenProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem woodenItem = CustomTableItemProvider.GetItems(woodenProjectMaster)
                                 .WhereEquals("SKUID", SKUID).LastOrDefault();

                        if (woodenItem != null)
                        {
                            projectInfo.ProjectID = woodenItem.GetValue("ItemID", 0);
                            projectInfo.ServiceID = woodenItem.GetValue("ServiceID", 0);
                        }
                    }

                    if (wallPrintingProjectMasterInfo != null)
                    {
                        // Gets all data records from the custom table whose 'ItemText' field value starts with 'New text'                
                        CustomTableItem wallItem = CustomTableItemProvider.GetItems(wallPrintingProjectMaster)
                                 .WhereEquals("SKUID", SKUID).LastOrDefault();

                        if (wallItem != null)
                        {
                            projectInfo.ProjectID = wallItem.GetValue("ItemID", 0);
                            projectInfo.ServiceID = wallItem.GetValue("ServiceID", 0);
                        }
                    }

                    if (album != null)
                    {
                        if (album.AlbumID != 0)
                        {
                            projectInfo.ProjectID = album.AlbumID;
                            projectInfo.ServiceID = 0;                            
                        }
                    }
                }

                return projectInfo;
            }
            catch (Exception)
            {
                return projectInfo;
            }
        }
    }
}
