using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.ShoppingCart
{
    public class AddToCartModel
    {
        public int UserID { get; set; }
        public int SKUID { get; set; }
        public int SKUUnits { get; set; }
    }
}
