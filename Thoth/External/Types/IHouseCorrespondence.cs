namespace Thoth.External.Types
{
    /// <summary> A single house of a practitioner's natal celestial wheel, alongside the arcana which relate to its cusp. </summary>
    public interface IHouseCorrespondence
    {
        /// <summary> The house's number, from 1 to 12. </summary>
        int House { get; }

        /// <summary> The zodiac sign which sits upon this house's cusp, e.g. Taurus. </summary>
        string ZodiacSign { get; }

        /// <summary> The degree of the cusp within its zodiac sign (0-29). </summary>
        int Degree { get; }

        /// <summary> The Major Arcana card which relates to this house's zodiac sign, and its archetypical relationships. </summary>
        IArcanaCard Zodiac { get; }

        /// <summary> The Decan card which relates to this house's cusp, and its archetypical relationships. </summary>
        IArcanaCard Decan { get; }

        /// <summary> The Court card which relates to this house's cusp, and its archetypical relationships. </summary>
        IArcanaCard Court { get; }
    }
}
