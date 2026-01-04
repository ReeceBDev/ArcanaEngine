using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Zodiacal;

namespace Thoth.Managers
{
    internal interface ICardMeaningService
    {
        IArchetype GetGrowthCardByYear(int year);
        IArchetype GetCardByName(string name);
        IArchetype GetTeacherCard(DateTime birthDate);
        IArchetype GetPersonalityCard(DateTime birthDate);
        IArchetype GetZodiacCard(AstrologicalSign sign);
        IArchetype GetDecanCard(AstrologicalSign sign, int degree);
        IArchetype GetCourtCard(AstrologicalSign sign, int degree);
    }
}
