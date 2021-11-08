using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Core.Interfaces;

namespace Hippo.Core.Messages
{
    /// <summary>
    /// Response payload for a new ChannelMessage API Request.
    /// </summary>
    public class CreateChannelResponse : ChannelMessage, ITraceable
    {
        /// <summary>
        /// The GUID of the ChannelMessage which was created.
        /// </summary>
        /// <example>4208d635-7618-4150-b8a8-bc3205e70e32</example>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace striing</returns>
        public virtual string FormatTrace()
        => $"{GetType().Name}[ChannelId={Id},Appid={AppId}, Name={Name}, RevisionSelectionStrategy={RevisionSelectionStrategy}, RevisionNumber={RevisionNumber}, RevisionRange={RevisionRange}]";
    }
}
