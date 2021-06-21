using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Models;

namespace Hippo.Messages
{

    public class ChannelMessage
    {
        /// <summary>
        /// The GUID of the Application which the channel belongs to.
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
        public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

        /// <summary>
        /// The revision number to fix the channel to - ignored if RevisionSelectionStrategy is not UseSpecifiedRevision.
        /// </summary>
        /// <example>1.2.3</example>
        public string RevisionNumber { get; set; }

        /// <summary>
        /// The range rule for selecting the active revision - ignored if RevisionSelectionStrategy is not UseRangeRule.
        /// </summary>
        /// <example>~1.2.3</example>
        public string RevisionRange { get; set; }
    }
}
