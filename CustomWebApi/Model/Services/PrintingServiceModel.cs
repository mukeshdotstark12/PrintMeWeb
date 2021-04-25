using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Services
{
    public class PrintingServiceModel
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string ColorCode { get; set; }
        public int ServicesID { get; set; }
        public int MinimumPrice { get; set; }
    }
}
