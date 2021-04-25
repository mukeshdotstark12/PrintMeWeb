using System.Web;

namespace PrintForMe.Models.Store
{
    public class StoreViewModel
    {
        public int StoreInfoID { get; set; }
        public int SiteID { get; set; }
        public string StoreName { get; set; }
        public HttpPostedFileBase[] Logo { get; set; }
        public string StoreLogoPath { get; set; }
        public string WhatsappNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        //public StoreBrandViewModel Brands;
        //public StoreDepartmentViewModel Departments;
        //public StoreCollectionViewModel Collections;

    }
}