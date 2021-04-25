using System;

namespace PrintForMe.Models.Album
{
    public class AlbumImages
    {
        public int ItemID { get; set; }
        public Guid ItemGUID { get; set; }
        public int AlbumID { get; set; }
        public string ImagesName { get; set; }
        public string AlbumImagesLocation { get; set; }
        public DateTime AddedDate { get; set; }
        public int Isactive { get; set; }
    }
}