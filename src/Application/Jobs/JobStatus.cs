using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hippo.Application.Jobs;

[JsonConverter(typeof(StringEnumConverter))]
public enum JobStatus
{
    Unknown = 0,
    Pending = 1,
    Running = 2,
    Dead = 3,
}
