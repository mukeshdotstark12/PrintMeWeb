using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrintForMe.Models.Account
{
    public class RetrievePasswordViewModel
    {
        [Required(ErrorMessage = "PrintForMe.Email.Empty")]
        [DisplayName("PrintForMe.Email")]
        [EmailAddress(ErrorMessage = "PrintForMe.General.InvalidEmail")]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        public string Email { get; set; }
    }
}