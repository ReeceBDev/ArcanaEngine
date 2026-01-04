using Thoth.Types.Alchemy;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth.CardDataStructure
{
    /// <summary> Brings an Arcana and its related aspects together into one archetypical whole. </summary>
    internal interface IArchetype
    {
        /// <summary> The arcana in question for this archetype. </summary>
        IArcana Arcana { get; }

        /// <summary> The zodiacal sign which this Arcana relates to. </summary>
        AstrologicalSign? Zodiac { get; }

        /// <summary> The alchemical element which this Arcana represents. </summary>
        AlchemicalElement? Element { get; }

        /// <summary> The astrological mode which this Arcana represents. </summary>
        AstrologicalMode? Mode { get; }
    }
}
