using PrintForMe.Models.PrintingService;
using System.Collections.Generic;

namespace PrintForMe.Models.Menu
{
    public class MenuItemViewModel
    {
        public string MenuItemText { get; set; }
        public string MenuItemRelativeUrl { get; set; }
        public string MenuItemIcon { get; set; }
        public string MenuItemLink { get; set; }
        public IEnumerable<PrintingServiceModel> MenuItems {get;set;}
    }
}