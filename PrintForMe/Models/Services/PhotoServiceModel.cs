using System.Collections.Generic;

namespace PrintForMe.Models.Services
{
    public class PhotoServiceModel
    {
        public int MinimumPrice { get; set; }
        public IEnumerable<object> Size { get; set; }
        public IEnumerable<object> PaperMaterial { get; set; }
        public IEnumerable<object> FrameColor { get; set; }
        public int SelectedFrameColor { get; set; }
        public int SelectedSize { get; set; }
        public int SelectedPaperMaterial { get; set; }
        public int ServiceID { get; set; }
        public string PlankThickness { get; set; }
        public string PaintingSize { get; set; }

    }
}