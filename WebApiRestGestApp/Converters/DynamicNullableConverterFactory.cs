using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiRestGestApp.Converters
{
    public class DynamicNullableConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return Nullable.GetUnderlyingType(typeToConvert) != null;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type underlyingType = Nullable.GetUnderlyingType(typeToConvert);
            Type converterType = typeof(DynamicNullableConverter<>).MakeGenericType(underlyingType);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }

        private class DynamicNullableConverter<T> : JsonConverter<T?> where T : struct
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string value = reader.GetString();

                    if (string.IsNullOrWhiteSpace(value))
                        return null;

                    try
                    {
                        var converter = TypeDescriptor.GetConverter(typeof(T));
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                        {
                            return (T?)converter.ConvertFromString(value);
                        }
                    }
                    catch { }
                }

                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                return JsonSerializer.Deserialize<T>(ref reader, options);
            }

            public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                    JsonSerializer.Serialize(writer, value.Value, options);
                else
                    writer.WriteNullValue();
            }
        }
    }
}