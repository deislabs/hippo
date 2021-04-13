using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hippo.ViewModels
{
    public class AccountRegisterForm
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
