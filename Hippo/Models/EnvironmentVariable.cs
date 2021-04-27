using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class EnvironmentVariable: BaseEntity
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
