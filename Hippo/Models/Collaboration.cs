using System.Diagnostics.CodeAnalysis;

namespace Hippo.Models
{
    [SuppressMessage("", "CA1724", Justification = "Come on, the P2P namespace? Really?")]
    public class Collaboration : BaseEntity
    {
        public Application Application { get; set; }
        public Account User { get; set; }
    }
}
