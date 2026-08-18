namespace Thoth.WebAPI.Responses
{
    /// <summary> A live snapshot of the sky at the moment of the request. </summary>
    /// <param name="FetchedAt"> The server clock at computation time. </param>
    /// <param name="Placements"> One entry per classical body — always all ten. </param>
    /// <param name="Events"> Sign ingresses and stations within the requested window. </param>
    /// <param name="Aspects"> Body pairs currently within orb of a classical aspect. </param>
    internal sealed record AstroLiveResponse(
        DateTimeOffset FetchedAt,
        IEnumerable<PlanetPlacementResponse> Placements,
        IEnumerable<AstroEventResponse> Events,
        IEnumerable<AspectInfoResponse> Aspects);
}
