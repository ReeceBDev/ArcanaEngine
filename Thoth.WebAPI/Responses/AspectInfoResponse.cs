namespace Thoth.WebAPI.Responses
{
    /// <summary> A pair of bodies currently within orb of a classical aspect. </summary>
    /// <param name="Aspect"> One of conjunction, sextile, square, trine, opposition. </param>
    /// <param name="OrbDeg"> Distance from the exact angle. </param>
    /// <param name="Applying"> True when the orb is shrinking. </param>
    /// <param name="PeaksAt"> The exact perfection time, past or future; null when the aspect never perfects. </param>
    /// <param name="StopsAt"> When the pair leaves orb. </param>
    /// <param name="StartedAt"> When the pair entered orb. </param>
    internal sealed record AspectInfoResponse(
        string PlanetA,
        string SignA,
        string PlanetB,
        string SignB,
        string Aspect,
        double OrbDeg,
        bool Applying,
        DateTimeOffset? PeaksAt,
        DateTimeOffset? StopsAt,
        DateTimeOffset? StartedAt);
}
