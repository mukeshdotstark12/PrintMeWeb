using PrintForMe.Models.Store;
using System;
using System.Collections.Generic;

namespace PrintForMe.Models.WoodenPallets
{
    public class AddWoodenProject
    {
        public AddWoodenProject()
        {
            AddWoodenProjectDetails = new List<AddWoodenProjectDetail>();
        }
        public IEnumerable<ServiceSettingModel> Size { get; set; }
        public int SelectedSize { get; set; }
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public int SizeID { get; set; }
        public int SKUID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime ProjectDate { get; set; }
        public int ServiceID { get; set; }
        public string PlankThickness { get; set; }
        public double TotalPrice { get; set; }
        public List<AddWoodenProjectDetail> AddWoodenProjectDetails { get; set; }
    }
}