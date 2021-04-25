using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class AlbumFormat
    {
        public int ItemID { get; set; }
        public Guid ItemGuid { get; set; }
        public string AlbumPageSize { get; set; }
        public string AlbumPageSizeCode { get; set; }
        public string Culture { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }
    }
}
