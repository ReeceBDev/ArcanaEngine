using Thoth.External.Types;
using Thoth.Types.Thoth;

namespace Thoth.External.InternalConcreteDependencies
{
    /// <summary> A single house of a practitioner's natal celestial wheel, alongside the arcana which relate to its cusp. </summary>
    internal sealed class HouseCorrespondence : IHouseCorrespondence
    {
        /// <summary> The house's number, from 1 to 12. </summary>
        public int House { get; }

        /// <summary> The zodiac sign which sits upon this house's cusp, e.g. Taurus. </summary>
        public string ZodiacSign { get; }

        /// <summary> The degree of the cusp within its zodiac sign (0-29). </summary>
        public int Degree { get; }

        /// <summary> The Major Arcana card which relates to this house's zodiac sign, and its archetypical relationships. </summary>
        public IArcanaCard Zodiac { get; }

        /// <summary> The Decan card which relates to this house's cusp, and its archetypical relationships. </summary>
        public IArcanaCard Decan { get; }

        /// <summary> The Court card which relates to this house's cusp, and its archetypical relationships. </summary>
        public IArcanaCard Court { get; }

        public HouseCorrespondence(IZodiacalHouse houseInput)
        {
            House = houseInput.House;
            ZodiacSign = houseInput.Cusp.Sign.ToString();
            Degree = houseInput.Cusp.RelativeDegree;

            Zodiac = new ArcanaCard(houseInput.Arcana.Zodiac, ArcanaRole.PersonalZodiacalCard);
            Decan = new ArcanaCard(houseInput.Arcana.Decan, ArcanaRole.PersonalDecanCard);
            Court = new ArcanaCard(houseInput.Arcana.Court, ArcanaRole.PersonalCourtCard);
        }
    }
}
