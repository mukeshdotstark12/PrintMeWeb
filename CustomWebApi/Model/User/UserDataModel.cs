using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.User
{
    public class UserDataModel
    {
        public int UserID { get; set; }
        public int PaymentType { get; set; }
        public int AddressID { get; set; }
        public int ShoppingCartID { get; set; }
        public int ShippingOptionID { get; set; }
    }
}