namespace Thoth.Types.Zodiacal
{
    /// <summary>
    /// Represents a celestial position as a zodiac sign and degree within that sign.
    /// </summary>
    internal readonly record struct EclipticDegree : IEclipticDegree
    {
        /// <summary> The zodiac sign (Aries, Taurus, etc.) </summary>
        public ZodiacSign Sign { get; init; }

        /// <summary> The relative degree within the zodiac sign (0-29) </summary>
        public int RelativeDegree { get; init; }

        /// <summary> The absolute degree along the ecliptic (0-359) </summary>
        public int AbsoluteDegree { get; init; }

        static EclipticDegree()
        {
            if (Enum.GetValues(typeof(ZodiacSign)).Length != 12)
                throw new Exception($"{nameof(ZodiacSign)} does not contain twelve entries! There should only be twelve entries!");
        }

        public EclipticDegree(int absoluteDegree)
        {
            if (absoluteDegree < 0 || absoluteDegree >= 360)
                throw new ArgumentOutOfRangeException(nameof(absoluteDegree), $"The input for this was out of bounds - {nameof(absoluteDegree)} must be between 0 and 359.");

            int signIndex = absoluteDegree / 30;

            Sign = (ZodiacSign)signIndex;
            RelativeDegree = absoluteDegree % 30;
            AbsoluteDegree = absoluteDegree;
        }
    }
}
