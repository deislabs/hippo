using Microsoft.AspNetCore.Identity;

namespace Hippo.Core.Models
{
    /// <summary>
    /// The generic IdentityUser has a lot of built in properties
    /// that can be used for identities.
    /// By using IdentityUser to inherit we are saying that we want to add on to it.
    /// </summary>
    public class Account : IdentityUser
    {
        /// <summary>
        /// Personal signing key used for signing off releases and determining trust.
        /// </summary>
        public virtual Key SigningKey { get; set; }
    }
}
