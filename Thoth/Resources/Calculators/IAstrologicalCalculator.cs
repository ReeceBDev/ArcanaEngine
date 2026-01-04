using Thoth.Types.Thoth;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal interface IAstrologicalCalculator
    {
        int GetZodiacalSunDegree(DateTime birthDate);
        int GetEclipticDegreeByTime(CelestialBody body, DateTimeOffset birthTime);
        int GetAscendantByTime(DateTimeOffset birthTime, double latitude, double longitude);
        EclipticZodiac GetEclipticZodiacByDegree(int absoluteDegree);
    }
}