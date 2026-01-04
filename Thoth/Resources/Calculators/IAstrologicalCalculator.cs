using Thoth.Types.Thoth;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal interface IAstrologicalCalculator
    {
        ICelestialDegree GetZodiacalSunDegree(DateTime birthDate);
        ICelestialDegree GetCelestialPositionByTime(CelestialBody body, DateTimeOffset birthTime);
        ICelestialDegree GetAscendantByTime(DateTimeOffset birthTime, double latitude, double longitude);
    }
}