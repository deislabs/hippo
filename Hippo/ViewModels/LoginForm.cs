using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class LoginForm
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
