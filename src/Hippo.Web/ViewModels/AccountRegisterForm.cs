using System.ComponentModel.DataAnnotations;
using Hippo.Core.Interfaces;

namespace Hippo.Web.ViewModels
{
    public class AccountRegisterForm : ITraceable
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string PasswordConfirm { get; set; }

        public string FormatTrace() =>
            $"{nameof(AccountRegisterForm)}[username={UserName}]";
    }
}
