using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class MainAlbumPageo_Result
    {
        public Guid AlbumPageRowGUID { get; set; }
        public string PageLayoutCode { get; set; }
        public int PhotoCount { get; set; }
        public TemplateLayout1 LayoutOne { get; set; }
        public TemplateLayout2 LayoutTwo { get; set; }
        public TemplateLayout3 LayoutThree { get; set; }
        public TemplateLayout4 LayoutFour { get; set; }
        public TemplateLayout5 LayoutFive { get; set; }
        public TemplateLayout6 LayoutSix { get; set; }
    }
}
