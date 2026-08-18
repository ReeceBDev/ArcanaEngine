using Thoth.External.Types;

namespace Thoth.Resources.Calculators
{
    /// <summary> Computes live-sky snapshots via Swiss Ephemeris. </summary>
    internal interface ILiveAstroCalculator
    {
        /// <summary> Returns the placements at <paramref name="now"/>, the ingresses and stations within
        /// [<paramref name="from"/>, <paramref name="to"/>], and the aspects currently within orb. </summary>
        LiveSkySnapshot GetSnapshot(DateTimeOffset now, DateTimeOffset from, DateTimeOffset to);
    }
}
