using CMS.Membership;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PrintForMe.Models.User
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }

        [DisplayName("PrintForMe.MobileNumber")]
        [Required(ErrorMessage = "PrintForMe.Mobile.Empty")]
        [MaxLength(20, ErrorMessage = "PrintForMe.General.MaximumInputLengthExceeded")]
        [MinLength(6, ErrorMessage = "PrintForMe.General.MinimumInputLengthExceeded")]
        public string MobileNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "PrintForMe.Email.Empty")]
        [DisplayName("PrintForMe.Email")]
        [EmailAddress(ErrorMessage = "PrintForMe.General.InvalidEmail")]
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }

        public UserModel()
        {

        }

        public UserModel(UserInfo item)
        {
            UserName = item.UserName;
            FirstName = item.FirstName;
            LastName = item.LastName;
            FullName = item.FullName;
            Gender = item.GetStringValue("Gender", "");

            MobileNumber = item.GetStringValue("MobileNumber", "");
            Email = item.GetStringValue("Email", "");

            var userRoleIDs = UserRoleInfoProvider.GetUserRoles().Column("RoleID").WhereEquals("UserID", item.UserID);
            if (userRoleIDs.Count() == 0)
            {
                Role = "Client";
            }
            else
            {
                var roles = RoleInfoProvider.GetRoles().Column("RoleDisplayName").WhereIn("RoleID", userRoleIDs).Select(r => r.RoleDisplayName);
                Role = String.Join(", ", roles);
            }
        }
    }
}