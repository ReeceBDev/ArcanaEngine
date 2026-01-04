using Thoth.Types.Thoth.ArcanaData;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal class ThothCalculator : IThothCalculator
    {
        public int CalculateGrowthOffset(DateTime nativety, int targetYear)
        {;
            int birthDate = nativety.Day;
            int birthMonth = nativety.Month;
            int baseSum = birthDate + birthMonth + targetYear;

            return CalculateCrossSum(baseSum);
        }

        public int CalculateCrossSum(int number)
        {
            //Guard clause in case a number is so small that its cross sum would be equivalent to itself!
            if (number <= 9)
                return number;

            int calculatedValue = 0;

            //When a number is no higher than maximum final cross-sum, we must assume that the caller wanted to reduce the number to its smallest possible cross-sum.
            //When a number is larger, we simply reduce it down until it fits within its first possible cross-sum result, the limit of course being 22...
            do
            {
                calculatedValue = 0;
                while (number > 0)
                {
                    number = Math.DivRem(number, 10, out var digit);
                    calculatedValue += digit;
                }

                number = calculatedValue;
            }
            while (calculatedValue > 22);

            return calculatedValue;
        }

        public MajorArcana GetArcanaByZodiac(EclipticZodiac zodiac)
            => ArcanaToZodiacMapping.EclipticToArcana[zodiac];
    }
}