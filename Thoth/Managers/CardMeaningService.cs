using Thoth.Resources.Calculators;
using Thoth.Types.Thoth.ArcanaData;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Zodiacal;

namespace Thoth.Managers
{
    internal class CardMeaningService : ICardMeaningService
    {
        private IThothCalculator thothCalculator;
        private ICardIndexNavigator cardGenerator;

        public CardMeaningService(IThothCalculator setThothCalculator, ICardIndexNavigator setCardGenerator)
        {
            thothCalculator = setThothCalculator;
            cardGenerator = setCardGenerator;
        }

        public IArchetype GetGrowthCardByYear(int targetYear)
        {
            var growthArcanaIndex = ArcanaCalculator.CalculateGrowthCard(targetYear);
            var growthArcana = ArcanaPopulator.GrabMajorArcanaByIndex(growthArcanaIndex);
            var growthCard = ArchetypePopulator.GrabArchetypesByArcana(growthArcana);

            return growthCard;
        }

        public IArchetype GetCardByName(string name)
        {
            throw new NotImplementedException();
        }
        
        public IArchetype GetTeacherCard(DateTime birthDate)
        {
            throw new NotImplementedException();
        }

        public IArchetype GetPersonalityCard(DateTime birthDate)
        {
            int birthDay = birthDate.Day;
            int birthMonth = birthDate.Month;
            int birthYear = birthDate.Year;
            int birthSum = birthDay + birthMonth + birthYear;
            int crossSum = thothCalculator.CalculateCrossSum(birthSum);
            MajorArcana arcana = (MajorArcana) crossSum;

            return cardGenerator.GetCardByArcana(arcana);
        }

        public IArchetype GetZodiacCard(AstrologicalSign sign)
        {
            throw new NotImplementedException();
        }

        public IArchetype GetDecanCard(AstrologicalSign sign, int degree)
        {
            throw new NotImplementedException();
        }

        public IArchetype GetCourtCard(AstrologicalSign sign, int degree)
        {
            throw new NotImplementedException();
        }
    }
}
