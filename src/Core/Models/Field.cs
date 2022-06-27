using Hippo.Core.Serialization;
using Newtonsoft.Json;

namespace Hippo.Core.Models;

[JsonConverter(typeof(FieldConverter))]
public class Field<TValue>
{
    public TValue Value { get; private set; }

    public Field(TValue value)
    {
        Value = value;
    }

    public static implicit operator Field<TValue>(TValue value)
    {
        return new Field<TValue>(value);
    }
}
