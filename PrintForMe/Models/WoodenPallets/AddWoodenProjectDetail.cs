namespace PrintForMe.Models.WoodenPallets
{
    public class AddWoodenProjectDetail
    {
        public int ItemID { get; set; }
        public int ProjectID { get; set; }
        public string ImageUrl { get; set; }
        public int NoOfCopy { get; set; }
        public int SizeID { get; set; }
        public bool IsLowResolution { get; set; }
        public string ImageToString { get; set; }
        public string Price { get; set; }
    }
}