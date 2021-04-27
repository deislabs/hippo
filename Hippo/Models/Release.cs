using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Release: BaseEntity
    {
        [Required]
        public string Revision { get; set; }

        [Required]
        public Build Build { get; set; }

        [Required]
        public Config Config { get; set; }
    }
}
