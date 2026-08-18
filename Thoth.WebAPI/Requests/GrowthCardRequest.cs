namespace Thoth.WebAPI.Requests
{
    /// <summary> Carries a practitioner's birth date, target year, and optional range offsets for retrieving growth cards. </summary>
    internal sealed class GrowthCardRequest
    {
        /// <summary> The practitioner's date of birth. Expected format: <c>yyyy-MM-dd</c>. </summary>
        public string BirthDate { get; init; } = "";

        /// <summary> The target year for which the growth cards should be calculated. </summary>
        public int Year { get; init; }

        /// <summary> Optional number of years before the target year to include. </summary>
        public int? Before { get; init; }

        /// <summary> Optional number of years after the target year to include. </summary>
        public int? After { get; init; }
    }
}