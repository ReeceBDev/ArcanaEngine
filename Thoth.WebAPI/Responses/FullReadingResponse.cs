namespace Thoth.WebAPI.Responses
{
    /// <summary> Contains all cards available to a fully configured practitioner nativety. </summary>
    /// <param name="PersonalityCards"> The practitioner's personality and character cards. Null when <see cref="InvalidNameError"/> is non-null. </param>
    /// <param name="NameCards"> Arcana derived from the practitioner's name via Hebrew gematria. Null when <see cref="InvalidNameError"/> is non-null. </param>
    /// <param name="Correspondences"> All celestial correspondence cards unlocked by a complete nativety. Null when <see cref="InvalidNameError"/> is non-null. </param>
    /// <param name="Houses"> The twelve fixed natal houses of the celestial wheel, each with its cusp sign, degree and related arcana. Null when <see cref="InvalidNameError"/> is non-null. </param>
    /// <param name="CuspWarning"> True when the zodiacal sun sign remains ambiguous despite the supplied birth time. </param>
    /// <param name="CuspWarningMessage"> Human-readable cusp warning. Null when <see cref="CuspWarning"/> is false. </param>
    /// <param name="InvalidNameError"> Non-null when the name was rejected due to an unresolved letter C.
    /// Contains the full K / Z substitution guidance and should be surfaced directly to the practitioner. </param>
    internal record FullReadingResponse(
        IEnumerable<ArcanaCardResponse>? PersonalityCards,
        IEnumerable<ArcanaCardResponse>? NameCards,
        IEnumerable<CorrespondenceResponse>? Correspondences,
        IEnumerable<HouseCorrespondenceResponse>? Houses,
        bool CuspWarning,
        string? CuspWarningMessage,
        string? InvalidNameError);
}
