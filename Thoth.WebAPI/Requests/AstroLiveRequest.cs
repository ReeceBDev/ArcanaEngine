namespace Thoth.WebAPI.Requests
{
    /// <summary> Carries the timeline window bounds for the live astrology snapshot. </summary>
    internal sealed class AstroLiveRequest
    {
        /// <summary> Window start, ISO-8601 with offset, e.g. <c>2026-08-18T09:23:11.000Z</c>. </summary>
        public string From { get; init; } = "";

        /// <summary> Window end, ISO-8601 with offset, e.g. <c>2026-08-18T09:23:11.000Z</c>. </summary>
        public string To { get; init; } = "";
    }
}
