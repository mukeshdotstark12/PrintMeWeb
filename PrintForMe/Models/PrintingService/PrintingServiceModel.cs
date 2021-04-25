namespace PrintForMe.Models.PrintingService
{
    public class PrintingServiceModel
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string ColorCode { get; set; }
    }

    enum PrintingService
    {
        WoodenPallets = 2,
        WallPanels = 3,
        Albums = 4,
        Photo = 5,
        WoodenPalletsArabic = 6,
        WallPanelsArabic = 7,
        AlbumsArabic = 8,
        PhotoArabic = 9
    }
}