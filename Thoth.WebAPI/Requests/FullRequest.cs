namespace Thoth.WebAPI.Requests
{
    /// <summary> Carries a complete practitioner nativety for retrieving all available cards. </summary>
    /// <param name="BirthDate"> The practitioner's date of birth. Expected format: <c>yyyy-MM-dd</c>. </param>
    /// <param name="Name"> The practitioner's full name, with each part separated by spaces.
    /// Must not contain the letter C unless it is directly followed by H. See the K / Z substitution rule. </param>
    /// <param name="BirthTime"> The practitioner's precise birth time including timezone offset.
    /// Expected format: <c>yyyy-MM-ddTHH:mm:sszzz</c>, e.g. <c>1990-05-14T14:30:00+01:00</c>. </param>
    /// <param name="Latitude"> Latitude of the practitioner's birth location, used to calculate the ascendant sign. </param>
    /// <param name="Longitude"> Longitude of the practitioner's birth location, used to calculate the ascendant sign. </param>
    internal record FullRequest(
        string BirthDate,
        string Name,
        string BirthTime,
        double Latitude,
        double Longitude);
}
