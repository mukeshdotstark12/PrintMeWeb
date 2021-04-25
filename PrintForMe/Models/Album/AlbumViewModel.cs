using System.Collections.Generic;

namespace PrintForMe.Models.Album
{
    public class AlbumViewModel
    {
        //public AlbumViewModel()
        //{
        //    UploadPhotoQueue_Result = new List<USP_PhotoAlbum_UploadPhotoQueue_Result>();
        //    DownloadPhotoQueue_Result = new List<USP_PhotoAlbum_DownloadPhotoQueue_Result>();
        //}
        public Album PhotoAlbum { get; set; }

        public List<AlbumImages> DownloadPhotoQueue_Result { get; set; }

        public List<AlbumPage> GetAlbumPages_Result { get; set; }

        public List<AlbumPagePhotos> GetAlbumPagesWithPhoto_Result { get; set; }
        public List<MainAlbumPageo_Result> GetMainAlbumPage_Result { get; set; }

        public int LoggedinUserID { get; set; }

    }
}