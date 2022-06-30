using Hippo.Core.Models;
using Newtonsoft.Json;

namespace Hippo.Core.Serialization;

public class FieldConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var fieldInnerPropertyType = objectType
            .GetProperty(nameof(Field<string>.Value))?
            .PropertyType;
        var jsonValue = ReadInnerFieldValue(reader, serializer, fieldInnerPropertyType);

        return Activator.CreateInstance(objectType, new[] { jsonValue });
    }

    private static object? ReadInnerFieldValue(JsonReader reader, JsonSerializer serializer, Type? fieldInnerPropertyType)
    {
        if (reader.TokenType == JsonToken.Null || fieldInnerPropertyType is null)
            return null;

        return serializer.Deserialize(reader, fieldInnerPropertyType);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        object? val = null;
        if (value is not null)
        {
            var fieldInnerProperty = value
                .GetType()
                .GetProperty(nameof(Field<string>.Value));

            val = fieldInnerProperty is null ? null : fieldInnerProperty.GetValue(value);
        }

        serializer.Serialize(writer, val);
    }
}
