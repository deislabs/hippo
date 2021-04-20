using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Domain: BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Certificate[] Certificates { get; set; }
    }
}
