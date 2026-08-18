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

            var app = builder.Build();
            app.UseCors();

            app.MapReadingEndpoints();
            app.MapAstroEndpoints();

            app.Run();
        }
    }
}
