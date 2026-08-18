using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    /// <summary> A single house of a practitioner's natal celestial wheel, alongside the arcana which correspond to its cusp. </summary>
    internal interface IZodiacalHouse
    {
        /// <summary> The house's number, from 1 to 12. </summary>
        int House { get; }

        /// <summary> The ecliptic degree of the house's cusp. </summary>
        IEclipticDegree Cusp { get; }

        /// <summary> The set of Arcana which relate to this house's cusp degree. </summary>
        IZodiacalArcanaCorrespondence Arcana { get; }
    }
}
