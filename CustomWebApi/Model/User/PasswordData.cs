using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.User
{
    public class PasswordData
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Hash { get; set; }
    }
}