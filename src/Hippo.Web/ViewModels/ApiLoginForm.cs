using System.ComponentModel.DataAnnotations;
using Hippo.Core.Interfaces;

namespace Hippo.Web.ViewModels
{
    public class ApiLoginForm : ITraceable
    {
        [Required]
        [Display(Name = "username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string FormatTrace() =>
            $"{nameof(ApiLoginForm)}[username={UserName}]";
    }
}
