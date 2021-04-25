using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Services
{
    public class ServiceSettingModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public bool Availability { get; set; }
        public int ServiceID { get; set; }
        public int ItemID { get; set; }
        public double ProductPrice { get; set; }
    }
}
