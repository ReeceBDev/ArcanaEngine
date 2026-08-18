using System.Collections.Immutable;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth.Data
{
    internal static class ArcanaToZodiacMapping
    {
        private static readonly ImmutableDictionary<ZodiacSign, MajorArcana> dataSource =
            ImmutableDictionary.CreateRange(new KeyValuePair<ZodiacSign, MajorArcana>[]
            {
                new(ZodiacSign.Aries, MajorArcana.TheEmperor),
                new(ZodiacSign.Taurus, MajorArcana.TheHierophant),
                new(ZodiacSign.Gemini, MajorArcana.TheLovers),
                new(ZodiacSign.Cancer, MajorArcana.TheChariot),
                new(ZodiacSign.Leo, MajorArcana.Lust),
                new(ZodiacSign.Virgo, MajorArcana.TheHermit),
                new(ZodiacSign.Libra, MajorArcana.Adjustment),
                new(ZodiacSign.Scorpio, MajorArcana.Death),
                new(ZodiacSign.Sagittarius, MajorArcana.Art),
                new(ZodiacSign.Capricorn, MajorArcana.TheDevil),
                new(ZodiacSign.Aquarius, MajorArcana.TheStar),
                new(ZodiacSign.Pisces, MajorArcana.TheMoon)
            });

        public static ImmutableDictionary<ZodiacSign, MajorArcana> EclipticToArcana { get; } = dataSource;
        public static ImmutableDictionary<MajorArcana, ZodiacSign> ArcanaToEcliptic { get; } = dataSource.ToImmutableDictionary(i => i.Value, i => i.Key);
    }
}
