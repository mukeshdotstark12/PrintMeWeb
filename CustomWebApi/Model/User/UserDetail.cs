using CMS.Helpers;
using CMS.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomWebApi.Models.User
{
    public class UserDetail
    {
        public int UserID { get; set; }
        public int UserCustomerID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public  DateTime UserCreated { get; set; }        
        public  bool Enabled { get; set; }                        
        public  Guid UserGUID { get; set; }   
        public  string PreferredCultureCode { get; set; }
        public  DateTime LastLogon { get; set; }
        public  string FullName { get; set; }
        public  string LastName { get; set; }                
        public  bool UserEnabled { get; set; }                                                                                                     
        public  string UserPicture { get; set; }    
    }
}