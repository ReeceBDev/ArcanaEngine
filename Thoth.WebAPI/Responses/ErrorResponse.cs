namespace Thoth.WebAPI.Responses
{
    /// <summary> Returned when a request could not be fulfilled due to a client error —
    /// and, via UnhandledExceptionHandler, when a server fault escapes the pipeline
    /// (then Error = "internal_error", HTTP 500). `detail` and `traceId` extend the
    /// envelope ADDITIVELY (existing clients read error/message and must not break):
    /// `detail` is "ExceptionType: message" — never a stack trace — gated by
    /// configuration (default off in production); `traceId` is always present and
    /// matches the server log line for the request, so a client-side traceId finds
    /// the stack. </summary>
    /// <param name="Error"> A short machine-readable error code, e.g. <c>invalid_date</c> or <c>invalid_name</c>. </param>
    /// <param name="Message"> A human-readable description of what went wrong. </param>
    /// <param name="Detail"> Optional "ExceptionType: message" for unhandled server faults; present only when diagnostics allow. </param>
    /// <param name="TraceId"> Optional request correlation id (HttpContext.TraceIdentifier) matching the server log line. </param>
    internal record ErrorResponse(string Error, string Message, string? Detail = null, string? TraceId = null);
}
