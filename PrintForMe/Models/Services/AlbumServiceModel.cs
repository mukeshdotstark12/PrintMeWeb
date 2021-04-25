using PrintForMe.Models.Store;
using System.Collections.Generic;

namespace PrintForMe.Models.Services
{
    public class AlbumServiceModel
    {
        public int MinimumPrice { get; set; }
        public IEnumerable<AlbumFormat> AlbumFormat { get; set; }
        public IEnumerable<AlbumNoofPages> AlbumSize { get; set; }
        public List<PaperMaterialModel> PaperMaterial { get; set; }
        public string SelectedSize { get; set; }
        public string SelectedNoofpages { get; set; }
        public string SelectedPaperMaterial { get; set; }
        public int ServiceID { get; set; }
        public int UserId { get; set; }
        public bool Isactive { get; set; }
        public int AlbumID { get; set; }
        public System.Guid RowGUID { get; set; }
        public System.DateTime CreatedDate { get; set; }


    }
}