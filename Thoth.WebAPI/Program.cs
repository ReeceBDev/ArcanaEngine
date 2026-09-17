namespace Thoth.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Starting Thoth web API host...");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()));

            // THE global exception wrapper: anything the pipeline lets escape is
            // answered by UnhandledExceptionHandler with the house JSON error
            // envelope (500 internal_error + traceId) instead of a bare, CORS-less
            // 500. AddProblemDetails is the middleware's construction fallback (the
            // handler itself always writes the envelope, so it never runs).
            builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            // Pipeline order is load-bearing: UseExceptionHandler goes FIRST —
            // before UseCors — so the error response still exits through CORS and
            // carries Access-Control-Allow-Origin for browser clients.
            app.UseExceptionHandler();

            app.UseCors();

            app.MapReadingEndpoints();
            app.MapAstroEndpoints();

            app.Run();
        }
    }
}
