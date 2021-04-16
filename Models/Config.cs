using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hippo.Models
{
    public class Config: BaseEntity
    {
        [Required]
        public App App { get; set; }

        public List<EnvironmentVariable> EnvironmentVariables { get; set; }
    }
}
