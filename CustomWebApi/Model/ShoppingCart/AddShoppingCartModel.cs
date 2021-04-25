using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.ShoppingCart
{
    public class AddShoppingCartModel
    {
        public int ShoppingCartUserID { get; set; }
        public int ShoppingCartSiteID { get; set; }
        public int ShoppingCartCurrencyID { get; set; }
    }
}
