using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Key: BaseEntity
    {

        [Required]
        public Account Owner { get; set; }

        [Required]
        public string PublicKey { get; set; }
    }
}
