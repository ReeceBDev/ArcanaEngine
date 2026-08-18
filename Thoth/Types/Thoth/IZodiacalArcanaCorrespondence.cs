namespace Thoth.Types.Thoth
{
    /// <summary> A set of Arcana which relate to a Zodiacal correspondence upon a practitioner's natevity. </summary>
    internal interface IZodiacalArcanaCorrespondence
    {
        /// <summary> The Major Arcana card which relates to this correspondence's Zodiac, and its archetypical relationships. </summary>
        IArchetype Zodiac { get; }

        /// <summary> The Decan card which relates to this correspondence, and its archetypical relationships. </summary>
        IArchetype Decan { get; }

        /// <summary> The Court card which relates to this correspondence, and its archetypical relationships. </summary>
        IArchetype Court { get; }
    }
}