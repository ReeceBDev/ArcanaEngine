using System.Text.Json;
using System.Text.Json.Serialization;
using Thoth.Types.Alchemy;
using Thoth.Types.Thoth;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Json
{
    internal class ArchetypeConverter : JsonConverter<Archetype>
    {
        public override Archetype Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Archetype archetype;
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            // Deserialize nested types as concrete implementations of their interfaces
            Arcana? arcana = JsonSerializer.Deserialize<Arcana>(root.GetProperty("Arcana").GetRawText(), options);
            // Note: Add future types here... No need to validate like the arcana below, as only arcana is truly required...

            // Validate whether arcana has been set correctly.
            if (arcana is null)
                throw new JsonException("Required property 'Arcana' is missing or invalid. This type is essential as it forms the basis of the archetype!");

            // Deserialize implicitly concrete types (enums)
            AlchemicalElement? element = DeserializeNullableProperty<AlchemicalElement>(root, "Element", options);
            AstrologicalMode? mode = DeserializeNullableProperty<AstrologicalMode>(root, "Mode", options);
            EclipticZodiac? zodiac = DeserializeNullableProperty<EclipticZodiac>(root, "Zodiac", options);


            // Combine all realized data structures together into the whole and final type
            archetype = new Archetype
            {
                Arcana = arcana, 
                Element = element,
                Mode = mode,
                Zodiac = zodiac
            };

            return archetype;
        }

        public override void Write(Utf8JsonWriter writer, Archetype value, JsonSerializerOptions options)
        {
            throw new NotSupportedException("Serialization of ArchetypeData is not supported.");
        }

        /// <summary>
        /// Helper method to deserialize an optional nullable property.
        /// </summary>
        private static T? DeserializeNullableProperty<T>(JsonElement root, string propertyName, JsonSerializerOptions options) where T : struct
        {
            if (!root.TryGetProperty(propertyName, out JsonElement propertyElement))
                return null;

            return JsonSerializer.Deserialize<T?>(propertyElement.GetRawText(), options);
        }
    }
}
