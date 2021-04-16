using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hippo.Models
{
    public class App: BaseEntity
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public Account Owner { get; set; }

        public Account[] Collaborators { get; set; }
    }
}
