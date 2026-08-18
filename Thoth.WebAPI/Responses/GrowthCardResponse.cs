namespace Thoth.WebAPI.Responses
{
    /// <summary> Contains a single growth card for a given year. </summary>
    /// <param name="Year"> The year this growth card belongs to. </param>
    /// <param name="Card"> The growth card for the given year. </param>
    internal record GrowthCardResponse(int Year, ArcanaCardResponse Card);
}