using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Collaboration : BaseEntity
    {
        [Required]
        public Application Application { get; set; }
        [Required]
        public Account User { get; set; }
    }
}
