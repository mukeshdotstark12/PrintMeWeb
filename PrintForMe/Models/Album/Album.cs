using System;

namespace PrintForMe.Models.Album
{
    public class Album
    {
        public Guid AlbumRowGUID { get; set; }
        public DateTime AlbumCreatedDate { get; set; }
        public int LoggedinUserID { get; set; }
        public Boolean AlbumStatus { get; set; }
        public string AlbumPageType { get; set; }
        public string AlbumSize { get; set; }
        public string AlbumPageCountCode { get; set; }
        public int AlbumID { get; set; }
        public string ImagesName { get; set; }
        public int Price { get; set; }
        public string ImagesLocation { get; set; }
        public string SKUID { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}