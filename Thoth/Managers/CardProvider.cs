using Thoth.Resources.Calculators;
using Thoth.Resources.Json;
using Thoth.Types.Thoth;
using Thoth.Types.Thoth.Data;
using Thoth.Types.Zodiacal;

namespace Thoth.Managers
{
    internal class CardProvider(
        IThothCalculator thothCalculator,
        IThothDeck cardBuilder,
        IGemetriaCalculator gemetriaCalculator,
        IAstrologicalCalculator astrologicalCalculator)
        : ICardProvider
    {
        private readonly IThothCalculator thothCalculator = thothCalculator;
        private readonly IThothDeck cardBuilder = cardBuilder;
        private readonly IGemetriaCalculator gemetriaCalculator = gemetriaCalculator;
        private readonly IAstrologicalCalculator astrologicalCalculator = astrologicalCalculator;

        public IArchetype GetGrowthCardByYear(int year, DateTime nativetyDate)
        {
            int nativetyDay = nativetyDate.Day;
            int nativetyMonth = nativetyDate.Month;

            int precalculatedSum = nativetyDay + nativetyMonth + year;
            int crossSum = thothCalculator.CalculateCrossSum(precalculatedSum);

            return cardBuilder.FetchMajorArcana(crossSum); ;
        }

        public IArchetype GetCardByHebrewName(string name)
        {
            int nameAsGemetria = gemetriaCalculator.GetGemetriaHebrewAppromimation(name);
            int nameCrossSum = thothCalculator.CalculateCrossSum(nameAsGemetria);

            return cardBuilder.FetchMajorArcana(nameCrossSum);
        }

        public IArchetype? GetTeacherCard(DateTime birthDate)
        {
            int personalityCrossSum = CalculatePersonalitySum(birthDate);
            var teacherCrossSum = thothCalculator.CalculateCrossSum(personalityCrossSum);

            // The practitioner draws no teacher card if their cross-sum cannot be further reduced from their character card...
            return personalityCrossSum == teacherCrossSum ? null : cardBuilder.FetchMajorArcana(teacherCrossSum);
        }

        public IArchetype GetPersonalityCard(DateTime birthDate)
        {
            int crossSum = CalculatePersonalitySum(birthDate);
            return cardBuilder.FetchMajorArcana(crossSum);
        }

        public IArchetype GetZodiacCard(ZodiacSign sign)
        {
            MajorArcana arcana = thothCalculator.GetMajorArcanaByZodiac(sign);

            return cardBuilder.FetchMajorArcana((int)arcana);
        }

        public IArchetype GetDecanCard(int absoluteDegree)
        {
            MinorArcanaAddedToOffset minorArcana = thothCalculator.GetDecanCardByAbsoluteDegree(absoluteDegree);

            return cardBuilder.FetchMinorArcana(minorArcana);
        }

        public IArchetype GetCourtCard(int absoluteDegree)
        {
            MinorArcanaAddedToOffset minorArcana = thothCalculator.GetCourtCardByAbsoluteDegree(absoluteDegree);

            return cardBuilder.FetchMinorArcana(minorArcana);
        }

        private int CalculatePersonalitySum(DateTime birthDate)
        {
            int birthDay = birthDate.Day;
            int birthMonth = birthDate.Month;
            int birthYear = birthDate.Year;

            int birthSum = birthDay + birthMonth + birthYear;
            int crossSum = thothCalculator.CalculateCrossSum(birthSum);

            return crossSum;
        }
    }
}
