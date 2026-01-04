using System.Collections.Immutable;
using Thoth.Resources.Calculators;
using Thoth.Resources.Json;
using Thoth.Types.Thoth.ArcanaData;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Zodiacal;

namespace Thoth.Managers
{
    internal class CardProvider(
        IThothCalculator thothCalculator, 
        IThothDeck cardBuilder, 
        ITransliterator transliterator, 
        IGemetriaCalculator gemetriaCalculator,
        IAstrologicalCalculator astrologicalCalculator)
        : ICardProvider
    {
        private readonly IThothCalculator thothCalculator = thothCalculator;
        private readonly IThothDeck cardBuilder = cardBuilder;
        private readonly ITransliterator transliterator = transliterator;
        private readonly IGemetriaCalculator gemetriaCalculator = gemetriaCalculator;
        private readonly IAstrologicalCalculator astrologicalCalculator = astrologicalCalculator;

        public IArchetype GetGrowthCardByYear(int year, DateTime nativetyDate)
        {
            int nativetyDay = nativetyDate.Day;
            int nativetyMonth = nativetyDate.Month;

            int precalculatedSum = nativetyDay + nativetyMonth + year;
            int crossSum = thothCalculator.CalculateCrossSum(precalculatedSum);

            return cardBuilder.GetMajorArcanaByIndex(crossSum); ;
        }

        public IArchetype GetCardByHebrewName(string name)
        {
            string hebrewLiteralSymbols = transliterator.ConvertToHebrewSymbomatically(name);
            int nameAsGemetria = gemetriaCalculator.GetGemetriaValues(hebrewLiteralSymbols);
            int nameCrossSum = thothCalculator.CalculateCrossSum(nameAsGemetria);

            return cardBuilder.GetMajorArcanaByIndex(nameCrossSum);
        }
        
        public IArchetype? GetTeacherCard(DateTime birthDate)
        {
            int personalityCrossSum = CalculatePersonalitySum(birthDate);
            var teacherCrossSum = thothCalculator.CalculateCrossSum(personalityCrossSum);

            // The practitioner draws no teacher card if their cross-sum cannot be further reduced from their character card...
            return personalityCrossSum == teacherCrossSum ? null : cardBuilder.GetMajorArcanaByIndex(teacherCrossSum);
        }

        public IArchetype GetPersonalityCard(DateTime birthDate)
        {
            int crossSum = CalculatePersonalitySum(birthDate);
            return cardBuilder.GetMajorArcanaByIndex(crossSum);
        }

        public IArchetype GetZodiacCard(int absoluteDegree)
        {
            EclipticZodiac zodiac = astrologicalCalculator.GetEclipticZodiacByDegree(absoluteDegree);
            MajorArcana arcana = thothCalculator.GetArcanaByZodiac(zodiac);

            return cardBuilder.GetMajorArcanaByIndex((int) arcana);
        }

        public IArchetype GetDecanCard(int absoluteDegree)
        {
            throw new NotImplementedException();
        }

        public IArchetype GetCourtCard(int absoluteDegree)
        {
            throw new NotImplementedException();
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
