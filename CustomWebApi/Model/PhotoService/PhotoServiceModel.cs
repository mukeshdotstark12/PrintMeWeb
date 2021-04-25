using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.PhotoService
{
    public class PhotoServiceModel
    {
        public PhotoServiceModel()
        {
            PhotoServiceDetailModel = new List<PhotoServiceDetailModel>();
        }                                
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public int PaperMaterialID { get; set; }
        public int SKUID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime ProjectDate { get; set; }
        public double TotalPrice { get; set; }
        public int ServiceId { get; set; }
        public List<PhotoServiceDetailModel> PhotoServiceDetailModel { get; set; }
    }
}
