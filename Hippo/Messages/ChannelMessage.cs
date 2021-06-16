using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Messages
{
    public class ChannelMessage
    {
        /// <summary>
        /// The GUID of the ApplicationMessage which the channel belongs to.
        /// </summary>
        /// <example>4208d635-7618-4150-b8a8-bc3205e70e32</example>
        [Required]
        public Guid AppId { get; set; }

        /// <summary>
        /// The name of the Channel.
        /// </summary>
        /// <example>My Channel</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Specifies if the channels is fixed to a revision or uses a revision range.
        /// </summary>
        /// <example>true</example>
        [Required]
        public bool FixedToRevision { get; set; }

        /// <summary>
        /// The semver Revision number to fix the channel to.
        /// </summary>
        /// <example>1.2.3</example>
        public string RevisionNumber { get; set; }

        /// <summary>
        /// The semver Range Rule for the revision.
        /// </summary>
        /// <example>~1.2.3</example>
        public string RevisionRange { get; set; }
    }
}
