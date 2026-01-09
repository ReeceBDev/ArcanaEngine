using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal interface IAstrologicalCalculator
    {
        /// <summary>
        /// Attempts to calculate the degree of the zodiacal sun degree from a birth date.
        /// It would be ideal to check whether this is near a cusp, for accuracy.
        /// </summary>
        IEclipticDegree ApproximateZodiacalSun(DateTime birthDate);
        IEclipticDegree GetCelestialPositionByTime(CelestialBody body, DateTimeOffset birthTime);
        IEclipticDegree GetAscendantByTime(DateTimeOffset birthTime, double latitude, double longitude);
        bool CheckIfNearCusp(DateTime birthDate);
    }
}