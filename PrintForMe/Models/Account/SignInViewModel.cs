using System.ComponentModel.DataAnnotations;

namespace PrintForMe.Models.Account
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "PrintForMe.SignIn.EmailUserName.Empty")]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "PrintForMe.Register.Password.Empty")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        public string Password { get; set; }


        public bool StaySignedIn { get; set; }
        public bool SignInIsPersistent { get; internal set; }
    }
}