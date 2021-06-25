using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class EnvironmentVariable : BaseEntity
    {
        public Configuration Configuration { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
