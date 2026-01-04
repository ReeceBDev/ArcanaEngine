using Thoth.Managers;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    internal class ZodiacalCorrespondence : IZodiacalArcanaCorrespondence
    {
        public ZodiacalCorrespondence(ICardProvider cardFetcher, int absoluteDegree)
        {
            Zodiac = cardFetcher.GetZodiacCard(absoluteDegree);
            Decan = cardFetcher.GetDecanCard(absoluteDegree);
            Court = cardFetcher.GetCourtCard(absoluteDegree);
        }

        public IArchetype Zodiac { get; init; }

        public IArchetype Decan { get; init; }

        public IArchetype Court { get; init; }
    }
}
