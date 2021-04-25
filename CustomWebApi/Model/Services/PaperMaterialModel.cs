using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Services
{
    public class PaperMaterialModel
    {
        public string PageType { get; set; }
        public bool Availability { get; set; }
        public int Id { get; set; }
        public Guid RowGuid { get; set; }
        public int? ItemID { get; set; }
        public Guid PaperMaterialCode { get; set; }
    }
}
