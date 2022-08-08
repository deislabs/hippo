using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hippo.Core.Enums;

/// <summary>
/// The strategy to use to select a revision for the Channel
/// </summary>

[JsonConverter(typeof(StringEnumConverter))]
public enum ChannelRevisionSelectionStrategy
{
    /// <summary>
    /// Use a range rule to select the most appropriate revision for the channel
    /// </summary>
    UseRangeRule = 0,
    /// <summary>
    /// Use a specific revision version for the channel
    /// </summary>
    UseSpecifiedRevision = 1
}
