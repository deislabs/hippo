using Hippo.Core.Models;
using Newtonsoft.Json;

namespace Hippo.Core.Serialization;

public class FieldConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var fieldInnerPropertyType = objectType
            .GetProperty(nameof(Field<string>.Value))
            .PropertyType;
        var jsonValue = ReadInnerFieldValue(reader, serializer, fieldInnerPropertyType);

        return Activator.CreateInstance(objectType, new[] { jsonValue });
    }

    private static object ReadInnerFieldValue(JsonReader reader, JsonSerializer serializer, Type fieldInnerPropertyType)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
            
        return serializer.Deserialize(reader, fieldInnerPropertyType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var fieldInnerProperty = value
            .GetType()
            .GetProperty(nameof(Field<string>.Value));

        var fieldInnerValue = fieldInnerProperty.GetValue(value);
        serializer.Serialize(writer, fieldInnerValue);
    }
}
