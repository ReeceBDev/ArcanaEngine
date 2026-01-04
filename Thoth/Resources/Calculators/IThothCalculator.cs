using Thoth.Types.Thoth.ArcanaData;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal interface IThothCalculator
    {
        int CalculateGrowthOffset(DateTime birthDate, int targetYear);
        int CalculateCrossSum(int number);
        MajorArcana GetArcanaByZodiac(EclipticZodiac zodiac);
    }
}
