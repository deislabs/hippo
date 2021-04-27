using System;
using System.ComponentModel.DataAnnotations;

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
