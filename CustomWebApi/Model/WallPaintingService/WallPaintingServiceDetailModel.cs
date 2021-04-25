using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.WallPaintingService
{
    public class WallPaintingServiceDetailModel
    {
        public int ItemID { get; set; }
        public int ProjectID { get; set; }
        public string ImageUrl { get; set; }
        public int NoOfCopy { get; set; }
        public int SizeID { get; set; }
    }
}
