using System;

namespace PrintForMe.Models.Store
{
    public class AlbumNoofPages
    {
        public int? ItemID { get; set; }
        public string Size { get; set; }
        public bool Availability { get; set; }
        public Guid AlbumSizeCode { get; set; }
        public string Culture { get; set; }
    }
}