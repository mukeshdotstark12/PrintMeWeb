using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models
{
    public class UserLogin
    {
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
    }
}