using Thoth.Types.Thoth.ArcanaData;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Thoth.Data;

namespace Thoth.Resources.Json
{
    internal interface IThothDeck
    {
        /// <summary> Get a Major arcana by its roman numerical value - its order in the cycle of the universe. This calculation is 'Fool-Safe' (The fool is both 0 and 22 - a new beginning.) </summary>
        IArchetype GetMajorArcanaByIndex(int arcanaNumeric);

        IArcana GrabMinorArcanaByIndexAndSuit(int arcanaNumeric, MinorArcanaSuit suit);
    }
}