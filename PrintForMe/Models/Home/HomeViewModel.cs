namespace PrintForMe.Models.Home
{
    public class HomeViewModel
    {
        private CMS.DocumentEngine.Types.PrintForme.Home homeNode;

        public string DocumentName { get; set; }
        public string LandingImageUrl { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }
        // Maps the data from the Home page type's fields to the view model properties

        public HomeViewModel(CMS.DocumentEngine.Types.PrintForme.Home homePage)
        {
            DocumentName = homePage.DocumentName;
            Heading = homePage.Fields.HeadingTitle;
            LandingImageUrl = homePage.Fields.LandingImage;
            Text = homePage.Fields.HeadingText;
        }
    }
}