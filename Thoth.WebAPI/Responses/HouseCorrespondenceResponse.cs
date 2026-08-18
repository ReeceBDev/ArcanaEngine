using Thoth.External.Types;

namespace Thoth.WebAPI.Responses
{
    /// <summary> A serialisable representation of an <see cref="IHouseCorrespondence"/>.
    /// Captures a single natal house's cusp placement and its three related arcana cards. </summary>
    /// <param name="House"> The house's number, from 1 to 12. </param>
    /// <param name="Sign"> The zodiac sign which sits upon this house's cusp. </param>
    /// <param name="Degree"> The degree of the cusp within its sign, from 0 to 29. </param>
    /// <param name="Zodiac"> The Major Arcana card relating to this house's zodiac sign. </param>
    /// <param name="Decan"> The Minor Arcana decan card relating to this house's cusp. </param>
    /// <param name="Court"> The court card relating to this house's cusp. </param>
    internal record HouseCorrespondenceResponse(
        int House,
        string Sign,
        int Degree,
        ArcanaCardResponse Zodiac,
        ArcanaCardResponse Decan,
        ArcanaCardResponse Court)
    {
        /// <summary> Maps an <see cref="IHouseCorrespondence"/> to its serialisable response shape. </summary>
        internal static HouseCorrespondenceResponse From(IHouseCorrespondence house)
            => new(
                house.House,
                house.ZodiacSign,
                house.Degree,
                ArcanaCardResponse.From(house.Zodiac),
                ArcanaCardResponse.From(house.Decan),
                ArcanaCardResponse.From(house.Court));
    }
}
