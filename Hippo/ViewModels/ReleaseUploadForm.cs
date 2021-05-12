using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class ReleaseUploadForm
    {
        [Required]
        public Guid AppId { get; set; }

        [Required]
        public string Revision { get; set; }

        [Required]
        [Display(Name = "Upload URL")]
        public string UploadUrl { get; set; }
    }
}
