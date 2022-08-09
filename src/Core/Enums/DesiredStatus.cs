using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hippo.Core.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum DesiredStatus
{
    Running = 2,
    Dead = 3,
}
