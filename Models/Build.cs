using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Build: BaseEntity
    {
        [Required]
        public string UploadUrl { get; set; }
    }
}
