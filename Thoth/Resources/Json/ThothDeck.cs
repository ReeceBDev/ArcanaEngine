using System;
using System.Text.Json;
using Thoth.Types.Thoth.ArcanaData;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Thoth.Data;

namespace Thoth.Resources.Json
{
    internal class ThothDeck : IThothDeck
    {
        private readonly Dictionary<MajorArcana, IArchetype> majorArcanaCache = new();

        public IArchetype GetMajorArcanaByIndex(int arcanaNumeric)
        {
            IArchetype arcanaArchetype;
            MajorArcana arcanaIdentity;

            //Handle the Fool being both 0 and 22... (Around and around we go...)
            if (arcanaNumeric == 22) { arcanaNumeric = 0; }

            // Check whether the given input parameter numeric is valid.
            arcanaIdentity = Enum.IsDefined(typeof(MajorArcana), arcanaNumeric) ?
                (MajorArcana)arcanaNumeric :
                throw new ArgumentOutOfRangeException($"{nameof(arcanaNumeric)} was not a digit listed within {nameof(MajorArcana)}!");

            arcanaArchetype = GetCachedOrGenerate(arcanaIdentity);

            return arcanaArchetype;
        }

        public IArcana GrabMinorArcanaByIndexAndSuit(int index, MinorArcanaSuit suit)
        {
            //Note: Remember to make this Fool-Safe, too!
            throw new NotImplementedException();
        }

        /// <summary> Get the cached archetype or generate it if was not already present. </summary>
        private IArchetype GetCachedOrGenerate(MajorArcana arcanaIdentity)
        {
            IArchetype? arcanaArchetype;

            if (!majorArcanaCache.TryGetValue(arcanaIdentity, out arcanaArchetype))
            {
                arcanaArchetype = DeserializeMajorArcana(arcanaIdentity);
                majorArcanaCache[arcanaIdentity] = arcanaArchetype;
            }

            return arcanaArchetype;
        }

        /// <summary> Perform the actual deserialization of an arcana. </summary>
        private IArchetype DeserializeMajorArcana(MajorArcana arcanaIdentity)
        {
            string targetPrefix = ((int)arcanaIdentity).ToString();
            string filePath = $"Data/Thoth_MajorArcana/{targetPrefix}_{arcanaIdentity.ToString()}.json";
            string arcanaJson;
            IArchetype? arcanaArchetype;

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arcana file not found: {filePath}");

            arcanaJson = File.ReadAllText(filePath);
            arcanaArchetype = JsonSerializer.Deserialize<Archetype>(arcanaJson);

            if (arcanaArchetype is null)
                throw new JsonException($"Failed to deserialize arcana from {filePath}");

            return arcanaArchetype;
        }
    }
}
