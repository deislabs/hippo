using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippo.ViewModels
{
    public class BuildUploadForm
    {
        [Required]
        public Guid AppId { get; set; }

        [Required]
        public string UploadUrl { get; set; }
    }
}
