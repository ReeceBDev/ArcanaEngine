namespace Thoth.WebAPI.Requests
{
    /// <summary> Carries a practitioner's date of birth for retrieving their personality cards. </summary>
    /// <param name="BirthDate"> The practitioner's date of birth. Expected format: <c>yyyy-MM-dd</c>. </param>
    internal record BirthdateRequest(string BirthDate);
}
