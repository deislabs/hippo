using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hippo.Models
{
    public class App
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Account Owner { get; set; }

        public Account[] Collaborators { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Modified { get; set; }
    }
}
