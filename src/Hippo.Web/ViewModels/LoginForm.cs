using System.ComponentModel.DataAnnotations;
using Hippo.Core.Interfaces;

namespace Hippo.Web.ViewModels
{
    public class LoginForm : ITraceable
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string FormatTrace() =>
            $"{nameof(LoginForm)}[username={UserName}]";
    }
}
