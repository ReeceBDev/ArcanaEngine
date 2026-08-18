namespace Thoth.WebAPI.Requests
{
    /// <summary> Carries a practitioner's date of birth and full name for retrieving their personality and name cards. </summary>
    /// <param name="BirthDate"> The practitioner's date of birth. Expected format: <c>yyyy-MM-dd</c>. </param>
    /// <param name="Name"> The practitioner's full name, with each part separated by spaces.
    /// Must not contain the letter C unless it is directly followed by H. See the K / Z substitution rule. </param>
    internal record NameRequest(string BirthDate, string Name);
}
