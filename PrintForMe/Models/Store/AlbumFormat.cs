using System;

namespace PrintForMe.Models.Store
{
    public class AlbumFormat
    {
        public int ItemID { get; set; }
        public Guid ItemGuid { get; set; }
        public string AlbumPageSize { get; set; }
        public string AlbumPageSizeCode { get; set; }
        public string Culture { get; set; }
        public int Price { get; set; }
        public bool Availability { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}