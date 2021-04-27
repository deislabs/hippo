using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class AppEditForm
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
