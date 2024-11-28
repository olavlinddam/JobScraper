using JobScraper.Api;
using JobScraper.Application;
using JobScraper.Infrastructure;
using Serilog;
using Serilog.Settings.Configuration;

try
{
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Services.AddPresentation(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
    }

    var app = builder.Build();
    {
        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}