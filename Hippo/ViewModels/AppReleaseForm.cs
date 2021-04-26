using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippo.ViewModels
{
    public class AppReleaseForm
    {
        [Required]
        public Guid Id { get; set; }

        public string Revision { get; set; }
    }
}
