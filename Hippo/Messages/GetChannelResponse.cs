using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;
using SemVer;

namespace Hippo.Messages
{
    /// <summary>
    /// Response body for a get ChannelMessage API Request.
    /// </summary>
    public class GetChannelResponse : ChannelMessage, ITraceable
    {
        /// <summary>
        /// ITraceable.FormatTrace implementation.
        /// </summary>
        /// <returns>Trace striing</returns>
        public virtual string FormatTrace()
        => $"{GetType().Name}[Appid={AppId}, Name={Name}, FixedToRevision={FixedToRevision}, RevisionNumber={RevisionNumber}]";
    }
}

