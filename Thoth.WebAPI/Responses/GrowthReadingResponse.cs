namespace Thoth.WebAPI.Responses
{
    /// <summary> Contains growth cards centred on a target year, with any requested surrounding years. </summary>
    /// <param name="TargetYear"> The requested central year. </param>
    /// <param name="Cards"> Growth cards returned for the requested year range. </param>
    internal record GrowthReadingResponse(
        int TargetYear,
        IEnumerable<GrowthCardResponse> Cards);
}