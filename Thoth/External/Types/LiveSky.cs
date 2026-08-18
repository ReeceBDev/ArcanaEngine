using Thoth.Resources.Calculators;

namespace Thoth.External.Types
{
    /// <summary> Entry point for live-sky computations driven by Swiss Ephemeris. </summary>
    public static class LiveSky
    {
        /// <summary> Computes a snapshot of the sky at <paramref name="now"/>:
        /// current placements of the ten classical bodies, the ingresses and stations within
        /// [<paramref name="from"/>, <paramref name="to"/>], and the aspects currently within orb. </summary>
        public static LiveSkySnapshot GetSnapshot(DateTimeOffset now, DateTimeOffset from, DateTimeOffset to)
        {
            using LiveAstroCalculator calculator = new();
            return calculator.GetSnapshot(now, from, to);
        }
    }
}
