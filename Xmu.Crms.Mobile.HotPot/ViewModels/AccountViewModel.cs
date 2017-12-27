using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xmu.Crms.Mobile.HotPot.ViewModels
{
    public class UsernameAndPassword
    {
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class SigninResult
    {
        public long Id { get; set; }

        //public Xmu.Crms.Shared.Models.Type Type { get; set; }

        public string Name { get; set; }

        public long Exp { get; set; }

        public string Jwt { get; set; }
    }
}