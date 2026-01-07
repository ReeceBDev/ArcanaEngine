using Thoth.Types.Thoth;
using Thoth.Types.Thoth.Data;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal interface IThothCalculator
    {
        int CalculateGrowthOffset(DateTime birthDate, int targetYear);
        int CalculateCrossSum(int number);
        MajorArcana GetMajorArcanaByZodiac(EclipticZodiac zodiac);
        MinorArcanaAddedToOffset GetDecanCardByAbsoluteDegree(int absoluteDegree);
        MinorArcanaAddedToOffset GetCourtCardByAbsoluteDegree(int absoluteDegree);
    }
}
