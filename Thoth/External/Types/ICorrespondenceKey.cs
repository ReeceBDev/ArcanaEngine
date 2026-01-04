namespace Thoth.External.Types
{
    /// <summary> A set of Arcana which relate to a Zodiacal correspondence upon a practitioner's natevity. </summary>
    public interface ICorrespondenceKey
    {
        /// <summary> The target of this correspondence, i.e. The rising sun, Mars, Venus, etc. </summary>
        public CorrespondenceKeyOption Role { get; }

        /// <summary> The Major Arcana card which relates to this correspondence's Zodiac, and its archetypical relationships. </summary>
        IArcanaCard Zodiac { get; }

        /// <summary> The Decan card which relates to this correspondence, and its archetypical relationships. </summary>
        IArcanaCard Decan { get; }

        /// <summary> The Court card which relates to this correspondence, and its archetypical relationships. </summary>
        IArcanaCard Court { get; }
    }
}