using Thoth.Types.Thoth;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal class AstrologicalCalculator : IAstrologicalCalculator
    {
        public ICelestialDegree GetZodiacalSunDegree(DateTime birthDate)
        {
            //Calculate based on birth date. If it's near a cusp, warn the user that they are on the cusp between X and Y.
            //And that they need to input their location at the time in order to calculate the rest (we can infer timezone!))
            throw new NotImplementedException();
        }

        public ICelestialDegree GetCelestialPositionByTime(CelestialBody body, DateTimeOffset birthTime)
        {
            throw new NotImplementedException();
        }

        public ICelestialDegree GetAscendantByTime(DateTimeOffset birthTime, double latitude, double longitude)
        {
            throw new NotImplementedException();
        }
    }
}