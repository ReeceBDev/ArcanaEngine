using System.Text.Json.Serialization;

namespace Thoth.WebAPI.Responses
{
    /// <summary> A sky event within the requested window: a sign ingress or a retrograde/direct station. </summary>
    /// <param name="Id"> Stable across refreshes; used as a client-side key. </param>
    /// <param name="Kind"> Either <c>"ingress"</c> or <c>"station"</c>. </param>
    /// <param name="Direction"> Stations only: <c>"retrograde"</c> or <c>"direct"</c>. Omitted for ingresses. </param>
    /// <param name="Time"> The plotted moment of the event. </param>
    /// <param name="StartedAt"> The event itself. </param>
    internal sealed record AstroEventResponse(
        string Id,
        string Kind,
        string Planet,
        string Sign,
        DateTimeOffset Time,
        DateTimeOffset? PeaksAt,
        DateTimeOffset? StopsAt,
        DateTimeOffset StartedAt,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Direction);
}
