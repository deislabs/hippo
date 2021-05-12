using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class ApiLoginForm
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
