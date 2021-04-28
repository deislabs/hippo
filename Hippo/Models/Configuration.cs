using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Configuration: BaseEntity
    {
        [Required]
        public ICollection<EnvironmentVariable> EnvironmentVariables { get; set; }
    }
}
