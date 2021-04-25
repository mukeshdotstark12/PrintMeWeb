using CMS.Ecommerce;
using System.Collections.Generic;
using System.ComponentModel;

namespace PrintForMe.Models.Checkout
{
    public class PaymentMethodViewModel
    {
        [DisplayName("Payment method")]
        public int PaymentMethodID { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public List<PaymentOptionInfo> PaymentMethods { get; set; }


        /// <summary>
        /// Creates a payment method model.
        /// </summary>
        /// <param name="paymentMethod">Selected payment method.</param>
        /// <param name="paymentMethods">List of all available payment methods.</param>
        public PaymentMethodViewModel(PaymentOptionInfo paymentMethod, List<PaymentOptionInfo> paymentMethods)
        {
            PaymentMethods = paymentMethods;

            if (paymentMethod != null)
            {
                PaymentMethodID = paymentMethod.PaymentOptionID;
            }
        }


        /// <summary>
        /// Creates an empty payment method model.
        /// Required by the MVC framework for model binding during form submission.
        /// </summary>
        public PaymentMethodViewModel()
        {
        }
    }
}