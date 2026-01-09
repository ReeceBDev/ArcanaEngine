namespace Thoth.Types.Zodiacal
{
    /// <summary>
    /// Represents a celestial position as a zodiac sign and degree within that sign.
    /// </summary>
    internal interface IEclipticDegree
    {
        /// <summary> The zodiac sign (Aries, Taurus, etc.) </summary>
        ZodiacSign Sign { get; }

        /// <summary> The degree within the sign (0-29) </summary>
        int RelativeDegree { get; }

        /// <summary> The absolute degree along the ecliptic (0-359) </summary>
        int AbsoluteDegree { get; }
    }
}
