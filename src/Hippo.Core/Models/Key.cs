using System.ComponentModel.DataAnnotations;

using Hippo.Core;

namespace Hippo.Core.Models
{
    public class Key : BaseEntity
    {
        /// <summary>
        /// public key used for determining trust.
        /// </summary>
        [Required]
        public string PublicKey { get; set; }

        /// <summary>
        /// Private key used for signing releases.
        /// </summary>
        [Required]
        public string PrivateKey { get; set; }
    }
}
