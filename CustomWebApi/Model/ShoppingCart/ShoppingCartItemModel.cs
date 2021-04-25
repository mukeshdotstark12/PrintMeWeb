using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.ShoppingCart
{
    public class ShoppingCartItemModel
    {
        public string SKUName { get; set; }
        public int SKUID { get; set; }
        public int CartItemUnits { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal SKUPrice { get; set; }
        public int CartItemID { get; set; }
        public string SKUImagePath { get; set; }
        public ProjectInfo ProjectInfo { get; set; }
    }

    public class ProjectInfo
    {
        public int ProjectID { get; set; }
        public int ServiceID { get; set; }
    }
}