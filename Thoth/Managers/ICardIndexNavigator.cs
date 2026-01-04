using Thoth.Types.Thoth.ArcanaData;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Thoth.Data;

namespace Thoth.Managers
{
    internal interface ICardIndexNavigator
    {
        IArchetype GetCardByArcana(MajorArcana arcana);
        IArchetype GetCardByArcana(MinorArcana arcana);
        IArcana GrabMajorArcanaByIndex(int index);
        IArcana GrabMinorArcanaByIndexAndSuit(int index, ArcanaSuit);
        IArchetype GrabArchetypesByArcana(IArcana arcana);
    }
}
