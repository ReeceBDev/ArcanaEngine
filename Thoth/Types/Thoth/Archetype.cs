using Thoth.Types.Alchemy;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    internal readonly record struct Archetype : IArchetype
    {
        public IArcana Arcana { get; init; }
        public AlchemicalElement? Element { get; init; }
        public AstrologicalMode? Mode { get; init; }
        public ZodiacSign? Zodiac { get; init; }
    }
}