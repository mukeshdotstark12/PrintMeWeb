using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class AlbumViewModel
    {
        public AlbumViewModel()
        {
            GetAlbumDetail = new List<AlbumViewModel>();
        }
       

        //public AlbumPageDetail albumPageDetail1 { get; set; }

        public int UserID { get; set; }
        public int AlbumPageID { get; set; }
        public int Template { get; set; }
        public DateTime ProjectDate { get; set; }
        public int ProjectID { get; set; }
        public int PaperCount { get; set; }
        public int PaperSize { get; set; }
        public int PaperMaterial { get; set; }
        public string SKUID { get; set; }
        public int ServiceId { get; set; }

        public List<AlbumImageDetail> AlbumPageImageDetail { get; set; }

        public List<AlbumPageDetail> albumPageDetail { get; set; }
        public List<AlbumViewModel> GetAlbumDetail { get; set; }

    }

    public class AlbumImageDetail
    {
        public int Position { get; set; }
        public string ImageUrl { get; set; }

    }

    public class AlbumPageDetail
    {
        public int AlbumPageID { get; set; }
        public int Template { get; set; }
        public List<AlbumImageDetail> AlbumPageImageDetail { get; set; }

    }

    
}
