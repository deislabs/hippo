using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Domain: BaseEntity
    {

        [Required]
        public Account Owner { get; set; }

        public string Name { get; set; }
    }
}
