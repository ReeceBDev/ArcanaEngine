namespace Thoth.WebAPI.Responses
{
    /// <summary> User-facing warning messages relating to zodiacal cusp proximity. </summary>
    internal static class CuspWarningMessages
    {
        /// <summary> Shown when the practitioner's birth date falls near a zodiacal cusp,
        /// meaning adjacent days may yield a different zodiacal sun sign. </summary>
        internal const string NearCusp =
            "Your date of birth falls near the cusp of a zodiacal change. " +
            "Adjacent days may yield a different zodiacal sun sign. " +
            "Please provide your exact birth time so a more accurate result can be confirmed.";
    }
}
