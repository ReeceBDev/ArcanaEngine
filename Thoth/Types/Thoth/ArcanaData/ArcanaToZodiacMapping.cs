using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth.ArcanaData
{
    internal static class ArcanaToZodiacMapping
    {
        private static readonly ImmutableDictionary<EclipticZodiac, MajorArcana> dataSource =
            ImmutableDictionary.CreateRange(new KeyValuePair<EclipticZodiac, MajorArcana>[]
            {
                new(EclipticZodiac.Aries, MajorArcana.TheEmperor),
                new(EclipticZodiac.Taurus, MajorArcana.TheHierophant),
                new(EclipticZodiac.Gemini, MajorArcana.TheLovers),
                new(EclipticZodiac.Cancer, MajorArcana.TheChariot),
                new(EclipticZodiac.Leo, MajorArcana.Lust),
                new(EclipticZodiac.Virgo, MajorArcana.TheHermit),
                new(EclipticZodiac.Libra, MajorArcana.Adjustment),
                new(EclipticZodiac.Scorpio, MajorArcana.Death),
                new(EclipticZodiac.Sagittarius, MajorArcana.Art),
                new(EclipticZodiac.Capricorn, MajorArcana.TheDevil),
                new(EclipticZodiac.Aquarius, MajorArcana.TheStar),
                new(EclipticZodiac.Pisces, MajorArcana.TheMoon)
            });

        public static ImmutableDictionary<EclipticZodiac, MajorArcana> EclipticToArcana { get; } = dataSource;
        public static ImmutableDictionary<MajorArcana, EclipticZodiac> ArcanaToEcliptic { get; } = dataSource.ToImmutableDictionary(i => i.Value, i => i.Key);
    }
}
