using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrintForMe.Models.Subscription
{
    public class SubscribeModel
    {
        [Required(ErrorMessage = "General.RequireEmail")]
        [EmailAddress(ErrorMessage = "General.CorrectEmailFormat")]
        [DisplayName("PrintForMe.SubscriberEmail")]
        [MaxLength(250, ErrorMessage = "PrintForMe.LongEmail")]
        public string Email { get; set; }
    }
}