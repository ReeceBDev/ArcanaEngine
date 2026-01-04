using System.Text.Json;
using Thoth.Types.Thoth.CardDataStructure;

namespace Thoth.Resources.Json
{
    internal class ArcanaDeserializer
    {
        public IArchetype DeserializeMajorArcanaByNumeric(int arcanaIdentity)
        {
            string targetPrefix = arcanaIdentity.ToString();
            string filePath = $"Data/Thoth_MajorArcana/{arcanaIdentity}.json";
            string arcanaJson;
            IArchetype? arcanaArchetype;

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arcana file not found: {filePath}");

            arcanaJson = File.ReadAllText($"Data/Thoth_MajorArcana/{targetPrefix}" + "*.json");
            arcanaArchetype = JsonSerializer.Deserialize<Archetype>(arcanaJson);

            if (arcanaArchetype is null)
                throw new JsonException($"Failed to deserialize arcana from {filePath}");

            return arcanaArchetype;
        }
    }
}
