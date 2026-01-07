using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal interface IAstrologicalCalculator
    {
        IEclipticDegree GetZodiacalSunDegree(DateTime birthDate);
        IEclipticDegree GetCelestialPositionByTime(CelestialBody body, DateTimeOffset birthTime);
        IEclipticDegree GetAscendantByTime(DateTimeOffset birthTime, double latitude, double longitude);
    }
}