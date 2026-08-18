using Thoth.Managers;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    /// <summary> A single house of a practitioner's natal celestial wheel, alongside the arcana which correspond to its cusp. </summary>
    internal sealed class ZodiacalHouse : IZodiacalHouse
    {
        /// <summary> The house's number, from 1 to 12. </summary>
        public int House { get; }

        /// <summary> The ecliptic degree of the house's cusp. </summary>
        public IEclipticDegree Cusp { get; }

        /// <summary> The set of Arcana which relate to this house's cusp degree. </summary>
        public IZodiacalArcanaCorrespondence Arcana { get; }

        public ZodiacalHouse(ICardProvider cardProvider, int house, IEclipticDegree cusp)
        {
            if (house < 1 || house > 12)
                throw new ArgumentOutOfRangeException(nameof(house), $"The input for this was out of bounds - {nameof(house)} must be between 1 and 12.");

            House = house;
            Cusp = cusp;
            Arcana = new ZodiacalCorrespondence(cardProvider, cusp);
        }
    }
}
