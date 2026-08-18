namespace Thoth.WebAPI.Responses
{
    /// <summary> Contains all cards derivable from a practitioner's birth date alone. </summary>
    /// <param name="PersonalityCards"> The practitioner's personality-derived cards, including the personality card and, when distinct, the character card. </param>
    /// <param name="ZodiacalSunCards"> The practitioner's zodiacal-sun-derived cards available from birth date alone: court, decan, and zodiacal cards. </param>
    /// <param name="CuspWarning"> True when the birth date falls near a zodiacal cusp. Adjacent dates may yield a different zodiacal sun sign, so an exact birth time should be supplied for greater accuracy. </param>
    /// <param name="CuspWarningMessage"> Human-readable cusp warning. Null when <see cref="CuspWarning"/> is false. </param>
    internal record BirthdateReadingResponse(
        IEnumerable<ArcanaCardResponse> PersonalityCards,
        IEnumerable<ArcanaCardResponse> ZodiacalSunCards,
        bool CuspWarning,
        string? CuspWarningMessage);
}