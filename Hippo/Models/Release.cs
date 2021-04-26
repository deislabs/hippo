using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hippo.Models
{
    public class Release: BaseEntity
    {
        [Required]
        public string Revision { get; set; }

        [Required]
        public Build Build { get; set; }

        [Required]
        public Config Config { get; set; }
    }
}
