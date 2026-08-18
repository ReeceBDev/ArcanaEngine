using System.Collections.Immutable;

namespace Thoth.Types.Thoth.Data
{
    /// <summary>
    /// Decan cards (2-10 of each suit) with their absolute starting degrees.
    /// Each decan spans 10 degrees.
    /// </summary>
    internal static class ZodiacToDecanMapping
    {
        private static readonly ImmutableDictionary<int, MinorArcanaAddedToOffset> dataSource =
            ImmutableDictionary.CreateRange(new KeyValuePair<int, MinorArcanaAddedToOffset>[]
            {
                // WANDS (Fire: Aries, Leo, Sagittarius)
                new(0, MinorArcanaAddedToOffset.Dominion),
                new(10, MinorArcanaAddedToOffset.Virtue),
                new(20, MinorArcanaAddedToOffset.Completion),
                new(120, MinorArcanaAddedToOffset.Strife),
                new(130, MinorArcanaAddedToOffset.Victory),
                new(140, MinorArcanaAddedToOffset.Valour),
                new(240, MinorArcanaAddedToOffset.Swiftness),
                new(250, MinorArcanaAddedToOffset.Strength),
                new(260, MinorArcanaAddedToOffset.Oppression),

                // CUPS (Water: Cancer, Scorpio, Pisces)
                new(90, MinorArcanaAddedToOffset.Love),
                new(100, MinorArcanaAddedToOffset.Abundance),
                new(110, MinorArcanaAddedToOffset.Luxury),
                new(210, MinorArcanaAddedToOffset.Disappointment),
                new(220, MinorArcanaAddedToOffset.Pleasure),
                new(230, MinorArcanaAddedToOffset.Debauch),
                new(330, MinorArcanaAddedToOffset.Indolence),
                new(340, MinorArcanaAddedToOffset.Happiness),
                new(350, MinorArcanaAddedToOffset.Satiety),

                // SWORDS (Air: Gemini, Libra, Aquarius)
                new(60, MinorArcanaAddedToOffset.Peace),
                new(70, MinorArcanaAddedToOffset.Sorrow),
                new(80, MinorArcanaAddedToOffset.Truce),
                new(180, MinorArcanaAddedToOffset.Defeat),
                new(190, MinorArcanaAddedToOffset.Science),
                new(200, MinorArcanaAddedToOffset.Futility),
                new(300, MinorArcanaAddedToOffset.Interference),
                new(310, MinorArcanaAddedToOffset.Cruelty),
                new(320, MinorArcanaAddedToOffset.Ruin),

                // DISKS (Earth: Taurus, Virgo, Capricorn)
                new(30, MinorArcanaAddedToOffset.Change),
                new(40, MinorArcanaAddedToOffset.Works),
                new(50, MinorArcanaAddedToOffset.Power),
                new(150, MinorArcanaAddedToOffset.Worry),
                new(160, MinorArcanaAddedToOffset.Success),
                new(170, MinorArcanaAddedToOffset.Failure),
                new(270, MinorArcanaAddedToOffset.Prudence),
                new(280, MinorArcanaAddedToOffset.Gain),
                new(290, MinorArcanaAddedToOffset.Wealth),
            });

        public static ImmutableDictionary<int, MinorArcanaAddedToOffset> DegreeToDecan { get; } = dataSource;
        public static ImmutableDictionary<MinorArcanaAddedToOffset, int> DecanToDegree { get; } =
            dataSource.ToImmutableDictionary(i => i.Value, i => i.Key);
    }
}
