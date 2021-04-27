using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class AccountRegisterForm
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
    }
}
