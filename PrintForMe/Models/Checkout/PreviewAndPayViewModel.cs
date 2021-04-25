namespace PrintForMe.Models.Checkout
{
    public class PreviewAndPayViewModel
    {
        public DeliveryDetailsViewModel DeliveryDetails { get; set; }

        public ShoppingCartViewModel Cart { get; set; }

        public PaymentMethodViewModel PaymentMethod { get; set; }
    }
}