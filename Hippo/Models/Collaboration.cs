using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Hippo.Models
{
    [SuppressMessage("", "CA1724", Justification = "Come on, the P2P namespace? Really?")]
    public class Collaboration : BaseEntity
    {
        [Required]
        public Application Application { get; set; }
        [Required]
        public Account User { get; set; }
    }
}
