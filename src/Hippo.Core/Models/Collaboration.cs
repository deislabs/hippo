using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using Hippo.Core;

namespace Hippo.Core.Models
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
