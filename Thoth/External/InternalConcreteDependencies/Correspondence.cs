using Thoth.External.Types;
using Thoth.Types.Thoth;

namespace Thoth.External.InternalConcreteDependencies
{
    /// <summary> A set of Arcana which relate to a Zodiacal correspondence upon a practitioner's natevity. </summary>
    internal sealed class Correspondence : ICorrespondence
    {
        /// <summary> The target of this correspondence, i.e. The rising sun, Mars, Venus, etc. </summary>
        public CorrespondenceOption Role { get; }

        /// <summary> The Major Arcana card which relates to this correspondence's Zodiac, and its archetypical relationships. </summary>
        public IArcanaCard Zodiac { get; }

        /// <summary> The Decan card which relates to this correspondence, and its archetypical relationships. </summary>
        public IArcanaCard Decan { get; }

        /// <summary> The Court card which relates to this correspondence, and its archetypical relationships. </summary>
        public IArcanaCard Court { get; }

        public Correspondence(IZodiacalArcanaCorrespondence correspondenceInput, CorrespondenceOption correspondenceType)
        {
            Role = correspondenceType;

            Zodiac = new ArcanaCard(correspondenceInput.Zodiac, ArcanaRole.PersonalZodiacalCard);
            Decan = new ArcanaCard(correspondenceInput.Decan, ArcanaRole.PersonalDecanCard);
            Court = new ArcanaCard(correspondenceInput.Court, ArcanaRole.PersonalCourtCard);
        }
    }
}