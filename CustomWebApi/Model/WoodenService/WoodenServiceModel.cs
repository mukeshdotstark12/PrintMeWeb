using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.WoodenService
{
    public class WoodenServiceModel
    {
        public WoodenServiceModel()
        {
            WoodenServiceDetailModel = new List<WoodenServiceDetailModel>();
        }
        public int ProjectID { get; set; }
        public int UserID { get; set; }        
        public int SKUID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime ProjectDate { get; set; }
        public double TotalPrice { get; set; }
        public int ServiceId { get; set; }
        public string PlankThickness { get; set; }
        public List<WoodenServiceDetailModel> WoodenServiceDetailModel { get; set; }
    }
}
