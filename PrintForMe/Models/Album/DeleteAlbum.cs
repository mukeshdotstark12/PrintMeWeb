using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrintForMe.Models.Album
{
    public class DeleteAlbum
    {
        public int DeleteAlbumID { get; set; }
        public string DeleteAlbumNoofDays { get; set; }
        public DateTime DeleteAlbumDate { get; set; }
    }
}