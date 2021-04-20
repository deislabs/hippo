using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Models
{
    public class Key: BaseEntity
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
