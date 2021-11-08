using System.ComponentModel.DataAnnotations;

using Hippo.Core;

namespace Hippo.Core.Models
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
