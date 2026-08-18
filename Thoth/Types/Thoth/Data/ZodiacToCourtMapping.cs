using System.Collections.Immutable;

namespace Thoth.Types.Thoth.Data
{
    /// <summary>
    /// Court cards with their absolute starting degrees (21° of starting sign).
    /// Each court card spans 30 degrees across sign boundaries.
    /// </summary>
    internal static class ZodiacToCourtMapping
    {
        private static readonly ImmutableDictionary<int, MinorArcanaAddedToOffset> dataSource =
            ImmutableDictionary.CreateRange(new KeyValuePair<int, MinorArcanaAddedToOffset>[]
            {
            // WANDS
            new(231, MinorArcanaAddedToOffset.KnightOfWands),
            new(351, MinorArcanaAddedToOffset.QueenOfWands),
            new(111, MinorArcanaAddedToOffset.PrinceOfWands),

            // CUPS
            new(321, MinorArcanaAddedToOffset.KnightOfCups),
            new(81, MinorArcanaAddedToOffset.QueenOfCups),
            new(201, MinorArcanaAddedToOffset.PrinceOfCups),

            // SWORDS
            new(51, MinorArcanaAddedToOffset.KnightOfSwords),
            new(171, MinorArcanaAddedToOffset.QueenOfSwords),
            new(291, MinorArcanaAddedToOffset.PrinceOfSwords),

            // DISKS
            new(141, MinorArcanaAddedToOffset.KnightOfDisks),
            new(261, MinorArcanaAddedToOffset.QueenOfDisks),
            new(21, MinorArcanaAddedToOffset.PrinceOfDisks),
            });

        public static ImmutableDictionary<int, MinorArcanaAddedToOffset> DegreeToCourt { get; } = dataSource;
        public static ImmutableDictionary<MinorArcanaAddedToOffset, int> CourtToDegree { get; } =
            dataSource.ToImmutableDictionary(i => i.Value, i => i.Key);
    }

}
