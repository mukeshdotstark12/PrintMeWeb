namespace PrintForMe.Models.Checkout
{
    public class ShoppingCartItemViewModel
    {
        public string SKUName { get; set; }
        public string SKUImage { get; set; }

        public int SKUID { get; set; }

        public int CartItemUnits { get; set; }

        public decimal ListPrice { get; set; }
        public decimal OldPrice { get; set; }
        public decimal TotalDiscount { get; set; }

        public int CartItemID { get; set; }
        public string SKUDisplayImage { get; set; }

        public int SKUDepartmentID { get; set; }
    }
}