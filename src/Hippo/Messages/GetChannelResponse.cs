using Hippo.Logging;

namespace Hippo.Messages;

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
        => $"{GetType().Name}[Appid={AppId}, Name={Name}, RevisionSelectionStrategy={RevisionSelectionStrategy}, RevisionNumber={RevisionNumber}]";
}
