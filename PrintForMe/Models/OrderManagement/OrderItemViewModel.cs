namespace PrintForMe.Models.OrderManagement
{
    public class OrderItemViewModel
    {
        public int SKUID { get; set; }
        public string SKUImagePath { get; set; }
        public string SKUName { get; set; }
        public string SKUSize { get; set; }
        public int UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPriceInMainCurrency { get; set; }
        public int SKUDepartmentID { get; set; }
        public ServiceDetail serviceDetail { get; set; }
    }
}