using Thoth.Types.Thoth;
using Thoth.Types.Thoth.Data;

namespace Thoth.Resources.Json
{
    internal interface IThothDeck
    {
        /// <summary> Get a Major arcana by its roman numerical value - its order in the cycle of the universe. This calculation is 'Fool-Safe' (The fool is both 0 and 22 - a new beginning.) </summary>
        IArchetype FetchMajorArcana(int arcanaNumeric);
        
        IArchetype FetchMinorArcana(MinorArcanaAddedToOffset suitOffset, int cardNumber);
        IArchetype FetchMinorArcana(MinorArcanaAddedToOffset arcanaReference);
    }
}