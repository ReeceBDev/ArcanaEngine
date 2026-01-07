using Thoth.Managers;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    internal class ZodiacalCorrespondence : IZodiacalArcanaCorrespondence
    {
        public ZodiacalCorrespondence(ICardProvider cardProvider, IEclipticDegree zodiacalDegree)
        {
            Zodiac = cardProvider.GetZodiacCard(zodiacalDegree.Sign);
            Decan = cardProvider.GetDecanCard(zodiacalDegree.AbsoluteDegree);
            Court = cardProvider.GetCourtCard(zodiacalDegree.AbsoluteDegree);
        }

        public IArchetype Zodiac { get; init; }

        public IArchetype Decan { get; init; }

        public IArchetype Court { get; init; }
    }
}
