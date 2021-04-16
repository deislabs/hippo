using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hippo.Models
{
    public class Config
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public App App { get; set; }

        public IEnumerable<EnvironmentVariable> EnvironmentVariables { get; set; }
    }
}
