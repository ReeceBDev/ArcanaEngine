using Thoth.Resources.Json;
using Thoth.Types.Thoth;
using Thoth.Types.Thoth.Data;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    internal class ThothCalculator(IThothDeck thothDeck) : IThothCalculator
    {
        private IThothDeck thothDeck = thothDeck;

        private const int decanDegreeStep = 10;             // Degree difference between consecutive decans
        private const int courtCardDegreeStep = 30;         // Degree difference between consecutive court cards.
        private const int firstCourtCardDegree = 21;        // Starting degree of the first court card. Court cards then increment by each 30 degree step.
        private const int totalZodiacDegrees = 360;         // Total degrees in the circle which forms the zodiac.


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

        public MajorArcana GetMajorArcanaByZodiac(EclipticZodiac zodiac)
            => ArcanaToZodiacMapping.EclipticToArcana[zodiac];

        public MinorArcanaAddedToOffset GetDecanCardByAbsoluteDegree(int absoluteDegree)
        {
            if (absoluteDegree < 0 || absoluteDegree > 359)
                throw new ArgumentOutOfRangeException(nameof(absoluteDegree));


            // Count how many full 10-degree steps fit below the input degree
            int numberOfStepsBelow = absoluteDegree / decanDegreeStep;

            // Compute the lower decan degree based on the step count
            int snappedDecanDegree = numberOfStepsBelow * decanDegreeStep;


            return ZodiacToDecanMapping.DegreeToDecan[snappedDecanDegree];
        }

        public MinorArcanaAddedToOffset GetCourtCardByAbsoluteDegree(int absoluteDegree)
        {
            if (absoluteDegree < 0 || absoluteDegree > 359)
                throw new ArgumentOutOfRangeException(nameof(absoluteDegree));


            // Distance from the first court card degree
            double offsetFromFirstCard = absoluteDegree - firstCourtCardDegree;

            // Count how many full 30-degree steps fit below the input degree
            int numberOfStepsBelow = (int)Math.Floor(offsetFromFirstCard / courtCardDegreeStep);

            // Compute the lower court card degree based on the step count
            int snappedCourtCardDegree = numberOfStepsBelow * courtCardDegreeStep + firstCourtCardDegree;

            // Wrap around the zodiac circle when the degree is below the first card
            int wrappedCourtCardDegree = (snappedCourtCardDegree + totalZodiacDegrees) % totalZodiacDegrees;


            return ZodiacToCourtMapping.DegreeToCourt[wrappedCourtCardDegree];
        }
    }
}