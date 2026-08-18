namespace Thoth.WebAPI.Responses
{
    /// <summary> Returned when a request could not be fulfilled due to a client error. </summary>
    /// <param name="Error"> A short machine-readable error code, e.g. <c>invalid_date</c> or <c>invalid_name</c>. </param>
    /// <param name="Message"> A human-readable description of what went wrong. </param>
    internal record ErrorResponse(string Error, string Message);
}
