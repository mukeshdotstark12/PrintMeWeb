using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class AlbumImages
    {
        public int ItemID { get; set; }
        public Guid ItemGUID { get; set; }
        public int AlbumID { get; set; }
        public string ImagesName { get; set; }
        public string AlbumImagesLocation { get; set; }
        public DateTime AddedDate { get; set; }
        public bool Isactive { get; set; }
    }
}
