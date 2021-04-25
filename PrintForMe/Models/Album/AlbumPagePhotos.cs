using System;

namespace PrintForMe.Models.Album
{
    public class AlbumPagePhotos
    {
        public string Albumpageid { get; set; }
        public int APItemID { get; set; }
        public Guid AlbumPageRowGUID { get; set; }
        public int APAlbumID { get; set; }
        public int APLItemID { get; set; }
        public bool DefaultLayout { get; set; }
        public string PageLayoutCode { get; set; }
        public int PhotoCount { get; set; }
        public int AlbumPagePhotoID { get; set; }
        public string Name { get; set; }
        public string PhotoLocation { get; set; }
        public int Sequence { get; set; }
        public Guid RowGUID { get; set; }
        public string Row_num { get; set; }
    }
}