using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Thoth.WebAPI.Responses;

namespace Thoth.WebAPI
{
    /// <summary> THE global exception wrapper — the one place an unhandled exception
    /// is turned into a client-visible answer. Registered with
    /// AddExceptionHandler&lt;UnhandledExceptionHandler&gt;() and armed by
    /// app.UseExceptionHandler() FIRST in the pipeline (before UseCors, so the error
    /// response still passes UseCors on the way out and carries
    /// Access-Control-Allow-Origin for allowlisted origins — this API allows any
    /// origin, so a CORS-less 500 is unreadable by exactly every browser client).
    /// Every response uses the house JSON error envelope (ErrorResponse), extending
    /// it additively: clients that read error/message keep working. Per-route
    /// try/catch and explicit error returns stay as they are — this wrapper only
    /// catches what escapes (today's routes catch internally; the wrapper is the
    /// net for the routes that never will). It never rethrows. </summary>
    internal sealed class UnhandledExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<UnhandledExceptionHandler> _logger;

        /// <summary> Explicit web (camelCase) serializer options — the envelope MUST be
        /// camelCase JSON regardless of any DI-registered JsonOptions (e.g. the
        /// ProblemDetails source-gen context AddProblemDetails seeds, which carries
        /// metadata only for its own types). </summary>
        private static readonly JsonSerializerOptions EnvelopeJson = new(JsonSerializerDefaults.Web);

        public UnhandledExceptionHandler(ILogger<UnhandledExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <summary> Writes the 500 envelope and reports the exception handled. The
        /// log line carries the SAME traceId the body does, so a client-side traceId
        /// is one grep away from the stack trace. </summary>
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // HttpContext.TraceIdentifier — always present, per-request.
            string traceId = context.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception on {Method} {Path} traceId={TraceId}",
                context.Request.Method,
                context.Request.Path,
                traceId);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse(
                Error: "internal_error",
                Message: "An unexpected error occurred.",
                Detail: IncludeExceptionDetail(context)
                    ? $"{exception.GetType().Name}: {exception.Message}"   // type + message only — NEVER a stack trace
                    : null,
                TraceId: traceId),
                EnvelopeJson),
                cancellationToken);

            return true;   // handled — the middleware does not rethrow
        }

        /// <summary> Whether the envelope carries `detail`. Reads
        /// Diagnostics__IncludeExceptionDetail (double underscore in the environment =
        /// the Diagnostics:… config section). An explicit parsable value wins (true OR
        /// false); otherwise detail is ON outside production and OFF in production —
        /// `detail` is type + exception message, which is information disclosure on a
        /// public endpoint, so prod stays dark unless rbowen flips the override while
        /// the app is in build/test (the value lives in the canonical env file, never
        /// in git). </summary>
        private static bool IncludeExceptionDetail(HttpContext context)
        {
            IConfiguration configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            IHostEnvironment environment = context.RequestServices.GetRequiredService<IHostEnvironment>();

            return bool.TryParse(configuration["Diagnostics:IncludeExceptionDetail"], out bool explicitValue)
                ? explicitValue
                : !environment.IsProduction();
        }
    }
}
