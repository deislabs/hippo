using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Build: BaseEntity
    {
        public App App { get; set; }

        [Required]
        public string UploadUrl { get; set; }
    }
}
