using Thoth.Managers;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    internal class ZodiacalCorrespondence : IZodiacalArcanaCorrespondence
    {
        public ZodiacalCorrespondence(ICardMeaningService cardFetcher, AstrologicalSign sign, int degree)
        {
            Zodiac = cardFetcher.GetZodiacCard(sign);
            Decan = cardFetcher.GetDecanCard(sign, degree);
            Court = cardFetcher.GetCourtCard(sign, degree);
        }

        public ZodiacalCorrespondence(ICardMeaningService cardFetcher, ICelestialDegree celestialDegree) 
            : this(cardFetcher, celestialDegree.Sign, celestialDegree.Degree)
        {
        }

        public IArchetype Zodiac { get; init; }

        public IArchetype Decan { get; init; }

        public IArchetype Court { get; init; }
    }
}
