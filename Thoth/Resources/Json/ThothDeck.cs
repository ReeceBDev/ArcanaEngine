using System.Text.Json;
using System.Text.Json.Serialization;
using Thoth.Types.Thoth;
using Thoth.Types.Thoth.Data;

namespace Thoth.Resources.Json
{
    internal class ThothDeck : IThothDeck
    {
        private readonly Dictionary<Enum, IArchetype> arcanaCache = new();

        private const string MajorArcanaPath = "Resources/Data/Thoth_MajorArcana";
        private const string MinorArcanaPath = "Resources/Data/Thoth_MinorArcana";

        public IArchetype FetchMajorArcana(int arcanaNumeric)
        {
            IArchetype arcanaArchetype;
            MajorArcana arcanaIdentity;

            //Handle the Fool being both 0 and 22... (Around and around we go...)
            if (arcanaNumeric == 22) { arcanaNumeric = 0; }

            // Check whether the given input parameter numeric is valid.
            arcanaIdentity = Enum.IsDefined(typeof(MajorArcana), arcanaNumeric) ?
                (MajorArcana)arcanaNumeric :
                throw new ArgumentOutOfRangeException($"{nameof(arcanaNumeric)} was not a digit listed within {nameof(MajorArcana)}!");

            arcanaArchetype = GetCachedOrGenerate(arcanaIdentity, MajorArcanaPath);

            return arcanaArchetype;
        }

        public IArchetype FetchMinorArcana(MinorArcanaAddedToOffset arcanaReference)
            => GetCachedOrGenerate(arcanaReference, MinorArcanaPath);

        public IArchetype FetchMinorArcana(MinorArcanaAddedToOffset suitOffset, int cardNumber)
        {
            MinorArcanaAddedToOffset arcanaIdentity;

            // Check whether the given input parameter numeric is valid.
            int arcanaEnumIndex = (int)suitOffset + cardNumber;

            arcanaIdentity = Enum.IsDefined(typeof(MinorArcanaAddedToOffset), arcanaEnumIndex) ?
                (MinorArcanaAddedToOffset)arcanaEnumIndex :
                throw new ArgumentOutOfRangeException($"{nameof(arcanaEnumIndex)} was not a digit listed within {nameof(MinorArcanaAddedToOffset)} when added to this offset! Ensure that the suitOffset added to the cardNumber does indeed match a given card's index within the Enum!");

            return FetchMinorArcana(arcanaIdentity);
        }


        /// <summary> Get the cached archetype or generate it if was not already present. </summary>
        private IArchetype GetCachedOrGenerate<T>(T arcanaIdentity, string filePath) where T : Enum
        {
            IArchetype? arcanaArchetype;

            if (!arcanaCache.TryGetValue(arcanaIdentity, out arcanaArchetype))
            {
                arcanaArchetype = DeserializeArcana(arcanaIdentity, filePath);
                arcanaCache[arcanaIdentity] = arcanaArchetype;
            }

            return arcanaArchetype;
        }

        /// <summary> Perform the actual deserialization of an arcana. </summary>
        private IArchetype DeserializeArcana<T>(T arcanaIdentity, string rootPath) where T : Enum
        {
            string searchPattern = $"*_{arcanaIdentity.ToString()}.json";
            string filePath;
            string arcanaJson;
            IArchetype? arcanaArchetype;
            string[] matchingFiles = Directory.GetFiles(rootPath, searchPattern);

            JsonSerializerOptions options = new();
            options.Converters.Add(new ArchetypeConverter());
            options.Converters.Add(new JsonStringEnumConverter());

            // Obtain a single file with a name matching the given enum.
            if (matchingFiles.Length > 1)
                throw new InvalidOperationException($"Multiple files found matching pattern: {searchPattern}");

            filePath = matchingFiles[0];

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Major Arcana file not found: {filePath}");


            // Deserialize the archetype from the file arcana data.
            arcanaJson = File.ReadAllText(filePath);
            arcanaArchetype = JsonSerializer.Deserialize<Archetype>(arcanaJson, options);

            if (arcanaArchetype is null)
                throw new JsonException($"Failed to deserialize major arcana from {filePath}");

            return arcanaArchetype;
        }
    }
}
