using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models
{
    public class UserRegister
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PreferredCultureCode { get; set; }
        public string MobileNumber { get; set; }

    }
}