using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels
{
    public class AppNewForm: ITraceable
    {
        [Required]
        public string Name { get; set; }

        public string FormatTrace() =>
            $"{nameof(AppNewForm)}[name={Name}]";
    }
}
