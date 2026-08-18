using Thoth.External.Types;

namespace Thoth.WebAPI.Responses
{
    /// <summary> A serialisable representation of an <see cref="ICorrespondence"/>.
    /// Captures a single celestial body's three related arcana cards. </summary>
    /// <param name="Role"> The celestial body this correspondence belongs to, e.g. <see cref="CorrespondenceOption.ZodiacalSun"/>. </param>
    /// <param name="Zodiac"> The Major Arcana card relating to this correspondence's zodiac sign. </param>
    /// <param name="Decan"> The Minor Arcana decan card relating to this correspondence. </param>
    /// <param name="Court"> The court card relating to this correspondence. </param>
    internal record CorrespondenceResponse(
        string Role,
        ArcanaCardResponse Zodiac,
        ArcanaCardResponse Decan,
        ArcanaCardResponse Court)
    {
        /// <summary> Maps an <see cref="ICorrespondence"/> to its serialisable response shape. </summary>
        internal static CorrespondenceResponse From(ICorrespondence correspondence)
            => new(
                correspondence.Role.ToString(),
                ArcanaCardResponse.From(correspondence.Zodiac),
                ArcanaCardResponse.From(correspondence.Decan),
                ArcanaCardResponse.From(correspondence.Court));
    }
}
