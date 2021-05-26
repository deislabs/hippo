using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels
{
    public class ApiLoginForm: ITraceable
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string FormatTrace() =>
            $"{nameof(ApiLoginForm)}[username={UserName}]";
    }
}
