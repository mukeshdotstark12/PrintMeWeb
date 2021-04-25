namespace PrintForMe.Models
{
    public class PaymentData
    {
        public int OrderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CCPhoneNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine { get; set; }
        public string RegionName { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
        public string Hash { get; set; }
    }
}