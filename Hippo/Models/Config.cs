using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Config: BaseEntity
    {
        [Required]
        public List<EnvironmentVariable> EnvironmentVariables { get; set; }
    }
}
