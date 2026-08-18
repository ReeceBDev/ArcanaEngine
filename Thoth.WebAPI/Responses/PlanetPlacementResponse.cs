namespace Thoth.WebAPI.Responses
{
    /// <summary> The current tropical placement of a single celestial body. </summary>
    /// <param name="Planet"> One of the ten classical bodies. </param>
    /// <param name="Sign"> The tropical sign the body currently occupies. </param>
    /// <param name="DegreeInSign"> The body's position within its sign, 0–30. </param>
    /// <param name="Speed"> Apparent speed in ecliptic longitude, degrees per day; negative when retrograde. </param>
    /// <param name="Retrograde"> Convenience flag: true when <paramref name="Speed"/> is negative. </param>
    /// <param name="PeaksAt"> When the body crosses 15° of its sign; null when already past it. </param>
    /// <param name="StopsAt"> When the body leaves its sign. </param>
    /// <param name="StartedAt"> When the body entered its sign. </param>
    internal sealed record PlanetPlacementResponse(
        string Planet,
        string Sign,
        double DegreeInSign,
        double Speed,
        bool Retrograde,
        DateTimeOffset? PeaksAt,
        DateTimeOffset? StopsAt,
        DateTimeOffset? StartedAt);
}
