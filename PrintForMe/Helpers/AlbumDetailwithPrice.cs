using CMS.DataEngine;
using PrintForMe.Models.Album;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PrintForMe.Helpers
{
    public static class AlbumDetailwithPrice
    {
        private readonly static string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

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
    }
}