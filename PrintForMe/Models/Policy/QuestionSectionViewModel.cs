using PrintForMe.Models.Checkout;
using System.Collections.Generic;

namespace PrintForMe.Models.Policy
{
    public class QuestionSectionViewModel
    {

        public IEnumerable<PaymentMethodViewModel> PaymentOptions { get; set; }
        public IEnumerable<QuestionAndAnswerModel> Questions { get; set; }
    }
}