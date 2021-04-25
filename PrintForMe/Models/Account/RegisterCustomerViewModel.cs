using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrintForMe.Models.Account
{
    public class RegisterCustomerViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "PrintForMe.Email.Empty")]
        [DisplayName("PrintForMe.Email")]
        [EmailAddress(ErrorMessage = "PrintForMe.General.InvalidEmail")]
        //[MaxLength(100, ErrorMessage = "ECS.General.MaximumInputLengthExceeded")]
        public string Email { get; set; }


        [DisplayName("PrintForMe.MobileNumber")]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("PrintForMe.Password")]
        [Required(ErrorMessage = "PrintForMe.Register.Password.Empty")]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("PrintForMe.Register.PasswordConfirmation")]
        [Required(ErrorMessage = "PrintForMe.Register.PasswordConfirmation.Empty")]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        [Compare("Password", ErrorMessage = "PrintForMe.Register.PasswordConfirmation.Invalid")]
        public string PasswordConfirmation { get; set; }


        [DisplayName("PrintForMe.FirstName")]
        [Required(ErrorMessage = "PrintForMe.Register.FirstName.Empty")]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        public string FirstName { get; set; }


        [DisplayName("PrintForMe.LastName")]
        [Required(ErrorMessage = "PrintForMe.Register.LastName.Empty")]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        public string LastName { get; set; }


        [DisplayName("PrintForMe.MobileNumber")]
        [Required(ErrorMessage = "PrintForMe.Mobile.Empty")]
        [MaxLength(20, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        [MinLength(6, ErrorMessage = "PrintForMe.General.MinimumInputLengthExceeded")]
        public string MobileNumber { get; set; }

        [DisplayName("PrintForMe.TermsPrivacyPolicyAccept")]
        //[Range(typeof(bool), "true", "true", ErrorMessage = "PrintForMe.TermsPrivacyPolicyAccept.Empty")]
        [CheckBoxRequired(ErrorMessage = "PrintForMe.TermsPrivacyPolicyAccept.Empty")]
        public bool IsPolicyAccepted { get; set; }


        public string Role { get; set; }
    }
}