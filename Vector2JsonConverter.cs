using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Numerics;

namespace Delfinovin
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON object from the reader.
            var jsonObject = JObject.Load(reader);

            // Deserialize the object using the values of the x and y properties.
            return new Vector2((float)jsonObject["X"], (float)jsonObject["Y"]);
        }

        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            // Serialize the Vector2 as a JSON object with x and y properties.
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            serializer.Serialize(writer, value.X);
            writer.WritePropertyName("Y");
            serializer.Serialize(writer, value.Y);
            writer.WriteEndObject();
        }
    }
}
