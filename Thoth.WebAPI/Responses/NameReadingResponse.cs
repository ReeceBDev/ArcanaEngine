namespace Thoth.WebAPI.Responses
{
    /// <summary> Contains personality cards and name cards derived from a practitioner's birth date and name. </summary>
    /// <param name="PersonalityCards"> The practitioner's personality and character cards. Null when <see cref="InvalidNameError"/> is non-null. </param>
    /// <param name="NameCards"> Arcana derived from the practitioner's name via Hebrew gematria. Null when <see cref="InvalidNameError"/> is non-null. </param>
    /// <param name="InvalidNameError"> Non-null when the name was rejected due to an unresolved letter C.
    /// Contains the full K / Z substitution guidance and should be surfaced directly to the practitioner. </param>
    internal record NameReadingResponse(
        IEnumerable<ArcanaCardResponse>? PersonalityCards,
        IEnumerable<ArcanaCardResponse>? NameCards,
        string? InvalidNameError);
}
