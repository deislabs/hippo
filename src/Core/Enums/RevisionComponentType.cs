using System.ComponentModel;

namespace Hippo.Core.Enums;

public enum RevisionComponentType
{
    [Description("HTTP")]
    Http = 1,

    [Description("Redis")]
    Redis = 2,
}
