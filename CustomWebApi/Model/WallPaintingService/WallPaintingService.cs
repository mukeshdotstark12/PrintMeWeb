using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.WallPaintingService
{
    public class WallPaintingService
    {
        public WallPaintingService()
        {
            WallPaintingServiceDetail = new List<WallPaintingServiceDetailModel>();
        }
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public int SKUID { get; set; }
        public int PaperMaterialID { get; set; }
        public int FrameColorID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime ProjectDate { get; set; }
        public double TotalPrice { get; set; }
        public int ServiceId { get; set; }
        public string PaintingSize { get; set; }
        public List<WallPaintingServiceDetailModel> WallPaintingServiceDetail { get; set; }
    }
}
