using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class AlbumAddtoCart
    {
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public int AlbumId { get; set; }
        public int SKUID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime ProjectDate { get; set; }
        public Guid ItemGuid { get; set; }

    }
}
