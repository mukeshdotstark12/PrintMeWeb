using System;

namespace PrintForMe.Models.Store
{
    public class PaperMaterialModel
    {
        public int? ItemID { get; set; }
        public string PageType { get; set; }
        public bool Availability { get; set; }
        public Guid PaperMaterialCode { get; set; }
    }
}