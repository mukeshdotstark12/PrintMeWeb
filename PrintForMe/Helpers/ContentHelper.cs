using CMS.DocumentEngine;
using CMS.Membership;
using CMS.SiteProvider;
using System.Linq;

namespace PrintForMe.Helpers
{
    public class ContentHelper
    {
        private static readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        public static string GetStoreImage()
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            var pages = tree.SelectNodes().Type("PrintForMe.CompanyInfo")
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .Published()
                .FirstOrDefault();

            return pages.GetValue<string>("Logo", string.Empty);
        }
    }
}