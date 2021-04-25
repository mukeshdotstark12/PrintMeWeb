using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class TemplateLayout1
    {
        public Guid AlbumPageRowGUID { get; set; }
        public string PageLayoutCode { get; set; }
        public int PhotoCount { get; set; }
        public string Name_1 { get; set; }
        public int? Sequence_1 { get; set; }
        public Guid? AlbumPagePhotoRowGUID_1 { get; set; }
        public string Name_2 { get; set; }
        public int? Sequence_2 { get; set; }
        public Guid? AlbumPagePhotoRowGUID_2 { get; set; }
    }
}
