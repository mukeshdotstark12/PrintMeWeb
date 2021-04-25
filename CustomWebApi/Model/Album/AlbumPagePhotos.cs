using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class AlbumPagePhotos
    {
        public string Albumpageid { get; set; }
        public string Row_num { get; set; }
        public int ProjectID { get; set; }
        public int Layout { get; set; }
        public int AlbumPagePhotoID { get; set; }
        public string Name { get; set; }
        public string PhotoLocation { get; set; }
        public int Position { get; set; }
        public Guid RowGUID { get; set; }
    }
}
