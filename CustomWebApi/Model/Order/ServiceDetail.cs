using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Order
{
    public class ServiceDetail
    {
        public int AlbumID { get; set; }
        public int NoOfPages { get; set; }
        public int quantity { get; set; }
        public string Size { get; set; }
        public decimal price { get; set; }
        public string PaperMaterial { get; set; }
        public int TotalPhotos { get; set; }
        public string ThicknessOfPallets { get; set; }
        public string FrameColor { get; set; }
        public string ImagePath { get; set; }
    }
}
