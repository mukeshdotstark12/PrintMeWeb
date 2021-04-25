using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

namespace PrintForMe
{
    public class ApplicationConfig
    {
        public static void RegisterFeatures(IApplicationBuilder builder)
        {
            // Enable required Kentico features

            // Uncomment the following to use the Page builder feature
            builder.UsePreview();
            builder.UsePageBuilder();

            // Enables the data annotation localization feature
            builder.UseDataAnnotationsLocalization();

            builder.UseResourceSharingWithAdministration();
        }
    }
}