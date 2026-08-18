namespace Thoth.External.Types
{
    /// <summary> A snapshot of the live sky: current placements of the ten classical bodies,
    /// the ingresses and stations within a window, and the aspect pairs currently within orb. </summary>
    public sealed record LiveSkySnapshot(
        DateTimeOffset FetchedAt,
        IReadOnlyList<LivePlacement> Placements,
        IReadOnlyList<LiveAstroEvent> Events,
        IReadOnlyList<LiveAspect> Aspects);

    /// <summary> The current tropical placement of a single celestial body.
    /// The started/peaks/stops triple marks sign entry, mid-sign (15°) crossing and sign exit. </summary>
    public sealed record LivePlacement(
        string Planet,
        string Sign,
        double DegreeInSign,
        double Speed,
        bool Retrograde,
        DateTimeOffset? PeaksAt,
        DateTimeOffset? StopsAt,
        DateTimeOffset? StartedAt);

    /// <summary> A sky event: either a sign ingress or a retrograde/direct station. </summary>
    public sealed record LiveAstroEvent(
        string Id,
        string Kind,
        string Planet,
        string Sign,
        string? Direction,
        DateTimeOffset Time,
        DateTimeOffset? PeaksAt,
        DateTimeOffset? StopsAt,
        DateTimeOffset StartedAt);

    /// <summary> A pair of bodies currently within orb of a classical aspect. </summary>
    public sealed record LiveAspect(
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
