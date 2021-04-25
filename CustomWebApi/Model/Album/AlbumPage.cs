using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
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

    public class AlbumPages
    {
        public int AlbumPageID { get; set; }
    }
}
