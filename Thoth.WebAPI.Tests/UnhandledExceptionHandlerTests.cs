using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Thoth.WebAPI.Tests
{
    /// <summary> Pins the global error envelope (UnhandledExceptionHandler). The real
    /// Program boots with the wrapper armed FIRST in the pipeline; today's reading
    /// routes catch their own faults internally (existing explicit mappings are
    /// pinned unchanged here), so the escape path is exercised through the handler
    /// directly — the same class the middleware invokes, with the same
    /// configuration-driven detail gating (default off in production). </summary>
    public sealed class UnhandledExceptionHandlerTests
    {
        private static (UnhandledExceptionHandler Handler, DefaultHttpContext Context) Harness(
            string? includeDetail = null,
            string environment = "Development")
        {
            var configuration = new ConfigurationBuilder();
            var settings = new Dictionary<string, string?>();
            var services = new ServiceCollection();

            if (includeDetail is not null)
            {
                settings["Diagnostics:IncludeExceptionDetail"] = includeDetail;
            }

            IConfiguration config = configuration.AddInMemoryCollection(settings).Build();
            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IHostEnvironment>(new FakeHostEnvironment(environment));

            DefaultHttpContext context = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider(),
                Response =
                {
                    Body = new MemoryStream(),   // DefaultHttpContext defaults to Stream.Null
                },
            };

            context.Request.Method = "POST";
            context.Request.Path = "/boom";

            return (new UnhandledExceptionHandler(NullLogger<UnhandledExceptionHandler>.Instance), context);
        }

        private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = environmentName;
            public string ApplicationName { get; set; } = "test";
            public string ContentRootPath { get; set; } = "/";
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }

        private static async Task<JsonElement> ReadEnvelopeAsync(HttpResponse response)
        {
            response.Body.Position = 0;
            using JsonDocument document = await JsonDocument.ParseAsync(response.Body);

            return document.RootElement.Clone();
        }

        [Fact]
        public async Task UnhandledException_Answers500JsonEnvelope()
        {
            (UnhandledExceptionHandler handler, HttpContext context) = Harness();   // Development: detail on by default

            bool handled = await handler.TryHandleAsync(context, new ArgumentException("Key is a required property"), CancellationToken.None);

            Assert.True(handled, "the handler must report the exception handled — the middleware must never rethrow");
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", context.Response.ContentType);

            JsonElement envelope = await ReadEnvelopeAsync(context.Response);
            Assert.Equal("internal_error", envelope.GetProperty("error").GetString());
            Assert.False(string.IsNullOrEmpty(envelope.GetProperty("message").GetString()));
            Assert.Equal(context.TraceIdentifier, envelope.GetProperty("traceId").GetString());
            Assert.False(string.IsNullOrEmpty(envelope.GetProperty("traceId").GetString()),
                "traceId must ALWAYS be present — it is the client-side grep key for the server stack trace");
            Assert.False(string.IsNullOrEmpty(envelope.GetProperty("detail").GetString()),
                "Development shows detail by default");
        }

        [Fact]
        public async Task Detail_CarriesExceptionTypeAndMessage_NeverAStackTrace()
        {
            (UnhandledExceptionHandler handler, HttpContext context) = Harness();

            await handler.TryHandleAsync(context, new ArgumentException("Key is a required property"), CancellationToken.None);

            JsonElement envelope = await ReadEnvelopeAsync(context.Response);
            string detail = envelope.GetProperty("detail").GetString() ?? "";

            Assert.Equal("ArgumentException: Key is a required property", detail);   // ExceptionType: message
            Assert.DoesNotContain("   at ", detail);                                 // stack frame marker — never leaves the server
        }

        [Fact]
        public async Task Detail_IsAbsentInProduction_OverrideFlipsItBothWays()
        {
            (UnhandledExceptionHandler dark, HttpContext darkContext) = Harness(environment: "Production");

            await dark.TryHandleAsync(darkContext, new ArgumentException("x"), CancellationToken.None);

            JsonElement darkEnvelope = await ReadEnvelopeAsync(darkContext.Response);
            Assert.False(string.IsNullOrEmpty(darkEnvelope.GetProperty("traceId").GetString()));
            Assert.False(darkEnvelope.TryGetProperty("detail", out JsonElement d) && d.ValueKind != JsonValueKind.Null,
                "production never leaks exception detail unless the override is set");

            (UnhandledExceptionHandler lit, HttpContext litContext) = Harness(environment: "Production", includeDetail: "true");

            await lit.TryHandleAsync(litContext, new ArgumentException("x"), CancellationToken.None);

            JsonElement litEnvelope = await ReadEnvelopeAsync(litContext.Response);
            Assert.False(string.IsNullOrEmpty(litEnvelope.GetProperty("detail").GetString()));

            (UnhandledExceptionHandler forcedDark, HttpContext forcedDarkContext) = Harness(includeDetail: "false");

            await forcedDark.TryHandleAsync(forcedDarkContext, new ArgumentException("x"), CancellationToken.None);

            JsonElement forcedDarkEnvelope = await ReadEnvelopeAsync(forcedDarkContext.Response);
            Assert.False(forcedDarkEnvelope.TryGetProperty("detail", out JsonElement fd) && fd.ValueKind != JsonValueKind.Null,
                "an explicit false hides detail even outside production");
        }

        [Fact]
        public async Task RealPipeline_BootsWithTheWrapperArmed_AndExistingMappingsAreUnchanged()
        {
            using HttpClient client = new WebApplicationFactory<Program>().CreateClient();

            using HttpResponseMessage ok = await client.PostAsync("/reading/birthdate",
                new StringContent("{\"birthDate\":\"1990-06-15\"}", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, ok.StatusCode);

            using HttpResponseMessage bad = await client.PostAsync("/reading/birthdate",
                new StringContent("{\"birthDate\":\"not-a-date\"}", Encoding.UTF8, "application/json"));

            // The route's own explicit mapping (400 invalid_date) is untouched —
            // the wrapper only catches what ESCAPES.
            Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);

            using JsonDocument document = await JsonDocument.ParseAsync(await bad.Content.ReadAsStreamAsync());
            Assert.Equal("invalid_date", document.RootElement.GetProperty("error").GetString());
        }
    }
}
