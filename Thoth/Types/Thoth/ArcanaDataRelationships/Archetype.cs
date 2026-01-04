using Thoth.Types.Alchemy;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth.CardDataStructure
{
    internal readonly record struct Archetype : IArchetype
    {
        public IArcana Arcana { get; init; }
        public AlchemicalElement? Element { get; init; }
        public AstrologicalMode? Mode { get; init; }
        public AstrologicalSign? Zodiac { get; init; }
    }
}