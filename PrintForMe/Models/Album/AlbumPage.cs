using System;

namespace PrintForMe.Models.Album
{
    public class AlbumPage
    {
        public int APItemID { get; set; }
        public Guid AlbumPageRowGUID { get; set; }
        public int APAlbumID { get; set; }
        public int APLItemID { get; set; }
        public bool DefaultLayout { get; set; }
        public string PageLayoutCode { get; set; }
        public int PhotoCount { get; set; }


    }
}