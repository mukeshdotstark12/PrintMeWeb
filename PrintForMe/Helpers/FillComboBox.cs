using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using PrintForMe.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrintForMe.Helpers
{
    public static class FillComboBox
    {
        private readonly static string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        public static IEnumerable<object> GetPapaerMaterial()
        {
            // Prepares the code name (class name) of the custom table
            string paperMaterials = "PrintForme.PaperMaterials";

            // Gets the custom table
            DataClassInfo paperMaterialsInfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            if (paperMaterialsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(paperMaterials)
                    .WhereEquals("Availability", true)
                    .WhereEquals("Culture", mCultureName)
                    .Columns("PageType", "Availability", "ItemID", "ItemGUID").ToList();

                //var selectList = items.Select(s =>
                //{
                //    var ItemID = ValidationHelper.GetString(s.GetValue("ItemID"), "");
                //    var PageType = ValidationHelper.GetString(s.GetValue("PageType"), "");

                //    return new SelectListItem
                //    {
                //        Value = ItemID,
                //        Text = PageType
                //    };
                //});

                return items;
            }

            return null;
        }

        public static IEnumerable<ServiceSettingModel> GetPapaerMaterialForDescription()
        {
            // Prepares the code name (class name) of the custom table
            string paperMaterials = "PrintForme.PaperMaterials";

            // Gets the custom table
            DataClassInfo paperMaterialsInfo = DataClassInfoProvider.GetDataClassInfo(paperMaterials);
            if (paperMaterialsInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(paperMaterials)
                    .WhereEquals("Availability", true)
                    .WhereEquals("Culture", mCultureName)
                    .Columns("PageType", "Availability", "ItemID", "ItemGUID").ToList();

                var paperMaterialModel = items.Select(item => new ServiceSettingModel()
                {
                    Code = ValidationHelper.GetString(item.GetValue("PageType"), ""),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0)
                });

                return paperMaterialModel;
            }

            return null;
        }

        public static IEnumerable<ServiceSettingModel> GetPapaerSize(int serviceId)
        {
            // Prepares the code name (class name) of the custom table
            string serviceSettingsClassName = "PrintForme.ServiceSettings";

            // Gets the custom table
            DataClassInfo serviceSettings = DataClassInfoProvider.GetDataClassInfo(serviceSettingsClassName);
            if (serviceSettings != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(serviceSettingsClassName)
                    .WhereEquals("ServiceID", serviceId)
                    .Columns("Code", "ItemID", "Description", "Price").ToList();

                // Creates a collection of view models based on the menu item and page data
                var sizeModel = items.Select(item => new ServiceSettingModel()
                {
                    Code = ValidationHelper.GetString(item.GetValue("Code"), "") + " " + ValidationHelper.GetString(item.GetValue("Description"), ""),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Description = ValidationHelper.GetString(item.GetValue("Description"), ""),
                    ProductPrice = ValidationHelper.GetDouble(item.GetValue("Price"), 0)
                });

                return sizeModel;
            }

            return null;
        }

        public static IEnumerable<object> GetFrameColor()
        {
            // Prepares the code name (class name) of the custom table
            string frameColor = "PrintForme.FrameColors";

            // Gets the custom table
            DataClassInfo frameColorInfo = DataClassInfoProvider.GetDataClassInfo(frameColor);
            if (frameColorInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(frameColor)
                    .WhereEquals("Availability", true)
                    .WhereEquals("Culture", mCultureName)
                     .Columns("ColorName", "Availability", "ItemID").ToList();

                return items;
            }

            return null;
        }

        public static IEnumerable<ServiceSettingModel> GetFrameColorForDescription()
        {
            // Prepares the code name (class name) of the custom table
            string frameColor = "PrintForme.FrameColors";

            // Gets the custom table
            DataClassInfo frameColorInfo = DataClassInfoProvider.GetDataClassInfo(frameColor);
            if (frameColorInfo != null)
            {
                // Gets the first custom table record whose value in the 'ItemName' field is equal to "SampleName"
                List<CustomTableItem> items = CustomTableItemProvider.GetItems(frameColor)
                    .WhereEquals("Availability", true)
                    .WhereEquals("Culture", mCultureName)
                     .Columns("ColorName", "Availability", "ItemID").ToList();

                var frameColorModel = items.Select(item => new ServiceSettingModel()
                {
                    Code = ValidationHelper.GetString(item.GetValue("ColorName"), ""),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0)
                });

                return frameColorModel;
            }

            return null;
        }

        public static IEnumerable<AlbumNoofPages> GetAlbumSize()
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
                    Size = ValidationHelper.GetString(item.GetValue("Size"), "") + " " + @ResHelper.GetString("PrintForMe.AlbumSizePages"),
                    ItemID = ValidationHelper.GetInteger(item.GetValue("ItemID"), 0),
                    Availability = ValidationHelper.GetBoolean(item.GetValue("Availability"), false),
                    AlbumSizeCode = ValidationHelper.GetGuid(item.GetValue("ItemGUID"), Guid.NewGuid())
                });



                return pageModel;
            }



            return null;
        }

    }
}