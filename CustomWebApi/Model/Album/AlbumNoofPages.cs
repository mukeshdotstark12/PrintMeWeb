using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class AlbumNoofPages
    {
        public int? ItemID { get; set; }
        public string Size { get; set; }
        public bool Availability { get; set; }
        public Guid AlbumSizeCode { get; set; }
    }
}
