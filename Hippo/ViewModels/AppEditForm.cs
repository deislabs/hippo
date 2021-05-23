using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels
{
    public class AppEditForm: ITraceable
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string FormatTrace()
        {
            return $"{nameof(AppEditForm)}[id={Id}, name={Name}]";
        }
    }
}
