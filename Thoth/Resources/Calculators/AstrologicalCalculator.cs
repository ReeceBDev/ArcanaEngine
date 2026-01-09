using SwissEphNet;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal class AstrologicalCalculator : IAstrologicalCalculator
    {
        private readonly SwissEph swissEph;

        public AstrologicalCalculator()
        {
            swissEph = new SwissEph();
            // Set the path to ephemeris files if needed
            // swissEph.swe_set_ephe_path("path/to/ephemeris/files");
        }

        public bool CheckIfNearCusp(DateTime birthDate)
        {
            double julianDay = ToJulianDay(birthDate, 12.0);
            double julianDayBefore = julianDay - 1;
            double julianDayAfter = julianDay + 1;

            ZodiacSign degreeBefore = CalculateZodiacalSunDegree(julianDayBefore).Sign;
            ZodiacSign degreeNow = CalculateZodiacalSunDegree(julianDay).Sign;
            ZodiacSign degreeAfter = CalculateZodiacalSunDegree(julianDayAfter).Sign;

            // Check if sign changes within the day before or after
            return (degreeNow != degreeBefore || degreeNow != degreeAfter);
        }

        public IEclipticDegree ApproximateZodiacalSun(DateTime birthDate)
        {
            double julianDay = ToJulianDay(birthDate, 12.0);
            return CalculateZodiacalSunDegree(julianDay);
        }

        public IEclipticDegree GetCelestialPositionByTime(CelestialBody body, DateTimeOffset birthTime)
        {
            double julianDay = ToJulianDay(birthTime);

            double[] position = new double[6];
            string serr = "";

            int planetId = GetSwissPlanetId(body);

            // Calculate planetary position (geocentric, apparent, tropical)
            int result = swissEph.swe_calc_ut(
                julianDay,
                planetId,
                SwissEph.SEFLG_SWIEPH,
                position,
                ref serr);

            if (result < 0)
                throw new InvalidOperationException($"Failed to calculate {body} position: {serr}");

            int absoluteDegree = (int)Math.Floor(position[0]);

            return new EclipticDegree(absoluteDegree);
        }

        public IEclipticDegree GetAscendantByTime(DateTimeOffset birthTime, double latitude, double longitude)
        {
            double julianDay = ToJulianDay(birthTime);

            double[] cusps = new double[13]; // 12 house cusps + 1
            double[] ascmc = new double[10]; // Ascendant, MC, etc.
            string serr = "";

            // Calculate houses using Placidus system
            int result = swissEph.swe_houses(
                julianDay,
                latitude,
                longitude,
                'P', // Placidus house system
                cusps,
                ascmc);

            if (result < 0)
                throw new InvalidOperationException($"Failed to calculate Ascendant: {serr}");

            // ascmc[0] contains the Ascendant degree (0-359.999)
            int absoluteDegree = (int)Math.Floor(ascmc[0]);

            return new EclipticDegree(absoluteDegree);
        }

        // Dispose pattern for SwissEph
        public void Dispose()
        {
            swissEph?.Dispose();
        }

        /// <summary>
        /// Calculate only the zodiac sign (not full degree) for a given Julian Day
        /// </summary>
        private IEclipticDegree CalculateZodiacalSunDegree(double julianDay)
        {
            double[] position = new double[6];
            string serr = "";

            int result = swissEph.swe_calc_ut(
                julianDay,
                SwissEph.SE_SUN,
                SwissEph.SEFLG_SWIEPH,
                position,
                ref serr);

            if (result < 0)
                throw new InvalidOperationException($"Failed to calculate Sun position: {serr}");

            int absoluteDegree = (int)Math.Floor(position[0]);
            //old code return (EclipticZodiac)(absoluteDegree / 30);
            return new EclipticDegree(absoluteDegree);
        }

        /// <summary>
        /// Convert Date to Julian Day
        /// </summary>
        private double ToJulianDay(DateTime date, double hour)
        {
            return swissEph.swe_julday(
                date.Year,
                date.Month,
                date.Day,
                hour,
                SwissEph.SE_GREG_CAL); // Gregorian calendar
        }

        /// <summary>
        /// Convert DateTimeOffset to Julian Day (UTC) using Time and TimeZone information
        /// </summary>
        private double ToJulianDay(DateTimeOffset dateTimeOffset)
        {
            DateTime utc = dateTimeOffset.UtcDateTime;
            double time = utc.Hour + (utc.Minute / 60.0) + (utc.Second / 3600.0);

            return swissEph.swe_julday(
                utc.Year,
                utc.Month,
                utc.Day,
                time,
                SwissEph.SE_GREG_CAL);
        }

        /// <summary>
        /// Map CelestialBody enum to Swiss Ephemeris planet IDs
        /// </summary>
        private int GetSwissPlanetId(CelestialBody body)
        {
            return body switch
            {
                CelestialBody.Sun => SwissEph.SE_SUN,
                CelestialBody.Moon => SwissEph.SE_MOON,
                CelestialBody.Mercury => SwissEph.SE_MERCURY,
                CelestialBody.Venus => SwissEph.SE_VENUS,
                CelestialBody.Mars => SwissEph.SE_MARS,
                CelestialBody.Jupiter => SwissEph.SE_JUPITER,
                CelestialBody.Saturn => SwissEph.SE_SATURN,
                _ => throw new ArgumentException($"Unsupported celestial body: {body}")
            };
        }
    }
}